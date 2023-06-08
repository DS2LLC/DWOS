using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DWOS.UI.Utilities.Calculators
{
    /// <summary>
    /// Interaction logic for OverheadWindow.xaml
    /// </summary>
    public partial class OverheadWindow : CalculatorWindow
    {
        #region Properties

        public override string CalculationType
        {
            get
            {
                return TYPE_OVERHEAD_V1;
            }
        }

        public override string JsonData
        {
            get
            {
                var jsonData = OverheadJsonData.CreateFrom(DataContext as OverheadContext);

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

        protected override object Result
        {
            get
            {
                var context = DataContext as OverheadContext;

                if (context == null)
                {
                    LogManager
                        .GetCurrentClassLogger()
                        .Warn("context was null - returning default value.");

                    return 0M;
                }
                else
                {
                    return context.HourlyOverheadCostPerPart;
                }
            }
        }

        #endregion

        #region Methods

        public OverheadWindow(CalculatorData calculatorData)
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
            var model = new OverheadContext();
            model.PartsPerHour = CalculatorData.PartsPerHour;
            model.Accept += (object eventSender, EventArgs eventArgs) =>
            {
                DialogResult = true;
            };

            OverheadJsonData jsonData = null;
            if (!string.IsNullOrEmpty(CalculatorData.JsonData))
            {
                try
                {
                    jsonData = JsonConvert.DeserializeObject<OverheadJsonData>(CalculatorData.JsonData);
                }
                catch (Exception exc)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(JSON_SERIALIZATION_ERROR, Title);
                    LogManager.GetCurrentClassLogger().Warn(exc, "Error deserializing JSON.");
                }
            }

            if (jsonData != null)
            {
                model.HourlyOverheadCost = jsonData.HourlyOverheadCost;
            }

            DataContext = model;
        }

        private void XamNumericEditor_EditModeStarted(object sender, Infragistics.Windows.Editors.Events.EditModeStartedEventArgs e)
        {
            var xamEditor = sender as Infragistics.Windows.Editors.XamMaskedEditor;
            xamEditor?.SelectAll();
        }

        #endregion

        #region OverheadJsonData

        private class OverheadJsonData
        {
            #region Properties

            public decimal HourlyOverheadCost { get; set; }

            #endregion

            #region Methods

            public static OverheadJsonData CreateFrom(OverheadContext context)
            {
                if (context == null)
                {
                    return null;
                }

                return new OverheadJsonData()
                {
                    HourlyOverheadCost = context.HourlyOverheadCost
                };
            }

            #endregion
        }

        #endregion
    }
}
