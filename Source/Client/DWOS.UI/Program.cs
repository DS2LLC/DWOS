using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using DWOS.Data;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Licensing;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;
using Infragistics.Themes;
using Infragistics.Win.AppStyling;
using NLog;

namespace DWOS.UI
{
    internal static class Program
    {
        #region Properties

        /// <summary>
        /// Gets the path to the user directory file indicating if DWOS should
        /// clean that directory at startup.
        /// </summary>
        private static string ClearMediaFile
        {
            get
            {
                return Path.Combine(FileSystem.UserDocumentPath(), "clear_at_startup");
            }
        }

        #endregion

        #region Methods

        [STAThread]
        private static void Main()
        {
            System.Windows.Forms.Application.ThreadException	+= Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve             += CurrentDomain_AssemblyResolve;
            
            About.RegisterAssembly(Assembly.GetExecutingAssembly());
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            new DWOS.UI.App() { ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown };
            System.Windows.Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            //create sync context in case WPF BackgroundWorker gets called before a WPF control is loaded and the context is set http://stackoverflow.com/questions/13527442/why-does-the-backgroundworker-not-call-the-runworkercompleted-on-the-right-threa
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));

            LoadStyles();
            
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Client Version: {0}", About.ApplicationVersion);
            logger.Info("CLR Version: {0}", Environment.Version);
            logger.Info("OS Version: {0} {1}", Environment.OSVersion, Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit");

 			SplashScreen.Start();
            SplashScreen.UpdateStatusText("Connecting to application server...");

            bool cleanupFailed = false;
            var doCleanup = File.Exists(ClearMediaFile);
            if (doCleanup)
            {
                try
                {
                    CleanUserFiles();
                }
                catch (Exception exc)
                {
                    // calling MessageBoxUtilities.ShowMessageBoxOK throws error here - not running in proper thread
                    System.Windows.Forms.MessageBox.Show("Please close all open media files and start DWOS again", "DWOS");
                    LogManager.GetCurrentClassLogger().Warn(exc, "Cleanup prior to app server connect failed.");
                    cleanupFailed = true;
                }
            }

            if (cleanupFailed)
            {
                SplashScreen.Stop();
            }
            else
            {
                var connInfo = ConnectToApplicationServer();

                if (connInfo == null)
                {
                    SplashScreen.Stop();
                }
                else
                {
                    SplashScreen.UpdateStatusText("Validating connection to data server...");

                    if (ApplicationSettings.Current.ValidateConnectionToDatabase())
                    {
                        SplashScreen.UpdateStatusText("Loading settings...");

                        if (doCleanup)
                        {
                            ApplicationSettings.Current.ClearImageCache();
                        }

                        ApplicationSettings.Current.PreLoadSettings();

                        RegisterDependencies(connInfo);

                        //start security manager and create the main form
                        SecurityManager.Current.Initialize();

                        SplashScreen.UpdateStatusText("Loading application...");

                        if(UserSettings.Default.ResetLayouts)
                            ResetLayouts();

                        DWOSApp.MainForm = new Main(DependencyContainer.Resolve<Data.Date.ICalendarPersistence>())
                        {
                            Opacity = 0
                        };

                        System.Windows.Forms.Application.Run(DWOSApp.MainForm);

                        //dispose security data, close smart card connection
                        SecurityManager.Current.Dispose();
                    }
                    else
                    {
                        //stop splash screen to show message box
                        SplashScreen.Stop();
                        System.Windows.Forms.Application.DoEvents();
                        System.Windows.Forms.Application.Exit();
                    }
                }
            }
        }

