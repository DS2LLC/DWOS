using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using DWOS.Portal.Models;

namespace DWOS.Portal.Filters
{
    /// <summary>
    /// Requires authentication for some Web API routes.
    /// </summary>
    public class DwosAuthorizeAttribute : Attribute, IAuthenticationFilter
    {
        #region Properties

        public bool AllowMultiple => true;

        #endregion

        #region Methods

        public static async Task<LoginInfo> LoginInfoFromAsync(string param)
        {
            var base64Bytes = Convert.FromBase64String(param);

            var decodedParam = Encoding.ASCII.GetString(base64Bytes);
            var authParameters = decodedParam.Split(':');

            if (authParameters.Length >= 2)
            {
                var info = new LoginInfo
                {
                    UserName = authParameters[0],
                    Password = authParameters[1]
                };

                if (await DataAccess.IsPasswordCorrect(info.UserName, info.Password))
                {
                    return info;
                }

                return null;
            }

            return null;
        }

        #endregion

        #region IAuthenticationFilter Members

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var req = context.Request;
            var auth = req.Headers.Authorization;

            if (auth == null || auth.Scheme != "Basic")
            {
                context.ErrorResult = new AuthenticationFailureResult(req);
                return;
            }

            // Check for presence of credentials
            if (string.IsNullOrEmpty(auth.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult(req);
                return;
            }

            // Check credential format
            var loginInfo = await LoginInfoFromAsync(auth.Parameter);

            if (loginInfo == null)
            {
                context.ErrorResult = new AuthenticationFailureResult(req);
            }
            else
            {
                context.Principal = new LoginPrincipal(new LoginIdentity(loginInfo));
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        #endregion

        #region AuthenticationFailureResult

        private class AuthenticationFailureResult : IHttpActionResult
        {
            #region Properties

            private HttpRequestMessage Request { get; }

            #endregion

            #region Methods

            public AuthenticationFailureResult(HttpRequestMessage request)
            {
                Request = request;
            }

            #endregion

            #region IHttpActionResult Members

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    RequestMessage = Request,
                    ReasonPhrase = "Unauthorized"
                };

                return Task.FromResult(response);
            }

            #endregion
        }

        #endregion

        #region LoginPrincipal

        private class LoginPrincipal : IPrincipal
        {
            #region Methods

            public LoginPrincipal(IIdentity identity)
            {
                Identity = identity;
            }

            #endregion

            #region IPrincipalMembers

            public IIdentity Identity { get; }

            public bool IsInRole(string role) => true;

            #endregion
        }

        #endregion

        #region LoginIdentity

        public class LoginIdentity : IIdentity
        {
            #region Properties

            public LoginInfo Login { get; }

            #endregion

            #region Methods

            public LoginIdentity(LoginInfo login)
            {
                Login = login;
            }

            #endregion

            #region IIdentity Members

            public string AuthenticationType => "Basic";

            public bool IsAuthenticated => true;

            public string Name => Login.UserName;

            #endregion
        }

        #endregion

        #region LoginInfo

        public class LoginInfo
        {
            #region Properties

            public string UserName { get; set; }

            public string Password { get; set; }

            #endregion

            #region Methods

            public static async Task<LoginInfo> FromAsync(string param)
            {
                var base64Bytes = Convert.FromBase64String(param);

                var decodedParam = Encoding.ASCII.GetString(base64Bytes);
                var authParameters = decodedParam.Split(':');

                if (authParameters.Length >= 2)
                {
                    var info = new LoginInfo
                    {
                        UserName = authParameters[0],
                        Password = authParameters[1]
                    };

                    if (await DataAccess.IsPasswordCorrect(info.UserName, info.Password))
                    {
                        return info;
                    }

                    return null;
                }

                return null;
            }

            #endregion
        }

        #endregion
    }
}