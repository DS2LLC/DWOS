using DWOS.Data.Datasets;
using System;
using System.Collections.Generic;

namespace DWOS.Data.Process
{
    /// <summary>
    /// Imports an <see cref="ProcessExport"/> instance.
    /// </summary>
    public sealed class ProcessImporter
    {
        #region Properties

        /// <summary>
        /// Gets the dataset for this instance.
        /// </summary>
        public ProcessesDataset Dataset
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a list of issues for this instance.
        /// </summary>
        public List<string> Issues { get; } = new List<string>();

        /// <summary>
        /// Gets a mapping between ListID (as listed in the process file)
        /// and a non-persisted list.
        /// </summary>
        public Dictionary<int, ProcessesDataset.ListsRow> CreatedListsMap { get; } =
            new Dictionary<int, ProcessesDataset.ListsRow>();

        /// <summary>
        /// Gets a mapping between ProcessQuestionId (as listed in the process file)
        /// and a non-persisted question.
        /// </summary>
        public Dictionary<int, ProcessesDataset.ProcessQuestionRow> CreatedQuestionsMap { get; } =
            new Dictionary<int, ProcessesDataset.ProcessQuestionRow>();

        /// <summary>
        /// Gets the <see cref="ProcessExport"/> object for this instance.
        /// </summary>
        public ProcessExport Data
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessImporter"/> class.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="data"></param>
        /// <exception cref="ArgumentNullException">
        /// Either <paramref name="dataset"/> or <paramref name="data"/> is null.
        /// </exception>
        public ProcessImporter(ProcessesDataset dataset, ProcessExport data)
        {
            if (dataset == null)
            {
                throw new ArgumentNullException("dataset", "dataset cannot be null");
            }
            else if (data == null)
            {
                throw new ArgumentNullException("data", "data cannot be null");
            }

            Dataset = dataset;
            Data = data;
        }

        /// <summary>
        /// Converts the process to a database row.
        /// </summary>
        /// <returns>
        /// Database row that was added to <see cref="Dataset"/> along with
        /// all nested values (aliases, questions, etc.).
        /// </returns>
        public ProcessesDataset.ProcessRow Import()
        {
            return Data.CreateRow(this);
        }

        #endregion
    }
}
