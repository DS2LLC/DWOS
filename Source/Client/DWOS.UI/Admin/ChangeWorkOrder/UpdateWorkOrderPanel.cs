using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Datasets;
using NLog;
using DWOS.Shared;
using DWOS.Data.Datasets.OrderHistoryDataSetTableAdapters;
using DWOS.UI.Utilities;
using System.Collections.Generic;
using System.Drawing;
using Infragistics.Win;

namespace DWOS.UI.Admin.ChangeWorkOrder
{
    /// <summary>
    /// Panel for <see cref="ChangeWorkOrderMain"/> - allows user to change a
    /// locked Work Order.
    /// </summary>
    public partial class UpdateWorkOrderPanel : UserControl, IChangeWorkOrderPanel
    {
        #region Fields

        private int? _customerId;
        private int? _partId;

        private readonly ISet<int> _loadedCustomerIds =
            new HashSet<int>();

        private readonly OrdersDataSet _dsOrders =
            new OrdersDataSet();

        private readonly OrderHistoryTableAdapter _taOrderHistory
            = new OrderHistoryTableAdapter();

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        public UpdateWorkOrderPanel()
        {
            InitializeComponent();

            // Setup cboPart
            cboPart.DataSource = new DataView(_dsOrders.PartSummary);
            cboPart.DisplayMember = _dsOrders.PartSummary.NameColumn.ColumnName;
            cboPart.ValueMember = _dsOrders.PartSummary.PartIDColumn.ColumnName;

            // Setup cboCustomer
            cboCustomer.DataSource = new DataView(_dsOrders.CustomerSummary);
            cboCustomer.DisplayMember = _dsOrders.CustomerSummary.NameColumn.ColumnName;
            cboCustomer.ValueMember = _dsOrders.CustomerSummary.CustomerIDColumn.ColumnName;
        }

        private void LoadParts()
        {
            var customerId= _customerId ?? 0;

            if (_loadedCustomerIds.Contains(customerId))
            {
                return;
            }

            using (var taPart = new PartSummaryTableAdapter { ClearBeforeFill = false })
            {
                taPart.FillByCustomerActive(_dsOrders.PartSummary, customerId);
            }

            _loadedCustomerIds.Add(customerId);
        }

        private void UpdatePartFilter()
        {
            (cboPart.DataSource as DataView).RowFilter = _customerId.HasValue
                ? $"CustomerID = {_customerId}"
                : "0 = 1";

            cboPart.DataBind();
        }

        private void LogMessage(string msg)
        {
            txtStatus.AppendText(msg);
            txtStatus.AppendText(Environment.NewLine);
        }

        #endregion

        #region Events

        private void cboCustomer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                _customerId = cboCustomer.Value as int?;
                cboPart.Value = null;

