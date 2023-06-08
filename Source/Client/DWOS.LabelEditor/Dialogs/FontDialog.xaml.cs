using System.Windows;


namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for FontDialog.xaml
    /// </summary>
    public partial class FontDialog : Window
    {
        #region Properties

        /// <summary>
        /// Gets or sets the selected font.
        /// </summary>
        public Neodynamic.SDK.Printing.Font Font
        {
            get
            {
                return this.fontUC1.GetFont();
            }
            set
            {
                this.fontUC1.SetFont(value);
            }
        }

        #endregion

        #region Methods

        public FontDialog()
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
