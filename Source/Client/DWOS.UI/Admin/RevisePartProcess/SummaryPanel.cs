using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DWOS.Shared.Wizard;
using Infragistics.Win.UltraWinTree;

namespace DWOS.UI.Admin.RevisePartProcess
{
    public partial class SummaryPanel : UserControl, IWizardPanel
    {
        #region Fields

        private RevisePartProcessController _controller;

        #endregion

        #region Methods

        public SummaryPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region IWizardPanel Members

        public string Title => "Summary";

        public string SubTitle => string.Empty;

        public Action<IWizardPanel> OnValidStateChanged { get; set; }

        public Control PanelControl => this;

        public bool IsValid => true;

        public void Initialize(WizardController controller)
        {
            _controller = controller as RevisePartProcessController;
        }

        public void OnFinished()
        {
            _controller = null;
        }

        public void OnMoveFrom()
        {
        }

        public void OnMoveTo()
        {
            tvwSummary.Nodes.Clear();

            // Processes
            var processes = _controller.Processes?.Success ?? new List<ProcessData>();
            var processesNode = tvwSummary.Nodes.Add("Processes", $"Updated {processes.Count} Process{(processes.Count == 1 ? string.Empty : "es")}");

            foreach (var process in processes)
            {
                processesNode.Nodes.Add(new UltraTreeNode { Text = process.Name });
            }

            // Process packages
            var packages = _controller.ProcessPackages?.Success ?? new List<ProcessPackageData>();
            var packagesNode = tvwSummary.Nodes.Add("Process Packages", $"Updated {packages.Count} Package{(packages.Count == 1 ? string.Empty : "s")}");

            foreach (var package in packages)
            {
                packagesNode.Nodes.Add(new UltraTreeNode { Text = package.Name });
            }

            // Parts
            var parts = _controller.Parts?.Success ?? new List<PartData>();
            var partsNode = tvwSummary.Nodes.Add("Parts", $"Updated {parts.Count} Part{(parts.Count == 1 ? string.Empty : "s")}");

            foreach (var part in parts)
            {
                partsNode.Nodes.Add(new UltraTreeNode { Text = part.DisplayText });
            }

            // Errors
            var processErrors = _controller.Processes?.Failure ?? new List<ProcessData>();
            var partErrors = _controller.Parts?.Failure ?? new List<PartData>();
            var packageErrors = _controller.ProcessPackages?.Failure ?? new List<ProcessPackageData>();

            if (processErrors.Count > 0 || partErrors.Count > 0 || packageErrors.Count > 0)
            {
                var errorsNode = tvwSummary.Nodes.Add("Errors", "Failed to fully update some parts/processes/process packages.");
                foreach (var error in processErrors)
                {
                    errorsNode.Nodes.Add($@"Process - {error.Name}");
                }

                foreach (var error in packageErrors)
                {
                    errorsNode.Nodes.Add($"Package - {error.Name}");
                }

                foreach (var error in partErrors)
                {
                    errorsNode.Nodes.Add($@"Part - {error.DisplayText}");
                }
            }

        }

        #endregion
    }
}
