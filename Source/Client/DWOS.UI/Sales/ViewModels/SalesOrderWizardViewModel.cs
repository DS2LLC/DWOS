using DWOS.Data;
using DWOS.Data.Date;
using DWOS.Shared.Utilities;
using DWOS.UI.Sales.Models;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace DWOS.UI.Sales.ViewModels
{
    public class SalesOrderWizardViewModel : Utilities.ViewModelBase
    {
        #region Fields

        public event EventHandler<Utilities.SavedDataEventArgs> Saved;
        public event EventHandler WorkOrderMoved;
        private StepType _currentStep;
        private DateTime? _orderDate;
        private DateTime? _estShipDate;
        private DateTime? _requiredDate;
        private CustomerSummary _selectedCustomer;
        private CustomerShipping _selectedShippingMethod;
        private CustomerAddress _selectedShippingAddress;
        private WorkOrderViewModel _selectedWorkOrder;
        private string _customerWorkOrder;
        private string _purchaseOrderNumber;
        private string _priority;
        private decimal _basePriceLot;
        private bool _isAccepted;
        private string _contractReviewText;
        private bool _autoUpdateSalesOrderDates;

        private string _currencyMask = "{currency:6.2}";

        private SystemFieldInfo _reqDateSettings = new SystemFieldInfo();
        private SystemFieldInfo _serialNumberSettings = new SystemFieldInfo();

        #endregion

        #region Properties

        public StepType CurrentStep
        {
            get => _currentStep;
            private set
            {
                if (Set(nameof(CurrentStep), ref _currentStep, value))
                {
                    RaisePropertyChanged(nameof(IsFirstStepActive));
                    RaisePropertyChanged(nameof(IsSecondStepActive));
                    RaisePropertyChanged(nameof(Title));
                    RaisePropertyChanged(nameof(NextStepText));
                }
            }
        }

        public bool IsFirstStepActive =>
            _currentStep == StepType.First;

        public bool IsSecondStepActive =>
            _currentStep == StepType.Second;

        public string Title => $"New Sales Order ({(int)_currentStep + 1} of 2)";

        public ObservableCollection<CustomerSummary> Customers { get; } =
            new ObservableCollection<CustomerSummary>();

        public ObservableCollection<string> Priorities { get; } =
            new ObservableCollection<string>();

        public DateTime? OrderDate
        {
            get => _orderDate;
            set => Set(nameof(OrderDate), ref _orderDate, value);
        }

        public DateTime? EstShipDate
        {
            get => _estShipDate;
            set
            {
                if (Set(nameof(EstShipDate), ref _estShipDate, value))
                {
                    // Assumption: This was set manually by the user.
                    // If it isn't manually set by the user, set the 
                    // 'auto update' flag to true again.
                    // Assumption: If the user clears estimated ship date,
                    // it's not intentional and needs to be reset.
                    _autoUpdateSalesOrderDates = !value.HasValue;
                }
            }
        }

        public DateTime? RequiredDate
        {
            get => _requiredDate;
            set => Set(nameof(RequiredDate), ref _requiredDate, value);
        }

        public CustomerSummary SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (Set(nameof(SelectedCustomer), ref _selectedCustomer, value))
                {
                    if (_autoUpdateSalesOrderDates)
                    {
                        AutoUpdateSalesOrderDates();
                    }

                    AutoUpdateSalesOrderFields();
                    ResetCustomFields();

                    // Any existing Work Orders were added
                    // for a different customer.
                    WorkOrders.Clear();

                    // Reset address fields
                    SelectedShippingMethod = _selectedCustomer?.ShippingMethods.FirstOrDefault(ship => ship.IsDefault)
                        ?? _selectedCustomer?.ShippingMethods.FirstOrDefault();

                    SelectedShippingAddress = _selectedCustomer?.ShippingAddresses.FirstOrDefault(ship => ship.IsDefault)
                        ?? _selectedCustomer?.ShippingAddresses.FirstOrDefault();

                    // Use customer parts
                    Parts.Clear();

                    if (_selectedCustomer != null)
                    {
                        foreach (var part in _selectedCustomer.Parts.Select(PartViewModel.From).OrderBy(p => p.Name))
                        {
                            Parts.Add(part);
                        }
                    }

                    // Use default lot price (if defined)
                    var defaultLotPrice = _selectedCustomer?.DefaultPriceLot;
                    if (defaultLotPrice.HasValue)
                    {
                        BasePriceLot = defaultLotPrice.Value;
                    }

                    // Refresh fields - may need to show validation errors
                    RaisePropertyChanged(nameof(CustomerWorkOrder));
                    RaisePropertyChanged(nameof(PurchaseOrderNumber));
                    RaisePropertyChanged(nameof(IsDocumentValid));
                }
            }
        }

        public ObservableCollection<PartViewModel> Parts { get; } =
            new ObservableCollection<PartViewModel>();

        public CustomerShipping SelectedShippingMethod
        {
            get => _selectedShippingMethod;
            set => Set(nameof(SelectedShippingMethod), ref _selectedShippingMethod, value);
        }

        public CustomerAddress SelectedShippingAddress
        {
            get => _selectedShippingAddress;
            set => Set(nameof(SelectedShippingAddress), ref _selectedShippingAddress, value);
        }

        public string CustomerWorkOrder
        {
            get => _customerWorkOrder;
            set => Set(nameof(CustomerWorkOrder), ref _customerWorkOrder, value);
        }

        public string PurchaseOrderNumber
        {
            get => _purchaseOrderNumber;
            set => Set(nameof(PurchaseOrderNumber), ref _purchaseOrderNumber, value);
        }

        public string Priority
        {
            get => _priority;
            set => Set(nameof(Priority), ref _priority, value);
        }

        public decimal BasePriceLot
        {
            get => _basePriceLot;
            set
            {
                if (Set(nameof(BasePriceLot), ref _basePriceLot, value))
                {
                    RaisePropertyChanged(nameof(FeeTotal));
                    RaisePropertyChanged(nameof(TotalPrice));
                    RefreshPrices();
                }
            }
        }

        public ObservableCollection<OrderFeeViewModel> OrderFees { get; } =
            new ObservableCollection<OrderFeeViewModel>();

        public decimal FeeTotal
        {
            get
            {
                var feeTotal = 0M;
                foreach (var fee in OrderFees)
                {
                    feeTotal += OrderPrice.CalculateFees(fee.FeeType.ToString(),
                        fee.Charge,
                        _basePriceLot,
                        1,
                        nameof(OrderPrice.enumPriceUnit.Lot),
                        0);
                }

                return feeTotal;
            }
        }

        public decimal TotalPrice => _basePriceLot + FeeTotal;

        public ObservableCollection<WorkOrderViewModel> WorkOrders { get; } =
            new ObservableCollection<WorkOrderViewModel>();

        public WorkOrderViewModel SelectedWorkOrder
        {
            get => _selectedWorkOrder;
            set => Set(nameof(SelectedWorkOrder), ref _selectedWorkOrder, value);
        }

        public bool IsAccepted
        {
            get => _isAccepted;
            set => Set(nameof(IsAccepted), ref _isAccepted, value);
        }

        public ObservableCollection<Utilities.MediaLink> MediaLinks { get; } =
            new ObservableCollection<Utilities.MediaLink>();

        public ObservableCollection<Utilities.DocumentLink> DocumentLinks { get; } =
            new ObservableCollection<Utilities.DocumentLink>();

        public bool IsDocumentValid
        {
            get
            {
                if (_selectedCustomer == null || !_selectedCustomer.RequiresDocument)
                {
                    return true;
                }

                return MediaLinks.Count + DocumentLinks.Count > 0;
            }
        }

        public SystemFieldInfo ReqDateSettings
        {
            get => _reqDateSettings;
            set => Set(nameof(ReqDateSettings), ref _reqDateSettings, value);
        }

        public SystemFieldInfo SerialNumberSettings
        {
            get => _serialNumberSettings;
            set => Set(nameof(SerialNumberSettings), ref _serialNumberSettings, value);
        }

        public ObservableCollection<ICustomFieldViewModel> CustomFields { get; } =
            new ObservableCollection<ICustomFieldViewModel>();

        public string ContractReviewText
        {
            get => _contractReviewText;
            private set => Set(nameof(ContractReviewText), ref _contractReviewText, value);
        }

        public string CurrencyMask
        {
            get => _currencyMask;
            private set => Set(nameof(CurrencyMask), ref _currencyMask, value);
        }

        public string NextStepText
        {
            get
            {
                switch (_currentStep)
                {
                    case StepType.Second:
                        return "Finish";
                    default:
                        return "Next";
                }
            }
        }

        public bool ShowBasicSerialNumberEditor => _serialNumberSettings != null
            && _serialNumberSettings.IsVisible 
            && AppSettingsProvider.Settings.SerialNumberEditorType == SerialNumberType.Basic;

        public bool ShowAdvancedSerialNumberEditor => _serialNumberSettings != null
            && _serialNumberSettings.IsVisible
            && AppSettingsProvider.Settings.SerialNumberEditorType == SerialNumberType.Advanced;

        public bool AllowNewParts =>
            AppSettingsProvider.Settings.CanCreatePartsInSalesOrderWizard;

        public ICommand NextStep { get; }

        public ICommand PreviousStep { get; }

        public ICommand AddWorkOrder { get; }

        public ICommand DeleteWorkOrder { get; }

        public ICommand EditWorkOrder { get; }

        public ICommand MoveWorkOrderUp { get; }

        public ICommand MoveWorkOrderDown { get; }

        public ICommand EditFees { get; }

        public IDateTimeNowProvider DateTimeProvider { get; }

        public IDwosApplicationSettingsProvider AppSettingsProvider { get; }

        public ISalesOrderWizardView View { get; private set; }

        internal SalesOrderWizardPersistence Persistence { get; }

        internal ILeadTimeScheduler Scheduler { get; }

        #endregion

        #region Methods

        public SalesOrderWizardViewModel()
        {
            Utilities.ISecurityManager securityManager;
            if (IsInDesignMode)
            {
                DateTimeProvider = new DateTimeNowProvider();
                AppSettingsProvider = new DesignTime.FakeSettingsProvider();
                securityManager = new DesignTime.FakeSecurityManager();
            }
            else
            {
                DateTimeProvider = DependencyContainer.Resolve<IDateTimeNowProvider>();
                AppSettingsProvider = DependencyContainer.Resolve<IDwosApplicationSettingsProvider>();

                var settings = AppSettingsProvider.Settings;
                if (settings.SchedulingEnabled)
                {
                    if (settings.SchedulerType == SchedulerType.ProcessLeadTime)
                    {
                        var scheduler = new RoundingLeadTimeScheduler();
                        scheduler.LoadData();
                        Scheduler = scheduler;
                    }
                    else if (settings.SchedulerType == SchedulerType.ProcessLeadTimeHour)
                    {
                        var scheduler = new LeadTimeScheduler();
                        scheduler.LoadData();
                        Scheduler = scheduler;
                    }
                }

                securityManager = Utilities.SecurityManager.Current;

                CurrencyMask = "{currency:6." + ApplicationSettings.Current.PriceDecimalPlaces + "}";
            }

            Persistence = new SalesOrderWizardPersistence(AppSettingsProvider, securityManager, new DateTimeNowProvider());

            NextStep = new RelayCommand(
                () =>
                {
                    if (_currentStep == StepType.First)
                    {
                        CurrentStep = StepType.Second;
                    }
                    else if (_currentStep == StepType.Second)
                    {
                        DoFinish();
                        CurrentStep = StepType.First;
                    }
                },
                () =>
                {
                    if (_currentStep == StepType.First)
                    {
                        return _selectedCustomer != null
                            && string.IsNullOrEmpty(ValidateAll())
                            && IsDocumentValid
                            && CustomFields.All(field => !field.IsRequired || !string.IsNullOrEmpty(field.Value));
                    }
                    if (_currentStep == StepType.Second)
                    {
                        return _isAccepted
                            && WorkOrders.Count > 0;
                    }

                    return false;
                });

            PreviousStep = new RelayCommand(
                () =>
                {
                    if (_currentStep == StepType.Second)
                    {
                        CurrentStep = StepType.First;
                    }
                });

            AddWorkOrder = new RelayCommand(
                () =>
                {
                    var workOrder = View?.ShowAddOrderDialog(this);
                    if (workOrder != null)
                    {
                        if (HasCorrectProcesses(workOrder))
                        {
                            workOrder.Order = WorkOrders.Count + 1;
                            WorkOrders.Add(workOrder);
                            RefreshPrices();
                            UpdateSchedule(workOrder);
                        }
                        else
                        {
                            View?.ShowWorkOrderProcessesError();
                        }
                    }
                },
                () => _selectedCustomer != null && _currentStep == StepType.Second);

            EditWorkOrder = new RelayCommand(
                () =>
                {
                    var updatedWorkOrder = View?.ShowEditOrderDialog(this, _selectedWorkOrder);
                    if (updatedWorkOrder != null)
                    {
                        if (HasCorrectProcesses(updatedWorkOrder))
                        {
                            _selectedWorkOrder.UpdateFrom(updatedWorkOrder);
                            UpdateSchedule(_selectedWorkOrder);
                        }
                        else
                        {
                            View?.ShowWorkOrderProcessesError();
                        }
                    }
                },
                () => _selectedCustomer != null && _selectedWorkOrder != null && _currentStep == StepType.Second);

            MoveWorkOrderUp = new RelayCommand(
                () =>
                {
                    var previousOrder = WorkOrders.FirstOrDefault(c => c.Order == _selectedWorkOrder.Order - 1);

                    if (previousOrder != null)
                    {
                        previousOrder.Order++;
                        _selectedWorkOrder.Order--;
                        WorkOrderMoved?.Invoke(this, EventArgs.Empty);
                    }
                },
                () => _selectedCustomer != null && _selectedWorkOrder != null && _currentStep == StepType.Second);

            MoveWorkOrderDown = new RelayCommand(
                () =>
                {
                    var nextOrder = WorkOrders.FirstOrDefault(c => c.Order == _selectedWorkOrder.Order + 1);

                    if (nextOrder != null)
                    {
                        nextOrder.Order--;
                        _selectedWorkOrder.Order++;
                        WorkOrderMoved?.Invoke(this, EventArgs.Empty);
                    }

                },
                () => _selectedCustomer != null && _selectedWorkOrder != null && _currentStep == StepType.Second);

            EditFees = new RelayCommand(
                () =>
                {
                    if (View == null)
                    {
                        return;

                    }
                    if (View.ShowEditFeesDialog(this) ?? false)
                    {
                        // Dialog is responsible for accepting/rejecting changes.
                        // Refresh total
                        RaisePropertyChanged(nameof(FeeTotal));
                        RaisePropertyChanged(nameof(TotalPrice));
                        RefreshPrices();

                        // Expedite-related checks
                        var expediteFeeId = Properties.Settings.Default.OrderPriorityExpedite;
                        var expediteFees = OrderFees
                            .Where(fee => fee.OrderFeeTypeId == expediteFeeId)
                            .ToList();

                        if (expediteFees.Count > 0)
                        {
                            // Check to see if priority should be raised
                            if (Priority == Properties.Settings.Default.OrderPriorityDefault && (View.ShowPriorityWarning()))
                            {
                                Priority = Properties.Settings.Default.OrderPriorityExpedite;
                            }

                            // Warn if total rush charge is below minimum
                            var totalRushCharge = expediteFees
                                .Sum(fee => fee.CalculateAmount(_basePriceLot, 1, OrderPrice.enumPriceUnit.Lot, 0));

                            if (totalRushCharge < Properties.Settings.Default.MinimumOrderRushCharge)
                            {
                                View.ShowRushChargeWarning(totalRushCharge, Properties.Settings.Default.MinimumOrderRushCharge);
                            }
                        }
                    }
                });

            DeleteWorkOrder = new RelayCommand(
                () =>
                {
                    if (View?.ShowDeleteOrderDialog() ?? false)
                    {
                        WorkOrders.Remove(_selectedWorkOrder);
                    }
                },
                () => _selectedCustomer != null && _selectedWorkOrder != null && _currentStep == StepType.Second);

            MediaLinks.CollectionChanged += MediaCollectionChanged;
            DocumentLinks.CollectionChanged += MediaCollectionChanged;

            AutoUpdateSalesOrderDates();
        }

        public override string Validate(string propertyName)
        {
            if (propertyName == nameof(SelectedCustomer))
            {
                if (_selectedCustomer == null)
                {
                    return "Customer is required.";
                }
            }
            else if (propertyName == nameof(CustomerWorkOrder))
            {
                if (_selectedCustomer != null && _selectedCustomer.RequiresCustomerWorkOrder && string.IsNullOrEmpty(_customerWorkOrder))
                {
                    return "Customer WO is required.";
                }
            }
            else if (propertyName == nameof(PurchaseOrderNumber))
            {
                if (_selectedCustomer != null && _selectedCustomer.RequiresPurchaseOrder && string.IsNullOrEmpty(_purchaseOrderNumber))
                {
                    return "Purchase Order is required.";
                }
            }
            else if (propertyName == nameof(RequiredDate))
            {
                if (_reqDateSettings != null && _reqDateSettings.IsRequired && !_requiredDate.HasValue)
                {
                    return "Required date is required.";
                }
            }
            else if (propertyName == nameof(OrderDate))
            {
                if (!_orderDate.HasValue)
                {
                    return "Order Date is required.";
                }
            }
            else if (propertyName == nameof(EstShipDate))
            {
                if (!_estShipDate.HasValue)
                {
                    return "Estimated Ship Date is required.";
                }
            }
            else if (propertyName == nameof(Priority))
            {
                if (string.IsNullOrEmpty(_priority))
                {
                    return "Priority is required.";
                }
            }

            return null;
        }

        public override string ValidateAll() => Validate(nameof(SelectedCustomer))
            + Validate(nameof(CustomerWorkOrder))
            + Validate(nameof(PurchaseOrderNumber))
            + Validate(nameof(RequiredDate))
            + Validate(nameof(OrderDate))
            + Validate(nameof(EstShipDate))
            + Validate(nameof(Priority));

        private void RefreshPrices()
        {
            if (WorkOrders.Count == 0)
            {
                return;
            }

            var basePricePerOrder = _basePriceLot / WorkOrders.Count;

            var isFirstWorkOrder = true;
            foreach (var workOrder in WorkOrders)
            {
                var feeAmount = 0M;
                if (isFirstWorkOrder)
                {
                    // Apply fixed fees to first order only
                    var fixedFees = OrderFees
                        .Where(fee => fee.FeeType == OrderPrice.enumFeeType.Fixed);

                    foreach (var fixedFee in fixedFees)
                    {
                        feeAmount += OrderPrice.CalculateFees(
                            nameof(OrderPrice.enumFeeType.Fixed),
                            fixedFee.Charge,
                            basePricePerOrder,
                            workOrder.Quantity,
                            nameof(OrderPrice.enumPriceUnit.Lot),
                            0M);
                    }
                }

                // Apply percentage fees
                var percentFees = OrderFees
                    .Where(fee => fee.FeeType == OrderPrice.enumFeeType.Percentage);

                foreach (var percentFee in percentFees)
                {
                    feeAmount += OrderPrice.CalculateFees(
                        nameof(OrderPrice.enumFeeType.Percentage),
                        percentFee.Charge,
                        basePricePerOrder,
                        workOrder.Quantity,
                        nameof(OrderPrice.enumPriceUnit.Lot),
                        0M);
                }

                workOrder.TotalPrice = basePricePerOrder + feeAmount;

                isFirstWorkOrder = false;
            }
        }

        private void UpdateSchedule(WorkOrderViewModel workOrder)
        {
            if (workOrder == null || Scheduler == null)
            {
                return;
            }

            var estShipDate = Scheduler.UpdateScheduleDates(
                new ViewModelLeadTimeOrder(workOrder));

            // Update the estimated ship date to be the calculated one
            // at minimum.
            if (_autoUpdateSalesOrderDates && _estShipDate <= estShipDate)
            {
                EstShipDate = estShipDate;
            }
        }

        private void DoFinish()
        {
            // Assumption: Data is valid and there is at least one Work Order

            // Save
            var salesOrderId = Persistence.SaveSalesOrder(this);

            // Reset fields
            IsAccepted = false;
            AutoUpdateSalesOrderDates();
            AutoUpdateSalesOrderFields();
            Priority = Properties.Settings.Default.OrderPriorityDefault;
            MediaLinks.Clear();
            DocumentLinks.Clear();
            WorkOrders.Clear();
            OrderFees.Clear();

            // Trigger Saved notification
            Saved?.Invoke(this, new Utilities.SavedDataEventArgs(salesOrderId));
        }

        private void AutoUpdateSalesOrderDates()
        {
            var currentTime = DateTimeProvider.Now;
            OrderDate = currentTime;
            DateTime defaultShipDate = currentTime.AddBusinessDays(_selectedCustomer?.LeadTime ?? AppSettingsProvider.Settings.OrderLeadTime);

            if (_reqDateSettings?.IsVisible ?? false)
            {
                RequiredDate = defaultShipDate;
            }

            EstShipDate = defaultShipDate;

            _autoUpdateSalesOrderDates = true;
        }

        private void AutoUpdateSalesOrderFields()
        {
            CustomerWorkOrder = _selectedCustomer?.DefaultCustomerWorkOrder;
            PurchaseOrderNumber = _selectedCustomer?.DefaultPurchaseOrder;
        }

        private void ResetCustomFields()
        {
            CustomFields.Clear();

            var fields = _selectedCustomer?.Fields
                ?? Enumerable.Empty<CustomField>();

            foreach (var field in fields.OrderByDescending(f => f.IsRequired).ThenBy(f => f.Name))
            {
                CustomFields.Add(CreateViewModel(field));
            }
        }

        public void Initialize(ISalesOrderWizardView view)
        {
            View = view ?? throw new ArgumentNullException(nameof(view));

            // Settings
            ReqDateSettings = Persistence.GetField("Order", "Required Date");
            SerialNumberSettings = Persistence.GetField("Order", "Serial Number");

            // Customers
            Customers.Clear();

            foreach (var customer in Persistence.RetrieveCustomers().OrderBy(c => c.Name))
            {
                Customers.Add(customer);
            }

            SelectedCustomer = Customers.FirstOrDefault();

            // Priorities
            foreach (var priority in Persistence.RetrievePriorities().OrderBy(p => p))
            {
                Priorities.Add(priority);
            }

            Priority = Properties.Settings.Default.OrderPriorityDefault;

            // Contract review text
            ContractReviewText =
                FormatContractReviewText(AppSettingsProvider.Settings.ContractReviewText);
        }

        private void LoadProcesses(PartViewModel part)
        {
            if (part == null || part.Processes != null)
            {
                return;
            }

            part.LoadProcesses(Persistence.GetPartProcesses(part.PartId));
        }

        private bool HasCorrectProcesses(WorkOrderViewModel workOrder)
        {
            if (workOrder == null)
            {
                return false;
            }

            var skipProcessCheck = AppSettingsProvider.Settings.AllowDifferentProcessesInSalesOrderWizard
                || workOrder.SelectedPart.IsNew // Part added through redline
                || (WorkOrders.Count == 0 // First order
                || (WorkOrders.Count == 1 && workOrder.Order == WorkOrders.First().Order)) // Updating only order
                || WorkOrders.All(wo => wo.SelectedPart.IsNew); // All parts are new

            if (skipProcessCheck)
            {
                return true;
            }

            // Assumption: all Work Orders in the Sales Order are either new
            // or have the same processes.
            var requiredProcesses = WorkOrders
                .First(wo => !wo.SelectedPart.IsNew)
                .Processes
                .OrderBy(p => p.StepOrder)
                .ToList();

            var workOrderProcesses = workOrder
                .Processes
                .OrderBy(p => p.StepOrder)
                .ToList();

            var hasCorrectProcesses = true;
            if (workOrder.Processes.Count == requiredProcesses.Count)
            {
                for (var i = 0; i < requiredProcesses.Count; i++)
                {
                    var requiredProcess = requiredProcesses[i];
                    var workOrderProcess = workOrder.Processes[i];

                    if (requiredProcess.ProcessId != workOrderProcess.ProcessId)
                    {
                        hasCorrectProcesses = false;
                        break;
                    }
                }
            }
            else
            {
                hasCorrectProcesses = false;
            }

            return hasCorrectProcesses;
        }

        private static string FormatContractReviewText(string contractReviewText)
        {
            // Contract review text is a snippet using HTML formatting
            const string contractReviewStyle = @"html, body { margin: 0; padding: 0; font-family: 'Verdana'}";
            return $@"<!DOCTYPE html><html><head><style>{contractReviewStyle}</style></head><body>{contractReviewText}</body></html>";
        }

        private static ICustomFieldViewModel CreateViewModel(CustomField field)
        {
            if (field == null)
            {
                return null;
            }

            if (field.ListItems.Count > 0)
            {
                return ComboBoxFieldViewModel.From(field);
            }

            return TextFieldViewModel.From(field);
        }

        #endregion

        #region Events

        private void MediaCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                RaisePropertyChanged(nameof(IsDocumentValid));
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error updating media count.");
            }
        }

        #endregion

        #region WorkOrderViewModel

        public class WorkOrderViewModel : Utilities.ViewModelBase
        {
            #region Fields

            public event EventHandler Accepted;
            private int _orderId;
            private string _customerWorkOrder;
            private PartViewModel _selectedPart;
            private int _quantity;
            private decimal _totalPrice;

            #endregion

            #region Properties

            public int Order
            {
                get => _orderId;
                set => Set(nameof(Order), ref _orderId, value);
            }

            public string CustomerWorkOrder
            {
                get => _customerWorkOrder;
                set => Set(nameof(CustomerWorkOrder), ref _customerWorkOrder, value);
            }

            public PartViewModel SelectedPart
            {
                get => _selectedPart;
                set
                {
                    if (Set(nameof(SelectedPart), ref _selectedPart, value))
                    {
                        RaisePropertyChanged(nameof(Weight));
                        LoadProcesses();
                    }
                }
            }

            public int Quantity
            {
                get => _quantity;
                set
                {
                    if (Set(nameof(Quantity), ref _quantity, value))
                    {
                        RaisePropertyChanged(nameof(Weight));
                        RaisePropertyChanged(nameof(AreSerialNumbersValid));
                    }
                }
            }

            public decimal? Weight
            {
                get
                {
                    var partWeight = _selectedPart?.Weight;

                    if (partWeight.HasValue)
                    {
                        return _quantity * partWeight;
                    }

                    return null;
                }
            }

            public ObservableCollection<OrderProcessViewModel> Processes { get; } =
                new ObservableCollection<OrderProcessViewModel>();

            public string SerialNumber
            {
                get => SerialNumbers.FirstOrDefault()?.Number;
                set
                {
                    var existingNumber = SerialNumbers.FirstOrDefault();
                    if (string.IsNullOrEmpty(value))
                    {
                        if (existingNumber != null)
                        {
                            SerialNumbers.Clear();
                            RaisePropertyChanged(nameof(SerialNumber));
                        }
                    }
                    else
                    {
                        if (existingNumber != null)
                        {
                            var previousValue = existingNumber.Number;
                            existingNumber.Number = value;
                            if (previousValue != value)
                            {
                                RaisePropertyChanged(nameof(SerialNumber));
                            }
                        }
                        else
                        {
                            SerialNumbers.Add(new SerialNumberViewModel() { Number = value });
                            RaisePropertyChanged(nameof(SerialNumber));
                        }
                    }
                }
            }

            public ObservableCollection<SerialNumberViewModel> SerialNumbers { get; } =
                new ObservableCollection<SerialNumberViewModel>();

            public bool CanAddSerialNumbers =>
                SerialNumbers.Count < 10000;

            /// <summary>
            /// Gets or set the total price for this instance.
            /// </summary>
            /// <remarks>
            /// Uses for display only, as it includes order fees.
            /// </remarks>
            public decimal TotalPrice
            {
                get => _totalPrice;
                set => Set(nameof(TotalPrice), ref _totalPrice, value);
            }

            public SalesOrderWizardViewModel SalesOrder { get; }

            public ICommand Accept { get; }

            public bool IsValid => string.IsNullOrEmpty(ValidateAll());

            public bool AreSerialNumbersValid
            {
                get
                {
                    if (!SalesOrder.ShowAdvancedSerialNumberEditor || SalesOrder.SerialNumberSettings == null)
                    {
                        return true;
                    }

                    // Require the number of serial numbers to be equal to Quantity
                    // if requiring -or- entering serial numbers.
                    var countsMustBeEqual = SalesOrder.SerialNumberSettings.IsRequired ||
                        SerialNumbers.Count > 0;

                    return !countsMustBeEqual || SerialNumbers.Count == _quantity;
                }
            }

            #endregion

            #region Methods

            public WorkOrderViewModel(SalesOrderWizardViewModel salesOrder)
            {
                SalesOrder = salesOrder ?? throw new ArgumentNullException(nameof(salesOrder));

                Accept = new RelayCommand(
                    () => Accepted?.Invoke(this, EventArgs.Empty),
                    () => IsValid);

                SerialNumbers.CollectionChanged += SerialNumbers_CollectionChanged;
            }

            public override string Validate(string propertyName)
            {
                if (propertyName == nameof(SelectedPart))
                {
                    if (_selectedPart == null)
                    {
                        return "Part is required.";
                    }
                }
                else if (propertyName == nameof(SerialNumber) || propertyName == nameof(SerialNumbers))
                {
                    bool isRequired = SalesOrder.SerialNumberSettings?.IsRequired
                        ?? false;

                    if (isRequired && SerialNumbers.Count == 0)
                    {
                        return "Serial number is required.";
                    }
                    else if (!AreSerialNumbersValid)
                    {
                        return "Serial numbers are invalid.";
                    }
                }
                else if (propertyName == nameof(Quantity))
                {
                    if (_quantity <= 0)
                    {
                        return "Quantity is required.";
                    }
                }
                else if (propertyName == nameof(CustomerWorkOrder))
                {
                    var customer = SalesOrder.SelectedCustomer;
                    if (customer != null && customer.RequiresCustomerWorkOrder && string.IsNullOrEmpty(_customerWorkOrder))
                    {
                        return "Customer WO is required.";
                    }
                }

                return null;
            }

            public override string ValidateAll() => Validate(nameof(SelectedPart))
                + Validate(nameof(SerialNumber))
                + Validate(nameof(SerialNumbers))
                + Validate(nameof(Quantity))
                + Validate(nameof(CustomerWorkOrder));

            public WorkOrderViewModel Clone()
            {
                var newViewModel = new WorkOrderViewModel(SalesOrder)
                {
                    _orderId = _orderId,
                    _customerWorkOrder = _customerWorkOrder,
                    _selectedPart = _selectedPart,
                    _quantity = _quantity,
                    _totalPrice = _totalPrice
                };

                foreach (var process in Processes)
                {
                    newViewModel.Processes.Add(process.Clone());
                }

                foreach (var serialNumber in SerialNumbers)
                {
                    newViewModel.SerialNumbers.Add(serialNumber.Clone());
                }

                return newViewModel;
            }

            public void UpdateFrom(WorkOrderViewModel newData)
            {
                // Assumption - Sales Order is the same
                CustomerWorkOrder = newData._customerWorkOrder;
                SelectedPart = newData._selectedPart;
                Quantity = newData._quantity;
                TotalPrice = newData._totalPrice;

                Processes.Clear();
                foreach (var process in newData.Processes)
                {
                    Processes.Add(process.Clone());
                }

                SerialNumbers.Clear();
                foreach (var serialNumber in newData.SerialNumbers)
                {
                    SerialNumbers.Add(serialNumber.Clone());
                }
            }

            private void LoadProcesses()
            {
                if (_selectedPart == null)
                {
                    return;
                }

                SalesOrder.LoadProcesses(_selectedPart);

                Processes.Clear();
                foreach (var process in _selectedPart.Processes)
                {
                    Processes.Add(OrderProcessViewModel.From(process, SalesOrder.AppSettingsProvider.Settings));
                }
            }

            #endregion

            #region Events

            private void SerialNumbers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                try
                {
                    if (e.Action == NotifyCollectionChangedAction.Add && SerialNumbers.Count == (_quantity + 1))
                    {
                        Quantity += 1;
                        // Automatically raises serial numbers valid property
                    }
                    else
                    {
                        RaisePropertyChanged(nameof(AreSerialNumbersValid));
                    }

                    RaisePropertyChanged(nameof(CanAddSerialNumbers));
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger()
                        .Error(exc, "Error handling collection changed event.");
                }
            }

            #endregion
        }

        #endregion

        #region OrderProcessViewModel

        public class OrderProcessViewModel : Utilities.ViewModelBase
        {
            #region Fields

            private int _stepOrder;
            private bool _cocData;
            private decimal? _loadCapacityVariance;
            private decimal? _loadCapacityWeight;
            private int? _loadCapacityQuantity;
            private ProcessLeadTime _leadTime;
            private DateTime? _estEndDate;

            #endregion

            #region Properties

            public int ProcessId { get; }

            public int ProcessAliasId { get; }

            public string Department { get; }

            public int StepOrder
            {
                get => _stepOrder;
                set => Set(nameof(StepOrder), ref _stepOrder, value);
            }

            public bool CocData
            {
                get => _cocData;
                set => Set(nameof(CocData), ref _cocData, value);
            }

            public decimal? LoadCapacityVariance
            {
                get => _loadCapacityVariance;
                set => Set(nameof(LoadCapacityVariance), ref _loadCapacityVariance, value);
            }

            public decimal? LoadCapacityWeight
            {
                get => _loadCapacityWeight;
                set => Set(nameof(LoadCapacityWeight), ref _loadCapacityWeight, value);
            }

            public int? LoadCapacityQuantity
            {
                get => _loadCapacityQuantity;
                set => Set(nameof(LoadCapacityQuantity), ref _loadCapacityQuantity, value);
            }

            public ProcessLeadTime LeadTime
            {
                get => _leadTime;
                set => Set(nameof(ProcessLeadTime), ref _leadTime, value);
            }

            public DateTime? EstEndDate
            {
                get => _estEndDate;
                set => Set(nameof(EstEndDate), ref _estEndDate, value);
            }

            #endregion

            #region Methods

            public OrderProcessViewModel(int processId, int processAliasId, string department)
            {
                ProcessId = processId;
                ProcessAliasId = processAliasId;
                Department = department;
            }

            public OrderProcessViewModel Clone()
            {
                return new OrderProcessViewModel(ProcessId, ProcessAliasId, Department)
                {
                    _stepOrder = _stepOrder,
                    _cocData = _cocData,
                    _loadCapacityVariance = _loadCapacityVariance,
                    _loadCapacityWeight = _loadCapacityWeight,
                    _loadCapacityQuantity = _loadCapacityQuantity
                };
            }

            public static OrderProcessViewModel From(PartProcess process, ApplicationSettings settings)
            {
                if (settings == null)
                {
                    throw new ArgumentNullException(nameof(settings));
                }

                if (process == null)
                {
                    return null;
                }

                return new OrderProcessViewModel(process.ProcessId, process.ProcessAliasId, process.Department)
                {
                    _stepOrder = process.StepOrder,
                    _cocData = process.CocCount > 0 || settings.DisplayProcessCOCByDefault,
                    _loadCapacityVariance = process.LoadCapacityVariance,
                    _loadCapacityWeight = process.LoadCapacityWeight,
                    _loadCapacityQuantity = process.LoadCapacityQuantity,
                    _leadTime = process.LeadTime
                };
            }

            #endregion
        }

        #endregion

        #region SerialNumberViewModel

        public class SerialNumberViewModel : Utilities.ViewModelBase
        {
            private string _number;

            public string Number
            {
                get => _number;
                set => Set(nameof(Number), ref _number, value);
            }

            public SerialNumberViewModel Clone() =>
                new SerialNumberViewModel { _number = _number };
        }

        #endregion

        #region ViewModelLeadTimeOrder

        private class ViewModelLeadTimeOrder : ILeadTimeOrder
        {
            public WorkOrderViewModel ViewModel { get; }

            public ViewModelLeadTimeOrder(WorkOrderViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public int OrderId => ViewModel.Order;

            public int? PartQuantity => ViewModel.Quantity;

            public bool HasPartMarking => false;

            public IEnumerable<ILeadTimeProcess> Processes => ViewModel.Processes
                .Select(p => new ViewModelLeadTimeProcess(p));
        }

        #endregion

        #region ViewModelLeadTimeProcess

        private class ViewModelLeadTimeProcess : ILeadTimeProcess
        {
            public OrderProcessViewModel ViewModel { get; }

            public ViewModelLeadTimeProcess(OrderProcessViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public int ProcessId => ViewModel.ProcessId;

            public int StepOrder => ViewModel.StepOrder;

            public ProcessLeadTime LeadTime => ViewModel.LeadTime;

            public DateTime? EstEndDate
            {
                get => ViewModel.EstEndDate;
                set => ViewModel.EstEndDate = value;
            }
        }

        #endregion

        #region StepType

        public enum StepType
        {
            First,
            Second
        }

        #endregion
    }
}
