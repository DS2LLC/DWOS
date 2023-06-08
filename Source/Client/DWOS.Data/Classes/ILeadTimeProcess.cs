using System;

namespace DWOS.Data
{
    public interface ILeadTimeProcess
    {
        int ProcessId { get; }

        int StepOrder { get; }

        ProcessLeadTime LeadTime { get; }

        DateTime? EstEndDate { get; set; }
    }
}
