using DWOS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace DWOS.UI.Utilities.Calculators
{
    public sealed class MarkupContext : INotifyPropertyChanged
    {
        #region Fields

        private BindingList<MarkupItem> _markupItems;
        private object[] _selectedItems;
        private decimal _laborCost;
        private decimal _materialCost;
        private decimal _overheadCost;
        private decimal _totalCost;
        private decimal _totalMarkup;

        #endregion

        #region Properties

        public IList<MarkupItem> MarkupItems
        {
            get
            {
                return _markupItems;
            }
        }

        /// <summary>
        /// Gets or sets an array of selected items
        /// </summary>
        /// <remarks>
        /// Used by XamDataGrid's SelectedDataItems property.
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

        public decimal LaborCost
        {
            get
            {
                return _laborCost;
            }
            set
            {
                if (_laborCost != value)
                {
                    _laborCost = value;
                    OnPropertyChanged(nameof(LaborCost));
                    UpdateTotals();
                }
            }
        }

        public decimal MaterialCost
        {
            get
            {
                return _materialCost;
            }
            set
            {
                if (_materialCost != value)
                {
                    _materialCost = value;
                    OnPropertyChanged(nameof(MaterialCost));
                    UpdateTotals();
                }
            }
        }

        public decimal OverheadCost
        {
            get
            {
                return _overheadCost;
            }
            set
            {
                if (_overheadCost != value)
                {
                    _overheadCost = value;
                    OnPropertyChanged(nameof(OverheadCost));
                    UpdateTotals();
                }
            }
        }

        public decimal TotalCost
        {
            get
            {
                return _totalCost;
            }
        }

        public decimal TotalMarkup
        {
            get
            {
                return _totalMarkup;
            }
        }

        public ICommand AddCommand
        {
            get; private set;
        }

        public ICommand RemoveCommand
        {
            get; private set;
        }

        #endregion

        #region Methods

        public MarkupContext()
        {
            _markupItems = new BindingList<MarkupItem>();
            _markupItems.ListChanged += MarkupItems_ListChanged;
            AddCommand = new AddItemCommand(this);
            RemoveCommand = new RemoveItemCommand(this);
        }


        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private void UpdateTotals()
        {

             _totalCost = _laborCost + _materialCost + _overheadCost;
            _totalMarkup = _markupItems.Sum(i => i.CalculateAmount(_totalCost));

            OnPropertyChanged(nameof(TotalCost));
            OnPropertyChanged(nameof(TotalMarkup));
        }

        #endregion

        #region Events

        private void MarkupItems_ListChanged(object sender, ListChangedEventArgs e)
        {
            UpdateTotals();
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region MarkupItem

        public sealed class MarkupItem : INotifyPropertyChanged
        {

            #region Fields

            private string _name;
            private MarkupType _markupType;
            private decimal _amount;

            #endregion

            #region Properties

            public string Name
            {
                get
                {
                    return _name;
                }
                set
                {
                    if (_name != value)
                    {
                        _name = value;
                        OnPropertyChanged(nameof(Name));
                    }
                }
            }

            public MarkupType MarkupType
            {
                get
                {
                    return _markupType;
                }

                set
                {
                    if (_markupType != value)
                    {
                        _markupType = value;
                        OnPropertyChanged(nameof(MarkupType));
                    }
                }
            }

            public decimal Amount
            {
                get
                {
                    return _amount;
                }
                set
                {
                    if (_amount != value)
                    {
                        _amount = value;
                        OnPropertyChanged(nameof(Amount));
                    }
                }
            }

            #endregion

            #region Methods

            private void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;

                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            public decimal CalculateAmount(decimal totalCost)
            {
                if (_markupType == MarkupType.Fixed)
                {
                    return _amount;
                }
                else if (_markupType == MarkupType.Percentage)
                {
                    return (_amount / 100) * totalCost;
                }
                else
                {
                    return 0M;
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

            public MarkupContext Instance
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public AddItemCommand(MarkupContext instance)
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
                if (Instance?._markupItems == null)
                {
                    return;
                }

                Instance._markupItems.Add(new MarkupItem());
            }

            #endregion
        }

        #endregion

        #region RemoveItemCommand

        private sealed class RemoveItemCommand : ICommand
        {
            #region Properties

            public MarkupContext Instance
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public RemoveItemCommand(MarkupContext instance)
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

                var selectedItem = items[0] as MarkupItem;

                if (selectedItem != null)
                {
                    Instance._markupItems.Remove(selectedItem);
                    Instance.SelectedItems = null;
                }
            }

            #endregion
        }

        #endregion
    }
}
