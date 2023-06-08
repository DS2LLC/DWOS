using DWOS.Data;
using DWOS.Data.Reports;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.Reports.ReportData
{
    /// <summary>
    /// Encapsulates data functionality for <see cref="EmployeeProcessingReport"/>.
    /// </summary>
    internal class EmployeeProcessingData
    {
        #region Fields

        private static readonly ILogger _log =
            LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of users for this instance.
        /// </summary>
        public ICollection<User> Users { get; private set; }

        public ICollection<Department> Departments { get; private set; }

        /// <summary>
        /// Gets or sets a value that indicates if the report should include product class.
        /// </summary>
        /// <value>
        /// <c>true</c> if showing product class; otherwise, <c>false</c>.
        /// </value>
        public bool ShowProductClass { get; private set; }

        /// <summary>
        /// Gets or sets a value that indicates if the report should include
        /// revenue (processing amount).
        /// </summary>
        /// <value>
        /// <c>true</c> if showing revenue; otherwise, <c>false</c>.
        /// </value>
        public bool ShowRevenue { get; private set; }

        #endregion

        /// <summary>
        /// Creates a new <see cref="EmployeeProcessingData"/> instance from a given date range.
        /// </summary>
        /// <param name="fromDate">
        /// The start date
        /// </param>
        /// <param name="toDate">
        /// The ending date
        /// </param>
        /// <param name="settings">
        /// Application-wide settings
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token to watch.
        /// </param>
        /// <returns></returns>
        public static async Task<EmployeeProcessingData> GetReportDataAsync(
            DateTime fromDate, DateTime toDate,
            ApplicationSettings settings, CancellationToken cancellationToken)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            var useProductClass = FieldUtilities.IsFieldEnabled("Order", "Product Class");

            var useRevenue = settings.PartPricingType == PricingType.Process;

            var userDict = new Dictionary<int, User>();
            var departmentDict = new Dictionary<string, Department>();

            try
            {
                using (var dataAccess = new EmployeeProcessingPerformance(fromDate, toDate))
                {
                    var orderProductClassDict = useProductClass
                        ? dataAccess.RetrieveProductClasses()
                        : new Dictionary<int, string>();

                    var orderAmountDict = useRevenue
                        ? (await dataAccess.RetrieveOrderAmounts(cancellationToken)).ToDictionary(amt => amt.OrderId)
                        : new Dictionary<int, EmployeeProcessingPerformance.OrderAmount>();

                    // Processing - User
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _log.Info("Canceled before employee processing step.");
                    }
                    else
                    {
                        foreach (var processingRow in await dataAccess.RetrieveEmployeeProcessedAsync(cancellationToken))
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                _log.Info("Canceled during employee processing step.");
                                break;
                            }

                            var userId = processingRow.UserId;

                            // Retrieve/add user
                            if (!userDict.TryGetValue(userId, out var user))
                            {
                                user = NewUser(userId);
                                userDict[userId] = user;
                            }

                            // Retrieve product class
                            orderProductClassDict.TryGetValue(processingRow.OrderId, out var productClass);

                            // Retrieve order amount
                            orderAmountDict.TryGetValue(processingRow.OrderId, out var orderAmountInfo);

                            // Add order action
                            user.OrderActions.Add(new OrderAction
                            {
                                OrderId = processingRow.OrderId,
                                ProductClass = productClass,
                                ActionType = OrderActionType.Processed,
                                Department = processingRow.Department,
                                ProcessName = processingRow.ProcessName,
                                ProcessRevenue = orderAmountInfo?.CalculateProcessTotal(processingRow.ProcessAmount),
                                ActionDate = processingRow.EndDate
                            });
                        }
                    }

                    // Processing - Department
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _log.Info("Canceled before department processing step.");
                    }
                    else
                    {
                        foreach (var processingRow in await dataAccess.RetrieveDepartmentProcessedAsync(cancellationToken))
                        {
                            if (!processingRow.EndDate.HasValue)
                            {
                                // Do not include processes that haven't ended;
                                // should not typically happen.
                                continue;
                            }

                            var departmentName = processingRow.Department;

                            if (!departmentDict.TryGetValue(departmentName, out var department))
                            {
                                department = NewDepartment(departmentName);
                                departmentDict[departmentName] = department;
                            }

                            // Retrieve product class
                            orderProductClassDict.TryGetValue(processingRow.OrderId, out var productClass);

                            // Retrieve order amount
                            orderAmountDict.TryGetValue(processingRow.OrderId, out var orderAmountInfo);

                            // Add order action
                            department.OrderActions.Add(new OrderAction
                            {
                                OrderId = processingRow.OrderId,
                                ProductClass = productClass,
                                ActionType = OrderActionType.Processed,
                                Department = processingRow.Department,
                                ProcessName = processingRow.ProcessName,
                                ProcessRevenue = orderAmountInfo?.CalculateProcessTotal(processingRow.ProcessAmount),
                                ActionDate = processingRow.EndDate.Value
                            });
                        }
                    }

                    // QA department
                    if (!departmentDict.TryGetValue(settings.DepartmentQA, out var qaDepartment))
                    {
                        qaDepartment = NewDepartment(settings.DepartmentQA);
                        departmentDict[settings.DepartmentQA] = qaDepartment;
                    }

                    // Part Inspections
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _log.Info("Canceled before inspection step.");
                    }
                    else
                    {
                        foreach (var inspectionRow in await dataAccess.RetrieveInspectedAsync(cancellationToken))
                        {
                            var userId = inspectionRow.UserId;

                            // Retrieve/add user
                            if (!userDict.TryGetValue(userId, out var user))
                            {
                                user = NewUser(userId);
                                userDict[userId] = user;
                            }

                            // Retrieve product class
                            orderProductClassDict.TryGetValue(inspectionRow.OrderId, out var productClass);

                            // Add order action
                            OrderAction inspectionAction = new OrderAction
                            {
                                OrderId = inspectionRow.OrderId,
                                ProductClass = productClass,
                                ActionType = OrderActionType.PartInspection,
                                Department = settings.DepartmentQA,
                                ActionDate = inspectionRow.InspectionDate
                            };

                            user.OrderActions.Add(inspectionAction);
                            qaDepartment.OrderActions.Add(inspectionAction);
                        }
                    }

                    //Final Inspection
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _log.Info("Canceled before final inspection step.");
                    }
                    else
                    {
                        foreach (var cocRow in await dataAccess.RetrieveFinalInspectedAsync(cancellationToken))
                        {
                            var userId = cocRow.UserId;

                            // Retrieve/add user
                            if (!userDict.TryGetValue(userId, out var user))
                            {
                                user = NewUser(userId);
                                userDict[userId] = user;
                            }

                            // Retrieve product class
                            orderProductClassDict.TryGetValue(cocRow.OrderId, out var productClass);

                            // Add order action
                            OrderAction cocAction = new OrderAction
                            {
                                OrderId = cocRow.OrderId,
                                ProductClass = productClass,
                                ActionType = OrderActionType.FinalInspection,
                                Department = settings.DepartmentQA,
                                ActionDate = cocRow.DateCertified
                            };

                            user.OrderActions.Add(cocAction);
                            qaDepartment.OrderActions.Add(cocAction);
                        }
                    }

                    // Shipping department
                    if (!departmentDict.TryGetValue(settings.DepartmentShipping, out var shippingDepartment))
                    {
                        shippingDepartment = NewDepartment(settings.DepartmentShipping);
                        departmentDict[settings.DepartmentShipping] = shippingDepartment;
                    }

                    // Shipping
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _log.Info("Canceled before shipping step.");
                    }
                    else
                    {
                        foreach (var shippedRow in await dataAccess.RetrieveShippedAsync(cancellationToken))
                        {
                            var userId = shippedRow.UserId;

                            // Retrieve/add user
                            if (!userDict.TryGetValue(userId, out var user))
                            {
                                user = NewUser(userId);
                                userDict[userId] = user;
                            }

                            // Retrieve product class
                            orderProductClassDict.TryGetValue(shippedRow.OrderId, out var productClass);

                            // Add order action
                            OrderAction shippingAction = new OrderAction
                            {
                                OrderId = shippedRow.OrderId,
                                ProductClass = productClass,
                                ActionType = OrderActionType.Shipped,
                                Department = settings.DepartmentShipping,
                                ActionDate = shippedRow.DateShipped
                            };

                            user.OrderActions.Add(shippingAction);
                            shippingDepartment.OrderActions.Add(shippingAction);
                        }
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                _log.Warn(ex, "Canceled employee processing report.");
            }

            if (cancellationToken.IsCancellationRequested)
            {
                _log.Info("Report was canceled.");
                return null;
            }
            else
            {
                return new EmployeeProcessingData
                {
                    Users = userDict.Values,
                    Departments = departmentDict.Values,
                    ShowProductClass = useProductClass,
                    ShowRevenue = useRevenue
                };
            }
        }

        private static User NewUser(int userId)
        {
            string userName;
            using (var taUser = new Data.Datasets.SecurityDataSetTableAdapters.UsersTableAdapter())
            {
                userName = taUser.GetUserName(userId);
            }

            return new User
            {
                UserId = userId,
                Name = userName,
                OrderActions = new List<OrderAction>()
            };
        }

        private static Department NewDepartment(string departmentName)
        {
            return new Department
            {
                Name = departmentName,
                OrderActions = new List<OrderAction>()
            };
        }

        #region User

        public class User
        {
            /// <summary>
            /// Gets or sets the ID for this instance.
            /// </summary>
            public int UserId { get; set; }

            /// <summary>
            /// Gets or sets the name for this instance.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the order actions for this instance.
            /// </summary>
            public ICollection<OrderAction> OrderActions { get; set; }

            /// <summary>
            /// Gets the number of orders that this user processed.
            /// </summary>
            public long OrdersProcessed => OrderActions
                .Where(o => o.ActionType == OrderActionType.Processed)
                .Select(o => o.OrderId)
                .Distinct()
                .LongCount();

            /// <summary>
            /// Gets the number of orders that this user inspected.
            /// </summary>
            /// <remarks>
            /// If a user performed multiple inspections for an order, this count
            /// only includes them once.
            /// </remarks>
            public long OrdersInspected => OrderActions
                .Where(o => o.ActionType == OrderActionType.PartInspection)
                .Select(o => o.OrderId)
                .Distinct()
                .LongCount();

            /// <summary>
            /// Gets the number of orders that this user put through final inspection.
            /// </summary>
            /// <remarks>
            /// This count includes each order once, even when the user revises a COC.
            /// </remarks>
            public long OrdersFinalInspected => OrderActions
                .Where(o => o.ActionType == OrderActionType.FinalInspection)
                .Select(o => o.OrderId)
                .Distinct()
                .LongCount();

            /// <summary>
            /// Gets the number of orders that this user shipped.
            /// </summary>
            public long OrdersShipped => OrderActions
                .Where(o => o.ActionType == OrderActionType.Shipped)
                .Select(o => o.OrderId)
                .Distinct()
                .LongCount();
        }

        public class Department
        {
            public string Name { get; set; }

            public ICollection<OrderAction> OrderActions { get; set; }

            /// <summary>
            /// Gets the number of orders that this department processed.
            /// </summary>
            public long OrdersProcessed => OrderActions
                .Where(o => o.ActionType == OrderActionType.Processed)
                .Select(o => o.OrderId)
                .Distinct()
                .LongCount();

            /// <summary>
            /// Gets the number of orders that this user inspected.
            /// </summary>
            /// <remarks>
            /// If a user performed multiple inspections for an order, this count
            /// only includes them once.
            /// </remarks>
            public long OrdersInspected => OrderActions
                .Where(o => o.ActionType == OrderActionType.PartInspection)
                .Select(o => o.OrderId)
                .Distinct()
                .LongCount();

            /// <summary>
            /// Gets the number of processes that this department completed.
            /// </summary>
            public long ProcessesCompleted => OrderActions
                .Where(o => o.ActionType == OrderActionType.Processed)
                .LongCount();

            /// <summary>
            /// Gets the number of orders that this department put through final inspection.
            /// </summary>
            /// <remarks>
            /// This count includes each order once, even when a user revises a COC.
            /// </remarks>
            public long OrdersFinalInspected => OrderActions
                .Where(o => o.ActionType == OrderActionType.FinalInspection)
                .Select(o => o.OrderId)
                .Distinct()
                .LongCount();

            /// <summary>
            /// Gets the number of orders that this department shipped.
            /// </summary>
            public long OrdersShipped => OrderActions
                .Where(o => o.ActionType == OrderActionType.Shipped)
                .Select(o => o.OrderId)
                .Distinct()
                .LongCount();
        }

        #endregion

        #region OrderAction

        /// <summary>
        /// Represents an action taken on an order.
        /// </summary>
        public class OrderAction
        {
            #region Properties

            /// <summary>
            /// Gets the action type for this instance.
            /// </summary>
            public OrderActionType ActionType { get; set; }

            /// <summary>
            /// Gets the action date for this instance.
            /// </summary>
            public DateTime ActionDate { get; set; }

            /// <summary>
            /// Gets the order ID for this instance.
            /// </summary>
            public int OrderId { get; set; }

            /// <summary>
            /// Gets the process name for this instance.
            /// </summary>
            public string ProcessName { get; set; }

            /// <summary>
            /// Gets or sets the process revenue for this instance.
            /// </summary>
            public decimal? ProcessRevenue { get; internal set; }

            /// <summary>
            /// Gets the product class for this instance.
            /// </summary>
            public string ProductClass { get; set; }

            /// <summary>
            /// Gets or sets the department for this instance.
            /// </summary>
            public string Department { get; set; }

            #endregion
        }

        #endregion

        #region OrderAction

        /// <summary>
        /// Represents an action type.
        /// </summary>
        public enum OrderActionType
        {
            /// <summary>
            /// Order processing
            /// </summary>
            Processed,

            /// <summary>
            /// Order/part inspection
            /// </summary>
            /// <remarks>
            /// Can represent a passed or failed review.
            /// </remarks>
            PartInspection,

            /// <summary>
            /// Order Final Inspection
            /// </summary>
            FinalInspection,

            /// <summary>
            /// Order shipped
            /// </summary>
            Shipped
        }

        #endregion

        #region DepartmentSummary

        public class DepartmentSummary
        {
            /// <summary>
            /// Gets or sets the department name for this instance.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets a collection of order processes for this instance.
            /// </summary>
            public ISet<int> OrderProcesses { get; set; }

            /// <summary>
            /// Gets the number of processes that this department completed.
            /// </summary>
            public int ProcessesCompleted => OrderProcesses.Count;
        }

        #endregion
    }
}
