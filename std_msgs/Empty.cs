using System;
using ROSBridgeLib.Core;
using SimpleJSON;

namespace ROSBridgeLib.std_msgs
{
    public class Empty : ROSBridgeMessage
    {
        public override string ROSMessageType { get { return "std_msgs/Empty;"; } }
        
        public Empty() {}
        
        public override void Deserialize(JSONNode msg) { }

        public override string ToYAMLString() { return "{}"; }
    }
}