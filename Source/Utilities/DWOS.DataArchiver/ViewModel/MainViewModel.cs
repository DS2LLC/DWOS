using DWOS.DataArchiver.Messages;
using DWOS.Shared;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.DataArchiver.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        private Step _currentStep;

        #endregion

        #region Properties

        public Step CurrentStep
        {
            get => _currentStep;
            set
            {
                if (Set(nameof(CurrentStep), ref _currentStep, value))
                {
                    // Raise property change handlers for 'show step' members
                    RaisePropertyChanged(nameof(ShowInitialOptions));
                    RaisePropertyChanged(nameof(ShowConfirmation));
                    RaisePropertyChanged(nameof(ShowArchive));
                    RaisePropertyChanged(nameof(ShowSummary));
                }
            }
        }

        public bool ShowInitialOptions => _currentStep == Step.InitialOptions;

        public bool ShowConfirmation => _currentStep == Step.Confirmation;

        public bool ShowArchive => _currentStep == Step.Archive;

        public bool ShowSummary => _currentStep == Step.Summary;

        public string TitleText =>
            $@"DWOS Data Archiver { About.ApplicationVersion}";

        #endregion

        #region Methods

        public MainViewModel(IMessenger messenger)
            : base(messenger)
        {
            MessengerInstance.Register<MoveToStepMessage>(this, GoToStep);
        }

        private void GoToStep(MoveToStepMessage msg)
        {
            if (msg == null)
            {
                return;
            }

            CurrentStep = msg.Step;
        }

        #endregion
    }
}