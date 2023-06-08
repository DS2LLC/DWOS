using DWOS.Reports;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Linq;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Shared.Utilities;
using System.Windows.Interop;

namespace DWOS.UI
{
    /// <summary>
    /// Helper for scanning batch WIP barcodes.
    /// </summary>
    internal class BatchSummaryBarcodeScanner
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
        /// Initializes a new instance of the <see cref="BatchSummaryBarcodeScanner"/> class.
        /// </summary>
        /// <param name="mainForm"></param>
        public BatchSummaryBarcodeScanner(Main mainForm)
        {
            _mainForm = mainForm;
            _mainForm.SelectedTabChanged += _mainForm_SelectedTabChanged;
            
            Scanner = new BarcodeScanner(Report.BARCODE_BATCH_ACTION_PREFFIX);
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
                if (!(_mainForm.ActiveTab is BatchSummary))
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

                    if (int.TryParse(barcodeText, out int batchId))
                    {
                        BatchAction(batchId);
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
                if (_mainForm.ActiveTab is BatchSummary)
                    Scanner.Start();
                else
                    Scanner.Stop();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on main form tab changed.");
            }
        }
       
        private void BatchAction(int batchId)
        {
            string errorMsg;

            if (!CanProcessBatch(batchId, out errorMsg))
            {
                Sound.BeepError();
                DWOSApp.MainForm.FlyoutManager.DisplayFlyout("Batch", errorMsg, true);
                return;
            }

            Sound.Beep();

            var batchInfo = BatchProcessInfo.NewBatchProcessInfo(batchId);
            batchInfo.TopMost = true;

            var validationResults = batchInfo.Validate();
            if (!validationResults.IsValid)
            {
                MessageBoxUtilities.ShowMessageBoxWarn(validationResults.ErrorMessage,
                    validationResults.ErrorHeader,
                    validationResults.ErrorFooter);

                return;
            }

            using (new MainRefreshHelper(_mainForm))
            {
                batchInfo.AnswerQuestions();

                if (batchInfo.ProcessingResults == null)
                    return;

                batchInfo.CopyAnswers();

                //save any changes thus far, because UpdateCost and ProcessingActivity query the DB for answers
                batchInfo.SaveChanges();

                if (batchInfo.IsBatchProcessComplete)
                {
                    batchInfo.CompleteProcessing();
                }
            }
        }

        private bool CanProcessBatch(int batchId, out string errorMsg)
        {
            if (!SecurityManager.Current.IsInRole("BatchOrderProcessing"))
            {
                errorMsg = "You do not have permission to process an order.";
                return false;
            }

            OrderProcessingDataSet.BatchRow batch;
            using (var taBatch = new BatchTableAdapter())
            {
                batch = taBatch.GetDataBy(batchId).FirstOrDefault();

                if (batch == null)
                {
                    errorMsg = $"Cannot Find Batch {batchId}.";
                    return false;
                }
            }

            if (batch.WorkStatus != ApplicationSettings.Current.WorkStatusInProcess)
            {
                errorMsg = $"Batch {batchId} is not ready for processing.";
                return false;
            }

            if (batch.CurrentLocation != Properties.Settings.Default.CurrentDepartment)
            {
                errorMsg = $"Batch {batchId} is outside of the current department.";
                return false;
            }

            if (ApplicationSettings.Current.MultipleLinesEnabled && !batch.IsCurrentLineNull() && batch.CurrentLine != Properties.Settings.Default.CurrentLine)
            {
                errorMsg = $"Batch {batchId} is outside of the current line.";
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
                int batchId;

                if (e.Output != null && int.TryParse(e.Output, out batchId))
                {
                    if (!_mainForm.CanFocus)
                    {
                        // A dialog box is already open.
                        Sound.BeepError();
                        DWOSApp.MainForm.FlyoutManager.DisplayFlyout("Batch", "Cannot show Batch Processing - another window is open.", true);
                        _logger.Info("Skipping barcode finished - already showing a popup.");
                        return;
                    }

                    _logger.Info("Successfully read barcode.");

                    if (e.Postfix == Report.BARCODE_BATCH_ACTION_PREFFIX)
                        BatchAction(batchId);
                }
                else if (!string.IsNullOrEmpty(e.Output))
                {
                    _logger.Info($"Invalid barcode input: {e.Output}");
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error during barcode finishing for batch scanner.");

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

        private void _mainForm_SelectedTabChanged(object sender, EventArgs e) { UpdateEnabledState(); }

        #endregion
    }
}
