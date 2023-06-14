using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace DWOS.AutomatedWorkOrderTool.Dialogs
{
    /// <summary>
    /// Interaction logic for AddManufacturerDialog.xaml
    /// </summary>
    public partial class AddManufacturerDialog
    {
        #region Properties

        public CustomerViewModel Customer { get; internal set; }

        #endregion

        #region Methods

        public AddManufacturerDialog()
        {
            InitializeComponent();
        }

        public AddManufacturerDialog(MetroDialogSettings settings)
            : base(settings)
        {
            InitializeComponent();
        }

        public AddManufacturerDialog(MetroWindow parentWindow)
            :base(parentWindow)
        {
            InitializeComponent();
        }

        public AddManufacturerDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            :base(parentWindow, settings)
        {
            InitializeComponent();
        }

        internal Task<ManufacturerViewModel> WaitForResult()
        {
            var tcs = new TaskCompletionSource<ManufacturerViewModel>();
            Action cleanup = null;

            // Setup event handlers
            void OkHandler(object sender, RoutedEventArgs args)
            {
                cleanup?.Invoke();
                tcs.TrySetResult(GetResult());
            }

            OkButton.Click += OkHandler;

            void CancelHandler(object sender, RoutedEventArgs args)
            {
                cleanup?.Invoke();
                tcs.TrySetResult(null);
            }

            CancelButton.Click += CancelHandler;

            void CancelKeyHandler(object sender, KeyEventArgs args)
            {
                if (args.Key == Key.Escape)
                {
                    cleanup?.Invoke();
                    tcs.TrySetResult(null);
                }
            }

            KeyDown += CancelKeyHandler;

            // Register cancellation handler
            var cancelRegistration = DialogSettings.CancellationToken.Register(() =>
            {
                cleanup?.Invoke();
                tcs.TrySetResult(null);
            });

            cleanup = () =>
            {
                // Clean up event handlers
                OkButton.Click -= OkHandler;
                CancelButton.Click -= CancelHandler;
                KeyDown -= CancelKeyHandler;

                // Dispose of the cancellation callback because it's not needed
                cancelRegistration.Dispose();
            };

            return tcs.Task;
        }

        private ManufacturerViewModel GetResult()
        {
            var vm = InnerControl.DataContext as AddManufacturerViewModel;
            return vm?.SelectedManufacturer;
        }

        #endregion

        #region Events

        private void AddManufacturerDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = InnerControl.DataContext as AddManufacturerViewModel;
            vm?.LoadData(Customer);
        }

        #endregion
    }
}
