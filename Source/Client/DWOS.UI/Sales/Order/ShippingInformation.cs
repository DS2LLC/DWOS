using System;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using System.Linq;
using System.Windows.Interop;
using DWOS.UI.Sales.Order;
using DWOS.UI.Utilities;

namespace DWOS.UI.Sales
{
    public partial class ShippingInformation : DataPanel
    {
        #region Fields

        private OrderShipmentTableAdapter _taOrderShipment;

        #endregion

        #region Properties

        public OrdersDataSet Dataset
        {
            get { return base._dataset as OrdersDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.OrderShipment.ShipmentIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public ShippingInformation() { InitializeComponent(); }

        public void LoadData(OrdersDataSet dataset, OrderShipmentTableAdapter taOrderShipment)
        {
            Dataset = dataset;
            _taOrderShipment = taOrderShipment;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.OrderShipment.TableName;

            base.BindValue(this.txtShipment, Dataset.OrderShipment.ShipmentPackageIDColumn.ColumnName);
            base.BindValue(this.dteDateCertified, Dataset.OrderShipment.DateShippedColumn.ColumnName);
            base.BindValue(this.cboUser, Dataset.OrderShipment.ShippingUserIDColumn.ColumnName);
            base.BindValue(this.cboCourier, Dataset.OrderShipment.ShippingCarrierIDColumn.ColumnName);
            base.BindValue(this.txtTrackingNumber, Dataset.OrderShipment.TrackingNumberColumn.ColumnName);
            base.BindValue(this.numPartQty, Dataset.OrderShipment.PartQuantityColumn.ColumnName);

            base.BindList(this.cboUser, Dataset.UserSummary, Dataset.UserSummary.UserIDColumn.ColumnName, Dataset.UserSummary.NameColumn.ColumnName);
            base.BindList(this.cboCourier, Dataset.d_ShippingCarrier, Dataset.d_ShippingCarrier.CarrierIDColumn.ColumnName, Dataset.d_ShippingCarrier.CarrierIDColumn.ColumnName);

            numGrossWeight.MaskInput =
                $"nnnnnnnn.{string.Concat(Enumerable.Repeat("n", ApplicationSettings.Current.WeightDecimalPlaces))} lbs";

            base._panelLoaded = true;
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            base.BeforeMoveToNewRecord(id);

            numGrossWeight.Value = null;
            txtPackageType.Text = string.Empty;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            var currentOrderShipment = CurrentRecord as OrdersDataSet.OrderShipmentRow;

            if (currentOrderShipment == null)
            {
                return;
            }

            var weight = _taOrderShipment.GetGrossWeightOverride(currentOrderShipment.ShipmentPackageID);

            if (weight.HasValue)
            {
                numGrossWeight.Value = weight;
            }

            var packageType = _taOrderShipment.GetPackageTypeName(currentOrderShipment.ShipmentPackageID);

            if (!string.IsNullOrEmpty(packageType))
            {
                txtPackageType.Text = packageType;
            }

            btnResend.Enabled = !(currentOrderShipment.ShipmentPackageRow?.IsNotificationSentNull() ?? true);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            _taOrderShipment = null;
        }

        #endregion

        #region Events

        private void btnResend_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(CurrentRecord is OrdersDataSet.OrderShipmentRow currentOrderShipment) || currentOrderShipment.ShipmentPackageRow == null)
                {
                    return;
                }

                var resendDialog = new ResendShippingNotificationDialog();
                resendDialog.Load(currentOrderShipment.ShipmentPackageRow);
                var helper = new WindowInteropHelper(resendDialog) { Owner = DWOSApp.MainForm.Handle };

                if (resendDialog.ShowDialog() ?? false)
                {
                    MessageBoxUtilities.ShowMessageBoxOK(
                        "DWOS will resend shipping notification emails for this package after you click OK or Apply to save your changes.",
                        "Resend Notification");

                    currentOrderShipment.ShipmentPackageRow.NotificationEmails = string.Join(",", resendDialog.EmailAddresses);
                    currentOrderShipment.ShipmentPackageRow.SetNotificationSentNull();
                }


                GC.KeepAlive(helper);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error while resetting notification data for shipment.");
            }
        }

        #endregion
    }
}