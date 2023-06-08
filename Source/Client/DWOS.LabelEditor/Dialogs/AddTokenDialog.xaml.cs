using System.Collections.Generic;
using System.Windows;

namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for AddTokenDialog.xaml
    /// </summary>
    public partial class AddTokenDialog : Window
    {
        #region Properties

        public Token SelectedToken
        {
            get
            {
                if(lstTokens.Items.Count > 0)
                    return this.lstTokens.SelectedItem as Token;

                return null;
            }
        }

        #endregion

        #region Methods

        public AddTokenDialog(List<Token> tokens)
        {
            InitializeComponent();

            this.lstTokens.ItemsSource = tokens;
            if (lstTokens.Items.Count > 0)
                lstTokens.SelectedIndex = 0;
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion
    }
}
