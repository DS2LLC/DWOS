using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.QuoteDataSetTableAdapters;
using DWOS.Shared.Utilities;
using DWOS.UI.Utilities;
using DWOS.UI.Utilities.Calculators;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Interop;

namespace DWOS.UI.Sales
{
    /// <summary>
    /// Advanced price calculation widget.
    /// </summary>
    public partial class PriceCalculatorWidget : UserControl, IPriceWidget
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private QuoteDataSet.QuotePartPriceDataTable _dtQuotePrice;
        private QuoteDataSet.QuotePartPriceCalculationDataTable _dtQuotePriceCalculation;
        private List<PriceByType> _activePriceByTypes =
            new List<PriceByType>();

        private List<PriceByType> _priceByTypes;

        #endregion

        #region Properties

        public double SurfaceArea
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="PriceCalculatorWidget"/> class.
        /// </summary>
        public PriceCalculatorWidget()
        {
            InitializeComponent();
        }

        private void UpdateEachPrice()
        {
            decimal pricePerPart = GetPricePerPart();

            if (pricePerPart > curEachPrice.MaxValue)
            {
                MessageBoxUtilities.ShowMessageBoxWarn(
                    "Total price has exceeded maximum value. Please correct the markup or costs as needed.",
                    "Incorrect Price Total Value");
            }
            else if (pricePerPart < curEachPrice.MinValue)
            {
                MessageBoxUtilities.ShowMessageBoxWarn(
                    "Total price is less than the minimum value. Please correct the markup or costs as needed.",
                    "Incorrect Price Total Value");
            }
            else
            {
                curEachPrice.Value = pricePerPart;
            }
        }

        private void UpdateTotalCost()
        {
            decimal price = this.GetPricePerPart();

            if (price > curTotalCost.MaxValue)
            {
                price = curTotalCost.MaxValue;
            }
            else if (price < curTotalCost.MinValue)
            {
                price = curTotalCost.MinValue;
            }

            curTotalCost.Value = price;
        }

        private decimal GetPricePerPart()
        {
            decimal markupTotal = curMarkupTotal.Value;
            decimal pricePerPart = GetCostSubtotal() + markupTotal;
            return pricePerPart;
        }

        private decimal GetCostSubtotal()
        {
            decimal laborCost = curLaborCost.Value;
            decimal materialCost = curMaterialCost.Value;
            decimal overheadCost = curOverheadCost.Value;

            return laborCost + materialCost + overheadCost;
        }

        private static CalculatorWindow CreateWindow(CalculationStep step, decimal rate, decimal laborCost = 0M, decimal materialCost = 0M, decimal overheadCost = 0M, double surfaceArea = 0D)
        {
            var preferredCalculationTypeMap = new Dictionary<CalculationStep, string>()
            {
                { CalculationStep.Rate, CalculatorWindow.TYPE_RATE_V1 },
                { CalculationStep.Labor, CalculatorWindow.TYPE_LABOR_V1 },
                { CalculationStep.Markup, CalculatorWindow.TYPE_MARKUP_V1 },
                { CalculationStep.Overhead, CalculatorWindow.TYPE_OVERHEAD_V1 },
                { CalculationStep.Material, CalculatorWindow.TYPE_MATERIAL_V1 }
            };

            var data = new CalculatorData()
            {
                CalculationType = preferredCalculationTypeMap[step],
                PartsPerHour = rate,
                Step = step,
                LaborCost = laborCost,
                MaterialCost = materialCost,
                OverheadCost = overheadCost,
                SurfaceArea = surfaceArea
            };

            var window = CalculatorWindow.CreateWindow(data);
            var helper = new WindowInteropHelper(window) { Owner = DWOSApp.MainForm.Handle };
            return window;
        }

