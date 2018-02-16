using SimpleJSON;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.Core;

namespace ROSBridgeLib.mavros_msgs
{
    public class StateMsg : ROSBridgeMessage
    {
        public HeaderMsg Header { get; private set; }
        public bool Connected { get; private set; }
        public bool Armed { get; private set; }
        public bool Guided { get; private set; }
        public string Mode { get; private set; }
        public uint SystemStatus { get; private set; }

        public override string ROSMessageType
        {
            get { return "mavros_msgs/State"; }
        }
        
        public StateMsg(){ }

        public StateMsg(JSONNode msg)
        {
            Header = new HeaderMsg(msg["header"]);
            Connected = bool.Parse(msg["connected"]);
            Armed = bool.Parse(msg["armed"]);
            Guided = bool.Parse(msg["guided"]);
            Mode = msg["mode"];
            SystemStatus = uint.Parse(msg["system_status"]);
        }

        public StateMsg(HeaderMsg header, bool connected, bool armed, bool guided, string mode, uint systemStatus)
        {
            Header = header;
            Connected = connected;
            Armed = armed;
            Guided = guided;
            Mode = mode;
            SystemStatus = systemStatus;
        }

        public override void Deserialize(JSONNode msg)
        {
            Header = new HeaderMsg(msg["header"]);
            Connected = bool.Parse(msg["connected"]);
            Armed = bool.Parse(msg["armed"]);
            Guided = bool.Parse(msg["guided"]);
            Mode = msg["mode"];
            SystemStatus = uint.Parse(msg["system_status"]);
        }
        
        public override string ToString()
        {
            return $"{ROSMessageType} [header={Header}, connected={Connected}, armed={Armed}, guided={Guided}, mode={Mode}, system_status={SystemStatus}]";
        }
        
        public override string ToYAMLString()
        {
            return $"{{\"header\" : {Header}, \"connected\" : {Connected}, \"armed\" : {Armed}, \"guided\" : {Guided}, \"mode\" : {Mode}, \"system_status\" : {SystemStatus}}}";            
        }
        
    }
}