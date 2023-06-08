using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.Data.Datasets.OrderShipmentDataSetTableAdapters;
using DWOS.Data.Order;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using NLog;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Infragistics.Win.UltraWinEditors;

namespace DWOS.UI.ShippingRec.ShippingManagerPanels
{
    public partial class ShipmentInfo : DataPanel
    {
        #region Fields

        private const string WEIGHT_COLUMN = "Weight";
        private const string ORDER_ID_COLUMN = "OrderID";
        private const string PART_QTY_COLUMN = "PartQuantity";

        private const string WEIGHT_FIELD_NAME = "Weight";
        private const string FIELD_CATEGORY_PARTS = "Part";
        private OrderShipmentTableAdapter _taOrderShipment;

        private readonly ConcurrentDictionary<int, List<int>> _relatedCustomers =
            new ConcurrentDictionary<int, List<int>>();

        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("ShippingInfo", new UltraGridBandSettings());

        /// <summary>
        /// Used for calculating order gross weight
        /// </summary>
        private OrdersDataSet _dsOrders;

        public event EventHandler <OrderDeletedEventArgs> OrderDeleted;

        public event EventHandler ValueChanged;

        public class OrderDeletedEventArgs : EventArgs
        {
            public int ShipmentPackageID { get; set; }
            public int OrderShipmentID { get; set; }
        }

        #endregion

        #region Properties

        private OrderShipmentDataSet Dataset
        {
            get { return base._dataset as OrderShipmentDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.ShipmentPackage.ShipmentPackageIDColumn.ColumnName; }
        }

        /// <summary>
        ///     Gets the currently selected order ID.
        /// </summary>
        internal int? SelectedOrderID
        {
            get
            {
                if(this.grdOrders.Selected.Rows.Count == 1)
                    return (int) this.grdOrders.Selected.Rows[0].Cells["OrderID"].Value;

                if(this.grdOrders.ActiveRow != null)
                    return (int) this.grdOrders.ActiveRow.Cells["OrderID"].Value;

                return null;
            }
        }

        private bool DisplayWeightColumn { get; set; }

        #endregion

        #region Methods

        public ShipmentInfo() { InitializeComponent(); }

