using System;
using System.CodeDom;
using System.Collections.Generic;
using SimpleJSON;
using WebSocketSharp;

namespace ROSBridgeLib.Core
{
    public class ServiceManager
    {
        private WebSocket socket;
        private Dictionary<string, Tuple<Type, Type>> allServices;
        private Dictionary<string, ROSBridgeServiceClient> serviceClients;

        public ServiceManager(WebSocket socket)
        {
            this.socket = socket;
            
            allServices = new Dictionary<string, Tuple<Type, Type>>();
            serviceClients = new Dictionary<string, ROSBridgeServiceClient>();

            this.socket.OnMessage += OnMessage;
        }

        public void CallService(string service, string args)
        {
            string request = ROSBridgeMsg.CallService(service, args);
            System.Diagnostics.Debug.Print($"Sending: {request}");
            socket.Send(request);
        }
        
        public ROSBridgeServiceClient GetServiceClient(string service)
        {
            ROSBridgeServiceClient client;
            return serviceClients.TryGetValue(service, out client) ? client : null;
        }

        public ROSBridgeServiceClient<T_REQ, U_RES> GetServiceClient <T_REQ, U_RES>(string service)
            where T_REQ : IServiceRequest where U_RES : IServiceResponse
        {
            var pair = new Tuple<Type,Type>(typeof(T_REQ), typeof(U_RES));
            ThrowIfServiceExistsUnderDifferentType(service, pair);
            return (ROSBridgeServiceClient<T_REQ, U_RES>) GetServiceClient(service);
        }

        public ROSBridgeServiceClient<T_REQ, U_RES> AddServiceClient<T_REQ, U_RES>(string service, ServiceResponseCallback<U_RES> callback) 
            where T_REQ : IServiceRequest where U_RES : IServiceResponse, new()
        {
            var pair = new Tuple<Type, Type>(typeof(T_REQ), typeof(U_RES));
            ThrowIfServiceExistsUnderDifferentType(service, pair);
            
            ROSBridgeServiceClient serviceClient;
            if (!serviceClients.TryGetValue(service, out serviceClient))
            {
                serviceClient = new ROSBridgeServiceClient<T_REQ,U_RES>(socket, service);
                serviceClients.Add(service, serviceClient);
                CacheService(service, pair);
            }

            var client = (ROSBridgeServiceClient<T_REQ, U_RES>) serviceClient;
            client.AddServiceResponse(callback);

            return client;
        }

        private void ThrowIfServiceExistsUnderDifferentType(string service, Tuple<Type, Type> requestResponsePair)
        {
           Tuple<Type, Type> existingPair;
            if (allServices.TryGetValue(service, out existingPair))
            {
                if (!existingPair.Equals(requestResponsePair))
                {
                    throw new Exception($"Service already exists under Request Response pair: {existingPair.Item1}, {existingPair.Item2}");
                }
            }
        }

        private void CacheService(string service, Tuple<Type, Type> requestResponsePair)
        {
            if (!allServices.ContainsKey(service))
            {
                allServices.Add(service, requestResponsePair);
            }
        }
        
        private void OnMessage(object sender, MessageEventArgs args)
        {
            string s = args.Data;
            if (string.IsNullOrEmpty(s))
                return;
            
            JSONNode node = JSONNode.Parse(s);

            string op = node["op"];
            if (op == "service_response")
            {
               GetServiceClient(node["service"])?.ProcessRawResponse(node["values"]);
            }
        }

    }
}