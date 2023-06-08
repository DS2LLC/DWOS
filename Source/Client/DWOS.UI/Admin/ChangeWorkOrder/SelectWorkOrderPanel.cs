using System;
using System.Linq;
using System.Windows.Forms;
using NLog;
using DWOS.UI.Utilities;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;

namespace DWOS.UI.Admin.ChangeWorkOrder
{
     /// <summary>
    /// Panel for <see cref="ChangeWorkOrderMain"/> - allows user to select a
    /// locked Work Order.
    /// </summary>
    public partial class SelectWorkOrderPanel : UserControl, IChangeWorkOrderPanel
    {
        #region Constants

        private const string STATUS_CLOSED = "Closed";

        #endregion

        #region Fields

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        public SelectWorkOrderPanel()
        {
            InitializeComponent();
        }

        private bool IsWorkOrderLocked(int orderId)
        {
            using (var taOrder = new OrderTableAdapter())
            {
                using (var dtOrder = taOrder.GetByOrderID(orderId))
                {
                    var order = dtOrder.FirstOrDefault();

                    if (order == null)
                    {
                        return false;
                    }

                    if (!order.IsStatusNull() && order.Status == STATUS_CLOSED)
                    {
                        return true;
                    }
                }

                return (taOrder.GetAnswerCount(orderId) ?? 0) != 0;
            }
        }

        private void CheckWorkOrder()
        {
            try
            {
                CanNavigateToNextPanel = false;
                numWorkOrder.Enabled = false;
                btnCheckWorkOrder.Enabled = false;
                OnCanNavigateToNextPanelChange?.Invoke(this, CanNavigateToNextPanel);

                var orderId = numWorkOrder.Value as int? ?? 0;

                if (orderId < 1)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(
                        "Please enter a valid Work Order number.",
                        MainForm.Text);

                    return;
                }

                _logger.Info($"Checking WO #{orderId} to see if it is 'locked'.");

                if (IsWorkOrderLocked(orderId))
                {
                    lblLocked.Visible = true;
                    CanNavigateToNextPanel = true;
                    OnCanNavigateToNextPanelChange?.Invoke(this, CanNavigateToNextPanel);
                }
                else
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(
                        $"WO #{orderId} is not locked by DWOS - please change its part through Order Entry.",
                        MainForm.Text);
                }
            }
            finally
            {
                if (!CanNavigateToNextPanel)
                {
                    numWorkOrder.Enabled = true;
                    btnCheckWorkOrder.Enabled = true;
                }
            }
        }

        #endregion

        #region Events

        private void btnCheckWorkOrder_Click(object sender, EventArgs e)
        {
            try
            {
                CheckWorkOrder();
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error checking Work Order.");
            }
        }

        private void numWorkOrder_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CheckWorkOrder();
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error checking Work Order.");
            }
        }

        #endregion

        #region IChangeWorkOrderPanel Implementation

        public ChangeWorkOrderMain MainForm { get; set; }

        public Action<IChangeWorkOrderPanel, bool> OnCanNavigateToNextPanelChange { get; set; }

        public bool CanNavigateToNextPanel { get; private set; }

        public void Finish()
        {
            // Do nothing
        }

        public void OnNavigateFrom()
        {
            MainForm.SelectedOrderId = numWorkOrder.Value as int?;
        }

        public void OnNavigateTo()
        {
            CanNavigateToNextPanel = false;
            numWorkOrder.Value = MainForm.SelectedOrderId;
            numWorkOrder.Enabled = true;
            btnCheckWorkOrder.Enabled = true;
            lblLocked.Visible = false;
        }

        #endregion
    }
}
