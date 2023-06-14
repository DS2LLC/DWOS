using DWOS.Data;
using NLog;
using System;
using System.Windows.Forms;

namespace DWOS.Server.Admin.SettingsPanels
{
    /// <summary>
    /// Interface for editing portal settings.
    /// </summary>
    public partial class SettingsWebPortalInfo : UserControl, ISettingsPanel
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public string PanelKey
        {
            get { return "Web"; }
        }

        public bool IsValid
        {
            get { return true; }
        }

        #endregion

        #region Methods

        public SettingsWebPortalInfo()
        {
            InitializeComponent();
        }

        #endregion Methods

        #region ISettingsPanel Members

        public bool Editable
        {
            get { return true; } 
        }

        public void LoadData()
        {
            try
            {
                var settings = ApplicationSettings.Current;

                Enabled = this.Editable;

                chkShowLateOrderNotification.Checked = settings.ShowLateOrderNotificationSetting;

                var skinStyle = settings.SkinStyle;
                this.cboSkinStyle.SelectedIndex = skinStyle == null ? 0 : this.cboSkinStyle.FindString(skinStyle);

                this.teTagline.Text = settings.CompanyTagline;
            }
            catch (Exception exc)
            {
                const string errorMsg = "Error loading settings.";
                _log.Error(exc, errorMsg);
            }
        }

        public void SaveData()
        {
            var settings = ApplicationSettings.Current;

            settings.ShowLateOrderNotificationSetting = chkShowLateOrderNotification.Checked;
            ApplicationSettings.Current.SkinStyle = cboSkinStyle.Text;
            ApplicationSettings.Current.CompanyTagline = this.teTagline.Text;
        }

        #endregion
    }
}
