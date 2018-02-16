using SimpleJSON;
using ROSBridgeLib.std_msgs;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class PoseStampedMsg : Core.ROSBridgeMessage {
			private HeaderMsg _header;
			private PoseMsg _pose;
			
			public override string ROSMessageType
			{
				get { return "geometry_msgs/PoseStamped"; }
			}
			
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

			public HeaderMsg GetHeader() {
				return _header;
			}

			public PoseMsg GetPose() {
				return _pose;
			}
			
			public override void Deserialize(JSONNode msg)
			{
				_header = new HeaderMsg(msg["header"]);
				_pose = new PoseMsg(msg["pose"]);
			}
			
			public override string ToString() {
				return ROSMessageType + "  [header=" + _header.ToString() + ",  pose=" + _pose.ToString() + "]";
			}

			public override string ToYAMLString() {
				return "{\"header\" : " + _header.ToYAMLString() + ", \"pose\" : " + _pose.ToYAMLString() + "}";
			}
		}
	}
}