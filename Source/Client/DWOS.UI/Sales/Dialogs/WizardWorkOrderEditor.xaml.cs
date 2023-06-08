using DWOS.UI.Sales.ViewModels;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Windows;
using static DWOS.UI.Sales.ViewModels.SalesOrderWizardViewModel;

namespace DWOS.UI.Sales.Dialogs
{
    /// <summary>
    /// Interaction logic for WizardWorkOrderEditor.xaml
    /// </summary>
    public partial class WizardWorkOrderEditor
    {
        #region Fields

        public static readonly DependencyProperty MainViewModelProperty = DependencyProperty.Register(
            nameof(MainViewModel), typeof(SalesOrderWizardViewModel), typeof(WizardWorkOrderEditor));

        public static readonly DependencyProperty WorkOrderProperty = DependencyProperty.Register(
            nameof(WorkOrder), typeof(WorkOrderViewModel), typeof(WizardWorkOrderEditor), new FrameworkPropertyMetadata { PropertyChangedCallback = WorkOrderChanged });

        private readonly PersistWpfWindowState _persistState;

        #endregion

        #region Properties

        public SalesOrderWizardViewModel MainViewModel
        {
            get => GetValue(MainViewModelProperty) as SalesOrderWizardViewModel;
            set => SetValue(MainViewModelProperty, value);
        }

        public WorkOrderViewModel WorkOrder
        {
            get => GetValue(WorkOrderProperty) as WorkOrderViewModel;
            set => SetValue(WorkOrderProperty, value);
        }

        #endregion

        #region Methods

        public WizardWorkOrderEditor()
        {
            _persistState = new PersistWpfWindowState(this);
            Icon = Properties.Resources.Order_32.ToWpfImage();
            InitializeComponent();
        }

        #endregion

        #region Events

        private static void WorkOrderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (!(d is WizardWorkOrderEditor editor))
                {
                    return;
                }


                if (e.OldValue is WorkOrderViewModel oldValue)
                {
                    oldValue.Accepted -= editor.WorkOrder_Accepted;
                }

                if (e.NewValue is WorkOrderViewModel newValue)
                {
                    newValue.Accepted += editor.WorkOrder_Accepted;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error changing WorkOrder field.");
            }
        }

        private  void WorkOrder_Accepted(object sender, EventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error accepting dialog.");
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void AddSerialNumberButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = WorkOrder;

                if (vm != null && vm.CanAddSerialNumbers)
                {
                    vm.SerialNumbers.Add(new SerialNumberViewModel());
                }
                else
                {
                    MessageBox.Show(
                        "Cannot add another serial number.",
                        "Serial Numbers",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error adding serial number");
            }
        }

        private void DeleteSerialNumberButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = WorkOrder;
                var selectedItem = SerialNumbersGrid.SelectedItem as SerialNumberViewModel;

                if (vm != null && selectedItem != null)
                {
                    vm.SerialNumbers.Remove(selectedItem);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error deleting serial number");
            }
        }

        #endregion
    }
}
