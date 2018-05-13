using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Threading.Tasks;
using TextProcessor.Infrastructure.Helpers;
using TextProcessor.Resource;
using TextProcessor.Web.Models.Response;
using TextProcessor.Web.Models.Response.Exceptions;

namespace TextProcessor.Web.Infrastructure.Middleware
{
    /// <summary>
    /// Error handling middleware
    /// Credits: https://github.com/sulhome/log-request-response-middleware
    /// </summary>
    public class ErrorWrappingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorWrappingMiddleware> _logger;
        private readonly IHostingEnvironment _env;

        public ErrorWrappingMiddleware(RequestDelegate next, ILogger<ErrorWrappingMiddleware> logger, IHostingEnvironment env)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            Exception exception = null;
            string jsonResponse = string.Empty;
            try
            {
                await _next.Invoke(context);
            }
            catch (BadRequestException badReqEx)
            {
                HandleException(Const.BadRequestException, badReqEx, badReqEx.StatusCode, badReqEx.Message);
            }
            catch (NotFoundException notFoundEx)
            {
                HandleException(Const.NotFoundException, notFoundEx, notFoundEx.StatusCode, notFoundEx.Message);
            }
            catch (UnauthorizedException authEx)
            {
                HandleException(Const.NotFoundException, authEx, authEx.StatusCode, authEx.Message);
            }
            catch (ApiException apiEx)
            {
                HandleException(Const.UnhandledException, apiEx, apiEx.StatusCode, apiEx.Message);
            }
            catch (Exception ex)
            {
                HandleException(Const.UnhandledException, ex, HttpStatusCode.InternalServerError, Global.InternalServerError);
            }

            void HandleException(EventId eventId, Exception ex, HttpStatusCode httpStatusCode, string message)
            {
                exception = ex;
                _logger.LogError(eventId, ex, ex.Message);
                context.Response.StatusCode = (int)httpStatusCode;
                context.Response.ContentType = "application/json";
                ApiResponse response;
                //Log stack trace
                if (!_env.IsProduction())
                {
                    response = new JsonErrorResponse(httpStatusCode, exception.Message, ex);
                }
                else
                {
                    response = new JsonErrorResponse(httpStatusCode, exception.Message);
                }

                jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }

            if (!context.Response.HasStarted && exception != null)
            {
                await context.Response.WriteAsync(jsonResponse);
            }
        }

        private class JsonErrorResponse : ApiResponse
        {
            public JsonErrorResponse(HttpStatusCode statusCode, string message = null, object DeveloperMessage = null)
                : base(statusCode, message)
            {
            }
            public object DeveloperMessage { get; set; }
        }
    }

}
