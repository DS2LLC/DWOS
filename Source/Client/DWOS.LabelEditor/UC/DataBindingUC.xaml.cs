using System.Windows.Controls;

namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for DataBindingUC.xaml
    /// </summary>
    public partial class DataBindingUC : UserControl
    {
        #region Properties

        public string ItemDataField
        {
            get
            {
                return txtItemDataField.Text;
            }
            set
            {
                txtItemDataField.Text = value;
            }
        }

        public string ItemDataFieldFormatString
        {
            get
            {
                return txtItemDataFieldFormatString.Text;
            }
            set
            {
                txtItemDataFieldFormatString.Text = value;
            }
        }

        #endregion

        #region Methods

        public DataBindingUC()
        {
            InitializeComponent();
        }

        #endregion
    }
}
