using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Date;
using DWOS.UI.Sales.ViewModels;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DWOS.UI.Sales.Models
{
    internal class SalesOrderWizardPersistence
    {
        #region Fields

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public IDwosApplicationSettingsProvider SettingsProvider { get; }

        public Utilities.ISecurityManager SecurityManager { get; }

        public IDateTimeNowProvider CurrentTimeProvider { get; }

        #endregion

        #region Methods

        public SalesOrderWizardPersistence(
            IDwosApplicationSettingsProvider appSettingsProvider,
            Utilities.ISecurityManager securityManager,
            IDateTimeNowProvider currentTimeProvider)
        {
            SettingsProvider = appSettingsProvider
                ?? throw new ArgumentNullException(nameof(appSettingsProvider));

            SecurityManager = securityManager
                ?? throw new ArgumentNullException(nameof(securityManager));

            CurrentTimeProvider = currentTimeProvider
                ?? throw new ArgumentNullException(nameof(currentTimeProvider));
        }

        public List<CustomerSummary> RetrieveCustomers()
        {
            // Load lists for custom fields
            Dictionary<int, List<string>> listValuesDict;
            using (var dtListValues = new OrdersDataSet.ListValuesDataTable())
            {
                using (var taListValues = new ListValuesTableAdapter())
                {
                    taListValues.Fill(dtListValues);
                }

                listValuesDict = dtListValues.GroupBy(lv => lv.ListID)
                    .ToDictionary(
                        lvGroup => lvGroup.Key,
                        lvGroup => lvGroup.Select(lv => lv.Value).OrderBy(val => val).ToList());
            }

            // Load customers
            var customers = new List<CustomerSummary>();

            using (var dtCustomer = new OrdersDataSet.CustomerSummaryDataTable())
            {
                using (var taCustomer = new CustomerSummaryTableAdapter())
                {
                    taCustomer.FillActive(dtCustomer);
                }

                using (var taCustomerFields = new Data.Datasets.CustomersDatasetTableAdapters.Customer_FieldsTableAdapter { ClearBeforeFill = false })
                using (var taCustomFields = new CustomFieldTableAdapter())
                using (var taParts = new PartSummaryTableAdapter())
                using (var taCustomerShipping = new CustomerShippingSummaryTableAdapter())
                using (var taCustomerAddress = new CustomerAddressTableAdapter())
                using (var taCustomerDefaultPrice = new CustomerDefaultPriceTableAdapter())
                {
                    foreach (var customerRow in dtCustomer)
                    {
                        // Load custom fields
                        var fields = new List<CustomField>();
                        using (var dtCustomFields = new OrdersDataSet.CustomFieldDataTable())
                        {
                            taCustomFields.FillByCustomer(dtCustomFields, customerRow.CustomerID);

                            foreach (var customFieldRow in dtCustomFields)
                            {
                                if (!customFieldRow.IsVisible)
                                {
                                    continue;
                                }

                                if (customFieldRow.IsListIDNull())
                                {

                                    fields.Add(CustomField.NewTextField(customFieldRow.CustomFieldID,
                                        customFieldRow.Name,
                                        customFieldRow.Required,
                                        customFieldRow.IsDescriptionNull() ? null : customFieldRow.Description,
                                        customFieldRow.IsDefaultValueNull() ? null : customFieldRow.DefaultValue));
                                }
                                else
                                {
                                    listValuesDict.TryGetValue(customFieldRow.ListID, out var valuesForList);

                                    fields.Add(CustomField.NewComboBoxField(customFieldRow.CustomFieldID,
                                        customFieldRow.Name,
                                        customFieldRow.Required,
                                        customFieldRow.IsDescriptionNull() ? null : customFieldRow.Description,
                                        customFieldRow.IsDefaultValueNull() ? null : customFieldRow.DefaultValue,
                                        valuesForList));
                                }
                            }
                        }

                        // Load parts
                        var parts = new List<Part>();

                        using (var dtParts = new OrdersDataSet.PartSummaryDataTable())
                        {
                            taParts.FillByCustomerActive(dtParts, customerRow.CustomerID);

                            foreach (var partRow in dtParts.OrderBy(part => part.Name))
                            {
                                var partRowWeight = partRow.IsWeightNull()
                                    ? (decimal?)null
                                    : partRow.Weight;

                                parts.Add(new Part(partRow.PartID, partRow.Name, partRowWeight, partRow.PartMarking));
                            }
                        }

                        // Load shipping methods
                        var shippingMethods = new List<CustomerShipping>();
                        using (var dtCustomerShipping = new OrdersDataSet.CustomerShippingSummaryDataTable())
                        {
                            taCustomerShipping.FillByCustomer(dtCustomerShipping, customerRow.CustomerID);

                            var customerShippingMethods = dtCustomerShipping
                                .Where(ship => ship.Active)
                                .OrderBy(ship => ship.Name);

                            foreach (var shippingRow in customerShippingMethods)
                            {
                                var shippingId = shippingRow.CustomerShippingID;
                                var shippingName = shippingRow.IsNameNull() ? null : shippingRow.Name;
                                var shippingDefault = shippingRow.DefaultShippingMethod;
                                shippingMethods.Add(new CustomerShipping(shippingId, shippingName, shippingDefault));
                            }

                        }

                        // Load shipping addresses
                        var addresses = new List<CustomerAddress>();
                        using (var dtCustomerAddress = new OrdersDataSet.CustomerAddressDataTable())
                        {
                            taCustomerAddress.FillByCustomer(dtCustomerAddress, customerRow.CustomerID);

                            var customerAddresses = dtCustomerAddress
                                .Where(address => address.Active)
                                .OrderBy(address => address.Name);

                            foreach (var addressRow in customerAddresses)
                            {
                                addresses.Add(new CustomerAddress(
                                    addressRow.CustomerAddressID,
                                    addressRow.Name,
                                    addressRow.CountryID,
                                    addressRow.IsDefault));
                            }
                        }

                        // Load default lot price
                        decimal? defaultPriceLot;
                        using (var dtCustomerDefaultPrice = new OrdersDataSet.CustomerDefaultPriceDataTable())
                        {
                            taCustomerDefaultPrice.FillByCustomer(dtCustomerDefaultPrice, customerRow.CustomerID);
                            defaultPriceLot = dtCustomerDefaultPrice
                                .FirstOrDefault(p => p.PriceUnit == nameof(OrderPrice.enumPriceUnit.Lot))
                                ?.DefaultPrice;
                        }

                        // Load customer preferences for system fields
                        var purchaseOrderField = taCustomerFields
                            .GetField(customerRow.CustomerID, "PO")
                            .FirstOrDefault();

                        var customerWorkOrderField = taCustomerFields
                            .GetField(customerRow.CustomerID, "Customer WO")
                            .FirstOrDefault();

                        var documentsField = taCustomerFields
                            .GetField(customerRow.CustomerID, "Documents")
                            .FirstOrDefault();

                        // Add customer
                        customers.Add(new CustomerSummary(customerRow.CustomerID, customerRow.Name, customerRow.LeadTime, fields, parts, shippingMethods, addresses)
                        {
                            DefaultPurchaseOrder = purchaseOrderField == null || purchaseOrderField.IsDefaultVaueNull()
                                ? null
                                : purchaseOrderField.DefaultVaue,

                            DefaultCustomerWorkOrder = customerWorkOrderField == null || customerWorkOrderField.IsDefaultVaueNull()
                                ? null
                                : customerWorkOrderField.DefaultVaue,

                            RequiresPurchaseOrder = purchaseOrderField != null && purchaseOrderField.Required,
                            RequiresCustomerWorkOrder = customerWorkOrderField != null && customerWorkOrderField.Required,
                            RequiresDocument = documentsField != null && documentsField.Required,

                            DefaultPriceLot = defaultPriceLot
                        });
                    }
                }
            }

            return customers;
        }

        public List<string> RetrievePriorities()
        {
            var priorities = new List<string>();
            using (var dtPriorities = new OrdersDataSet.d_PriorityDataTable())
            {
                using (var taPriorities = new d_PriorityTableAdapter())
                {
                    taPriorities.Fill(dtPriorities);
                }

                foreach (var priorityRow in dtPriorities)
                {
                    priorities.Add(priorityRow.PriorityID);
                }
            }

            return priorities;
        }

        public List<OrderFeeType> RetrieveOrderFeeTypes()
        {
            var defaultFeeIds = new HashSet<string>();
            var defaultFeesString = SettingsProvider.Settings.DefaultFees;

            if (!string.IsNullOrEmpty(defaultFeesString))
            {
                defaultFeeIds = new HashSet<string>(defaultFeesString.Split(new[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries));
            }

            var feeTypes = new List<OrderFeeType>();

            using (var dtOrderFeeType = new OrdersDataSet.OrderFeeTypeDataTable())
            {
                using (var taOrderFeeType = new OrderFeeTypeTableAdapter())
                {
                    taOrderFeeType.Fill(dtOrderFeeType);
                }

                foreach (var orderFeeRow in dtOrderFeeType)
                {
                    Enum.TryParse<OrderPrice.enumFeeType>(orderFeeRow.FeeType, out var feeType);
                    feeTypes.Add(new OrderFeeType(orderFeeRow.OrderFeeTypeID,
                        feeType,
                        orderFeeRow.Price,
                        defaultFeeIds.Contains(orderFeeRow.OrderFeeTypeID)));
                }
            }

            return feeTypes;
        }

        public List<CustomerFee> RetrieveDefaultFees(CustomerSummary customer)
        {
            if (customer == null)
            {
                return new List<CustomerFee>();
            }

            var customerFees = new List<CustomerFee>();
            using (var taCustomerFee = new CustomerFeeTableAdapter())
            {
                using (var dtCustomerFee = taCustomerFee.GetDataByCustomer(customer.CustomerId))
                {
                    foreach (var customerFeeRow in dtCustomerFee)
                    {
                        var fee = new CustomerFee(
                            customerFeeRow.CustomerFeeID,
                            customerFeeRow.OrderFeeTypeID,
                            customerFeeRow.Charge);

                        customerFees.Add(fee);
                    }
                }
            }

            return customerFees;
        }

        private OrdersDataSet CreateSaveDataSet(int customerId)
        {
            var dsOrders = new OrdersDataSet();

            try
            {
                using (new UsingDataSetLoad(dsOrders))
                {
                    using (var taCustomers = new CustomerSummaryTableAdapter())
                    {
                        taCustomers.FillByCustomerID(dsOrders.CustomerSummary, customerId);
                    }

                    using (var taCustomerShipping = new CustomerShippingSummaryTableAdapter())
                    {
                        taCustomerShipping.FillByCustomer(dsOrders.CustomerShippingSummary, customerId);
                    }

                    using (var taCustomerAddress = new CustomerAddressTableAdapter())
                    {
                        taCustomerAddress.FillByCustomer(dsOrders.CustomerAddress, customerId);
                    }

                    using (var taPriorities = new d_PriorityTableAdapter())
                    {
                        taPriorities.Fill(dsOrders.d_Priority);
                    }

                    using (var taStatus = new d_OrderStatusTableAdapter())
                    {
                        taStatus.Fill(dsOrders.d_OrderStatus);
                    }

                    using (var taUsers = new UserSummaryTableAdapter())
                    {
                        taUsers.Fill(dsOrders.UserSummary);
                    }

                    using (var taFeeType = new d_FeeTypeTableAdapter())
                    {
                        taFeeType.Fill(dsOrders.d_FeeType);
                    }

                    using (var taOrderFeeType = new OrderFeeTypeTableAdapter())
                    {
                        taOrderFeeType.Fill(dsOrders.OrderFeeType);
                    }
                }
            }
            catch (Exception)
            {
                LogManager.GetCurrentClassLogger().Warn("DataSet Errors: " + dsOrders.GetDataErrors());
                throw;
            }

            return dsOrders;
        }

        private TableAdapterManager CreateSaveManager()
        {
            return new TableAdapterManager
            {
                SalesOrderTableAdapter = new SalesOrderTableAdapter(),
                MediaTableAdapter = new MediaTableAdapter(),
                SalesOrder_MediaTableAdapter = new SalesOrder_MediaTableAdapter(),
                SalesOrder_DocumentLinkTableAdapter = new SalesOrder_DocumentLinkTableAdapter(),
                OrderTableAdapter = new OrderTableAdapter(),
                OrderSerialNumberTableAdapter = new OrderSerialNumberTableAdapter(),
                OrderProcessesTableAdapter = new OrderProcessesTableAdapter(),
                OrderCustomFieldsTableAdapter = new OrderCustomFieldsTableAdapter(),
                OrderFeesTableAdapter = new OrderFeesTableAdapter(),
                OrderPartMarkTableAdapter = new OrderPartMarkTableAdapter(),
                BatchTableAdapter = new BatchTableAdapter(),
                BatchOrderTableAdapter = new BatchOrderTableAdapter(),
                BatchProcessesTableAdapter = new BatchProcessesTableAdapter(),
                BatchProcess_OrderProcessTableAdapter = new BatchProcess_OrderProcessTableAdapter()
            };
        }

        public int SaveSalesOrder(SalesOrderWizardViewModel viewModel)
        {
            // Create all unsaved parts
            foreach (var part in viewModel.WorkOrders.Select(wo => wo.SelectedPart).Where(part => part.IsNew))
            {
                part.PartId = SavePart(part, viewModel.SelectedCustomer.CustomerId);
            }

            // Determine price per Work Order
            var orderBasePrice = viewModel.BasePriceLot / viewModel.WorkOrders.Count;
            var settings = SettingsProvider.Settings;

            // Save data
            int salesOrderId;

            using (var dsOrders = CreateSaveDataSet(viewModel.SelectedCustomer.CustomerId))
            {
                using (var taManager = CreateSaveManager())
                {
                    // Save Sales Order
                    var salesOrderRow = dsOrders.SalesOrder.NewSalesOrderRow();
                    salesOrderRow.Status = Properties.Settings.Default.OrderStatusOpen;
                    salesOrderRow.OrderDate = viewModel.OrderDate ?? DateTime.MinValue;
                    salesOrderRow.CreatedBy = SecurityManager.UserInfo.UserID;
                    salesOrderRow.CustomerID = viewModel.SelectedCustomer.CustomerId;
                    salesOrderRow.PurchaseOrder = viewModel.PurchaseOrderNumber;
                    salesOrderRow.CustomerWO = viewModel.CustomerWorkOrder;

                    if (viewModel.RequiredDate.HasValue)
                    {
                        salesOrderRow.RequiredDate = viewModel.RequiredDate.Value;
                    }

                    salesOrderRow.EstShipDate = viewModel.EstShipDate ?? DateTime.MinValue;
                    dsOrders.SalesOrder.AddSalesOrderRow(salesOrderRow);

                    // Save Work Orders
                    var isFirstWorkOrder = true;
                    foreach (var workOrder in viewModel.WorkOrders.OrderBy(wo => wo.Order))
                    {
                        // Skip Work Order if part wasn't previously saved.
                        if (workOrder.SelectedPart.IsNew)
                        {
                            LogManager.GetCurrentClassLogger()
                                .Error("Attempted to save Work Order with new Part");

                            continue;
                        }

                        // Work Order
                        var orderRow = dsOrders.Order.NewOrderRow();
                        orderRow.SalesOrderRow = salesOrderRow;
                        orderRow.OrderDate = viewModel.OrderDate ?? DateTime.MinValue;
                        orderRow.Status = Properties.Settings.Default.OrderStatusOpen;
                        orderRow.Priority = viewModel.Priority;
                        orderRow.CreatedBy = SecurityManager.UserInfo.UserID;
                        orderRow.ContractReviewed = viewModel.IsAccepted;
                        orderRow.PartQuantity = workOrder.Quantity;

                        if (workOrder.Weight.HasValue)
                        {
                            const decimal maxWeight = 999999.99999999M;
                            var weightDecimalPlaces = SettingsProvider.Settings.WeightDecimalPlaces;
                            var decimalPlacesOffset = (decimal)Math.Pow(10, weightDecimalPlaces);
                            var orderWeight = workOrder.Weight.Value;

                            // Truncate the weight to  the specified number of
                            // decimal places. If the weight is larger than the
                            // maximum allowed, use the maximum allowed value.
                            orderRow.Weight = Math.Truncate(Math.Min(orderWeight, maxWeight) * decimalPlacesOffset)
                                / decimalPlacesOffset;
                        }

                        orderRow.BasePrice = orderBasePrice;
                        orderRow.PriceUnit = nameof(OrderPrice.enumPriceUnit.Lot);
                        orderRow.CustomerID = viewModel.SelectedCustomer.CustomerId;
                        orderRow.PartID = workOrder.SelectedPart.PartId;
                        orderRow.Hold = false;
                        orderRow.OrderType = (int)OrderType.Normal;
                        orderRow.OriginalOrderType = (int)OrderType.Normal;

                        var shippingCountryId = viewModel.SelectedShippingAddress != null
                            ? viewModel.SelectedShippingAddress.CountryId
                            : settings.CompanyCountry;

                        if (settings.ImportExportApprovalEnabled && shippingCountryId != settings.CompanyCountry)
                        {
                            orderRow.WorkStatus = settings.WorkStatusPendingImportExportReview;
                        }
                        else
                        {
                            orderRow.WorkStatus = OrderControllerExtensions.GetNewOrderWorkStatus(
                                viewModel.SelectedCustomer.CustomerId,
                                SecurityManager.RequiresOrderReview);
                        }

                        orderRow.CurrentLocation = settings.DepartmentSales;
                        orderRow.PurchaseOrder = viewModel.PurchaseOrderNumber;
                        orderRow.CustomerWO = workOrder.CustomerWorkOrder;
                        orderRow.EstShipDate = viewModel.EstShipDate ?? DateTime.MinValue;

                        if (viewModel.RequiredDate.HasValue)
                        {
                            orderRow.RequiredDate = viewModel.RequiredDate.Value;
                        }

                        if (viewModel.SelectedShippingMethod != null)
                        {
                            orderRow.ShippingMethod = viewModel.SelectedShippingMethod.CustomerShippingId;
                        }

                        if (viewModel.SelectedShippingAddress != null)
                        {
                            orderRow.CustomerAddressID = viewModel.SelectedShippingAddress.CustomerAddressId;
                        }

                        dsOrders.Order.AddOrderRow(orderRow);

                        // Fees
                        if (isFirstWorkOrder)
                        {
                            // Apply fixed fees to first work order
                            var fixedFees = viewModel.OrderFees
                                .Where(fee => fee.FeeType == OrderPrice.enumFeeType.Fixed);

                            foreach (var fee in fixedFees)
                            {
                                var row = dsOrders.OrderFees.NewOrderFeesRow();
                                row.OrderFeeTypeID = fee.OrderFeeTypeId;
                                row.Charge = fee.Charge;
                                row.OrderID = orderRow.OrderID;
                                dsOrders.OrderFees.AddOrderFeesRow(row);
                            }
                        }

                        // Apply percentage fees
                        var percentFees = viewModel.OrderFees
                            .Where(fee => fee.FeeType == OrderPrice.enumFeeType.Percentage);

                        foreach (var fee in percentFees)
                        {
                            var row = dsOrders.OrderFees.NewOrderFeesRow();
                            row.OrderFeeTypeID = fee.OrderFeeTypeId;
                            row.Charge = fee.Charge;
                            row.OrderID = orderRow.OrderID;
                            dsOrders.OrderFees.AddOrderFeesRow(row);
                        }

                        // Processes
                        var previousProcessIds = new HashSet<int>();
                        foreach (var orderProcess in workOrder.Processes)
                        {
                            var orderProcessesRow = dsOrders.OrderProcesses.NewOrderProcessesRow();
                            orderProcessesRow.OrderID = orderRow.OrderID;
                            orderProcessesRow.ProcessID = orderProcess.ProcessId;
                            orderProcessesRow.ProcessAliasID = orderProcess.ProcessAliasId;
                            orderProcessesRow.StepOrder = orderProcess.StepOrder;
                            orderProcessesRow.Department = orderProcess.Department;
                            orderProcessesRow.COCData = orderProcess.CocData;
                            orderProcessesRow.OrderProcessType = (int)OrderProcessType.Normal;

                            if (orderProcess.LoadCapacityVariance.HasValue)
                            {
                                orderProcessesRow.LoadCapacityVariance = orderProcess.LoadCapacityVariance.Value;
                            }

                            if (orderProcess.LoadCapacityWeight.HasValue)
                            {
                                orderProcessesRow.LoadCapacityWeight = orderProcess.LoadCapacityWeight.Value;
                            }

                            if (orderProcess.LoadCapacityQuantity.HasValue)
                            {
                                orderProcessesRow.LoadCapacityQuantity = orderProcess.LoadCapacityQuantity.Value;
                            }

                            if (orderProcess.EstEndDate.HasValue)
                            {
                                orderProcessesRow.EstEndDate = orderProcess.EstEndDate.Value;
                            }

                            // Requisites
                            if (previousProcessIds.Count > 0)
                            {
                                var requisites = GetProcessRequisites(orderProcess.ProcessId)
                                    .Where(req => previousProcessIds.Contains(req.ChildProcessId))
                                    .ToList();

                                if (requisites.Count > 0)
                                {
                                    var matchingRequisite = requisites
                                        .OrderByDescending(req => req.Hours)
                                        .FirstOrDefault();

                                    orderProcessesRow.RequisiteProcessID = matchingRequisite.ChildProcessId;
                                    orderProcessesRow.RequisiteHours = matchingRequisite.Hours;
                                }
                            }

                            // Add process to order
                            dsOrders.OrderProcesses.AddOrderProcessesRow(orderProcessesRow);
                            previousProcessIds.Add(orderProcess.ProcessId);
                        }

                        // Custom Fields
                        foreach (var customField in viewModel.CustomFields)
                        {
                            if (string.IsNullOrEmpty(customField.Value))
                            {
                                continue;
                            }

                            var customFieldRow = dsOrders.OrderCustomFields.NewOrderCustomFieldsRow();
                            customFieldRow.OrderID = orderRow.OrderID;
                            customFieldRow.CustomFieldID = customField.CustomFieldId;
                            customFieldRow.Value = customField.Value;
                            dsOrders.OrderCustomFields.AddOrderCustomFieldsRow(customFieldRow);
                        }

                        // Serial Numbers
                        var partOrder = 1;
                        foreach (var serialNumber in workOrder.SerialNumbers)
                        {
                            if (string.IsNullOrEmpty(serialNumber.Number))
                            {
                                continue;
                            }

                            var row = dsOrders.OrderSerialNumber.NewOrderSerialNumberRow();
                            row.Active = true;
                            row.OrderRow = orderRow;
                            row.Number = serialNumber.Number;
                            row.PartOrder = partOrder;
                            dsOrders.OrderSerialNumber.AddOrderSerialNumberRow(row);

                            partOrder++;
                        }

                        // Part marking
                        if (workOrder.SelectedPart.IsPartMarking)
                        {
                            // Check part for part marking
                            PartsDataset.Part_PartMarkingRow markFromPart;
                            using (var taPartMark = new Data.Datasets.PartsDatasetTableAdapters.Part_PartMarkingTableAdapter())
                            {
                                markFromPart = taPartMark
                                    .GetDataByPartID(workOrder.SelectedPart.PartId)
                                    .FirstOrDefault();
                            }

                            var orderPartMarkRow = dsOrders.OrderPartMark.NewOrderPartMarkRow();
                            orderPartMarkRow.OrderRow = orderRow;

                            if (markFromPart != null)
                            {
                                // Add from part
                                if (!markFromPart.IsProcessSpecNull())
                                {
                                    orderPartMarkRow.ProcessSpec = markFromPart.ProcessSpec;
                                }

                                if (!markFromPart.IsDef1Null())
                                {
                                    orderPartMarkRow.Line1 = markFromPart.Def1;
                                }

                                if (!markFromPart.IsDef2Null())
                                {
                                    orderPartMarkRow.Line2 = markFromPart.Def2;
                                }

                                if (!markFromPart.IsDef3Null())
                                {
                                    orderPartMarkRow.Line3 = markFromPart.Def3;
                                }

                                if (!markFromPart.IsDef4Null())
                                {
                                    orderPartMarkRow.Line4 = markFromPart.Def4;
                                }
                            }
                            else
                            {
                                // Check model/customer for part marking
                                OrderProcessingDataSet.PartMarkingRow markFromModel;
                                using (var taPM = new Data.Datasets.OrderProcessingDataSetTableAdapters.PartMarkingTableAdapter())
                                {
                                    markFromModel = taPM
                                        .GetDataByPart(workOrder.SelectedPart.PartId)
                                        .FirstOrDefault();
                                }

                                if (markFromModel != null)
                                {
                                    // Add from model/customer
                                    orderPartMarkRow.PartMarkingID = markFromModel.PartMarkingID;
                                    orderPartMarkRow.ProcessSpec = markFromModel.ProcessSpec;

                                    if (!markFromModel.IsDef1Null())
                                    {
                                        orderPartMarkRow.Line1 = markFromModel.Def1;
                                    }

                                    if (!markFromModel.IsDef2Null())
                                    {
                                        orderPartMarkRow.Line2 = markFromModel.Def2;
                                    }

                                    if (!markFromModel.IsDef3Null())
                                    {
                                        orderPartMarkRow.Line3 = markFromModel.Def3;
                                    }

                                    if (!markFromModel.IsDef4Null())
                                    {
                                        orderPartMarkRow.Line4 = markFromModel.Def4;
                                    }
                                }
                            }

                            dsOrders.OrderPartMark.AddOrderPartMarkRow(orderPartMarkRow);
                        }

                        isFirstWorkOrder = false;
                    }

                    // Save Media
                    foreach (var mediaLink in viewModel.MediaLinks)
                    {
                        var mediaRow = dsOrders.Media.NewMediaRow();
                        mediaRow.Name = mediaLink.Item.Name;
                        mediaRow.FileName = mediaLink.Item.FileName;
                        mediaRow.FileExtension = mediaLink.Item.FileExtension;
                        mediaRow.Media = mediaLink.Item.Data;
                        dsOrders.Media.AddMediaRow(mediaRow);

                        var salesOrderMediaRow = dsOrders.SalesOrder_Media.NewSalesOrder_MediaRow();
                        salesOrderMediaRow.SalesOrderRow = salesOrderRow;
                        salesOrderMediaRow.MediaRow = mediaRow;
                        dsOrders.SalesOrder_Media.AddSalesOrder_MediaRow(salesOrderMediaRow);
                    }

                    // Save Document Links
                    foreach (var documentLink in viewModel.DocumentLinks)
                    {
                        var salesOrderLinkRow = dsOrders.SalesOrder_DocumentLink.NewSalesOrder_DocumentLinkRow();
                        salesOrderLinkRow.SalesOrderRow = salesOrderRow;
                        salesOrderLinkRow.DocumentInfoID = documentLink.DocumentInfoId;
                        salesOrderLinkRow.LinkToType = nameof(Documents.LinkType.SalesOrder);
                        salesOrderLinkRow.LinkToKey = salesOrderRow.SalesOrderID;
                        dsOrders.SalesOrder_DocumentLink.AddSalesOrder_DocumentLinkRow(salesOrderLinkRow);
                    }

                    if (settings.AutomaticallyBatchSalesOrder && !settings.OrderReviewEnabled)
                    {
                        AutoCreateBatches(salesOrderRow, dsOrders, settings);
                        _logger.Info("Auto created batches for Sales Order");
                    }

                    var addedOrders = DataUtilities.GetRowsByRowState(dsOrders.Order, DataRowState.Added);

                    taManager.UpdateAll(dsOrders);

                    using (var taHistory = new Data.Datasets.OrderHistoryDataSetTableAdapters.OrderHistoryTableAdapter())
                    {
                        foreach (var orderRow in addedOrders.OfType<OrdersDataSet.OrderRow>())
                        {
                            taHistory.UpdateOrderHistory(
                                orderRow.OrderID,
                                "New Sales Order",
                                $"Order created as part of Sales Order {salesOrderRow.SalesOrderID}.",
                                SecurityManager.UserInfo.UserName);
                        }
                    }

                    salesOrderId = salesOrderRow.SalesOrderID;
                }
            }

            return salesOrderId;
        }

        private static void AutoCreateBatches(OrdersDataSet.SalesOrderRow salesOrder,
            OrdersDataSet dsOrders, ApplicationSettings appSettings)
        {
            // Group orders together by process
            var orderIdProcessesDict = new Dictionary<int, List<int>>();

            foreach (var wo in salesOrder.GetOrderRows())
            {
                if (wo.GetOrderProcessesRows().Length == 0)
                {
                    // Work Order was created through redline
                    // feature - skip for now
                    continue;
                }

                if (appSettings.BatchMultipleProcesses)
                {
                    orderIdProcessesDict[wo.OrderID] = wo.GetOrderProcessesRows()
                        .OrderBy(op => op.StepOrder)
                        .Select(op => op.ProcessID)
                        .ToList();
                }
                else
                {
                    orderIdProcessesDict[wo.OrderID] = wo.GetOrderProcessesRows()
                        .OrderBy(op => op.StepOrder)
                        .Take(1)
                        .Select(op => op.ProcessID)
                        .ToList();
                }
            }

            // Determine orders to be batched together
            var batches = new List<NewBatch>();
            foreach (var orderIdAndProcesses in orderIdProcessesDict)
            {
                var orderId = orderIdAndProcesses.Key;
                var processes = orderIdAndProcesses.Value;

                var matchingBatch = batches.FirstOrDefault(b => b.Matches(processes));
                if (matchingBatch != null)
                {
                    matchingBatch.OrderIds.Add(orderId);
                }
                else
                {
                    batches.Add(new NewBatch(processes, orderId));
                }
            }

            // Create batches
            foreach (var newBatch in batches)
            {
                var firstOrderRow = dsOrders.Order.FindByOrderID(newBatch.OrderIds.First());
                var newBatchRow = dsOrders.Batch.NewBatchRow();
                newBatchRow.Active = true;
                newBatchRow.OpenDate = DateTime.Now;
                newBatchRow.WorkStatus = appSettings.WorkStatusChangingDepartment;
                newBatchRow.CurrentLocation = appSettings.DepartmentSales;
                newBatchRow.SchedulePriority = 0;
                newBatchRow.SalesOrderID = salesOrder.SalesOrderID;
                dsOrders.Batch.AddBatchRow(newBatchRow);

                foreach (var orderId in newBatch.OrderIds)
                {
                    var orderRow = dsOrders.Order.FindByOrderID(orderId);
                    dsOrders.BatchOrder.AddBatchOrderRow(newBatchRow,
                        orderRow,
                        orderRow.IsPartQuantityNull() ? 0 : orderRow.PartQuantity);
                }

                var orderProcessesByStep = newBatchRow.GetBatchOrderRows()
                    .SelectMany(batchOrder => batchOrder.OrderRow.GetOrderProcessesRows())
                    .GroupBy(orderProcess => orderProcess.StepOrder)
                    .ToList();

                foreach (var step in orderProcessesByStep)
                {
                    var firstOrderProcess = step.First();
                    var stepOrder = step;
                    var newBatchProcessRow = dsOrders.BatchProcesses.NewBatchProcessesRow();
                    newBatchProcessRow.BatchRow = newBatchRow;
                    newBatchProcessRow.ProcessID = firstOrderProcess.ProcessID;
                    newBatchProcessRow.StepOrder = firstOrderProcess.StepOrder;
                    newBatchProcessRow.Department = firstOrderProcess.Department;
                    dsOrders.BatchProcesses.AddBatchProcessesRow(newBatchProcessRow);

                    foreach (var orderProcessRow in step)
                    {
                        dsOrders.BatchProcess_OrderProcess.AddBatchProcess_OrderProcessRow(
                            newBatchProcessRow,
                            orderProcessRow);
                    }
                }
            }
        }

        public SystemFieldInfo GetField(string category, string fieldName) =>
            FieldUtilities.GetField(category, fieldName);

        private int SavePart(PartViewModel part, int customerId)
        {
            PartsDataset.PartDataTable dtParts = null;
            Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter taParts = null;
            Data.Datasets.OrderHistoryDataSetTableAdapters.PartHistoryTableAdapter taHistory = null;

            try
            {
                dtParts = new PartsDataset.PartDataTable();
                taParts = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter();
                taHistory = new Data.Datasets.OrderHistoryDataSetTableAdapters.PartHistoryTableAdapter();

                var newPartRow = dtParts.NewPartRow();
                newPartRow.Name = part.Name.ToUpper();
                newPartRow.CustomerID = customerId;
                newPartRow.Active = true;
                newPartRow.PartMarking = false;
                newPartRow.LastModified = CurrentTimeProvider.Now;
                dtParts.AddPartRow(newPartRow);

                taParts.Update(dtParts);

                taHistory.UpdatePartHistory(
                    newPartRow.PartID,
                    "New Sales Order",
                    $"New part created when adding a new Sales Order.",
                    SecurityManager.UserInfo.UserName);

                _logger.Info($"Saved new part {newPartRow.PartID} - {newPartRow.Name}");
                return newPartRow.PartID;
            }
            finally
            {
                dtParts?.Dispose();
                taParts?.Dispose();
                taHistory?.Dispose();
            }
        }

        public IEnumerable<PartProcess> GetPartProcesses(int partId)
        {
            using (var taPartProcess = new PartProcessSummaryTableAdapter())
            {
                List<PartProcess> processes;
                using (var dtPartProcess = new OrdersDataSet.PartProcessSummaryDataTable())
                {
                    taPartProcess.FillByPart(dtPartProcess, partId);

                    processes = dtPartProcess
                        .OrderBy(processRow => processRow.StepOrder)
                        .Select(PartProcess.From)
                        .ToList();
                }

                foreach (var process in processes)
                {
                    if (process.LoadCapacityQuantity.HasValue || process.LoadCapacityQuantity.HasValue)
                    {
                        continue;
                    }

                    var loadCapacityWeight = taPartProcess.GetLoadCapacityWeight(process.ProcessId);

                    if (loadCapacityWeight.HasValue)
                    {
                        process.LoadCapacityWeight = loadCapacityWeight;
                    }
                    else
                    {
                        var qtyDecimal = taPartProcess.GetLoadCapacityQuantity(process.ProcessId);

                        if (qtyDecimal.HasValue)
                        {
                            var qty = (int)Math.Round(qtyDecimal.Value, MidpointRounding.AwayFromZero);
                            process.LoadCapacityQuantity = qty;
                        }
                    }
                }

                return processes;
            }
        }

        private IEnumerable<ProcessRequisite> GetProcessRequisites(int processId)
        {
            using (var dtProcessRequisite = new ProcessesDataset.ProcessRequisiteDataTable())
            {
                using (var taProcessRequisite = new Data.Datasets.ProcessesDatasetTableAdapters.ProcessRequisiteTableAdapter())
                {
                    taProcessRequisite.FillByParent(dtProcessRequisite, processId);
                }

                return dtProcessRequisite
                    .Select(reqRow => new ProcessRequisite(reqRow.ChildProcessID, reqRow.Hours))
                    .ToList();
            }
        }

        #endregion


        #region NewBatch

        private class NewBatch
        {
            public List<int> ProcessIds { get; }

            public List<int> OrderIds { get; }

            public NewBatch(List<int> processIds, int orderId)
            {
                ProcessIds = processIds ?? throw new ArgumentNullException();
                OrderIds = new List<int> { orderId };
            }

            public bool Matches(List<int> processIds) =>
                Enumerable.SequenceEqual(ProcessIds, processIds);
        }

        #endregion
    }
}
