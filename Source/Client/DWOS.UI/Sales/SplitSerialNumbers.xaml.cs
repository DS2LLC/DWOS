using System;
using DWOS.Data.Datasets;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DWOS.UI.Utilities;

namespace DWOS.UI.Sales
{
    /// <summary>
    /// Interaction logic for SplitSerialNumbers.xaml
    /// </summary>
    public partial class SplitSerialNumbers
    {
        #region Methods

        public SplitSerialNumbers()
        {
            InitializeComponent();
            DataContext = new SplitSerialNumberContext();
            Icon = Properties.Resources.Barcode32.ToWpfImage();
            ErrorImage.Source = Properties.Resources.Error_16.ToWpfImage();
            WarningImage.Source = Properties.Resources.Warning_16.ToWpfImage();
        }

        public void Load(int originalOrderId, ICollection<OrdersDataSet.OrderRow> orders)
        {
            if (orders == null)
            {
                throw new ArgumentNullException(nameof(orders));
            }

            var model = DataContext as SplitSerialNumberContext;

            if (model == null)
            {
                return;
            }

            model.OriginalOrderId = originalOrderId;
            model.SerialNumbers.Clear();
            model.Orders.Clear();

            var originalOrder = orders.FirstOrDefault(r => r.OrderID == originalOrderId);

            if (originalOrder != null)
            {
                var serialNumbers = originalOrder
                    .GetOrderSerialNumberRows()
                    .Where(s => s.IsValidState() && s.Active)
                    .Select(model.NewSerialNumber)
                    .ToList();

                // Set a default order ID for each serial number
                var qtyDict = orders.ToDictionary(o => o.OrderID, o => o.IsPartQuantityNull() ? 0 : o.PartQuantity);

                var skip = 0;
                foreach (var orderId in orders.Select(o => o.OrderID))
                {
                    var qty = qtyDict[orderId];
                    foreach (var number in serialNumbers.Skip(skip).Take(qty))
                    {
                        number.OrderId = orderId;
                    }

                    skip += qty;
                }

                model.SerialNumbers.AddRange(serialNumbers);
            }

            model.Orders.AddRange(orders);
        }

        /// <summary>
        /// Syncs dialog data with a data table.
        /// </summary>
        /// <param name="dtSerialNumbers">Data table to use for persisting order serial numbers.</param>
        public void Sync(OrdersDataSet.OrderSerialNumberDataTable dtSerialNumbers)
        {
            if (dtSerialNumbers == null)
            {
                throw new ArgumentNullException(nameof(dtSerialNumbers));
            }

            var model = DataContext as SplitSerialNumberContext;

            if (model == null)
            {
                return;
            }

            var dateRemoved = DateTime.Now;

            foreach (var serialNumber in model.SerialNumbers)
            {
                if (serialNumber.OrderId == serialNumber.OriginalRow.OrderID)
                {
                    continue;
                }

                // Remove existing serial number
                serialNumber.OriginalRow.Active = false;
                serialNumber.OriginalRow.DateRemoved = dateRemoved;

                // Add for new order
                var newSerialNumberRow = dtSerialNumbers.NewOrderSerialNumberRow();
                newSerialNumberRow.Active = true;
                newSerialNumberRow.Number = serialNumber.Number;
                newSerialNumberRow.OrderID = serialNumber.OrderId;
                newSerialNumberRow.PartOrder = dtSerialNumbers.Count(s => s.OrderID == serialNumber.OrderId) + 1;

                dtSerialNumbers.AddOrderSerialNumberRow(newSerialNumberRow);
            }
        }

        #endregion

        #region Events

        private void SplitSerialNumbers_OnLoaded(object sender, RoutedEventArgs e)
        {
            var model = DataContext as SplitSerialNumberContext;

            if (model == null)
            {
                return;
            }

            model.Completed += Model_Completed;
        }

        private void SplitSerialNumbers_OnUnloaded(object sender, RoutedEventArgs e)
        {
            var model = DataContext as SplitSerialNumberContext;

            if (model == null)
            {
                return;
            }

            model.Completed -= Model_Completed;
        }

        private void Model_Completed(object sender, EventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            string msg =
                "Do you want to skip this step?\n(Serial Numbers will not be moved, but Split Order will continue.)";

            var boxResult = MessageBoxUtilities.ShowMessageBoxYesOrNo(msg, "Split Serial Numbers");

            if (boxResult == System.Windows.Forms.DialogResult.Yes)
            {
                DialogResult = false;
            }
        }

        #endregion

        #region SplitSerialNumbersContext

        private class SplitSerialNumberContext : INotifyPropertyChanged, IDataErrorInfo
        {
            #region Fields

            public event EventHandler Completed;
            private int _originalOrderId;

            #endregion

            #region Properties

