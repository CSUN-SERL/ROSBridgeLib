using System;
using WebSocketSharp;

namespace ROSBridgeLib.Core
{
    public class ROSBridgePublisher
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

        public ROSBridgePublisher(WebSocket socket, string topic, Type messageType)
        {
            this.socket = socket;
            this.topic = topic;
            this.messageType = messageType;
        }

        public void Publish(IMsg msg)
        {
            socket.Send(ROSBridgeMsg.Publish(topic, msg.ToYAMLString()));
        }
    }
}