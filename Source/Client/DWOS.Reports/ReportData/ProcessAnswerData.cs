using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Reports.ProcessPartsReportTableAdapters;

namespace DWOS.Reports.ReportData
{
    /// <summary>
    /// Represents all data in <see cref="ProcessAnswerReport"/>.
    /// </summary>
    internal class ProcessAnswerData
    {
        /// <summary>
        /// Gets the Process IDs for this instance.
        /// </summary>
        public List<int> ProcessIds { get; private set; }

        public List<OrderData> Orders { get; private set; }

        /// <summary>
        /// Gets groups of columns for this instance.
        /// </summary>
        public List<ReportColumnGroup> ColumnGroups { get; private set; }

        /// <summary>
        /// Gets the process name for this instance.
        /// </summary>
        public List<string> ProcessNames { get; private set; }

        /// <summary>
        /// Generates new report data.
        /// </summary>
        /// <param name="processIds">Process IDs for the report.</param>
        /// <param name="customerIds">Customer IDs for the report.</param>
        /// <param name="fromDate">Start date.</param>
        /// <param name="toDate">End date.</param>
        /// <returns></returns>
        public static ProcessAnswerData From(List<int> processIds, List<int> customerIds, DateTime fromDate, DateTime toDate)
        {
            using (var dtOrderSummary = new Data.Reports.ProcessPartsReport.OrderProcessSummaryDataTable())
            {
                // Orders
                using (var taOrders = new OrderProcessSummaryTableAdapter() { ClearBeforeFill = false })
                {
                    foreach (var processId in processIds)
                    {
                        taOrders.FillByDate(dtOrderSummary, processId, toDate, fromDate);
                    }
                }

                // Process data
                var processNames = new List<string>();

                if (dtOrderSummary.Count > 0)
                {
                    processNames = dtOrderSummary
                        .Select(o => o.ProcessName)
                        .Distinct()
                        .OrderBy(name => name)
                        .ToList();
                }

                var reportData = new ProcessAnswerData
                {
                    ProcessNames = processNames,
                    ProcessIds = new List<int>(processIds),
                    ColumnGroups = ReportColumnGroup.GroupsFrom(processIds)
                };

                Func<Data.Reports.ProcessPartsReport.OrderProcessSummaryRow, bool> customerSelector;

                if (customerIds == null || customerIds.Count == 0)
                {
                    customerSelector = (row) => true;
                }
                else
                {
                    customerSelector = (row) => customerIds.Contains(row.CustomerID);
                }

                reportData.Orders = dtOrderSummary
                    .Where(customerSelector)
                    .Select(o => OrderData.From(reportData, o))
                    .ToList();

                return reportData;
            }
        }


        #region OrderData

        /// <summary>
        /// Represents an order in <see cref="ProcessAnswerReport"/>.
        /// </summary>
        internal class OrderData
        {
            #region Properties

            public int OrderId { get; private set; }

            public string CustomerName { get; private set; }

            public string PurchaseOrder { get; private set; }

            public string Status { get; private set; }

            public string PartName { get; private set; }

            public string ProcessName { get; private set; }

            public string ProcessRev { get; private set; }

            public string ProcessAliasName { get; private set; }

            public DateTime? CompletedDate { get; private set; }

            public string CurrentLocation { get; private set; }

            public List<string> SerialNumbers { get; private set; }

            /// <summary>
            /// Gets a list of answer values grouped by an identifier.
            /// </summary>
            /// <remarks>
            /// The dictionary for each group maps column ID to response.
            /// </remarks>
            public List<Dictionary<int, string>> AnswerColumns { get; private set; }

            #endregion

            #region Methods

