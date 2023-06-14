using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.Data.Datasets;
using NLog;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    internal class PartManager : IPartManager
    {
        #region Fields

        private readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public IDataManager DataManager { get; }

        #endregion

        #region Methods

        public PartManager(IDataManager dataManager)
        {
            DataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
        }

        private void Validate(int customerId, IEnumerable<MasterListPart> parts)
        {
            PartsDataset dsParts = null;
            AwotDataSet dsAwot = null;

            try
            {
                dsParts = new PartsDataset();
                dsAwot = new AwotDataSet();

                // Load data
                DataManager.LoadPartData(dsParts, customerId);
                DataManager.LoadOspFormatData(dsAwot, customerId);

                // Validate
                foreach (var masterListPart in parts)
                {
                    // Check required fields
                    if (string.IsNullOrEmpty(masterListPart.Name))
                    {
                        masterListPart.Status = MasterListPart.PartStatus.Invalid;
                        masterListPart.ImportNotes = "Does not have a name.";
                        continue;
                    }

                    if (string.IsNullOrEmpty(masterListPart.OspCode))
                    {
                        masterListPart.Status = MasterListPart.PartStatus.Invalid;
                        masterListPart.ImportNotes = "Does not have an OSP code.";
                        continue;
                    }

                    var matchingPartRow = dsParts.Part.FirstOrDefault(p => p.Name == masterListPart.Name);
                    var decodedOspFormatInfo = Decode(dsAwot, masterListPart.OspCode);

                    // Check Airframe & Manufacturer
                    var airframe = dsParts.d_Airframe
                        .FirstOrDefault(a => a.AirframeID == masterListPart.ProductCode);

                    if (decodedOspFormatInfo != null && airframe != null)
                    {
                        var incorrectManufacturer =
                            (airframe.IsManufacturerIDNull() &&
                             !string.IsNullOrEmpty(decodedOspFormatInfo.Manufacturer)) ||
                            (!airframe.IsManufacturerIDNull() &&
                             decodedOspFormatInfo.Manufacturer != airframe.ManufacturerID);

                        if (incorrectManufacturer)
                        {
                            masterListPart.Status = MasterListPart.PartStatus.Invalid;
                            masterListPart.ImportNotes = $"Manufacturer is incorrect for airframe '{masterListPart.ProductCode}'.";
                            continue;
                        }
                    }

                    if (matchingPartRow == null)
                    {
                        // New part
                        if (decodedOspFormatInfo == null || !decodedOspFormatInfo.IsValid)
                        {
                            masterListPart.Status = MasterListPart.PartStatus.Invalid;
                            masterListPart.ImportNotes = "OSP format does not match code map information in AWOT.";
                        }
                        else
                        {
                            masterListPart.Status = MasterListPart.PartStatus.New;
                            masterListPart.ImportNotes = string.Empty;
                        }
                    }
                    else
                    {
                        // Existing part
                        var infoFromPart = GetInfoFromPart(matchingPartRow);

                        if (PartProcessingInfo.AreMatch(infoFromPart, decodedOspFormatInfo))
                        {
                            masterListPart.Status = MasterListPart.PartStatus.Existing;
                            masterListPart.ImportNotes = string.Empty;
                        }
                        else
                        {
                            masterListPart.Status = MasterListPart.PartStatus.ExistingWithWarning;
                            masterListPart.ImportNotes = "OSP Format does not match existing part's processes";
                        }
                    }
                }
            }
            finally
            {
                dsParts?.Dispose();
                dsAwot?.Dispose();
            }
        }

        private static PartProcessingInfo GetInfoFromPart(PartsDataset.PartRow partRow)
        {
            var partInfo = new PartProcessingInfo
            {
                Manufacturer = partRow.IsManufacturerIDNull() ? null : partRow.ManufacturerID,
                Processes = new List<PartProcessingInfo.Process>()
            };

            // Processes
            foreach (var process in partRow.GetPartProcessRows().OrderBy(p => p.StepOrder))
            {
                var loadCapacityQuantity = process.IsLoadCapacityQuantityNull() ? (int?)null : process.LoadCapacityQuantity;
                var loadCapacityWeight = process.IsLoadCapacityWeightNull() ? (decimal?)null : process.LoadCapacityWeight;

                partInfo.Processes.Add(new PartProcessingInfo.Process
                {
                    ProcessId = process.ProcessID,
                    ProcessAliasId = process.ProcessAliasID,
                    LoadCapacityQuantity = loadCapacityQuantity,
                    LoadCapacityWeight = loadCapacityWeight
                });
            }

            var partMarkRow = partRow.GetPart_PartMarkingRows().FirstOrDefault();

            if (partMarkRow != null)
            {
                partInfo.Marking = new PartProcessingInfo.PartMarking
                {
                    ProcessSpec = partMarkRow.IsProcessSpecNull() ? null : partMarkRow.ProcessSpec,
                    Def1 = partMarkRow.IsDef1Null() ? null : partMarkRow.Def1,
                    Def2 = partMarkRow.IsDef2Null() ? null : partMarkRow.Def2,
                    Def3 = partMarkRow.IsDef3Null() ? null : partMarkRow.Def3,
                    Def4 = partMarkRow.IsDef4Null() ? null : partMarkRow.Def4,
                };
            }

            return partInfo;
        }

        private static bool IsSectionBlank(string section) =>
            string.IsNullOrEmpty(section) || section.IsMatchingTo("^0+$");

        #endregion

        #region IPartManager Members

        public Task ValidateAsync(int customerId, IEnumerable<MasterListPart> parts)
        {
            if (parts == null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            return Task.Factory.StartNew(() =>
            {
                Validate(customerId, parts);
            });
        }

        public PartProcessingInfo Decode(AwotDataSet dsAwot, string ospCode)
        {
            if (string.IsNullOrEmpty(ospCode))
            {
                return null;
            }

            var formatParts = ospCode.Split("-").Select(s => s.Trim()).ToList();

            var manufacturerCode = formatParts.FirstOrDefault();
            var matchingFormat = dsAwot.OSPFormat.FirstOrDefault(f => f.Code == manufacturerCode);

            if (matchingFormat == null)
            {
                // OSP format code not found
                return null;
            }

            // Decode each part
            var remainingSections = new Queue<string>(formatParts.Skip(1));
            var remainingSectionRows = new Queue<AwotDataSet.OSPFormatSectionRow>(matchingFormat.GetOSPFormatSectionRows().OrderBy(s => s.SectionOrder));

            if (remainingSections.Count != remainingSectionRows.Count)
            {
                // User did not define enough OSP Format code sections.
                return null;
            }

            var processingInfo = new PartProcessingInfo
            {
                Manufacturer = matchingFormat.ManufacturerID,
                Processes = new List<PartProcessingInfo.Process>()
            };

            var sectionCount = 0;

            while (remainingSections.Count > 0)
            {
                sectionCount++;
                var section = remainingSections.Dequeue();
                var sectionRow = remainingSectionRows.Dequeue();

                if (!IsSectionBlank(section))
                {
                    if (sectionRow.Role == RoleType.Process.ToString())
                    {
                        // Add Process
                        var matchingOspProcess = sectionRow.GetOSPFormatSectionProcessRows()
                            .FirstOrDefault(p => p.Code == section);

                        if (matchingOspProcess != null)
                        {
                            processingInfo.Processes.Add(new PartProcessingInfo.Process
                            {
                                ProcessId = matchingOspProcess.ProcessID,
                                ProcessAliasId = matchingOspProcess.ProcessAliasID,
                                LoadCapacityQuantity = DataManager.GetLoadCapacityQuantity(matchingOspProcess.ProcessID),
                                LoadCapacityWeight = DataManager.GetLoadCapacityWeight(matchingOspProcess.ProcessID)
                            });
                        }
                        else
                        {
                            Logger.Warn($"Missing code map for {section} in section {sectionCount}");
                            processingInfo.IsValid = false;
                        }
                    }
                    else if (sectionRow.Role == RoleType.PartMark.ToString())
                    {
                        // Add part marking
                        var matchingPartMark = sectionRow.GetOSPFormatSectionPartMarkRows()
                            .FirstOrDefault(p => p.Code == section);

                        if (matchingPartMark != null)
                        {
                            processingInfo.Marking = new PartProcessingInfo.PartMarking
                            {
                                ProcessSpec = matchingPartMark.IsProcessSpecNull() ? null : matchingPartMark.ProcessSpec,
                                Def1 = matchingPartMark.IsDef1Null() ? null : matchingPartMark.Def1,
                                Def2 = matchingPartMark.IsDef2Null() ? null : matchingPartMark.Def2,
                                Def3 = matchingPartMark.IsDef3Null() ? null : matchingPartMark.Def3,
                                Def4 = matchingPartMark.IsDef4Null() ? null : matchingPartMark.Def4,
                            };
                        }
                        else
                        {
                            Logger.Warn($"Missing code map for {section} in section {sectionCount}");
                            processingInfo.IsValid = false;
                        }
                    }
                }
            }

            return processingInfo;
        }

        #endregion
    }
}
