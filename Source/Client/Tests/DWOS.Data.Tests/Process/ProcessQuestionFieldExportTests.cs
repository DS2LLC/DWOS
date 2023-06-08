using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Process;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.Data.Tests.Process
{
    [TestClass]
    public class ProcessQuestionFieldExportTests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            const string expectedTokenName = "Token";
            const string expectedFieldName = "Field";

            // Setup
            var dtField = new ProcessesDataset.ProcessQuestionFieldDataTable();
            var row = dtField.NewProcessQuestionFieldRow();
            row.FieldName = expectedFieldName;
            row.TokenName = expectedTokenName;

            var target = new ProcessQuestionFieldExport(row);
            Assert.AreEqual(expectedTokenName, target.TokenName);
            Assert.AreEqual(expectedFieldName, target.FieldName);
        }

        [TestMethod]
        public void CreateRowTest()
        {
            const string expectedTokenName = "Token";
            const string expectedFieldName = "Field";

            // Setup
            var dsProcesses = CreateTestDataset();
            var target = new ProcessQuestionFieldExport
            {
                TokenName = expectedTokenName,
                FieldName = expectedFieldName
            };

            var export = CreateTestExport(target);

            var importer = new ProcessImporter(dsProcesses, export);

            // Test - should exercise CreateRow method
            importer.Import();

            var row = dsProcesses.ProcessQuestionField.FirstOrDefault();
            Assert.IsNotNull(row);
            Assert.AreEqual(expectedTokenName, row.TokenName);
            Assert.AreEqual(expectedFieldName, row.FieldName);
            Assert.IsNotNull(row.ProcessQuestionRow);
        }

        [TestMethod]
        public void CreateRowInvalidFieldTest()
        {
            // Setup
            var target = new ProcessQuestionFieldExport
            {
                TokenName = null,
                FieldName = null
            };

            var export = CreateTestExport(target);

            var dsProcesses = CreateTestDataset();

            var importer = new ProcessImporter(dsProcesses, export);

            // Test - should exercise CreateRow method
            importer.Import();

            var row = dsProcesses.ProcessQuestionField.FirstOrDefault();
            Assert.IsNull(row);
        }

        private static ProcessesDataset CreateTestDataset()
        {
            var dsProcesses = new ProcessesDataset();
            var listRow = dsProcesses.Lists.NewListsRow();
            listRow.ListID = 4;
            listRow.Name = "Test";
            dsProcesses.Lists.AddListsRow(listRow);
            return dsProcesses;
        }

        private static ProcessExport CreateTestExport(ProcessQuestionFieldExport target)
        {
            var export = new ProcessExport
            {
                Name = "Test",
                Department = "Test",
                Revision = "Test",
                Description = "Test",
                ProcessAliases = new List<ProcessAliasExport>
                {
                    new ProcessAliasExport
                    {
                        Name = "Test"
                    }
                },
                ProcessInspections = new List<ProcessInspectionExport>(),
                ProcessSteps = new List<ProcessStepExport>
                {
                    new ProcessStepExport
                    {
                        Name = "Step",
                        ProcessStepConditions = new List<ProcessStepConditionExport>(),
                        ProcessQuestions = new List<ProcessQuestionExport>()
                        {
                            new ProcessQuestionExport
                            {
                                Name = "Question",
                                InputType = "Decimal",
                                ProcessQuestionFields = new List<ProcessQuestionFieldExport>
                                {
                                    target
                                }
                            }
                        }
                    }
                }
            };

            return export;
        }
    }
}
