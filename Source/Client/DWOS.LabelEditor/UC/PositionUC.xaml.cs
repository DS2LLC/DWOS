using System.Windows.Controls;

namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for PositionUC.xaml
    /// </summary>
    public partial class PositionUC : UserControl
    {
        #region Properties

        public double ItemX
        {
            get
            {
                try
                {
                    return double.Parse(txtItemX.Text);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                txtItemX.Text = value.ToString();
            }
        }

        public double ItemY
        {
            get
            {
                try
                {
                    return double.Parse(txtItemY.Text);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                txtItemY.Text = value.ToString();
            }
        }

        #endregion

        #region Methods

        public PositionUC()
        {
            InitializeComponent();
        }

        #endregion
    }
}
