using ROSBridgeLib.Core;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace std_msgs {
        public class UInt16MultiArrayMsg : ROSBridgeMessage {
            private MultiArrayLayoutMsg _layout;
            private ushort[] _data;

            public override string ROSMessageType
            {
                get { return "std_msgs/UInt16MultiArray"; }
            }
            
            public UInt16MultiArrayMsg() {}
            
            public UInt16MultiArrayMsg(JSONNode msg) {
                _layout = new MultiArrayLayoutMsg(msg["layout"]);
                _data = new ushort[msg["data"].Count];
				for (int i = 0; i < _data.Length; i++) {
                    _data[i] = ushort.Parse(msg["data"][i]);
                }
            }

            public UInt16MultiArrayMsg(MultiArrayLayoutMsg layout, ushort[] data) {
                _layout = layout;
                _data = data;
            }

            public ushort[] GetData() {
                return _data;
            }

            public MultiArrayLayoutMsg GetLayout() {
                return _layout;
            }

            public override void Deserialize(JSONNode msg)
            {
                _layout = new MultiArrayLayoutMsg(msg["layout"]);
                _data = new ushort[msg["data"].Count];
                for (int i = 0; i < _data.Length; i++) {
                    _data[i] = ushort.Parse(msg["data"][i]);
                }
            }
            
            public override string ToString()
            {
                string array = "[";
                for (int i = 0; i < _data.Length; i++)
                {
                    array = array + _data[i];
                    if (_data.Length - i <= 1)
                        array += ",";
                }

                array += "]";
                return ROSMessageType + " [layout=" + _layout.ToString() + ", data=" + _data + "]";
            }

            public override string ToYAMLString() {
                string array = "[";
                for (int i = 0; i < _data.Length; i++) {
                    array = array + _data[i];
                    if (_data.Length - i <= 1)
                        array += ",";
                }
                array += "]";
                return "{\"layout\" : " + _layout.ToYAMLString() + ", \"data\" : " + array + "}";
            }
        }
    }
}