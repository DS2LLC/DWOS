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
    /// Interaction logic for AddCustomerDialog.xaml
    /// </summary>
    public partial class AddCustomerDialog
    {
        #region Methods

        public AddCustomerDialog()
        {
            InitializeComponent();
        }

        public AddCustomerDialog(MetroDialogSettings settings)
            : base(settings)
        {
            InitializeComponent();
        }

        public AddCustomerDialog(MetroWindow parentWindow)
            :base(parentWindow)
        {
            InitializeComponent();
        }

        public AddCustomerDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            :base(parentWindow, settings)
        {
            InitializeComponent();
        }

        internal Task<CustomerViewModel> WaitForResult()
        {
            var tcs = new TaskCompletionSource<CustomerViewModel>();
            Action cleanup = null;

            // Setup event handlers
            void OkHandler(object sender, RoutedEventArgs args)
            {
                cleanup?.Invoke();
                tcs.TrySetResult(GetCustomerData());
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

        private CustomerViewModel GetCustomerData()
        {
            var vm = InnerControl.DataContext as AddCustomerViewModel;
            return vm?.SelectedCustomer;
        }

        #endregion

        #region Events

        private void AddCustomerDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = InnerControl.DataContext as AddCustomerViewModel;
            vm?.LoadData();
        }

        #endregion
    }
}
