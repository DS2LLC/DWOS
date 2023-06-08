using System.Windows;
using Neodynamic.SDK.Printing;

namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for ImageItemDialog.xaml
    /// </summary>
    public partial class ImageItemDialog : Window
    {
        #region Properties

        /// <summary>
        /// Gets or sets image settings.
        /// </summary>
        public ImageItem ImageItem
        { 
            get
            {
                //create a new ImageItem and return it based on dialog settings
                ImageItem newImgItem = pictureUC1.GetImageItem();
                newImgItem.Name = generalUC1.ItemName;
                newImgItem.Comments = generalUC1.ItemComments;
                newImgItem.X = positionUC1.ItemX;
                newImgItem.Y = positionUC1.ItemY;
                newImgItem.DataField = dataBindingUC1.ItemDataField;
                newImgItem.DataFieldFormatString = dataBindingUC1.ItemDataFieldFormatString;
                return newImgItem;
            }
            set
            {
                //set picture tab
                pictureUC1.SetImageItem(value);
                //set position tab
                positionUC1.ItemX = value.X;
                positionUC1.ItemY = value.Y;
                //set data binding
                dataBindingUC1.ItemDataField = value.DataField;
                dataBindingUC1.ItemDataFieldFormatString = value.DataFieldFormatString;
                //set general tab
                generalUC1.ItemName = value.Name;
                generalUC1.ItemComments = value.Comments;
            }
        }

        #endregion

        #region Methods

        public ImageItemDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion



    }
}
