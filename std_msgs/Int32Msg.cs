using ROSBridgeLib.Core;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace std_msgs {
		public class Int32Msg : ROSBridgeMessage {
			private int _data;

			public override string ROSMessageType
			{
				get { return "std_msgs/Int32"; }
			}

			public Int32Msg() { }
			
			public Int32Msg(JSONNode msg) {
				_data = msg["data"].AsInt;
			}
			
			public Int32Msg(int data) {
				_data = data;
			}
			
			public int GetData() {
				return _data;
			}
			
			public override void Deserialize(JSONNode msg)
			{
				_data = msg["data"].AsInt;
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