using System.Windows.Input;
using DWOS.DataArchiver.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.DataArchiver.ViewModel
{
    public class SummaryViewModel : ViewModelBase
    {
        #region Fields

        private string _summaryText;

        #endregion

        #region Properties

        public string SummaryText
        {
            get => _summaryText;
            set => Set(nameof(SummaryText), ref _summaryText, value);
        }

        public GlobalOptionsProvider Options { get; }

        public ICommand Finish { get; }

        #endregion

        #region Methods

        public SummaryViewModel(IMessenger messenger, GlobalOptionsProvider options)
            : base(messenger)
        {
            Options = options;
            Finish = new RelayCommand(() => MessengerInstance.Send(CloseProgramMessage.Instance));
        }

        public void LoadData()
        {
            var totalFileSize = Options.BytesSaved;
            SummaryText = $"{totalFileSize.ToString("0.### MB")} of data was archived & removed from DWOS.";
        }

        #endregion
    }
}
