using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Shared.Utilities;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsQuality: UserControl, ISettingsPanel
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private bool _imageChanged;

        #endregion

        #region Properties

        public string PanelKey
        {
            get { return "Quality"; }
        }

        #endregion
        public bool CanDock
        {
            get { return true; }
        }
        public SettingsQuality()
        {
            this.InitializeComponent();
            this.cboPrintOption.Items.Add(new Infragistics.Win.ValueListItem(ReportPrintSetting.Nothing, "Do Nothing"));
            this.cboPrintOption.Items.Add(new Infragistics.Win.ValueListItem(ReportPrintSetting.Printer, "Print to Default Printer"));
            this.cboPrintOption.Items.Add(new Infragistics.Win.ValueListItem(ReportPrintSetting.Pdf, "Create PDF"));
        }

        #region ISettingsPanel Members

        public bool Editable
        {
            get { return SecurityManager.Current.IsInRole("ApplicationSettings.Edit"); }
        }

        public void LoadData()
        {
            try
            {
                Enabled = this.Editable;

                this.txtCOC.Text = ApplicationSettings.Current.COCWarranty;
                this.txtQualityName.Text = ApplicationSettings.Current.COCSignatureName;
                this.txtQualityTitle.Text = ApplicationSettings.Current.COCSignatureTitle;
                
                var logoPath = ApplicationSettings.Current.COCSignatureImagePath;

                if (logoPath != null && System.IO.File.Exists(logoPath))
                    this.picQualitySignature.Image = Bitmap.FromFile(logoPath);

                var printOption = ApplicationSettings.Current.DefaultCOCPrintSetting;
                var printOptionItem = this.cboPrintOption.FindItemByValue<ReportPrintSetting>((i) => i == printOption);

                if (printOptionItem != null)
                {
                    this.cboPrintOption.SelectedItem = printOptionItem;
                }
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error loading settings.";
                _log.Error(exc, errorMsg);
            }
        }

        public void SaveData()
        {
            ApplicationSettings.Current.COCWarranty = this.txtCOC.Text;
            ApplicationSettings.Current.COCSignatureName = this.txtQualityName.Text;
            ApplicationSettings.Current.COCSignatureTitle = this.txtQualityTitle.Text;

            //Update the cache version to force to get new image on app restart
            if (this._imageChanged)
            {
                var img = (picQualitySignature.Image ?? picQualitySignature.DefaultImage) as Bitmap;

                if (img != null)
                {
                    var logoTempFile = System.IO.Path.GetTempFileName() + ".png";
                    img.Save(logoTempFile, ImageFormat.Png);

                    ApplicationSettings.Current.COCSignatureImagePath = logoTempFile;
                }
                else
                {
                    ApplicationSettings.Current.COCSignatureImagePath = null;


                }

                ApplicationSettings.Current.CacheVersion = ApplicationSettings.Current.CacheVersion + 1;
            }

            var printOption = (ReportPrintSetting)(this.cboPrintOption.SelectedItem?.DataValue ?? ReportPrintSetting.Nothing);
            ApplicationSettings.Current.DefaultCOCPrintSetting = printOption;
        }

        #endregion

        #region Events
        
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                using(var fd = new OpenFileDialog())
                {
                    fd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";

                    if(fd.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                    {
                        this.picQualitySignature.Image = new Bitmap(fd.FileName);
                        this._imageChanged = true;
                    }
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error opening media.");
            }
        }

        #endregion

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            this.picQualitySignature.Image = null;
            this._imageChanged = true;

        }
    }
}