using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ScheduleDatasetTableAdapters;
using NLog;
using DWOS.Shared.Utilities;

namespace DWOS.Data
{
    /// <summary>
    ///   Responsible for creating a schedule.
    /// </summary>
    public class WorkScheduler : IWorkScheduler
    {
        #region Fields
        
        private const int MAX_GENERATIONS = 100;
        private const int MAX_SHIFTS = 3;

        public const string DEPT_COLUMN = "NextDept";
        public const string PROCESS_COLUMN = "NextProcess";
        public const string PROCESS_NAME_COLUMN = "NextProcessName";
        public const string ORDERPROCESS_COLUMN = "NextOrderProcess";
        public const string SCORE_COLUMN = "Score";
        public const string LASTESTSHIPDATE_COLUMN = "LastEstShipDate";
        
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private List<DepartmentSchedule> _deptSchedules;

        #endregion

        #region Properties

        public WorkScheduleSettings ScheduleSettings { get; set; }

        public DateTime StartDate { get; private set; }

        public int StartGeneration { get; private set; }

        public ScheduleDataset Data { get; private set; }

        public string PercentLate { get; private set; }

        public string TotalOrders { get; private set; }

        public string AvgDaysLate { get; private set; }

        public List<NotificationMessage> Notifications { get; private set; }

        #endregion

        #region Methods

        public WorkScheduler() { Notifications = new List <NotificationMessage>(); }

        public void LoadData()
        {
            if (this.Data == null)
            {
                Data = new ScheduleDataset();
                Data.OrderSchedule.Columns.Add(LASTESTSHIPDATE_COLUMN, typeof(DateTime));
            }

            this.Data.EnforceConstraints = false;

            using (var taProcess = new ProcessTableAdapter())
                taProcess.Fill(this.Data.Process);
            
            using (var taDepartment = new d_DepartmentTableAdapter())
                taDepartment.Fill(this.Data.d_Department);

            using (var taOrderSchedule = new OrderScheduleTableAdapter())
                taOrderSchedule.Fill(this.Data.OrderSchedule);

            using (var taOrderProcesses = new OrderProcessesTableAdapter())
                taOrderProcesses.Fill(this.Data.OrderProcesses);

            if(!this.Data.OrderProcesses.Columns.Contains("Generation"))
                this.Data.OrderProcesses.Columns.Add("Generation", typeof(int));

            this.Data.EnforceConstraints = true;
        }

        public List<DepartmentSchedule> CreateDepartmentSchedules(DateTime startTime, int? startShift = null)
        {
            SetStartDate(startTime, startShift);
            Notifications.Clear();
            RemoveOrdersNotToBeScheduled();

            _deptSchedules        = new List<DepartmentSchedule>();
            var currentGeneration = this.StartGeneration;
            var shiftSchedules    = this.CreateShiftSchedules(currentGeneration); //scheduled tasks for current generation

            while (this.Data.OrderProcesses.Select("EndDate IS NULL").Length > 0 && currentGeneration < MAX_GENERATIONS)
            {
                _log.Debug("Creating department schedules for generation {0} with {1} remaining orders.".FormatWith(currentGeneration, this.Data.OrderProcesses.Select("EndDate IS NULL").Length));

                foreach (var deptRow in this.Data.d_Department)
                {
                    var scheduleTasksByDept = shiftSchedules.FindAll(st => st.Department == deptRow.DepartmentID);

                    if (scheduleTasksByDept.Count > 0)
                    {
                        //add department task if does not already exist
                        if (!_deptSchedules.Exists(dt => dt.Department == deptRow.DepartmentID))
                            _deptSchedules.Add(new DepartmentSchedule() { Department = deptRow.DepartmentID, ShiftSchedules = new List<ShiftSchedule>() });

                        var deptTask = _deptSchedules.FirstOrDefault(dt => dt.Department == deptRow.DepartmentID);

                        foreach (var deptScheduleTask in scheduleTasksByDept)
                            deptTask.ShiftSchedules.Add(deptScheduleTask);
                    }
                }

                //Update End Times for OrderProcessing to prevent the order process step from getting selected again
                foreach (var shiftSchedule in shiftSchedules)
                {
                    shiftSchedule.Generation = currentGeneration;

                    foreach (ScheduleDataset.OrderScheduleRow order in shiftSchedule.Orders)
                    {
                        if (!order.IsNull(ORDERPROCESS_COLUMN))
                        {
                            ScheduleDataset.OrderProcessesRow op = this.Data.OrderProcesses.FindByOrderProcessesID(Convert.ToInt32(order["NextOrderProcess"]));

                            if(op != null)
                            {
                                op.EndDate = shiftSchedule.EndDateTimeResolved;
                                op["Generation"] = currentGeneration;
                            }
                        }
                    }
                }

                //create next generation of tasks
                currentGeneration++;
                shiftSchedules = this.CreateShiftSchedules(currentGeneration);
            }

            this.LogUnprocessedOrders();

            this.CalculateTaskStartTimes(_deptSchedules);
            this.CalculateEstimatedShipDate();
            this.CalculateStats();

            return _deptSchedules;
        }

