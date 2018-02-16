using System;
using System.CodeDom;
using System.Collections.Generic;
using ROSBridgeLib.Core.Generic;
using SimpleJSON;
using WebSocketSharp;

namespace ROSBridgeLib.Core
{
    public class ServiceManager
    {
        private WebSocket socket;
        private Dictionary<string, Tuple<Type, Type>> allServices;
        private Dictionary<string, ServiceClient> serviceClients;

        public ServiceManager(WebSocket socket)
        {
            this.socket = socket;
            
            allServices = new Dictionary<string, Tuple<Type, Type>>();
            serviceClients = new Dictionary<string, ServiceClient>();

            this.socket.OnMessage += OnMessage;
        }

        public void CallService(string service, string args)
        {
            string request = ROSBridgeMessage.CallService(service, args);
            System.Diagnostics.Debug.Print($"Sending: {request}");
            socket.Send(request);
        }
        
        public ServiceClient GetServiceClient(string service)
        {
            ServiceClient client;
            return serviceClients.TryGetValue(service, out client) ? client : null;
        }

        public ServiceClient<TRequest, TResponse> GetServiceClient <TRequest, TResponse>(string service)
            where TRequest : IServiceRequest where TResponse : IServiceResponse
        {
            var pair = new Tuple<Type,Type>(typeof(TRequest), typeof(TResponse));
            ThrowIfServiceExistsUnderDifferentType(service, pair);
            return (ServiceClient<TRequest, TResponse>) GetServiceClient(service);
        }

        public ServiceClient<TRequest, TResponse> AddServiceClient<TRequest, TResponse>(string service, ServiceResponseCallback<TResponse> callback = null) 
            where TRequest : IServiceRequest where TResponse : IServiceResponse, new()
        {
            var pair = new Tuple<Type, Type>(typeof(TRequest), typeof(TResponse));
            ThrowIfServiceExistsUnderDifferentType(service, pair);
            
            ServiceClient serviceClient;
            if (!serviceClients.TryGetValue(service, out serviceClient))
            {
                serviceClient = new ServiceClient<TRequest,TResponse>(socket, service);
                serviceClients.Add(service, serviceClient);
                CacheService(service, pair);
            }
            var client = (ServiceClient<TRequest, TResponse>)serviceClient;

            if (callback != null)
            {
                client.AddServiceResponse(callback);
            }

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