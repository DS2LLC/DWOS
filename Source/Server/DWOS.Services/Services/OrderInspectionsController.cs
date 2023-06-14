using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Services.Messages;
using NLog;
using System;
using System.Collections.Generic;
using DWOS.Data.Order.Activity;
using System.Web.Http;

namespace DWOS.Services
{
    public class OrderInspectionsController : ApiController
    {
        #region Fields
        
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        [HttpGet]
        [ServiceExceptionFilter("Error getting inspection for order.")]
        public ResponseBase GetNextByOrder(int orderId)
        {
            var info = Create(orderId);
            return new InspectionResponse { Success = true, ErrorMessage = null, Inspection = info };
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "ControlInspection")]
        [ServiceExceptionFilter("Error saving answers.")]
        public ResponseBase Save(InspectionSaveAnswerRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            return SaveInspection(request.OrderInspection);
        }

        #endregion

        #region Factories
        
        private static InspectionInfo Create(int orderId)
        {
            const int placeholderUserId = 1; // UserID is only used when completing inspection
            PartInspectionDataSet partInspectionDataSet = null;
            DocumentsDataSet.DocumentInfoDataTable documentInfoTable = null;
            Data.Datasets.DocumentsDataSetTableAdapters.DocumentInfoTableAdapter taDocumentInfo = null;

            try
            {
                _log.Debug("Creating inspection info for order {0}.", orderId);

                var activity = new ControlInspectionActivity(orderId, placeholderUserId);
                activity.Initialize();

                if (!activity.PartInspectionTypeID.HasValue)
                    return null;

                partInspectionDataSet = new PartInspectionDataSet() { EnforceConstraints = false };
                documentInfoTable = new DocumentsDataSet.DocumentInfoDataTable();
                taDocumentInfo = new Data.Datasets.DocumentsDataSetTableAdapters.DocumentInfoTableAdapter();

                using (var ta = new Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionTypeTableAdapter())
                {
                    ta.FillBy(partInspectionDataSet.PartInspectionType, activity.PartInspectionTypeID.Value);
                }

                using (var ta = new Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionQuestionTableAdapter())
                {
                    ta.FillBy(partInspectionDataSet.PartInspectionQuestion, activity.PartInspectionTypeID.Value);
                }

                using (var taCondition = new Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionQuestionConditionTableAdapter())
                {
                    taCondition.FillByInspection(partInspectionDataSet.PartInspectionQuestionCondition, activity.PartInspectionTypeID.Value);
                }

                var inspection = partInspectionDataSet.PartInspectionType.FindByPartInspectionTypeID(activity.PartInspectionTypeID.Value);

                if(inspection == null)
                    return null;

                var inspectionInfo = new InspectionInfo();
                inspectionInfo.InspectionId = inspection.PartInspectionTypeID;
                inspectionInfo.Name = inspection.Name;
                inspectionInfo.TestReference = inspection.IsTestReferenceNull() ? null : inspection.TestReference;
                inspectionInfo.TestRequirements = inspection.IsTestRequirementsNull() ? null : inspection.TestRequirements;
                inspectionInfo.InspectionQuestions = new List<InspectionQuestionInfo>();
                inspectionInfo.Lists = new List <ListInfo>();

                foreach(var questionRow in inspection.GetPartInspectionQuestionRows())
                {
                    var question = new InspectionQuestionInfo();

                    question.InspectionQuestionID = questionRow.PartInspectionQuestionID;
                    question.InspectionId = questionRow.PartInspectionTypeID;
                    question.Name = questionRow.Name;
                    question.StepOrder = questionRow.StepOrder;

                    //this is a temp work around to handle the new Sampleset question that is not extended to mobile yet.
                    //sampleset questions behave like decimal with manual entry.
                    question.InputType = (questionRow.InputType == "SampleSet")?"Decimal": questionRow.InputType;

                    question.MinValue = questionRow.IsMinValueNull() ? null : questionRow.MinValue;
                    question.MaxValue = questionRow.IsMaxValueNull() ? null : questionRow.MaxValue;
                    question.ListID = questionRow.IsListIDNull() ? -1 : questionRow.ListID;
                    question.DefaultValue = questionRow.IsDefaultValueNull() ? null : questionRow.DefaultValue;
                    question.NumericUnits = questionRow.IsNumericUntisNull() ? null : questionRow.NumericUntis;

                    // Conditions
                    question.Conditions = new List<InspectionQuestionCondition>();

                    foreach (var conditionRow in questionRow.GetMainConditionRows())
                    {
                        question.Conditions.Add(new InspectionQuestionCondition
                        {
                            CheckInspectionQuestionId = conditionRow.CheckPartInspectionQuestionID,
                            Operator = conditionRow.Operator,
                            Value = conditionRow.Value
                        });
                    }

                    //if has a list and has not been addded yet then add to list of list values
                    if(question.ListID > 0 && !inspectionInfo.Lists.Exists(li => li.ListId == question.ListID))
                    {
                        var listValues = new DWOS.Data.Datasets.ListsDataSet.ListValuesDataTable();

                        using(var ta = new Data.Datasets.ListsDataSetTableAdapters.ListValuesTableAdapter())
                            ta.FillBy(listValues, question.ListID);

                        var listInfo = new ListInfo() {ListId = question.ListID, Values = new List <string>()};

                        foreach(var listValue in listValues)
                            listInfo.Values.Add(listValue.Value);

                        inspectionInfo.Lists.Add(listInfo);
                    }

                    inspectionInfo.InspectionQuestions.Add(question);
                }

                using (var inspectionDocTable = new DocumentsDataSet.DocumentLinkDataTable())
                {
                    using (var taInspectionDoc = new Data.Datasets.DocumentsDataSetTableAdapters.DocumentLinkTableAdapter())
                    {
                        taInspectionDoc.FillBy(inspectionDocTable,
                            "ControlInspection",
                            inspection.PartInspectionTypeID);
                    }

                    if (inspectionDocTable.Count > 0)
                    {
                        inspectionInfo.Documents = new List<DocumentInfo>();

                        foreach (var inspectionDocRow in inspectionDocTable)
                        {
                            taDocumentInfo.FillByID(documentInfoTable, inspectionDocRow.DocumentInfoID);
                            var document = documentInfoTable.FindByDocumentInfoID(inspectionDocRow.DocumentInfoID);
                            var currentRevisionId = (taDocumentInfo.GetCurrentRevisionID(inspectionDocRow.DocumentInfoID) as int?);
                            if (currentRevisionId.HasValue)
                            {
                                inspectionInfo.Documents.Add(new DocumentInfo()
                                {
                                    CurrentRevisionId = currentRevisionId.Value,
                                    DocumentInfoID = inspectionDocRow.DocumentInfoID,
                                    DocumentType = "ControlInspection",
                                    MediaType = document.MediaType,
                                    Name = document.Name
                                });
                            }
                        }
                    }
                }

                _log.Debug("Inspection Info details {0}.", inspectionInfo);
                return inspectionInfo;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting inspection info for order id " + orderId);
                return null;
            }
            finally
            {
                partInspectionDataSet?.Dispose();
                documentInfoTable?.Dispose();
                taDocumentInfo?.Dispose();
            }
        }

