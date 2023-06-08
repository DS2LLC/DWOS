using System.Windows;
using System.Windows.Controls;

namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for SizeUC.xaml
    /// </summary>
    public partial class SizeUC : UserControl
    {
        #region Properties

        public double ItemWidth
        {
            get
            {
                try
                {
                    return double.Parse(txtItemWidth.Text);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                txtItemWidth.Text = value.ToString();
            }
        }

        public double ItemHeight
        {
            get
            {
                try
                {
                    return double.Parse(txtItemHeight.Text);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                txtItemHeight.Text = value.ToString();
            }
        }

        public int ItemRotationAngle
        {
            get
            {
                try
                {
                    return int.Parse(txtItemRotationAngle.Text);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                txtItemRotationAngle.Text = value.ToString();
            }
        }


        public Visibility RotationAngleVisibility
        {
            get
            {
                return gbRotation.Visibility;
            }
            set
            {
                gbRotation.Visibility = value;
            }
        }

        #endregion

        #region Methods

        public SizeUC()
        {
            InitializeComponent();
        }

        #endregion
    }
}
