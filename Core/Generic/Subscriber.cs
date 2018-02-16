namespace ROSBridgeLib.Core.Generic
{
    public class Subscriber<TMessage> : Subscriber where TMessage : IMessage
    {
        public new event ROSCallback<TMessage> MessageRecieved;

        public Subscriber(string topic, int queueSize) :
            base(topic, queueSize, typeof(TMessage))
        { }

        public void Unsubscribe(ROSCallback<TMessage> callback)
        {
            MessageRecieved -= callback;
        }

        public void Subscribe(ROSCallback<TMessage> callback)
        {
            MessageRecieved += callback;
        }

        protected override void ProcessMsg(IMessage msg)
        {
            base.ProcessMsg(msg);
            MessageRecieved?.Invoke((TMessage)msg);
        }
    }
}