        public void SaveEstimatedShipDates()
        {
            using (var ta = new OrderScheduleTableAdapter())
            {
                foreach (ScheduleDataset.OrderScheduleRow os in this.Data.OrderSchedule)
                {
                    if (!os.IsEstShipDateNull())
                    {
                        ta.UpdateEstShipDate(os.EstShipDate, os.OrderID);

                        if (os.IsInitialEstShipDateNull())
                            ta.UpdateInitialEstShipDate(os.EstShipDate.ToShortDateString(), os.OrderID);
                    }
                }
            }

            using (var ta = new OrderProcessesTableAdapter())
            {
                //Update estimated ship date for each order processes
                foreach (ScheduleDataset.OrderProcessesRow op in this.Data.OrderProcesses)
                {
                    if (!op.IsEstEndDateNull())
                        ta.UpdateEstEndDate(op.EstEndDate, op.OrderProcessesID);
                }
            }

            //Save work schedule summary
            using (var taWSS = new WorkScheduleSummaryTableAdapter())
            {
                using (var taWSD = new WorkScheduleDetailTableAdapter())
                {
                    var dsSchedule = new ScheduleDataset();

                    foreach (var deptTask in this._deptSchedules)
                    {
                        foreach (var batch in deptTask.ShiftSchedules)
                        {
                            if (batch.Generation == this.StartGeneration)
                            {
                                var wsSummary            = dsSchedule.WorkScheduleSummary.NewWorkScheduleSummaryRow();
                                wsSummary.DepartmentID   = batch.Department;
                                wsSummary.ScheduledDate  = batch.StartDateTime;// this.StartDate;
                                wsSummary.Shift          = batch.Generation;
                                wsSummary.EstimatedParts = batch.PartCount;
                                wsSummary.RunDate        = DateTime.Now;

                                dsSchedule.WorkScheduleSummary.AddWorkScheduleSummaryRow(wsSummary);

                                foreach (ScheduleDataset.OrderScheduleRow o in batch.Orders)
                                    dsSchedule.WorkScheduleDetail.AddWorkScheduleDetailRow(wsSummary, o.OrderID, Convert.ToInt32(o[ORDERPROCESS_COLUMN]));

                                taWSS.Update(dsSchedule.WorkScheduleSummary);
                                taWSD.Update(dsSchedule.WorkScheduleDetail);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void SetStartDate(DateTime startTime, int? startShift)
        {
            if (startTime.DayOfWeek == System.DayOfWeek.Saturday)
            {
                startTime = startTime.AddDays(2);
                startTime = startTime.AddHours(-startTime.Hour + 1);
            }
            if (startTime.DayOfWeek == System.DayOfWeek.Sunday)
            {
                startTime = startTime.AddDays(1);
                startTime = startTime.AddHours(-startTime.Hour + 1);
            }

            this.StartDate = startTime.StartOfDay();
            this.StartGeneration = startShift.HasValue ? startShift.Value : this.CalculateShiftByHour(startTime.Hour);
        }

        /// <summary>
        ///   Calculates the shift based on the hour of the day.
        /// </summary>
        /// <param name="hour"> The hour [0-23]. </param>
        /// <returns> </returns>
        public int CalculateShiftByHour(int hour)
        {
            int totalHoursFor3Shifts = (WorkScheduleSettings.DAYSTARTTIMEHOURS + WorkScheduleSettings.SHIFTLENGTHHOURS * MAX_SHIFTS);

            if (this.ScheduleSettings.MaxNumberOfShifts >= 3 && hour >= totalHoursFor3Shifts - (WorkScheduleSettings.SHIFTLENGTHHOURS * 1))
                return 3;
            else if (this.ScheduleSettings.MaxNumberOfShifts >= 2 && hour >= totalHoursFor3Shifts - (WorkScheduleSettings.SHIFTLENGTHHOURS * 2))
                return 2;
            else
                return 1;
        }

        /// <summary>
        ///   Calcualtes the task start times.
        /// </summary>
        /// <param name="deptTasks"> The dept tasks. </param>
        private void CalculateTaskStartTimes(List<DepartmentSchedule> deptTasks)
        {
            foreach (var deptTask in deptTasks)
            {
                foreach (var workOrderBatch in deptTask.ShiftSchedules)
                {
                    TimeSpan startTime           = this.CalculateStartTime(workOrderBatch.Department, workOrderBatch.Generation);
                    workOrderBatch.StartDateTime = this.StartDate.Add(startTime);
                    workOrderBatch.Duration      = TimeSpan.FromHours(WorkScheduleSettings.SHIFTLENGTHHOURS);

                    _log.Debug("Setting times gen " + workOrderBatch.Generation + " dept " + workOrderBatch.Department + " start " + startTime.TotalHours);

                    foreach (ScheduleDataset.OrderScheduleRow order in workOrderBatch.Orders)
                    {
                        if (!order.IsNull(ORDERPROCESS_COLUMN))
                        {
                            ScheduleDataset.OrderProcessesRow op = this.Data.OrderProcesses.FindByOrderProcessesID(Convert.ToInt32(order["NextOrderProcess"]));

                            if (op != null)
                            {
                                if (op.IsStartDateNull())
                                    op.StartDate = workOrderBatch.StartDateTime;

                                op.EndDate = workOrderBatch.EndDateTimeResolved;
                                op.EstEndDate = workOrderBatch.EndDateTimeResolved;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///   Calculates the start time for the department based on the generation.
        /// </summary>
        /// <param name="department"> The department. </param>
        /// <param name="generation"> The generation. </param>
        /// <returns> </returns>
        private TimeSpan CalculateStartTime(string department, int generation)
        {
            //int numberOfShiftsPerDay = this.ScheduleSettings.NumberOfShifts(department);
            int calculatedShift = this.CalculateShift(generation);
            int calculatedDays = Convert.ToInt32(Math.Ceiling(generation / (double)this.ScheduleSettings.MaxNumberOfShifts)) - 1;

            calculatedDays = DateUtilities.AddBusinessDays(this.StartDate, calculatedDays).Subtract(this.StartDate).Days;

            //Given generation 2
            //Day => calculated day
            //Hour of Day => shiftOffset +  ((calculatedShift - 1) * shiftLengthHours) = 6AM + ((2 - 1) * 8) => 2PM
            return new TimeSpan(calculatedDays, WorkScheduleSettings.DAYSTARTTIMEHOURS + ((calculatedShift - 1) * WorkScheduleSettings.SHIFTLENGTHHOURS), 0, 0);
        }

        /// <summary>
        ///   Determines whether this is a valid generation for department.
        /// </summary>
        /// <param name="department"> The department. </param>
        /// <param name="generation"> The generation. </param>
        /// <returns> <c>true</c> if [is valid generation for department] [the specified department]; otherwise, <c>false</c> . </returns>
        private bool IsValidGenerationForDepartment(string department, int generation)
        {
            int numberOfShiftsPerDay = this.ScheduleSettings.NumberOfShifts(department);

            //No shift means this dept is not defined in the setting dialog
            if(numberOfShiftsPerDay <= 0)
            {
                var id = "NO_" + department;
                
                if(!this.Notifications.Exists(nm => nm.Id == id)) //only add message 1 time
                    this.Notifications.Add(new NotificationMessage() {Id = id, Message = "There are no shifts defined for the department {0}.".FormatWith(department), Level = NotificationLevel.Warning});

                return false;
            }

            //if is dept with max number of shifts then always a valid shift
            if (this.ScheduleSettings.MaxNumberOfShifts == numberOfShiftsPerDay)
                return true;
            else
            {
                int calculatedShift = this.CalculateShift(generation);

                //return true if the calculated shift is avalialbe for this department
                return calculatedShift <= numberOfShiftsPerDay;
            }
        }

        /// <summary>
        ///   Calcualtes the shift based on the generation.
        /// </summary>
        /// <param name="generation"> The generation. </param>
        /// <returns> </returns>
        private int CalculateShift(int generation)
        {
            if (generation <= this.ScheduleSettings.MaxNumberOfShifts)
                return generation;

            int calculatedShift = generation - this.ScheduleSettings.MaxNumberOfShifts;

            //determine which shift it is
            while (calculatedShift > this.ScheduleSettings.MaxNumberOfShifts)
                calculatedShift = calculatedShift - this.ScheduleSettings.MaxNumberOfShifts;

            Debug.Assert(calculatedShift >= 1 && calculatedShift <= this.ScheduleSettings.MaxNumberOfShifts, "Shift is not within correct range.");

            return calculatedShift;
        }

        /// <summary>
        ///   Creates the schedule tasks for this generation.
        /// </summary>
        /// <param name="generation"> The generation. </param>
        /// <returns> </returns>
        private List<ShiftSchedule> CreateShiftSchedules(int generation)
        {
            _log.Info("Creating dept shift schedules for generation " + generation);

            var shiftSchedules = new List<ShiftSchedule>();

            //add additional columns to the table
            var dtOrders = this.Data.OrderSchedule.Copy() as ScheduleDataset.OrderScheduleDataTable;
            dtOrders.Columns.Add(DEPT_COLUMN, typeof(string));
            dtOrders.Columns.Add(PROCESS_NAME_COLUMN, typeof(string));
            dtOrders.Columns.Add(PROCESS_COLUMN, typeof(int));
            dtOrders.Columns.Add(ORDERPROCESS_COLUMN, typeof(int));
            dtOrders.Columns.Add(SCORE_COLUMN, typeof(int));

            this.LoadNextProcesses(dtOrders);
            this.ScoreOrders(dtOrders, generation);

            foreach (var deptRow in this.Data.d_Department)
            {
                //if not a valid shift for the department then don't create a task (is working this day and shift)
                if (!this.IsValidGenerationForDepartment(deptRow.DepartmentID, generation))
                    continue;

                var dept               = deptRow.DepartmentID;
                var ordersList         = (dtOrders.Select(DEPT_COLUMN + " = '" + DWOS.Data.Datasets.Utilities.SqlBless(dept) + "'", SCORE_COLUMN + " DESC") as ScheduleDataset.OrderScheduleRow[]).ToList();
                var maxProductionCount = this.ScheduleSettings.PartProductionCount(dept, this.CalculateShift(generation));

                if (ordersList.Count > 0)
                {
                    var batch = new ShiftSchedule { Department = dept, Orders = new List<ScheduleDataset.OrderScheduleRow>(), Shift = CalculateShift(generation), MaxProductionCount = maxProductionCount };
                    shiftSchedules.Add(batch);

                    while (ordersList.Count > 0)
                    {
                        var orderScheduleRow = this.FindNextAvailableOrder(ordersList, batch.PartCount, maxProductionCount);

                        //if unable to find the next order to include in this batch
                        if (orderScheduleRow == null)
                            break; //only complete one scheduled task for this generation (shift)
                        else
                        {
                            batch.Orders.Add(orderScheduleRow);

                            if (!orderScheduleRow.IsPartQuantityNull())
                            {
                                batch.PartCount += orderScheduleRow.PartQuantity;
                            }

                            ordersList.Remove(orderScheduleRow);
                        }
                    }
                }
            }

            return shiftSchedules;
        }

        /// <summary>
        ///   Create a score for every order where low score is first priority.
        /// </summary>
        /// <param name="orders"> The orders. </param>
        /// <param name="generation"> The generation. </param>
        private void ScoreOrders(ScheduleDataset.OrderScheduleDataTable orders, int generation)
        {
            var startTime = this.CalculateStartTime(null, generation);
            var processingDate = this.StartDate.Add(startTime);

            foreach (var order in orders)
            {
                int orderScore    = order.SchedulePriority * ApplicationSettings.Current.ScheduleOrderPriorityMultiplier;
                int priorityScore = this.ScheduleSettings.GetPriorityWeight(order["Priority"].ToString());
                int customerScore = this.ScheduleSettings.GetCustomerScore((int)order["CustomerID"]);
                int processScore  = this.Data.OrderProcesses.Select("OrderID = " + order["OrderID"] + " AND EndDate IS NULL").Length * ApplicationSettings.Current.ScheduleProcessCountMultiplier; ; //remaining processes

                int timeScore = 0;
                var requiredDate = order.IsRequiredDateNull() ? DateTime.MaxValue : order.RequiredDate;
                var daysLate = requiredDate.Date.Subtract(processingDate.Date).Days;
                if (daysLate <= 0)
                    timeScore = (Math.Abs(daysLate) + 1) * ApplicationSettings.Current.ScheduleDaysLateCountMultiplier;
                else
                    timeScore = (ApplicationSettings.Current.ScheduleDaysLateCountMultiplier / 2) - daysLate;
                
                //Higher the score the more likely to get picked for this shift
                order[SCORE_COLUMN] = priorityScore + timeScore + customerScore + processScore + orderScore;
            }
        }

        /// <summary>
        ///   Finds the next order that does not exceed the max part count.
        /// </summary>
        /// <param name="rows"> The rows. </param>
        /// <param name="currentPartCount"> The current part count. </param>
        /// <param name="maxPartCount"> The max part count. </param>
        /// <returns> </returns>
        private ScheduleDataset.OrderScheduleRow FindNextAvailableOrder(List<ScheduleDataset.OrderScheduleRow> rows, int currentPartCount, int maxPartCount)
        {
            for (int i = 0; i < rows.Count; i++)
            {
                //if this is the first order (no current part count), then use it -- Required also incase partcount is greater than PARTCOUNTPERSHIFT
                if (currentPartCount == 0)
                    return rows[0];

                //if the current part count + next part count is not over the limit then use that part
                var orderPartCount = rows[i].IsPartQuantityNull() ? 0 : rows[i].PartQuantity;

                if (currentPartCount + orderPartCount <= maxPartCount)
                    return rows[i];
            }

            return null;
        }

        /// <summary>
        ///   Loads the next department for each order into the table.
        /// </summary>
        /// <param name="orders"> The orders. </param>
        private void LoadNextProcesses(ScheduleDataset.OrderScheduleDataTable orders)
        {
            foreach (var order in orders)
            {
                //Get Next un-processed 
                var processes = this.Data.OrderProcesses.Select("EndDate IS NULL AND OrderID = " + order["OrderID"], "StepOrder");
                
                if (processes != null && processes.Length > 0)
                {
                    order[DEPT_COLUMN]         = processes[0]["Department"];
                    order[PROCESS_COLUMN]      = processes[0]["ProcessID"];
                    order[ORDERPROCESS_COLUMN] = processes[0]["OrderProcessesID"];
                    order[PROCESS_NAME_COLUMN] = this.Data.Process.FindByProcessID((int)processes[0]["ProcessID"]).Name;
                }
            }
        }

        private void CalculateStats()
        {
            int lateCount = 0;
            var averages = new List<double>();

            foreach (ScheduleDataset.OrderScheduleRow os in this.Data.OrderSchedule)
            {
                var ops = this.Data.OrderProcesses.Select("OrderID = " + os.OrderID, "StepOrder DESC") as ScheduleDataset.OrderProcessesRow[];

                //if last step date to be completed is null or negative then must be late
                if (ops.Length > 0)
                {
                    if (ops[0].IsEndDateNull())
                        lateCount++;
                    else
                    {
                        var requiredDate = os.IsRequiredDateNull() ? DateTime.MaxValue : os.RequiredDate;
                        double daysEarly = requiredDate.Subtract(ops[0].EndDate).TotalDays;

                        if (daysEarly < 0)
                        {
                            lateCount++;
                            averages.Add(Math.Abs(daysEarly));
                        }
                    }
                }
            }

            //Don't want to divide by zero
            if (this.Data.OrderSchedule.Count > 0)
                this.PercentLate = Convert.ToInt32((lateCount / (double)this.Data.OrderSchedule.Count) * 100.0) + " %";
            else
                this.PercentLate = "0 %";

            this.TotalOrders = this.Data.OrderSchedule.Count.ToString();

            //Make sure we actually have something to average
            if (averages.Count > 0)
                this.AvgDaysLate = String.Format("{0:0.0}", averages.Average());
            else
                this.AvgDaysLate = "0.0";
        }

        private void CalculateEstimatedShipDate()
        {
            foreach (ScheduleDataset.OrderScheduleRow order in this.Data.OrderSchedule)
            {
                var processes = this.Data.OrderProcesses.Select("OrderID = " + order["OrderID"], "StepOrder DESC");

                if (processes != null && processes.Length > 0)
                {
                    //get last steps end date as the end of the process
                    if(!processes[0].IsNull("EndDate"))
                    {
                        //move current est ship date to last est ship date
                        if(!order.IsEstShipDateNull())
                            order[LASTESTSHIPDATE_COLUMN] = order.EstShipDate;
                        order.EstShipDate = (DateTime)processes[0]["EndDate"];
                    }
                    else
                        _log.Info("Order est ship date not updated as END Date was null : " + order.OrderID);
                }
                else
                    _log.Info("Order est ship date not updated due to no processes: " + order.OrderID);
            }
        }

        private void LogUnprocessedOrders()
        {
            DataRow[] unprocessed = this.Data.OrderProcesses.Select("EndDate IS NULL");

            foreach (DataRow item in unprocessed)
                _log.Info("Unprocessed order: " + item["OrderID"]);
        }

        private void RemoveOrdersNotToBeScheduled()
        {
            //get all orders that
            //  - Are not normal priority
            //  - do not have processes to be completed (i.e. nothing to schedule)
            var ordersToRemove = Data.OrderSchedule
                .Where(order => (!order.IsPriorityNull() && order.Priority != "Normal") || order.GetOrderProcessesRows().Count(opr => opr.IsEndDateNull()) == 0)
                .ToList();

            foreach (var or in ordersToRemove)
            {
                foreach (var op in or.GetOrderProcessesRows())
                {
                    Data.OrderProcesses.RemoveOrderProcessesRow(op);
                }

                Data.OrderSchedule.RemoveOrderScheduleRow(or);
            }
        }

        #endregion
    }
}