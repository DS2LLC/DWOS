using DWOS.DataArchiver.Model;
using System.Collections.Generic;
using ByteSizeLib;

namespace DWOS.DataArchiver
{
    public class GlobalOptionsProvider
    {
        #region Properties

        public string ConnectionString { get; set; }

        public int OrderCount { get; set; }

        public string Directory { get; internal set; }

        public List<Order> OrdersToArchive { get; set; }

        public List<SalesOrder> SalesOrdersToArchive { get; internal set; }

        public ByteSize BytesSaved { get; internal set; }

        #endregion
    }
}
