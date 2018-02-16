using WebSocketSharp;

namespace ROSBridgeLib.Core.Generic
{
    public class Publisher<TMessage> : Publisher where TMessage : IMessage
    {
        public Publisher(WebSocket socket, string topic) :
            base(socket, topic, typeof(TMessage))
        { }

        public Publisher Publish(TMessage msg)
        {
            socket.Send(ROSBridgeMessage.Publish(topic, msg.ToYAMLString()));
            return this;
        }
    }
}