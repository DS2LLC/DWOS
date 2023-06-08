using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Utilities.Validation;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinToolTip;
using NLog;

namespace DWOS.UI.Utilities
{
    public partial class CustomFieldsWidget : UserControl
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private Dictionary<int, string> _dictionary = new Dictionary<int, string>();

        #endregion

        #region Properties

        public Dictionary<int, string> CustomFields
        {
            get 
            {
                _dictionary.Clear();

                foreach (Control control in this.tableCustomFields.Controls)
                {
                    if (control is UltraTextEditor && control.Tag is CustomFieldUIInfo)
                    {
                        int customFieldID = ((CustomFieldUIInfo)control.Tag).CustomFieldID;
                        string customFieldValue = ((UltraTextEditor)control).Value == null ? String.Empty : ((UltraTextEditor)control).Value.ToString();
                        _dictionary.Add(customFieldID, customFieldValue);
                    }
                }

                return _dictionary; 
            }
        } 

        #endregion

        #region Methods

        public CustomFieldsWidget()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Create the custom fields based on the customer.
        /// </summary>
        public int CreateCustomFields(int customerID, DWOS.Utilities.Validation.ValidatorManager manager, UltraToolTipManager tipManager, ErrorProvider errorProvider)
        {
            try
            {
                _log.Debug("Creating custom fields controls for customer {0}.", customerID);

                ClearCustomFields(manager);

                var dsOrders = new OrdersDataSet() { EnforceConstraints = false };
                
                using(var taCustomer = new CustomerSummaryTableAdapter())
                    taCustomer.FillByCustomerID(dsOrders.CustomerSummary, customerID);

                //Get the custom fields for customer
                using(var taCustomField = new CustomFieldTableAdapter())
                    taCustomField.FillByCustomer(dsOrders.CustomField, customerID);

                var tableRowCount = 0;

                this.tableCustomFields.Controls.Clear();

                //Find current customer
                var customer = dsOrders.CustomerSummary.FindByCustomerID(customerID);

                if (customer != null)
                {
                    var customFields = customer.GetCustomFieldRows();

                    foreach (var customField in customFields)
                    {
                        var label  = new Label { Text = customField.Name + ":", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                        var txtBox = new UltraTextEditor { Tag = new CustomFieldUIInfo { CustomFieldID = customField.CustomFieldID }, Dock = DockStyle.Fill, MaxLength = 255, Margin = new Padding(2, 0, 0, 2) };

                        if (!customField.IsDescriptionNull())
                            tipManager.SetUltraToolTip(txtBox, new UltraToolTipInfo(customField.Description, ToolTipImage.Info, customField.Name, DefaultableBoolean.True));

                        if (customField.Required)
                        {
                            var validator = new ImageDisplayValidator(new TextControlValidator(txtBox, customField.Name + " is required."), errorProvider);
                            ((CustomFieldUIInfo)txtBox.Tag).Validator = validator;
                            manager.Add(validator);
                        }

                        this.tableCustomFields.Controls.Add(label, 0, tableRowCount);
                        this.tableCustomFields.Controls.Add(txtBox, 1, tableRowCount);

                        tableRowCount++;
                    }
                }

                this.tableCustomFields.RowCount = tableRowCount;
                this.pnlCustomFields.Height = tableRowCount > 0 ? this.tableCustomFields.Height : 0;
                this.pnlCustomFields.Visible = tableRowCount > 0;

                return tableRowCount;
            }
            catch (Exception exc)
            {
                string errorMsg = "Error creating customer custom fields.";
                _log.Error(exc, errorMsg);
                return 0;
            }
        }

        public void ClearCustomFields(DWOS.Utilities.Validation.ValidatorManager manager)
        {
            try
            {
                //clean up previous controls
                var controls = new Control[this.tableCustomFields.Controls.Count];
                this.tableCustomFields.Controls.CopyTo(controls, 0);

                foreach (var control in controls)
                {
                    //remove the validator from the previous custom field
                    if (control.Tag is CustomFieldUIInfo && ((CustomFieldUIInfo)control.Tag).Validator != null)
                        manager.Remove(((CustomFieldUIInfo)control.Tag).Validator);

                    control.DataBindings.Clear();
                    control.Dispose();
                }

                this.tableCustomFields.Controls.Clear();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error clearing previous custom fields.");
            }
        }

        #endregion

        #region CustomFieldUIInfo

        private class CustomFieldUIInfo
        {
            public int CustomFieldID { get; set; }
            public DisplayValidator Validator { get; set; }
        }

        #endregion
    }
}
