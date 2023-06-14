using DWOS.Data.Datasets;
using DWOS.Data.Order.Activity;
using DWOS.Services.Messages;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Data.Process;
using System.Web.Http;

namespace DWOS.Services
{
    public class OrderProcessAnswerController : ApiController
    {
        #region Methods

        /// <summary>
        /// Retrieves order process answers.
        /// </summary>
        /// <remarks>
        /// This route uses POST because it can create answers for the order.
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ServiceExceptionFilter("Error getting process answers.")]
        public ResponseBase Retrieve(OrderProcessAnswerRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            return new OrderProcessAnswerResponse
            {
                Success = true,
                ErrorMessage = null,
                OrderProcessAnswers = CreateAnswers(request.OrderId, request.OrderProcessId)
            };
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "OrderProcessing")]
        [ServiceExceptionFilter("Error saving answers.")]
        public ResponseBase Save(OrderProcessAnswerSaveRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            return SaveAnswers(request.UserId, request.OrderId,
                request.CurrentProcessedPartQty, request.OrderProcessId,
                request.OrderProcessAnswers);
        }

        #endregion

        #region Factories

        public static List<OrderProcessAnswerInfo> CreateAnswers(int orderId, int orderProcessId)
        {
            var orderProcesses = new List<OrderProcessAnswerInfo>();

            try
            {
                //ensure answers are created
                OrderProcessingDataSet.OrderSummaryDataTable.CreateOrderAnswersWithLock(orderId);

                var orderAnswers = new Data.Datasets.OrderProcessingDataSet.OrderProcessAnswerDataTable();

                using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessAnswerTableAdapter())
                    ta.FillByOrderProcessesID(orderAnswers, orderProcessId);

                // Add only the first answer for each question
                var questions = orderAnswers.GroupBy(oa => oa.ProcessQuestionID);

                foreach (var questionGroup in questions)
                {
                    // If there are duplicate answers, prefer showing the one with the smallest ID
                    var oa = questionGroup
                        .OrderBy(opa => opa.OrderProcesserAnswerID)
                        .FirstOrDefault();

                    if (oa == null)
                    {
                        // Should not normally happen
                        continue;
                    }

                    orderProcesses.Add(new OrderProcessAnswerInfo()
                    {
                        OrderId = orderId,
                        OrderProcessId = oa.OrderProcessesID,
                        Answer = oa.IsAnswerNull() ? null : oa.Answer,
                        ProcessQuestionId = oa.ProcessQuestionID,
                        OrderProcessAnswerId = oa.OrderProcesserAnswerID,
                        Completed = oa.Completed,
                        CompletedBy = oa.IsCompletedByNull() ? -1 : oa.CompletedBy,
                        CompletedDate = oa.IsCompletedDataNull() ? DateTime.MinValue : oa.CompletedData
                    });
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error creating order process infos.");
            }

            return orderProcesses;
        }

        private static OrderProcessAnswerSaveResponse SaveAnswers(int userId, int orderId, int currentProcessedPartQty, int orderProcessId, List<OrderProcessAnswerInfo> orderAnswers)
        {
            OrderProcessAnswerTableAdapter taOrderProcessAnswer = null;
            OrderCustomFieldsTableAdapter taOrderField = null;
            OrderSummaryTableAdapter taOrder = null;

            try
            {
                if (orderAnswers == null)
                {
                    return new OrderProcessAnswerSaveResponse() { Success = false, ErrorMessage = "Invalid data" };
                }

                taOrderProcessAnswer = new OrderProcessAnswerTableAdapter();
                taOrderField = new OrderCustomFieldsTableAdapter();
                taOrder = new OrderSummaryTableAdapter();

                // Load process data
                var dsOrderProcessing = new OrderProcessingDataSet { EnforceConstraints = false };
                taOrder.FillById(dsOrderProcessing.OrderSummary, orderId);

                using (var taCustomField = new CustomFieldTableAdapter())
                {
                    taCustomField.FillByOrder(dsOrderProcessing.CustomField, orderId);
                }

                taOrderField.FillByOrder(dsOrderProcessing.OrderCustomFields, orderId);

                int processId;

                using (var taOrderProcess = new OrderProcessSummaryTableAdapter())
                {
                    processId = taOrderProcess.GetProcessId(orderProcessId) ?? -1;
                }

                using (var taQuestion = new ProcessQuestionTableAdapter())
                {
                    taQuestion.FillBy(dsOrderProcessing.ProcessQuestion, processId);
                }

                using (var taQuestionField = new ProcessQuestionFieldTableAdapter())
                {
                    taQuestionField.FillByProcess(dsOrderProcessing.ProcessQuestionField, processId);
                }

                var orderRow = dsOrderProcessing.OrderSummary.FirstOrDefault();

                // Save answers
                foreach(var oa in orderAnswers)
                {
                    var row = dsOrderProcessing.OrderProcessAnswer.NewOrderProcessAnswerRow();
                    row.Answer = oa.Answer;
                    row.Completed = oa.Completed;
                    //if (oa.Completed && oa.CompletedDate > ((DateTime.Now).Subtract(new TimeSpan(0, 1, 0))))
                    if (oa.Completed && oa.CompletedBy < 0)
                    {
                        oa.CompletedBy = userId;
                    }

                    if (oa.Completed && oa.CompletedBy > 0)
                    {
                        row.CompletedBy = oa.CompletedBy;
                    }
                    else
                    {
                        row.SetCompletedByNull();
                    }

                    if (oa.Completed && oa.CompletedDate > DateTime.MinValue)
                    {
                        row.CompletedData = oa.CompletedDate;
                    }
                    else
                    {
                        row.SetCompletedDataNull();
                    }

                    row.OrderRow = orderRow;
                    row.OrderProcesserAnswerID = oa.OrderProcessAnswerId;
                    row.OrderProcessesID = oa.OrderProcessId;
                    row.ProcessQuestionID = oa.ProcessQuestionId;

                    ProcessUtilities.EnsureFieldsExist(row, orderRow, dsOrderProcessing);
                    ProcessUtilities.SetFieldValue(row, orderRow);
                    dsOrderProcessing.OrderProcessAnswer.Rows.Add(row);

                    row.AcceptChanges(); //not sure if we need this

                    if (oa.ProcessQuestionId > 0)
                    {
                        // need modified to let table adapter
                        // know to do an update not an insert
                        row.SetModified(); 
                    }
                    else
                    {
                        // should not ever make it into here; the order
                        // process answers should only be doing updates
                        row.SetAdded(); 
                    }
                }

                taOrderProcessAnswer.Update(dsOrderProcessing.OrderProcessAnswer);
                taOrderField.Update(dsOrderProcessing.OrderCustomFields);

                var department = orderRow.CurrentLocation;
                var currentLine = taOrder.GetCurrentLine(orderId);

                var activity = new ProcessingActivity(orderId, new ActivityUser(userId, department, currentLine))
                { 
                    OrderProcessID = orderProcessId,
                    CurrentProcessedPartQty = currentProcessedPartQty
                };

                var results = activity.Complete() as ProcessingActivity.ProcessingActivityResults;

                if (results == null)
                {
                    return new OrderProcessAnswerSaveResponse() { Success = false, ErrorMessage = "Invalid activity data" };
                }

                var response = new OrderProcessAnswerSaveResponse() { Success = true };

                if (results.NextRequisiteProcessID.HasValue && results.NextRequisiteHours.HasValue)
                {
                    using(var taProcess = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter())
                    {
                        var processName = taProcess.GetProcessName(results.NextRequisiteProcessID.Value);
                        response.HasNextProcessTimeConstraint = true;
                        response.NextProcessTimeConstraintMessage = "Process {0} must be completed within the next {1} hours.".FormatWith(processName, results.NextRequisiteHours.Value.ToString("N2"));
                    }
                }
                
                return response;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error saving answers for order {0}.".FormatWith(orderId));
                return new OrderProcessAnswerSaveResponse() { Success = true, ErrorMessage = "Error saving answers for order {0}.".FormatWith(orderId) + exc.Message };
            }
            finally
            {
                taOrderProcessAnswer?.Dispose();
                taOrderField?.Dispose();
                taOrder?.Dispose();
            }
        }

        #endregion
    }
}