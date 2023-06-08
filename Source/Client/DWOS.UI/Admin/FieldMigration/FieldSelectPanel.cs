using System;
using System.Windows.Forms;
using DWOS.Shared.Wizard;

namespace DWOS.UI.Admin.FieldMigration
{
    public partial class FieldSelectPanel : UserControl, IWizardPanel
    {
        #region Methods

        public FieldSelectPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region IWizardPanel members

        public string Title => "Select Field";

        public string SubTitle => string.Empty;

        public Action<IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller)
        {
        }

        public Control PanelControl => this;

        public bool IsValid => true;

        public void OnMoveTo()
        {
        }

        public void OnMoveFrom()
        {
        }

        public void OnFinished()
        {
        }

        #endregion
    }
}
