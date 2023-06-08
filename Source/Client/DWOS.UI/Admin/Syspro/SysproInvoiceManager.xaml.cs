using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderInvoiceDataSetTableAdapters;
using DWOS.Shared;
using DWOS.UI.Utilities;
using NLog;
using System.Collections;

namespace DWOS.UI.Admin.Syspro
{
    /// <summary>
    /// Interaction logic for SysproInvoiceManager.xaml
    /// </summary>
    public partial class SysproInvoiceManager
    {
        #region Fields

        private const string DIALOG_TITLE = "SYSPRO";
        private static readonly TimeSpan TIMER_INTERVAL = TimeSpan.FromSeconds(15);
        private readonly object _timerLock = new object();
        private DispatcherTimer _timer;
        private readonly GridSettingsPersistence<XamDataGridSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<XamDataGridSettings>("SysproManager", new XamDataGridSettings());

        #endregion

        #region Methods

        public SysproInvoiceManager()
        {
            InitializeComponent();
            DeleteButton.LargeImage = Properties.Resources.Delete_32.ToWpfImage();
            ResetButton.LargeImage = Properties.Resources.Reset_48.ToWpfImage();
            Icon = Properties.Resources.Paper32.ToWpfImage();
            DataContext = new ManagerContext();
        }

        #endregion

        #region Events

        private void SysproInvoiceManager_OnLoaded(object sender, RoutedEventArgs e)
        {
            lock (_timerLock)
            {
                _timer = new DispatcherTimer()
                {
                    Interval = TIMER_INTERVAL
                };

                _timer.Tick += Timer_Tick;

                _timer.Start();
            }

            var vm = DataContext as ManagerContext;

            try
            {
                vm?.Refresh();
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading SYSPRO invoice manager", exc);
            }

            // Load settings
            _gridSettingsPersistence.LoadSettings().ApplyTo(InvoicesGrid);
        }
        private void SysproInvoiceManager_OnUnloaded(object sender, RoutedEventArgs e)
        {
            lock (_timerLock)
            {
                _timer?.Stop();
            }

            // Save settings
            var settings = new XamDataGridSettings();
            settings.RetrieveSettingsFrom(InvoicesGrid);
            _gridSettingsPersistence.SaveSettings(settings);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lock (_timerLock)
            {
                if (!_timer.IsEnabled)
                {
                    return;
                }

                _timer.Stop();

                var vm = DataContext as ManagerContext;
                vm?.Refresh();

                _timer.Start();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region ManagerContext

        private sealed class ManagerContext : INotifyPropertyChanged
        {
            #region Fields

            private object[] _selectedItems;
            private readonly object _uiUpdateLock = new object();

            #endregion

            #region Properties

            public ObservableCollection<SysproInvoice> Invoices { get; } = new ObservableCollection<SysproInvoice>();

            /// <summary>
            /// Gets or sets selected items.
            /// </summary>
            public object[] SelectedItems
            {
                get { return _selectedItems; }
                set
                {
                    if (_selectedItems != value)
                    {
                        _selectedItems = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItems)));
                    }
                }
            }

            public bool HasSysproInvoiceSelected
            {
                get
                {
                    if (_selectedItems == null)
                    {
                        return false;
                    }

                    return _selectedItems.Length > 0 &&
                        _selectedItems.All(i => i is SysproInvoice);
                }
            }

            public bool HasSuccessfulSelected
            {
                get
                {
                    if (_selectedItems == null)
                    {
                        return false;
                    }

                    return _selectedItems
                        .OfType<SysproInvoice>()
                        .Any(i => i.Status == SysproInvoiceStatus.Successful);
                }
            }

            public bool HasPendingSelected
            {
                get
                {
                    if (_selectedItems == null)
                    {
                        return false;
                    }

                    return _selectedItems
                        .OfType<SysproInvoice>()
                        .Any(i => i.Status == SysproInvoiceStatus.Pending);
                }
            }

            public ICommand Retry { get; private set; }

            public ICommand Delete { get; private set; }

            #endregion

            #region Methods

            public ManagerContext()
            {
                Retry = new RetryCommand(this);
                Delete = new DeleteCommand(this);
            }

