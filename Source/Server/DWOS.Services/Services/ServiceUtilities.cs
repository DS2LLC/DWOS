using DWOS.Data.Datasets;
using DWOS.Services.Messages;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Data.Process;

namespace DWOS.Services
{
    /// <summary>
    /// Provides utility methods for use in services.
    /// </summary>
    internal static class ServiceUtilities
    {
        #region Methods

        internal static ProcessInfo CreateInfoForProcess(int processId)
        {
            OrderProcessingDataSet processDataSet = null;

            try
            {
                processDataSet = DataSetForProcess(processId);
                var process = processDataSet.Process.FindByProcessID(processId);

                if (process == null)
                {
                    return null;
                }

                return CreateProcessInfo(process);
            }
            finally
            {
                processDataSet?.Dispose();
            }
        }

        internal static OrderProcessDetailInfo CreateInfoForOrderProcess(int orderProcessId)
        {
            OrderProcessingDataSet processDataSet = null;

            try
            {
                processDataSet = DataSetForOrderProcess(orderProcessId);
                var orderProcess = processDataSet.OrderProcesses.FindByOrderProcessesID(orderProcessId);
                var process = processDataSet.Process.FindByProcessID(orderProcess?.ProcessID ?? -1);
                var order = processDataSet.OrderSummary.FirstOrDefault();

                if (process == null || order == null)
                {
                    return null;
                }

                var procInfo = CreateProcessInfo(process, order);
                var details = new OrderProcessDetailInfo
                {
                    OrderProcessId = orderProcessId,
                    ProcessSteps = procInfo.ProcessSteps,
                    Documents = procInfo.Documents,
                    ProcessId = procInfo.ProcessId,
                    IsPaperless = procInfo.IsPaperless,
                    Name = procInfo.Name,
                    Lists = procInfo.Lists
                };

                return details;
            }
            finally
            {
                processDataSet?.Dispose();
            }
        }

