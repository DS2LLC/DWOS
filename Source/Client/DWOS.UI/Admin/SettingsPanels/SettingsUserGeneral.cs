using DWOS.Data;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Windows.Forms;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsUserGeneral : UserControl, ISettingsPanel
    {
        #region Methods
        public bool CanDock
        {
            get { return false; }
        }
        public SettingsUserGeneral()
        {
            InitializeComponent();

            foreach (PartMarkingDeviceType type in Enum.GetValues(typeof(PartMarkingDeviceType)))
            {
                cboPartMarkType.Items.Add(type);
            }
        }

        #endregion

        #region ISettingsPanel Members

        public bool Editable
        {
            get { return SecurityManager.Current.IsInRole("ApplicationSettings.Edit"); }
        }

        public string PanelKey
        {
            get
            {
                return "UserGeneral";
            }
        }

        public void LoadData()
        {
            try
            {
                base.Enabled = this.Editable;
                this.numPresentationModeSpeed.Value = UserSettings.Default.PresentationModeSpeed;
                this.chkCleanupMedia.Checked = UserSettings.Default.CleanupMediaAfterUpload;

                var deviceTypeItem = cboPartMarkType.FindItemByValue<PartMarkingDeviceType>(i => i == UserSettings.Default.ParkMarkingType);

                if (deviceTypeItem == null)
                {
                    cboPartMarkType.SelectedIndex = 0;
                }
                else
                {
                    cboPartMarkType.SelectedItem = deviceTypeItem;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error Loading Settings");
            }
        }

        public void SaveData()
        {
            try
            {
                UserSettings.Default.PresentationModeSpeed = (int)this.numPresentationModeSpeed.Value;
                UserSettings.Default.CleanupMediaAfterUpload = this.chkCleanupMedia.Checked;
                UserSettings.Default.ParkMarkingType =
                    cboPartMarkType.SelectedItem?.DataValue as PartMarkingDeviceType? ??
                    PartMarkingDeviceType.VideoJetExcel;

                UserSettings.Default.Save();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error Saving Settings");
            }
        }

        #endregion

        private void numPresentationModeSpeed_ValidationError(object sender, Infragistics.Win.UltraWinEditors.ValidationErrorEventArgs e)
        {
            try
            {
                this.numPresentationModeSpeed.Value = UserSettings.DefaultPresentationModeSpeed;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error while recoving from validation error.");
            }
        }
    }
}
