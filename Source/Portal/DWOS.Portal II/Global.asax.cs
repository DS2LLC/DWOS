using System.Reflection;
using System.Web.Http;
using System.Web.Routing;
using DWOS.Data;
using DWOS.Portal.Models;
using DWOS.Shared;

namespace DWOS.Portal
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ApplicationSettings.Current.UpdateDatabaseConnection(DataAccess.ConnectionString);
            About.RegisterAssembly(Assembly.GetExecutingAssembly());
            DependencyContainerConfig.RegisterDependencies();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
