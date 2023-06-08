using System.Windows.Forms;
using DWOS.UI.Utilities;
using DWOS.Data;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsSalesOrderWizard : UserControl, ISettingsPanel
    {
        #region Methods
        public bool CanDock
        {
            get { return true; }
        }
        public SettingsSalesOrderWizard()
        {
            InitializeComponent();
        }

        #endregion

        #region ISettingsPanel

        public bool Editable => SecurityManager.Current.IsInRole("ApplicationSettings.Edit");

        public string PanelKey => "SalesOrder";

        public void LoadData()
        {
            var settings = ApplicationSettings.Current;
            chkEnableWizard.Checked = settings.SalesOrderWizardEnabled;
            chkRedline.Checked = settings.CanCreatePartsInSalesOrderWizard;
            chkAllowDifferentProcesses.Checked = settings.AllowDifferentProcessesInSalesOrderWizard;
        }

        public void SaveData()
        {
            var settings = ApplicationSettings.Current;
            settings.SalesOrderWizardEnabled = chkEnableWizard.Checked;
            settings.CanCreatePartsInSalesOrderWizard = chkRedline.Checked;
            settings.AllowDifferentProcessesInSalesOrderWizard = chkAllowDifferentProcesses.Checked;
        }

        #endregion
    }
}
