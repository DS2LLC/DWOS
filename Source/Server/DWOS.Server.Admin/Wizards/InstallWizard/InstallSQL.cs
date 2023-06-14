using DWOS.Data;
using DWOS.Server.Admin.SQL;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.Shared.Wizard;
using Microsoft.Win32;
using NLog;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DWOS.Server.Admin.Wizards.InstallWizard
{
    /// <summary>
    /// The 'install database' step of the installation wizard.
    /// </summary>
    public partial class InstallSQL : UserControl, IWizardPanel, IUserNotifier
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private bool _installed;

        #endregion

        #region Methods

        public InstallSQL() { InitializeComponent(); }

        private void InstallDB()
        {
            try
            {
                this.txtStatus.Text = "";

                if(ServerSettings.Default.DBConnectionString != null)
                {
                    string dataFolder = this.txtFilePath.Text;
                    string dbName = this.txtDBName.Text;

                    using(var conn = new SqlConnection(ServerSettings.Default.DBConnectionString))
                    {
                        conn.Open();
                        var installer = new DbInstaller(this);
                        installer.Install(conn, dataFolder, dbName);
                    }

                    //Update connection string to include this Initial Catalog...
                    var connBuilder = new SqlConnectionStringBuilder(ServerSettings.Default.DBConnectionString) {InitialCatalog = dbName};

                    //save connection info to this DB
                    ServerSettings.Default.DBConnectionString = connBuilder.ConnectionString;
                    ServerSettings.Default.Save();
                    UpdateCustomerPortal();

                    //Validate connection
                    if(ValidateDB())
                    {
                        UpdateDB();

                        MessageBox.Show("Successfully connected to the database.", About.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this._installed = true;
                    }
                    else
                        MessageBox.Show("Unable to verify the database tables.", About.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch(SqlException sqlExc)
            {
                if(sqlExc.Number == 5120)
                    MessageBox.Show("SQL Server does not have access to the folder {0}, please ensure the account running the SQL Server service has full control permissions on the folder.".FormatWith(this.txtFilePath.Text), About.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                else
                    LogManager.GetCurrentClassLogger().Error("SQL Error attaching DB.", sqlExc);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error installing DB.");
            }
        }

        private void UpdateDB()
        {
            using (var conn = new SqlConnection(ServerSettings.Default.DBConnectionString))
            {
                conn.Open();
                var fromVersion = SqlUtility.GetVersion(conn)?.ToMajorMinorBuild();
                var toVersion = new Version(About.ApplicationVersion).ToMajorMinorBuild();

                if (fromVersion != null && fromVersion < toVersion)
                {
                    var upgrader = new DbUpgrader(this);
                    var result = upgrader.UpgradeDb(conn, fromVersion, toVersion);

                    if (!(result?.HasErrors ?? false))
                    {
                        ServerSettings.Default.Version = About.ApplicationVersion;
                        ServerSettings.Default.Save();
                    }
                }
            }
        }

        private bool ValidateDB()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if(ServerSettings.Default.DBConnectionString != null)
                {
                    var connBuilder = new SqlConnectionStringBuilder(ServerSettings.Default.DBConnectionString);
                    if(!String.IsNullOrWhiteSpace(connBuilder.InitialCatalog))
                    {
                        this.txtDBName.Text = connBuilder.InitialCatalog;

                        using(var conn = new SqlConnection(ServerSettings.Default.DBConnectionString))
                        {
                            conn.Open();

                            try
                            {
                                //OPTIONAL
                                using(SqlCommand command = conn.CreateCommand())
                                {
                                    //attempt to get file location
                                    command.CommandText = String.Format("SELECT * FROM sys.master_files WHERE database_id = DB_ID(N'{0}')", connBuilder.InitialCatalog);
                                    using(SqlDataReader reader = command.ExecuteReader())
                                    {
                                        if(reader.Read())
                                        {
                                            string path = reader["physical_name"].ToString();
                                            this.txtFilePath.Text = Path.GetDirectoryName(path);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }

                            using(SqlCommand command = conn.CreateCommand())
                            {
                                ShowNotification("Validating connection to database.");
                                command.CommandText = "SELECT COUNT(*) FROM ApplicationSettings";
                                object settingsCount = command.ExecuteScalar();

                                if(settingsCount != null)
                                {
                                    ShowNotification("Successfully connected to the database.");
                                    return true;
                                }
                                else
                                {
                                    ShowNotification("Unable to verify the database tables.");
                                    return false;
                                }
                            }
                        }
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void UpdateCustomerPortal()
        {
            _log.Info("Begin update Customer Portal connection string.");

            try
            {
                RegistryKey serverKey = Registry.LocalMachine.OpenSubKey(@"Software\Dynamic Software Solutions\DWOS Server", false);

                if (serverKey == null)
                {
                    RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                    serverKey = localMachine64.OpenSubKey(@"Software\Dynamic Software Solutions\DWOS Server", false);
                }

                if (serverKey != null)
                {
                    object webConfigPath = serverKey.GetValue("Web");

                    if (webConfigPath != null && File.Exists(webConfigPath.ToString()))
                    {
                        FileSystem.SetFileAttributes(webConfigPath.ToString(), FileAttributes.Normal);

                        XDocument document = XDocument.Load(webConfigPath.ToString());
                        XElement node = document.XPathSelectElement("//add[@name='DWOS.Data.Properties.Settings.DWOSDataConnectionString']");
                        node.Attribute("connectionString").Value = ServerSettings.Default.DBConnectionString;
                        document.Save(webConfigPath.ToString());

                        _log.Info("Updated Customer Portal web config with connection string.");
                    }
                    else
                        _log.Info("Customer Portal not installed.");
                }
                else
                    _log.Info(@"Unable to open registry key: Software\Dynamic Software Solutions\DWOS Server.");
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error attempting to update customer portal connection string.");
            }
        }

        #endregion

        #region Events

        private void btnInstall_Click(object sender, EventArgs e)
        {
            InstallDB();

            OnValidStateChanged?.Invoke(this);
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            try
            {
                using(var folderDlg = new FolderBrowserDialog())
                {
                    folderDlg.Description = "Location to store data files";

                    if(folderDlg.ShowDialog(this) == DialogResult.OK)
                        this.txtFilePath.Text = folderDlg.SelectedPath;
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting folder.");
            }
        }

        #endregion

        #region IWizardPanel

        public string Title
        {
            get { return "Install DWOS Data"; }
        }

        public string SubTitle
        {
            get { return "Install the DWOS data in your database."; }
        }

        public Action <IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller) { }

        public Control PanelControl
        {
            get { return this; }
        }

        public bool IsValid
        {
            get { return this._installed; }
        }

        public void OnMoveTo()
        {
            this.txtFilePath.Text = @"C:\Data";
            this.txtDBName.Text = @"DWOS";

            this._installed = ValidateDB();

            OnValidStateChanged?.Invoke(this);

            if(this._installed)
                MessageBox.Show("Database is installed and running, you can skip this step.", About.ApplicationName, MessageBoxButtons.OK);
        }

        public void OnMoveFrom() { }

        public void OnFinished() { }

        #endregion

        #region IUserNotifier Members

        public void ShowNotification(string message)
        {
            this.txtStatus.Text += message + Environment.NewLine;
            _log.Info(message);
            this.txtStatus.ScrollToCaret();
        }

        #endregion
    }
}