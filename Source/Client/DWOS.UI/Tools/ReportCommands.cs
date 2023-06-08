using System;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Interop;
using DWOS.Reports;
using DWOS.Reports.ReportOptions;
using DWOS.Shared;
using DWOS.UI.Properties;
using DWOS.UI.Reports;
using DWOS.UI.Sales;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinToolbars;
using DWOS.Data;
using System.Collections.Generic;

namespace DWOS.UI.Tools
{
    internal class CustomReportCommand: CommandBase
    {
        #region Fields

        private readonly Type _reportType;

        #endregion

        #region Methods

        public CustomReportCommand(ToolBase tool, Type reportType, string securityRole)
            : base(tool, securityRole)
        {
            this._reportType = reportType;
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                if (this._reportType == typeof(ShipRecReport))
                {
                    using (var frm = new DateAndGroupingOptions())
                    {
                        if (this._reportType == typeof(ShipRecReport))
                        {
                            var report = new ShipRecReport();
                            if (report != null)
                            {
                                frm.LoadReport("Ship/Receive", report, (from) => report.FromDate = from, (to) => report.ToDate = to, (groupBy) => report.GroupBy = groupBy);
                                frm.ShowDialog(Form.ActiveForm);
                            }
                        }
                    }
                }
                else
                {
                    using (var frm = new ReportInput())
                    {
                        var report = this._reportType.Assembly.CreateInstance(this._reportType.FullName) as Report;
                        if (report != null)
                        {
                            frm.LoadReport(report);
                            frm.ShowDialog(Form.ActiveForm);
                        }
                    }
                }
            }
        }

