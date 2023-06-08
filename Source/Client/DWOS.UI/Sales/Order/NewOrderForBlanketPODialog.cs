using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Utilities.Validation;
using Infragistics.Win.UltraWinEditors;
using NLog;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Order;

namespace DWOS.UI.Sales.Order
{
    /// <summary>
    /// Dialog that adds a new Work Order to a specified Blanket PO.
    /// </summary>
    /// <remarks>
    /// Before showing this dialog, call <see cref="LoadData(OrdersDataSet.OrderTemplateRow)"/>
    /// to retrieve data for the specified Blanket PO.
    /// </remarks>
    public partial class NewOrderForBlanketPODialog : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private DWOS.Utilities.Validation.ValidatorManager _validationManager;

        #endregion

        #region Properties

        public int PartQuantity
        {
            get { return this.numPartQty.Value == null ? 0 : Convert.ToInt32(this.numPartQty.Value); }
        }

        public Dictionary <int, string> CustomFields
        {
            get { return this.customFieldsWidget.CustomFields; }
        }

        #endregion

        #region Methods

        public NewOrderForBlanketPODialog()
        {
            InitializeComponent();

            _log.Info("Add Blanket PO form created.");

            _validationManager = new DWOS.Utilities.Validation.ValidatorManager();
        }

        public void LoadData(OrdersDataSet dataset, OrderTableAdapter taOrder, OrdersDataSet.OrderTemplateRow orderTemplate)
        {
            var allocHelper = BlanketAllocationHelper.From(dataset, taOrder);

            this.txtBlanketPONumber.Text = orderTemplate.OrderTemplateID.ToString();

            var usedQty = allocHelper.GetAllocatedQuantity(orderTemplate);

            this.numAvailableQty.Value = orderTemplate.InitialQuantity - usedQty;
            this.numPartQty.MaxValue = Math.Max(0, orderTemplate.InitialQuantity - usedQty);

            //Load custom fields for customer
            var fieldCount = this.customFieldsWidget.CreateCustomFields(orderTemplate.CustomerID, _validationManager, tipManager, errProvider);

            if(fieldCount > 0)
                this.tabFields.Tabs[1].Visible = true;
            else
                this.tabFields.Tabs[1].Visible = false;

            _validationManager.Add(new ImageDisplayValidator(new NumericControlValidator(this.numPartQty), errProvider));
            _validationManager.ValidationSummary = new TabValidationDisplay(tabFields);
        }

        #endregion

        #region Events

        private void AddBlanketPOOrder_Load(object sender, EventArgs e)
        {
            ActiveControl = this.numPartQty;
            this.numPartQty.Focus();
        }

        private void numPartQty_Enter(object sender, EventArgs e)
        {
            if(sender is UltraWinEditorMaskedControlBase)
                ((UltraWinEditorMaskedControlBase) sender).SelectAll();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(_validationManager.ValidateControls())
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        #endregion
    }
}