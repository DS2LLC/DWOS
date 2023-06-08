using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsGeneralInfo: UserControl, ISettingsPanel
    {
        #region Fields

        private const string IMAGE_FORMAT_FILTER = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public enum enumPricingType { Simple, ByProcess }

        private bool _accreditationLogoChanged;
        private bool _placeholderImageChanged;
        private readonly DisplayDisabledTooltips _displayDisabledTooltips;

        #endregion

        #region Properties

        public string PanelKey
        {
            get { return "General"; }
        }

        #endregion

        #region Methods
        public bool CanDock
        {
            get { return false; }
        }
        public SettingsGeneralInfo()
        {
            this.InitializeComponent();

            _displayDisabledTooltips = new DisplayDisabledTooltips(ultraGroupBox1, ultraToolTipManager1);

            cboScheduleType.Items.Add(new Infragistics.Win.ValueListItem(SchedulerType.ProductionCapacity, "Production Capacity"));
            cboScheduleType.Items.Add(new Infragistics.Win.ValueListItem(SchedulerType.ProcessLeadTime, "Process Lead Time (Days)"));
            cboScheduleType.Items.Add(new Infragistics.Win.ValueListItem(SchedulerType.ProcessLeadTimeHour, "Process Lead Time (Hours)"));
            cboScheduleType.Items.Add(new Infragistics.Win.ValueListItem(SchedulerType.Manual, "Manual (Per Department)"));
            cboScheduleType.Items.Add(new Infragistics.Win.ValueListItem(SchedulerType.ManualAllDepartments, "Manual (All Departments)"));

            cboProcessConfirm.Items.Add(new Infragistics.Win.ValueListItem(TravelerProcessConfirmationType.None, "None"));
            cboProcessConfirm.Items.Add(new Infragistics.Win.ValueListItem(TravelerProcessConfirmationType.QtyDateBy, "Qty/Date/By"));
            cboProcessConfirm.Items.Add(new Infragistics.Win.ValueListItem(TravelerProcessConfirmationType.CompletedCheckbox, "Checkbox"));
            cboProcessConfirm.Items.Add(new Infragistics.Win.ValueListItem(TravelerProcessConfirmationType.TimeInTimeOut, "Time In/Out"));
        }

        #endregion

        #region ISettingsPanel Members

        public bool Editable
        {
            get { return SecurityManager.Current.IsInRole("ApplicationSettings.Edit"); }
        }

        public void LoadData()
        {
            try
            {
                Enabled = this.Editable;
                var appSettings = ApplicationSettings.Current;

                chkSchedulingEnabled.Checked = appSettings.SchedulingEnabled;
                chkLoadCapacity.Checked = appSettings.UseLoadCapacity;
                chkUsePriceUnitQuantities.Checked = appSettings.UsePriceUnitQuantities;

                var schedulerItem = cboScheduleType.FindItemByValue<SchedulerType>(i => i == appSettings.SchedulerType);
                if (schedulerItem == null)
                {
                    cboScheduleType.SelectedIndex = 0;
                }
                else
                {
                    cboScheduleType.SelectedItem = schedulerItem;
                }

                var processConfirmItem = cboProcessConfirm.FindItemByValue<TravelerProcessConfirmationType>(i => i == appSettings.TravelerProcessConfirmation);

                if (processConfirmItem == null)
                {
                    cboProcessConfirm.SelectedIndex = 1;
                }
                else
                {
                    cboProcessConfirm.SelectedItem = processConfirmItem;
                }

                chkAllowOrderReviewOwnOrder.Checked = appSettings.AllowReviewYourOwnOrders;
                numOrderLeadTime.Value = appSettings.OrderLeadTime;
                numQuoteExpirtation.Value = appSettings.QuoteExpirationDays;
                numMinPrice.Value = appSettings.MinimumOrderPrice;
                numProcessMinPrice.Value = appSettings.MinimumProcessPrice;
                cboPrinterType.SelectedIndex = (int)appSettings.DefaultPrinterType;
                // cboScheduleType value change sets value for dteScheduleDate
                chkScheduleReset.Checked = appSettings.ScheduleResetEnabled;
                numWeightDecimals.Value = appSettings.WeightDecimalPlaces;
                numProcessDecimals.Value = appSettings.ProcessingDecimalPlaces;
                chkAdditionalCustomers.Checked = appSettings.AllowAdditionalCustomersForContacts;
                chkSaveOrderPrintHistory.Checked = appSettings.SaveWorkOrderPrintHistory;
                chkIncludeHoldsWithLateOrders.Checked = appSettings.IncludeHoldsInLateOrders;
                chkUsePlaceholder.Checked = appSettings.UseReportPlaceholderImage;
                chkCompanyLogoPackingSlip.Checked = appSettings.ShowCompanyLogoOnPackingSlip;
                txtOrderFormat.Text = appSettings.OrderItemFormat;
                txtOrderTokens.Text = "%ID%, %REQUIREDDATE%, %CUSTOMERNAME%";
                chkPrintRejoinedOrders.Checked = appSettings.PrintSummariesForRejoinedOrders;
                txtDefaultFees.Text = appSettings.DefaultFees;
                chkApplyDefaultFees.Checked = appSettings.ApplyDefaultFeesEnabled;  

                var timeValue = appSettings.SchedulerType == SchedulerType.Manual || appSettings.SchedulerType == SchedulerType.ManualAllDepartments
                    ? appSettings.ScheduleResetTime
                    : appSettings.ReceivingRolloverTime;

                dteScheduleDate.DateTime = DateTime.Now.StartOfDay().Add(timeValue);

                var accreditationLogoPath = appSettings.AccreditationLogoImagePath;

                if (accreditationLogoPath != null && System.IO.File.Exists(accreditationLogoPath))
                    this.picAccredidationLogo.Image = Bitmap.FromFile(accreditationLogoPath);

                var placeholderPath = appSettings.ReportPlaceholderImagePath;

                if (placeholderPath != null && System.IO.File.Exists(placeholderPath))
                    this.picPlaceholder.Image = Bitmap.FromFile(placeholderPath);
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error loading settings.";
                _log.Error(exc, errorMsg);
            }
        }

        public void SaveData()
        {
            var schedulerType = (cboScheduleType.Value as SchedulerType?) ?? SchedulerType.ProcessLeadTime;

            ApplicationSettings.Current.SchedulingEnabled = this.chkSchedulingEnabled.Checked;
            ApplicationSettings.Current.UseLoadCapacity = this.chkLoadCapacity.Checked;
            ApplicationSettings.Current.UsePriceUnitQuantities = this.chkUsePriceUnitQuantities.Checked;
            ApplicationSettings.Current.SchedulerType = schedulerType;
            ApplicationSettings.Current.AllowReviewYourOwnOrders = this.chkAllowOrderReviewOwnOrder.Checked;
            ApplicationSettings.Current.OrderLeadTime = Convert.ToInt32(this.numOrderLeadTime.Value);
            ApplicationSettings.Current.QuoteExpirationDays = Convert.ToInt32(this.numQuoteExpirtation.Value);
            ApplicationSettings.Current.MinimumOrderPrice = Convert.ToDouble(this.numMinPrice.Value);
            ApplicationSettings.Current.MinimumProcessPrice = Convert.ToDouble(this.numProcessMinPrice.Value);
            ApplicationSettings.Current.DefaultPrinterType = cboPrinterType.SelectedItem != null ? (DWOS.Data.PrinterType) Convert.ToInt32(cboPrinterType.SelectedItem.DataValue) : DWOS.Data.PrinterType.Document;
            ApplicationSettings.Current.ScheduleResetEnabled = this.chkScheduleReset.Checked;
            ApplicationSettings.Current.WeightDecimalPlaces = Convert.ToInt32(this.numWeightDecimals.Value);
            ApplicationSettings.Current.ProcessingDecimalPlaces = Convert.ToInt32(this.numProcessDecimals.Value);
            ApplicationSettings.Current.AllowAdditionalCustomersForContacts = chkAdditionalCustomers.Checked;
            ApplicationSettings.Current.SaveWorkOrderPrintHistory = this.chkSaveOrderPrintHistory.Checked;
            ApplicationSettings.Current.UseReportPlaceholderImage = this.chkUsePlaceholder.Checked;
            ApplicationSettings.Current.IncludeHoldsInLateOrders = chkIncludeHoldsWithLateOrders.Checked;
            ApplicationSettings.Current.ShowCompanyLogoOnPackingSlip = this.chkCompanyLogoPackingSlip.Checked;
            ApplicationSettings.Current.OrderItemFormat = this.txtOrderFormat.Text;
            ApplicationSettings.Current.TravelerProcessConfirmation = (cboProcessConfirm.Value as TravelerProcessConfirmationType?) ?? TravelerProcessConfirmationType.QtyDateBy;
            ApplicationSettings.Current.PrintSummariesForRejoinedOrders = chkPrintRejoinedOrders.Checked;
            ApplicationSettings.Current.DefaultFees = txtDefaultFees.Text;
            ApplicationSettings.Current.ApplyDefaultFeesEnabled = chkApplyDefaultFees.Checked;


            if (schedulerType == SchedulerType.Manual || schedulerType == SchedulerType.ManualAllDepartments)
            {
                ApplicationSettings.Current.ScheduleResetTime = dteScheduleDate.DateTime.Subtract(DateTime.Now.StartOfDay());
            }
            else
            {
                ApplicationSettings.Current.ReceivingRolloverTime = dteScheduleDate.DateTime.Subtract(DateTime.Now.StartOfDay());
            }

            if (this._accreditationLogoChanged)
            {
                var img = (picAccredidationLogo.Image ?? picAccredidationLogo.DefaultImage) as Bitmap;

                if (img != null)
                {
                    var logoTempFile = System.IO.Path.GetTempFileName() + ".png";
                    img.Save(logoTempFile, ImageFormat.Png);

                    ApplicationSettings.Current.AccreditationLogoImagePath = logoTempFile;
                }
            }

            if (this._placeholderImageChanged)
            {
                var img = (picPlaceholder.Image ?? picPlaceholder.DefaultImage) as Bitmap;

                if (img != null)
                {
                    var logoTempFile = System.IO.Path.GetTempFileName() + ".png";
                    img.Save(logoTempFile, ImageFormat.Png);

                    ApplicationSettings.Current.ReportPlaceholderImagePath = logoTempFile;
                }
            }


            //Update the cache version to force to get new images on app restart
            if (this._accreditationLogoChanged || this._placeholderImageChanged)
            {
                ApplicationSettings.Current.CacheVersion = ApplicationSettings.Current.CacheVersion + 1;
            }
        }

        #endregion

        #region Events

        private void btnBrowseAccreditation_Click(object sender, EventArgs e)
        {
            try
            {
                using (var fd = new OpenFileDialog())
                {
                    fd.Filter = IMAGE_FORMAT_FILTER;

                    if (fd.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                    {
                        this.picAccredidationLogo.Image = new Bitmap(fd.FileName);
                        this._accreditationLogoChanged = true;
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error opening accreditation media.");
            }
        }

        private void chkSchedulingEnabled_CheckedChanged(object sender, EventArgs e)
        {
            cboScheduleType.Enabled = chkSchedulingEnabled.Checked;
            dteScheduleDate.Enabled = chkSchedulingEnabled.Checked;
        }
        private void btnBrowsePlaceholder_Click(object sender, EventArgs e)
        {
            try
            {
                using (var fd = new OpenFileDialog())
                {
                    fd.Filter = IMAGE_FORMAT_FILTER;

                    if (fd.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                    {
                        this.picPlaceholder.Image = new Bitmap(fd.FileName);
                        this._placeholderImageChanged = true;
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error opening placeholder media.");
            }
        }

        private void chkUsePlaceholder_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bool controlsEnabled = chkUsePlaceholder.Checked;
                btnBrowsePlaceholder.Enabled = controlsEnabled;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing 'use placeholder' checkbox.");
            }
        }

        private void cboScheduleType_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                var selectedScheduleType = cboScheduleType.Value as SchedulerType?;
                var manualSchedulerSelected = selectedScheduleType == SchedulerType.Manual || selectedScheduleType == SchedulerType.ManualAllDepartments;
                chkScheduleReset.Enabled = manualSchedulerSelected;

                lblScheduleWarning.Visible = selectedScheduleType.HasValue
                    && selectedScheduleType != SchedulerType.ProcessLeadTime
                    && selectedScheduleType != SchedulerType.ProcessLeadTimeHour
                    && selectedScheduleType != ApplicationSettings.Current.SchedulerType;

                var timeValue = manualSchedulerSelected
                    ? ApplicationSettings.Current.ScheduleResetTime
                    : ApplicationSettings.Current.ReceivingRolloverTime;

                dteScheduleDate.DateTime = DateTime.Now.StartOfDay().Add(timeValue);

                var dteScheduleDateToolip = ultraToolTipManager1.GetUltraToolTip(dteScheduleDate);

                dteScheduleDateToolip.ToolTipTitle = manualSchedulerSelected
                    ? "Schedule Reset Time"
                    : "Rollover Time";

                dteScheduleDateToolip.ToolTipTextFormatted = manualSchedulerSelected
                    ? @"When using manual scheduling, work order priorities reset at this time every day."
                    : @"The rollover time is used to determine when to add an additional day for processing within receiving.<br/><br/>For example rollover time is set to 8 AM;<br/><br/>&edsp;If an order is entered at 7 AM the first process will include the current day for processing.<br/>&edsp;If an order is entered at 9 AM the first process will NOT include the current day for processing.&edsp;<br/>";
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing schedule type");
            }
        }

        #endregion

    }
}