        private static ProcessInfo CreateProcessInfo(OrderProcessingDataSet.ProcessRow process, OrderProcessingDataSet.OrderSummaryRow order = null)
        {
            DocumentsDataSet.DocumentInfoDataTable documentInfoTable = null;

            Data.Datasets.DocumentsDataSetTableAdapters.DocumentInfoTableAdapter taDocumentInfo = null;
            Data.Datasets.DocumentsDataSetTableAdapters.DocumentLinkTableAdapter taDocumentLink = null;
            Data.Datasets.ListsDataSetTableAdapters.ListValuesTableAdapter taListValues = null;

            try
            {
                documentInfoTable = new DocumentsDataSet.DocumentInfoDataTable();
                taDocumentInfo = new Data.Datasets.DocumentsDataSetTableAdapters.DocumentInfoTableAdapter();
                taDocumentLink = new Data.Datasets.DocumentsDataSetTableAdapters.DocumentLinkTableAdapter();
                taListValues = new Data.Datasets.ListsDataSetTableAdapters.ListValuesTableAdapter();


                var processInfo = new ProcessInfo();
                processInfo.ProcessId = process.ProcessID;
                processInfo.Department = process.Department;
                processInfo.Description = process.IsDescriptionNull() ? null : process.Description;
                processInfo.Name = process.Name;
                processInfo.Revision = process.IsRevisionNull() ? null : process.Revision;
                processInfo.IsPaperless = process.IsPaperless;
                processInfo.ProcessSteps = new List<ProcessStepInfo>();
                processInfo.Lists = new List<ListInfo>();

                foreach (var processStep in process.GetProcessStepsRows())
                {
                    var step = new ProcessStepInfo();
                    step.ProcessStepId = processStep.ProcessStepID;
                    step.ProcessId = processStep.ProcessID;
                    step.Name = processStep.Name;
                    step.Description = processStep.IsDescriptionNull() ? null : processStep.Description;
                    step.StepOrder = processStep.StepOrder;
                    step.ProcessQuestions = new List<ProcessQuestionInfo>();
                    step.Conditions = new List<ProcessStepCondition>();

                    foreach (var processQuestion in processStep.GetProcessQuestionRows())
                    {
                        string questionMinValue;
                        string questionMaxValue;

                        if (order == null)
                        {
                            questionMinValue = ProcessUtilities.MinValue(processQuestion);
                            questionMaxValue = ProcessUtilities.MaxValue(processQuestion);
                        }
                        else
                        {
                            questionMinValue = ProcessUtilities.MinValue(processQuestion, order);
                            questionMaxValue = ProcessUtilities.MaxValue(processQuestion, order);
                        }

                        var question = new ProcessQuestionInfo();

                        question.ProcessQuestionId = processQuestion.ProcessQuestionID;
                        question.ProcessStepId = processQuestion.ProcessStepID;
                        question.Name = processQuestion.Name;
                        question.StepOrder = processQuestion.StepOrder;

                        //this is a temp work around to handle the new Sampleset question that is not extended to mobile yet.
                        //sampleset questions behave like decimal with manual entry.
                        question.InputType = (processQuestion.InputType == "SampleSet") ? "Decimal" : processQuestion.InputType;
                       // question.InputType = processQuestion.InputType;

                        question.MinValue = questionMinValue;
                        question.MaxValue = questionMaxValue;
                        question.ListID = processQuestion.IsListIDNull() ? -1 : processQuestion.ListID;

                        var defaultValue = processQuestion.IsDefaultValueNull()
                            ? null
                            : processQuestion.DefaultValue;

                        if (string.IsNullOrEmpty(defaultValue) && processQuestion.InputType == InputType.PartQty.ToString() && order != null)
                        {
                            // Use order's qty
                            defaultValue = order.IsPartQuantityNull()
                                ? null
                                : order.PartQuantity.ToString();
                        }

                        question.DefaultValue = defaultValue;
                        question.OperatorEditable = processQuestion.OperatorEditable;
                        question.Required = processQuestion.Required;
                        question.Notes = processQuestion.IsNotesNull() ? null : processQuestion.Notes;
                        question.NumericUnits = processQuestion.IsNumericUntisNull() ? null : processQuestion.NumericUntis;

                        //if has a list and has not been addded yet then add to list of list values
                        if (question.ListID > 0 && !processInfo.Lists.Exists(li => li.ListId == question.ListID))
                        {
                            var listValues = new ListsDataSet.ListValuesDataTable();

                            taListValues.FillBy(listValues, question.ListID);

                            var listInfo = new ListInfo() { ListId = question.ListID, Values = new List<string>() };

                            foreach (var listValue in listValues)
                                listInfo.Values.Add(listValue.Value);

                            processInfo.Lists.Add(listInfo);
                        }

                        step.ProcessQuestions.Add(question);
                    }

                    using (var stepDocumentTable = new DocumentsDataSet.DocumentLinkDataTable())
                    {
                        taDocumentLink.FillBy(stepDocumentTable,
                            "ProcessSteps",
                            processStep.ProcessStepID);

                        if (stepDocumentTable.Count > 0)
                        {
                            step.Documents = new List<DocumentInfo>();

                            foreach (var stepDocRow in stepDocumentTable)
                            {
                                taDocumentInfo.FillByID(documentInfoTable, stepDocRow.DocumentInfoID);

                                var document = documentInfoTable.FindByDocumentInfoID(stepDocRow.DocumentInfoID);
                                var currentRevisionId = (taDocumentInfo.GetCurrentRevisionID(stepDocRow.DocumentInfoID) as int?);

                                if (currentRevisionId.HasValue)
                                {
                                    step.Documents.Add(new DocumentInfo()
                                    {
                                        CurrentRevisionId = currentRevisionId.Value,
                                        DocumentInfoID = stepDocRow.DocumentInfoID,
                                        DocumentType = "ProcessSteps",
                                        MediaType = document.MediaType,
                                        Name = document.Name
                                    });
                                }
                            }
                        }
                    }

                    foreach (var condition in processStep.GetProcessStepConditionRows())
                    {
                        step.Conditions.Add(new ProcessStepCondition
                        {
                            ProcessQuestionId = condition.IsProcessQuestionIdNull() ? -1 : condition.ProcessQuestionId,
                            ProcessStepId = condition.ProcessStepId,

                            //this is a temp work around to handle the new Sampleset question that is not extended to mobile yet.
                            //sampleset questions behave like decimal with manual entry.
                            InputType = (condition.InputType == "SampleSet") ? "Decimal" : condition.InputType,
                            //InputType = condition.InputType,

                            Operator = condition.Operator,
                            Value = condition.Value
                        });
                    }

                    processInfo.ProcessSteps.Add(step);
                }

                var docs = new List<DocumentInfo>();

                using (var processDocumentTable = new DocumentsDataSet.DocumentLinkDataTable())
                {
                    taDocumentLink.FillBy(processDocumentTable,
                        "Process",
                        process.ProcessID);

                    foreach (var processDocumentRow in processDocumentTable)
                    {
                        taDocumentInfo.FillByID(documentInfoTable, processDocumentRow.DocumentInfoID);

                        var document = documentInfoTable.FindByDocumentInfoID(processDocumentRow.DocumentInfoID);
                        var currentRevisionId = (taDocumentInfo.GetCurrentRevisionID(processDocumentRow.DocumentInfoID) as int?);

                        if (currentRevisionId.HasValue)
                        {
                            docs.Add(new DocumentInfo()
                            {
                                CurrentRevisionId = currentRevisionId.Value,
                                DocumentInfoID = processDocumentRow.DocumentInfoID,
                                DocumentType = "Process",
                                MediaType = document.MediaType,
                                Name = document.Name
                            });
                        }
                    }
                }

                if (docs.Count > 0)
                {
                    processInfo.Documents = docs;
                }

                return processInfo;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting process info for process id " + process.ProcessID);
                return null;
            }
            finally
            {
                documentInfoTable.Dispose();
                taDocumentInfo.Dispose();
                taDocumentLink.Dispose();
                taListValues?.Dispose();
            }
        }

