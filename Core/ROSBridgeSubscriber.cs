using System;
using System.CodeDom;
using SimpleJSON;

namespace ROSBridgeLib.Core
{
    public class ROSBridgeSubscriber <T> where T : IMsg
    {
        private string topic;
        private object callbackOwner;
        private ROSCallback<T> callback;
        private Type messageType;

        public string Topic
        {
            get { return topic; }
        }
        
        public object CallbackOwner
        {
            get { return callbackOwner; }
        }

        public Type MessageType
        {
            get { return messageType; }
        }
        
        public ROSBridgeSubscriber(string topic, object callbackOwner, ROSCallback<T> callback)
        {
            
            this.topic = topic;
            this.callbackOwner = callbackOwner;
            this.messageType = typeof(T);
            Subscribe(callback);
        }
        
        public bool Blocked { get; set; }
        
        public void Unsubscribe()
        {
            //todo somehow remove this object from the ROSBridgeWebsocketConnection
            callback = null;
        }

        public void Subscribe(ROSCallback<T> callback)
        {
            this.callback = callback;
        }
        
        public void ProcessMsg(JSONNode json)
        {
            if (Blocked)
                return;
            
            T msg = Activator.CreateInstance<T>();
            msg.Deserialize(json);
            callback?.Invoke(msg);
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

        public static bool operator==(ROSBridgeSubscriber<T> lhs, ROSBridgeSubscriber<T> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator!=(ROSBridgeSubscriber<T> lhs, ROSBridgeSubscriber<T> rhs)
        {
            return !lhs.Equals(rhs);
        }
        
        protected bool Equals(ROSBridgeSubscriber<T> other)
        {
            if (ReferenceEquals(this, other))
                return true;
            
            if (other != null)
            {
                return topic == other.topic
                       && callbackOwner == other.callbackOwner
                       && callback == other.callback;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (topic != null ? topic.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (callback != null ? callback.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (callbackOwner != null ? callbackOwner.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Blocked.GetHashCode();
                return hashCode;
            }
        }
    }
}