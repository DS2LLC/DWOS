using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.Shared.Utilities;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsCompanyInfo: UserControl, ISettingsPanel
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private ApplicationSettingsDataSet.CountryDataTable dtCountry;
        private bool _imageChanged;

        #endregion

        #region Properties

        public string PanelKey
        {
            get { return "Company"; }
        }

        #endregion

        #region Methods
        public bool CanDock
        {
            get { return true; }
        }
        public SettingsCompanyInfo()
        {
            this.InitializeComponent();
        }

        #endregion

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

                // Load country combobox
                using (var taCountry = new CountryTableAdapter())
                {
                    dtCountry = taCountry.GetData();
                }

                cboCountry.DataSource = dtCountry.DefaultView;
                cboCountry.DisplayMember = dtCountry.NameColumn.ColumnName;
                cboCountry.ValueMember = dtCountry.CountryIDColumn.ColumnName;

                // Populate fields
                this.txtUserName.Text  = ApplicationSettings.Current.CompanyName;
                this.txtAddress1.Text  = ApplicationSettings.Current.CompanyAddress1;
                this.txtCity.Text      = ApplicationSettings.Current.CompanyCity;
                this.txtState.Text     = ApplicationSettings.Current.CompanyState;
                this.txtZipCode.Text   = ApplicationSettings.Current.CompanyZip;
                this.cboCountry.Value  = dtCountry.FindByCountryID(ApplicationSettings.Current.CompanyCountry) != null
                    ? ApplicationSettings.Current.CompanyCountry : 1;
                this.txtPhone.Text     = ApplicationSettings.Current.CompanyPhone;
                this.txtFax.Text       = ApplicationSettings.Current.CompanyFax;
                this.txtTagline.Text   = ApplicationSettings.Current.CompanyTagline;
                this.txtWebURL.Text    = ApplicationSettings.Current.CompanyUrl;
                this.txtPortalURL.Text = ApplicationSettings.Current.PortalUrl; ;
                
                var logoPath = ApplicationSettings.Current.CompanyLogoImagePath;
                
                if (logoPath != null && System.IO.File.Exists(logoPath))
                    this.picCompanyLogo.Image = Bitmap.FromFile(logoPath);
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error loading settings.";
                _log.Error(exc, errorMsg);
            }
        }

        public void SaveData()
        {
            ApplicationSettings.Current.CompanyName = this.txtUserName.Text;
            ApplicationSettings.Current.CompanyAddress1 = this.txtAddress1.Text;
            ApplicationSettings.Current.CompanyCity = this.txtCity.Text;
            ApplicationSettings.Current.CompanyState = this.txtState.Text;
            ApplicationSettings.Current.CompanyZip = this.txtZipCode.Text;
            ApplicationSettings.Current.CompanyCountry = Convert.ToInt32(this.cboCountry.Value);
            ApplicationSettings.Current.CompanyPhone = this.txtPhone.Text;
            ApplicationSettings.Current.CompanyFax = this.txtFax.Text;
            ApplicationSettings.Current.CompanyTagline = this.txtTagline.Text;
            ApplicationSettings.Current.CompanyUrl = this.txtWebURL.Text;
            ApplicationSettings.Current.PortalUrl = this.txtPortalURL.Text; ;
            
            //Update the cache version to force to get new image on app restart
            if(this._imageChanged)
            {
                var img = (picCompanyLogo.Image ?? picCompanyLogo.DefaultImage) as Bitmap;
                
                if (img != null)
                {
                    var logoTempFile = System.IO.Path.GetTempFileName() + ".png";
                    img.Save(logoTempFile, ImageFormat.Png);

                    ApplicationSettings.Current.CompanyLogoImagePath = logoTempFile;
                }

                ApplicationSettings.Current.CacheVersion = ApplicationSettings.Current.CacheVersion + 1;
            }
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
                        this.picCompanyLogo.Image = new Bitmap(fd.FileName);
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
    }
}