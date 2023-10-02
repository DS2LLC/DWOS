using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using CronExpressionDescriptor;
using DWOS.Data;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using NLog;
using System.Collections.Generic;
using System.Linq;
using Infragistics.Win.UltraWinGrid;
using System.Windows.Interop;

namespace DWOS.UI.Sales.Customer
{
    public partial class CustomerContacts : DataPanel
    {
        #region Fields

        private readonly BindingList<NotificationItem> _notifications =
            new BindingList<NotificationItem>();

        private readonly BindingList<AdditionalCustomerItem> _additionalCustomers =
            new BindingList<AdditionalCustomerItem>();

        private DWOS.Utilities.Validation.ValidatorManager _validationManager;

        private GridSettingsPersistence<UltraGridBandSettings> _customersGridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("CustomerContacts_Customers", new UltraGridBandSettings());

        #endregion

        #region Properties

        public CustomersDataset Dataset
        {
            get => _dataset as CustomersDataset;
            set => _dataset = value;
        }

        protected override string BindingSourcePrimaryKey =>
            Dataset.Contact.ContactIDColumn.ColumnName;

        /// <summary>
        ///     Gets a value indicating whether an email address is required for this contact.
        /// </summary>
        /// <value><c>true</c> if [requires email]; otherwise, <c>false</c>.</value>
        private bool RequiresEmail => chkHoldNotification.Checked
            || chkShippingNotification.Checked
            || chkCertNotification.Checked
            || chkIsAuthorized.Checked
            || chkApprovalNotification.Checked
            || chkLateOrderNotification.Checked
            || _notifications.Count > 0;

        #endregion

        #region Methods

        public CustomerContacts()
        {
            InitializeComponent();
            grdNotifications.DataSource = _notifications;
            grdAdditionalCustomers.DataSource = _additionalCustomers;
        }

        public void LoadData(CustomersDataset dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.Contact.TableName;

            //bind column to control
            BindValue(txtContactName, Dataset.Contact.NameColumn.ColumnName);
            BindValue(chkContactPrimary, Dataset.Contact.PrimaryContactColumn.ColumnName);
            BindValue(txtContactPhone, Dataset.Contact.PhoneNumberColumn.ColumnName);
            BindValue(txtContactFax, Dataset.Contact.FaxNumberColumn.ColumnName);
            BindValue(txtContactEmail, Dataset.Contact.EmailAddressColumn.ColumnName);
            BindValue(txtContactNotes, Dataset.Contact.NotesColumn.ColumnName);
            BindValue(chkActive, Dataset.Contact.ActiveColumn.ColumnName);

            BindValue(chkHoldNotification, Dataset.Contact.HoldNotificationColumn.ColumnName);
            BindValue(chkShippingNotification, Dataset.Contact.ShippingNotificationColumn.ColumnName);
            BindValue(chkIncludeCOCInShipping, Dataset.Contact.IncludeCOCInShippingNotificationsColumn.ColumnName);
            BindValue(chkCertNotification, Dataset.Contact.COCNotificationColumn.ColumnName);
            BindValue(chkApprovalNotification, Dataset.Contact.ApprovalNotificationColumn.ColumnName);
            BindValue(chkLateOrderNotification, Dataset.Contact.LateOrderNotificationColumn.ColumnName);
            BindValue(cbchkOrderReceiptNotification, Dataset.Contact.OrderReceiptNotificationColumn.ColumnName);
            BindValue(chkIsAuthorized, Dataset.Contact.PortalAuthorizedColumn.ColumnName);
            BindValue(dtePortalAuthDate, Dataset.Contact.PortalAuthorizationSentColumn.ColumnName);
            BindValue(cboManufacturer, Dataset.Contact.ManufacturerIDColumn.ColumnName);
            BindValue(cboInvoicePreference, Dataset.Contact.InvoicePreferenceColumn.ColumnName);

            BindList(cboManufacturer, Dataset.d_Manufacturer, Dataset.d_Manufacturer.ManufacturerIDColumn.ToString(), Dataset.d_Manufacturer.ManufacturerIDColumn.ToString());
            BindList(cboInvoicePreference, Dataset.d_InvoicePreference, Dataset.d_InvoicePreference.InvoicePreferenceIDColumn.ToString(), Dataset.d_InvoicePreference.InvoicePreferenceIDColumn.ToString());

            // Hide 'other customers' tab if feature is disabled
            ultraTabControl1.Tabs["OtherCustomers"].Visible =
                ApplicationSettings.Current.AllowAdditionalCustomersForContacts;

            // Hide 'approval notification' option if feature is disabled
            chkApprovalNotification.Visible =
                ApplicationSettings.Current.OrderApprovalEnabled;

            //this.chkIncludeCOCInShipping.Enabled = this.chkShippingNotification.Checked;

            _panelLoaded = true;
        }