                LoadParts();
                UpdatePartFilter();
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error handling cboCustomer ValueChanged event.");
            }
        }

        private void cboPart_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                _partId = cboPart.Value as int?;
                OnCanNavigateToNextPanelChange?.Invoke(this, CanNavigateToNextPanel);
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error handling cboPart ValueChanged event.");
            }
        }

        private void cboPart_InitializeDataItem(object sender, Infragistics.Win.InitializeDataItemEventArgs e)
        {
            try
            {
                var partId = (e.ValueListItem.DataValue as int?) ?? 0;
                var partRow = _dsOrders.PartSummary.FindByPartID(partId);

                // Use different styling for inactive parts.
                if (partRow != null && !partRow.Active)
                {
                    e.ValueListItem.Appearance.FontData.Italic = DefaultableBoolean.True;
                    e.ValueListItem.Appearance.ForeColor = Color.Gray;
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error initializing part dropdown item.");
            }
        }

        #endregion

        #region IChangeWorkOrderPanel Implementation

        public ChangeWorkOrderMain MainForm { get; set; }

        public Action<IChangeWorkOrderPanel, bool> OnCanNavigateToNextPanelChange { get; set; }

        public bool CanNavigateToNextPanel =>
            _partId.HasValue && _customerId.HasValue;

        public void Finish()
        {
            try
            {
                var customerId = _customerId
                    ?? throw new InvalidOperationException("User did not select a customer.");

                var partId = _partId
                    ?? throw new InvalidOperationException("User did not select a part.");

                var userName = SecurityManager.Current?.UserName
                    ?? throw new InvalidOperationException("User is logged-out.");

                var orderId = MainForm.SelectedOrderId ?? 0;

                LogMessage($"Starting update for WO #{orderId}");

                // Update part
                int? originalPartId;
                int? originalCustomerId;

                using (var taOrder = new OrderTableAdapter())
                {
                    using (var dtOrder = taOrder.GetByOrderID(orderId))
                    {
                        var order = dtOrder.FirstOrDefault();

                        if (order == null)
                        {
                            throw new InvalidOperationException("Order not found.");
                        }

                        originalPartId = order.IsPartIDNull()
                            ? (int?)null
                            : order.PartID;

                        if (partId != originalPartId)
                        {
                            order.PartID = partId;
                        }

                        originalCustomerId = order.IsCustomerIDNull()
                            ? (int?)null
                            : order.CustomerID;

                        if (customerId != originalCustomerId)
                        {
                            order.CustomerID = customerId;
                            order.SetCustomerAddressIDNull();
                            order.SetShippingMethodNull();
                        }

                        taOrder.Update(dtOrder);
                    }
                }

                LogMessage("Successfully updated part.");

                // Update custom fields
                var statusMessages = new List<string>();

                using (var dtOrderCustomField = new OrdersDataSet.OrderCustomFieldsDataTable())
                {
                    using (var taOrderCustomField = new OrderCustomFieldsTableAdapter())
                    {
                        taOrderCustomField.FillByOrder(dtOrderCustomField, orderId);

                        var orderCustomFields = dtOrderCustomField.ToList();

                        foreach (var orderCustomFieldRow in orderCustomFields)
                        {
                            var originalFieldRow = _dsOrders.CustomField
                                .FindByCustomFieldID(orderCustomFieldRow.CustomFieldID);

                            if (originalFieldRow == null)
                            {
                                _logger.Error("Unable to load original Custom Field row.");
                                continue;
                            }

                            var matchingFieldRow = _dsOrders.CustomField
                                .FirstOrDefault(f => f.CustomerID == customerId
                                    && string.Equals(
                                        f.Name?.Trim(),
                                        originalFieldRow.Name?.Trim(),
                                        StringComparison.InvariantCultureIgnoreCase));

                            if (matchingFieldRow == null)
                            {
                                if (orderCustomFieldRow.IsValueNull() || string.IsNullOrEmpty(orderCustomFieldRow.Value))
                                {
                                    statusMessages.Add($"Deleted custom field {originalFieldRow.Name} from order.");
                                }
                                else
                                {
                                    statusMessages.Add($"Deleted custom field {originalFieldRow.Name} from order - value was " +
                                        $"\"{orderCustomFieldRow.Value}\".");
                                }

                                orderCustomFieldRow.Delete();
                            }
                            else
                            {
                                statusMessages.Add($"Updated custom field {originalFieldRow.Name} " +
                                    $"to use field #{matchingFieldRow.CustomFieldID}.");

                                orderCustomFieldRow.CustomFieldID = matchingFieldRow.CustomFieldID;
                            }
                        }

                        taOrderCustomField.Update(dtOrderCustomField);
                    }

                    foreach (var statusMessage in statusMessages)
                    {
                        _taOrderHistory.UpdateOrderHistory(
                            orderId,
                            "Administration",
                            statusMessage,
                            userName);
                    }

                    if (dtOrderCustomField.Count > 0)
                    {
                        LogMessage("Successfully updated custom fields.");
                    }

                }

                // Add log file entry
                if (partId != originalPartId)
                {
                    _taOrderHistory.UpdateOrderHistory(
                        orderId,
                        "Administration",
                        $"Changed part from {originalPartId} to {partId} via an administration tool.",
                        userName);
                }

                if (customerId != originalCustomerId)
                {
                    _taOrderHistory.UpdateOrderHistory(
                        orderId,
                        "Administration",
                        $"Changed customer from {originalCustomerId} to {customerId} via an administration tool.",
                        userName);
                }

                LogMessage("Successfully updated order history.");
                LogMessage("------------------------------------------");
                LogMessage("UPDATE SUCCESSFUL - please double-check the Work Order in Order Entry to see if all data is correct.");
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog(
                    "An error occurred while updating the work order.",
                    exc,
                    false);

                LogMessage("UPDATE FAILED");

                throw;
            }
        }

        public void OnNavigateFrom()
        {
            // Do nothing
        }

        public void OnNavigateTo()
        {
            cboCustomer.ValueChanged -= cboCustomer_ValueChanged;
            cboPart.ValueChanged -= cboPart_ValueChanged;

            try
            {
                txtStatus.Clear();
                cboCustomer.Value = null;
                cboPart.Value = null;

                var orderId = MainForm.SelectedOrderId ?? 0;
                numWorkOrder.Value = orderId;

                // Get current part and customer
                _customerId = null;
                _partId = null;

                using (var taOrder = new OrderTableAdapter())
                {
                    using (var dtOrder = taOrder.GetByOrderID(orderId))
                    {
                        var order = dtOrder.FirstOrDefault();

                        if (order == null)
                        {
                            throw new InvalidOperationException("Order not found.");
                        }

                        if (!order.IsCustomerIDNull())
                        {
                            _customerId = order.CustomerID;
                        }

                        if (!order.IsPartIDNull())
                        {
                            _partId = order.PartID;
                        }
                    }
                }

                // Load data
                using (var taCustomer = new CustomerSummaryTableAdapter { ClearBeforeFill = false })
                {
                    taCustomer.FillByActiveOrInUse(_dsOrders.CustomerSummary);
                }

                using (var taCustomField = new CustomFieldTableAdapter { ClearBeforeFill = false })
                {
                    foreach (var customerId in _dsOrders.CustomerSummary.Select(c => c.CustomerID))
                    {
                        taCustomField.FillByCustomer(_dsOrders.CustomField, customerId);
                    }
                }

                LoadParts();

                // Ensure that the Work Order's current part has been loaded.
                if (_partId.HasValue && _dsOrders.PartSummary.FindByPartID(_partId.Value) == null)
                {
                    using (var taPart = new PartSummaryTableAdapter { ClearBeforeFill = false })
                    {
                        taPart.FillByPart(_dsOrders.PartSummary, _partId.Value);
                    }
                }

                // Update customer selection
                if (_customerId.HasValue)
                {
                    cboCustomer.Value = _customerId.Value;
                }
                else
                {
                    cboCustomer.SelectedIndex = 0;
                }

                // Update part selection
                UpdatePartFilter();

                if (_partId.HasValue)
                {
                    cboPart.Value = _partId.Value;
                }
                else
                {
                    cboPart.SelectedIndex = 0;
                }
            }
            finally
            {
                cboCustomer.ValueChanged += cboCustomer_ValueChanged;
                cboPart.ValueChanged += cboPart_ValueChanged;
            }
        }

        #endregion
    }
}
