using NLog;
using System;
using System.Collections;

namespace DWOS.UI.Sales
{
    /// <summary>
    ///   Customer sorter to sort part process nodes by there step order
    /// </summary>
    internal class PartProcessNodeNodesSorter : IComparer
    {
        #region IComparer Members

        public int Compare(object x, object y)
        {
            try
            {
                if(x == null || y == null)
                    return 0;

                if(x is PartProcessNode && y is PartProcessNode && ((PartProcessNode)x).DataRow != null && ((PartProcessNode)y).DataRow != null)
                    return ((PartProcessNode)x).DataRow.StepOrder.CompareTo(((PartProcessNode)y).DataRow.StepOrder);
                else
                    return x.ToString().CompareTo(y.ToString());
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error sorting part process nodes.");
                return 0;
            }
        }

        #endregion
    }
}
