using ROSBridgeLib.Core;
using SimpleJSON;
/**
 * Define a header message. These have been hand-crafted from the corresponding msg file.
 * 
 * Version History
 * 
 * @author Michael Jenkin, Robert Codd-Downey and Andrew Speers
 * @version 3.0
 */

namespace ROSBridgeLib {
	namespace std_msgs {
		public class HeaderMsg : ROSBridgeMsg {
			private int _seq;
			private TimeMsg _stamp;
			private string _frame_id;
			
			public override string ROSMessageType
			{
				get { return "std_msgs/Header"; }
			}

			public HeaderMsg() { }
			
			public HeaderMsg(JSONNode msg) {
				_seq = int.Parse (msg["seq"]);
				_stamp = new TimeMsg (msg ["stamp"]);
				_frame_id = msg["frame_id"];
			}
			
			public HeaderMsg(int seq, TimeMsg stamp, string frame_id) {
				_seq = seq;
				_stamp = stamp;
				_frame_id = frame_id;
			}
			
			public int GetSeq() {
				return _seq;
			}
			
			public TimeMsg GetTimeMsg() {
				return _stamp;
			}

			public string GetFrameId() {
				return _frame_id;
			}

			public override void Deserialize(JSONNode msg)
			{
				_seq = int.Parse (msg["seq"]);
				_stamp = new TimeMsg (msg ["stamp"]);
				_frame_id = msg["frame_id"];
			}

			public override string ToString()
			{
				return ROSMessageType + " [seq=" + _seq + ", stamp=" + _stamp + ", frame_id=" + _frame_id + "]";
			}
			
			public override string ToYAMLString() {
				return "{\"seq\": " + _seq + ", \"stamp\": " + _stamp.ToYAMLString () + ", \"frame_id\": \"" + _frame_id + "\"}";
			}
		}
	}
}