        /// <summary>
        /// Setup dependency container.
        /// </summary>
        /// <param name="connInfo"></param>
        private static void RegisterDependencies(DwosServerInfo connInfo)
        {
            DependencyContainer.Register(new DwosServerInfoProvider(connInfo));
            DependencyContainer.Register<Data.Date.ICalendarPersistence>(new Data.Date.CalendarPersistence());
            DependencyContainer.Register<Data.Date.IDateTimeNowProvider>(new Data.Date.DateTimeNowProvider());
            DependencyContainer.Register<IDwosApplicationSettingsProvider>(new DwosApplicationSettingsProvider());

            // Register licenses for thermal label SDK
            Neodynamic.SDK.Printing.ThermalLabel.LicenseOwner = "DS2 LLC-Ultimate Edition-Developer License";
            Neodynamic.SDK.Printing.ThermalLabel.LicenseKey = "L6LD54CTJ9K5T7YTBLKGXB5YPGTA9BDV4NWTQN83DH98SEMTEA3Q";

            Neodynamic.Windows.WPF.ThermalLabelEditor.ThermalLabelEditor.LicenseOwner = "DS2 LLC-Developer License";
            Neodynamic.Windows.WPF.ThermalLabelEditor.ThermalLabelEditor.LicenseKey = "AP6DBQUTCFL8UVWSHU8VYTTJZXDK57SJBJQG8G8BGVUG89KYCXTQ";

            Neodynamic.Windows.Forms.ThermalLabelEditor.ThermalLabelEditor.LicenseOwner = "DS2 LLC-Developer License";
            Neodynamic.Windows.Forms.ThermalLabelEditor.ThermalLabelEditor.LicenseKey = "AP6DBQUTCFL8UVWSHU8VYTTJZXDK57SJBJQG8G8BGVUG89KYCXTQ";
        }

        /// <summary>
        /// Updates the database connection so the datasets can use the same db connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
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

            ApplicationSettings.Current.UpdateDatabaseConnection(builder.ToString());

            if(!string.IsNullOrEmpty(builder.Password))
                builder.Password = "********"; //strip out password before writing out to file system
            
