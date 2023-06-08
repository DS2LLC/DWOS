using System;
using System.Collections.Generic;

namespace DWOS.Data.Coc
{
    public class CertificateBatch
    {
        #region Properties

        public int BatchId { get; }

        public int? SalesOrderId { get; set; }

        public List<BatchProcess> BatchProcesses { get; }

        #endregion

        #region Methods

        public CertificateBatch(int batchId, int? salesOrderId, List<BatchProcess> batchProcesses)
        {
            BatchId = batchId;
            SalesOrderId = salesOrderId;
            BatchProcesses = batchProcesses
                ?? throw new ArgumentNullException(nameof(batchProcesses));
        }

        #endregion

        #region BatchProcess

        public class BatchProcess
        {
            #region Properties

            public int BatchProcessId { get; }

            public int ProcessId { get; set; }

            public List<int> OrderProcessesIds { get; }

            #endregion

            #region Methods

            public BatchProcess(int batchProcessID, int processId, List<int> orderProcessesIds)
            {
                BatchProcessId = batchProcessID;
                ProcessId = processId;
                OrderProcessesIds = orderProcessesIds
                    ?? throw new ArgumentNullException(nameof(orderProcessesIds));
            }

            #endregion
        }

        #endregion
    }
}
