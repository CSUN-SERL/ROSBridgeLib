using SimpleJSON;

namespace ROSBridgeLib.Core
{
    public delegate void ROSCallback<T>(T msg) where T : IMsg;	 
    
    public interface IMsg
    {
        string ROSMessageType { get; }
        void Deserialize(JSONNode json);
        string ToYAMLString();
    }
}