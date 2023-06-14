using CommonServiceLocator;
using DWOS.DataArchiver.Services;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.DataArchiver.ViewModel
{
    public class ViewModelLocator
    {
        #region Properties

        public MainViewModel Main => 
                ServiceLocator.Current.GetInstance<MainViewModel>();

        public ConnectionWindowViewModel ConnectionWindow =>
            ServiceLocator.Current.GetInstance<ConnectionWindowViewModel>();

        public InitialOptionsViewModel InitialOptions =>
            ServiceLocator.Current.GetInstance<InitialOptionsViewModel>();

        public ConfirmationViewModel Confirmation =>
            ServiceLocator.Current.GetInstance<ConfirmationViewModel>();

        public ArchiveViewModel Archive =>
            ServiceLocator.Current.GetInstance<ArchiveViewModel>();

        public SummaryViewModel Summary =>
            ServiceLocator.Current.GetInstance<SummaryViewModel>();

        #endregion

        #region Methods

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (!SimpleIoc.Default.IsRegistered<IMessenger>())
            {
                SimpleIoc.Default.Register<GlobalOptionsProvider>();
                SimpleIoc.Default.Register<IFilePickerService, FilePickerService>();
                SimpleIoc.Default.Register(() => Messenger.Default);
                SimpleIoc.Default.Register<MainViewModel>();
                SimpleIoc.Default.Register<InitialOptionsViewModel>();
                SimpleIoc.Default.Register<ConfirmationViewModel>();
                SimpleIoc.Default.Register<ArchiveViewModel>();
                SimpleIoc.Default.Register<SummaryViewModel>();
                SimpleIoc.Default.Register<ConnectionWindowViewModel>();
            }
        }

        public void Cleanup()
        {
            Main.Cleanup();
            InitialOptions.Cleanup();
            Confirmation.Cleanup();
            Archive.Cleanup();
            Summary.Cleanup();
        }

        #endregion
    }
}