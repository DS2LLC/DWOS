using System;
using System.Collections.Generic;
using System.Web.Http;
using DWOS.Portal.Models;
using DWOS.Shared;
using Mindscape.Raygun4Net.Builders;

namespace DWOS.Portal.Controllers
{
    /// <summary>
    /// Web API controller that logs client-side errors.
    /// </summary>
    public class ErrorController : ApiController
    {
        [HttpPost]
        public IHttpActionResult LogError(ClientError error)
        {
            var stackTrace = error?.StackTrace;
            var userAgent = error?.UserAgent;
            if (stackTrace == null || stackTrace.Count == 0)
            {
                return Ok();
            }

            var msg = new Mindscape.Raygun4Net.Messages.RaygunMessage
            {
                Details =
                {
                    MachineName = Environment.MachineName,
                    Version = About.ApplicationVersion,
                    Environment = RaygunEnvironmentMessageBuilder.Build(),
                    UserCustomData = new Dictionary<string, object>
                    {
                        {"UserName", Environment.UserName},
                        {"Error Time", DateTime.Now.ToUniversalTime()},
                        {"CLR Version", Environment.Version}
                    }
                }
            };

            //Exception
            msg.Details.Error = RaygunErrorMessageBuilder.Build(new ClientException());
            msg.Details.UserCustomData = new Dictionary<string, object>
            {
                { "Stack", stackTrace},
                { "UserAgent", userAgent}
            };

#if !DEBUG
            RaygunClientProvider.Raygun.SendInBackground(msg);
#endif

            return Ok();
        }

        class ClientException : Exception
        {
            public ClientException() : base("A client error has occurred.")
            {
            }
        }
    }
}
