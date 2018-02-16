using ROSBridgeLib.Core;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace std_msgs {
		public class UInt8Msg : ROSBridgeMessage {
			private byte _data;
			
			public override string ROSMessageType
			{
				get{ return "std_msgs/UInt8"; }
			}
			
			public UInt8Msg(){}
			
			public UInt8Msg(JSONNode msg) {
				_data = byte.Parse(msg);
			}
			
			public UInt8Msg(byte data) {
				_data = data;
			}
			
			public byte GetData() {
				return _data;
			}
			
			public override void Deserialize(JSONNode msg)
			{
				_data = byte.Parse(msg);
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