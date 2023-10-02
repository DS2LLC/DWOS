using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using DWOS.Reports;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Reports;
using DWOS.Data.Order;
using DWOS.Reports.ReportData;
using System.Threading;

namespace DWOS.UI.Reports
{
    public class WorkOrderLabelReport : ILabelReport
    {
        #region Fields

        private GenericWorkOrderLabelReport _report =
            new GenericWorkOrderLabelReport(LabelFactory.LabelType.WO);

        #endregion

        #region Properties

        public OrdersDataSet.OrderRow Order
        {
            get
            {
                return _report.Order;
            }
            set
            {
                _report.Order = value;
            }
        }

        #endregion

        #region ILabelReport Members

        public void DisplayReport(CancellationToken cancellationToken)
        {
            _report.DisplayReport(cancellationToken);
        }

        public void PrintLabel(string printerName)
        {
            _report.PrintLabel(printerName);
        }

        public void Dispose()
        {
            _report?.Dispose();
            _report = null;
        }

        #endregion
    }

    public class ExternalReworkLabelReport : ILabelReport
    {
        #region Fields

        private GenericWorkOrderLabelReport _report =
            new GenericWorkOrderLabelReport(LabelFactory.LabelType.ExternalRework);

        #endregion

        #region Properties

        public OrdersDataSet.OrderRow Order
        {
            get
            {
                return _report.Order;
            }
            set
            {
                _report.Order = value;
            }
        }

        #endregion

        #region Methods

        public ExternalReworkLabelReport(OrdersDataSet.OrderRow order)
        {
            Order = order;
        }

        #endregion

        #region ILabelReport Members

        public void DisplayReport(CancellationToken cancellationToken)
        {
            _report.DisplayReport(cancellationToken);
        }

        public void PrintLabel(string printerName)
        {
            _report.PrintLabel(printerName);
        }

        public void Dispose()
        {
            _report?.Dispose();
            _report = null;
        }

        #endregion
    }
    public class OutsideProcessingLabelReport : ILabelReport
    {
        #region Fields

        private GenericWorkOrderLabelReport _report =
            new GenericWorkOrderLabelReport(LabelFactory.LabelType.OutsideProcessing);

        #endregion

        #region Properties

        public OrdersDataSet.OrderRow Order
        {
            get
            {
                return _report.Order;
            }
            set
            {
                _report.Order = value;
            }
        }

        #endregion

        #region Methods

        public OutsideProcessingLabelReport(OrdersDataSet.OrderRow order)
        {
            Order = order;
        }

        #endregion

        #region ILabelReport Members

        public void DisplayReport(CancellationToken cancellationToken)
        {
            _report.DisplayReport(cancellationToken);
        }

        public void PrintLabel(string printerName)
        {
            _report.PrintLabel(printerName);
        }

        public void Dispose()
        {
            _report?.Dispose();
            _report = null;
        }

        #endregion
    }

    internal class GenericWorkOrderLabelReport : ILabelReport
    {
        #region Properties

        public OrdersDataSet.OrderRow Order { get; set; }

        public LabelFactory.LabelType LabelType { get; private set; }

        #endregion

        #region Methods

        public GenericWorkOrderLabelReport(LabelFactory.LabelType labelType)
        {
            var labelTypeInvalid = labelType != LabelFactory.LabelType.WO &&
                labelType != LabelFactory.LabelType.OutsideProcessing &&
                labelType != LabelFactory.LabelType.ExternalRework;

            if (labelTypeInvalid)
            {
                throw new ArgumentException(nameof(labelType), "Invalid label type.");
            }

            LabelType = labelType;
        }

        #endregion

        public void DisplayReport(CancellationToken cancellationToken)
        {
            var labelData = GetData(Order);

            try
            {
                if (Order.OrderID >= 0)
                {
                    ReportNotifier.OnReportCreated(new ReportCreatedEventArgs(Order.OrderID, $"{LabelType} Label", SecurityManager.Current.UserName));
                }
                LabelFactory.PreviewLabel(labelData);
            }
            catch (LabelPrinterException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error printing work order label");
            }
        }

        public void PrintLabel(string printerName)
        {
            var labelData = GetData(Order);

            try
            {
                if (Order.OrderID >= 0)
                {
                    ReportNotifier.OnReportCreated(new ReportCreatedEventArgs(Order.OrderID, $"{LabelType} Label", SecurityManager.Current.UserName));
                }

                LabelFactory.PrintLabel(labelData, printerName);
            }
            catch (LabelPrinterException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error printing work order label");
            }
        }

        public void Dispose()
        {
            Order = null;
        }

