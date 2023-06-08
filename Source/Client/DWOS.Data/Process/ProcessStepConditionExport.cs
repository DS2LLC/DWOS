using DWOS.Data.Datasets;

namespace DWOS.Data.Process
{
    /// <summary>
    /// Represents process question information for import/export.
    /// </summary>
    public class ProcessStepConditionExport
    {
        #region Properties

        /// <summary>
        /// Gets or sets the input type for this step condition.
        /// </summary>
        public string InputType { get; set; }


        /// <summary>
        /// Gets or sets the original Process Question ID for this
        /// step condition.
        /// </summary>
        /// <remarks>
        /// Used to match back up with process question.
        /// </remarks>
        public int ProcessQuestionIdOriginal { get; set; }

        /// <summary>
        /// Gets or sets the comparison operator for this step condition.
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Gets or sets the comparison value for this step condition. 
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the step order for this step condition.
        /// </summary>
        public int StepOrder { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ProcessStepConditionExport"/> class.
        /// </summary>
        public ProcessStepConditionExport() { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ProcessStepConditionExport"/> class.
        /// </summary>
        /// <param name="processCondition"></param>
        public ProcessStepConditionExport(Data.Datasets.ProcessesDataset.ProcessStepConditionRow processCondition)
        {
            this.InputType = processCondition.InputType;
            this.ProcessQuestionIdOriginal = processCondition.IsProcessQuestionIdNull() ? -1 : processCondition.ProcessQuestionId;
            this.Operator = processCondition.Operator;
            this.Value = processCondition.Value;
            this.StepOrder = processCondition.StepOrder;
        }

        /// <summary>
        /// Creates a row for this step condition.
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="processStep"></param>
        public void CreateRow(ProcessImporter importer, ProcessesDataset.ProcessStepsRow processStep)
        {
            if (string.IsNullOrEmpty(this.InputType))
            {
                importer.Issues.Add("Cannot import process step: InputType was not provided");
                return;
            }

            var processCondition = importer.Dataset.ProcessStepCondition.NewProcessStepConditionRow();

            processCondition.ProcessStepsRow = processStep;
            processCondition.InputType = this.InputType;
            processCondition.Operator = this.Operator;
            processCondition.Value = this.Value;
            processCondition.StepOrder = this.StepOrder;

            if (importer.CreatedQuestionsMap.ContainsKey(ProcessQuestionIdOriginal))
            {
                processCondition.ProcessQuestionRow = importer.CreatedQuestionsMap[ProcessQuestionIdOriginal];
            }

            importer.Dataset.ProcessStepCondition.AddProcessStepConditionRow(processCondition);
        }

        #endregion
    }
}
