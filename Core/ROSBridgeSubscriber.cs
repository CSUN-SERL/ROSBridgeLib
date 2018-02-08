using System;
using System.Collections.Generic;
using SimpleJSON;

namespace ROSBridgeLib.Core
{
    public class ROSBridgeSubscriber
    {
        protected string topic;
        protected Type messageType;
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
        
        public bool Blocked { get; set; }
        
        public ROSBridgeSubscriber(string topic, int queueSize, Type messageType)
        {
            this.topic = topic;
            this.messageType = messageType;
            this.queueSize = queueSize;
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
        
        public void ProcessMsg()
        {
            if (Blocked || rawMessages.Count == 0)
                return;

            JSONNode node = rawMessages.Dequeue();
            
            IMsg msg = (IMsg)Activator.CreateInstance(messageType);
            msg.Deserialize(node);
            MessageRecieved?.Invoke(msg);
        }

        public override bool Equals(object other)
        {
            var otherSub = other as ROSBridgeSubscriber;
            if (otherSub != null)
            {
                return Equals(otherSub);
            }

            return false;
        }

        public static bool operator==(ROSBridgeSubscriber lhs, ROSBridgeSubscriber rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator!=(ROSBridgeSubscriber lhs, ROSBridgeSubscriber rhs)
        {
            return !lhs.Equals(rhs);
        }
        
        protected bool Equals(ROSBridgeSubscriber<IMsg> other)
        {
            if (ReferenceEquals(this, other))
                return true;
            
            if (other != null)
            {
                return topic == other.topic;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (topic != null ? topic.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Blocked.GetHashCode();
                return hashCode;
            }
        }
    }
    
    public class ROSBridgeSubscriber<T> : ROSBridgeSubscriber where T : IMsg
    {
        public new event ROSCallback<T> MessageRecieved;


        public ROSBridgeSubscriber(string topic, int queueSize) :
            base(topic, queueSize, typeof(T))
        {
        }
        
        public void Unsubscribe(ROSCallback<T> callback)
        {
            MessageRecieved -= callback;
        }

        public void Subscribe(ROSCallback<T> callback)
        {
            MessageRecieved += callback;
        }

        public override bool Equals(object other)
        {
            var otherSub = other as ROSBridgeSubscriber<T>;
            if (otherSub != null)
            {
                return Equals(otherSub);
            }

            return false;
        }

        protected bool Equals(ROSBridgeSubscriber<T> other)
        {
            if (ReferenceEquals(this, other))
                return true;
            
            if (other != null)
            {
                return topic == other.topic;
            }

            return false;
        }

        public static bool operator==(ROSBridgeSubscriber<T> lhs, ROSBridgeSubscriber<T> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator!=(ROSBridgeSubscriber<T> lhs, ROSBridgeSubscriber<T> rhs)
        {
            return !lhs.Equals(rhs);
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (topic != null ? topic.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Blocked.GetHashCode();
                return hashCode;
            }
        }
    }
}