using System;
using System.Windows.Forms;
using DWOS.Shared.Wizard;

namespace DWOS.QBExport.Syncing
{
    /// <summary>
    /// Wizard step for selecting a sync source.
    /// </summary>
    public partial class SourceSelection : UserControl, IWizardPanel
    {
        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceSelection"/> class.
        /// </summary>
        public SourceSelection()
        {
            InitializeComponent();
        }

        #endregion

        #region IWizardPanel Members

        public string Title { get { return "Select Sync Source."; } }

        public string SubTitle { get { return string.Empty; } }

        public Action<IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller) { }

        public Control PanelControl { get { return this; } }

        public bool IsValid { get { return true; } }

        public void OnMoveTo() { }

        public void OnMoveFrom()
        {
            QBSyncController.Source = this.rbDWOS.Checked ? SyncSource.DWOS : SyncSource.QuickBooks;
        }

        public void OnFinished() { }
        
        #endregion
    }
}
