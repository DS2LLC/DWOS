using DWOS.Shared.Data;
using DWOS.Shared.Settings;
using System;
using System.Linq;

namespace DWOS.Data
{
    /// <summary>
    /// Manages settings specific to the client application.
    /// </summary>
    public sealed class UserSettings : RegistrySettingsBase
    {
        #region Fields

        private static readonly Lazy<UserSettings> _lazyLoader = new Lazy<UserSettings>(() => new UserSettings());

        private const string KEY = "SOFTWARE\\DS2\\DWOS";
        private const string SERVER_ARG = "-SERVER";

        /// <summary>
        /// Address of the demo server.
        /// </summary>
        public const string DemoServerAddress = "www.mydwos.com";

        /// <summary>
        /// Port of the demo server.
        /// </summary>
        public const int DemoServerPort = 8080;

        /// <summary>
        /// Default value (in) seconds for <see cref="PresentationModeSpeed"/>.
        /// </summary>
        public const int DefaultPresentationModeSpeed = 5;

        #endregion

        #region Properties

        protected override string RegistryKeyName 
        {
            get { return KEY;  }
        }

        protected override Microsoft.Win32.RegistryKey RegistryHive
        {
            get { return Microsoft.Win32.Registry.CurrentUser; }
        }

        /// <summary>
        /// Gets the default <see cref="UserSettings"/> instance.
        /// </summary>
        public static UserSettings Default
        {
            get
            {
                return _lazyLoader.Value;
            }
        }

        /// <summary>
        /// Gets or sets the server address.
        /// </summary>
        [DataColumn]
        public string ServerAddress { get; set; }

        /// <summary>
        /// Gets or sets the server port.
        /// </summary>
        [DataColumn(DefaultValue = 8080)]
        public int ServerPort { get; set; }

        /// <summary>
        /// Gets or sets the default value for
        /// <see cref="ScannerSettings.ScanDeviceName"/>.
        /// </summary>
        [DataColumn]
        public string ScanDeviceName { get; set; }

        /// <summary>
        /// Gets or sets the default value for
        /// <see cref="ScannerSettings.ScanOutputPDF"/>.
        /// </summary>
        [DataColumn(DefaultValue=true)]
        public bool ScanOutputPDF { get; set; }

        /// <summary>
        /// Gets or sets the default value for
        /// <see cref="ScannerSettings.ScanQuality"/>.
        /// </summary>
        [DataColumn(DefaultValue =ScannerSettings.DEFAULT_SCAN_QUALITY)]
        public int ScanQuality { get; set; }

        /// <summary>
        /// Gets or sets the default value for
        /// <see cref="ScannerSettings.ScanResolution"/>.
        /// </summary>
        [DataColumn(DefaultValue =ScannerSettings.DEFAULT_SCAN_RESOLUTION)]
        public int ScanResolution { get; set; }

        /// <summary>
        /// Gets or sets the default value for
        /// <see cref="ScannerSettings.ScanShowFullUI"/>.
        /// </summary>
        [DataColumn]
        public bool ScanShowFullUI { get; set; }

        /// <summary>
        /// Gets or sets a collection of tabs to show.
        /// </summary>
        [DataColumn(FieldConverterType = typeof(TabInfoToJson))]
        public TabInfoCollection TabData { get; set; }

        /// <summary>
        /// Gets or sets the key of the last selected tab.
        /// </summary>
        [DataColumn]
        public string LastSelectedTab { get; set; }

        /// <summary>
        /// Gets or sets the path ot the document directory.
        /// </summary>
        [DataColumn]
        public string DocumentsWorkingDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value used to indicate if the application should
        /// remove layout files at startup.
        /// </summary>
        /// <value>
        /// <c>true</c> if files should be removed; otherwise, <c>false</c>.
        /// </value>
        [DataColumn]
        public bool ResetLayouts { get; set; }

        /// <summary>
        /// Gets or sets the name of the default printer.
        /// </summary>
        [DataColumn]
        public string DefaultPrinterName { get; set; }

        /// <summary>
        /// Gets or sets the name of the default shipping label.
        /// </summary>
        [DataColumn]
        public string ShippingLabelPrinterName { get; set; }

        /// <summary>
        /// Gets or sets the language to use when communicating with the
        /// shipping label printer.
        /// </summary>
        /// <remarks>
        /// This should be a Neodynamic.SDK.Printing.ProgrammingLanguage value.
        /// </remarks>
        [DataColumn]
        public string ShippingLabelPrinterLanguage { get; set; }

        [DataColumn(DefaultValue = 203)]
        public int LabelPrinterDpi { get; set; }

        /// <summary>
        /// Gets or sets the type of scale to use.
        /// </summary>
        [DataColumn(FieldConverterType= typeof(EnumToIntConverter<ScaleType>))]
        public ScaleType ScaleType { get; set; }

        /// <summary>
        /// Gets or sets the type of part marking device to use.
        /// </summary>
        [DataColumn(FieldConverterType= typeof(EnumToIntConverter<PartMarkingDeviceType>), DefaultValue = PartMarkingDeviceType.VideoJetExcel)]
        public PartMarkingDeviceType ParkMarkingType { get; set; }

