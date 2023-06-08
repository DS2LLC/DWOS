using System;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.UI.Utilities;
using NLog;
using System.Drawing;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsPortal: UserControl, ISettingsPanel
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public const string CUSTOMER_PORTAL_TEMPLATE = "CustomerPortal";
        public const string CUSTOMER_PORTAL_TOKENS = "%USERNAME%, %PASSWORD%, %LOGO%, %COMPANY%, %COMPANY_ADDRESS%, %COMPANY_CITY%, %COMPANY_STATE%, %COMPANY_ZIP%, %COMPANY_PHONE%, %COMPANY_FAX%, %COMPANY_EMAIL%, %PORTAL_URL%, %COMPANY_URL%";

        #endregion

        #region Properties

        public string PanelKey
        {
            get { return "Portal"; }
        }

        #endregion

        #region Methods
        public bool CanDock
        {
            get { return false; }
        }
        public SettingsPortal()
        {
            this.InitializeComponent();
        }

        public bool Editable
        {
            get { return SecurityManager.Current.IsInRole("ApplicationSettings.Edit"); }
        }

        public void LoadData()
        {
            try
            {
                Enabled             = this.Editable;
                var template        = this.GetTemplate();


                if (template == null)
                {
                    txtTokens.Text = CUSTOMER_PORTAL_TOKENS;
                }
                else
                {
                    htmlEditor.RenderSettings = new PortalRenderSettings();
                    htmlEditor.LoadHtml(template.Template);
                    txtTokens.Text = template.IsTokensNull() ? CUSTOMER_PORTAL_TOKENS : template.Tokens;
                }

                this.txtCCAddress.Text = ApplicationSettings.Current.PortalAuthorizationEmailCC;
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error loading settings.";
                _log.Error(exc, errorMsg);
            }
        }

        public void SaveData()
        {
            ApplicationSettings.Current.PortalAuthorizationEmailCC = this.txtCCAddress.Text;

            using(var ta = new TemplatesTableAdapter())
            {
                var template = ta.GetDataById(CUSTOMER_PORTAL_TEMPLATE).FirstOrDefault();
                
                var templateTable = new ApplicationSettingsDataSet.TemplatesDataTable();

                if(template == null)
                {
                    template             = templateTable.NewTemplatesRow();
                    template.TemplateID  = CUSTOMER_PORTAL_TEMPLATE;
                    template.Description = "Customer Portal authorization template.";
                    template.Tokens      = CUSTOMER_PORTAL_TOKENS;
                    template.Template = htmlEditor.GetHtml();
                    templateTable.AddTemplatesRow(template);
                }
                else
                {
                    templateTable.ImportRow(template);
                    template = templateTable.Rows[0] as ApplicationSettingsDataSet.TemplatesRow;
                    template.Template = htmlEditor.GetHtml();
                }


                ta.Update(templateTable);
            }
        }

        private ApplicationSettingsDataSet.TemplatesRow GetTemplate()
        {
            using(var ta = new TemplatesTableAdapter())
            {
                var templates = ta.GetDataById(CUSTOMER_PORTAL_TEMPLATE);
                return templates.FirstOrDefault();
            }
        }

        #endregion

        #region PortalRenderSettings

        private sealed class PortalRenderSettings : HtmlEditor.DefaultRenderSettings
        {
            #region Fields

            private readonly string _resizedLogoData = GenerateLogoData();

            #endregion

            #region Properties

            public override string CompanyLogoImagePath
            {
                get
                {
                    return _resizedLogoData;
                }
            }

            #endregion

            #region Methods

            private static string GenerateLogoData()
            {
                try
                {
                    var companyLogoPath = ApplicationSettings.Current.CompanyLogoImagePath;
                    var imageSize = new Size(400, 200);
                    using (var originalImg = Image.FromFile(companyLogoPath))
                    {
                        var newSize = MediaUtilities.Resize(originalImg.Size, imageSize);

                        using (var newImage = new Bitmap(imageSize.Width, imageSize.Height))
                        {
                            int xOffset = (imageSize.Width - newSize.Width) / 2;
                            int yOffset = (imageSize.Height - newSize.Height) / 2;
                            using (var gfx = Graphics.FromImage(newImage))
                            {
                                gfx.DrawImage(originalImg, xOffset, yOffset, newSize.Width, newSize.Height);
                                return "data:image/png;base64," + Convert.ToBase64String(newImage.GetImageAsBytesPng());
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    const string errorMsg = "Error generating resized logo for Portal Settings.";
                    LogManager.GetCurrentClassLogger().Error(exc, errorMsg);

                    return ApplicationSettings.Current.CompanyLogoImagePath;
                }
            }

            #endregion
        }
        #endregion
    }
}