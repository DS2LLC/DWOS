using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Shared.Utilities;
using DWOS.UI.Utilities;
using NLog;
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
    /// Price Point Manager window.
    /// </summary>
    public partial class PricePointDialog
    {
        #region Properties

        public IEnumerable<IPricePointDialogItem> PricePoints
        {
            get
            {
                if (!(DataContext is IPricePointDialogContext context))
                {
                    LogManager.GetCurrentClassLogger().Warn("context should not be null.");
                    return null;
                }

                return context.AllPricePoints;
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="PricePointDialog"/>
        /// class using a default context.
        /// </summary>
        public PricePointDialog()
            : this(new DefaultPricePointContext())
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="PricePointDialog"/> class.
        /// </summary>
        /// <param name="context">Data context to use for the dialog</param>
        public PricePointDialog(IPricePointDialogContext context)
        {
            InitializeComponent();
            DataContext = context;
            Icon = Properties.Resources.PriceUnit_24.ToWpfImage();
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is IPricePointDialogContext context))
            {
                LogManager.GetCurrentClassLogger().Warn("context should not be null.");
            }
            else
            {
                context.Accept += Context_Accept;
                context.SaveFailed += Context_SaveFailed;

                if (string.IsNullOrEmpty(context.ItemValue))
                {
                    itemLabel.Visibility = Visibility.Collapsed;
                    partTextBox.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Context_SaveFailed(object sender, EventArgs e)
        {
            MessageBoxUtilities.ShowMessageBoxWarn(
                "Encountered an error while saving price point data.",
                "Price Point Manager");
        }

        private void Context_Accept(object sender, EventArgs e)
        {
            DialogResult = true;
        }

        #endregion

        #region PricePointManagerContext

        /// <summary>
        /// Price point dialog implementation that manages system-wide defaults.
        /// </summary>
        private sealed class DefaultPricePointContext : INotifyPropertyChanged, IPricePointDialogContext
        {
            #region Fields

            /// <summary>
            /// Occurs when a user accepts changes made.
            /// </summary>
            public event EventHandler Accept;

            /// <summary>
            /// Occurs when there is an error while saving changes made.
            /// </summary>
            public event EventHandler SaveFailed;

            private IPricePointDialogItem _selectedPricePoint;
            private PricePointCategory _selectedCategory;
            private readonly List<PricePointCategory> _categories;

            #endregion

            #region Properties

            public string Title => ApplicationSettings.Current.EnableVolumePricing
                    ? "Price Point Manager (Volume Discount)"
                    : "Price Point Manager (Unit Quantities)";

            public string ItemLabel => string.Empty;

            // The standard editor does not show a part name.
            public string ItemValue => string.Empty;

            /// <summary>
            /// Gets a collection of pricing options.
            /// </summary>
            public IEnumerable<PriceByType> PriceByOptions { get; } =
                Enum.GetValues(typeof(PriceByType)).OfType<PriceByType>();

            /// <summary>
            /// Gets or sets the currently selected pricing option.
            /// </summary>
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
                        SelectedPricePoint = PricePoints.FirstOrDefault();
                    }
                }
            }

            /// <summary>
            /// Gets a collection of currently visible price points.
            /// </summary>
            /// <remarks>
            /// These price points belong to the currently selected
            /// pricing option. See <see cref="SelectedPriceByOption"/>.
            /// </remarks>
            public ObservableCollection<IPricePointDialogItem> PricePoints =>
                _selectedCategory?.PricePoints;

            public IEnumerable<IPricePointDialogItem> AllPricePoints =>
                _categories.SelectMany(c => c.PricePoints);

            /// <summary>
            /// Gets or sets the currently selected price point.
            /// </summary>
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

            public bool IsValid
            {
                get
                {
                    return _categories.All(category => category.IsValid);
                }
            }

            /// <summary>
            /// Gets the 'accept changes' command.
            /// </summary>
            public ICommand AcceptCommand { get; }

            public ICommand AddCommand { get; }

            public ICommand DeleteCommand { get; }

            public bool UsingVolumeDiscounts { get; } = 
                ApplicationSettings.Current.EnableVolumePricing;

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultPricePointContext"/> class.
            /// </summary>
            public DefaultPricePointContext()
            {
                
                _categories = PricePointCategory.CreateCategories(UsingVolumeDiscounts);

                foreach (var category in _categories)
                {
                    category.PropertyChanged += Category_PropertyChanged;
                }

                _selectedCategory = _categories.FirstOrDefault();
                _selectedPricePoint = _categories.FirstOrDefault()?.PricePoints?.FirstOrDefault();

                AcceptCommand = new AcceptCommandImplementation(this);
                AddCommand = new AddCommandImplementation(this);
                DeleteCommand = new DeleteCommandImplementation(this);
            }

            /// <summary>
            /// Performs activities after the user accepts changes.
            /// </summary>
            /// <remarks>
            /// This method is public because
            /// <see cref="AcceptCommand"/> calls it.
            /// </remarks>
            public void OnAccept()
            {
                try
                {
                    if (!IsValid)
                    {
                        return;
                    }

                    foreach (var category in _categories)
                    {
                        category.SaveAll();
                    }

                    var handler = Accept;
                    handler?.Invoke(this, new EventArgs());
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error saving price point data.");
                    var handler = SaveFailed;
                    handler?.Invoke(this, new EventArgs());
                }
            }

            /// <summary>
            /// Adds a price point to the current category.
            /// </summary>
            public void AddPricePoint()
            {
                var pricePoint = _selectedCategory?.AddPricePoint();
                SelectedPricePoint = pricePoint;
            }

            /// <summary>
            /// Deletes the specified price point from the current category.
            /// </summary>
            /// <param name="pricePoint"></param>
            public void DeletePricePoint(IPricePointDialogItem pricePoint)
            {
                if (pricePoint == null)
                {
                    return;
                }

                _selectedCategory?.DeletePricePoint(pricePoint);
                SelectedPricePoint = PricePoints.FirstOrDefault();
            }

            private void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion

            #region Events

            private void Category_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                // Trigger refresh of validation UI if any price point category changes
                OnPropertyChanged(nameof(IsValid));
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region PricePointCategory

        /// <summary>
        /// Represents a category of system's default price points.
        /// </summary>
        private sealed class PricePointCategory : INotifyPropertyChanged, IDataErrorInfo
        {
            #region Fields

            private readonly List<string> _pricePointErrors = new List<string>();
            private readonly int _pricePointId;
            private readonly OrdersDataSet.PricePointDetailDataTable _dtPricePointDetail;
            private readonly bool _usingVolumeDiscounts;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the category's price type.
            /// </summary>
            public PriceByType PriceByType { get; }

            /// <summary>
            /// Gets the price points of the category.
            /// </summary>
            public ObservableCollection<IPricePointDialogItem> PricePoints { get; }

            public bool IsValid => PricePoints.Count > 0 && _pricePointErrors.Count == 0;


            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="PricePointCategory"/> class.
            /// </summary>
            /// <param name="dtPricePointDetail">Source of price point data</param>
            /// <param name="pricePointId">ID of the price point that this category represents.</param>
            /// <param name="pricingType">The pricing type of the category</param>
            /// <param name="pricePoints">The price points of the category</param>
            /// <param name="usingVolumeDiscounts">
            /// <c>true</c> if using volume discount pricing; otherwise, <c>false</c>.
            /// </param>
            private PricePointCategory(OrdersDataSet.PricePointDetailDataTable dtPricePointDetail, int pricePointId, PriceByType pricingType, IList<IPricePointDialogItem> pricePoints, bool usingVolumeDiscounts)
            {
                _dtPricePointDetail = dtPricePointDetail;
                _pricePointId = pricePointId;
                PriceByType = pricingType;
                PricePoints = new ObservableCollection<IPricePointDialogItem>(pricePoints);
                _usingVolumeDiscounts = usingVolumeDiscounts;

                foreach (var pricePoint in PricePoints)
                {
                    pricePoint.PropertyChanged += PricePoint_PropertyChanged;
                }
            }

            public IPricePointDialogItem AddPricePoint()
            {
                var options = OrderPrice.GetPriceUnits(PriceByType).ToList();

                if (!options.Any())
                {
                    throw new InvalidOperationException("This category is not associated with a 'price-by' type.");
                }

                IPricePointDialogItem pricePoint;
                switch (PriceByType)
                {
                    case PriceByType.Quantity:
                        pricePoint = new QuantityPricePoint()
                        {
                            CalculateBy = options.First(),
                            MinValue = 0,
                            MaxValue = 0
                        };
                        break;
                    case PriceByType.Weight:
                        pricePoint = new WeightPricePoint()
                        {
                            CalculateBy = options.First(),
                            MinValue = 0,
                            MaxValue = 0
                        };
                        break;
                    default:
                        pricePoint = null;
                        break;
                }

                if (pricePoint != null)
                {
                    PricePoints.Add(pricePoint);
                    pricePoint.PropertyChanged += PricePoint_PropertyChanged;
                    Validate();
                }

                return pricePoint;
            }

            public void DeletePricePoint(IPricePointDialogItem pricePoint)
            {
                if (pricePoint == null)
                {
                    return;
                }

                var originalRow = _dtPricePointDetail.FindByPricePointDetailID(pricePoint.OriginalRowId);

                PricePoints.Remove(pricePoint);
                originalRow?.Delete();
                pricePoint.PropertyChanged -= PricePoint_PropertyChanged;
                Validate();
            }

            public void SaveAll()
            {
                foreach (var pricePoint in PricePoints)
                {
                    var originalRow = _dtPricePointDetail.FindByPricePointDetailID(pricePoint.OriginalRowId);

                    if (originalRow == null)
                    {
                        var newRow = _dtPricePointDetail.NewPricePointDetailRow();
                        newRow.PricePointID = _pricePointId;
                        newRow.MinValue = pricePoint.MinValueString;
                        newRow.MaxValue = pricePoint.MaxValueString;
                        newRow.PriceUnit = pricePoint.CalculateBy.ToString();
                        _dtPricePointDetail.AddPricePointDetailRow(newRow);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(pricePoint.Error))
                        {
                            throw new InvalidOperationException("Cannot save invalid data.");
                        }

                        string minValue = pricePoint.MinValueString;

                        string maxValue = pricePoint.MaxValueString;

                        if (originalRow.MinValue != minValue)
                        {
                            originalRow.MinValue = minValue;
                        }

                        bool maxValueChanged = (originalRow.IsMaxValueNull() && !string.IsNullOrEmpty(maxValue)) ||
                            (!originalRow.IsMaxValueNull() && originalRow.MaxValue != maxValue);

                        if (maxValueChanged)
                        {
                            originalRow.MaxValue = maxValue;
                        }

                        originalRow.PriceUnit = pricePoint.CalculateBy.ToString();
                    }
                }

                using (var taPricePointDetail = new Data.Datasets.OrdersDataSetTableAdapters.PricePointDetailTableAdapter())
                {
                    taPricePointDetail.Update(_dtPricePointDetail);
                }
            }

            private void Validate()
            {
                _pricePointErrors.Clear();

                IPricePointDialogItem previous = null;
                var logger = LogManager.GetCurrentClassLogger();

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
                        logger.Error(exc);
                        _pricePointErrors.Add("Unable to validate price point.");
                    }

                    string currentError = current.Error;

                    if (!string.IsNullOrEmpty(currentError))
                    {
                        _pricePointErrors.Add(currentError);
                    }

                    previous = current;
                }

                // Trigger PropertyChanged for IsValid - used to disable type
                // selection when a category is invalid.
                var handler = PropertyChanged;
                handler?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
            }

            public static List<PricePointCategory> CreateCategories(bool usingVolumeDiscounts)
            {
                var dtPricePoint = new OrdersDataSet.PricePointDataTable();
                var dtPricePointDetail = new OrdersDataSet.PricePointDetailDataTable();

                using (var taPricePoint = new Data.Datasets.OrdersDataSetTableAdapters.PricePointTableAdapter())
                {
                    if (usingVolumeDiscounts)
                    {
                        taPricePoint.FillVolumeDefault(dtPricePoint);
                    }
                    else
                    {
                        taPricePoint.FillDefault(dtPricePoint);
                    }
                }

                var pricePointId = dtPricePoint.FirstOrDefault()?.PricePointID ?? 0;

                using (var taDetail = new Data.Datasets.OrdersDataSetTableAdapters.PricePointDetailTableAdapter())
                {
                    taDetail.FillByPricePoint(dtPricePointDetail, pricePointId);
                }

                var pricePointRows = dtPricePointDetail;
                var qtyPricePoints = new List<QuantityPricePoint>();
                var weightPricePoints = new List<WeightPricePoint>();
                foreach (var detail in pricePointRows)
                {
                    // Create price point
                    var priceUnit = OrderPrice.ParsePriceUnit(detail.PriceUnit);
                    int? maxPriceQty = null;
                    decimal? maxPriceWeight = null;

                    switch (priceUnit)
                    {
                        case OrderPrice.enumPriceUnit.Each:
                        case OrderPrice.enumPriceUnit.Lot:
                            int.TryParse(detail.MinValue, out var minPriceQty);

                            if (!detail.IsMaxValueNull())
                            {
                                NullableParser.TryParse(detail.MaxValue, out maxPriceQty);
                            }

                            qtyPricePoints.Add(new QuantityPricePoint(detail.PricePointDetailID)
                            {
                                CalculateBy = priceUnit,
                                MinValue = minPriceQty,
                                MaxValue = maxPriceQty
                            });

                            break;
                        case OrderPrice.enumPriceUnit.EachByWeight:
                        case OrderPrice.enumPriceUnit.LotByWeight:
                            decimal.TryParse(detail.MinValue, out var minPriceWeight);

                            if (!detail.IsMaxValueNull())
                            {
                                NullableParser.TryParse(detail.MaxValue, out maxPriceWeight);
                            }

                            weightPricePoints.Add(new WeightPricePoint(detail.PricePointDetailID)
                            {
                                CalculateBy = priceUnit,
                                MinValue = minPriceWeight,
                                MaxValue = maxPriceWeight
                            });

                            break;
                    }
                }

                var returnList = new List<PricePointCategory>();
                returnList.Add(new PricePointCategory(dtPricePointDetail, pricePointId, PriceByType.Quantity, qtyPricePoints.OrderBy(i => i).ToList<IPricePointDialogItem>(), usingVolumeDiscounts));
                returnList.Add(new PricePointCategory(dtPricePointDetail, pricePointId, PriceByType.Weight, weightPricePoints.OrderBy(i => i).ToList<IPricePointDialogItem>(), usingVolumeDiscounts));

                return returnList;
            }

            #endregion

            #region Events

            private void PricePoint_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(IPricePointDialogItem.CalculateBy))
                {
                    if (!_usingVolumeDiscounts)
                    {
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

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            #region IDataErrorInfo Members

            public string Error
            {
                get
                {
                    return this[nameof(PricePoints)];
                }
            }

            public string this[string columnName]
            {
                get
                {
                    if (columnName == nameof(PricePoints) && _pricePointErrors.Any())
                    {
                        return string.Join(Environment.NewLine, _pricePointErrors);
                    }

                    return string.Empty;
                }
            }

            #endregion
        }

        #endregion

        #region PricePoint<T>

        private abstract class PricePoint<T> : IPricePointDialogItem, IComparable<PricePoint<T>>
            where T : struct, IEquatable<T>, IComparable<T>
        {
            #region Fields

            private T _minValue;
            private T? _maxValue;
            private OrderPrice.enumPriceUnit _calculateBy;
            private readonly Dictionary<string, string> _errors =
                new Dictionary<string, string>();

            #endregion

            #region Properties

            public string DisplayString
            {
                get
                {

                    T? maxQuantity = _maxValue;

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

            public T MinValue
            {
                get => _minValue;

                set
                {
                    if (!Equals(_minValue, value))
                    {
                        _minValue = value;
                        OnPropertyChanged(nameof(MinValue));
                    }
                }
            }

            public T? MaxValue
            {
                get => _maxValue;

                set
                {
                    if (!Equals(_maxValue, value))
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

            protected abstract bool CoversLowestValue
            {
                get;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new price point instance.
            /// </summary>
            protected PricePoint()
            {

            }

            /// <summary>
            /// Initializes a price point instance with a row ID.
            /// </summary>
            /// <param name="originalRowId"></param>
            protected PricePoint(int originalRowId)
            {
                OriginalRowId = originalRowId;
            }

            protected void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                if (propertyName != nameof(DisplayString))
                {
                    OnPropertyChanged(nameof(DisplayString));
                }
            }

            protected abstract bool HasGap(PricePoint<T> nextValue);

            #endregion

            #region IPricePoint Members

            public event PropertyChangedEventHandler PropertyChanged;

            public string Error
            {
                get
                {
                    return string.Join(Environment.NewLine, _errors.Values);
                }
            }

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

                        // DisplayString uses CalculateBy's value
                        OnPropertyChanged(nameof(DisplayString));
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
                        MaxValue = default(T);
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

            public int OriginalRowId
            {
                get;
            }

            public Type MinEditorType => typeof(T);

            public Type MaxEditorType => typeof(Nullable<T>);

            public abstract IEnumerable<OrderPrice.enumPriceUnit> CalculateByOptions { get; }

            public abstract string EditorMask { get; }

            public abstract string EditorFormat { get; }

            public string MinValueString
            {
                get
                {
                    return MinValue.ToString();
                }
            }

            public string MaxValueString
            {
                get
                {
                    return MaxValue.ToString();
                }
            }

            public void Validate(IPricePointDialogItem previous, IPricePointDialogItem next)
            {
                var previousValue = previous as PricePoint<T>;
                var nextValue = next as PricePoint<T>;

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
                return CompareTo(obj as PricePoint<T>);
            }

            #endregion

            #region IComparable<PricePoint<T>> Members

            public int CompareTo(PricePoint<T> other)
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

        #region QuantityPricePoint

        private sealed class QuantityPricePoint : PricePoint<int>
        {
            #region Properties

            public override IEnumerable<OrderPrice.enumPriceUnit> CalculateByOptions { get; } =
                OrderPrice.GetPriceUnits(PriceByType.Quantity);


            protected override bool CoversLowestValue => MinValue == 1;

            public override string EditorMask => "nnnnnn";

            public override string EditorFormat => "0";

            #endregion

            #region Methods

            public QuantityPricePoint()
            {

            }

            public QuantityPricePoint(int originalRowId)
                : base(originalRowId)
            {

            }

            protected override bool HasGap(PricePoint<int> nextValue)
            {
                if (nextValue == null)
                {
                    throw new ArgumentNullException(nameof(nextValue), @"Cannot be null.");
                }

                return !MaxValue.HasValue ||
                    (nextValue.MinValue - MaxValue.Value) != 1;
            }

            #endregion
        }

        #endregion

        #region WeightPricePoint

        private sealed class WeightPricePoint : PricePoint<decimal>
        {
            #region Properties

            public override IEnumerable<OrderPrice.enumPriceUnit> CalculateByOptions { get; } =
                OrderPrice.GetPriceUnits(PriceByType.Weight);

            protected override bool CoversLowestValue => MinValue == 0M;

            public override string EditorMask => "nnnnnn.nn";

            public override string EditorFormat => "0.00 lbs.";

            #endregion

            #region Methods

            public WeightPricePoint()
            {

            }

            public WeightPricePoint(int originalRowId)
                : base(originalRowId)
            {

            }

            protected override bool HasGap(PricePoint<decimal> nextValue)
            {
                if (nextValue == null)
                {
                    throw new ArgumentNullException(nameof(nextValue), @"Cannot be null.");
                }
                else if (!MaxValue.HasValue)
                {
                    return true;
                }

                var difference = nextValue.MinValue - MaxValue.Value;

                return !(difference > 0M && difference <= 0.01M);
            }

            #endregion
        }

        #endregion

        #region AcceptCommandImplementation

        private sealed class AcceptCommandImplementation : ICommand
        {
            #region Properties

            public DefaultPricePointContext Context
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public AcceptCommandImplementation(DefaultPricePointContext context)
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

            public DefaultPricePointContext Context
            {
                get;
            }

            #endregion

            #region Methods

            public AddCommandImplementation(DefaultPricePointContext context)
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
                return Context.UsingVolumeDiscounts;
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

            public DefaultPricePointContext Context
            {
                get;
            }

            #endregion

            #region Methods

            public DeleteCommandImplementation(DefaultPricePointContext context)
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
                return Context.UsingVolumeDiscounts &&
                    Context.PricePoints.Count > 1 &&
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
    }
}
