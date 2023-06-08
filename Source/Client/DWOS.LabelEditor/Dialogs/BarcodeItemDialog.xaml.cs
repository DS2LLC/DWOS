using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;
using Neodynamic.SDK.Printing;
using System.Linq;

namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for BarcodeItemDialog.xaml
    /// </summary>
    public partial class BarcodeItemDialog : Window
    {
        #region Fields

        private BarcodeItem _bcItem = null;
        private List<Token> _tokens = null;

        #endregion

        #region Properties

        public List<Token> Tokens
        {
            get { return _tokens; }
            set
            {
                contentUC1.Tokens = _tokens = value;
            }
        }

        /// <summary>
        /// Gets or sets barcode settings.
        /// </summary>
        public BarcodeItem BarcodeItem
        {
            get
            {
                const double minModuleSize = 0.0001;

                //set properties based on dialog inputs
                _bcItem.Comments = generalUC1.ItemComments;
                _bcItem.CornerRadius = new RectangleCornerRadius(strokeFillUC1.ItemCornerRadius);
                _bcItem.BackColor = strokeFillUC1.ItemFillColor;
                _bcItem.Height = sizeUC1.ItemHeight;
                _bcItem.Width = sizeUC1.ItemWidth;
                _bcItem.Name = generalUC1.ItemName;
                _bcItem.PrintAsGraphic = generalUC1.PrintAsGraphic;
                _bcItem.BorderColor = strokeFillUC1.ItemStrokeColor;
                _bcItem.BorderThickness = new FrameThickness(strokeFillUC1.ItemStrokeThickness);
                _bcItem.X = positionUC1.ItemX;
                _bcItem.Y = positionUC1.ItemY;
                _bcItem.RotationAngle = sizeUC1.ItemRotationAngle;
                _bcItem.Font.UpdateFrom(fontUC1.GetFont());
                _bcItem.ForeColor = (Neodynamic.SDK.Printing.Color)Enum.Parse(typeof(Neodynamic.SDK.Printing.Color), cboForeColor.SelectedValue.ToString());
                _bcItem.Sizing = (Neodynamic.SDK.Printing.BarcodeSizing)Enum.Parse(typeof(Neodynamic.SDK.Printing.BarcodeSizing), cboBarcodeSizing.SelectedValue.ToString());
                _bcItem.BarcodeAlignment = (Neodynamic.SDK.Printing.BarcodeAlignment)Enum.Parse(typeof(Neodynamic.SDK.Printing.BarcodeAlignment), cboBarcodeAlignment.SelectedValue.ToString());
                _bcItem.DataField = dataBindingUC1.ItemDataField;
                _bcItem.DataFieldFormatString = dataBindingUC1.ItemDataFieldFormatString;
                _bcItem.Symbology = barcodeUC1.BarcodeSymbology;
                _bcItem.Code = barcodeUC1.BarcodeCode;
                _bcItem.BarWidth = barcodeUC1.BarcodeBarWidth;
                _bcItem.BarRatio = barcodeUC1.BarcodeBarRatio;
                _bcItem.BarHeight = barcodeUC1.BarcodeBarHeight;
                _bcItem.DisplayCode = barcodeUC1.BarcodeDisplayCode;

                var moduleSize = Math.Max(barcodeUC1.BarcodeModuleSize, minModuleSize);
                _bcItem.HanXinCodeModuleSize = _bcItem.AztecCodeModuleSize = _bcItem.DataMatrixModuleSize = _bcItem.QRCodeModuleSize = moduleSize;

                if (contentUC1.chkUseTokens.IsChecked.GetValueOrDefault() && String.IsNullOrWhiteSpace(_bcItem.Name))
                {
                    // Tag is used for multifields in barcode
                    _bcItem.Tag = JsonConvert.ToString(contentUC1.MultifieldContent);
                    _bcItem.Code = contentUC1.MultifieldPreview;
                }
                else
                {
                    _bcItem.CodeFormatPattern = contentUC1.BarcodeFormat;
                }

                return _bcItem;
            }
            set 
            {
                _bcItem = value.Clone() as BarcodeItem;

                //set barcode
                barcodeUC1.BarcodeSymbology = _bcItem.Symbology;
                barcodeUC1.BarcodeCode = _bcItem.Code;
                barcodeUC1.BarcodeBarWidth = _bcItem.BarWidth;
                barcodeUC1.BarcodeBarRatio = _bcItem.BarRatio;
                barcodeUC1.BarcodeBarHeight = _bcItem.BarHeight;
                barcodeUC1.BarcodeDisplayCode = _bcItem.DisplayCode;

                txtTokenName.Text = String.IsNullOrWhiteSpace(_bcItem.Name) ? "None" : _bcItem.Name;

                // Set module size - use largest size from available fields
                barcodeUC1.BarcodeModuleSize = new double[] {
                    _bcItem.AztecCodeModuleSize,
                    _bcItem.DataMatrixModuleSize,
                    _bcItem.QRCodeModuleSize,
                    _bcItem.HanXinCodeModuleSize
                }.Max();

                //set font
                fontUC1.SetFont(_bcItem.Font);

                //set fill & stroke                
                strokeFillUC1.ItemFillColor = _bcItem.BackColor;
                //for simplicity we are going to use uniform corner radius (but you can change it if needed!)
                strokeFillUC1.ItemCornerRadius = _bcItem.CornerRadius.TopLeft;
                strokeFillUC1.ItemStrokeColor = _bcItem.BorderColor;
                //for simplicity we are going to use uniform border thickness (but you can change it if needed!)
                strokeFillUC1.ItemStrokeThickness = _bcItem.BorderThickness.Left;

                cboForeColor.SelectedItem = _bcItem.ForeColor.ToString();

                //set position & size tab
                positionUC1.ItemX = _bcItem.X;
                positionUC1.ItemY = _bcItem.Y;
                sizeUC1.ItemWidth = _bcItem.Width;
                sizeUC1.ItemHeight = _bcItem.Height;
                sizeUC1.ItemRotationAngle = _bcItem.RotationAngle;

                cboBarcodeSizing.SelectedItem = _bcItem.Sizing.ToString();
                cboBarcodeAlignment.SelectedItem = _bcItem.BarcodeAlignment.ToString();

                //set data binding
                dataBindingUC1.ItemDataField = _bcItem.DataField;
                dataBindingUC1.ItemDataFieldFormatString = _bcItem.DataFieldFormatString;

                //set general tab
                generalUC1.ItemName = _bcItem.Name;
                generalUC1.ItemComments = _bcItem.Comments;

                //set content tab - based on if it's for an actual token or just a generic barcode
                if (String.IsNullOrWhiteSpace(_bcItem.Name))
                {
                    contentUC1.grdGeneric.Visibility = Visibility.Visible;
                    contentUC1.grdToken.Visibility = Visibility.Collapsed;

                    // Tag is used for multifields in barcode
                    var json = _bcItem.Tag;
                    contentUC1.MultifieldContent = JsonConvert.DeserializeObject<string>(json);

                    if (!string.IsNullOrWhiteSpace(_bcItem.Tag))
                        contentUC1.chkUseTokens.IsChecked = true;
                }
                else
                {
                    contentUC1.grdGeneric.Visibility = Visibility.Collapsed;
                    contentUC1.grdToken.Visibility = Visibility.Visible;
                    contentUC1.Code = _bcItem.Code;
                    contentUC1.BarcodeFormat = _bcItem.CodeFormatPattern;
                }
            }
        }

        #endregion

        #region Methods

        public BarcodeItemDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboForeColor.ItemsSource = Enum.GetNames(typeof(Neodynamic.SDK.Printing.Color));
            cboBarcodeAlignment.ItemsSource = Enum.GetNames(typeof(Neodynamic.SDK.Printing.BarcodeAlignment));
            cboBarcodeSizing.ItemsSource = Enum.GetNames(typeof(Neodynamic.SDK.Printing.BarcodeSizing));

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion
    }
}
