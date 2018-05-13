using System;
using System.Net;
using System.Runtime.Serialization;

namespace TextProcessor.Web.Models.Response.Exceptions
{
    [Serializable]
    public class ApiException : Exception
    {
        public ApiException(HttpStatusCode statusCode, string errorCode, string errorDescription)
            : base($"{errorCode}::{errorDescription}")
        {
            StatusCode = statusCode;
        }

        public ApiException(ApiException apiException): base(apiException.Message)
        {
            this.StatusCode = apiException.StatusCode;
        }

        public ApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ApiException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
        public HttpStatusCode StatusCode { get; private set; }

        public void SetStatus(HttpStatusCode code)
        {
            this.StatusCode = code;
        }
    }
}
