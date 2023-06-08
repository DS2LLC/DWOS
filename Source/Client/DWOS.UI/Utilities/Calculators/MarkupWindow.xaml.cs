using DWOS.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DWOS.UI.Utilities.Calculators
{
    /// <summary>
    /// Interaction logic for MarkupWindow.xaml
    /// </summary>
    public partial class MarkupWindow : CalculatorWindow
    {
        #region Properties

        public override string CalculationType
        {
            get
            {
                return TYPE_MARKUP_V1;
            }
        }

        public override string JsonData
        {
            get
            {
                var context = DataContext as MarkupContext;

                if (context == null)
                {
                    return string.Empty;
                }

                var items = context.MarkupItems
                    .Select(i => MarkupItemJsonData.CreateFrom(i))
                    .ToList();

                return JsonConvert.SerializeObject(items);
            }
        }

        protected override object Result
        {
            get
            {
                var context = DataContext as MarkupContext;

                if (context == null)
                {
                    LogManager
                        .GetCurrentClassLogger()
                        .Warn("context was null - returning default value.");

                    return 0M;
                }
                else
                {
                    return context.TotalMarkup;
                }
            }
        }

        #endregion

        #region Methods

        public MarkupWindow(CalculatorData calculatorData)
            : base(calculatorData)
        {
            InitializeComponent();
        }

        protected override bool SupportsResultType(Type resultType)
        {
            return resultType == typeof(decimal);
        }

        #endregion

        #region Events

        private void CalculatorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var model = new MarkupContext();
            model.LaborCost = CalculatorData.LaborCost;
            model.MaterialCost = CalculatorData.MaterialCost;
            model.OverheadCost = CalculatorData.OverheadCost;

            List<MarkupItemJsonData> jsonItems = null;
            if (!string.IsNullOrEmpty(CalculatorData.JsonData))
            {
                try
                {
                    jsonItems = JsonConvert.DeserializeObject<List<MarkupItemJsonData>>(CalculatorData.JsonData);
                }
                catch (Exception exc)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(JSON_SERIALIZATION_ERROR, Title);
                    LogManager.GetCurrentClassLogger().Warn(exc, "Error deserializing JSON.");
                }
            }


            foreach (var jsonItem in jsonItems ?? Enumerable.Empty<MarkupItemJsonData>())
            {
                var markupItem = new MarkupContext.MarkupItem()
                {
                    Name = jsonItem.Name,
                    Amount = jsonItem.Amount,
                    MarkupType = jsonItem.MarkupType
                };

                model.MarkupItems.Add(markupItem);
            }

            DataContext = model;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void DataGrid_InitializeRecord(object sender,Infragistics.Windows.DataPresenter.Events.InitializeRecordEventArgs e)
        {
            var dataRecord = e.Record as Infragistics.Windows.DataPresenter.DataRecord;
            if (dataRecord == null)
            {
                return;
            }

            var item = dataRecord.DataItem as MarkupContext.MarkupItem;

            if (item != null)
            {
                var cell = dataRecord.Cells[nameof(item.Amount)];

                if (cell == null)
                {
                    LogManager.GetCurrentClassLogger().Warn("Amount cell not found");
                }
                else if (item.MarkupType == MarkupType.Fixed)
                {
                    // Display as currency
                    cell.SetEditorSettings(typeof(Infragistics.Windows.Editors.XamCurrencyEditor),
                        Resources["currencyEditor"] as Style,
                        null);
                }
                else if (item.MarkupType == MarkupType.Percentage)
                {
                    // Display as percentage
                    cell.SetEditorSettings(typeof(Infragistics.Windows.Editors.XamNumericEditor),
                        Resources["percentEditor"] as Style,
                        typeof(decimal));
                }
            }
        }

        #endregion

        #region MarkupItemJsonData

        private sealed class MarkupItemJsonData
        {
            #region Properties

            public string Name { get; set; }

            public decimal Amount { get; set; }

            [JsonConverter(typeof(StringEnumConverter))]
            public MarkupType MarkupType { get; set; }

            #endregion

            #region Methods

            public static MarkupItemJsonData CreateFrom(MarkupContext.MarkupItem item)
            {
                if (item == null)
                {
                    return null;
                }

                return new MarkupItemJsonData()
                {
                    Name = item.Name,
                    Amount = item.Amount,
                    MarkupType = item.MarkupType
                };
            }

            #endregion
        }

        #endregion
    }
}