        /// <summary>
        /// Gets or sets the name of the selected scale's port (if any).
        /// </summary>
        [DataColumn]
        public string ScalePortName { get; set; }

        /// <summary>
        /// Minimum amount of time, in seconds, to show each tab when Presentation Mode is enabled.
        /// </summary>
        [DataColumn(DefaultValue = DefaultPresentationModeSpeed)]
        public int PresentationModeSpeed { get; set; }

        /// <summary>
        /// Gets or sets a value to indicate if media should be deleted from
        /// the local system after upload.
        /// </summary>
        /// <value>
        /// <c>true</c> if files should be deleted; otherwise, <c>false</c>.
        /// </value>
        [DataColumn(DefaultValue = true)]
        public bool CleanupMediaAfterUpload { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of closed orders to show.
        /// </summary>
        /// <remarks>
        /// The number of closed orders can be very high, and this setting
        /// prevents Order Entry and other places in the application from
        /// showing too many closed orders.
        /// </remarks>
        [DataColumn(DefaultValue = 200)]
        public int MaxClosedOrders { get; set; }

        /// <summary>
        /// Gets or sets the value indicating if Order Entry filter should be
        /// exact by default (when available.)
        /// </summary>
        /// <returns>
        /// <c>true</c> if filters should be exact; otherwise, <c>false</c>.
        /// </returns>
        [DataColumn(DefaultValue = false)]
        public bool OrderEntryExactSearch { get; set; }

        /// <summary>
        /// Gets or sets media widget settings.
        /// </summary>
        [DataColumn]
        public MediaSettings Media { get; set; }

        /// <summary>
        /// Gets or sets shipping settings.
        /// </summary>
        [DataColumn]
        public ShippingSettings Shipping { get; set; }

        /// <summary>
        /// Gets or sets the directory last used to export invoices.
        /// </summary>
        [DataColumn]
        public string LastInvoiceDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the application should print
        /// work order travelers after split.
        /// </summary>
        /// <value>
        /// <c>true</c> if WO Travelers should be printed; otherwise,
        /// <c>false</c>.
        /// </value>
        [DataColumn(DefaultValue = false)]
        public bool SplitPrintTraveler { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the application should print
        /// a work order traveler after rejoin.
        /// </summary>
        /// <value>
        /// <c>true</c> if WO Travelers should be printed; otherwise,
        /// <c>false</c>.
        /// </value>
        [DataColumn(DefaultValue = false)]
        public bool RejoinPrintTraveler { get; set; }

        /// <summary>
        /// Gets or sets report settings.
        /// </summary>
        [DataColumn]
        public ReportSettings Report { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if 'quick print' should be checked by default.
        /// </summary>
        /// <value>
        /// <c>true</c> if 'quick print' is checked by default;
        /// otherwise, <c>false</c>.
        /// </value>
        [DataColumn(DefaultValue = true)]
        public bool QuickPrint { get; set; }

        #endregion

        #region Methods

        private UserSettings()
        {
            Load();
            LoadSettingsFromEnvironmentArgs();
        }

        private bool LoadSettingsFromEnvironmentArgs()
        {
            try
            {
                var args = Environment.GetCommandLineArgs().ToList();

                if (args.Contains(SERVER_ARG, StringComparer.CurrentCultureIgnoreCase))
                {
                    var comp = StringComparer.CurrentCultureIgnoreCase;
                    var serverIndex = args.IndexOf(s => comp.Compare(SERVER_ARG, s) == 0);

                    if (serverIndex > 0 && serverIndex < args.Count - 1)
                    {
                        var serverAddress   = args[serverIndex + 1];
                        var address         = serverAddress.Split(':');
                        
                        if (address.Length == 2)
                        {
                            this.ServerAddress = address[0];
                            this.ServerPort = Convert.ToInt32(address[1]);
                            
                            NLog.LogManager.GetCurrentClassLogger().Info("Loading server address settings from environment arg.");
                            return true;
                        }

                    }
                }

                return false;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading settings from environment variable.");
                return false;
            }
        }

        /// <summary>
        /// Saves this settings to the registry.
        /// </summary>
        public void Save()
        {
            ValidateSettings();
            base.SaveSettings();
        }

        /// <summary>
        /// Validates settings.
        /// </summary>
        /// <remarks>
        /// If <see cref="ScanResolution"/> or <see cref="ScanQuality"/> are
        /// out of range, they are set to default values.
        /// </remarks>
        public void ValidateSettings()
        {
            if (ScanResolution < ScannerSettings.MIN_SCAN_RESOLUTION || ScanResolution > ScannerSettings.MAX_SCAN_RESOLUTION)
            {
                ScanResolution = ScannerSettings.DEFAULT_SCAN_RESOLUTION;
            }

            if (ScanQuality < ScannerSettings.MIN_SCAN_QUALITY || ScanQuality > ScannerSettings.MAX_SCAN_QUALITY)
            {
                ScanQuality = ScannerSettings.DEFAULT_SCAN_QUALITY;
            }
        }

        #endregion
    }
}
