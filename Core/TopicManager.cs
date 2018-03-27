using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ROSBridgeLib.Core.Generic;
using SimpleJSON;
using WebSocketSharp;

namespace ROSBridgeLib.Core
{
    public class TopicManager
    {
        private WebSocket socket;
        private Thread thread; // todo use Task instead

        private Dictionary<string, Type> allTopics;
        private Dictionary<string, Subscriber> subscribers;
        private Dictionary<string, Publisher> publishers;

        private object _queueLock = new object();


        public TopicManager(WebSocket socket)
        {
            this.socket = socket;

            allTopics = new Dictionary<string, Type>();
            subscribers = new Dictionary<string, Subscriber>();
            publishers = new Dictionary<string, Publisher>();

            this.socket.OnMessage += OnMessage;
        }

        public Publisher GetPublisher(string topic)
        {
            Publisher publisher;
            return publishers.TryGetValue(topic, out publisher) ? publisher : null;
        }

        public Subscriber<T> GetSubscriber<T>(string topic) where T : IMessage
        {
            ThrowIfTopicExistsUnderDifferentType(topic, typeof(T), "obtain");
            return GetSubscriber(topic) as Subscriber<T>;
        }

        public Subscriber GetSubscriber(string topic)
        {
            Subscriber subscriber;
            return subscribers.TryGetValue(topic, out subscriber) ? subscriber : null;
        }

        public void Connect()
        {
            foreach (var sub in subscribers)
            {
                SendSubscribeOperation(sub.Value);
            }

            foreach (var pub in publishers)
            {
                SendAdvertiseOperation(pub.Value);
            }

            // todo use Task instead
            //thread = new Thread(Run);
            //thread.Start();
        }

        /// <summary>
        /// Disconnect from the remote ros environment.
        /// </summary>
        public void Disconnect()
        {
            // todo use Task instead
            //thread.Abort();

            foreach (var subscriber in subscribers)
            {
                SendUnSubscribeOperation(subscriber.Value);
            }

            foreach (var publisher in publishers)
            {
                SendUnAdvertiseOperation(publisher.Value);
            }
        }

        public Publisher Advertise<T>(string topic) where T : IMessage
        {
            Type messageType = typeof(T);
            ThrowIfTopicExistsUnderDifferentType(topic, messageType, "advertise on");

            Publisher publisher;
            if (!publishers.TryGetValue(topic, out publisher))
            {
                publisher = new Publisher(socket, topic, messageType);
                publishers.Add(topic, publisher);

                CacheTopic(topic, messageType);
                SendAdvertiseOperation(publisher);
            }

            return publisher;
        }

        public Publisher Publish(string topic, IMessage msg)
        {
            return GetPublisher(topic)?.Publish(msg);
        }

        public Subscriber<T> Subscribe<T>(string topic, ROSCallback<T> callback, int queueSize = 10) where T : IMessage, new()
        {
            Type messageType = typeof(T);
            ThrowIfTopicExistsUnderDifferentType(topic, messageType, "subscribe to");

            Subscriber subscriber;
            if (!subscribers.TryGetValue(topic, out subscriber))
            {
                subscriber = new Subscriber<T>(topic, queueSize);
                subscribers.Add(topic, subscriber);

                CacheTopic(topic, messageType);
                SendSubscribeOperation(subscriber);
            }

            var sub = (Subscriber<T>)subscriber;

            sub.Subscribe(callback);

            return sub;
        }

        private void CacheTopic(string topic, Type messageType)
        {
            if (!allTopics.ContainsKey(topic))
            {
                allTopics.Add(topic, messageType);
            }
        }

        private void ThrowIfTopicExistsUnderDifferentType(string topic, Type type, string operation)
        {
            Type messageType;
            if (allTopics.TryGetValue(topic, out messageType))
            {
                if (type != messageType)
                {
                    throw new Exception(
                        $"Topic: {topic} already exists for message type: {messageType}. You are attempting to {operation} the same topic with message type {type}");
                }
            }
        }

        private void Run()
        {
            while (true)
            {
                // todo use Task instead
                //Thread.Sleep(10);
                Render();
            }
        }

        public void Render()
        {
            lock (_queueLock)
            {
                foreach (var sub in subscribers)
                {
                    sub.Value.ProcessOldestMsg();
                }
            }
        }

        private void OnMessage(object sender, MessageEventArgs args)
        {
            string data = args.Data;

            if (string.IsNullOrEmpty(data))
                return;

            JSONNode node = JSONNode.Parse(data);
            string operation = node["op"];

            if (operation == "publish")
            {
                lock (_queueLock)
                {
                    GetSubscriber(node["topic"])?.EnqueueRawMsg(node["msg"]);
                }
            }
        }

        private void SendSubscribeOperation(Subscriber subscriber)
        {
            string s = ROSBridgeMessage.Subscribe(subscriber.Topic, ((IMessage)Activator.CreateInstance(subscriber.MessageType)).ROSMessageType);
            Debug.Print($"Sending: {s}");
            socket.Send(s);
        }

        private void SendUnSubscribeOperation(Subscriber subscriber)
        {
            string s = ROSBridgeMessage.UnSubscribe(subscriber.Topic);
            Debug.Print($"Sending: {s}");
            socket.Send(s);
        }

        private void SendAdvertiseOperation(Publisher subscriber)
        {
            string s = ROSBridgeMessage.Advertise(subscriber.Topic, ((IMessage)Activator.CreateInstance(subscriber.MessageType)).ROSMessageType);
            Debug.Print($"Sending: {s}");
            socket.Send(s);
        }

        private void SendUnAdvertiseOperation(Publisher publisher)
        {
            string s = ROSBridgeMessage.UnAdvertise(publisher.Topic);
            Debug.Print($"Sending: {s}");
            socket.Send(s);
        }
    }
}