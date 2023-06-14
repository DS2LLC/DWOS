using DWOS.Services.Messages;
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace DWOS.Services
{
    /// <summary>
    /// Attribute for routes that require the current user to have one or
    /// more <see cref="AuthorizeAttribute.Roles"/>.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="AuthorizeAttribute"/> to provide a custom
    /// response on errors.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class DwosAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);

            if (actionContext.Response == null)
            {
                // Authorization passed
                return;
            }

            if (!actionContext.Response.IsSuccessStatusCode)
            {
                // Provide custom response for the authorization error.
                actionContext.Response = actionContext.Request
                    .CreateResponse(actionContext.Response.StatusCode, ResponseBase.Error("User does not have the correct permission."));
            }
        }
    }
}
