using System;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsAuthenticationInfo: UserControl, ISettingsPanel
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public bool CanDock
        {
            get { return false; }
        }
        public string PanelKey
        {
            get { return "Authentication"; }
        }

        #endregion

        #region Methods

        public SettingsAuthenticationInfo()
        {
            this.InitializeComponent();
        }

        #endregion

        #region ISettingsPanel Members

        public bool Editable
        {
            get { return SecurityManager.Current.IsInRole("ApplicationSettings.Edit"); }
        }

        public void LoadData()
        {
            try
            {
                Enabled = this.Editable;
                cboAuthType.SelectedIndex = (int)ApplicationSettings.Current.LoginType;
                numMinPin.Value = ApplicationSettings.Current.UserPinMinLength; 
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error loading settings.";
                _log.Error(exc, errorMsg);
            }
        }

        public void SaveData()
        {
            ApplicationSettings.Current.LoginType = cboAuthType.SelectedItem != null ? (LoginType) Convert.ToInt32(cboAuthType.SelectedItem.DataValue) : LoginType.PinOrSmartcard;
            ApplicationSettings.Current.UserPinMinLength = Convert.ToInt32(this.numMinPin.Value); 
        }

        #endregion
    }
}