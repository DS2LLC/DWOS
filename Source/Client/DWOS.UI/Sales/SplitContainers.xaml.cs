using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using Infragistics.Windows.Editors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DWOS.UI.Sales
{
    /// <summary>
    /// Interaction logic for SplitContainers.xaml
    /// </summary>
    public partial class SplitContainers : Window
    {
        #region Fields

        private SplitContainersContext _model;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a collection containing the original work order and split work orders.
        /// </summary>
        public IEnumerable<Order> Orders
        {
            get
            {
                return _model.Orders;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Instantiates a new instance of the <see cref="SplitContainers"/> class.
        /// </summary>
        public SplitContainers()
        {
            InitializeComponent();
            _model = new SplitContainersContext();
            DataContext = _model;
            (Resources["packageTypes"] as ComboBoxItemsProvider).ItemsSource = _model.PackageTypes;
        }

        /// <summary>
        /// Loads order data into this instance.
        /// </summary>
        /// <remarks>
        /// This order data does not need to be persisted in the database.
        /// </remarks>
        /// <param name="originalOrderID">The OrderID for the order being split.</param>
        /// <param name="orders">A collection of orders to show containers for.</param>
        /// <param name="originalPartQuantity">The initial part quantity for the order being split.</param>
        public void Load(int originalOrderID, int originalPartQuantity, IEnumerable<OrdersDataSet.OrderRow> orders, OrdersDataSet.ShipmentPackageTypeDataTable dtPackageType)
        {

            _model.Load(originalOrderID, originalPartQuantity, orders, dtPackageType);
        }

        /// <summary>
        /// Syncs dialog data with a data table.
        /// </summary>
        /// <param name="dtContainers">Data table to use for persisting order containers.</param>
        /// <param name="dtContainerItems">Data table to use for persistence order container items.</param>
        public void Sync(OrdersDataSet.OrderContainersDataTable dtContainers, OrdersDataSet.OrderContainerItemDataTable dtContainerItems)
        {
            if (dtContainers == null)
            {
                throw new ArgumentNullException("dtContainers", "dtContainers cannot be null");
            }

            if (dtContainerItems == null)
            {
                throw new ArgumentNullException(nameof(dtContainerItems));
            }

            foreach (var removedContainerID in _model.ContainersToDelete)
            {
                var removedContainerRow = dtContainers.FindByOrderContainerID(removedContainerID);

                if (removedContainerRow != null)
                {
                    removedContainerRow.Delete();
                    // Items from the container are automatically removed
                }
            }

            foreach (var removedContainerItemId in _model.ContainerItemsToDelete)
            {
                var removedItemRow = dtContainerItems.FindByOrderContainerItemID(removedContainerItemId);
                removedItemRow?.Delete();
            }

            foreach (var order in _model.Orders)
            {
                foreach (var container in order.Containers)
                {
                    if (container.IsNew)
                    {
                        var newRow = dtContainers.NewOrderContainersRow();
                        newRow.OrderID = order.OrderID;
                        newRow.PartQuantity = container.PartQuantity;
                        newRow.IsActive = container.IsActive;
                        newRow.ShipmentPackageTypeID = container.PackageType?.ShipmentPackageTypeId ?? 1;

                        if (container.Weight.HasValue)
                        {
                            newRow.Weight = container.Weight.Value;
                        }
                        else
                        {
                            newRow.SetWeightNull();
                        }

                        dtContainers.AddOrderContainersRow(newRow);

                        // Assumption: Every container item is new
                        foreach (var containerItem in container.Items)
                        {
                            dtContainerItems.AddOrderContainerItemRow(newRow, containerItem.PackageType?.ShipmentPackageTypeId ?? 1);
                        }

                    }
                    else if (container.OrderContainerID.HasValue)
                    {
                        var existingRow = dtContainers.FindByOrderContainerID(container.OrderContainerID.Value);

                        if (existingRow != null)
                        {
                            if (existingRow.PartQuantity != container.PartQuantity)
                            {
                                existingRow.PartQuantity = container.PartQuantity;
                            }

                            if (existingRow.IsActive != container.IsActive)
                            {
                                existingRow.IsActive = container.IsActive;
                            }

                            if (existingRow.IsWeightNull() != container.Weight.HasValue)
                            {
                                if (!existingRow.IsWeightNull() && existingRow.Weight != container.Weight.Value)
                                {
                                    existingRow.Weight = container.Weight.Value;
                                }
                            }
                            else if (container.Weight.HasValue)
                            {
                                existingRow.Weight = container.Weight.Value;
                            }
                            else
                            {
                                existingRow.SetWeightNull();
                            }

                            var shipmentPackageTypeId = container.PackageType?.ShipmentPackageTypeId ?? 1;

                            if (existingRow.ShipmentPackageTypeID != shipmentPackageTypeId)
                            {
                                existingRow.ShipmentPackageTypeID = shipmentPackageTypeId;
                            }

                            foreach (var containerItem in container.Items)
                            {
                                if (containerItem.OrderContainerItemID.HasValue)
                                {
                                    // Existing item
                                    var existingItemRow = dtContainerItems.FindByOrderContainerItemID(containerItem.OrderContainerItemID.Value);

                                    if (existingItemRow != null && existingItemRow.ShipmentPackageTypeID != (containerItem.PackageType?.ShipmentPackageTypeId ?? 1))
                                    {
                                        existingItemRow.ShipmentPackageTypeID = (containerItem.PackageType?.ShipmentPackageTypeId ?? 1);
                                    }
                                }
                                else
                                {
                                    // New item
                                    dtContainerItems.AddOrderContainerItemRow(existingRow, containerItem.PackageType?.ShipmentPackageTypeId ?? 1);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Events

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            string msg = "Do you want to skip this step?\n(Containers will not be split, but Split Order will continue.)";

            var boxResult = MessageBoxUtilities.ShowMessageBoxYesOrNo(msg, "Split Containers");

            if (boxResult == System.Windows.Forms.DialogResult.Yes)
            {
                DialogResult = false;
            }
        }

        private void mainGrid_InitializeRecord(object sender, Infragistics.Windows.DataPresenter.Events.InitializeRecordEventArgs e)
        {
            // Expand all items by default
            e.Record.IsExpanded = true;
        }

        #endregion

        #region SplitContainersContext

        /// <summary>
        /// Data Context for <see cref="SplitContainers"/>.
        /// </summary>
        private sealed class SplitContainersContext : INotifyPropertyChanged
        {
            #region Fields

            private int _originalOrderID;
            private int _originalPartQuantity;
            private object[] _selectedRecords;

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets selected records.
            /// </summary>
            public object[] SelectedRecords
            {
                get
                {
                    return _selectedRecords;
                }
                set
                {
                    if (_selectedRecords != value)
                    {
                        _selectedRecords = value;
                        OnPropertyChanged(nameof(SelectedRecords));
                    }
                }
            }

            public List<Order> Orders { get; } = new List<Order>();

            public ObservableCollection<PackageType> PackageTypes { get; } =
                new ObservableCollection<PackageType>();

            /// <summary>
            /// Gets a list containing OrderContainerIDs that should be deleted when saving.
            /// </summary>
            public List<int> ContainersToDelete { get; } = new List<int>();

            /// <summary>
            /// Gets a list container OrderContainerItemIDs that should be deleted when saving.
            /// </summary>
            public List<int> ContainerItemsToDelete { get; } = new List<int>();

            public int OriginalOrderID
            {
                get
                {
                    return _originalOrderID;
                }
                set
                {
                    if (_originalOrderID != value)
                    {
                        _originalOrderID = value;
                        OnPropertyChanged(nameof(OriginalOrderID));
                    }
                }
            }

            public int OriginalPartQuantity
            {
                get
                {
                    return _originalPartQuantity;
                }
                set
                {
                    if (_originalPartQuantity != value)
                    {
                        _originalPartQuantity = value;
                        OnPropertyChanged(nameof(OriginalPartQuantity));
                    }
                }
            }

            public ICommand AddCommand
            {
                get;
                private set;
            }

            public ICommand AddItemCommand { get; }

            public ICommand RemoveCommand
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public SplitContainersContext()
            {
                AddCommand = new AddCommand(this);
                AddItemCommand = new RelayCommand(DoAddItem, CanAddItem);
                RemoveCommand = new RemoveCommand(this);
            }

            private void DoAddItem()
            {
                if (SelectedRecords == null || SelectedRecords.Length != 1)
                {
                    return;
                }

                if (SelectedRecords[0] is Container selectedContainer)
                {
                    selectedContainer.Items.Add(new ContainerItem(selectedContainer, PackageTypes.ToList()));
                }
            }

            private bool CanAddItem() => SelectedRecords != null &&
                    SelectedRecords.Length == 1 &&
                    SelectedRecords[0] is Container;

            private void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            public void Load(int originalOrderID, int originalPartQuantity, IEnumerable<OrdersDataSet.OrderRow> orders, OrdersDataSet.ShipmentPackageTypeDataTable dtPackageType)
            {
                if (orders == null)
                {
                    throw new ArgumentNullException(nameof(orders));
                }

                if (dtPackageType == null)
                {
                    throw new ArgumentNullException(nameof(dtPackageType));
                }

                PackageTypes.Clear();
                var packageTypes = dtPackageType.Select(row => new PackageType(row)).ToList();

                foreach (var packageType in packageTypes)
                {
                    PackageTypes.Add(packageType);
                }

                OriginalOrderID = originalOrderID;
                OriginalPartQuantity = originalPartQuantity;

                foreach (var order in orders)
                {
                    Orders.Add(new Order(order, packageTypes));
                }
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region Order

        public sealed class Order
        {
            #region Properties

            public int OrderID
            {
                get;
                private set;
            }

            public int PartQuantity
            {
                get;
                private set;
            }

            public BindingList<Container> Containers { get; } = new BindingList<Container>();

            #endregion

            #region Methods

            public Order(OrdersDataSet.OrderRow dataRow, List<PackageType> packageTypes)
            {
                if (dataRow == null)
                {
                    throw new ArgumentNullException("dataRow", "dataRow cannot be null");
                }

                OrderID = dataRow.OrderID;
                PartQuantity = dataRow.PartQuantity;

                foreach (var containerRow in dataRow.GetOrderContainersRows())
                {
                    Containers.Add(new Container(containerRow, packageTypes));
                }
            }

            #endregion
        }

        #endregion

        #region Container

        public sealed class Container : INotifyPropertyChanged
        {
            #region Fields

            private int _partQuantity;
            private bool _isActive;
            private decimal? _weight;
            private bool _hasChanged;
            private PackageType _packageType;

            #endregion

            #region Properties

            public int? OrderContainerID
            {
                get;
                private set;
            }

            public int OrderID
            {
                get;
                private set;
            }

            public int PartQuantity
            {
                get
                {
                    return _partQuantity;
                }
                set
                {
                    if (_partQuantity != value)
                    {
                        _partQuantity = value;
                        OnPropertyChanged(nameof(PartQuantity));
                    }
                }
            }

            public bool IsActive
            {
                get
                {
                    return _isActive;
                }
                set
                {
                    if (_isActive != value)
                    {
                        _isActive = value;
                        OnPropertyChanged(nameof(IsActive));
                    }
                }
            }

            public bool HasChanged
            {
                get
                {
                    return _hasChanged;
                }
                set
                {
                    if (_hasChanged != value)
                    {
                        _hasChanged = value;
                        OnPropertyChanged(nameof(HasChanged));
                    }
                }
            }

            public decimal? Weight
            {
                get
                {
                    return _weight;
                }
                set
                {
                    if (_weight != value)
                    {
                        _weight = value;
                        OnPropertyChanged(nameof(Weight));
                    }
                }
            }

            public bool IsNew
            {
                get
                {
                    return !OrderContainerID.HasValue;
                }
            }

            public PackageType PackageType
            {
                get => _packageType;
                set
                {
                    if (_packageType != value)
                    {
                        _packageType = value;
                        OnPropertyChanged(nameof(PackageType));
                    }
                }
            }

            public BindingList<ContainerItem> Items { get; }

            #endregion

            #region Methods

            public Container(int orderID, List<PackageType> packageTypes)
            {
                OrderContainerID = null;
                OrderID = orderID;
                _partQuantity = 1;
                _isActive = true;
                _weight = null;
                _packageType = packageTypes.FirstOrDefault(type => type.ShipmentPackageTypeId == 1);
                Items = new BindingList<ContainerItem>();
            }

            public Container(OrdersDataSet.OrderContainersRow dataRow, List<PackageType> packageTypes)
            {
                if (dataRow == null)
                {
                    throw new ArgumentNullException("dataRow", "dataRow cannot be null");
                }

                OrderContainerID = dataRow.OrderContainerID;
                OrderID = dataRow.OrderID;
                _partQuantity = dataRow.PartQuantity;
                _isActive = dataRow.IsActive;
                _weight = dataRow.IsWeightNull() ? (decimal?)null : dataRow.Weight;
                _packageType = packageTypes.FirstOrDefault(type => type.ShipmentPackageTypeId == dataRow.ShipmentPackageTypeID);

                var containerItems = dataRow
                    .GetOrderContainerItemRows()
                    .Select(item => new ContainerItem(item, this, packageTypes))
                    .ToList();

                Items = new BindingList<ContainerItem>(containerItems);
            }

            private void OnPropertyChanged(string propertyName)
            {
                if (propertyName != nameof(HasChanged))
                {
                    HasChanged = true;
                }

                var handler = PropertyChanged;

                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region ContainerItem

        public class ContainerItem : INotifyPropertyChanged
        {
            #region Fields

            private PackageType _packageType;
            #endregion

            #region Properties

            public int? OrderContainerItemID { get; }

            public PackageType PackageType
            {
                get => _packageType;
                set
                {
                    if (_packageType != value)
                    {
                        _packageType = value;
                        OnPropertyChanged(nameof(PackageType));
                    }
                }
            }

            public Container Parent { get; }

            public bool HasChanged { get; private set; }


            #endregion

            #region Methods

            public ContainerItem(Container parent, List<PackageType> packageTypes)
            {
                Parent = parent ?? throw new ArgumentNullException(nameof(parent));
                OrderContainerItemID = null;
                _packageType = packageTypes.FirstOrDefault(type => type.ShipmentPackageTypeId == 1);
            }

            public ContainerItem(OrdersDataSet.OrderContainerItemRow dataRow, Container parent, List<PackageType> packageTypes)
            {
                Parent = parent ?? throw new ArgumentNullException(nameof(parent));
                if (dataRow == null)
                {
                    throw new ArgumentNullException(nameof(dataRow));
                }

                OrderContainerItemID = dataRow.OrderContainerItemID;
                _packageType = packageTypes.FirstOrDefault(type => type.ShipmentPackageTypeId == dataRow.ShipmentPackageTypeID);
            }

            private void OnPropertyChanged(string propertyName)
            {
                if (propertyName != nameof(HasChanged))
                {
                    HasChanged = true;
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region PackageType

        public class PackageType
        {
            #region Properties

            public OrdersDataSet.ShipmentPackageTypeRow Row { get; }

            public int ShipmentPackageTypeId => Row.ShipmentPackageTypeID;

            public string Name => Row.Name;

            #endregion

            #region Methods

            public PackageType(OrdersDataSet.ShipmentPackageTypeRow row)
            {
                Row = row ?? throw new ArgumentNullException(nameof(row));
            }

            // DisplayMemberPath doesn't seem to be working
            public override string ToString() => Name;

            #endregion
        }

        #endregion

        #region AddCommand

        /// <summary>
        /// Implementation of the 'add container' command.
        /// </summary>
        private class AddCommand : ICommand
        {
            #region Properties

            public SplitContainersContext Context
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Instantiates a new instance of the <see cref="AddCommand"/> class.
            /// </summary>
            /// <param name="context">Dialog data context.</param>
            public AddCommand(SplitContainersContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context", "context cannot be null");
                }

                Context = context;
                context.PropertyChanged += Context_PropertyChanged;
            }

            #endregion

            #region Events

            private void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != nameof(Context.SelectedRecords))
                {
                    return;
                }

                var handler = CanExecuteChanged;

                if (handler != null)
                {
                    handler(this, new EventArgs());
                }
            }
            #endregion

            #region ICommand Members

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return Context.SelectedRecords != null &&
                    Context.SelectedRecords.Length == 1 &&
                    Context.SelectedRecords[0] is Order;
            }

            public void Execute(object parameter)
            {
                if (Context.SelectedRecords == null ||
                    Context.SelectedRecords.Length != 1)
                {
                    return;
                }

                var selectedOrder = Context.SelectedRecords[0] as Order;

                if (selectedOrder != null)
                {
                    selectedOrder.Containers.Add(new Container(selectedOrder.OrderID, Context.PackageTypes.ToList()));
                }
            }

            #endregion
        }


        #endregion

        #region RemoveCommand

        /// <summary>
        /// Implementation of the 'remove container' command.
        /// </summary>
        private class RemoveCommand : ICommand
        {
            #region Properties

            public SplitContainersContext Context
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Instantiates a new instance of the <see cref="RemoveCommand"/> class.
            /// </summary>
            /// <param name="context">Dialog data context.</param>
            public RemoveCommand(SplitContainersContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context", "context cannot be null");
                }

                Context = context;
                context.PropertyChanged += Context_PropertyChanged;
            }

            #endregion

            #region Events

            private void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != nameof(Context.SelectedRecords))
                {
                    return;
                }

                var handler = CanExecuteChanged;

                if (handler != null)
                {
                    handler(this, new EventArgs());
                }
            }

            #endregion

            #region ICommand Members

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return Context.SelectedRecords != null &&
                    Context.SelectedRecords.Length == 1 &&
                    (Context.SelectedRecords[0] is Container || Context.SelectedRecords[0] is ContainerItem);
            }

            public void Execute(object parameter)
            {
                // Find container
                if (Context.SelectedRecords == null ||
                    Context.SelectedRecords.Length != 1)
                {
                    return;
                }

                if (Context.SelectedRecords[0] is Container selectedContainer)
                {
                    var order = Context.Orders.FirstOrDefault(o => o.OrderID == selectedContainer.OrderID);

                    if (order != null)
                    {
                        order.Containers.Remove(selectedContainer);

                        if (!selectedContainer.IsNew && selectedContainer.OrderContainerID.HasValue)
                        {
                            Context.ContainersToDelete.Add(selectedContainer.OrderContainerID.Value);
                        }
                    }
                }
                else if (Context.SelectedRecords[0] is ContainerItem selectedContainerItem)
                {
                    var container = selectedContainerItem.Parent;

                    if (container != null)
                    {
                        container.Items.Remove(selectedContainerItem);

                        if (selectedContainerItem.OrderContainerItemID.HasValue)
                        {
                            Context.ContainerItemsToDelete.Add(selectedContainerItem.OrderContainerItemID.Value);
                        }
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
