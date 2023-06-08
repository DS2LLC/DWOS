using System;
using System.Windows.Forms;
using DWOS.Shared.Wizard;
using Infragistics.Win.UltraWinTree;

namespace DWOS.UI.Admin.FieldMigration
{
    public partial class SummaryPanel : UserControl, IWizardPanel
    {
        #region Fields

        private FieldMigrationController _controller;

        #endregion

        #region Methods

        public SummaryPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region IWizardPanel members

        public string Title => "Migration Results";

        public string SubTitle => string.Empty;

        public Action<IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller)
        {
            _controller = controller as FieldMigrationController;
        }

        public Control PanelControl => this;

        public bool IsValid => true;

        public void OnMoveTo()
        {
            if (_controller?.IsMigrationFinished ?? false)
            {
                tvwSummary.Nodes.Add(new UltraTreeNode { Text = @"Migration completed successfully." });
            }
            else
            {
                tvwSummary.Nodes.Add(new UltraTreeNode { Text = @"Migration failed." });
            }
        }

        public void OnMoveFrom()
        {
            tvwSummary.Nodes.Clear();
        }

        public void OnFinished()
        {
            _controller = null;
        }

        #endregion
    }
}
