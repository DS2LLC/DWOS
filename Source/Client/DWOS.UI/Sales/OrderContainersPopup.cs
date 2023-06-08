using DWOS.Data.Datasets;
using NLog;
using System;
using System.Windows.Forms;

namespace DWOS.UI.Sales
{
    public partial class OrderContainersPopup : Form
    {
        #region Properties

        public bool IsQuickView
        {
            get => !orderContainerWidget.Enabled;
            set => orderContainerWidget.Enabled = !value;
        }

        #endregion

        #region Methods

        public OrderContainersPopup()
        {
            InitializeComponent();
        }

        public void LoadOrder(OrdersDataSet dsOrders, OrdersDataSet.OrderRow order)
        {

            if (dsOrders == null)
            {
                throw new ArgumentNullException(nameof(dsOrders));
            }

            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            orderContainerWidget.LoadData(dsOrders);
            orderContainerWidget.LoadOrder(order);
        }

        #endregion

        #region Events

        private void OrderContainersPopup_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                orderContainerWidget.EndEditing();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error closing order containers popup.");
            }
        }

        #endregion
    }
}
