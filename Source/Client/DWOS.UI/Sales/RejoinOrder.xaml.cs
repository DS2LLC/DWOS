using System;
using System.Collections.Generic;
using DWOS.Data.Datasets;
using System.ComponentModel;
using NLog;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DWOS.Data.Datasets.ListsDataSetTableAdapters;
using DWOS.UI.Utilities;

namespace DWOS.UI.Sales
{
    /// <summary>
    /// Interaction logic for RejoinOrder.xaml
    /// </summary>
    public partial class RejoinOrder
    {
        #region Properties

        public bool PrintTraveler
        {
            get { return ViewModel.PrintTraveler; }
            set { ViewModel.PrintTraveler = value; }
        }

        public int DestinationOrderId => ViewModel?.SelectedOrderId ?? -1;

        public int OrderChangeReasonId => ViewModel?.SelectedReason?.OrderChangeReasonId ?? -1;

        #endregion

        private RejoinOrderDataContext ViewModel => DataContext as RejoinOrderDataContext;

        #region Methods

        public RejoinOrder()
        {
            InitializeComponent();
            DataContext = new RejoinOrderDataContext();
            Icon = Properties.Resources.Order_Join_32.ToWpfImage();
        }

        public void Load(OrdersDataSet.OrderRow orgOrder, ICollection<OrdersDataSet.OrderRow> list)
        {
            try
            {
                ViewModel?.Load(orgOrder, list);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading rejoin order data.");
            }
        }

        #endregion

        #region Events

        private void RejoinOrder_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;

            if (vm == null)
            {
                return;
            }

            vm.Completed += VmOnCompleted;
        }

        private void RejoinOrder_OnUnloaded(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;

            if (vm == null)
            {
                return;
            }

            vm.Completed -= VmOnCompleted;
        }

        private void VmOnCompleted(object sender, EventArgs eventArgs)
        {
            DialogResult = true;
        }

        #endregion

        #region RejoinOrderDataContext

        private class RejoinOrderDataContext : INotifyPropertyChanged, IDataErrorInfo
        {
            #region Fields

            public event EventHandler Completed;
            private bool _printTraveler;
            private int _sourceOrderId;
            private int? _selectedOrderId;
            private Reason _selectedReason;

            #endregion

            #region Properties

            public bool PrintTraveler
            {
                get { return _printTraveler; }
                set
                {
                    if (_printTraveler != value)
                    {
                        _printTraveler = value;
                        OnPropertyChanged(nameof(PrintTraveler));
                    }
                }
            }

            public List<int> OrderIds { get; } = new List<int>();

            public List<Reason> Reasons { get; } = new List<Reason>();

            public int SourceOrderId
            {
                get { return _sourceOrderId; }
                set
                {
                    if (_sourceOrderId != value)
                    {
                        _sourceOrderId = value;
                        OnPropertyChanged(nameof(SourceOrderId));
                    }
                }
            }

            public int? SelectedOrderId
            {
                get { return _selectedOrderId; }
                set
                {
                    if (_selectedOrderId != value)
                    {
                        _selectedOrderId = value;
                        OnPropertyChanged(nameof(SelectedOrderId));
                    }
                }
            }

            public Reason SelectedReason
            {
                get { return _selectedReason; }
                set
                {
                    if (_selectedReason != value)
                    {
                        _selectedReason = value;
                        OnPropertyChanged(nameof(SelectedReason));
                    }
                }
            }

            public ICommand Accept { get; }

            #endregion

            #region Methods

            public RejoinOrderDataContext()
            {
                Accept = new AcceptCommand(this);
            }

            public void Load(OrdersDataSet.OrderRow orgOrder, ICollection<OrdersDataSet.OrderRow> rejoinableOrders)
            {
                SourceOrderId = orgOrder.OrderID;

                OrderIds.Clear();
                OrderIds.AddRange(rejoinableOrders.Select(o => o.OrderID).OrderBy(id => id));
                SelectedOrderId = OrderIds.Cast<int?>().FirstOrDefault();

                Reasons.Clear();

                using (var taOrderChangeReason = new d_OrderChangeReasonTableAdapter())
                {
                    using (var data = taOrderChangeReason.GetDataByChangeType((int) OrderChangeType.Rejoin))
                    {
                        foreach (var reason in data)
                        {
                            Reasons.Add(new Reason(reason.OrderChangeReasonID, reason.Name));
                        }
                    }
                }

                SelectedReason = Reasons.FirstOrDefault();
            }

            public void DoAccept()
            {
                Completed?.Invoke(this, EventArgs.Empty);
            }

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            #region IDataErrorInfo

            public string Error
            {
                get
                {
                    if (!_selectedOrderId.HasValue || _selectedReason == null)
                    {
                        return "There is a problem with one or more fields.";
                    }

                    return string.Empty;
                }
            }

            public string this[string columnName]
            {
                get
                {
                    if (columnName == nameof(SelectedOrderId) && !_selectedOrderId.HasValue)
                    {
                        return "Select an order to rejoin to.";
                    }

                    if(columnName == nameof(SelectedReason) && _selectedReason == null)
                    {
                        return "Select a rejoin reason.";
                    }

                    return string.Empty;
                }
            }

            #endregion
        }

        #endregion

        #region Reason

        private class Reason
        {
            public int OrderChangeReasonId { get; }

            public string Name { get; }

            public Reason(int orderChangeReasonId, string name)
            {
                OrderChangeReasonId = orderChangeReasonId;
                Name = name;
            }
        }

        #endregion

        #region AcceptCommand

        private class AcceptCommand : ICommand
        {
            #region Properties

            public RejoinOrderDataContext Context { get; }

            #endregion

            #region Methods

            public AcceptCommand(RejoinOrderDataContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                Context = context;
            }

            #endregion

            #region ICommand Members

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public bool CanExecute(object parameter)
            {
                return string.IsNullOrEmpty(Context.Error);
            }

            public void Execute(object parameter)
            {
                if (!CanExecute(parameter))
                {
                    return;
                }

                Context.DoAccept();
            }

            #endregion
        }

        #endregion
    }
}