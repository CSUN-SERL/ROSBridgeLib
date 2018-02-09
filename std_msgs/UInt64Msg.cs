using ROSBridgeLib.Core;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace std_msgs {
		public class Uint64Msg : ROSBridgeMsg {
			private ulong _data;

			public override string ROSMessageType
			{
				get{ return "std_msgs/UInt64"; }
			}

			public Uint64Msg() {}
			
			public Uint64Msg(JSONNode msg) {
				_data = ulong.Parse(msg);
			}
			
			public Uint64Msg(ulong data) {
				_data = data;
			}
			
			public ulong GetData() {
				return _data;
			}
			
			public override string ToString() {
				return ROSMessageType + " [data=" + _data + "]";
			}
			
			public override void Deserialize(JSONNode msg)
			{
				_data = ulong.Parse(msg);
			}

			public override string ToYAMLString() {
				return "{\"data\" : " + _data + "}";
			}
		}
	}
}