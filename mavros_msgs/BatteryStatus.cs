using SimpleJSON;
using ROSBridgeLib.std_msgs;

namespace ROSBridgeLib.mavros_msgs
{
    public class BatteryStatus : ROSBridgeMsg
    {
        // Represent battery status from SYSTEM_STATUS
        
        public HeaderMsg Header;
        public float Voltage; // [V]
        public float Current; // [A]
        public float Remaining; // 0..1

        public BatteryStatus(HeaderMsg header)
        {
            this.Header = header;
        }

        public BatteryStatus(JSONNode msg)
        {
            Header = new HeaderMsg(msg["header"]);
            Voltage = float.Parse(msg["voltage"]);
            Current = float.Parse(msg["current"]);
            Remaining = float.Parse(msg["remaining"]);
        }

        public static string GetMessageType()
        {
            return "mavros_msgs/BatteryStatus";
        }
        
        public override string ToString()
        {
            return $"mavros_msgs/BatteryStatus [header={Header}, voltage={Voltage}, current={Current}, remainging={Remaining}]";
        }

        public override string ToYAMLString()
        {
            return $"{{\"header\" : {Header}, \"voltage\" : {Voltage}, \"current\" : {Current}, \"remaining\" : {Remaining}}}";
        }
    }
}