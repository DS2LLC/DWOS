using DWOS.Services.Messages;
using NLog;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;

namespace DWOS.Services
{
    /// <summary>
    /// Represents the attributes for a custom exception filter that logs
    /// errors using NLog.
    /// </summary>
    internal class ServiceExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private static readonly ILogger LOGGER = LogManager.GetCurrentClassLogger();

        public ServiceExceptionFilterAttribute(string errorText)
        {
            ErrorText = errorText;
        }

        public string ErrorText { get; }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is HttpResponseException httpException)
            {
                LOGGER.Error(httpException, $"{ErrorText} - {httpException.Response?.StatusCode}");
            }
            else if (actionExecutedContext.Exception != null)
            {
                LOGGER.Error(actionExecutedContext.Exception, ErrorText);
            }

            actionExecutedContext.Response = actionExecutedContext.Request
                .CreateResponse(HttpStatusCode.OK, ResponseBase.Error(ErrorText));
        }
    }
}
