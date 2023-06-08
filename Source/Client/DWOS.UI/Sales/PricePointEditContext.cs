using DWOS.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace DWOS.UI.Sales
{
    /// <summary>
    /// <see cref="IPricePointDialogContext"/> implementer to use when showing
    /// an 'edit (quote) part process pricing options' window.
    /// </summary>
    public sealed class PricePointEditContext : IPricePointDialogContext, INotifyPropertyChanged
    {
        #region Fields

        private IPricePointDialogItem _selectedPricePoint;
        private readonly List<PricePointCategory> _categories;
        private PricePointCategory _selectedCategory;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="PricePointEditContext"/> class.
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="pricePoints"></param>
        private PricePointEditContext(string partName, ObservableCollection<IPricePointDialogItem> pricePoints)
        {
            ItemValue = partName;
            _categories = CreateCategories(pricePoints);
            AcceptCommand = new AcceptCommandImplementation(this);
            AddCommand = new AddCommandImplementation(this);
            DeleteCommand = new DeleteCommandImplementation(this);

            var priceByTypes = new List<PriceByType>();
            foreach (var category in _categories.OrderBy(c => c.PriceByType))
            {
                category.PricePoints.CollectionChanged += PricePoints_CollectionChanged;
                foreach (var point in category.PricePoints)
                {
                    point.PropertyChanged += Point_PropertyChanged;
                }

                priceByTypes.Add(category.PriceByType);
            }

            PriceByOptions = priceByTypes;

            _selectedCategory = _categories.FirstOrDefault();
            _selectedPricePoint = _selectedCategory?.PricePoints.FirstOrDefault();
        }

        private List<PricePointCategory> CreateCategories(ObservableCollection<IPricePointDialogItem> pricePoints)
        {
            var priceByTypes = pricePoints.GroupBy(point => OrderPrice.GetPriceByType(point.CalculateBy));

            var categories = new List<PricePointCategory>();
            foreach (var priceByType in priceByTypes)
            {
                categories.Add(PricePointCategory.Create(priceByType.Key, priceByType));
            }

            return categories;
        }

        /// <summary>
        /// Creates a new <see cref="PricePointEditContext"/> instance using
        /// the given collection.
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="quantityPricePoints"></param>
        /// <param name="weightPricePoints"></param>
        /// <returns></returns>
        public static PricePointEditContext From(string partName, IEnumerable<QuantityPricePointItem> quantityPricePoints, IEnumerable<WeightPricePointItem> weightPricePoints)
        {
            if (quantityPricePoints == null)
            {
                throw new ArgumentNullException(nameof(quantityPricePoints));
            }

            if (weightPricePoints == null)
            {
                throw new ArgumentNullException(nameof(weightPricePoints));
            }

            var orderedQuantityPricePoints = quantityPricePoints
                .OrderBy(point => point.MinValue)
                .OfType<IPricePointDialogItem>();

            var orderedWeightPricePoints = weightPricePoints
                .OrderBy(point => point.MinValue)
                .OfType<IPricePointDialogItem>();

            var contextPoints = new ObservableCollection<IPricePointDialogItem>(orderedQuantityPricePoints.Concat(orderedWeightPricePoints));
            return new PricePointEditContext(partName, contextPoints);
        }

        private void OnAccept()
        {
            try
            {
                if (!IsValid)
                {
                    return;
                }

                var handler = Accept;
                handler?.Invoke(this, new EventArgs());
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error saving quote price point data.");
                var handler = SaveFailed;
                handler?.Invoke(this, new EventArgs());
            }
        }

        private void AddPricePoint()
        {
            if (_selectedCategory == null)
            {
                return;
            }

            IPricePointDialogItem newPricePoint = _selectedCategory.AddNew();

            if (newPricePoint == null)
            {
                LogManager.GetCurrentClassLogger().Error("Category has an invalid price by type.");
            }
            else
            {
                newPricePoint.PropertyChanged += Point_PropertyChanged;
                SelectedPricePoint = newPricePoint;
            }
        }

        private void DeletePricePoint(IPricePointDialogItem pricePoint)
        {
            if (pricePoint == null)
            {
                return;
            }

            _selectedCategory?.PricePoints.Remove(pricePoint);
            pricePoint.PropertyChanged -= Point_PropertyChanged;
            SelectedPricePoint = PricePoints.FirstOrDefault();
        }

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Validate()
        {
            IPricePointDialogItem previous = null;

            var sortedPricePoints = PricePoints.OrderBy(i => i).ToList();
            for (int currentIndex = 0; currentIndex < sortedPricePoints.Count; ++currentIndex)
            {
                var current = sortedPricePoints[currentIndex];

                IPricePointDialogItem next = null;
                if (currentIndex < (sortedPricePoints.Count - 1))
                {
                    next = sortedPricePoints[currentIndex + 1];
                }

                try
                {
                    current.Validate(previous, next);
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc);
                }

                previous = current;
            }
        }

        #endregion

        #region Events

        private void PricePoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Validate();
        }

        private void Point_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.PropertyName) && e.PropertyName != nameof(IPricePointDialogItem.DisplayString))
            {
                Validate();
            }
        }

        #endregion

        #region IPricePointDialogContext Members

        public string Title => "Edit Price Points";

        public string ItemLabel => "Part:";

        public string ItemValue { get; }

        public event EventHandler Accept;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SaveFailed;

        public ICommand AcceptCommand { get; }

        public ICommand AddCommand { get; }

        public ICommand DeleteCommand { get; }

        public bool IsValid => _categories.All(category => category.IsValid);

        public IEnumerable<PriceByType> PriceByOptions { get; }

        public ObservableCollection<IPricePointDialogItem> PricePoints =>
            _selectedCategory?.PricePoints;

        public IEnumerable<IPricePointDialogItem> AllPricePoints =>
            _categories.SelectMany(c => c.PricePoints);

        public PriceByType SelectedPriceByOption
        {
            get => _selectedCategory?.PriceByType ?? PriceByType.Quantity;
            set
            {
                var valueBeforeUpdate = _selectedCategory?.PriceByType ?? PriceByType.Quantity;
                if (valueBeforeUpdate != value)
                {
                    // Update selected category
                    _selectedCategory = _categories.FirstOrDefault(i => i.PriceByType == value);

                    OnPropertyChanged(nameof(SelectedPriceByOption));

                    // Trigger UI refresh for price points
                    OnPropertyChanged(nameof(PricePoints));
                    SelectedPricePoint = PricePoints?.FirstOrDefault();
                }
            }
        }

        public IPricePointDialogItem SelectedPricePoint
        {
            get => _selectedPricePoint;

            set
            {
                if (_selectedPricePoint != value)
                {
                    _selectedPricePoint = value;
                    OnPropertyChanged(nameof(SelectedPricePoint));
                }
            }
        }

        // Users cannot access the 'edit price points' dialog unless
        // the system enables volume discounts.
        public bool UsingVolumeDiscounts => true;

        #endregion

        #region QuantityPricePointItem

        /// <summary>
        /// <see cref="IPricePointDialogItem"/> implementation for use with
        /// <see cref="PricePointEditContext"/>.
        /// </summary>
        /// <remarks>
        /// This class uses <see cref="int"/> for values because it always presents a quantity.
        /// </remarks>
        public sealed class QuantityPricePointItem : IPricePointDialogItem, IComparable<QuantityPricePointItem>
        {
            #region Fields

            private int _minValue;
            private int? _maxValue;
            private OrderPrice.enumPriceUnit _calculateBy;
            private readonly Dictionary<string, string> _errors =
                new Dictionary<string, string>();

            #endregion

            #region Properties

            public int MinValue
            {
                get => _minValue;
                set
                {
                    if (_minValue != value)
                    {
                        _minValue = value;
                        OnPropertyChanged(nameof(MinValue));
                    }
                }
            }

            public int? MaxValue
            {
                get => _maxValue;

                set
                {
                    if (_maxValue != value)
                    {
                        bool triggerHasMaxQuantity = _maxValue.HasValue != value.HasValue;

                        _maxValue = value;
                        OnPropertyChanged(nameof(MaxValue));

                        if (triggerHasMaxQuantity)
                        {
                            OnPropertyChanged(nameof(HasMaxValue));
                            OnPropertyChanged(nameof(EraseMaxValue));
                        }
                    }
                }
            }

            private bool CoversLowestValue => MinValue == 1;

            #endregion

            #region Methods

            public QuantityPricePointItem(OrderPrice.enumPriceUnit calculateBy, int minValue, int? maxValue)
            {
                _calculateBy = calculateBy;
                _minValue = minValue;
                _maxValue = maxValue;
            }

            private bool HasGap(QuantityPricePointItem nextValue)
            {
                if (nextValue == null)
                {
                    throw new ArgumentNullException(nameof(nextValue));
                }

                return !MaxValue.HasValue ||
                    (nextValue.MinValue - MaxValue.Value) != 1;
            }

            private void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                if (propertyName != nameof(DisplayString))
                {
                    OnPropertyChanged(nameof(DisplayString));
                }
            }

            #endregion

            #region IPricePoint Members

            public string this[string columnName]
            {
                get
                {
                    _errors.TryGetValue(columnName, out var value);
                    return value;
                }
            }

            public OrderPrice.enumPriceUnit CalculateBy
            {
                get => _calculateBy;
                set
                {
                    if (_calculateBy != value)
                    {
                        _calculateBy = value;
                        OnPropertyChanged(nameof(CalculateBy));
                    }
                }
            }
            public bool HasMaxValue
            {
                get => _maxValue.HasValue;
                set
                {
                    if (_maxValue.HasValue == value)
                    {
                        return;
                    }

                    if (value)
                    {
                        MaxValue = 0;
                    }
                    else
                    {
                        MaxValue = null;
                    }

                    OnPropertyChanged(nameof(HasMaxValue));
                    OnPropertyChanged(nameof(EraseMaxValue));
                }
            }

            public bool EraseMaxValue
            {
                get => !HasMaxValue;
                set => HasMaxValue = !value;
            }

            public IEnumerable<OrderPrice.enumPriceUnit> CalculateByOptions { get; } =
                OrderPrice.GetPriceUnits(PriceByType.Quantity);

            public string DisplayString
            {
                get
                {
                    int? maxQuantity = _maxValue;

                    string calculateBy;
                    switch (_calculateBy)
                    {
                        case OrderPrice.enumPriceUnit.Each:
                        case OrderPrice.enumPriceUnit.EachByWeight:
                            calculateBy = "Each";
                            break;
                        case OrderPrice.enumPriceUnit.Lot:
                        case OrderPrice.enumPriceUnit.LotByWeight:
                            calculateBy = "Lot";
                            break;
                        default:
                            calculateBy = string.Empty;
                            break;
                    }

                    return maxQuantity.HasValue
                        ? $"{_minValue}-{maxQuantity} ({calculateBy})"
                        : $"{_minValue}+ ({calculateBy})";
                }
            }

            public string EditorFormat => "0";

            public string EditorMask => "nnnnnn";

            public string Error => string.Join(Environment.NewLine, _errors.Values);

            public Type MaxEditorType => typeof(int?);

            public string MaxValueString => _maxValue.ToString();

            public Type MinEditorType => typeof(int);

            public string MinValueString => _minValue.ToString();

            // Unused by per-process pricing editors
            public int OriginalRowId => -1;

            public event PropertyChangedEventHandler PropertyChanged;

            public void Validate(IPricePointDialogItem previous, IPricePointDialogItem next)
            {
                var previousValue = previous as QuantityPricePointItem;
                var nextValue = next as QuantityPricePointItem;

                if (previous != null  && previousValue == null)
                {
                    throw new ArgumentException(@"Expected same type.", nameof(previous));
                }

                if (next != null && nextValue == null)
                {
                    throw new ArgumentException(@"Expected same type.", nameof(next));
                }

                _errors.Clear();
                if (MaxValue.HasValue && MinValue.CompareTo(MaxValue.Value) > 0)
                {
                    _errors.Add(nameof(MinValue), "Minimum value is greater than maximum value");
                }
                else if (previousValue == null && nextValue == null)
                {
                    // This is the only price point.
                    if (!CoversLowestValue)
                    {
                        _errors.Add(nameof(MinValue), "Does not cover all possible values.");
                    }

                    if (MaxValue.HasValue)
                    {
                        _errors.Add(nameof(MaxValue), "Does not cover all possible values.");
                    }
                }
                else if (previousValue != null && nextValue == null)
                {
                    // Last value
                    if (previousValue.HasGap(this))
                    {
                        _errors.Add(nameof(MinValue), "There is a gap between price points.");
                    }

                    if (MaxValue.HasValue)
                    {
                        _errors.Add(nameof(MaxValue), "Cannot specify a maximum value.");
                    }
                }
                else if (previousValue == null)
                {
                    // First value
                    if (!CoversLowestValue)
                    {
                        _errors.Add(nameof(MinValue), "Does not cover all possible values.");
                    }

                    if (HasGap(nextValue))
                    {
                        _errors.Add(nameof(MaxValue), "There is a gap between price points.");
                    }
                }
                else
                {
                    // Middle value
                    if (previousValue.HasGap(this))
                    {
                        _errors.Add(nameof(MinValue), "There is a gap between price points.");
                    }

                    if (HasGap(nextValue))
                    {
                        _errors.Add(nameof(MaxValue), "There is a gap between price points.");
                    }
                }

                OnPropertyChanged(string.Empty); // Trigger all field changes for validation
            }

            public int CompareTo(object obj)
            {
                return CompareTo(obj as QuantityPricePointItem);
            }

            #endregion

            #region IComparable<PricePoint Members>

            public int CompareTo(QuantityPricePointItem other)
            {
                if (other == null)
                {
                    return -1;
                }

                return _minValue.CompareTo(other._minValue);
            }

            #endregion
        }

        #endregion


        #region WeightPricePointItem

        /// <summary>
        /// <see cref="IPricePointDialogItem"/> implementation for use with
        /// <see cref="PricePointEditContext"/>.
        /// </summary>
        /// <remarks>
        /// This class uses <see cref="decimal"/> for values because it always presents a weight.
        /// </remarks>
        public sealed class WeightPricePointItem : IPricePointDialogItem, IComparable<WeightPricePointItem>
        {
            #region Fields

            private decimal _minValue;
            private decimal? _maxValue;
            private OrderPrice.enumPriceUnit _calculateBy;
            private readonly Dictionary<string, string> _errors =
                new Dictionary<string, string>();

            #endregion

            #region Properties

            public decimal MinValue
            {
                get => _minValue;
                set
                {
                    if (_minValue != value)
                    {
                        _minValue = value;
                        OnPropertyChanged(nameof(MinValue));
                    }
                }
            }

            public decimal? MaxValue
            {
                get => _maxValue;

                set
                {
                    if (_maxValue != value)
                    {
                        bool triggerHasMaxQuantity = _maxValue.HasValue != value.HasValue;

                        _maxValue = value;
                        OnPropertyChanged(nameof(MaxValue));

                        if (triggerHasMaxQuantity)
                        {
                            OnPropertyChanged(nameof(HasMaxValue));
                            OnPropertyChanged(nameof(EraseMaxValue));
                        }
                    }
                }
            }

            private bool CoversLowestValue => MinValue == 0M;

            #endregion

            #region Methods

            public WeightPricePointItem(OrderPrice.enumPriceUnit calculateBy, decimal minValue, decimal? maxValue)
            {
                _calculateBy = calculateBy;
                _minValue = minValue;
                _maxValue = maxValue;
            }

            private bool HasGap(WeightPricePointItem nextValue)
            {
                if (nextValue == null)
                {
                    throw new ArgumentNullException(nameof(nextValue));
                }
                if (!MaxValue.HasValue)
                {
                    return true;
                }

                var difference = nextValue.MinValue - MaxValue.Value;

                return !(difference > 0M && difference <= 0.01M);
            }

            private void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                if (propertyName != nameof(DisplayString))
                {
                    OnPropertyChanged(nameof(DisplayString));
                }
            }

            #endregion

            #region IPricePoint Members

            public string this[string columnName]
            {
                get
                {
                    _errors.TryGetValue(columnName, out var value);
                    return value;
                }
            }

            public OrderPrice.enumPriceUnit CalculateBy
            {
                get => _calculateBy;
                set
                {
                    if (_calculateBy != value)
                    {
                        _calculateBy = value;
                        OnPropertyChanged(nameof(CalculateBy));
                    }
                }
            }
            public bool HasMaxValue
            {
                get => _maxValue.HasValue;
                set
                {
                    if (_maxValue.HasValue == value)
                    {
                        return;
                    }

                    if (value)
                    {
                        MaxValue = 0;
                    }
                    else
                    {
                        MaxValue = null;
                    }

                    OnPropertyChanged(nameof(HasMaxValue));
                    OnPropertyChanged(nameof(EraseMaxValue));
                }
            }

            public bool EraseMaxValue
            {
                get => !HasMaxValue;
                set => HasMaxValue = !value;
            }

            public IEnumerable<OrderPrice.enumPriceUnit> CalculateByOptions { get; } =
                OrderPrice.GetPriceUnits(PriceByType.Weight);

            public string DisplayString
            {
                get
                {
                    decimal? maxQuantity = _maxValue;

                    string calculateBy;
                    switch (_calculateBy)
                    {
                        case OrderPrice.enumPriceUnit.Each:
                        case OrderPrice.enumPriceUnit.EachByWeight:
                            calculateBy = "Each";
                            break;
                        case OrderPrice.enumPriceUnit.Lot:
                        case OrderPrice.enumPriceUnit.LotByWeight:
                            calculateBy = "Lot";
                            break;
                        default:
                            calculateBy = string.Empty;
                            break;
                    }

                    return maxQuantity.HasValue
                        ? $"{_minValue}-{maxQuantity} ({calculateBy})"
                        : $"{_minValue}+ ({calculateBy})";
                }
            }

            public string EditorFormat => "0.00 lbs.";

            public string EditorMask => "nnnnnn.nn";

            public string Error => string.Join(Environment.NewLine, _errors.Values);

            public Type MaxEditorType => typeof(decimal?);

            public string MaxValueString => _maxValue.ToString();

            public Type MinEditorType => typeof(decimal);

            public string MinValueString => _minValue.ToString();

            // Unused by per-process pricing editors
            public int OriginalRowId => -1;

            public event PropertyChangedEventHandler PropertyChanged;

            public void Validate(IPricePointDialogItem previous, IPricePointDialogItem next)
            {
                var previousValue = previous as WeightPricePointItem;
                var nextValue = next as WeightPricePointItem;

                if (previous != null && previousValue == null)
                {
                    throw new ArgumentException(@"Expected same type.", nameof(previous));
                }

                if (next != null && nextValue == null)
                {
                    throw new ArgumentException(@"Expected same type.", nameof(next));
                }

                _errors.Clear();
                if (MaxValue.HasValue && MinValue.CompareTo(MaxValue.Value) > 0)
                {
                    _errors.Add(nameof(MinValue), "Minimum value is greater than maximum value");
                }
                else if (previousValue == null && nextValue == null)
                {
                    // This is the only price point.
                    if (!CoversLowestValue)
                    {
                        _errors.Add(nameof(MinValue), "Does not cover all possible values.");
                    }

                    if (MaxValue.HasValue)
                    {
                        _errors.Add(nameof(MaxValue), "Does not cover all possible values.");
                    }
                }
                else if (previousValue != null && nextValue == null)
                {
                    // Last value
                    if (previousValue.HasGap(this))
                    {
                        _errors.Add(nameof(MinValue), "There is a gap between price points.");
                    }

                    if (MaxValue.HasValue)
                    {
                        _errors.Add(nameof(MaxValue), "Cannot specify a maximum value.");
                    }
                }
                else if (previousValue == null)
                {
                    // First value
                    if (!CoversLowestValue)
                    {
                        _errors.Add(nameof(MinValue), "Does not cover all possible values.");
                    }

                    if (HasGap(nextValue))
                    {
                        _errors.Add(nameof(MaxValue), "There is a gap between price points.");
                    }
                }
                else
                {
                    // Middle value
                    if (previousValue.HasGap(this))
                    {
                        _errors.Add(nameof(MinValue), "There is a gap between price points.");
                    }

                    if (HasGap(nextValue))
                    {
                        _errors.Add(nameof(MaxValue), "There is a gap between price points.");
                    }
                }

                OnPropertyChanged(string.Empty); // Trigger all field changes for validation
            }

            public int CompareTo(object obj)
            {
                return CompareTo(obj as WeightPricePointItem);
            }

            #endregion

            #region IComparable<WeightPricePoint> Members

            public int CompareTo(WeightPricePointItem other)
            {
                if (other == null)
                {
                    return -1;
                }

                return _minValue.CompareTo(other._minValue);
            }

            #endregion
        }

        #endregion

        #region AcceptCommandImplementation

        private sealed class AcceptCommandImplementation : ICommand
        {
            #region Properties

            private PricePointEditContext Context
            {
                get;
            }

            #endregion

            #region Methods

            public AcceptCommandImplementation(PricePointEditContext context)
            {
                Context = context;
            }

            #endregion

            #region ICommand Members

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object parameter)
            {
                return Context.IsValid;
            }

            public void Execute(object parameter)
            {
                if (!CanExecute(parameter))
                {
                    return;
                }

                Context.OnAccept();
            }

            #endregion
        }

        #endregion

        #region AddCommandImplementation

        private sealed class AddCommandImplementation : ICommand
        {
            #region Properties

            private PricePointEditContext Context
            {
                get;
            }

            #endregion

            #region Methods

            public AddCommandImplementation(PricePointEditContext context)
            {
                Context = context;
            }

            #endregion

            #region ICommand Members

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                if (!CanExecute(parameter))
                {
                    return;
                }

                Context.AddPricePoint();
            }

            #endregion
        }

        #endregion

        #region DeleteCommandImplementation

        private sealed class DeleteCommandImplementation : ICommand
        {
            #region Properties

            private PricePointEditContext Context
            {
                get;
            }

            #endregion

            #region Methods

            public DeleteCommandImplementation(PricePointEditContext context)
            {
                Context = context;
            }

            #endregion

            #region ICommand Members

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object parameter)
            {
                return Context.PricePoints.Count > 1 && 
                    Context.SelectedPricePoint != null;
            }

            public void Execute(object parameter)
            {
                if (!CanExecute(parameter))
                {
                    return;
                }

                Context.DeletePricePoint(Context.SelectedPricePoint);
            }

            #endregion
        }

        #endregion

        #region PricePointCategory

        private class PricePointCategory
        {
            public PriceByType PriceByType { get; }
            public ObservableCollection<IPricePointDialogItem> PricePoints { get; }

            public bool IsValid =>
                PricePoints.All(point => string.IsNullOrEmpty(point.Error));

            private PricePointCategory(PriceByType priceByType, ObservableCollection<IPricePointDialogItem> pricePoints)
            {
                PriceByType = priceByType;
                PricePoints = pricePoints;
            }

            public static PricePointCategory Create(PriceByType priceByType, IEnumerable<IPricePointDialogItem> pricePoints)
            {
                if (pricePoints == null)
                {
                    throw new ArgumentNullException(nameof(pricePoints));
                }

                return new PricePointCategory(priceByType, new ObservableCollection<IPricePointDialogItem>(pricePoints));
            }

            public IPricePointDialogItem AddNew()
            {
                IPricePointDialogItem newPricePoint;

                switch (PriceByType)
                {
                    case PriceByType.Quantity:
                        newPricePoint = new QuantityPricePointItem(OrderPrice.enumPriceUnit.Each, 0, 0);
                        break;
                    case PriceByType.Weight:
                        newPricePoint = new WeightPricePointItem(OrderPrice.enumPriceUnit.EachByWeight, 0M, 0M);
                        break;
                    default:
                        newPricePoint = null;
                        break;
                }

                if (newPricePoint != null)
                {
                    PricePoints.Add(newPricePoint);
                }
                return newPricePoint;
            }
        }

        #endregion
    }
}
