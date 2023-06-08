using System;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderHistoryDataSetTableAdapters;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using NLog;

namespace DWOS.Data.Order.Activity
{
    /// <summary>
    /// An inspection activity that occurs on an order for a specific inspection.
    /// </summary>
    public class ControlInspectionActivity : OrderActivity
    {
        #region Fields

        private bool _initialized;
        private string _orderStatus;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the order process ID for this instance.
        /// </summary>
        /// <value>
        /// Order process ID if this instance has been initialized and there
        /// was a previous process; otherwise, <c>null</c>
        /// </value>
        public int? OrderProcessID { get; set; }
        public ReworkType? SelectedReworkType { get; set; }

        /// <summary>
        /// Gets or sets the process ID for this instance.
        /// </summary>
        /// <value>
        /// Process ID if this instance has been initialized and there was a
        /// previous process; otherwise, <c>null</c>
        /// </value>
        public int? ProcessID { get; set; }

        /// <summary>
        /// Gets or sets the process alias ID for this instance.
        /// </summary>
        /// <value>
        /// Process alias ID if this instance has been initialized and there
        /// was a previous process; otherwise, <c>null</c>
        /// </value>
        public int? ProcessAliasID { get; set; }

        /// <summary>
        /// Gets or sets the count of remaining inspections for this instance.
        /// </summary>
        public int RemainingInspectionCount { get; set; }

        /// <summary>
        /// Gets or sets the part inspection type ID for this instance.
        /// </summary>
        /// <value>
        /// The part inspection type ID if this instance has been fully
        /// initialized; otherwise, <c>null</c>
        /// </value>
        public int? PartInspectionTypeID { get; set; }

        /// <summary>
        /// Gets or sets the part inspection ID for this instance.
        /// </summary>
        /// <value>
        /// The part type ID if this instance uses an existing part
        /// inspection; otherwise, <c>0</c>
        /// </value>
        public int PartInspectionID { get; set; }

        /// <summary>
        /// Gets or sets the part ID for this instance.
        /// </summary>
        public int PartID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the inspection passed or failed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the inspection passed; otherwise, <c>false</c>.
        /// </value>
        public bool PassedInspection { get; set; }

        /// <summary>
        /// Gets or sets the part quantity for this instance.
        /// </summary>
        public int PartQuantity { get; set; }

        /// <summary>
        /// Gets or sets the failure quantity for this instance.
        /// </summary>
        public int FailQuantity { get; set; }

        /// <summary>
        /// Gets or sets the inspecting user's ID for this instance.
        /// </summary>
        private int UserId { get; set; }

        /// <summary>
        /// Gets a value indicating if this instance can be completed without
        /// user input.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can be automatically completed;
        /// otherwise, <c>false</c>
        /// </value>
        public bool CanAutoComplete
        {
            get
            {
                if (!_initialized || !PartInspectionTypeID.HasValue)
                {
                    return false;
                }

                using (var dtQuestion = new PartInspectionDataSet.PartInspectionQuestionDataTable())
                {
                    using (var ta = new Datasets.PartInspectionDataSetTableAdapters.PartInspectionQuestionTableAdapter())
                    {
                        ta.FillBy(dtQuestion, PartInspectionTypeID.Value);
                    }

                    return dtQuestion.Count == 0 ||
                        dtQuestion.All(row => !row.IsDefaultValueNull() && !string.IsNullOrEmpty(row.DefaultValue));
                }
            }
        }

        public bool IsValid => PartInspectionTypeID > 0 && HasCorrectOrderStatus;

        public bool HasCorrectOrderStatus => _orderStatus == ApplicationSettings.Current.WorkStatusPendingQI;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ControlInspectionActivity"/> class.
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="userId"></param>
        public ControlInspectionActivity(int orderID, int userId) : base(orderID) { this.UserId = userId; }

