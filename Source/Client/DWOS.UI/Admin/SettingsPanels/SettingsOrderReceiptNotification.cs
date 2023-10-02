using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsOrderReceiptNotification
        : UserControl, ISettingsPanel
    {
        #region Fields

        public const string TEMPLATE_ID = "OrderReceiptNotification";
        public const string TEMPLATE_TOKENS = "%ORDER%, %PURCHASEORDER%, %SERIALNUMBERS%, %PART%, " +
            "%OPERATORNOTES%, %SUBTOTAL%, %FEES%, %TOTAL%";

        #endregion

        #region Methods
        public bool CanDock
        {
            get { return true; }
        }
        public SettingsOrderReceiptNotification()
        {
            InitializeComponent();
            htmlEditor.Tokens = new List<HtmlEditor.Token>
            {
                new HtmlEditor.Token
                {
                    Tag = "%FEES%",
                    Value = "<p>Example Fee - $0.00</p>"
                }
            };
        }

        private ApplicationSettingsDataSet.TemplatesRow GetTemplate()
        {
            using (var ta = new TemplatesTableAdapter())
            {
                var templates = ta.GetDataById(TEMPLATE_ID);
                return templates.FirstOrDefault();
            }
        }

        #endregion

        #region ISettingsPanel Members

        public string PanelKey =>
            "OrderReceiptNotification";

        public bool Editable =>
            SecurityManager.Current.IsInRole("ApplicationSettings.Edit");

        public void LoadData()
        {
            try
            {
                Enabled = Editable;

                var template = GetTemplate();
                if (template != null)
                {
                    htmlEditor.LoadHtml(template.Template);
                }

                txtTokens.Text = TEMPLATE_TOKENS;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error loading settings.");
            }
        }

        public void SaveData()
        {
            using (var ta = new TemplatesTableAdapter())
            {
                var template = ta.GetDataById(TEMPLATE_ID).FirstOrDefault();

                var templateTable = new ApplicationSettingsDataSet.TemplatesDataTable();

                if (template == null)
                {
                    template = templateTable.NewTemplatesRow();
                    template.Description = "Email template to notify customer that an order needs approval.";
                    template.TemplateID = TEMPLATE_ID;
                    template.Tokens = TEMPLATE_TOKENS;
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

        #endregion
    }
}
