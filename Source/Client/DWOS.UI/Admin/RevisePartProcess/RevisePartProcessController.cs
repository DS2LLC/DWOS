using System;
using System.Collections.Generic;
using DWOS.Shared.Wizard;
using NLog;

namespace DWOS.UI.Admin.RevisePartProcess
{
    public class RevisePartProcessController : WizardController
    {
        #region Properties

        public override string WizardTitle => "Revise Parts and Processes";

        public RevisionResults<ProcessData> Processes { get; set; }

        public RevisionResults<PartData> Parts { get; set; }

        public RevisionResults<ProcessPackageData> ProcessPackages { get; set; }

        #endregion

        #region Methods

        public override void Finished()
        {
        }

        public WizardDialog NewDialog()
        {
            var wizard = new WizardDialog();
            var panels = new List<IWizardPanel>
            {
                new ReviseProcessesPanel(),
                new RevisePackagesPanel(),
                new RevisePartsPanel(),
                new SummaryPanel()
            };

            wizard.InitializeWizard(this, panels);
            return wizard;
        }

        #endregion
    }
}
