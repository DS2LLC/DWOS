using System;
using System.Reflection;
using System.Windows.Forms;
using DWOS.Shared;

namespace DWOS.Server.Admin
{
    internal static class Program
    {
        #region Methods

        /// <summary>
        ///   The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            About.RegisterAssembly(Assembly.GetExecutingAssembly());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += Application_ThreadException;
            
            DWOS.Data.ServerSettings.Default.UpdateDatabaseConnection("DWOS Server Admin");
            Application.Run(new Main());
        }

        #endregion

        #region Events

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            NLog.LogManager.GetCurrentClassLogger().Error(e.Exception, "Error running application.");
        }

        #endregion
    }
}