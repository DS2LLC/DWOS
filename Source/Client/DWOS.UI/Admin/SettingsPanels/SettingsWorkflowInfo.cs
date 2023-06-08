using System;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsWorkflowInfo: UserControl, ISettingsPanel
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public string PanelKey
        {
            get { return "Workflow"; }
        }

        #endregion

        #region Methods
        public bool CanDock
        {
            get { return false; }
        }

        public SettingsWorkflowInfo()
        {
            this.InitializeComponent();
            this.cboStrictnessLevel.Items.Add(new Infragistics.Win.ValueListItem(ProcessStrictnessLevel.Strict, "Strict"));
            this.cboStrictnessLevel.Items.Add(new Infragistics.Win.ValueListItem(ProcessStrictnessLevel.Normal, "Normal"));

            // Hide 'Auto Complete' option unless it's in-use.
            if (ApplicationSettings.Current.ProcessStrictnessLevel == ProcessStrictnessLevel.AutoComplete)
            {
                cboStrictnessLevel.Items.Add(new Infragistics.Win.ValueListItem(ProcessStrictnessLevel.AutoComplete, "Allow Automatic Completion"));
            }
        }

        #endregion

        #region ISettingsPanel Members

        public bool Editable
        {
            get { return SecurityManager.Current.IsInRole("ApplicationSettings.Edit"); }
        }

        public void LoadData()
        {
            try
            {
                Enabled = this.Editable;

                chkUseCOC.Checked = ApplicationSettings.Current.COCEnabled;
                chkSkipCocByDefault.Checked = !ApplicationSettings.Current.AllowSkippingCoc;
                chkPartMarking.Checked = ApplicationSettings.Current.PartMarkingEnabled;
                chkDisplayProcessCOC.Checked = ApplicationSettings.Current.DisplayProcessCOCByDefault;
                chkCocCustomerAlias.Checked = ApplicationSettings.Current.DisplayCustomerProcessAliasOnCoc;
                chkAllowProcessSplit.Checked = ApplicationSettings.Current.AllowPartialProcessLoads;
                chkMulitpleBatchProcesses.Checked = ApplicationSettings.Current.BatchMultipleProcesses;
                chkTimeTracking.Checked = ApplicationSettings.Current.TimeTrackingEnabled;
                chkMultipleLines.Checked = ApplicationSettings.Current.MultipleLinesEnabled;
                chkOrderReview.Checked = ApplicationSettings.Current.OrderReviewEnabled;
                chkOrderApproval.Checked = ApplicationSettings.Current.OrderApprovalEnabled;
                numApprovalReminderDays.Value = ApplicationSettings.Current.OrderApprovalReminderDays;
                chkImportExport.Checked = ApplicationSettings.Current.ImportExportApprovalEnabled;
                chkCheckIn.Checked = ApplicationSettings.Current.OrderCheckInEnabled;
                chkRepairStatement.Checked = ApplicationSettings.Current.RepairStatementEnabled;
                chkBillOfLading.Checked = ApplicationSettings.Current.BillOfLadingEnabled;
                chkLadingIncludeContainers.Checked = ApplicationSettings.Current.BillOfLadingIncludeContainers;
                chkPartProcesses.Checked = ApplicationSettings.Current.PartProcessRequired;
                numBarcodeInterval.Value = ApplicationSettings.Current.BarcodeIntervalMilliseconds;
                chkProcessSuggestions.Checked = ApplicationSettings.Current.UseProcessSuggestions;
                chkShowBatchDeletePrompt.Checked = ApplicationSettings.Current.ShowBatchDeletePrompt;
                chkAutoBatch.Checked = ApplicationSettings.Current.AutomaticallyBatchSalesOrder;
                chkContinueBatchAfterProcessing.Checked = ApplicationSettings.Current.ContinueBatchAfterProcessing;
                chkRequireSingleProductClassShipping.Checked = ApplicationSettings.Current.RequireSingleProductClassForShipments;
                chkAutoImportProcesses.Checked = ApplicationSettings.Current.AutoImportProcessesToOrder;
                chkReceivingCanAddParts.Checked = ApplicationSettings.Current.ReceivingCanAddParts;
                cboShowCODonTravelers.Checked = ApplicationSettings.Current.ShowCODOnTraveler;

                chkSkipCocByDefault.Enabled = chkUseCOC.Checked;
                chkDisplayProcessCOC.Enabled = chkUseCOC.Checked;
                chkCocCustomerAlias.Enabled = chkUseCOC.Checked;

                chkLadingIncludeContainers.Enabled = chkBillOfLading.Checked;

                var inspectionLevel = ApplicationSettings.Current.ProcessStrictnessLevel;
                var pricingTypeItem = this.cboStrictnessLevel.FindItemByValue<ProcessStrictnessLevel>((i) => i == inspectionLevel);

                if (pricingTypeItem != null)
                {
                    this.cboStrictnessLevel.SelectedItem = pricingTypeItem;
                }
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error loading settings.";
                _log.Error(exc, errorMsg);
            }
        }

        public void SaveData()
        {
            ApplicationSettings.Current.COCEnabled = this.chkUseCOC.Checked;
            ApplicationSettings.Current.AllowSkippingCoc = !chkSkipCocByDefault.Checked;
            ApplicationSettings.Current.PartMarkingEnabled = this.chkPartMarking.Checked;
            ApplicationSettings.Current.DisplayProcessCOCByDefault = this.chkDisplayProcessCOC.Checked;
            ApplicationSettings.Current.DisplayCustomerProcessAliasOnCoc = chkCocCustomerAlias.Checked;
            ApplicationSettings.Current.AllowPartialProcessLoads = this.chkAllowProcessSplit.Checked;
            ApplicationSettings.Current.BatchMultipleProcesses = this.chkMulitpleBatchProcesses.Checked;
            ApplicationSettings.Current.TimeTrackingEnabled = this.chkTimeTracking.Checked;
            ApplicationSettings.Current.MultipleLinesEnabled = this.chkMultipleLines.Checked;
            ApplicationSettings.Current.OrderReviewEnabled = this.chkOrderReview.Checked;
            ApplicationSettings.Current.OrderApprovalEnabled = chkOrderApproval.Checked;
            ApplicationSettings.Current.OrderApprovalReminderDays = numApprovalReminderDays.Value as int? ?? 1;
            ApplicationSettings.Current.ImportExportApprovalEnabled = chkImportExport.Checked;
            ApplicationSettings.Current.OrderCheckInEnabled = this.chkCheckIn.Checked;
            ApplicationSettings.Current.RepairStatementEnabled = this.chkRepairStatement.Checked;
            ApplicationSettings.Current.BillOfLadingEnabled = chkBillOfLading.Checked;
            ApplicationSettings.Current.BillOfLadingIncludeContainers = chkLadingIncludeContainers.Checked;
            ApplicationSettings.Current.PartProcessRequired = this.chkPartProcesses.Checked;
            ApplicationSettings.Current.BarcodeIntervalMilliseconds = Convert.ToInt32(this.numBarcodeInterval.Value);
            ApplicationSettings.Current.UseProcessSuggestions = chkProcessSuggestions.Checked;
            ApplicationSettings.Current.ShowBatchDeletePrompt = chkShowBatchDeletePrompt.Checked;
            ApplicationSettings.Current.AutomaticallyBatchSalesOrder = chkAutoBatch.Checked;
            ApplicationSettings.Current.ContinueBatchAfterProcessing = chkContinueBatchAfterProcessing.Checked;
            ApplicationSettings.Current.RequireSingleProductClassForShipments = chkRequireSingleProductClassShipping.Checked;
            ApplicationSettings.Current.AutoImportProcessesToOrder = chkAutoImportProcesses.Checked;
            ApplicationSettings.Current.ReceivingCanAddParts = chkReceivingCanAddParts.Checked;
            ApplicationSettings.Current.ShowCODOnTraveler = cboShowCODonTravelers.Checked;

            var inspectionLevel = (ProcessStrictnessLevel)(this.cboStrictnessLevel.SelectedItem?.DataValue ?? ProcessStrictnessLevel.Strict);
            ApplicationSettings.Current.ProcessStrictnessLevel = inspectionLevel;
        }

        #endregion

        private void chkUseCOC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                chkSkipCocByDefault.Enabled = chkUseCOC.Checked;
                chkDisplayProcessCOC.Enabled = chkUseCOC.Checked;
                chkCocCustomerAlias.Enabled = chkUseCOC.Checked;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling 'use COC' change.");
            }
        }

        private void chkBillOfLading_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                chkLadingIncludeContainers.Enabled = chkBillOfLading.Checked;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling Bill of Lading option change");
            }
        }
    }
}