using ROSBridgeLib.Core;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace std_msgs {
		public class UInt16Msg : ROSBridgeMessage {
			private ushort _data;

			public override string ROSMessageType
			{
				get{ return "std_msgs/UInt16"; }
			}
			
			public UInt16Msg() {}
			
			public UInt16Msg(JSONNode msg) {
				_data = ushort.Parse(msg);
			}
			
			public UInt16Msg(ushort data) {
				_data = data;
			}
			
			public ushort GetData() {
				return _data;
			}
			
			public override void Deserialize(JSONNode msg)
			{
				_data = ushort.Parse(msg);
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