using System;
using System.Data.SqlClient;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderHistoryDataSetTableAdapters;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using NLog;

namespace DWOS.Data.Order.Activity
{
    /// <summary>
    /// A processing activity that occurs on an order for a specific process.
    /// </summary>
    public class ProcessingActivity : OrderActivity
    {
        #region Fields

        private string _currentDepartment;
        private int _userId;
        private int? _currentLineId;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the order process ID for this instance.
        /// </summary>
        /// <value>
        /// The order process ID if this instance has been initialized;
        /// otherwise, <c>null</c>.
        /// </value>
        public int? OrderProcessID { get; set; }

        /// <summary>
        /// Gets or sets the process ID for this instance.
        /// </summary>
        /// <value>
        /// The process ID if this instance has been initialized;
        /// otherwise, <c>null</c>.
        /// </value>
        public int? ProcessID { get; set; }

        /// <summary>
        /// Gets or sets the number parts currently being processed.
        /// </summary>
        /// <value>The processed part count.</value>
        public int CurrentProcessedPartQty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if this activity should not
        /// automatically update process costs on completion.
        /// </summary>
        public bool SkipCostUpdate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if this activity should not
        /// automatically check in an order if manual check-in is disabled.
        /// </summary>
        public bool SkipAutoCheckIn { get; set; }

        /// <summary>
        /// Gets or sets the ID of the batch being processed.
        /// </summary>
        /// <value>
        /// <c>null</c> if this instance does not represent a batch process activity.
        /// </value>
        public int? BatchId { get; set; }

        #endregion

        #region Methods

        public ProcessingActivity(int orderID, ActivityUser user)
            : base(orderID)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _currentDepartment = user.CurrentDepartment;
            _userId = user.UserId;
            _currentLineId = user.CurrentLineId;
        }
        
        /// <summary>
        /// Sets <see cref="OrderProcessID"/> and <see cref="ProcessID"/>
        /// if they are not already set.
        /// </summary>
        public override void Initialize()
        {
            if (!OrderProcessID.HasValue)
            {
                FindOrderProcess();
            }

            if (OrderProcessID.HasValue && !ProcessID.HasValue)
            {
                FindProcess();
            }
        }

