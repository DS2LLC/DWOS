using DWOS.Data.Datasets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Data.Process
{
    /// <summary>
    /// Represents process question information for import/export.
    /// </summary>
    public class ProcessQuestionExport
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name for this process question.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the notes for this process question.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the input type for this process question.
        /// </summary>
        public string InputType { get; set; }

        /// <summary>
        /// Gets or sets the minimum value for this process question.
        /// </summary>
        public string MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value for this process question.
        /// </summary>
        public string MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the tolerance for this process question.
        /// </summary>
        public string Tolerance { get; set; }

        /// <summary>
        /// Gets or sets the default value for this process question.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the numeric units for this process question.
        /// </summary>
        /// <remarks>
        /// This is misspelled for backwards-compatibility.
        /// </remarks>
        public string NumericUntis { get; set; }

        /// <summary>
        /// Gets or sets the step order for this process question.
        /// </summary>
        public decimal StepOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if this process question can be
        /// edited by operators.
        /// </summary>
        /// <value>
        /// <c>true</c> if it is editable; otherwise, <c>false</c>.
        /// </value>
        public bool OperatorEditable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if this process question must have
        /// a response.
        /// </summary>
        /// <value>
        /// <c>true</c> if it is required; otherwise, <c>false</c>.
        /// </value>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the process question identifier original value.
        /// </summary>
        /// <remarks>
        /// Used to match back up with step conditions.
        /// </remarks>
        public int ProcessQuestionIdOriginal { get; set; }

        /// <summary>
        /// Gets or sets the list ID for this process question
        /// </summary>
        /// <remarks>
        /// Import substitutes this value for a list ID for the system.
        /// </remarks>
        public int ListID { get; set; }

        /// <summary>
        /// Gets or sets the name of the list for this process question.
        /// </summary>
        public string ListName { get; set; }

        /// <summary>
        /// Gets or sets a collection of values of the list for this
        /// process question.
        /// </summary>
        public List<string> ListValues { get; set; }

        /// <summary>
        /// Gets or sets a collection of process question fields for this
        /// process question.
        /// </summary>
        public List<ProcessQuestionFieldExport> ProcessQuestionFields { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ProcessQuestionExport"/> class.
        /// </summary>
        public ProcessQuestionExport() { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ProcessQuestionExport"/> class.
        /// </summary>
        /// <param name="processQuestion"></param>
        public ProcessQuestionExport(Data.Datasets.ProcessesDataset.ProcessQuestionRow processQuestion)
        {
            this.Name = processQuestion.Name;
            this.Notes = processQuestion.IsNotesNull() ? null : processQuestion.Notes;
            this.InputType = processQuestion.InputType;
            this.MinValue = processQuestion.IsMinValueNull() ? null : processQuestion.MinValue;
            this.MaxValue = processQuestion.IsMaxValueNull() ? null : processQuestion.MaxValue;
            this.Tolerance = processQuestion.IsToleranceNull() ? null : processQuestion.Tolerance;
            this.ListID = processQuestion.IsListIDNull() ? 0 : processQuestion.ListID;
            this.DefaultValue = processQuestion.IsDefaultValueNull() ? null : processQuestion.DefaultValue;
            this.NumericUntis = processQuestion.IsNumericUntisNull() ? null : processQuestion.NumericUntis;
            this.StepOrder = processQuestion.StepOrder;
            this.OperatorEditable = processQuestion.OperatorEditable;
            this.Required = processQuestion.Required;
            this.ProcessQuestionIdOriginal = processQuestion.ProcessQuestionID;

            //Copy List Values in order to recreate the list
            var list = processQuestion.ListsRow;

            if (list != null)
            {
                ListName = list.Name;

                if (list.GetListValuesRows().Length == 0)
                {
                    using (var ta = new Data.Datasets.ProcessesDatasetTableAdapters.ListValuesTableAdapter() { ClearBeforeFill = false })
                        ta.FillByListId(((Data.Datasets.ProcessesDataset)processQuestion.Table.DataSet).ListValues, list.ListID);
                }

                ListValues = list.GetListValuesRows().Convert(lv => lv.Value);
            }

            ProcessQuestionFields = processQuestion
                .GetProcessQuestionFieldRows()
                .Select(f => new ProcessQuestionFieldExport(f))
                .ToList();
        }

        /// <summary>
        /// Creates a row for this process question.
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="processStep"></param>
        /// <returns></returns>
        public ProcessesDataset.ProcessQuestionRow CreateRow(ProcessImporter importer, ProcessesDataset.ProcessStepsRow processStep)
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                importer.Issues.Add("Cannot import process question: Name was not provided");
                return null;
            }
            else if (string.IsNullOrEmpty(this.InputType))
            {
                importer.Issues.Add("Cannot import process question: InputType was not provided");
                return null;
            }

            var processQuestion = importer.Dataset.ProcessQuestion.NewProcessQuestionRow();
            processQuestion.Name = this.Name;
            processQuestion.Notes = this.Notes;
            processQuestion.InputType = this.InputType;
            processQuestion.MinValue = this.MinValue;
            processQuestion.MaxValue = this.MaxValue;
            processQuestion.Tolerance = this.Tolerance;
            processQuestion.DefaultValue = this.DefaultValue;
            processQuestion.NumericUntis = this.NumericUntis;
            processQuestion.StepOrder = this.StepOrder;
            processQuestion.OperatorEditable = this.OperatorEditable;
            processQuestion.Required = this.Required;
            processQuestion.ProcessStepsRow = processStep;

            if (this.ListID > 0)
            {
                processQuestion.ListID = this.ListID;

                var list = importer.Dataset.Lists.FindByListID(this.ListID);

                //assume list is not the same if id and name do not match
                if (list == null || list.Name != this.ListName)
                {
                    ReplaceList(importer, processQuestion);
                }
            }

            importer.Dataset.ProcessQuestion.AddProcessQuestionRow(processQuestion);
            return processQuestion;
        }

        private void ReplaceList(ProcessImporter importer, ProcessesDataset.ProcessQuestionRow processQuestion)
        {
            ProcessesDataset.ListsRow replacementList;
            importer.CreatedListsMap.TryGetValue(this.ListID, out replacementList);

            if (replacementList == null)
            {
                importer.Issues.Add("Adding new list {0} as it does not currently exist.".FormatWith(ListName));
                replacementList = importer.Dataset.Lists.AddListsRow(this.ListName);
                this.ListValues.ForEach(lv => importer.Dataset.ListValues.AddListValuesRow(lv, replacementList));
                importer.CreatedListsMap[this.ListID] = replacementList;
            }

            processQuestion.ListsRow = replacementList;
        }

        #endregion
    }
}