        public override void Initialize()
        {
            if (this._initialized)
            {
                return;
            }

            //get order and part info
            using(var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
            {
                var orderSummaries = new Data.Datasets.OrderProcessingDataSet.OrderSummaryDataTable();
                ta.FillById(orderSummaries, this.OrderID);

                var orderSummary = orderSummaries.FindByOrderID(this.OrderID);

                if(orderSummary != null)
                {
                    _orderStatus = orderSummary.WorkStatus;
                    this.PartID = orderSummary.PartID;
                    this.PartQuantity = orderSummary.IsPartQuantityNull() ? 0 : orderSummary.PartQuantity;
                }
                else
                    return;
            }

            //get order process
            using (var tOP = new OrderProcessesTableAdapter())
            {
                var orderProcesses = new OrderProcessingDataSet.OrderProcessesDataTable();
                tOP.FillBy(orderProcesses, OrderID);

                //find last compelted process
                var op = orderProcesses.OrderBy(ob => ob.StepOrder).LastOrDefault(w => !w.IsEndDateNull());

                if(op != null)
                {
                    this.OrderProcessID = op.OrderProcessesID;
                    this.ProcessID = op.ProcessID;
                    this.ProcessAliasID = op.ProcessAliasID;
                }
                else
                    return;
            }

            //get all inspections already completed for this order process id
            var partInspections = new PartInspectionDataSet.PartInspectionDataTable();

            using (var taPartInspections = new Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionTableAdapter())
                taPartInspections.FillByOrderProcessID(partInspections, this.OrderProcessID);

            //get all process inspections for current process
            using (var ta = new ProcessInspectionsTableAdapter())
            {
                var processInspections = new OrderProcessingDataSet.ProcessInspectionsDataTable();
                ta.FillByProcess(processInspections, this.ProcessID.Value);
                
                this.RemainingInspectionCount = processInspections.Count;

                foreach (var processInspection in processInspections)
                {
                    this.RemainingInspectionCount--; //keep track of remaining inspections

                    //see if there is an existing inspection not completed
                    var inspection = partInspections.FirstOrDefault(pi => pi.PartInspectionTypeID == processInspection.PartInspectionTypeID);

                    if (inspection == null || inspection.IsStatusNull() || !inspection.Status)
                    {
                        this.PartInspectionTypeID = processInspection.PartInspectionTypeID;

                        if(inspection != null)
                            this.PartInspectionID = inspection.PartInspectionID;

                        break; //break to stop counting remaining inspections
                    }
                }
            }

            this._initialized = true;
        }

        public override ActivityResults Complete()
        {
            var results = new ControlInspectionActivityResults
            {
                OrderID = OrderID,
                RemainingInspections = RemainingInspectionCount,
                ReworkOriginalOrder = !PassedInspection && FailQuantity >= PartQuantity
            };

            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    OrderProcessesTableAdapter taOrderProcesses = null;
                    OrderSummaryTableAdapter taOrder = null;
                    OrderHistoryTableAdapter taOrderHistory = null;

                    try
                    {
                        taOrderProcesses = new OrderProcessesTableAdapter
                        {
                            Connection = conn,
                            Transaction = transaction
                        };

                        taOrder = new OrderSummaryTableAdapter
                        {
                            Connection = conn,
                            Transaction = transaction
                        };

                        taOrderHistory = new OrderHistoryTableAdapter
                        {
                            Connection = conn,
                            Transaction = transaction
                        };


                        using (var processes = new OrderProcessingDataSet.OrderProcessesDataTable())
                        {
                            taOrderProcesses.FillBy(processes, OrderID);

                            results.CompletedProcesses  = processes.Count(op => !op.IsEndDateNull());
                            results.TotalProcesses      = processes.Count;
                        }

                        if(results.RemainingInspections < 1 || results.ReworkOriginalOrder)
                        {
                            results.WorkStatus =  ApplicationSettings.Current.WorkStatusChangingDepartment;
                            var orderSummaryTable = new OrderProcessingDataSet.OrderSummaryDataTable();
                            taOrder.FillById(orderSummaryTable, OrderID);

                            var orderSummary = orderSummaryTable.FirstOrDefault();

                            if (orderSummary != null)
                            {
                                if (results.ReworkOriginalOrder)
                                {
                                    if (SelectedReworkType.HasValue)
                                    {
                                        results.WorkStatus = SelectedReworkType == ReworkType.Quarantine
                                            ? OrderUtilities.WorkStatusAfterQuarantine(orderSummary.RequireCoc)
                                            : ApplicationSettings.Current.WorkStatusPendingReworkPlanning;
                                    }
                                    else
                                    {
                                        results.WorkStatus = ApplicationSettings.Current.WorkStatusPendingReworkAssessment;
                                    }
                                }
                                else
                                {
                                    //if has all processes completed
                                    if(results.CompletedProcesses == results.TotalProcesses)
                                    {
                                        var orderType = (OrderType)orderSummary.OrderType;
                                        var isPartMarking = Convert.ToBoolean(taOrder.IsPartMarking(OrderID));

                                        results.WorkStatus = OrderUtilities.WorkStatusAfterProcessing(orderType, isPartMarking, orderSummary.RequireCoc);

                                        taOrder.ResetSchedulePriority(OrderID);
                                    }
                                }
                            }
                            else
                            {
                                LogManager.GetCurrentClassLogger().Error($"Could not find order row for order {OrderID}");
                            }
                        }
                        else
                            results.WorkStatus =  ApplicationSettings.Current.WorkStatusPendingQI;

                        //If Moving to the Shipping work status then auto move to shipping department
                        if (results.WorkStatus ==  ApplicationSettings.Current.WorkStatusShipping)
                        {
                            taOrder.UpdateOrderLocation(ApplicationSettings.Current.DepartmentShipping, OrderID);
                        }

                        if (!String.IsNullOrEmpty(results.WorkStatus))
                        {
                            //update order status
                            taOrderHistory.UpdateOrderHistory(this.OrderID,
                                "Control Inspection",
                                $"Inspection '{PartInspectionTypeID}' performed with {this.FailQuantity} rejected and work status set to '{results.WorkStatus}'.",
                                GetUserName(UserId));

                            taOrder.UpdateWorkStatus(results.WorkStatus, OrderID);
                        }

                        if (results.RemainingInspections == 0)
                        {
                            new OrderTimerHelper(conn, transaction).StopAllOrderTimers(OrderID);
                        }

                        transaction.Commit();

                        // Auto check-in
                        if (!ApplicationSettings.Current.OrderCheckInEnabled && results.WorkStatus == ApplicationSettings.Current.WorkStatusChangingDepartment)
                        {
                            var checkin = new OrderCheckInController(OrderID);
                            var checkinResult = checkin.AutoCheckIn(UserId);

                            if (!checkinResult.Response)
                            {
                                LogManager.GetCurrentClassLogger().Warn($"Auto check-in failed for order {OrderID}.");
                            }
                        }

                        return results;
                    }
                    finally
                    {
                        taOrder?.Dispose();
                        taOrderProcesses?.Dispose();
                        taOrderHistory?.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Completes all inspection questions and the inspection.
        /// </summary>
        /// <returns></returns>
        public ControlInspectionActivityResults AutoCompleteInspection()
        {
            if (!PartInspectionTypeID.HasValue)
            {
                var errorMsg = string.Format("{0} must have a value before auto completion.",
                    nameof(PartInspectionTypeID));

                throw new InvalidOperationException(errorMsg);
            }

            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    PartInspectionDataSet dsInspection = null;
                    Datasets.PartInspectionDataSetTableAdapters.PartInspectionTableAdapter taPartInspection = null;
                    Datasets.PartInspectionDataSetTableAdapters.PartInspectionQuestionTableAdapter taQuestion = null;
                    Datasets.PartInspectionDataSetTableAdapters.PartInspectionAnswerTableAdapter taAnswer = null;

                    try
                    {
                        dsInspection = new PartInspectionDataSet()
                        {
                            EnforceConstraints = false
                        };

                        taPartInspection = new Datasets.PartInspectionDataSetTableAdapters.PartInspectionTableAdapter
                        {
                            Connection = conn,
                            Transaction = transaction
                        };

                        taQuestion = new Datasets.PartInspectionDataSetTableAdapters.PartInspectionQuestionTableAdapter
                        {
                            Connection = conn,
                            Transaction = transaction
                        };

                        taAnswer = new Datasets.PartInspectionDataSetTableAdapters.PartInspectionAnswerTableAdapter
                        {
                            Connection = conn,
                            Transaction = transaction
                        };

                        taPartInspection.FillByInspection(dsInspection.PartInspection, PartInspectionID);

                        var inspection = dsInspection.PartInspection.FirstOrDefault();

                        if (inspection == null)
                        {
                            // Add new PartInspection
                            inspection = dsInspection.PartInspection.NewPartInspectionRow();
                            inspection.OrderID = OrderID;
                            inspection.PartQuantity = PartQuantity;
                            inspection.PartInspectionTypeID = PartInspectionTypeID.Value;

                            if (OrderProcessID.HasValue)
                            {
                                inspection.OrderProcessID = OrderProcessID.Value;
                            }

                            dsInspection.PartInspection.AddPartInspectionRow(inspection);
                        }

                        inspection.QAUserID = UserId;
                        inspection.AcceptedQty = PartQuantity;
                        inspection.RejectedQty = 0;
                        inspection.Status = true;
                        inspection.InspectionDate = DateTime.Now;

                        taQuestion.FillBy(dsInspection.PartInspectionQuestion, PartInspectionTypeID.Value);

                        if (PartInspectionID > 0)
                        {
                            taAnswer.FillByPartInspection(dsInspection.PartInspectionAnswer, PartInspectionID);
                        }

                        foreach (var question in dsInspection.PartInspectionQuestion)
                        {
                            var answer = dsInspection.PartInspectionAnswer
                                .FirstOrDefault(pia => pia.PartInspectionQuestionID == question.PartInspectionQuestionID);

                            if (answer == null)
                            {
                                // Add new answer
                                answer = dsInspection.PartInspectionAnswer.NewPartInspectionAnswerRow();
                                answer.PartInspectionQuestionRow = question;
                                answer.PartInspectionRow = inspection;
                                dsInspection.PartInspectionAnswer.AddPartInspectionAnswerRow(answer);
                            }

                            if (question.IsDefaultValueNull())
                            {
                                answer.SetAnswerNull();
                            }
                            else
                            {
                                answer.Answer = question.DefaultValue.Trim();
                            }

                            answer.Completed = true;
                            answer.CompletedBy = UserId;
                            answer.CompletedData = DateTime.Now;
                        }

                        taPartInspection.Update(dsInspection.PartInspection);
                        taAnswer.Update(dsInspection.PartInspectionAnswer);

                        new OrderTimerHelper(conn, transaction).StopAllOrderTimers(OrderID);

                        PartInspectionID = inspection.PartInspectionID;

                        transaction.Commit();
                        return Complete() as ControlInspectionActivityResults;
                    }
                    finally
                    {
                        dsInspection?.Dispose();
                        taPartInspection?.Dispose();
                        taQuestion?.Dispose();
                        taAnswer?.Dispose();
                    }
                }
            }
        }

        public CancellationResults CancelInvalidInspection()
        {
            if (IsValid)
            {
                return null;
            }

            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    OrderSummaryTableAdapter taOrderSummary = null;
                    OrderHistoryTableAdapter taOrderHistory = null;
                    OrderProcessesTableAdapter taOrderProcesses = null;

                    try
                    {
                        taOrderSummary = new OrderSummaryTableAdapter
                        {
                            Connection = conn,
                            Transaction = transaction
                        };

                        taOrderHistory = new OrderHistoryTableAdapter
                        {
                            Connection = conn,
                            Transaction = transaction
                        };

                        taOrderProcesses = new OrderProcessesTableAdapter
                        {
                            Connection = conn,
                            Transaction = transaction
                        };

                        var orderSummary = taOrderSummary.GetDataById(OrderID)?.FirstOrDefault();

                        var workStatus = ApplicationSettings.Current.WorkStatusChangingDepartment;

                        bool completedAllProcesses;

                        using (var processes = new OrderProcessingDataSet.OrderProcessesDataTable())
                        {
                            taOrderProcesses.FillBy(processes, OrderID);
                            completedAllProcesses = processes.Count(op => !op.IsEndDateNull()) == processes.Count;
                        }

                        if (completedAllProcesses && orderSummary != null)
                        {
                            var orderType = (OrderType)orderSummary.OrderType;
                            var isPartMarking = Convert.ToBoolean(taOrderSummary.IsPartMarking(OrderID));
                            var requireCoc = taOrderSummary.RequireCoc(OrderID) ?? false;
                            workStatus = OrderUtilities.WorkStatusAfterProcessing(orderType, isPartMarking, requireCoc);

                            taOrderSummary.ResetSchedulePriority(OrderID);
                        }

                        taOrderHistory.UpdateOrderHistory(OrderID,
                            "Control Inspection",
                            $"Unable to determine inspection required for order {OrderID}. Changing order status to \'{workStatus}\'.",
                            GetUserName(UserId));

                        taOrderSummary.UpdateWorkStatus(workStatus, OrderID);

                        transaction.Commit();

                        return new CancellationResults
                        {
                            OrderID = OrderID,
                            WorkStatus = workStatus
                        };
                    }
                    finally
                    {
                        taOrderSummary?.Dispose();
                        taOrderHistory?.Dispose();
                        taOrderProcesses?.Dispose();
                    }
                }
            }
        }

