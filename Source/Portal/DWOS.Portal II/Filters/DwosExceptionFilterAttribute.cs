using System.Web.Http.Filters;
using NLog;

namespace DWOS.Portal.Filters
{
    /// <summary>
    /// Logs Web API exceptions.
    /// </summary>
    public class DwosExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext?.Exception;
            if (exception == null)
            {
                return;
            }

            var loggerName = exception.TargetSite?.DeclaringType?.FullName ??
                             typeof(DwosExceptionFilterAttribute).FullName;

            LogManager.GetLogger(loggerName).Error(exception);
        }
    }
}