        private static OrderProcessingDataSet DataSetForProcess(int processId)
        {
            var processDataSet = new OrderProcessingDataSet {EnforceConstraints = false};

            using (var ta = new ProcessTableAdapter())
            {
                ta.FillByProcess(processDataSet.Process, processId);
            }

            using (var ta = new ProcessStepsTableAdapter())
            {
                ta.FillBy(processDataSet.ProcessSteps, processId);
            }

            using (var ta = new ProcessQuestionTableAdapter())
            {
                ta.FillBy(processDataSet.ProcessQuestion, processId);
            }

            using (var ta = new ProcessStepConditionTableAdapter())
            {
                ta.Fill(processDataSet.ProcessStepCondition, processId);
            }

            return processDataSet;
        }

        private static OrderProcessingDataSet DataSetForOrderProcess(int orderProcessId)
        {
            var processDataSet = new OrderProcessingDataSet {EnforceConstraints = false};

            using (var ta = new OrderProcessesTableAdapter())
            {
                ta.FillByID(processDataSet.OrderProcesses, orderProcessId);
            }

            var orderProcess = processDataSet.OrderProcesses.FindByOrderProcessesID(orderProcessId);

            if (orderProcess == null)
            {
                return processDataSet;
            }

            // Process
            using (var ta = new ProcessTableAdapter())
            {
                ta.FillByProcess(processDataSet.Process, orderProcess.ProcessID);
            }

            using (var ta = new ProcessStepsTableAdapter())
            {
                ta.FillBy(processDataSet.ProcessSteps, orderProcess.ProcessID);
            }

            using (var ta = new ProcessQuestionTableAdapter())
            {
                ta.FillBy(processDataSet.ProcessQuestion, orderProcess.ProcessID);
            }

            using (var ta = new ProcessQuestionFieldTableAdapter())
            {
                ta.FillByProcess(processDataSet.ProcessQuestionField, orderProcess.ProcessID);
            }

            using (var ta = new ProcessStepConditionTableAdapter())
            {
                ta.Fill(processDataSet.ProcessStepCondition, orderProcess.ProcessID);
            }

            // Order
            using (var ta = new OrderSummaryTableAdapter())
            {
                ta.FillById(processDataSet.OrderSummary, orderProcess.OrderID);
            }

            using (var ta = new OrderCustomFieldsTableAdapter())
            {
                ta.FillByOrder(processDataSet.OrderCustomFields, orderProcess.OrderID);
            }

            using (var ta = new CustomFieldTableAdapter())
            {
                ta.FillByOrder(processDataSet.CustomField, orderProcess.OrderID);
            }

            return processDataSet;
        }

        internal static ProcessAliasInfo CreateProcessAlias(OrderProcessingDataSet.OrderProcessesRow orderProcess)
        {
            if (orderProcess == null)
            {
                return null;
            }

            var processAliasInfo = new ProcessAliasInfo()
            {
                ProcessId = orderProcess.ProcessID,
                ProcessAliasId = orderProcess.ProcessAliasID,
            };

            DocumentsDataSet.DocumentInfoDataTable documentInfoTable = null;
            DocumentsDataSet.DocumentLinkDataTable aliasDocumentTable = null;

            var docs = new List<DocumentInfo>();
            try
            {
                documentInfoTable = new DocumentsDataSet.DocumentInfoDataTable();
                aliasDocumentTable = new DocumentsDataSet.DocumentLinkDataTable();

                using (var ta = new Data.Datasets.DocumentsDataSetTableAdapters.DocumentLinkTableAdapter())
                {
                    ta.FillBy(aliasDocumentTable,
                        "ProcessAlias",
                        orderProcess.ProcessAliasID);
                }

                using (var taDocumentInfo = new Data.Datasets.DocumentsDataSetTableAdapters.DocumentInfoTableAdapter())
                {
                    foreach (var aliasDocRow in aliasDocumentTable)
                    {
                        taDocumentInfo.FillByID(documentInfoTable, aliasDocRow.DocumentInfoID);

                        var document = documentInfoTable.FindByDocumentInfoID(aliasDocRow.DocumentInfoID);
                        var currentRevisionId = (taDocumentInfo.GetCurrentRevisionID(aliasDocRow.DocumentInfoID) as int?);

                        if (currentRevisionId.HasValue)
                        {
                            docs.Add(new DocumentInfo()
                            {
                                CurrentRevisionId = currentRevisionId.Value,
                                DocumentInfoID = aliasDocRow.DocumentInfoID,
                                DocumentType = "ProcessAlias",
                                MediaType = document.MediaType,
                                Name = document.Name
                            });
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting process alias info for process alias id {0}",
                    orderProcess.ProcessAliasID);
            }
            finally
            {
                documentInfoTable?.Dispose();
                aliasDocumentTable?.Dispose();
            }

            if (docs.Count > 0)
            {
                processAliasInfo.Documents = docs;
            }

            return processAliasInfo;
        }

        #endregion
    }
}
