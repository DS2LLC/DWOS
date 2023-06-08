using System;
using System.Windows.Forms;
using DWOS.Data;
using Infragistics.Win.UltraWinEditors;

namespace DWOS.UI.Utilities
{
    public partial class ScannerProperties : UserControl
    {
        #region Methods

        public ScannerProperties()
        {
            InitializeComponent();
        }

        public void LoadData(ScannerSettingsType widgetType)
        {
            var settings = UserSettings.Default.Media.Get(widgetType) ??
                ScannerSettings.DefaultFrom(UserSettings.Default);

            txtScannerSource.Text = settings.ScanDeviceName;
            optScannerType.CheckedIndex = settings.ScanOutputPDF ? 0 : 1;
            numJPEGQuality.Value = settings.ScanQuality;
            numResolution.Value = settings.ScanResolution;
            chkShowFullUI.Checked = settings.ScanShowFullUI;
        }

        public void SaveData(ScannerSettingsType type)
        {
            var settings = new ScannerSettings()
            {
                ScanDeviceName = this.txtScannerSource.Text,
                ScanOutputPDF = optScannerType.CheckedItem.DisplayText == "PDF",
                ScanQuality = Convert.ToInt32(numJPEGQuality.Value),
                ScanResolution = Convert.ToInt32(numResolution.Value),
                ScanShowFullUI = chkShowFullUI.Checked
            };

            UserSettings.Default.Media.Set(type, settings);
            UserSettings.Default.Save();
        }

        private void PickScannerSource()
        {
            using (var _twain = new Dynamsoft.DotNet.TWAIN.DynamicDotNetTwain())
            {
                _twain.SupportedDeviceType = Dynamsoft.DotNet.TWAIN.Enums.EnumSupportedDeviceType.SDT_ALL;

                if (_twain.SelectSource())
                    txtScannerSource.Text = _twain.CurrentSourceName;
            }
        }

        #endregion

        #region Events

        private void btnPickScannerSource_Click(object sender, EventArgs e)
        {
            try
            {
                PickScannerSource();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error picking scanner source.");
            }
        }

        private void txtScannerSource_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                txtScannerSource.Text = null;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error clearing scan source text.");
            }
        }

        #endregion
    }
}