using DWOS.Data.Datasets;

namespace DWOS.UI.Sales
{
    /// <summary>
    /// Represents a process widget for use in quoting.
    /// </summary>
    public interface IQuoteProcessWidget
    {
        /// <summary>
        /// Gets the current record.
        /// </summary>
        QuoteDataSet.QuotePartRow CurrentRecord { get; }

        /// <summary>
        /// Gets the current record's dataset.
        /// </summary>
        QuoteDataSet Dataset { get; }

        /// <summary>
        /// Gets or sets a value indicating if the widget is ediable.
        /// </summary>
        bool Editable { get; set; }

        /// <summary>
        /// Loads initial data.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="panelIsEditable"></param>
        void LoadData(QuoteDataSet dataset, bool panelIsEditable);

        /// <summary>
        /// Loads row-specific data.
        /// </summary>
        /// <param name="quotePart"></param>
        void LoadRow(QuoteDataSet.QuotePartRow quotePart);

        /// <summary>
        /// Saves row data.
        /// </summary>
        void SaveRow();

        /// <summary>
        /// Refreshes enabled status of buttons.
        /// </summary>
        void UpdateButtonEnabledStates();
    }
}