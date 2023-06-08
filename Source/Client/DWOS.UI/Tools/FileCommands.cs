using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderStatusDataSetTableAdapters;
using DWOS.Shared;
using DWOS.UI.Documents;
using DWOS.UI.Properties;
using DWOS.UI.Security;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using NLog;

namespace DWOS.UI.Tools
{
    internal class UpdateCommand : CommandBase
    {
        #region Methods

        public UpdateCommand(ToolBase tool)
            : base(tool)
        {
            SecurityManager.Current.UserUpdated += Current_UserUpdated;
        }

        public override void OnClick()
        {
            try
            {
                using(var frm = new Utilities.UpdateInformation())
                {
                    frm.ShowDialog();
                    Properties.Settings.Default.LastVersionUpdateShown = About.ApplicationVersion;
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error checking for new version.");
            }
        }

        private static void AutoShowUpdateInfo()
        {
            //if empty then set to current version
            if(String.IsNullOrEmpty(Properties.Settings.Default.LastVersionUpdateShown))
                Properties.Settings.Default.LastVersionUpdateShown = About.ApplicationVersion;

            if(Properties.Settings.Default.LastVersionUpdateShown != About.ApplicationVersion)
            {
                using (var frm = new Utilities.UpdateInformation())
                {
                    frm.ShowDialog();
                    Properties.Settings.Default.LastVersionUpdateShown = About.ApplicationVersion;
                }
            }
        }

        #endregion
        
        #region Events

        private void Current_UserUpdated(object sender, UserUpdatedEventArgs e)
        {
            if(SecurityManager.Current.IsValidUser)
                AutoShowUpdateInfo();
        }

        #endregion
    }

    internal class UpdateAvailableCommand : CommandBase
    {
        #region Fields

        private const int INITIAL_DELAY = 10000;

        #endregion

        #region Methods

        public UpdateAvailableCommand(ToolBase tool)
            : base(tool)
        {
            //hide unless an update is available
            base.Button.Visible = false;
            
            ScheduledUpdateCheck();
        }

        public override void OnClick()
        {
            try
            {
                var version = new Version(About.ApplicationVersion);

                //check latest version with out the revision to force to get latest revision
                Updater.CheckUpdates(version.Major + "." + version.Minor + "." + version.Build, false);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error checking for new version.");
            }
        }

