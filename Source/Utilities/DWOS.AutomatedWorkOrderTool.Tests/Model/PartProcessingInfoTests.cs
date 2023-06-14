using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DWOS.AutomatedWorkOrderTool.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.AutomatedWorkOrderTool.Tests.Model
{
    [TestClass]
    public class PartProcessingInfoTests
    {
        [TestMethod]
        public void HasSameProcessesTests()
        {
            // Case - No processes, same manufacturer
            var infoFromPart = new PartProcessingInfo
            {
                Manufacturer = "A"
            };

            var infoFromOspFormat = new PartProcessingInfo
            {
                Manufacturer = "A"
            };

            Assert.IsTrue(PartProcessingInfo.AreMatch(infoFromPart, infoFromOspFormat));

            // Case - No processes, different manufacturer
            infoFromPart = new PartProcessingInfo
            {
                Manufacturer = "A"
            };

            infoFromOspFormat = new PartProcessingInfo
            {
                Manufacturer = "B"
            };

            Assert.IsFalse(PartProcessingInfo.AreMatch(infoFromPart, infoFromOspFormat));

            // Case - Same processes, same manufacturer
            infoFromPart = new PartProcessingInfo
            {
                Manufacturer = "A",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 1, ProcessAliasId =  100 },
                    new PartProcessingInfo.Process { ProcessId = 2, ProcessAliasId =  200 }
                }
            };

            infoFromOspFormat = new PartProcessingInfo
            {
                Manufacturer = "A",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 1, ProcessAliasId =  100 },
                    new PartProcessingInfo.Process { ProcessId = 2, ProcessAliasId =  200 }
                }
            };

            Assert.IsTrue(PartProcessingInfo.AreMatch(infoFromPart, infoFromOspFormat));

            // Case - Same processes, different manufacturer
            infoFromPart = new PartProcessingInfo
            {
                Manufacturer = "A",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 1, ProcessAliasId =  100 },
                    new PartProcessingInfo.Process { ProcessId = 2, ProcessAliasId =  200 }
                }
            };

            infoFromOspFormat = new PartProcessingInfo
            {
                Manufacturer = "B",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 1, ProcessAliasId =  100 },
                    new PartProcessingInfo.Process { ProcessId = 2, ProcessAliasId =  200 }
                }
            };

            Assert.IsFalse(PartProcessingInfo.AreMatch(infoFromPart, infoFromOspFormat));

            // Case - Different processes
            infoFromPart = new PartProcessingInfo
            {
                Manufacturer = "A",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 1, ProcessAliasId =  100 },
                    new PartProcessingInfo.Process { ProcessId = 2, ProcessAliasId =  200 }
                }
            };

            infoFromOspFormat = new PartProcessingInfo
            {
                Manufacturer = "A",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 100, ProcessAliasId =  1 },
                    new PartProcessingInfo.Process { ProcessId = 200, ProcessAliasId =  2 }
                }
            };

            Assert.IsFalse(PartProcessingInfo.AreMatch(infoFromPart, infoFromOspFormat));

            // Case - part has more processes than OSP format
            infoFromPart = new PartProcessingInfo
            {
                Manufacturer = "A",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 1, ProcessAliasId =  100 },
                    new PartProcessingInfo.Process { ProcessId = 2, ProcessAliasId =  200 },
                    new PartProcessingInfo.Process { ProcessId = 3, ProcessAliasId =  300 }
                }
            };

            infoFromOspFormat = new PartProcessingInfo
            {
                Manufacturer = "B",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 1, ProcessAliasId =  100 },
                    new PartProcessingInfo.Process { ProcessId = 2, ProcessAliasId =  200 }
                }
            };

            // Case - Same processes, same manufacturer, and same part marking
            infoFromPart = new PartProcessingInfo
            {
                Manufacturer = "A",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 1, ProcessAliasId =  100 },
                    new PartProcessingInfo.Process { ProcessId = 2, ProcessAliasId =  200 }
                },
                Marking = new PartProcessingInfo.PartMarking
                {
                    ProcessSpec =  "Spec A",
                    Def1 = "A"
                }
            };

            infoFromOspFormat = new PartProcessingInfo
            {
                Manufacturer = "A",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 1, ProcessAliasId =  100 },
                    new PartProcessingInfo.Process { ProcessId = 2, ProcessAliasId =  200 }
                },
                Marking = new PartProcessingInfo.PartMarking
                {
                    ProcessSpec =  "Spec A",
                    Def1 = "A"
                }
            };

            Assert.IsTrue(PartProcessingInfo.AreMatch(infoFromPart, infoFromOspFormat));

            // Case - Same processes, same manufacturer, but different part marking spec
            infoFromPart = new PartProcessingInfo
            {
                Manufacturer = "A",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 1, ProcessAliasId =  100 },
                    new PartProcessingInfo.Process { ProcessId = 2, ProcessAliasId =  200 }
                },
                Marking = new PartProcessingInfo.PartMarking
                {
                    ProcessSpec =  "Spec A",
                    Def1 = "A"
                }
            };

            infoFromOspFormat = new PartProcessingInfo
            {
                Manufacturer = "A",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 1, ProcessAliasId =  100 },
                    new PartProcessingInfo.Process { ProcessId = 2, ProcessAliasId =  200 }
                },
                Marking = new PartProcessingInfo.PartMarking
                {
                    ProcessSpec =  "Spec B",
                    Def1 = "A"
                }
            };

            Assert.IsFalse(PartProcessingInfo.AreMatch(infoFromPart, infoFromOspFormat));

            // Case - Same processes, same manufacturer, but different part marking lines
            infoFromPart = new PartProcessingInfo
            {
                Manufacturer = "A",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 1, ProcessAliasId =  100 },
                    new PartProcessingInfo.Process { ProcessId = 2, ProcessAliasId =  200 }
                },
                Marking = new PartProcessingInfo.PartMarking
                {
                    ProcessSpec =  "Spec A",
                    Def1 = "A"
                }
            };

            infoFromOspFormat = new PartProcessingInfo
            {
                Manufacturer = "A",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 1, ProcessAliasId =  100 },
                    new PartProcessingInfo.Process { ProcessId = 2, ProcessAliasId =  200 }
                },
                Marking = new PartProcessingInfo.PartMarking
                {
                    ProcessSpec =  "Spec A",
                    Def1 = "B"
                }
            };

            Assert.IsFalse(PartProcessingInfo.AreMatch(infoFromPart, infoFromOspFormat));

            // Case - Same processes, same manufacturer, but only OSP Format specifies part marking
            infoFromPart = new PartProcessingInfo
            {
                Manufacturer = "A",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 1, ProcessAliasId =  100 },
                    new PartProcessingInfo.Process { ProcessId = 2, ProcessAliasId =  200 }
                }
            };

            infoFromOspFormat = new PartProcessingInfo
            {
                Manufacturer = "A",
                Processes = new List<PartProcessingInfo.Process>
                {
                    new PartProcessingInfo.Process { ProcessId = 1, ProcessAliasId =  100 },
                    new PartProcessingInfo.Process { ProcessId = 2, ProcessAliasId =  200 }
                },
                Marking = new PartProcessingInfo.PartMarking
                {
                    ProcessSpec =  "Spec A",
                    Def1 = "A"
                }

            };

            Assert.IsTrue(PartProcessingInfo.AreMatch(infoFromPart, infoFromOspFormat));

            // Case - Both are null
            Assert.IsTrue(PartProcessingInfo.AreMatch(null, null));

            // Case - One is null
            Assert.IsFalse(PartProcessingInfo.AreMatch(new PartProcessingInfo(), null));
            Assert.IsFalse(PartProcessingInfo.AreMatch(null, new PartProcessingInfo()));
        }
    }
}