        private static CalculatorWindow CreateWindow(DataRow existingData, decimal rate, decimal laborCost = 0M, decimal materialCost = 0M, decimal overheadCost = 0M, double surfaceArea = 0D)
        {
            CalculationStep step;
            var existingQuotePriceData = existingData as QuoteDataSet.QuotePartPriceCalculationRow;

            if (existingQuotePriceData == null || !Enum.TryParse(existingQuotePriceData.Step, out step))
            {
                return null;
            }

            var data = new CalculatorData()
            {
                CalculationType = existingQuotePriceData.CalculationType,
                PartsPerHour = rate,
                Step = step,
                JsonData = existingQuotePriceData.Data,
                LaborCost = laborCost,
                MaterialCost = materialCost,
                OverheadCost = overheadCost,
                SurfaceArea = surfaceArea
            };

            var window = CalculatorWindow.CreateWindow(data);
            var helper = new WindowInteropHelper(window) { Owner = DWOSApp.MainForm.Handle };
            return window;
        }

        private DataRow RetrieveCalculationData(CalculationStep step)
        {
            if (_dtQuotePriceCalculation == null || CurrentPriceData == null)
            {
                return null;
            }

            bool previouslyLoadedData = _dtQuotePriceCalculation
                .Any(i => i.QuotePartPriceID == CurrentPriceData.QuotePartPriceID && i.Step == step.ToString());

            if (!previouslyLoadedData) // Do not replace unsaved data
            {
                using (var taCalculation = new QuotePartPriceCalculationTableAdapter() { ClearBeforeFill = false })
                {
                    taCalculation.FillBy(_dtQuotePriceCalculation,
                        CurrentPriceData.QuotePartPriceID,
                        step.ToString());
                }
            }

            return _dtQuotePriceCalculation
                .Where(i => i.QuotePartPriceID == CurrentPriceData.QuotePartPriceID && i.Step == step.ToString())
                .OrderByDescending(i => i.QuotePartPriceCalculationID)
                .FirstOrDefault();
        }

        private void SaveExistingCalculationData(DataRow calculationData, string calculationType, string jsonData)
        {
            var quotePriceCalculation = calculationData as QuoteDataSet.QuotePartPriceCalculationRow;
            if (_dtQuotePriceCalculation == null)
            {
                LogManager.GetCurrentClassLogger().Warn("This widget is not setup to save calculation data.");
                return;
            }
            else if (quotePriceCalculation == null)
            {
                return;
            }

            quotePriceCalculation.Data = jsonData;
        }

        private void SaveNewCalculationData(CalculationStep step, string calculationType, string jsonData)
        {
            if (_dtQuotePrice == null || _dtQuotePriceCalculation == null)
            {
                LogManager.GetCurrentClassLogger().Warn("This widget is not setup to save calculation data.");
                return;
            }

            var quotePartPrice = CurrentPriceData as QuoteDataSet.QuotePartPriceRow;
            var quotePart = CurrentRow as QuoteDataSet.QuotePartRow;

            if (quotePartPrice == null)
            {
                quotePartPrice = _dtQuotePrice.NewQuotePartPriceRow();
                quotePartPrice.QuotePartRow = quotePart;
                _dtQuotePrice.AddQuotePartPriceRow(quotePartPrice);

                CurrentPriceData = quotePartPrice;
            }

            _dtQuotePriceCalculation.AddQuotePartPriceCalculationRow(quotePartPrice,
                step.ToString(),
                calculationType,
                jsonData);
        }

        private void AddValueChangedHandlers()
        {
            curLaborCost.ValueChanged += Cost_ValueChanged;
            curMaterialCost.ValueChanged += Cost_ValueChanged;
            curOverheadCost.ValueChanged += Cost_ValueChanged;
            curMarkupTotal.ValueChanged += Cost_ValueChanged;
        }

        private void RemoveValueChangedHandlers()
        {
            curLaborCost.ValueChanged -= Cost_ValueChanged;
            curMaterialCost.ValueChanged -= Cost_ValueChanged;
            curOverheadCost.ValueChanged -= Cost_ValueChanged;
            curMarkupTotal.ValueChanged -= Cost_ValueChanged;
        }

        #endregion

        #region Events

