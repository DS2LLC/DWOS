using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DWOS.Shared.Wizard;
using Infragistics.Win.UltraWinTree;

namespace DWOS.QBExport.Syncing
{

    /// <summary>
    /// Wizard step that shows the results of a sync.
    /// </summary>
    public partial class SyncSummary : UserControl, IWizardPanel
    {
        #region Methods

        public SyncSummary()
        {
            InitializeComponent();
        }

        private void PopulateResultsTree()
        {
            var syncResults = QBSyncController.SyncResults;

            if (syncResults.Count > 0)
            {
                var successes = this.GetResults(MessageType.Success);
                var errors = this.GetResults(MessageType.Error);
                var warnings = this.GetResults(MessageType.Warning);

                // Add the overview node
                var overviewNode = new UltraTreeNode { Text = "Overview" };
                overviewNode.Nodes.Add(new UltraTreeNode { Text = "Successful: " + successes.Count });
                overviewNode.Nodes.Add(new UltraTreeNode{ Text = "Errors: " + errors.Count });
                overviewNode.Nodes.Add(new UltraTreeNode{ Text = "Warnings: " + warnings.Count });
                overviewNode.Expanded = true;
                tvwSummary.Nodes.Add(overviewNode);

                // Add the details
                var errorsNode = new UltraTreeNode { Text = "Errors" };
                foreach (var error in errors)
                {
                    errorsNode.Nodes.Add(new UltraTreeNode { Text = error });
                }

                var warningsNode = new UltraTreeNode { Text = "Warnings" };
                foreach (var warning in warnings)
                {
                    warningsNode.Nodes.Add(new UltraTreeNode { Text = warning });
                }

                var successNode = new UltraTreeNode { Text = "Successes" };
                foreach (var success in successes)
                {
                    successNode.Nodes.Add(new UltraTreeNode { Text = success });
                }

                var detailsNode = new UltraTreeNode { Text = "Details" };
                detailsNode.Nodes.Add(errorsNode);
                detailsNode.Nodes.Add(warningsNode);
                detailsNode.Nodes.Add(successNode);
                tvwSummary.Nodes.Add(detailsNode);
            }
            else
            {
                tvwSummary.Nodes.Add(new UltraTreeNode("None", "No actions occurred during the sync."));
            }
        }

        private List<string> GetResults(MessageType msgType)
        {
            var results = new List<string>();
            var syncResults = QBSyncController.SyncResults;
            
            if (syncResults.ContainsKey(msgType))
                results = syncResults[msgType];

            return results;
        }

        #endregion

        #region IWizardPanel Members

        public string Title { get { return "Sync Summary"; } }

        public string SubTitle { get { return "A review of the sync results."; } }

        public Action<IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller) { }

        public Control PanelControl { get { return this; } }

        public bool IsValid { get { return true; } }

        public void OnMoveTo()
        {
            this.PopulateResultsTree();
        }

        public void OnMoveFrom()
        {
            // Clear the tree, it will be populated when the user navigates back to the panel
            this.tvwSummary.Nodes.Clear();
        }

        public void OnFinished() { }

        #endregion
    }
}
