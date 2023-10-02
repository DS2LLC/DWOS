using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Shared;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinTree;
using System.Diagnostics;

namespace DWOS.UI.Admin
{
    public partial class Settings: Form
    {
        #region Fields

        private List<ISettingsPanel> _panels;
        private List<ISettingsPanel> _panelsLoaded;
        private SecurityFormWatcher _securityWatcher;

        #endregion

        #region Methods

        public Settings()
        {
            this.InitializeComponent();
            this._securityWatcher = new SecurityFormWatcher(this, this.btnCancel);
        }

        private void Save()
        {
            var cacheVersion = ApplicationSettings.Current.CacheVersion;

            foreach(var settingsPanel in this._panels)
            {
                //if panel was loaded and panel is editable
                if (this.pnlContent.ClientArea.Controls.Contains((UserControl)settingsPanel) && settingsPanel.Editable)
                {
                    settingsPanel.SaveData();
                }
            }

            //save to database
            ApplicationSettings.Current.Save();

            //mark layouts to be reset to ensure they are recreated with new images if cahce version changed
            if (cacheVersion != ApplicationSettings.Current.CacheVersion)
            {
                UserSettings.Default.ResetLayouts = true;
            }
        }

        private void LoadData()
        {
            this._panelsLoaded = new List<ISettingsPanel>();
            this._panels = new List<ISettingsPanel>();
            _panels.Add(new SettingsPanels.SettingsCompanyInfo());
            _panels.Add(new SettingsPanels.SettingsEmailInfo());
            _panels.Add(new SettingsPanels.SettingsQuality());
            _panels.Add(new SettingsPanels.SettingsCOCData());
            _panels.Add(new SettingsPanels.SettingsShippingNotification());
            _panels.Add(new SettingsPanels.SettingsPortal());
            _panels.Add(new ShippingRec.ShippingManagerPanels.ShippingSettings());
            _panels.Add(new SettingsPanels.SettingsAuthenticationInfo());
            _panels.Add(new SettingsPanels.SettingsGeneralInfo());
            _panels.Add(new SettingsPanels.SettingsReportNotification());
            _panels.Add(new SettingsPanels.SettingsRequiredFields());
            _panels.Add(new SettingsPanels.SettingsAccountingInfo());
            _panels.Add(new SettingsPanels.SettingsWorkflowInfo());
            _panels.Add(new SettingsPanels.SettingsUserGeneral());
            _panels.Add(new SettingsPanels.SettingsPricingInfo());

            _panels.Add(new SettingsPanels.SettingsCertificateNotification());
            _panels.Add(new SettingsPanels.SettingsContractReview());
            _panels.Add(new SettingsPanels.SettingsSalesOrderWizard());
            _panels.Add(new SettingsPanels.SettingsOrderApprovalNotification());
            _panels.Add(new SettingsPanels.SettingsOrderReceiptNotification());
            _panels.Add(new SettingsPanels.SettingsHoldNotification());
            _panels.Add(new SettingsPanels.SettingsLateOrderNotification());
            _panels.Add(new SettingsPanels.SettingsShipping());
        }

        private void DisplayPanel(UltraTreeNode node)
        {


            foreach (var settingsPanel in this._panels)
            {
                if (node != null && node.Tag != null && node.Tag.ToString() == settingsPanel.PanelKey)
                {
                    if (!this.pnlContent.ClientArea.Controls.Contains((UserControl)settingsPanel))
                    {
                        settingsPanel.LoadData(); 
                        
                        ((UserControl)settingsPanel).Visible = false;
                        if (settingsPanel.CanDock)
                            ((UserControl)settingsPanel).Dock = DockStyle.Fill;
                        else
                            ((UserControl)settingsPanel).Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
                        
                        ((UserControl)settingsPanel).Location = new System.Drawing.Point(0, 0);
                        this.pnlContent.ClientArea.Controls.Add((UserControl)settingsPanel);
                    }
                    ((UserControl)settingsPanel).Width = this.pnlContent.Width;
                    ((UserControl)settingsPanel).Visible = true;

                }
                else
                {
                    ((UserControl)settingsPanel).Visible = false;
                }
            }
        }

        #endregion

        #region Events

        private void Settings_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadData();
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading settings.", exc);
            }

            this.tvwSettings.ExpandAll();
            this.tvwSettings.Nodes[0].Select();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                this.Save();
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error saving settings.", exc);
            }
        }

        private void tvwSettings_AfterSelect(object sender, SelectEventArgs e)
        {
            if (e.NewSelections.Count != 1)
            {
                return;
            }

            var node = e.NewSelections[0];

            if (node.Tag != null)
            {
                try
                {
                    this.DisplayPanel(node);
                }
                catch (Exception exc)
                {
                    NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error displaying panel.");
                }
            }
            else if (node.Nodes.Count > 0)
            {
                node.Nodes[0].Select();
            }
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            this._panels = null;

            if (this._securityWatcher != null)
            {
                this._securityWatcher.Dispose();
            }

            this._securityWatcher = null;
        }

        private void btnVideo_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(VideoLinks.SettingsTutorial);
            }
            catch (Exception exc)
            {
                MessageBoxUtilities.ShowMessageBoxWarn("The program associated with playing videos is not found or is unable to start.", "Play Video");
                NLog.LogManager.GetCurrentClassLogger().Debug(exc, "Error starting process for: " + VideoLinks.SettingsTutorial);
            }
        }

        #endregion
    }
}