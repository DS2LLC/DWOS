using System;

namespace DWOS.Data.Datasets
{


    public partial class OrderHistoryDataSet
    {
        public static void UpdateOrderHistory(int orderID, string category, string description, string userName)
        {
            try
            {
                using (var ta = new OrderHistoryDataSetTableAdapters.OrderHistoryTableAdapter())
                    ta.UpdateOrderHistory(orderID, category, description, userName);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error writing order history.");
            }
        }

        public static void UpdatePartHistory(int partId, string category, string description, string userName)
        {
            try
            {
                using (var ta = new OrderHistoryDataSetTableAdapters.PartHistoryTableAdapter())
                    ta.UpdatePartHistory(partId, category, description, userName);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error writing part history.");
            }
        }

    }
}

namespace DWOS.Data.Datasets.OrderHistoryDataSetTableAdapters
{
    partial class PartHistoryTableAdapter
    {
        private static readonly NLog.ILogger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        public void UpdatePartHistory(int partId, string category, string description, string userName)
        {
            if (userName == null)
            {
                userName = Environment.UserName;
            }

            Insert(partId,
                category.TrimToMaxLength(50),
                description.TrimToMaxLength(255),
                userName.TrimToMaxLength(50),
                Environment.MachineName.TrimToMaxLength(50),
                DateTime.Now);

            LOGGER.Info($"Updated history for part #{partId}");
        }
    }

    public partial class OrderHistoryTableAdapter {
        private static readonly NLog.ILogger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        public void UpdateOrderHistory(int orderId, string category, string description, string userName)
        {
            if (userName == null)
            {
                userName = Environment.UserName;
            }

            Insert(orderId,
                category.TrimToMaxLength(50),
                description.TrimToMaxLength(255),
                userName.TrimToMaxLength(50),
                Environment.MachineName.TrimToMaxLength(50),
                DateTime.Now);

            LOGGER.Info($"Updated history for order #{orderId}");
        }
    }
}
