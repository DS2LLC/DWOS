using System;
using System.Linq;
using System.Runtime.Serialization;
using DWOS.Shared.Utilities;
using Newtonsoft.Json;
using System.IO;
using NLog;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Represents a connection file.
    /// </summary>
    /// <remarks>
    /// A file that contains info used to connect to DWOS Server. This file will be used as an alternative to provide connection details with a file, instead of allowing the user
    /// to select the server connection. This will also allow a custom database connection string to use instead of the one from the DWOS server.
    /// </remarks>
    [DataContract]
    public class ConnectionFile
    {
        #region Properties

        /// <summary>
        /// Gets or sets the server address.
        /// </summary>
        [DataMember]
        public string ServerAddress { get; set; }

        /// <summary>
        /// Gets or sets the server port.
        /// </summary>
        [DataMember]
        public int ServerPort { get; set; }

        /// <summary>
        /// Gets or sets the database connection string.
        /// </summary>
        [DataMember]
        public string DatabaseConnection { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the connection file from the users desktop.
        /// </summary>
        /// <returns>ConnectionFile.</returns>
        public static ConnectionFile LoadConnectionFile()
        {
#if DEBUG
            // Always show the 'select server' dialog for developers.
            // Otherwise, it makes it very easy to connect to a server where the client
            // has breaking changes.
            const int minDialogFileCount = 0;
#else
            const int minDialogFileCount = 1;
#endif

            try
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (!Directory.Exists(path))
                {
                    LogManager.GetCurrentClassLogger().Warn("Desktop directory does not exist: {0}", path);
                    return null;
                }

                var connectionFiles = Directory
                    .GetFiles(path, "*.dwos")
                    .ToList();

                string file = connectionFiles.FirstOrDefault();

                if (connectionFiles.Count > minDialogFileCount)
                {
                    using (var cbo = new ComboBoxForm() { Text = "DWOS Connection", StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen })
                    {
                        cbo.TopMost = true; // Always show above splash screen
                        cbo.chkOption.Visible = false;

                        foreach (var connFile in connectionFiles)
                        {
                            cbo.ComboBox.Items.Add(connFile, Path.GetFileNameWithoutExtension(connFile));
                        }

                        cbo.ComboBox.SelectedIndex = 0;
                        cbo.FormLabel.Text = "Connection:";

                        if (cbo.ShowDialog() == System.Windows.Forms.DialogResult.OK && cbo.ComboBox.SelectedItem != null)
                            file = cbo.ComboBox.SelectedItem.DataValue.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(file))
                {
                    var connectionJson = FileSystem.Decode(File.ReadAllText(file));
                    return JsonConvert.DeserializeObject<ConnectionFile>(connectionJson);
                }

                return null;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading connection file.");
                return null;
            }
        }

        public static void SaveConnectionFile(string name, ConnectionFile connFile)
        {
            try
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                var file = Path.Combine(path, name + ".dwos");

                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                var encodedJson = FileSystem.Encode(JsonConvert.SerializeObject(connFile));
                File.WriteAllText(file, encodedJson);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error saving connection file.");
            }
        }

#if (DEBUG || NIGHTLY)

        /// <summary>
        /// Creates default connection files that may be useful to developers.
        /// </summary>
        public static void CreateSampleConnectionFile()
        {
            SaveConnectionFile("DPS", new ConnectionFile() { ServerAddress = "192.119.238.21", ServerPort = 8080, DatabaseConnection = "Data Source=192.119.238.21,1433;Initial Catalog=DPSData;Integrated Security=False;Persist Security Info=True;User ID=sa;Password=trebor;MultipleActiveResultSets=True;Application Name=DWOS" });
            SaveConnectionFile("DPS Local", new ConnectionFile() { ServerAddress = "DPSData", ServerPort = 8080, DatabaseConnection = "Data Source=DPSData,1433;Initial Catalog=DPSData;Integrated Security=False;Persist Security Info=True;User ID=sa;Password=trebor;MultipleActiveResultSets=True;Application Name=DWOS" });
            SaveConnectionFile("DS2 Dev", new ConnectionFile() { ServerAddress = "DS2-DWOS-DEV-1", ServerPort = 8080, DatabaseConnection = "Data Source=DS2-DWOS-DEV-1;Initial Catalog=DWOS;Integrated Security=False;Persist Security Info=True;User ID=sa;Password=DS2M@ster!;MultipleActiveResultSets=True;Application Name=DWOS" });
        }
#endif

#endregion
    }
}
