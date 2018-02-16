using System;
using WebSocketSharp;

namespace ROSBridgeLib.Core
{
    public class Publisher
    {
        protected WebSocket socket;
        protected string topic;
        protected Type messageType;
        
        public Type MessageType
        {
            get { return messageType; }
        }

        public string Topic
        {
            get { return topic; }
        }

        public Publisher(WebSocket socket, string topic, Type messageType)
        {
            this.socket = socket;
            this.topic = topic;
            this.messageType = messageType;
        }

        public Publisher Publish(IMessage msg)
        {
            socket.Send(ROSBridgeMessage.Publish(topic, msg.ToYAMLString()));
            return this;
        }
    }
}