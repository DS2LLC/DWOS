using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data;
using DWOS.UI.Utilities;
using DWOS.Shared.Utilities;

namespace DWOS.UI.Sales
{
    public partial class PriceWidget : UserControl, IPriceWidget
    {
        #region Fields

        public event EventHandler<PriceChangedEventArgs> PriceSyncClicked;
        private QuoteDataSet.QuotePartPriceDataTable _dtQuotePrice;
        private List<PriceByType> _activePriceByTypes;
        private List<PriceByType> _priceByTypes;

        #endregion

        #region Methods

        public PriceWidget()
        {
            InitializeComponent();
        }

        public void UpdatePrice(PricingStrategy priceStrategy, decimal amount)
        {
            const string MAX_WARNING = "Total price exceeded maximum value. Please correct process prices as needed.";
            const string MIN_WARNING = "Total price is below minimum value. Please correct process prices as needed.";
            const string WARNING_TITLE = "Incorrect Total Price";
            const int currencyRounding = 2;

            var currentPart = CurrentRow as QuoteDataSet.QuotePartRow;

            if (priceStrategy == PricingStrategy.Each && (currentPart.IsEachPriceNull() || currentPart.EachPrice != amount))
            {
                decimal newValue;
                if (amount > curEachPrice.MaxValue)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(MAX_WARNING, WARNING_TITLE);
                    newValue = curEachPrice.MaxValue;
                }
                else if (amount < curEachPrice.MinValue)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(MIN_WARNING, WARNING_TITLE);
                    newValue = curEachPrice.MinValue;
                }
                else
                {
                    newValue = amount;
                }

                curEachPrice.Value = Math.Round(newValue, currencyRounding);
                curEachPrice.DataBindings[0].WriteValue(); // manually refresh binding
                PriceChanged?.Invoke(this,
                    new PriceChangedEventArgs(PricingStrategy.Each, curEachPrice.Value));
            }
            else if (priceStrategy == PricingStrategy.Lot && (currentPart.IsLotPriceNull() || currentPart.LotPrice != amount))
            {
                decimal newValue;
                if (amount > curLotPrice.MaxValue)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(MAX_WARNING, WARNING_TITLE);
                    newValue = curLotPrice.MaxValue;
                }
                else if (amount < curLotPrice.MinValue)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(MIN_WARNING, WARNING_TITLE);
                    newValue = curLotPrice.MinValue;
                }
                else
                {
                    newValue = amount;
                }

                curLotPrice.Value = Math.Round(newValue, currencyRounding);
                curLotPrice.DataBindings[0].WriteValue(); // manually refresh binding
                PriceChanged?.Invoke(this,
                    new PriceChangedEventArgs(PricingStrategy.Lot, curLotPrice.Value));
            }
        }

        #endregion

        #region Events

        private void curEachPrice_Validated(object sender, EventArgs e)
        {
            try
            {
                var handler = PriceChanged;
                handler?.Invoke(this,
                    new PriceChangedEventArgs(PricingStrategy.Each, curEachPrice.Value));
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error changing each price.");
            }
        }

        private void curLotPrice_Validated(object sender, EventArgs e)
        {
            try
            {
                var handler = PriceChanged;
                handler?.Invoke(this,
                    new PriceChangedEventArgs(PricingStrategy.Lot, curLotPrice.Value));
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error changing lot price.");
            }
        }

        private void curEachPrice_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                PriceSyncClicked?.Invoke(this,
                    new PriceChangedEventArgs(PricingStrategy.Each, curEachPrice.Value));
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error syncing each price.");
            }
        }

        private void curLotPrice_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                PriceSyncClicked?.Invoke(this,
                    new PriceChangedEventArgs(PricingStrategy.Lot, curLotPrice.Value));
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error syncing lot price.");
            }
        }

        private void cboPriceBy_Validated(object sender, EventArgs e)
        {
            try
            {
                PriceByTypeChanged?.Invoke(this,
                    new EventArgsTemplate<PriceByType>(PriceBy));
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error updating price by type.");
            }
        }

        #endregion

        #region IPriceWidget Members

        public event EventHandler<PriceChangedEventArgs> PriceChanged;

        public event EventHandler<EventArgsTemplate<PriceByType>> PriceByTypeChanged;

        public QuoteDataSet.QuotePartPriceRow CurrentPriceData
        {
            get
            {
                return null;
            }
        }

        public DataTable DataTable
        {
            get
            {
                return _dtQuotePrice;
            }
        }

        public DataRow CurrentRow
        {
            get;
            private set;
        }

        public PriceByType PriceBy =>
            (cboPriceBy.Value as PriceByType?) ?? PriceByType.Quantity;

        public void LoadRow(QuoteDataSet.QuotePartRow quotePart)
        {
            var allowedPriceByTypes = new List<PriceByType>(_activePriceByTypes)
            {
                (PriceByType)Enum.Parse(typeof(PriceByType), quotePart.PriceBy)
            };

            cboPriceBy.DataSource = allowedPriceByTypes
                .Distinct()
                .ToList();

            cboPriceBy.DataBindings[0].ReadValue(); // Required; otherwise, value can be blanked out

            CurrentRow = quotePart;
        }

        public void SaveRow()
        {
            // Binding ensures that values are saved.
        }

        public void Setup(BindingSource bsData,
            QuoteDataSet.QuotePartPriceDataTable dtPrice,
            QuoteDataSet.QuotePartPriceCalculationDataTable dtPriceCalculation,
            IEnumerable<PriceByType> activePriceByTypes)
        {
            var priceDecimalPlaces = ApplicationSettings.Current.PriceDecimalPlaces;
            curEachPrice.MaskInput = "{currency:6." + priceDecimalPlaces + "}";
            curLotPrice.MaskInput = "{currency:6." + priceDecimalPlaces + "}";

            // EachPrice and LotPrice can be null, and using binding instead
            // of manually updating those values allows them to be null.
            const string eachPricePropertyName = nameof(QuoteDataSet.QuotePartRow.EachPrice);
            const string lotPricePropertyName = nameof(QuoteDataSet.QuotePartRow.LotPrice);
            curEachPrice.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(curEachPrice), bsData, eachPricePropertyName));
            curLotPrice.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(curLotPrice), bsData, lotPricePropertyName));

            _dtQuotePrice = dtPrice;

            // Setup list of active price by types
            _activePriceByTypes = activePriceByTypes.ToList();
            _priceByTypes = new List<PriceByType>(_activePriceByTypes);

            // Setup binding for price-by
            var priceByPropertyName = nameof(QuoteDataSet.QuotePartRow.PriceBy);
            cboPriceBy.DataBindings.Add(new Binding(
                ControlUtilities.GetBindingProperty(cboPriceBy), bsData, priceByPropertyName));

            cboPriceBy.DataSource = _priceByTypes;
        }

        #endregion
    }
}
