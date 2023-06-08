using DWOS.Data;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsPricingInfo : UserControl, ISettingsPanel
    {
        public bool CanDock
        {
            get { return true; }
        }
        public SettingsPricingInfo()
        {
            InitializeComponent();

            cboPartPricing.Items.Add(new Infragistics.Win.ValueListItem(PricingType.Part, "Part"));
            cboPartPricing.Items.Add(new Infragistics.Win.ValueListItem(PricingType.Process, "Process"));
        }

        #region ISettingsPanel Members

        public bool Editable
        {
            get
            {
                return true;
            }
        }

        public string PanelKey
        {
            get
            {
                return "Pricing";
            }
        }

        public void LoadData()
        {
            var pricingType = ApplicationSettings.Current.PartPricingType;
            var pricingTypeItem = cboPartPricing.FindItemByValue<PricingType>((i) => i == pricingType);
            var enableVolumePricing = ApplicationSettings.Current.EnableVolumePricing;

            if (pricingTypeItem != null)
            {
                cboPartPricing.SelectedItem = pricingTypeItem;
            }

            chkVolumeDiscount.Checked = enableVolumePricing;

            chkProcessPriceWarning.Checked = ApplicationSettings.Current.ProcessPriceWarningEnabled;
            chkProcessPriceWarning.Enabled = pricingType == PricingType.Process;
        }

        public void SaveData()
        {
            var pricingType = (PricingType)(cboPartPricing.SelectedItem?.DataValue ?? PricingType.Part);
            var enableVolumePricing = chkVolumeDiscount.Checked;

            ApplicationSettings.Current.PartPricingType = pricingType;
            ApplicationSettings.Current.EnableVolumePricing = enableVolumePricing;
            ApplicationSettings.Current.ProcessPriceWarningEnabled = chkProcessPriceWarning.Checked;
        }

        #endregion

        #region Events

        private void cboPartPricing_ValueChanged(object sender, EventArgs e)
        {
            var pricingType = cboPartPricing.Value as PricingType?;
            chkProcessPriceWarning.Enabled = (pricingType ?? PricingType.Part) == PricingType.Process;
        }

        private void pnlProcessPriceWarning_MouseHover(object sender, EventArgs e)
        {
            try
            {
                // Show tooltip for disabled control
                if (chkProcessPriceWarning.Enabled)
                {
                    return;
                }

                toolTipManager.ShowToolTip(chkProcessPriceWarning);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error showing process price warning tooltip.");
            }
        }

        private void pnlProcessPriceWarning_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                // Hide tooltip for disabled control
                if (chkProcessPriceWarning.Enabled || !toolTipManager.IsToolTipVisible(chkProcessPriceWarning))
                {
                    return;
                }

                toolTipManager.HideToolTip();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error hiding process price warning tooltip.");
            }
        }

        #endregion
    }
}
