using System.Linq;

namespace DWOS.Data.Order.Activity
{
    /// <summary>
    /// A processing activity that occurs on a batch for a specific process.
    /// </summary>
    public class BatchProcessingActivity
    {
        #region Methods

        /// <summary>
        /// Gets the batches primary order used to order process on by default.
        /// </summary>
        /// <param name="batchId">The batch identifier.</param>
        /// <param name="batchProcessId">The batch process identifier.</param>
        /// <returns>OrderToProcess.</returns>
        public static OrderToProcess GetBatchesPrimaryOrderToProcess(int batchId, int? batchProcessId = null)
        {
            var otp = new OrderToProcess();

            //find current batch process id if not set
            using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter())
            {
                using(var bpTable = new Data.Datasets.OrderProcessingDataSet.BatchProcessesDataTable())
                {
                    DWOS.Data.Datasets.OrderProcessingDataSet.BatchProcessesRow batchProcess = null;

                    if(!batchProcessId.HasValue)
                    {
                        ta.FillBy(bpTable, batchId);
                        batchProcess = bpTable.OrderBy(ob => ob.StepOrder).FirstOrDefault(r => r.IsEndDateNull());
                    }
                    else
                    {
                        ta.FillByBatchProcess(bpTable, batchProcessId.Value);
                        batchProcess = bpTable.FirstOrDefault();
                    }

                    if(batchProcess != null)
                    {
                        otp.BatchProcessId = batchProcess.BatchProcessID;
                        otp.ProcessId = batchProcess.ProcessID;
                    }
                }
            }

            //Find Order, which is just the first order sorted by batch order id
            using(var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchOrderTableAdapter() {ClearBeforeFill = false})
            {
                using(var batchOrders = new Data.Datasets.OrderProcessingDataSet.BatchOrderDataTable())
                {
                    ta.FillBy(batchOrders, batchId);

                    var primaryBatchOrder = batchOrders.OrderBy(bo => bo.BatchOrderID).FirstOrDefault();
                    
                    if(primaryBatchOrder != null)
                        otp.OrderId = primaryBatchOrder.OrderID;
                }
            }

            //Find Order Process for the order and batch process
            using(var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcess_OrderProcessTableAdapter() {ClearBeforeFill = false})
            {
                using(var orderProcesses = new Data.Datasets.OrderProcessingDataSet.BatchProcess_OrderProcessDataTable())
                {
                    ta.FillByBatchProcessAndOrder(orderProcesses, otp.BatchProcessId, otp.OrderId);

                    var orderProcess = orderProcesses.FirstOrDefault(opr => opr.BatchProcessID == otp.BatchProcessId);
                    
                    if(orderProcess != null)
                        otp.OrderProcessId = orderProcess.OrderProcessID;
                }
            }

            return otp;
        }

        #endregion

        #region OrderToProcess

        /// <summary>
        /// Represents an order to process and its process.
        /// </summary>
        public class OrderToProcess
        {
            /// <summary>
            /// Gets or sets the order ID of this instance.
            /// </summary>
            public int OrderId { get; set; }

            /// <summary>
            /// Gets or sets the order process ID of this instance.
            /// </summary>
            public int OrderProcessId { get; set; }

            /// <summary>
            /// Gets or sets the batch process ID of this instance.
            /// </summary>
            public int BatchProcessId { get; set; }

            /// <summary>
            /// Gets or sets the process ID of this instance.
            /// </summary>
            public int ProcessId { get; set; }
        }

        #endregion
    }
}
