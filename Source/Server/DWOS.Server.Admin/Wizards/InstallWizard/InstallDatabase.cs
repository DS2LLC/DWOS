using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Shared;
using DWOS.Shared.Wizard;
using NLog;

namespace DWOS.Server.Admin.Wizards.InstallWizard
{
    /// <summary>
    /// The 'install SQL Server' step of the installation wizard.
    /// </summary>
    public partial class InstallDatabase : UserControl, IWizardPanel
    {
        #region Fields

        private bool _inChkExisting;
        private bool _inChkInstall;

        private bool _installed;
        private string _sqlExpressPath;
        private string _configFilePath;

        #endregion

        #region Methods

        public InstallDatabase()
        {
            InitializeComponent();
        }

        private void StartDownload()
        {
            try
            {
                OnValidStateChanged?.Invoke(this);

                this.btnInstall.Enabled = false; //disable button from being clicked again after first click...

                this.prgDownload.Value = 0;

                var webClient = new WebClient();
                webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;
                webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;

                this._sqlExpressPath = Path.Combine(Path.GetTempPath(), "SQLServer2017-SSEI-Expr.exe");
                _configFilePath = NewConfigFile();

                try
                {
                    if(File.Exists(this._sqlExpressPath))
                        File.Delete(this._sqlExpressPath);
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Unable to delete installer.");
                }

                this.lblDownloadStatus.Text = "Downloading...";
                webClient.DownloadFileAsync(new Uri(Properties.Settings.Default.SQLServerInstallURL), this._sqlExpressPath);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error downloading SQL Server Express.");
            }
        }

        private void StartInstall()
        {
            try
            {
                var ableToInstall = !String.IsNullOrEmpty(this._sqlExpressPath)
                    && File.Exists(this._sqlExpressPath)
                    && !string.IsNullOrEmpty(_configFilePath)
                    && File.Exists(_configFilePath);

                if (ableToInstall)
                {
                    var args = string.Format(Properties.Settings.Default.SQLServerInstallArgsFormat, _configFilePath);
                    var psi = new ProcessStartInfo(this._sqlExpressPath, args);
                    Process process = Process.Start(psi);
                    process.WaitForExit();

                    this.prgInstall.Value = 100;
                    this.lblInstallStatus.Text = "Completed";
                    this._installed = true;

                    //set default connection string based on new database
                    var connBuilder = new SqlConnectionStringBuilder();
                    connBuilder.PersistSecurityInfo = true;
                    connBuilder.UserID = "sa";
                    connBuilder.Password = "P@ssword!";
                    connBuilder.DataSource = Environment.MachineName + @"\SQLEXPRESS";
                    ServerSettings.Default.DBConnectionString = connBuilder.ConnectionString;
                }
            }
            catch(Exception exc)
            {
                this._installed = false;
                ErrorMessageBox.ShowDialog("Error installing SQL Server Express.", exc);
            }
            finally
            {
                OnValidStateChanged?.Invoke(this);

                CleanUp();
            }
        }

        private string NewConfigFile()
        {
            const string configuration = "[OPTIONS]\r\n"
                + "ACTION=\"INSTALL\"\r\n"
                + "FEATURES=\"SQL\"\r\n"
                + "INSTANCENAME=\"SQLEXPRESS\"\r\n"
                + "Enableranu=1\r\n"
                + "SECURITYMODE=\"SQL\"\r\n"
                + "SAPWD=\"P@ssword!\"\r\n"
                + "NPENABLED=1\r\n"
                + "TCPENABLED=1\r\n"
                + "BROWSERSVCSTARTUPTYPE=\"Automatic\"\r\n";

            var configFileName = Path.Combine(Path.GetTempPath(), "SQLConfig.ini");
            File.WriteAllText(configFileName, configuration, Encoding.ASCII);

            return configFileName;
        }

        private void CleanUp()
        {
            try
            {
                if (!String.IsNullOrEmpty(this._sqlExpressPath) && File.Exists(this._sqlExpressPath))
                    File.Delete(this._sqlExpressPath);

                if (!string.IsNullOrEmpty(_configFilePath) && File.Exists(_configFilePath))
                    File.Delete(_configFilePath);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Unable to delete installer & config file.");
            }
        }

        #endregion

        #region Events

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) { this.prgDownload.Value = e.ProgressPercentage; }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.lblDownloadStatus.Text = "Complete";
            this.prgDownload.Value = 100;
            StartInstall();
        }

        private void chkUseExistingDB_CheckedChanged(object sender, EventArgs e)
        {
            if(!this._inChkInstall)
            {
                this._inChkExisting = true;
                this.chkInstallDB.Checked = false;
                this._installed = this.chkUseExistingDB.Checked;
                this.grpInstall.Enabled = this.chkInstallDB.Checked;

                OnValidStateChanged?.Invoke(this);

                this._inChkExisting = false;
            }
        }

        private void chkInstallDB_CheckedChanged(object sender, EventArgs e)
        {
            if(!this._inChkExisting)
            {
                this._inChkInstall = true;
                this.grpInstall.Enabled = this.chkInstallDB.Checked;
                this.chkUseExistingDB.Checked = false;
                this._inChkInstall = false;
            }
        }

        private void btnInstall_Click(object sender, EventArgs e) { StartDownload(); }

        #endregion

        #region IWizardPanel

        public string Title
        {
            get { return "Install Database Server"; }
        }

        public string SubTitle
        {
            get { return "Install the Microsoft SQL Server database."; }
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

        public void OnMoveTo() { }

        public void OnMoveFrom() { }

        public void OnFinished() { }

        #endregion
    }
}