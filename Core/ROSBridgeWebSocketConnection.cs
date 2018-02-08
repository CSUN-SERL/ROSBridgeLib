using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System;
using System.CodeDom;
using System.Linq;
using System.Reflection.Emit;
using WebSocketSharp;
using SimpleJSON;
using UnityEngine;

 namespace ROSBridgeLib.Core
 {
	 
 public class ROSBridgeWebSocketConnection
 {
	 private class RenderTask
	 {
		 private Type _subscriber;
		 private string _topic;
		 private ROSBridgeMsg _msg;

		 public RenderTask(Type subscriber, string topic, ROSBridgeMsg msg)
		 {
			 _subscriber = subscriber;
			 _topic = topic;
			 _msg = msg;
		 }

		 public Type getSubscriber()
		 {
			 return _subscriber;
		 }

		 public ROSBridgeMsg getMsg()
		 {
			 return _msg;
		 }

		 public string getTopic()
		 {
			 return _topic;
		 }
	 };

	 private Dictionary<string, Type> allTopics;
	 private Dictionary<string, ROSBridgeSubscriber> subscribers;
	 private Dictionary<string, Type> advertisements;

	 private string _host;
	 private int _port;
	 private WebSocket _ws;
	 private Thread _myThread;
	 private List<Type> _subscribers; // our subscribers
	 private List<Type> _publishers; //our publishers
	 private Type _serviceResponse; // to deal with service responses
	 private string _serviceName = null;
	 private string _serviceValues = null;
	 private List<RenderTask> _taskQ = new List<RenderTask>();

	 private object _queueLock = new object();

	 private static string GetMessageType(Type t)
	 {
		 return (string) t
			 .GetMethod("GetMessageType", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
			 .Invoke(null, null);
	 }

	 private static string GetMessageTopic(Type t)
	 {
		 return (string) t
			 .GetMethod("GetMessageTopic", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
			 .Invoke(null, null);
	 }

	 private static ROSBridgeMsg ParseMessage(Type t, JSONNode node)
	 {
		 return (ROSBridgeMsg) t
			 .GetMethod("ParseMessage", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
			 .Invoke(null, new object[] {node});
	 }

	 private static void Update(Type t, ROSBridgeMsg msg)
	 {
		 t.GetMethod("CallBack", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
			 .Invoke(null, new object[] {msg});
	 }

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

	 private static void IsValidSubscriber(Type t)
	 {
		 if (t.GetMethod("CallBack", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) == null)
			 throw new Exception("missing Callback method");
		 if (t.GetMethod("GetMessageType", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) == null)
			 throw new Exception("missing GetMessageType method");
		 if (t.GetMethod("GetMessageTopic", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) ==
		     null)
			 throw new Exception("missing GetMessageTopic method");
		 if (t.GetMethod("ParseMessage", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) == null)
			 throw new Exception("missing ParseMessage method");
	 }

	 private static void IsValidPublisher(Type t)
	 {
		 if (t.GetMethod("GetMessageType", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) == null)
			 throw new Exception("missing GetMessageType method");
		 if (t.GetMethod("GetMessageTopic", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) ==
		     null)
			 throw new Exception("missing GetMessageTopic method");
	 }

	 /**
	  * Make a connection to a host/port. 
	  * This does not actually start the connection, use Connect to do that.
	  */
	 public ROSBridgeWebSocketConnection(string host, int port)
	 {
		 _host = host;
		 _port = port;

		 _ws = null;
		 _myThread = null;
		 _subscribers = new List<Type>();
		 _publishers = new List<Type>();

		 subscribers = new Dictionary<string, ROSBridgeSubscriber>();
		 advertisements = new Dictionary<string, Type>();
		 allTopics = new Dictionary<string, Type>();
		 
	 }

	 public ROSBridgeSubscriber GetSubscriber(string topic)
	 {
		 ROSBridgeSubscriber subscriber;
		 bool contains;
		 lock (_queueLock)
		 {
			 contains = subscribers.TryGetValue(topic, out subscriber);	 
		 }
		 return contains ? subscriber : null;
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
					 $"Topic: {topic} already exists for message type: {messageType}. You are attempting to {operation} on the same topic with message type {type}");
			 }
		 }
	 }

	 public ROSBridgePublisher<T> Advertise<T>(string topic) where T : IMsg
	 {
		 Type messageType = typeof(T);
		 ThrowIfTopicExistsUnderDifferentType(topic, messageType, "advertise");

		 if (!advertisements.ContainsKey(topic))
		 {
			 advertisements.Add(topic, messageType);
			 CacheTopic(topic, messageType);
		 }

		 return new ROSBridgePublisher<T>(_ws, topic);
	 }

	 private void CacheTopic(string topic, Type messageType)
	 {
		 if (!allTopics.ContainsKey(topic))
		 {
			 allTopics.Add(topic, messageType);
		 }
	 }

	 private void SendSubscription(string topic, string rosMessageType)
	 {
		 string s = ROSBridgeMsg.Subscribe(topic, rosMessageType);
		 Debug.Log(s);
		 _ws?.Send(s);
	 }
	 
	 public ROSBridgeSubscriber<T> Subscribe<T>(string topic, ROSCallback<T> callback, int queueSize) where T : IMsg
	 {
		 Type messageType = typeof(T);
		 ThrowIfTopicExistsUnderDifferentType(topic, messageType, "subscribe");
		 
		 ROSBridgeSubscriber subscriber;
//		 lock (_queueLock)
//		 {
			 if (!subscribers.TryGetValue(topic, out subscriber))
			 {
				 subscriber = new ROSBridgeSubscriber<T>(topic, queueSize);
				 subscribers.Add(topic, subscriber);
				 CacheTopic(topic, messageType);
				 SendSubscription(topic, Activator.CreateInstance<T>().ROSMessageType);
			 }
			 
			 ((ROSBridgeSubscriber<T>) subscriber).Subscribe(callback);
		 //}

		 return (ROSBridgeSubscriber <T>) subscriber;
	 }

	 /**
	  * Connect to the remote ros environment.
	  */
	 public void Connect()
	 {
		 if (_myThread != null)
			 return;
		 
		 _myThread = new System.Threading.Thread(Run);
		 _myThread.Start();
	 }

	 /**
	  * Disconnect from the remote ros environment.
	  */
	 public void Disconnect()
	 {
		 _myThread.Abort();
		 foreach (Type p in _subscribers)
		 {
			 _ws.Send(ROSBridgeMsg.UnSubscribe(GetMessageTopic(p)));
			 Debug.Log("Sending " + ROSBridgeMsg.UnSubscribe(GetMessageTopic(p)));
		 }

		 foreach (Type p in _publishers)
		 {
			 _ws.Send(ROSBridgeMsg.UnAdvertise(GetMessageTopic(p)));
			 Debug.Log("Sending " + ROSBridgeMsg.UnAdvertise(GetMessageTopic(p)));
		 }

		 _ws.Close();
	 }

	 public void Initialize()
	 {
		 _ws = new WebSocket(_host + ":" + _port);
		 _ws.OnMessage += (sender, e) => this.OnMessage(e.Data);
		 _ws.Connect();
	 }
	 
	 public void SubscribeAll()
	 {
		 var subEnum = subscribers.GetEnumerator();
		 while(subEnum.MoveNext())
		 {
			 var sub = subEnum.Current.Value;
			 IMsg msg = (IMsg)Activator.CreateInstance(sub.MessageType);
			 //_ws.Send(ROSBridgeMsg.Subscribe(GetMessageTopic(p), GetMessageType(p)));
			SendSubscription(sub.Topic, msg.ROSMessageType);
		 }
		 subEnum.Dispose();
	 }

	 public void AdvertiseAll()
	 {
//		 foreach (Type p in _publishers)
//		 {
//			 _ws.Send(ROSBridgeMsg.Advertise(GetMessageTopic(p), GetMessageType(p)));
//			 Debug.Log("Sending " + ROSBridgeMsg.Advertise(GetMessageTopic(p), GetMessageType(p)));
//		 }

		 var enumerator = advertisements.GetEnumerator();
		 while (enumerator.MoveNext())
		 {
			 string topic = enumerator.Current.Key;
			 IMsg msg = (IMsg)Activator.CreateInstance(enumerator.Current.Value);
			 string pubOp = ROSBridgeMsg.Advertise(topic, msg.ROSMessageType);
			 Debug.Log($"Sending {pubOp}");
		 }
		 enumerator.Dispose();
	 }
	 
	 public void Run()
	 {
		 Initialize();
		 SubscribeAll();
		 AdvertiseAll();
		 
		 while (true)
		 {
			 Thread.Sleep(1000);
		 }
	 }

	 private void EnqueueSubscriber(string topic, JSONNode jsonMsg)
	 {
		GetSubscriber(topic)?.EnqueueRawMsg(jsonMsg);
 	 }
	 
	 private void OnMessage(string s)
	 {
		 //Debug.Log ("Got a message " + s);
		 if ((s != null) && !s.Equals(""))
		 {
			 JSONNode node = JSONNode.Parse(s);
			 //Debug.Log ("Parsed it");
			 string op = node["op"];
			 //Debug.Log ("Operation is " + op);
			 if ("publish".Equals(op)) // recieving a published message from the server
			 {
				 string topic = node["topic"];
				 EnqueueSubscriber(topic, node["msg"]);
				 //Debug.Log ("Got a message on " + topic);
//				 foreach (Type p in _subscribers)
//				 {
//					 if (topic.Equals(GetMessageTopic(p)))
//					 {
//						 //Debug.Log ("And will parse it " + GetMessageTopic (p));
//						 ROSBridgeMsg msg = ParseMessage(p, node["msg"]);
//						 RenderTask newTask = new RenderTask(p, topic, msg);
//						 lock (_queueLock)
//						 {
//							 bool found = false;
//							 for (int i = 0; i < _taskQ.Count; i++)
//							 {
//								 if (_taskQ[i].getTopic().Equals(topic))
//								 {
//									 _taskQ.RemoveAt(i);
//									 _taskQ.Insert(i, newTask);
//									 found = true;
//									 break;
//								 }
//							 }
//
//							 if (!found)
//								 _taskQ.Add(newTask);
//						 }
//
//					 }
//				 }
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
		 else
			 Debug.Log("Got an empty message from the web socket");
	 }

	 public void Render()
	 {
//		 RenderTask newTask = null;
//		 lock (_queueLock)
//		 {
//			 if (_taskQ.Count > 0)
//			 {
//				 newTask = _taskQ[0];
//				 _taskQ.RemoveAt(0);
//			 }
//		 }
//
//		 if (newTask != null)
//		 {
//			 Update(newTask.getSubscriber(), newTask.getMsg());
//		 }

		 foreach (var sub in subscribers)
		 {
			 GetSubscriber(sub.Key)?.ProcessMsg();
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
			 Debug.Log("Sending " + s);
			 _ws.Send(s);
		 }
	 }
	 }
 }