            public List<SerialNumber> SerialNumbers { get; } = new List<SerialNumber>();

            public IEnumerable<int> OrderIds => Orders.Select(o => o.OrderID);

            public List<OrdersDataSet.OrderRow> Orders { get; } = new List<OrdersDataSet.OrderRow>();

            public int OriginalOrderId
            {
                get { return _originalOrderId; }
                set
                {
                    if (_originalOrderId != value)
                    {
                        _originalOrderId = value;
                        OnPropertyChanged(nameof(OriginalOrderId));
                    }
                }
            }

            public ICommand Accept { get; }

            public Visibility WarningMessageVisibility => ShowQuantityWarning ? Visibility.Visible : Visibility.Collapsed;

            public string Warning
            {
                get
                {
                    if (ShowQuantityWarning)
                    {
                        return "Original order did not have a serial number for each part in the order.";
                    }

                    return string.Empty;
                }
            }

            private bool ShowQuantityWarning
            {
                get
                {
                    return Orders.SelectMany(o => o.GetOrderSerialNumberRows()).Count(s => s.IsValidState() && s.Active) !=
                           Orders.Sum(o => o.IsPartQuantityNull() ? 0 : o.PartQuantity);
                }
            }

            public Visibility ErrorMessageVisibility
                => string.IsNullOrEmpty(Error) ? Visibility.Collapsed : Visibility.Visible;

            #endregion

            #region Methods

            public SplitSerialNumberContext()
            {
                Accept = new AcceptCommand(this);
            }

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public void DoAccept()
            {
                Completed?.Invoke(this, EventArgs.Empty);
            }

            public SerialNumber NewSerialNumber(OrdersDataSet.OrderSerialNumberRow arg)
            {
                if (arg == null)
                {
                    return null;
                }

                return new SerialNumber(this, arg);
            }

            public void Validate()
            {
                OnPropertyChanged(nameof(Error));
                OnPropertyChanged(nameof(ErrorMessageVisibility));
                OnPropertyChanged(nameof(Warning));
                OnPropertyChanged(nameof(WarningMessageVisibility));
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            #region IDataErrorInfo Members

            public string Error
            {
                get
                {
                    if (ShowQuantityWarning)
                    {
                        return null;
                    }

                    var errorMsgs = new List<string>();

                    foreach (var order in Orders)
                    {
                        var partQty = order.IsPartQuantityNull() ? 0 : order.PartQuantity;
                        var serialNumberCount = SerialNumbers.Count(n => n.OrderId == order.OrderID);

                        if (partQty > serialNumberCount)
                        {
                            var difference = partQty - serialNumberCount;
                            var errorMsg = difference > 1
                                ? $"Please move {difference} serial numbers to {order.OrderID}."
                                : $"Please move 1 serial number to {order.OrderID}.";

                            errorMsgs.Add(errorMsg);
                        }
                        else if (partQty < serialNumberCount)
                        {
                            errorMsgs.Add($"{order.OrderID} has too many serial numbers.");
                        }
                    }

                    return string.Join("\n", errorMsgs);
                }
            }

            public string this[string columnName] => null;

            #endregion
        }

        #endregion

        #region SerialNumber

        private class SerialNumber : INotifyPropertyChanged
        {
            #region Fields

            private int _orderId;

            #endregion

            #region Properties

            public string Number => OriginalRow.IsNumberNull() ? null : OriginalRow.Number;

            public int OrderId
            {
                get { return _orderId; }
                set
                {
                    if (_orderId != value)
                    {
                        _orderId = value;
                        OnPropertyChanged(nameof(OrderId));
                        ParentContext.Validate();
                    }
                }
            }

            public SplitSerialNumberContext ParentContext { get; set; }

            public OrdersDataSet.OrderSerialNumberRow OriginalRow { get; }

            #endregion

            #region Methods

            public SerialNumber(SplitSerialNumberContext parentContext, OrdersDataSet.OrderSerialNumberRow row)
            {
                if (parentContext == null)
                {
                    throw new ArgumentNullException(nameof(parentContext));
                }

                if (row == null)
                {
                    throw new ArgumentNullException(nameof(row));
                }

                ParentContext = parentContext;
                OriginalRow = row;
                OrderId = row.OrderID;
            }

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region AcceptCommand

        private class AcceptCommand : ICommand
        {
            public SplitSerialNumberContext Context { get; }

            #region Methods

            public AcceptCommand(SplitSerialNumberContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                Context = context;
            }

            #endregion

            #region ICommand Members

            public void Execute(object parameter)
            {
                if (!CanExecute(parameter))
                {
                    return;
                }

                Context.DoAccept();
            }

            public bool CanExecute(object parameter)
            {
                return string.IsNullOrEmpty(Context.Error);
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            #endregion
        }

        #endregion
    }
}