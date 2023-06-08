using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DWOS.UI.Utilities;
using DWOS.Data;
using System.IO;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using NLog;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsShipping : UserControl, ISettingsPanel
    {
        #region Fields

        private const string REPAIR_TEMPLATE = "RepairsExport";
        private const string REPAIR_TOKENS = "%COMPANY%, %RECEIVEDDATE%, %CUSTOMERNAME%";
        private const string REPAIR_DESCRIPTION = "Default text to use for statements of repairs";

        private bool _imageChanged;

        #endregion

        #region Methods
        public bool CanDock
        {
            get { return true; }
        }
        public SettingsShipping()
        {
            InitializeComponent();
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
                        picRepairLogo.Image = new Bitmap(fd.FileName);
                        _imageChanged = true;
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error opening media.");
            }
        }

        #endregion

        #region ISettingsPanel Members

        public bool Editable => SecurityManager.Current.IsInRole("ApplicationSettings.Edit");

        public string PanelKey => "Shipping";

        public void LoadData()
        {
            try
            {
                Enabled = Editable;

                ApplicationSettingsDataSet.TemplatesRow template = null;
                using(var ta = new TemplatesTableAdapter())
                {
                    template = ta.GetDataById(REPAIR_TEMPLATE).FirstOrDefault();
                }

                if (template == null)
                {
                    txtRepairTokens.Value = REPAIR_TOKENS;
                }
                else
                {
                    htmlRepair.LoadHtml(template.Template);
                    txtRepairTokens.Value = template.IsTokensNull() ? REPAIR_TOKENS : template.Tokens;
                }

                var logoPath = ApplicationSettings.Current.ShippingRepairsStatementLogo;
                if (logoPath != null && File.Exists(logoPath))
                {
                    picRepairLogo.Image = Image.FromFile(logoPath);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading settings.");
            }
        }

        public void SaveData()
        {
            using (var dtTemplate = new ApplicationSettingsDataSet.TemplatesDataTable())
            {
                using (var taTemplate = new TemplatesTableAdapter())
                {
                    taTemplate.FillById(dtTemplate, REPAIR_TEMPLATE);

                    var template = dtTemplate.FirstOrDefault();

                    var templateText = htmlRepair?.GetHtml() ?? string.Empty;

                    if (template == null)
                    {
                        template = dtTemplate.NewTemplatesRow();
                        template.Description = REPAIR_DESCRIPTION;
                        template.TemplateID = REPAIR_TEMPLATE;
                        template.Tokens = REPAIR_TOKENS;
                        template.Template = templateText;
                        dtTemplate.AddTemplatesRow(template);
                    }
                    else
                    {
                        template.Template = templateText;
                    }

                    taTemplate.Update(dtTemplate);
                }
            }

            //Update the cache version to force to get new image on app restart
            if (!_imageChanged)
            {
                return;
            }

            var img = (picRepairLogo.Image ?? picRepairLogo.DefaultImage) as Bitmap;

            if (img != null)
            {
                var logoTempFile = Path.GetTempFileName() + ".png";
                img.Save(logoTempFile, ImageFormat.Png);

                ApplicationSettings.Current.ShippingRepairsStatementLogo = logoTempFile;
            }

            ApplicationSettings.Current.CacheVersion = ApplicationSettings.Current.CacheVersion + 1;
        }

        #endregion
    }
}