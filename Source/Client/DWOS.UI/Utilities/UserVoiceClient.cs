using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Linq;
using System.Net;
using System.Web;
using DWOS.UI.Support;

namespace DWOS.UI.UserVoice
{
    public class UserVoiceClient : ISupportTicketClient
    {
        #region Fields

        private string _apiKey = Properties.Settings.Default.UserVoiceAPIKey;
        private string _apiSecret = Properties.Settings.Default.UserVoiceAPISecret;
        private string _subdomain = Properties.Settings.Default.UserVoiceSubDomain;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Adds the ticket.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        public void AddTicket(Ticket ticket)
        {
            ThreadPool.QueueUserWorkItem((t) => BeginAddTicket((Ticket)t), ticket);
        }

        private void BeginAddTicket(Ticket ticket)
        {
            var client = new Client(_subdomain, _apiKey, _apiSecret);
            client.LoginAs(ticket.FromAddress);
            var question = client.Post("/api/v1/tickets.json", new
            {
                name = ticket.UserName,
                email = ticket.FromAddress,
                ticket = new { state = "open", subject = ticket.Subject, message = ticket.Message }
            })["ticket"];
        }

        #endregion Methods

        private class Client
        {
            #region Fields

            public const string CLIENT_VERSION = "0.0.3";
            private RestClient _consumer;
            private RestClient _accessToken;
            private string _callback;
            private string _apiKey;
            private string _apiSecret;
            private string _uservoiceDomain;
            private string _protocol;
            private string _subdomainName;
            public string Token;
            public string Secret;

            #endregion Fields

            #region Methods

            /// <summary>
            /// Initializes a new instance of the <see cref="Client"/> class.
            /// </summary>
            /// <param name="subdomainName">Name of the subdomain.</param>
            /// <param name="apiKey">The API key.</param>
            /// <param name="apiSecret">The API secret.</param>
            /// <param name="callback">The callback.</param>
            /// <param name="token">The token.</param>
            /// <param name="secret">The secret.</param>
            /// <param name="uservoiceDomain">The uservoice domain.</param>
            /// <param name="protocol">The protocol.</param>
            public Client(string subdomainName, string apiKey, string apiSecret = null, string callback = null, string token = null, string secret = null, string uservoiceDomain = null, string protocol = null)
            {
                this._protocol = protocol ?? "https";
                this._uservoiceDomain = uservoiceDomain ?? "uservoice.com";
                this._apiKey = apiKey;
                this._apiSecret = apiSecret;
                this._subdomainName = subdomainName;
                this._callback = callback;
                this.Token = token;
                this.Secret = secret;
                _consumer = new RestClient(this._protocol + "://" + this._subdomainName + "." + this._uservoiceDomain);
                if(apiSecret != null)
                {
                    _consumer.Authenticator = OAuth1Authenticator.ForRequestToken(apiKey, apiSecret, callback);
                }
                if(token != null && secret != null)
                {
                    _accessToken = new RestClient(this._protocol + "://" + this._subdomainName + "." + this._uservoiceDomain);
                    _accessToken.Authenticator = OAuth1Authenticator.ForAccessToken(apiKey, apiSecret, token, secret);
                }
            }

            /// <summary>
            /// Gets the token.
            /// </summary>
            /// <returns></returns>
            private RestClient GetToken()
            {
                if(_accessToken != null)
                {
                    return _accessToken;
                }
                return _consumer;
            }

