using System;
using System.Windows.Forms;
using DWOS.Shared.Wizard;

namespace DWOS.Server.Admin.Wizards.InstallWizard
{
    /// <summary>
    /// The final step of the installation wizard.
    /// </summary>
    public partial class InstallFinish : UserControl, IWizardPanel
    {
        public InstallFinish()
        {
            InitializeComponent();
        }

        #region IWizardPanel

        public string Title
        {
            get { return "Summary"; }
        }

        public string SubTitle
        {
            get { return "DWOS Setup Summary."; }
        }

        public Action <IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller) { }

        public Control PanelControl
        {
            get { return this; }
        }

        public bool IsValid
        {
            get { return true; }
        }

        public void OnMoveTo()
        {
            OnValidStateChanged?.Invoke(this);
        }

        public void OnMoveFrom() { }

        public void OnFinished() { }

        #endregion
    }
}