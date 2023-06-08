using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.SecurityDataSetTableAdapters;
using DWOS.Data.Reports.ProcessPartsReportTableAdapters;
using NLog;

namespace DWOS.Reports.ReportData
{
    /// <summary>
    /// Contains data for <see cref="EmployeeReceivingReport"/>.
    /// </summary>
    public class EmployeeReceivingData
    {
        #region Properties

        /// <summary>
        /// Gets the list of users for this instance.
        /// </summary>
        public List<User> Users { get; private set; }

        #endregion

        #region Methods

        private EmployeeReceivingData()
        {
        }

        /// <summary>
        /// Gets report data for the given time span.
        /// </summary>
        /// <param name="fromDate">The (inclusive) start of the date range.</param>
        /// <param name="toDate">The (inclusive) end of the date range.</param>
        /// <returns></returns>
        public static EmployeeReceivingData GetReportData(DateTime fromDate, DateTime toDate)
        {
            SecurityDataSet.UsersDataTable dtUsers = null;
            Data.Reports.ProcessPartsReport.EmployeePerformanceCreatedDataTable dtCreated = null;
            Data.Reports.ProcessPartsReport.EmployeePerformanceReviewedDataTable dtReviewed = null;

            try
            {
                dtUsers = new SecurityDataSet.UsersDataTable();
                dtCreated = new Data.Reports.ProcessPartsReport.EmployeePerformanceCreatedDataTable();
                dtReviewed = new Data.Reports.ProcessPartsReport.EmployeePerformanceReviewedDataTable();

                using (var taUsers = new UsersTableAdapter())
                {
                    taUsers.Fill(dtUsers);
                }

                using (var taCreated = new EmployeePerformanceCreatedTableAdapter())
                {
                    taCreated.Fill(dtCreated, fromDate, toDate);
                }

                using (var taReviewed = new EmployeePerformanceReviewedTableAdapter())
                {
                    taReviewed.Fill(dtReviewed, fromDate, toDate);
                }

                return ReportDataFrom(dtUsers, dtCreated, dtReviewed);
            }
            finally
            {
                dtUsers?.Dispose();
                dtCreated?.Dispose();
                dtReviewed?.Dispose();
            }
        }

        private static EmployeeReceivingData ReportDataFrom(SecurityDataSet.UsersDataTable dtUsers,
            Data.Reports.ProcessPartsReport.EmployeePerformanceCreatedDataTable dtCreated,
            Data.Reports.ProcessPartsReport.EmployeePerformanceReviewedDataTable dtReviewed)
        {
            var userIdDictionary = dtUsers.ToDictionary(
                userRow => userRow.UserID,
                userRow => new User
                {
                    Name = userRow.Name,
                    OrderActions = new List<OrderAction>()
                });

            // Orders may not specify a created user
            userIdDictionary.Add(-1, new User {Name = "N/A", OrderActions = new List<OrderAction>()});

            foreach (var createdRow in dtCreated)
            {
                User user;

                if (createdRow.IsCreatedByNull())
                {
                    user = userIdDictionary[-1];
                }
                else if (!userIdDictionary.TryGetValue(createdRow.CreatedBy, out user))
                {
                    LogManager.GetCurrentClassLogger().Warn("Could not find user id {0} - skipping", createdRow.CreatedBy);
                    continue;
                }

                user.OrderActions.Add(new OrderAction
                {
                    OrderId = createdRow.OrderID,
                    PartQuantity = createdRow.IsPartQuantityNull() ? 0 : createdRow.PartQuantity,
                    CustomerName = createdRow.IsCustomerNameNull() ? string.Empty : createdRow.CustomerName,
                    PartName = createdRow.IsPartNameNull() ? string.Empty : createdRow.PartName,
                    ActionType = OrderActionType.Created,
                    ActionDate = createdRow.Created
                });
            }

            foreach (var reviewedRow in dtReviewed)
            {
                if (!userIdDictionary.TryGetValue(reviewedRow.ReviewedBy, out User user))
                {
                    LogManager.GetCurrentClassLogger().Warn("Could not find user id {0} - skipping", reviewedRow.ReviewedBy);
                    continue;
                }

                user.OrderActions.Add(new OrderAction
                {
                    OrderId = reviewedRow.OrderID,
                    PartQuantity = reviewedRow.IsPartQuantityNull() ? 0 : reviewedRow.PartQuantity,
                    CustomerName = reviewedRow.IsCustomerNameNull() ? string.Empty : reviewedRow.CustomerName,
                    PartName = reviewedRow.IsPartNameNull() ? string.Empty : reviewedRow.PartName,
                    ActionType = OrderActionType.Reviewed,
                    ActionDate = reviewedRow.Reviewed
                });
            }

            return new EmployeeReceivingData
            {
                Users = userIdDictionary.Values
                    .Where(u => u.OrderActions.Count > 0)
                    .ToList()
            };
        }

        #endregion

        #region User

        /// <summary>
        /// Represents a user and orders that they interacted with.
        /// </summary>
        public class User
        {
            #region Properties

            /// <summary>
            /// Gets or sets the name for this instance.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the order actions for this instance.
            /// </summary>
            public List<OrderAction> OrderActions { get; set; }

            #endregion
        }

        #endregion

        #region Order

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
            /// Gets the part quantity for this instance.
            /// </summary>
            public int PartQuantity { get; set; }

            /// <summary>
            /// Gets the customer name for this instance.
            /// </summary>
            public string CustomerName { get; set; }

            /// <summary>
            /// Gets the part name for this instance.
            /// </summary>
            public string PartName { get; set; }

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
            /// Order entry
            /// </summary>
            Created,

            /// <summary>
            /// Order review
            /// </summary>
            /// <remarks>
            /// Can represent a passed or failed review.
            /// </remarks>
            Reviewed
        }

        #endregion
    }
}
