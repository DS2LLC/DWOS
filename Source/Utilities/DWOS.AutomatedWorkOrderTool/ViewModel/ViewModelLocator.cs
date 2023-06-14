using CommonServiceLocator;
using DWOS.AutomatedWorkOrderTool.Services;
using DWOS.Data;
using DWOS.Data.Order;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class ViewModelLocator
    {
        #region Properties

        public MainWindowViewModel MainWindow => ServiceLocator.Current.GetInstance<MainWindowViewModel>();

        public ConnectionWindowViewModel ConnectionWindow => ServiceLocator.Current.GetInstance<ConnectionWindowViewModel>();

        public UserViewModel User => ServiceLocator.Current.GetInstance<UserViewModel>();

        public LoginViewModel Login => ServiceLocator.Current.GetInstance<LoginViewModel>();

        public AddCustomerViewModel AddCustomer => ServiceLocator.Current.GetInstance<AddCustomerViewModel>();

        public AddManufacturerViewModel AddManufacturer => ServiceLocator.Current.GetInstance<AddManufacturerViewModel>();

        public OspFormatEditorViewModel OspFormat => ServiceLocator.Current.GetInstance<OspFormatEditorViewModel>();

        public AddOspFormatSectionViewModel AddOspFormatSection => ServiceLocator.Current.GetInstance<AddOspFormatSectionViewModel>();

        public AddOspFormatCodeMapViewModel AddOspFormatCodeMap => ServiceLocator.Current.GetInstance<AddOspFormatCodeMapViewModel>();

        public MasterListViewModel MasterList =>
            ServiceLocator.Current.GetInstance<MasterListViewModel>();

        public ShippingManifestViewModel ShippingManifest =>
            ServiceLocator.Current.GetInstance<ShippingManifestViewModel>();

        #endregion

        #region Methods

        public ViewModelLocator()
        {
            if (!SimpleIoc.Default.IsRegistered<IMessenger>())
            {
                ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

                // Dependencies
                var serverManager = new ServerManager();
                SimpleIoc.Default.Register(() => Messenger.Default);
                SimpleIoc.Default.Register<IServerManager>(() => serverManager);
                SimpleIoc.Default.Register<IUserManager, UserManager>();
                SimpleIoc.Default.Register<IDataManager, DataManager>();
                SimpleIoc.Default.Register<ISettingsProvider, SettingsProvider>();
                SimpleIoc.Default.Register<IFileService, FileService>();
                SimpleIoc.Default.Register<ICustomerManager, CustomerManager>();
                SimpleIoc.Default.Register<IDepartmentManager, DepartmentManager>();
                SimpleIoc.Default.Register<IDocumentManager, DocumentManager>();
                SimpleIoc.Default.Register<IPartManager, PartManager>();
                SimpleIoc.Default.Register<IPriceUnitPersistence, PriceUnitPersistence>();
                SimpleIoc.Default.Register<ILeadTimePersistence, LeadTimePersistence>();
                SimpleIoc.Default.Register<UnsavedDataService>();

                // View Models
                SimpleIoc.Default.Register<MainWindowViewModel>();
                SimpleIoc.Default.Register<ConnectionWindowViewModel>();
                SimpleIoc.Default.Register<UserViewModel>();
                SimpleIoc.Default.Register<LoginViewModel>();
                SimpleIoc.Default.Register<AddCustomerViewModel>();
                SimpleIoc.Default.Register<AddManufacturerViewModel>();
                SimpleIoc.Default.Register<OspFormatEditorViewModel>();
                SimpleIoc.Default.Register<AddOspFormatSectionViewModel>();
                SimpleIoc.Default.Register<AddOspFormatCodeMapViewModel>();

                SimpleIoc.Default.Register<MasterListViewModel>();
                SimpleIoc.Default.Register<ShippingManifestViewModel>();
            }
        }

        public void Cleanup()
        {
            MainWindow.Cleanup();
            ConnectionWindow.Cleanup();
            User.Cleanup();
            Login.Cleanup();
            AddCustomer.Cleanup();
            AddManufacturer.Cleanup();
            OspFormat.Cleanup();
            AddOspFormatSection.Cleanup();
            AddOspFormatCodeMap.Cleanup();

            MasterList.Cleanup();
        }

        #endregion
    }
}