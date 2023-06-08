using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Process;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.Data.Tests.Process
{
    [TestClass]
    public class ProcessQuestionExportTests
    {
        [TestMethod]
        public void CreateRowNullFieldsTest()
        {
            // Setup
            var dsProcesses = CreateTestDataset();
            var target = new ProcessQuestionExport
            {
                Name = "Question",
                InputType =  "Decimal"
            };
            var export = CreateTestExport(target);

            var importer = new ProcessImporter(dsProcesses, export);

            // Test - should exercise CreateRow method
            importer.Import();

            Assert.IsTrue(dsProcesses.ProcessQuestion.Any());
            Assert.IsFalse(dsProcesses.ProcessQuestionField.Any());
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

        private static ProcessExport CreateTestExport(ProcessQuestionExport target)
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
                            target
                        }
                    }
                }
            };

            return export;
        }
    }
}