        public void LoadData(OrderShipmentDataSet dataset, OrderShipmentTableAdapter taOS)
        {
            _dsOrders = new OrdersDataSet() { EnforceConstraints = false };
            this._taOrderShipment = taOS;
            Dataset = dataset;

            var fields = new ApplicationSettingsDataSet.FieldsDataTable();
            using(var ta = new FieldsTableAdapter())
                ta.FillByCategory(fields, FIELD_CATEGORY_PARTS);

            var weightField = fields.FirstOrDefault(f => f.Name == WEIGHT_FIELD_NAME);
            if(weightField != null && weightField.IsVisible)
                DisplayWeightColumn = true;

            if(!DisplayWeightColumn)
            {
                var numWeightHeight = numWeight.Height + 6; // offset determined through trial & error
                this.lblWeight.Visible = false;
                this.numWeight.Visible = false;

                grpShippingInfo.Height -= numWeightHeight;
                grpOrders.Location = new Point(grpOrders.Location.X, grpOrders.Location.Y - numWeightHeight);
                grpOrders.Height += numWeightHeight;
            }

            //Add new columns Weight
            if(!Dataset.OrderShipment.Columns.Contains(WEIGHT_COLUMN))
                Dataset.OrderShipment.Columns.Add(WEIGHT_COLUMN, typeof(double));

            //bind order shipments grid
            this.bsOrderShipments.DataSource = Dataset;
            this.bsOrderShipments.DataMember = Dataset.OrderShipment.TableName;

            //bind customer contacts
            this.bsCustomerContacts.DataSource = Dataset;
            this.bsCustomerContacts.DataMember = Dataset.d_Contact.TableName;

            //bind shipment packages
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.ShipmentPackage.TableName;

            // Bind shipping carriers
            bsShippingCarrier.DataSource = Dataset;
            bsShippingCarrier.DataMember = Dataset.d_ShippingCarrier.TableName;

            //bind shipping addresses
            bsCustomerAddress.DataSource = Dataset;
            bsCustomerAddress.DataMember = Dataset.CustomerAddress.TableName;

            //bind column to control
            // UpdateMode for several bindings is Never because they are manually updated;
            // otherwise, database sync fails to use updated values.
            base.BindValue(this.numPackageNumber, Dataset.ShipmentPackage.PackageNumberColumn.ColumnName,
                DataSourceUpdateMode.Never);

            base.BindValue(this.cboCustomer, Dataset.ShipmentPackage.CustomerIDColumn.ColumnName);

            base.BindValue(cboPackageType, Dataset.ShipmentPackage.ShipmentPackageTypeIDColumn.ColumnName,
                DataSourceUpdateMode.Never);

            base.BindValue(this.cboShipTo, Dataset.ShipmentPackage.CustomerAddressIDColumn.ColumnName,
                DataSourceUpdateMode.Never);

            base.BindValue(this.cboShippingCarrier, Dataset.ShipmentPackage.ShippingCarrierIDColumn.ColumnName,
                DataSourceUpdateMode.Never);

            base.BindValue(this.txtTrackingNumber, Dataset.ShipmentPackage.TrackingNumberColumn.ColumnName,
                DataSourceUpdateMode.Never);

            base.BindValue(this.txtCarrierNumber, Dataset.ShipmentPackage.CarrierCustomerNumberColumn.ColumnName,
                DataSourceUpdateMode.Never);

            base.BindValue(numWeight, Dataset.ShipmentPackage.GrossWeightColumn.ColumnName,
                DataSourceUpdateMode.Never);

            base.BindValue(this.dteShipDate, Dataset.ShipmentPackage.CloseDateColumn.ColumnName,
               DataSourceUpdateMode.Never);

            //pick up changes to update notifications column
            this.cboNotifications.ValueChanged += cboNotifications_ValueChanged;
            this.cboNotifications.InitializeLayout += CboNotifications_InitializeLayout;

            //bind lists
            base.BindList(this.cboPackageType, Dataset.ShipmentPackageType,
                Dataset.ShipmentPackageType.ShipmentPackageTypeIDColumn.ColumnName,
                Dataset.ShipmentPackageType.NameColumn.ToString());

            base.BindList(this.cboShippingCarrier, Dataset.d_ShippingCarrier, Dataset.d_ShippingCarrier.CarrierIDColumn.ColumnName, Dataset.d_ShippingCarrier.CarrierIDColumn.ColumnName);
            base.BindList(this.cboUser, Dataset.Users, Dataset.Users.UserIDColumn.ColumnName, Dataset.Users.NameColumn.ColumnName);
            base.BindList(this.cboCustomer, Dataset.Customer, Dataset.Customer.CustomerIDColumn.ColumnName, Dataset.Customer.NameColumn.ColumnName);

            this.cboShipTo.DataSource = this.bsCustomerAddress;
            this.cboShipTo.ValueMember = Dataset.CustomerAddress.CustomerAddressIDColumn.ColumnName;
            this.cboShipTo.DisplayMember = Dataset.CustomerAddress.NameColumn.ColumnName;

            cboShippingCarrier.DataSource = bsShippingCarrier;
            cboShippingCarrier.DisplayMember = Dataset.d_ShippingCarrier.CarrierIDColumn.ColumnName;
            cboShippingCarrier.ValueMember = Dataset.d_ShippingCarrier.CarrierIDColumn.ColumnName;

            LoadCurrentShipperInfo();

            numWeight.MaskInput =
                $"nnnnnnnn.{string.Concat(Enumerable.Repeat("n", ApplicationSettings.Current.WeightDecimalPlaces))} lbs";

            // Show/hide tracking number
            var trackingNumberField = FieldUtilities.GetField("Shipping", "Tracking Number");

            if (!trackingNumberField.IsVisible)
            {
                var pnlShippingTrackingNumberHeight = pnlShippingTrackingNumber.Height;
                pnlShippingTrackingNumber.Visible = false;
                grpShippingInfo.Controls.Remove(pnlShippingTrackingNumber); // Fixes resize issue & allows panels below to resize correctly
                grpShippingInfo.Height -= pnlShippingTrackingNumberHeight;
                grpOrders.Location = new Point(grpOrders.Location.X, grpOrders.Location.Y - pnlShippingTrackingNumberHeight);
                grpOrders.Height += pnlShippingTrackingNumberHeight;
            }

            // Mark panel as loaded
            base._panelLoaded = true;
        }

