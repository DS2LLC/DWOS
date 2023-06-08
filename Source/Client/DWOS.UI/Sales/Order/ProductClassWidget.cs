using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Utilities.Validation;
using NLog;

namespace DWOS.UI.Sales.Order
{
    public partial class ProductClassWidget : UserControl
    {
        #region Fields

        private ProductClassType _editorType;

        private readonly Lazy<ApplicationSettingsDataSet.FieldsRow> _productClassFieldLazy = new Lazy<ApplicationSettingsDataSet.FieldsRow>(
            () =>
            {
                ApplicationSettingsDataSet.FieldsDataTable fields;

                using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                {
                    fields = ta.GetByCategory("Order");
                }

                var productClassField = fields.FirstOrDefault(f => f.Name == "Product Class");
                return productClassField;
            });

        #endregion

        #region Properties

        public OrdersDataSet.OrderRow CurrentOrder { get; private set; }

        public OrdersDataSet Dataset { get; set; }

        public CustomersDataset.Customer_FieldsDataTable CustomerFields { get; private set; }

        public bool ReadOnly
        {
            get
            {
                if (_editorType == ProductClassType.Textbox)
                {
                    return txtProductClass.ReadOnly;
                }

                return cboProductClass.ReadOnly;
            }
            set
            {
                if (_editorType == ProductClassType.Textbox)
                {
                    txtProductClass.ReadOnly = value;
                }
                else
                {
                    cboProductClass.ReadOnly = value;
                }
            }
        }

        public override string Text
        {
            get
            {
                if (_editorType == ProductClassType.Textbox)
                {
                    return txtProductClass.Text;
                }

                return cboProductClass.Text;
            }
            set
            {
                if (_editorType == ProductClassType.Textbox)
                {
                    txtProductClass.Text = value;
                }
                else
                {
                    if (!string.IsNullOrEmpty(value) && !cboProductClass.Items.Contains(value))
                    {
                        cboProductClass.Items.Add(value);
                    }

                    cboProductClass.Text = value;
                }
            }
        }

        #endregion

        #region Methods

        public ProductClassWidget()
        {
            InitializeComponent();
        }

        public void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            if (_editorType == ProductClassType.Textbox)
            {
                manager.Add(
                    new ImageDisplayValidator(
                        new TextControlValidator(txtProductClass, "Product Class is required")
                        {
                            ValidationRequired = RequireValidation
                        }, errProvider));
            }
            else
            {
                manager.Add(
                    new ImageDisplayValidator(
                        new TextControlValidator(cboProductClass, "Product Class is required")
                        {
                            ValidationRequired = RequireValidation
                        }, errProvider));
            }
        }

        /// <summary>
        /// Refreshes the enabled/disabled status of this control's validators.
        /// </summary>
        /// <param name="manager">
        /// Manager with this control's validators.
        /// </param>
        /// <param name="customerId">
        /// The ID of the current customer.
        /// </param>
        /// <remarks>
        /// The <paramref name="customerId"/> parameter exists to resolve an
        /// issue where <see cref="CurrentOrder"/> has not been refreshed yet
        /// because it refreshes after the customer changes in
        /// <see cref="OrderInformation"/>.
        /// </remarks>
        public void RefreshValidators(ValidatorManager manager, int customerId)
        {
            var validator = _editorType == ProductClassType.Textbox
                ? manager.Find(txtProductClass)
                : manager.Find(cboProductClass);

            if (validator == null)
            {
                return;
            }

            validator.IsEnabled = IsProductClassRequired(customerId);
        }

        private bool RequireValidation()
        {

            if (CurrentOrder?.RowState != DataRowState.Added)
            {
                return false;
            }

            var customerId = CurrentOrder.CustomerID;

            return IsProductClassRequired(customerId);
        }

        public void LoadRow(OrdersDataSet.OrderRow row)
        {
            CurrentOrder = row;

            if (_editorType == ProductClassType.Combobox)
            {
                cboProductClass.Items.Clear();

                foreach (var productClassRow in Dataset.ProductClass.OrderBy(pc => pc.Name))
                {
                    cboProductClass.Items.Add(productClassRow.Name);
                }
            }

            if (CurrentOrder == null)
            {
                txtProductClass.Clear();
                cboProductClass.Clear(); 
            }
            else
            {
                var productClass = CurrentOrder.GetOrderProductClassRows().FirstOrDefault();

                if (productClass == null || productClass.IsProductClassNull())
                {
                    txtProductClass.Clear();
                    cboProductClass.Clear();
                }
                else
                {
                    txtProductClass.Text = productClass.ProductClass;

                    if (Dataset.ProductClass.All(pc => pc.Name != productClass.ProductClass))
                    {
                        cboProductClass.Items.Add(productClass.ProductClass);
                    }

                    cboProductClass.Text = productClass.ProductClass;
                }
            }
        }

        public void SaveRow()
        {
            if (CurrentOrder == null || !CurrentOrder.IsValidState() || Dataset == null)
            {
                return;
            }

            var currentProductClass = CurrentOrder.GetOrderProductClassRows().FirstOrDefault();
            var productClass = _editorType == ProductClassType.Textbox ? txtProductClass.Text : cboProductClass.Text;

            if (string.IsNullOrEmpty(productClass))
            {
                currentProductClass?.Delete();
            }
            else if (currentProductClass != null)
            {
                if (currentProductClass.IsProductClassNull() || currentProductClass.ProductClass != productClass)
                {
                    currentProductClass.ProductClass = productClass;
                }
            }
            else
            {
                var newClassRow = Dataset.OrderProductClass.NewOrderProductClassRow();
                newClassRow.OrderRow = CurrentOrder;
                newClassRow.ProductClass = productClass;
                Dataset.OrderProductClass.AddOrderProductClassRow(newClassRow);
            }
        }

        public void LoadData(OrdersDataSet dataset, CustomersDataset.Customer_FieldsDataTable dtCustomerFields)
        {
            _editorType = ApplicationSettings.Current.ProductClassEditorType;

            txtProductClass.Visible = _editorType == ProductClassType.Textbox;
            cboProductClass.Visible = _editorType == ProductClassType.Combobox;

            cboProductClass.ButtonsRight["Delete"].Visible = !(_productClassFieldLazy.Value?.IsRequired ?? false);

            Dataset = dataset;
            CustomerFields = dtCustomerFields;
        }

        private void OnDispose()
        {
            Dataset = null;
            CustomerFields = null;
        }

        private bool IsProductClassRequired(int customerId)
        {
            var systemField = _productClassFieldLazy.Value;
            var requiredBySystem = systemField == null || systemField.IsRequired;

            var customerField = CustomerFields.FirstOrDefault(field => field.CustomerID == customerId && field.FieldID == systemField?.FieldID);
            var requiredByCustomer = customerField?.Required ?? false;

            return requiredBySystem || requiredByCustomer;
        }

        #endregion

        #region Events

        private void cboProductClass_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                if (e.Button.Key != "Delete")
                {
                    return;
                }

                cboProductClass.Text = string.Empty;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error deleting data from product class dropdown.");
            }
        }

        #endregion
    }
}
