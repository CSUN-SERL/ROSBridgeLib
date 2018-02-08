using SimpleJSON;
using ROSBridgeLib.std_msgs;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class PoseStampedMsg : Core.ROSBridgeMsg {
			public HeaderMsg _header;
			public PoseMsg _pose;
			
			public PoseStampedMsg() {}
			
			public PoseStampedMsg(JSONNode msg) {
				_header = new HeaderMsg(msg["header"]);
				_pose = new PoseMsg(msg["pose"]);
			}
 			
            public PoseStampedMsg(HeaderMsg header, PoseMsg pose)
            {
                _header = header;
                _pose = pose;
            }

			public static string GetMessageType() {
				return "geometry_msgs/PoseStamped";
			}
			
			public HeaderMsg GetHeader() {
				return _header;
			}

			public PoseMsg GetPose() {
				return _pose;
			}
			
			public override string ToString() {
				return "geometry_msgs/PoseStamped [header=" + _header.ToString() + ",  pose=" + _pose.ToString() + "]";
			}

			public override string ROSMessageType
			{
				get { return "geometry_msgs/PoseStamped"; }
			}

			public override void Deserialize(JSONNode msg)
			{
				_header = new HeaderMsg(msg["header"]);
				_pose = new PoseMsg(msg["pose"]);
			}

			public override string ToYAMLString() {
				return "{\"header\" : " + _header.ToYAMLString() + ", \"pose\" : " + _pose.ToYAMLString() + "}";
			}
		}
	}
}