using System;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsShippingNotification: UserControl, ISettingsPanel
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        
        public const string SHIPPING_TEMPLATE = "ShipmentNotification";
        public const string SHIPPING_TOKENS = "%LOGO%, %COMPANY%, %COMPANY_ADDRESS%, %COMPANY_CITY%, %COMPANY_STATE%, %COMPANY_ZIP%, %COMPANY_PHONE%, %COMPANY_FAX%, %COMPANY_EMAIL%, %PORTAL_URL%, %COMPANY_URL%";

        #endregion

        #region Properties
        public bool CanDock
        { 
            get { return true; } 
        }
        public string PanelKey
        {
            get { return "ShippingNotification"; }
        }

        #endregion

        #region Methods

        public SettingsShippingNotification()
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
                Enabled = this.Editable;

                var template = this.GetTemplate();

                if (template == null)
                {
                    txtTokens.Text = SHIPPING_TOKENS;
                }
                else
                {
                    htmlEditor.LoadHtml(template.Template);
                    txtTokens.Text = template.IsTokensNull() ? SHIPPING_TOKENS : template.Tokens;
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
            using(var ta = new TemplatesTableAdapter())
            {
                var template = ta.GetDataById(SHIPPING_TEMPLATE).FirstOrDefault();

                var templateTable = new ApplicationSettingsDataSet.TemplatesDataTable();
                if(template == null)
                {
                    template = templateTable.NewTemplatesRow();
                    template.Description = "Shipment Notification template.";
                    template.TemplateID = SHIPPING_TEMPLATE;
                    template.Tokens = SHIPPING_TOKENS;
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
                var templates = ta.GetDataById(SHIPPING_TEMPLATE);
                return templates.FirstOrDefault();
            }
        }

        #endregion
    }
}