            /// <summary>
            /// Requests the specified method.
            /// </summary>
            /// <param name="method">The method.</param>
            /// <param name="path">The path.</param>
            /// <param name="body">The body.</param>
            /// <returns></returns>
            /// <exception cref="ApplicationError">
            /// Invalid JSON received:  + response.Content
            /// or
            /// </exception>
            /// <exception cref="Unauthorized"></exception>
            /// <exception cref="NotFound"></exception>
            /// <exception cref="ApiError"></exception>
            public JToken Request(Method method, string path, Object body = null)
            {
                //Console.WriteLine(method + " " + path + "\n" + body);
                var request = new RestRequest(path.Split('?').First(), method);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("API-Client", string.Format("uservoice-csharp-{0}", CLIENT_VERSION));
                if(body != null)
                {
                    //Console.WriteLine("BODY PARAMETER " + body.ToString());
                    request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
                }
                if(body == null)
                {
                    var queryString = string.Join(string.Empty, path.Split('?').Skip(1));
                    var getParams = HttpUtility.ParseQueryString(queryString);
                    if(null != getParams)
                    {
                        foreach(string k in getParams.AllKeys)
                        {
                            //Console.WriteLine("GET PARAMTER " + k + "=" + getParams[k]);
                            request.AddParameter(k, getParams[k], ParameterType.GetOrPost);
                        }
                    }
                }
                if(_apiSecret == null)
                {
                    request.AddParameter("client", _apiKey, ParameterType.GetOrPost);
                }
                var response = GetToken().Execute(request);

                JToken result = null;
                try
                {
                    if(response.ContentType.StartsWith("application/json"))
                    {
                        result = JObject.Parse(response.Content);
                    }
                    else
                    {
                        result = new JObject();
                        var values = HttpUtility.ParseQueryString(response.Content);
                        if(null != values.AllKeys)
                        {
                            foreach(String k in values.AllKeys)
                            {
                                if(null != k)
                                {
                                    result[k] = values[k];
                                }
                            }
                        }
                    }
                }
                catch(Newtonsoft.Json.JsonReaderException)
                {
                    throw new ApplicationError("Invalid JSON received: " + response.Content);
                }

                if(!HttpStatusCode.OK.Equals(response.StatusCode))
                {
                    string msg = response.Content;

                    if(result != null && null != result["errors"] && null != result["errors"]["message"])
                    {
                        msg = result["errors"].Value<string>("message");
                    }
                    switch((int)response.StatusCode)
                    {
                        case 401: throw new Unauthorized(msg);
                        case 404: throw new NotFound(msg);
                        case 500: throw new ApplicationError(msg);
                        default: throw new ApiError(msg);
                    }
                }
                return result;
            }

            /// <summary>
            /// Logins as.
            /// </summary>
            /// <param name="email">The email.</param>
            /// <returns>Client object</returns>
            public Client LoginAs(string email)
            {
                var parameters = new JObject();
                parameters["user"] = new JObject();
                parameters["user"]["email"] = email;
                parameters["request_token"] = RequestToken().Token;
                var result = Request(Method.POST, "/api/v1/users/login_as", parameters);
                return LoginWithAccessToken((string)result["token"]["oauth_token"], (string)result["token"]["oauth_token_secret"]);
            }

            /// <summary>
            /// Logins the with access token.
            /// </summary>
            /// <param name="token">The token.</param>
            /// <param name="secret">The secret.</param>
            /// <returns>Client object</returns>
            public Client LoginWithAccessToken(string token, string secret)
            {
                return new Client(_subdomainName, _apiKey, _apiSecret, _callback, token, secret, _uservoiceDomain, _protocol);
            }

            /// <summary>
            /// Requests the token.
            /// </summary>
            /// <param name="callback">The callback.</param>
            /// <returns>Client object</returns>
            /// <exception cref="Unauthorized">Failed to get request token</exception>
            public Client RequestToken(string callback = null)
            {
                var request = new RestRequest("/oauth/request_token", Method.POST);
                var response = _consumer.Execute(request);
                var result = HttpUtility.ParseQueryString(response.Content);

                if(null == result || null == result["oauth_token"] || null == result["oauth_token_secret"])
                {
                    throw new Unauthorized("Failed to get request token");
                }
                return LoginWithAccessToken((string)result["oauth_token"], (string)result["oauth_token_secret"]);
            }

            /// <summary>
            /// Gets the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>JToken</returns>
            public JToken Get(string path) { return Request(Method.GET, path); }

            /// <summary>
            /// Deletes the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>JToken</returns>
            public JToken Delete(string path) { return Request(Method.DELETE, path); }

            /// <summary>
            /// Posts the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="parameters">The parameters.</param>
            /// <returns>JToken</returns>
            public JToken Post(string path, Object parameters) { return Request(Method.POST, path, parameters); }

            /// <summary>
            /// Puts the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="parameters">The parameters.</param>
            /// <returns>JToken</returns>
            public JToken Put(string path, Object parameters) { return Request(Method.PUT, path, parameters); }

            #endregion Methods
        }
    }

    public class ApiError : Exception { public ApiError(string msg) : base(msg) { } }
    public class Unauthorized : ApiError { public Unauthorized(string msg) : base(msg) { } }
    public class NotFound : ApiError { public NotFound(string msg) : base(msg) { } }
    public class ApplicationError : ApiError { public ApplicationError(string msg) : base(msg) { } }
}
