using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Server.Admin.Licensing;
using DWOS.Shared;
using Infragistics.Win.UltraWinTree;
using NLog;
using System.ServiceModel;

namespace DWOS.Server.Admin.SettingsPanels
{
    /// <summary>
    /// Settings dialog for the server administration application.
    /// </summary>
    public partial class Settings : Form
    {
        #region Fields

        private List<ISettingsPanel> _panels;
        private List<ISettingsPanel> _panelsLoaded;

        #endregion
        
        #region Methods

        public Settings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// (Attempts to) save data for every panel.
        /// </summary>
        /// <returns>True if data was saved; otherwise, false.</returns>
        private bool Save()
        {
            try
            {
                if (this._panelsLoaded.Any(p => !p.IsValid))
                {
                    return false;
                }

                var cacheVersion = ApplicationSettings.Current.CacheVersion;

                foreach (var settingsPanel in this._panels)
                {
                    //if panel was loaded and panel is editable
                    if (this._panelsLoaded.Contains(settingsPanel) && settingsPanel.Editable)
                        settingsPanel.SaveData();
                }

                ReloadCompanyInfoOnServer();

                //save to database
                ApplicationSettings.Current.Save();
                return true;
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error saving settings.", exc);
                return false;
            }
        }

        private void ReloadCompanyInfoOnServer()
        {
            const string errorMessage = "Error reloading company info.";
            try
            {
                //Tell the DWOS Server to reload its settings
                var client = new LicenseServiceClient();
                client.Open();
                client.ReloadCompanyInfoAsync();
            }
            catch (EndpointNotFoundException endpointException)
            {
                // Server is not running
                LogManager.GetCurrentClassLogger()
                    .Warn(endpointException, errorMessage);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, errorMessage);
            }
        }

        private void LoadData()
        {
            try
            {
                this._panelsLoaded = new List<ISettingsPanel>();
                this._panels = new List<ISettingsPanel>
                {
                    this.pnlSettingsAuthenticationInfo,
                    this.pnlSettingsWebPortalInfo,
                    this.pnlSettings,
                    this.pnlBackup,
                    this.pnlAccounting
                };

                foreach (var settingsPanel in this._panels)
                {
                    ((UserControl)settingsPanel).Visible = false;
                    ((UserControl)settingsPanel).Dock = DockStyle.Fill;
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading settings.", exc);
            }
        }

        private void DisplayPanel(UltraTreeNode node)
        {
            foreach (var settingsPanel in this._panels)
            {
                if (node != null && node.Tag != null && node.Tag.ToString() == settingsPanel.PanelKey)
                {
                    if (!this._panelsLoaded.Contains(settingsPanel))
                    {
                        settingsPanel.LoadData();
                        this._panelsLoaded.Add(settingsPanel);
                    }

                    ((UserControl)settingsPanel).Visible = true;
                }
                else
                    ((UserControl)settingsPanel).Visible = false;
            }
        }

        #endregion

        #region Events

        private void Settings_Load(object sender, EventArgs e)
        {
            this.LoadData();

            this.tvwSettings.ExpandAll();
            this.tvwSettings.Nodes[0].Selected = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) { }

        private void tvwSettings_BeforeSelect(object sender, BeforeSelectEventArgs e)
        {
            try
            {
                if (this._panelsLoaded.Any(p => !p.IsValid))
                {
                    e.Cancel = true;
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error processing selection.", exc);
            }
        }

        private void tvwSettings_AfterSelect(object sender, SelectEventArgs e)
        {
            if (e.NewSelections.Count == 1)
            {
                var node = e.NewSelections[0];
                if (node.Tag != null)
                    this.DisplayPanel(node);
                else if (node.Nodes.Count > 0)
                    node.Nodes[0].Selected = true;
            }
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            this._panels = null;
        }

        #endregion
    }
}
