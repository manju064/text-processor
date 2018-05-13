using System.Net;
using TextProcessor.Resource;

namespace TextProcessor.Web.Models.Response
{
    public class ApiOkResponse<T> : ApiResponse
    {
        public T Data { get; }

        public ApiOkResponse(T data)
            : base(HttpStatusCode.OK, Global.SuccessMessage)
        {
            Data = data;
        }
    }
}
