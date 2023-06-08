using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using NLog;

namespace DWOS.UI.Sales.Order
{
    public partial class WorkDescriptionWidget : UserControl
    {
        #region Properties

        public OrdersDataSet Dataset { get; private set; }

        public OrdersDataSet.OrderRow CurrentOrder { get; private set; }

        public bool ReadOnly
        {
            get => cboWorkDescription.ReadOnly;
            set => cboWorkDescription.ReadOnly = value;
        }

        #endregion

        #region Methods

        public WorkDescriptionWidget()
        {
            InitializeComponent();
        }


        public void LoadData(OrdersDataSet dataset)
        {
            Dataset = dataset;

            cboWorkDescription.DataSource = new DataView(dataset.WorkDescription);
            cboWorkDescription.DisplayMember = dataset.WorkDescription.DescriptionColumn.ColumnName;
            cboWorkDescription.ValueMember = dataset.WorkDescription.WorkDescriptionIDColumn.ColumnName;
        }

        public void LoadRow(OrdersDataSet.OrderRow order)
        {
            CurrentOrder = order;

            if (CurrentOrder == null)
            {
                cboWorkDescription.Clear();
            }
            else
            {
                cboWorkDescription.Value = CurrentOrder
                    .GetOrderWorkDescriptionRows()
                    .FirstOrDefault()
                    ?.WorkDescriptionID;
            }
        }

        public void SaveRow()
        {
            if (CurrentOrder == null || !CurrentOrder.IsValidState() || Dataset == null)
            {
                return;
            }

            var currentDescriptionRow = CurrentOrder
                .GetOrderWorkDescriptionRows()
                .FirstOrDefault();

            var selectedDescriptionId = cboWorkDescription.Value as int?;

            if (!selectedDescriptionId.HasValue)
            {
                currentDescriptionRow?.Delete();
            }
            else if (currentDescriptionRow != null)
            {
                if (currentDescriptionRow.WorkDescriptionID != selectedDescriptionId)
                {
                    currentDescriptionRow.WorkDescriptionID = selectedDescriptionId.Value;
                }
            }
            else
            {
                var newDescriptionRow = Dataset.OrderWorkDescription.NewOrderWorkDescriptionRow();
                newDescriptionRow.OrderRow = CurrentOrder;
                newDescriptionRow.WorkDescriptionID = selectedDescriptionId.Value;
                Dataset.OrderWorkDescription.AddOrderWorkDescriptionRow(newDescriptionRow);
            }
        }

        private void OnDispose()
        {
            Dataset = null;
        }

        #endregion

        #region Events

        private void cboWorkDescription_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                if (ReadOnly || e.Button.Key != "Delete")
                {
                    return;
                }

                cboWorkDescription.Value = null;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error clicking editor button.");
            }
        }

        #endregion
    }
}
