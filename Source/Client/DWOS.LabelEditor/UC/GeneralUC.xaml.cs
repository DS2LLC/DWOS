using System.Windows.Controls;

namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for GeneralUC.xaml
    /// </summary>
    public partial class GeneralUC : UserControl
    {
        #region Properties

        public string ItemName
        {
            get
            {
                return txtItemName.Text;
            }
            set
            {
                txtItemName.Text = value;
            }
        }

        public string ItemComments
        {
            get
            {
                return txtItemComments.Text;
            }
            set
            {
                txtItemComments.Text = value;
            }
        }

        public bool PrintAsGraphic
        {
            get
            {
                return chkPrintAsGraphic.IsChecked == true;
            }
            set
            {
                chkPrintAsGraphic.IsChecked = value;
            }
        }

        #endregion

        #region Methods

        public GeneralUC()
        {
            InitializeComponent();
        }

        #endregion
    }
}
