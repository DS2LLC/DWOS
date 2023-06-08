using DWOS.Shared.Data;
using Newtonsoft.Json;

namespace DWOS.Data
{
    /// <summary>
    /// Contains scanner settings.
    /// </summary>
    public sealed class ScannerSettings
    {
        #region Fields

        public const int MIN_SCAN_RESOLUTION = 50;
        public const int MAX_SCAN_RESOLUTION = 300;
        public const int DEFAULT_SCAN_RESOLUTION = 100;

        public const int MIN_SCAN_QUALITY = 10;
        public const int MAX_SCAN_QUALITY = 100;
        public const int DEFAULT_SCAN_QUALITY = 60;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the scanner's name.
        /// </summary>
        public string ScanDeviceName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating use of the full scanner UI.
        /// </summary>
        /// <value>
        /// <c>true</c> to show the full scanner UI; otherwise, <c>false</c>.
        /// </value>
        public bool ScanShowFullUI { get; set; }

        /// <summary>
        /// Gets or sets a numerical value indicating the quality of
        /// scanned JPEG images.
        /// </summary>
        public int ScanQuality { get; set; }

        /// <summary>
        /// Gets or sets the resolution to use when scanning images.
        /// </summary>
        public int ScanResolution { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if scans documents should use PDF.
        /// </summary>
        /// <value>
        /// <c>true</c> to use PDF; <c>false</c> to use multi-page TIFF.
        /// </value>
        public bool ScanOutputPDF { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a default instance based on current user settings.
        /// </summary>
        /// <param name="userSettings"><see cref="UserSettings"/>
        /// instance to retrieve values from.
        /// </param>
        /// <returns>A new <see cref="ScannerSettings"/> instance.</returns>
        public static ScannerSettings DefaultFrom(UserSettings userSettings)
        {
            return new ScannerSettings
            {
                ScanDeviceName = userSettings.ScanDeviceName,
                ScanShowFullUI = userSettings.ScanShowFullUI,
                ScanQuality = userSettings.ScanQuality,
                ScanResolution = userSettings.ScanResolution,
                ScanOutputPDF = userSettings.ScanOutputPDF
            };
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
            if (ScanResolution < MIN_SCAN_RESOLUTION || ScanResolution > MAX_SCAN_RESOLUTION)
            {
                ScanResolution = DEFAULT_SCAN_RESOLUTION;
            }

            if (ScanQuality < MIN_SCAN_QUALITY || ScanQuality > MAX_SCAN_QUALITY)
            {
                ScanQuality = DEFAULT_SCAN_QUALITY;
            }
        }

        #endregion

        #region JsonConverter

        /// <summary>
        /// Converts <see cref="ScannerSettings"/> instances to/from
        /// JSON strings.
        /// </summary>
        public sealed class JsonConverter : IFieldConverter
        {
            #region IFieldConverter Members

            public object ConvertFromField(object value)
            {
                if (value == null)
                {
                    return null;
                }

                return JsonConvert.DeserializeObject(value.ToString(), typeof(ScannerSettings));
            }

            public object ConvertToField(object value)
            {
                if (value == null)
                {
                    return null;
                }

                return JsonConvert.SerializeObject(value,
                    Formatting.None,
                    new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.None });
            }

            #endregion
        }

        #endregion
    }
}
