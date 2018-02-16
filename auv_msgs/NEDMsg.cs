using SimpleJSON;
using ROSBridgeLib.Core;

/**
 * Define a auv_msgs NED message. This has been hand-crafted from the corresponding
 * auv_msgs message file.
 * 
 * @author Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace auv_msgs {
		public class NEDMsg : ROSBridgeMessage {
			private float _north, _east, _depth;

			public override string ROSMessageType
			{
				get { return "auv_msgs/NED"; }
			}
			
			public NEDMsg() {}
			
			public NEDMsg(JSONNode msg) {
				_north = float.Parse(msg["north"]);
				_east  = float.Parse(msg["east"]);
				_depth = float.Parse(msg["depth"]);
			}

			public NEDMsg(float north, float east, float depth) {
				_north = north;
				_east = east;
				_depth = depth;
			}
			
			public float GetNorth() {
				return _north;
			}
			
			public float GetEast() {
				return _east;
			}
			
			public float GetDepth() {
				return _depth;
			}
			
			public override void Deserialize(JSONNode msg)
			{
				_north = float.Parse(msg["north"]);
				_east  = float.Parse(msg["east"]);
				_depth = float.Parse(msg["depth"]);
			}
			
			public override string ToString() {
				return ROSMessageType + " [north=" + _north + ",  east=" + _east + ", depth=" + _depth + "]";
			}

			public override string ToYAMLString() {
				return "{\"north\": " + _north + ", \"east\": " + _east + ", \"depth\": " + _depth + "}";
			}
		}
	}
}
