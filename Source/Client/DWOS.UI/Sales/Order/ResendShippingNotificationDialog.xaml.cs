using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;

namespace DWOS.UI.Sales.Order
{
    /// <summary>
    /// Interaction logic for ResendShippingNotificationDialog.xaml
    /// </summary>
    public partial class ResendShippingNotificationDialog
    {
        #region Properties

        public IEnumerable<string> EmailAddresses =>
            ViewModel?.Contacts.Where(c => c.Include).Select(c => c.EmailAddress);

        private DialogContext ViewModel =>
            DataContext as DialogContext;

        #endregion

        #region Methods

        public ResendShippingNotificationDialog()
        {
            InitializeComponent();
            Icon = Properties.Resources.Shipping_32.ToWpfImage();
            DataContext = new DialogContext();
        }

        public void Load(OrdersDataSet.ShipmentPackageRow shipmentPackage)
        {
            ViewModel?.Load(shipmentPackage);
        }

        #endregion

        #region Events

        private void ResendShippingNotificationDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;

            if (vm != null)
            {
                vm.Completed += VmOnCompleted;
            }
        }

        private void ResendShippingNotificationDialog_OnUnloaded(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;

            if (vm != null)
            {
                vm.Completed -= VmOnCompleted;
            }
        }

        private void VmOnCompleted(object sender, EventArgs eventArgs)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error accepting resend shipping notification dialog.");
            }
        }

        #endregion

        #region DialogContext

        private class DialogContext : INotifyPropertyChanged
        {
            #region Fields

            public event EventHandler Completed;
            private int _packageNumber;

            #endregion

            #region Properties

            public int PackageNumber
            {
                get => _packageNumber;
                private set
                {
                    if (_packageNumber != value)
                    {
                        _packageNumber = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageNumber)));
                    }
                }
            }

            public ObservableCollection<Contact> Contacts { get; } =
                new ObservableCollection<Contact>();

            public ICommand Accept { get; }

            #endregion

            #region Methods

            public DialogContext()
            {
                Accept = new RelayCommand(() => Completed?.Invoke(this, EventArgs.Empty), () => Contacts.Any(c => c.Include));
            }

            public void Load(OrdersDataSet.ShipmentPackageRow shipmentPackage)
            {
                if (shipmentPackage == null)
                {
                    throw new ArgumentNullException(nameof(shipmentPackage));
                }

                PackageNumber = shipmentPackage.ShipmentPackageID;

                var notificationEmails = shipmentPackage.IsNotificationEmailsNull()
                    ? new HashSet<string>()
                    : new HashSet<string>(shipmentPackage.NotificationEmails.Split(","));

                var contacts = new List<Contact>();
                var contactRows = shipmentPackage.CustomerSummaryRow?.GetContactSummaryRows() ?? Enumerable.Empty<OrdersDataSet.ContactSummaryRow>();

                foreach (var contactRow in contactRows)
                {
                    if (!contactRow.Active || contactRow.IsEmailAddressNull())
                    {
                        continue;
                    }

                    if (contactRow.ShippingNotification || notificationEmails.Contains(contactRow.EmailAddress))
                    {
                        var includeEmail = notificationEmails.Remove(contactRow.EmailAddress);
                        var contact = Contact.From(contactRow);
                        contact.Include = includeEmail;
                        contacts.Add(contact);
                    }
                }

                // Add 'N/A emails at top'
                foreach (var addressWithoutContact in notificationEmails)
                {
                    var contact = Contact.From(addressWithoutContact);
                    contact.Include = true;
                    Contacts.Add(contact);
                }

                // Add emails for contacts
                foreach (var contact in contacts.OrderBy(c => c.Name))
                {
                    Contacts.Add(contact);
                }
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region Contact

        private class Contact : INotifyPropertyChanged
        {
            #region Fields

            private bool _include;

            #endregion

            #region Properties

            public string Name { get; }

            public string EmailAddress { get; }

            public bool Include
            {
                get => _include;
                set
                {
                    if (_include != value)
                    {
                        _include = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Include)));
                    }
                }
            }

            #endregion

            #region Methods

            private Contact(string name, string emailAddress)
            {
                Name = name;
                EmailAddress = emailAddress;
            }


            public static Contact From(string email)
            {
                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentNullException(nameof(email));
                }

                return new Contact("N/A", email);
            }

            public static Contact From(OrdersDataSet.ContactSummaryRow contactRow)
            {
                if (contactRow == null)
                {
                    throw new ArgumentNullException(nameof(contactRow));
                }

                return new Contact(
                    contactRow.IsNameNull() ? "N/A" : contactRow.Name,
                    contactRow.IsEmailAddressNull() ? string.Empty : contactRow.EmailAddress);
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion
    }
}
