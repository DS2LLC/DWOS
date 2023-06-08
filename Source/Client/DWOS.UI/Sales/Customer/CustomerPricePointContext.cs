using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Shared.Utilities;
using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;

namespace DWOS.UI.Sales
{
    internal class CustomerPricePointContext : IPricePointDialogContext, INotifyPropertyChanged
    {
        #region Fields

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private bool _isValid;
        private PriceByType _selectedPriceByType;
        private IPricePointDialogItem _selectedPricePoint;
        private readonly ISet<int> _removedPricePoints = new HashSet<int>();

        #endregion

        #region Properties

        private ObservableCollection<IPricePointDialogItem> WeightPricePoints { get; } =
            new ObservableCollection<IPricePointDialogItem>();

        private ObservableCollection<IPricePointDialogItem> QuantityPricePoints { get; } =
            new ObservableCollection<IPricePointDialogItem>();

        public CustomersDataset Dataset { get; }
        private CustomersDataset.CustomerRow Customer { get; }
        private CustomersDataset.CustomerPricePointRow PricePointRow { get; }

        #endregion

        #region Methods

        public CustomerPricePointContext(CustomersDataset dataset, CustomersDataset.CustomerRow customer)
        {
            Dataset = dataset ?? throw new ArgumentNullException(nameof(dataset));
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));

            AcceptCommand = new RelayCommand(AcceptChanges, CanAccept);
            AddCommand = new RelayCommand(AddPricePoint, CanAddPricePoint);
            DeleteCommand = new RelayCommand(RemovePricePoint, CanRemovePricePoint);

            PricePointRow = UsingVolumeDiscounts
                ? Customer.GetCustomerPricePointRows().FirstOrDefault(p => !p.IsTypeNull() && p.Type == "VOLUME")
                : Customer.GetCustomerPricePointRows().FirstOrDefault(p => p.IsTypeNull());

            var qtyPricePoints = new List<QuantityPricePoint>();
            var weightPricePoints = new List<WeightPricePoint>();

            if (PricePointRow == null)
            {
                // Load system defaults
                var dtPricePoint = new OrdersDataSet.PricePointDataTable();
                var dtPricePointDetail = new OrdersDataSet.PricePointDetailDataTable();

                using (var taPrice = new Data.Datasets.OrdersDataSetTableAdapters.PricePointTableAdapter())
                {
                    if (UsingVolumeDiscounts)
                    {
                        taPrice.FillVolumeDefault(dtPricePoint);
                    }
                    else
                    {
                        taPrice.FillDefault(dtPricePoint);
                    }
                }

                var pricePointId = dtPricePoint.FirstOrDefault()?.PricePointID ?? 0;

                using (var taDetail = new Data.Datasets.OrdersDataSetTableAdapters.PricePointDetailTableAdapter())
                {
                    taDetail.FillByPricePoint(dtPricePointDetail, pricePointId);
                }


                foreach (var detail in dtPricePointDetail)
                {
                    var priceUnit = OrderPrice.ParsePriceUnit(detail.PriceUnit);

                    switch (OrderPrice.GetPriceByType(priceUnit))
                    {
                        case PriceByType.Quantity:
                            // Add quantity
                            int.TryParse(detail.MinValue, out var minInt);
                            NullableParser.TryParse(detail.IsMaxValueNull() ? null : detail.MaxValue, out int? maxInt);

                            var qtyPricePoint = new QuantityPricePoint
                            {
                                CalculateBy = priceUnit,
                                MinValue = minInt,
                                MaxValue = maxInt
                            };

                            qtyPricePoints.Add(qtyPricePoint);
                            qtyPricePoint.PropertyChanged += PricePointChanged;

                            break;
                        case PriceByType.Weight:
                            // Add weight
                            decimal.TryParse(detail.MinValue, out var minDec);
                            NullableParser.TryParse(detail.IsMaxValueNull() ? null : detail.MaxValue, out decimal? maxDec);

                            var weightPricePoint = new WeightPricePoint
                            {
                                CalculateBy = priceUnit,
                                MinValue = minDec,
                                MaxValue = maxDec
                            };

                            weightPricePoints.Add(weightPricePoint);
                            weightPricePoint.PropertyChanged += PricePointChanged;

                            break;
                        default:
                            _logger.Error("Could not load system default price point");
                            break;
                    }
                }
            }
            else
            {
                // Construct qty/weight items from customer's price points
                foreach (var detail in PricePointRow.GetCustomerPricePointDetailRows())
                {
                    var priceUnit = OrderPrice.ParsePriceUnit(detail.PriceUnit);

                    switch (OrderPrice.GetPriceByType(priceUnit))
                    {
                        case PriceByType.Quantity:
                            // Add quantity
                            int.TryParse(detail.MinValue, out var minInt);
                            NullableParser.TryParse(detail.IsMaxValueNull() ? null : detail.MaxValue, out int? maxInt);

                            var qtyPricePoint = new QuantityPricePoint(detail.CustomerPricePointDetailID)
                            {
                                CalculateBy = priceUnit,
                                MinValue = minInt,
                                MaxValue = maxInt
                            };

                            qtyPricePoints.Add(qtyPricePoint);
                            qtyPricePoint.PropertyChanged += PricePointChanged;

                            break;

                        case PriceByType.Weight:
                            // Add weight
                            decimal.TryParse(detail.MinValue, out var minDec);
                            NullableParser.TryParse(detail.IsMaxValueNull() ? null : detail.MaxValue, out decimal? maxDec);

                            var weightPricePoint = new WeightPricePoint(detail.CustomerPricePointDetailID)
                            {
                                CalculateBy = priceUnit,
                                MinValue = minDec,
                                MaxValue = maxDec
                            };

                            weightPricePoints.Add(weightPricePoint);
                            weightPricePoint.PropertyChanged += PricePointChanged;

                            break;
                        default:
                            _logger.Error("Could not load system default price point");
                            break;
                    }
                }
            }

