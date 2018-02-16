using SimpleJSON;
using ROSBridgeLib.Core;

/**
 * Define a geographic_msgs GeoPoint message. This has been hand-crafted from the corresponding
 * geographic_msgs message file.
 * 
 * @author Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace geographic_msgs {
		public class GeoPointMsg : ROSBridgeMessage {
			private float _latitude, _longitude, _altitude;

			public override string ROSMessageType
			{
				get { return "geographic_msgs/GeoPoint"; }
			}
			
			public GeoPointMsg() { }
			
			public GeoPointMsg(JSONNode msg) {
				_latitude = float.Parse(msg["latitude"]);
				_longitude  = float.Parse(msg["longitude"]);
				_altitude = float.Parse(msg["altitude"]);
			}

			public GeoPointMsg(float latitude, float longitude, float altitude) {
				_latitude = latitude;
				_longitude = longitude;
				_altitude = altitude;
			}
			
			public float GetLatitude() {
				return _latitude;
			}
			
			public float GetLongitude() {
				return _longitude;
			}
			
			public float GetAltitude() {
				return _altitude;
			}
			
			public override void Deserialize(JSONNode msg)
			{
				_latitude = float.Parse(msg["latitude"]);
				_longitude  = float.Parse(msg["longitude"]);
				_altitude = float.Parse(msg["altitude"]);
			}
			
			public override string ToString() {
				return ROSMessageType + " [latitude=" + _latitude + ",  longitude=" + _longitude + ", altitude=" + _altitude + "]";
			}

			public override string ToYAMLString() {
				return "{\"latitude\": " + _latitude + ", \"longitude\": " + _longitude + ", \"altitude\": " + _altitude + "}";
			}
		}
	}
}