using Microsoft.AspNet.WebApi.Extensions.Compression.Server;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Owin;
using System;
using System.Net.Http.Extensions.Compression.Core.Compressors;
using System.Web.Http;

namespace DWOS.Services
{
    /// <summary>
    /// Startup class for the mobile API application.
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            // Route configuration
            config.Routes.MapHttpRoute(
                name: "Api",
                routeTemplate: "dwos/{controller}/{action}",
                defaults: new { action = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "Root",
                routeTemplate: "dwos",
                defaults: new { controller = "Index" }
            );

            // Json serialization configuration
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings()
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            // GZIP & Deflate responses when possible
            config.MessageHandlers.Insert(0, new ServerCompressionHandler(new GZipCompressor(), new DeflateCompressor()));

            // Log requests
            config.MessageHandlers.Add(new RequestLogHandler());

            appBuilder.UseWebApi(config);
        }

        /// <summary>
        /// Starts a new instance of the mobile web API.
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <returns></returns>
        public static IDisposable StartApp(string baseAddress)
        {
            return WebApp.Start<Startup>(baseAddress);
        }
    }
}
