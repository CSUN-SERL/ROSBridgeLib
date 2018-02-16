using ROSBridgeLib.Core;
using SimpleJSON;

/**
 * Define a geometry_msgs point message. This has been hand-crafted from the corresponding
 * geometry_msgs message file.
 * 
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class PointMsg : ROSBridgeMessage {
			private float _x, _y, _z;

			public override string ROSMessageType
			{
				get { return "geometry_msgs/Point"; }
			}

			public PointMsg() { }
			
			public PointMsg(JSONNode msg) {
				_x = float.Parse(msg["x"]);
				_y = float.Parse(msg["y"]);
				_z = float.Parse(msg["z"]);
			}

			public PointMsg(float x, float y, float z) {
				_x = x;
				_y = y;
				_z = z;
			}
			
			public float GetX() {
				return _x;
			}
			
			public float GetY() {
				return _y;
			}
			
			public float GetZ() {
				return _z;
			}
			
			public override void Deserialize(JSONNode msg)
			{
				_x = float.Parse(msg["x"]);
				_y = float.Parse(msg["y"]);
				_z = float.Parse(msg["z"]);
			}
			
			public override string ToString() {
				return ROSMessageType + " [x=" + _x + ",  y=" + _y + ", z=" + _z + "]";
			}

			public override string ToYAMLString() {
				return "{\"x\": " + _x + ", \"y\": " + _y + ", \"z\": " + _z + "}";
			}
		}
	}
}
