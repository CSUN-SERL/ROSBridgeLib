using SimpleJSON;
using ROSBridgeLib.std_msgs;

namespace ROSBridgeLib.mavros_msgs
{
    public class StateMsg : ROSBridgeMsg
    {
        public static readonly string DefaultTopic = "mavros/state";
        
        public HeaderMsg Header;
        public bool Connected;
        public bool Armed;
        public bool Guided;
        public string Mode;
        public uint SystemStatus;
        
        public StateMsg(JSONNode msg)
        {
            Header = new HeaderMsg(msg["header"]);
            Connected = bool.Parse(msg["connected"]);
            Armed = bool.Parse(msg["armed"]);
            Guided = bool.Parse(msg["guided"]);
            Mode = msg["mode"];
            SystemStatus = uint.Parse(msg["system_status"]);
        }

        public static string GetMessageType()
        {
            return "mavros_msgs/State";
        }
        
        public override string ToString()
        {
            return $"{GetMessageType()} [header={Header}, connected={Connected}, armed={Armed}, guided={Guided}, mode={Mode}, system_status={SystemStatus}]";
        }

        public override string ToYAMLString()
        {
            return $"{{\"header\" : {Header}, \"connected\" : {Connected}, \"armed\" : {Armed}, \"guided\" : {Guided}, \"mode\" : {Mode}, \"system_status\" : {SystemStatus}}}";            
        }
        
    }
}