        private void CboNotifications_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            this.cboNotifications.DisplayMember = "Email";
            this.cboNotifications.ValueMember = "ID";
            this.cboNotifications.CheckedListSettings.ItemCheckArea = ItemCheckArea.Item;
            this.cboNotifications.CheckedListSettings.EditorValueSource = Infragistics.Win.EditorWithComboValueSource.CheckedItems;
            this.cboNotifications.CheckedListSettings.ListSeparator = ",";
            this.cboNotifications.CheckedListSettings.CheckStateMember = "Selected";
            this.cboNotifications.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
            e.Layout.Bands[0].Columns["ID"].Hidden = true;
        }

        /// <summary>
        ///     Adds a new shipment package based on the customer ID.
        /// </summary>
        /// <param name="customerID">The customer ID.</param>
        /// <param name="packageNumber">The package number.</param>
        /// <returns></returns>
        public OrderShipmentDataSet.ShipmentPackageRow AddShipmentPackage(int customerID, int packageNumber)
        {
            //create new row
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as OrderShipmentDataSet.ShipmentPackageRow;

            //find customer number based on order id
            cr.CustomerID = customerID;
            cr.PackageNumber = packageNumber;
            cr.CloseDate = DateTime.Now;
            cr.Active = true;
            cr.TrackingNumber = null;
            cr.CarrierCustomerNumber = null;

            //Set shipping carrier
            var defaultCustomerShipping = Dataset.CustomerShipping
                .FirstOrDefault(i => i.CustomerID == customerID && i.DefaultShippingMethod && i.Active);

            if (defaultCustomerShipping != null)
            {
                cr.ShippingCarrierID = defaultCustomerShipping.CarrierID;
            }

            // Set address
            var address = Dataset.CustomerAddress
                .FirstOrDefault(i => i.CustomerID == customerID && i.IsDefault && i.Active);

            if (address != null)
            {
                cr.CustomerAddressID = address.CustomerAddressID;
            }

            //get default notifications
            var contactIds = Dataset.d_Contact
                .Where(c => IsValidContactFor(c, customerID))
                .Select(c => c.ContactID)
                .ToList();

            var contactFilter = contactIds.Count > 0
                ? $"ContactID IN ({string.Join(",", contactIds)})"
                : "1 = 0";

            var contacts = Dataset.d_Contact.Select(contactFilter) as OrderShipmentDataSet.d_ContactRow[];
            if(contacts != null && contacts.Length > 0)
            {
                string notifications = null;
                foreach(var item in contacts)
                {
                    if(notifications != null)
                        notifications += ",";
                    notifications += item.EmailAddress;
                }

                cr.NotificationEmails = notifications;
            }

            rowVw.EndEdit(); //required
            return cr;
        }


        private static bool IsValidContactFor(OrderShipmentDataSet.d_ContactRow c, int customerId) =>
            c != null && c.Active
                && !c.IsEmailAddressNull()
                && (c.CustomerID == customerId || (ApplicationSettings.Current.AllowAdditionalCustomersForContacts && c.GetContactAdditionalCustomerSummaryRows().Any(customer => customer.CustomerID == customerId && customer.IncludeInShippingNotifications)));


