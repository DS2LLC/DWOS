using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using DWOS.AutomatedWorkOrderTool.Dialogs;
using DWOS.AutomatedWorkOrderTool.Dialogs.ImportMasterList;
using DWOS.AutomatedWorkOrderTool.Dialogs.ImportShippingManifest;
using DWOS.AutomatedWorkOrderTool.Messages;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.AutomatedWorkOrderTool.Services;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using NLog;
using MahApps.Metro.Controls.Dialogs;

namespace DWOS.AutomatedWorkOrderTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Fields

        /// <summary>
        /// If the user has unsaved changes and decides to close the app, this
        /// flag allows this window to close itself.
        /// </summary>
        private bool _forceClose;

        #endregion

        #region Properties

        private object SelectedItem =>
            (DataContext as MainWindowViewModel)?.SelectedItem;

        #endregion

        #region Methods

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void HandleShowDialog(ShowDialogMessage msg)
        {
            if (msg == null)
            {
                return;
            }

            var messenger = SimpleIoc.Default.GetInstance<IMessenger>();

            switch (msg.Type)
            {
                case ShowDialogMessage.DialogType.AddCustomer:
                    var customerResult = await this.ShowAddCustomerDialog();
                    if (customerResult != null)
                    {
                        messenger.Send(new AddCustomerMessage(customerResult));
                    }

                    break;
                case ShowDialogMessage.DialogType.AddManufacturer:
                    if (SelectedItem is CustomerViewModel manufacturerCustomer)
                    {
                        var mfgResult = await this.ShowAddManufacturerDialog(manufacturerCustomer);

                        if (mfgResult != null)
                        {
                            messenger.Send(new AddOspFormatMessage(manufacturerCustomer, mfgResult));
                        }
                    }

                    break;
                case ShowDialogMessage.DialogType.ImportMasterList:
                    if (SelectedItem is CustomerViewModel masterListCustomer)
                    {
                        var masterListWindow = new MasterListWindow();
                        masterListWindow.Load(masterListCustomer);
                        masterListWindow.ShowDialog();
                    }

                    break;
                case ShowDialogMessage.DialogType.ImportShippingManifest:
                    if (SelectedItem is CustomerViewModel shippingCustomer)
                    {
                        var shippingManifestWindow = new ShippingManifestWindow();
                        shippingManifestWindow.Load(shippingCustomer);
                        shippingManifestWindow.ShowDialog();
                    }

                    break;
            }
        }

        private async void HandleShowAddSectionMessage(ShowAddSectionDialogMessage msg)
        {
            if (msg?.CurrentFormat == null)
            {
                return;
            }

            var result = await this.ShowAddOspFormatSectionDialog(msg.CurrentFormat, msg.CurrentSections);

            if (result != null)
            {
                var messenger = SimpleIoc.Default.GetInstance<IMessenger>();
                messenger.Send(new AddOspFormatSectionMessage(result));
            }
        }

        private async void HandleShowAddCodeMapMessage(ShowAddCodeMapDialogMessage msg)
        {
            if (msg?.CurrentFormat == null || msg.CurrentSections == null)
            {
                return;
            }

            var result = await this.ShowAddOspFormatCodeMapDialog(msg);

            if (result != null)
            {
                var messenger = SimpleIoc.Default.GetInstance<IMessenger>();

                if (result.CodeMapType == RoleType.Process)
                {
                    messenger.Send(new AddOspProcessMessage(result.Process));
                }
                else if (result.CodeMapType == RoleType.PartMark)
                {
                    messenger.Send(new AddOspPartMarkMessage(result.PartMark));
                }
            }
        }

        private async void HandleErrorMessage(ErrorMessage msg)
        {
            if (msg == null)
            {
                return;
            }

            string errorText;

            if (msg.Exception != null)
            {
                LogManager.GetCurrentClassLogger().Error(msg.Exception, msg.Message);
                errorText = msg.Message + Environment.NewLine + msg.Exception.Message + Environment.NewLine + Environment.NewLine + msg.Exception;
            }
            else
            {
                errorText = msg.Message;
            }

            await this.ShowMessageAsync("Error", errorText);
        }

        private async void HandleSuccessMessage(SuccessMessage msg)
        {
            if (msg == null)
            {
                return;
            }

            switch (msg.Type)
            {
                case SuccessMessage.SuccessType.SaveOspFormat:
                    await this.ShowMessageAsync("Success", "Successfully saved changes to an OSP Format.");
                    break;
            }
        }

        private async void HandleConfirmActionMessage(ConfirmActionMessage msg)
        {
            try
            {
                if (msg == null)
                {
                    return;
                }

                var result = await this.ShowMessageAsync(msg.Title, msg.Message, MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Affirmative)
                {
                    msg.OnConfirm?.Invoke();
                }
            }
            catch (Exception exc)
            {
                HandleErrorMessage(new ErrorMessage(exc, "Error showing confirm message."));
            }
        }

        private async Task<MessageDialogResult> ShowUnsavedChangesWarningAsync()
        {
            const string warningMsg = "There are unsaved changes. Would you like to continue?";

            var dialogSettings = new MetroDialogSettings
                                 {
                                     AffirmativeButtonText = "Yes",
                                     NegativeButtonText = "No"
                                 };

            return await this.ShowMessageAsync("AWOT", warningMsg, MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
        }

        #endregion

        #region Events

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Register message handlers
                SimpleIoc.Default.GetInstance<IUserManager>().UserChanged += OnUserChanged;
                var messenger = SimpleIoc.Default.GetInstance<IMessenger>();
                messenger.Register<ShowDialogMessage>(this, HandleShowDialog);
                messenger.Register<ShowAddSectionDialogMessage>(this, HandleShowAddSectionMessage);
                messenger.Register<ShowAddCodeMapDialogMessage>(this, HandleShowAddCodeMapMessage);
                messenger.Register<ErrorMessage>(this, HandleErrorMessage);
                messenger.Register<SuccessMessage>(this, HandleSuccessMessage);
                messenger.Register<ConfirmActionMessage>(this, HandleConfirmActionMessage);

                // Sort list of customers
                CustomersView.Items.SortDescriptions.Add(new SortDescription(nameof(CustomerViewModel.Name), ListSortDirection.Ascending));
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading main window.");
            }
        }

        private void MainWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SimpleIoc.Default.GetInstance<IUserManager>().UserChanged -= OnUserChanged;
                SimpleIoc.Default.GetInstance<IMessenger>().Unregister(this);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error unloading main window.");
            }
        }

        private void OnUserChanged(object sender, UserChangedEventArgs args)
        {
            try
            {
                if (args.Type == UserChangedEventArgs.ChangeType.Expected)
                {
                    return;
                }

                this.ShowMessageAsync("Lost Connection", "Lost connection to the server. Please login again.").ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        LogManager.GetCurrentClassLogger().Error(task.Exception, "Error showing dialog.");
                    }
                });
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error handling user change event.");
            }
        }

        private async void CustomersView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == SelectedItem)
            {
                // This is supposed to be selected
                return;
            }
            
            if (!(DataContext is MainWindowViewModel vm))
            {
                // Data Context is invalid.
                return;
            }

            if (vm.HasUnsavedChanges)
            {
                // Update the list view while handing LayoutUpdated.
                // Changing the selection here causes an infinite loop of updates.
                // Workaround is based on https://stackoverflow.com/a/4029075
                CustomersView.LayoutUpdated += ResetCustomerViewSelection;

                if (await ShowUnsavedChangesWarningAsync() == MessageDialogResult.Affirmative)
                {
                    vm.ClearUnsavedChanges();
                    vm.SelectedItem = e.NewValue;

                    if (SelectedItem is ISelectable selectable)
                    {
                        selectable.IsSelected = true;
                    }
                }
            }
            else
            {
                vm.SelectedItem = e.NewValue;
            }
        }

        private void ResetCustomerViewSelection(object sender, EventArgs e)
        {
            if (SelectedItem is ISelectable selectable)
            {
                selectable.IsSelected = true;
            }

            CustomersView.LayoutUpdated -= ResetCustomerViewSelection;
        }

        private async void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            try
            {
                if (_forceClose)
                {
                    return;
                }

                if (!((DataContext as MainWindowViewModel)?.HasUnsavedChanges ?? false))
                {
                    return;
                }

                // Show warning for unsaved changes
                e.Cancel = true;
                var result = await ShowUnsavedChangesWarningAsync();

                if (result == MessageDialogResult.Affirmative)
                {
                    _forceClose = true;
                    Close();
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error closing main window.");
            }
        }

        #endregion
    }
}