        public override ActivityResults Complete()
        {
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    OrderHistoryTableAdapter taOrderHistory = null;
                    OrderSummaryTableAdapter taOrder = null;
                    BatchTableAdapter taBatch = null;

                    try
                    {
                        taOrderHistory = new OrderHistoryTableAdapter
                        {
                            Connection = conn,
                            Transaction = transaction
                        };

                        taOrder = new OrderSummaryTableAdapter
                        {
                            Connection = conn,
                            Transaction = transaction
                        };

                        taBatch = new BatchTableAdapter
                        {
                            Connection = conn,
                            Transaction = transaction
                        };

                        var results = GetResults(conn, transaction);

                        if (results.ProcessDepartment != this._currentDepartment)
                        {
                            taOrderHistory.UpdateOrderHistory(OrderID,
                                "Order Processing",
                                $"Order attempted to be processed in incorrect department {_currentDepartment}. Details: {results}",
                                this.GetUserName(this._userId));
                        }
                        else
                        {
                            taOrderHistory.UpdateOrderHistory(OrderID,
                                "OrderProcessing",
                                $"Order processed in department {_currentDepartment}. Details: {results}",
                                this.GetUserName(this._userId));
                        }

                        if(!string.IsNullOrEmpty(results.WorkStatus))
                        {
                            taOrder.UpdateWorkStatus(results.WorkStatus, OrderID);
                        }

                        //If Moving to the Shipping status then auto move to shipping department
                        if (results.WorkStatus ==  ApplicationSettings.Current.WorkStatusShipping)
                        {
                            taOrder.UpdateOrderLocation(ApplicationSettings.Current.DepartmentShipping, OrderID);
                        }

                        // Reset manually-set department schedule for the order/batch.
                        var resetManualDeptSchedule = ApplicationSettings.Current.SchedulerType == SchedulerType.Manual
                            && results.WorkStatus != ApplicationSettings.Current.WorkStatusInProcess;

                        var resetManualSchedule = ApplicationSettings.Current.SchedulerType == SchedulerType.ManualAllDepartments
                            && results.IsAllProcessCompleted
                            && results.InspectionCount == 0;

                        if (resetManualDeptSchedule || resetManualSchedule)
                        {
                            taOrder.ResetSchedulePriority(OrderID);

                            if (BatchId.HasValue)
                            {
                                taBatch.ResetSchedulePriority(BatchId.Value);
                            }
                        }

                        transaction.Commit();

                        // Auto check-in
                        if (!SkipAutoCheckIn && !ApplicationSettings.Current.OrderCheckInEnabled && results.WorkStatus == ApplicationSettings.Current.WorkStatusChangingDepartment)
                        {
                            var checkin = new OrderCheckInController(OrderID);
                            var checkinResult = checkin.AutoCheckIn(_userId);

                            if (!checkinResult.Response)
                            {
                                LogManager.GetCurrentClassLogger().Warn($"Auto check-in failed for order {OrderID}.");
                            }
                        }

                        return results;
                    }
                    finally
                    {
                        taOrderHistory?.Dispose();
                        taOrder?.Dispose();
                        taBatch?.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Populates this instance using the current process for the order.
        /// </summary>
        private void FindOrderProcess()
        {
            using(var tOP = new OrderProcessesTableAdapter())
            {
                var orderProcesses = new OrderProcessingDataSet.OrderProcessesDataTable();
                tOP.FillCurrentProcess(orderProcesses, OrderID);

                OrderProcessingDataSet.OrderProcessesRow op = orderProcesses.FirstOrDefault();

                if (op != null)
                {
                    OrderProcessID = op.OrderProcessesID;
                }
            }
        }

        /// <summary>
        /// Populates this instance with the process associated with this
        /// instance's order process.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This instance does not have its <see cref="OrderProcessID"/> set.
        /// </exception>
        private void FindProcess()
        {
            if (!OrderProcessID.HasValue)
            {
                var errorMsg = string.Format("Must initialize {0} first.",
                    nameof(OrderProcessID));

                throw new InvalidOperationException(errorMsg);
            }

            using(var tOP = new OrderProcessesTableAdapter())
            {
                var orderProcesses = new OrderProcessingDataSet.OrderProcessesDataTable();
                tOP.FillByID(orderProcesses, OrderProcessID.Value);

                OrderProcessingDataSet.OrderProcessesRow op = orderProcesses.FirstOrDefault();

                if (op != null)
                {
                    ProcessID = op.ProcessID;
                }
            }
        }

        private ProcessingActivityResults GetResults(SqlConnection conn, SqlTransaction transaction)
        {
            OrderProcessesTableAdapter taOrderProcess = null;
            OrderSummaryTableAdapter taOrder = null;

            try
            {
                taOrderProcess = new OrderProcessesTableAdapter
                {
                    Connection = conn,
                    Transaction = transaction
                };

                taOrder = new OrderSummaryTableAdapter
                {
                    Connection = conn,
                    Transaction = transaction
                };

                var results = new ProcessingActivityResults(OrderID, OrderProcessID.GetValueOrDefault(), CurrentProcessedPartQty);

                if(!OrderProcessID.HasValue)
                    return results;

                results.Initialize();

                //if completed all answers, regardless if the process is complete or not (partial loads)
                if(results.TotalAnswers == results.CompletedAnswers)
                {
                    //update total processed count
                    taOrderProcess.UpdateProcessedPartCount(CurrentProcessedPartQty + results.PreviousProcessedPartCount.GetValueOrDefault(), OrderProcessID.Value);
                }

                //if completed all questions OR no order process OR all parts in the order processed via partial loads
                if (results.IsProcessComplete)
                {
                    //Mark process as completed
                    taOrderProcess.UpdateEndDate(DateTime.Now, OrderProcessID.Value);

                    // Update processing line
                    if (ApplicationSettings.Current.MultipleLinesEnabled && _currentLineId.HasValue)
                    {
                        taOrderProcess.UpdateProcessingLine(_currentLineId, OrderProcessID.Value);
                    }

                    //ReQuery to get updated number of processes completed
                    results.CompletedProcesses = taOrderProcess.GetCompletedProcessCount(OrderID).GetValueOrDefault();

                    if (results.InspectionCount > 0) //Do an inspection
                    {
                        results.WorkStatus = ApplicationSettings.Current.WorkStatusPendingQI;
                    }
                    else
                    {
                        if (results.IsAllProcessCompleted) //If done processing
                        {
                            var orderSummaryTable = new OrderProcessingDataSet.OrderSummaryDataTable();
                            taOrder.FillById(orderSummaryTable, OrderID);

                            var orderSummary = orderSummaryTable.FirstOrDefault();

                            var orderType = (OrderType)orderSummary.OrderType;
                            var isPartMarking = taOrder.IsPartMarking(OrderID).ConvertTo(false);

                            results.WorkStatus = OrderUtilities.WorkStatusAfterProcessing(orderType, isPartMarking, orderSummary.RequireCoc);
                        }
                        else
                            results.WorkStatus = ApplicationSettings.Current.WorkStatusChangingDepartment;
                    }

                    new OrderTimerHelper(conn, transaction).CompleteOrderProcessTimers(OrderID);

                    if (!SkipCostUpdate)
                    {
                        try
                        {
                            new ProcessCostHelper(conn, transaction).UpdateCost(OrderProcessID.Value);
                        }
                        catch (Exception exc)
                        {
                            LogManager.GetCurrentClassLogger().Error(exc, "Error updating order cost.");
                        }
                    }
                }
                else if (results.ProcessDepartment != this._currentDepartment)
                {
                    results.WorkStatus = ApplicationSettings.Current.WorkStatusChangingDepartment;
                }
                else
                {
                    results.WorkStatus = ApplicationSettings.Current.WorkStatusInProcess;
                }

                return results;
            }
            finally
            {
                taOrderProcess?.Dispose();
                taOrder?.Dispose();
            }
        }

        public bool CanSkip()
        {
            if (ApplicationSettings.Current.ProcessStrictnessLevel != ProcessStrictnessLevel.AutoComplete)
            {
                return false;
            }

            int answerCount;
            using (var taOrderProcessAnswer = new OrderProcessAnswerTableAdapter())
            {
                answerCount = taOrderProcessAnswer.GetCountByOrderProcessesID(OrderProcessID) ?? 0;
            }

            var canSkip = false;
            if (answerCount == 0)
            {
                using (var taProcessQuestion = new ProcessQuestionTableAdapter())
                {
                    var questionCount = taProcessQuestion.GetCountBy(ProcessID ?? 0) ?? 0;
                    canSkip = questionCount == 0;
                }
            }

            return canSkip;
        }

        public void SkipActivity()
        {
            var partQty = 0;

            using (var taOrderSummary = new OrderSummaryTableAdapter())
            {
                using (var dtOrderSummary = taOrderSummary.GetDataById(OrderID))
                {
                    var order = dtOrderSummary.FirstOrDefault();

                    if (order != null && !order.IsPartQuantityNull())
                    {
                        partQty = order.PartQuantity;
                    }
                }
            }

            CurrentProcessedPartQty = partQty;
            Complete();
        }

        #endregion

        #region ProcessingActivityResults

        /// <summary>
        /// Represents activity results for <see cref="ProcessingActivity"/>.
        /// </summary>
        public class ProcessingActivityResults : ActivityResults
        {
            #region Properties

            /// <summary>
            /// Gets the order process ID for this instance.
            /// </summary>
            public int OrderProcessId { get; private set; }

            /// <summary>
            /// Gets the completed answers count for this instance.
            /// </summary>
            public int CompletedAnswers { get; private set; }

            /// <summary>
            /// Gets the total answers count for this instance.
            /// </summary>
            public int TotalAnswers { get; private set; }

            /// <summary>
            /// Gets a value indicating if processing is complete for the
            /// current process.
            /// </summary>
            /// <value>
            /// <c>true</c> if processing is complete; otherwise, <c>false</c>.
            /// </value>
            public bool IsProcessComplete { get; private set; }

            /// <summary>
            /// Gets the total part count in the order.
            /// </summary>
            /// <value>The total part count.</value>
            public int TotalPartCount { get; private set; }
            
            /// <summary>
            /// Gets the current processed part count during this processing load.
            /// </summary>
            /// <value>The current processed part count.</value>
            public int CurrentProcessedPartCount { get; private set; }
            
            /// <summary>
            /// Gets the previous processed part count during any previous partial loads.
            /// </summary>
            /// <value>
            /// The previous processed part count if found, or <c>null</c> if
            /// there was not a previous partial load
            /// </value>
            public int? PreviousProcessedPartCount { get; private set; }
            
            /// <summary>
            /// Gets or sets a value indicating whether this instance has remainging parts to process.
            /// </summary>
            /// <remarks>
            /// Applicable if using partial loads.
            /// </remarks>
            /// <value><c>true</c> if this instance has remainging parts to process; otherwise, <c>false</c>.</value>
            public bool HasRemaingingPartsToProcess { get; private set; }

            /// <summary>
            /// Gets or sets the number of completed processes.
            /// </summary>
            /// <value>The completed processes.</value>
            public int CompletedProcesses { get; set; }

            /// <summary>
            /// Gets or sets the total number of processes in this order.
            /// </summary>
            /// <value>The total processes.</value>
            public int TotalProcesses { get; set; }

            /// <summary>
            /// Gets if all process completed. Compares completed processes to total processes.
            /// </summary>
            /// <value><c>true</c> if this instance is all process completed; otherwise, <c>false</c>.</value>
            public bool IsAllProcessCompleted { get { return CompletedProcesses == TotalProcesses; } }
            
            /// <summary>
            /// Gets the processes department.
            /// </summary>
            /// <value>The process department.</value>
            public string ProcessDepartment { get; private set; }

            /// <summary>
            /// Gets the inspection count that this process has.
            /// </summary>
            /// <value>The inspection count.</value>
            public int InspectionCount { get; private set; }

            /// <summary>
            /// Gets a value indicating whether this process is paperless.
            /// </summary>
            /// <value><c>true</c> if this instance is paperless; otherwise, <c>false</c>.</value>
            public bool IsPaperless { get; private set; }
            
            /// <summary>
            /// Gets or sets the next requisite process that has a time constraint.
            /// </summary>
            /// <remarks>
            /// This process has to be completed in the specified hours.
            /// </remarks>
            /// <value>The next requisite process unique identifier.</value>
            public int? NextRequisiteProcessID { get; private set; }

            /// <summary>
            /// Gets or sets the next requisite hours.
            /// </summary>
            /// <remarks>
            /// This is the time that the next requisite process has to be
            /// completed in.
            /// </remarks>
            /// <value>The next requisite hours.</value>
            public decimal? NextRequisiteHours { get; private set; }

            #endregion

            #region Methods
            
            /// <summary>
            /// Initializes a new instance of the <see cref="ProcessingActivityResults"/> class.
            /// </summary>
            /// <param name="orderId"></param>
            /// <param name="orderProcessId"></param>
            /// <param name="currentProcessedPartCount"></param>
            public ProcessingActivityResults(int orderId, int orderProcessId, int currentProcessedPartCount)
            {
                this.OrderID        = orderId;
                this.OrderProcessId = orderProcessId;
                this.IsPaperless    = true; //by default is paperless
                this.CurrentProcessedPartCount = currentProcessedPartCount;
            }

            /// <summary>
            /// Initializes this instance and loads all properties based on order and current order process id.
            /// </summary>
            public void Initialize()
            {
                using (var taAnswers = new OrderProcessAnswerTableAdapter())
                {
                    using (var dtAnswers = new OrderProcessingDataSet.OrderProcessAnswerDataTable())
                    {
                        taAnswers.FillByOrderProcessesID(dtAnswers, OrderProcessId);

                        // There could be multiple answers for the same question.
                        // Only consider the first answer for each.
                        var answers = dtAnswers
                            .GroupBy(answer => answer.ProcessQuestionID)
                            .Select(questionGroup => questionGroup.OrderBy(a => a.OrderProcesserAnswerID).FirstOrDefault())
                            .ToList();

                        this.TotalAnswers     = answers.Count;
                        this.CompletedAnswers = answers.Count(opa => opa.Completed);

                        this.IsPaperless = taAnswers.GetIsPaperless(OrderProcessId).ConvertTo(true);

                        if(!IsPaperless) //Paper-Based then all answers are completed
                            this.CompletedAnswers = this.TotalAnswers;

                        this.TotalPartCount   = taAnswers.GetOrderPartCount(this.OrderID).GetValueOrDefault();
                    }
                }

                using (var taProcessInspection = new ProcessInspectionsTableAdapter())
                    this.InspectionCount = taProcessInspection.GetInspectionCountByOrderProcess(OrderProcessId).GetValueOrDefault();

                //determine if all processes where completed
                using (var taOrderProcesses = new OrderProcessesTableAdapter())
                {
                    var orderProcesses = new OrderProcessingDataSet.OrderProcessesDataTable();
                    taOrderProcesses.FillBy(orderProcesses, OrderID);

                    this.CompletedProcesses = orderProcesses.Count(op => !op.IsEndDateNull());
                    this.TotalProcesses     = orderProcesses.Count;

                    //Determine any process pre-requisites
                    var orderProcess = orderProcesses.FirstOrDefault(op => op.OrderProcessesID == OrderProcessId);
                    if (orderProcess != null)
                    {
                        this.ProcessDepartment = orderProcess.Department;
                        
                        if(!orderProcess.IsPartCountNull())
                            this.PreviousProcessedPartCount = orderProcess.PartCount;

                        //get the next process that has this process as a pre-requisite
                        var op = orderProcesses.FirstOrDefault(o => !o.IsRequisiteProcessIDNull() && o.RequisiteProcessID == orderProcess.ProcessID);
                        if (op != null && !op.IsRequisiteHoursNull())
                        {
                            this.NextRequisiteHours = op.RequisiteHours;
                            this.NextRequisiteProcessID = op.ProcessID;
                        }
                    }
                }

                //determine if process has been completed
                this.IsProcessComplete = CompletedAnswers == TotalAnswers;

                //if we are requiring all parts to be processed in an order before we can move to next process
                if (ApplicationSettings.Current.AllowPartialProcessLoads)
                {
                    var toBeProcessed = this.TotalPartCount - (this.CurrentProcessedPartCount + this.PreviousProcessedPartCount.GetValueOrDefault());
                    HasRemaingingPartsToProcess = toBeProcessed > 0;

                    if (HasRemaingingPartsToProcess)
                        this.IsProcessComplete = false;
                }
            }

            public override string ToString()
            {
                return "Order {0}, Work Status {1}, Answers {2} / {3}, Processes {4} / {5}, for Order Process {6}".FormatWith(base.OrderID, WorkStatus, CompletedAnswers, TotalAnswers, CompletedProcesses, TotalProcesses, OrderProcessId);
            }

            #endregion
        }

        #endregion
    }


}