            public void Refresh()
            {
                lock (_uiUpdateLock)
                {
                    SysproInvoiceTableAdapter taSyspro = null;
                    SysproInvoiceOrderTableAdapter taSysproInvoice = null;
                    OrderInvoiceDataSet dsOrderInvoice = new OrderInvoiceDataSet() { EnforceConstraints = false };

                    try
                    {
                        var previousSelection = _selectedItems ?? new object[0];
                        var selectedInvoices = previousSelection.OfType<SysproInvoice>()
                            .Select(i => i.Row.SysproInvoiceId)
                            .ToList();

                        var selectedOrders = previousSelection.OfType<SysproInvoiceOrder>()
                            .Select(i => i.OrderId)
                            .ToList();

                        // Load Data
                        taSyspro = new SysproInvoiceTableAdapter();
                        taSysproInvoice = new SysproInvoiceOrderTableAdapter();
                        taSyspro.Fill(dsOrderInvoice.SysproInvoice);
                        taSysproInvoice.Fill(dsOrderInvoice.SysproInvoiceOrder);

                        foreach (var invoiceRow in dsOrderInvoice.SysproInvoice)
                        {
                            var match = Invoices.FirstOrDefault(i => i.Row.IsValidState() && i.Row.SysproInvoiceId == invoiceRow.SysproInvoiceId);

                            if (match == null)
                            {
                                Invoices.Add(SysproInvoice.From(invoiceRow));
                            }
                            else
                            {
                                match.Update(invoiceRow);
                            }
                        }

                        var newSelection = new ArrayList();
                        var orders = Invoices.SelectMany(i => i.Orders).ToList();

                        // Update selection
                        foreach (var selectedOrderId in selectedOrders)
                        {
                            var match = orders.FirstOrDefault(o => o.OrderId == selectedOrderId);

                            if (match != null)
                            {
                                newSelection.Add(match);
                            }
                        }

                        foreach (var selectedInvoiceId in selectedInvoices)
                        {
                            var match = Invoices.FirstOrDefault(i => i.Row.SysproInvoiceId == selectedInvoiceId);

                            if (match != null)
                            {
                                newSelection.Add(match);
                            }
                        }

                        SelectedItems = newSelection.ToArray();
                    }
                    finally
                    {
                        taSyspro?.Dispose();
                        taSysproInvoice?.Dispose();
                    }
                }
            }

            public int RetrySelected()
            {
                var selectedOrders = _selectedItems.OfType<SysproInvoice>()
                    .Where(i => i.Status != SysproInvoiceStatus.Pending)
                    .SelectMany(i => i.Orders)
                    .ToList();

                SelectedItems = null; // Fix issue where commands check status of deleted orders

                using (var taOrderInvoice = new OrderInvoiceTableAdapter())
                {
                    foreach (var order in selectedOrders)
                    {
                        var invoice = taOrderInvoice.GetInvoice(order.OrderId);

                        taOrderInvoice.AddInvoice(null, order.OrderId);

                        OrderHistoryDataSet.UpdateOrderHistory(order.OrderId,
                            "Invoice Manager",
                            $"Invoice value changed from {invoice} to",
                            SecurityManager.Current.UserName);
                    }
                }

                return selectedOrders.Count;

            }

