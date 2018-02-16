using SimpleJSON;
using ROSBridgeLib.Core;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace std_msgs {
		public class Float32Msg : ROSBridgeMessage {
			private float _data;

			public override string ROSMessageType
			{
				get { return "std_msgs/Float32"; }
			}
			
			public Float32Msg(){ }			
			
			public Float32Msg(JSONNode msg) {
				_data = float.Parse(msg);
			}
			
			public Float32Msg(float data) {
				_data = data;
			}
			
			public float GetData() {
				return _data;
			}
			
			public override void Deserialize(JSONNode msg)
			{
				_data = float.Parse(msg);
			}
			
			public override string ToString() {
				return ROSMessageType + " [data=" + _data.ToString() + "]";
			}

			public override string ToYAMLString() {
				return "{\"data\" : " + _data.ToString() + "}";
			}
		}
	}
}