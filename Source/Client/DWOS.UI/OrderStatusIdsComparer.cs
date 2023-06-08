using System.Collections;
using System.Linq;

namespace DWOS.UI
{
    internal class OrderStatusIdsComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            var xAsBatches = x as OrderStatusData.IdsInfo;
            var yAsBatches = y as OrderStatusData.IdsInfo;

            if (xAsBatches == null)
            {
                return yAsBatches == null ? 0 : -1;
            }

            if (yAsBatches == null)
            {
                return 1;
            }

            var xBatch = xAsBatches.Ids.FirstOrDefault();
            var yBatch = yAsBatches.Ids.FirstOrDefault();

            if (xBatch > yBatch)
            {
                return 1;
            }
            else if (xBatch == yBatch)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
