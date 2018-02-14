using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using SimpleJSON;
using WebSocketSharp;

namespace ROSBridgeLib.Core
{
    public class TopicManager
    {
        private WebSocket socket;
        private Thread thread; // todo use Task instead
        
        private Dictionary<string, Type> allTopics;
        private Dictionary<string, ROSBridgeSubscriber> subscribers;
        private Dictionary<string, ROSBridgePublisher> publishers;
        
        private object _queueLock = new object();
        

        public TopicManager(WebSocket socket) 
        {
            this.socket = socket;
            
            allTopics = new Dictionary<string, Type>();
            subscribers = new Dictionary<string, ROSBridgeSubscriber>();
            publishers = new Dictionary<string, ROSBridgePublisher>();

            this.socket.OnMessage += OnMessage;
        }

        public ROSBridgePublisher GetPublisher(string topic)
        {
            ROSBridgePublisher publisher;
            return publishers.TryGetValue(topic, out publisher) ? publisher : null;
        }
	 
        public ROSBridgeSubscriber<T> GetSubscriber<T>(string topic) where T: IMsg
        {
            ThrowIfTopicExistsUnderDifferentType(topic, typeof(T), "obtain");
            return GetSubscriber(topic) as ROSBridgeSubscriber<T>;
        }
        
        public ROSBridgeSubscriber GetSubscriber(string topic)
        {
            ROSBridgeSubscriber subscriber;
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
            thread = new Thread(Run);
            thread.Start();
        }
        
        /// <summary>
        /// Disconnect from the remote ros environment.
        /// </summary>
        public void Disconnect()
        {
            // todo use Task instead
            thread.Abort();

            foreach (var subscriber in subscribers)
            {
                SendUnSubscribeOperation(subscriber.Value);
            }

            foreach (var publisher in publishers)
            {
                SendUnAdvertiseOperation(publisher.Value);
            }
        }
        
        public ROSBridgePublisher Advertise<T>(string topic) where T : IMsg
        {
            Type messageType = typeof(T);
            ThrowIfTopicExistsUnderDifferentType(topic, messageType, "advertise on");

            ROSBridgePublisher publisher;
            if (!publishers.TryGetValue(topic, out publisher))
            {
                publisher = new ROSBridgePublisher(socket, topic, messageType);
                publishers.Add(topic, publisher);
                
                CacheTopic(topic, messageType);
                SendAdvertiseOperation(publisher);
            }
		 
            return publisher;
        }
	 
        public ROSBridgeSubscriber<T> Subscribe<T>(string topic, ROSCallback<T> callback, int queueSize) where T : IMsg, new()
        {
            Type messageType = typeof(T);
            ThrowIfTopicExistsUnderDifferentType(topic, messageType, "subscribe to");
		 
            ROSBridgeSubscriber subscriber;
            if (!subscribers.TryGetValue(topic, out subscriber))
            {
                subscriber = new ROSBridgeSubscriber<T>(topic, queueSize);
                subscribers.Add(topic, subscriber);
                
                CacheTopic(topic, messageType);
                SendSubscribeOperation(subscriber);
            }

            var sub = (ROSBridgeSubscriber<T>) subscriber;

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
                Thread.Sleep(10);
                Render();
            }
        }
        
        private void Render()
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
            string s = args.Data;
		 
            if (string.IsNullOrEmpty(s))
                return;	 

            JSONNode node = JSONNode.Parse(s);
            string op = node["op"];
            
            if (op == "publish")
            {
                lock(_queueLock)
                {
                    GetSubscriber(node["topic"])?.EnqueueRawMsg(node["msg"]);
                }
            }
        }
        
        private void SendSubscribeOperation(ROSBridgeSubscriber subscriber)
        {
            string s = ROSBridgeMsg.Subscribe(subscriber.Topic, ((IMsg)Activator.CreateInstance(subscriber.MessageType)).ROSMessageType);
            Debug.Print($"Sending: {s}");
            socket.Send(s);
        }

        private void SendUnSubscribeOperation(ROSBridgeSubscriber subscriber)
        {
            string s = ROSBridgeMsg.UnSubscribe(subscriber.Topic);
            Debug.Print($"Sending: {s}");
            socket.Send(s);
        }
	 
        private void SendAdvertiseOperation(ROSBridgePublisher subscriber)
        {
            string s = ROSBridgeMsg.Advertise(subscriber.Topic, ((IMsg)Activator.CreateInstance(subscriber.MessageType)).ROSMessageType);
            Debug.Print($"Sending: {s}");
            socket.Send(s);
        }
	 
        private void SendUnAdvertiseOperation(ROSBridgePublisher publisher)
        {
            string s = ROSBridgeMsg.UnAdvertise(publisher.Topic);
            Debug.Print($"Sending: {s}");
            socket.Send(s);
        }
    }
}