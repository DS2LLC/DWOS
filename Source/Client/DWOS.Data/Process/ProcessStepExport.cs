using DWOS.Data.Datasets;
using System;
using System.Collections.Generic;

namespace DWOS.Data.Process
{
    /// <summary>
    /// Represents process question information for import/export.
    /// </summary>
    public class ProcessStepExport
    {

        #region Properties

        /// <summary>
        /// Gets or sets the name for this process step.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description for this process step.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the step order for this process step.
        /// </summary>
        public decimal StepOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if this process step should appear
        /// on the COC by default.
        /// </summary>
        public bool COCData { get; set; }

        /// <summary>
        /// Gets or sets a collection of process questions for this
        /// process step.
        /// </summary>
        public List<ProcessQuestionExport> ProcessQuestions { get; set; }

        /// <summary>
        /// Gets or sets a collection of step conditions for this
        /// process step.
        /// </summary>
        public List<ProcessStepConditionExport> ProcessStepConditions { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ProcessStepExport"/> class.
        /// </summary>
        public ProcessStepExport() { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ProcessStepExport"/> class.
        /// </summary>
        /// <param name="processStep"></param>
        public ProcessStepExport(Data.Datasets.ProcessesDataset.ProcessStepsRow processStep)
        {
            this.Name = processStep.Name;
            this.Description = processStep.IsDescriptionNull() ? null : processStep.Description;
            this.StepOrder = processStep.StepOrder;
            this.COCData = processStep.COCData;

            ProcessQuestions = new List<ProcessQuestionExport>();
            processStep.GetProcessQuestionRows().ForEach(par => ProcessQuestions.Add(new ProcessQuestionExport(par)));

            ProcessStepConditions = new List <ProcessStepConditionExport>();
            processStep.GetProcessStepConditionRows().ForEach(psc => ProcessStepConditions.Add(new ProcessStepConditionExport(psc)));
        }

        /// <summary>
        /// Creates a row for this process step.
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        public ProcessesDataset.ProcessStepsRow CreateRow(ProcessImporter importer, ProcessesDataset.ProcessRow process)
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                importer.Issues.Add("Cannot import process step: Name was not provided");
                return null;
            }

            var processStep = importer.Dataset.ProcessSteps.NewProcessStepsRow();
            processStep.Name = this.Name;
            processStep.ProcessRow = process;
            processStep.Description = this.Description;
            processStep.StepOrder = this.StepOrder;
            processStep.COCData = this.COCData;

            importer.Dataset.ProcessSteps.AddProcessStepsRow(processStep);

            return processStep;
        }

        #endregion
    }
}