            public int RemoveSelected()
            {
                var invoicesToRemove = _selectedItems.OfType<SysproInvoice>()
                    .Where(i => i.Status != SysproInvoiceStatus.Pending)
                    .ToList();

                SelectedItems = null;
                Invoices.Clear();

                using (var taSyspro = new SysproInvoiceTableAdapter())
                {
                    foreach (var invoice in invoicesToRemove)
                    {
                        invoice.Row.Delete();
                        taSyspro.Update(invoice.Row);
                    }
                }

                Refresh();

                return invoicesToRemove.Count;
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region SysproInvoice

        private class SysproInvoice : INotifyPropertyChanged
        {
            #region Properties

            public string TransmissionReference => Row.TransmissionReference;

            public SysproInvoiceStatus Status
            {
                get
                {
                    SysproInvoiceStatus status;
                    Enum.TryParse(Row.Status, out status);
                    return status;
                }
            }

            public string Message => Row.IsMessageNull() ? string.Empty : Row.Message;

            public DateTime Created => Row.Created;

            public IEnumerable<SysproInvoiceOrder> Orders { get; private set; }

            public OrderInvoiceDataSet.SysproInvoiceRow Row { get; private set; }

            #endregion

            #region Methods

            private SysproInvoice()
            {
            }

            public static SysproInvoice From(OrderInvoiceDataSet.SysproInvoiceRow invoiceRow)
            {
                if (invoiceRow == null)
                {
                    return null;
                }

                return new SysproInvoice
                {
                    Orders = invoiceRow.GetSysproInvoiceOrderRows().Select(SysproInvoiceOrder.From),
                    Row = invoiceRow
                };
            }

            public void Update(OrderInvoiceDataSet.SysproInvoiceRow invoiceRow)
            {
                if (invoiceRow == null || !Row.IsValidState() || invoiceRow.SysproInvoiceId != Row.SysproInvoiceId)
                {
                    return;
                }

                Row = invoiceRow;

                // Assumption: Only status and message can change
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message)));
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region SysproInvoiceOrder

        private sealed class SysproInvoiceOrder
        {
            #region Properties

            public int OrderId => Row.OrderId;

            public OrderInvoiceDataSet.SysproInvoiceOrderRow Row { get; private set; }

            #endregion

            #region Methods

            private SysproInvoiceOrder()
            {
            }

            public static SysproInvoiceOrder From(OrderInvoiceDataSet.SysproInvoiceOrderRow orderRow)
            {
                if (orderRow == null)
                {
                    return null;
                }

                return new SysproInvoiceOrder
                {
                    Row = orderRow
                };
            }

            #endregion
        }

        #endregion

        #region RetryCommand

        private sealed class RetryCommand : ICommand
        {
            private readonly ManagerContext _context;

            public RetryCommand(ManagerContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                _context = context;
            }

            #region ICommand Members

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public bool CanExecute(object parameter)
            {
                try
                {
                    return _context.HasSysproInvoiceSelected && !_context.HasPendingSelected;
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error running CanExecute for retry command.");
                    return false;
                }
            }

            public void Execute(object parameter)
            {
                if (!CanExecute(parameter))
                {
                    return;
                }

                try
                {
                    var dialogResult =
                        MessageBoxUtilities.ShowMessageBoxYesOrNo(
                            "Are you sure that you want to reset invoice numbers for the selected orders?", DIALOG_TITLE);

                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        var orderCount = _context.RetrySelected();

                        var msg = orderCount > 1
                            ? $"Successfully reset the invoice for {orderCount} orders."
                            : $"Successfully reset the invoice for 1 order.";

                        MessageBoxUtilities.ShowMessageBoxOK(msg,
                           DIALOG_TITLE); 
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error running Execute for retry command.");
                }
            }

            #endregion
        }

        #endregion

        #region RemoveCommand

        private sealed class DeleteCommand : ICommand
        {
            private readonly ManagerContext _context;

            public DeleteCommand(ManagerContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                _context = context;
            }

            #region ICommand Members

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public bool CanExecute(object parameter)
            {
                try
                {
                    return _context.HasSysproInvoiceSelected && !_context.HasPendingSelected;
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error running CanExecute for remove command.");
                    return false;
                }
            }

            public void Execute(object parameter)
            {
                if (!CanExecute(parameter))
                {
                    return;
                }

                try
                {
                    var dialogResult =
                        MessageBoxUtilities.ShowMessageBoxYesOrNo(
                            "Are you sure that you want to remove these invoices?\n(This will not delete data in SYSPRO.)", DIALOG_TITLE);

                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        var count = _context.RemoveSelected();

                        var msg = count > 1
                            ? $"Successfully deleted {count} invoices from DWOS."
                            : "Successfully deleted 1 invoice from DWOS.";

                        MessageBoxUtilities.ShowMessageBoxOK(msg,
                            DIALOG_TITLE);
                    }

                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error running Execute for remove command.");
                }
            }

            #endregion
        }

        #endregion
    }
}