        private static InspectionSaveAnswerResponse SaveInspection(OrderInspectionInfo inspectionInfo)
        {
            if(inspectionInfo == null)
                    return new InspectionSaveAnswerResponse() {Success = false, ErrorMessage = "No inspection."};
            
            var orderId = inspectionInfo.OrderID;
            
            try
            {
                _log.Debug("Saving order inspection for order {0}.", inspectionInfo.OrderID);

                var activity = new ControlInspectionActivity(orderId, inspectionInfo.UserID);
                activity.Initialize();

                if (!activity.PartInspectionTypeID.HasValue)
                {
                    return new InspectionSaveAnswerResponse() { Success = false, ErrorMessage = "No inspection for this order." };
                }

                //Save all the answers to DB
                var dsInspections = new Data.Datasets.PartInspectionDataSet() {EnforceConstraints = false};
                PartInspectionDataSet.PartInspectionRow currentInspection = null;

                var taManager = new Data.Datasets.PartInspectionDataSetTableAdapters.TableAdapterManager();
                taManager.PartInspectionTableAdapter = new Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionTableAdapter();
                taManager.PartInspectionAnswerTableAdapter = new Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionAnswerTableAdapter();

                //if inspection exists
                if(activity.PartInspectionID > 0)
                {
                    taManager.PartInspectionTableAdapter.FillByInspection(dsInspections.PartInspection, activity.PartInspectionID);
                    taManager.PartInspectionAnswerTableAdapter.FillByPartInspection(dsInspections.PartInspectionAnswer, activity.PartInspectionID);

                    currentInspection = dsInspections.PartInspection.FindByPartInspectionID(activity.PartInspectionID);
                }

                //create new inspection
                if(currentInspection == null)
                {
                    currentInspection                      = dsInspections.PartInspection.NewPartInspectionRow();
                    currentInspection.OrderID              = activity.OrderID;
                    currentInspection.PartQuantity         = activity.PartQuantity;
                    currentInspection.OrderProcessID       = activity.OrderProcessID.GetValueOrDefault();
                    currentInspection.PartInspectionTypeID = activity.PartInspectionTypeID.Value;
                }
            
                currentInspection.Status         = inspectionInfo.Status;
                currentInspection.QAUserID       = inspectionInfo.UserID;
                currentInspection.AcceptedQty    = inspectionInfo.AcceptedQty;
                currentInspection.RejectedQty    = inspectionInfo.RejectedQty;
                currentInspection.Notes          = inspectionInfo.Notes;
                currentInspection.InspectionDate = inspectionInfo.InspectionDate;

                if(currentInspection.RowState == System.Data.DataRowState.Detached)
                    dsInspections.PartInspection.AddPartInspectionRow(currentInspection);

                foreach(var answerInfo in inspectionInfo.InspectionAnswers)
                {
                    //find answer or create it
                    var answerRow = dsInspections.PartInspectionAnswer.FirstOrDefault(w => w.PartInspectionQuestionID == answerInfo.InspectionQuestionID) ?? dsInspections.PartInspectionAnswer.NewPartInspectionAnswerRow();

                    answerRow.Answer = answerInfo.Answer;
                    answerRow.Completed = answerInfo.Completed;

                    if(answerInfo.Completed && answerInfo.CompletedBy > 0)
                        answerRow.CompletedBy = answerInfo.CompletedBy;

                    if(answerInfo.Completed && answerInfo.CompletedDate > DateTime.MinValue)
                        answerRow.CompletedData = answerInfo.CompletedDate;

                    answerRow.PartInspectionID = currentInspection.PartInspectionID;
                    answerRow.PartInspectionQuestionID = answerInfo.InspectionQuestionID;

                    if(answerRow.RowState == System.Data.DataRowState.Detached)
                        dsInspections.PartInspectionAnswer.Rows.Add(answerRow);
                }

                // Set information related to rework
                activity.FailQuantity = currentInspection.RejectedQty;
                activity.PassedInspection = inspectionInfo.Status;
                // User does not set rework type through mobile app - done after inspection.

                //Complete the activity
                var results = activity.Complete() as ControlInspectionActivity.ControlInspectionActivityResults;

                if (results == null)
                    return new InspectionSaveAnswerResponse() { Success = false, ErrorMessage = "No results." };

                _log.Debug("Order Inspection saved for order {0} with results: {1}.", inspectionInfo.OrderID, results);

                //update inspection and answers to DB
                taManager.UpdateAll(dsInspections);

                var response = new InspectionSaveAnswerResponse() { Success = true };
                
                return response;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error saving answers for order {0}.".FormatWith(orderId));
                return new InspectionSaveAnswerResponse() { Success = true, ErrorMessage = "Error saving answers for order {0}.".FormatWith(orderId) + exc.Message };
            }
        }

        #endregion
    }
}