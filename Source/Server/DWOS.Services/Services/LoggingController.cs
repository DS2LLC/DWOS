using DWOS.Services.Messages;
using System.Web.Http;

namespace DWOS.Services
{
    public class LoggingController : ApiController
    {
        [HttpPost]
        [ServiceExceptionFilter("Error logging message.")]
        public ResponseBase Add(LogMessageRequest request)
        {
            Data.Datasets.UserLogging.AddAnalytics(request.Message);
            return new ResponseBase() { ErrorMessage = string.Empty, Success = true };
        }
    }
}
