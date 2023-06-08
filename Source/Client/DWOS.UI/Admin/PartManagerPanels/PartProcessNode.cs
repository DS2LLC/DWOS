using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using System.Collections.Generic;
using System.Data;

namespace DWOS.UI.Admin.PartManagerPanels
{
    internal class PartProcessNode: DataNode<PartsDataset.PartProcessRow>
    {
        public const string KEY_PREFIX = "PP";

        #region Fields

        private readonly List<KeyValuePair<int, int>> _partProcessUnanswered;

        #endregion

        #region Properties

        public int UnAnsweredQuestions
        {
            get
            {
                //if not in valid state then return 0 to prevent trying to validate the node.
                if(base.DataRow == null || base.DataRow.RowState == DataRowState.Detached || this._partProcessUnanswered == null)
                    return 0;

                foreach(var kvp in this._partProcessUnanswered)
                {
                    if(kvp.Key == base.DataRow.PartProcessID)
                        return kvp.Value;
                }

                return -1;
            }
            set
            {
                //find existing then remove it
                var kvpFound = new KeyValuePair<int, int>(0, 0);
                foreach(var kvp in this._partProcessUnanswered)
                {
                    if(kvp.Key == base.DataRow.PartProcessID)
                    {
                        kvpFound = kvp;
                        break;
                    }
                }

                if(kvpFound.Key != 0)
                    this._partProcessUnanswered.Remove(kvpFound);

                //add updated key
                this._partProcessUnanswered.Add(new KeyValuePair<int, int>(base.DataRow.PartProcessID, value));
            }
        }

        public bool AlreadyDisplayedWarning { get; set; }

        #endregion

        #region Methods

        public PartProcessNode(PartsDataset.PartProcessRow cr, List<KeyValuePair<int, int>> partProcessUnanswered)
            : base(cr, cr.PartProcessID.ToString(), KEY_PREFIX, cr.ProcessID.ToString())
        {
            this._partProcessUnanswered = partProcessUnanswered;

            LeftImages.Add(Properties.Resources.Process_16);

            UpdateNodeUI();

            bool showError = !base.DataRow.ProcessRow.Active ||
                base.DataRow.ProcessRow.IsIsApprovedNull() ||
                !base.DataRow.ProcessRow.IsApproved;

            if (showError)
                base.LeftImages.Add(Properties.Resources.RoundDashRed_32);
        }

        public override bool CanDelete
        {
            get
            {
                return true;
            }
        }

        public override void UpdateNodeUI()
        {
            string processName = null;

            if(base.DataRow.ProcessAliasRow.Name.Contains(base.DataRow.ProcessRow.ProcessName))
                processName = base.DataRow.ProcessAliasRow.Name;
            else
                processName = base.DataRow.ProcessAliasRow.Name + " [" + base.DataRow.ProcessRow.ProcessName + "]";

            Text = base.DataRow.StepOrder + " - " + processName;
        }

        #endregion
    }
}
