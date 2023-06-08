using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Data.Order
{
    public static class BatchUtilities
    {
        #region Methods

        public static bool CanBatchFromProcessing(int remainingProcesses, string orderWorkStatus)
        {
            if (remainingProcesses > 0)
            {
                return true;
            }

            var appSettings = ApplicationSettings.Current;

            if (appSettings.BatchMultipleProcesses)
            {
                if (orderWorkStatus == appSettings.WorkStatusPendingQI)
                {
                    return true;
                }

                if (orderWorkStatus == appSettings.WorkStatusPartMarking || orderWorkStatus == appSettings.WorkStatusFinalInspection)
                {
                    return appSettings.ContinueBatchAfterProcessing;
                }
            }

            return false;
        }

        public static bool CanBatchFromInspection(int remainingProcesses, string orderWorkStatus)
        {
            if (remainingProcesses > 0)
            {
                return true;
            }

            var appSettings = ApplicationSettings.Current;

            if (orderWorkStatus == appSettings.WorkStatusPendingQI)
            {
                return true;
            }

            if (orderWorkStatus == appSettings.WorkStatusPartMarking || orderWorkStatus == appSettings.WorkStatusFinalInspection)
            {
                return appSettings.ContinueBatchAfterProcessing;
            }

            return false;
        }

        public static bool CanBatchAfterProcessing(string orderWorkStatus)
        {
            var appSettings = ApplicationSettings.Current;

            if (orderWorkStatus == appSettings.WorkStatusPartMarking || orderWorkStatus == appSettings.WorkStatusFinalInspection)
            {
                return appSettings.ContinueBatchAfterProcessing;
            }

            return false;
        }

        public static string WorkStatusForBatch(IEnumerable<string> orderWorkStatuses)
        {
            if (orderWorkStatuses == null)
            {
                throw new ArgumentNullException(nameof(orderWorkStatuses));
            }

            var appSettings = ApplicationSettings.Current;
            return orderWorkStatuses.OrderBy(WorkStatusValue).FirstOrDefault();
        }

        private static int WorkStatusValue(string status)
        {
            var appSettings = ApplicationSettings.Current;

            if (status == appSettings.WorkStatusPendingQI)
            {
                return 1;
            }
            else if (status == appSettings.WorkStatusInProcess)
            {
                return 2;
            }
            else if (status == appSettings.WorkStatusChangingDepartment)
            {
                return 3;
            }
            else if (status == appSettings.WorkStatusPartMarking)
            {
                return 4;
            }
            else if (status == appSettings.WorkStatusFinalInspection)
            {
                return 5;
            }
            else
            {
                return int.MaxValue;
            }
        }

        #endregion
    }
}
