using DWOS.Data.Process;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DWOS.Data.Datasets;

namespace DWOS.Data.Tests.Process
{
    [TestClass]
    public sealed class ProcessImporterTests
    {
        private ProcessesDataset _dataset;

        [TestInitialize]
        public void Initialize()
        {
            _dataset = new ProcessesDataset();

            var noneListRow = _dataset.Lists.NewListsRow();
            noneListRow.ListID = 4;
            noneListRow.Name = "<None>";
            _dataset.Lists.AddListsRow(noneListRow);

            var visualInspectionRow = _dataset.PartInspectionType.NewPartInspectionTypeRow();
            visualInspectionRow.PartInspectionTypeID = 13;
            visualInspectionRow.Name = "Visual Inspection";
            visualInspectionRow.Active = true;
            _dataset.PartInspectionType.AddPartInspectionTypeRow(visualInspectionRow);
        }

        [TestMethod]
        public void ImportTest()
        {
            var importer = new ProcessImporter(_dataset, ImportData());
            var processRow = importer.Import();

            // Process
            Assert.IsNotNull(processRow);
            Assert.AreEqual("Test", processRow.Name);
            Assert.AreEqual("Test Description", processRow.Description);
            Assert.AreEqual("<None>", processRow.Revision = "<None>");
            Assert.AreEqual("QA", processRow.Department = "QA");
            Assert.AreEqual(true, processRow.IsPaperless = true);
            Assert.AreEqual("CAA", processRow.Category = "CAA");

            // Alias
            var processAliasRow = processRow.GetProcessAliasRows().FirstOrDefault();
            Assert.IsNotNull(processAliasRow);
            Assert.AreEqual("Test - Alias", processAliasRow.Name);

            // Step
            var processStepRow = processRow.GetProcessStepsRows().FirstOrDefault();
            Assert.IsNotNull(processStepRow);
            Assert.AreEqual("Step 1", processStepRow.Name);
            Assert.AreEqual(1.0M, processStepRow.StepOrder);

            // Step Question
            var processQuestionRow = processStepRow.GetProcessQuestionRows().FirstOrDefault();
            Assert.IsNotNull(processQuestionRow);
            Assert.AreEqual("Primary", processQuestionRow.Name);
            Assert.AreEqual("String", processQuestionRow.InputType);
            Assert.AreEqual(1.0M, processQuestionRow.StepOrder);
            Assert.AreEqual(true, processQuestionRow.OperatorEditable);
            Assert.AreEqual(true, processQuestionRow.Required);
            Assert.AreEqual(4, processQuestionRow.ListID);

            // Inspection
            var processInspection = processRow.GetProcessInspectionsRows().FirstOrDefault();
            Assert.IsNotNull(processInspection);
            Assert.AreEqual(13, processInspection.PartInspectionTypeID);
            Assert.IsNotNull(processInspection.PartInspectionTypeRow);
            Assert.AreEqual("Visual Inspection", processInspection.PartInspectionTypeRow.Name);
            Assert.AreEqual(1, processInspection.StepOrder);
            Assert.AreEqual(true, processInspection.COCData);
        }

        [TestMethod]
        public void ConditionExportTest()
        {
            var importer = new ProcessImporter(_dataset, ConditionExport());
            var processRow = importer.Import();

            Assert.IsNotNull(processRow);

            var processConditionRows = processRow
                .GetProcessStepsRows()
                .SelectMany(step => step.GetProcessStepConditionRows())
                .ToList();

            Assert.IsTrue(processConditionRows.Count == 1);
            Assert.IsFalse(processConditionRows.First().IsProcessQuestionIdNull());
        }

        [TestMethod]
        public void ConditionOutOfOrderExportTest()
        {
            // This test should fail currently
            var importer = new ProcessImporter(_dataset, ConditionOutOfOrderExport());
            var processRow = importer.Import();

            Assert.IsNotNull(processRow);

            var processConditionRows = processRow
                .GetProcessStepsRows()
                .SelectMany(step => step.GetProcessStepConditionRows())
                .ToList();

            Assert.IsTrue(processConditionRows.Count == 1);
            Assert.IsFalse(processConditionRows.First().IsProcessQuestionIdNull());
        }

        private ProcessExport ImportData()
        {
            return new ProcessExport()
            {
                Name = "Test",
                Description = "Test Description",
                Revision = "<None>",
                Department = "QA",
                IsPaperless = true,
                Category = "CAA",

                ProcessAliases = new List<ProcessAliasExport>()
                {
                    new ProcessAliasExport()
                    {
                        Name = "Test - Alias"
                    }
                },

                ProcessSteps = new List<ProcessStepExport>()
                {
                    new ProcessStepExport()
                    {
                        Name = "Step 1",
                        StepOrder = 1.0M,

                        ProcessQuestions = new List<ProcessQuestionExport>()
                        {
                            new ProcessQuestionExport()
                            {
                                  Name = "Primary",
                                  InputType = "String",
                                  StepOrder = 1.0M,
                                  OperatorEditable = true,
                                  Required = true,
                                  ProcessQuestionIdOriginal = 6075,
                                  ListID = 4,
                                  ListName = "<None>",
                                  ListValues = new List<string>()
                            }
                        },

                        ProcessStepConditions = new List<ProcessStepConditionExport>()
                    }
                },

                ProcessInspections = new List<ProcessInspectionExport>()
                {
                    new ProcessInspectionExport()
                    {
                        PartInspectionTypeID = 13,
                        PartInspectionName = "Visual Inspection",
                        StepOrder = 1,
                        COCData = true
                    }
                }
            };
        }

