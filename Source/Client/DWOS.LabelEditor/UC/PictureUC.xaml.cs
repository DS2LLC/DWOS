using Neodynamic.SDK.Printing;
using Neodynamic.Windows.ThermalLabelEditor;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for PictureUC.xaml
    /// </summary>
    public partial class PictureUC : UserControl
    {
        #region Fields

        private ImageItem _imgItem = null;
        private bool _init = true;

        #endregion

        #region Methods

        public PictureUC()
        {
            InitializeComponent();
        }
        
        public void SetImageItem(ImageItem imgItem)
        {
            //clone source for working with...
            _imgItem = imgItem.Clone() as ImageItem;

            //display ImageItem's properties through UI elements...
            txtFileName.Text = _imgItem.SourceFile;
            
            if (_imgItem.Width == 0 && _imgItem.Height == 0)
            {
                rbtOriginalSize.IsChecked = true;
                rbtResize.IsChecked = false;
            }
            else
            {
                rbtOriginalSize.IsChecked = false;
                rbtResize.IsChecked = true;
                txtNewWidth.Text = _imgItem.Width.ToString();
                txtNewHeight.Text = _imgItem.Height.ToString();
            }

            cboLockAspectRatio.SelectedValue = _imgItem.LockAspectRatio.ToString();
            cboFlip.SelectedValue = _imgItem.Flip.ToString();
            txtRotation.Text = _imgItem.RotationAngle.ToString();

            cboDithering.SelectedValue = _imgItem.MonochromeSettings.DitherMethod.ToString();
            sldThreshold.Value = _imgItem.MonochromeSettings.Threshold;
            chkInvert.IsChecked = _imgItem.MonochromeSettings.ReverseEffect;

            //update picture preview
            UpdatePicturePreview();

            _init = false;
        }

        public ImageItem GetImageItem()
        {
            return _imgItem;
        }
        
        private void UpdatePicturePreview()
        {
            imgPreview.Source = ImageItemUtils.ConvertToBitmap(_imgItem);
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RenderOptions.SetBitmapScalingMode(imgPreview, BitmapScalingMode.NearestNeighbor);

            //load dithering methods
            cboDithering.ItemsSource = Enum.GetNames(typeof(DitherMethod));
            //load lock aspect ratio
            cboLockAspectRatio.ItemsSource = Enum.GetNames(typeof(LockAspectRatio));
            //load flip options
            cboFlip.ItemsSource = Enum.GetNames(typeof(Flip));
        }

        private void rbtOriginalSize_Checked(object sender, RoutedEventArgs e)
        {
            if (rbtResize != null)
            {
                rbtResize.IsChecked = false;
                txtNewWidth.IsEnabled = false;
                txtNewHeight.IsEnabled = false;
            }

            if (_init == false)
            {
                _imgItem.Width = 0;
                _imgItem.Height = 0;
                UpdatePicturePreview();
            }
        }

        private void rbtResize_Checked(object sender, RoutedEventArgs e)
        {
            if(rbtOriginalSize != null)
                rbtOriginalSize.IsChecked = false;

            if (_init == false)
            {
                txtNewWidth.IsEnabled = true;
                txtNewHeight.IsEnabled = true;

                try
                {
                    _imgItem.Width = double.Parse(txtNewWidth.Text);
                }
                catch
                {
                    _imgItem.Width = 0;
                }
                try
                {
                    _imgItem.Height = double.Parse(txtNewHeight.Text);
                }
                catch 
                {
                    _imgItem.Height = 0;
                }
                UpdatePicturePreview();
            }
        }

        private void btnSelectPicture_Click(object sender, RoutedEventArgs e)
        {
            //open file dialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Picture (*.jpg, *.gif, *.bmp, *.png, *.tif)|*.jpg; *.gif; *.bmp; *.png; *.tif";
            
            if (dlg.ShowDialog() == true)
            {
                //set source file
                txtFileName.Text = dlg.FileName;
                
                //_imgItem.SourceFile = dlg.FileName;
                //_imgItem.SourceBinary = File.ReadAllBytes(dlg.FileName);
                _imgItem.SourceBase64 = Convert.ToBase64String(File.ReadAllBytes(dlg.FileName), Base64FormattingOptions.None);
                //update picture preview
                UpdatePicturePreview();
            }
        }

        private void txtNewWidth_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_init == false)
            {
                try
                {
                    _imgItem.Width = double.Parse(txtNewWidth.Text);
                }
                catch
                {
                    _imgItem.Width = 0;
                }                
                UpdatePicturePreview();
            }
        }

        private void txtNewHeight_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_init == false)
            {
                try
                {
                    _imgItem.Height = double.Parse(txtNewHeight.Text);
                }
                catch
                {
                    _imgItem.Height = 0;
                }
                UpdatePicturePreview();
            }
        }

        private void cboFlip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_init == false)
            {
                _imgItem.Flip = (Flip)Enum.Parse(typeof(Flip), cboFlip.SelectedValue.ToString());
                UpdatePicturePreview();
            }
        }

        private void txtRotation_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_init == false)
            {
                try
                {
                    _imgItem.RotationAngle = int.Parse(txtRotation.Text);
                }
                catch
                {
                    _imgItem.RotationAngle = 0;
                }
                UpdatePicturePreview();
            }
        }

        private void cboDithering_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_init == false)
            {
                _imgItem.MonochromeSettings.DitherMethod = (DitherMethod)Enum.Parse(typeof(DitherMethod), cboDithering.SelectedValue.ToString());
                UpdatePicturePreview();
            }
        }

        private void sldThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_init == false)
            {
                _imgItem.MonochromeSettings.Threshold = (int)sldThreshold.Value;
                UpdatePicturePreview();
            }

            lblThreshold.Text = string.Format("Threshold ({0}%):", (int)sldThreshold.Value);
        }

        private void chkInvert_Checked(object sender, RoutedEventArgs e)
        {
            if (_init == false)
            {
                _imgItem.MonochromeSettings.ReverseEffect = (chkInvert.IsChecked == true);
                UpdatePicturePreview();
            }
        }

        private void cboLockAspectRatio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_init == false)
            {
                _imgItem.LockAspectRatio = (LockAspectRatio)Enum.Parse(typeof(LockAspectRatio), cboLockAspectRatio.SelectedValue.ToString());
                UpdatePicturePreview();
            }
        }

        #endregion
    }
}
