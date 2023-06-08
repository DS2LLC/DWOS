using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ScheduleDatasetTableAdapters;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinDock;
using Infragistics.Win.UltraWinGanttView;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinSchedule;
using Infragistics.Win.UltraWinToolbars;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class ProcessingSchedule: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private List<DepartmentSchedule> _deptTasks;
        private OrderPriorityCommand _orderPriorityCmd;
        private WorkScheduler _scheduler;

        #endregion

        #region Methods

        public ProcessingSchedule()
        {
            this.InitializeComponent();

            notificationPanel.NotificationPane = ultraDockManager1.PaneFromKey("notificationPane") as DockableControlPane;
        }

        private void LoadData()
        {
            this._scheduler = new WorkScheduler();
            this._scheduler.ScheduleSettings = new WorkScheduleSettings();
            this._scheduler.LoadData();

            //Load Holidays into the calendar
            using(var ta = new WorkHolidayTableAdapter())
            {
                ta.FillFromDate(this._scheduler.Data.WorkHoliday, DateTime.Now.ToShortDateString());

                foreach(ScheduleDataset.WorkHolidayRow h in this._scheduler.Data.WorkHoliday)
                {
                    this.calInfo.Holidays.Add(h.Holiday, 1, h.Name);
                    this.calInfo.DateSettings.Add(new CalendarDateSettings(h.Holiday){IsWorkDay = DefaultableBoolean.False});
                }
            }
        }

        private void LoadSchedule()
        {
            try
            {
                this.guagePercentLate.GuageCount = "0";
                this.guageTotalOrders.GuageCount = "0";
                this.guageAvgDaysLate.GuageCount = "0";

                using(new UsingWaitCursor(this))
                {
                    if(_deptTasks != null)
                    {
                        _deptTasks.Clear();
                        _deptTasks = null;
                    }

                    calInfo.Projects.Clear();
                    calInfo.Tasks.Clear();
                    calInfo.CustomTaskColumns.Clear();
                    notificationPanel.Clear();

                    //add project
                    calInfo.Projects.Add(new Project{Name = "Order Schedule", StartDate = this._scheduler.StartDate});
                    
                    //add project task
                    var projectTask = new Task{Name = "Orders", StartDateTime = this._scheduler.StartDate};
                    projectTask.TimelineSettings.BarSettingsSummary.BarAppearance.BackColor = Color.Red;
                    calInfo.Tasks.Add(projectTask);

                    //create all tasks
                    _deptTasks = this._scheduler.CreateDepartmentSchedules(DateTime.Now);

                    gvSchedule.TimelineSettings.BarSettingsSummary.BarTextInside = Infragistics.Win.UltraWinSchedule.TaskUI.BarTextField.Name;
                    calInfo.CustomTaskColumns.Add("Utilization", typeof(float), false);

                    //add the tasks to the schedule
                    foreach(var item in _deptTasks)
                    {
                        var departmentTask = new Task() {Name = item.Department};
                        departmentTask.TimelineSettings.BarSettings.BarHeight = 20; 
                        projectTask.Tasks.Add(departmentTask);
                        
                        foreach(var shiftSchedule in item.ShiftSchedules)
                        {
                            var task           = new Task { Name = shiftSchedule.Orders.Count + " Orders", Tag = shiftSchedule };
                            task.Notes         = "Parts " + shiftSchedule.PartCount + Environment.NewLine + "Orders " + shiftSchedule.Orders.Count;
                            task.StartDateTime = shiftSchedule.StartDateTime;
                            task.SetDuration(shiftSchedule.Duration, TimeSpanFormat.Hours);

                            //Show load capacity
                            task.TimelineSettings.BarSettings.BarHeight = 10;
                            task.TimelineSettings.BarSettings.PercentCompleteBarHeight = 7;
                            task.TimelineSettings.BarSettings.PercentCompleteBarAppearance.BackColor = Color.Yellow;
                            task.TimelineSettings.BarSettings.PercentCompleteBarVisible = DefaultableBoolean.True;
                            if (shiftSchedule.MaxProductionCount > 0)
                            {
                                var utlilization = ((float)shiftSchedule.PartCount / (float)shiftSchedule.MaxProductionCount) * 100;
                                task.PercentComplete = utlilization > 100 ? 100 : (utlilization < 0 ? 0 : utlilization);
                            }

                            departmentTask.Tasks.Add(task);
                            
                            task.SetCustomProperty("Utilization", task.PercentComplete);
                        }
                    }

                    notificationPanel.AddNotifications(_scheduler.Notifications);

                    this.UpdateStats();

                    LoadTimeline();
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error determining schedule.", exc);
            }
        }

        private void LoadTimeline()
        {
            calInfo.Owners.Clear();
            timeline.PrimaryInterval = new DateInterval(1, DateIntervalUnits.Days);
            
            foreach(var orderSchedule in this._scheduler.Data.OrderSchedule)
            {
                var order = orderSchedule.OrderID;
                calInfo.Owners.Add(order.ToString());

                foreach(var  orderProcess in orderSchedule.GetOrderProcessesRows())
                {
                    if(!orderProcess.IsStartDateNull() && !orderProcess.IsEndDateNull())
                        calInfo.Appointments.Add(new Appointment(orderProcess.EndDate.Subtract(TimeSpan.FromHours(8)), orderProcess.EndDate) { OwnerKey = order.ToString(), Subject = orderProcess.Department });
                }
            }

            calInfo.Owners.UnassignedOwner.Visible = false;
            timeline.ActivityHeight = 20;
            timeline.MaximumOwnersInView = 25;
        }

        /// <summary>
        ///   Loads the grid based on the selected task to show all orders in this task.
        /// </summary>
        /// <param name="task"> The task. </param>
        private void LoadGrid(Task task)
        {
            if (task != null)
            {
                var workOrderBatch = task.Tag as ShiftSchedule;
                
                if (workOrderBatch != null && workOrderBatch.Orders.Count > 0)
                {
                    var dt = workOrderBatch.Orders[0].Table.Clone(); //copy table structure

                    //add all of this tasks orders into the table
                    foreach (ScheduleDataset.OrderScheduleRow item in workOrderBatch.Orders)
                        dt.Rows.Add(item.ItemArray);

                    this.grdOrders.SetDataBinding(dt, null, true, true);
                    this.grdOrders.Text = workOrderBatch.Department + " Orders - Shift " + workOrderBatch.Shift;
                    this.grdOrders.Enabled = true;
                    this.grdOrders.Visible = true;

                    splitContainer1.Panel2Collapsed = false;
                    splitContainer1.Panel2.Show();

                    return;
                }
            }

            splitContainer1.Panel2Collapsed = true;
            splitContainer1.Panel2.Hide();

            this.grdOrders.Text = string.Empty;
            this.grdOrders.Enabled = false;
            this.grdOrders.Visible = false;
            ShowOrderInTasks(null);
        }

        /// <summary>
        ///   Updates the schedule stats.
        /// </summary>
        private void UpdateStats()
        {
            this.guagePercentLate.GuageCount = this._scheduler.PercentLate;
            this.guageTotalOrders.GuageCount = this._scheduler.TotalOrders;
            this.guageAvgDaysLate.GuageCount = this._scheduler.AvgDaysLate;
        }

        private void CreateWorkScheduleReports(int shift)
        {
            if(this._deptTasks != null)
            {
                var workItems = new List<DepartmentShiftWorkItem>();

                foreach(var deptTask in this._deptTasks)
                {
                    foreach (var workOrderBatch in deptTask.ShiftSchedules)
                    {
                        if (workOrderBatch.Generation == shift)
                        {
                            var workItem = new DepartmentShiftWorkItem
                                               {
                                                   Department = deptTask.Department,
                                                   Orders = workOrderBatch.Orders,
                                                   Shift = shift,
                                                   StartDate = workOrderBatch.StartDateTime
                                               };

                            workItems.Add(workItem);
                        }
                    }
                }

                var r = new DepartmentShiftWorkScheduleReport(workItems);
                r.DisplayReport();
            }
        }

        private void CreateScheduleSummaryReports()
        {
            var r = new ScheduleStatusReport(this._scheduler.Data.OrderSchedule);
            r.DisplayReport();
        }

        private void ShowSettings()
        {
            using(var settings = new ProcessScheduleSettings())
            {
                settings.LoadSettings(this._scheduler.ScheduleSettings, this._scheduler.ScheduleSettings.WorkSchedules);

                if(settings.ShowDialog(this) == DialogResult.OK)
                {
                    this._scheduler.ScheduleSettings.HasChanges = true;
                    this._scheduler.LoadData();
                    this.LoadSchedule();
                }
            }
        }

        private void SaveSettings()
        {
            this._scheduler.ScheduleSettings.SaveSettings();
        }

        private void ShowOrderProcesses(int order)
        {
            try
            {
                using(var mlf = new MultiLineForm())
                {
                    mlf.dropDown.InitializeLayout += (s, ee) =>
                                                     {
                                                         foreach(UltraGridColumn col in mlf.dropDown.DisplayLayout.Bands[0].Columns)
                                                             col.Hidden = true;

                                                         mlf.dropDown.DisplayLayout.Bands[0].Columns[this._scheduler.Data.OrderProcesses.StepOrderColumn.ColumnName].Hidden = false;
                                                         mlf.dropDown.DisplayLayout.Bands[0].Columns[this._scheduler.Data.OrderProcesses.OrderIDColumn.ColumnName].Hidden = false;
                                                         mlf.dropDown.DisplayLayout.Bands[0].Columns[this._scheduler.Data.OrderProcesses.StartDateColumn.ColumnName].Hidden = false;
                                                         mlf.dropDown.DisplayLayout.Bands[0].Columns[this._scheduler.Data.OrderProcesses.EndDateColumn.ColumnName].Hidden = false;
                                                         mlf.dropDown.DisplayLayout.Bands[0].Columns[this._scheduler.Data.OrderProcesses.DepartmentColumn.ColumnName].Hidden = false;
                                                         mlf.dropDown.DisplayLayout.Bands[0].Columns[this._scheduler.Data.OrderProcesses.OrderIDColumn.ColumnName].Header.Caption = "WO";

                                                         mlf.dropDown.DisplayLayout.Bands[0].Columns[this._scheduler.Data.OrderProcesses.StepOrderColumn.ColumnName].SortIndicator = SortIndicator.Ascending;
                                                     };

                    mlf.dropDown.DoubleClickRow += (s1, e1) =>
                                                   {
                                                       if(e1.Row.IsDataRow)
                                                           this.SelectOrder((ScheduleDataset.OrderProcessesRow)e1.Row.ListObject);
                                                   };

                    mlf.FormLabel.Text = "Order Processes:";
                    mlf.Text = "Order Processes";
                    mlf.dropDown.DataSource = this._scheduler.Data.OrderProcesses.Select("OrderID = " + order);
                    mlf.ShowDialog(this);
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error displaying order summary.";
                _log.Error(exc, errorMsg);
            }
        }

        private void SelectOrder(ScheduleDataset.OrderProcessesRow opRow)
        {
            try
            {
                Task deptTask = this.gvSchedule.Project.TaskFromName(opRow.Department);
                Task orderTask = null;

                foreach(Task shiftTask in deptTask.Tasks)
                {
                    if(shiftTask != null && shiftTask.Tag is ShiftSchedule)
                    {
                        var batch = shiftTask.Tag as ShiftSchedule;
                        ScheduleDataset.OrderScheduleRow os = batch.Orders.Find(osRow => Convert.ToInt32(osRow[WorkScheduleSettings.ORDERPROCESS_COLUMN]) == opRow.OrderProcessesID);

                        if(os != null)
                        {
                            orderTask = shiftTask;
                            break;
                        }
                    }
                }

                if(orderTask != null)
                {
                    //select task in gantt view
                    this.gvSchedule.ActiveTask = orderTask;
                    this.gvSchedule.EnsureTaskInView(orderTask, false);

                    this.LoadGrid(orderTask);

                    //Find order row in grid
                    UltraGridRow row = null;

                    foreach(UltraGridRow r in this.grdOrders.Rows)
                    {
                        if(Convert.ToInt32(r.Cells["OrderID"].Value) == opRow.OrderID)
                        {
                            row = r;
                            break;
                        }
                    }

                    if(row != null)
                    {
                        this.grdOrders.ActiveRow = row;
                        row.Selected = true;
                        this.grdOrders.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error displaying row task by order process row.";
                _log.Error(exc, errorMsg);
            }
        }

        private void ShowOrderInTasks(int? order)
        {
            try
            {
                if (!order.HasValue)
                {
                    this.gvSchedule.Project.Tasks[0].Tasks.ForEach(deptTask => deptTask.Tasks.ForEach(ot => ot.TimelineSettings.BarSettings.BarAppearance.Reset()));
                    return;
                }

                var foundTasks = new List <Task>();

                foreach (Task deptTask in this.gvSchedule.Project.Tasks[0].Tasks)
                {
                    foreach(Task shiftTask in deptTask.Tasks)
                    {
                        if(shiftTask != null && shiftTask.Tag is ShiftSchedule)
                        {
                            var batch = shiftTask.Tag as ShiftSchedule;
                            var os = batch.Orders.FirstOrDefault(osRow => osRow.OrderID == order.Value);

                            if(os != null)
                                foundTasks.Add(shiftTask);
                            else
                                shiftTask.TimelineSettings.BarSettings.BarAppearance.Reset();
                        }
                    }
                }
                

                foundTasks.ForEach(t =>
                                   {
                                       t.TimelineSettings.BarSettings.BarAppearance.BackColor2 = Color.Red;
                                       t.TimelineSettings.BarSettings.BarAppearance.BackColor = Color.Red;});
            }
            catch (Exception exc)
            {
                string errorMsg = "Error highlighting selected order.";
                _log.Error(exc, errorMsg);
            }
        }

        #endregion

        #region Events

        private void ProcessingSchedule_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadData();

                this._orderPriorityCmd = new OrderPriorityCommand(this.toolbarManager.Tools["OrderPriority"]);
            }
            catch(Exception exc)
            {
                string errorMsg = "Error loading order schedule.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void ProcessingSchedule_Shown(object sender, EventArgs e)
        {
            this.LoadSchedule();
        }

        private void gvSchedule_ActiveTaskChanged(object sender, ActiveTaskChangedEventArgs e)
        {
            this.LoadGrid(e.ActiveTask);
        }

        private void grdOrders_InitializeLayout(object sender, InitializeLayoutEventArgs e) {}

        private void grdOrders_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            try
            {
                int orderID = Convert.ToInt32(e.Row.Cells["OrderID"].Value);
                this.ShowOrderProcesses(orderID);
            }
            catch(Exception exc)
            {
                string errorMsg = "Error displaying order summary.";
                _log.Error(exc, errorMsg);
            }
        }

        private void ultraToolbarsManager1_ToolClick(object sender, ToolClickEventArgs e)
        {
            try
            {
                switch (e.Tool.Key)
                {
                    case "Settings": // ButtonTool
                        this.ShowSettings();
                        break;
                    case "SaveSettings": // ButtonTool
                        this.SaveSettings();
                        break;
                    case "CreateReport": // ButtonTool
                        using(var form = new TextBoxForm())
                        {
                            form.Text = "Print Schedule";
                            form.FormLabel.Text = "Shift Number";
                            form.FormTextBox.Text = this._scheduler.StartGeneration.ToString();

                            if(form.ShowDialog(this) == DialogResult.OK)
                            {
                                int shift = 0;
                                if(int.TryParse(form.FormTextBox.Text, out shift))
                                    this.CreateWorkScheduleReports(shift);
                            }
                        }
                        break;
                    case "PrintSchedule": // ButtonTool
                        this.CreateScheduleSummaryReports();
                        break;
                    case "Find": // ButtonTool
                        using(var form = new TextBoxForm())
                        {
                            form.Text = "Find Work order";
                            form.FormLabel.Text = "Work Order";

                            if(form.ShowDialog(this) == DialogResult.OK)
                            {
                                int orderID = 0;
                                if(int.TryParse(form.FormTextBox.Text, out orderID))
                                {
                                    ShowOrderProcesses(orderID);
                                    ShowOrderInTasks(orderID);
                                }
                            }
                        }
                        break;
                    case "Reload": // ButtonTool
                        this._scheduler.LoadData();
                        this.LoadSchedule();
                        break;
                    case "SaveSchedule": // ButtonTool
                        this._scheduler.SaveEstimatedShipDates();
                        break;
                    case "Differences":
                        using(var frm = new Schedule.ScheduleComparison())
                        {
                            frm.LoadData(_scheduler.Data.OrderSchedule);
                            frm.ShowDialog(this);
                        }
                        break;
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error in Processing Schedule dialog.", exc);
            }
        }

        private void ProcessingSchedule_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if(this._scheduler.ScheduleSettings.HasChanges)
                {
                    if(MessageBoxUtilities.ShowMessageBoxYesOrNo("Settings have changed. Do you want to save the changes?", "Save Changes") == DialogResult.Yes)
                        this.SaveSettings();
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error closing form.";
                _log.Error(exc, errorMsg);
            }
        }

        private void grdOrders_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if(this.grdOrders.Selected.Rows.Count > 0 && this.grdOrders.Selected.Rows[0].IsDataRow)
                this._orderPriorityCmd.SelecteOrderID = Convert.ToInt32(this.grdOrders.Selected.Rows[0].Cells["OrderID"].Value);

            int? orderId = null;

            if (grdOrders.Selected.Rows.Count > 0 && grdOrders.Selected.Rows[0].IsDataRow)
            {
                var row = ((DataRowView) grdOrders.Selected.Rows[0].ListObject).Row as ScheduleDataset.OrderScheduleRow;
                
                if(row != null)
                    orderId = row.OrderID;
            }

            ShowOrderInTasks(orderId);
        }

        private void grdOrders_ClickCell(object sender, ClickCellEventArgs e)
        {
            try
            {
                if (e.Cell != null)
                {
                    e.Cell.Row.Selected = true;

                    if (e.Cell.Column.Key == "PartName")
                    {
                        using (var p = new QuickViewPart())
                        {
                            using (var ta = new Data.Datasets.OrderStatusDataSetTableAdapters.OrderStatusTableAdapter())
                            {
                                var orderID = 0;

                                if (e.Cell.Row != null && e.Cell.Row.IsDataRow && e.Cell.Row.Band.Columns.Exists("OrderID"))
                                {
                                    object o = e.Cell.Row.GetCellValue("OrderID");
                                    if (o != null && o != DBNull.Value)
                                        orderID = Convert.ToInt32(o);
                                }

                                if(orderID > 0)
                                {
                                    int? partID = ta.GetPartID(orderID);

                                    if(partID.HasValue)
                                    {
                                        p.PartID = partID.Value;
                                        p.ShowDialog(this);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error displaying quick view.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }
        
        #endregion
    }
}