            /// <summary>
            /// Generates a single instance from a single order row.
            /// </summary>
            /// <param name="reportData"></param>
            /// <param name="orderProcess"></param>
            /// <returns></returns>
            public static OrderData From(ProcessAnswerData reportData, Data.Reports.ProcessPartsReport.OrderProcessSummaryRow orderProcess)
            {
                if (reportData == null || orderProcess == null)
                {
                    return null;
                }

                // Serial Numbers
                var serialNumbers = new List<string>();

                using (var dtSerialNumber = new Data.Reports.ProcessPartsReport.OrderSerialNumberDataTable())
                {
                    using (var taOrderSerialNumbers = new OrderSerialNumberTableAdapter())
                    {
                        taOrderSerialNumbers.ClearBeforeFill = true;
                        taOrderSerialNumbers.FillActive(dtSerialNumber, orderProcess.OrderID);
                    }

                    serialNumbers.AddRange(dtSerialNumber
                        .Where(serialNumberRow => !serialNumberRow.IsNumberNull())
                        .Select(serialNumberRow => serialNumberRow.Number));
                }

                // Answers
                var answerColumns = GetAnswerColumns(reportData, orderProcess);

                return new OrderData
                {
                    OrderId = orderProcess.OrderID,
                    CustomerName = orderProcess.CustomerName,
                    PurchaseOrder = orderProcess.IsPurchaseOrderNull() ? "NA" : orderProcess.PurchaseOrder,
                    Status = orderProcess.IsStatusNull() ? "NA" : orderProcess.Status,
                    PartName = orderProcess.PartName,
                    ProcessName = orderProcess.ProcessName,
                    ProcessRev = orderProcess.IsProcessRevNull() ? "NA" : orderProcess.ProcessRev,
                    ProcessAliasName = orderProcess.ProcessAliasName,
                    CompletedDate = orderProcess.IsCompletedDateNull() ? (DateTime?)null : orderProcess.CompletedDate,
                    CurrentLocation = orderProcess.CurrentLocation,
                    SerialNumbers = serialNumbers,
                    AnswerColumns = answerColumns
                };
            }

            private static List<Dictionary<int, string>> GetAnswerColumns(ProcessAnswerData reportData, Data.Reports.ProcessPartsReport.OrderProcessSummaryRow orderProcess)
            {
                var groupAnswers = new List<Dictionary<int, string>>();

                using (var dtAnswer = new Data.Reports.ProcessPartsReport.ProcessAnswerSummaryDataTable())
                {
                    // Retrieve all answers for the order process
                    using (var taAnswer = new ProcessAnswerSummaryTableAdapter())
                    {
                        taAnswer.FillBy(dtAnswer, orderProcess.OrderProcessesID);
                    }

                    // Add groups with identifiers
                    foreach (var columnGroup in reportData.ColumnGroups.Where(cg => cg.Identifier != null))
                    {
                        // Get steps with the same identifier
                        var groupedIdentifiers = dtAnswer
                            .Where(a => columnGroup.Identifier.ProcessQuestionIds.Contains(a.ProcessQuestionID))
                            .GroupBy(a => a.IsAnswerNull() ? string.Empty : a.Answer)
                            .OrderBy(a => a.Key)
                            .ToList();

                        foreach (var identityGroup in groupedIdentifiers)
                        {
                            var answerColumns = new Dictionary<int, string>
                            {
                                { columnGroup.Identifier.ColumnId, identityGroup.Key }
                            };

                            var stepIds = identityGroup.Select(g => g.ProcessStepID).ToList();

                            foreach (var column in columnGroup.QuestionColumns)
                            {
                                var answers = dtAnswer
                                    .Where(a => !a.IsAnswerNull() && stepIds.Contains(a.ProcessStepID) && column.ProcessQuestionIds.Contains(a.ProcessQuestionID));

                                if (column.CanAddAnswers)
                                {
                                    var total = answers.Sum(a => decimal.TryParse(a.Answer, out decimal val) ? val : 0M);
                                    answerColumns.Add(column.ColumnId, total.ToString(System.Globalization.CultureInfo.InvariantCulture));
                                }
                                else
                                {
                                    var uniqueAnswers = answers.Select(a => a.Answer).Distinct().OrderBy(a => a);
                                    answerColumns.Add(column.ColumnId, string.Join(", ", uniqueAnswers));
                                }
                            }

                            groupAnswers.Add(answerColumns);
                        }
                    }

                    // Add groups without identifiers.
                    // These are grouped by Step ID - each step should have its
                    // own row in the report.
                    var groupsWithoutIdentifiers = reportData.ColumnGroups
                        .Where(cg => cg.Identifier == null)
                        .ToList();

                    foreach (var group in groupsWithoutIdentifiers)
                    {
                        // Get questions for each step
                        var steps = group.QuestionColumns
                            .SelectMany(c => c.Questions)
                            .GroupBy(c => c.ProcessStepId);

                        foreach (var stepQuestions in steps)
                        {
                            var answerColumns = new Dictionary<int, string>();

                            foreach (var column in group.QuestionColumns)
                            {
                                // Get question IDs that belong to the question and the step.
                                var questionIds = column.ProcessQuestionIds
                                    .Intersect(stepQuestions.Select(q => q.ProcessQuestionId))
                                    .ToList();

                                var answer = dtAnswer
                                    .FirstOrDefault(a => !a.IsAnswerNull() && questionIds.Contains(a.ProcessQuestionID));

                                if  (answer != null)
                                {
                                    answerColumns.Add(column.ColumnId, answer.IsAnswerNull() ? null : answer.Answer);
                                }
                            }

                            if (answerColumns.Count > 0)
                            {
                                groupAnswers.Add(answerColumns);
                            }
                        }
                    }
                }

                return groupAnswers;
            }

