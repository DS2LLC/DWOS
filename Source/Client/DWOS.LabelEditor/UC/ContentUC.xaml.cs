using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Neodynamic.SDK.Printing;
using NLog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for ContentUC.xaml
    /// </summary>
    public partial class ContentUC :  UserControl, INotifyPropertyChanged
    {
        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;

        private string _barcodeFormat = null;
        private string _barcodePreview = null;

        #endregion

        #region Properties

        public List<Token> Tokens { get; set; }

        public string Code { get; set; }

        public string BarcodeFormat
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_barcodeFormat))
                    _barcodeFormat = "{0}";

                return _barcodeFormat;
            }

            set
            {
                _barcodeFormat = string.IsNullOrWhiteSpace(value) ? "{0}" : value;
                OnPropertyChanged(nameof(BarcodeFormat));
            }
        }

        public string BarcodePreview
        {
            get { return _barcodePreview; }
            set
            {
                _barcodePreview = value;
                OnPropertyChanged(nameof(BarcodePreview));
            }
        }

        public string MultifieldContent
        {
            get
            {
                return this.txtMultifieldContent.Text;
            }

            set
            {
                this.txtMultifieldContent.Text = value;
                OnPropertyChanged(nameof(MultifieldContent));
            }
        }

        public string MultifieldPreview
        {
            get { return this.txtMultifieldsPreview.Text; }
        }

        #endregion

        #region Methods

        public ContentUC()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #endregion

        #region Events

        private void btnAddToken_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddTokenDialog tokenDialog = new AddTokenDialog(this.Tokens);
                tokenDialog.Owner = Window.GetWindow(this);

                if (tokenDialog.ShowDialog() == true)
                {
                    var token = tokenDialog.SelectedToken;
                    txtMultifieldContent.Text = txtMultifieldContent.Text.Insert(txtMultifieldContent.SelectionStart, "%" + token.ID + "%");
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error adding token.");
            }
        }

        private void txtFormat_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.BarcodePreview = string.Format(txtFormat.Text, this.Code);
            }
            catch (FormatException)
            {
                BarcodePreview = "Error - Invalid Format";
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing format.");
            }
        }

        private void txtMultifieldContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.txtMultifieldsPreview.Text = Token.ReplaceTokens(txtMultifieldContent.Text, this.Tokens);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing multifield content.");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
