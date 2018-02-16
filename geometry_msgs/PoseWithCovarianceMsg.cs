using ROSBridgeLib.Core;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class PoseWithCovarianceMsg : ROSBridgeMessage {
			public PoseMsg _pose;
			private double[] _covariance = new double[36] {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
														   0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
														   0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
														   0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
														   0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
														   0.0, 0.0, 0.0, 0.0, 0.0, 0.0};

			public override string ROSMessageType
			{
				get { return "geometry_msgs/PoseWithCovariance";  }
			}
			
			public PoseWithCovarianceMsg() { }
			
			public PoseWithCovarianceMsg(JSONNode msg) {
				_pose = new PoseMsg(msg["pose"]);
				// Treat covariance
				for (int i = 0; i < msg["covariance"].Count; i++ ) {
					_covariance[i] = double.Parse(msg["covariance"][i]);
				}
			}
			
			public PoseWithCovarianceMsg(PoseMsg pose, double[] covariance) {
				_pose = pose;
				_covariance = covariance;
			}
			
			public PoseMsg GetPose() {
				return _pose;
			}

			public double[] GetCovariance() {
				return _covariance;
			}
			
			public override void Deserialize(JSONNode msg)
			{
				_pose = new PoseMsg(msg["pose"]);
				// Treat covariance
				for (int i = 0; i < msg["covariance"].Count; i++ ) {
					_covariance[i] = double.Parse(msg["covariance"][i]);
				}
			}

			public override string ToString() {
				string array = "[";
				for (int i = 0; i < _covariance.Length; i++) {
					array = array + _covariance[i].ToString();
					if (_covariance.Length - i <= 1) array += ",";
				}
				array += "]";
				return ROSMessageType + " [pose=" + _pose.ToString() + ",  covariance=" + array + "]";
			}
			
			public override string ToYAMLString() {
				string array = "[";
                for (int i = 0; i < _covariance.Length; i++) {
                    array = array + _covariance[i].ToString();
                    if (_covariance.Length - i <= 1) array += ",";
                }
                array += "]";
				return "{\"pose\" : " + _pose.ToYAMLString() + ", \"covariance\" : " + array + "}";
			}
		}
	}
}