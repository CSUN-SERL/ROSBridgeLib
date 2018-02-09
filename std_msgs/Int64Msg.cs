using ROSBridgeLib.Core;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace std_msgs {
		public class Int64Msg : ROSBridgeMsg {
			private long _data;

			public override string ROSMessageType
			{
				get { return "std_msgs/Int64"; }
			}
			
			public Int64Msg() {}
			
			public Int64Msg(JSONNode msg) {
				_data = long.Parse(msg["data"]);
			}
			
			public Int64Msg(long data) {
				_data = data;
			}
			
			public long GetData() {
				return _data;
			}

			public override void Deserialize(JSONNode msg)
			{
				_data = long.Parse(msg["data"]);
			}

						
			public override string ToString() {
				return ROSMessageType + " [data=" + _data + "]";
			}
			
			public override string ToYAMLString() {
				return "{\"data\" : " + _data + "}";
			}
		}
	}
}