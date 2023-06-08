using System;
using System.Windows;
using System.Windows.Controls;


namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for LabelDoc.xaml
    /// </summary>
    public partial class LabelDoc : Window
    {
        #region Fields

        Neodynamic.SDK.Printing.UnitType _currentLabelUnit = Neodynamic.SDK.Printing.UnitType.Inch;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the selected unit type.
        /// </summary>
        public Neodynamic.SDK.Printing.UnitType LabelUnit
        {
            get { return _currentLabelUnit; }
            set
            {
                _currentLabelUnit = value;
                cboUnit.SelectedItem = _currentLabelUnit.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the selected label orientation.
        /// </summary>
        public Neodynamic.SDK.Printing.PrintOrientation LabelOrientation
        {
            get
            {
                if (cboOrientation.SelectedItem != null)
                    return (Neodynamic.SDK.Printing.PrintOrientation)Enum.Parse(typeof(Neodynamic.SDK.Printing.PrintOrientation), cboOrientation.Text);

                return Neodynamic.SDK.Printing.PrintOrientation.Portrait;
            }
            set
            {
                cboOrientation.SelectedItem = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the selected label width.
        /// </summary>
        public double LabelWidth
        {
            get { return double.Parse(txtWidth.Text); }
            set { txtWidth.Text = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets the selected label height.
        /// </summary>
        public double LabelHeight
        {
            get { return double.Parse(txtHeight.Text); }
            set { txtHeight.Text = value.ToString(); }
        }

        #endregion

        #region Methods

        public LabelDoc()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboUnit.DataContext = Enum.GetNames(typeof(Neodynamic.SDK.Printing.UnitType));
            cboOrientation.DataContext = Enum.GetNames(typeof(Neodynamic.SDK.Printing.PrintOrientation));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cboUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Neodynamic.SDK.Printing.UnitType newUnit = (Neodynamic.SDK.Printing.UnitType)Enum.Parse(typeof(Neodynamic.SDK.Printing.UnitType), cboUnit.SelectedItem.ToString());
            txtWidth.Text = Neodynamic.Windows.ThermalLabelEditor.UnitUtils.Convert(_currentLabelUnit, double.Parse(txtWidth.Text), newUnit, 2).ToString();
            txtHeight.Text = Neodynamic.Windows.ThermalLabelEditor.UnitUtils.Convert(_currentLabelUnit, double.Parse(txtHeight.Text), newUnit, 2).ToString();
            _currentLabelUnit = newUnit;

        }

        #endregion
    }
}
