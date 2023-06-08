using System;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinToolTip;
using NLog;

namespace DWOS.UI
{
    public partial class UserSettingsView: UserControl
    {
        #region Fields

        private bool _settingsLoaded;
        private readonly DisplayDisabledTooltips _displayDisabledTooltips;

        #endregion

        #region Methods

        public UserSettingsView()
        {
            this.InitializeComponent();
            _displayDisabledTooltips = new DisplayDisabledTooltips(this, ultraToolTipManager1);
        }

        public void LoadSettings()
        {
            try
            {
                if(this._settingsLoaded)
                    return;
                else
                {
                    this.cboPrinter.DataSource = PrinterUtilities
                        .GetInstalledPrinterNames()
                        .ToList();

                    if(!String.IsNullOrWhiteSpace(UserSettings.Default.DefaultPrinterName))
                    {
                        ValueListItem printerItem = this.cboPrinter.FindItemByValue<string>(pn => pn == UserSettings.Default.DefaultPrinterName);
                        if(printerItem != null)
                            this.cboPrinter.SelectedItem = printerItem;
                    }

                    var serverSettings = DependencyContainer.Resolve<DwosServerInfoProvider>()?.ConnectionInfo;

                    txtDatabaseName.Text = serverSettings?.ServerAddress ?? UserSettings.Default.ServerAddress ;

                    this.chkResetLayout.Checked = UserSettings.Default.ResetLayouts;
                    this.chkTouchEnabled.Checked = Properties.Settings.Default.TouchEnabled;
                    this.chkAnimations.Checked = Properties.Settings.Default.WIPAnimations;
                    this.chkFullRowHighlight.Checked = Properties.Settings.Default.WIPHighlightLateFullRow;

                    if (serverSettings?.FromConnectionFile ?? false)
                    {
                        btnChangeServer.Enabled = false;
                        ultraToolTipManager1.SetUltraToolTip(btnChangeServer, new UltraToolTipInfo(
                            "Cannot change server while using a connection file.",
                            ToolTipImage.None,
                            "Change Server",
                            DefaultableBoolean.True));
                    }

                    this._settingsLoaded = true;
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error loading settings.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        public void SaveSettings()
        {
            try
            {
                if(this._settingsLoaded)
                {
                    //if no printer selected then clear out default printer
                    UserSettings.Default.DefaultPrinterName = cboPrinter.Value?.ToString();
                    UserSettings.Default.ResetLayouts = this.chkResetLayout.Checked;
                    Properties.Settings.Default.TouchEnabled = this.chkTouchEnabled.Checked;
                    Properties.Settings.Default.WIPAnimations = this.chkAnimations.Checked;
                    Properties.Settings.Default.WIPHighlightLateFullRow = this.chkFullRowHighlight.Checked;

                    Settings.Default.Save();
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving settings.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion

        #region Events

        private void btnChangeServer_Click(object sender, EventArgs e)
        {
            using (var frm = new ServerConnection())
            {
                if(frm.ShowDialog(this) == DialogResult.OK)
                {
                    var restart = MessageBoxUtilities.ShowMessageBoxYesOrNo("For settings to be applied DWOS must be restarted. Do you want to restart now?", "Restart DWOS?", "If not restarted, the new settings will be applied when DWOS is opened again.");

                    if(restart == DialogResult.Yes)
                    {
                        Program.ChangeServerNow(frm.ServerAddress, frm.ServerPort);
                    }
                    else
                    {
                        Program.ChangeServerAtNextStart(frm.ServerAddress, frm.ServerPort);
                        this.txtDatabaseName.Text = frm.ServerAddress;
                    }
                }
            }
        }

        private void cboPrinter_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                if (e.Button.Key == "Delete")
                {
                    cboPrinter.Value = null;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error clicking editor  button");
            }
        }

        #endregion
    }
}
