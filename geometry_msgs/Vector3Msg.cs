using ROSBridgeLib.Core;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class Vector3Msg : ROSBridgeMessage {
			private double _x;
			private double _y;
			private double _z;
			
			public override string ROSMessageType
			{
				get { return "geometry_msgs/Vector3"; }
			}
			
			public Vector3Msg() { }
			
			public Vector3Msg(JSONNode msg) {
				_x = double.Parse(msg["x"]);
				_y = double.Parse(msg["y"]);
				_z = double.Parse(msg["z"]);
			}
			
			public Vector3Msg(double x, double y, double z) {
				_x = x;
				_y = y;
				_z = z;
			}
			
			public double GetX() {
				return _x;
			}
			
			public double GetY() {
				return _y;
			}
			
			public double GetZ() {
				return _z;
			}
			
			public override void Deserialize(JSONNode msg)
			{
				_x = double.Parse(msg["x"]);
				_y = double.Parse(msg["y"]);
				_z = double.Parse(msg["z"]);
			}

			public override string ToString() {
				return ROSMessageType + " [x=" + _x + ",  y="+ _y + ",  z=" + _z + "]";
			}

			
			public override string ToYAMLString() {
				return "{\"x\" : " + _x + ", \"y\" : " + _y + ", \"z\" : " + _z + "}";
			}
		}
	}
}