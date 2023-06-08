using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Shared;
using Infragistics.Win;
using NLog;

namespace DWOS.UI.Utilities
{
    public static class PrinterUtilities
    {
        #region Properties

        public static IEnumerable<string> GetInstalledPrinterNames() =>
            PrinterSettings.InstalledPrinters.OfType<string>();

        #endregion

        #region Methods

        /// <summary>
        /// Gets the default printer name on the current computer.
        /// </summary>
        /// <returns> </returns>
        public static string GetDefaultPrinterName()
        {
            try
            {
                var ps = new PrinterSettings();
                return ps.PrinterName;
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting default printer name.");
                return null;
            }
        }

        /// <summary>
        /// Allows the user to select the printer to use via a dialog.
        /// </summary>
        /// <returns> The name of the printer selected by the user. </returns>
        public static string SelectPrinterNameDialog(PrinterType type)
        {
            // Current form may be unavailable due to debugging/threading issue
            var activeForm = Form.ActiveForm == null || Form.ActiveForm.IsDisposed
                ? DWOSApp.MainForm
                : Form.ActiveForm;

            if (activeForm == null)
            {
                LogManager.GetCurrentClassLogger().Warn("Unable to show print dialog.");
                return string.Empty;
            }
            else if (activeForm.InvokeRequired)
            {
                return (string)activeForm.Invoke(
                    (Func<Form, PrinterType, string>)DoSelectPrinterNameDialog,
                    activeForm,
                    type);
            }
            else
            {
                return DoSelectPrinterNameDialog(activeForm, type);
            }
        }

        private static string DoSelectPrinterNameDialog(Form form, PrinterType type)
        {
            ComboBoxForm cbo = null;

            try
            {
                cbo = new ComboBoxForm
                {
                    Text = $"Select {type} Printer"
                };

                cbo.FormLabel.Text = "Printer:";
                cbo.ComboBox.DataSource = GetInstalledPrinterNames().ToList();
                cbo.ComboBox.NullText = "<None>";
                cbo.ComboBox.DropDownStyle = DropDownStyle.DropDownList;
                cbo.chkOption.Visible = true;
                cbo.chkOption.Text = "Is Default";

                //Find default printer and select it
                var defaultPrinterName = GetDefaultPrinterName();

                if (!string.IsNullOrWhiteSpace(defaultPrinterName))
                {
                    var printerItem = cbo.ComboBox.FindItemByValue<string>(pn => pn == defaultPrinterName);
                    if (printerItem != null)
                    {
                        cbo.ComboBox.SelectedItem = printerItem;
                    }
                }

                var isPrinterSelected = cbo.ShowDialog(form) == DialogResult.OK
                    && cbo.ComboBox.SelectedItem != null;

                if (isPrinterSelected)
                {
                    var printerName = cbo.ComboBox.Text;

                    if (cbo.chkOption.Checked)
                    {
                        var userSettings = UserSettings.Default;
                        switch (type)
                        {
                            case PrinterType.Paper:
                                userSettings.DefaultPrinterName = printerName;
                                break;
                            case PrinterType.Label:
                                userSettings.ShippingLabelPrinterName = printerName;
                                break;
                            default:
                                LogManager.GetCurrentClassLogger()
                                    .Error($"Printer type {type} not supported");
                                break;
                        }

                        userSettings.Save();
                    }

                    return printerName;
                }
                else
                {
                    string errorMsg;
                    switch (type)
                    {
                        case PrinterType.Label:
                            errorMsg = "No label printer selected to print to.";
                            break;
                        default:
                            errorMsg = "No default printer on this computer.";
                            break;
                    }

                    MessageBoxUtilities.ShowMessageBoxWarn(errorMsg, "No Printer");
                }

                return null;
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error while selecting printer.", exc);
                return null;
            }
            finally
            {
                cbo?.Dispose();
            }
        }

        #endregion
    }
}