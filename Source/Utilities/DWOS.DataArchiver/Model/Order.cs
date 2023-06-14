using System;

namespace DWOS.DataArchiver.Model
{
    public class Order
    {
        #region Properties

        public int OrderId { get; set; }

        public string CustomerName { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        #endregion
    }
}
