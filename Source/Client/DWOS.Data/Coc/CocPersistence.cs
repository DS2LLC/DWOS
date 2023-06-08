using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.Data.Datasets.COCDatasetTableAdapters;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Data.Datasets.OrderHistoryDataSetTableAdapters;
namespace DWOS.Data.Coc
{
    public class CocPersistence
    {
        #region Properties

        public ApplicationSettings AppSettings { get; }

        #endregion

        #region Methods

        public CocPersistence(ApplicationSettings appSettings)
        {
            AppSettings = appSettings
                ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public CertificateCustomer GetCustomer(int customerId)
        {
            string customerName;
            using (var  dtCustomer = new CustomersDataset.CustomerDataTable())
            {
                using (var taCustomer = new CustomerTableAdapter())
                {
                    taCustomer.FillBy(dtCustomer, customerId);
                }

                customerName = dtCustomer.FirstOrDefault()?.Name;
            }

            return new CertificateCustomer(customerId, customerName);
        }

        public CertificateBatchOrder GetOrder(int orderId, int batchId)
        {
            int partId;
            string partName;
            List<string> serialNumbers;
            using (var dsCoc = new COCDataset { EnforceConstraints = false })
            {
                using (var taCocPart = new COCPartTableAdapter())
                {
                    taCocPart.FillByOrder(dsCoc.COCPart, orderId);
                }

                var partRow = dsCoc.COCPart.FirstOrDefault();
                partId = partRow?.PartID ?? -1;
                partName = partRow?.Name;

                using (var taSerialNumbers = new OrderSerialNumberTableAdapter())
                {
                    taSerialNumbers.FillByOrder(dsCoc.OrderSerialNumber, orderId);
                }

                serialNumbers = dsCoc.OrderSerialNumber
                    .Where(serial => serial.Active && !serial.IsNumberNull())
                    .Select(serial => serial.Number)
                    .ToList();
            }

            // Imported price
            decimal? importedPrice;
            using (var taCoc = new COCTableAdapter())
            {
                importedPrice = taCoc.GetImportedValue(orderId);
            }

            // Batch quantity
            int batchQuantity;
            using (var dtBatchOrder = new OrderProcessingDataSet.BatchOrderDataTable())
            {
                using (var taBatchOrder = new Datasets.OrderProcessingDataSetTableAdapters.BatchOrderTableAdapter())
                {
                    taBatchOrder.FillBy(dtBatchOrder, batchId);
                }

                var batchOrderRow = dtBatchOrder.FirstOrDefault(batchOrder => batchOrder.OrderID == orderId);
                batchQuantity = batchOrderRow?.PartQuantity ?? 0;
            }

            // Load custom fields
            var fields = new List<CertificateBatchOrder.CustomField>();
            using (var dtCustomField = new COCDataset.OrderCustomFieldSummaryDataTable())
            {
                using (var taCustomField = new OrderCustomFieldSummaryTableAdapter())
                {
                    taCustomField.FillByOrder(dtCustomField, orderId);
                }

                foreach (var fieldRow in dtCustomField)
                {
                    fields.Add(new CertificateBatchOrder.CustomField(
                        fieldRow.CustomFieldID,
                        fieldRow.Name,
                        fieldRow.IsValueNull() ? null : fieldRow.Value,
                        fieldRow.DisplayOnCOC));
                }
            }

            // Load part-level custom fields
            var partFields = new List<CertificateBatchOrder.PartCustomField>();

            using (var taPartCustomFields = new COCPartCustomFieldsTableAdapter())
            {
                using (var dtPartCustomField = taPartCustomFields.GetByPart(partId))
                {
                    foreach (var fieldRow in dtPartCustomField)
                    {
                        partFields.Add(new CertificateBatchOrder.PartCustomField(
                            fieldRow.PartLevelCustomFieldID,
                            fieldRow.Name,
                            fieldRow.IsValueNull() ? null : fieldRow.Value,
                            fieldRow.DisplayOnCOC));
                    }
                }
            }

            // Load part mark
            CertificateBatchOrder.OrderPartMark partMark = null;

            using (var taOPM = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderPartMarkTableAdapter())
            {
                var orderPMTable = new OrderProcessingDataSet.OrderPartMarkDataTable();
                taOPM.Fill(orderPMTable, orderId);

                var orderPMRow = orderPMTable.FirstOrDefault();

                if (orderPMRow != null)
                {
                    partMark = new CertificateBatchOrder.OrderPartMark(
                        orderPMRow.IsProcessSpecNull() ? null : orderPMRow.ProcessSpec,
                        orderPMRow.IsPartMarkedDateNull() ? (DateTime?)null : orderPMRow.PartMarkedDate);
                }
            }

            // Load reworks
            var reworks = new List<CertificateBatchOrder.OrderRework>();
            using (var dtReworkSummary = new COCDataset.ReworkSummaryDataTable())
            {
                using (var taReworkSummary = new ReworkSummaryTableAdapter())
                {
                    taReworkSummary.FillByOrder(dtReworkSummary, orderId);
                }

                // Show full rework first - similar to normal COC
                var fullRework = dtReworkSummary
                    .Where(r => r.OriginalOrderID == orderId && r.IsReworkOrderIDNull());

                var splitRework = dtReworkSummary
                    .Where(r => !r.IsReworkOrderIDNull() && r.ReworkOrderID == orderId);

                foreach (var rework in fullRework.Concat(splitRework))
                {
                    reworks.Add(new CertificateBatchOrder.OrderRework(rework.Name, rework.ShowOnDocuments));
                }
            }

            // Load CustomerID
            int customerId;
            using (var taOrderSummary = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
            {
                customerId = taOrderSummary.GetCustomerId(orderId) ?? -1;
            }

            return new CertificateBatchOrder(orderId, customerId, partId,
                partName,
                batchQuantity,
                serialNumbers,
                importedPrice,
                GetOrderProcesses(orderId),
                fields,
                partFields,
                partMark,
                reworks);
        }

        private List<CertificateBatchOrder.OrderProcess> GetOrderProcesses(int orderId)
        {
            var dsCoc = new COCDataset { EnforceConstraints = false };
            using (var taOrderProcesses = new OrderProcessesTableAdapter())
            {
                taOrderProcesses.Fill(dsCoc.OrderProcesses, orderId);
            }

            using (var taProcess = new ProcessTableAdapter())
            {
                taProcess.Fill(dsCoc.Process, orderId);
            }

            using (var taProcessSteps = new ProcessStepsTableAdapter())
            {
                taProcessSteps.Fill(dsCoc.ProcessSteps, orderId);
            }

            using (var taProcessQuestion = new ProcessQuestionTableAdapter())
            {
                taProcessQuestion.Fill(dsCoc.ProcessQuestion, orderId);
            }

            using (var taOrderProcessAnswer = new OrderProcessAnswerTableAdapter())
            {
                taOrderProcessAnswer.Fill(dsCoc.OrderProcessAnswer, orderId);
            }

            using (var taPartInspection = new PartInspectionTableAdapter())
            {
                taPartInspection.FillByOrder(dsCoc.PartInspection, orderId);
            }

            using (var taProcessInspection = new ProcessInspectionsTableAdapter { ClearBeforeFill = false })
            {
                foreach (var process in dsCoc.Process)
                {
                    taProcessInspection.FillByProcess(dsCoc.ProcessInspections, process.ProcessID);
                }
            }

            using (var taPartInspectionAnswer = new PartInspectionAnswersTableAdapter { ClearBeforeFill = false })
            {
                foreach (var partInspectionRow in dsCoc.PartInspection)
                {
                    taPartInspectionAnswer.Fill(dsCoc.PartInspectionAnswers,
                        partInspectionRow.PartInspectionID);
                }
            }

            var processes = new List<CertificateBatchOrder.OrderProcess>();
            using (var taProcess = new ProcessTableAdapter())
            {
                foreach (var orderProcessRow in dsCoc.OrderProcesses.OrderBy(op => op.StepOrder))
                {
                    // Steps
                    var steps = new List<CertificateBatchOrder.OrderProcessStep>();

                    foreach (var stepRow in orderProcessRow.ProcessRow.GetProcessStepsRows().OrderBy(step => step.StepOrder))
                    {
                        var questions = new List<CertificateBatchOrder.OrderProcessQuestion>();

                        foreach (var questionRow in stepRow.GetProcessQuestionRows().OrderBy(question => question.StepOrder))
                        {
                            // There may be duplicate answer rows.
                            // Use one that has been completed without a null answer.
                            var answerRow = questionRow
                                .GetOrderProcessAnswerRows()
                                .FirstOrDefault(row => row.OrderProcessesID == orderProcessRow.OrderProcessesID
                                    && row.Completed
                                    && !row.IsAnswerNull());

                            questions.Add(new CertificateBatchOrder.OrderProcessQuestion(
                                questionRow.Name,
                                questionRow.Required,
                                Enum.TryParse<InputType>(questionRow.InputType, out var inputType)
                                    ? inputType
                                    : InputType.None,
                                questionRow.IsNumericUntisNull() ? null : questionRow.NumericUntis,
                                answerRow?.Answer));
                        }

                        steps.Add(new CertificateBatchOrder.OrderProcessStep(
                            stepRow.Name,
                            stepRow.COCData,
                            questions));
                    }

                    // Inspections
                    var inspections = new List<CertificateBatchOrder.PartInspection>();

                    foreach (var processInspectionRow in orderProcessRow.ProcessRow.GetProcessInspectionsRows())
                    {
                        var partInspectionRow = orderProcessRow.GetPartInspectionRows()
                            .FirstOrDefault(partInspection => partInspection.PartInspectionTypeID == processInspectionRow.PartInspectionTypeID);

                        if (partInspectionRow == null)
                        {
                            continue;
                        }

                        var questions = new List<CertificateBatchOrder.PartInpsectionQuestion>();

                        foreach (var questionRow in partInspectionRow.GetPartInspectionAnswersRows())
                        {
                            questions.Add(new CertificateBatchOrder.PartInpsectionQuestion(
                                questionRow.QuestionName,
                                questionRow.IsAnswerNull() ? null : questionRow.Answer));
                        }

                        inspections.Add(new CertificateBatchOrder.PartInspection(partInspectionRow.Name,
                            partInspectionRow.RejectedQty,
                            processInspectionRow.COCData,
                            questions));
                    }

                    // Alias Name
                    var aliasName = string.Empty;

                    if (AppSettings.DisplayCustomerProcessAliasOnCoc)
                    {
                        aliasName = taProcess.GetCustomerProcessAliasName(orderProcessRow.OrderProcessesID);
                    }

                    if (string.IsNullOrEmpty(aliasName))
                    {
                        aliasName = taProcess.GetProcessAliasName(orderProcessRow.OrderProcessesID);
                    }

                    processes.Add(new CertificateBatchOrder.OrderProcess(
                        orderProcessRow.OrderProcessesID,
                        orderProcessRow.ProcessID,
                        aliasName,
                        orderProcessRow.ProcessRow.IsDescriptionNull()
                            ? null
                            : orderProcessRow.ProcessRow.Description,
                        orderProcessRow.COCData,
                        steps,
                        inspections));
                }
            }

            return processes;
        }

        public void QueueBatchNotifications(int batchCocId, List<Contact> contacts)
        {
            if (contacts == null)
            {
                throw new ArgumentNullException(nameof(contacts));
            }

            using (var taNotification = new BatchCOCNotificationTableAdapter())
            {
                using (var dtNotification = new COCDataset.BatchCOCNotificationDataTable())
                {
                    foreach (var selectedContact in contacts)
                    {
                        var notificationRow = dtNotification.NewBatchCOCNotificationRow();
                        notificationRow.BatchCOCID = batchCocId;
                        notificationRow.ContactID = selectedContact.ContactId;
                        dtNotification.AddBatchCOCNotificationRow(notificationRow);
                    }

                    taNotification.Update(dtNotification);
                }
            }
        }

        public IEnumerable<Contact> GetContacts(int customerId)
        {
            var contacts = new List<Contact>();

            using (var dtContact = new COCDataset.d_ContactDataTable())
            {
                using (var taContact = new d_ContactTableAdapter { ClearBeforeFill = false })
                {
                    taContact.FillActiveByCustomer(dtContact, customerId);

                    if (AppSettings.AllowAdditionalCustomersForContacts)
                    {
                        taContact.FillSecondaryContactsByCustomer(dtContact, customerId);
                    }
                }

                foreach (var contactRow in dtContact)
                {
                    contacts.Add(new Contact(
                        contactRow.ContactID,
                        contactRow.IsEmailAddressNull() ? null : contactRow.EmailAddress,
                        contactRow.COCNotification,
                        contactRow.Active));
                }
            }

            return contacts;
        }

        public CertificateBatch GetBatch(int batchId)
        {
            int? salesOrderId = null;
            var batchProcesses = new List<CertificateBatch.BatchProcess>();

            using (var dsOrderProcessing = new OrderProcessingDataSet { EnforceConstraints = false })
            {
                using (var taBatch = new Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
                {
                    taBatch.FillBy(dsOrderProcessing.Batch, batchId);
                }
                using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter())
                {
                    ta.FillBy(dsOrderProcessing.BatchProcesses, batchId);
                }

                using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcess_OrderProcessTableAdapter())
                {
                    ta.FillByBatch(dsOrderProcessing.BatchProcess_OrderProcess, batchId);
                }

                foreach (var batchProcessRow in dsOrderProcessing.BatchProcesses.OrderBy(bp => bp.StepOrder))
                {
                    var orderProcessIds = batchProcessRow.GetBatchProcess_OrderProcessRows()
                        .Select(relation => relation.OrderProcessID)
                        .ToList();

                    batchProcesses.Add(new CertificateBatch.BatchProcess(
                        batchProcessRow.BatchProcessID,
                        batchProcessRow.ProcessID,
                        orderProcessIds));
                }

                var batchRow = dsOrderProcessing.Batch.FirstOrDefault();

                if (batchRow != null && !batchRow.IsSalesOrderIDNull())
                {
                    salesOrderId = batchRow.SalesOrderID;
                }
            }

            return new CertificateBatch(batchId, salesOrderId, batchProcesses);
        }

        public int SaveCertificate(BatchCertificate newBatchCoc)
        {
            if (newBatchCoc == null)
            {
                throw new ArgumentNullException(nameof(newBatchCoc));
            }

            if (newBatchCoc.QualityInspector == null)
            {
                throw new ArgumentException(
                    "View model must have quality inspector.",
                    nameof(newBatchCoc));
            }

            // Save new batch COC
            int batchCocId;
            using (var taManager = new Data.Datasets.COCDatasetTableAdapters.TableAdapterManager())
            {
                taManager.BatchCOCTableAdapter = new BatchCOCTableAdapter();
                taManager.BatchCOCOrderTableAdapter = new BatchCOCOrderTableAdapter();

                using (var dsCoc = new COCDataset())
                {
                    var batchCocRow = dsCoc.BatchCOC.NewBatchCOCRow();
                    batchCocRow.BatchID = newBatchCoc.Batch.BatchId;
                    batchCocRow.DateCertified = newBatchCoc.DateCertified;
                    batchCocRow.QAUser = newBatchCoc.QualityInspector.UserID;
                    batchCocRow.COCInfo = ZipUtilities.CompressString(newBatchCoc.InfoHtml);
                    batchCocRow.IsCompressed = true;
                    dsCoc.BatchCOC.AddBatchCOCRow(batchCocRow);

                    foreach (var order in newBatchCoc.Orders)
                    {
                        var batchCocOrderRow = dsCoc.BatchCOCOrder.NewBatchCOCOrderRow();
                        batchCocOrderRow.BatchCOCRow = batchCocRow;
                        batchCocOrderRow.OrderID = order.OrderId;
                        dsCoc.BatchCOCOrder.AddBatchCOCOrderRow(batchCocOrderRow);
                    }

                    taManager.UpdateAll(dsCoc);
                    batchCocId = batchCocRow.BatchCOCID;
                }
            }

            // Assumption - this is not a revised batch cert
            // Move orders to Shipping and add order history note
            using (var taOrderSummary = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
            {
                using (var taOrderHistory = new OrderHistoryTableAdapter())
                {
                    foreach (var order in newBatchCoc.Orders)
                    {
                        taOrderSummary.UpdateWorkStatus(AppSettings.WorkStatusShipping, order.OrderId);
                        taOrderSummary.UpdateOrderLocation(AppSettings.DepartmentShipping, order.OrderId);

                        taOrderHistory.UpdateOrderHistory(
                            order.OrderId,
                            "Final Inspection",
                            $"COC created for Batch {newBatchCoc.Batch.BatchId}; order moved to {AppSettings.DepartmentShipping}.",
                            newBatchCoc.QualityInspector.UserName);
                    }
                }
            }

            return batchCocId;
        }

        public string GetTemplate()
        {
            using (var taTemplates = new TemplatesTableAdapter())
            {
                var templateRow = taTemplates
                    .GetDataById("COCData")
                    .FirstOrDefault();

                if (templateRow != null)
                {
                    return templateRow.Template;
                }
            }

            return null;
        }

        public BatchCertificate GetBatchCoc(int batchCocId)
        {
            using (var dsCoc = new COCDataset { EnforceConstraints = false })
            {
                using (var taBatchCoc = new BatchCOCTableAdapter())
                {
                    taBatchCoc.FillByBatchCoc(dsCoc.BatchCOC, batchCocId);
                }

                using (var taBatchCocOrder = new BatchCOCOrderTableAdapter())
                {
                    taBatchCocOrder.FillByBatchCoc(dsCoc.BatchCOCOrder, batchCocId);
                }

                using (var taUser = new UsersTableAdapter())
                {
                    int userId = dsCoc.BatchCOC.FirstOrDefault()?.QAUser ?? -1;
                    taUser.FillByUser(dsCoc.Users, userId);
                }

                var batchCocRow = dsCoc.BatchCOC.FirstOrDefault();
                var userRow = dsCoc.Users.FirstOrDefault();

                if (batchCocRow == null || userRow == null)
                {
                    return null;
                }

                var orders = batchCocRow.GetBatchCOCOrderRows()
                    .Select(batchCocOrderRow => GetOrder(batchCocOrderRow.OrderID, batchCocRow.BatchID))
                    .ToList();

                return new BatchCertificate
                {
                    Batch = GetBatch(batchCocRow.BatchID),
                    BatchCocId = batchCocId,
                    DateCertified = batchCocRow.DateCertified,
                    InfoHtml = batchCocRow.IsCompressed
                        ? batchCocRow.COCInfo.DecompressString()
                        : batchCocRow.COCInfo,
                    Orders = orders,
                    QualityInspector = new User(userRow.UserID, userRow.Name)
                };
            }
        }

        #endregion
    }
}
