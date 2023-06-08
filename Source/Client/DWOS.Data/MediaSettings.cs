using DWOS.Shared.Data;
using DWOS.Shared.Settings;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Reflection;

namespace DWOS.Data
{
    /// <summary>
    /// Defines user settings for media widgets.
    /// </summary>
    public sealed class MediaSettings : NestedRegistrySettingsBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets scanner settings for part widgets.
        /// </summary>
        /// <remarks>
        /// Used by <see cref="Get(ScannerSettingsType)"/> and
        /// <see cref="Set(ScannerSettingsType, ScannerSettings)"/>.
        /// </remarks>
        [ScannerType(ScannerSettingsType.Part)]
        [DataColumn(FieldConverterType = typeof(ScannerSettings.JsonConverter))]
        public ScannerSettings PartWidgetSettings { get;  set;}

        /// <summary>
        /// Gets or sets scanner settings for order widgets.
        /// </summary>
        /// <remarks>
        /// Used by <see cref="Get(ScannerSettingsType)"/> and
        /// <see cref="Set(ScannerSettingsType, ScannerSettings)"/>.
        /// </remarks>
        [ScannerType(ScannerSettingsType.Order)]
        [DataColumn(FieldConverterType = typeof(ScannerSettings.JsonConverter))]
        public ScannerSettings OrderWidgetSettings { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaSettings"/>
        /// class.
        /// </summary>
        /// <param name="hive"></param>
        /// <param name="baseKey"></param>
        public MediaSettings(RegistryKey hive, string baseKey)
            : base(hive, baseKey)
        {

        }

        /// <summary>
        /// Retrieves the value associated with the given
        /// <see cref="ScannerSettingsType"/>.
        /// </summary>
        /// <param name="type">Type to search for.</param>
        /// <returns>Value of the property associated with type.</returns>
        /// <exception cref="ArgumentException">Property not found.</exception>
        public ScannerSettings Get(ScannerSettingsType type)
        {
            var foundProperty = FindProperty(type);

            if (foundProperty == null)
            {
                throw new ArgumentException("type", "Could not find property for given type.");
            }

            return foundProperty.GetValue(this, null) as ScannerSettings;
        }
        /// <summary>
        ///  Sets the property assoicated with the given
        /// <see cref="ScannerSettingsType"/>.
        /// </summary>
        /// <param name="type">Type to search for.</param>
        /// <param name="value">The new value of the property.</param>
        /// <exception cref="ArgumentException">Property not found.</exception>
        public void Set(ScannerSettingsType type, ScannerSettings value)
        {
            var foundProperty = FindProperty(type);

            if (foundProperty == null)
            {
                throw new ArgumentException("type", "Could not find property for given type.");
            }

            value?.ValidateSettings();
            foundProperty.SetValue(this, value, null);
        }

        private static PropertyInfo FindProperty(ScannerSettingsType type)
        {
            var properties = typeof(MediaSettings).GetProperties();

            PropertyInfo foundProperty = null;
            foreach (PropertyInfo prop in properties)
            {
                if (prop == null)
                {
                    continue;
                }

                var attr = prop.GetCustomAttributes(typeof(ScannerTypeAttribute), false)
                    .FirstOrDefault() as ScannerTypeAttribute;

                if (attr?.SettingsType == type)
                {
                    foundProperty = prop;
                    break;
                }
            }



            return foundProperty;
        }

        #endregion
    }
}
