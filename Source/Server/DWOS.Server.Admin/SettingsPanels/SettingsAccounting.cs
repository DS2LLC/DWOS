using System;
using System.Windows.Forms;
using DWOS.Data;
using NLog;
using System.IO;

namespace DWOS.Server.Admin.SettingsPanels
{
    public partial class SettingsAccounting : UserControl, ISettingsPanel
    {
        #region Methods

        public SettingsAccounting()
        {
            InitializeComponent();
        }

        private bool UpdateErrorInfo()
        {
            if (!chkEnabled.Checked)
            {
                errorProvider.SetError(txtSave, string.Empty);
                errorProvider.SetError(txtError, string.Empty);
                return true;
            }

            var isSavePathValid = !string.IsNullOrEmpty(txtSave.Text) && Directory.Exists(txtSave.Text);
            var isErrorPathValid = !string.IsNullOrEmpty(txtError.Text) && Directory.Exists(txtError.Text);

            if (isSavePathValid)
            {
                errorProvider.SetError(txtSave, string.Empty);
            }
            else
            {
                errorProvider.SetError(txtSave, "Valid directory required.");
            }

            if (isErrorPathValid)
            {
                errorProvider.SetError(txtError, string.Empty);
            }
            else
            {
                errorProvider.SetError(txtError, "Valid directory required.");
            }

            return isSavePathValid && isErrorPathValid;
        }

        #endregion

        #region Events

        private void btnBrowseSave_Click(object sender, EventArgs e)
        {
            try
            {
                var currentDirectory = txtSave.Text;

                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.SelectedPath = currentDirectory;
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        txtSave.Text = folderDialog.SelectedPath;
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error selecting save folder.");
            }
        }

        private void btnBrowseError_Click(object sender, EventArgs e)
        {
            try
            {
                var currentDirectory = txtError.Text;

                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.SelectedPath = currentDirectory;
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        txtError.Text = folderDialog.SelectedPath;
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error selecting save folder.");
            }
        }

        private void chkEnabled_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var isEnabled = chkEnabled.Checked;
                numUpdateIntervalMin.Enabled = isEnabled;
                numCleanInvoices.Enabled = isEnabled;
                btnBrowseSave.Enabled = isEnabled;
                btnBrowseError.Enabled = isEnabled;
                chkAutoInvoice.Enabled = isEnabled;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing SYSPRO enabled value.");
            }
        }

        #endregion

        #region ISettingsPanel members

        public bool Editable => true;

        public string PanelKey => "Accounting";

        public bool IsValid
        {
            get
            {
                return UpdateErrorInfo();
            }
        }

        public void LoadData()
        {
            try
            {
                var sysproSettings = ServerSettings.Default.SysproSettings;
                chkEnabled.Checked = sysproSettings.Enabled;
                numUpdateIntervalMin.Value = sysproSettings.UpdateIntervalMinutes;
                numCleanInvoices.Value = sysproSettings.CleanSuccessfulInvoiceDays;
                txtSave.Text = sysproSettings.SaveDirectory;
                txtError.Text = sysproSettings.ErrorDirectory;
                chkAutoInvoice.Checked = sysproSettings.AutomaticInvoicing;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading settings.");
            }
        }

        public void SaveData()
        {
            try
            {
                ApplicationSettings.Current.SysproIntegrationEnabled = chkEnabled.Checked;

                var sysproSettings = ServerSettings.Default.SysproSettings;
                sysproSettings.Enabled = chkEnabled.Checked;
                sysproSettings.UpdateIntervalMinutes = Convert.ToInt32(numUpdateIntervalMin.Value);
                sysproSettings.CleanSuccessfulInvoiceDays = Convert.ToInt32(numCleanInvoices.Value);
                sysproSettings.SaveDirectory = txtSave.Text;
                sysproSettings.ErrorDirectory = txtError.Text;
                sysproSettings.AutomaticInvoicing = chkAutoInvoice.Checked;

                ServerSettings.Default.Save();

                if (sysproSettings.AutomaticInvoicing && sysproSettings.Enabled)
                {
                    ApplicationSettings.Current.InvoiceIntervalMinutes = sysproSettings.UpdateIntervalMinutes;
                }
                else
                {
                    ApplicationSettings.Current.InvoiceIntervalMinutes = null;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error saving settings.");
            }
        }

        #endregion

    }
}
