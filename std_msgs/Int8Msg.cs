using ROSBridgeLib.Core;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace std_msgs {
		public class Int8Msg : ROSBridgeMsg {
			private sbyte _data;

			public override string ROSMessageType
			{
				get{ return "std_msgs/Int8"; }
			}
			
 			public Int8Msg() {}
 			
 			public Int8Msg(JSONNode msg) {
 				_data = sbyte.Parse(msg["data"]);
 			}
 			
 			public Int8Msg(sbyte data) {
 				_data = data;
 			}
 			
 			public sbyte GetData() {
 				return _data;
 			}
 			
			public override void Deserialize(JSONNode msg)
			{
				_data = sbyte.Parse(msg["data"]);
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