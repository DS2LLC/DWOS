using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.UI.Sales
{    
    /// <summary>
    /// Fields used in order searches.
    /// </summary>
    public enum OrderSearchField
    {
        SO,
        WO,
        PO,
        Part,
        Customer,
        CustomerWO,
        Quantity,
        COC,
        User,
        Price,
        Batch,
        Package,
        SerialNumber,
        ProductClass,
        Custom
    }

    public enum OrderChildRowType
    {
        PartMark
    }
}
