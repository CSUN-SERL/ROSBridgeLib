using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using WebSocketSharp;
using SimpleJSON;

namespace ROSBridgeLib.Core
{

	public class ROSBridgeConnection
	{
		private string host;
		private int port;
		private WebSocket socket;

		private TopicManager topicManager;
		private ServiceManager serviceManager;

		public TopicManager TopicManager
		{
			get
			{
				ThrowIfNotConnected("Attempting to get TopicManager before initialization");
				return topicManager;
			}
		}

		public ServiceManager ServiceManager
		{
			get
			{
				ThrowIfNotConnected("Attempting to get ServiceManager before initialization");
				return serviceManager;
			}
		}

		/// <summary>
		/// Make a connection to a host/port. 
		/// This does not actually start the connection, use Connect to do that.
		/// </summary>
		public ROSBridgeConnection(string host, int port)
		{
			this.host = host;
			this.port = port;
			this.socket = null;
		}

		~ROSBridgeConnection()
		{
			Disconnect();
		}

		/// <summary>
		/// Connect to the remote ros environment.
		/// </summary>
		public void Connect()
		{
			if (socket != null)
				return;
			
			socket = new WebSocket(host + ":" + port);
			socket.OnMessage += OnMessage;
			socket.Connect();
			
			topicManager = new TopicManager(socket);
			serviceManager = new ServiceManager(socket);
			
			topicManager.Connect();
		}

		/// <summary>
		/// Disconnect from the remove ros environment.
		/// </summary>
		public void Disconnect()
		{
			topicManager.Disconnect();
			socket.Close();
		}

		private void ThrowIfNotConnected(string operation)
		{
			if (socket == null)
			{
				throw new Exception($"{operation}. Call Connect() first!");
			}
		}

		private void OnMessage(object sender, MessageEventArgs args)
		{
			string s = args.Data;

			if (string.IsNullOrEmpty(s))
			{
				Debug.Print("got an empty message.");
			}
		}
	}
}
