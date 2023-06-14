using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using DWOS.AutomatedWorkOrderTool.Messages;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.AutomatedWorkOrderTool.Services;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Date;
using DWOS.Data.Order;
using DWOS.Reports;
using DWOS.Reports.Reports;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Infragistics.Documents.Excel;
using NLog;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class ShippingManifestViewModel : ViewModelBase
    {
        #region Fields

        public event EventHandler DialogExit;
        public event EventHandler OrdersLoaded;

        private CustomerViewModel _customer;
        private string _shippingFileName;
        private DialogStatus _currentStatus;
        private bool _isLoading;
        private bool _hasError;
        private string _errorText;
        private decimal _importProgress;
        private DateTime? _orderDate;
        private bool _printWorkOrderTravelers = true;

        #endregion

        #region Properties

        public IFileService FilePickerService { get; }

        public IDocumentManager DocumentManager { get; }

        public ICustomerManager CustomerManager { get; }

        public ILeadTimePersistence LeadTimePersistence { get; }

        public IPriceUnitPersistence PriceUnitPersistence { get; }

        public ISettingsProvider SettingsProvider { get; }

        public IUserManager UserManager { get; }

        public IDataManager DataManager { get; }

        public CustomerViewModel Customer
        {
            get => _customer;
            set => Set(nameof(Customer), ref _customer, value);
        }

        public DialogStatus CurrentStatus
        {
            get => _currentStatus;
            set
            {
                if (Set(nameof(CurrentStatus), ref _currentStatus, value))
                {
                    RaisePropertyChanged(nameof(ShowSetup));
                    RaisePropertyChanged(nameof(ShowConfirmation));
                    RaisePropertyChanged(nameof(ShowImport));
                    RaisePropertyChanged(nameof(ShowResults));
                }
            }
        }

        public ObservableCollection<CustomField> Fields { get; } =
            new ObservableCollection<CustomField>();

        public bool ShowSetup => _currentStatus == DialogStatus.Setup;

        public bool ShowConfirmation => _currentStatus == DialogStatus.Confirmation;

        public bool ShowImport => _currentStatus == DialogStatus.Import;

        public bool ShowResults => _currentStatus == DialogStatus.Results;

        public ICommand SelectShippingManifest { get; }

        public string ShippingFileName
        {
            get => _shippingFileName;
            set => Set(nameof(ShippingFileName), ref _shippingFileName, value);
        }

        public ICommand Continue { get; }

        public ICommand CloseDialog { get; }

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(nameof(IsLoading), ref _isLoading, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => Set(nameof(HasError), ref _hasError, value);
        }

        public string ErrorText
        {
            get => _errorText;
            set => Set(nameof(ErrorText), ref _errorText, value);
        }

        public ObservableCollection<ShippingManifestOrder> Orders { get; } =
            new ObservableCollection<ShippingManifestOrder>();

        public ObservableCollection<ImportSummaryItem> ImportDetails { get; } =
            new ObservableCollection<ImportSummaryItem>();

        public ICommand GoBack { get; }

        public ICommand ImportOrders { get; }

        public decimal ImportProgress
        {
            get => _importProgress;
            set => Set(nameof(ImportProgress), ref _importProgress, value);
        }

        public DateTime? OrderDate
        {
            get => _orderDate;
            set => Set(nameof(OrderDate), ref _orderDate, value);
        }

#if SHOW_TRAVELER_OPTION
        public bool ShowWorkOrderTravelerOption => true;
#else
        public bool ShowWorkOrderTravelerOption => false;
#endif

        public bool PrintWorkOrderTravelers
        {
            get => _printWorkOrderTravelers;
            set => Set(nameof(PrintWorkOrderTravelers), ref _printWorkOrderTravelers, value);
        }

        #endregion

#region Methods

        public ShippingManifestViewModel(IMessenger messenger, IFileService filePicker,
            ICustomerManager customerManager, IDocumentManager documentManager, IDataManager dataManager,
            IUserManager userManager, ISettingsProvider settingsProvider, IPriceUnitPersistence priceUnitPersistence,
            ILeadTimePersistence leadTimePersistence)
            : base(messenger)
        {
            FilePickerService = filePicker ?? throw new ArgumentNullException(nameof(filePicker));
            CustomerManager = customerManager ?? throw new ArgumentNullException(nameof(customerManager));
            DocumentManager = documentManager ?? throw new ArgumentNullException(nameof(documentManager));
            DataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            SettingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            PriceUnitPersistence = priceUnitPersistence ?? throw new ArgumentNullException(nameof(priceUnitPersistence));
            LeadTimePersistence = leadTimePersistence ?? throw new ArgumentNullException(nameof(leadTimePersistence));

            SelectShippingManifest = new RelayCommand(() =>
            {
                var newMasterList = FilePickerService.GetSpreadsheet();

                if (!string.IsNullOrEmpty(newMasterList))
                {
                    ShippingFileName = newMasterList;
                }
            });

            Continue = new RelayCommand(DoContinue, CanContinue);
            CloseDialog = new RelayCommand(() => DialogExit?.Invoke(this, EventArgs.Empty));
            GoBack = new RelayCommand(DoGoBack, CanGoBack);

            ImportOrders = new RelayCommand(DoImportOrders, () => Orders.Count > 0 && ImportProgress == 0);
        }

        public void Load(CustomerViewModel customer)
        {
            Customer = customer;
            IsLoading = false;
            Fields.Clear();
            CurrentStatus = DialogStatus.Setup;
            OrderDate = null;
            ImportProgress = 0;
            ImportDetails.Clear();
            ShippingFileName = string.Empty;

            if (customer == null)
            {
                return;
            }

            foreach (var customField in CustomerManager.GetCustomFields(customer).Where(c => c.IsVisible))
            {
                Fields.Add(CustomField.From(customField));
            }
        }

        private async void DoContinue()
        {
            try
            {
                if (_currentStatus == DialogStatus.Setup)
                {
                    IsLoading = true;
                    CurrentStatus = DialogStatus.Confirmation;

                    var importer = new ShippingManifestImporter(_shippingFileName, _customer, DocumentManager);
                    var result = importer.ValidateFile();
                    HasError = !result.IsSuccessful;
                    ErrorText = result.IsSuccessful ? string.Empty : result.Message;

                    Orders.Clear();

                    if (result.IsSuccessful)
                    {
                        OrderDate = importer.GetOrderDateFromWorksheet();
                        var worksheetOrders = await importer.GetOrdersFromWorksheet();
                        await ValidateAsync(_customer, worksheetOrders);

                        foreach (var order in worksheetOrders)
                        {
                            Orders.Add(order);
                        }

                        if (worksheetOrders.All(order => order.Status == ShippingManifestOrder.OrderStatus.Invalid))
                        {
                            HasError = true;
                            ErrorText = "No valid orders to import.";
                        }
                    }

                    // Trigger event for parts load so the UI can sort items
                    OrdersLoaded?.Invoke(this, EventArgs.Empty);

                    // Set IsLoading to false; this must be the last thing
                    // that continue does because unit tests depend on its
                    // value being set.
                    IsLoading = false;
                }
                else if (_currentStatus == DialogStatus.Confirmation)
                {
                    ImportProgress = 0;
                    CurrentStatus = DialogStatus.Import;
                }
                else if (_currentStatus == DialogStatus.Import)
                {
                    CurrentStatus = DialogStatus.Results;
                }
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error pressing continue in wizard for shipping manifest."));
            }
        }

        private Task ValidateAsync(CustomerViewModel customer, List<ShippingManifestOrder> orders)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            if (orders == null)
            {
                throw new ArgumentNullException(nameof(orders));
            }

            return Task.Factory.StartNew(() =>
            {
                // Load part data
                using (var dsParts = new PartsDataset())
                {
                    DataManager.LoadPartData(dsParts, _customer.Id);

                    foreach (var order in orders)
                    {
                        // Check required fields
                        if (string.IsNullOrEmpty(order.Part))
                        {
                            order.Status = ShippingManifestOrder.OrderStatus.Invalid;
                            order.ImportNotes = "Does not have a part.";
                            continue;
                        }

                        if (!order.Quantity.HasValue)
                        {
                            order.Status = ShippingManifestOrder.OrderStatus.Invalid;
                            order.ImportNotes = "Does not have a quantity.";
                            continue;
                        }

                        var matchingPart = dsParts.Part.FirstOrDefault(p => p.Name == order.Part);

                        if (matchingPart == null)
                        {
                            order.Status = ShippingManifestOrder.OrderStatus.Invalid;
                            order.ImportNotes = "Part not found.";
                        }
                        else if (!matchingPart.Active)
                        {
                            order.Status = ShippingManifestOrder.OrderStatus.Invalid;
                            order.ImportNotes = "Part is inactive.";
                        }
                        else if (!string.IsNullOrEmpty(order.WorkOrder) && DataManager.GetMatchingWorkOrder(order.WorkOrder, customer.Id) is OrdersDataSet.OrderRow existingOrder)
                        {

                            order.ExistingOrderId = existingOrder.OrderID;
                            if (existingOrder.IsPartIDNull() || matchingPart.PartID != existingOrder.PartID)
                            {
                                // Order with same Customer WO has a different part
                                order.Status = ShippingManifestOrder.OrderStatus.Invalid;
                                order.ImportNotes = $"Part does not match existing WO: {existingOrder.OrderID}";
                            }
                            else
                            {
                                // Order exists in DWOS
                                order.Status = ShippingManifestOrder.OrderStatus.Existing;
                                order.ImportNotes = $"WO exists in DWOS: {existingOrder.OrderID}";
                            }
                        }
                        else
                        {
                            order.Status = DataManager.GetOrderCount(matchingPart.PartID) > 0
                                ? ShippingManifestOrder.OrderStatus.New
                                : ShippingManifestOrder.OrderStatus.NewWithoutExistingOrders;

                            order.ImportNotes = string.Empty;
                        }
                    }
                }
            });
        }

        private void DoGoBack()
        {
            try
            {
                if (_currentStatus == DialogStatus.Confirmation)
                {
                    CurrentStatus = DialogStatus.Setup;
                }
                else if (_currentStatus == DialogStatus.Import)
                {
                    CurrentStatus = DialogStatus.Confirmation;
                }
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error going back."));
            }
        }

        private void DoImportOrders()
        {
            const int overheadEnd = 5;

            try
            {
                var ordersToImport = Orders
                    .Where(CanImport)
                    .ToList();

                if (ordersToImport.Count == 0)
                {
                    ImportDetails.Add(new ImportSummaryItem(ImportSummaryItem.ItemType.Info,
                        "There were no parts to import."));

                    return;
                }

                var user = UserManager.CurrentUser;

                if (user == null)
                {
                    ImportDetails.Add(new ImportSummaryItem(ImportSummaryItem.ItemType.Info,
                        "Must be logged-in to import orders."));

                    return;
                }

                ImportProgress = 0;
                var orderDate = OrderDate ?? DateTime.Now;
                var shippingManifestFile = SaveShippingManifest(orderDate);

                // Import orders
                var progressStep = (100 - overheadEnd) / Convert.ToDecimal(ordersToImport.Count);

                ILeadTimeScheduler scheduler = null;
                if (SettingsProvider.UseLeadTimeDayScheduling)
                {
                    var roundingScheduler = new RoundingLeadTimeScheduler(new DateTimeNowProvider());
                    roundingScheduler.LoadData(LeadTimePersistence, DateTime.Now);
                    scheduler = roundingScheduler;
                }
                else if (SettingsProvider.UseLeadTimeHourScheduling)
                {
                    var hourScheduler = new LeadTimeScheduler(new DateTimeNowProvider());
                    hourScheduler.LoadData(LeadTimePersistence);
                    scheduler = hourScheduler;
                }

                var reportItems = new List<IImportReportItem>();

                using (var dsOrders = new OrdersDataSet())
                {
                    DataManager.LoadOrderData(dsOrders, _customer.Id);

                    var customerRow = dsOrders.CustomerSummary.FirstOrDefault(row => row.CustomerID == _customer.Id);
                    var productClass = CustomerManager.GetDefaultValue(_customer, "Product Class");

                    foreach (var order in Orders)
                    {
                        if (order.Status == ShippingManifestOrder.OrderStatus.Invalid)
                        {
                            reportItems.Add(ErrorImportReportItem.CreateFrom(order));
                            continue;
                        }

                        if (order.Status == ShippingManifestOrder.OrderStatus.Existing)
                        {
                            reportItems.Add(ErrorImportReportItem.CreateFrom(order));
                            continue;
                        }

                        ImportProgress += progressStep;

                        var matchingPart = dsOrders.PartSummary.FirstOrDefault(part => part.Active && part.Name == order.Part);

                        if (matchingPart != null)
                        {
                            // Create order
                            var orderQuantity = order.Quantity ?? 0;
                            var orderWeight = matchingPart.IsWeightNull() ? 0 : matchingPart.Weight * orderQuantity;

                            var orderRow = dsOrders.Order.NewOrderRow();
                            orderRow.FromShippingManifest = true;
                            orderRow.CustomerID = _customer.Id;
                            orderRow.PartSummaryRow = matchingPart;
                            orderRow.PartQuantity = orderQuantity;
                            orderRow.Weight = orderWeight;
                            orderRow.Priority = "Normal";
                            orderRow.CreatedBy = user.Id;
                            orderRow.ContractReviewed = true; // Automatically review contracts
                            orderRow.Hold = false;
                            orderRow.OrderType = (int)OrderType.Normal;
                            orderRow.OriginalOrderType = (int)OrderType.Normal;
                            orderRow.WorkStatus = GetNewWorkOrderStatus();
                            orderRow.Status = "Open";
                            orderRow.CurrentLocation = SettingsProvider.DepartmentSales;

                            if (!string.IsNullOrEmpty(order.WorkOrder))
                            {
                                orderRow.CustomerWO = order.WorkOrder;
                            }

                            if (!string.IsNullOrEmpty(order.PurchaseOrder))
                            {
                                orderRow.PurchaseOrder = order.PurchaseOrder;
                            }

                            if (_orderDate.HasValue)
                            {
                                orderRow.OrderDate = _orderDate.Value;
                            }

                            if (order.DueDate.HasValue)
                            {
                                orderRow.RequiredDate = order.DueDate.Value;
                            }

                            (var priceUnit, var basePrice) = GetPriceData(matchingPart, orderQuantity, orderWeight);

                            orderRow.PriceUnit = priceUnit.ToString();
                            orderRow.BasePrice = basePrice;

                            dsOrders.Order.AddOrderRow(orderRow);

                            // Shipping method
                            var shippingMethods = customerRow.GetCustomerShippingSummaryRows();
                            orderRow.CustomerShippingSummaryRow =
                                shippingMethods.FirstOrDefault(r => r.DefaultShippingMethod) ??
                                shippingMethods.FirstOrDefault();

                            // Customer Address
                            var customerAddresses = customerRow.GetCustomerAddressRows();
                            orderRow.CustomerAddressRow =
                                customerAddresses.FirstOrDefault(r => r.IsDefault) ??
                                customerAddresses.FirstOrDefault();

                            // Assumption: Imported work orders should never
                            // use the default Customer WO or PO as they can
                            // be provided during import

                            // Product class
                            if (!string.IsNullOrEmpty(productClass))
                            {
                                dsOrders.OrderProductClass.AddOrderProductClassRow(orderRow, productClass);
                            }

                            // Add processes
                            var stepOrder = 1;
                            var previousProcessIds = new List<int>();

                            var processLeadTimes = new OrderProcessLeadTimes();
                            foreach (var processRow in matchingPart.GetPartProcessSummaryRows().OrderBy(p => p.StepOrder))
                            {
                                var orderProcessRow = dsOrders.OrderProcesses.NewOrderProcessesRow();
                                orderProcessRow.OrderRow = orderRow;
                                orderProcessRow.ProcessID = processRow.ProcessID;
                                orderProcessRow.ProcessAliasID = processRow.ProcessAliasID;
                                orderProcessRow.StepOrder = stepOrder;
                                orderProcessRow.Department = processRow.Department;
                                orderProcessRow.COCData =
                                    (!processRow.IsCOCCountNull() && processRow.COCCount > 0) ||
                                    SettingsProvider.DisplayProcessCocByDefault;

                                orderProcessRow.OrderProcessType = (int)OrderProcessType.Normal;

                                // Load capacity
                                if (!processRow.IsLoadCapacityVarianceNull())
                                {
                                    orderProcessRow.LoadCapacityVariance = processRow.LoadCapacityVariance;
                                }

                                var hasCapacity = false;

                                if (!processRow.IsLoadCapacityQuantityNull())
                                {
                                    hasCapacity = true;
                                    orderProcessRow.LoadCapacityQuantity = processRow.LoadCapacityQuantity;
                                }

                                if (!processRow.IsLoadCapacityWeightNull())
                                {
                                    hasCapacity = true;
                                    orderProcessRow.LoadCapacityWeight = processRow.LoadCapacityWeight;
                                }

                                if (!hasCapacity)
                                {
                                    // Retrieve load capacity from process
                                    var loadCapacityWeight = DataManager.GetLoadCapacityWeight(processRow.ProcessID);

                                    if (loadCapacityWeight.HasValue)
                                    {
                                        orderProcessRow.LoadCapacityWeight = loadCapacityWeight.Value;
                                    }
                                    else
                                    {
                                        var loadCapacityQuantity = DataManager.GetLoadCapacityQuantity(processRow.ProcessID);

                                        if (loadCapacityQuantity.HasValue)
                                        {
                                            orderProcessRow.LoadCapacityQuantity = loadCapacityQuantity.Value;
                                        }
                                    }
                                }

                                // Process Requisites
                                var processRequisites = DataManager
                                    .GetProcessRequisites(processRow.ProcessID)
                                    .Where(req => previousProcessIds.Contains(req.ChildProcessId))
                                    .ToList();

                                if (processRequisites.Count > 0)
                                {
                                    var matchingReq = processRequisites.OrderByDescending(req => req.Hours).FirstOrDefault();
                                    orderProcessRow.RequisiteProcessID = matchingReq.ChildProcessId;
                                    orderProcessRow.RequisiteHours = matchingReq.Hours;
                                }

                                // Process Prices
                                if (SettingsProvider.PartPricingType == PricingType.Process)
                                {
                                    foreach (var processPriceRow in processRow.GetPartProcessPriceSummaryRows())
                                    {
                                        var pricePoint = PricePoint.From(processPriceRow);

                                        if (!pricePoint.Matches(orderQuantity, orderWeight, priceUnit))
                                        {
                                            continue;
                                        }

                                        orderProcessRow.Amount = Math.Max(processPriceRow.Amount, 0M);
                                        break;
                                    }
                                }

                                dsOrders.OrderProcesses.AddOrderProcessesRow(orderProcessRow);

                                stepOrder++;
                                previousProcessIds.Add(processRow.ProcessID);
                                processLeadTimes.Add(orderProcessRow.OrderProcessesID, processRow);
                            }

                            // Part mark
                            if (matchingPart.PartMarking)
                            {
                                var partMark = DataManager.FromPart(matchingPart.PartID);

                                if (partMark != null)
                                {
                                    var partMarkRow = dsOrders.OrderPartMark.NewOrderPartMarkRow();

                                    partMarkRow.OrderRow = orderRow;

                                    if (!string.IsNullOrEmpty(partMark.ProcessSpec))
                                    {
                                        partMarkRow.ProcessSpec = partMark.ProcessSpec;
                                    }

                                    if (!string.IsNullOrEmpty(partMark.Line1))
                                    {
                                        partMarkRow.Line1 = partMark.Line1;
                                    }

                                    if (!string.IsNullOrEmpty(partMark.Line2))
                                    {
                                        partMarkRow.Line2 = partMark.Line2;
                                    }

                                    if (!string.IsNullOrEmpty(partMark.Line3))
                                    {
                                        partMarkRow.Line3 = partMark.Line3;
                                    }

                                    if (!string.IsNullOrEmpty(partMark.Line4))
                                    {
                                        partMarkRow.Line4 = partMark.Line4;
                                    }

                                    dsOrders.OrderPartMark.AddOrderPartMarkRow(partMarkRow);
                                }
                            }

                            // Custom fields
                            foreach (var field in Fields)
                            {
                                // Assumption: all required fields have a value
                                if (string.IsNullOrEmpty(field.Value))
                                {
                                    continue;
                                }

                                var customFieldRow = dsOrders.OrderCustomFields.NewOrderCustomFieldsRow();
                                customFieldRow.OrderRow = orderRow;
                                customFieldRow.CustomFieldID = field.CustomFieldId;
                                customFieldRow.Value = field.Value;

                                dsOrders.OrderCustomFields.AddOrderCustomFieldsRow(customFieldRow);
                            }

                            // Estimated Ship Date - using scheduler
                            orderRow.EstShipDate = scheduler?.UpdateScheduleDates(orderRow, processLeadTimes)
                                ?? DateTime.Now.AddBusinessDays(customerRow.LeadTime);

                            // Link shipping manifest
                            var docLinkRow = dsOrders.Order_DocumentLink.NewOrder_DocumentLinkRow();
                            docLinkRow.DocumentInfoID = shippingManifestFile.DocumentInfoId;
                            docLinkRow.LinkToType = "WorkOrder";
                            docLinkRow.LinkToKey = orderRow.OrderID;
                            dsOrders.Order_DocumentLink.AddOrder_DocumentLinkRow(docLinkRow);

                            // Add order report entry
                            reportItems.Add(PersistedImportReportItem.CreateFromNew(orderRow, order.Status));
                        }
                        else
                        {
                            // Should not happen under normal use, but part
                            // might have been deleted between validation
                            // and import.
                            var errorMsg = $"Could not find existing part '{order.Part}' in DWOS.";

                            ImportDetails.Add(new ImportSummaryItem(ImportSummaryItem.ItemType.Error,
                                errorMsg));

                            order.ImportNotes = errorMsg;
                            order.Status = ShippingManifestOrder.OrderStatus.Invalid;
                            reportItems.Add(ErrorImportReportItem.CreateFrom(order));
                        }
                    }

                    DataManager.SaveOrderData(dsOrders);

                    using (var report = new ExcelReport("Shipping Manifest Import", OrderReportContents(reportItems)))
                    {
                        FilePickerService.Open(report);
                    }

                    ImportDetails.Add(new ImportSummaryItem(ImportSummaryItem.ItemType.Info,
                        $"Added {ordersToImport.Count} order{(ordersToImport.Count != 1 ? "s" : "")}."));

                    if (PrintWorkOrderTravelers)
                    {
                        foreach (var orderRow in dsOrders.Order.OrderBy(o => o.OrderID))
                        {
                            using (var traveler = new WorkOrderTravelerReport(orderRow))
                            {
                                FilePickerService.Print(traveler);
                            }
                        }

                        ImportDetails.Add(new ImportSummaryItem(ImportSummaryItem.ItemType.Info,
                            $"Printed work order traveler{(ordersToImport.Count != 1 ? "s" : "")}."));
                    }
                }
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error importing master list."));
                ImportDetails.Add(new ImportSummaryItem(ImportSummaryItem.ItemType.Error,
                    "Failed to import master list."));
            }

            ImportProgress = 100;
        }

        private PriceResult GetPriceData(OrdersDataSet.PartSummaryRow matchingPart, int quantity, decimal weight)
        {
            var priceUnit = PriceUnitPersistence.DeterminePriceUnit(_customer.Id, quantity,
                weight, PriceByType.Quantity);

            var basePrice = OrderPrice.DetermineBasePrice(matchingPart, priceUnit) ?? 0;

            // Calculate process-level price
            if (SettingsProvider.PartPricingType == PricingType.Process)
            {
                var priceUnitFromPart = priceUnit;
                var processPriceData = matchingPart
                    .GetPartProcessSummaryRows()
                    .SelectMany(proc => proc.GetPartProcessPriceSummaryRows())
                    .ToList();

                if (!processPriceData.All(p => p.IsMinValueNull() && p.IsMaxValueNull()))
                {
                    foreach (var processPrice in processPriceData)
                    {
                        var pricePoint = PricePoint.From(processPrice);

                        if (pricePoint.PriceByType != OrderPrice.GetPriceByType(priceUnit))
                        {
                            continue;
                        }

                        var matchesQty = pricePoint.PriceByType == PriceByType.Quantity && pricePoint.MatchesQuantity(quantity);

                        if (matchesQty)
                        {
                            // Ensure that the new price unit's
                            // pricing type is consistent with the
                            // value calculated by OrderPrice.
                            switch (OrderPrice.GetPriceByType(priceUnit))
                            {
                                case PriceByType.Quantity:
                                    priceUnitFromPart = OrderPrice.GetQuantityValue(OrderPrice.ParsePriceUnit(processPrice.PriceUnit));
                                    break;
                                case PriceByType.Weight:
                                    priceUnitFromPart = OrderPrice.GetWeightValue(OrderPrice.ParsePriceUnit(processPrice.PriceUnit));
                                    break;
                            }
                        }
                    }
                }

                var processPriceSum = 0M;

                foreach (var processPrice in processPriceData)
                {
                    var pricePoint = PricePoint.From(processPrice);

                    if (pricePoint.Matches(quantity, weight, priceUnitFromPart))
                    {
                        processPriceSum += processPrice.Amount;
                    }
                }

                if (processPriceSum > 0M)
                {
                    priceUnit = priceUnitFromPart;
                    basePrice = processPriceSum;
                }
            }


            return new PriceResult
            {
                PriceUnit = priceUnit,
                BasePrice = basePrice
            };
        }

        private static bool CanImport(ShippingManifestOrder p) =>
            p != null &&
            (p.Status == ShippingManifestOrder.OrderStatus.New || p.Status == ShippingManifestOrder.OrderStatus.NewWithoutExistingOrders);

        private static ICollection<ExcelReport.ReportTable> OrderReportContents(IEnumerable<IImportReportItem> reportItems)
        {
            var reportTable = new ExcelReport.ReportTable
            {
                Name = "Imported Orders",
                FormattingOptions = new ExcelReport.TableFormattingOptions
                {
                    BorderAroundCells = true
                },
                Columns = new List<ExcelReport.Column>
                {
                    new ExcelReport.Column("WO")
                    {
                        Width = 15
                    },
                    new ExcelReport.Column("Status")
                    {
                        Width = 20
                    },
                    new ExcelReport.Column("Info")
                    {
                        Width = 75
                    },
                    new ExcelReport.Column("Part")
                    {
                        Width = 40
                    },
                    new ExcelReport.Column("Quantity")
                    {
                        Width = 20
                    },
                    new ExcelReport.Column("Customer WO")
                    {
                        Width = 30
                    },
                    new ExcelReport.Column("PO")
                    {
                        Width = 30
                    }
                },
                IncludeCompanyHeader = true,
                Rows = new List<ExcelReport.Row>()
            };

            var groupedItems = reportItems
                .GroupBy(s => s.Status)
                .ToList();

            // Include invalid items first
            var invalidItems = groupedItems.FirstOrDefault(group => group.Key == ShippingManifestOrder.OrderStatus.Invalid)
                ?? Enumerable.Empty<IImportReportItem>();

            foreach (var order in invalidItems.OrderBy(o => o.OrderId))
            {
                reportTable.Rows.Add(new ExcelReport.Row
                {
                    Cells = new object[]
                    {
                        order.OrderId?.ToString() ?? "N/A",
                        FormatStatus(order.Status),
                        order.ImportNotes,
                        order.Part,
                        order.Quantity,
                        order.CustomerWorkOrder,
                        order.PurchaseOrder
                    },

                    ForegroundColor = Color.Red,
                    IsBold = true
                });
            }

            // Then, show existing orders
            var existingItems = groupedItems.FirstOrDefault(group => group.Key == ShippingManifestOrder.OrderStatus.Existing)
                ?? Enumerable.Empty<IImportReportItem>();

            foreach (var order in existingItems.OrderBy(o => o.OrderId))
            {
                reportTable.Rows.Add(new ExcelReport.Row
                {
                    Cells = new object[]
                    {
                        order.OrderId?.ToString() ?? "N/A",
                        FormatStatus(order.Status),
                        order.ImportNotes,
                        order.Part,
                        order.Quantity,
                        order.CustomerWorkOrder,
                        order.PurchaseOrder
                    },
                    ForegroundColor = Color.Gray
                });
            }

            // Next, show new orders with parts that do not have other orders in DWOS
            var newItemsWithoutExistingOrders = groupedItems.FirstOrDefault(group => group.Key == ShippingManifestOrder.OrderStatus.NewWithoutExistingOrders)
                ?? Enumerable.Empty<IImportReportItem>();

            foreach (var order in newItemsWithoutExistingOrders.OrderBy(o => o.OrderId))
            {
                reportTable.Rows.Add(new ExcelReport.Row
                {
                    Cells = new object[]
                    {
                        order.OrderId?.ToString() ?? "N/A",
                        FormatStatus(order.Status),
                        new ExcelReport.Cell("New Part") { BackgroundColor = Color.IndianRed, IsBold = true },
                        order.Part,
                        order.Quantity,
                        order.CustomerWorkOrder,
                        order.PurchaseOrder
                    }
                });
            }

            // Finally, show every other new order
            var newItems = groupedItems.FirstOrDefault(group => group.Key == ShippingManifestOrder.OrderStatus.New)
                ?? Enumerable.Empty<IImportReportItem>();

            foreach (var order in newItems.OrderBy(o => o.OrderId))
            {
                reportTable.Rows.Add(new ExcelReport.Row
                {
                    Cells = new object[]
                    {
                        order.OrderId?.ToString() ?? "N/A",
                        FormatStatus(order.Status),
                        string.Empty,
                        order.Part,
                        order.Quantity,
                        order.CustomerWorkOrder,
                        order.PurchaseOrder
                    }
                });
            }

            return new List<ExcelReport.ReportTable>
            {
                reportTable
            };
        }

        private static string FormatStatus(ShippingManifestOrder.OrderStatus status) =>
            status == ShippingManifestOrder.OrderStatus.NewWithoutExistingOrders ? "New" : status.ToString();

        private string GetNewWorkOrderStatus()
        {
            var systemUsesOrderReview = SettingsProvider.OrderReviewEnabled;
            var customerUsesOrderReview = CustomerManager.UsesOrderReview(_customer);

            // Typically, the user must require order reviews before using
            // order review.This check should be skipped (for now) because
            // we're automatically adding multiple work orders.
            return systemUsesOrderReview && customerUsesOrderReview
                ? SettingsProvider.WorkStatusPendingOrderReview
                : SettingsProvider.WorkStatusChangingDepartment;
        }

        private DocumentFile SaveShippingManifest(DateTime manifestDate)
        {
            const string rootDirName = "AWOT";
            const string shippingManifestDirName= "Shipping Manifests";

            var mediaType = Path.GetExtension(_shippingFileName);

            var rootDirectory = DocumentManager.GetFolder(rootDirName) ??
                                DocumentManager.CreateFolder(rootDirName);

            var shippingManifestDirectory = DocumentManager.GetFolder(rootDirectory, shippingManifestDirName) ??
                                            DocumentManager.CreateFolder(rootDirectory, shippingManifestDirName);

            var customerDirectory = DocumentManager.GetFolder(shippingManifestDirectory, _customer.Name) ??
                                    DocumentManager.CreateFolder(shippingManifestDirectory, _customer.Name);


            // Ensure that new document has a unique name.
            var fileName = $"{manifestDate:yyyy-MM-dd}{mediaType}";

            var fileNumber = 1;
            while (DocumentManager.GetFile(customerDirectory, fileName, mediaType) != null)
            {
                fileName = $"{manifestDate:yyyy-MM-dd}_{fileNumber}{mediaType}";
                fileNumber++;
            }

            var shippingManifestFile = DocumentManager.CreateFile(customerDirectory, fileName, mediaType);
            DocumentManager.Revise(shippingManifestFile, _shippingFileName, UserManager.CurrentUser);

            return shippingManifestFile;
        }

        private bool CanContinue()
        {
            try
            {
                switch (_currentStatus)
                {
                    case DialogStatus.Setup:
                        return Fields.All(f => f.IsValid) && !string.IsNullOrEmpty(_shippingFileName);
                    case DialogStatus.Confirmation:
                        return !_hasError && Orders.Count(CanImport) > 0;
                    case DialogStatus.Import:
                        return _importProgress == 100;
                }
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error checking valid state of wizard for master list."));
            }

            return false;
        }

        private bool CanGoBack() => ImportProgress == 0;