        private LabelFactory.LabelData GetData(OrdersDataSet.OrderRow order)
        {
            var dtPart = new DWOS.Data.Datasets.PartsDataset.PartDataTable();
            var taParts = new DWOS.Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter();
            taParts.FillByPartID(dtPart, order.PartID);
            //var drPart = taParts.
            var drPart = dtPart.AsEnumerable()
                .SingleOrDefault(r => r.PartID == order.PartID);

            var tokens = new List<LabelFactory.TokenValue>();
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERNAME, order.CustomerSummaryRow.Name ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ORDERPRIORITY, order.IsPriorityNull() ? "NONE" : order.Priority ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.WORKORDER, order.OrderID.ToString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERWO, order.IsCustomerWONull() ? "NONE" : order.CustomerWO ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTNAME, dtPart.Count > 0 ? dtPart[0].Name : "Unknown" ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTDESCRIPTION, dtPart.Count > 0 && !dtPart[0].IsDescriptionNull() ? dtPart[0].Description : String.Empty));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTQUANTITY, order.IsPartQuantityNull() ? "Unknown" : order.PartQuantity.ToString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTREVISION, drPart.IsRevisionNull() ? " " : drPart.Revision.ToString()));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.USERNAME, SecurityManager.Current.UserName ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.DATE, DateTime.Now.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PURCHASEORDER, order.IsPurchaseOrderNull() ? "NONE" : order.PurchaseOrder ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.REQUIREDDATE, order.IsRequiredDateNull() ? "NONE" : order.RequiredDate.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ESTSHIPDATE, order.IsEstShipDateNull() ? "NONE" : order.EstShipDate.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CHECKINCOMMAND, Report.BARCODE_ORDER_CHECKIN_PREFFIX + order.OrderID.ToString() + Report.BARCODE_ORDER_CHECKIN_PREFFIX ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ADDORDERCOMMAND, string.Format("{0}{1}{0}", Report.BARCODE_ORDER_ACTION_PREFFIX, order.OrderID)));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSORDERCOMMAND, string.Format("{0}{1}{0}", Report.BARCODE_ORDER_PROCESS_PREFIX, order.OrderID)));

            // Serial Number
            var serialNumber = LabelReportUtilities.GetSerialNumber(order);

            if (serialNumber != null)
            {
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.SERIALNUMBER,
                    serialNumber.IsNumberNull() ? string.Empty : serialNumber.Number));
            }

            //customer address
            var customer = LabelReportUtilities.GetCustomer(order);

            if (customer != null)
            {
                tokens.AddRange(LabelReportUtilities.AddressTokens(customer));
            }

            var customerAddress = LabelReportUtilities.GetCustomerAddress(order);

            if (customerAddress != null)
            {
                tokens.AddRange(LabelReportUtilities.ShippingAddressTokens(customerAddress));
            }

            //Gross Weight
            var grossWeight = OrderUtilities.CalculateGrossWeight(order);
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.GROSSWEIGHT, Math.Round(grossWeight).ToString("N0") ));

            // Surface Area
            var surfaceArea = OrderUtilities.CalculateTotalSurfaceAreaInches(order.OrderID);
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.TOTALSURFACEAREA, string.Format("{0:N0} in²", Math.Round(surfaceArea))));

            //load any custom fields
            tokens.AddRange(LabelFactory.GetCustomTokensValuesByOrder(order.OrderID, order.CustomerID));

            //Fill processes
            var dsOP = new OrderProcessingDataSet { EnforceConstraints = false };

            using (var taProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                taProcesses.FillBy(dsOP.OrderProcesses, order.OrderID);
            using (var taProcesses = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessAliasTableAdapter())
                taProcesses.Fill(dsOP.ProcessAlias, order.OrderID);
            using (var taProcesses = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter())
                taProcesses.FillByOrder(dsOP.Process, order.OrderID);

            foreach (var orderProcess in dsOP.OrderProcesses)
            {
                int? fixtureCount = LabelReportUtilities.GetFixtureCount(orderProcess, order);

                decimal? weightPerFixture = null;

                if (fixtureCount.HasValue)
                {
                    weightPerFixture = LabelReportUtilities.GetFixtureWeight(order, fixtureCount.Value);
                }

                var fixtureCountString = fixtureCount.HasValue ?
                    fixtureCount.Value.ToString() :
                    "Unknown";

                var fixtureWeightString = weightPerFixture.HasValue ?
                    weightPerFixture.Value.ToString(LabelReportUtilities.FIXTURE_WEIGHT_FORMAT) :
                    "Unknown";

                switch (orderProcess.StepOrder)
                {
                    case 1:
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME1, orderProcess.ProcessRow.Name ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT1, orderProcess.Department ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES1, fixtureCountString));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT1, fixtureWeightString));
                        break;
                    case 2:
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME2, orderProcess.ProcessRow.Name ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT2, orderProcess.Department ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES2, fixtureCountString));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT2, fixtureWeightString));
                        break;
                    case 3:
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME3, orderProcess.ProcessRow.Name ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT3, orderProcess.Department ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES3, fixtureCountString));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT3, fixtureWeightString));
                        break;
                    case 4:
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME4, orderProcess.ProcessRow.Name ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT4, orderProcess.Department ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES4, fixtureCountString));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT4, fixtureWeightString));
                        break;
                    case 5:
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME5, orderProcess.ProcessRow.Name ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT5, orderProcess.Department ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES5, fixtureCountString));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT5, fixtureWeightString));
                        break;
                    case 6:
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME6, orderProcess.ProcessRow.Name ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT6, orderProcess.Department ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES6, fixtureCountString));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT6, fixtureWeightString));
                        break;
                    case 7:
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME7, orderProcess.ProcessRow.Name ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT7, orderProcess.Department ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES7, fixtureCountString));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT7, fixtureWeightString));
                        break;
                    case 8:
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSNAME8, orderProcess.ProcessRow.Name ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSDEPT8, orderProcess.Department ));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTURES8, fixtureCountString));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSFIXTUREWEIGHT8, fixtureWeightString));
                        break;
                }
            }

            try
            {
                return LabelFactory.LabelData.GetLabelForOrder(LabelType, order.OrderID, tokens) ??
                    LabelFactory.LabelData.CreateDefault();
            }
            finally
            {
                if (dtPart != null)
                    dtPart.Dispose();
                if (taParts != null)
                    taParts.Dispose();
            }
        }
    }

    public class ContainerLabelReport : ILabelReport
    {
        #region Fields

        private GenericContainerLabelReport _report = new GenericContainerLabelReport(LabelFactory.LabelType.Container);

        #endregion

        #region Properties

        public int OrderID
        {
            get
            {
                return _report.OrderID;
            }
            set
            {
                _report.OrderID = value;
            }
        }

        public OrdersDataSet.OrderRow Order
        {
            get
            {
                return _report.Order;
            }
            set
            {
                _report.Order = value;
            }
        }

        public OrdersDataSet.OrderContainersRow OrderContainer
        {
            get
            {
                return _report.OrderContainer;
            }
            set
            {
                _report.OrderContainer = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of containers to print in addition
        /// to ones that belong to <see cref="Order"/>.
        /// </summary>
        public int FakeContainerCount
        {
            get
            {
                return _report.FakeContainerCount;
            }
            set
            {
                _report.FakeContainerCount = value;
            }
        }

        #endregion

        #region ILabelReport Members

        public void DisplayReport(CancellationToken cancellationToken)
        {
            _report.DisplayReport(cancellationToken);
        }

        public void PrintLabel(string printerName)
        {
            _report.PrintLabel(printerName);
        }

        public void Dispose()
        {
            _report?.Dispose();
            _report = null;
        }

        #endregion
    }

    public class OutsideProcessingContainerLabelReport : ILabelReport
    {
        #region Fields

        private GenericContainerLabelReport _report = new GenericContainerLabelReport(LabelFactory.LabelType.OutsideProcessingContainer);

        #endregion

        #region Properties

        public int OrderID
        {
            get
            {
                return _report.OrderID;
            }
            set
            {
                _report.OrderID = value;
            }
        }

        public OrdersDataSet.OrderRow Order
        {
            get
            {
                return _report.Order;
            }
            set
            {
                _report.Order = value;
            }
        }

        public OrdersDataSet.OrderContainersRow OrderContainer
        {
            get
            {
                return _report.OrderContainer;
            }
            set
            {
                _report.OrderContainer = value;
            }
        }

        #endregion

        #region ILabelReport Members

        public void DisplayReport(CancellationToken cancellationToken)
        {
            _report.DisplayReport(cancellationToken);
        }

        public void PrintLabel(string printerName)
        {
            _report.PrintLabel(printerName);
        }

        public void Dispose()
        {
            _report?.Dispose();
            _report = null;
        }

        #endregion
    }
    public class ExternalReworkContainerLabelReport : ILabelReport
    {
        #region Fields

        private GenericContainerLabelReport _report = new GenericContainerLabelReport(LabelFactory.LabelType.ExternalReworkContainer);

        #endregion

        #region Properties

        public int OrderID
        {
            get
            {
                return _report.OrderID;
            }
            set
            {
                _report.OrderID = value;
            }
        }

        public OrdersDataSet.OrderRow Order
        {
            get
            {
                return _report.Order;
            }
            set
            {
                _report.Order = value;
            }
        }

        public OrdersDataSet.OrderContainersRow OrderContainer
        {
            get
            {
                return _report.OrderContainer;
            }
            set
            {
                _report.OrderContainer = value;
            }
        }

        #endregion

        #region ILabelReport Members

        public void DisplayReport(CancellationToken cancellationToken)
        {
            _report.DisplayReport(cancellationToken);
        }

        public void PrintLabel(string printerName)
        {
            _report.PrintLabel(printerName);
        }

        public void Dispose()
        {
            _report?.Dispose();
            _report = null;
        }

        #endregion
    }

    internal class GenericContainerLabelReport : ILabelReport
    {
        #region Properties

        public int OrderID { get; set; }

        public OrdersDataSet.OrderRow Order { get; set; }

        public OrdersDataSet.OrderContainersRow OrderContainer { get; set; }

        public LabelFactory.LabelType LabelType { get; private set; }

        public int FakeContainerCount { get; set; }

        #endregion

        #region Methods

        public GenericContainerLabelReport(LabelFactory.LabelType labelType)
        {
            var invalidLabelType = labelType != LabelFactory.LabelType.Container &&
                labelType != LabelFactory.LabelType.OutsideProcessingContainer &&
                labelType != LabelFactory.LabelType.ExternalReworkContainer;

            if (invalidLabelType)
            {
                throw new ArgumentException(nameof(labelType), "Invalid label type.");
            }

            LabelType = labelType;
        }

        #endregion

        public void DisplayReport(CancellationToken cancellationToken)
        {
            if (Order == null)
            {
                Order = GetOrder(OrderID);
            }

            foreach (var labelData in GetData(Order))
            {
                try
                {
                    LabelFactory.PreviewLabel(labelData);
                }
                catch (LabelPrinterException exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Error printing container label.");
                }
            }
        }

        public void PrintLabel(string printerName)
        {
            if (Order == null)
            {
                Order = GetOrder(OrderID);
            }

            foreach (var labelData in GetData(Order))
            {
                try
                {
                    LabelFactory.PrintLabel(labelData, printerName);
                }
                catch (LabelPrinterException exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Error printing container label.");
                }
            }
        }

        private static OrdersDataSet.OrderRow GetOrder(int orderID)
        {
            var dsOrders = new OrdersDataSet();
            dsOrders.EnforceConstraints = false;


            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.CustomerSummaryTableAdapter())
                ta.FillByOrder(dsOrders.CustomerSummary, orderID);

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.PartSummaryTableAdapter())
                ta.FillByOrder(dsOrders.PartSummary, orderID);

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter())
                ta.FillByOrderID(dsOrders.Order, orderID);

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderContainersTableAdapter())
                ta.FillByOrder(dsOrders.OrderContainers, orderID);

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderSerialNumberTableAdapter())
                ta.FillByOrder(dsOrders.OrderSerialNumber, orderID);

            return dsOrders.Order.FindByOrderID(orderID);
        }

        public void Dispose()
        {
            Order = null;
        }

        private IEnumerable<LabelFactory.LabelData> GetData(OrdersDataSet.OrderRow order)
        {
            var list = new List<LabelFactory.LabelData>();

            var tokens = new List<LabelFactory.TokenValue>();
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERNAME, order.CustomerSummaryRow.Name ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ORDERPRIORITY, order.IsPriorityNull() ? "NONE" : order.Priority ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.WORKORDER, order.OrderID.ToString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERWO, order.IsCustomerWONull() ? "NONE" : order.CustomerWO ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTNAME,  order.PartSummaryRow.Name ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTDESCRIPTION, LabelReportUtilities.GetPartDescription(order) ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTQUANTITY, order.IsPartQuantityNull() ? "Unknown" : order.PartQuantity.ToString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.USERNAME, SecurityManager.Current.UserName ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.DATE, DateTime.Now.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PURCHASEORDER, order.IsPurchaseOrderNull() ? "NONE" : order.PurchaseOrder ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.REQUIREDDATE, order.IsRequiredDateNull() ? "NONE" : order.RequiredDate.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ESTSHIPDATE, order.IsEstShipDateNull() ? "NONE" : order.EstShipDate.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CHECKINCOMMAND, Report.BARCODE_ORDER_CHECKIN_PREFFIX + order.OrderID.ToString() + Report.BARCODE_ORDER_CHECKIN_PREFFIX ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ADDORDERCOMMAND, string.Format("{0}{1}{0}", Report.BARCODE_ORDER_ACTION_PREFFIX, order.OrderID)));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PROCESSORDERCOMMAND, string.Format("{0}{1}{0}", Report.BARCODE_ORDER_PROCESS_PREFIX, order.OrderID)));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERCOUNT, order.GetOrderContainersRows().Count().ToString() ));

            // Serial Number
            var serialNumber = LabelReportUtilities.GetSerialNumber(order);

            if (serialNumber != null)
            {
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.SERIALNUMBER,
                    serialNumber.IsNumberNull() ? string.Empty : serialNumber.Number));
            }

            //customer address
            var customer = LabelReportUtilities.GetCustomer(order);

            if (customer != null)
            {
                tokens.AddRange(LabelReportUtilities.AddressTokens(customer));
            }

            //load any custom fields
            tokens.AddRange(LabelFactory.GetCustomTokensValuesByOrder(order.OrderID, order.CustomerID));
            
            //IF OrderContainer == null then print all containers, else just print the selected container.
            if(OrderContainer == null)
            {
                var containerNumber = 1;
                foreach(var container in order.GetOrderContainersRows().OrderBy(o => o.OrderContainerID))
                {
                    var localTokens = tokens.ConvertAll(t => t);
                    localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.GROSSWEIGHT, Math.Round(container.IsWeightNull() ? 0 : container.Weight).ToString("N0") ));
                    localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINER, container.OrderContainerID.ToString() ));
                    localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERQTY, container.PartQuantity.ToString() ));
                    localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERNUMBER, containerNumber.ToString() ));

                    list.Add(LabelFactory.LabelData.GetLabelForOrder(LabelType, order.OrderID, localTokens) ??
                        LabelFactory.LabelData.CreateDefault());
                    containerNumber++;
                }

                for (int fakeContainerID = 1; fakeContainerID <= FakeContainerCount; fakeContainerID++)
                {
                    const decimal defaultWeight = 0M;
                    const int defaultPartCount = 0;

                    var localTokens = tokens.ConvertAll(t => t);
                    localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.GROSSWEIGHT, defaultWeight.ToString("N0") ));
                    localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINER, fakeContainerID.ToString() ));
                    localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERQTY, defaultPartCount.ToString() ));
                    localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERNUMBER, containerNumber.ToString() ));

                    list.Add(LabelFactory.LabelData.GetLabelForOrder(LabelType, order.OrderID, localTokens) ??
                        LabelFactory.LabelData.CreateDefault());

                    containerNumber++;
                }
            }
            else
            {
                var containerNumber = 1 + order.GetOrderContainersRows()
                    .OrderBy(o => o.OrderContainerID)
                    .ToList()
                    .IndexOf(oc => oc.OrderContainerID == OrderContainer.OrderContainerID);

                var localTokens = new List<LabelFactory.TokenValue>(tokens);
                localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.GROSSWEIGHT, Math.Round(OrderContainer.IsWeightNull() ? 0 : OrderContainer.Weight).ToString("N0") ));
                localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINER, OrderContainer.OrderContainerID.ToString() ));
                localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERQTY, OrderContainer.PartQuantity.ToString() ));
                localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERNUMBER, containerNumber.ToString() ));

                list.Add(LabelFactory.LabelData.GetLabelForOrder(LabelFactory.LabelType.Container, order.OrderID, localTokens) ??
                    LabelFactory.LabelData.CreateDefault());
            }

            return list;
        }
    }

    public class COCLabelReport : ILabelReport
    {
        #region Properties

        public int OrderId { get; set; }

        public int? COCId { get; set; }

        private OrdersDataSet.OrderRow Order { get; set; }

        private OrdersDataSet.COCRow COC { get; set; }

        #endregion

        public void DisplayReport(CancellationToken cancellationToken)
        {
            LoadData();

            var labelData = GetData(Order);

            try
            {
                LabelFactory.PreviewLabel(labelData);
            }
            catch (LabelPrinterException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error printing COC container label.");
            }
        }

        public void PrintLabel(string printerName)
        {
            LoadData();

            var labelData = GetData(Order);

            try
            {
                LabelFactory.PrintLabel(labelData, printerName);
            }
            catch (LabelPrinterException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error printing COC container label.");
            }
        }

        private void LoadData()
        {
            var dsOrders = new OrdersDataSet() { EnforceConstraints = false };

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter())
            {
                ta.FillByOrderID(dsOrders.Order, this.OrderId);
                Order = dsOrders.Order.FirstOrDefault();
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.CustomerSummaryTableAdapter())
            {
                ta.FillByOrder(dsOrders.CustomerSummary, this.OrderId);
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.PartSummaryTableAdapter())
            {
                ta.FillByOrder(dsOrders.PartSummary, this.OrderId);
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderContainersTableAdapter())
            {
                ta.FillByOrder(dsOrders.OrderContainers, this.OrderId);
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderSerialNumberTableAdapter())
            {
                ta.FillByOrder(dsOrders.OrderSerialNumber, this.OrderId);
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.COCTableAdapter())
            {
                ta.FillByOrderNoData(dsOrders.COC, this.OrderId);

                if (COCId.HasValue)
                {
                    this.COC = dsOrders.COC.FirstOrDefault(i => i.COCID == COCId.Value);
                }
                else
                {
                    var sortedCOCs = dsOrders.COC.ToList();
                    sortedCOCs.Sort((t1, t2) => t1.DateCertified.CompareTo(t2.DateCertified));
                    this.COC = sortedCOCs.LastOrDefault();
                }
            }

            if(this.COC != null)
            {
                using(var ta = new Data.Datasets.OrdersDataSetTableAdapters.UserSummaryTableAdapter())
                {
                    ta.FillBy(dsOrders.UserSummary, COC.QAUser);
                }
            }
        }

        public void Dispose()
        {
            Order = null;
        }

        private LabelFactory.LabelData GetData(OrdersDataSet.OrderRow order)
        {
            var tokens = new List<LabelFactory.TokenValue>();
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERNAME, order.CustomerSummaryRow.Name ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ORDERPRIORITY, order.IsPriorityNull() ? "NONE" : order.Priority ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.WORKORDER, order.OrderID.ToString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERWO, order.IsCustomerWONull() ? "NONE" : order.CustomerWO ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTNAME, order.PartSummaryRow.Name ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTDESCRIPTION, LabelReportUtilities.GetPartDescription(order) ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTQUANTITY, order.IsPartQuantityNull() ? "Unknown" : order.PartQuantity.ToString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.USERNAME, this.COC == null || this.COC.UserSummaryRow == null ? SecurityManager.Current.UserName : this.COC.UserSummaryRow.Name));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.DATE, this.COC == null ? DateTime.Now.ToShortDateString() : this.COC.DateCertified.ToShortDateString()));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PURCHASEORDER, order.IsPurchaseOrderNull() ? "NONE" : order.PurchaseOrder ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.REQUIREDDATE, order.IsRequiredDateNull() ? "NONE" : order.RequiredDate.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ESTSHIPDATE, order.IsEstShipDateNull() ? "NONE" : order.EstShipDate.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ADDORDERCOMMAND, string.Format("{0}{1}{0}", Report.BARCODE_ORDER_ACTION_PREFFIX, order.OrderID)));

            // Serial Number
            var serialNumber = LabelReportUtilities.GetSerialNumber(order);

            if (serialNumber != null)
            {
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.SERIALNUMBER,
                    serialNumber.IsNumberNull() ? string.Empty : serialNumber.Number));
            }

            //customer address
            var customer = LabelReportUtilities.GetCustomer(order);

            if (customer != null)
            {
                tokens.AddRange(LabelReportUtilities.AddressTokens(customer));
            }

            //Gross Weight
            var grossWeight = OrderUtilities.CalculateGrossWeight(order);
            
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.GROSSWEIGHT, Math.Round(grossWeight).ToString("N0") ));

            // Surface Area
            var surfaceArea = OrderUtilities.CalculateTotalSurfaceAreaInches(order.OrderID);
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.TOTALSURFACEAREA, string.Format("{0:N0} in²", Math.Round(surfaceArea))));

            //load any custom fields
            tokens.AddRange(LabelFactory.GetCustomTokensValuesByOrder(order.OrderID, order.CustomerID));

            return LabelFactory.LabelData.GetLabelForOrder(LabelFactory.LabelType.COC, order.OrderID, tokens) ??
                LabelFactory.LabelData.CreateDefault();
        }
    }

    public class COCContainerLabelReport : ILabelReport
    {
        #region Properties

        public int OrderId { get; set; }

        public int? COCId { get; set; }

        private OrdersDataSet.OrderRow Order { get; set; }

        private OrdersDataSet.COCRow COC { get; set; }


        #endregion

        public void DisplayReport(CancellationToken cancellationToken)
        {
            LoadData();

            foreach (var labelData in GetData(Order))
            {
                try
                {
                    LabelFactory.PreviewLabel(labelData);
                }
                catch (LabelPrinterException exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Error printing COC container label.");
                }
            }
        }

        public void PrintLabel(string printerName)
        {
            LoadData();

            foreach (var labelData in GetData(Order))
            {
                try
                {
                    LabelFactory.PrintLabel(labelData, printerName);
                }
                catch (LabelPrinterException exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Error printing COC container label.");
                }
            }
        }

        private void LoadData()
        {
            var dsOrders = new OrdersDataSet() { EnforceConstraints = false };

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter())
            {
                ta.FillByOrderID(dsOrders.Order, this.OrderId);
                Order = dsOrders.Order.FirstOrDefault();
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.CustomerSummaryTableAdapter())
            {
                ta.FillByOrder(dsOrders.CustomerSummary, this.OrderId);
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.PartSummaryTableAdapter())
            {
                ta.FillByOrder(dsOrders.PartSummary, this.OrderId);
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderContainersTableAdapter())
            {
                ta.FillByOrder(dsOrders.OrderContainers, this.OrderId);
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderSerialNumberTableAdapter())
            {
                ta.FillByOrder(dsOrders.OrderSerialNumber, this.OrderId);
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.COCTableAdapter())
            {
                ta.FillByOrderNoData(dsOrders.COC, this.OrderId);

                if (this.COCId.HasValue)
                {
                    this.COC = dsOrders.COC.FirstOrDefault(i => i.COCID == COCId.Value);
                }
                else
                {
                    var sortedCOCs = dsOrders.COC.ToList();
                    sortedCOCs.Sort((t1, t2) => t1.DateCertified.CompareTo(t2.DateCertified));
                    this.COC = sortedCOCs.LastOrDefault();
                }
            }

            if(this.COC != null)
            {
                using(var ta = new Data.Datasets.OrdersDataSetTableAdapters.UserSummaryTableAdapter())
                {
                    ta.FillBy(dsOrders.UserSummary, COC.QAUser);
                }
            }
        }

        public void Dispose()
        {
            Order = null;
        }

        private IEnumerable<LabelFactory.LabelData> GetData(OrdersDataSet.OrderRow order)
        {
            var labels = new List<LabelFactory.LabelData>();

            var tokens = new List<LabelFactory.TokenValue>();
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERNAME, order.CustomerSummaryRow.Name ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ORDERPRIORITY, order.IsPriorityNull() ? "NONE" : order.Priority ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.WORKORDER, order.OrderID.ToString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERWO, order.IsCustomerWONull() ? "NONE" : order.CustomerWO ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTNAME, order.PartSummaryRow.Name ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTDESCRIPTION, LabelReportUtilities.GetPartDescription(order)));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTQUANTITY, order.IsPartQuantityNull() ? "Unknown" : order.PartQuantity.ToString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.USERNAME, this.COC == null || this.COC.UserSummaryRow == null ? SecurityManager.Current.UserName : this.COC.UserSummaryRow.Name));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.DATE, this.COC == null ? DateTime.Now.ToShortDateString() : this.COC.DateCertified.ToShortDateString()));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PURCHASEORDER, order.IsPurchaseOrderNull() ? "NONE" : order.PurchaseOrder ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.REQUIREDDATE, order.IsRequiredDateNull() ? "NONE" : order.RequiredDate.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ESTSHIPDATE, order.IsEstShipDateNull() ? "NONE" : order.EstShipDate.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERCOUNT, order.GetOrderContainersRows().Count().ToString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ADDORDERCOMMAND, string.Format("{0}{1}{0}", Report.BARCODE_ORDER_ACTION_PREFFIX, order.OrderID)));

            // Serial Number
            var serialNumber = LabelReportUtilities.GetSerialNumber(order);

            if (serialNumber != null)
            {
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.SERIALNUMBER,
                    serialNumber.IsNumberNull() ? string.Empty : serialNumber.Number));
            }

            //customer address
            var customer = LabelReportUtilities.GetCustomer(order);

            if (customer != null)
            {
                tokens.AddRange(LabelReportUtilities.AddressTokens(customer));
            }
            
            //Gross Weight
            var partWeight = 0M;
            using (var ta = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
            {
                var parts = ta.GetByOrder(order.OrderID);
                var part = parts.FindByPartID(order.PartID);

                if (part != null && !part.IsWeightNull())
                    partWeight = part.Weight;
            }

            //load any custom fields
            tokens.AddRange(LabelFactory.GetCustomTokensValuesByOrder(order.OrderID, order.CustomerID));

            var containerNumber = 1;
            foreach (var container in order.GetOrderContainersRows().OrderBy(o => o.OrderContainerID))
            {
                var localTokens = tokens.ConvertAll(t => t);
                localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINER, container.OrderContainerID.ToString() ));
                localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERQTY, container.PartQuantity.ToString() ));
                localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERNUMBER, containerNumber.ToString() ));
                
                var weight = container.IsWeightNull() ? 0 : container.Weight;
                localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.GROSSWEIGHT, Math.Round(weight).ToString("N0") ));

                labels.Add(LabelFactory.LabelData.GetLabelForOrder(LabelFactory.LabelType.COCContainer, order.OrderID, localTokens) ??
                    LabelFactory.LabelData.CreateDefault());

                containerNumber++;
            }

            return labels;
        }
    }

    public class ReceivingContainerLabelReport : ILabelReport
    {
        #region Properties

        public PartsDataset.ReceivingSummaryRow ReceivingRow { get; set; }


        #endregion

        public void DisplayReport(CancellationToken cancellationToken)
        {
                var labelData = GetData(ReceivingRow,1);
                try
                {
                    LabelFactory.PreviewLabel(labelData);
                }
                catch (LabelPrinterException exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Error printing COC container label.");
                }

        }

        public void PrintLabel(string printerName)
        {
            var PageCount = ReceivingRow.Containers;
            if (PageCount > 0)
                for (int i = 1; i <= PageCount; i++)
                {
                    var labelData = GetData(ReceivingRow,i);

                    try
                    {
                        LabelFactory.PrintLabel(labelData, printerName);
                    }
                    catch (LabelPrinterException exc)
                    {
                        LogManager.GetCurrentClassLogger().Warn(exc, "Error printing COC container label.");
                    }
                }
        }

        private void LoadData()
        {

        }

        public void Dispose()
        {
            //Order = null;
        }

        private LabelFactory.LabelData GetData (PartsDataset.ReceivingSummaryRow receivingRow, int ContainerNo)
        {
            //get part and customer info.


            var tokens = new List<LabelFactory.TokenValue>();
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERNAME, receivingRow.CustomerName.ToString()));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERADDRESS1, receivingRow.Address1.ToString()));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERCITYSTATEZIP, (receivingRow.City.ToString()+" "+ receivingRow.State+", "+ receivingRow.Zip.ToString())));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTNAME, receivingRow.PartName.ToString()));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ORDERPRIORITY, receivingRow.Priority));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERWO, receivingRow.IsCustomerWONull() ? "NONE" : receivingRow.CustomerWO));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTQUANTITY, receivingRow.PartQuantity.ToString()));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.DATE, DateTime.Now.ToShortDateString()));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PURCHASEORDER, receivingRow.IsPurchaseOrderNull() ? "NONE" : receivingRow.PurchaseOrder));
            try
            {
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.REQUIREDDATE, receivingRow.ReqDate.ToShortDateString()));
            }
            catch (Exception)
            {
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.REQUIREDDATE, ""));
            }

            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERCOUNT, receivingRow.Containers.ToString()));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERNUMBER, ContainerNo.ToString()));
            //tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ADDORDERCOMMAND, string.Format("{0}{1}{0}", Report.BARCODE_ORDER_ACTION_PREFFIX, receivingRow.OrderID)));


            return LabelFactory.LabelData.GetLabelForCustomer(LabelFactory.LabelType.ReceivingContainer,receivingRow.CustomerID, tokens) ??
                LabelFactory.LabelData.CreateDefault();
        }
    }

    public class PartRackingLabelReport : ILabelReport
    {
        #region Properties

        public int LabelCount 
        {
            set { _labelCount = value; } 
        }
        public OrdersDataSet.OrderSummaryRow Order { get; set; }

        private int _labelCount = 1;

        #endregion

        public void DisplayReport(CancellationToken cancellationToken)
        {
            var labelData = GetData(Order, _labelCount);
            try
            {
                LabelFactory.PreviewLabel(labelData);
            }
            catch (LabelPrinterException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error displaying Racking label.");
            }
        }

        public void PrintLabel(string printerName)
        {

            for (int i = 0; i < _labelCount; i++)
            {
                var labelData = GetData(Order, i);
                try
                {
                    LabelFactory.PrintLabel(labelData, printerName);
                }
                catch (LabelPrinterException exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Error printing Racking label.");
                }
            }
        }

        public void Dispose()
        {
            //Order = null;
        }

        private LabelFactory.LabelData GetData(OrdersDataSet.OrderSummaryRow orderRow, int ContainerNo)
        {

            try
            {
                var tokens = new List<LabelFactory.TokenValue>();
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.WORKORDER, orderRow.OrderID.ToString())); 
            
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PURCHASEORDER, orderRow.IsPurchaseOrderNull() ? "NONE" : orderRow.PurchaseOrder));
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERNAME, orderRow.CustomerName.ToString()));
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTNAME, orderRow.PartName.ToString()));
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTDESCRIPTION, (orderRow.IsPartDescNull()) ?"":orderRow.PartDesc.ToString()));
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ORDERPRIORITY, orderRow.Priority));
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERWO, orderRow.IsCustomerWONull() ? "NONE" : orderRow.CustomerWO));
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTQUANTITY, orderRow.PartQuantity.ToString()));
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.DATE, DateTime.Now.ToShortDateString()));
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERNUMBER, ContainerNo.ToString()));
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ADDORDERCOMMAND, string.Format("{0}{1}{0}", Report.BARCODE_ORDER_ACTION_PREFFIX, orderRow.OrderID)));
                //Get the current process from OrderProcesses
                using (var daOrderProcess = new Data.Datasets.OrdersDataSetTableAdapters.OrderProcessesTableAdapter())
                {
                    Data.Datasets.OrdersDataSet.OrderProcessesRow drOrderProcesses = (Data.Datasets.OrdersDataSet.OrderProcessesRow)daOrderProcess.GetCurrentProcess(orderRow.OrderID).Rows[0];
                    using (var daProcessAlias = new Data.Datasets.OrdersDataSetTableAdapters.ProcessAliasSummaryTableAdapter())
                    {
                        Data.Datasets.OrdersDataSet.ProcessAliasSummaryDataTable dtProcessAliasSummary = new OrdersDataSet.ProcessAliasSummaryDataTable();
                        daProcessAlias.FillByProcessId(dtProcessAliasSummary, drOrderProcesses.ProcessID); 
                        Data.Datasets.OrdersDataSet.ProcessAliasSummaryRow drProcessAlias = (Data.Datasets.OrdersDataSet.ProcessAliasSummaryRow)dtProcessAliasSummary.Rows[0];
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CURRENTPROCESS,drProcessAlias.ProcessName));
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CURRENTPROCESSALIAS, drProcessAlias.Name));
                    }
                }
                return LabelFactory.LabelData.GetLabelForCustomer(LabelFactory.LabelType.Racking, orderRow.CustomerID, tokens) ??
                LabelFactory.LabelData.CreateDefault();
            }
            catch (LabelPrinterException exc)
            {

                LogManager.GetCurrentClassLogger().Warn(exc, "Error Getting Order label Data.");
                return null;
            }
           
        }
    }

    public class ReworkContainerLabelReport : ILabelReport
    {
        #region Fields

        private GenericReworkContainerLabelReport _report;

        #endregion

        #region Properties

        public int OrderID
        {
            get
            {
                return _report.OrderID;
            }
        }

        #endregion

        #region Methods

        public ReworkContainerLabelReport(int orderID)
        {
            _report = new GenericReworkContainerLabelReport(LabelFactory.LabelType.ReworkContainer, orderID);
        }

        #endregion

        #region ILabelReport Members

        public void DisplayReport(CancellationToken cancellationToken)
        {
            _report.DisplayReport(cancellationToken);
        }

        public void PrintLabel(string printerName)
        {
            _report.PrintLabel(printerName);
        }

        public void Dispose()
        {
            _report?.Dispose();
            _report = null;
        }

        #endregion
    }

    public class HoldContainerLabelReport : ILabelReport
    {
        #region Fields

        private GenericReworkContainerLabelReport _report;

        #endregion

        #region Properties

        public int OrderID
        {
            get
            {
                return _report.OrderID;
            }
        }

        #endregion

        #region Methods

        public HoldContainerLabelReport(int orderID)
        {
            _report = new GenericReworkContainerLabelReport(LabelFactory.LabelType.HoldContainer, orderID);
        }

        #endregion

        #region ILabelReport Members

        public void DisplayReport(CancellationToken cancellationToken)
        {
            _report.DisplayReport(cancellationToken);
        }

        public void PrintLabel(string printerName)
        {
            _report.PrintLabel(printerName);
        }

        public void Dispose()
        {
            _report?.Dispose();
            _report = null;
        }

        #endregion
    }
    public class OutsideReworkContainerLabelReport : ILabelReport
    {
        #region Fields

        private GenericReworkContainerLabelReport _report;

        #endregion

        #region Properties

        public int OrderID
        {
            get
            {
                return _report.OrderID;
            }
        }

        #endregion

        #region Methods

        public OutsideReworkContainerLabelReport(int orderID)
        {
            _report = new GenericReworkContainerLabelReport(LabelFactory.LabelType.OutsideProcessingReworkContainer, orderID);
        }

        #endregion

        #region ILabelReport Members

        public void DisplayReport(CancellationToken cancellationToken)
        {
            _report.DisplayReport();
        }

        public void PrintLabel(string printerName)
        {
            _report.PrintLabel(printerName);
        }

        public void Dispose()
        {
            _report?.Dispose();
            _report = null;
        }

        #endregion
    }

    internal class GenericReworkContainerLabelReport : ILabelReport
    {
        #region Properties

        public int OrderID { get; private set; }

        public LabelFactory.LabelType LabelType
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public GenericReworkContainerLabelReport(LabelFactory.LabelType labelType, int orderID)
        {
            var labelTypeInvalid = labelType != LabelFactory.LabelType.ReworkContainer &&
                labelType != LabelFactory.LabelType.OutsideProcessingReworkContainer &&
                labelType != LabelFactory.LabelType.HoldContainer;

            if (labelTypeInvalid)
            {
                throw new ArgumentException(nameof(labelType), "Invalid label type.");
            }

            OrderID = orderID;
            LabelType = labelType;
        }

        private OrdersDataSet.OrderRow LoadOrder()
        {
            var dsOrders = new OrdersDataSet() { EnforceConstraints = false };
            OrdersDataSet.OrderRow order;
            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter())
            {
                ta.FillByOrderID(dsOrders.Order, OrderID);
                order = dsOrders.Order.FirstOrDefault();
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.CustomerSummaryTableAdapter())
            {
                ta.FillByOrder(dsOrders.CustomerSummary, OrderID);
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.PartSummaryTableAdapter())
            {
                ta.FillByOrder(dsOrders.PartSummary, OrderID);
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderContainersTableAdapter())
            {
                ta.FillByOrder(dsOrders.OrderContainers, OrderID);
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderSerialNumberTableAdapter())
            {
                ta.FillByOrder(dsOrders.OrderSerialNumber, OrderID);
            }

            return order;
        }

        private IEnumerable<LabelFactory.LabelData> GetData(OrdersDataSet.OrderRow order)
        {
            var labels = new List<LabelFactory.LabelData>();

            var tokens = new List<LabelFactory.TokenValue>();
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERNAME, order.CustomerSummaryRow.Name ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ORDERPRIORITY, order.IsPriorityNull() ? "NONE" : order.Priority ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.WORKORDER, order.OrderID.ToString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERWO, order.IsCustomerWONull() ? "NONE" : order.CustomerWO ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTNAME, order.PartSummaryRow.Name ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTDESCRIPTION, LabelReportUtilities.GetPartDescription(order)));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTQUANTITY, order.IsPartQuantityNull() ? "Unknown" : order.PartQuantity.ToString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.USERNAME, SecurityManager.Current.UserName ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.DATE, DateTime.Now.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PURCHASEORDER, order.IsPurchaseOrderNull() ? "NONE" : order.PurchaseOrder ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.REQUIREDDATE, order.IsRequiredDateNull() ? "NONE" : order.RequiredDate.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ESTSHIPDATE, order.IsEstShipDateNull() ? "NONE" : order.EstShipDate.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERCOUNT, order.GetOrderContainersRows().Count().ToString() ));

            // Serial Number
            var serialNumber = LabelReportUtilities.GetSerialNumber(order);

            if (serialNumber != null)
            {
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.SERIALNUMBER,
                    serialNumber.IsNumberNull() ? string.Empty : serialNumber.Number));
            }

            //customer address
            var customer = LabelReportUtilities.GetCustomer(order);

            if (customer != null)
            {
                tokens.AddRange(LabelReportUtilities.AddressTokens(customer));
            }

            //Gross Weight
            var partWeight = 0M;
            using (var ta = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
            {
                var parts = ta.GetByOrder(order.OrderID);
                var part = parts.FindByPartID(order.PartID);

                if (part != null && !part.IsWeightNull())
                    partWeight = part.Weight;
            }

            //load any custom fields
            tokens.AddRange(LabelFactory.GetCustomTokensValuesByOrder(order.OrderID, order.CustomerID));

            //Fill processes
            var dsOP = new OrderProcessingDataSet { EnforceConstraints = false };

            if (order.WorkStatus == "Pending Rework Planning")
            {
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.REWORKPENDING, "** Pending Rework Planning **" ));
            }
            else if (order.OrderType == 5 && order.Hold) //Rework HOLD
            {
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.REWORKPENDING, "** Pending Rework Join **" ));
            }
            else
            {
                using (var taProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                    taProcesses.FillBy(dsOP.OrderProcesses, order.OrderID);
                using (var taProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessAliasTableAdapter())
                    taProcesses.Fill(dsOP.ProcessAlias, order.OrderID);
                using (var taProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter())
                    taProcesses.FillByOrder(dsOP.Process, order.OrderID);

                foreach (var orderProcess in dsOP.OrderProcesses)
                {
                    tokens.AddRange(LabelReportUtilities.ReworkTokensForProcess(orderProcess, order));
                }
            }

            int containerNumber = 1;
            foreach (var container in order.GetOrderContainersRows().OrderBy(o => o.OrderContainerID))
            {
                var localTokens = tokens.ConvertAll(t => t);
                localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINER, container.OrderContainerID.ToString()));
                localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERQTY, container.PartQuantity.ToString()));
                localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CONTAINERNUMBER, containerNumber.ToString()));

                var weight = container.IsWeightNull() ? 0 : container.Weight;
                localTokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.GROSSWEIGHT, Math.Round(weight).ToString("N0")));

                labels.Add(LabelFactory.LabelData.GetLabelForOrder(LabelType, order.OrderID, localTokens) ??
                    LabelFactory.LabelData.CreateDefault());

                containerNumber++;
            }

            return labels;
        }

        #endregion

        #region ILabelReport Members

        public void DisplayReport(CancellationToken cancellationToken)
        {
            var order = LoadOrder();

            foreach (var labelData in GetData(order))
            {
                try
                {
                    LabelFactory.PreviewLabel(labelData);
                }
                catch (LabelPrinterException exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Error printing COC container label.");
                }
            }
        }

        public void PrintLabel(string printerName)
        {
            var order = LoadOrder();

            foreach (var labelData in GetData(order))
            {
                try
                {
                    LabelFactory.PrintLabel(labelData, printerName);
                }
                catch (LabelPrinterException exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Error printing COC container label.");
                }
            }
        }

        public void Dispose()
        {
        }

        #endregion
    }

    public class SalesOrderWorkOrderLabelsReport : ILabelReport
    {
        #region Fields

        private readonly OrdersDataSet.SalesOrderRow _salesOrder;

        #endregion

        #region Methods

        public SalesOrderWorkOrderLabelsReport(OrdersDataSet.SalesOrderRow salesOrder) { _salesOrder = salesOrder; }

        public void Dispose()
        {
        }

        public void DisplayReport(CancellationToken cancellationToken)
        {
            foreach (var order in _salesOrder.GetOrderRows())
            {
                var report = new WorkOrderLabelReport() { Order = order };
                report.DisplayReport(cancellationToken);
            }
        }

        public void PrintLabel(string printerName)
        {
            foreach (var order in _salesOrder.GetOrderRows())
            {
                var report = new WorkOrderLabelReport() { Order = order };
                report.PrintLabel(printerName);
            }
        }

        #endregion Methods
    }

    /// <summary>
    /// Report that contains the Work Order Traveler for each order
    /// in a sales order.
    /// </summary>
    /// <remarks>
    /// Does not actually print labels.
    /// </remarks>
    public class SalesOrderWorkOrderTravelersReport : ILabelReport
    {
        #region Fields

        private readonly OrdersDataSet.SalesOrderRow _salesOrder;

        #endregion

        #region Methods

        public SalesOrderWorkOrderTravelersReport(OrdersDataSet.SalesOrderRow salesOrder) { _salesOrder = salesOrder; }

        public void Dispose()
        {
        }

        public void DisplayReport(CancellationToken cancellationToken)
        {
            foreach (var order in _salesOrder.GetOrderRows())
            {
                var report = new WorkOrderTravelerReport(order);
                report.DisplayReport(cancellationToken);
            }
        }

        public void PrintLabel(string printerName)
        {
            foreach (var order in _salesOrder.GetOrderRows())
            {
                var report = new WorkOrderTravelerReport(order);
                report.PrintReport();
            }
        }

        #endregion Methods
    }

    public class RepairsStatementsReport : ILabelReport
    {
        #region Fields

        private RepairStatementData _reportData;

        #endregion

        #region Methods

        public RepairsStatementsReport(RepairStatementData reportData)
        {
            if (reportData == null)
            {
                throw new ArgumentNullException(nameof(reportData));
            }

            _reportData = reportData;
        }

        #endregion

        #region ILabelReport Members

        public void DisplayReport(CancellationToken cancellationToken)
        {
            foreach (var orderDate in _reportData.ReceivedDates)
            {
                var report = new RepairStatementReport(_reportData, orderDate);
                report.DisplayReport();
            }
        }

        public void PrintLabel(string printerName)
        {
            foreach (var orderDate in _reportData.ReceivedDates)
            {
                var report = new RepairStatementReport(_reportData, orderDate);
                report.PrintReport();
            }
        }

        public void Dispose()
        {
        }

        #endregion
    }

    internal class ReworkLabelReport : ILabelReport
    {
        #region Properties

        public OrdersDataSet.OrderRow Order { get; set; }

        public ReportLabelType Type { get; }

        private LabelFactory.LabelType LabelType
        {
            get
            {
                switch (Type)
                {
                    case ReportLabelType.Rework:
                        return LabelFactory.LabelType.Rework;
                    case ReportLabelType.Hold:
                        return LabelFactory.LabelType.Hold;
                    case ReportLabelType.OutsideProcessingRework:
                        return LabelFactory.LabelType.OutsideProcessingRework;
                    default:
                        throw new InvalidOperationException("Incorrect label type");
                }
            }
        }

        #endregion

        public ReworkLabelReport(OrdersDataSet.OrderRow order, ReportLabelType type)
        {
            Order = order;
            Type = type;
        }

        public void DisplayReport(CancellationToken cancellationToken)
        {
            var labelData = GetData(Order);

            try
            {
                LabelFactory.PreviewLabel(labelData);
            }
            catch (LabelPrinterException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error printing rework label.");
            }
        }

        public void PrintLabel(string printerName)
        {
            var labelData = GetData(Order);

            try
            {
                LabelFactory.PrintLabel(labelData, printerName);
            }
            catch (LabelPrinterException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error printing rework label.");
            }
        }

        public void Dispose()
        {
            Order = null;
        }

        private LabelFactory.LabelData GetData(OrdersDataSet.OrderRow order)
        {
            var dtPart = new DWOS.Data.Datasets.PartsDataset.PartDataTable();
            var taParts = new DWOS.Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter();
            taParts.FillByPartID(dtPart, order.PartID);

            var tokens = new List<LabelFactory.TokenValue>();
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ORDERPRIORITY, order.IsPriorityNull() ? "NA" : order.Priority ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.WORKORDER, order.OrderID.ToString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERWO, order.IsCustomerWONull() ? "NONE" : order.CustomerWO ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PURCHASEORDER, order.IsPurchaseOrderNull() ? "NONE" : order.PurchaseOrder ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTNAME, dtPart.Count > 0 ? dtPart[0].Name : "Unknown" ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTDESCRIPTION, LabelReportUtilities.GetPartDescription(order)));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.PARTQUANTITY, order.IsPartQuantityNull() ? "Unknown" : order.PartQuantity.ToString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.USERNAME, SecurityManager.Current.UserName ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.DATE, DateTime.Now.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.REQUIREDDATE, order.IsRequiredDateNull() ? "NONE" : order.RequiredDate.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.ESTSHIPDATE, order.IsEstShipDateNull() ? "NONE" : order.EstShipDate.ToShortDateString() ));
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CHECKINCOMMAND, Report.BARCODE_ORDER_CHECKIN_PREFFIX + order.OrderID.ToString() + Report.BARCODE_ORDER_CHECKIN_PREFFIX ));

            // Serial Number
            var serialNumber = LabelReportUtilities.GetSerialNumber(order);

            if (serialNumber != null)
            {
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.SERIALNUMBER,
                    serialNumber.IsNumberNull() ? string.Empty : serialNumber.Number));
            }

            //customer name
            var customer = LabelReportUtilities.GetCustomer(order);

            if (customer != null)
            {
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.CUSTOMERNAME, customer.Name ));
            }

            //Gross Weight
            var grossWeight = OrderUtilities.CalculateGrossWeight(order);

            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.GROSSWEIGHT, Math.Round(grossWeight).ToString("N0") ));

            // Surface Area
            var surfaceArea = OrderUtilities.CalculateTotalSurfaceAreaInches(order.OrderID);
            tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.TOTALSURFACEAREA, string.Format("{0:N0} in²", Math.Round(surfaceArea))));

            //load any custom fields
            tokens.AddRange(LabelFactory.GetCustomTokensValuesByOrder(order.OrderID, order.CustomerID));

            //Fill processes
            var dsOP = new OrderProcessingDataSet { EnforceConstraints = false };


            if (Order.WorkStatus == "Pending Rework Planning")
            {
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.REWORKPENDING, "** Pending Rework Planning **" ));
            }
            else if (Order.OrderType == 5 && Order.Hold) //Rework HOLD
            {
                tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.REWORKPENDING, "** Pending Rework Join **" ));
            }
            else
            {
                using (var taProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                    taProcesses.FillBy(dsOP.OrderProcesses, order.OrderID);
                using (var taProcesses = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessAliasTableAdapter())
                    taProcesses.Fill(dsOP.ProcessAlias, order.OrderID);
                using (var taProcesses = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter())
                    taProcesses.FillByOrder(dsOP.Process, order.OrderID);

                foreach (var orderProcess in dsOP.OrderProcesses)
                {
                    tokens.AddRange(LabelReportUtilities.ReworkTokensForProcess(orderProcess, order));
                }
            }

            var customerID = order.CustomerID;

            try
            {
                return LabelFactory.LabelData.GetLabelForOrder(LabelType, order.OrderID, tokens) ??
                    LabelFactory.LabelData.CreateDefault();
            }
            finally
            {
                if (dtPart != null)
                    dtPart.Dispose();
                if (taParts != null)
                    taParts.Dispose();
            }
        }

        #region ReportLabelType

        public enum ReportLabelType
        {
            Rework,
            Hold,
            OutsideProcessingRework
        }

        #endregion
    }

    public class ShippingPackageLabel : ILabelReport
    {
        #region Properties

        public OrderShipmentDataSet.ShipmentPackageRow Package
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public ShippingPackageLabel(OrderShipmentDataSet.ShipmentPackageRow package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            Package = package;
        }

        private static LabelFactory.LabelData GetData(OrderShipmentDataSet.ShipmentPackageRow package)
        {
            var tokens = new List<LabelFactory.TokenValue>();
            tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.CUSTOMERNAME.ToString(), Value = package.CustomerRow.Name });
            tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.USERNAME.ToString(), Value = SecurityManager.Current.UserName });
            tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.DATE.ToString(), Value = DateTime.Now.ToShortDateString() });
            tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.SHIPPINGCARRIER.ToString(), Value = package.IsShippingCarrierIDNull() ? string.Empty : package.ShippingCarrierID });
            tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.BOXNUMBER.ToString(), Value = package.PackageNumber.ToString() });
            tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.SHIPMENTID.ToString(), Value = package.ShipmentPackageID.ToString() });
            tokens.AddRange(LabelReportUtilities.AddressTokens(package.CustomerRow));
            tokens.AddRange(LabelReportUtilities.ShippingAddressTokens(package.CustomerAddressRow));

            return LabelFactory.LabelData.GetLabelForCustomer(LabelFactory.LabelType.ShippingPackage, package.CustomerID, tokens) ??
                LabelFactory.LabelData.CreateDefault();
        }

        #endregion

        #region ILabelReport Members

        public void DisplayReport(CancellationToken cancellationToken)
        {
            var labelData = GetData(Package);

            try
            {
                LabelFactory.PreviewLabel(labelData);
            }
            catch (LabelPrinterException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error printing work order label");
            }
        }

        public void PrintLabel(string printerName)
        {
            var labelData = GetData(Package);

            try
            {
                LabelFactory.PrintLabel(labelData, printerName);
            }
            catch (LabelPrinterException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error printing work order label");
            }
        }

        public void Dispose()
        {
        }

        #endregion
    }

    public class ShippingOrderLabel : ILabelReport
    {
        #region Properties

        public int OrderId
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public ShippingOrderLabel(int orderId)
        {
            OrderId = orderId;
        }

        private LabelFactory.LabelData GetData(int orderId)
        {
            const string none = "None";

            using (var taOrders = new Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
            {
                using (OrdersReport.OrderDataTable orders = taOrders.GetByOrder(orderId))
                {
                    if (orders.Count != 1)
                    {
                        return null;
                    }

                    var order = orders[0];
                    var customerID = taOrders.GetCustomerId(order.OrderID).GetValueOrDefault();

                    var tokens = new List<LabelFactory.TokenValue>();
                    tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.CUSTOMERNAME.ToString(), Value = order.CustomerName });
                    tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.ORDERPRIORITY.ToString(), Value = order.IsPriorityNull() ? none : order.Priority });
                    tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.WORKORDER.ToString(), Value = orderId.ToString() });
                    tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.CUSTOMERWO.ToString(), Value = order.IsCustomerWONull() ? none : order.CustomerWO });
                    tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.PARTNAME.ToString(), Value = order.PartName });
                    tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.PARTDESCRIPTION.ToString(), Value = LabelReportUtilities.GetPartDescription(order) });
                    tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.PURCHASEORDER.ToString(), Value = order.IsPurchaseOrderNull() ? none : order.PurchaseOrder });
                    tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.PARTQUANTITY.ToString(), Value = order.PartQuantity.ToString() });
                    tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.USERNAME.ToString(), Value = SecurityManager.Current.UserName });
                    tokens.Add(new LabelFactory.TokenValue() { TokenName = LabelFactory.LabelTokens.DATE.ToString(), Value = DateTime.Now.ToShortDateString() });

                    // Serial Number
                    var serialNumber = LabelReportUtilities.GetSerialNumber(order);

                    if (serialNumber != null)
                    {
                        tokens.Add(LabelFactory.TokenValue.From(LabelFactory.LabelTokens.SERIALNUMBER,
                            serialNumber.IsNumberNull() ? string.Empty : serialNumber.Number));
                    }

                    //customer address
                    var customer = LabelReportUtilities.GetCustomer(order);

                    if (customer != null)
                    {
                        tokens.AddRange(LabelReportUtilities.AddressTokens(customer));
                    }

                    //Part Revision
                    tokens.Add(LabelReportUtilities.PartRevision(order.PartID));

                    //Package ID
                    tokens.Add(LabelReportUtilities.PackageID(order.OrderID));

                    //add custom field values for this order
                    tokens.AddRange(LabelFactory.GetCustomTokensValuesByOrder(order.OrderID, customerID));

                    return LabelFactory.LabelData.GetLabelForOrder(LabelFactory.LabelType.ShippingOrder, orderId, tokens) ??
                        LabelFactory.LabelData.CreateDefault();
                }
            }
        }

        #endregion

        #region ILabelReport Members

        public void PrintLabel(string printerName)
        {
            var labelData = GetData(OrderId);

            try
            {
                LabelFactory.PrintLabel(labelData, printerName);
            }
            catch (LabelPrinterException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error printing work order label");
            }
        }

        public void DisplayReport(CancellationToken cancellationToken)
        {
            var labelData = GetData(OrderId);

            try
            {
                LabelFactory.PreviewLabel(labelData);
            }
            catch (LabelPrinterException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error printing work order label");
            }
        }

        public void Dispose()
        {
        }

        #endregion
    }
}