using System;
using SimpleJSON;
using WebSocketSharp;

namespace ROSBridgeLib.Core
{
    public class ServiceClient
    {
        protected WebSocket socket;
        protected string service;
        protected Type serviceResponseType;

        public event ServiceResponseCallback<IServiceResponse> ResponseReceived; 
        
        public string Service
        {
            get { return service; }
        }

        public Type ServiceResponseType
        {
            get { return serviceResponseType; }
        }

        public ServiceClient(WebSocket socket, string service, Type serviceResponseType)
        {
            this.socket = socket;
            this.service = service;
            this.serviceResponseType = serviceResponseType;
        }

        public ServiceClient CallService(IServiceRequest request)
        {
            string req = ROSBridgeMessage.CallService(service, request.ToJSONList());
            System.Diagnostics.Debug.Print($"Sending: {req}");
            socket.Send(req);
            return this;
        }
        
        public void ProcessRawResponse(JSONNode rawResponse)
        {
            IServiceResponse response = (IServiceResponse)Activator.CreateInstance(serviceResponseType);
            response.Deserialize(rawResponse);
            ProcessResponse(response);
        }

        protected virtual void ProcessResponse(IServiceResponse response)
        {
            ResponseReceived?.Invoke(response);
        }
    }
}