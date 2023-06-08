using DWOS.Data.Datasets;
using DWOS.Utilities.Validation;
using Infragistics.Win.UltraWinTree;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DWOS.UI.Admin.PartManagerPanels
{
    public class ProcessAnswerControlValiditor: ControlValidatorBase
    {
        #region Fields

        private Data.Datasets.PartsDatasetTableAdapters.PartProcessAnswerTableAdapter _taPPA;

        #endregion

        #region Methods

        public ProcessAnswerControlValiditor(UltraTree control)
            : base(control)
        {
            this._taPPA = new Data.Datasets.PartsDatasetTableAdapters.PartProcessAnswerTableAdapter();
        }

        internal override void ValidateControl(object sender, CancelEventArgs e)
        {
            var editor = Control as UltraTree;
            var _invalidNodes = new List<PartProcessNode>();

            if(editor != null && editor.Enabled)
            {
                foreach(UltraTreeNode node in editor.Nodes)
                {
                    //if unanswered questions has not been set
                    if(((PartProcessNode)node).UnAnsweredQuestions == -1)
                    {
                        //NOTE: This will only work correctly until the user edits the dataset 
                        //		in working memory as this query hits the backend database and the 
                        //		data may not be updated yet. Let the PPA Form keep track after it edits the dataset.
                        PartsDataset.PartProcessRow processRow = ((PartProcessNode)node).DataRow;
                        ((PartProcessNode)node).UnAnsweredQuestions = Convert.ToInt32(this._taPPA.GetUnAnsweredQuestionCount(processRow.PartProcessID));
                    }

                    if(((PartProcessNode)node).UnAnsweredQuestions > 0)
                    {
                        _invalidNodes.Add((PartProcessNode)node);
                        node.Override.NodeAppearance.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                        node.Override.NodeAppearance.ResetForeColor();
                }

                if(_invalidNodes.Count > 0)
                {
                    e.Cancel = true;
                    FireAfterValidation(false, String.Format(ErrorMessage, String.Concat(_invalidNodes.ConvertAll(ppn => ppn.Text).ToArray())));
                    return;
                }
            }

            //passed
            e.Cancel = false;
            FireAfterValidation(true, String.Empty);
        }

        public override void Dispose()
        {
            if (this._taPPA != null)
            {
                this._taPPA.Dispose();
                this._taPPA = null;
            }
            base.Dispose();
        }

        #endregion
    }
}
