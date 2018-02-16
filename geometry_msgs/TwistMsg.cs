using ROSBridgeLib.Core;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class TwistMsg : ROSBridgeMessage {
			private Vector3Msg _linear;
			private Vector3Msg _angular;

			public override string ROSMessageType
			{
				get { return "geometry_msgs/Twist"; }
			}

			public TwistMsg() { }

			public TwistMsg(JSONNode msg) {
				_linear = new Vector3Msg(msg["linear"]);
				_angular = new Vector3Msg(msg["angular"]);
			}
			
			public TwistMsg(Vector3Msg linear, Vector3Msg angular) {
				_linear = linear;
				_angular = angular;
			}
			
			public Vector3Msg GetLinear() {
				return _linear;
			}

			public Vector3Msg GetAngular() {
				return _angular;
			}
			
			public override void Deserialize(JSONNode msg)
			{
				_linear = new Vector3Msg(msg["linear"]);
				_angular = new Vector3Msg(msg["angular"]);
			}

			public override string ToString() {
				return ROSMessageType + " [linear=" + _linear.ToString() + ",  angular=" + _angular.ToString() + "]";
			} 
			
			public override string ToYAMLString() {
				return "{\"linear\" : " + _linear.ToYAMLString() + ", \"angular\" : " + _angular.ToYAMLString() + "}";
			}
		}
	}
}