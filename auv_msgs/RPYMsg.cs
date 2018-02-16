using System;
using ROSBridgeLib.Core;
using SimpleJSON;

/**
 * Define a auv_msgs NED message. This has been hand-crafted from the corresponding
 * auv_msgs message file.
 * 
 * @author Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace auv_msgs {
		public class RPYMsg : ROSBridgeMessage {
			private float _roll, _pitch, _yaw;

			private static float Rad2Deg = 180 / (float)Math.PI;
			
			public override string ROSMessageType
			{
				get { return "auv_msgs/RPY"; }
			}
			
			public RPYMsg() {}
			
			public RPYMsg(JSONNode msg) {
				_roll = float.Parse(msg["roll"]);
				_pitch  = float.Parse(msg["pitch"]);
				_yaw = float.Parse(msg["yaw"]);
			}

			public RPYMsg(float rollRadians, float pitchRadians, float yawRadians) {
				_roll = rollRadians;
				_pitch = pitchRadians;
				_yaw = yawRadians;
			}

			public float GetRoll() {
				return _roll;
			}

			public float GetPitch() {
				return _pitch;
			}

			public float GetYaw() {
				return _yaw;
			}

			public float GetRollDegrees()  {
				return _roll * Rad2Deg;
			}

			public float GetPitchDegrees()  {
				return _pitch * Rad2Deg;
			}

			public float GetYawDegrees()  {
				return _yaw * Rad2Deg;
			}

			public override void Deserialize(JSONNode msg)
			{
				_roll = float.Parse(msg["roll"]);
				_pitch  = float.Parse(msg["pitch"]);
				_yaw = float.Parse(msg["yaw"]);
			}
			
			public override string ToString() {
				return ROSMessageType + " [roll=" + _roll + ",  pitch=" + _pitch + ", yaw=" + _yaw + "]";
			}

			public override string ToYAMLString() {
				return "{\"roll\": " + _roll + ", \"pitch\": " + _pitch + ", \"yaw\": " + _yaw + "}";
			}
		}
	}
}

