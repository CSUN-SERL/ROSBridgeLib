using ROSBridgeLib.Core;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace std_msgs {
		public class UInt32Msg : ROSBridgeMsg {
			private uint _data;
			
			public override string ROSMessageType
			{
				get{ return "std_msgs/UInt32"; }
			}
			
			public UInt32Msg() {}
			
			public UInt32Msg(JSONNode msg) {
				_data = uint.Parse(msg);
			}
			
			public UInt32Msg(uint data) {
				_data = data;
			}
			
			public uint GetData() {
				return _data;
			}
			
			public override string ToString() {
				return ROSMessageType + " [data=" + _data + "]";
			}

			public override void Deserialize(JSONNode msg)
			{
				_data = uint.Parse(msg);
			}

			public override string ToYAMLString() {
				return "{\"data\" : " + _data + "}";
			}
		}
	}
}