using SimpleJSON;

namespace ROSBridgeLib.std_msgs
{
    public class Empty : Core.ROSBridgeMsg
    {
        public override string ROSMessageType { get { return "std_msgs/Empty;"; } }
        
        public override void Deserialize(JSONNode node) { }

        public override string ToYAMLString()
        {
            return "{}";
        }
    }
}