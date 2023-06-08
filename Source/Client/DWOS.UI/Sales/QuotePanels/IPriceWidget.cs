using System.Data;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using System;
using DWOS.Data;
using System.Collections.Generic;
using DWOS.Shared.Utilities;

namespace DWOS.UI.Sales
{
    /// <summary>
    /// Represents a price widget for use in quoting.
    /// </summary>
    public interface IPriceWidget
    {
        /// <summary>
        /// Occurs when a price changes.
        /// </summary>
        event EventHandler<PriceChangedEventArgs> PriceChanged;

        /// <summary>
        /// Occurs when the 'price by' setting changes.
        /// </summary>
        event EventHandler<EventArgsTemplate<PriceByType>> PriceByTypeChanged;

        /// <summary>
        /// Gets the current price data of the quote.
        /// </summary>
        /// <remarks>
        /// This can be null if the widget does not use calculation data.
        /// </remarks>
        QuoteDataSet.QuotePartPriceRow CurrentPriceData { get; }

        /// <summary>
        /// Gets the current row.
        /// </summary>
        DataRow CurrentRow { get; }

        /// <summary>
        /// Gets the current row's data table.
        /// </summary>
        DataTable DataTable { get; }

        /// <summary>
        /// Gets the selected price by type
        /// </summary>
        PriceByType PriceBy { get; }

        /// <summary>
        /// Loads row data into this widget.
        /// </summary>
        /// <param name="quotePart">Row to load data for.</param>
        void LoadRow(QuoteDataSet.QuotePartRow quotePart);

        /// <summary>
        /// Saves row data from this widget.
        /// </summary>
        void SaveRow();

        /// <summary>
        /// Initializes this widget for use with a <see cref="DataPanel"/>.
        /// </summary>
        /// <param name="bsData">Binding Source of the data panel.</param>
        /// <param name="dtPrice">Price data table.</param>
        /// <param name="dtPriceCalculation">Price Calculation data table.</param>
        /// <param name="activePriceByTypes">Price types that are currently active in the system</param>
        void Setup(BindingSource bsData,
            QuoteDataSet.QuotePartPriceDataTable dtPrice,
            QuoteDataSet.QuotePartPriceCalculationDataTable dtPriceCalculation,
            IEnumerable<PriceByType> activePriceByTypes);
    }
}