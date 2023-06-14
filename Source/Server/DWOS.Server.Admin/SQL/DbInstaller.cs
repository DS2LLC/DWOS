using DWOS.Shared;
using DWOS.Shared.Utilities;
using Ionic.Zip;
using NLog;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Security.AccessControl;
using System.Windows.Forms;

namespace DWOS.Server.Admin.SQL
{
    /// <summary>
    /// Performs setup and installation of DWOS's database.
    /// </summary>
    internal sealed class DbInstaller
    {
        #region Fields

        private const string INSTALL_ZIP_NAME = "DWOS.Database.zip";
        private const string INSTALL_FOLDER_NAME = "Database";
        private const int COUNT_CANCELLED = -1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the notifier for this instance.
        /// </summary>
        public IUserNotifier Notifier
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="DbInstaller"/> class.
        /// </summary>
        /// <param name="notifier"></param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="notifier"/> is null.
        /// </exception>
        public DbInstaller(IUserNotifier notifier)
        {
            if (notifier == null)
            {
                throw new ArgumentNullException(nameof(notifier));
            }

            Notifier = notifier;
        }

        /// <summary>
        /// Installs the database.
        /// </summary>
        /// <param name="conn">An open connection.</param>
        /// <param name="dataFolder">Folder to save database files to.</param>
        /// <param name="dbName">Name of the database.</param>
        public void Install(SqlConnection conn, string dataFolder, string dbName)
        {
            Notifier.ShowNotification("Begin database creation.");

            //Copy files to folder
            int fileCount = CopyFiles(dataFolder);

            if (fileCount == COUNT_CANCELLED)
                return;

            Notifier.ShowNotification("{0} Files copied.".FormatWith(fileCount));

            string mdfFile = Path.Combine(dataFolder, "DWOS.mdf");
            string ldfFile = Path.Combine(dataFolder, "DWOS.ldf");

            //Attach Database
            using (SqlCommand command = conn.CreateCommand())
            {
                //http://msdn.microsoft.com/en-us/library/ms190209.aspx
                //Create the database using Attach
                Notifier.ShowNotification("Attaching DWOS database as {0}.".FormatWith(dbName));
                command.CommandText = string.Format("CREATE DATABASE {0} ON (FILENAME = '{1}'), (FILENAME = '{2}') FOR ATTACH;", dbName, mdfFile, ldfFile);
                command.ExecuteNonQuery();

                //Take DB offline
                Notifier.ShowNotification("Taking {0} database offline.".FormatWith(dbName));
                command.CommandText = string.Format("ALTER DATABASE {0} SET OFFLINE;", dbName);
                command.ExecuteNonQuery();

                //Change location of MDF file, then name is the Logical Name found in the properties of SQL Server instance.
                Notifier.ShowNotification("Changing file location DWOSData file to {0}.".FormatWith(mdfFile));
                command.CommandText = string.Format("ALTER DATABASE {0} MODIFY FILE ( NAME = 'DWOSData', FILENAME = '{1}' )", dbName, mdfFile);
                command.ExecuteNonQuery();

                //Change location of LDF file
                Notifier.ShowNotification("Changing file location DWOSData_log file to {0}.".FormatWith(ldfFile));
                command.CommandText = string.Format("ALTER DATABASE {0} MODIFY FILE ( NAME = 'DWOSData_log', FILENAME = '{1}' )", dbName, ldfFile);
                command.ExecuteNonQuery();

                //Put DB online
                Notifier.ShowNotification("Putting {0} database online.".FormatWith(dbName));
                command.CommandText = string.Format("ALTER DATABASE {0} SET ONLINE;", dbName);
                command.ExecuteNonQuery();
            }
        }

        private int CopyFiles(string folder)
        {
            int fileCount = 0;

            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                    FileSystem.AddDirectoryPermission(folder, FileSystemRights.FullControl, FileSystem.AccountType.AuthenticatedUsers);
                }

                string sourceFile = Path.Combine(Application.StartupPath, INSTALL_FOLDER_NAME, INSTALL_ZIP_NAME);

                Notifier.ShowNotification("Copying database files.");

                ZipFile zipFile = ZipFile.Read(sourceFile);

                foreach (ZipEntry zipEntry in zipFile)
                {
                    Notifier.ShowNotification(string.Format("Extracting file {0} to folder {1}.", zipEntry.FileName, folder));
                    string extractedFile = Path.Combine(folder, zipEntry.FileName);

                    if (File.Exists(extractedFile))
                    {
                        DialogResult result = MessageBox.Show("The file '{0}' already exists, do you want to overwrite it?".FormatWith(extractedFile), About.ApplicationName, MessageBoxButtons.YesNoCancel);
                        if (result == DialogResult.No)
                            continue;
                        if (result == DialogResult.Cancel)
                            return COUNT_CANCELLED;
                    }

                    zipEntry.Extract(folder, ExtractExistingFileAction.OverwriteSilently);
                    FileSystem.AddFilePermission(extractedFile, FileSystemRights.FullControl, FileSystem.AccountType.AuthenticatedUsers);
                    fileCount++;
                }

                return fileCount;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error installing service.");
                return 0;
            }
        }

        #endregion
    }
}
