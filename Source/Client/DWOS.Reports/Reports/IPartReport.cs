using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.Reports
{
    /// <summary>
    /// Interface implemented by part-specific reports.
    /// </summary>
    public interface IPartReport : IReport
    {
        /// <summary>
        /// Gets or sets the starting date of the report.
        /// </summary>
        DateTime FromDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ending date of the report.
        /// </summary>
        DateTime ToDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Customer ID to use for order searches.
        /// </summary>
        int CustomerID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Part ID to use for order searches.
        /// </summary>
        int PartID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the title of the report.
        /// </summary>
        string Title
        {
            get;
        }
    }
}
