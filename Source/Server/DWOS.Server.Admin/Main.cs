using DWOS.Data;
using DWOS.Shared;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace DWOS.Server.Admin
{
    /// <summary>
    /// Main form of the server administration application.
    /// </summary>
    public partial class Main: Form
    {
        #region Fields
        
        private string _updaterModulePath;

        #endregion

        #region Methods
        
        public Main()
        {
            this.InitializeComponent();
        }
        
        private void CheckForUpdatesSilently()
        {
            int sleepMilliseconds = 10000; // 10 seconds

            Thread.Sleep(sleepMilliseconds);

            if (File.Exists(_updaterModulePath))
            {
                Process process = Process.Start(_updaterModulePath, "/silent");
                process.Close();
            }
        }
        
        /// <summary>
        /// Automatically start the setup wizard after license check if command line arg of 'firstRun' is passed in.
        /// </summary>
        /// <param name="isActivated">if set to <c>true</c> [is activated].</param>
        private void AutoStartWizard()
        {
            try
            {
                //if no version set then run NEW install wizard
                if(String.IsNullOrWhiteSpace(ServerSettings.Default.Version))
                {
                    if(Wizards.InstallWizard.InstallWizardController.StartWizard() == System.Windows.Forms.DialogResult.OK)
                        UpdateStatus();
                }
                else
                {
                    //major.minor.build.revision
                    var installedVersion    = new Version(ServerSettings.Default.Version);
                    var currentVersion      = new Version(About.ApplicationVersion);

                    installedVersion    = new Version(installedVersion.Major, installedVersion.Minor, installedVersion.Build);
                    currentVersion      = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build);

                    //if running a newer Server Admin tool
                    if (currentVersion> installedVersion)
                    {
                        if(Wizards.UpgradeWizard.UpgradeWizardController.StartWizard() == System.Windows.Forms.DialogResult.OK)
                        {
                            UpdateStatus();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error starting wizard.");
            }
        }

        private void UpdateStatus()
        {
            try
            {
                //this.licenseStatus1.UpdateStatus();
                this.databaseStatus1.BeginUpdateStatus();
                this.serverStatus1.BeginUpdateStatus();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error updating status.");
            }
        }

        public static void ForceServerToReload()
        {
            const string errorMessage = "Error reloading license info.";

            try
            {
                var client = new Licensing.LicenseServiceClient();
                client.Open();
                client.ReloadCompanyInfoAsync();
            }
            catch (EndpointNotFoundException endpointException)
            {
                // Server is not running.
                LogManager.GetCurrentClassLogger()
                    .Warn(endpointException, errorMessage);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, errorMessage);
            }
        }

        /// <summary>
        /// Opens a Explorer window view with the specific folder, file, or application selected.
        /// </summary>
        /// <param name="pathToSelect"></param>
        private static void OpenExplorerWithSelection(string pathToSelect)
        {
            string argument = @"/select, " + pathToSelect.Replace("\\/", "\\").Replace("/", "\\");
            Process.Start("explorer.exe", argument);
        }
        
        #endregion

        #region Events
        
        private void Main_Load(object sender, EventArgs e)
        {
            if(DesignMode)
                return;

            _updaterModulePath = Path.Combine(Application.StartupPath, "updater.exe");

            // Start update thread
            var thread = new Thread(new ThreadStart(CheckForUpdatesSilently));
            thread.Start();
        }

        private void setupWizardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Wizards.InstallWizard.InstallWizardController.StartWizard();
            UpdateStatus();
        }

        private void configureUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(_updaterModulePath))
            {
                Process process = Process.Start(_updaterModulePath, "/configure");
                process.Close();
            }
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(_updaterModulePath))
            {
                Process process = Process.Start(_updaterModulePath, "/checknow");
                process.Close();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Main_Shown(object sender, EventArgs e) { AutoStartWizard(); }

        private void upgradWizardMenuItem_Click(object sender, EventArgs e)
        {
            if (Wizards.UpgradeWizard.UpgradeWizardController.StartWizard() == System.Windows.Forms.DialogResult.OK)
            {
                UpdateStatus();
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new DWOS.Server.Admin.SettingsPanels.Settings())
            {
                settingsForm.ShowDialog(Form.ActiveForm);
            }
        }
     
        private void openLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenExplorerWithSelection(NLogExtensions.FindLogFileName());
        }

        private void openServerLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string serverKeyName = @"SYSTEM\CurrentControlSet\Services\DWOSServer";
            const string errorMessage = "Cannot open server logs.";

            try
            {
                string serverExePath = string.Empty;
                using (var serverRegistry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(serverKeyName, false))
                {
                    if (serverRegistry != null)
                    {
                        string imagePath = Convert.ToString(serverRegistry.GetValue("ImagePath"));
                        var fileNameMatch = Regex.Match(imagePath, "^\"[^\"]*\"");

                        if (fileNameMatch.Success)
                        {
                            serverExePath = fileNameMatch.Value.Trim(new char[] { '"' });
                        }
                    }
                }

                var serverLogDir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(serverExePath), "Logs"));
                if (serverLogDir.Exists)
                {
                    FileSystemInfo mostRecentLogFile = serverLogDir.GetFileSystemInfos()
                        .OrderByDescending(i => i.CreationTime)
                        .FirstOrDefault();

                    if (mostRecentLogFile == null)
                    {
                        Process.Start("explorer.exe", serverLogDir.FullName);
                    }
                    else
                    {
                        OpenExplorerWithSelection(mostRecentLogFile.FullName);
                    }
                }
                else
                {
                    MessageBox.Show(errorMessage, "Show Server Logs");
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog(errorMessage, exc);
            }
        }

        private void backupDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var backupForm = new Tasks.DatabaseBackupDialog())
                {
                    backupForm.ShowDialog(ActiveForm);
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error backing up database.", exc);
            }
        }

        #endregion
    }
}