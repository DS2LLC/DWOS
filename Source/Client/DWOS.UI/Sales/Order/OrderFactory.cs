using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Order;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace DWOS.UI.Sales
{
    public class OrderFactory
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private bool _useRequiredDate = true;
        private readonly ISet<string> _disallowedRelations = new HashSet<string>()
            {
                "FK_OrderProcessAnswer_Order", // cyclical relationship with order via order process
                "FK_OrderContainers_Order", // split as a separate step
                "FK_BatchOrder_Order", // new orders should not go into a batch
                "FK_OrderSerialNumber_Order", // split as a separate step
                "FK_OrderProcessesOperator_OrderProcesses", // labor time stays with existing order
                "FK_OrderOperator_Order", // labor time stays with existing order
                "FK_Order_Order", // do not copy dependent orders
                "FK_OrderApproval_Order", // do not copy order approvals
            };

        #endregion

        #region Properties

        public ILeadTimeScheduler Scheduler { get; set; }
        public OrdersDataSet Orders { get; set; }
        public TableAdapterManager TableManager { get; set; }

        #endregion

        #region Methods

        public void Load()
        {
            Orders = new OrdersDataSet() {EnforceConstraints = false};
            TableManager = new TableAdapterManager
            {
                OrderTableAdapter = new OrderTableAdapter() { ClearBeforeFill = false },
                PartSummaryTableAdapter = new PartSummaryTableAdapter() { ClearBeforeFill = false },
                OrderProcessesTableAdapter = new OrderProcessesTableAdapter() { ClearBeforeFill = false },
                OrderContainersTableAdapter = new OrderContainersTableAdapter() { ClearBeforeFill = false },
                OrderCustomFieldsTableAdapter = new OrderCustomFieldsTableAdapter() { ClearBeforeFill = false },
                COCTableAdapter = new COCTableAdapter() { ClearBeforeFill = false },

                OrderFeesTableAdapter = new OrderFeesTableAdapter() { ClearBeforeFill = false },
                OrderFeeTypeTableAdapter = new OrderFeeTypeTableAdapter() { ClearBeforeFill = false },
                Order_MediaTableAdapter = new Order_MediaTableAdapter() { ClearBeforeFill = false },

                CustomerCommunicationTableAdapter = new CustomerCommunicationTableAdapter() { ClearBeforeFill = false },
                OrderShipmentTableAdapter = new OrderShipmentTableAdapter() { ClearBeforeFill = false },
                OrderReviewTableAdapter = new OrderReviewTableAdapter() { ClearBeforeFill = false },
                PartInspectionTypeTableAdapter = new PartInspectionTypeTableAdapter(),
                PartInspectionTableAdapter = new PartInspectionTableAdapter(),
                PartInspectionAnswerTableAdapter = new PartInspectionAnswerTableAdapter() { ClearBeforeFill = false },
                OrderProcessAnswerTableAdapter = new OrderProcessAnswerTableAdapter() { ClearBeforeFill = false },
                OrderPartMarkTableAdapter = new OrderPartMarkTableAdapter() { ClearBeforeFill = false },
                OrderNoteTableAdapter = new OrderNoteTableAdapter() { ClearBeforeFill = false },
                OrderHoldTableAdapter = new OrderHoldTableAdapter() { ClearBeforeFill = false },
                InternalReworkTableAdapter = new InternalReworkTableAdapter() { ClearBeforeFill = false },
                OrderChangeTableAdapter = new OrderChangeTableAdapter() { ClearBeforeFill = false },

                SalesOrderTableAdapter = new SalesOrderTableAdapter() { ClearBeforeFill = false },

                OrderSerialNumberTableAdapter = new OrderSerialNumberTableAdapter { ClearBeforeFill = false },

                CustomerAddressTableAdapter = new CustomerAddressTableAdapter { ClearBeforeFill = false }
            };

            // Check required date field
            ApplicationSettingsDataSet.FieldsDataTable fields;

            using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
            {
                fields = ta.GetByCategory("Order");
            }

            var requiredDateField = fields.FirstOrDefault(f => f.Name == "Required Date");
            _useRequiredDate = requiredDateField == null || requiredDateField.IsVisible;
        }

        public void Load(OrdersDataSet orders, TableAdapterManager tableManager)
        {
            Orders = orders;
            TableManager = tableManager;

            // Check required date field
            ApplicationSettingsDataSet.FieldsDataTable fields;

            using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
            {
                fields = ta.GetByCategory("Order");
            }

            var requiredDateField = fields.FirstOrDefault(f => f.Name == "Required Date");
            _useRequiredDate = requiredDateField == null || requiredDateField.IsVisible;
        }

        public OrdersDataSet.OrderRow AddOrder(OrdersDataSet.OrderTemplateRow orderTemplate, int partQuantity, Dictionary<int, string> customFields)
        {
            if (orderTemplate == null || partQuantity < 1)
                return null;

            _log.Info("Adding a new work order based on blanket PO {0}.", orderTemplate.OrderTemplateID);

            //load customer
            if (Orders.CustomerSummary.FindByCustomerID(orderTemplate.CustomerID) == null)
            {
                using(var ta = new CustomerSummaryTableAdapter() {ClearBeforeFill = false})
                    ta.FillByCustomerID(Orders.CustomerSummary, orderTemplate.CustomerID);
            }

            //load part
            if(Orders.PartSummary.FindByPartID(orderTemplate.PartID) == null)
                TableManager.PartSummaryTableAdapter.FillByPart(Orders.PartSummary, orderTemplate.PartID);
            
            var orderDate = DateTime.Now;

            //create new order based on the blanket PO info
            var orderRow = Orders.Order.NewOrderRow();
            orderRow.OrderDate = orderDate;
            orderRow.OrderTemplateID = orderTemplate.OrderTemplateID;
            orderRow.CustomerID = orderTemplate.CustomerID;
            orderRow.PartID = orderTemplate.PartID;
            orderRow.PartQuantity = partQuantity;
            orderRow.ContractReviewed = true;
            orderRow.OrderType = (int)OrderType.Normal;
            orderRow.OriginalOrderType = (int)OrderType.Normal;
            orderRow.PriceUnit = OrderPrice.enumPriceUnit.Each.ToString();
            orderRow.BasePrice = 0;
            orderRow.Status = Properties.Settings.Default.OrderStatusOpen;
            orderRow.Priority = Properties.Settings.Default.OrderPriorityDefault;
            orderRow.CreatedBy = SecurityManager.Current.UserID;
            orderRow.Hold = false;
            orderRow.CurrentLocation = ApplicationSettings.Current.DepartmentSales;
            orderRow.WorkStatus = OrderControllerExtensions.GetNewOrderWorkStatus(orderRow.IsCustomerIDNull() ? 0 : orderRow.CustomerID, OrderInformation.UserRequiresOrderReview);

            var defaultShipDate = orderDate.AddBusinessDays(orderRow.CustomerSummaryRow?.LeadTime ?? ApplicationSettings.Current.OrderLeadTime);

            if (_useRequiredDate)
            {
                orderRow.RequiredDate = defaultShipDate;
            }

            orderRow.EstShipDate = defaultShipDate;

            if (!orderTemplate.IsPurchaseOrderNull())
                orderRow.PurchaseOrder = orderTemplate.PurchaseOrder;
            if (!orderTemplate.IsPriorityNull())
                orderRow.Priority = orderTemplate.Priority;
            if (!orderTemplate.IsShippingMethodNull())
                orderRow.ShippingMethod = orderTemplate.ShippingMethod;
            if (!orderTemplate.IsPriceUnitNull())
                orderRow.PriceUnit = orderTemplate.PriceUnit;
            if (!orderTemplate.IsBasePriceNull())
                orderRow.BasePrice = orderTemplate.BasePrice;
            if (!orderTemplate.IsCustomerAddressIDNull())
                orderRow.CustomerAddressID = orderTemplate.CustomerAddressID;

            if (orderTemplate.PartSummaryRow != null && !orderTemplate.PartSummaryRow.IsWeightNull())
            {
                const decimal maxWeight = 999999.99M;
                orderRow.Weight = Math.Min(orderTemplate.PartSummaryRow.Weight * Convert.ToDecimal(partQuantity), maxWeight);
            }

            orderRow.EndEdit();
            Orders.Order.AddOrderRow(orderRow);

            //force add children data since the part was pre-selected
            AddOrderProcesses(orderRow);
            AddOrderPartMark(orderRow);
            AddCustomFields(orderRow, customFields);

            return orderRow;
        }

        public void Save()
        {
            TableManager.UpdateAll(Orders);
        }

        private void AddOrderPartMark(OrdersDataSet.OrderRow order)
        {
            try
            {
                if (order == null || order.IsPartIDNull() || order.PartSummaryRow == null || !order.PartSummaryRow.PartMarking)
                {
                    return;
                }

                _log.Info("Adding part mark to order.");

                OrderProcessingDataSet.PartMarkingRow partMarkForOrder;

                using (var taPM = new Data.Datasets.OrderProcessingDataSetTableAdapters.PartMarkingTableAdapter())
                {
                    partMarkForOrder = taPM.GetDataByPart(order.PartID).FirstOrDefault();
                }

                if (partMarkForOrder != null)
                {
                    _log.Info("Order creating new part marking {0}.", partMarkForOrder.PartMarkingID);

                    var newPartMark = Orders.OrderPartMark.NewOrderPartMarkRow();

                    newPartMark.OrderRow = order;
                    newPartMark.PartMarkingID = partMarkForOrder.PartMarkingID;
                    newPartMark.ProcessSpec = partMarkForOrder.ProcessSpec;
                    newPartMark.Line1 = partMarkForOrder.IsDef1Null() ? null : partMarkForOrder.Def1;
                    newPartMark.Line2 = partMarkForOrder.IsDef2Null() ? null : partMarkForOrder.Def2;
                    newPartMark.Line3 = partMarkForOrder.IsDef3Null() ? null : partMarkForOrder.Def3;
                    newPartMark.Line4 = partMarkForOrder.IsDef4Null() ? null : partMarkForOrder.Def4;

                    Orders.OrderPartMark.AddOrderPartMarkRow(newPartMark);
                }
                else //if need to add a blank template because Part says use part marking but has no template
                {
                    _log.Info("Order creating new blank part marking.");

                    var newPartMark = Orders.OrderPartMark.NewOrderPartMarkRow();
                    newPartMark.OrderRow = order;

                    Orders.OrderPartMark.AddOrderPartMarkRow(newPartMark);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding order part mark.");
            }
        }

        private void AddCustomFields(OrdersDataSet.OrderRow order, Dictionary<int, string> customFields)
        {
            try
            {
                _log.Debug("Adding custom fields to order.");

                CustomerSummaryTableAdapter taCustomer = new CustomerSummaryTableAdapter();
                CustomFieldTableAdapter taCustomField = new CustomFieldTableAdapter();

                //Get the custom fields for customer
                taCustomField.FillByCustomer(Orders.CustomField, order.CustomerID);

                //Find current customer
                var customer = Orders.CustomerSummary.FindByCustomerID(order.CustomerID);

                if (customer != null)
                {
                    var customFieldRows = customer.GetCustomFieldRows();

                    foreach (var customFieldRow in customFieldRows)
                    {
                        //Add the custom field to the order
                        string value = string.Empty;
                        if (customFields.ContainsKey(customFieldRow.CustomFieldID))
                        {
                            value = customFields[customFieldRow.CustomFieldID];
                        }

                        Orders.OrderCustomFields.AddOrderCustomFieldsRow(order, customFieldRow, value);
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error adding custom fields to order.";
                _log.Error(exc, errorMsg);
            }
        }

        private void AddOrderProcesses(OrdersDataSet.OrderRow order)
        {
            PartProcessSummaryTableAdapter ppsTA = null;
            Data.Datasets.ProcessesDatasetTableAdapters.ProcessRequisiteTableAdapter processReqTA = null;

            try
            {
                _log.Info("Adding order processes to the order.");

                var orderProcessRows = order.GetOrderProcessesRows();

                //if contains existing process rows then nothing to do
                if (orderProcessRows.Length > 0)
                    return;

                if (order.IsPartIDNull())
                    return;

                if (Scheduler == null && ApplicationSettings.Current.SchedulingEnabled)
                {
                    if (ApplicationSettings.Current.SchedulerType == SchedulerType.ProcessLeadTime)
                    {
                        var scheduler = new RoundingLeadTimeScheduler();
                        scheduler.LoadData();
                        Scheduler = scheduler;
                    }
                    else if (ApplicationSettings.Current.SchedulerType == SchedulerType.ProcessLeadTimeHour)
                    {
                        var scheduler = new LeadTimeScheduler();
                        scheduler.LoadData();
                        Scheduler = scheduler;
                    }
                }

                //Find Part Processes
                ppsTA = new PartProcessSummaryTableAdapter();
                processReqTA = new Data.Datasets.ProcessesDatasetTableAdapters.ProcessRequisiteTableAdapter();
                var processes = ppsTA.GetData(order.PartID);
                var pastProcesses = new HashSet<int>();

                //For each process required for the part
                var processLeadTimes = new OrderProcessLeadTimes();
                foreach (var process in processes.OrderBy(p => p.StepOrder))
                {
                    var orderProcess = Orders.OrderProcesses.NewOrderProcessesRow();
                    orderProcess.OrderID = order.OrderID;
                    orderProcess.ProcessID = process.ProcessID;
                    orderProcess.ProcessAliasID = process.ProcessAliasID;
                    orderProcess.StepOrder = process.StepOrder;
                    orderProcess.Department = process.Department;
                    orderProcess.COCData = (!process.IsCOCCountNull() && process.COCCount > 0) || ApplicationSettings.Current.DisplayProcessCOCByDefault; //only default on COC if COC Count exists 
                    orderProcess.OrderProcessType = (int)OrderProcessType.Normal;

                    // Load Capacity
                    if (!process.IsLoadCapacityVarianceNull())
                    {
                        orderProcess.LoadCapacityVariance = process.LoadCapacityVariance;
                    }

                    var hasCapacity = false;

                    if (!process.IsLoadCapacityWeightNull())
                    {
                        orderProcess.LoadCapacityWeight = process.LoadCapacityWeight;
                        hasCapacity = true;
                    }

                    if (!process.IsLoadCapacityQuantityNull())
                    {
                        orderProcess.LoadCapacityQuantity = process.LoadCapacityQuantity;
                        hasCapacity = true;
                    }

                    if (!hasCapacity)
                    {
                        // Check process
                        var weight = ppsTA.GetLoadCapacityWeight(process.ProcessID);

                        if (weight.HasValue)
                        {
                            orderProcess.LoadCapacityWeight = weight.Value;
                        }
                        else
                        {
                            var qtyDecimal = ppsTA.GetLoadCapacityWeight(process.ProcessID);

                            if (qtyDecimal.HasValue)
                            {
                                var qty = (int)Math.Round(qtyDecimal.Value, MidpointRounding.AwayFromZero);
                                orderProcess.LoadCapacityQuantity = qty;
                            }
                        }
                    }

                    // Process prerequisites
                    if (pastProcesses.Count > 0)
                    {
                        using (var processReqTable = new ProcessesDataset.ProcessRequisiteDataTable())
                        {
                            processReqTA.FillByParent(processReqTable, process.ProcessID);

                            //Add process pre requisites to the order process if they exist
                            var processReqs = processReqTable.Where(p => pastProcesses.Contains(p.ChildProcessID));

                            if (processReqs.Any())
                            {
                                var lowestReq = processReqs.OrderByDescending(r => r.Hours).FirstOrDefault();
                                orderProcess.RequisiteProcessID = lowestReq.ChildProcessID;
                                orderProcess.RequisiteHours = lowestReq.Hours;
                            }
                        }
                    }

                    Orders.OrderProcesses.AddOrderProcessesRow(orderProcess);

                    //add list of processes already added
                    if (!pastProcesses.Contains(process.ProcessID))
                        pastProcesses.Add(process.ProcessID);

                    processLeadTimes.Add(orderProcess.OrderProcessesID, process);
                }

                //update schedule
                if (Scheduler != null)
                {
                    order.EstShipDate = Scheduler.UpdateScheduleDates(order, processLeadTimes);
                }

                _log.Info("Added {0} order processes to order {1}.", processes.Count, order.OrderID);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error creating order processes.");
            }
            finally
            {
                ppsTA?.Dispose();
                processReqTA?.Dispose();
            }
        }

        /// <summary>
        /// Loads the order into the existing dataset.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        public void LoadOrderData(int orderId)
        {
            TableManager.OrderTableAdapter.FillByOrderID(Orders.Order, orderId);

            //load order custom fields
            TableManager.OrderCustomFieldsTableAdapter.FillByOrder(Orders.OrderCustomFields, orderId);

            //load order fees
            TableManager.OrderFeesTableAdapter.FillByOrder(Orders.OrderFees, orderId);
            TableManager.OrderFeeTypeTableAdapter.Fill(Orders.OrderFeeType);

            //load media
            TableManager.Order_MediaTableAdapter.FillByOrder(Orders.Order_Media, orderId);

            //Load Communications
            TableManager.CustomerCommunicationTableAdapter.FillByOrder(Orders.CustomerCommunication, orderId);

            //Load COCs
            TableManager.COCTableAdapter.FillByOrderNoData(Orders.COC, orderId);

            //Load Shipments
            TableManager.OrderShipmentTableAdapter.FillByOrder(Orders.OrderShipment, orderId);

            //Load Order Review
            TableManager.OrderReviewTableAdapter.FillByOrder(Orders.OrderReview, orderId);

            //Load Order Processing Node
            TableManager.OrderProcessesTableAdapter.FillBy(Orders.OrderProcesses, orderId);
            TableManager.OrderProcessAnswerTableAdapter.FillBy(Orders.OrderProcessAnswer, orderId);

            //Load Inspections
            TableManager.PartInspectionTypeTableAdapter.Fill(Orders.PartInspectionType);
            TableManager.PartInspectionTableAdapter.FillBy(Orders.PartInspection, orderId);
            TableManager.PartInspectionAnswerTableAdapter.FillBy(Orders.PartInspectionAnswer, orderId);

            //Load Order Part Mark
            TableManager.OrderPartMarkTableAdapter.FillByOrder(Orders.OrderPartMark, orderId);

            //Load Order Notes
            TableManager.OrderNoteTableAdapter.FillByOrder(Orders.OrderNote, orderId);

            //Load Order Holds
            TableManager.OrderHoldTableAdapter.Fill(Orders.OrderHold, orderId);

            //Load Internal Rework
            TableManager.InternalReworkTableAdapter.FillByReworkOrderID(Orders.InternalRework, orderId);
            TableManager.InternalReworkTableAdapter.FillByOriginalOrderID(Orders.InternalRework, orderId);

            //Load Container Node
            TableManager.OrderContainersTableAdapter.FillByOrder(Orders.OrderContainers, orderId);

            // Load serial numbers
            TableManager.OrderSerialNumberTableAdapter.FillByOrder(Orders.OrderSerialNumber, orderId);

            // Load shipping info
            using (var taShipping = new CustomerShippingSummaryTableAdapter())
            {
                taShipping.FillByOrder(Orders.CustomerShippingSummary, orderId);
            }

            TableManager.CustomerAddressTableAdapter.FillByOrder(Orders.CustomerAddress, orderId);
        }

        public List<OrdersDataSet.OrderRow> SplitOrder(OrdersDataSet.OrderRow currentOrder, List<SplitOrderInfo> splits, int reasonCode)
        {
            var newOrders = DoSplit(currentOrder, splits);

            if (newOrders == null || newOrders.Count == 0)
            {
                // Could not find order to split
                return newOrders;
            }

            foreach (var newOrder in newOrders)
            {
                // add link between split and the original order
                var changeRow = Orders.OrderChange.NewOrderChangeRow();
                changeRow.ChangeType = (int)OrderChangeType.Split;
                changeRow.ReasonCode = reasonCode;
                changeRow.ParentOrderID = currentOrder.OrderID;
                changeRow.ChildOrderID = newOrder.OrderID;
                changeRow.UserID = SecurityManager.Current.UserID;
                changeRow.DateCreated = DateTime.Now;

                Orders.OrderChange.AddOrderChangeRow(changeRow);
            }

            OrderHistoryDataSet.UpdateOrderHistory(currentOrder.OrderID, "Order Entry", "Order split.", SecurityManager.Current.UserName);

            return newOrders;
        }

        public List<OrdersDataSet.OrderRow> SplitRework(OrdersDataSet.OrderRow currentOrder, List<SplitOrderInfo> splits) =>
            DoSplit(currentOrder, splits);

        private List<OrdersDataSet.OrderRow> DoSplit(OrdersDataSet.OrderRow currentOrder, List<SplitOrderInfo> splits)
        {
            var orgOrder = splits.Find(x => x.IsOriginalOrder);
            if (orgOrder == null || currentOrder == null)
            {
                // Cannot find order to split
                return new List<OrdersDataSet.OrderRow>();
            }

            int originalQty = currentOrder.PartQuantity;
            decimal originalBasePrice = currentOrder.IsBasePriceNull()
                ? 0M
                : currentOrder.BasePrice;

            currentOrder.PartQuantity = orgOrder.PartQty;

            // Split unit price for Lot orders
            if (currentOrder.IsPriceUnitNull() || OrderPrice.IsPriceUnitLot(currentOrder.PriceUnit))
            {
                var fractionOfTotalQty = (Convert.ToDecimal(orgOrder.PartQty) / Convert.ToDecimal(originalQty));
                currentOrder.BasePrice = fractionOfTotalQty * originalBasePrice;
            }

            // Split weight
            var partWeight = GetPartWeight(currentOrder);
            if (partWeight.HasValue)
            {
                currentOrder.Weight = partWeight.Value * orgOrder.PartQty;
            }

            var newOrders = new List<OrdersDataSet.OrderRow>();

            foreach (var so in splits.Where(x => !x.IsOriginalOrder))
            {
                //paste node to parent
                var copiedProxy = CopyCommand.CopyRows(currentOrder);
                CleanOriginalForSplit(copiedProxy);

                var newOrder = DataNode<DataRow>.AddPastedDataRows(copiedProxy, Orders.Order) as OrdersDataSet.OrderRow;

                // Update quantity
                newOrder.PartQuantity = so.PartQty;

                // Disable 'Imported' indicator for the split order
                newOrder.FromShippingManifest = false;

                //update all OrderID's on OrderProcessAnswers and Part Inspections
                foreach (var op in newOrder.GetOrderProcessesRows())
                {
                    foreach (var opa in op.GetOrderProcessAnswerRows())
                    {
                        opa.OrderID = newOrder.OrderID;
                    }

                    foreach (var pi in op.GetPartInspectionRows())
                    {
                        pi.OrderID = newOrder.OrderID;
                    }
                }

                // Remove fixed fees - they stay with the original order
                foreach (var fee in newOrder.GetOrderFeesRows())
                {
                    if (fee.OrderFeeTypeRow.FeeType != nameof(OrderPrice.enumFeeType.Percentage))
                    {
                        fee.Delete();
                    }
                }

                // Split unit price for Lot orders
                if (newOrder.IsPriceUnitNull() || OrderPrice.IsPriceUnitLot(newOrder.PriceUnit))
                {
                    var fractionOfTotalQty = (Convert.ToDecimal(newOrder.PartQuantity) / Convert.ToDecimal(originalQty));
                    newOrder.BasePrice = fractionOfTotalQty * originalBasePrice;
                }

                // Split weight
                if (partWeight.HasValue)
                {
                    newOrder.Weight = partWeight.Value * newOrder.PartQuantity;
                }

                if (ApplicationSettings.Current.AllowPartialProcessLoads)
                {
                    // update part counts for order processes
                    foreach (var origProcess in currentOrder.GetOrderProcessesRows())
                    {
                        var newProcess = newOrder.GetOrderProcessesRows()
                            .FirstOrDefault(opr => opr.ProcessID == origProcess.ProcessID && opr.StepOrder == origProcess.StepOrder);

                        if (newProcess == null || origProcess.IsPartCountNull())
                        {
                            continue;
                        }

                        var processedQty = origProcess.PartCount;
                        var origQty = orgOrder.PartQty;

                        if (processedQty <= origQty)
                        {
                            //EX: processed 10, accepted 20 => orig = 10, new = 0
                            origProcess.PartCount = processedQty;
                            newProcess.PartCount = 0;
                        }
                        else
                        {
                            //EX: processed 10, accepted 6 => orig = 6, new = 4
                            origProcess.PartCount = origQty;
                            newProcess.PartCount = processedQty - origQty;
                        }
                    }
                }

                //Load COC data if exists
                var cocs = newOrder.GetCOCRows();
                var cocOriginals = currentOrder.GetCOCRows();

                for (int i = 0; i < cocs.Length; i++)
                {
                    cocs[i].COCInfo = i < cocOriginals.Length
                        ? TableManager.COCTableAdapter.GetCOCInfo(cocOriginals[i].COCID)
                        : string.Empty;
                }

                // Load Batch COC data
                // Required to save existing Batch COCs for new orders
                var batchCocOrders = newOrder.GetBatchCOCOrderRows();

                for (var i = 0; i < batchCocOrders.Length; i++)
                {
                    var batchCoc = batchCocOrders[i].BatchCOCRow;

                    if (batchCoc == null)
                    {
                        _log.Error("Did not load Batch COC data.");
                    }
                    else
                    {
                        batchCoc.COCInfo = TableManager.BatchCOCTableAdapter.GetCocInfo(batchCoc.BatchCOCID);
                    }
                }

                newOrders.Add(newOrder);
            }

            return newOrders;
        }

        private void CleanOriginalForSplit(DataRowProxy copiedProxy)
        {
            var childProxies = copiedProxy.ChildProxies
                .Where(i => !_disallowedRelations.Contains(i.ParentRelation))
                .ToList();

            foreach (var childProxy in childProxies)
            {
                CleanOriginalForSplit(childProxy);
            }

            copiedProxy.ChildProxies = childProxies;
        }

        public void Rejoin(OrdersDataSet.OrderRow sourceOrder, OrdersDataSet.OrderRow destinationOrder, int reasonCode)
        {
            if (sourceOrder == null || destinationOrder == null)
            {
                return;
            }

            var rejoinTime = DateTime.Now;

            var srcPartQty = sourceOrder.IsPartQuantityNull() ? 0 : sourceOrder.PartQuantity;
            var destPartQty = destinationOrder.IsPartQuantityNull() ? 0 : destinationOrder.PartQuantity;

            var isSrcLot = sourceOrder.IsPriceUnitNull() || OrderPrice.IsPriceUnitLot(sourceOrder.PriceUnit);
            var isDestLot = destinationOrder.IsPriceUnitNull() || OrderPrice.IsPriceUnitLot(destinationOrder.PriceUnit);


            var newDestQty = srcPartQty + destPartQty;

            // Rejoin lot prices
            if (isSrcLot && isDestLot)
            {
                if (destinationOrder.IsBasePriceNull() || destPartQty == 0)
                {
                    _log.Warn("Cannot calculate base price for destination order.");
                }
                else
                {
                    var newDestPrice = (destinationOrder.BasePrice / Convert.ToDecimal(destPartQty)) * newDestQty;
                    destinationOrder.BasePrice = newDestPrice;
                }
            }

            // Rejoin weight
            var calculatedPerPieceWeight = GetPartWeight(destinationOrder);
            if (calculatedPerPieceWeight.HasValue)
            {
                destinationOrder.Weight = calculatedPerPieceWeight.Value * newDestQty;
            }

            sourceOrder.Weight = 0;

            destinationOrder.PartQuantity = newDestQty;

            sourceOrder.PartQuantity = 0;
            sourceOrder.Status = Properties.Settings.Default.OrderStatusClosed;
            sourceOrder.Invoice = "NA"; // prevent export
            sourceOrder.BasePrice = 0M;
            sourceOrder.CompletedDate = rejoinTime;

            // Remove fees from source order
            foreach (var sourceFee in sourceOrder.GetOrderFeesRows())
            {
                sourceFee.Delete();
            }

            // Link source and destination orders
            var changeRow = Orders.OrderChange.NewOrderChangeRow();
            changeRow.ChangeType = (int)OrderChangeType.Rejoin;
            changeRow.ReasonCode = reasonCode;
            changeRow.ParentOrderID = sourceOrder.OrderID;
            changeRow.ChildOrderID = destinationOrder.OrderID;
            changeRow.UserID = SecurityManager.Current.UserID;
            changeRow.DateCreated = rejoinTime;

            Orders.OrderChange.AddOrderChangeRow(changeRow);

            OrderHistoryDataSet.UpdateOrderHistory(sourceOrder.OrderID, "Order Entry", "Order rejoined.", SecurityManager.Current.UserName);
            OrderHistoryDataSet.UpdateOrderHistory(destinationOrder.OrderID, "Order Entry", "Order rejoined.", SecurityManager.Current.UserName);
        }

        private decimal? GetPartWeight(OrdersDataSet.OrderRow order)
        {
            if (order.PartSummaryRow == null)
            {
                var partId = order.IsPartIDNull() ? -1 : order.PartID;

                using (var dtPart = new OrdersDataSet.PartSummaryDataTable())
                {
                    TableManager.PartSummaryTableAdapter.FillByPart(dtPart, partId);
                    var part = dtPart.FirstOrDefault();

                    if (part == null || part.IsWeightNull())
                    {
                        return EstimatePartWeight(order);
                    }

                    return part.Weight;
                }
            }

            if (order.PartSummaryRow.IsWeightNull())
            {
                return EstimatePartWeight(order);
            }

            return order.PartSummaryRow.Weight;
        }

        private static decimal? EstimatePartWeight(OrdersDataSet.OrderRow order)
        {
            var partQty = order.IsPartQuantityNull() ? 0 : order.PartQuantity;

            if (partQty > 0)
            {
                return order.IsWeightNull() ? (decimal?)null : order.Weight / partQty;
            }

            return null;
        }

        #endregion
    }
}