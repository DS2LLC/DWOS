using Newtonsoft.Json;
using NLog;
using System;
using System.Windows;

namespace DWOS.UI.Utilities.Calculators
{
    /// <summary>
    /// Interaction logic for RateWindow.xaml
    /// </summary>
    public partial class RateWindow : CalculatorWindow
    {
        #region Properties

        protected override object Result
        {
            get
            {
                var context = DataContext as RateContext;
                return context?.PartsPerHour ?? 0;
            }
        }
        public override string CalculationType
        {
            get
            {
                return TYPE_RATE_V1;
            }
        }

        public override string JsonData
        {
            get
            {
                var jsonData = RateJsonData.CreateFrom(DataContext as RateContext);

                if (jsonData == null)
                {
                    return string.Empty;
                }
                else
                {
                    return JsonConvert.SerializeObject(jsonData);
                }
            }
        }

        #endregion

        #region Methods

        public RateWindow(CalculatorData calculatorData)
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
            var model = new RateContext();

            RateJsonData jsonData = null;
            if (!string.IsNullOrEmpty(CalculatorData.JsonData))
            {
                try
                {
                    jsonData = JsonConvert.DeserializeObject<RateJsonData>(CalculatorData.JsonData);
                }
                catch (Exception exc)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(JSON_SERIALIZATION_ERROR, Title);
                    LogManager.GetCurrentClassLogger().Warn(exc, "Error deserializing JSON.");
                }
            }

            if (jsonData != null)
            {
                model.BarsPerRack = jsonData.BarsPerRack;
                model.FeetPerMinute = jsonData.FeetPerMinute;
                model.PartsPerBar = jsonData.PartsPerBar;
                model.RackSpacing = jsonData.RackSpacing;
            }

            DataContext = model;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Editor_EditModeStarted(object sender, Infragistics.Windows.Editors.Events.EditModeStartedEventArgs e)
        {
            var xamEditor = sender as Infragistics.Windows.Editors.XamMaskedEditor;
            xamEditor?.SelectAll();
        }

        #endregion

        #region RateJsonData

        private sealed class RateJsonData
        {
            #region Properties

            public decimal FeetPerMinute { get; set; }

            public decimal RackSpacing { get; set; }

            public int BarsPerRack { get; set; }

            public int PartsPerBar { get; set; }

            #endregion

            #region Methods

            public static RateJsonData CreateFrom(RateContext context)
            {
                if (context == null)
                {
                    return null;
                }

                return new RateJsonData()
                {
                    BarsPerRack = context.BarsPerRack,
                    FeetPerMinute = context.FeetPerMinute,
                    PartsPerBar = context.PartsPerBar,
                    RackSpacing = context.RackSpacing
                };
            }

            #endregion
        }

        #endregion


    }
}
