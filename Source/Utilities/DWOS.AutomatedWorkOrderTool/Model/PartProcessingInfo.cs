using System;
using System.Collections.Generic;

namespace DWOS.AutomatedWorkOrderTool.Model
{
    public class PartProcessingInfo
    {
        #region Properties

        public string Manufacturer { get; set; }

        public List<Process> Processes { get; set; }

        public PartMarking Marking { get; set; }

        public bool IsValid { get; set; } = true;

        #endregion

        #region Methods

        public static bool AreMatch(PartProcessingInfo infoFromPart, PartProcessingInfo infoFromOspFormat)
        {
            if (ReferenceEquals(infoFromPart, infoFromOspFormat))
            {
                return true;
            }

            if (infoFromPart == null || infoFromOspFormat == null)
            {
                return false;
            }

            // Manufacturer
            if (infoFromPart.Manufacturer != infoFromOspFormat.Manufacturer)
            {
                return false;
            }

            // Processes
            var partProcesses = infoFromPart.Processes ?? new List<Process>();
            var ospProcesses = infoFromOspFormat.Processes ?? new List<Process>();

            if (partProcesses.Count != ospProcesses.Count)
            {
                return false;
            }

            for (var i = 0; i < partProcesses.Count; ++i)
            {
                var partProcess = partProcesses[i];
                var ospProcess = ospProcesses[i];

                // Check ProcessId and ProcessAliasId
                // Skip check for load capacity as it is irrelevant when
                // comparing order of processes.
                if (partProcess.ProcessId != ospProcess.ProcessId || partProcess.ProcessAliasId != ospProcess.ProcessAliasId)
                {
                    return false;
                }
            }

            // Part Marking
            if (infoFromPart.Marking == null)
            {
                return true;
            }

            if (infoFromOspFormat.Marking == null)
            {
                return false;
            }

            return infoFromPart.Marking.ProcessSpec == infoFromOspFormat.Marking.ProcessSpec &&
                infoFromPart.Marking.Def1 == infoFromOspFormat.Marking.Def1 &&
                infoFromPart.Marking.Def2 == infoFromOspFormat.Marking.Def2 &&
                infoFromPart.Marking.Def3 == infoFromOspFormat.Marking.Def3 &&
                infoFromPart.Marking.Def4 == infoFromOspFormat.Marking.Def4;
        }

        #endregion

        #region Process

        public class Process
        {
            #region Properties

            public int ProcessId { get; set; }

            public int ProcessAliasId { get; set; }

            public int? LoadCapacityQuantity { get; set; }

            public decimal? LoadCapacityWeight { get; set; }

            #endregion
        }

        #endregion

        #region PartMarking

        public class PartMarking
        {
            #region Properties

            public string ProcessSpec { get; set; }

            public string Def1 { get; set; }

            public string Def2 { get; set; }

            public string Def3 { get; set; }

            public string Def4 { get; set; }

            #endregion
        }

        #endregion
    }
}
