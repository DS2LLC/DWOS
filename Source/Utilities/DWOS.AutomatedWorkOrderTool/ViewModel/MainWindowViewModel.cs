using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DWOS.AutomatedWorkOrderTool.Messages;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.AutomatedWorkOrderTool.Model.AwotDataSetTableAdapters;
using DWOS.AutomatedWorkOrderTool.Services;
using DWOS.Shared;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public UnsavedDataService UnsavedData { get; }

        #region Fields

        private object _selectedItem;

        #endregion

        #region Properties

        public IUserManager UserManager { get; }
        public ICustomerManager CustomerManager { get; }

        public IDataManager DataManager { get; }

#if NIGHTLY
        public string TitleText => 
            $@"DWOS Automated Work Order Tool { About.ApplicationVersion} - {About.ApplicationReleaseDate:d} Nightly";
#else
        public string TitleText =>
            $@"DWOS Automated Work Order Tool { About.ApplicationVersion}";
#endif

        public bool LoggedIn => UserManager.CurrentUser != null;

        public bool CanEditMasterList => UserManager.CurrentUser != null &&
            UserManager.CurrentUser.IsInRole(DwosUser.MASTER_LIST_ROLE);

        public bool CanAddShippingManifest => UserManager.CurrentUser != null &&
            UserManager.CurrentUser.IsInRole(DwosUser.SHIPPING_MANIFEST_ROLE);

        public bool ShowLoggedInPrompt => UserManager.CurrentUser == null;

        public ICommand LogOut { get; }

        public ICommand AddCustomer { get; }

        public ICommand AddManufacturer { get; }

        public ObservableCollection<CustomerViewModel> Customers { get; } =
            new ObservableCollection<CustomerViewModel>();

        public object SelectedItem
        {
            get => _selectedItem;
            set => Set(nameof(SelectedItem), ref _selectedItem, value);
        }

        public ICommand ShowMasterListDialog { get; }

        public ICommand ShowShippingManifestDialog { get; }

        public bool HasUnsavedChanges =>
            _selectedItem is OspFormatViewModel vm && UnsavedData.HasUnsavedChanges(vm);

        #endregion

        #region Methods

        public MainWindowViewModel(IUserManager userManager, ICustomerManager customerManager, IDataManager dataManager, UnsavedDataService unsavedData, IMessenger messenger)
            : base(messenger)
        {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            CustomerManager = customerManager ?? throw new ArgumentNullException(nameof(customerManager));
            DataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
            UnsavedData = unsavedData ?? throw new ArgumentNullException(nameof(unsavedData));
            UserManager.UserChanged += UserManagerOnUserChanged;

            LogOut = new RelayCommand(DoLogout, CanLogout);
            AddCustomer = new RelayCommand(ShowAddCustomerPrompt, CanAddCustomer);
            AddManufacturer = new RelayCommand(ShowAddManufacturerPrompt, CanAddManufacturer);
            ShowMasterListDialog = new RelayCommand(ShowMasterListPrompt, CanImportMasterList);
            ShowShippingManifestDialog = new RelayCommand(ShowShippingManifestPrompt, CanImportShippingManifest);

            MessengerInstance.Register<AddCustomerMessage>(this, HandleAddCustomerMessage);
            MessengerInstance.Register<AddOspFormatMessage>(this, HandleAddOspFormatMessage);
        }

        public void ClearUnsavedChanges()
        {
            if (!(_selectedItem is OspFormatViewModel vm))
            {
                return;
            }

            UnsavedData.Clear(vm);
        }

        private void DoLogout()
        {
            UserManager.LogOut();
        }

        private void ShowAddCustomerPrompt() =>
            MessengerInstance.Send(new ShowDialogMessage(ShowDialogMessage.DialogType.AddCustomer));

        private void ShowAddManufacturerPrompt() =>
            MessengerInstance.Send(new ShowDialogMessage(ShowDialogMessage.DialogType.AddManufacturer));

        private void ShowMasterListPrompt() =>
            MessengerInstance.Send(new ShowDialogMessage(ShowDialogMessage.DialogType.ImportMasterList));

        private void ShowShippingManifestPrompt() =>
            MessengerInstance.Send(new ShowDialogMessage(ShowDialogMessage.DialogType.ImportShippingManifest));

        private bool CanLogout() => LoggedIn;

        private bool CanAddCustomer() => CanEditMasterList;

        private bool CanAddManufacturer() => CanEditMasterList && _selectedItem is CustomerViewModel;

        private bool CanImportMasterList() => CanEditMasterList && _selectedItem is CustomerViewModel;

        private bool CanImportShippingManifest() => _selectedItem is CustomerViewModel;

        private void HandleAddCustomerMessage(AddCustomerMessage msg)
        {
            if (msg?.NewCustomer == null)
            {
                return;
            }

            CustomerManager.Add(msg.NewCustomer);
            Customers.Add(msg.NewCustomer);
        }

        private void HandleAddOspFormatMessage(AddOspFormatMessage msg)
        {
            try
            {
                var customer = msg?.Customer;
                var mfg = msg?.Manufacturer;

                if (customer == null || mfg == null)
                {
                    return;
                }

                var matchingCustomer = Customers.FirstOrDefault(c => c.Id == customer.Id);

                if (matchingCustomer != null)
                {
                    matchingCustomer.Formats.Add(OspFormatViewModel.From(DataManager.NewOspFormat(matchingCustomer.Id, mfg.Name)));
                }
            }
            catch (Exception exc)
            {
                // Failed to add OSP Format
                MessengerInstance.Send(new ErrorMessage(exc, "Failed to add a new OSP Format for the customer."));
            }
        }

        private void LoadCustomers()
        {
            Customers.Clear();
            CustomerManager.Load();
            foreach (var customer in CustomerManager.CurrentCustomers)
            {
                Customers.Add(customer);
            }
        }

        #endregion

        #region Events

        private void UserManagerOnUserChanged(object sender, EventArgs eventArgs)
        {
            try
            {
                RaisePropertyChanged(nameof(LoggedIn));
                RaisePropertyChanged(nameof(ShowLoggedInPrompt));

                if (LoggedIn)
                {
                    LoadCustomers();
                }
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error logging in/loading orders."));
            }

        }

        #endregion
    }
}