        private void UpdateCheckCompleted(DS2LicenseActivation.VersionInfo versionInfo)
        {
            try
            {
                if(DWOSApp.MainForm == null)
                    return;

                if (DWOSApp.MainForm.InvokeRequired)
                {
                    DWOSApp.MainForm.BeginInvoke(new Action<DS2LicenseActivation.VersionInfo>(UpdateCheckCompleted), versionInfo);
                }
                else
                {
                    if (versionInfo == null || versionInfo.Version == null)
                        return;

                    var currentVersion = new Version(About.ApplicationVersion);
                    var newVersion = new Version(versionInfo.Version);

                    if(currentVersion.Major == newVersion.Major && currentVersion.Minor == newVersion.Minor && currentVersion.Build == newVersion.Build && newVersion.Revision > currentVersion.Revision)
                    {
                        this.Button.Visible = true;
                        ((ToolBase) this.Button.Button).SharedProps.AppearancesSmall.Appearance.BackColor = System.Drawing.Color.Red;
                        DWOSApp.MainForm.FlyoutManager.DisplayFlyout("New Version", "Version {0} of DWOS available.".FormatWith(newVersion.ToString()), DateTime.Now.Subtract(versionInfo.ReleaseDate).TotalDays > 3);
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error checking for updated version of DWOS.");
            }
        }

        private void ScheduledUpdateCheck()
        {
            try
            {
                //check for updates if not checked recently
                var t = new Thread(ts =>
                {
                    Thread.Sleep(INITIAL_DELAY);

                    var client = new DS2LicenseActivation.ActivationServiceClient();
                    client.GetLatestVersionCompleted += (s, e) =>
                    {
                        if (e.Error == null)
                            UpdateCheckCompleted(e.Result);
                    };

                    client.GetLatestVersionAsync("DWOS", DS2LicenseActivation.ReleaseGroup.Normal);
                });

                t.Start();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error scheduling to check for new update.");
            }
        }

        #endregion
    }

    internal class RefreshCommand: CommandBase
    {
        #region Fields

        protected Main _frmMain;

        #endregion

        #region Methods

        public RefreshCommand(ToolBase tool, Main frmMain)
            : base(tool)
        {
            this._frmMain = frmMain;
        }

        public override void OnClick()
        {
            this._frmMain.RefreshData();
        }

        #endregion
    }

    internal class SortGridCommand: CommandBase
    {
        #region Fields

        protected Main _frmMain;

        #endregion

        #region Methods

        public SortGridCommand(ToolBase tool, Main frmMain)
            : base(tool)
        {
            this._frmMain = frmMain;
        }

        public override void OnClick()
        {
            try
            {
                var activeGrid = this._frmMain.ActiveTab as IOrderSummary;

                activeGrid?.ApplyDefaultSort();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error sorting grid columns.");
            }
        }

        #endregion
    }

    internal class PrintGridCommand: CommandBase
    {
        #region Fields

        private Main _frmMain;

        #endregion

        #region Methods

        public PrintGridCommand(ToolBase tool, Main frmMain)
            : base(tool)
        {
            this._frmMain = frmMain;
        }

        public override void OnClick()
        {
            try
            {
                (_frmMain.ActiveTab as IReportTab)?.DisplayReport();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error printing grid.";
                _log.Error(exc, errorMsg);
            }
        }

        #endregion
    }

    internal class ExitCommand: CommandBase
    {
        #region Fields

        protected Main _frmMain;

        #endregion

        #region Methods

        public ExitCommand(ToolBase tool, Main frmMain)
            : base(tool)
        {
            this._frmMain = frmMain;
        }

        public override void OnClick()
        {
            this._frmMain.Close();
        }

        #endregion
    }

    internal class UserActivationsCommand: CommandBase
    {
        #region Fields

        #endregion

        #region Methods

        public UserActivationsCommand(ToolBase tool)
            : base(tool) {}

        public override void OnClick()
        {
            try
            {
                using(var dlg = new UserActivations())
                {
                    dlg.ShowDialog(Form.ActiveForm);
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error displaying user activations dialog.");
            }
        }

        #endregion
    }

    internal class DepartmentLocationCommand: CommandBase
    {
        #region Fields

        protected Main _frmMain;
        public event EventHandler DepartmentChanged;

        #endregion

        #region Methods

        public DepartmentLocationCommand(ToolBase tool, Main frmMain)
            : base(tool, "DepartmentLocation")
        {
            this._frmMain = frmMain;

            this.LoadDepartments();
            tool.ToolValueChanged += tool_ToolValueChanged;
        }

        public void LoadDepartments()
        {
            try
            {
                var cbt = ((ComboBoxTool)base.Button.Button);
                var list = cbt.ValueList;
                list.ValueListItems.Clear();

                using(var ta = new d_DepartmentTableAdapter())
                {
                    using(var departments = ta.GetActiveDepartments())
                    {
                        foreach(OrderStatusDataSet.d_DepartmentRow row in departments)
                        {
                            if (row.IsSystemNameNull() || row.SystemName != "None")
                            {
                                list.ValueListItems.Add(row.DepartmentID, row.DepartmentID);
                            }
                        }
                    }
                }

                list.SelectedIndex = Math.Max(cbt.ValueList.FindString(Settings.Default.CurrentDepartment), 0);
                Settings.Default.CurrentDepartment = ((ValueListItem)list.SelectedItem).DisplayText;
                cbt.Value = ((ValueListItem)list.SelectedItem).DisplayText;
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading available departments", exc);
            }
        }

        #endregion
        
        #region Events

        private void tool_ToolValueChanged(object sender, ToolEventArgs e)
        {
            try
            {
                _log.Debug("tool_AfterToolExitEditMode");

                //update currently selected item
                var cbt = e.Tool as ComboBoxTool;

                if (!String.IsNullOrEmpty(cbt.Text))
                    Settings.Default.CurrentDepartment = cbt.Text;

                DepartmentChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error after department location changed.");
            }
        }

        #endregion
    }

    internal class ProcessingLineCommand: CommandBase
    {
        #region Fields

        private readonly Main _frmMain;

        #endregion

        #region Properties

        public override bool Enabled => base.Enabled && ApplicationSettings.Current.MultipleLinesEnabled;

        #endregion

        #region Methods

        public ProcessingLineCommand(ToolBase tool, Main frmMain)
            : base(tool, "DepartmentLocation")
        {
            this._frmMain = frmMain;

            LoadProcessingLines(Settings.Default.CurrentDepartment);
            tool.ToolValueChanged += tool_ToolValueChanged;
            HideIfDisabled = true;
        }

        public void LoadProcessingLines(string currentDepartment)
        {
            try
            {
                var cbt = ((ComboBoxTool)base.Button.Button);
                var list = cbt.ValueList;
                list.ValueListItems.Clear();

                var selectedIndex = 0;
                list.ValueListItems.Add(-1, "(None)");
                using (var ta = new ProcessingLineTableAdapter())
                {
                    using (var departments = ta.GetForDepartment(currentDepartment))
                    {
                        foreach (var row in departments)
                        {
                            list.ValueListItems.Add(row.ProcessingLineID, row.Name);

                            if (row.ProcessingLineID == Settings.Default.CurrentLine)
                            {
                                selectedIndex = list.ValueListItems.Count - 1;
                            }
                        }
                    }
                }

                list.SelectedIndex = selectedIndex;
                Settings.Default.CurrentLine = ((ValueListItem)list.SelectedItem).DataValue as int? ?? -1;
                cbt.Value = ((ValueListItem)list.SelectedItem).DataValue;
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading available processing lines", exc);
            }
        }

        #endregion
        
        #region Events

        private void tool_ToolValueChanged(object sender, ToolEventArgs e)
        {
            try
            {
                _log.Debug("tool_AfterToolExitEditMode");

                //update currently selected item
                var cbt = e.Tool as ComboBoxTool;
                if (cbt != null)
                    Settings.Default.CurrentLine = cbt.Value as int? ?? -1;

                DWOSApp.MainForm.Commands.RefreshAll();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error after processing line changed.");
            }
        }

        #endregion
    }

    internal class UrlCommand : CommandBase
    {
        #region Properties

        public override bool Enabled
        {
            get { return true; }
        }

        public string Url
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public UrlCommand(ToolBase tool, string url)
            : base(tool)
        {
            Url = url;
        }

        public override void OnClick()
        {
            try
            {
                Process.Start(Url);
            }
            catch (Exception exc)
            {
                MessageBoxUtilities.ShowMessageBoxWarn("Default web browser is not found or is unable to start.", "Unable to Start");
                _log.Debug(exc, "Error starting process to open {0}", Url);
            }
        }

        #endregion
    }

    internal class FeedbackCommand : UrlCommand
    {
        #region Methods

        public FeedbackCommand(ToolBase tool)
            : base(tool, "http://feedback.getdwos.com")
        {
        }

        #endregion
    }

    internal class KnowledgeBaseCommand : UrlCommand
    {
        #region Methods

        public KnowledgeBaseCommand(ToolBase tool)
            : base(tool, "http://knowledge.getdwos.com")
        {
        }

        #endregion
    }

    internal class AnnouncementsCommand : UrlCommand
    {
        #region Methods

        public AnnouncementsCommand(ToolBase tool)
            : base(tool, "http://www.getdwos.com/Features/Articles.aspx")
        {
        }

        #endregion
    }

    internal class TicketCommand : CommandBase
    {
        #region Properties

        public override bool Enabled
        {
            get { return true; }
        }

        #endregion

        #region Methods

        public TicketCommand(ToolBase tool)
            : base(tool) { }

        public override void OnClick()
        {
            using (var ticket = new CustomerTicket())
            {
                ticket.ShowDialog(Form.ActiveForm);
            }
        }

        #endregion
    }

    internal class HelpCommand: UrlCommand
    {
        #region Methods

        public HelpCommand(ToolBase tool, Form mainForm) : base(tool, HelpLink.HELP_URL)
        {
            mainForm.KeyUp += mainForm_KeyUp;
        }

        private void mainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                e.Handled = true;
                OnClick();
            }
        }

        #endregion
    }

