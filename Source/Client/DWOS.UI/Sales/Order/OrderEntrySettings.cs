using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DWOS.UI.Utilities;
using DWOS.Shared;
using DWOS.Data;
using DWOS.Utilities.Validation;
using NLog;

namespace DWOS.UI.Sales.Order
{
    public partial class OrderEntrySettings : UserControl
    {
        #region Fields

        private const int MIN_ORDER_COUNT = 200;
        private bool _settingsLoaded;
        private ValidatorManager _manager;

        #endregion

        #region Methods

        public OrderEntrySettings()
        {
            InitializeComponent();
            _manager = new ValidatorManager();
        }

        public void LoadSettings()
        {
            try
            {
                Enabled = SecurityManager.Current.IsValidUser;

                numMaxClosedOrders.Value = UserSettings.Default.MaxClosedOrders;

                if (!_settingsLoaded)
                {
                    var closedOrdersValidator = new NumericControlValidator(numMaxClosedOrders)
                    {
                        MinimumValue = MIN_ORDER_COUNT
                    };

                    _manager.Add(new ImageDisplayValidator(closedOrdersValidator, errorProvider));

                    _settingsLoaded = true;
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error loading settings.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        public void SaveSettings()
        {
            try
            {
                if (!_settingsLoaded)
                {
                    return;
                }

                var orderCount = Convert.ToInt32(numMaxClosedOrders.Value);

                if (_manager.ValidateControls())
                {
                    UserSettings.Default.MaxClosedOrders = orderCount;
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error saving settings.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void OnDisposing()
        {
            _manager?.Dispose();
        }

        #endregion

        #region Events

        private void numMaxClosedOrders_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                _manager.ValidateControls();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing closed orders value.");
            }
        }

        #endregion
    }
}
