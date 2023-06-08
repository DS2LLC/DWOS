using System;
using System.Windows.Forms;
using DWOS.Shared;
using DWOS.UI.Admin;
using DWOS.UI.Utilities;
using DWOS.Data;
using Infragistics.Win;
using System.Linq;
using Infragistics.Win.UltraWinEditors;
using NLog;

namespace DWOS.UI.ShippingRec.ShippingManagerPanels
{
    public partial class ShippingSettings : UserControl, ISettingsPanel
    {
        #region Fields

        private const int DPI_MIN = 10;
        private const int DPI_DEFAULT = 203;
        private bool _settingsLoaded;

        #endregion

        #region Methods
        public bool CanDock
        {
            get { return false; }
        }
        public ShippingSettings()
        {
            InitializeComponent();
        }

        public void LoadSettings()
        {
            try
            {
                if (_settingsLoaded)
                {
                    return;
                }

                Enabled = Editable;
                var printers = PrinterUtilities.GetInstalledPrinterNames()
                    .ToList();

                cboLabelPrinter.DataSource = printers;

                if(!string.IsNullOrWhiteSpace(UserSettings.Default.ShippingLabelPrinterName))
                {
                    ValueListItem printerItem = cboLabelPrinter.FindItemByValue <string>(pn => pn == UserSettings.Default.ShippingLabelPrinterName);
                    if(printerItem != null)
                    {
                        cboLabelPrinter.SelectedItem = printerItem;
                    }
                }

                cboPrinter.DataSource = printers;

                if (!string.IsNullOrWhiteSpace(UserSettings.Default.DefaultPrinterName))
                {
                    ValueListItem printerItem = cboPrinter.FindItemByValue<string>(pn => pn == UserSettings.Default.DefaultPrinterName);
                    if (printerItem != null)
                    {
                        cboPrinter.SelectedItem = printerItem;
                    }
                }

                cboLabelPrinterLanguage.DataSource = Enum.GetNames(typeof(Neodynamic.SDK.Printing.ProgrammingLanguage));

                ValueListItem languageItem;
                if (string.IsNullOrWhiteSpace(UserSettings.Default.ShippingLabelPrinterLanguage))
                {
                    languageItem = cboLabelPrinterLanguage.FindItemByValue<string>(pn => pn == "ZPL");
                }
                else
                {
                    languageItem = cboLabelPrinterLanguage.FindItemByValue<string>(pn => pn == UserSettings.Default.ShippingLabelPrinterLanguage);
                }

                cboLabelPrinterLanguage.SelectedItem = languageItem
                    ?? cboLabelPrinterLanguage.FindItemByValue<string>(pn => pn == "ZPL");

                if (UserSettings.Default.LabelPrinterDpi >= DPI_MIN)
                {
                    numLabelPrinterDpi.Value = UserSettings.Default.LabelPrinterDpi;
                }
                else
                {
                    numLabelPrinterDpi.Value = DPI_MIN;
                }

                chkPrintOrderLabel.Checked = UserSettings.Default.Shipping.PrintOrderLabel;
                chkPrintPackageLabel.Checked = UserSettings.Default.Shipping.PrintPackageLabel;
                chkPrintShippingManifest.Checked = UserSettings.Default.Shipping.PrintPackingList;
                chkPrintCOC.Checked = UserSettings.Default.Shipping.PrintCoc;
                chkQuickPrint.Checked = UserSettings.Default.Shipping.QuickPrint;
                dteRolloverTime.Value = Properties.Settings.Default.ShippingRolloverTime;

                numPackingSlipCount.Enabled = UserSettings.Default.Shipping.PrintPackingList;
                numPackingSlipCount.Value = UserSettings.Default.Shipping.PackingListCount;

                numBillOfLadingCopies.Value = UserSettings.Default.Shipping.BillOfLadingCount;

                _settingsLoaded = true;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error loading settings.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        public void SaveSettings()
        {
            try
            {
                if(!_settingsLoaded)
                {
                    return;
                }

                UserSettings.Default.DefaultPrinterName = cboPrinter.Value?.ToString();
                UserSettings.Default.ShippingLabelPrinterName = cboLabelPrinter.Value?.ToString();

                if (cboLabelPrinterLanguage.SelectedItem != null)
                {
                    UserSettings.Default.ShippingLabelPrinterLanguage = cboLabelPrinterLanguage.SelectedItem.DisplayText;
                }

                UserSettings.Default.LabelPrinterDpi = numLabelPrinterDpi.Value as int? ?? DPI_DEFAULT;
                UserSettings.Default.Shipping.PrintOrderLabel = chkPrintOrderLabel.Checked;
                UserSettings.Default.Shipping.PrintPackageLabel = chkPrintPackageLabel.Checked;
                UserSettings.Default.Shipping.PrintPackingList = chkPrintShippingManifest.Checked;
                UserSettings.Default.Shipping.PrintCoc = chkPrintCOC.Checked;
                UserSettings.Default.Shipping.QuickPrint = chkQuickPrint.Checked;
                Properties.Settings.Default.ShippingRolloverTime = dteRolloverTime.DateTime;
                UserSettings.Default.Shipping.PackingListCount = Convert.ToInt32(numPackingSlipCount.Value);
                UserSettings.Default.Shipping.BillOfLadingCount = Convert.ToInt32(numBillOfLadingCopies.Value);

                Properties.Settings.Default.Save();
                UserSettings.Default.Save();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving settings.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion


        #region Events

        private void chkPrintShippingManifest_CheckedChanged(object sender, EventArgs e)
        {
            numPackingSlipCount.Enabled = chkPrintShippingManifest.Checked;
        }

        private void cboPrinter_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                if (e.Button.Key == "Delete")
                {
                    cboPrinter.Value = null;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error clicking editor button.");
            }
        }

        private void cboLabelPrinter_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                if (e.Button.Key == "Delete")
                {
                    cboLabelPrinter.Value = null;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error clicking editor button.");
            }
        }

        #endregion

        #region ISettingsPanel Members

        public string PanelKey => "UserShipping";

        public bool Editable =>
            SecurityManager.Current.IsValidUser;

        public void LoadData()
        {
            LoadSettings();
        }

        public void SaveData()
        {
            SaveSettings();
        }

        #endregion
    }
}