        #endregion
    }

    internal class CustomerVolumeCommand : CommandBase
    {
        #region Methods

        public CustomerVolumeCommand(ToolBase tool)
            : base(tool, "CustomerVolume")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                using (var frm = new DateOptions())
                {
                    var report = new CustomerPartVolumeReport();
                    frm.LoadReport("Customer Part Volume", report, (from) => report.FromDate = from, (to) => report.ToDate = to);
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
        }

        #endregion
    }

    internal class DepartmentVolumeCommand : CommandBase
    {
        #region Methods

        public DepartmentVolumeCommand(ToolBase tool)
            : base(tool, "CustomerVolume")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (var frm = new DateOptions())
            {
                var report = new DepartmentPartVolumeReport();
                frm.LoadReport("Department Part Volume", report, from => report.FromDate = from, to => report.ToDate = to);
                frm.ShowDialog(Form.ActiveForm);
            }
        }

        #endregion
    }

    internal class OrderStatusByCustomerReportCommand : CommandBase
    {
        #region Methods

        public OrderStatusByCustomerReportCommand(ToolBase tool)
            : base(tool, "CurrentOrderStatusByCustomerReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (var frm = new DateAndCustomerOptions())
                    {
                        var report = new CurrentOrderStatusReport();
                        frm.LoadReport("Order Status", report, (from) => report.FromDate = from, (to) => report.ToDate = to, (customer) => report.CustomerIds = new List<int> { customer });
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class OrderLeadTimeReportCommand : CommandBase
    {
        #region Methods

        public OrderLeadTimeReportCommand(ToolBase tool)
            : base(tool, "OrderTurnOverReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (var frm = new DateAndCustomerOptions())
                    {
                        var report = new LeadTimeReport();
                        frm.LoadReport("Lead Time", report, (from) => report.FromDate = from, (to) => report.ToDate = to, (customer) => report.CustomerID = customer);
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class StatusReportCommand : CommandBase
    {
        #region Methods

        public StatusReportCommand(ToolBase tool)
            : base(tool, "StatusReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (var filter = new OrderFilter())
                    {
                        filter.ShowDialog();
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class WorkInProcessReportCommand : CommandBase
    {
        #region Methods

        public WorkInProcessReportCommand(ToolBase tool)
            : base(tool, "StatusReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        private void DisplayReport(object report)
        {
            ((Report)report).DisplayReport();
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    Application.DoEvents();
                    
                    var report = new WorkInProgressReport();
                    if (report != null)
                    {
                        var t = new Thread(this.DisplayReport);
                        t.Start(report);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class ProductionSalesReportCommand : CommandBase
    {
        #region Methods

        public ProductionSalesReportCommand(ToolBase tool)
            : base(tool, "ProductionReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (var frm = new DateOptions())
                    {
                        var report = new ProductionSalesReportExcel();
                        frm.LoadReport("Production Sales", report, (from) => report.FromDate = from, (to) => report.ToDate = to);
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class ProcessUsageReportCommand : CommandBase
    {
        #region Methods

        public ProcessUsageReportCommand(ToolBase tool)
            : base(tool, "ProcessUsageSummaryReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (var frm = new DateOptions())
                    {
                        var report = new ProcessUsageSummaryReport();
                        frm.LoadReport("Production Summary", report, (from) => report.FromDate = from, (to) => report.ToDate = to);
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class ProcessRevenueReportCommand : CommandBase
    {
        #region Methods

        public ProcessRevenueReportCommand(ToolBase tool)
            : base(tool, "ProcessUsageSummaryReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (var frm = new DateOptions())
                    {
                        var report = new ProcessRevenueReport();
                        frm.LoadReport("Revenue Summary", report, (from) => report.FromDate = from, (to) => report.ToDate = to);
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion Methods
    }

    internal class LostAndQuarantinedOrdersReportCommand : CommandBase
    {
        #region Methods

        public LostAndQuarantinedOrdersReportCommand(ToolBase tool)
            : base(tool, "ClosedOrdersReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (var frm = new DateOptions())
                    {
                        var report = new LostAndQuarantinedOrdersReport();
                        frm.LoadReport("Lost and Quarantined", report, (from) => report.FromDate = from, (to) => report.ToDate = to);
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class OrdersByProcessReportCommand : CommandBase
    {
        #region Methods

        public OrdersByProcessReportCommand(ToolBase tool)
            : base(tool, "OrdersByProcessReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (var frm = new OrderProcessOptions())
                    {
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class OrdersOnHoldReportCommand : CommandBase
    {
        #region Methods

        public OrdersOnHoldReportCommand(ToolBase tool)
            : base(tool, "ClosedOrdersReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (var frm = new DateOptions())
                    {
                        var report = new OrdersOnHoldReport();
                        frm.LoadReport(report.Title, report, (from) => report.FromDate = from, (to) => report.ToDate = to);
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class ClosedOrdersReportCommand: CommandBase
    {
        #region Methods

        public ClosedOrdersReportCommand(ToolBase tool)
            : base(tool, "ClosedOrdersReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                try
                {
                    using(var frm = new ClosedOrdersOptions())
                    {
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
                catch(Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class SecurityGroupPermissionsReportCommand: CommandBase
    {
        #region Methods

        public SecurityGroupPermissionsReportCommand(ToolBase tool)
            : base(tool, "SecurityGroupPermissionsReport")
        {
            base.Refresh();
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                try
                {
                    using(new UsingWaitCursor(Form.ActiveForm))
                    {
                        var report = new SecurityGroupPermissionsReport();
                        report.DisplayReport();
                    }
                }
                catch(Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class SecurityAuditReportCommand: CommandBase
    {
        #region Methods

        public SecurityAuditReportCommand(ToolBase tool)
            : base(tool, "SecurityAuditReport")
        {
            base.Refresh();
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                try
                {
                    using(new UsingWaitCursor())
                    {
                        var report = new UserSecurityAuditReport();
                        report.DisplayReport();
                    }
                }
                catch(Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class UnInvoicedReportCommand: CommandBase
    {
        #region Methods

        public UnInvoicedReportCommand(ToolBase tool)
            : base(tool, "UnInvoicedReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                var report = new UnInvoicedOrdersReport();
                report.DisplayReport();
            }
        }

        #endregion
    }
    
    internal class LateOrdersReportCommand: CommandBase
    {
        #region Methods

        public LateOrdersReportCommand(ToolBase tool)
            : base(tool, "LateOrdersReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            ComboBoxForm optionForm = null;

            try
            {
                optionForm = new ComboBoxForm()
                {
                    Text = "Late Orders Report Type"
                };

                optionForm.ComboBox.Items.Add(LateOrderReport.GroupByType.Customer, "Customer");
                optionForm.ComboBox.Items.Add(LateOrderReport.GroupByType.Department, "Department");

                if (FieldUtilities.IsFieldEnabled("Order", "Product Class"))
                {
                    optionForm.ComboBox.Items.Add(LateOrderReport.GroupByType.ProductClass, "Product Class");
                }

                LateOrderReport.GroupByType defaultType;
                Enum.TryParse(UserSettings.Default.Report.LateOrderReportGroupType, out defaultType);
                var selectedItem = optionForm.ComboBox.FindItemByValue<LateOrderReport.GroupByType>(i => i == defaultType);

                if (selectedItem == null)
                {
                    optionForm.ComboBox.SelectedIndex = 0;
                }
                else
                {
                    optionForm.ComboBox.SelectedItem = selectedItem;
                }

                optionForm.FormLabel.Text = "Report Type:";

                if (optionForm.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                {
                    var reportType = optionForm.ComboBox.Value as LateOrderReport.GroupByType? ?? LateOrderReport.GroupByType.Customer;

                    Application.DoEvents();
                    var report = new LateOrderReport(reportType);
                    report.DisplayReport();

                    UserSettings.Default.Report.LateOrderReportGroupType = reportType.ToString();
                    UserSettings.Default.Save();
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error creating report.", exc);
            }
            finally
            {
                optionForm?.Dispose();
            }
        }

        #endregion
    }
    
    internal class ExternalReworkOrdersReportCommand : CommandBase
    {
        #region Methods

        public ExternalReworkOrdersReportCommand(ToolBase tool)
            : base(tool, "ReworkOrdersReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (var frm = new DateOptions())
                    {
                        var report = new ExternalReworkReport();
                        frm.LoadReport("External Rework Orders", report, (from) => report.FromDate = from, (to) => report.ToDate = to);
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class InternalReworkOrdersReportCommand : CommandBase
    {
        #region Methods

        public InternalReworkOrdersReportCommand(ToolBase tool)
            : base(tool, "ReworkOrdersReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (var frm = new DateOptions())
                    {
                        var report = new InternalReworkReport();
                        frm.LoadReport("Internal Rework Orders", report, (from) => report.FromDate = from, (to) => report.ToDate = to);
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class QuoteLogReportCommand: CommandBase
    {
        #region Methods

        public QuoteLogReportCommand(ToolBase tool)
            : base(tool, "OpenOrdersReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                try
                {
                    using(var frm = new DateOptions())
                    {
                        var report = new QuoteLogReport();
                        frm.LoadReport("Quote Log Report", report, (from) => report.FromDate = from, (to) => report.ToDate = to);
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
                catch(Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class ShippingDetailReportCommand : CommandBase
    {
        #region Methods

        public ShippingDetailReportCommand(ToolBase tool)
            : base(tool, "ShippingReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (var frm = new ShippingOptions())
                    {
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class OpenOrderValuesReportCommand : CommandBase
    {
        #region Methods

        public OpenOrderValuesReportCommand(ToolBase tool)
            : base(tool, "SalesReports")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        private void DisplayReport(object report)
        {
            ((Report)report).DisplayReport();
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    var oovReport = new OpenOrderValuesReport();
                    var t = new Thread(this.DisplayReport);
                    t.Start(oovReport);
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class ProductionByDepartmentReportCommand : CommandBase
    {
        #region Methods

        public ProductionByDepartmentReportCommand(ToolBase tool)
            : base(tool, "SalesReports")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (var frm = new DateOptions())
                    {
                        var report = new ProductionByDepartmentReport();
                        frm.LoadReport(ProductionByDepartmentReport.TITLE, report, (from) => report.FromDate = from, (to) => report.ToDate = to);
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }

    internal class ProcessScheduleReportCommand : CommandBase
    {
        #region Methods

        public ProcessScheduleReportCommand(ToolBase tool)
            : base(tool, "SalesReports")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        private void DisplayReport(object report)
        {
            ((Report)report).DisplayReport();
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    var report = new ProcessScheduleReport();
                    var t = new Thread(this.DisplayReport);
                    t.Start(report);
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating report.", exc);
                }
            }
        }

        #endregion
    }


    internal class WIPHistoryReportCommand : CommandBase
    {
        #region Methods

        public WIPHistoryReportCommand(ToolBase tool)
            : base(tool, "SalesReports")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                using (var frm = new DateOptions())
                {
                    var report = new WorkInProgressHistoryReport();
                    frm.LoadReport("WIP History", report, (from) => report.FromDate = from, (to) => report.ToDate = to);
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
        }

        #endregion
    }

    internal class DiscrepancyReportCommand : CommandBase
    {
        #region Methods

        public DiscrepancyReportCommand(ToolBase tool)
            : base(tool, "SalesReports")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                using (var frm = new DateOptions())
                {
                    var report = new COCDiscrepancyReport();
                    frm.LoadReport("Discrepancy Report", report, (from) => report.FromDate = from, (to) => report.ToDate = to);
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
        }

        #endregion
    }

    internal class SalesByESDReportCommand : CommandBase
    {
        #region Methods

        public SalesByESDReportCommand(ToolBase tool)
            : base(tool, "SalesReports")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                    var report = new SalesByEstShipDateReport();
                    report.DisplayReport();
            }
        }

        #endregion
    }

    internal class PartHistoryReportCommand : CommandBase
    {
        #region Methods

        public PartHistoryReportCommand(ToolBase tool)
            : base(tool, "PartHistoryReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                using (var optionsForm = new PartOptions(new PartHistoryReport()))
                {
                    optionsForm.ShowDialog(Form.ActiveForm);
                }
            }
        }

        #endregion
    }

   internal class BatchProductionReportCommand : CommandBase
    {
        #region Methods

        public BatchProductionReportCommand(ToolBase tool)
            : base(tool, "ProductionReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                using (var frm = new DateOptions())
                {
                    var report = new BatchProductionReport();
                    frm.LoadReport("Batch Production Report", report, (from) => report.FromDate = from, (to) => report.ToDate = to);
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
        }

        #endregion
    }

    internal class TimeTrackingReportCommand : CommandBase
    {
        #region Methods

        public TimeTrackingReportCommand(ToolBase tool)
            : base(tool, "TimeTrackingReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                if (!Enabled)
                {
                    return;
                }

                using (var form = new TimeTrackingOptions())
                {
                    form.ShowDialog(Form.ActiveForm);
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error creating report.", exc);
            }
        }

        #endregion
    }

    internal class OrderCostReportCommand : CommandBase
    {
        #region Methods

        public OrderCostReportCommand(ToolBase tool)
            : base(tool, "SalesReports")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            try
            {
                var lastMonth = DateTime.Today.AddMonths(-1);

                using (var frm = new DateAndCustomerOptions())
                {
                    frm.DefaultFromDate = DateUtilities.GetFirstDayOfMonth(lastMonth);
                    frm.DefaultToDate = DateUtilities.GetLastDayOfMonth(lastMonth);

                    var report = new OrderCostReport();
                    frm.LoadReport(report.Title, report, (from) => report.FromDate = from, (to) => report.ToDate = to, (customer) => report.CustomerID = customer);
                    frm.ShowDialog(Form.ActiveForm);
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error creating report.", exc);
            }
        }

        #endregion
    }

    internal class ProfitComparisonReportCommand : CommandBase
    {
        #region Methods

        public ProfitComparisonReportCommand(ToolBase tool)
            : base(tool, "SalesReports")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (var optionsForm = new PartOptions(new ProfitComparisonReport()))
            {
                optionsForm.ShowDialog(Form.ActiveForm);
            }
        }

        #endregion
    }

   internal class RevenueByPartReportCommand : CommandBase
    {
        #region Methods

        public RevenueByPartReportCommand(ToolBase tool)
            : base(tool, "SalesReports")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (var frm = new DateOptions())
            {
                var report = new RevenueByPartReport();
                frm.LoadReport("Revenue By Part Report", report, (from) => report.FromDate = from, (to) => report.ToDate = to);
                frm.ShowDialog(Form.ActiveForm);
            }
        }

        #endregion
    }

    internal class DeliveryPerformanceReportCommand : CommandBase
    {
        #region Methods

        public DeliveryPerformanceReportCommand(ToolBase tool)
            : base(tool, "DeliveryPerformanceReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (var frm = new DateAndProductClassOptions())
            {
                frm.LoadReportInfo("Delivery Performance Report", CreateReport);
                frm.ShowDialog(Form.ActiveForm);
            }
        }

        private Report CreateReport(DateAndProductClassOptions.ReportData reportData)
        {
            if (reportData == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(reportData.ProductClass))
            {
                return new DeliveryPerformanceSummaryReport(reportData.FromDate, reportData.ToDate);
            }

            return new DeliveryPerformanceReport(reportData.FromDate, reportData.ToDate, reportData.ProductClass);
        }

        #endregion
    }

    internal class EmployeeReceivingReportCommand : CommandBase
    {
        #region Methods

        public EmployeeReceivingReportCommand(ToolBase tool)
            : base(tool, "EmployeePerformanceReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (var frm = new DateOptions())
            {
                frm.Width += 100; // Needs additional space for title
                var report = new EmployeeReceivingReport();
                frm.LoadReport("Employee Receiving Performance", report, from => report.FromDate = from, to => report.ToDate = to);
                frm.ShowDialog(Form.ActiveForm);
            }
        }

        #endregion
    }

    internal class EmployeeProcessingReportCommand : CommandBase
    {
        #region Methods

        public EmployeeProcessingReportCommand(ToolBase tool)
            : base(tool, "EmployeePerformanceReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (var frm = new DateOptions())
            {
                frm.Width += 100; // Needs additional space for title
                var report = new EmployeeProcessingReport();
                frm.LoadReport("Employee Processing Performance", report, from => report.FromDate = from, to => report.ToDate = to);
                frm.ShowDialog(Form.ActiveForm);
            }
        }

        #endregion
    }

    internal class ProcessingAnswersReportCommand : CommandBase
    {
        #region Methods

        public ProcessingAnswersReportCommand(ToolBase tool)
            : base(tool, "ProcessingAnswersReport")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            try
            {
                var frm = new ProcessAnswerReportOptions();
                var helper = new WindowInteropHelper(frm) { Owner = DWOSApp.MainForm.Handle };
                if (frm.ShowDialog() ?? false)
                {
                    var report = frm.GenerateReport();
                    if (report == null)
                    {
                        // Report should not be null
                        _log.Error("Cannot show Processing Answers Report.");

                        MessageBoxUtilities.ShowMessageBoxError(
                            "Unable to show report. Please try again.",
                            "Processing Answers");
                    }
                    else
                    {
                        report.DisplayReport();
                    }
                }
                GC.KeepAlive(helper);
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error creating report.", exc);
            }
        }

        #endregion
    }

    internal class TurnAroundTimeReportCommand : CommandBase
    {
        #region Methods

        public TurnAroundTimeReportCommand(ToolBase tool)
            : base(tool, "SalesReports")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (var frm = new DateOptions())
            {
                var report = new TurnAroundTimeReport();
                frm.LoadReport("Turn Around Time", report, from => report.FromDate = from, to => report.ToDate = to);
                frm.ShowDialog(Form.ActiveForm);
            }
        }

        #endregion
    }

    internal class ShippedByPriorityReportCommand : CommandBase
    {
        #region Methods

        public ShippedByPriorityReportCommand(ToolBase tool)
            : base(tool, "SalesReports")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (var frm = new DateOptions())
            {
                var report = new ShippedByPriorityReport();
                frm.LoadReport("Shipped By Priority", report, from => report.FromDate = from, to => report.ToDate = to);
                frm.ShowDialog(Form.ActiveForm);
            }
        }

        #endregion
    }

    internal class RevenueByProgramReportCommand: CommandBase
    {
        #region Methods

        public RevenueByProgramReportCommand(ToolBase tool)
            : base(tool, "SalesReports")
        {
            HideIfUnAuthorized = Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (var frm = new RevenueByProgramOptions())
            {
                frm.ShowDialog(Form.ActiveForm);
            }
        }

        #endregion
    }
}