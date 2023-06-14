using System.Collections.Generic;
using System.Windows.Forms;
using DWOS.Shared.Wizard;
using NLog;

namespace DWOS.Server.Admin.Wizards.InstallWizard
{
    /// <summary>
    /// Controller for the installation wizard.
    /// </summary>
    internal class InstallWizardController : WizardController
    {
        public override string WizardTitle
        {
            get { return "DWOS Server Installer"; }
        }

        public override void Finished() { }

        /// <summary>
        /// Starts the installation wizard.
        /// </summary>
        /// <returns></returns>
        public static DialogResult StartWizard()
        {
            LogManager.GetCurrentClassLogger().Info("Starting installation wizard.");
            var ctrl = new InstallWizardController();
            var wizard = new WizardDialog();
            var panels = new List <IWizardPanel>();

            panels.Add(new InstallLicense());
            panels.Add(new InstallDatabase());
            panels.Add(new ConnectToDatabase());
            panels.Add(new InstallSQL());
            panels.Add(new OpenFirewall());
            panels.Add(new InstallService());
            panels.Add(new InstallFinish());

            wizard.InitializeWizard(ctrl, panels);
            return wizard.ShowDialog(Form.ActiveForm);
        }
    }
}