            foreach (var pricePoint in qtyPricePoints.OrderBy(p => p))
            {
                QuantityPricePoints.Add(pricePoint);
            }

            foreach (var pricePoint in weightPricePoints.OrderBy(p => p))
            {
                WeightPricePoints.Add(pricePoint);
            }

            SelectedPricePoint = PricePoints.FirstOrDefault();
            Validate();
        }

        private void AcceptChanges()
        {
            try
            {
                // Use existing price point row or initialize new row
                var pricePointRow = PricePointRow;
                if (pricePointRow == null)
                {
                    var type = UsingVolumeDiscounts ? "VOLUME" : null;
                    pricePointRow = Dataset.CustomerPricePoint.AddCustomerPricePointRow(Customer, type);
                }

                var existingPricePointDetails = pricePointRow.GetCustomerPricePointDetailRows().ToList();

                // Save changes to price point row
                foreach (var pricePoint in QuantityPricePoints.Concat(WeightPricePoints))
                {
                    var matchingDetailRow = existingPricePointDetails.FirstOrDefault(p => p.CustomerPricePointDetailID == pricePoint.OriginalRowId);

                    if (matchingDetailRow == null)
                    {
                        // Add price point
                        Dataset.CustomerPricePointDetail.AddCustomerPricePointDetailRow(pricePointRow,
                            Dataset.d_PriceUnit.FindByPriceUnitID(pricePoint.CalculateBy.ToString()),
                            pricePoint.MinValueString,
                            string.IsNullOrEmpty(pricePoint.MaxValueString) ? null : pricePoint.MaxValueString);
                    }
                    else
                    {
                        // Edit price point
                        matchingDetailRow.MinValue = pricePoint.MinValueString;

                        if (string.IsNullOrEmpty(pricePoint.MaxValueString))
                        {
                            matchingDetailRow.SetMaxValueNull();
                        }
                        else
                        {
                            matchingDetailRow.MaxValue = pricePoint.MaxValueString;
                        }

                        matchingDetailRow.PriceUnit = pricePoint.CalculateBy.ToString();
                    }
                }

                // Delete removed price points
                foreach (var pricePointDetailId in _removedPricePoints)
                {
                    var matchingDetailRow = existingPricePointDetails.FirstOrDefault(p => p.CustomerPricePointDetailID == pricePointDetailId);
                    matchingDetailRow?.Delete();
                }

                _removedPricePoints.Clear();

                Accept?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error saving price point data.");
                SaveFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        private void AddPricePoint()
        {
            var priceUnits = OrderPrice.GetPriceUnits(_selectedPriceByType);

            if (_selectedPriceByType == PriceByType.Quantity)
            {
                var pricePoint = new QuantityPricePoint
                {
                    CalculateBy = priceUnits.First(),
                    MinValue = 0,
                    MaxValue = 0
                };

                QuantityPricePoints.Add(pricePoint);
                pricePoint.PropertyChanged += PricePointChanged;
                Validate();
            }
            else if (_selectedPriceByType == PriceByType.Weight)
            {
                var pricePoint = new WeightPricePoint
                {
                    CalculateBy = priceUnits.First(),
                    MinValue = 0,
                    MaxValue = 0
                };

                WeightPricePoints.Add(pricePoint);
                pricePoint.PropertyChanged += PricePointChanged;
                Validate();
            }

            SelectedPricePoint = PricePoints.LastOrDefault();
        }

        private void RemovePricePoint()
        {
            var pricePoint = _selectedPricePoint;
            PricePoints.Remove(pricePoint);
            _removedPricePoints.Add(pricePoint.OriginalRowId);
            pricePoint.PropertyChanged -= PricePointChanged;

            Validate();
            SelectedPricePoint = PricePoints.FirstOrDefault();
        }

        private bool CanAccept() => IsValid;

        private bool CanAddPricePoint() => UsingVolumeDiscounts;

        private bool CanRemovePricePoint() => UsingVolumeDiscounts &&
            PricePoints.Count > 1 &&
            SelectedPricePoint != null;

        private void Validate()
        {
            var isValid = true;

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
                    _logger.Error(exc);
                    isValid = false;
                }

                string currentError = current.Error;

                if (!string.IsNullOrEmpty(currentError))
                {
                    isValid = false;
                }

                previous = current;
            }

            IsValid = isValid;
        }

        private static string GetDisplayString<T>(OrderPrice.enumPriceUnit calculateBy, T minValue, T? maxValue)
            where T : struct, IEquatable<T>, IComparable<T>
        {
            string calcString;
            switch (calculateBy)
            {
                case OrderPrice.enumPriceUnit.Each:
                case OrderPrice.enumPriceUnit.EachByWeight:
                    calcString = "Each";
                    break;
                case OrderPrice.enumPriceUnit.Lot:
                case OrderPrice.enumPriceUnit.LotByWeight:
                    calcString = "Lot";
                    break;
                default:
                    calcString = string.Empty;
                    break;
            }

            return maxValue.HasValue
                ? $"{minValue}-{maxValue} ({calcString})"
                : $"{minValue}+ ({calcString})";
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Events

        private void PricePointChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IPricePointDialogItem.CalculateBy))
            {
                if (!UsingVolumeDiscounts)
                {
                    // Ensure that both price points do not have the same calculation strategy.
                    var senderPricePoint = sender as IPricePointDialogItem;
                    var otherPricePoint = PricePoints.FirstOrDefault(point => point != senderPricePoint);
                    if (senderPricePoint != null && otherPricePoint != null)
                    {
                        otherPricePoint.CalculateBy = OrderPrice.GetOppositeValue(senderPricePoint.CalculateBy);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(e.PropertyName) && e.PropertyName != nameof(IPricePointDialogItem.DisplayString))
            {
                Validate();
            }
        }

        #endregion

        #region IPricePointDialogContext Members

        public event EventHandler Accept;

        public event EventHandler SaveFailed;

        public string Title => "Edit Price Points";

        public string ItemLabel => "Customer:";

        public string ItemValue => Customer.Name;

        public ICommand AcceptCommand { get; }

        public ICommand AddCommand { get; }

        public ICommand DeleteCommand { get; }

        public bool IsValid
        {
            get => _isValid;
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public IEnumerable<PriceByType> PriceByOptions { get; } =
                Enum.GetValues(typeof(PriceByType)).OfType<PriceByType>();

        public ObservableCollection<IPricePointDialogItem> PricePoints => _selectedPriceByType == PriceByType.Quantity
            ? QuantityPricePoints
            : WeightPricePoints;

        public IEnumerable<IPricePointDialogItem> AllPricePoints =>
            QuantityPricePoints.Concat(WeightPricePoints);

        public PriceByType SelectedPriceByOption
        {
            get => _selectedPriceByType;
            set
            {
                if (_selectedPriceByType != value)
                {
                    _selectedPriceByType = value;
                    OnPropertyChanged(nameof(SelectedPriceByOption));

                    // Trigger refresh of price point UI
                    OnPropertyChanged(nameof(PricePoints));
                    SelectedPricePoint = PricePoints.FirstOrDefault();
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

        public bool UsingVolumeDiscounts { get; } =
            ApplicationSettings.Current.EnableVolumePricing;

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region QuantityPricePoint

        private class QuantityPricePoint : IPricePointDialogItem, IComparable<QuantityPricePoint>
        {
            private int _minValue;
            private int? _maxValue;
            private readonly Dictionary<string, string> _errors =
                new Dictionary<string, string>();

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
                        var triggerHasMaxQuantity = _maxValue.HasValue != value.HasValue;

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

            #region Methods

            public QuantityPricePoint()
            {
                OriginalRowId = -1;
            }

            public QuantityPricePoint(int pricePointDetailId)
            {
                OriginalRowId = pricePointDetailId;
            }

            private void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            private bool HasGap(QuantityPricePoint nextValue)
            {
                if (nextValue == null)
                {
                    throw new ArgumentNullException(nameof(nextValue));
                }

                return !MaxValue.HasValue ||
                    (nextValue.MinValue - MaxValue.Value) != 1;
            }

            #endregion

            #region IPricePointDialogItem Members

            public string this[string columnName]
            {
                get
                {
                    _errors.TryGetValue(columnName, out var value);
                    return value;
                }
            }

            public IEnumerable<OrderPrice.enumPriceUnit> CalculateByOptions { get; } =
                OrderPrice.GetPriceUnits(PriceByType.Quantity);

            private OrderPrice.enumPriceUnit _calculateBy;
            public OrderPrice.enumPriceUnit CalculateBy
            {
                get => _calculateBy;
                set
                {
                    if (_calculateBy != value)
                    {
                        _calculateBy = value;
                        OnPropertyChanged(nameof(CalculateBy));
                        OnPropertyChanged(nameof(DisplayString));
                    }
                }
            }

            public string DisplayString => GetDisplayString(_calculateBy, _minValue, _maxValue);

            public string MinValueString => _minValue.ToString();

            public string MaxValueString => _maxValue.ToString();

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
            public bool EraseMaxValue { get => !HasMaxValue; set => HasMaxValue = !value; }

            public string EditorMask => "nnnnnn";

            public string EditorFormat => "0";

            public Type MinEditorType => typeof(int);

            public Type MaxEditorType => typeof(int?);

            public int OriginalRowId { get; }

            public string Error => string.Join(Environment.NewLine, _errors.Values);


            public event PropertyChangedEventHandler PropertyChanged;

            public int CompareTo(object obj)
            {
                return CompareTo(obj as QuantityPricePoint);
            }

            public void Validate(IPricePointDialogItem previous, IPricePointDialogItem next)
            {
                var previousValue = previous as QuantityPricePoint;
                var nextValue = next as QuantityPricePoint;

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
                    if (MinValue != 1)
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
                    if (MinValue != 1)
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
            #endregion

            #region IComparable<QuantityPricePoint> Members

            public int CompareTo(QuantityPricePoint other)
            {
                return other == null ? -1 : _minValue.CompareTo(other._minValue);
            }

            #endregion
        }

        #endregion

        #region WeightPricePoint

        private class WeightPricePoint : IPricePointDialogItem, IComparable<WeightPricePoint>
        {
            private decimal _minValue;
            private decimal? _maxValue;
            private readonly Dictionary<string, string> _errors =
                new Dictionary<string, string>();

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
                        var triggerHasMaxQuantity = _maxValue.HasValue != value.HasValue;

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

            #region Methods

            public WeightPricePoint()
            {
                OriginalRowId = -1;
            }

            public WeightPricePoint(int pricePointDetailId)
            {
                OriginalRowId = pricePointDetailId;
            }

            private void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            private bool HasGap(WeightPricePoint nextValue)
            {
                if (nextValue == null)
                {
                    throw new ArgumentNullException(nameof(nextValue), @"Cannot be null.");
                }

                if (!MaxValue.HasValue)
                {
                    return true;
                }

                var difference = nextValue.MinValue - MaxValue.Value;

                return !(difference > 0M && difference <= 0.01M);
            }

            #endregion

            #region IPricePointDialogItem Members

            public string this[string columnName]
            {
                get
                {
                    _errors.TryGetValue(columnName, out var value);
                    return value;
                }
            }

            public IEnumerable<OrderPrice.enumPriceUnit> CalculateByOptions { get; } =
                OrderPrice.GetPriceUnits(PriceByType.Weight);

            private OrderPrice.enumPriceUnit _calculateBy;
            public OrderPrice.enumPriceUnit CalculateBy
            {
                get => _calculateBy;
                set
                {
                    if (_calculateBy != value)
                    {
                        _calculateBy = value;
                        OnPropertyChanged(nameof(CalculateBy));
                        OnPropertyChanged(nameof(DisplayString));
                    }
                }
            }

            public string DisplayString => GetDisplayString(_calculateBy, _minValue, _maxValue);

            public string MinValueString => _minValue.ToString(CultureInfo.CurrentCulture);

            public string MaxValueString => _maxValue?.ToString(CultureInfo.CurrentCulture) ?? string.Empty;

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
            public bool EraseMaxValue { get => !HasMaxValue; set => HasMaxValue = !value; }

            public string EditorMask => "nnnnnn.nn";

            public string EditorFormat => "0.00 lbs.";

            public Type MinEditorType => typeof(decimal);

            public Type MaxEditorType => typeof(decimal?);

            public int OriginalRowId { get; }

            public string Error => string.Join(Environment.NewLine, _errors.Values);


            public event PropertyChangedEventHandler PropertyChanged;

            public int CompareTo(object obj)
            {
                return CompareTo(obj as WeightPricePoint);
            }

            public void Validate(IPricePointDialogItem previous, IPricePointDialogItem next)
            {
                var previousValue = previous as WeightPricePoint;
                var nextValue = next as WeightPricePoint;

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
                    if (MinValue != 0)
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
                    if (MinValue != 0)
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
            #endregion

            #region IComparable<WeightPricePoint> Members

            public int CompareTo(WeightPricePoint other)
            {
                return other == null ? -1 : _minValue.CompareTo(other._minValue);
            }

            #endregion
        }

        #endregion
    }
}
