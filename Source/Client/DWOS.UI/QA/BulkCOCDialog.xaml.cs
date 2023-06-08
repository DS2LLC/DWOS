using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.UI.Reports;
using DWOS.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DWOS.UI.QA
{
    /// <summary>
    /// Interaction logic for BulkCOCDialog.xaml
    /// </summary>
    public partial class BulkCOCDialog : Window
    {
        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="BulkCOCDialog"/> class.
        /// </summary>
        /// <param name="shippingPackageID"></param>
        public BulkCOCDialog(int shippingPackageID)
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="BulkCOCDialog"/> class.
        /// </summary>
        /// <param name="shipment"></param>
        public BulkCOCDialog(OrderShipmentDataSet.ShipmentPackageRow shipment)
        {
            if (shipment == null)
            {
                throw new ArgumentNullException(nameof(shipment));
            }

            InitializeComponent();
            Icon = Properties.Resources.Certificate_16.ToWpfImage();
            DataContext = new BulkCOCContext(shipment.ShipmentPackageID, shipment.CustomerID);
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            grdOrders.Records.ExpandAll(false);

            var context = DataContext as BulkCOCContext;

            if (context != null)
            {
                context.OnCompleted += Context_OnCompleted;
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            var context = DataContext as BulkCOCContext;

            if (context != null)
            {
                context.OnCompleted -= Context_OnCompleted;
            }
        }

        private void Context_OnCompleted(object sender, EventArgs e)
        {
            try
            {
                var context = DataContext as BulkCOCContext;

                if (context == null)
                {
                    DialogResult = false;
                    return;
                }

                // Add bulk certificate
                using (var taBulkCOC = new Data.Datasets.COCDatasetTableAdapters.BulkCOCTableAdapter())
                {
                    using (var dtBulkCOC = new COCDataset.BulkCOCDataTable())
                    {
                        var cocRow = dtBulkCOC.NewBulkCOCRow();
                        cocRow.DateCertified = context.DateCertified;
                        cocRow.QAUser = SecurityManager.Current.UserID;
                        cocRow.ShipmentPackageID = context.ShippingPackageID;
                        dtBulkCOC.AddBulkCOCRow(cocRow);
                        taBulkCOC.Update(dtBulkCOC);

                        context.BulkCOCID = cocRow.BulkCOCID;
                    }
                }

                using (var taBulkCOCOrder = new Data.Datasets.COCDatasetTableAdapters.BulkCOCOrderTableAdapter())
                {
                    using (var dtBulkCOCOrder = new COCDataset.BulkCOCOrderDataTable())
                    {
                        foreach (var order in context.Orders)
                        {
                            var cocOrderRow = dtBulkCOCOrder.NewBulkCOCOrderRow();
                            cocOrderRow.BulkCOCID = context.BulkCOCID;
                            cocOrderRow.OrderID = order.OrderID;
                            dtBulkCOCOrder.AddBulkCOCOrderRow(cocOrderRow);
                        }

                        taBulkCOCOrder.Update(dtBulkCOCOrder);
                    }
                }

                // Add notifications
                using (var taNotification = new Data.Datasets.COCDatasetTableAdapters.BulkCOCNotificationTableAdapter())
                {
                    using (var dtNotification = new COCDataset.BulkCOCNotificationDataTable())
                    {
                        foreach (var selectedContact in context.SelectedContacts.OfType<ContactData>())
                        {
                            var notificationRow = dtNotification.NewBulkCOCNotificationRow();
                            notificationRow.BulkCOCID = context.BulkCOCID;
                            notificationRow.ContactID = selectedContact.Contact.ContactID;
                            dtNotification.AddBulkCOCNotificationRow(notificationRow);
                        }

                        taNotification.Update(dtNotification);
                    }
                }

                // Display/print report
                var bulkReport = new BulkCOCReport(context.BulkCOCID);

                if (context.PrintBulkCOC)
                {
                    bulkReport.PrintReport(context.PrintCopies);
                }
                else
                {
                    bulkReport.DisplayReport();
                }

                DialogResult = true;
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error creating bulk certificate", exc);
                DialogResult = false;
            }
        }

        #endregion

        #region BulkCOCContext

        /// <summary>
        /// Represents context for <see cref="BulkCOCDialog"/>.
        /// </summary>
        private sealed class BulkCOCContext : INotifyPropertyChanged
        {
            #region Fields

            /// <summary>
            /// Occurs on dialog completion.
            /// </summary>
            public event EventHandler OnCompleted;

            private int _shippingPackageID;
            private int _bulkCOCID;
            private DateTime _dateCertified;
            private bool _printBulkCOC;
            private int _printCopies;
            private ObservableCollection<object> _selectedContacts;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the shipping package ID.
            /// </summary>
            public int ShippingPackageID
            {
                get
                {
                    return _shippingPackageID;
                }
            }

            /// <summary>
            /// Gets or sets the bulk COC's ID.
            /// </summary>
            public int BulkCOCID
            {
                get
                {
                    return _bulkCOCID;
                }
                set
                {
                    if (_bulkCOCID != value)
                    {
                        _bulkCOCID = value;
                        OnPropertyChanged(nameof(BulkCOCID));
                    }
                }
            }

            /// <summary>
            /// Gets the list of orders in the bulk COC.
            /// </summary>
            public List<OrderData> Orders
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the name of the quality inspector.
            /// </summary>
            public string QualityInspector
            {
                get
                {
                    return SecurityManager.Current.UserName;
                }
            }

            /// <summary>
            /// Gets or sets the certification date of the bulk COC.
            /// </summary>
            public DateTime DateCertified
            {
                get
                {
                    return _dateCertified;
                }

                set
                {
                    if (_dateCertified != value)
                    {
                        _dateCertified = value;
                        OnPropertyChanged(nameof(DateCertified));
                    }
                }
            }

            /// <summary>
            /// Gets or sets value indicating if DWOS should quick print the
            /// bulk COC.
            /// </summary>
            public bool PrintBulkCOC
            {
                get
                {
                    return _printBulkCOC;
                }

                set
                {
                    if (_printBulkCOC != value)
                    {
                        _printBulkCOC = value;
                        OnPropertyChanged(nameof(PrintBulkCOC));
                    }
                }
            }

            /// <summary>
            /// Gets or sets the number of copies to print.
            /// </summary>
            public int PrintCopies
            {
                get
                {
                    return _printCopies;
                }

                set
                {
                    _printCopies = value;
                }
            }

            public ObservableCollection<ContactData> Contacts { get; } = new ObservableCollection<ContactData>();

            /// <summary>
            /// Gets or sets the selected contacts for this instance.
            /// </summary>
            /// <remarks>
            /// The Infragistics control used to show this requires this
            /// property have a setter and be of this type.
            /// </remarks>
            public ObservableCollection<object> SelectedContacts
            {
                get
                {
                    return _selectedContacts;
                }
                set
                {
                    if (_selectedContacts != value)
                    {
                        _selectedContacts = value;
                        OnPropertyChanged(nameof(SelectedContacts));
                    }
                }
            }

            /// <summary>
            /// Gets the Complete command.
            /// </summary>
            public ICommand CompleteCommand
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="BulkCOCContext"/> class.
            /// </summary>
            /// <param name="shippingPackageID"></param>
            /// <param name="customerId"></param>
            public BulkCOCContext(int shippingPackageID, int customerId)
            {
                _shippingPackageID = shippingPackageID;
                _dateCertified = DateTime.Today;
                _bulkCOCID = -1;

                Orders = new List<OrderData>();

                // Load orders
                using (var taOrderSummary = new Data.Datasets.COCDatasetTableAdapters.OrderSummaryTableAdapter())
                {
                    using (var dtOrders = taOrderSummary.GetByShipment(_shippingPackageID))
                    {
                        foreach (var order in dtOrders)
                        {
                            Orders.Add(new OrderData(order));
                        }
                    }
                }

                // Load contacts
                var dtContact = new COCDataset.d_ContactDataTable();
                using (var taContact = new Data.Datasets.COCDatasetTableAdapters.d_ContactTableAdapter { ClearBeforeFill = false })
                {
                    taContact.FillActiveByCustomer(dtContact, customerId);

                    if (ApplicationSettings.Current.AllowAdditionalCustomersForContacts)
                    {
                        taContact.FillSecondaryContactsByCustomer(dtContact, customerId);
                    }
                }

                var contacts = dtContact
                    .Where(c => c.Active && !c.IsEmailAddressNull() && !string.IsNullOrEmpty(c.EmailAddress))
                    .ToList();

                var selectedContacts = new ObservableCollection<object>();

                foreach (var contact in contacts)
                {
                    var contactData = new ContactData(contact);
                    Contacts.Add(contactData);

                    if (contact.COCNotification)
                    {
                        selectedContacts.Add(contactData);
                    }
                }

                _selectedContacts = selectedContacts;

                CompleteCommand = new CompleteCommand(this);
            }

            /// <summary>
            /// Completes the dialog.
            /// </summary>
            public void Complete()
            {
                OnCompleted?.Invoke(this, new EventArgs());
            }

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region OrderData

        /// <summary>
        /// Represents an order.
        /// </summary>
        private sealed class OrderData
        {
            /// <summary>
            /// Gets the row for the order.
            /// </summary>
            public COCDataset.OrderSummaryRow Order
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the OrderID.
            /// </summary>
            public int OrderID
            {
                get
                {
                    return Order.OrderID;
                }
            }

            /// <summary>
            /// Gets the Customer WO.
            /// </summary>
            public string CustomerWO
            {
                get
                {
                    return Order.IsCustomerWONull() ?
                        string.Empty :
                        Order.CustomerWO;
                }
            }

            /// <summary>
            /// Gets the Purchase Order.
            /// </summary>
            public string PurchaseOrder
            {
                get
                {
                    return Order.IsPurchaseOrderNull() ?
                        string.Empty :
                        Order.PurchaseOrder;
                }
            }

            /// <summary>
            /// Gets the part's name.
            /// </summary>
            public string PartName
            {
                get
                {
                    return Order.PartName;
                }
            }

            /// <summary>
            /// Gets the part quantity.
            /// </summary>
            public int PartQuantity
            {
                get
                {
                    return Order.IsPartQuantityNull() ?
                        0 :
                        Order.PartQuantity;
                }
            }

            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="OrderData"/> class.
            /// </summary>
            /// <param name="order"></param>
            public OrderData(COCDataset.OrderSummaryRow order)
            {
                if (order == null)
                {
                    throw new ArgumentNullException(nameof(order));
                }

                Order = order;
            }
        }

        #endregion

        #region ContactData

        private class ContactData
        {
            public COCDataset.d_ContactRow Contact { get; }

            public string EmailAddress => Contact.IsEmailAddressNull() ? null : Contact.EmailAddress;

            public bool CocNotification => Contact.COCNotification;

            public ContactData(COCDataset.d_ContactRow contact)
            {
                Contact = contact;
            }
        }

        #endregion

        #region CompleteCommand

        /// <summary>
        /// Implementation of <see cref="BulkCOCContext.CompleteCommand"/>.
        /// </summary>
        private sealed class CompleteCommand : ICommand
        {
            #region Properties

            public BulkCOCContext Context
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public CompleteCommand(BulkCOCContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                Context = context;
            }

            #endregion

            #region ICommand Members

            // CanExecuteChanged is required for ICommand implementation
#pragma warning disable 67

            public event EventHandler CanExecuteChanged;

#pragma warning restore 67

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                Context.Complete();
            }

            #endregion
        }

        #endregion
    }
}
