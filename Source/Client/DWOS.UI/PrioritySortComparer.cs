using System.Collections;
using System.Collections.Generic;

namespace DWOS.UI
{
    /// <summary>
    /// Compares priority values - allows them to be properly sorted.
    /// </summary>
    public class PrioritySortComparer : IComparer, IComparer<string>
    {
        #region Methods

        private static int GetPriorityOrder(string priority)
        {
            if(string.IsNullOrEmpty(priority))
                return 99;

            priority = priority.ToLower();

            switch(priority)
            {
                case "first priority":
                case "first":
                    return 1;
                case "weekend expedite":
                    return 2;
                case "expedite":
                    return 3;
                case "rush":
                    return 4;
                case "normal":
                    return 5;
            }

            return 10;
        }

        #endregion

        #region IComparer<string>

        public int Compare(string xValue, string yValue)
        {
            var xPriority = GetPriorityOrder(xValue);
            var yPriority = GetPriorityOrder(yValue);

            return xPriority.CompareTo(yPriority);
        }

        #endregion

        #region IComparer Members

        public int Compare(object x, object y)
        {
            var xValue = x?.ToString() ?? string.Empty;
            var yValue = y?.ToString() ?? string.Empty;

            return Compare(xValue, yValue);
        }

        #endregion

    }
}