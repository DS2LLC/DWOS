using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsCOCData : UserControl, ISettingsPanel
    {
        #region Fields

        private const string COC_TEMPLATE = "COCData";
        private const string COC_TOKENS = "%ACCEPTEDTEXT%, %REJECTEDTEXT%, %TOTALTEXT%, %PROCESSTEXT%, %SERIALNUMBER%, %IMPORTVALUE%, ";
        private const string COC_DESCRIPTION = "Default text to use for COC data.";

        #endregion

        #region Methods
        public bool CanDock
        {
            get { return true; }
        }
        public SettingsCOCData()
        {
            InitializeComponent();
        }

        #endregion

        #region ISettingsPanel Members

        public bool Editable
        {
            get
            {
                return SecurityManager.Current.IsInRole("ApplicationSettings.Edit");
            }
        }

        public string PanelKey
        {
            get
            {
                return "COCData";
            }
        }

        public void LoadData()
        {
            try
            {
                Enabled = Editable;

                ApplicationSettingsDataSet.TemplatesRow template = null;
                using(var ta = new TemplatesTableAdapter())
                {
                    template = ta.GetDataById(COC_TEMPLATE).FirstOrDefault();
                }

                if (template == null)
                {
                    txtTokens.Value = COC_TOKENS;
                }
                if (template != null)
                {
                    txtTemplate.Value = template.Template;
                    txtTokens.Value = template.IsTokensNull() ? COC_TOKENS : template.Tokens;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error loading COCData settings.");
            }
        }

        public void SaveData()
        {
            using (var dtTemplate = new ApplicationSettingsDataSet.TemplatesDataTable())
            {
                using (var taTemplate = new TemplatesTableAdapter())
                {
                    taTemplate.FillById(dtTemplate, COC_TEMPLATE);

                    var template = dtTemplate.FirstOrDefault();

                    if (template == null)
                    {
                        template = dtTemplate.NewTemplatesRow();
                        template.Description = COC_DESCRIPTION;
                        template.TemplateID = COC_TEMPLATE;
                        template.Tokens = COC_TOKENS;
                        template.Template = txtTemplate.Value.ToString();
                        dtTemplate.AddTemplatesRow(template);
                    }
                    else
                    {
                        template.Template = txtTemplate.Value.ToString();
                    }

                    taTemplate.Update(dtTemplate);
                }
            }
        }

        #endregion
    }
}
