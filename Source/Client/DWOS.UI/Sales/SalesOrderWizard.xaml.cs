using DWOS.Data;
using DWOS.UI.Sales.Dialogs;
using DWOS.UI.Sales.ViewModels;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Linq;
using System.Windows;
using static DWOS.UI.Sales.ViewModels.SalesOrderWizardViewModel;

namespace DWOS.UI.Sales
{
    /// <summary>
    /// Interaction logic for SalesOrderWizard.xaml
    /// </summary>
    public partial class SalesOrderWizard : ISalesOrderWizardView
    {
        #region Fields

        private readonly PersistWpfWindowState _persistState;
        private bool _forceClose;

        #endregion

        #region Properties

        public SalesOrderWizardViewModel ViewModel =>
            DataContext as SalesOrderWizardViewModel;

        #endregion

        #region Methods

        public SalesOrderWizard()
        {
            _persistState = new PersistWpfWindowState(this);
            Icon = Properties.Resources.SalesOrder_32.ToWpfImage();
            InitializeComponent();
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if (vm != null)
                {
                    vm.Initialize(this);
                    vm.Saved += Vm_Saved;
                }

                SecurityManager.Current.UserUpdated += Current_UserUpdated;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading Sales Order Wizard.");
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if (vm != null)
                {
                    vm.Saved -= Vm_Saved;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading Sales Order Wizard.");
            }
        }

        private void Vm_Saved(object sender, SavedDataEventArgs e)
        {
            try
            {
                DWOSApp.MainForm.FlyoutManager.DisplayFlyout("Order", $"Sales Order {e.Id} was created");
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error showing 'created sales order' flyout");
            }
        }


        private void Current_UserUpdated(object sender, UserUpdatedEventArgs e)
        {
            try
            {
                _forceClose = true;
                Close();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling user updated event.");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if (vm == null)
                {
                    return;
                }

                var showPrompt = !_forceClose &&
                    (vm.CurrentStep != StepType.First || vm.MediaLinks.Count > 0 || vm.DocumentLinks.Count > 0);

                if (showPrompt)
                {
                    var result = MessageBox.Show(
                        "Are you sure that you want to close Sales Order Wizard?",
                        "Sales Order Wizard",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    e.Cancel = result != MessageBoxResult.Yes;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling window closing event.");
            }
        }

        #endregion

        #region ISalesOrderWizardView Members

        public WorkOrderViewModel ShowAddOrderDialog(SalesOrderWizardViewModel mainViewModel)
        {
            var dialog = new WizardWorkOrderEditor()
            {
                MainViewModel = mainViewModel,
                WorkOrder = new WorkOrderViewModel(mainViewModel)
                {
                    CustomerWorkOrder = mainViewModel.CustomerWorkOrder
                }
            };

            if (dialog.ShowDialog() ?? false)
            {
                return dialog.WorkOrder;
            }

            return null;
        }

        public WorkOrderViewModel ShowEditOrderDialog(SalesOrderWizardViewModel mainViewModel, WorkOrderViewModel workOrder)
        {
            var dialog = new WizardWorkOrderEditor()
            {
                MainViewModel = mainViewModel,
                WorkOrder = workOrder.Clone() // Do not use original instance
            };

            if (dialog.ShowDialog() ?? false)
            {
                return dialog.WorkOrder;
            }

            return null;
        }

        public bool? ShowDeleteOrderDialog()
        {
            var result = MessageBox.Show("Are you sure that you want to delete this part from the Sales Order?",
                "Sales Order",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            return result == MessageBoxResult.Yes;
        }

        public bool? ShowEditFeesDialog(SalesOrderWizardViewModel viewModel)
        {
            var dialog = new OrderFeesDialog();
            dialog.LoadData(viewModel.OrderFees.Select(fee => fee.Clone()), viewModel.SelectedCustomer);

            var result = dialog.ShowDialog();

            if (result ?? false)
            {
                // Accept changes
                viewModel.OrderFees.Clear();

                foreach (var fee in dialog.OrderFees)
                {
                    viewModel.OrderFees.Add(fee);
                }
            }

            return result;
        }

        public bool ShowPriorityWarning()
        {
            const string warningMessage = "The current order has a " +
                "'Expedite' fee, but the priority is still 'Normal'.\r\n\r\n" +
                "Do you want to set the priority to 'Expedite'?";

            var result = MessageBox.Show(warningMessage,
                "Incorrect Priority",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            return result == MessageBoxResult.Yes;
        }

        public void ShowWorkOrderProcessesError()
        {
            const string errorMessage = "The part that you selected does " +
                "not have the same processes as other parts in the " +
                "Sales Order.";

            MessageBox.Show(errorMessage,
                "Incorrect Processes",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public void ShowRushChargeWarning(decimal currentRushCharge, decimal minimumRushCharge)
        {
            var warningMsg = $"The current order has an 'Expedite' fee of " +
                $"{currentRushCharge.ToString(OrderPrice.CurrencyFormatString)}, " +
                $"which is less than the minimum amount.\r\n\r\n" +
                $"Please correct this to meet the minimum amount of " +
                $"{minimumRushCharge.ToString(OrderPrice.CurrencyFormatString)}.";
            MessageBox.Show(warningMsg,
                "Below Minimum Priority Charge",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

        }

        #endregion
    }
}