    internal class DocumentManagerCommand : CommandBase
    {
        #region Fields

        //private DocumentManager _dialog;

        #endregion

        #region Methods

        public DocumentManagerCommand(ToolBase tool)
            : base(tool, "Documents")
        {
            
        }

        public override void OnClick()
        {
            try
            {
                using (var frm = new DocumentManager())
                {
                    frm.btnOK.Visible = false;
                    frm.btnClose.Visible = true;
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error displaying order summary.";
                _log.Error(exc, errorMsg);
            }
        }
        #endregion
    }

    internal class VideoCommand : CommandBase
    {
        #region Properties

        public string Url { get; set; }

        #endregion

        #region Methods

        public VideoCommand(ToolBase tool)
            : base(tool)
        {

        }

        public override void OnClick()
        {
            try
            {
                if (Url != null)
                    System.Diagnostics.Process.Start(Url);
            }
            catch (Exception exc)
            {
                MessageBoxUtilities.ShowMessageBoxWarn("Default program for playing videos is not found or is unable to start.", "Unable to Start");
                _log.Debug(exc, "Error starting process for " + Url);
            }
        }

        #endregion
    }

    internal class LogFileCommand : CommandBase
    {
        #region Properties

        public override bool Enabled
        {
            get { return true; }
        }

        #endregion

        #region Methods

        public LogFileCommand(ToolBase tool)
            : base(tool)
        {
            
        }
        
        public override void OnClick()
        {
            try
            {
                var file = NLogExtensions.FindLogFileName();

                if(!String.IsNullOrWhiteSpace(file) && System.IO.File.Exists(file))
                {
                    var argument = @"/select, " + file;

                    try
                    {
                        System.Diagnostics.Process.Start("explorer.exe", argument);
                    }
                    catch (Exception exc)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn("Default program for file type is not found or is unable to start.", "Unable to Start");
                        _log.Debug(exc, "Error starting process for: " + file);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Info(exc, "Error during click of online help command.");
            }
        }

        #endregion
    }
}