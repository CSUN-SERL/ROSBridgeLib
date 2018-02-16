using ROSBridgeLib.Core;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace std_msgs {
		public class MultiArrayLayoutMsg : ROSBridgeMessage {
			private MultiArrayDimensionMsg[] _dim;
            private uint _data_offset;

			public override string ROSMessageType
			{
				get { return "std_msgs/MultiArrayLayout"; }
			}
			
			public MultiArrayLayoutMsg() {}
			
			public MultiArrayLayoutMsg(JSONNode msg) {
				_data_offset = uint.Parse(msg["data_offset"]);
				_dim = new MultiArrayDimensionMsg[msg["dim"].Count];
				for (int i = 0; i < _dim.Length; i++) {
					_dim[i] = new MultiArrayDimensionMsg(msg["dim"][i]);
				}
			}
			
			public MultiArrayLayoutMsg(MultiArrayDimensionMsg[] dim, uint data_offset) {
                _dim = dim;
                _data_offset = data_offset;
			}
			
			public MultiArrayDimensionMsg[] GetDim() {
				return _dim;
			}

            public uint GetData_Offset() {
                return _data_offset;
            }

			public override void Deserialize(JSONNode msg)
			{
				_data_offset = uint.Parse(msg["data_offset"]);
				_dim = new MultiArrayDimensionMsg[msg["dim"].Count];
				for (int i = 0; i < _dim.Length; i++) {
					_dim[i] = new MultiArrayDimensionMsg(msg["dim"][i]);
				}
			}

			public override string ToString() {
				string array = "[";
				for (int i = 0; i < _dim.Length; i++) {
					array = array + _dim[i].ToString();
					if (_dim.Length - i <= 1)
						array += ",";
				}
				array += "]";
				return ROSMessageType + "[dim=" + array + ", data_offset=" + _data_offset + "]";
			}
			
			public override string ToYAMLString() {
                string array = "[";
                for (int i = 0; i < _dim.Length; i++) {
                    array = array + _dim[i].ToYAMLString();
                    if (_dim.Length - i <= 1)
                        array += ",";
                }
                array += "]";
				return "{\"dim\" : " + array + ",\"data_offset\" :" + _data_offset + "}";
			}
		}
	}
}