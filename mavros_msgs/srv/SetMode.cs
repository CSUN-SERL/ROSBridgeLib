using ROSBridgeLib.Core;
using ROSBridgeLib.Extensions;
using SimpleJSON;

// http://docs.ros.org/api/mavros_msgs/html/srv/SetMode.html

namespace ROSBridgeLib.mavros_msgs.srv
{
    
//      uint8 MAV_MODE_PREFLIGHT = 0
//      uint8 MAV_MODE_STABILIZE_DISARMED = 80
//      uint8 MAV_MODE_STABILIZE_ARMED = 208
//      uint8 MAV_MODE_MANUAL_DISARMED	= 64
//      uint8 MAV_MODE_MANUAL_ARMED	= 192
//      uint8 MAV_MODE_GUIDED_DISARMED = 88
//      uint8 MAV_MODE_GUIDED_ARMED = 216
//      uint8 MAV_MODE_AUTO_DISARMED = 92
//      uint8 MAV_MODE_AUTO_ARMED = 220
//      uint8 MAV_MODE_TEST_DISARMED = 66
//      uint8 MAV_MODE_TEST_ARMED = 194
    public enum MAVMode : uint
    {
        Preflight = 0,
        StabilizeDisarmed = 80,
        StabilizeArmed = 208,
        ManualDisarmed = 64,
        ManualArmed = 192,
        GuidedDisarmed = 88,
        GuidedArmed = 216,
        AutoDisarmed = 92,
        AutoArmed = 220,
        TestDisarmed = 66,
        TestArmed = 194
    }
    
    public class SetModeRequest : IServiceRequest
    {
        public static readonly string OffBoard = "OFFBOARD";

        public MAVMode BaseMode;
        public string CustomMode;

        public SetModeRequest(MAVMode baseMode, string customMode)
        {
            BaseMode = baseMode;
            CustomMode = customMode;
        }
        
        public string ToJSONList()
        {
            return$"[{(uint)BaseMode}, \"{CustomMode}\"]";
        }

        public override string ToString()
        {
            return $"mavros_msgs/srv/SetModeRequest [base_mode={(uint)BaseMode}, custom_mode={CustomMode}]";
        }
    }

    public class SetModeResponse : IServiceResponse
    {
        public bool ModeSent { get; private set; }
        
        public SetModeResponse() { }
        
        public void Deserialize(JSONNode msg)
        {
            ModeSent = bool.Parse(msg["mode_sent"]);
        }

        public override string ToString()
        {
            return $"mavros_msgs/srv/SetModeResponse [mode_sent={ModeSent.ToStringLower()}]";
        }
    }
}