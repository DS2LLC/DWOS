using System;
using System.ComponentModel;
using System.Linq;
using DWOS.Data.Datasets;

namespace DWOS.UI.Sales.Order
{
    public partial class SerialNumberInformation : DataPanel
    {
        #region Fields

        private readonly BindingList<SerialNumberInfo> _serialNumberData;

        #endregion

        #region Properties

        public OrdersDataSet Dataset
        {
            get { return _dataset as OrdersDataSet; }
            set { _dataset = value; }
        }

        protected override string BindingSourcePrimaryKey => Dataset.Order.OrderIDColumn.ColumnName;

        #endregion

        #region Methods

        public SerialNumberInformation()
        {
            InitializeComponent();
            _serialNumberData = new BindingList<SerialNumberInfo>();
        }

        public void LoadData(OrdersDataSet dataset)
        {
            _dataset = dataset;
            bsData.DataSource = dataset;
            bsData.DataMember = dataset.Order.TableName;
            grdSerialNumbers.DataSource = _serialNumberData;
            _panelLoaded = true;
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            base.BeforeMoveToNewRecord(id);
            _serialNumberData.Clear();
        }
        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            var currentOrder = CurrentRecord as OrdersDataSet.OrderRow;

            if (currentOrder == null)
            {
                return;
            }

            var serialNumberRows = currentOrder.GetOrderSerialNumberRows().Where(s => s.IsValidState() && !s.Active);

            foreach (var serialNumberRow in serialNumberRows)
            {
                _serialNumberData.Add(new SerialNumberInfo(serialNumberRow));
            }
        }

        #endregion

        #region SerialNumberInfo

        private class SerialNumberInfo
        {
            public string Number => Row.IsNumberNull() ? string.Empty : Row.Number;

            public DateTime? DateRemoved => Row.IsDateRemovedNull() ? (DateTime?)null : Row.DateRemoved;

            OrdersDataSet.OrderSerialNumberRow Row
            {
                get;
            }

            public SerialNumberInfo(OrdersDataSet.OrderSerialNumberRow  row)
            {
                if (row == null)
                {
                    throw new ArgumentNullException(nameof(row));
                }

                Row = row;
            }
        }

        #endregion
    }
}
