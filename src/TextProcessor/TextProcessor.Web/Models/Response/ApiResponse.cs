using System.Net;
using Newtonsoft.Json;
using TextProcessor.Resource;

namespace TextProcessor.Web.Models.Response
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; }

        public ApiResponse(HttpStatusCode statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }
       
        private static string GetDefaultMessageForStatusCode(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.OK:
                    return Global.SuccessMessage;
                case HttpStatusCode.NotFound:
                    return Global.NotFound;
                case HttpStatusCode.BadRequest:
                    return Global.BadRequest;
                case HttpStatusCode.InternalServerError:
                    return Global.InternalServerError;
                case HttpStatusCode.Unauthorized:
                    return Global.Unauthorized;
                default:
                    return null;
            }
        }
    }
}
