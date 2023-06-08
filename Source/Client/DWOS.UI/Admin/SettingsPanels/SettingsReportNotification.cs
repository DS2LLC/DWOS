using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinEditors;
using NLog;
using Infragistics.Win;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsReportNotification: UserControl, ISettingsPanel
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Default template for report notifications.
        /// </summary>
        public const string REPORT_NOTIFICATION_TEMPLATE = "ReportNotification";

        /// <summary>
        /// DataTable that persists templates.
        /// </summary>
        private readonly ApplicationSettingsDataSet.TemplatesDataTable _dtTemplates;

        /// <summary>
        /// The currently selected template.
        /// </summary>
        private ApplicationSettingsDataSet.TemplatesRow _currentTemplate;

        /// <summary>
        /// List of all available templates.
        /// </summary>
        private readonly IReadOnlyList<string> _allTemplates = new List<string>
        {
            REPORT_NOTIFICATION_TEMPLATE,
            "ReportNotification_CurrentOrderStatusReport",
            "ReportNotification_OrderReceiptReport"
        };

        #endregion

        #region Properties

        public string PanelKey
        {
            get { return "ReportNotification"; }
        }

        #endregion

        #region Methods
        public bool CanDock
        {
            get { return true; }
        }
        public SettingsReportNotification()
        {
            this.InitializeComponent();
            _dtTemplates = new ApplicationSettingsDataSet.TemplatesDataTable();
            cboTemplate.DataSource = _dtTemplates.DefaultView;
            cboTemplate.DisplayMember = _dtTemplates.TemplateIDColumn.ToString();
            cboTemplate.ValueMember = _dtTemplates.TemplateIDColumn.ToString();
        }

        public bool Editable
        {
            get { return SecurityManager.Current.IsInRole("ApplicationSettings.Edit"); }
        }

        public void LoadData()
        {
            try
            {
                cboTemplate.ValueChanged -= cboTemplate_ValueChanged;

                Enabled = this.Editable;

                using (var taTemplate = new TemplatesTableAdapter { ClearBeforeFill = false })
                {
                    foreach (var templateId in _allTemplates)
                    {
                        taTemplate.FillById(_dtTemplates, templateId);
                    }
                }

                var defaultTemplate = _dtTemplates.FindByTemplateID(REPORT_NOTIFICATION_TEMPLATE)
                    ?? CreateTemplate(REPORT_NOTIFICATION_TEMPLATE);

                cboTemplate.SelectedItem = cboTemplate
                    .FindItemByValue<string>(v => v == defaultTemplate.TemplateID);

                LoadTemplate(defaultTemplate);
            }
            catch (Exception exc)
            {
                const string errorMsg = "Error loading settings.";
                _log.Error(exc, errorMsg);
            }
            finally
            {
                cboTemplate.ValueChanged += cboTemplate_ValueChanged;
            }
        }

        private void LoadTemplate(ApplicationSettingsDataSet.TemplatesRow template)
        {
            if (template != null)
            {
                htmlEditor.LoadHtml(template.Template);
                txtTokens.Text = template.IsTokensNull() ? string.Empty : template.Tokens;
            }
            else
            {
                htmlEditor.LoadHtml(string.Empty);
                txtTokens.Text = string.Empty;
            }

            _currentTemplate = template;
        }

        private ApplicationSettingsDataSet.TemplatesRow CreateTemplate(string templateId)
        {
            string description;
            switch (templateId)
            {
                case "ReportNotification_CurrentOrderStatusReport":
                    description = "Email template to send customers order status reports.";
                    break;
                case "ReportNotification_OrderReceiptReport":
                    description = "Email template to send customers order receipt reports";
                    break;
                default:
                    description = "Email template to send customers automated reports.";
                    break;
            }

            return _dtTemplates.AddTemplatesRow(
                templateId,
                string.Empty,
                description,
                "%REPORTNAME%,  %LOGO%");
        }

        public void SaveData()
        {
            SyncCurrentTemplate();

            using (var ta = new TemplatesTableAdapter())
            {
                ta.Update(_dtTemplates);
            }
        }

        private void SyncCurrentTemplate()
        {
            if (_currentTemplate != null)
            {
                _currentTemplate.Template = htmlEditor.GetHtml();
            }
        }

        private void DisposeCodeBehind()
        {
            _dtTemplates?.Dispose();
        }

        #endregion

        #region Events

        private void cboTemplate_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                SyncCurrentTemplate();

                var currentTemplateId = cboTemplate.Value as string;
                var currentTemplate = _dtTemplates.FindByTemplateID(currentTemplateId);
                LoadTemplate(currentTemplate);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing selected template.");
            }
        }

        private void cboTemplate_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                if (e.Button.Key == "AddTemplate")
                {
                    // Get list of reports that do not have templates
                    var templatesToShow = new List<string>();

                    foreach (var reportId in _allTemplates)
                    {
                        if (_dtTemplates.FindByTemplateID(reportId) == null)
                        {
                            templatesToShow.Add(reportId);
                        }
                    }

                    if (templatesToShow.Count > 0)
                    {
                        // Show dialog
                        using (var cbo = new ComboBoxForm())
                        {
                            cbo.Text = "Templates";
                            cbo.ComboBox.Items
                                .AddRange(templatesToShow.Select(t => new ValueListItem(t)).ToArray());

                            cbo.ComboBox.SelectedIndex = 0;
                            cbo.FormLabel.Text = "Template:";

                            if (cbo.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                            {
                                var template = CreateTemplate(cbo.ComboBox.Value as string);
                                template.Template = htmlEditor.GetHtml();

                                cboTemplate.SelectedItem = cboTemplate
                                    .FindItemByValue<string>(v => v == template.TemplateID);
                            }
                        }
                    }
                    else
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            "All templates have already been added.",
                            "Report Notifications");
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling editor button click event.");
            }
        }

        #endregion
    }
}