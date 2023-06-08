using System;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Shared.Wizard;

namespace DWOS.UI.Admin.FieldMigration
{
    public partial class CustomFieldSelectPanel : UserControl, IWizardPanel
    {
        #region Fields

        private FieldMigrationController _controller;

        #endregion

        #region Methods

        public CustomFieldSelectPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region Events
        private void cboCustomField_ValueChanged(object sender, EventArgs e)
        {
            OnValidStateChanged?.Invoke(this);
        }

        #endregion

        #region IWizardPanel members

        public string Title => "Select Custom Field";
        public string SubTitle => string.Empty;
        public Action<IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller)
        {
            _controller = controller as FieldMigrationController;

            // Load data
            using (var dsCustomField = new OrdersDataSet.CustomFieldDataTable())
            {
                using (var taCustomField = new CustomFieldTableAdapter())
                {
                    taCustomField.Fill(dsCustomField);
                }

                var fieldNames = dsCustomField
                    .Select(s => s.Name)
                    .Distinct()
                    .OrderBy(name => name)
                    .ToList();

                cboCustomField.Clear();

                foreach (var fieldName in fieldNames)
                {
                    // Add to combobox
                    cboCustomField.Items.Add(fieldName);
                }

                cboCustomField.SelectedIndex = fieldNames.Count > 0 ? 0 : -1;
            }
        }

        public Control PanelControl => this;
        public bool IsValid => cboCustomField.Value != null;
        public void OnMoveTo()
        {
        }

        public void OnMoveFrom()
        {
            if (_controller != null)
            {
                _controller.CustomFieldName = cboCustomField.Text;
            }
        }

        public void OnFinished()
        {
            _controller = null;
        }

        #endregion


    }
}
