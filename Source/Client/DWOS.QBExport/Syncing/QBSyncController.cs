using System.Collections.Generic;
using DWOS.Shared.Wizard;

namespace DWOS.QBExport.Syncing
{
    /// <summary>
    /// Controller for QuickBooks sync operations.
    /// </summary>
    public class QBSyncController : WizardController
    {
        #region Properties

        /// <summary>
        /// Gets or sets the source for the sync.
        /// </summary>
        internal static SyncSource Source { get; set; }

        /// <summary>
        /// Gets or sets the results of the sync.
        /// </summary>
        internal static Dictionary<MessageType, List<string>> SyncResults { get; set; } 

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new dialog for QuickBooks sync using a new
        /// <see cref="QBSyncController"/> instance.
        /// </summary>
        /// <returns></returns>
        public static WizardDialog CreateWizard()
        {
            var ctrl = new QBSyncController();
            var wizard = new WizardDialog();
            var panels = new List<IWizardPanel>
                             {
                                 new SourceSelection(), 
                                 new SyncProgress(), 
                                 new SyncSummary()
                             };

            wizard.InitializeWizard(ctrl, panels);

            return wizard;
        }

        #endregion

        #region WizardController Members

        public override string WizardTitle
        {
            get
            {
                return "QuickBooks Sync Wizard";
            }
        }

        public override void Finished() { }

        #endregion
    }
}
