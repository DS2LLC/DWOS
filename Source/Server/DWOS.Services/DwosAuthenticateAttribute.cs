using DWOS.Services.Messages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace DWOS.Services
{
    /// <summary>
    /// Attribute for routes that require basic user id/PIN authentication.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class DwosAuthenticateAttribute : Attribute, IAuthenticationFilter
    {
        #region IAuthenticationFilter Members

        public bool AllowMultiple => false;

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
            var loginInfo = await LoginInfo.FromAsync(auth.Parameter);

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
                var response = Request.CreateResponse(HttpStatusCode.Unauthorized, ResponseBase.Error("PIN is incorrect."));
                return Task.FromResult(response);
            }

            #endregion
        }

        #endregion

        #region LoginPrincipal

        private class LoginPrincipal : IPrincipal
        {
            #region Fields

            private readonly Lazy<ISet<string>> _userRoles;

            #endregion

            #region Properties

            public LoginIdentity LoginIdentity { get; }

            #endregion

            #region Methods

            public LoginPrincipal(LoginIdentity identity)
            {
                LoginIdentity = identity;
                _userRoles = new Lazy<ISet<string>>(GetUserRoles);
            }

            private ISet<string> GetUserRoles()
            {
                var userRoles = new HashSet<string>();
                using (var taUserRoles = new Data.Datasets.SecurityDataSetTableAdapters.User_SecurityRolesTableAdapter())
                {
                    var userSecurityRoles = new Data.Datasets.SecurityDataSet.User_SecurityRolesDataTable();
                    taUserRoles.FillAllByUser(userSecurityRoles, LoginIdentity.Login.UserId);

                    foreach (var securityRole in userSecurityRoles)
                    {
                        userRoles.Add(securityRole.SecurityRoleID);
                    }
                }

                return userRoles;
            }

            #endregion

            #region IPrincipalMembers

            public IIdentity Identity => LoginIdentity;

            public bool IsInRole(string role) => _userRoles.Value.Contains(role);

            #endregion
        }

        #endregion

        #region LoginIdentity

        private class LoginIdentity : IIdentity
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

            public string Name => Login.UserId.ToString();

            #endregion
        }

        #endregion

        #region LoginInfo

        private class LoginInfo
        {
            #region Properties

            public int UserId { get; private set; }

            private string Pin { get; set; }

            #endregion

            #region Methods

            public static Task<LoginInfo> FromAsync(string param)
            {
                return Task.Run(() => GetLoginInfo(param));
            }

            private static LoginInfo GetLoginInfo(string param)
            {
                var base64Bytes = Convert.FromBase64String(param);
                var decodedParam = Encoding.ASCII.GetString(base64Bytes);
                var authParameters = decodedParam.Split(':');

                if (authParameters.Length >= 2 && int.TryParse(authParameters[0], out var userId))
                {
                    var info = new LoginInfo
                    {
                        UserId = userId,
                        Pin = authParameters[1]
                    };

                    int actualUserId;
                    using (var taOrders = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.UsersTableAdapter())
                    {
                        actualUserId = taOrders.GetUserIdByUserLoginPin(info.Pin) ?? -1;
                    }

                    if (actualUserId == info.UserId)
                    {
                        return info;
                    }
                }

                return null;
            }

            #endregion
        }

        #endregion
    }
}