            #endregion
        }

        #endregion

        #region ReportColumnGroup

        /// <summary>
        /// Represents a group of process question columns in <see cref="ProcessAnswerReport"/>.
        /// </summary>
        internal class ReportColumnGroup
        {
            #region Properties

            /// <summary>
            /// Gets the column that identifies the group.
            /// </summary>
            /// <remarks>
            /// The user selects identifying columns when running the report.
            /// </remarks>
            public ReportColumn Identifier { get; private set; }

            /// <summary>
            /// Gets a list of columns generated from process questions.
            /// </summary>
            public List<ReportColumn> QuestionColumns { get; private set; }

            /// <summary>
            /// Gets a list of all columns in the group.
            /// </summary>
            public IEnumerable<ReportColumn> AllColumns
            {
                get
                {
                    var list = new List<ReportColumn>();

                    if (Identifier != null)
                    {
                        list.Add(Identifier);
                    }

                    if (QuestionColumns != null)
                    {
                        list.AddRange(QuestionColumns);
                    }

                    return list;
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Generates a list of all column groups for a process.
            /// </summary>
            /// <param name="processIds"></param>
            /// <returns></returns>
            public static List<ReportColumnGroup> GroupsFrom(List<int> processIds)
            {
                var dtQuestionSummary = new Data.Reports.ProcessPartsReport.ProcessQuestionSummaryDataTable();

                try
                {
                    using (var taQuestionSummary = new ProcessQuestionSummaryTableAdapter { ClearBeforeFill = false })
                    {
                        foreach (var processId in processIds)
                        {
                            taQuestionSummary.FillByProcess(dtQuestionSummary, processId);
                        }
                    }

                    var columnGroups = new List<ReportColumnGroup>();

                    // Combine groups that are identified by a question of the same name.
                    var identifyingQuestions = dtQuestionSummary
                        .Where(q => q.IdentifiesProcessGroup)
                        .ToList();

                    var groupsByName = identifyingQuestions
                        .OrderBy(q => q.StepOrder)
                        .GroupBy(q => q.QuestionName.ToUpperInvariant());

                    var columnId = 1;
                    foreach (var groupsWithSameName in groupsByName)
                    {
                        var firstQuestionRow = groupsWithSameName.First();
                        var groupInputType = (InputType)Enum.Parse(typeof(InputType), firstQuestionRow.InputType);

                        // Initialize group & add identity column

                        var columnForGroups = new ReportColumnGroup
                        {
                            Identifier = new ReportColumn(groupInputType)
                            {
                                Name = firstQuestionRow.QuestionName,
                                ColumnId = columnId,
                                Questions = groupsWithSameName
                                    .Select(g => new ReportColumnQuestion(g.ProcessStepID, g.ProcessQuestionID))
                                    .ToList()
                            },

                            QuestionColumns = new List<ReportColumn>(),
                        };

                        ++columnId;

                        columnGroups.Add(columnForGroups);

                        var stepsInGroup = groupsWithSameName.Select(s => s.ProcessStepID).ToList();

                        // Add columns for each applicable question
                        var stepQuestions = dtQuestionSummary
                            .Where(q => !q.IdentifiesProcessGroup && q.IncludeInProcessGroup && stepsInGroup.Contains(q.ProcessStepID))
                            .OrderBy(q => q.StepOrder)
                            .ThenBy(q => q.QuestionStepOrder);

                        foreach (var questionRow in stepQuestions)
                        {
                            var matchingColumn = columnForGroups.QuestionColumns
                                .FirstOrDefault(q => string.Equals(q.Name, questionRow.QuestionName, StringComparison.InvariantCultureIgnoreCase));

                            if (matchingColumn == null)
                            {
                                var questionInputType = (InputType)Enum.Parse(typeof(InputType), questionRow.InputType);

                                columnForGroups.QuestionColumns.Add(new ReportColumn(questionInputType)
                                {
                                    ColumnId = columnId,
                                    Name = questionRow.QuestionName,
                                    Questions = new List<ReportColumnQuestion>
                                    {
                                        new ReportColumnQuestion(questionRow.ProcessStepID, questionRow.ProcessQuestionID)
                                    }
                                });

                                ++columnId;
                            }
                            else
                            {
                                // Group exists - combine them
                                matchingColumn.Questions.Add(
                                    new ReportColumnQuestion(questionRow.ProcessStepID, questionRow.ProcessQuestionID));
                            }
                        }
                    }

                    // Combine groups without an identifying question
                    var addedQuestionIds = new HashSet<int>(columnGroups
                        .SelectMany(group => group.AllColumns)
                        .SelectMany(column => column.ProcessQuestionIds));

                    var questionsWithoutIdentifier = dtQuestionSummary
                        .Where(q => q.IncludeInProcessGroup && !addedQuestionIds.Contains(q.ProcessQuestionID))
                        .OrderBy(q => q.StepOrder)
                        .ThenBy(q => q.QuestionStepOrder)
                        .ToList();

                    if (questionsWithoutIdentifier.Count > 0)
                    {
                        var groupWithoutIdentifier = new ReportColumnGroup
                        {
                            QuestionColumns = new List<ReportColumn>()
                        };

                        ++columnId;
                        columnGroups.Add(groupWithoutIdentifier);

                        foreach (var questionRow in questionsWithoutIdentifier)
                        {
                            var matchingColumn = groupWithoutIdentifier.QuestionColumns
                                .FirstOrDefault(q => string.Equals(q.Name, questionRow.QuestionName, StringComparison.InvariantCultureIgnoreCase));

                            if (matchingColumn == null)
                            {
                                var questionInputType = (InputType)Enum.Parse(typeof(InputType), questionRow.InputType);

                                groupWithoutIdentifier.QuestionColumns.Add(new ReportColumn(questionInputType)
                                {
                                    ColumnId = columnId,
                                    Name = questionRow.QuestionName,
                                    Questions = new List<ReportColumnQuestion>
                                    {
                                        new ReportColumnQuestion(questionRow.ProcessStepID, questionRow.ProcessQuestionID)
                                    }
                                });

                                ++columnId;
                            }
                            else
                            {
                                // Group exists - combine them
                                matchingColumn.Questions.Add(
                                    new ReportColumnQuestion(questionRow.ProcessStepID, questionRow.ProcessQuestionID));

                            }
                        }
                    }

                    return columnGroups;
                }
                finally
                {
                    dtQuestionSummary?.Dispose();
                }
            }

            #endregion
        }

        #endregion

        #region ReportColumn

        /// <summary>
        /// Represents a single process question column in <see cref="ProcessAnswerReport"/>.
        /// </summary>
        internal class ReportColumn
        {
            #region Properties

            /// <summary>
            /// Gets an identifier for this instance.
            /// </summary>
            /// <remarks>
            /// This is not a database identifier; it only identifies the column within the report.
            /// </remarks>
            public int ColumnId { get; set; }

            public string Name { get; set; }

            public List<ReportColumnQuestion> Questions { get; set; }

            public IEnumerable<int> ProcessQuestionIds => Questions?.Select(q => q.ProcessQuestionId);

            public bool CanAddAnswers => InputType == InputType.DecimalDifference
                        || InputType == InputType.Decimal
                        || InputType == InputType.Integer
                        || InputType == InputType.TimeDuration;

            public InputType InputType { get; }

            #endregion

            #region Methods

            public ReportColumn(InputType inputType)
            {
                InputType = inputType;
            }

            #endregion
        }

        #endregion

        #region ReportColumnQuestion

        /// <summary>
        /// Represents a question in <see cref="ReportColumn"/>.
        /// </summary>
        internal class ReportColumnQuestion
        {
            #region Properties

            public int ProcessStepId { get; }

            public int ProcessQuestionId { get; }

            #endregion

            #region Methods

            public ReportColumnQuestion(int processStepId, int processQuestionId)
            {
                ProcessStepId = processStepId;
                ProcessQuestionId = processQuestionId;
            }

            #endregion
        }

        #endregion
    }
}
