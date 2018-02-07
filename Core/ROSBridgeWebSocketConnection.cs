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

	 private static Dictionary<string, Type> allTopics;
	 private static Dictionary<string, List<object/*ROSBridgeSubscriber<IMsg>*/>> subscribers;
	 private static Dictionary<string, Type> advertisements;

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
		 _myThread = null;
		 _subscribers = new List<Type>();
		 _publishers = new List<Type>();
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

	 public ROSBridgeSubscriber<T> Subscribe<T>(string topic, object callbackOwner, ROSCallback<T> callback) where T : IMsg
	 {
		 Type messageType = typeof(T);
		 ThrowIfTopicExistsUnderDifferentType(topic, messageType, "subscribe");

		 List<object> list;
		 if (!subscribers.TryGetValue(topic, out list))
		 {
			 list = new List<object>();
			 subscribers.Add(topic, list);
			 CacheTopic(topic, messageType);
		 }

		 var subscriber = new ROSBridgeSubscriber<T>(topic, callbackOwner, callback);
		 int count = list.Count;
		 for (int i = 0; i < count; ++i)
		 {
			 if (list[i].Equals(subscriber))
			 {
				 return (ROSBridgeSubscriber<T>)list[i];
			 }
		 }

		 list.Add(subscriber);
		 return subscriber;
	 }


	 private Action<IMsg> ConvertAction<T>(Action<T> myActionT) where T : IMsg
	 {
		 return myActionT == null ? null : new Action<IMsg>(o => myActionT((T)o));
	 }

	 /**
	  * Connect to the remote ros environment.
	  */
	 public void Connect()
	 {
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

	 private void SendAllSubscriptions()
	 {
		 var subEnum = subscribers.GetEnumerator();
		 while(subEnum.MoveNext())
		 {
			 var sub = (ROSBridgeSubscriber<IMsg>) subEnum.Current.Value[0];
			 IMsg t = (IMsg)Activator.CreateInstance(sub.MessageType);
			 //_ws.Send(ROSBridgeMsg.Subscribe(GetMessageTopic(p), GetMessageType(p)));
			 string subscribeOp = ROSBridgeMsg.Subscribe(subEnum.Current.Key, t.ROSMessageType);
			 _ws.Send(subscribeOp);
			 //Debug.Log("Sending " + ROSBridgeMsg.Subscribe(GetMessageTopic(p), GetMessageType(p)));
			 Debug.Log($"Sending {subscribeOp}");
		 }
		 subEnum.Dispose();
	 }

	 private void SendAllAdvertisements()
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
	 
	 private void Run()
	 {
		 _ws = new WebSocket(_host + ":" + _port);
		 _ws.OnMessage += (sender, e) => this.OnMessage(e.Data);
		 _ws.Connect();

		 SendAllSubscriptions();
		 SendAllAdvertisements();
		 
		 while (true)
		 {
			 Thread.Sleep(1000);
		 }
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
			 if ("publish".Equals(op))
			 {
				 string topic = node["topic"];
				 //Debug.Log ("Got a message on " + topic);
				 foreach (Type p in _subscribers)
				 {
					 if (topic.Equals(GetMessageTopic(p)))
					 {
						 //Debug.Log ("And will parse it " + GetMessageTopic (p));
						 ROSBridgeMsg msg = ParseMessage(p, node["msg"]);
						 RenderTask newTask = new RenderTask(p, topic, msg);
						 lock (_queueLock)
						 {
							 bool found = false;
							 for (int i = 0; i < _taskQ.Count; i++)
							 {
								 if (_taskQ[i].getTopic().Equals(topic))
								 {
									 _taskQ.RemoveAt(i);
									 _taskQ.Insert(i, newTask);
									 found = true;
									 break;
								 }
							 }

							 if (!found)
								 _taskQ.Add(newTask);
						 }

					 }
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
		 else
			 Debug.Log("Got an empty message from the web socket");
	 }

	 public void Render()
	 {
		 RenderTask newTask = null;
		 lock (_queueLock)
		 {
			 if (_taskQ.Count > 0)
			 {
				 newTask = _taskQ[0];
				 _taskQ.RemoveAt(0);
			 }
		 }

		 if (newTask != null)
			 Update(newTask.getSubscriber(), newTask.getMsg());

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
