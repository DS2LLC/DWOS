using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Reports;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Interop;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Order.Activity;
using DWOS.Shared.Utilities;
using DWOS.UI.Tools;

namespace DWOS.UI
{
    /// <summary>
    /// Helper for scanning order WIP barcodes.
    /// </summary>
    internal class OrderSummaryBarcodeScanner
    {
        #region Fields

        private readonly Main _mainForm;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the barcode scanner for this instance.
        /// </summary>
        public BarcodeScanner Scanner { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSummaryBarcodeScanner"/> class.
        /// </summary>
        /// <param name="mainForm"></param>
        public OrderSummaryBarcodeScanner(Main mainForm)
        {
            _mainForm = mainForm;
            _mainForm.SelectedTabChanged += _mainForm_SelectedTabChanged;

            Scanner = new BarcodeScanner(Report.BARCODE_ORDER_PROCESS_PREFIX, Report.BARCODE_ORDER_PROCESS_OLD_PREFIX);
            Scanner.BarcodingFinished += _scanner_BarcodingFinished;

            UpdateEnabledState();
        }

        /// <summary>
        /// Call when client settings change.
        /// </summary>
        public void OnSettingsChange()
        {
            Scanner.OnSettingsChange();
        }

        /// <summary>
        /// Shows <see cref="BarcodePrompt"/> to the user and reads its input.
        /// </summary>
        public void ShowScanPrompt()
        {
            var stoppedScanner = false;

            try
            {
                if (!(_mainForm.ActiveTab is OrderSummary2))
                {
                    return;
                }

                Scanner.Stop();
                stoppedScanner = true;

                var dialog = new BarcodePrompt();
                var helper = new WindowInteropHelper(dialog) { Owner = _mainForm.Handle };

                if (dialog.ShowDialog() ?? false)
                {
                    var barcodeText = dialog.BarcodeContent?.Trim().Trim(Scanner.Prefixes.ToArray());

                    if (int.TryParse(barcodeText, out int orderId))
                    {
                        OrderAction(orderId);
                    }
                    else
                    {
                        DWOSApp.MainForm.FlyoutManager.DisplayFlyout("Barcode Scan", "Unable to read barcode.", true);
                    }
                }

                GC.KeepAlive(helper);
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error scanning with prompt.");
            }
            finally
            {
                if (stoppedScanner)
                {
                    Scanner.Start();
                }
            }
        }

        private void UpdateEnabledState()
        {
            try
            {
                if (_mainForm.ActiveTab is IOrderSummary)
                    Scanner.Start();
                else
                    Scanner.Stop();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on main form tab changed.");
            }
        }

        private void OrderAction(int orderId)
        {
            string errorMsg;

            if (!CanProcessOrder(orderId, out errorMsg))
            {
                Sound.BeepError();
                DWOSApp.MainForm.FlyoutManager.DisplayFlyout("Order", errorMsg, true);
                return;
            }

            Sound.Beep();

            // Process -or- auto-complete
            var processingActivity = new ProcessingActivity(orderId,
                new ActivityUser(SecurityManager.Current.UserID, Properties.Settings.Default.CurrentDepartment,
                    Properties.Settings.Default.CurrentLine));

            processingActivity.Initialize();

            using (new MainRefreshHelper(_mainForm))
            {
                if (processingActivity.CanSkip())
                {
                    processingActivity.SkipActivity();
                    var title = "Order";
                    var msg = $"Automatically processed Order {processingActivity.OrderID}";

                    DWOSApp.MainForm.FlyoutManager.DisplayFlyout(title, msg);
                }
                else
                {
                    using (var op = new OrderProcessing2(processingActivity))
                    {
                        op.TopMost = true;
                        op.ShowDialog(Form.ActiveForm);
                    }
                }
            }
        }

        private bool CanProcessOrder(int orderId, out string errorMsg)
        {
            if (!SecurityManager.Current.IsInRole("OrderProcessing"))
            {
                errorMsg = "You do not have permission to process an order.";
                return false;
            }

            OrdersDataSet.OrderRow order;
            using (var taOrder = new OrderTableAdapter())
            {
                order = taOrder.GetByOrderID(orderId).FirstOrDefault();
                if (order == null)
                {
                    errorMsg = $"Cannot Find Order {orderId}.";
                    return false;
                }

                if (taOrder.GetInBatch(orderId) as bool? ?? false)
                {
                    errorMsg = $"Order {orderId} is in a batch.";
                    return false;
                }
            }

            if (order.WorkStatus != ApplicationSettings.Current.WorkStatusInProcess)
            {
                errorMsg = $"Order {orderId} is not ready for processing.";
                return false;
            }

            if (order.CurrentLocation != Properties.Settings.Default.CurrentDepartment)
            {
                errorMsg = $"Order {orderId} is outside of the current department.";
                return false;
            }

            if (ApplicationSettings.Current.MultipleLinesEnabled && !order.IsCurrentLineNull() && order.CurrentLine != Properties.Settings.Default.CurrentLine)
            {
                errorMsg = $"Order {orderId} is outside of the current line.";
                return false;
            }

            errorMsg = string.Empty;
            return true;
        }

        #endregion

        #region Events

        private void _scanner_BarcodingFinished(object sender, BarcodeScanner.BarcodeScannerEventArgs e)
        {
            try
            {
                int orderId;

                if (e.Output != null && int.TryParse(e.Output, out orderId))
                {
                    if (!_mainForm.CanFocus)
                    {
                        // A dialog box is already open.
                        Sound.BeepError();
                        DWOSApp.MainForm.FlyoutManager.DisplayFlyout("Order", "Cannot show Order Processing - another window is open.", true);
                        _logger.Info("Skipping barcode finished - already showing a popup.");
                        return;
                    }

                    _logger.Info("Successfully read barcode.");

                    if (Scanner.Prefixes.Contains(e.Postfix ?? (char)0))
                        OrderAction(orderId);
                }
                else if (!string.IsNullOrEmpty(e.Output))
                {
                    _logger.Info($"Invalid barcode input: {e.Output}");
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error during barcode finishing for order scanner.");

                try
                {
                    DWOSApp.MainForm.FlyoutManager.DisplayFlyout("Barcode Scan", "Error scanning barcode.", true);
                }
                catch (Exception flyoutExc)
                {
                    LogManager.GetCurrentClassLogger().Error(flyoutExc, "Error showing flyout w/ error message.");
                }
            }
        }

        private void _mainForm_SelectedTabChanged(object sender, EventArgs e)
        {
            UpdateEnabledState();
        }

        #endregion
    }
}
