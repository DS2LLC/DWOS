using System;
using System.Windows;
using System.Windows.Controls;

namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for StrokeFillUC.xaml
    /// </summary>
    public partial class StrokeFillUC : UserControl
    {
        #region Properties

        public Neodynamic.SDK.Printing.Color ItemStrokeColor
        {
            get
            {
                return (Neodynamic.SDK.Printing.Color)Enum.Parse(typeof(Neodynamic.SDK.Printing.Color), cboStrokeColor.SelectedValue.ToString());
                
            }
            set
            {
                cboStrokeColor.SelectedItem = value.ToString();
            }
        }
        
        public Neodynamic.SDK.Printing.Color ItemFillColor
        {
            get
            {
                return (Neodynamic.SDK.Printing.Color)Enum.Parse(typeof(Neodynamic.SDK.Printing.Color), cboFillColor.SelectedValue.ToString());

            }
            set
            {
                cboFillColor.SelectedItem = value.ToString();
            }
        }

        public double ItemStrokeThickness
        {
            get
            {
                try
                {
                    return double.Parse(txtStrokeThickness.Text);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                txtStrokeThickness.Text = value.ToString();
            }
        }

        public double ItemCornerRadius
        {
            get
            {
                try
                {
                    return double.Parse(txtCornerRadius.Text);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                txtCornerRadius.Text = value.ToString();
            }
        }

        public Visibility CornerRadiusOptionVisibility
        {
            get
            {
                return gbRoundedCorners.Visibility;
            }
            set
            {
                gbRoundedCorners.Visibility = value;
            }
        }

        public Visibility FillOptionVisibility
        {
            get
            {
                return gbFill.Visibility;
            }
            set
            {
                gbFill.Visibility = value;
            }
        }

        #endregion

        #region Methods

        public StrokeFillUC()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //load colors
            cboFillColor.ItemsSource = Enum.GetNames(typeof(Neodynamic.SDK.Printing.Color));
            cboStrokeColor.ItemsSource = Enum.GetNames(typeof(Neodynamic.SDK.Printing.Color));
        }

        #endregion

    }

}
