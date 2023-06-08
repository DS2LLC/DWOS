namespace DWOS.Data
{
    /// <summary>
    /// Represents a token in a  report.
    /// </summary>
    public class ReportToken
    {
        #region Fields

        /// <summary>
        /// Default width for report tokens.
        /// </summary>
        /// <remarks>
        /// Infragistics: the column width including padding, in 256ths of the
        /// '0' digit character width in the workbook's default font.
        /// http://help.infragistics.com/Help/Doc/WinForms/2012.2/CLR4.0/HTML/Infragistics4.Documents.Excel.v12.2~Infragistics.Documents.Excel.Worksheet~DefaultColumnWidth.html
        /// </remarks>
        public const int WIDTH = 100;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the field name for this instance.
        /// </summary>
        /// <remarks>
        /// This can be a Custom Field identifier or a
        /// <see cref="ReportFieldMapper.enumReportTokens"/> value.
        /// </remarks>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the display name for this instance.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or set a value indicating if this instance is for a
        /// custom field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this is for a custom field; otherwise, <c>false</c>.
        /// </value>
        public bool IsCustom { get; set; }

        /// <summary>
        /// Gets or sets the order of this token in relation to other tokens
        /// for the same report.
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the width of this instance.
        /// </summary>
        public int Width { get; set; } = WIDTH;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportToken"/> class.
        /// </summary>
        public ReportToken()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportToken"/> class.
        /// </summary>
        /// <param name="reportToken"></param>
        public ReportToken(ReportFieldMapper.enumReportTokens reportToken)
        {
            FieldName = reportToken.ToString();
            DisplayName = ReportFieldMapper.GetDisplayName(reportToken);
        }

        /// <summary>
        /// Creates a new <see cref="ReportToken"/> instance from a field row.
        /// </summary>
        /// <param name="row"></param>
        /// <returns>
        /// A new <see cref="ReportToken"/> instance of
        /// <paramref name="row"/> is not null; otherwise, <c>null</c>.
        /// </returns>
        public static ReportToken From(DWOS.Data.Datasets.ReportFieldsDataSet.ReportFieldsRow row)
        {
            if (row == null)
            {
                return null;
            }
            else
            {
                return new ReportToken()
                {
                    FieldName = row.FieldName,
                    DisplayName = row.DisplayName,
                    Width = row.Width,
                    IsCustom = row.IsCustomField,
                    DisplayOrder = row.DisplayOrder
                };
            }
        }

        #endregion
    }
}
