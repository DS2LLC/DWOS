using System;
using System.Windows.Forms;
using DWOS.UI.Utilities;
using NLog;
using DWOS.Data;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsContractReview : UserControl, ISettingsPanel
    {
        public bool CanDock
        {
            get { return false; }
        }
        public SettingsContractReview()
        {
            InitializeComponent();
        }

        #region ISettingsPanel Members

        public bool Editable =>
            SecurityManager.Current.IsInRole("ApplicationSettings.Edit");


        public string PanelKey => "ContractReview";

        public void LoadData()
        {
            try
            {
                Enabled = Editable;
                txtContactReview.Value = ApplicationSettings.Current.ContractReviewText;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error loading ContractReview settings.");
            }
        }

        public void SaveData()
        {
            ApplicationSettings.Current.ContractReviewText = txtContactReview.Value?.ToString()
                ?? string.Empty;
        }

        #endregion
    }
}
