using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DWOS.Data;
using DWOS.DataImporter.Licensing;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using NLog;
using DWOS.DataImporter.Properties;
using System.IO;

namespace DWOS.DataImporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IUserNotifier
    {
        #region Fields

        /// <summary>
        /// Part of a message from exceptions that are thrown when trying to
        /// access a file that is already in use.
        /// </summary>
        private const string EXCEPTION_MESSAGE_FILE_IN_USE = "because it is being used by another process.";

        /// <summary>
        /// Default length of time (in seconds) to wait for a connection to the selected SQL server.
        /// </summary>
        private const int DEFAULT_SQL_TIMEOUT = 120;

        #endregion

        #region Methods

        public MainWindow()
        {
            InitializeComponent();
            Title = "DWOS Data Migration " + About.ApplicationVersion;
        }

        private void LoadServer()
        {
            bool hasUserSettingsServer = !String.IsNullOrWhiteSpace(UserSettings.Default.ServerAddress);
            bool hasSettingsServer = !String.IsNullOrWhiteSpace(Settings.Default.ApplicationServerAddress);
            
            if (hasSettingsServer)
            {
                var parts = Properties.Settings.Default.ApplicationServerAddress.Split(':');

                if(parts.Length >= 2)
                {
                    txtServerName.Text = parts[0];
                    txtServerPort.Text = parts[1];
                    return;
                }
            }
            else if (hasUserSettingsServer)
            {
                txtServerName.Text = UserSettings.Default.ServerAddress;
                txtServerPort.Text = UserSettings.Default.ServerPort.ToString();
                return;
            }

            FindServer();
        }

        private void FindServer()
        {
            try
            {
                    var dc = new DiscoveryClient(new UdpDiscoveryEndpoint());
                    var fc = new FindCriteria(typeof(ILicenseService));
                    fc.MaxResults = 1; //limited to speed up the search --- Set to 1 to just use the first server found
                    fc.Duration = TimeSpan.FromSeconds(1);
                    FindResponse fr = dc.Find(fc);

                    // here is the really nasty part
                    // i am just returning the first channel, but it may not work.
                    // you have to do some logic to decide which uri to use from the discovered uris
                    // for example, you may discover "127.0.0.1", but that one is obviously useless.
                    // also, catch exceptions when no endpoints are found and try again.
                    if (fr.Endpoints.Count > 0)
                    {
                        EndpointDiscoveryMetadata endPoint = fr.Endpoints.FirstOrDefault(a => !a.Address.Uri.IsLoopback) ?? fr.Endpoints.FirstOrDefault();
                        txtServerName.Text = endPoint.Address.Uri.Host;
                        txtServerPort.Text = endPoint.Address.Uri.Port.ToString();
                    }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error finding server automatically.");
            }
        }

        private void SaveServer()
        {
            Settings.Default.ApplicationServerAddress = this.txtServerName.Text + ":" + this.txtServerPort.Text;
            Settings.Default.Save();
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

            builder.ConnectTimeout = DEFAULT_SQL_TIMEOUT;

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
        private static bool ConnectToApplicationServer()
        {
            try
            {
                LogManager.GetCurrentClassLogger().Info("Connecting to app server");

                var serverAddress = Properties.Settings.Default.ApplicationServerAddress;

                //connect to client to get application information...
                var client = new LicenseServiceClient(new NetTcpBinding(SecurityMode.None, false), new EndpointAddress("net.tcp://" + serverAddress + "/LicenseService"));
                var response = client.GetApplicationInfo(new GetApplicationInfoRequest());
                var appInfo = response.GetApplicationInfoResult;
                client.Close();

                //check to see if this client is compatible with the server
                if (!IsClientServerConnectionValid(About.ApplicationVersion, appInfo))
                {
                    var minimumClientVersion = String.IsNullOrWhiteSpace(appInfo.MinimumClientVersion) ? appInfo.ServerVersion : appInfo.MinimumClientVersion;

                    if (MessageBox.Show("You do not have the correct client version of DWOS. Please install client version " + minimumClientVersion + ".", About.ApplicationName) == MessageBoxResult.OK)
                        return false;
                }

                UpdateDatabaseConnection(appInfo.DatabaseConnection);
                return true;
            }
            catch
            {
                MessageBox.Show("Unable to connect to server.", About.ApplicationName);
                return false;
            }
        }

        private static bool IsClientServerConnectionValid(string client, ApplicationInfo serverInfo)
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

        private void ClearNotifications()
        {
            txtNotifications.Text = null;
        }

        private void AddNotification(string notification) { txtNotifications.Text += (notification + Environment.NewLine); }

        private void AddNotification(Exception exc)
        {
            string message = "ERROR: " + exc.Message + Environment.NewLine +
                    exc.StackTrace;

            txtNotifications.Text += message + Environment.NewLine;
        }

        private static void OpenTemplate(string templateFileName)
        {
            var file = System.IO.Path.Combine(FileSystem.ApplicationPath(), "Template", templateFileName);

            if (File.Exists(file))
            {
                Process.Start(new ProcessStartInfo(file));
            }
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnPartExport.IsEnabled = false;
            btnPartImport.IsEnabled = false;
            btnCustomerExport.IsEnabled = false;
            btnCustomerImport.IsEnabled = false;
            btnContactExport.IsEnabled = false;
            btnContactImport.IsEnabled = false;

            LoadServer();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(txtServerName.Text))
                {
                    MessageBox.Show("No server name set.");
                    return;
                }

                if (String.IsNullOrWhiteSpace(txtServerPort.Text))
                {
                    MessageBox.Show("No server port.");
                    return;
                }

                SaveServer();

                var connected = ConnectToApplicationServer();

                btnPartExport.IsEnabled = connected;
                btnPartImport.IsEnabled = connected;
                btnCustomerExport.IsEnabled = connected;
                btnCustomerImport.IsEnabled = connected;
                btnContactExport.IsEnabled = connected;
                btnContactImport.IsEnabled = connected;

                if (connected)
                    MessageBox.Show("Connected successfully.", About.ApplicationName);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error connecting to server.");
            }
        }

        private void btnCustomerExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearNotifications();
                var export = new CustomerExchange(this);
                export.Export();
            }
            catch (Exception exc)
            {
                if (exc.Message.EndsWith(EXCEPTION_MESSAGE_FILE_IN_USE))
                {
                    AddNotification("ERROR: Unable to export customers. The selected file cannot be opened because it is in use.");
                }
                else
                {
                    AddNotification(exc);
                    NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error exporting customers.");
                }
            }
        }

        private void btnCustomerImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearNotifications();
                var export = new CustomerExchange(this);
                export.Import();
            }
            catch (Exception exc)
            {
                if (exc.Message.EndsWith(EXCEPTION_MESSAGE_FILE_IN_USE))
                {
                    AddNotification("ERROR: Unable to import customers. The selected file cannot be opened because it is in use.");
                }
                else
                {
                    AddNotification(exc);
                    NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error importing customers.");
                }
            }
        }

        private void btnPartImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearNotifications();
                var export = new PartExchange(this);
                export.Import();
            }
            catch (Exception exc)
            {
                if (exc.Message.EndsWith(EXCEPTION_MESSAGE_FILE_IN_USE))
                {
                    AddNotification("ERROR: Unable to import parts. The selected file cannot be opened because it is in use.");
                }
                else
                {
                    AddNotification(exc);
                    NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error importing parts.");
                }
            }
        }
        
        private void btnPartExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearNotifications();
                
                var export = new PartExchange(this);
                export.Export();
            }
            catch (Exception exc)
            {
                if (exc.Message.EndsWith(EXCEPTION_MESSAGE_FILE_IN_USE))
                {
                    AddNotification("ERROR: Unable to export parts. The selected file cannot be opened because it is in use.");
                }
                else
                {
                    AddNotification(exc);
                    NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error exporting parts.");
                }
            }
        }

        private void btnContactExport_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearNotifications();
                var exchange = new ContactExchange(this);
                exchange.Export();
            }
            catch (Exception exc)
            {
                if (exc.Message.EndsWith(EXCEPTION_MESSAGE_FILE_IN_USE))
                {
                    AddNotification("ERROR: Unable to export contacts. The selected file cannot be opened because it is in use.");
                }
                else
                {
                    AddNotification(exc);
                    LogManager.GetCurrentClassLogger().Error(exc, "Error exporting contacts.");
                }
            }
        }

        private void btnContactImport_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearNotifications();
                var exchange = new ContactExchange(this);
                exchange.Import();
            }
            catch (Exception exc)
            {
                if (exc.Message.EndsWith(EXCEPTION_MESSAGE_FILE_IN_USE))
                {
                    AddNotification("ERROR: Unable to import contacts. The selected file cannot be opened because it is in use.");
                }
                else
                {
                    AddNotification(exc);
                    LogManager.GetCurrentClassLogger().Error(exc, "Error importing contacts.");
                }
            }

        }

        private void CustomerTemplate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OpenTemplate("Customers.xlsx");
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error displaying customer template.");
            }
        }

        private void PartTemplate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OpenTemplate("Parts.xlsx");
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error displaying parts template.");
            }
        }

        private void ContactTemplate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OpenTemplate("Contacts.xlsx");
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error displaying contacts template.");
            }
        }

        #endregion

        #region IUserNotifier Members

        public void ShowNotification(string message)
        {
            AddNotification(message);
        }

        #endregion
    }
}
