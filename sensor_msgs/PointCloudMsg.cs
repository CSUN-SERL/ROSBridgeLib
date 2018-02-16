using System;
using ROSBridgeLib.Core;
using SimpleJSON;
using ROSBridgeLib.std_msgs;

/**
 * Define a point cloud message.
 * 
 * @author Mathias Ciarlo Thorstensen
 */

namespace ROSBridgeLib {
	namespace sensor_msgs {
		public class PointCloudMsg : ROSBridgeMessage {
			private HeaderMsg _header;
			private uint _width, _height;
			private uint _point_step, _row_step;
			private uint _num_points;

			private byte[] _data;

			public override string ROSMessageType
			{
				get{ return "sensor_msgs/PointCloud2"; }
			}
			
			public PointCloudMsg() { }
			
			public PointCloudMsg (JSONNode msg) {
				_header = new HeaderMsg (msg ["header"]);

				_width = uint.Parse(msg["width"]);
				_height = uint.Parse(msg["height"]);
				_point_step = uint.Parse(msg["point_step"]);
				_row_step = uint.Parse(msg["row_step"]);
				_num_points = _width * _height;

				_data = System.Convert.FromBase64String(msg ["data"]); // Converts the JSONNode to a byte array
			}
			
			public byte[] GetData()	{
				return _data;
			}

			public uint GetWidth() {
				return _width;
			}

			public uint GetHeight() {
				return _height;
			}

			public uint GetPointStep() {
				return _point_step;
			}

			public uint GetRowStep() {
				return _row_step;
			}

			public uint GetNumPoints() {
				return _num_points;
			}

			public override void Deserialize(JSONNode msg)
			{
				_header = new HeaderMsg (msg ["header"]);

				_width = uint.Parse(msg["width"]);
				_height = uint.Parse(msg["height"]);
				_point_step = uint.Parse(msg["point_step"]);
				_row_step = uint.Parse(msg["row_step"]);
				_num_points = _width * _height;

				_data = System.Convert.FromBase64String(msg ["data"]); // Converts the JSONNode to a byte array
			}

			public override string ToString() {
				return ROSMessageType + " [width=" + _width + ", height=" + _height + ", point_step=" + _point_step + ", row_step=" + _row_step + ", num_points=" + _num_points + "]";
			}

			public override string ToYAMLString()
			{
				throw new NotImplementedException();
			}
			
			public DateTime FromUnixTime(long unixTime)
			{
				DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				return epoch.AddSeconds(unixTime);
			}
		}
	}
}