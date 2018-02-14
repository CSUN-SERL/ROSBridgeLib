using SimpleJSON;

namespace ROSBridgeLib.Core
{
    public delegate void ServiceResponseCallback<in T>(T response) where T : IServiceResponse;
   
    public interface IServiceRequest
    {
        /// <summary>
        /// Must serialize as a list of variables.
        /// <para/> https://github.com/RobotWebTools/rosbridge_suite/blob/master/ROSBRIDGE_PROTOCOL.md
        /// </summary>
        /// <returns>a json formatted list of this request's variables</returns>
        string ToJSONList();
    }
    
    public interface IServiceResponse
    {
        /// <summary>
        /// Deserialize the message to the values implemented by this response.
        /// </summary>
        /// <param name="msg"></param>
        void Deserialize(JSONNode msg);
    }
}