        private void curEachPrice_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                if (e.Button.Key == "Refresh")
                {
                    UpdateEachPrice();

                    // Immediately refresh value
                    curEachPrice.DataBindings[0].WriteValue();
                    var handler = PriceChanged;
                    handler?.Invoke(this,
                        new PriceChangedEventArgs(PricingStrategy.Each, curEachPrice.Value));
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error with manually refreshing price.");
            }
        }

        private void numRate_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                if (e.Button.Key != "Calculator")
                {
                    return;
                }

                CalculatorWindow window;
                var existingData = RetrieveCalculationData(CalculationStep.Rate);

                if (existingData == null)
                {
                    window = CreateWindow(CalculationStep.Rate, 0);
                }
                else
                {
                    window = CreateWindow(existingData, 0);
                }

                if (window.ShowDialog().GetValueOrDefault(false))
                {
                    numRate.Value = window.GetResult<decimal>();

                    if (existingData == null)
                    {
                        SaveNewCalculationData(CalculationStep.Rate, window.CalculationType, window.JsonData);
                    }
                    else
                    {
                        SaveExistingCalculationData(existingData, window.CalculationType, window.JsonData);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error with clicking rate editor button.");
            }
        }

        private void curLaborCost_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                if (e.Button.Key != "Calculator")
                {
                    return;
                }

                var rate = (numRate.Value as decimal?) ?? 0M;
                CalculatorWindow window;
                var existingData = RetrieveCalculationData(CalculationStep.Labor);

                if (existingData == null)
                {
                    window = CreateWindow(CalculationStep.Labor, rate);
                }
                else
                {
                    window = CreateWindow(existingData, rate);
                }

                if (window.ShowDialog().GetValueOrDefault(false))
                {
                    var laborCost = window.GetResult<decimal>();

                    if (laborCost > curLaborCost.MaxValue)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            "Labor cost has exceeded maximum value. Please correct labor cost data as needed.",
                             "Incorrect Labor Cost Value");

                        laborCost = curLaborCost.MaxValue;
                    }
                    else if (laborCost < curLaborCost.MinValue)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            "Labor cost is less than the minimum value. Please correct the labor cost data as needed.",
                            "Incorrect Labor Cost Value");

                        laborCost = curLaborCost.MinValue;
                    }

                    curLaborCost.Value = Math.Round(laborCost, 4);

                    if (existingData == null)
                    {
                        SaveNewCalculationData(CalculationStep.Labor, window.CalculationType, window.JsonData);
                    }
                    else
                    {
                        SaveExistingCalculationData(existingData, window.CalculationType, window.JsonData);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error with clicking labor editor button.");
            }
        }

        private void curMaterialCost_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                if (e.Button.Key != "Calculator")
                {
                    return;
                }

                var rate = (numRate.Value as decimal?) ?? 0M;
                CalculatorWindow window;
                var existingData = RetrieveCalculationData(CalculationStep.Material);

                if (existingData == null)
                {
                    window = CreateWindow(CalculationStep.Material, rate, surfaceArea: SurfaceArea);
                }
                else
                {
                    window = CreateWindow(existingData, rate, surfaceArea: SurfaceArea);
                }

                if (window.ShowDialog().GetValueOrDefault(false))
                {
                    var overheadCost = window.GetResult<decimal>();

                    if (overheadCost > curMaterialCost.MaxValue)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            "Material cost has exceeded maximum value. Please correct material cost data as needed.",
                             "Incorrect Material Cost Value");

                        overheadCost = curMaterialCost.MaxValue;
                    }
                    else if (overheadCost < curMaterialCost.MinValue)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            "Material cost is less than the minimum value. Please correct material cost data as needed.",
                            "Incorrect Material Cost Value");

                        overheadCost = curMaterialCost.MinValue;
                    }

                    curMaterialCost.Value = Math.Round(overheadCost, 4);

                    if (existingData == null)
                    {
                        SaveNewCalculationData(CalculationStep.Material, window.CalculationType, window.JsonData);
                    }
                    else
                    {
                        SaveExistingCalculationData(existingData, window.CalculationType, window.JsonData);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error with clicking labor editor button.");
            }
        }

        private void curOverheadCost_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                if (e.Button.Key != "Calculator")
                {
                    return;
                }

                var rate = (numRate.Value as decimal?) ?? 0M;
                CalculatorWindow window;
                var existingData = RetrieveCalculationData(CalculationStep.Overhead);

                if (existingData == null)
                {
                    window = CreateWindow(CalculationStep.Overhead, rate);
                }
                else
                {
                    window = CreateWindow(existingData, rate);
                }

                if (window.ShowDialog().GetValueOrDefault(false))
                {
                    var overheadCost = window.GetResult<decimal>();

                    if (overheadCost > curOverheadCost.MaxValue)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            "Overhead cost has exceeded maximum value. Please correct labor cost data as needed.",
                             "Incorrect Overhead Cost Value");

                        overheadCost = curOverheadCost.MaxValue;
                    }
                    else if (overheadCost < curOverheadCost.MinValue)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            "Overhead cost is less than the minimum value. Please correct the labor cost data as needed.",
                            "Incorrect Overhead Cost Value");

                        overheadCost = curOverheadCost.MinValue;
                    }

                    curOverheadCost.Value = Math.Round(overheadCost, 4);

                    if (existingData == null)
                    {
                        SaveNewCalculationData(CalculationStep.Overhead, window.CalculationType, window.JsonData);
                    }
                    else
                    {
                        SaveExistingCalculationData(existingData, window.CalculationType, window.JsonData);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error with clicking labor editor button.");
            }
        }

        private void curMarkupTotal_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                if (e.Button.Key != "Calculator")
                {
                    return;
                }

                var rate = (numRate.Value as decimal?) ?? 0M;
                decimal laborCost = curLaborCost.Value;
                decimal materialCost = curMaterialCost.Value;
                decimal overheadCost = curOverheadCost.Value;

                CalculatorWindow window;
                var existingData = RetrieveCalculationData(CalculationStep.Markup);

                if (existingData == null)
                {
                    window = CreateWindow(CalculationStep.Markup, rate, laborCost, materialCost, overheadCost);
                }
                else
                {
                    window = CreateWindow(existingData, rate, laborCost, materialCost, overheadCost);
                }

                if (window.ShowDialog().GetValueOrDefault(false))
                {
                    var markup = window.GetResult<decimal>();

                    if (markup > curMarkupTotal.MaxValue)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            "Markup has exceeded maximum value. Please correct markup data as needed.",
                             "Incorrect Markup Value");

                        markup = curMarkupTotal.MaxValue;
                    }
                    else if (markup < curMarkupTotal.MinValue)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            "Markup is less than the minimum value. Please correct the markup data as needed.",
                            "Incorrect Markup Value");

                        markup = curMarkupTotal.MinValue;
                    }

                    curMarkupTotal.Value = Math.Round(markup, 4);

                    if (existingData == null)
                    {
                        SaveNewCalculationData(CalculationStep.Markup, window.CalculationType, window.JsonData);
                    }
                    else
                    {
                        SaveExistingCalculationData(existingData, window.CalculationType, window.JsonData);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error with clicking markup editor button.");
            }
        }

        private void Cost_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateTotalCost();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating cost total.");
            }
        }

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
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing each price.");
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
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing lot price.");
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
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating price by type.");
            }
        }

        #endregion

        #region IPriceWidget Members

        public event EventHandler<PriceChangedEventArgs> PriceChanged;

        public event EventHandler<EventArgsTemplate<PriceByType>> PriceByTypeChanged;

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

        public QuoteDataSet.QuotePartPriceRow CurrentPriceData
        {
            get;
            private set;
        }

        public PriceByType PriceBy =>
            (cboPriceBy.Value as PriceByType?) ?? PriceByType.Quantity;

        public void LoadRow(QuoteDataSet.QuotePartRow quotePart)
        {
            try
            {
                RemoveValueChangedHandlers();
                curTotalCost.Value = 0M;

                var allowedPriceByTypes = new List<PriceByType>(_activePriceByTypes)
                {
                    (PriceByType)Enum.Parse(typeof(PriceByType), quotePart.PriceBy)
                };

                cboPriceBy.DataSource = allowedPriceByTypes
                    .Distinct()
                    .ToList();

                cboPriceBy.DataBindings[0].ReadValue(); // Required; otherwise, value can be blanked out

                CurrentRow = quotePart;

                var priceData = quotePart.GetQuotePartPriceRows()
                    .OrderByDescending(row => row.QuotePartPriceID)
                    .FirstOrDefault();

                if (priceData == null)
                {
                    // Empty cost/markup fields
                    numRate.Value = 0M;
                    curLaborCost.Value = 0M;
                    curMaterialCost.Value = 0M;
                    curOverheadCost.Value = 0M;
                    curMarkupTotal.Value = 0M;
                    curTargetPrice.ValueObject = null;
                }
                else
                {
                    numRate.Value = priceData.Rate;
                    curLaborCost.Value = priceData.LaborCost;
                    curMaterialCost.Value = priceData.MaterialCost;
                    curOverheadCost.Value = priceData.OverheadCost;
                    curMarkupTotal.Value = priceData.MarkupTotal;

                    curTargetPrice.ValueObject = priceData.IsTargetPriceNull() ?
                        (decimal?)null :
                        priceData.TargetPrice;
                }

                CurrentPriceData = priceData;
                UpdateTotalCost();
            }
            finally
            {
                AddValueChangedHandlers();
            }
        }

        public void SaveRow()
        {
            try
            {
                var quotePart = CurrentRow as QuoteDataSet.QuotePartRow;

                if (quotePart == null || !quotePart.IsValidState())
                {
                    return;
                }
                else if (_dtQuotePrice == null)
                {
                    throw new InvalidOperationException("This instance is not setup to add price data for quotes.");
                }

                var rate = (numRate.Value as decimal?) ?? 0M;
                decimal laborCost = curLaborCost.Value;
                decimal materialCost = curMaterialCost.Value;
                decimal overheadCost = curOverheadCost.Value;
                decimal markupTotal = curMarkupTotal.Value;
                var targetPrice = curTargetPrice.ValueObject as decimal?;

                bool allZeroValues = laborCost == 0M &&
                    materialCost == 0M &&
                    overheadCost == 0M &&
                    markupTotal == 0M;

                // Update current row, or add a new one UNLESS
                // the user has not set any values.
                if (CurrentPriceData != null || !allZeroValues)
                {
                    if (CurrentPriceData == null)
                    {
                        // Add new price data
                        CurrentPriceData = _dtQuotePrice.NewQuotePartPriceRow();
                        CurrentPriceData.QuotePartRow = quotePart;
                        _dtQuotePrice.AddQuotePartPriceRow(CurrentPriceData);
                    }

                    CurrentPriceData.Rate = rate;
                    CurrentPriceData.LaborCost = laborCost;
                    CurrentPriceData.MaterialCost = materialCost;
                    CurrentPriceData.OverheadCost = overheadCost;
                    CurrentPriceData.MarkupTotal = markupTotal;

                    if (targetPrice.HasValue)
                    {
                        CurrentPriceData.TargetPrice = targetPrice.Value;
                    }
                    else
                    {
                        CurrentPriceData.SetTargetPriceNull();
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error saving quote price data.");
            }
        }

        public void Setup(BindingSource bsData,
            QuoteDataSet.QuotePartPriceDataTable dtPrice,
            QuoteDataSet.QuotePartPriceCalculationDataTable dtPriceCalculation,
            IEnumerable<PriceByType> activePriceByTypes)
        {
            var currencyMask = "{currency:6." + ApplicationSettings.Current.PriceDecimalPlaces + "}";
            curEachPrice.MaskInput = currencyMask;
            curLotPrice.MaskInput = currencyMask;
            curMaterialCost.MaskInput = currencyMask;
            curLaborCost.MaskInput = currencyMask;
            curOverheadCost.MaskInput = currencyMask;
            curMarkupTotal.MaskInput = currencyMask;
            curTargetPrice.MaskInput = currencyMask;


            // Bind Each/Lot Prices to QuotePart
            const string eachPricePropertyName = nameof(QuoteDataSet.QuotePartRow.EachPrice);
            const string lotPricePropertyName = nameof(QuoteDataSet.QuotePartRow.LotPrice);
            curEachPrice.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(curEachPrice), bsData, eachPricePropertyName));
            curLotPrice.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(curLotPrice), bsData, lotPricePropertyName));

            _dtQuotePrice = dtPrice;
            _dtQuotePriceCalculation = dtPriceCalculation;

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
