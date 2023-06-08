using Infragistics.Windows.DataPresenter;
using System.Collections;
using System.Linq;

namespace DWOS.UI
{
    internal class OrderStatusIdsGrouper : IGroupByEvaluator
    {
        private const string GroupNotInBatch = "Not in Batch";
        private const string GroupInManyBatches = "In Multiple Batches";

        public IComparer SortComparer { get; } =
            new OrderStatusIdsComparer();

        public bool DoesGroupContainRecord(GroupByRecord groupByRecord, DataRecord record)
        {
            var batches = record.GetCellValue(groupByRecord.GroupByField) as OrderStatusData.IdsInfo;
            var desc = groupByRecord.Description;

            if (batches == null || batches.Ids.Count == 0)
            {
                return desc == GroupNotInBatch;
            }
            else if (batches.Ids.Count > 1)
            {
                return desc == GroupInManyBatches;
            }

            return desc == $"In Batch {batches.Ids.First()}";
        }

        public object GetGroupByValue(GroupByRecord groupByRecord, DataRecord record)
        {
            var batches = record.GetCellValue(groupByRecord.GroupByField) as OrderStatusData.IdsInfo;
            var desc = groupByRecord.Description;

            if (batches == null || batches.Ids.Count == 0)
            {
                groupByRecord.Description = GroupNotInBatch;
                return false;
            }
            else if (batches.Ids.Count > 1)
            {
                groupByRecord.Description = GroupInManyBatches;
                return true;
            }

            groupByRecord.Description = $"In Batch {batches.Ids.First()}";
            return batches.Ids.First();
        }
    }
}
