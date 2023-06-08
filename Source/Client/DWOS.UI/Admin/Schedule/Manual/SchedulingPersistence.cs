using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Order;
using DWOS.UI.Utilities;

namespace DWOS.UI.Admin.Schedule.Manual
{
    public class SchedulingPersistence : ISchedulingPersistence
    {
        #region Properties

        public ISecurityManager SecurityManager { get; }

        #endregion

        #region Methods

        public SchedulingPersistence(ISecurityManager securityManager)
        {
            SecurityManager = securityManager;
        }

        public void SaveChanges(SchedulingTabDataContext context)
        {
            var orderItems = new List<SchedulingHelper.OrderScheduleItem>();
            var batchItems = new List<SchedulingHelper.BatchScheduleItem>();

            foreach (var dept in context.Departments)
            {
                // Save updated, unscheduled orders
                foreach (var unscheduledItem in dept.Unscheduled.Where(o => o.Updated))
                {
                    if (unscheduledItem.Type == DepartmentDataContext.ScheduleDataType.Order)
                    {
                        orderItems.Add(new SchedulingHelper.OrderScheduleItem
                        {
                            OrderId = unscheduledItem.Id,
                            CurrentSchedulePriority = unscheduledItem.SchedulePriority,
                            PreviousSchedulePriority = unscheduledItem.OriginalSchedulePriority
                        });
                    }
                    else if (unscheduledItem.Type == DepartmentDataContext.ScheduleDataType.Batch)
                    {
                        batchItems.Add(new SchedulingHelper.BatchScheduleItem
                        {
                            BatchId = unscheduledItem.Id,
                            CurrentSchedulePriority = unscheduledItem.SchedulePriority,
                            PreviousSchedulePriority = unscheduledItem.OriginalSchedulePriority
                        });
                    }
                }

                // Save all scheduled items
                // Doing so prevents the daily schedule reset from undoing changes.
                foreach (var scheduledItem in dept.Scheduled)
                {
                    if (scheduledItem.Type == DepartmentDataContext.ScheduleDataType.Order)
                    {
                        orderItems.Add(new SchedulingHelper.OrderScheduleItem
                        {
                            OrderId = scheduledItem.Id,
                            CurrentSchedulePriority = scheduledItem.SchedulePriority,
                            PreviousSchedulePriority = scheduledItem.OriginalSchedulePriority
                        });
                    }
                    else if (scheduledItem.Type == DepartmentDataContext.ScheduleDataType.Batch)
                    {
                        batchItems.Add(new SchedulingHelper.BatchScheduleItem
                        {
                            BatchId = scheduledItem.Id,
                            CurrentSchedulePriority = scheduledItem.SchedulePriority,
                            PreviousSchedulePriority = scheduledItem.OriginalSchedulePriority
                        });
                    }
                }
            }

            SchedulingHelper.SaveSchedule(orderItems, batchItems, SecurityManager.UserInfo);

            // Successfully saved changes
            foreach (var dept in context.Departments)
            {
                foreach (var order in dept.Unscheduled)
                {
                    order.Updated = false;
                }

                foreach (var order in dept.Scheduled)
                {
                    order.Updated = false;
                }
            }
        }

        #endregion
    }
}