        /// <summary>
        ///     Adds a new order shipment to the current shipment package.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        public OrderShipmentDataSet.OrderShipmentRow AddOrderShipment(int orderID)
        {
            this.grdOrders.UpdateData();
            this.bsOrderShipments.SuspendBinding();

            var shipRow = CurrentRecord as OrderShipmentDataSet.ShipmentPackageRow;
            int partQty = this._taOrderShipment.GetPartQuantity(orderID).GetValueOrDefault(0);

            _log.Info("AddOrderShipment: Adding new order " + orderID + " to package " + shipRow.ShipmentPackageID);

            //create new row
            var rowVw = this.bsOrderShipments.AddNew() as DataRowView;
            var orderShipment = rowVw.Row as OrderShipmentDataSet.OrderShipmentRow;

            orderShipment.OrderID = orderID;
            orderShipment.ShippingUserID = SecurityManager.Current.UserID;
            orderShipment.DateShipped = DateTime.Now;
            orderShipment.PartQuantity = partQty;
            orderShipment.ShipmentPackageID = shipRow.ShipmentPackageID;

            if(!shipRow.IsShippingCarrierIDNull())
                orderShipment.ShippingCarrierID = shipRow.ShippingCarrierID;
            if(!shipRow.IsTrackingNumberNull())
                orderShipment.TrackingNumber = shipRow.TrackingNumber;
            if(!shipRow.IsCarrierCustomerNumberNull())
                orderShipment.CarrierCustomerNumber = shipRow.CarrierCustomerNumber;
            if (!shipRow.IsCustomerAddressIDNull())
                orderShipment.CustomerAddressID = shipRow.CustomerAddressID;

            this.bsOrderShipments.ResumeBinding();

            rowVw.EndEdit();
            this.grdOrders.DataBind();

            return orderShipment;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            //filter grid to show only orders in this package
            this.bsOrderShipments.Filter = "ShipmentPackageID = " + id;

            //filter contact people by customer and those that want to recieve notifications
            var shipRow = CurrentRecord as OrderShipmentDataSet.ShipmentPackageRow;
            if(shipRow != null)
            {
                // Filter list of contacts
                var contactIds = Dataset.d_Contact
                    .Where(c => IsValidContactFor(c, shipRow.CustomerID))
                    .Select(c => c.ContactID)
                    .ToList();
             
                bsCustomerContacts.Filter = contactIds.Count > 0
                    ? $"ContactID IN ({string.Join(",", contactIds)})"
                    : "1 = 0";

                //If drawing for the first time. We may want to store tables for each customer/table pair in a dictionary so that we don't have to redraw the table on every node change...
                if (this.cboNotifications.DataSource is null)
                {
                    //format notifications dropdown
                    var dt = new DataTable();
                    dt.Columns.Add("Selected", typeof(bool));
                    dt.Columns.Add("ID", typeof(int));
                    dt.Columns.Add("Email", typeof(string));
                    dt.Columns.Add("Name", typeof(string));
                    dt.Columns.Add("Phone", typeof(string));
                    foreach (var contactId in contactIds)
                    {
                        var row = Dataset.d_Contact.FirstOrDefault(c => contactId == c.ContactID);
                        //check for nulls -- Name and Email should always be valid but just in case
                        var ShipNotify = row.ShippingNotification;
                        var emailAddress = row.IsEmailAddressNull() ? string.Empty : row.EmailAddress;
                        var name = row.IsNameNull() ? string.Empty : row.Name;
                        var phoneNumber = row.IsPhoneNumberNull() ? string.Empty : row.PhoneNumber;
                        dt.Rows.Add(new object[] { ShipNotify, row.ContactID, emailAddress, name, phoneNumber });
                    }
                    this.cboNotifications.DataSource = dt;
                }
                else
                {   //Table exists. Just reset rows.
                    var notificationDT = this.cboNotifications.DataSource as DataTable;
                    notificationDT.Rows.Clear();
                    foreach (var contactId in contactIds)
                    {                        
                        var row = Dataset.d_Contact.FirstOrDefault(c => contactId == c.ContactID);
                        var ShipNotify = row.ShippingNotification;
                        //check for nulls -- Name and Email should always be valid but just in case
                        var emailAddress = row.IsEmailAddressNull() ? string.Empty : row.EmailAddress;
                        var name = row.IsNameNull() ? string.Empty : row.Name;
                        var phoneNumber = row.IsPhoneNumberNull() ? string.Empty : row.PhoneNumber;
                        notificationDT.Rows.Add(new object[] { ShipNotify, row.ContactID, emailAddress, name, phoneNumber });
                    }
                }
                // Filter list of addresses
                //    - Active -or-
                //    - Is the currently selected address
                var currentCustomerAddressId = shipRow.IsCustomerAddressIDNull()
                    ? -1
                    : shipRow.CustomerAddressID;

                bsCustomerAddress.Filter = $"CustomerID = {shipRow.CustomerID} AND " +
                    $"(Active = 1 OR CustomerAddressID = {currentCustomerAddressId})";

                // Filter shipping carriers
                // If the customer has any visible carriers, show them.
                // Otherwise, show all carriers.
                var currentCarrierId = shipRow.IsShippingCarrierIDNull()
                    ? null
                    : shipRow.ShippingCarrierID;

                var customerCarriers = Dataset.CustomerShipping
                    .Where(i => i.CustomerID == shipRow.CustomerID && (i.Active || i.CarrierID == currentCarrierId))
                    .ToList();

                if (customerCarriers.Count > 0)
                {
                    var formattedCarriers = customerCarriers
                        .Select(c => c.CarrierID)
                        .Distinct()
                        .Select(c => $"'{c}'");

                    bsShippingCarrier.Filter = $"CarrierID IN ({string.Join(",", formattedCarriers)})";
                }
                else
                {
                    // Show all carriers
                    bsShippingCarrier.Filter = null;
                }

                // Workaround for issue where cboShippingCarrier blanks its current value
                // when switching to a customer without the previously selected shipping carrier.
                try
                {
                    cboShippingCarrier.ValueChanged -= cboShippingCarrier_ValueChanged;
                    cboShippingCarrier.DataBindings[0].ReadValue();
                }
                finally
                {
                    cboShippingCarrier.ValueChanged += cboShippingCarrier_ValueChanged;
                }

                this.cboShipTo.DataBindings[0].ReadValue();

               
            }
            else
            {
                _log.Warn("No customer id filtered for contacts as package is null for id: " + (id ?? "Unknown"));
                this.bsCustomerContacts.Filter = "1 = 0";
                this.bsCustomerAddress.Filter = "CustomerID = 0";
            }

            UpdateMaximumNumberText();
        }

