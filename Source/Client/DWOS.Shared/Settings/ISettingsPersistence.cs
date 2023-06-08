using System;
using System.Collections.Generic;

namespace DWOS.Shared.Settings
{
    /// <summary>
    /// Interface for classes implementing settings persistence.
    /// </summary>
    public interface ISettingsPersistence
    {
        /// <summary>
        /// Gets or sets the connection string for the instance.
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Loads a setting from persistence.
        /// </summary>
        /// <param name="setting"></param>
        void LoadFromDataStore(SettingValue setting);

        /// <summary>
        /// Saves settings to persistence.
        /// </summary>
        /// <param name="settings"></param>
        void SaveToDataStore(IEnumerable<SettingValue> settings);

        /// <summary>
        /// Validates the connection to persistence.
        /// </summary>
        /// <remarks>
        /// Implementers should throw an exception if validation fails.
        /// </remarks>
        void ValidateConnection();
    }
}