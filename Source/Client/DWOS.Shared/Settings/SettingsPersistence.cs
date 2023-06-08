using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DWOS.Shared.Settings
{
    /// <summary>
    /// Implementation of <see cref="ISettingsPersistence"/> that uses the primary
    /// database.
    /// </summary>
    public sealed class SettingsPersistence : ISettingsPersistence
    {
        #region Methods

        /// <summary>
        /// Gets or sets the table name for this instance.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the name column for this instance.
        /// </summary>
        public string SettingNameColumn { get; set; }

        /// <summary>
        /// Gets or sets the value column for this instance.
        /// </summary>
        public string SettingValueColumn { get; set; }

        #endregion

        #region ISettingsPersistence Members

        public string ConnectionString { get; set; }

        public void LoadFromDataStore(SettingValue setting)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("SELECT {0} FROM {1} WHERE {2} = @settingName".FormatWith(SettingValueColumn, TableName, SettingNameColumn), connection))
                {
                    command.Parameters.AddWithValue("@settingName", setting.Name ?? string.Empty);
                    var obj = command.ExecuteScalar();
                    setting.Value = obj != null && obj != DBNull.Value ? obj.ToString() : null;
                    setting.ConvertedValue = null;
                }
            }
        }

        public void SaveToDataStore(IEnumerable<SettingValue> settings)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                foreach (var setting in settings)
                {
                    int result;
                    using (var updateCmd = new SqlCommand("UPDATE {0} SET {1} = @settingValue WHERE {2} = @settingName".FormatWith(TableName, SettingValueColumn, SettingNameColumn), connection))
                    {
                        updateCmd.Parameters.AddWithValue("@settingValue", setting.Value ?? string.Empty);
                        updateCmd.Parameters.AddWithValue("@settingName", setting.Name ?? string.Empty);
                        result = updateCmd.ExecuteNonQuery();
                    }

                    //if didnt update then must need to insert
                    if (result != 1)
                    {
                        using (var insertCmd = new SqlCommand("INSERT INTO {0} ({1}, {2}) VALUES (@settingName, @settingValue)".FormatWith(TableName, SettingNameColumn, SettingValueColumn), connection))
                        {
                            insertCmd.Parameters.AddWithValue("@settingValue", setting.Value ?? string.Empty);
                            insertCmd.Parameters.AddWithValue("@settingName", setting.Name ?? string.Empty);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public void ValidateConnection()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
            }
        }

        #endregion
    }
}