            LogManager.GetCurrentClassLogger().Info("Update Database Connection: {0}", builder.ToString());
		}

        /// <summary>
        /// Connects to application server to get the database connection.
        /// </summary>
        private static DwosServerInfo ConnectToApplicationServer(bool isFirstAttempt = true)
        {
            try
            {
                LogManager.GetCurrentClassLogger().Info("Connecting to app server, first attempt {0}", isFirstAttempt);
                //ConnectionFile.CreateSampleConnectionFile();

                var connFile = ConnectionFile.LoadConnectionFile();
                DwosServerInfo serverInfo;

                if (connFile == null || string.IsNullOrWhiteSpace(connFile.ServerAddress))
                {
                    //if not first-attempt OR no server address defined
                    if (string.IsNullOrWhiteSpace(UserSettings.Default.ServerAddress) || !isFirstAttempt)
                    {
                        string previousAddress = UserSettings.Default.ServerAddress;

                        using (var frm = new ServerConnection())
                        {
                            if (frm.ShowDialog() != DialogResult.OK)
                                return null;

                            UserSettings.Default.ServerAddress = frm.ServerAddress;
                            UserSettings.Default.ServerPort = frm.ServerPort;
                            UserSettings.Default.Save();
                        }

                        if (!isFirstAttempt && previousAddress != UserSettings.Default.ServerAddress)
                        {
                            try
                            {
                                ClearFilesAtStartup();
                            }
                            catch (Exception exc)
                            {
                                System.Windows.Forms.MessageBox.Show("Please close all open media files and try again.", "DWOS");
                                LogManager.GetCurrentClassLogger().Warn(exc, "Cleanup prior to app server connect failed.");
                                return null;
                            }
                        }
                    }

                    serverInfo = new DwosServerInfo()
                    {
                        ServerAddress = UserSettings.Default.ServerAddress,
                        ServerPort = UserSettings.Default.ServerPort
                    };
                }
                else
                {
                    SplashScreen.UpdateStatusText("Loading connection file...");
                    serverInfo = new DwosServerInfo()
                    {
                        ServerAddress = connFile.ServerAddress,
                        ServerPort = connFile.ServerPort,
                        FromConnectionFile = true
                    };
                }

                var client = new LicenseServiceClient(new NetTcpBinding(SecurityMode.None, false), new EndpointAddress(serverInfo.ServerAddressUri));
                var response = client.GetApplicationInfo(new GetApplicationInfoRequest());
                var appInfo  = response.GetApplicationInfoResult;
                client.Close();

                //check to see if this client is compatible with the server
                if(!IsClientServerConnectionValid(About.ApplicationVersion, appInfo))
                {
                    //stop splash before showing pop-up
                    SplashScreen.Stop();
                    System.Windows.Forms.Application.DoEvents();

                    var minimumClientVersion = string.IsNullOrWhiteSpace(appInfo.MinimumClientVersion) ? appInfo.ServerVersion : appInfo.MinimumClientVersion;

                    string updatePromptMsg = "You do not have the correct client version of DWOS.\nTo connect to {0}:{1}, please install client version {2}."
                        .FormatWith(serverInfo.ServerAddress, serverInfo.ServerPort, minimumClientVersion);

                    const string updatePromptFooter = "Select OK to download latest version or Cancel to change server properties.";

                    if (MessageBoxUtilities.ShowMessageBoxOKCancel(updatePromptMsg, "Incorrect Version", updatePromptFooter) == DialogResult.OK)
                    {
                        Updater.CheckUpdates(minimumClientVersion, false);
                        return null;
                    }
                    else //this will allow you to select a different server if needed
                        return ConnectToApplicationServer(false); //ask to try select the server
                }

                UpdateDatabaseConnection(connFile != null && !string.IsNullOrWhiteSpace(connFile.DatabaseConnection) ? connFile.DatabaseConnection : appInfo.DatabaseConnection);

                return serverInfo;
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

#if DEBUG
                if(serverInfo.DatabaseConnection.Contains("DPSDATA")) //If connecting to DPS ignore server version
                    return true;
                if(serverInfo.DatabaseConnection.Contains("sandsdist")) //If connecting to DPS ignore server version
                    return true;
#endif
                //ensure Major.Minor.Build matches between client and server
                if (clientVersion.CompareMajorMinorBuild(serverVersion))
                {
                    //If server has a minimum client version recommednation then use that to ensure we are using the right revision
                    if(!string.IsNullOrWhiteSpace(serverInfo.MinimumClientVersion))
                    {
                        Version clientMinVersion;

                        if(Version.TryParse(serverInfo.MinimumClientVersion, out clientMinVersion))
                        {
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
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error determining if can connect to server.");
                return false;
            }
        }

        private static void ResetLayouts()
        {
            try
            {
                SplashScreen.UpdateStatusText("Reseting Layouts...");
                var files = new List <string>();
                files.AddRange(Directory.GetFiles(FileSystem.UserAppDataPath(), "*.xml"));
                files.AddRange(Directory.GetFiles(FileSystem.UserAppDataPath(), "*.dat"));
                files.AddRange(Directory.GetFiles(FileSystem.UserAppDataPathVersion(), "*.*"));
                int deleteCount = 0;

                foreach(var file in files)
                {
                    try
                    {
                        File.Delete(file);
                        deleteCount++;
                    }
                    catch(Exception exc)
                    {
                        NLog.LogManager.GetCurrentClassLogger().Warn(exc, "Error deleting layout file {0}.".FormatWith(file));
                    }
                }

                NLog.LogManager.GetCurrentClassLogger().Info("Delete {0} files during reset layouts.".FormatWith(deleteCount));
            }
            catch(Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error reseting layouts.");
            }
            finally
            {
                UserSettings.Default.ResetLayouts = false;
            }
        }

        private static void Restart(bool asAdmin = false)
        {
            // Restart app
            #if DEBUG
            System.Windows.Forms.Application.Exit();
            return;
            #endif

#pragma warning disable 162
            var Info = new ProcessStartInfo();
            Info.Arguments = "/C ping 127.0.0.1 -n 3 && \"" + System.Windows.Forms.Application.ExecutablePath + "\"";
            Info.WindowStyle = ProcessWindowStyle.Hidden;
            Info.CreateNoWindow = true;
            Info.FileName = "cmd.exe";
            if(asAdmin)
                Info.Verb = "runas"; 
            Process.Start(Info);
            System.Windows.Forms.Application.Exit(); 
#pragma warning restore 162

        }

        internal static void ChangeServerNow(string serverAddress, int serverPort)
        {
            try
            {
                ClearFilesAtStartup();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Cleanup prior to delayed server change failed - stopping server change.");
                const string warningMessage = "There was an issue while setting up the server change. " +
                    "Please change the server again. If you see this message again, try logging into " +
                    "Windows as a different user.";

                ErrorMessageBox.ShowDialog(warningMessage, exc, false);
                return;
            }

            UserSettings.Default.ServerAddress = serverAddress;
            UserSettings.Default.ServerPort = serverPort;
            UserSettings.Default.Save();

            Restart();
        }

        internal static void ChangeServerAtNextStart(string serverAddress, int serverPort)
        {
            try
            {
                ClearFilesAtStartup();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Cleanup prior to delayed server change failed - stopping server change.");
                const string warningMessage = "There was an issue while setting up the server change. " +
                    "Please change the server again. If you see this message again, try logging into " +
                    "Windows as a different user.";

                ErrorMessageBox.ShowDialog(warningMessage, exc, false);
                return;
            }

            UserSettings.Default.ServerAddress = serverAddress;
            UserSettings.Default.ServerPort = serverPort;
            UserSettings.Default.Save();
        }

        private static void ClearFilesAtStartup()
        {
            var clearMediaFileInfo = new FileInfo(ClearMediaFile);

            if (clearMediaFileInfo.Directory != null && !clearMediaFileInfo.Directory.Exists)
            {
                clearMediaFileInfo.Directory.Create();
            }

            if (!clearMediaFileInfo.Exists)
            {
                using (clearMediaFileInfo.Create())
                {
                    // Create empty file
                }
            }

            clearMediaFileInfo.Attributes = FileAttributes.Hidden;
        }

        /// <summary>
        /// Cleans all user document directories containing server-specific data.
        /// </summary>
        /// <remarks>
        /// This method performs many deletions/renames, so expect this method
        /// to throw an exception when any user files are in use.
        /// </remarks>
        private static void CleanUserFiles()
        {
            var mediaCacheDir = FileSystem.GetFolder(FileSystem.enumFolderType.MediaCache, true);
            if (Directory.Exists(mediaCacheDir))
            {
                Directory.Delete(mediaCacheDir, true);
            }

            // Documents directory may contain changes, so this method moves
            // documents instead of deleting them.
            string documentDir;
            if (string.IsNullOrEmpty(UserSettings.Default.DocumentsWorkingDirectory))
            {
                documentDir = FileSystem.GetFolder(FileSystem.enumFolderType.Documents, true);
            }
            else
            {
                documentDir = UserSettings.Default.DocumentsWorkingDirectory;
            }

            if (Directory.Exists(documentDir) && Directory.EnumerateFileSystemEntries(documentDir).Any())
            {
                var documentDirInfo = new DirectoryInfo(documentDir);
                var backupFolderName = string.Format("{0}_{1}", documentDirInfo.Name,
                    DateTime.Now.ToString("MM.dd.yyyy.H.mm.ss"));

                var documentBackupDir = Path.Combine(documentDirInfo.Parent.FullName, backupFolderName);
                Directory.Move(documentDir, documentBackupDir);
            }

            // Delete file that indicates if DWOS should call this method at startup
            if (File.Exists(ClearMediaFile))
            {
                File.Delete(ClearMediaFile);
            }
        }

        private static void LoadStyles()
        {
            //Load WPF Styles
            ThemeManager.ApplicationTheme = new Office2013Theme(); 
            
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/DWOS.UI;component/Themes/DWOS.WPF.xaml", UriKind.Relative) });

            //Load WinForms Styles
            StyleManager.Load(new MemoryStream(Resources.DWOSStyle)); 
        }

        private static void HandleException(Exception exc)
        {
            if (exc != null && (exc.ToString().Contains("UltraTreeNode.get_IsLastViewableSibling") || exc.Source != null && exc.Source.StartsWith("Infragistics")))
            {
                LogManager.GetCurrentClassLogger().Error(exc, "An application error has occurred.");
            }
            else
            {
                ErrorMessageBox.ShowDialog("An application error has occurred.", exc);
            }
        }

        #endregion

        #region Events
        
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //hide un-needed assembly ref lookups from the logging
            if(args.Name != null && args.Name.Contains("InfragisticsWPF4.Themes") || args.Name.Contains("Neodynamic.Windows.ThermalLabelEditor.resources") ||  args.Name.Contains("XmlSerializers"))
                return null;

            LogManager.GetCurrentClassLogger().Info("Error resolving assembly: " + args.Name);

            Assembly[] asses = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly ass in asses)
            {
                if (ass.GetName().Name == args.Name)
                {
                    LogManager.GetCurrentClassLogger().Info("Resolved assembly as: " + ass.FullName);
                    return ass;
                }
            }

            return null;
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        private static void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            HandleException(unhandledExceptionEventArgs.ExceptionObject as Exception);
        }

        #endregion
    }
}