#endregion

#region CustomField

        public class CustomField : ViewModelBase
        {
#region Fields

            private string _fieldValue;

#endregion

#region Properties

            public int CustomFieldId { get; set; } // Set from row

            public string Value // Set default from row
            {
                get => _fieldValue;
                set
                {
                    if (Set(nameof(Value), ref _fieldValue, value))
                    {
                        RaisePropertyChanged(nameof(IsValid));
                    }
                }
            }

            public string Name { get; private set; }

            public string Description { get; private set; }

            public bool IsRequired { get; private set; }

            public bool IsValid => !(IsRequired && string.IsNullOrEmpty(_fieldValue));

#endregion

            public static CustomField From(Model.CustomField customField)
            {
                if (customField == null)
                {
                    return null;
                }

                return new CustomField
                {
                    CustomFieldId = customField.CustomFieldId,
                    Name = customField.Name,
                    Description = customField.Description,
                    IsRequired = customField.IsRequired,
                    Value = customField.DefaultValue
                };
            }
        }

#endregion

#region DialogStatus

        public enum DialogStatus
        {
            Setup,
            Confirmation,
            Import,
            Results
        }

#endregion

#region ShippingManifestImporter

        private class ShippingManifestImporter
        {
#region Fields

            private static string TEXT_PRIORITY = "pri ority";
            private static string TEXT_SHIPPER = "kac shipper";
            private static string TEXT_PO = "po nbr";
            private static string TEXT_PO_ITEM = "po item nbr";
            private static string TEXT_WO = "work order";
            private static string TEXT_PART = "part number";
            private static string TEXT_PROJECT = "proj ect";
            private static string TEXT_DUE = "due date";
            private static string TEXT_QTY = "ship qty";
            private static string TEXT_LOT = "lot cost";
            private static string TEXT_INVOICE = "invoice nbr";
            private static string TEXT_PACKING_SLIP = "vendors packslip";
            private static string TEXT_APPROVAL = "purchasing invoice approval";
            private static string TEXT_VENDOR_NUMBER = "vendor nbr";
            private static string TEXT_SOURCE_CODE = "srccod";

#endregion

#region Properties

            public string FileName { get; }

            public CustomerViewModel Customer { get; }

            public IDocumentManager DocumentManager { get; }

#endregion

#region Methods

            public ShippingManifestImporter(string fileName, CustomerViewModel customer, IDocumentManager documentManager)
            {
                FileName = fileName;
                Customer = customer ?? throw new ArgumentNullException(nameof(customer));
                DocumentManager = documentManager ?? throw new ArgumentNullException(nameof(documentManager));
            }

            public ValidationResult ValidateFile() =>
                CheckFileExists()
                ?? CheckFileFormat()
                ?? CheckPreviousShippingManifests()
                ?? ValidationResult.Success();

            public Task<List<ShippingManifestOrder>> GetOrdersFromWorksheet()
            {
                return Task.Factory.StartNew(
                    () =>
                    {
                        // Assumption: User calls ValidateFile() first
                        try
                        {
                            var orders = new List<ShippingManifestOrder>();

                            var workbook = Workbook.Load(FileName);
                            var worksheet = workbook.Worksheets[0];

                            // Determine column -> value mapping
                            var nameToIndexMap = GetNameToIndexMap(worksheet.Rows[4].Cells);

                            // Import each row
                            foreach (var row in worksheet.Rows.Skip(5))
                            {
                                if (row.Cells[0].GetText().ToLowerInvariant().StartsWith("total rows"))
                                {
                                    // End of orders in worksheet
                                    break;
                                }

                                var order = new ShippingManifestOrder();
                                if (nameToIndexMap.TryGetValue(TEXT_PRIORITY, out var priorityIndex))
                                {
                                    order.Priority = row.Cells[priorityIndex].GetText();
                                }

                                if (nameToIndexMap.TryGetValue(TEXT_SHIPPER, out var shipperIndex))
                                {
                                    order.KacShipper = row.Cells[shipperIndex].GetText();
                                }

                                if (nameToIndexMap.TryGetValue(TEXT_PO, out var purchaseOrderIndex))
                                {
                                    order.PurchaseOrder = row.Cells[purchaseOrderIndex].GetText();
                                }

                                if (nameToIndexMap.TryGetValue(TEXT_PO_ITEM, out var orderItemIndex))
                                {
                                    order.PurchaseOrderItem = row.Cells[orderItemIndex].GetText();
                                }

                                if (nameToIndexMap.TryGetValue(TEXT_WO, out var orderIndex))
                                {
                                    order.WorkOrder = row.Cells[orderIndex].GetText();
                                }

                                if (nameToIndexMap.TryGetValue(TEXT_PART, out var partIndex))
                                {
                                    order.Part = row.Cells[partIndex].GetText()?.Trim().ToUpper();
                                }

                                if (nameToIndexMap.TryGetValue(TEXT_PROJECT, out var projectIndex))
                                {
                                    order.Project = row.Cells[projectIndex].GetText();
                                }

                                if (nameToIndexMap.TryGetValue(TEXT_DUE, out var dueIndex))
                                {
                                    if (DateTime.TryParse(row.Cells[dueIndex].GetText(), out var dueDate))
                                    {
                                        order.DueDate = dueDate;
                                    }
                                }

                                if (nameToIndexMap.TryGetValue(TEXT_QTY, out var qtyIndex))
                                {
                                    if (int.TryParse(row.Cells[qtyIndex].GetText(), out var qty))
                                    {
                                        order.Quantity = qty;
                                    }
                                }

                                if (nameToIndexMap.TryGetValue(TEXT_LOT, out var lotIndex))
                                {
                                    order.LotCost = row.Cells[lotIndex].GetText();
                                }

                                if (nameToIndexMap.TryGetValue(TEXT_INVOICE, out var invoiceIndex))
                                {
                                    order.InvoiceNumber = row.Cells[invoiceIndex].GetText();
                                }

                                if (nameToIndexMap.TryGetValue(TEXT_PACKING_SLIP, out var packingSlipIndex))
                                {
                                    order.VendorPackslip = row.Cells[packingSlipIndex].GetText();
                                }

                                if (nameToIndexMap.TryGetValue(TEXT_APPROVAL, out var approvalIndex))
                                {
                                    order.PurchasingInvoiceApproval = row.Cells[approvalIndex].GetText();
                                }

                                if (nameToIndexMap.TryGetValue(TEXT_VENDOR_NUMBER, out var vendorIndex))
                                {
                                    order.VendorNumber = row.Cells[vendorIndex].GetText();
                                }

                                if (nameToIndexMap.TryGetValue(TEXT_SOURCE_CODE, out var srcIndex))
                                {
                                    order.SourceCode = row.Cells[srcIndex].GetText();
                                }

                                orders.Add(order);
                            }

                            return orders;
                        }
                        catch (Exception exc)
                        {
                            LogManager.GetCurrentClassLogger().Error(exc, "Error getting data from master list file.");
                            return new List<ShippingManifestOrder>();
                        }
                    });
            }

            private ValidationResult CheckFileExists()
            {
                if (File.Exists(FileName))
                {
                    return null;
                }

                return ValidationResult.Failure("File does not exist.");
            }

            private ValidationResult CheckFileFormat()
            {
                try
                {
                    var workbook = Workbook.Load(FileName);
                    var worksheet = workbook.Worksheets[0];

                    if (worksheet.Rows.Count() < 5)
                    {
                        return ValidationResult.Failure("Invalid format");
                    }

                    var header = worksheet.Rows[4];
                    var requiredFields = new List<string>
                    {
                        TEXT_PRIORITY,
                        TEXT_SHIPPER,
                        TEXT_PO,
                        TEXT_PO_ITEM,
                        TEXT_WO,
                        TEXT_PART,
                        TEXT_PROJECT,
                        TEXT_DUE,
                        TEXT_QTY,
                        TEXT_LOT,
                        TEXT_INVOICE,
                        TEXT_PACKING_SLIP,
                        TEXT_APPROVAL,
                        TEXT_VENDOR_NUMBER,
                        TEXT_SOURCE_CODE
                    };

                    var spreadsheetFields = header.Cells.Select(c => c.GetText().ToLowerInvariant().Trim()).ToList();

                    var missingFields = requiredFields.Except(spreadsheetFields);

                    return !missingFields.Any() ? null : ValidationResult.Failure("Invalid format");
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Unable to read master list.");
                    return ValidationResult.Failure("Could not read file.");
                }

            }

            private ValidationResult CheckPreviousShippingManifests()
            {
                try
                {
                    var rootDir = DocumentManager.GetFolder("AWOT");

                    if (rootDir == null)
                    {
                        // No shipping manifests have been added.
                        return null;
                    }

                    var shippingManifestDir = DocumentManager.GetFolder(rootDir, "Shipping Manifests");
                    if (shippingManifestDir == null)
                    {
                        // No shipping manifests have been added.
                        return null;
                    }

                    var customerDir = DocumentManager.GetFolder(shippingManifestDir, Customer.Name);
                    if (customerDir == null)
                    {
                        // No shipping manifests for this customer
                        return null;
                    }

                    foreach (var document in DocumentManager.GetFiles(customerDir))
                    {
                        if (DocumentManager.MatchesAnyRevision(document, FileName))
                        {
                            return ValidationResult.Failure("This shipping manifest was previously imported");
                        }
                    }

                    return null;
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Unable to read master list.");
                    return ValidationResult.Failure("Could not check previous revisions");
                }
            }

            private Dictionary<string, int> GetNameToIndexMap(WorksheetCellCollection cells)
            {
                var fields = new List<string>
                {
                    TEXT_PRIORITY,
                    TEXT_SHIPPER,
                    TEXT_PO,
                    TEXT_PO_ITEM,
                    TEXT_WO,
                    TEXT_PART,
                    TEXT_PROJECT,
                    TEXT_DUE,
                    TEXT_QTY,
                    TEXT_LOT,
                    TEXT_INVOICE,
                    TEXT_PACKING_SLIP,
                    TEXT_APPROVAL,
                    TEXT_VENDOR_NUMBER,
                    TEXT_SOURCE_CODE
                };

                var nameToIndexMap = new Dictionary<string, int>();

                foreach (var cell in cells)
                {
                    var headerText = cell.GetText()?.Trim().ToLowerInvariant();

                    if (string.IsNullOrEmpty(headerText))
                    {
                        continue;
                    }

                    var matchingField = fields.FirstOrDefault(f => f == headerText);

                    if (matchingField != null)
                    {
                        fields.Remove(matchingField);
                        nameToIndexMap.Add(matchingField, cell.ColumnIndex);
                    }
                }

                return nameToIndexMap;
            }
#endregion

            public DateTime? GetOrderDateFromWorksheet()
            {
                try
                {
                    var workbook = Workbook.Load(FileName);
                    var worksheet = workbook.Worksheets[0];

                    var dateRow = worksheet.Rows[3];
                    var dateCell = dateRow.Cells[10];

                    var dateText = dateCell.GetText();
                    var dateRegex = new Regex(@"^Date:
                                  \s*(?<month>\d+)\s*[-/]
                                  \s*(?<day>\d+)\s*[-/]
                                  \s*(?<year>\d+)\s*$",
                    RegexOptions.IgnorePatternWhitespace);

                    var dateMatch = dateRegex.Match(dateText);

                    if (dateMatch.Success)
                    {
                        var year = int.Parse(dateMatch.Groups["year"].Value);
                        var month = int.Parse(dateMatch.Groups["month"].Value);
                        var day = int.Parse(dateMatch.Groups["day"].Value);

                        return new DateTime(
                            CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(year),
                            month,
                            day);
                    }

                    LogManager.GetCurrentClassLogger().Warn($"Date not found: {dateText}");
                    return null;
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error getting data from master list file.");
                    return null;
                }
            }
        }

        #endregion

        #region IImportReportItem

        private interface IImportReportItem
        {
            int? OrderId { get; }

            string Part { get; }

            int Quantity { get; }

            string CustomerWorkOrder { get; }

            string PurchaseOrder { get; }

            ShippingManifestOrder.OrderStatus Status { get; }

            string ImportNotes { get; }
        }

        #endregion

        #region ImportReportItem

        private class ErrorImportReportItem : IImportReportItem
        {
            #region Methods

            public static ErrorImportReportItem CreateFrom(ShippingManifestOrder order)
            {
                if (order == null)
                {
                    return null;
                }

                return new ErrorImportReportItem
                {
                    OrderId = order.ExistingOrderId,
                    ImportNotes = order.ImportNotes,
                    CustomerWorkOrder = order.WorkOrder,
                    Part = order.Part,
                    PurchaseOrder = order.PurchaseOrder,
                    Quantity = order.Quantity ?? 0,
                    Status = order.Status
                };
            }

            #endregion

            #region IImportReportItem Members

            public int? OrderId { get; private set; }

            public string Part { get; private set; }

            public int Quantity { get; private set; }

            public string CustomerWorkOrder { get; private set; }

            public string PurchaseOrder { get; private set; }

            public ShippingManifestOrder.OrderStatus Status { get; private set; }

            public string ImportNotes { get; private set; }

            #endregion


        }

        private class PersistedImportReportItem : IImportReportItem
        {
            #region Properties

            public OrdersDataSet.OrderRow Order { get; }

            #endregion

            #region Methods

            private PersistedImportReportItem(OrdersDataSet.OrderRow order, ShippingManifestOrder.OrderStatus status)
            {
                Order = order ?? throw new ArgumentNullException(nameof(order));
                Status = status;
            }

            public static PersistedImportReportItem CreateFromNew(OrdersDataSet.OrderRow orderRow, ShippingManifestOrder.OrderStatus status)
            {
                if (orderRow == null)
                {
                    return null;
                }

                return new PersistedImportReportItem(orderRow, status);
            }

            #endregion

            #region IImportReportItem Members

            public int? OrderId => Order.OrderID;

            public string Part => Order.PartSummaryRow?.Name ?? (Order.IsPartIDNull() ? -1 : Order.PartID).ToString();

            public int Quantity => Order.IsPartQuantityNull() ? 0 : Order.PartQuantity;

            public string CustomerWorkOrder => Order.IsCustomerWONull() ? string.Empty : Order.CustomerWO;

            public string PurchaseOrder => Order.IsPurchaseOrderNull() ? string.Empty : Order.PurchaseOrder;

            public ShippingManifestOrder.OrderStatus Status { get; }

            public string ImportNotes => string.Empty;

            #endregion
        }

        #endregion

        #region PriceResult

        private class PriceResult
        {
            public OrderPrice.enumPriceUnit PriceUnit { get; set; }

            public decimal BasePrice { get; set; }

            public void Deconstruct(out OrderPrice.enumPriceUnit priceUnit, out decimal basePrice)
            {
                priceUnit = PriceUnit;
                basePrice = BasePrice;
            }
        }

        #endregion
    }
}
