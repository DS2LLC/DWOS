using System;
using System.Collections.Generic;
using System.Data;

namespace DWOS.Data.Datasets
{
    public partial class OrderProcessingDataSet
    {
        /// <summary>
        /// Gets the total order question count based on completed answers. Requires OrderProcessAnswer to be hydrated with this order.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="completedQuestions">The completed questions count.</param>
        /// <param name="totalQuestions">The total questions count.</param>
        public void GetTotalOrderQuestionCount(int orderID, out int completedQuestions, out int totalQuestions)
        {
            var opas = this.OrderProcessAnswer.Select("OrderID = " + orderID) as OrderProcessingDataSet.OrderProcessAnswerRow[];
            completedQuestions = 0;
            totalQuestions = opas.Length;

            foreach (var opa in opas)
            {
                if (opa.Completed)
                    completedQuestions++;
            }
        }

        /// <summary>
        /// Gets the process step question count for this order process step.
        /// </summary>
        /// <param name="orderProcessesID">The order processes ID.</param>
        /// <param name="completedQuestions">The completed questions.</param>
        /// <param name="totalQuestions">The total questions.</param>
        public void GetProcessStepQuestionCount(int orderProcessesID, out int completedQuestions, out int totalQuestions)
        {
            var opas = this.OrderProcessAnswer.Select("OrderProcessesID = " + orderProcessesID) as OrderProcessingDataSet.OrderProcessAnswerRow[];
            completedQuestions = 0;
            totalQuestions = opas.Length;

            foreach (var opa in opas)
            {
                if (opa.Completed)
                    completedQuestions++;
            }
        }

        //public void GetProcessStepQuestionCount(int orderProcessesID, int stepID, out int completedQuestions, out int totalQuestions)
        //{
        //    var opas = this.OrderProcessAnswer.Select("OrderProcessesID = " + orderProcessesID) as OrderProcessingDataSet.OrderProcessAnswerRow[];
        //    completedQuestions = 0;
        //    totalQuestions = opas.Length;

        //    foreach (var opa in opas)
        //    {
        //        if (opa.Completed)
        //            completedQuestions++;
        //    }
        //}

        partial class OrderSummaryDataTable
        {
            private static object _padLock = new object();

            /// <summary>
            /// Creates the answers for this order if they do not exist. Use a lock to sync access to this method.
            /// </summary>
            /// <param name="orderID">The order identifier.</param>
		    public static void CreateOrderAnswersWithLock(int orderID)
            {
                lock (_padLock)
                    CreateOrderAnswers(orderID);
            }

            /// <summary>
            /// Creates the answers for this order if they do not exist.
            /// </summary>
            /// <param name="orderID">The order ID.</param>
            public static void CreateOrderAnswers(int orderID)
            {
                OrderProcessingDataSetTableAdapters.ProcessQuestionTableAdapter pqTA = null;
                OrderProcessingDataSetTableAdapters.OrderProcessAnswerTableAdapter opaTA = null;
                OrderProcessingDataSetTableAdapters.PartProcessAnswerTableAdapter ppaTA = null;
                OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter osTA = null;
                OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter opTA = null;

                OrderProcessingDataSet opDS = new OrderProcessingDataSet();
                ProcessQuestionDataTable pqDT = opDS.ProcessQuestion;
                OrderProcessAnswerDataTable opaDT = new OrderProcessAnswerDataTable();
                OrderProcessesDataTable opDT = new OrderProcessesDataTable();
                OrderSummaryDataTable osDT = new OrderSummaryDataTable();
                OrderSummaryRow or = null;

                try
                {
                    //get order information
                    osTA = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter();
                    osTA.FillById(osDT, orderID);
                    or = osDT.FindByOrderID(orderID);

                    pqTA = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessQuestionTableAdapter();

                    ppaTA = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.PartProcessAnswerTableAdapter();
                    opaTA = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessAnswerTableAdapter();
                    opTA = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter();

                    opTA.FillBy(opDT, orderID);
                    opaTA.FillBy(opaDT, orderID);

                    //for each orders process
                    foreach (OrderProcessesRow opRow in opDT.Rows)
                    {
                        //Get Process questions
                        pqTA.FillBy(pqDT, opRow.ProcessID);

                        foreach (ProcessQuestionRow pqRow in pqDT) //for each process question
                        {
                            //see if has been answered already
                            int answerCount = opaDT.Select("ProcessQuestionID = " + pqRow.ProcessQuestionID + " AND OrderProcessesID = " + opRow.OrderProcessesID).Length;

                            //if no answer exists then stub out answer
                            if (answerCount == 0)
                            {
                                OrderProcessAnswerRow opaRow = opaDT.NewRow() as OrderProcessAnswerRow;

                                opaRow.ProcessQuestionID = pqRow.ProcessQuestionID;
                                opaRow.OrderProcessesID = opRow.OrderProcessesID;
                                opaRow.OrderID = orderID;
                                opaRow.Completed = false;

                                //get part specific answer, if does not exist try and get default process answer
                                object answer = ppaTA.GetAnswer2(opRow.ProcessID, or.PartID, pqRow.ProcessQuestionID) ?? ppaTA.GetDefaultAnswer(pqRow.ProcessQuestionID);
                                opaRow.Answer = answer == DBNull.Value || answer == null ? String.Empty : answer.ToString();

                                opaDT.AddOrderProcessAnswerRow(opaRow);
                            }
                        }
                    }

                    opaTA.Update(opaDT);
                }
                finally
                {
                    //dispose adapters
                    if (pqTA != null)
                        pqTA.Dispose();
                    if (opaTA != null)
                        opaTA.Dispose();
                    if (ppaTA != null)
                        ppaTA.Dispose();
                    if (osTA != null)
                        osTA.Dispose();
                    if (opTA != null)
                        opTA.Dispose();

                    //dispose tables
                    if (pqDT != null)
                        pqDT.Dispose();
                    if (opDT != null)
                        opDT.Dispose();
                    if (opaDT != null)
                        opaDT.Dispose();
                    if (osDT != null)
                        osDT.Dispose();

                    osTA = null;
                    opTA = null;
                    pqTA = null;
                    opaTA = null;
                    ppaTA = null;
                }
            }

