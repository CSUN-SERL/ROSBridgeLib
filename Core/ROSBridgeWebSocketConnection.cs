using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System;
using WebSocketSharp;
using SimpleJSON;
using UnityEngine;

 namespace ROSBridgeLib.Core
 {
	 
 public class ROSBridgeWebSocketConnection
 {
	private string _host;
	 private int _port;
	 private WebSocket _ws;
	 private Thread _myThread;
	 
	 private Dictionary<string, Type> allTopics;
	 private Dictionary<string, ROSBridgeSubscriber> subscribers;
	 private Dictionary<string, ROSBridgePublisher> publishers;
	 
	 private Type _serviceResponse; // to deal with service responses
	 private string _serviceName = null;
	 private string _serviceValues = null;

	 private object _queueLock = new object();

	 private static void ServiceResponse(Type t, string service, string yaml)
	 {
		 t.GetMethod("ServiceCallBack", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
			 .Invoke(null, new object[] {service, yaml});
	 }

	 private static void IsValidServiceResponse(Type t)
	 {
		 if (t.GetMethod("ServiceCallBack", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) ==
		     null)
			 throw new Exception("invalid service response handler");
	 }

	 
	 /// <summary>
	 /// Make a connection to a host/port. 
	 /// This does not actually start the connection, use Connect to do that.
	 /// </summary>
	 public ROSBridgeWebSocketConnection(string host, int port)
	 {
		 _host = host;
		 _port = port;

		 _ws = null;
		 _myThread = null;
		 
		 subscribers = new Dictionary<string, ROSBridgeSubscriber>();
		 publishers = new Dictionary<string, ROSBridgePublisher>();
		 allTopics = new Dictionary<string, Type>();
		 
	 }

	 public ROSBridgePublisher GetPublisher(string topic)
	 {
		 ROSBridgePublisher publisher;
		 return publishers.TryGetValue(topic, out publisher) ? publisher : null;
	 }
	 
	 public ROSBridgeSubscriber GetSubscriber(string topic)
	 {
		 ROSBridgeSubscriber subscriber;
		 return subscribers.TryGetValue(topic, out subscriber) ? subscriber : null;
	 }

	 public ROSBridgeSubscriber<T> GetSubscriber<T>(string topic) where T: IMsg
	 {
		ThrowIfTopicExistsUnderDifferentType(topic, typeof(T), "obtain");
		return GetSubscriber(topic) as ROSBridgeSubscriber<T>;
	 }
	 
	 
	 /**
	  * Add a service response callback to this connection.
	  */
	 public void AddServiceResponse(Type serviceResponse)
	 {
		 IsValidServiceResponse(serviceResponse);
		 _serviceResponse = serviceResponse;
	 }

	 private void ThrowIfTopicExistsUnderDifferentType(string topic, Type type, string operation)
	 {
		 Type messageType;
		 if (allTopics.TryGetValue(topic, out messageType))
		 {
			 if (type != messageType)
			 {
				 throw new Exception(
					 $"Topic: {topic} already exists for message type: {messageType}. You are attempting to {operation} same topic with message type {type}");
			 }
		 }
	 }

	 public ROSBridgePublisher Advertise<T>(string topic) where T : IMsg
	 {
		 Type messageType = typeof(T);
		 ThrowIfTopicExistsUnderDifferentType(topic, messageType, "advertise");

		 ROSBridgePublisher publisher;
		 if (!publishers.TryGetValue(topic, out publisher))
		 {
			 publisher = new ROSBridgePublisher(_ws, topic, messageType);
			 publishers.Add(topic, publisher);
			 CacheTopic(topic, messageType);
			 SendAdvertiseOperation(publisher);
		 }
		 
		 return publisher;
	 }

	 private void CacheTopic(string topic, Type messageType)
	 {
		 if (!allTopics.ContainsKey(topic))
		 {
			 allTopics.Add(topic, messageType);
		 }
	 }
	 
	 public ROSBridgeSubscriber<T> Subscribe<T>(string topic, ROSCallback<T> callback, int queueSize) where T : IMsg, new()
	 {
		 Type messageType = typeof(T);
		 ThrowIfTopicExistsUnderDifferentType(topic, messageType, "subscribe");
		 
		 ROSBridgeSubscriber subscriber;
		 if (!subscribers.TryGetValue(topic, out subscriber))
		 {
			 subscriber = new ROSBridgeSubscriber<T>(topic, queueSize);
			 subscribers.Add(topic, subscriber);
			 CacheTopic(topic, messageType);
			 SendSubscribeOperation(subscriber);
		 }

		 var sub = (ROSBridgeSubscriber<T>) subscriber;

		 sub.Subscribe(callback);
		 
		 return sub;
	 }
	 
	 /// <summary>
	 /// Connect to the remote ros environment.
	 /// </summary>
	 public void Connect()
	 {
		 if (_myThread != null)
			 return;
		 
		 Initialize();
		 
		 foreach (var sub in subscribers)
		 {
			 SendSubscribeOperation(sub.Value);
		 }

		 foreach (var pub in publishers)
		 {
			 SendAdvertiseOperation(pub.Value);
		 }
		 
		 _myThread = new Thread(Run);
		 _myThread.Start();
	 }

	 /// <summary>
	 /// Disconnect from the remove ros environment.
	 /// </summary>
	 public void Disconnect()
	 {
		 _myThread.Abort();

		 foreach (var subscriber in subscribers)
		 {
			 SendUnSubscribeOperation(subscriber.Value);
		 }

		 foreach (var publisher in publishers)
		 {
			 SendUnAdvertiseOperation(publisher.Value);
		 }

		 _ws.Close();
	 }

	 public void Initialize()
	 {
		 _ws = new WebSocket(_host + ":" + _port);
		 _ws.OnMessage += OnMessage;
		 _ws.Connect();
	 }
	 
	 public void Run()
	 {
		 while (true)
		 {
			 Thread.Sleep(10);
			 Render();
		 }
	 }
 
	 private void OnMessage(object sender, MessageEventArgs args)
	 {
		 string s = args.Data;
		 
		 if (string.IsNullOrEmpty(s))
		 {
			 Debug.Log("got an empty message");
			 return;	 
		 }

		 JSONNode node = JSONNode.Parse(s);
		 string op = node["op"];
		 if ("publish".Equals(op))
		 {
			 lock(_queueLock)
			 {
				 GetSubscriber(node["topic"])?.EnqueueRawMsg(node["msg"]);
			 }
		 }
		 else if ("service_response".Equals(op))
		 {
			 Debug.Log("Got service response " + node.ToString());
			 _serviceName = node["service"];
			 _serviceValues = (node["values"] == null) ? "" : node["values"].ToString();
		 }
		 else
			 Debug.Log("Must write code here for other messages");
	 }
	 
	 public void Render()
	 {
		 lock (_queueLock)
		 {
			 foreach (var sub in subscribers)
			 {
				 sub.Value.ProcessOldestMsg();
			 }	 
		 }

		 if (_serviceName != null)
		 {
			 ServiceResponse(_serviceResponse, _serviceName, _serviceValues);
			 _serviceName = null;
		 }
	 }

	 public void CallService(string service, string args)
	 {
		 if (_ws != null)
		 {
			 string s = ROSBridgeMsg.CallService(service, args);
			 Debug.Log($"Sending: {s}");
			 _ws.Send(s);
		 }
	 }
	 
	 private void SendSubscribeOperation(ROSBridgeSubscriber subscriber)
	 {
		 string s = ROSBridgeMsg.Subscribe(subscriber.Topic, ((IMsg)Activator.CreateInstance(subscriber.MessageType)).ROSMessageType);
		 Debug.Log($"Sending: {s}");
		 _ws?.Send(s);
	 }

	 private void SendUnSubscribeOperation(ROSBridgeSubscriber subscriber)
	 {
		 string s = ROSBridgeMsg.UnSubscribe(subscriber.Topic);
		 Debug.Log($"Sending: {s}");
		 _ws?.Send(s);
	 }
	 
	 private void SendAdvertiseOperation(ROSBridgePublisher subscriber)
	 {
		 string s = ROSBridgeMsg.Advertise(subscriber.Topic, ((IMsg)Activator.CreateInstance(subscriber.MessageType)).ROSMessageType);
		 Debug.Log($"Sending: {s}");
		 _ws?.Send(s);
	 }
	 
	 private void SendUnAdvertiseOperation(ROSBridgePublisher publisher)
	 {
		 string s = ROSBridgeMsg.UnAdvertise(publisher.Topic);
		 Debug.Log($"Sending: {s}");
		 _ws?.Send(s);
	 }
	 
	 }
 }
