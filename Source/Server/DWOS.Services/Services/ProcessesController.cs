using DWOS.Services.Messages;
using System;
using System.Web.Http;

namespace DWOS.Services
{
    public class ProcessesController : ApiController
    {
        #region Methods

        [HttpGet]
        [ServiceExceptionFilter("Error getting processes.")]
        public ResponseBase Get(int processId)
        {
            return new ProcessResponse { Success = true, ErrorMessage = null, Process = ServiceUtilities.CreateInfoForProcess(processId) };
        }

        #endregion
    }
}