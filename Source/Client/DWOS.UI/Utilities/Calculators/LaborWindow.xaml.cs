using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DWOS.UI.Utilities.Calculators
{
    /// <summary>
    /// Interaction logic for LaborWindow.xaml
    /// </summary>
    public partial class LaborWindow : CalculatorWindow
    {
        #region Properties

        protected override object Result
        {
            get
            {
                var context = DataContext as LaborContext;

                if (context == null)
                {
                    LogManager
                        .GetCurrentClassLogger()
                        .Warn("context was null - returning default value.");

                    return 0M;
                }
                else
                {
                    return context.HourlyLaborCostPerPart;
                }
            }
        }

        public override string CalculationType
        {
            get
            {
                return TYPE_LABOR_V1;
            }
        }

        public override string JsonData
        {
            get
            {
                var context = DataContext as LaborContext;

                if (context == null)
                {
                    return string.Empty;
                }

                var items = context.LaborItems
                    .Select(i => LaborItemJsonData.CreateFrom(i))
                    .ToList();

                return JsonConvert.SerializeObject(items);
            }
        }

        #endregion

        #region Methods

        public LaborWindow(CalculatorData calculatorData)
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
            var model = new LaborContext();
            model.PartsPerHour = CalculatorData.PartsPerHour;
            model.Accept += (object eventSender, EventArgs eventArgs) =>
            {
                DialogResult = true;
            };

            List<LaborItemJsonData> jsonItems = null;
            if (!string.IsNullOrEmpty(CalculatorData.JsonData))
            {
                try
                {
                    jsonItems = JsonConvert.DeserializeObject<List<LaborItemJsonData>>(CalculatorData.JsonData);
                }
                catch (Exception exc)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(JSON_SERIALIZATION_ERROR, Title);
                    LogManager.GetCurrentClassLogger().Warn(exc, "Error deserializing JSON.");
                }
            }


            foreach (var jsonItem in jsonItems ?? Enumerable.Empty<LaborItemJsonData>())
            {
                var laborItem = new LaborContext.LaborItem()
                {
                    Count = jsonItem.Count,
                    LaborType = jsonItem.LaborType,
                    Wage = jsonItem.Wage
                };

                model.LaborItems.Add(laborItem);
            }

            DataContext = model;
        }

        #endregion

        #region LaborItemJsonData

        private sealed class LaborItemJsonData
        {
            #region Properties

            public string LaborType { get; set; }

            public decimal Wage { get; set; }

            public int Count { get; set; }

            #endregion

            #region Methods

            public static LaborItemJsonData CreateFrom(LaborContext.LaborItem item)
            {
                if (item == null)
                {
                    return null;
                }

                return new LaborItemJsonData()
                {
                    Count = item.Count,
                    LaborType = item.LaborType,
                    Wage = item.Wage
                };
            }

            #endregion
        }

        #endregion
    }
}
