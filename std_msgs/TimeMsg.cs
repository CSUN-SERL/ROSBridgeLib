using System;
using ROSBridgeLib.Core;
using SimpleJSON;

/**
 * Define a time message. These have been hand-crafted from the corresponding msg file.
 * 
 * Version History
 * 3.1 - changed methods to start with an upper case letter to be more consistent with c#
 * style.
 * 3.0 - modification from hand crafted version 2.0
 * 
 * @author Michael Jenkin, Robert Codd-Downey and Andrew Speers
 * @version 3.1
 */

namespace ROSBridgeLib {
	namespace std_msgs {
		public class TimeMsg : ROSBridgeMessage {
			private int _secs, _nsecs;

			public override String ROSMessageType
			{
				get { return "std_msgs/Time"; }
			}
			
			public TimeMsg() {}
			
			public TimeMsg(JSONNode msg) {
				_secs = int.Parse(msg["secs"]);
				_nsecs = int.Parse (msg["nsecs"]);
			}

			public TimeMsg(int secs, int nsecs) {
				_secs = secs;
				_nsecs = nsecs;
			}

			public int GetSecs() {
				return _secs;
			}

			public int GetNsecs() {
				return _nsecs;
			}

			public override void Deserialize(JSONNode msg)
			{
				_secs = int.Parse(msg["secs"]);
				_nsecs = int.Parse (msg["nsecs"]);
			}

			public override string ToString() {
				return ROSMessageType + " [secs=" + _secs + ",  nsecs=" + _nsecs + "]";
			}

			public override string ToYAMLString() {
				return "{\"data\" : {\"secs\" : " + _secs + ", \"nsecs\" : " + _nsecs + "}}";
			}
		}
	}
}
