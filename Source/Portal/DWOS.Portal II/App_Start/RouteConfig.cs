using System.Web.Mvc;
using System.Web.Routing;

namespace DWOS.Portal
{
    /// <summary>
    /// MVC route configuration.
    /// </summary>
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Redirect Default.aspx to the proper root
            routes.MapRoute(
                name: "DefaultAspxRedirect",
                url: "Default.aspx",
                defaults: new { controller = "IndexRedirect", action = "Index" });

            // Normal routing
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id= UrlParameter.Optional }
            );
        }
    }
}