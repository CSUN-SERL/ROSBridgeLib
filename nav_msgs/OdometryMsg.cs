using SimpleJSON;
using ROSBridgeLib.Core;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.geometry_msgs;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace nav_msgs {
		public class OdometryMsg : ROSBridgeMessage {
			public HeaderMsg _header;
			public string _child_frame_id;
			public PoseWithCovarianceMsg _pose;
			public TwistWithCovarianceMsg _twist;
			
			public override string ROSMessageType
			{
				get { return "nav_msgs/Odometry"; }
			}
			
			public OdometryMsg() {}
			
			public OdometryMsg(JSONNode msg) {
				_header = new HeaderMsg(msg["header"]);
				_child_frame_id = msg["child_frame_id"].ToString();
				_pose = new PoseWithCovarianceMsg(msg["pose"]);
				_twist = new TwistWithCovarianceMsg(msg["twist"]);
			}

			public HeaderMsg GetHeader() {
				return _header;
			}

			public string GetChildFrameId() {
				return _child_frame_id;
			}

			public PoseWithCovarianceMsg GetPoseWithCovariance() {
				return _pose;
			}

			public TwistWithCovarianceMsg GetTwistWithCovariance() {
				return _twist;
			}
			
			public override void Deserialize(JSONNode msg)
			{
				_header = new HeaderMsg(msg["header"]);
				_child_frame_id = msg["child_frame_id"].ToString();
				_pose = new PoseWithCovarianceMsg(msg["pose"]);
				_twist = new TwistWithCovarianceMsg(msg["twist"]);
			}

			public override string ToString() {
				return ROSMessageType + " [header=" + _header.ToString() 
				       + ",  child_frame_id=" + _child_frame_id
				       + ",  pose=" + _pose.ToString() 
				       + ",  twist=" + _twist.ToString() + "]";
			}
			
			public override string ToYAMLString() {
				return "{\"header\" : " + _header.ToYAMLString() 
				  + ", \"child_frame_id\" : " + _child_frame_id
				  + ", \"pose\" : " + _pose.ToYAMLString() 
				  + ", \"twist\" : " + _twist.ToYAMLString() + "}";
			}
		}
	}
}