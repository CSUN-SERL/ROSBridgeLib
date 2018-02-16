using SimpleJSON;

namespace ROSBridgeLib.Core
{
    public delegate void ROSCallback<in T>(T msg) where T : IMessage;
    
    public interface IMessage
    {
        string ROSMessageType { get; }
        void Deserialize(JSONNode msg);
        string ToYAMLString();
    }
}