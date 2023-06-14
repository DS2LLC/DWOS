using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Windows;
using DWOS.Data;
using DWOS.Shared;
using System.ServiceModel;
using System.Windows.Threading;
using DWOS.AutomatedWorkOrderTool.Licensing;
using DWOS.AutomatedWorkOrderTool.Services;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using DWOS.Reports;
using DWOS.Shared.Utilities;
using GalaSoft.MvvmLight.Ioc;
using NLog;

namespace DWOS.AutomatedWorkOrderTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        #region Methods

        public App()
        {
            About.RegisterAssembly(Assembly.GetExecutingAssembly());
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow();

            if (Connect() && SetupApplication())
            {
                MainWindow?.Show();
            }
            else
            {
                Shutdown();
            }
        }

        private bool Connect()
        {
            var isConnected = false;
            var firstTry = true;

            while (true)
            {
                if (string.IsNullOrEmpty(UserSettings.Default.ServerAddress) || !firstTry)
                {
                    if (new ConnectionWindow().ShowDialog() == false)
                    {
                        // User canceled out of dialog
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(UserSettings.Default.ServerAddress))
                {
                    var serverInfo = new DwosServerInfo
                    {
                        ServerAddress = UserSettings.Default.ServerAddress,
                        ServerPort = UserSettings.Default.ServerPort
                    };

                    try
                    {
                        if (IsClientServerConnectionValid(About.ApplicationVersion, GetAppInfo(serverInfo), out var minimumClientVersion))
                        {
                            isConnected = true;
                            break;
                        }
                        if (minimumClientVersion != null)
                        {
                            var updatePromptMsg = "You do not have the correct client version of DWOS.\nTo connect to {0}:{1}, please install client version {2}."
                                .FormatWith(serverInfo.ServerAddress, serverInfo.ServerPort, minimumClientVersion);

                            ErrorWindow.ShowError(updatePromptMsg);
                        }
                        else
                        {
                            ErrorWindow.ShowError("Could not connect to server.");
                        }
                    }
                    catch (Exception exc)
                    {
                        LogManager.GetCurrentClassLogger().Warn(exc, "Error connecting");
                        ErrorWindow.ShowError("Could not connect to server.");
                    }

                    firstTry = false;
                }
            }

            return isConnected;
        }

        private static ApplicationInfo GetAppInfo(DwosServerInfo serverInfo)
        {
            var client = new LicenseServiceClient(new NetTcpBinding(SecurityMode.None, false), new EndpointAddress(serverInfo.ServerAddressUri));
            var response = client.GetApplicationInfo(new GetApplicationInfoRequest());
            var appInfo = response.GetApplicationInfoResult;
            client.Close();
            return appInfo;
        }

        public static bool IsClientServerConnectionValid(string client, ApplicationInfo serverInfo, out Version minimumClientVersion)
        {
            minimumClientVersion = null;

            try
            {
                var clientVersion = new Version(client);
                var serverVersion = new Version(serverInfo.ServerVersion);

                //ensure Major.Minor.Build matches between client and server
                if (clientVersion.CompareMajorMinorBuild(serverVersion))
                {
                    if (!string.IsNullOrWhiteSpace(serverInfo.MinimumClientVersion))
                    {
                        if (Version.TryParse(serverInfo.MinimumClientVersion, out var clientMinVersion))
                        {
                            minimumClientVersion = clientMinVersion;
                            //if the client minimum version is valid for this server version
                            if (clientMinVersion.CompareMajorMinorBuild(serverVersion))
                                return clientVersion.Revision >= clientMinVersion.Revision;
                        }
                    }

                    return true;
                }

                return false;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error determining if can connect to server.");
                return false;
            }
        }

        private bool SetupApplication()
        {

            var serverInfo = new DwosServerInfo
            {
                ServerAddress = UserSettings.Default.ServerAddress,
                ServerPort = UserSettings.Default.ServerPort
            };

            UpdateDatabaseConnection(GetAppInfo(serverInfo).DatabaseConnection);

            if (ApplicationSettings.Current.ValidateConnectionToDatabase(false))
            {
                // Register non-program dependencies
                DependencyContainer.Register<Data.Date.ICalendarPersistence>(new Data.Date.CalendarPersistence());
                DependencyContainer.Register<Data.Date.IDateTimeNowProvider>(new Data.Date.DateTimeNowProvider());
                DependencyContainer.Register<IDwosApplicationSettingsProvider>(new DwosApplicationSettingsProvider());

                // Set server info
                SimpleIoc.Default.GetInstance<IServerManager>().Initialize(serverInfo);

                // Register security manager for reports
                Report.SecurityManager = new AwotSecurityUserInfo(SimpleIoc.Default.GetInstance<IUserManager>());
                return true;
            }

            return false;
        }

        private static void UpdateDatabaseConnection(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            builder.ApplicationName = "DWOS"; //add the application name

            //Enable MARS to prevent any 'Open datareader' issues
            builder.MultipleActiveResultSets = true;

            //Set collection to no longer be read-only
            typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ConfigurationManager.ConnectionStrings, false);

            //Add connection to database from server
            ConfigurationManager.ConnectionStrings.Clear();	//clear current connections
            ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings("DWOS.Data.Properties.Settings.DWOSDataConnectionString", builder.ToString(), "System.Data.SqlClient"));
            ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings("DWOS.AutomatedWorkOrderTool.Properties.Settings.DWOSDataConnectionString", builder.ToString(), "System.Data.SqlClient"));

            ApplicationSettings.Current.UpdateDatabaseConnection(builder.ToString());

            if (!string.IsNullOrEmpty(builder.Password))
                builder.Password = "********"; //strip out password before writing out to file system

            LogManager.GetCurrentClassLogger().Info("Update Database Connection: {0}", builder.ToString());
        }

        #endregion

        #region Events

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            try
            {
                var userManager = SimpleIoc.Default.GetInstance<IUserManager>();
                userManager.LogOut(); // Check in any licenses

                // Cleanup
                var locator = Resources["Locator"] as ViewModelLocator;
                locator?.Cleanup();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error while closing application.");
            }
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Error(e.Exception, "Uncaught exception");

            try
            {
                var exception = e.Exception is TargetInvocationException invocationException
                    ? invocationException.InnerException
                    : e.Exception;

                if (exception != null)
                {
                    var errorText = $"Program Error:\n{exception.GetType()}\n{exception.Message}\n{e.Exception.StackTrace}";
                    ErrorWindow.ShowError(errorText);
                }
            }
            catch (Exception exc)
            {
                logger.Error(exc, "Error while showing error window.");
            }

            e.Handled = true;
        }

        #endregion
    }
}