            /// <summary>
            /// Creates the order processes.
            /// </summary>
            /// <param name="orderID">The order ID.</param>
            public static void CreateOrderProcesses(int orderID)
            {
                OrderProcessingDataSetTableAdapters.PartProcessTableAdapter ppTA = null;
                OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter osTA = null;
                OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter opTA = null;

                PartProcessDataTable ppDT = new PartProcessDataTable();
                OrderProcessesDataTable opDT = new OrderProcessesDataTable();
                OrderSummaryDataTable osDT = new OrderSummaryDataTable();
                OrderSummaryRow or = null;
                ProcessDataTable pDT = new ProcessDataTable();
                try
                {
                    osTA = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter();
                    osTA.FillById(osDT, orderID);
                    or = osDT.FindByOrderID(orderID);

                    if (or == null)
                        return;

                    //Find Part Processes
                    ppTA = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.PartProcessTableAdapter();
                    ppTA.FillBy(ppDT, or.PartID);

                    var pTA = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter() { ClearBeforeFill = true };

                    opTA = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter();

                    opTA.FillBy(opDT, orderID);

                    int orderStepOrder = 1;

                    //For each process required for the part
                    foreach (PartProcessRow ppr in ppDT.Rows)
                    {
                        pTA.FillByProcess(pDT, ppr.ProcessID); //Get Process steps

                        OrderProcessesRow opRow = null;
                        opRow = opDT.NewOrderProcessesRow();
                        opRow.OrderID = orderID;
                        opRow.ProcessID = ppr.ProcessID;
                        opRow.ProcessAliasID = ppr.ProcessAliasID;
                        opRow.StepOrder = orderStepOrder++;
                        opRow.Department = pDT[0].Department;
                        opDT.AddOrderProcessesRow(opRow);
                    }

                    opTA.Update(opDT);
                }
                finally
                {
                    //dispose adapters
                    if (ppTA != null)
                        ppTA.Dispose();
                    if (osTA != null)
                        osTA.Dispose();
                    if (opTA != null)
                        opTA.Dispose();
                    if (ppDT != null)
                        ppDT.Dispose();

                    osTA = null;
                    ppTA = null;
                }
            }
        }
    }
}

namespace DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters {
    public partial class OrderProcessesTableAdapter {
        public int GetOrderProcessesID(int orderId, string workStatus)
        {
            int? result = null;

            if (workStatus == ApplicationSettings.Current.WorkStatusInProcess)
            {
                result = GetCurrentOrderProcessID(orderId);
            }
            else if (workStatus == ApplicationSettings.Current.WorkStatusPendingQI)
            {
                result = GetPreviousOrderProcessesID(orderId);
            }

            return result ?? -1;
        }
    }
    public partial class BatchProcessesTableAdapter {
        public int GetBatchProcessesID(int batchId, string workStatus)
        {
            int? result = null;

            if (workStatus == ApplicationSettings.Current.WorkStatusInProcess)
            {
                result = GetCurrentBatchProcessID(batchId);
            }
            else if (workStatus == ApplicationSettings.Current.WorkStatusPendingQI)
            {
                result = GetPreviousBatchProcessID(batchId);
            }

            return result ?? -1;
        }
    }
}


