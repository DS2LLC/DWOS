using System.Windows.Input;
using DWOS.DataArchiver.Messages;
using DWOS.DataArchiver.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.DataArchiver.ViewModel
{
    public class InitialOptionsViewModel : ViewModelBase
    {
        #region Fields

        private int _orderCount = 100;
        private string _directory;

        #endregion

        #region Properties

        public IFilePickerService FilePicker { get; }

        public GlobalOptionsProvider Options { get; }

        public int OrderCount
        {
            get => _orderCount;
            set => Set(nameof(OrderCount), ref _orderCount, value);
        }

        public string Directory
        {
            set => Set(nameof(Directory), ref _directory, value);
        }

        public RelayCommand Next { get; }

        public ICommand BrowseFolder { get; }

        #endregion

        #region Methods

        public InitialOptionsViewModel(IMessenger messenger, GlobalOptionsProvider options, IFilePickerService filePicker)
            : base(messenger)
        {
            Options = options ?? throw new System.ArgumentNullException(nameof(options));
            FilePicker = filePicker;

            BrowseFolder = new RelayCommand(DoBrowseFolder);
            Next = new RelayCommand(GoNext, CanGoNext);
        }

        private void DoBrowseFolder()
        {
            var dir = FilePicker.GetDirectory();

            if (!string.IsNullOrEmpty(dir))
            {
                Directory = dir;
            }
        }

        private void GoNext()
        {
            Options.OrderCount = _orderCount;
            Options.Directory = _directory;

            MessengerInstance.Send(new MoveToStepMessage(Step.Confirmation));
        }

        private bool CanGoNext() => _orderCount > 0 && !string.IsNullOrEmpty(_directory);

        #endregion
    }
}
