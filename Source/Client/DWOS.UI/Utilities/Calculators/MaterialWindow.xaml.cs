using Newtonsoft.Json;
using NLog;
using System;
using System.Windows;

namespace DWOS.UI.Utilities.Calculators
{
    /// <summary>
    /// Interaction logic for MaterialWindow.xaml
    /// </summary>
    public partial class MaterialWindow : CalculatorWindow
    {
        #region Properties

        public override string CalculationType
        {
            get
            {
                return TYPE_MATERIAL_V1;
            }
        }

        public override string JsonData
        {
            get
            {
                var jsonData = MaterialJsonData.CreateFrom(DataContext as MaterialContext);

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
                var context = DataContext as MaterialContext;

                if (context == null)
                {
                    LogManager
                        .GetCurrentClassLogger()
                        .Warn("context was null - returning default value.");

                    return 0M;
                }
                else
                {
                    return context.MaterialCostPerPart;
                }
            }
        }

        #endregion

        #region Methods

        public MaterialWindow(CalculatorData calculatorData)
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
            var model = new MaterialContext();
            model.PartSquareFoot = CalculatorData.SurfaceArea / 144D; // Get square feet

            model.Accept += (object eventSender, EventArgs eventArgs) =>
            {
                DialogResult = true;
            };

            MaterialJsonData jsonData = null;
            if (!string.IsNullOrEmpty(CalculatorData.JsonData))
            {
                try
                {
                    jsonData = JsonConvert.DeserializeObject<MaterialJsonData>(CalculatorData.JsonData);
                }
                catch (Exception exc)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(JSON_SERIALIZATION_ERROR, Title);
                    LogManager.GetCurrentClassLogger().Warn(exc, "Error deserializing JSON.");
                }
            }

            if (jsonData != null)
            {
                model.SquareFootMaterialCost = jsonData.SquareFootMaterialCost;
                if (jsonData.PartSquareFoot != model.PartSquareFoot)
                {
                    areaChangedWarning.Visibility = Visibility.Visible;
                    Height += 20;// Expand window to show all content
                }
            }

            DataContext = model;
        }

        private void XamNumericEditor_EditModeStarted(object sender, Infragistics.Windows.Editors.Events.EditModeStartedEventArgs e)
        {
            var xamEditor = sender as Infragistics.Windows.Editors.XamMaskedEditor;
            xamEditor?.SelectAll();
        }

        #endregion

        #region MaterialJsonData

        private class MaterialJsonData
        {
            #region Properties

            public double PartSquareFoot
            {
                get; set;
            }

            public decimal SquareFootMaterialCost
            {
                get; set;
            }

            #endregion

            #region Methods

            public static MaterialJsonData CreateFrom(MaterialContext context)
            {
                if (context == null)
                {
                    return null;
                }

                return new MaterialJsonData()
                {
                    PartSquareFoot = context.PartSquareFoot,
                    SquareFootMaterialCost = context.SquareFootMaterialCost
                };
            }

            #endregion
        }

        #endregion

    }
}
