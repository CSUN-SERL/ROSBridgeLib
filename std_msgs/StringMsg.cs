using ROSBridgeLib.Core;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace std_msgs {
		public class StringMsg : ROSBridgeMessage {
			private string _data;

			public override string ROSMessageType
			{
				get{ return "std_msgs/String"; }
			}
			
			public StringMsg(){}
			
			public StringMsg(JSONNode msg) {
				_data = msg["data"];
			}
			
			public StringMsg(string data) {
				_data = data;
			}
			
			public string GetData() {
				return _data;
			}
			
			public override void Deserialize(JSONNode msg)
			{
				_data = msg["data"];
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