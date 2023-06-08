using System;
using System.Windows.Forms;
using nsoftware.InQB;
using System.Collections.Generic;
using DWOS.Data;
using DWOS.Data.Datasets;
using System.Linq;
using DWOS.Data.Customer;
using DWOS.QBExport.Syncing.Dialogs;
using DWOS.Shared.Wizard;
using NLog;

namespace DWOS.QBExport.Syncing
{
    /// <summary>
    /// Wizard step that shows sync progress.
    /// </summary>
    public partial class SyncProgress : UserControl, IWizardPanel
    {
        #region Fields

        private const int MAX_SYNC_ERRORS = 50;
        private const int MAX_QUICKBOOKS_LENGTH = 41;
        private const int MAX_QUICKBOOKS_PC_LENGTH = 25; //QB primary contact (first name) field max = 25 chars.

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        private Customer _qbCustomer;
        private Qblists _qbLists;
        private Item _qbItem;
        private Objsearch _qbSearch;

        private readonly CustomersDataset _dsCustomer = new CustomersDataset
        {
            EnforceConstraints = false
        };

        private Dictionary<MessageType, List<string>> _results = new Dictionary<MessageType, List<string>>();
        private readonly Dictionary<string, bool> _feeInQBCache = new Dictionary<string, bool>();
        private Dictionary<CustomersDataset.CustomerRow, string> _updateQueue = new Dictionary<CustomersDataset.CustomerRow, string>();
        private Dictionary<CustomersDataset.CustomerRow, string> _updateExistingQueue = new Dictionary<CustomersDataset.CustomerRow, string>();
        private List<CustomersDataset.CustomerRow> _addQueue = new List<CustomersDataset.CustomerRow>();
        private CustomersDataset.CustomerRow _customerSyncing;
        private int _syncErrorCount;
        private bool _syncing = true;
        private bool _abandonSync;
        private bool canceled;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncProgress"/> class.
        /// </summary>
        public SyncProgress()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Synchronizes the customers.
        /// </summary>
        private void SyncCustomers()
        {
            try
            {
                AddStatusMessage(MessageType.Normal, "Starting sync.");

                // Initialize QuickBooks objects
                _qbCustomer = new Customer
                {
                    QBConnectionString = ApplicationSettings.Current.QBConnectionString
                };

                _qbCustomer.OpenQBConnection();

                _qbLists = new Qblists
                {
                    QBConnectionString = ApplicationSettings.Current.QBConnectionString
                };

                _qbLists.OpenQBConnection();

                _qbItem = new Item
                {
                    QBConnectionString = ApplicationSettings.Current.QBConnectionString
                };

                _qbItem.OpenQBConnection();

                _qbSearch = new Objsearch
                {
                    QBConnectionString = ApplicationSettings.Current.QBConnectionString
                };

                _qbSearch.OpenQBConnection();

                // Start sync
                AddStatusMessage(MessageType.Normal, "Gathering customer data from DWOS.");
                using (var taCustomers = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
                {
                    taCustomers.Fill(_dsCustomer.Customer);
                }

                AddStatusMessage(MessageType.Normal, "Gathering customer data from QuickBooks.");
                var qbCustomerList = GetQuickBooksCustomers();

                if (QBSyncController.Source == SyncSource.DWOS)
                {
                    this.AddStatusMessage(MessageType.Normal, "Syncing fees from DWOS to QuickBooks.");
                    this.SyncDWOSFeesWithQB();

                    this.AddStatusMessage(MessageType.Normal, "Syncing customers from DWOS to QuickBooks.");

                    this.SetupProgressBar(_dsCustomer.Customer.Count);
                    AddStatusMessage(MessageType.Normal, "Customer selection process started...");
                    foreach (var customerRow in _dsCustomer.Customer)
                    {                       
                        if (!customerRow.Active)
                        {
                            // Only sync active customers
                            continue;
                        }

                        // Select customers to sync
                        else if (!DoesCustomerExistInQB(customerRow, out string quickBooksName))
                        {
                            if (CheckAddress(customerRow, out string msg, out string addr1, out string addr2))
                            {
                                AddStatusMessage(
                                    MessageType.Warning,
                                    msg);
                                customerRow.Address1 = addr1;
                                customerRow.Address2 = addr2;
                            }
                            // Prompt user to sync customer
                            var customerDialog = new SelectQuickBooksCustomer();
                            customerDialog.LoadData(customerRow, qbCustomerList);
                            customerDialog.ShowDialog();

                            if (customerDialog.DialogResult == DialogResult.Cancel)
                            {
                                this.canceled = true;
                                break;
                            }

                            if (customerDialog.DialogResult == DialogResult.OK)
                            {
                                if (string.IsNullOrWhiteSpace(customerDialog.SelectedCustomerName))
                                {
                                    // Customer does not exist in QB, add customer to be synced                                       
                                    _addQueue.Add(customerRow);
                                }
                                else
                                {
                                    // Sync the customer's account ID    
                                    customerRow.AccountingID = customerDialog.SelectedCustomerName;
                                    // Sync DWOS customer to QB customer
                                    _updateQueue.Add(customerRow, customerDialog.SelectedCustomerName);
                                }
                            }
                            else if (customerDialog.DialogResult == DialogResult.Ignore)
                            {
                                // Skip this customer
                                AddStatusMessage(MessageType.Warning, $"Skipped {customerRow.Name} - user declined to sync user.");
                            }
                        }
                        else
                        {
                            if (CheckAddress(customerRow, out string msg, out string addr1, out string addr2))
                            {
                                AddStatusMessage(
                                    MessageType.Warning,
                                    msg);
                                customerRow.Address1 = addr1;
                                customerRow.Address2 = addr2;
                            }
                            // Update existing QB customer information
                            _updateExistingQueue.Add(customerRow, quickBooksName);
                        }
                        // Update progress
                        this.UpdateProgressBar(1);
                    }

                    // Sync queued customers
                    SyncDWOSCustomersWithQB();
                   
                    if (this.canceled)
                    {
                        // Refresh the queues
                        _addQueue.Clear();
                        _updateQueue.Clear();
                        _updateExistingQueue.Clear();

                        // Refresh progress bar
                        this.UpdateProgressBar(-pgbSyncProgress.Value);
                        this.canceled = false;
                        this.AddStatusMessage(MessageType.Normal, "Sync selection canceled");
                    }
                    else
                    {
                        // Push progress bar to 100
                        this.UpdateProgressBar(pgbSyncProgress.Maximum - pgbSyncProgress.Value);
                        this.AddStatusMessage(MessageType.Normal, "Sync selection completed.");
                    }
                }

                else if (QBSyncController.Source == SyncSource.QuickBooks)
                {
                    this.AddStatusMessage(MessageType.Normal, "Syncing customers from QuickBooks to DWOS.");

                    this.SyncQBCustomersWithDWOS(qbCustomerList);
                    this.AddStatusMessage(MessageType.Normal, "Sync completed.");
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Unable to find the QuickBooks Request Processor on your system."))
                {
                    _log.Warn(e, "Unable to find the QuickBooks Request Processor on your system.");

                    this.AddStatusMessage(
                        MessageType.Error,
                        "Unable to find the QuickBooks Request Processor on your system. Ensure Quickbooks is installed on this machine, is running, and you started DWOS with administrative privelages.");
                }
                else
                {
                    var errorMsg = QBSyncController.Source == SyncSource.DWOS
                        ? "Unable to sync customers from DWOS to QuickBooks."
                        : "Unable to sync customers from QuickBooks to DWOS.";

                    _log.Error(e, errorMsg);

                    // Since error is unrecoverable, show it in both the
                    // Error and Normal tabs.
                    AddStatusMessage(MessageType.Error, errorMsg);
                    AddStatusMessage(MessageType.Normal, errorMsg);
                }
            }

        }

        /// <summary>
        /// Sync customers selected during customer selection/mapping process.
        /// </summary>
        private void SyncDWOSCustomersWithQB()
        {
            while (_syncing)
            {
                string function = string.Empty;

                if (this.canceled)
                {
                    break;
                }

                var confirmSyncDialog = MessageBox.Show("WARNING: The sync operation is about to begin\n" +
                    "Once the process has begun it can not be canceled for the duration of the sync operation.\n" +
                    "Press 'Cancel' now to cancel, or press 'OK' to continue", "Confirm Sync Operation",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (confirmSyncDialog != DialogResult.OK)
                {
                    this.canceled = true;
                    break;
                }

                // Sync customers from selection
                try
                {
                    if (_abandonSync)
                    {
                        break;
                    }

                    // Sync _addQueue selection
                    AddStatusMessage(MessageType.Normal, "Adding new customers to QuickBooks");
                    foreach (var customer in _addQueue)
                    {
                        _customerSyncing = customer;
                        function = "adding";
                        AddCustomerToQB(customer);
                    }
                    // Sync _updateQueue selection
                    AddStatusMessage(MessageType.Normal, "Mapping DWOS customers to QuickBooks");
                    foreach (KeyValuePair<CustomersDataset.CustomerRow, string> kvp in _updateQueue)
                    {
                        _customerSyncing = kvp.Key;
                        using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
                        {
                            ta.Update(kvp.Key);
                        }

                        // Update the customer
                        function = "updating";
                        UpdateCustomerInQB(kvp.Key, kvp.Value);
                    }
                    // Sync _updateExistingQueue selection
                    AddStatusMessage(MessageType.Normal, "Updating existing customers in QuickBooks");
                    foreach (KeyValuePair<CustomersDataset.CustomerRow, string> kvp in _updateExistingQueue)
                    {
                        _customerSyncing = kvp.Key;
                        // Update the customer
                        function = "updating";
                        UpdateCustomerInQB(kvp.Key, kvp.Value);
                    }
                }
                catch (Exception ex)
                {
                    _syncErrorCount++;

                    _log.Warn(ex, "Error during customer sync to QuickBooks.");

                    this.AddStatusMessage(
                        MessageType.Error,
                        "Error " + function + " customer, " + _customerSyncing + ", to QuickBooks, " + ex.Message
                        + ".");

                    CheckSyncCount();
                }
                finally
                {
                    // Refresh the queues
                    _addQueue.Clear();
                    _updateQueue.Clear();
                    _updateExistingQueue.Clear();
                    
                    _syncing = false;
                }
            }

        }

        private List<Customer> GetQuickBooksCustomers()
        {
            var qbCustomerList = new List<Customer>();

            _qbSearch.Reset();
            _qbSearch.QueryType = ObjsearchQueryTypes.qtCustomerSearch;
            _qbSearch.Search();

            foreach(var result in _qbSearch.Results)
            {
                // Load customer data from search result aggregate (XML)
                var customer = new Customer
                {
                    QBResponseAggregate = result.Aggregate
                };

                // only syncing active, valid customers right now.
                // if the customer has a parent name associated (or has a colon in its name), it is a 'job' in QB
                // and is a child of another customer so skip it

                var isValidCustomer = !(string.IsNullOrEmpty(customer.CustomerName)
                    && string.IsNullOrEmpty(customer.CompanyName));

                var includeCustomer = customer.IsActive
                    && string.IsNullOrEmpty(customer.ParentName)
                    && !customer.CustomerName.Contains(":");

                if (isValidCustomer && includeCustomer)
                {
                    qbCustomerList.Add(customer);
                }

                customer.CloseQBConnection();
            }

            return qbCustomerList;
        }

        /// <summary>Checks the address fields for max length in QuickBooks.</summary>
        /// <param name="customerRow">The customer row.</param>
        /// <param name="msg">The warning MSG.</param>
        /// <param name="addr1">The addr1.</param>
        /// <param name="addr2">The addr2.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        private static bool CheckAddress(CustomersDataset.CustomerRow customerRow, out string msg, out string addr1, out string addr2)
        {
            addr1 = null;
            addr2 = null;
            msg = null;

            if (!customerRow.IsAddress1Null() && customerRow.Address1.Length > MAX_QUICKBOOKS_LENGTH)
            {
                addr1 = customerRow.Address1.Left(41); /// Truncate to the max length in QB of 41
                msg = $"Truncating {customerRow.Name} - Address 1 is longer than maximum allowed in QuickBooks ({MAX_QUICKBOOKS_LENGTH} characters).";
            }
            if (!customerRow.IsAddress2Null() && customerRow.Address2.Length > MAX_QUICKBOOKS_LENGTH)
            {
                addr2 = customerRow.Address2.Left(41);/// Truncate to the max length in QB of 41
                msg = $"{msg}\n Truncating {customerRow.Name} - Address 2 is longer than maximum allowed in QuickBooks ({MAX_QUICKBOOKS_LENGTH} characters).";
            }
            else
            {
                msg = null;
            }

            return !string.IsNullOrEmpty(msg);
        }

        private static bool SkipCustomerSync(CustomersDataset.CustomerRow customerRow, out string msg)
        {
            if (customerRow.Name.Contains(":"))
            {
                msg = $"Skipping {customerRow.Name} - cannot import customer jobs at this time.";
            }
            else if (!customerRow.IsCityNull() && customerRow.City.Length > MAX_QUICKBOOKS_LENGTH)
            {
                 msg = $"Skipping {customerRow.Name} - City is longer than maximum allowed in QuickBooks ({MAX_QUICKBOOKS_LENGTH} characters).";
            }
            else if (!customerRow.IsStateNull() && customerRow.State.Length > MAX_QUICKBOOKS_LENGTH)
            {
                  msg = $"Skipping {customerRow.Name} - State is longer than maximum allowed in QuickBooks ({MAX_QUICKBOOKS_LENGTH} characters).";
            }
            else if (!customerRow.IsZipNull() && customerRow.Zip.Length > MAX_QUICKBOOKS_LENGTH)
            {
                   msg = $"Skipping {customerRow.Name} - Zip is longer than maximum allowed in QuickBooks ({MAX_QUICKBOOKS_LENGTH} characters).";
            }
            else
            {
                msg = null;
            }

            return !string.IsNullOrEmpty(msg);
        }

        /// <summary>
        /// Adds the customer to QuickBooks.
        /// </summary>
        /// <param name="customerRow">The customer row.</param>
        private void AddCustomerToQB(CustomersDataset.CustomerRow customerRow)
        {
            this.AddStatusMessage(MessageType.Normal, "Beginning sync for new QuickBooks customer " + customerRow.Name + ".");

            _qbCustomer.Reset();
            FillQBCustomerInfo(customerRow, true);
            _qbCustomer.Add();

            this.AddStatusMessage(MessageType.Success, customerRow.Name + " sync successful.");
        }

        /// <summary>
        /// Adds the customer to DWOS.
        /// </summary>
        /// <param name="customer">The QuickBooks customer.</param>
        private void AddCustomerToDWOS(Customer customer)
        {
            var dwosCustomerName = string.IsNullOrEmpty(customer.CompanyName)
                ? customer.CustomerName
                : customer.CompanyName;

            AddStatusMessage(MessageType.Normal,
                $"Beginning sync for new DWOS customer {dwosCustomerName}.");

            // new customer from QB, add to DWOS
            var newCustomer = _dsCustomer.Customer.NewCustomerRow();
            newCustomer.Name = dwosCustomerName;
            newCustomer.PaymentTerms = customer.TermsName;
            newCustomer.PrintInvoice = false;
            newCustomer.EmailInvoice = true;
            newCustomer.LeadTime = DWOS.Data.ApplicationSettings.Current.OrderLeadTime;
            newCustomer.OrderReview = true;
            newCustomer.OrderPriority = "Normal";
            newCustomer.Active = true;
            newCustomer.InvoiceLevelID = "Default";

            var qbAddress = new Address { Aggregate = customer.BillingAddress };
            newCustomer.Address1 = GetDWOSLine1(customer, qbAddress);
            newCustomer.Address2 = GetDWOSLine2(customer, qbAddress);
            newCustomer.City = qbAddress.City;
            newCustomer.State = qbAddress.State;
            newCustomer.Zip = qbAddress.PostalCode;
            newCustomer.CountryID = GetCountryId(qbAddress.State);
            _dsCustomer.Customer.AddCustomerRow(newCustomer);

            // Add customer address
            var newAddress = _dsCustomer.CustomerAddress.NewCustomerAddressRow();
            newAddress.CustomerRow = newCustomer;
            newAddress.Name = newCustomer.Name;
            newAddress.IsDefault = true;

            if (!newCustomer.IsAddress1Null())
            {
                newAddress.Address1 = newCustomer.Address1;
            }

            if (!newCustomer.IsAddress2Null())
            {
                newAddress.Address2 = newCustomer.Address2;
            }

            if (!newCustomer.IsCityNull())
            {
                newAddress.City = newCustomer.City;
            }

            if (!newCustomer.IsStateNull())
            {
                newAddress.State = newCustomer.State;
                newAddress.RequireRepairStatement = AddressUtilities.IsInUnitedStates(newCustomer.State) &&
                    !AddressUtilities.IsInUnitedStates(ApplicationSettings.Current.CompanyState);
            }
            else
            {
                newAddress.RequireRepairStatement = false;
            }

            if (!newCustomer.IsZipNull())
            {
                newAddress.Zip = newCustomer.Zip;
            }

            newAddress.CountryID = newCustomer.CountryID;

            _dsCustomer.CustomerAddress.AddCustomerAddressRow(newAddress);

            using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
            {
                ta.Update(_dsCustomer.Customer);
            }

            using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.CustomerAddressTableAdapter())
            {
                ta.Update(_dsCustomer.CustomerAddress);
            }

            AddStatusMessage(MessageType.Normal,
                $"Adding contact, {customer.ContactName}, for customer {dwosCustomerName}.");

            var dtCustomerContacts = _dsCustomer.Contact;
            var newContact = dtCustomerContacts.NewContactRow();
            newContact.Name = customer.ContactName;

            var phoneNumber = this.FormatQBPhoneNumberForDWOS(customer.Phone);

            newContact.PhoneNumber = phoneNumber;
            newContact.EmailAddress = customer.Email;
            newContact.CustomerID = newCustomer.CustomerID;
            newContact.PortalAuthorized = false;
            newContact.Active = true;
            newContact.PrimaryContact = true; // Mark as primary since this is a new customer and it will only have the one contact
            _dsCustomer.Contact.AddContactRow(newContact);

            using (var ta = new DWOS.Data.Datasets.CustomersDatasetTableAdapters.ContactTableAdapter())
                ta.Update(_dsCustomer.Contact);

            AddStatusMessage(MessageType.Success,
                $"{dwosCustomerName} sync successful.");
        }

        /// <summary>
        /// Updates the customer in qb.
        /// </summary>
        /// <param name="customerRow">The customer row.</param>
        /// <param name="quickBooksName">The name of the customer in QuickBooks</param>
        private void UpdateCustomerInQB(CustomersDataset.CustomerRow customerRow, string quickBooksName)
        {
            this.AddStatusMessage(MessageType.Normal, "Beginning sync for existing QuickBooks customer " + customerRow.Name + ".");

            _qbCustomer.Reset();
            _qbCustomer.GetByName(quickBooksName);
            FillQBCustomerInfo(customerRow, false);
            _qbCustomer.Update();

            this.AddStatusMessage(MessageType.Success, customerRow.Name + " sync successful.");
        }

        /// <summary>
        /// Updates the customer in DWOS.
        /// </summary>
        /// <param name="customer">The QuickBooks customer.</param>
        private void UpdateCustomerInDWOS(Customer customer)
        {
            var dwosCustomer = _dsCustomer.Customer
                .FirstOrDefault(c => c.Name == customer.CompanyName
                    || (!c.IsAccountingIDNull() && !string.IsNullOrEmpty(c.AccountingID) && c.AccountingID == customer.CompanyName)
                    || c.Name == customer.CustomerName
                    || (!c.IsAccountingIDNull() && !string.IsNullOrEmpty(c.AccountingID) && c.AccountingID == customer.CustomerName));

            if (dwosCustomer == null)
            {
                return;
            }

            this.AddStatusMessage(MessageType.Normal, "Beginning sync for existing QuickBooks customer " + customer.CompanyName + ".");

            dwosCustomer.PaymentTerms = customer.TermsName;

            var addr = new Address { Aggregate = customer.BillingAddress };
            dwosCustomer.Address1 = GetDWOSLine1(customer, addr);
            dwosCustomer.Address2 = GetDWOSLine2(customer, addr);
            dwosCustomer.City = addr.City;
            dwosCustomer.State = addr.State;
            dwosCustomer.Zip = addr.PostalCode;
            dwosCustomer.CountryID = GetCountryId(addr.State);

            using (var ta = new DWOS.Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
                ta.Update(_dsCustomer.Customer);

            var dtCustomerContacts = _dsCustomer.Contact;
            using (var taContacts = new DWOS.Data.Datasets.CustomersDatasetTableAdapters.ContactTableAdapter())
                taContacts.FillBy(dtCustomerContacts, dwosCustomer.CustomerID);

            var contactAdded = false;
            if (dtCustomerContacts.Count > 0)
            {
                var customerContact = dtCustomerContacts.FirstOrDefault(contact => !contact.IsNameNull() && contact.Name == customer.ContactName);
                if (customerContact != null)
                {
                    this.AddStatusMessage(MessageType.Normal, "Updating a contact, " + customer.ContactName + ", for customer " + customer.CompanyName + ".");

                    customerContact.Name = customer.ContactName;

                    var phoneNumber = FormatQBPhoneNumberForDWOS(customer.Phone);
                    customerContact.PhoneNumber = phoneNumber;
                    customerContact.EmailAddress = customer.Email;
                    customerContact.CustomerID = dwosCustomer.CustomerID;
                    customerContact.PortalAuthorized = false;
                    customerContact.Active = true;
                    contactAdded = true;
                }
            }

            // If the contact was not added and there is one in QB add it here
            if (!contactAdded && !string.IsNullOrEmpty(customer.ContactName))
            {
                this.AddStatusMessage(MessageType.Normal, "Adding a new contact, " + customer.ContactName + ", to the customer " + customer.CompanyName + ".");

                var newContact = dtCustomerContacts.NewContactRow();
                newContact.Name = customer.ContactName;

                var phoneNumber = FormatQBPhoneNumberForDWOS(customer.Phone);
                newContact.PhoneNumber = phoneNumber;
                newContact.EmailAddress = customer.Email;
                newContact.CustomerID = dwosCustomer.CustomerID;
                newContact.PortalAuthorized = false;
                newContact.Active = true;
                _dsCustomer.Contact.AddContactRow(newContact);
            }

            using (var ta = new DWOS.Data.Datasets.CustomersDatasetTableAdapters.ContactTableAdapter())
                ta.Update(_dsCustomer.Contact);

            this.AddStatusMessage(MessageType.Success, customer.CompanyName + " sync successful.");
        }

        private bool DoesCustomerExistInQB(CustomersDataset.CustomerRow customer, out string quickBooksName)
        {
            if (DoesCustomerExistInQB(customer.Name))
            {
                quickBooksName = customer.Name;
                return true;
            }

            if (!customer.IsAccountingIDNull() && DoesCustomerExistInQB(customer.AccountingID))
            {
                quickBooksName = customer.AccountingID;
                return true;
            }

            quickBooksName = null;
            return false;
        }

        /// <summary>
        /// Does the customer exist.
        /// </summary>
        /// <param name="customerName">Name of the customer.</param>
        /// <returns>TRUE if customer exists</returns>
        private bool DoesCustomerExistInQB(string customerName)
        {
            try
            {
                _qbCustomer.Reset();
                _qbCustomer.GetByName(customerName);

                //Reset Values
                ClearCustomCustomerField(_qbCustomer, ApplicationSettings.Current.InvoiceCustomerWOField);
                ClearCustomCustomerField(_qbCustomer, ApplicationSettings.Current.InvoiceTrackingNumberField);

                return true;
            }
            catch (InQBCustomerException customerException)
            {
                _log.Info(customerException, $"Customer does not exist in QuickBooks {customerName}");

                return false;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error locating customer in QuickBooks");

                return false;
            }
        }

        /// <summary>
        /// Does the customer exist.
        /// </summary>
        /// <param name="customer">The QuickBooks customer.</param>
        /// <returns>TRUE if customer exists</returns>
        private bool DoesCustomerExistInDWOS(Customer customer)
        {
            return _dsCustomer
                .Customer
                .Any(c => c.Name == customer.CompanyName
                    || (!c.IsAccountingIDNull() && !string.IsNullOrEmpty(c.AccountingID) && c.AccountingID == customer.CompanyName)
                    || c.Name == customer.CustomerName
                    || (!c.IsAccountingIDNull() && !string.IsNullOrEmpty(c.AccountingID) && c.AccountingID == customer.CustomerName));
        }

        /// <summary>
        /// Adds the QuickBook customers to DWOS.
        /// </summary>
        /// <param name="qbCustomers">The QuickBooks customers.</param>
        private void SyncQBCustomersWithDWOS(List<Customer> qbCustomers)
        {
            this.SetupProgressBar(qbCustomers.Count);
            foreach (var customer in qbCustomers)
            {
                try
                {
                    if (_abandonSync)
                    {
                        break;
                    }

                    if (DoesCustomerExistInDWOS(customer))
                    {
                        UpdateCustomerInDWOS(customer);
                    }
                    else
                    {
                        // Prompt user to sync customer
                        var customerDialog = new SelectDwosCustomer();
                        customerDialog.LoadData(customer, _dsCustomer.Customer);

                        if (customerDialog.ShowDialog() == DialogResult.OK)
                        {
                            if (customerDialog.SelectedCustomer == null)
                            {
                                // Add new customer
                                AddCustomerToDWOS(customer);
                            }
                            else
                            {
                                // Set the customer's Account ID - will be saved during update
                                customerDialog.SelectedCustomer.AccountingID = string.IsNullOrEmpty(customer.CompanyName)
                                    ? customer.CustomerName
                                    : customer.CompanyName;

                                // Update the Customer
                                UpdateCustomerInDWOS(customer);
                            }
                        }
                        else
                        {
                            // Skip this customer
                            AddStatusMessage(MessageType.Warning, $"Skipped {customer.CompanyName} - user declined to sync user.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _syncErrorCount++;

                    _log.Warn(ex, "Error during customer sync to DWOS.");

                    this.AddStatusMessage(MessageType.Error, "Error adding customer, " + customer.CompanyName + ", to DWOS, " + ex.Message + ".");
                    CheckSyncCount();
                }
                finally
                {
                    this.UpdateProgressBar(1);
                }
            }
        }

        /// <summary>
        /// Fills the QuickBooks customer information.
        /// </summary>
        /// <param name="customerRow">The customer row.</param>
        /// <param name="isNew">Indicates if the customer is new in QuickBooks.</param>
        private void FillQBCustomerInfo(CustomersDataset.CustomerRow customerRow, bool isNew)
        {
            if (isNew)
            {
                var formattedName = customerRow.Name.TrimToMaxLength(MAX_QUICKBOOKS_LENGTH);
                _qbCustomer.CustomerName = formattedName;
                _qbCustomer.CompanyName = formattedName;

                if (formattedName.Length != customerRow.Name.Length)
                {
                    // Had to truncate text for QuickBooks
                    customerRow.AccountingID = formattedName;
                    using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
                    {
                        ta.Update(customerRow);
                    }

                    AddStatusMessage(
                        MessageType.Warning,
                        $"{customerRow.Name} was renamed to {formattedName} for QuickBooks.");
                }
            }

            var paymentTerms = customerRow.IsPaymentTermsNull()
                ? string.Empty
                : customerRow.PaymentTerms;

            if (!string.IsNullOrEmpty(paymentTerms) && !TermsExistInQb(paymentTerms))
            {
                AccountingFieldValidation.ValidateTerms(paymentTerms);
            }
            _qbCustomer.TermsName = paymentTerms;

            var addr = new Address
            {
                Line1 = customerRow.Name.TrimToMaxLength(MAX_QUICKBOOKS_LENGTH),
                Line2 = customerRow.IsAddress1Null() ? string.Empty : customerRow.Address1,
                Line3 = customerRow.IsAddress2Null() ? string.Empty : customerRow.Address2,
                City = customerRow.IsCityNull() ? string.Empty : customerRow.City,
                State = customerRow.IsStateNull() ? string.Empty : customerRow.State,
                PostalCode = customerRow.IsZipNull() ? string.Empty : customerRow.Zip
            };

            _qbCustomer.BillingAddress = addr.Aggregate;

            var dtContacts = new CustomersDataset.ContactDataTable();
            using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.ContactTableAdapter())
            {
                ta.FillBy(dtContacts, customerRow.CustomerID);
            }

            if (dtContacts.Any())
            {
                var primaryContacts = dtContacts.Where(c => c.PrimaryContact == true).ToList();

                CustomersDataset.ContactRow primaryContact = null;
                primaryContact = primaryContacts.Count > 0 ? primaryContacts[0] : dtContacts.FirstOrDefault();

                if (primaryContact != null)
                {
                    if (primaryContact.Name.Length > MAX_QUICKBOOKS_PC_LENGTH)
                    {
                        _qbCustomer.ContactName = primaryContact.Name.TrimToMaxLength(MAX_QUICKBOOKS_PC_LENGTH);

                        AddStatusMessage(
                            MessageType.Warning,
                            $"{primaryContact.Name} was renamed to {_qbCustomer.ContactName} for QuickBooks.");
                    }
                    else if (primaryContact.IsNameNull())
                    {
                        _qbCustomer.ContactName = "";
                    }
                    else
                    {
                        _qbCustomer.ContactName = primaryContact.Name;
                    }

                    _qbCustomer.ContactName = primaryContact.Name;

                    var phoneNumber = primaryContact.IsPhoneNumberNull() ? "" : primaryContact.PhoneNumber;
                    if (!string.IsNullOrEmpty(phoneNumber))
                    {
                        // need to massage the phone number to fit QB format (limited to 21 chars)
                        // DWOS formats phone numbers as (555) 555-5555 Ext.5555, for QB we will use 555-555-5555 x.5555
                        phoneNumber = phoneNumber
                            .Replace("Ext.", "x.")
                            .Replace("(", "")
                            .Replace(") ", "-");
                    }

                    _qbCustomer.Phone = phoneNumber;
                    _qbCustomer.Email = primaryContact.IsEmailAddressNull() ? "" : primaryContact.EmailAddress;
                }
            }
        }

        /// <summary>
        /// Formats the QuickBooks phone number for DWOS.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>Phone number formatted for DWOS</returns>
        private string FormatQBPhoneNumberForDWOS(string phoneNumber)
        {
            // Phone number is in 555-555-5555 x.5555 format, needs to be (555) 555-5555 Ext.5555
            var numbers = phoneNumber.Where(char.IsNumber).ToList();

            var phoneNum = "(";
            for (var i = 0; i < numbers.Count; i++)
            {
                phoneNum += numbers[i];
                if (i == 2) // end of area code
                    phoneNum += ") ";
                else if (i == 5) // end of first 3 digits in number
                    phoneNum += "-";
                else if (i == 9) // end of phone number, all that is left is the extension
                    phoneNum += " Ext.";
            }

            return phoneNum;
        }

        /// <summary>
        /// Synchronizes the fees and processing items.
        /// </summary>
        private void SyncDWOSFeesWithQB()
        {
            this.AddStatusMessage(MessageType.Normal, "Syncing order processing fees.");

            var dt = new OrdersDataSet.OrderFeeTypeDataTable();
            using (var taFees = new Data.Datasets.OrdersDataSetTableAdapters.OrderFeeTypeTableAdapter())
            {
                taFees.Fill(dt);
            }

            foreach (var orderFeeRow in dt)
            {
                if (this.DoesFeeExistInQB(orderFeeRow.InvoiceItemName))
                {
                    this.AddStatusMessage(MessageType.Success, orderFeeRow.InvoiceItemName + " fee item exists, " + orderFeeRow.OrderFeeTypeID + " can be added to invoices by setting the description and price fields.");
                }
                else                               
                {
                    string chargeType;
                    using (var taFeeType = new Data.Datasets.OrdersDataSetTableAdapters.OrderFeeTypeTableAdapter())
                    {
                        //Fixed or percentage
                        chargeType = taFeeType.GetFeeType(orderFeeRow.OrderFeeTypeID);
                    }

                    AccountingFieldValidation.ValidateListItemFee(orderFeeRow.InvoiceItemName, orderFeeRow.Price, chargeType);
                }
            }
        }

        /// <summary>
        /// Does the fee exist in QuickBooks.
        /// </summary>
        /// <param name="feeName">Name of the fee.</param>
        /// <returns>TRUE if the fee was found in QuickBooks</returns>
        private bool DoesFeeExistInQB(string feeName)
        {
            try
            {
                //use cache if already checked
                if (this._feeInQBCache.ContainsKey(feeName))
                    return this._feeInQBCache[feeName];
                else
                {
                    _qbItem.Reset();
                    _qbItem.GetByName(feeName);

                    //cache the results
                    this._feeInQBCache.Add(feeName, true);

                    return !string.IsNullOrEmpty(_qbItem.ItemName);
                }
            }
            catch (InQBItemException itemException)
            {
                _log.Info(itemException, $"Fee does not exist in QuickBooks {feeName}");

                return false;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error locating fee in QuickBooks.");

                return false;
            }
        }

        private bool TermsExistInQb(string termsName)
        {
            if (string.IsNullOrEmpty(termsName))
            {
                return false;
            }

            // Check standard terms
            _qbLists.Reset();
            _qbLists.ListType = QblistsListTypes.ltStandardTerms;
            try
            {
                _qbLists.GetByName(termsName);
                return true;
            }
            catch (InQBQblistsException exc)
            {
                _log.Info(exc, "Standard term does not exist in QuickBooks {0}", termsName);
            }

            // Check date driven terms
            _qbLists.Reset();
            _qbLists.ListType = QblistsListTypes.ltDateDrivenTerms;

            try
            {
                _qbLists.GetByName(termsName);
                return true;
            }
            catch (InQBQblistsException exc)
            {
                _log.Warn(exc, "Terms do not exist in QuickBooks {0}", termsName);
            }

            return false;
        }

        /// <summary>
        /// Adds the status message.
        /// </summary>
        /// <param name="msgType">Type of the MSG.</param>
        /// <param name="message">The message.</param>
        private void AddStatusMessage(MessageType msgType, string message)
        {
            var str = message;

            TextBox textBox;

            if (msgType == MessageType.Error)
            {
                textBox = txtErrors;
            }
            else if (msgType == MessageType.Warning)
            {
                textBox = txtWarnings;
            }
            else //msgType == MessageType.Normal || msgType == MessageType.Success
            {
                textBox = txtMessages;
            }

            // Track the errors and warnings
            if (_results.Keys.Contains(msgType))
            {
                var list = _results[msgType];
                list.Add(message);
                _results[msgType] = list;
            }
            else
            {
                _results.Add(msgType, new List<string> { message });
            }

            textBox.AppendText("--" + message + Environment.NewLine);
        }

        private void CheckSyncCount()
        {
            if (_syncErrorCount >= MAX_SYNC_ERRORS)
            {
                AddStatusMessage(MessageType.Warning,
                    $"--A number ({_syncErrorCount}) of errors have been encountered, please resolve before attempting to sync again.{Environment.NewLine}");

                ultraTabControl1.Tabs["warning"].Selected = true;
                _abandonSync = true;
            }
        }

        /// <summary>
        /// Clears the custom customer field. This is required because it cannot be set for the invoice if it is already set at the customer level.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <param name="fieldName">Name of the field.</param>
        private void ClearCustomCustomerField(Customer customer, string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                return;
            }

            try
            {
                var customValue = customer.GetCustomField(fieldName);

                if (!string.IsNullOrEmpty(customValue))
                {
                    _log.Info("Clearing {0} from {1} for customer '{2}'.", fieldName, customValue, customer.CustomerName);
                    customer.SetCustomField(fieldName, string.Empty);
                }
            }
            catch (NullReferenceException exc)
            {
                // With QB2012, this clear operation might fail.
                _log.Warn(exc, "Error clearing QB field {0}", fieldName);
            }
        }

        /// <summary>
        /// Setups the progress bar.
        /// </summary>
        /// <param name="maxValue">The maximum value.</param>
        private void SetupProgressBar(int maxValue)
        {
            if (InvokeRequired)
            {
                Invoke((Action<int>)SetupProgressBar, maxValue);
                return;
            }

            pgbSyncProgress.Minimum = 0;
            pgbSyncProgress.Maximum = maxValue;
            pgbSyncProgress.Value = 0;
        }

        /// <summary>
        /// Updates the progress bar.
        /// </summary>
        /// <param name="increment">The increment.</param>
        private void UpdateProgressBar(int increment)
        {
            if (InvokeRequired)
            {
                Invoke((Action<int>)UpdateProgressBar, increment);
                return;
            }

            // Check for null control - workaround for apparent
            // NullReferenceException that causes #29947.
            if (pgbSyncProgress == null)
            {
                _log.Error($"{nameof(pgbSyncProgress)} should not be null.");

                return;
            }

            pgbSyncProgress.IncrementValue(increment);

            if (pgbSyncProgress.Value >= pgbSyncProgress.Maximum || _abandonSync)
            {
                IsValid = true;
                OnValidStateChanged(this);
            }
        }

        private string GetDWOSLine1(Customer customer, Address addr)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            if (addr == null)
            {
                throw new ArgumentNullException(nameof(addr));
            }

            return GetDWOSAddressLines(customer, addr)
                .FirstOrDefault();
        }

        private string GetDWOSLine2(Customer customer, Address addr)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            if (addr == null)
            {
                throw new ArgumentNullException(nameof(addr));
            }

            return GetDWOSAddressLines(customer, addr)
                .Skip(1)
                .FirstOrDefault();
        }

        private static List<string> GetDWOSAddressLines(Customer customer, Address addr)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            if (addr == null)
            {
                throw new ArgumentNullException(nameof(addr));
            }

            var customerName = customer.CustomerName.Trim().ToUpper();
            var companyName = customer.CompanyName.Trim().ToUpper();
            var contactName = customer.ContactName.Trim().ToUpper();

            var sourceAddressLines = new List<string>()
            {
                addr.Line1,
                addr.Line2,
                addr.Line3,
                addr.Line4,
                addr.Line5
            };

            var destAddressLines = new List<string>();

            foreach (var sourceLine in sourceAddressLines)
            {
                var normalizedLine = sourceLine.Trim().ToUpper();

                if (normalizedLine != customerName && normalizedLine != companyName && normalizedLine != contactName)
                {
                    destAddressLines.Add(sourceLine);
                }
            }

            return destAddressLines;
        }

        private int GetCountryId(string state)
        {
            if (string.IsNullOrWhiteSpace(state) || state.Length != 2)
            {
                return ApplicationSettings.Current.CompanyCountry;
            }

            return AddressUtilities.GetCountryId(state);
        }

        private void DisposeCodeBehind(bool disposing)
        {
            if (disposing)
            {
                // Cleanup QB connections
                try
                {
                    _qbCustomer?.CloseQBConnection();
                    _qbLists?.CloseQBConnection();
                    _qbItem?.CloseQBConnection();
                    _qbSearch?.CloseQBConnection();
                }
                catch (InQBCustomerException exc)
                {
                    _log.Warn(exc, "Unable to disconnect from QuickBooks.");
                }

                _qbCustomer?.Dispose();
                _qbLists?.Dispose();
                _qbItem?.Dispose();
                _qbSearch?.Dispose();

                // Cleanup DWOS data
                _dsCustomer.Dispose();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the Click event of the btnSync control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnSync_Click(object sender, EventArgs e)
        {
            try
            {
                btnSync.Enabled = false;

                // Clear results so there are not duplicates
                _syncErrorCount = 0;
                _results.Clear();
                txtMessages.Clear();
                txtErrors.Clear();
                txtWarnings.Clear();

                this.SyncCustomers();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error during accounting sync.");
            }
            finally
            {
                btnSync.Enabled = true;
            }
        }

        #endregion

        #region IWizardPanel Members

        public string Title => "Sync Progress";

        public string SubTitle => "Track the progress as the sync is completed.";

        public Action<IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller) { }

        public Control PanelControl => this;

        public bool IsValid { get; private set; }

        public void OnMoveTo()
        {
            // Reset progress bar
            pgbSyncProgress.Value = 0;
            // Make sure the error count is zeroed out
            _syncErrorCount = 0;
        }

        public void OnMoveFrom()
        {
            QBSyncController.SyncResults = _results;
        }

        public void OnFinished()
        {
            // Do nothing
        }

        #endregion
    }
}
