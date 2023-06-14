using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Service.Licensing;
using DWOS.Service.Properties;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using NLog;

namespace DWOS.Service
{
	static class Program
    {
        #region Methods
        
        /// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            
            About.RegisterAssembly(Assembly.GetExecutingAssembly());
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

            LogManager.GetCurrentClassLogger().Info("Application starting...");

            if (ConnectToApplicationServer())
                Application.Run(new Main());
		}

        /// <summary>
        /// Updates the database connection so the datasets can use the same db connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        private static void UpdateDatabaseConnection(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            builder.ApplicationName = "DWOS CheckIn"; //add the application name

            //Enable MARS to prevent any 'Open datareader' issues
            builder.MultipleActiveResultSets = true;

            //Set collection to no longer be read-only
            typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ConfigurationManager.ConnectionStrings, false);

            //Add connection to database from server
            ConfigurationManager.ConnectionStrings.Clear();	//clear current connections
            ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings("DWOS.Data.Properties.Settings.DWOSDataConnectionString", builder.ToString(), "System.Data.SqlClient"));

            ApplicationSettings.Current.UpdateDatabaseConnection(builder.ToString());

            if (!String.IsNullOrEmpty(builder.Password))
                builder.Password = "********"; //strip out password before writing out to file system

            LogManager.GetCurrentClassLogger().Info("Update Database Connection: {0}", builder.ToString());
        }

        /// <summary>
        /// Connects to application server to get the database connection.
        /// </summary>
        private static bool ConnectToApplicationServer(bool isFirstAttempt = true)
        {
            try
            {
                LogManager.GetCurrentClassLogger().Info("Connecting to app server, first attempt {0}", isFirstAttempt);

                var serverAddress = UserSettings.Default.ServerAddress;

                //if not first-attempt OR no server address defined
                if (String.IsNullOrEmpty(serverAddress) || !isFirstAttempt)
                {
                    using (var frm = new ServerConnection())
                    {
                        if (frm.ShowDialog() != DialogResult.OK)
                            return false;
                        else
                            serverAddress = UserSettings.Default.ServerAddress;
                    }
                }

                //connect to client to get application information...
                var serverInfo = new DwosServerInfo()
                {
                    ServerAddress = UserSettings.Default.ServerAddress,
                    ServerPort = UserSettings.Default.ServerPort
                };

                var client = new LicenseServiceClient(new NetTcpBinding(SecurityMode.None, false), new EndpointAddress(serverInfo.ServerAddressUri));
                var response = client.GetApplicationInfo(new GetApplicationInfoRequest());
                var appInfo = response.GetApplicationInfoResult;
                client.Close();

                //check to see if this client is compatible with the server
                if (!IsClientServerConnectionValid(About.ApplicationVersion, appInfo))
                {
                    Application.DoEvents();

                    var minimumClientVersion = String.IsNullOrWhiteSpace(appInfo.MinimumClientVersion) ? appInfo.ServerVersion : appInfo.MinimumClientVersion;

                    if (MessageBox.Show("You do not have the correct client version of DWOS. Please install client version " + minimumClientVersion + ".",  About.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Stop) == DialogResult.OK)
                    {
                        return false;
                    }
                    else //this will allow you to select a different server if needed
                        return ConnectToApplicationServer(false); //ask to try select the server
                }

                UpdateDatabaseConnection(appInfo.DatabaseConnection);
                return true;
            }
            catch
            {
                return ConnectToApplicationServer(false);
            }
        }

        public static bool IsClientServerConnectionValid(string client, ApplicationInfo serverInfo)
        {
            try
            {
                var clientVersion = new Version(client);
                var serverVersion = new Version(serverInfo.ServerVersion);

                //If server has a minimum client version then use that to ensure we are using the right revision
                if (!String.IsNullOrWhiteSpace(serverInfo.MinimumClientVersion))
                {
                    Version clientMinVersion;

                    if (Version.TryParse(serverInfo.MinimumClientVersion, out clientMinVersion))
                        return clientVersion.Major == serverVersion.Major && clientVersion.Minor == serverVersion.Minor && clientVersion.Build == serverVersion.Build && clientVersion.Revision >= clientMinVersion.Revision;
                }


                //valid client if Major.Minor.Build matches, okay for revision to not match
                return clientVersion.Major == serverVersion.Major && clientVersion.Minor == serverVersion.Minor && clientVersion.Build == serverVersion.Build;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error determining if can connect to server.");
                return false;
            }
        }

        #endregion

        #region Events

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            NLog.LogManager.GetCurrentClassLogger().Info("Error resolving assembly: " + args.Name);

            Assembly[] asses = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly ass in asses)
            {
                if (ass.GetName().Name == args.Name)
                {
                    NLog.LogManager.GetCurrentClassLogger().Info("Resolved assembly as: " + ass.FullName);
                    return ass;
                }
            }

            return null;
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (e.Exception != null && e.Exception.ToString().Contains("UltraTreeNode.get_IsLastViewableSibling"))
            {
                NLog.LogManager.GetCurrentClassLogger().Error(e.Exception, "An application error has occurred.");
            }
            else
            {
                ErrorMessageBox.ShowDialog("An application error has occurred.", e.Exception);
            }
        }

        #endregion
    }
}
