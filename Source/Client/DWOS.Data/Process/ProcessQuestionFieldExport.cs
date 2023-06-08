using DWOS.Data.Datasets;
using System;

namespace DWOS.Data.Process
{
    /// <summary>
    /// Represents process question field information for import/export.
    /// </summary>
    public class ProcessQuestionFieldExport
    {
        #region Properties

        /// <summary>
        /// Gets or sets the field name for this field.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the token name for this field.
        /// </summary>
        public string TokenName { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ProcessQuestionFieldExport"/> class.
        /// </summary>
        public ProcessQuestionFieldExport() { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ProcessQuestionFieldExport"/> class.
        /// </summary>
        /// <param name="field"></param>
        public ProcessQuestionFieldExport(ProcessesDataset.ProcessQuestionFieldRow field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            FieldName = field.FieldName;
            TokenName = field.TokenName;
        }

        /// <summary>
        /// Creates a row for this process question field.
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="processQuestion"></param>
        /// <returns></returns>
        public ProcessesDataset.ProcessQuestionFieldRow CreateRow(ProcessImporter importer, ProcessesDataset.ProcessQuestionRow processQuestion)
        {
            if (importer == null)
            {
                throw new ArgumentNullException(nameof(importer));
            }

            if (string.IsNullOrEmpty(FieldName))
            {
                importer.Issues.Add("Cannot import process question field: Field Name was not provided");
                return null;
            }

            if (string.IsNullOrEmpty(TokenName))
            {
                importer.Issues.Add("Cannot import process question field: Token Name was not provided");
                return null;
            }

            var field = importer.Dataset.ProcessQuestionField.NewProcessQuestionFieldRow();
            field.FieldName = FieldName;
            field.TokenName = TokenName;
            field.ProcessQuestionRow = processQuestion;
            importer.Dataset.ProcessQuestionField.AddProcessQuestionFieldRow(field);
            return field;
        }

        #endregion
    }
}
