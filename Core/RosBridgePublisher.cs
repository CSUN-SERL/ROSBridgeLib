using System;
using WebSocketSharp;

namespace ROSBridgeLib.Core
{
    public class ROSBridgePublisher<T> where T : IMsg
    {
        private WebSocket socket;
        private string topic;

        public Type MessageType{ get { return typeof(T); } }

        public ROSBridgePublisher(WebSocket socket, string topic)
        {
            this.socket = socket;
            this.topic = topic;
        }

        public void Publish(IMsg msg)
        {
            socket.Send(ROSBridgeMsg.Publish(topic, msg.ToYAMLString()));
        }
    }
}