using SimpleJSON;
using ROSBridgeLib.Core;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace std_msgs {
		public class Float64Msg : ROSBridgeMessage {
 			private double _data;
 
 			public override string ROSMessageType
 			{
 				get { return "std_msgs/Float64"; }
 			}
 			
			public Float64Msg() { }
			
 			public Float64Msg(JSONNode msg) {
 				_data = double.Parse(msg);
 			}
 			
 			public Float64Msg(double data) {
 				_data = data;
 			}
 			
 			public double GetData() {
 				return _data;
 			}
 			
 			public override string ToString() {
 				return ROSMessageType + " [data=" + _data.ToString() + "]";
 			}
 
 			public override void Deserialize(JSONNode msg)
 			{
 				_data = double.Parse(msg);
 			}
 
 			public override string ToYAMLString() {
 				return "{\"data\" : " + _data.ToString() + "}";
 			}
 		}
 	}
 }