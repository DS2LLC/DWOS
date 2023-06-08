using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using DWOS.Data;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;

namespace DWOS.UI.Utilities.Scale
{
    /// <summary>
    /// Interaction logic for TareWeightDialog.xaml
    /// </summary>
    public partial class TareWeightDialog
    {
        #region Properties

        private DialogContext ViewModel => DataContext as DialogContext;

        public decimal TareWeight
        {
            get => ViewModel?.Value ?? 0M;
            set
            {
                var vm = ViewModel;
                if (vm != null)
                {
                    vm.Value = value;
                }
            }
        }

        #endregion

        #region Methods

        public TareWeightDialog()
        {
            InitializeComponent();
            DataContext = new DialogContext();
            Icon = Properties.Resources.Settings_32.ToWpfImage();
        }

        #endregion

        #region Events
        private void TareWeightDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ValueEditor.Focus();
                ValueEditor.SelectAll();

                var vm = ViewModel;

                if (vm != null)
                {
                    vm.Accepted += VmOnAccepted;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading tare weight dialog.");
            }
        }

        private void TareWeightDialog_OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if (vm != null)
                {
                    vm.Accepted -= VmOnAccepted;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error unloading tare weight dialog.");
            }
        }

        private void VmOnAccepted(object sender, EventArgs eventArgs)
        {
            DialogResult = true;
        }

        #endregion

        #region DialogContext

        private class DialogContext : INotifyPropertyChanged
        {
            #region Fields

            public event EventHandler Accepted;
            private decimal _editorValue;

            #endregion

            #region Properties

            public decimal Value
            {
                get => _editorValue;
                set
                {
                    if (_editorValue != value)
                    {
                        _editorValue = value;
                        OnPropertyChanged(nameof(Value));
                    }
                }
            }

            public string Format =>
                $"0.{"0".Repeat(ApplicationSettings.Current.WeightDecimalPlaces)} lbs";

            public string Mask =>
                $"nnnnnn.{"n".Repeat(ApplicationSettings.Current.WeightDecimalPlaces)}";

            public ICommand Accept { get; }

            #endregion

            #region Methods

            public DialogContext()
            {
                Accept = new RelayCommand(
                    () =>
                    {
                        Accepted?.Invoke(this, EventArgs.Empty);
                    });
            }

            private void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            #endregion

            #region INotifyPropertyChnaged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion
    }
}
