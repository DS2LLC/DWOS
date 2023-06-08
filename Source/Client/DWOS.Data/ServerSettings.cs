using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using NLog;
using DWOS.Shared.Data;
using DWOS.Shared.Settings;
using Microsoft.Win32;

namespace DWOS.Data
{
    /// <summary>
    /// Manages settings specific to the server application.
    /// </summary>
    public sealed class ServerSettings : RegistrySettingsBase
    {
        #region Fields

        private static readonly Lazy<ServerSettings> _lazyLoader = new Lazy<ServerSettings>(() => new ServerSettings());
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private const string KEY = "SOFTWARE\\DS2\\DWOS";
        private const int INACTIVE_TIMEOUT_DEFAULT = 20; //In Minutes
        private const int INACTIVE_TIMEOUT_MINIMUM = 5; //In Minutes
        private const int DEFAULT_SERVICE_RESTART_TIMEOUT = 1; // In Minutes

        #endregion

        #region Properties

        protected override string RegistryKeyName
        {
            get
            {
                return KEY;
            }
        }

        protected override RegistryKey RegistryHive
        {
            get
            {
                return Microsoft.Win32.Registry.LocalMachine;
            }
        }

        /// <summary>
        /// Gets or sets the minimum client version.
        /// </summary>
        /// <remarks>
        /// This is saved to force clients to eventually upgrade to the latest
        /// version even though the client will still operate with the server.
        /// </remarks>
        /// <value>The minimum client version.</value>
        [DataColumn]
        public string MinimumClientVersion { get; set; }

        /// <summary>
        /// Gets or sets the administrator's email address.
        /// </summary>
        [DataColumn]
        public string AdminEmail { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for a customer site.
        /// </summary>
        [DataColumn]
        public string Fingerprint { get; set; }
        
        /// <summary>
        /// Gets or sets the key for the company.
        /// </summary>
        [DataColumn]
        public string CompanyKey { get; set; }

        /// <summary>
        /// Gets or sets the installed database version.
        /// </summary>
        /// <value>The version.</value>
        [DataColumn]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the server's license key.
        /// </summary>
        [DataColumn]
        public string LicenseKey { get; set; }

        /// <summary>
        /// Gets or sets the user inactivity timeout in minutes. If user is inactive beyond this time then auto-log them out.
        /// </summary>
        /// <value>The inactivity timeout.</value>
        [DataColumn(DefaultValue = INACTIVE_TIMEOUT_DEFAULT)]
        public int InactivityTimeout { get; set; }

        /// <summary>
        /// Gets or sets the string to use for database connections.
        /// </summary>
        [DataColumn(FieldName = "DBConn")]
        public string DBConnectionString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if mobile/REST services are enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if mobile/REST services are enabled; otherwise, <c>false</c>.
        /// </value>
        [DataColumn(DefaultValue = true)]
        public bool EnableRESTServices { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer portal server.
        /// </summary>
        /// <value>The customer portal server.</value>
        [DataColumn]
        public string CustomerPortalServer { get; set; }

        /// <summary>
        /// Gets or sets the full address of the customer portal site.
        /// </summary>
        [DataColumn]
        public string CustomerPortalWebSite { get; set; }

        /// <summary>
        /// Gets or sets a flag determining if the service should restart
        /// on failure.
        /// </summary>
        [DataColumn]
        public bool EnableServiceFailureRestart { get; set; }

        /// <summary>
        /// Gets or sets the time to wait before restarting the server service
        /// (in minutes.)
        /// </summary>
        [DataColumn(DefaultValue = DEFAULT_SERVICE_RESTART_TIMEOUT)]
        public int ServiceRestartTimeout { get; set; }

        /// <summary>
        /// Gets or sets a flag determining if the server should shrink
        /// the database every night as part of routine cleanup.
        /// </summary>
        /// <value>
        /// <c>true</c> if automated shrink is enabled; otherwise, <c>false</c>.
        /// </value>
        [DataColumn(DefaultValue = true)]
        public bool EnableNightlyDatabaseShrink { get; set; }

        /// <summary>
        /// Gets or sets backup-specific values.
        /// </summary>
        [DataColumn]
        public ServerBackupSettings Backup { get; set; }

        /// <summary>
        /// Gets or sets SYSPRO-specific values.
        /// </summary>
        [DataColumn]
        public ServerSysproSettings SysproSettings { get; set; }

        /// <summary>
        /// Gets the default <see cref="ServerSettings"/> instance.
        /// </summary>
        public static ServerSettings Default
        {
            get
            {
                return _lazyLoader.Value;
            }
        }

        #endregion

        #region Methods

        private ServerSettings()
        {
            Load();
        }

        /// <summary>
        /// Reload the settings from the registry.
        /// </summary>
        public void ReLoad()
        {
            Load();
        }

        protected override void AfterLoad()
        {
            base.AfterLoad();

            if (Fingerprint == null)
            {
                Fingerprint = CreateFingerprint();
            }

            ValidateSettings();
        }

        /// <summary>
        /// Saves this settings to the registry.
        /// </summary>
        public void Save()
        {
            base.SaveSettings();
        }

        private string CreateFingerprint()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Updates the database connection info into the connection strings used by Typed Datasets.
        /// </summary>
        public void UpdateDatabaseConnection(string applicationName)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(DBConnectionString);
                builder.ApplicationName = applicationName; //add the application name

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

                _log.Info("Update Database Connection: {0}", builder.ToString());
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating connection strings.");
            }
        }