        public override void EndEditing()
        {
            this.bsOrderShipments.EndEdit();
            base.EndEditing();
        }

        private void LoadCurrentShipperInfo()
        {
            //find user id
            int userID = SecurityManager.Current.UserID;

            //select operator
            if(userID >= 0)
            {
                ValueListItem userItem = this.cboUser.FindItemByValue <int>(i => i == userID);
                if(userItem != null)
                    this.cboUser.SelectedItem = userItem;
            }
        }

        public OrderShipmentDataSet.CustomerShippingRow GetShippingCarrier(int orderID)
        {
            int? osMethod = this._taOrderShipment.GetOrderShippingMethod(orderID);

            if(osMethod.HasValue)
                return Dataset.CustomerShipping.FindByCustomerShippingID(osMethod.Value);

            return null;
        }

        public OrderShipmentDataSet.CustomerAddressRow GetCustomerAddress(int orderID)
        {
            int? customerAddressID = this._taOrderShipment.GetCustomerAddressID(orderID);

            OrderShipmentDataSet.CustomerAddressRow address = null;
            if (customerAddressID.HasValue)
            {
                address = Dataset.CustomerAddress.FindByCustomerAddressID(customerAddressID.Value);
            }

            return address;
        }

        private void UpdateWeights(UltraGridRow row)
        {
            try
            {
                if(!DisplayWeightColumn)
                    return;

                if(row != null)
                {
                    //update weight
                    if(row.Cells[PART_QTY_COLUMN].Value != null && row.Cells[PART_QTY_COLUMN].Value != DBNull.Value)
                    {
                        var partQty = Convert.ToInt32(row.Cells[PART_QTY_COLUMN].Value);
                        var orderId = Convert.ToInt32(row.Cells[ORDER_ID_COLUMN].Value);
                        var partWeight = this._taOrderShipment.GetPartWeightByOrder(orderId).GetValueOrDefault();

                        row.Cells[WEIGHT_COLUMN].Value = partQty * partWeight;
                    }

                    row.Update();
                }

                var totalGrossWeight = 0m;

                foreach(var orderRow in this.grdOrders.Rows)
                {
                    var orderId = orderRow.Cells[ORDER_ID_COLUMN].Value as int? ?? -1;
                    totalGrossWeight += CalculateGrossWeight(orderId);
                }

                numWeight.NullText = $"{totalGrossWeight:N2} lbs. (estimated)";
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating row surface area.");
            }
        }

