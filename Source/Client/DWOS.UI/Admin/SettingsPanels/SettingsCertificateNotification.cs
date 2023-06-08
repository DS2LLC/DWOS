using System;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsCertificateNotification : UserControl, ISettingsPanel
    {
        #region Fields

        private const string TEMPLATE = "CertificateNotification";
        private const string DEFAULT_DESCRIPTION = "Email template to notify customer that a certificate has been made for an order.";
        private const string DEFAULT_TOKENS = "%ORDER%,%ORDERTYPE%,%LOGO%";

        #endregion
        public bool CanDock
        {
            get { return true; }
        }
        public SettingsCertificateNotification()
        {
            InitializeComponent();
        }

        #region ISettingsPanel Members

        public bool Editable => SecurityManager.Current.IsInRole("ApplicationSettings.Edit");

        public string PanelKey => "CertificateNotification";

        public void LoadData()
        {
            try
            {
                Enabled = Editable;

                var template = GetTemplate();

                if (template == null)
                {
                    txtTokens.Text = DEFAULT_TOKENS;
                }
                else
                {
                    htmlEditor.LoadHtml(template.Template);
                    txtTokens.Text = template.IsTokensNull() ? DEFAULT_TOKENS : template.Tokens;
                }
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error loading settings.";
                LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
            }
        }

        public void SaveData()
        {
            using(var ta = new TemplatesTableAdapter())
            {
                var template = ta.GetDataById(TEMPLATE).FirstOrDefault();

                var templateTable = new ApplicationSettingsDataSet.TemplatesDataTable();
                if(template == null)
                {
                    template             = templateTable.NewTemplatesRow();
                    template.Description = DEFAULT_DESCRIPTION;
                    template.Tokens      = DEFAULT_TOKENS;
                    template.TemplateID  = TEMPLATE;
                    template.Template = htmlEditor.GetHtml();

                    templateTable.AddTemplatesRow(template);
                }
                else
                {
                    templateTable.ImportRow(template);
                    template = templateTable.Rows[0] as ApplicationSettingsDataSet.TemplatesRow;

                    if (template != null)
                    {
                        template.Template = htmlEditor.GetHtml();
                    }
                }

                ta.Update(templateTable);
            }
        }
        private ApplicationSettingsDataSet.TemplatesRow GetTemplate()
        {
            using(var ta = new TemplatesTableAdapter())
            {
                var templates = ta.GetDataById(TEMPLATE);
                return templates.FirstOrDefault();
            }
        }

        #endregion
    }
}
