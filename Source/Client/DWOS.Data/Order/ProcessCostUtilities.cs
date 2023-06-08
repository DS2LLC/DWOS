namespace DWOS.Data.Order
{
    /// <summary>
    /// Contains utility methods related to process costing.
    /// </summary>
    public static class ProcessCostUtilities
    {
        #region Methods

        /// <summary>
        /// Updates an order process's cost based on its process and answers.
        /// </summary>
        /// <param name="orderProcessesID"></param>
        public static void UpdateCost(int orderProcessesID)
        {
            
            using (var conn = DbConnectionFactory.NewConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    new ProcessCostHelper(conn, transaction).UpdateCost(orderProcessesID);
                    transaction.Commit();
                }
            }

        }

        #endregion
    }
}
