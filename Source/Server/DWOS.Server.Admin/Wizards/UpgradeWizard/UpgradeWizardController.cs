using System.Collections.Generic;
using System.Windows.Forms;
using DWOS.Shared.Wizard;

namespace DWOS.Server.Admin.Wizards.UpgradeWizard
{
    /// <summary>
    /// Controller for the version upgrade wizard.
    /// </summary>
    internal class UpgradeWizardController : WizardController
    {
        /// <summary>
        /// Gets or sets the file name to use for the database backup.
        /// </summary>
        public string BackupFileName { get; set; }

        /// <summary>
        /// Gets or sets the database's connection string.
        /// </summary>
        public string DBConnectionString { get; set; }

        public override string WizardTitle
        {
            get { return "DWOS Server Upgrade"; }
        }

        public override void Finished() { }

        public static DialogResult StartWizard()
        {
            var ctrl = new UpgradeWizardController() { DBConnectionString = DWOS.Data.ServerSettings.Default.DBConnectionString };
            var wizard = new WizardDialog();
            var panels = new List<IWizardPanel>();

            //Stop Service
            panels.Add(new StartService() { RequiredServiceState = StartService.ServiceState.Stopped });
            //Configure how to connect as Admin
            panels.Add(new ConnectToDatabase());
            //Backup DB
            panels.Add(new BackupDatabase());
            //Run Scripts
            panels.Add(new UpgradeSQL());
            //Start Service
            panels.Add(new StartService() { RequiredServiceState = StartService.ServiceState.Started });

            // Update portal
            if (WebStatusHelpers.IsPortalInstalled)
            {
                panels.Add(new UpdatePortalConfiguration());
            }

            wizard.InitializeWizard(ctrl, panels);

           return wizard.ShowDialog(Form.ActiveForm);
        }
    }
}