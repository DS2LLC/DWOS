using DWOS.Data.Datasets;
using DWOS.Reports;
using NLog;
using System;

namespace DWOS.UI.Reports
{
    /// <summary>
    /// Allows use of <see cref="ReworkLabelReport"/> with an Order ID
    /// instead of a <see cref="OrdersDataSet.OrderRow"/> with pre-loaded
    /// dependencies.
    /// </summary>
    internal class ReworkLabelReportWrapper : ReportWrapper
    {
        #region Fields

        private readonly Lazy<ReworkLabelReport> _lazyReport;
        private readonly OrdersDataSet _dsOrders;

        #endregion

        #region Properties

        public override IReport InnerReport => _lazyReport.Value;

        public int OrderId { get; }

        public ReworkLabelReport.ReportLabelType LabelType { get; }

        #endregion

        #region Methods

        public ReworkLabelReportWrapper(int orderId, ReworkLabelReport.ReportLabelType labelType)
        {
            OrderId = orderId;
            LabelType = labelType;

            _lazyReport = new Lazy<ReworkLabelReport>(GenerateReport);
            _dsOrders = new OrdersDataSet();
        }

        private ReworkLabelReport GenerateReport()
        {
            LoadData(_dsOrders, OrderId);

            var order = _dsOrders.Order.FindByOrderID(OrderId);

            if (order == null)
            {
                LogManager.GetCurrentClassLogger().Warn($"Could not load data for Order # {OrderId}");
                return null;
            }

            return new ReworkLabelReport(order, LabelType);
        }

        public override void Dispose()
        {
            if (_lazyReport.IsValueCreated)
            {
                _lazyReport.Value.Dispose();
            }

            _dsOrders.Dispose();
        }

        private static void LoadData(OrdersDataSet dsOrders, int orderId)
        {
            try
            {
                new OrdersDataSetLoader(dsOrders, OrdersDataSetLoader.OptionalDependencies.Containers)
                    .LoadOrder(orderId);
            }
            catch
            {
                if (dsOrders.HasErrors)
                {
                    LogManager.GetCurrentClassLogger().Warn($"DataSet Errors: {dsOrders.GetDataErrors()}");
                }

                throw;
            }
        }

        #endregion
    }
}
