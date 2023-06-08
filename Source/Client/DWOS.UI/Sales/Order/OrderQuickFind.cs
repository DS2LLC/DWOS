using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using NLog;

namespace DWOS.UI.Sales
{
    public partial class OrderQuickFind : UserControl
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly ReadOnlyCollection<OrderSearchField> _exactSearchFields = new ReadOnlyCollection
            <OrderSearchField>(
                new List<OrderSearchField>
                {
                    OrderSearchField.SO,
                    OrderSearchField.WO,
                    OrderSearchField.COC,
                    OrderSearchField.Quantity,
                    OrderSearchField.Batch,
                    OrderSearchField.Package
                });

        private readonly Lazy<bool> _showSerialNumberLazy = new Lazy<bool>(
            () =>
            {
                ApplicationSettingsDataSet.FieldsDataTable fields;

                using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                {
                    fields = ta.GetByCategory("Order");
                }

                var requiredDateField = fields.FirstOrDefault(f => f.Name == "Serial Number");

                return requiredDateField != null && (requiredDateField.IsRequired || requiredDateField.IsVisible);
            });

        private readonly Lazy<bool> _showProductClassLazy = new Lazy<bool>(
            () =>
            {
                ApplicationSettingsDataSet.FieldsDataTable fields;

                using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                {
                    fields = ta.GetByCategory("Order");
                }

                var requiredDateField = fields.FirstOrDefault(f => f.Name == "Product Class");

                return requiredDateField != null && (requiredDateField.IsRequired || requiredDateField.IsVisible);
            });

        public event EventHandler FindOrder;

        #endregion

        #region Properties

        public OrderSearchField SelectedField => SelectedFieldItem.Type;

        public string CustomFieldName => SelectedFieldItem.CustomFieldName;

        public string SearchItem
        {
            get { return this.txtSearchItem.Text; }
        }

        public bool ExactMatch
        {
            get { return chkExactMatch.Checked; }
        }

        private FieldItem SelectedFieldItem => cboFilterField.Value as FieldItem;

        #endregion

        #region Methods

        public OrderQuickFind()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            this.cboFilterField.Items.Clear();

            // Fields
            this.cboFilterField.Items.Add(FieldItem.ForField(OrderSearchField.SO));
            this.cboFilterField.Items.Add(FieldItem.ForField(OrderSearchField.WO));
            this.cboFilterField.Items.Add(FieldItem.ForField(OrderSearchField.PO));
            this.cboFilterField.Items.Add(FieldItem.ForField(OrderSearchField.COC));
            this.cboFilterField.Items.Add(FieldItem.ForField(OrderSearchField.Part));
            this.cboFilterField.Items.Add(FieldItem.ForField(OrderSearchField.Customer));
            this.cboFilterField.Items.Add(FieldItem.ForField(OrderSearchField.CustomerWO));
            this.cboFilterField.Items.Add(FieldItem.ForField(OrderSearchField.Quantity));
            this.cboFilterField.Items.Add(FieldItem.ForField(OrderSearchField.User));
            this.cboFilterField.Items.Add(FieldItem.ForField(OrderSearchField.Batch));
            this.cboFilterField.Items.Add(FieldItem.ForField(OrderSearchField.Package));

            if (_showSerialNumberLazy.Value)
            {
                this.cboFilterField.Items.Add(FieldItem.ForField(OrderSearchField.SerialNumber));
            }

            if (_showProductClassLazy.Value)
            {
                this.cboFilterField.Items.Add(FieldItem.ForField(OrderSearchField.ProductClass));
            }

            // Custom Fields
            using (var dtCustomField = new OrdersDataSet.CustomFieldDataTable())
            {
                using (var taCustomField = new CustomFieldTableAdapter())
                {
                    taCustomField.Fill(dtCustomField);
                }

                var customFieldNames = dtCustomField
                        .Select(f => f.Name)
                        .Distinct()
                        .OrderBy(name => name);

                foreach (var name in customFieldNames)
                {
                    cboFilterField.Items.Add(FieldItem.ForCustomField(name));
                }
            }

            this.cboFilterField.SelectedIndex = 0;
        }

        #endregion

        #region Events

        private void OrderQuickFind_Load(object sender, EventArgs e) { LoadData(); }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if(FindOrder != null)
                FindOrder(this, e);
        }

        private void OrderQuickFind_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {
                this.txtSearchItem.Focus();
                this.txtSearchItem.SelectAll();
            }
        }

        private void txtSearchItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char) 13)
                this.btnFind.PerformClick();
        }

        private void cboFilterField_ValueChanged(object sender, EventArgs e)
        {
            chkExactMatch.CheckedChanged -= chkExactMatch_CheckedChanged;
            try
            {
                var lockedToExact = _exactSearchFields.Contains(SelectedField);

                if (lockedToExact)
                {
                    chkExactMatch.Checked = true;
                    chkExactMatch.Enabled = false;
                }
                else
                {
                    chkExactMatch.Checked = UserSettings.Default.OrderEntryExactSearch;
                    chkExactMatch.Enabled = true;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing filter value.");
            }
            finally
            {
                chkExactMatch.CheckedChanged += chkExactMatch_CheckedChanged;
            }
        }

        private void chkExactMatch_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UserSettings.Default.OrderEntryExactSearch = chkExactMatch.Checked;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error setting exact match value.");
            }
        }

        private void pnlExactMatch_MouseHover(object sender, EventArgs e)
        {
            try
            {
                // Show tool tip for disabled control
                if (chkExactMatch.Enabled)
                {
                    return;
                }

                ultraToolTipManager.ShowToolTip(chkExactMatch);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error showing exact match tool tip.");
            }
        }

        private void pnlExactMatch_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                // Hide tool tip for disabled control
                if (chkExactMatch.Enabled || !ultraToolTipManager.IsToolTipVisible(chkExactMatch))
                {
                    return;
                }

                ultraToolTipManager.HideToolTip();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error hiding exact match tool tip.");
            }
        }

        #endregion

        #region FieldItem

        private class FieldItem
        {
            public OrderSearchField Type { get; private set; }

            public string CustomFieldName { get; private set; }

            public static FieldItem ForField(OrderSearchField type)
            {
                return new FieldItem
                {
                    Type = type
                };
            }

            public static FieldItem ForCustomField(string customFieldName)
            {
                return new FieldItem
                {
                    Type = OrderSearchField.Custom,
                    CustomFieldName = customFieldName
                };
            }

            public override string ToString()
            {
                return Type == OrderSearchField.Custom ? CustomFieldName : Type.ToString();
            }
        }

        #endregion
    }
}