        public override void AddValidators(DWOS.Utilities.Validation.ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(txtContactName, "Contact name required.") {DefaultValue = "New Contact"}, errProvider));
            manager.Add(new ImageDisplayValidator(new EmailValidator(txtContactEmail, this), errProvider));
            _validationManager = manager;
        }

        public CustomersDataset.ContactRow AddContactRow(int customerID)
        {
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as CustomersDataset.ContactRow;
            cr.Name = "New Contact";
            cr.CustomerID = customerID;
            cr.PortalAuthorized = false;
            cr.Active = true;

            return cr;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            txtContactName.Focus();
            txtContactName.SelectAll();
            base.AfterMovedToNewRecord(id);

            var contact = CurrentRecord as CustomersDataset.ContactRow;
            btnEmail.Enabled = contact != null && contact.PortalAuthorized;
            btnAddReport.Enabled = contact != null;
            btnEditReport.Enabled = contact != null;
            btnRemoveReport.Enabled = contact != null;
            
            
            LoadReportTasks(contact);
            LoadAdditionalCustomers(contact);
            RefreshEmailValidator();
            RefreshAdditionalCustomersColumns();
            RefreshNotificationWarnings();
            RefreshIncludeCOCInShipping();
        }

        private void LoadReportTasks(CustomersDataset.ContactRow contact)
        {
            try
            {
                _notifications.Clear();

                if (contact != null)
                {
                    foreach (var reportTask in contact.GetReportTaskRows())
                    {
                        _notifications.Add(new NotificationItem(reportTask));
                    }
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error loading contact report tasks.");
            }
        }

        private void LoadAdditionalCustomers(CustomersDataset.ContactRow contact)
        {
            try
            {
                _additionalCustomers.Clear();

                if (contact != null)
                {
                    foreach (var row in contact.GetContactAdditionalCustomerRows())
                    {
                        _additionalCustomers.Add(new AdditionalCustomerItem(row));
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error loading additional customers for contact.");
            }
        }

        private CustomersDataset.ReportTaskRow AddReportTask()
        {
            try
            {
                var contact = CurrentRecord as CustomersDataset.ContactRow;

                if(contact != null)
                {
                    using(var cbo = new ComboBoxForm())
                    {
                        cbo.Text = "Add Report Notification";
                        cbo.FormLabel.Text = "Report:";
                        cbo.ComboBox.DropDownStyle = DropDownStyle.DropDownList;
                        cbo.ComboBox.ValueMember = Dataset.ReportType.ReportTypeIDColumn.ColumnName;
                        cbo.ComboBox.DisplayMember = Dataset.ReportType.DisplayNameColumn.ColumnName;
                        cbo.ComboBox.DataSource = Dataset.ReportType;
                        cbo.ComboBox.DataBind();

                        if(cbo.ComboBox.Items.Count > 0)
                            cbo.ComboBox.SelectedIndex = 0;

                        if(cbo.ShowDialog(this) == DialogResult.OK)
                        {
                            if(cbo.ComboBox.SelectedItem != null)
                            {
                                var reportType = ((DataRowView) cbo.ComboBox.SelectedItem.ListObject).Row as CustomersDataset.ReportTypeRow;
                                return Dataset.ReportTask.AddReportTaskRow(contact, reportType, reportType.IsDefaultScheduleNull() ? "0 20 23 ? * MON-FRI" : reportType.DefaultSchedule);
                            }
                        }
                    }
                }

                return null;
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error adding new report task.");
                return null;
            }
        }

        private void AuthorizeContactForPortal()
        {
            if (!(CurrentRecord is CustomersDataset.ContactRow contact))
            {
                return;
            }

            contact.SetPortalAuthorizationSentNull();
            dtePortalAuthDate.DataBindings[0].ReadValue(); // show that authorization date was removed

            MessageBoxUtilities.ShowMessageBoxOK(
                "The contact will be sent an email with login credentials after you click OK or Apply to save your changes.",
                "Authorize User");
        }

        private void RefreshEmailValidator()
        {
            var validator = _validationManager?.Find(txtContactEmail)?.Validator;

            if (validator != null)
            {
                validator.IsRequired = RequiresEmail;
            }
        }

        private void RefreshAdditionalCustomersColumns()
        {
            var mainBand = grdAdditionalCustomers.DisplayLayout.Bands[0];

            // Automated notifications
            var hasAutomatedNotification = _notifications.Count > 0;
            var automatedColumn = mainBand.Columns[nameof(AdditionalCustomerItem.IncludeInAutomatedNotifications)];
            automatedColumn.CellActivation = hasAutomatedNotification ? Activation.AllowEdit : Activation.Disabled;

            automatedColumn.Header.ToolTipText = hasAutomatedNotification
                ? string.Empty
                : "This contact has no automated notifications.";

            automatedColumn.Header.Appearance.Image = hasAutomatedNotification
                ? null
                : Properties.Resources.Warning_16;

            // Hold Notification
            var hasHoldNotification = chkHoldNotification.Checked;
            var holdColumn = mainBand.Columns[nameof(AdditionalCustomerItem.IncludeInHoldNotifications)];
            holdColumn.CellActivation = hasHoldNotification ? Activation.AllowEdit : Activation.Disabled;

            holdColumn.Header.ToolTipText = hasHoldNotification
                ? string.Empty
                : "Hold notifications are not enabled for this contact.";

            holdColumn.Header.Appearance.Image = hasHoldNotification
                ? null
                : Properties.Resources.Warning_16;

            // Shipping notifications
            var hasShippingNotification = chkShippingNotification.Checked;
            var shippingColumn = mainBand.Columns[nameof(AdditionalCustomerItem.IncludeInShippingNotifications)];
            shippingColumn.CellActivation = hasShippingNotification ? Activation.AllowEdit : Activation.Disabled;

            shippingColumn.Header.ToolTipText = hasShippingNotification
                ? string.Empty
                : "Shipping notifications are not enabled for this contact.";

            shippingColumn.Header.Appearance.Image = hasShippingNotification
                ? null
                : Properties.Resources.Warning_16;

            // Certificate notifications
            var hasCertificateNotification = chkCertNotification.Checked;
            var certificateColumn = mainBand.Columns[nameof(AdditionalCustomerItem.IncludeInCocNotifications)];
            certificateColumn.CellActivation = hasCertificateNotification ? Activation.AllowEdit : Activation.Disabled;

            certificateColumn.Header.ToolTipText = hasCertificateNotification
                ? string.Empty
                : "Certificate notifications are not enabled for this contact.";

            certificateColumn.Header.Appearance.Image = hasCertificateNotification
                ? null
                : Properties.Resources.Warning_16;

            // Approval notifications
            var hasApprovalNotifications = chkApprovalNotification.Checked;
            var approvalColumn = mainBand.Columns[nameof(AdditionalCustomerItem.IncludeInApprovalNotifications)];
            approvalColumn.CellActivation = hasApprovalNotifications ? Activation.AllowEdit : Activation.Disabled;

            approvalColumn.Header.ToolTipText = hasApprovalNotifications
                ? string.Empty
                : "Approval notifications are not enabled for this contact.";

            approvalColumn.Header.Appearance.Image = hasApprovalNotifications
                ? null
                : Properties.Resources.Warning_16;

            // Portal access
            var canAccessPortal = chkIsAuthorized.Checked;
            var portalColumn = mainBand.Columns[nameof(AdditionalCustomerItem.IncludeInPortal)];
            portalColumn.CellActivation = canAccessPortal ? Activation.AllowEdit : Activation.Disabled;

            portalColumn.Header.ToolTipText = canAccessPortal
                ? string.Empty
                : "Contact is not authorized for Portal.";

            portalColumn.Header.Appearance.Image = canAccessPortal
                ? null
                : Properties.Resources.Warning_16;

            // Late Order notifications
            var hasLateOrderNotifications = chkLateOrderNotification.Checked;
            var lateOrderColumn = mainBand.Columns[nameof(AdditionalCustomerItem.IncludeInLateOrderNotifications)];
            lateOrderColumn.CellActivation = hasLateOrderNotifications ? Activation.AllowEdit : Activation.Disabled;

            lateOrderColumn.Header.ToolTipText = hasLateOrderNotifications
                ? string.Empty
                : "Late order notifications are not enabled for this contact.";

            lateOrderColumn.Header.Appearance.Image = hasLateOrderNotifications
                ? null
                : Properties.Resources.Warning_16;
        }

        private void RefreshNotificationWarnings()
        {
            var showApprovalWarning = ApplicationSettings.Current.OrderApprovalEnabled
                && chkApprovalNotification.Checked
                && !chkIsAuthorized.Checked;

            picApprovalDisabled.Visible = showApprovalWarning;
        }
        private void RefreshIncludeCOCInShipping()
        {
            this.chkIncludeCOCInShipping.Enabled = this.chkShippingNotification.Checked;
            this.chkIncludeCOCInShipping.UseAppStyling = this.chkShippingNotification.Checked;
            this.chkIncludeCOCInShipping.Appearance.ForeColor = this.chkShippingNotification.Checked ? System.Drawing.Color.Black : System.Drawing.Color.Gray;
            if (!this.chkShippingNotification.Checked)
                this.chkIncludeCOCInShipping.Checked = false;
        }
        #endregion

        #region Events

        private void cboManufacturer_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                cboManufacturer.SelectedIndex = -1;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error clearing selected manufacturer.";
                _log.Error(exc, errorMsg);
            }
        }

        private void cboInvoicePreference_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                cboInvoicePreference.SelectedIndex = -1;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error clearing selected invoice preference.";
                _log.Error(exc, errorMsg);
            }
        }

        private void chkIsAuthorized_CheckedChanged(object sender, EventArgs e)
        {
            const string resetAuthorizedDatePrompt = "Would you like to send the contact an email with their login credentials?";

            try
            {
                if (_recordLoading || !_panelLoaded || !IsActivePanel)
                {
                    return;
                }

                // This method can invoke binding changes for other fields,
                // which resets the checkbox's value.
                // Writing the value here prevents VSTS #26057.
                chkIsAuthorized.DataBindings[0].WriteValue();

                // Refresh 'require email' validator
                RefreshEmailValidator();

                // Refresh 'other customers' grid
                RefreshAdditionalCustomersColumns();

                // Authorize/deauthorize contact
                var isAuthorized = chkIsAuthorized.Checked;

                if (isAuthorized)
                {
                    if (string.IsNullOrEmpty(txtContactEmail.Text))
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            "The contact has to have a valid email address to be authorized for use on the portal.",
                            "Unable to Authorize");

                        chkIsAuthorized.Checked = false;
                        return;
                    }

                    var authorizeContact = dtePortalAuthDate.Value == null
                        || MessageBoxUtilities.ShowMessageBoxYesOrNo(resetAuthorizedDatePrompt, "Authorize User") == DialogResult.Yes;

                    if (authorizeContact)
                    {
                        AuthorizeContactForPortal();
                    }
                }

                btnEmail.Enabled = isAuthorized;

                RefreshNotificationWarnings();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Failed to update authorized setting for contact.");
            }
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            try
            {
                AuthorizeContactForPortal();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Failed to authorize user for customer portal.");
            }
        }

        private void btnAddReport_Click(object sender, EventArgs e)
        {
            var report = AddReportTask();

            if (report != null)
            {
                _notifications.Add(new NotificationItem(report));
                RefreshEmailValidator();
                RefreshAdditionalCustomersColumns();
            }
        }

        private void btnEditReport_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdNotifications.Selected.Rows.Count != 1)
                {
                    return;
                }

                var selectedRow = grdNotifications.Selected.Rows[0];

                if (!selectedRow.IsDataRow)
                {
                    return;
                }

                // Edit selected report notification.
                var notificationItem = grdNotifications.Selected.Rows[0].ListObject as NotificationItem;
                var notificationRow = notificationItem.ReportTask;

                var dialog = new ContactReportTaskEditor(notificationRow.ReportTypeRow.DisplayName, notificationRow.Schedule);

                var helper = new WindowInteropHelper(dialog) { Owner = Handle };

                if (dialog.ShowDialog() ?? false)
                {
                    // Save changes to row
                    notificationRow.Schedule = dialog.Schedule;

                    // Update UI
                    notificationItem.RefreshSchedule();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error while editing a report task.");
            }
        }

        private void btnRemoveReport_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdNotifications.Selected.Rows.Count != 1)
                {
                    return;
                }

                var selectedRow = grdNotifications.Selected.Rows[0];

                if (selectedRow.IsDataRow)
                {
                    var notification = grdNotifications.Selected.Rows[0].ListObject as NotificationItem;
                    _notifications.Remove(notification);
                    notification.ReportTask.Delete();

                    RefreshEmailValidator();
                    RefreshAdditionalCustomersColumns();
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error removing a report task.");
            }
        }

        private void Notification_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (_recordLoading || !_panelLoaded || !IsActivePanel)
                {
                    return;
                }

                RefreshEmailValidator();
                RefreshAdditionalCustomersColumns();
                RefreshNotificationWarnings();
                RefreshIncludeCOCInShipping();

            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing checked value for notification");
            }
        }



        private void chkIncludeCOCInShipping_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (_recordLoading || !_panelLoaded || !IsActivePanel)
                {
                    return;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing checked value for include Order Acknowledgement notification");
            }

        }


        private void grdAdditionalCustomers_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                grdAdditionalCustomers.AfterColPosChanged -= grdAdditionalCustomers_AfterColPosChanged;
                grdAdditionalCustomers.AfterSortChange -= grdAdditionalCustomers_AfterSortChange;

                // Load settings
                _customersGridSettingsPersistence.LoadSettings().ApplyTo(grdAdditionalCustomers.DisplayLayout.Bands[0]);


                // Hide 'Approval Notification 'column if feature is disabled
                var approvalNotificationColumn = grdAdditionalCustomers.DisplayLayout.Bands[0].Columns[nameof(AdditionalCustomerItem.IncludeInApprovalNotifications)];
                var hideOrderApproval = !ApplicationSettings.Current.OrderApprovalEnabled;
                approvalNotificationColumn.Hidden = hideOrderApproval;
                approvalNotificationColumn.ExcludeFromColumnChooser = hideOrderApproval
                    ? ExcludeFromColumnChooser.True
                    : ExcludeFromColumnChooser.False;

                // Refresh columns based on current selections
                RefreshAdditionalCustomersColumns();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error initializing grid layout.");
            }
            finally
            {
                grdAdditionalCustomers.AfterColPosChanged += grdAdditionalCustomers_AfterColPosChanged;
                grdAdditionalCustomers.AfterSortChange += grdAdditionalCustomers_AfterSortChange;
            }
        }

        private void grdAdditionalCustomers_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdAdditionalCustomers.DisplayLayout.Bands[0]);
                _customersGridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position.");
            }
        }

        private void grdAdditionalCustomers_AfterSortChange(object sender, BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdAdditionalCustomers.DisplayLayout.Bands[0]);
                _customersGridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position.");
            }
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                var currentContact = CurrentRecord as CustomersDataset.ContactRow;

                if (currentContact == null)
                {
                    return;
                }

                var customers = new List<CustomersDataset.CustomerRow>(Dataset.Customer);
                customers.Remove(currentContact.CustomerRow);

                foreach (var additionalCustomer in _additionalCustomers)
                {
                    customers.Remove(additionalCustomer.Row.CustomerRow);
                }

                using (var cbo = new ComboBoxForm())
                {
                    cbo.chkOption.Visible = false;
                    cbo.Text = "Select Customer";

                    foreach (var customer in customers.OrderBy(c => c.Name))
                    {
                        cbo.ComboBox.Items.Add(new ValueListItem(customer, customer.Name));
                    }

                    cbo.ComboBox.SelectedIndex = 0;
                    cbo.FormLabel.Text = "Customer:";

                    if (cbo.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                    {
                        if (cbo.ComboBox.SelectedItem?.DataValue is CustomersDataset.CustomerRow selectedCustomer)
                        {
                            // Create row
                            var additionalCustomerRow = Dataset.ContactAdditionalCustomer.AddContactAdditionalCustomerRow(
                                currentContact,
                                selectedCustomer,
                                false,
                                false,
                                false,
                                false,
                                false,
                                false,
                                false,
                                false);

                            _additionalCustomers.Add(new AdditionalCustomerItem(additionalCustomerRow));

                            grdAdditionalCustomers.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error while adding customer.");
            }
        }

        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdAdditionalCustomers.Selected.Rows.Count != 1)
                {
                    return;
                }

                var selectedRow = grdAdditionalCustomers.Selected.Rows[0];

                if (selectedRow.IsDataRow)
                {
                    var additionalCustomer = selectedRow.ListObject as AdditionalCustomerItem;
                    _additionalCustomers.Remove(additionalCustomer);
                    additionalCustomer.Row.Delete();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error removing an additional customer.");
            }
        }

        private void cbchkOrderReceiptNotification_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (_recordLoading || !_panelLoaded || !IsActivePanel)
                {
                    return;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing checked value for include COC in shipping notification");
            }

        }

        #endregion

        #region EmailValidator

        private class EmailValidator : ControlValidatorBase
        {
            #region Fields

            private CustomerContacts _contactInfo;

            #endregion

            #region Methods

            public EmailValidator(UltraTextEditor control, CustomerContacts contactInfo)
                : base(control)
            {
                _contactInfo = contactInfo;
            }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                try
                {
                    if(Control != null && Control.Enabled && !((UltraTextEditor) Control).ReadOnly)
                    {
                        string emailAddress = Control.Text;

                        //if requires an email address, then ensure has something in text box
                        if(_contactInfo.CurrentRecord != null && _contactInfo.RequiresEmail && String.IsNullOrEmpty(emailAddress))
                        {
                            e.Cancel = true;
                            FireAfterValidation(false, "Email Address required if notifications are to be sent to the contact.");
                            return;
                        }
                        else if (!String.IsNullOrEmpty(emailAddress) && !emailAddress.IsValidEmail())
                        {
                            e.Cancel = true;
                            FireAfterValidation(false, "Email Address is not valid.");
                            return;
                        }
                    }

                    //passed
                    e.Cancel = false;
                    FireAfterValidation(true, "");
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error checking to see if email address is required.";
                    LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
                }
            }

            public override void Dispose()
            {
                _contactInfo = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region NotificationItem

        private class NotificationItem
        {
            public string Name { get; }

            public string Schedule { get; private set; }

            public CustomersDataset.ReportTaskRow ReportTask { get; }

            public NotificationItem(CustomersDataset.ReportTaskRow reportTask)
            {
                ReportTask = reportTask ?? throw new ArgumentNullException(nameof(reportTask));

                Name = reportTask.ReportTypeRow?.DisplayName;
                RefreshSchedule();
            }

            public void RefreshSchedule()
            {
                Schedule = ExpressionDescriptor.GetDescription(ReportTask.Schedule,
                    new Options { ThrowExceptionOnParseError = false });
            }
        }

        #endregion

        #region AdditionalCustomerItem

        /// <summary>
        /// Represents a secondary customer for a contact.
        /// </summary>
        private class AdditionalCustomerItem
        {
            public CustomersDataset.ContactAdditionalCustomerRow Row { get; }

            public string CustomerName => Row.CustomerRow.Name;

            public bool IncludeInPortal
            {
                get => Row.IncludeInPortal;
                set => Row.IncludeInPortal = value;
            }

            public bool IncludeInHoldNotifications
            {
                get => Row.IncludeInHoldNotifications;
                set => Row.IncludeInHoldNotifications = value;
            }

            public bool IncludeInShippingNotifications
            {
                get => Row.IncludeInShippingNotifications;
                set => Row.IncludeInShippingNotifications = value;
            }

            public bool IncludeInCocNotifications
            {
                get => Row.IncludeInCocNotifications;
                set => Row.IncludeInCocNotifications = value;
            }

            public bool IncludeInAutomatedNotifications
            {
                get => Row.IncludeInAutomatedNotifications;
                set => Row.IncludeInAutomatedNotifications = value;
            }

            public bool IncludeInApprovalNotifications
            {
                get => Row.IncludeInApprovalNotifications;
                set => Row.IncludeInApprovalNotifications = value;
            }

            public bool IncludeInLateOrderNotifications
            {
                get => Row.IncludeInLateOrderNotifications;
                set => Row.IncludeInLateOrderNotifications = value;
            }

            public AdditionalCustomerItem(CustomersDataset.ContactAdditionalCustomerRow row)
            {
                Row = row ?? throw new ArgumentNullException(nameof(row));
            }
        }

        #endregion


    }
}