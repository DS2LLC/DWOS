using System.Reflection;
using DWOS.Shared;
using Topshelf;

namespace DWOS.Server
{
    internal static class Program
    {
        private static void Main()
        {
            About.RegisterAssembly(Assembly.GetExecutingAssembly());
            ErrorMessageBox.PreventUIInteraction = true;
            
            HostFactory.Run(x =>
                            {
                                x.Service<DWOSService>();
                                x.RunAsLocalSystem();
                                x.SetDescription("Provides management and licensing for DWOS");
                                x.SetDisplayName("DWOS Server");
                                x.SetServiceName("DWOSServer");
                                x.StartAutomaticallyDelayed();
                            });
        }
    }
}