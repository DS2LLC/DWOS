using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace DWOS.UI.Utilities.Calculators
{
    /// <summary>
    /// Represents data used by <see cref="LaborWindow"/>.
    /// </summary>
    public sealed class LaborContext : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Fields

        /// <summary>
        /// Raised when the window should close successfully.
        /// </summary>
        public event EventHandler Accept;

        private BindingList<LaborItem> _laborItems;
        private object[] _selectedItems;
        private decimal _hourlyLaborCost;
        private decimal _partsPerHour;


        #endregion

        #region Properties

        /// <summary>
        /// Gets a collection of labor items.
        /// </summary>
        public IList<LaborItem> LaborItems
        {
            get
            {
                return _laborItems;
            }
        }

        /// <summary>
        /// Gets or sets an array of selected items
        /// </summary>
        /// <remarks>
        /// Used by XamDataGrid; this helps explain the weird return type.
        /// </remarks>
        public object[] SelectedItems
        {
            get
            {
                return _selectedItems;
            }
            set
            {
                if (_selectedItems != value)
                {
                    _selectedItems = value;
                    OnPropertyChanged(nameof(SelectedItems));
                }
            }
        }

        /// <summary>
        /// Gets or sets a rate.
        /// </summary>
        /// <remarks>
        /// The rate should have been calculated in a previous step.
        /// </remarks>
        public decimal PartsPerHour
        {
            get
            {
                return _partsPerHour;
            }
            set
            {
                if (_partsPerHour != value)
                {
                    _partsPerHour = value;
                    OnPropertyChanged(nameof(PartsPerHour));
                    OnPropertyChanged(nameof(HourlyLaborCostPerPart));
                }
            }
        }

        /// <summary>
        /// Gets the hourly labor cost.
        /// </summary>
        public decimal HourlyLaborCost
        {
            get
            {
                return _hourlyLaborCost;
            }
        }

        /// <summary>
        /// Gets the hourly labor cost per part.
        /// </summary>
        public decimal HourlyLaborCostPerPart
        {
            get
            {
                if (_partsPerHour == 0)
                {
                    return 0M;
                }

                return _hourlyLaborCost / Convert.ToDecimal(_partsPerHour);
            }
        }

        public ICommand AddCommand
        {
            get;
            private set;
        }

        public ICommand RemoveCommand
        {
            get;
            private set;
        }

        public ICommand AcceptCommand
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="LaborContext"/> class.
        /// </summary>
        public LaborContext()
        {
            _laborItems = new BindingList<LaborItem>();
            AddCommand = new AddItemCommand(this);
            RemoveCommand = new RemoveItemCommand(this);
            AcceptCommand = new AcceptDialogCommand(this);
            _laborItems.ListChanged += LaborItems_ListChanged;
        }

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Events

        private void LaborItems_ListChanged(object sender, ListChangedEventArgs e)
        {
            _hourlyLaborCost = _laborItems.Sum(i => i.Wage * Convert.ToDecimal(i.Count));
            OnPropertyChanged(nameof(HourlyLaborCost));
            OnPropertyChanged(nameof(HourlyLaborCostPerPart));
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
                return this[nameof(HourlyLaborCost)] +
                    this[nameof(HourlyLaborCostPerPart)] +
                    this[nameof(PartsPerHour)];
            }
        }

        public string this[string columnName]
        {
            get
            {
                const decimal maxCost = 999999.9999M;
                const decimal minCost = 0;
                string returnValue = string.Empty;
                if (columnName == nameof(HourlyLaborCost))
                {
                    if (_hourlyLaborCost > maxCost)
                    {
                        return "Hourly Labor Cost exceeds maximum value.";
                    }
                    else if (_hourlyLaborCost < minCost)
                    {
                        return "Hourly Labor Cost cannot be negative.";
                    }
                }
                else if (columnName == nameof(HourlyLaborCostPerPart))
                {
                    var costPerPart = HourlyLaborCostPerPart;
                    if (costPerPart > maxCost)
                    {
                        return "Cost Per Part exceeds maximum value.";
                    }
                    else if (costPerPart < minCost)
                    {
                        return "Cost Per Part cannot be negative.";
                    }
                    else if (_partsPerHour == 0)
                    {
                        return "Parts Per Hour is 0 - cannot calculate Cost Per Part.";
                    }
                }
                else if (columnName == nameof(PartsPerHour) && _partsPerHour == 0)
                {
                    return "Parts Per Hour cannot be 0.";
                }

                return returnValue;
            }
        }

        #endregion

        #region LaborItem

        /// <summary>
        /// Represents a category of labor.
        /// </summary>
        public sealed class LaborItem : INotifyPropertyChanged
        {
            #region Fields

            private string _laborType;
            private decimal _wage;
            private int _count;

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets the name of the labor item.
            /// </summary>
            public string LaborType
            {
                get
                {
                    return _laborType;
                }

                set
                {
                    if (_laborType != value)
                    {
                        _laborType = value;
                        OnPropertyChanged(nameof(LaborType));
                    }
                }
            }

            /// <summary>
            /// Gets or sets a value representing per-person, per-hour wage.
            /// </summary>
            public decimal Wage
            {
                get
                {
                    return _wage;
                }

                set
                {
                    if (_wage != value)
                    {
                        _wage = value;
                        OnPropertyChanged(nameof(Wage));
                    }
                }
            }

            /// <summary>
            /// Gets or sets the number of people in the labor category.
            /// </summary>
            public int Count
            {
                get
                {
                    return _count;
                }

                set
                {
                    if (_count != value)
                    {
                        _count = value;
                        OnPropertyChanged(nameof(Count));
                    }
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="LaborItem"/> class.
            /// </summary>
            public LaborItem()
            {
                _count = 1;
            }

            private void OnPropertyChanged(string propertyName)
            {
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

        #region AddItemCommand

        private sealed class AddItemCommand : ICommand
        {
            #region Properties

            public LaborContext Instance
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public AddItemCommand(LaborContext instance)
            {
                Instance = instance;
            }

            #endregion

            #region ICommand Members

            public event EventHandler CanExecuteChanged
            {
                add { }
                remove { }
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                if (Instance?._laborItems == null)
                {
                    return;
                }

                Instance._laborItems.Add(new LaborItem());
            }

            #endregion
        }

        #endregion

        #region RemoveItemCommand

        private sealed class RemoveItemCommand : ICommand
        {
            #region Properties

            public LaborContext Instance
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public RemoveItemCommand(LaborContext instance)
            {
                Instance = instance;
                Instance.PropertyChanged += Instance_PropertyChanged;
            }

            #endregion

            #region Events

            private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(Instance.SelectedItems))
                {
                    var handler = CanExecuteChanged;

                    if (handler != null)
                    {
                        handler(this, new EventArgs());
                    }
                }
            }

            #endregion

            #region ICommand Members

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                object[] items = Instance?.SelectedItems;

                return items != null && items.Length > 0;
            }

            public void Execute(object parameter)
            {

                object[] items = Instance?.SelectedItems;
                if (items == null || items.Length == 0)
                {
                    return;
                }

                var selectedLaborItem = items[0] as LaborItem;

                if (selectedLaborItem != null)
                {
                    Instance._laborItems.Remove(selectedLaborItem);
                    Instance.SelectedItems = null;
                }
            }

            #endregion
        }

        #endregion

        #region AcceptDialogCommand

        private sealed class AcceptDialogCommand : ICommand
        {
            #region Properties

            public LaborContext Instance
            {
                get;
                private set;
            }
            #endregion

            #region Methods

            public AcceptDialogCommand(LaborContext instance)
            {
                Instance = instance;
                Instance.PropertyChanged += Instance_PropertyChanged;
            }

            #endregion

            #region Events

            private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
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
                return string.IsNullOrEmpty(Instance?.Error);
            }

            public void Execute(object parameter)
            {
                if (!string.IsNullOrEmpty(Instance?.Error))
                {
                    return;
                }

                var handler = Instance.Accept;

                if (handler != null)
                {
                    handler(Instance, new EventArgs());
                }
            }

            #endregion
        }

        #endregion
    }
}
