using System.Collections.Generic;

namespace DWOS.Data
{
    public interface ILeadTimeOrder
    {
        int OrderId { get; }

        int? PartQuantity { get; }

        bool HasPartMarking { get; }

        IEnumerable<ILeadTimeProcess> Processes { get; }
    }
}
