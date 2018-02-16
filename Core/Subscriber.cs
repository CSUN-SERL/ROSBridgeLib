using System;
using System.Collections.Generic;
using SimpleJSON;

namespace ROSBridgeLib.Core
{
    public abstract class Subscriber
    {
        protected readonly string topic;
        protected readonly Type messageType;
        protected int queueSize;
        protected Queue<JSONNode> rawMessages; 
            
        public event ROSCallback<IMessage> MessageRecieved;
        
        public string Topic
        {
            get { return topic; }
        }

        public Type MessageType
        {
            get { return messageType; }
        }
        
        public Subscriber(string topic, int queueSize, Type messageType)
        {
            this.topic = topic;
            this.queueSize = queueSize;
            this.messageType = messageType;
            rawMessages = new Queue<JSONNode>(queueSize);
        }
        
        public void Unsubscribe(ROSCallback<IMessage> callback)
        {
            MessageRecieved -= callback;
        }

        public void Subscribe(ROSCallback<IMessage> callback)
        {
            MessageRecieved += callback;
        }

        public void EnqueueRawMsg(JSONNode node)
        {
            if (rawMessages.Count == queueSize)
            {
                rawMessages.Dequeue();
            }
            
            rawMessages.Enqueue(node);
        }

        public void ProcessOldestMsg()
        {
            if (rawMessages.Count == 0)
                return;

            IMessage msg = (IMessage)Activator.CreateInstance(messageType);
            msg.Deserialize(rawMessages.Dequeue());
            
            ProcessMsg(msg);
        }

        protected virtual void ProcessMsg(IMessage msg)
        {
            MessageRecieved?.Invoke(msg);
        }
    }
}