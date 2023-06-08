using System;
using System.Windows.Forms;
using DWOS.Shared;
using DWOS.Shared.Wizard;

namespace DWOS.UI.Admin.FieldMigration
{
    public partial class MigratePanel : UserControl, IWizardPanel
    {
        private FieldMigrationController _controller;

        #region Methods

        public MigratePanel()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void btnMigrate_Click(object sender, EventArgs e)
        {
            try
            {
                _controller?.Update();
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error migrating custom field data to fields.", exc);
            }

            pgbProgress.Value = 1;
            IsValid = true;
            OnValidStateChanged?.Invoke(this);
            btnMigrate.Enabled = false;
        }

        #endregion

        #region IWizardPanel members

        public string Title => "Confirm Selection";

        public string SubTitle => string.Empty;

        public Action<IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller)
        {
            _controller = controller as FieldMigrationController;
        }

        public Control PanelControl => this;

        public bool IsValid { get; set; }

        public void OnMoveTo()
        {
            lblCustomField.Text = _controller.CustomFieldName;
            pgbProgress.Value = 0;
            pgbProgress.Maximum = 1;
            btnMigrate.Enabled = true;

            var counts = _controller.GetUpdateCounts();
            var ordersText = $"{counts.Orders} {(counts.Orders == 1 ? "order" : "orders")}";
            var customerText = $"{counts.Customers} {(counts.Customers == 1 ? "customer" : "customers")}";
            lblSummary.Text = $@"This will update product class data for {ordersText} and {customerText}.";
        }

        public void OnMoveFrom()
        {
        }

        public void OnFinished()
        {
            _controller = null;
        }

        #endregion
    }
}
