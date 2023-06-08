using System;
using System.Windows;

using Neodynamic.SDK.Printing;

namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for TextItemDialog.xaml
    /// </summary>
    public partial class TextItemDialog : Window
    {
        #region Fields

        private TextItem _textItem = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the selected text settings.
        /// </summary>
        public TextItem TextItem
        {
            get
            {
                //set properties based on dialog inputs
                _textItem.Comments = generalUC1.ItemComments;
                _textItem.CornerRadius = new RectangleCornerRadius(strokeFillUC1.ItemCornerRadius);
                _textItem.BackColor = strokeFillUC1.ItemFillColor;
                _textItem.Height = sizeUC1.ItemHeight;
                _textItem.Width = sizeUC1.ItemWidth;
                _textItem.Name = generalUC1.ItemName;
                _textItem.PrintAsGraphic = generalUC1.PrintAsGraphic;
                _textItem.BorderColor = strokeFillUC1.ItemStrokeColor;
                _textItem.BorderThickness = new FrameThickness(strokeFillUC1.ItemStrokeThickness);
                _textItem.X = positionUC1.ItemX;
                _textItem.Y = positionUC1.ItemY;
                _textItem.RotationAngle = sizeUC1.ItemRotationAngle;
                _textItem.Font.UpdateFrom(fontUC1.GetFont());
                _textItem.ForeColor = (Neodynamic.SDK.Printing.Color)Enum.Parse(typeof(Neodynamic.SDK.Printing.Color), cboForeColor.SelectedValue.ToString());
                _textItem.Sizing = (Neodynamic.SDK.Printing.TextSizing)Enum.Parse(typeof(Neodynamic.SDK.Printing.TextSizing), cboTextSizing.SelectedValue.ToString());
                _textItem.DataField = dataBindingUC1.ItemDataField;
                _textItem.DataFieldFormatString = dataBindingUC1.ItemDataFieldFormatString;
                _textItem.TextAlignment = (Neodynamic.SDK.Printing.TextAlignment)Enum.Parse(typeof(Neodynamic.SDK.Printing.TextAlignment), cboAlignment.SelectedValue.ToString());
                _textItem.TextPadding = new FrameThickness(String.IsNullOrWhiteSpace(txtPaddingLeft.Text) ? 0 : Convert.ToDouble(txtPaddingLeft.Text), String.IsNullOrWhiteSpace(txtPaddingTop.Text) ? 0 : Convert.ToDouble(txtPaddingTop.Text), String.IsNullOrWhiteSpace(txtPaddingRight.Text) ? 0 : Convert.ToDouble(txtPaddingRight.Text), String.IsNullOrWhiteSpace(txtPaddingBottom.Text) ? 0 : Convert.ToDouble(txtPaddingBottom.Text));

                return _textItem;
            }
            set
            {
                _textItem = value.Clone() as TextItem;

                //set font
                fontUC1.SetFont(_textItem.Font);

                txtTokenName.Text = String.IsNullOrWhiteSpace(_textItem.Name) ? "None" : _textItem.Name;
                //set fill & stroke                
                strokeFillUC1.ItemFillColor = _textItem.BackColor;
                //for simplicity we are going to use uniform corner radius (but you can change it if needed!)
                strokeFillUC1.ItemCornerRadius = _textItem.CornerRadius.TopLeft;
                strokeFillUC1.ItemStrokeColor = _textItem.BorderColor;
                //for simplicity we are going to use uniform border thickness (but you can change it if needed!)
                strokeFillUC1.ItemStrokeThickness = _textItem.BorderThickness.Left;

                cboForeColor.SelectedItem = _textItem.ForeColor.ToString();

                //set position & size tab
                positionUC1.ItemX = _textItem.X;
                positionUC1.ItemY = _textItem.Y;
                sizeUC1.ItemWidth = _textItem.Width;
                sizeUC1.ItemHeight = _textItem.Height;
                sizeUC1.ItemRotationAngle = _textItem.RotationAngle;

                cboTextSizing.SelectedItem = _textItem.Sizing.ToString();
                cboAlignment.SelectedItem = _textItem.TextAlignment.ToString();

                txtPaddingLeft.Text = _textItem.TextPadding.Left.ToString();
                txtPaddingTop.Text = _textItem.TextPadding.Top.ToString();
                txtPaddingRight.Text = _textItem.TextPadding.Right.ToString();
                txtPaddingBottom.Text = _textItem.TextPadding.Bottom.ToString();

                //set data binding
                dataBindingUC1.ItemDataField = _textItem.DataField;
                dataBindingUC1.ItemDataFieldFormatString = _textItem.DataFieldFormatString;

                //set general tab
                generalUC1.ItemName = _textItem.Name;
                generalUC1.ItemComments = _textItem.Comments;

            }
        }

        #endregion

        #region Methods

        public TextItemDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboForeColor.ItemsSource = Enum.GetNames(typeof(Neodynamic.SDK.Printing.Color));
            cboTextSizing.ItemsSource = Enum.GetNames(typeof(Neodynamic.SDK.Printing.TextSizing));
            cboAlignment.ItemsSource = Enum.GetNames(typeof(Neodynamic.SDK.Printing.TextAlignment));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion
    }
}