        /// <summary>
        /// Gets the encrypted license from the registry.
        /// </summary>
        /// <returns>System.Byte[][].</returns>
        public byte[] GetEncryptedLicense()
        {
            using (var baseKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(KEY, false))
            {

                if (baseKey == null)
                    return null;

                var license = baseKey.GetValue("License");

                var encryptedLicense = license as byte[];
                if (encryptedLicense != null)
                    return encryptedLicense;

                return null;
            }
        }

        /// <summary>
        /// Save the encrypted license to the registry.
        /// </summary>
        /// <param name="encryptedLicense">The encrypted license.</param>
        public void SetEncryptedLicense(byte[] encryptedLicense)
        {
            using (var baseKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(KEY, true) ?? Microsoft.Win32.Registry.LocalMachine.CreateSubKey(KEY))
            {
                if(encryptedLicense != null)
                    baseKey.SetValue("License", encryptedLicense, Microsoft.Win32.RegistryValueKind.Binary);
            }
        }

        /// <summary>
        /// Validates the settings are properly initialized.
        /// </summary>
        private void ValidateSettings()
        {
            //if the time out is less than 5 mintues then something is wrong
            if(this.InactivityTimeout < INACTIVE_TIMEOUT_MINIMUM)
            {
                _log.Info("WARN: Inactivity timeout of {0} is less then min value of {1}, resetting to defalut value {2}.", this.InactivityTimeout, INACTIVE_TIMEOUT_MINIMUM, INACTIVE_TIMEOUT_DEFAULT);
                this.InactivityTimeout = INACTIVE_TIMEOUT_DEFAULT;
            }
        }

        /// <summary>
        /// Splits <see cref="CustomerPortalWebSite"/> into site and
        /// application portions.
        /// </summary>
        /// <param name="webSite">web site portion</param>
        /// <param name="webApplication">web app portion</param>
        /// <returns>
        /// <c>true</c> if <see cref="CustomerPortalWebSite"/> has both
        /// sections; otherwise, <c>false</c>.
        /// </returns>
        public bool GetCustomerPortalWebSiteAndApp(out string webSite, out string webApplication)
        {
            webSite = null;
            webApplication = null;

            if (String.IsNullOrWhiteSpace(CustomerPortalWebSite))
                return false;

            var webSiteApp = CustomerPortalWebSite.Split('\\');

            if (webSiteApp.Length == 2)
            {
                webSite = webSiteApp[0];
                webApplication = webSiteApp[1];
                return true;
            }

            return false;
        }

        #endregion
    }
}

