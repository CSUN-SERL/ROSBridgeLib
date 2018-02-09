using System;
using System.Collections.Generic;
using SimpleJSON;

namespace ROSBridgeLib.Core
{
    public abstract class ROSBridgeSubscriber
    {
        protected readonly string topic;
        protected readonly Type messageType;
        protected int queueSize;
        protected Queue<JSONNode> rawMessages; 
            
        public event ROSCallback<IMsg> MessageRecieved;
        
        public string Topic
        {
            get { return topic; }
        }

        public Type MessageType
        {
            get { return messageType; }
        }
        
        public ROSBridgeSubscriber(string topic, int queueSize, Type messageType)
        {
            this.topic = topic;
            this.queueSize = queueSize;
            this.messageType = messageType;
            rawMessages = new Queue<JSONNode>(queueSize);
        }
        
        public void Unsubscribe(ROSCallback<IMsg> callback)
        {
            MessageRecieved -= callback;
        }

        public void Subscribe(ROSCallback<IMsg> callback)
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

            IMsg msg = (IMsg)Activator.CreateInstance(messageType);
            msg.Deserialize(rawMessages.Dequeue());
            
            ProcessMsg(msg);
        }

        protected virtual void ProcessMsg(IMsg msg)
        {
            MessageRecieved?.Invoke(msg);
        }
    }
    
    public class ROSBridgeSubscriber<T> : ROSBridgeSubscriber where T : IMsg
    {
        public new event ROSCallback<T> MessageRecieved;
      
        public ROSBridgeSubscriber(string topic, int queueSize) :
            base(topic, queueSize, typeof(T))
        { }
        
        public void Unsubscribe(ROSCallback<T> callback)
        {
            MessageRecieved -= callback;   
        }

        public void Subscribe(ROSCallback<T> callback)
        {
            MessageRecieved += callback;   
        }
        
        protected override void ProcessMsg(IMsg msg)
        {
            base.ProcessMsg(msg);
            MessageRecieved?.Invoke((T)msg);
        }

    }
}