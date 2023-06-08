using DWOS.Shared.Utilities;
using Newtonsoft.Json;
using NLog;
using System;
using System.IO;

namespace DWOS.UI.Utilities
{
    public class GridSettingsPersistence<T> where T : class
    {
        #region Fields

        private const string SettingsDirectory = "Grids";

        #endregion

        #region Properties

        public string GridName { get; }

        public string ConfigPath
        {
            get
            {
                return Path.Combine(FileSystem.UserAppDataPath(),
                    SettingsDirectory,
                    $"{GridName}.json");
            }
        }

        private string ConfigDirectoryPath =>
            Path.Combine(FileSystem.UserAppDataPath(), SettingsDirectory);

        public T DefaultSettings { get; }

        #endregion

        #region Methods

        public GridSettingsPersistence(string gridName, T defaultSettings)
        {
            if (string.IsNullOrEmpty(gridName))
            {
                throw new ArgumentNullException(nameof(gridName));
            }

            GridName = gridName;
            DefaultSettings = defaultSettings ?? throw new ArgumentNullException(nameof(defaultSettings));

            if (!Directory.Exists(ConfigDirectoryPath))
            {
                Directory.CreateDirectory(ConfigDirectoryPath);
            }
        }

        public T LoadSettings()
        {
            try
            {
                FixConfigFileLocation();
                if (File.Exists(ConfigPath))
                {
                    return JsonConvert.DeserializeObject<T>(File.ReadAllText(ConfigPath)) ?? DefaultSettings;
                }

                return DefaultSettings;
            }
            catch (JsonSerializationException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error deserializing grid settings.");
                return DefaultSettings;
            }
        }

        public void SaveSettings(T settings)
        {
            if (settings == null)
            {
                return;
            }

            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(settings));
        }

        /// <summary>
        /// Moves existing configuration files to the correct location.
        /// </summary>
        private void FixConfigFileLocation()
        {
            if (GridName != "OrderProcess" && GridName != "PartProcess")
            {
                return;
            }

            var oldOrderProcessesPath = Path.Combine(FileSystem.UserAppDataPath(), $"{GridName}.json");
            if (File.Exists(oldOrderProcessesPath) && !File.Exists(ConfigPath))
            {
                // Move old config file to the new file path
                File.Move(oldOrderProcessesPath, ConfigPath);
            }
        }

        #endregion
    }
}
