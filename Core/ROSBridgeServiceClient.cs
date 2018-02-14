using System;
using SimpleJSON;
using UnityEngine;
using WebSocketSharp;

namespace ROSBridgeLib.Core
{
    public class ROSBridgeServiceClient
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

        public ROSBridgeServiceClient(WebSocket socket, string service, Type serviceResponseType)
        {
            this.socket = socket;
            this.service = service;
            this.serviceResponseType = serviceResponseType;
        }

        public void CallService(IServiceRequest request)
        {
            string req = ROSBridgeMsg.CallService(service, request.ToJSONList());
            System.Diagnostics.Debug.Print($"Sending: {req}");
            socket.Send(req);
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
    
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public class ROSBridgeServiceClient<T_REQ, U_RES> : ROSBridgeServiceClient
        where T_REQ : IServiceRequest where U_RES : IServiceResponse
    {
        public new event ServiceResponseCallback<U_RES> ResponseReceived;

        public ROSBridgeServiceClient(WebSocket socket, string service)
            : base(socket, service, typeof(U_RES))
        { }
        
        protected override void ProcessResponse(IServiceResponse response)
        {
            base.ProcessResponse(response);
            ResponseReceived?.Invoke((U_RES)response);
        }

        public void CallService(T_REQ request)
        {
            base.CallService(request);
        }

        public void AddServiceResponse(ServiceResponseCallback<U_RES> callback)
        {
            ResponseReceived += callback;
        }

        public void RemoveServiceResponse(ServiceResponseCallback<U_RES> callback)
        {
            ResponseReceived -= callback;
        }
    }
}