        private ProcessExport ConditionExport()
        {
            const int primaryQuestionId = 6136;
            const int conditionalQuestionId = 6137;

            return new ProcessExport()
            {
                Name = "Condition",
                Description = "Condition Test",
                Revision = "<None>",
                Department = "QA",
                IsPaperless = true,
                Category = "CAA",

                ProcessAliases = new List<ProcessAliasExport>()
                {
                    new ProcessAliasExport()
                    {
                        Name = "Condition"
                    }
                },

                ProcessSteps = new List<ProcessStepExport>()
                {
                    new ProcessStepExport()
                    {
                        Name = "Primary Step",
                        StepOrder = 1.0M,

                        ProcessQuestions = new List<ProcessQuestionExport>()
                        {
                            new ProcessQuestionExport()
                            {
                                  Name = "Primary",
                                  InputType = "String",
                                  StepOrder = 1.0M,
                                  OperatorEditable = true,
                                  Required = true,
                                  ProcessQuestionIdOriginal = primaryQuestionId,
                                  ListID = 4,
                                  ListName = "<None>",
                                  ListValues = new List<string>()
                            }
                        },

                        ProcessStepConditions = new List<ProcessStepConditionExport>()
                    },

                    new ProcessStepExport()
                    {
                        Name = "Conditional Step",
                        StepOrder = 2.0M,
                        ProcessQuestions = new List<ProcessQuestionExport>()
                        {
                            new ProcessQuestionExport()
                            {
                                Name = "Conditional Question",
                                InputType = "String",
                                StepOrder = 1.0M,
                                OperatorEditable = true,
                                Required = true,
                                ProcessQuestionIdOriginal = conditionalQuestionId,
                                ListID = 4,
                                ListName = "<None>",
                                ListValues = new List<string>()
                            }
                        },

                        ProcessStepConditions = new List<ProcessStepConditionExport>()
                        {
                            new ProcessStepConditionExport()
                            {
                                InputType = "ProcessQuestion",
                                ProcessQuestionIdOriginal = primaryQuestionId,
                                Operator = "Equal",
                                Value = "Testing",
                                StepOrder = 99
                            }
                        }
                    }
                },

                ProcessInspections = new List<ProcessInspectionExport>()
            };
        }

        private ProcessExport ConditionOutOfOrderExport()
        {
            const int primaryQuestionId = 6137;
            const int conditionalQuestionId = 6136;

            return new ProcessExport()
            {
                Name = "Condition",
                Description = "Condition Test",
                Revision = "<None>",
                Department = "QA",
                IsPaperless = true,
                Category = "CAA",

                ProcessAliases = new List<ProcessAliasExport>()
                {
                    new ProcessAliasExport()
                    {
                        Name = "Condition"
                    }
                },

                ProcessSteps = new List<ProcessStepExport>()
                {
                    new ProcessStepExport()
                    {
                        Name = "Conditional Step",
                        StepOrder = 2.0M,
                        ProcessQuestions = new List<ProcessQuestionExport>()
                        {
                            new ProcessQuestionExport()
                            {
                                Name = "Conditional Question",
                                InputType = "String",
                                StepOrder = 1.0M,
                                OperatorEditable = true,
                                Required = true,
                                ProcessQuestionIdOriginal = conditionalQuestionId,
                                ListID = 4,
                                ListName = "<None>",
                                ListValues = new List<string>()
                            }
                        },

                        ProcessStepConditions = new List<ProcessStepConditionExport>()
                        {
                            new ProcessStepConditionExport()
                            {
                                InputType = "ProcessQuestion",
                                ProcessQuestionIdOriginal = primaryQuestionId,
                                Operator = "Equal",
                                Value = "Testing",
                                StepOrder = 99
                            }
                        }
                    },

                    new ProcessStepExport()
                    {
                        Name = "Primary Step",
                        StepOrder = 1.0M,

                        ProcessQuestions = new List<ProcessQuestionExport>()
                        {
                            new ProcessQuestionExport()
                            {
                                  Name = "Primary",
                                  InputType = "String",
                                  StepOrder = 1.0M,
                                  OperatorEditable = true,
                                  Required = true,
                                  ProcessQuestionIdOriginal = primaryQuestionId,
                                  ListID = 4,
                                  ListName = "<None>",
                                  ListValues = new List<string>()
                            }
                        },

                        ProcessStepConditions = new List<ProcessStepConditionExport>()
                    },


                },

                ProcessInspections = new List<ProcessInspectionExport>()
            };
        }
    }
}
