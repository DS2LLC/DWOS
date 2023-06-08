using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Utilities.Validation;

namespace DWOS.UI.Sales.Order
{
    public partial class BasicSerialNumberWidget : UserControl, ISerialNumberWidget
    {
        #region Properties

        public OrdersDataSet.OrderRow CurrentOrder { get; private set; }

        public OrdersDataSet Dataset { get; set; }

        #endregion

        #region Methods

        public BasicSerialNumberWidget()
        {
            InitializeComponent();
        }

        #endregion

        #region ISerialNumberWidget Members

        public bool ReadOnly
        {
            get { return txtSerialNumber.ReadOnly; }

            set { txtSerialNumber.ReadOnly = value; }
        }

        public void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(
                new ImageDisplayValidator(
                    new TextControlValidator(txtSerialNumber, "Serial Number is required")
                    {
                        ValidationRequired = () => CurrentOrder?.RowState == DataRowState.Added
                    }, errProvider));
        }

        public void LoadOrder(int orderId)
        {
            LoadOrder(Dataset.Order.FindByOrderID(orderId));
        }

        public void LoadOrder(OrdersDataSet.OrderRow row)
        {
            CurrentOrder = row;

            if (CurrentOrder == null)
            {
                txtSerialNumber.Clear();
            }
            else
            {
                var serialNumber = CurrentOrder.GetOrderSerialNumberRows().FirstOrDefault(s => s.Active);

                if (serialNumber == null || serialNumber.IsNumberNull())
                {
                    txtSerialNumber.Clear();
                }
                else
                {
                    txtSerialNumber.Text = serialNumber.Number;
                }
            }
        }

        public void SaveRow()
        {
            if (CurrentOrder == null || !CurrentOrder.IsValidState() || Dataset == null)
            {
                return;
            }

            var currentSerialNumber = CurrentOrder.GetOrderSerialNumberRows().FirstOrDefault(s => s.Active);

            var number = txtSerialNumber.Text;

            if (string.IsNullOrEmpty(number))
            {
                if (currentSerialNumber != null && currentSerialNumber.RowState != DataRowState.Added)
                {
                    var dateRemoved = DateTime.Now;
                    foreach (var activeNumber in CurrentOrder.GetOrderSerialNumberRows().Where(s => s.Active))
                    {
                        activeNumber.Active = false;
                        activeNumber.DateRemoved = dateRemoved;
                    }
                }
                else
                {
                    currentSerialNumber?.Delete();
                }
            }
            else if (currentSerialNumber != null)
            {
                if (currentSerialNumber.IsNumberNull() || currentSerialNumber.Number != number)
                {
                    currentSerialNumber.Number = number;
                }
            }
            else
            {
                var newNumberRow = Dataset.OrderSerialNumber.NewOrderSerialNumberRow();
                newNumberRow.OrderRow = CurrentOrder;
                newNumberRow.PartOrder = 1;
                newNumberRow.Number = number;
                newNumberRow.Active = true;

                Dataset.OrderSerialNumber.AddOrderSerialNumberRow(newNumberRow);
            }
        }

        public void LoadData(OrdersDataSet dataset)
        {
            Dataset = dataset;
        }

        #endregion
    }
}