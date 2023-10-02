using DWOS.Data;
using DWOS.Data.Conditionals;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Datasets.PartInspectionDataSetTableAdapters;

using DWOS.Data.Order.Activity;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.UI.Documents;
using DWOS.UI.QA;
using DWOS.UI.Utilities;
using Infragistics.Win;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DWOS.UI
{
    public partial class PartInspection : Form
    {
        #region Fields

        private PartInspectionDataSet.PartInspectionRow _currentInspection;
        private bool _initializedBindings;

        private List <ControlQuestion> _questions = new List <ControlQuestion>();
        private ReworkAssessment _reworkAssessment;
        private SecurityFormWatcher _securityWatcher;

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public ControlInspectionActivity Activity { get; set; }

        public ControlInspectionActivity.ControlInspectionActivityResults ActivityResults { get; set; }

        private bool ReworkCanContinueInspections { get; set; } = true;

        public bool RemainingInspections
        {
            get { return ReworkCanContinueInspections && ActivityResults != null && ActivityResults.RemainingInspections > 0; }
        }
        
        #endregion

        #region Methods

        public PartInspection()
        {
            InitializeComponent();

            this._securityWatcher = new SecurityFormWatcher(this, this.btnCancel);
        }

        private void LoadData()
        {
            this.dsPartInspection.EnforceConstraints = false;
            this.dsPartInspection.PartInspectionType.BeginLoadData();
            this.dsPartInspection.PartInspection.BeginLoadData();
            this.dsPartInspection.Users.BeginLoadData();

            this.taLists.Fill(this.dsPartInspection.Lists);
            this.taListValues.Fill(this.dsPartInspection.ListValues);
            this.taInputType.Fill(this.dsPartInspection.d_InputType);
            this.taNumericUnits.Fill(this.dsPartInspection.NumericUnits);

            this.taInspectionType.Fill(this.dsPartInspection.PartInspectionType);
            this.taPartInspection.FillByOrderID(this.dsPartInspection.PartInspection, Activity.OrderID);
            this.taUsers.Fill(this.dsPartInspection.Users);
            this.taOrderSummary.FillById(this.dsOrderProcessing.OrderSummary, Activity.OrderID);
            this.taOrderProcesses.FillBy(this.dsOrderProcessing.OrderProcesses, Activity.OrderID);

            this.dsPartInspection.PartInspection.EndLoadData();
            this.dsPartInspection.PartInspectionType.EndLoadData();
            this.dsPartInspection.Users.EndLoadData();
            this.dsPartInspection.EnforceConstraints = true;

            this.docLinkList1.ClearLinks();
        }

        private void IntializeInspection()
        {
            if (!Activity.PartInspectionTypeID.HasValue)
            {
                return;
            }

            var inspection = this.dsPartInspection.PartInspectionType.FindByPartInspectionTypeID(Activity.PartInspectionTypeID.Value);

            if(inspection == null)
                return;

            if(!this._initializedBindings)
            {
                this.cboUser.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(this.cboUser), this.bsPartsInspection, this.dsPartInspection.PartInspection.QAUserIDColumn.ColumnName, true));
                this.dteDate.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(this.dteDate), this.bsPartsInspection, this.dsPartInspection.PartInspection.InspectionDateColumn.ColumnName, true));
                this.txtNotes.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(this.txtNotes), this.bsPartsInspection, this.dsPartInspection.PartInspection.NotesColumn.ColumnName, true));

                this._initializedBindings = true;
            }

            this.lblInspectionType.Text = inspection.Name;

            //find current inspection
            if(this.Activity.PartInspectionID > 0)
                _currentInspection = this.dsPartInspection.PartInspection.FindByPartInspectionID(this.Activity.PartInspectionID);

            //if not current inspection then create one
            if(this._currentInspection == null)
            {
                var drv = this.bsPartsInspection.AddNew() as DataRowView;
                this._currentInspection = drv.Row as PartInspectionDataSet.PartInspectionRow;

                this._currentInspection.OrderID = Activity.OrderID;
                this._currentInspection.PartQuantity = Activity.PartQuantity;
                this._currentInspection.OrderProcessID = Activity.OrderProcessID.Value;
                this._currentInspection.PartInspectionTypeID = Activity.PartInspectionTypeID.Value;
                this._currentInspection.InspectionDate = DateTime.Now;
            }
            
            this.bsPartsInspection.Filter = "PartInspectionID = " + this._currentInspection.PartInspectionID;

            this.txtOrderID.Text = Activity.OrderID.ToString();
            this.txtPartID.Text = this.taPartProcess.GetPartName(Activity.PartID);

            //find user id
            int userID = SecurityManager.Current.UserID;

            //select operator
            if(userID >= 0)
            {
                ValueListItem userItem = this.cboUser.FindItemByValue <int>(i => i == userID);
                if(userItem != null)
                    this.cboUser.SelectedItem = userItem;
            }

            this.docLinkList1.LoadLinks(LinkType.ControlInspection, inspection.PartInspectionTypeID);
            this.docLinkList1.SelectDefaultLink();
        }

        private void LoadInspectionQuestions()
        {
            if (Activity.PartInspectionTypeID.HasValue)
            {
                this.flowQuestions.SuspendLayout();
                this.flowQuestions.Controls.Clear();

                PartInspectionDataSet.PartInspectionTypeRow inspection = this.dsPartInspection.PartInspectionType.FindByPartInspectionTypeID(Activity.PartInspectionTypeID.Value);

                this.lblTestRef.Text = inspection.IsTestReferenceNull() ? "None" : inspection.TestReference;
                this.lblTestReq.Text = inspection.IsTestRequirementsNull() ? "None" : inspection.TestRequirements;

                this.taPartInspectionQuestion.FillBy(this.dsPartInspection.PartInspectionQuestion, Activity.PartInspectionTypeID.Value);
                taCondition.FillByInspection(dsPartInspection.PartInspectionQuestionCondition, Activity.PartInspectionTypeID.Value);

                foreach(PartInspectionDataSet.PartInspectionQuestionRow question in this.dsPartInspection.PartInspectionQuestion)
                {
                    var qp = new ControlQuestion();
                    qp.PartQuantity = this.Activity.PartQuantity; 
                    qp.CreateQuestion(question);
                    qp.OnSave += questionControl_OnSave;

                    this._questions.Add(qp);
                    this.flowQuestions.Controls.Add(qp);
                }

                this.flowQuestions.ResumeLayout();
            }

            bool enableButtons;
            if (this.flowQuestions.Controls.Count < 1)
            {
                int heightAdjustment = this.flowQuestionPanel.Height;
                this.flowQuestionPanel.Visible = false;

                // Resize flowBottom - otherwise, it's not visible.
                this.flowBottom.Height += heightAdjustment;

                // Resize window - cause flowBottom to use the amount of space
                // it would normally use.
                this.Height -= heightAdjustment;
                enableButtons = true;
            }
            else
            {
                enableButtons = this._questions.All(qp => qp.IsComplete);
            }

            this.btnPass.Enabled = enableButtons;
            this.btnFail.Enabled = enableButtons;
        }

        private void EvaluateConditions()
        {
            var questionIdsToSkip = new HashSet<int>();
            foreach (var question in _questions)
            {
                if (!question.IsComplete)
                {
                    continue;
                }

                var evaluator = new InspectionConditionEvaluator(question.GetCompletedAnswer(), question.InputType);

                foreach (var checkCondition in question.QuestionRow.GetCheckConditionRows())
                {
                    var conditionResult = evaluator.Evaluate(checkCondition);

                    if (!conditionResult.HasValue)
                    {
                        continue;
                    }

                    var meetsCondition = conditionResult.Value;

                    if (!meetsCondition)
                    {
                        // Skip question - at least one condition does not pass
                        questionIdsToSkip.Add(checkCondition.MainPartInspectionQuestionID);
                    }
                }
            }

            // Hide questions & show previously hidden questions that need
            // to be visible
            foreach (var question in _questions)
            {
                question.Skipped = questionIdsToSkip
                    .Contains(question.QuestionRow.PartInspectionQuestionID);
            }
        }

        private void SaveAnswerSamples(List<String> samples, int AnswerID)
        {

            try
            {
                using (var taSamples = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.InspectionAnswerSampleTableAdapter())
                {
                    var dtSamples = taSamples.GetData();
                    
                    double sampleValue = 0;
                    for (int i = 0; i < samples.Count; i++)
                    {
                        sampleValue = Convert.ToDouble(samples[i]);
                        DataRow dr =  dtSamples.NewRow();
                        dr["SampleValue"] = sampleValue;
                        dr["PartInspectionAnswer"] = AnswerID;
                        dtSamples.Rows.Add(dr);
                    }
                    taSamples.Update(dtSamples);
                }
                    
                

            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding samplevalues to table.");
            }


        }


        private bool SaveData(int acceptedQty, int rejectedQty, ReworkType? selectedReworkType = null)
        {
            try
            {
                LogManager.GetCurrentClassLogger().Info("Saving inspection data for order " + Activity.OrderID);

                //update status
                this._currentInspection.Status = rejectedQty == 0;
                this._currentInspection.RejectedQty = rejectedQty;
                this._currentInspection.AcceptedQty = acceptedQty;
                this._currentInspection.InspectionDate = DateTime.Now; //[10.31.14]

                this.bsPartsInspection.EndEdit();
                this.taPartInspection.Update(this.dsPartInspection.PartInspection);

                //Get existing answers
                this.taPartInspectionAnswer.FillByPartInspection(this.dsPartInspection.PartInspectionAnswer, this._currentInspection.PartInspectionID);

                foreach(ControlQuestion qp in this._questions)
                {
                    //Check to see if questions already exist
                    PartInspectionDataSet.PartInspectionAnswerRow originalAnswer = this.dsPartInspection.PartInspectionAnswer
                        .FirstOrDefault(pia => pia.PartInspectionQuestionID == qp.QuestionRow.PartInspectionQuestionID);
                    var AnswerRowIndex = -1;
                    //if no original than add one
                    if (originalAnswer == null)
                    {
                        var AnswerRow = this.dsPartInspection.PartInspectionAnswer.AddPartInspectionAnswerRow(this._currentInspection,
                            qp.QuestionRow,
                            qp.GetCompletedAnswer() == null ? null : qp.GetCompletedAnswer().Trim(),
                            true,
                            SecurityManager.Current.UserID,
                            DateTime.Now);
                        //save answers
                        this.taPartInspectionAnswer.Update(AnswerRow);
                        AnswerRowIndex = AnswerRow.PartInspectionAnswer;
                    }
                    else
                    {
                        //else update answer
                        originalAnswer.Answer = qp.GetCompletedAnswer() == null ? null : qp.GetCompletedAnswer().Trim();
                        originalAnswer.Completed = true;
                        originalAnswer.CompletedBy = SecurityManager.Current.UserID;
                        originalAnswer.CompletedData = DateTime.Now;
                        AnswerRowIndex = originalAnswer.PartInspectionAnswer;
                    }

               

                //update samples
                if (qp.Samples != null)
                    if ((AnswerRowIndex != -1) && (qp.Samples.Count > 0))
                    SaveAnswerSamples(qp.Samples, AnswerRowIndex);
                }

                //save answers
                this.taPartInspectionAnswer.Update(this.dsPartInspection.PartInspectionAnswer);

                //if rejected parts then we need to rework this
                Activity.FailQuantity = rejectedQty;
                Activity.PartInspectionID = _currentInspection.PartInspectionID;
                Activity.SelectedReworkType = selectedReworkType;
                Activity.PassedInspection = !selectedReworkType.HasValue; // User selects rework type after failing inspection
                
                ActivityResults = Activity.Complete() as ControlInspectionActivity.ControlInspectionActivityResults;

                return true;
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Warn("DataSet Errors: " + dsPartInspection.GetDataErrors());
                ErrorMessageBox.ShowDialog("Error saving data.", exc);
                return false;
            }
        }

        private void OnDisposeMe()
        {
            try
            {
                if(this._reworkAssessment != null)
                    this._reworkAssessment.Dispose();
                this._reworkAssessment = null;

                if(this._securityWatcher != null)
                    this._securityWatcher.Dispose();
                this._securityWatcher = null;

                this._currentInspection = null;

                if(this._questions != null)
                {
                    foreach (var cq in this._questions)
                    {
                        cq.Dispose();
                    }

                    this._questions.Clear();
                    this._questions = null;
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error disposing of part inspection.");
            }
        }

        #endregion

        #region Events

        private void PartInspection_Load(object sender, EventArgs e)
        {
            try
            {
                Activity.Initialize();
                LoadData();
                
                //load inspection type
                if (Activity.IsValid)
                {
                    IntializeInspection();
                    LoadInspectionQuestions();
                }
                else if (Activity.HasCorrectOrderStatus)
                {
                    // Unable to load part inspection
                    LogManager.GetCurrentClassLogger().Warn(
                        $"Unable to determine inspection required for order {Activity.OrderID} and order process {Activity.OrderProcessID.GetValueOrDefault(-1)}.");

                    var results = Activity.CancelInvalidInspection();

                    MessageBoxUtilities.ShowMessageBoxError(
                        $"Unable to find the part inspection for order {results.OrderID}. Moving to {results.WorkStatus}.",
                        Text);

                    Close();
                }
                else
                {
                    // Order is not actually pending inspection.
                    var errorMsg = $"{Activity.OrderID} has an incorrect work status.";
                    LogManager.GetCurrentClassLogger().Warn( errorMsg);
                    MessageBoxUtilities.ShowMessageBoxError(errorMsg, Text);
                    Close();
                }
            }
            catch(Exception exc)
            {
                this.btnFail.Enabled = false;
                this.btnPass.Enabled = false;
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void questionControl_OnSave(object sender, EventArgs e)
        {
            try
            {
                var senderControl = sender as ControlQuestion;

                if (senderControl == null)
                {
                    return;
                }

                EvaluateConditions();
                int currentIndex = this.flowQuestions.Controls.IndexOf(senderControl);
                this.btnPass.Enabled = this.btnFail.Enabled = this._questions.All(qp => qp.IsComplete);

                // Find next non-skipped question
                var nextQuestion = flowQuestions.Controls.OfType<ControlQuestion>()
                    .Skip(currentIndex + 1)
                    .FirstOrDefault(q => !q.Skipped);

                if (nextQuestion != null)
                {
                    this.flowQuestions.ScrollControlIntoView(this.flowQuestions.Controls[this.flowQuestions.Controls.Count - 1]); //scroll to end then back to this one so it is at top
                    this.flowQuestions.ScrollControlIntoView(nextQuestion);
                    nextQuestion.SetFocus();
                }
                else
                {
                    btnPass.Focus();
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error moving to next question.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void btnViewAnswers_Click(object sender, EventArgs e)
        {
            try
            {
                var processingActivity = new ProcessingActivity(Activity.OrderID,
                    new ActivityUser(SecurityManager.Current.UserID, Properties.Settings.Default.CurrentDepartment,
                        Properties.Settings.Default.CurrentLine))
                {
                    OrderProcessID = Activity.OrderProcessID
                };

                using (var frm = new OrderProcessing2(processingActivity) { Mode = OrderProcessing2.ProcessingMode.ViewOnly })
                    frm.ShowDialog(this);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error displaying the order processing answers.");
            }
        }

        private void btnPrintAnswers_Click(object sender, EventArgs e)
        {
            OrdersDataSet dsOrders = null;

            try
            {
                dsOrders = new OrdersDataSet {EnforceConstraints = false};

                using(var taUserSummary = new UserSummaryTableAdapter())
                    taUserSummary.Fill(dsOrders.UserSummary);

                using(var ta = new OrderTableAdapter())
                    ta.FillByOrderID(dsOrders.Order, Activity.OrderID);

                using (var ta = new OrderSerialNumberTableAdapter())
                {
                    ta.FillByOrder(dsOrders.OrderSerialNumber, Activity.OrderID);
                }

                var psr = new WorkOrderSummaryReport(dsOrders.Order.FirstOrDefault());
                psr.DisplayReport();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error displaying report.";
                LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
            }
            finally
            {
                dsOrders?.Dispose();
            }
        }

        private void btnPass_Click(object sender, EventArgs e)
        {
            try
            {
                bool allAnswered = this._questions.All(qp => qp.IsComplete);

                if (allAnswered && SaveData(this._currentInspection.PartQuantity, 0))
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error closing form.", exc);
                LogManager.GetCurrentClassLogger().Fatal(exc, "Error closing form.");
            }
        }

        private void btnFail_Click(object sender, EventArgs e)
        {
            try
            {
                bool allAnswered = this._questions.All(qp => qp.IsComplete);

                if(allAnswered)
                {
                    if(this._reworkAssessment == null)
                        this._reworkAssessment = new ReworkAssessment { OrderID = Activity.OrderID, InControlInspection = true, SelectedProcessAliasID = Activity.ProcessAliasID};

                    if (_reworkAssessment.ShowDialog(this) == DialogResult.OK)
                    {
                        _reworkAssessment.SaveData();

                        if (SaveData(_reworkAssessment.AcceptedQty, _reworkAssessment.FailedQty, _reworkAssessment.SelectedReworkType))
                        {
                            this._reworkAssessment.DisplayReworkSummary();
                            ReworkCanContinueInspections = this._reworkAssessment.CanContinueInspections;
                        }

                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error closing form.", exc);
                LogManager.GetCurrentClassLogger().Fatal(exc, "Error closing form.");
            }
        }
        private void btnOrderMedia_Click(object sender, EventArgs e)
        {
            MediaDialog.ShowWorkOrderDialog(this.Activity.OrderID);
        }

        private void btnPartMedia_Click(object sender, EventArgs e)
        {
            MediaDialog.ShowPartDialog(this.Activity.PartID);
        }

        private void PartInspection_Shown(object sender, EventArgs e)
        {
            try
            {
                if (flowQuestions.Controls.Count > 0)
                {
                    (flowQuestions.Controls[0] as ControlQuestion)?.SetFocus();
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error showing PartInspection dialog.");
            }
        }

        #endregion
    }
}