        private void UpdateMaximumNumberText()
        {
            var shipRow = CurrentRecord as OrderShipmentDataSet.ShipmentPackageRow;

            if (shipRow == null || !shipRow.IsValidState())
            {
                lblMaximumNumber.Text = string.Empty;
                return;
            }

            var packages = Dataset.ShipmentPackage.SelectPackages(shipRow.CustomerID, shipRow.ShipmentPackageTypeID);

            var maxPackageCount = 0;
            if (packages.Length > 0)
            {
                maxPackageCount = packages[0].PackageNumber;
            }

            lblMaximumNumber.Text = $"of {maxPackageCount}";
        }

        private decimal CalculateGrossWeight(int orderId)
        {
            var order = _dsOrders.Order.FindByOrderID(orderId);

            if (order == null)
            {
                using (var taOrder = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter { ClearBeforeFill  = false })
                {
                    taOrder.FillByOrderID(_dsOrders.Order, orderId);
                }

                using (var taContainer = new Data.Datasets.OrdersDataSetTableAdapters.OrderContainersTableAdapter { ClearBeforeFill  = false })
                {
                    taContainer.FillByOrder(_dsOrders.OrderContainers, orderId);
                }

                order = _dsOrders.Order.FindByOrderID(orderId);
            }

            return OrderUtilities.CalculateGrossWeight(order);
        }

        #endregion

        #region Events

        private void grdOrders_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                grdOrders.AfterColPosChanged -= grdOrders_AfterColPosChanged;
                grdOrders.AfterSortChange -= grdOrders_AfterSortChange;

                var band = e.Layout.Bands[0];
                band.Summaries.Clear();

                foreach (var v in band.Columns)
                {
                    switch(v.Key)
                    {
                        case ORDER_ID_COLUMN:
                            v.Header.Caption = "WO";
                            v.CellActivation = Activation.NoEdit;
                            break;
                        case PART_QTY_COLUMN:
                            v.Header.Caption = "Quantity";
                            break;
                        default:
                            v.Hidden = true;
                            break;
                    }
                }

                if(DisplayWeightColumn)
                {
                    if(band.Columns.Exists(WEIGHT_COLUMN))
                    {
                        var col = band.Columns[WEIGHT_COLUMN];
                        col.Hidden = false;
                        col.Format = "###,###,###.00 lbs";
                        col.CellActivation = Activation.NoEdit;

                        var saSum = band.Summaries.Add(SummaryType.Sum, col);
                        saSum.DisplayFormat = "SUM: {0:N2} lbs";
                    }
                }


                if (band.Columns.Exists(ORDER_ID_COLUMN))
                {
                    var col = band.Columns[ORDER_ID_COLUMN];
                    band.Summaries.Add(SummaryType.Count, col);
                }

                if (band.Columns.Exists(PART_QTY_COLUMN))
                {
                    var col = band.Columns[PART_QTY_COLUMN];
                    band.Summaries.Add(SummaryType.Sum, col);
                }

