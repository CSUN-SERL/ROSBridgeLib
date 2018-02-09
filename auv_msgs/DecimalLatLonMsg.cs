using SimpleJSON;
using ROSBridgeLib.Core;
/**
 * Define a geographic_msgs GeoPoint message. This has been hand-crafted from the corresponding
 * geographic_msgs message file.
 * 
 * @author Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace auv_msgs {
		public class DecimalLatLonMsg : ROSBridgeMsg {
			private double _latitude, _longitude;

			public override string ROSMessageType
			{
				get { return "auv_msgs/DecimalLatLon"; }
			}
			
			public DecimalLatLonMsg() {}

			public DecimalLatLonMsg(double latitude, double longitude) {
				_latitude = latitude;
				_longitude = longitude;
			}

			public DecimalLatLonMsg(JSONNode node)
			{
				_latitude = double.Parse(node["latitude"]);
				_longitude  = double.Parse(node["longitude"]);
			}

			public static string getMessageType() {
				return "auv_msgs/DecimalLatLon";
			}

			public double GetLatitude() {
				return _latitude;
			}

			public double GetLongitude() {
				return _longitude;
			}

			public override void Deserialize(JSONNode msg)
			{
				_latitude = double.Parse(msg["latitude"]);
				_longitude  = double.Parse(msg["longitude"]);
			}
			
			public override string ToString() {
				return ROSMessageType + " [latitude=" + _latitude + ",  longitude=" + _longitude + "]";
			}
			
			public override string ToYAMLString() {
				return "{\"latitude\": " + _latitude + ", \"longitude\": " + _longitude + "}";
			}
		}
	}
}
