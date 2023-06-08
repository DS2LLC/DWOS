using DWOS.Data.Datasets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Data.Process
{
    /// <summary>
    /// Represents process information for import/export.
    /// </summary>
    public class ProcessExport
    {
        #region Properties

        /// <summary>
        /// Gets or sets the process name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the process description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the process revision.
        /// </summary>
        public string Revision { get; set; }

        /// <summary>
        /// Gets or sets the process department.
        /// </summary>
        /// <remarks>
        /// This value does not have to be an existing department.
        /// </remarks>
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the process is paperless.
        /// </summary>
        /// <value>
        /// <c>true</c> if the process is paperless; otherwise, <c>false</c>.
        /// </value>
        public bool IsPaperless { get; set; }

        /// <summary>
        /// Gets or sets the process category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the collection of process aliases.
        /// </summary>
        public List<ProcessAliasExport> ProcessAliases { get; set; }

        /// <summary>
        /// Gets or sets the collection of process steps.
        /// </summary>
        public List<ProcessStepExport> ProcessSteps { get; set; }

        /// <summary>
        /// Gets or sets the collection of process inspections.
        /// </summary>
        public List<ProcessInspectionExport> ProcessInspections { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Fills this with data from an existing process.
        /// </summary>
        /// <param name="process"></param>
        public void Fill(Data.Datasets.ProcessesDataset.ProcessRow process)
        {
            this.Name = process.Name;
            this.Description = process.IsDescriptionNull() ? null : process.Description;
            this.Revision = process.IsRevisionNull() ? null : process.Revision;
            this.Department = process.Department;
            this.IsPaperless = process.IsPaperless;
            this.Category = process.Category;

            ProcessAliases = new List<ProcessAliasExport>();
            ProcessSteps = new List<ProcessStepExport>();
            ProcessInspections = new List<ProcessInspectionExport>();

            process.GetProcessAliasRows().ForEach(par => ProcessAliases.Add(new ProcessAliasExport(par)));
            process.GetProcessStepsRows().ForEach(par => ProcessSteps.Add(new ProcessStepExport(par)));
            process.GetProcessInspectionsRows().ForEach(pi => ProcessInspections.Add(new ProcessInspectionExport(pi)));
        }

        /// <summary>
        /// Creates a row from this process.
        /// </summary>
        /// <param name="importer"></param>
        /// <returns></returns>
        public Data.Datasets.ProcessesDataset.ProcessRow CreateRow(ProcessImporter importer)
        {
            const string majorIssueFormat = "Cannot import process: {0}";

            if (importer == null)
            {
                throw new ArgumentNullException("importer", "importer cannot be null");
            }
            else if (importer.Data != this)
            {
                throw new ArgumentException("importer", "importer was not setup correctly.");
            }

            if (string.IsNullOrEmpty(this.Name))
            {
                importer.Issues.Add(majorIssueFormat.FormatWith("Name was not provided"));
                return null;
            }
            else if (string.IsNullOrEmpty(this.Description))
            {

                importer.Issues.Add(majorIssueFormat.FormatWith("Description was not provided"));
                return null;
            }
            else if (string.IsNullOrEmpty(this.Revision))
            {
                importer.Issues.Add(majorIssueFormat.FormatWith("Revision was not provided"));
                return null;
            }
            else if (string.IsNullOrEmpty(this.Department))
            {
                importer.Issues.Add(majorIssueFormat.FormatWith("Department was not provided"));
                return null;
            }

            var process = importer.Dataset.Process.NewProcessRow();

            process.Name = this.Name;
            process.Description = this.Description;
            process.Revision = this.Revision;
            process.Department = this.Department;
            process.Category = this.Category;
            process.IsPaperless = this.IsPaperless;
            process.Frozen = false;
            process.Active = true;
            process.IsApproved = false;
            process.ModifiedDate = DateTime.Now;

            if (this.Category == null)
            {
                var categoryRow = importer.Dataset.d_ProcessCategory.FirstOrDefault();

                if (categoryRow != null)
                    process.Category = categoryRow["ProcessCategory"].ToString();
                else
                {
                    // No categories available, create a default one
                    process.Category = "Default";
                    importer.Dataset.d_ProcessCategory.Addd_ProcessCategoryRow(process.Category);
                    importer.Issues.Add("Added new category: " + process.Category);
                }
            }
            else
            {
                if (importer.Dataset.d_ProcessCategory.FindByProcessCategory(this.Category) == null)
                {
                    importer.Dataset.d_ProcessCategory.Addd_ProcessCategoryRow(this.Category);
                    importer.Issues.Add("Added new category: " + this.Category);
                }
            }

            importer.Dataset.Process.AddProcessRow(process);

            foreach (var pa in ProcessAliases)
            {
                pa.CreateRow(importer, process);
            }

            CreateRowsForSteps(importer, process);

            foreach (var pa in ProcessInspections)
            {
                pa.CreateRow(importer, process);
            }

            return process;
        }

        private void CreateRowsForSteps(ProcessImporter importer, ProcessesDataset.ProcessRow process)
        {
            // Add steps and their questions
            var stepsWithRows = new List<Tuple<ProcessStepExport, ProcessesDataset.ProcessStepsRow>>();

            foreach (var processStep in ProcessSteps)
            {
                var paRow = processStep.CreateRow(importer, process);

                if (paRow == null)
                {
                    continue;
                }

                stepsWithRows.Add(new Tuple<ProcessStepExport, ProcessesDataset.ProcessStepsRow>(processStep, paRow));

                foreach (var pq in processStep.ProcessQuestions)
                {
                    var pqRow = pq.CreateRow(importer, paRow);
                    importer.CreatedQuestionsMap[pq.ProcessQuestionIdOriginal] = pqRow;

                    foreach (var field in pq.ProcessQuestionFields ?? Enumerable.Empty<ProcessQuestionFieldExport>())
                    {
                        field.CreateRow(importer, pqRow);
                    }
                }
            }

            // Add conditions - they depend on questions for other steps
            foreach (var stepWithRow in stepsWithRows)
            {
                var step = stepWithRow.Item1;
                var stepRow = stepWithRow.Item2;

                foreach (var condition in step.ProcessStepConditions)
                {
                    condition.CreateRow(importer, stepRow);
                }
            }
        }

        #endregion
    }
}
