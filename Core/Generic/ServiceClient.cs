using WebSocketSharp;

namespace ROSBridgeLib.Core.Generic
{
    public class ServiceClient<TRequest, TResponse> : ServiceClient
        where TRequest : IServiceRequest 
        where TResponse : IServiceResponse
    {
        public new event ServiceResponseCallback<TResponse> ResponseReceived;

        public ServiceClient(WebSocket socket, string service)
            : base(socket, service, typeof(TResponse))
        { }

        protected override void ProcessResponse(IServiceResponse response)
        {
            base.ProcessResponse(response);
            ResponseReceived?.Invoke((TResponse)response);
        }

        public void CallService(TRequest request)
        {
            base.CallService(request);
        }

        public void AddServiceResponse(ServiceResponseCallback<TResponse> callback)
        {
            ResponseReceived += callback;
        }

        public void RemoveServiceResponse(ServiceResponseCallback<TResponse> callback)
        {
            ResponseReceived -= callback;
        }
    }
}