        #endregion

        #region ControlInspectionActivityResults

        /// <summary>
        /// Represents activity results for
        /// <see cref="ControlInspectionActivityResults"/>.
        /// </summary>
        public class ControlInspectionActivityResults : ActivityResults
        {
            /// <summary>
            /// Gets or sets a value indicating that the order needs to
            /// be reworked.
            /// </summary>
            /// <value>
            /// <c>true</c> if the order needs to be reworked; otherwise,
            /// <c>false</c>
            /// </value>
            public bool ReworkOriginalOrder { get; set; }

            /// <summary>
            /// Gets or sets the number of completed processes for
            /// the order.
            /// </summary>
            public int CompletedProcesses { get; set; }

            /// <summary>
            /// Gets or sets the number of processes for the order.
            /// </summary>
            public int TotalProcesses { get; set; }

            /// <summary>
            /// Gets or sets the number of remaining inspections for this
            /// order and process.
            /// </summary>
            public int RemainingInspections { get; set; }

            public override string ToString() =>
                $"Order {OrderID}; Rework Original Order {ReworkOriginalOrder}; Remaining Inspections {RemainingInspections}; Completed Processes {CompletedProcesses}";
        }

        #endregion

        #region CancelResults

        public class CancellationResults : ActivityResults
        {
        }

        #endregion
    }
}