                band.Override.SummaryFooterCaptionVisible = Infragistics.Win.DefaultableBoolean.False;
                band.Override.SummaryValueAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                band.Override.SummaryValueAppearance.FontData.SizeInPoints = 12;

                // Load grid settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(band);
            }
            finally
            {
                grdOrders.AfterColPosChanged += grdOrders_AfterColPosChanged;
                grdOrders.AfterSortChange += grdOrders_AfterSortChange;
            }
        }
        private void cboNotifications_ValueChanged(object sender, EventArgs e)
        {           
            if(!_recordLoading)
            {
                var shipRow = CurrentRecord as OrderShipmentDataSet.ShipmentPackageRow;
                if(shipRow != null)
                {
                    //force update underlying via direct set then call value changed
                    shipRow.NotificationEmails = this.cboNotifications.CheckedRows.Count > 0 ? this.cboNotifications.Text : null;
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void cboShipTo_ValueChanged(object sender, EventArgs e)
        {
            var shipRow = CurrentRecord as OrderShipmentDataSet.ShipmentPackageRow;

            if (_recordLoading || shipRow == null || cboShipTo.SelectedItem == null)
            {
                return;
            }

            var customerAddressId = Convert.ToInt32(cboShipTo.SelectedItem.DataValue);

            if (shipRow.IsCustomerAddressIDNull() || shipRow.CustomerAddressID != customerAddressId)
            {
                shipRow.CustomerAddressID = customerAddressId;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void grdOrders_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            try
            {
                foreach(var row in e.Rows)
                {
                    e.DisplayPromptMsg = false;

                    var shipment = ((DataRowView) row.ListObject).Row as OrderShipmentDataSet.OrderShipmentRow;
                    if(shipment != null)
                        OrderHistoryDataSet.UpdateOrderHistory(shipment.OrderID, "Shipping", "Order " + shipment.OrderID + " removed from package.", SecurityManager.Current.UserName);
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving order delete history.";
                _log.Error(exc, errorMsg);
            }
        }

        private void grdOrders_AfterRowsDeleted(object sender, EventArgs e)
        {
            this.grdOrders.UpdateData();

            var shipment = CurrentRecord as OrderShipmentDataSet.ShipmentPackageRow;

            DataRow[] orderRows = Dataset.OrderShipment.Select("ShipmentPackageID = " + shipment.ShipmentPackageID, "", DataViewRowState.Deleted);
            if(orderRows != null)
            {
                foreach(var or in orderRows)
                {
                    if(OrderDeleted != null)
                    {
                        int shipmentID = Convert.ToInt32(or[Dataset.OrderShipment.ShipmentIDColumn, DataRowVersion.Original]);
                        OrderDeleted(this, new OrderDeletedEventArgs {ShipmentPackageID = shipment.ShipmentPackageID, OrderShipmentID = shipmentID});
                    }
                }
            }

            UpdateWeights(null);
        }

        private void grdOrders_AfterRowInsert(object sender, RowEventArgs e) { UpdateWeights(e.Row); }

        private void grdOrders_InitializeRow(object sender, InitializeRowEventArgs e) { UpdateWeights(e.Row); }

        private void grdOrders_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == PART_QTY_COLUMN)
            {
                UpdateWeights(e.Cell.Row);

                // Save quantity change
                var rowView = e.Cell.Row.ListObject as DataRowView;
                var row = rowView?.Row as OrderShipmentDataSet.OrderShipmentRow;

                if (row != null)
                {
                    _taOrderShipment.Update(row);
                }
            }
        }

        private void cboShippingCarrier_ValueChanged(object sender, EventArgs e)
        {
            var shipRow = CurrentRecord as OrderShipmentDataSet.ShipmentPackageRow;

            if (_recordLoading || shipRow == null || cboShippingCarrier.SelectedItem == null)
            {
                return;
            }

            // Update data source
            var carrierId = cboShippingCarrier.SelectedItem.DataValue.ToString();

            if (shipRow.IsShippingCarrierIDNull() || carrierId != shipRow.ShippingCarrierID)
            {
                shipRow.ShippingCarrierID = carrierId;

                // Get the customer carrier number for the new shipping carrier ID
                var customerShippingRow =
                    Dataset.CustomerShipping.FirstOrDefault(
                        row => row.CarrierID == shipRow.ShippingCarrierID && row.CustomerID == shipRow.CustomerID);
                this.txtCarrierNumber.Text = customerShippingRow != null
                    ? customerShippingRow.CarrierCustomerNumber
                    : string.Empty;

                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void cboPackageType_ValueChanged(object sender, EventArgs e)
        {
            var shipRow = CurrentRecord as OrderShipmentDataSet.ShipmentPackageRow;
            if (_recordLoading || shipRow == null)
            {
                return;
            }

            // Update data source
            shipRow.ShipmentPackageTypeID = (cboPackageType.SelectedItem.DataValue as int?) ?? 1;
            ValueChanged?.Invoke(this, EventArgs.Empty);
            UpdateMaximumNumberText();
        }

        private void txtTrackingNumber_ValueChanged(object sender, EventArgs e)
        {
            var shipRow = CurrentRecord as OrderShipmentDataSet.ShipmentPackageRow;
            if (_recordLoading || shipRow == null)
            {
                return;
            }

            // Update data source
            shipRow.TrackingNumber = this.txtTrackingNumber.Text;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void txtCarrierNumber_ValueChanged(object sender, EventArgs e)
        {
            var shipRow = CurrentRecord as OrderShipmentDataSet.ShipmentPackageRow;

            if (_recordLoading || shipRow == null)
            {
                return;
            }

            // Update data source
            shipRow.CarrierCustomerNumber = txtCarrierNumber.Text;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void numPackageNumber_ValueChanged(object sender, EventArgs e)
        {
            var shipRow = CurrentRecord as OrderShipmentDataSet.ShipmentPackageRow;
            if (_recordLoading || shipRow == null)
            {
                return;
            }

            // Update data source
            shipRow.PackageNumber = numPackageNumber.Value as int? ?? 0;
            ValueChanged?.Invoke(this, EventArgs.Empty);
            UpdateMaximumNumberText();
        }

        private void numWeight_ValueChanged(object sender, EventArgs e)
        {
            var shipRow = CurrentRecord as OrderShipmentDataSet.ShipmentPackageRow;
            if (_recordLoading || shipRow == null)
            {
                return;
            }

            // Update data source
            //shipRow.GrossWeight = numPackageNumber.Value as int? ?? 0;
            var grossWeight = numWeight.Value as decimal?;

            if (grossWeight.HasValue)
            {
                shipRow.GrossWeight = grossWeight.Value;
            }
            else
            {
                shipRow.SetGrossWeightNull();
            }

            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void dteShipDate_ValueChanged(object sender, EventArgs e)
        {
            var shipRow = CurrentRecord as OrderShipmentDataSet.ShipmentPackageRow;

            if (_recordLoading || shipRow == null)
            {
                return;
            }

            // Update data source
            shipRow.CloseDate = dteShipDate.DateTime;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void grdOrders_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var band = grdOrders.DisplayLayout.Bands[0];
                var gridSettings = new UltraGridBandSettings();
                gridSettings.RetrieveSettingsFrom(band);
                _gridSettingsPersistence.SaveSettings(gridSettings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after showing/hiding/moving a column.");
            }
        }

        private void grdOrders_AfterSortChange(object sender, BandEventArgs e)
        {
            try
            {
                // Save settings
                var band = grdOrders.DisplayLayout.Bands[0];
                var gridSettings = new UltraGridBandSettings();
                gridSettings.RetrieveSettingsFrom(band);
                _gridSettingsPersistence.SaveSettings(gridSettings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after sorting a column.");
            }
        }

        #endregion
    }
}