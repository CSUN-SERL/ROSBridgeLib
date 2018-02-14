using ROSBridgeLib.Core;
using ROSBridgeLib.Extensions;
using SimpleJSON;

// http://docs.ros.org/api/mavros_msgs/html/srv/CommandBool.html

namespace ROSBridgeLib.mavros_msgs.srv
{
    public class CommandBoolRequest : IServiceRequest
    {
        public bool Value;

        public CommandBoolRequest(bool value)
        {
            Value = value;
        }

        public string ToJSONList()
        {
            return $"[{Value.ToStringLower()}]";
        }

        public override string ToString()
        {
            return $"mavros_msgs/srv/CommandBoolRequest [value={Value.ToStringLower()}]";
        }
    }
    
    public class CommandBoolResponse : IServiceResponse
    {
        public bool Success { get; private set; }
        public uint Result { get; private set; }
        
        public CommandBoolResponse() { }
        
        public void Deserialize(JSONNode msg)
        {
            Success = bool.Parse(msg["success"]);
            Result = uint.Parse(msg["result"]);
        }
        
        public override string ToString()
        {
            return $"mavros_msgs/srv/CommandBoolResponse [success={Success.ToStringLower()}, result={Result}]";
        }
        
    }
}