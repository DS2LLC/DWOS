using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.Data.Tests
{
    [TestClass]
    public class UpdateScriptTests
    {
        [TestMethod]
        public void UpdateScriptTest()
        {
            var scripts = UpdateScript.FromEmbeddedResources();
            Assert.IsNotNull(scripts);

            // Check order of scripts
            var scriptsAreOrdered = scripts
                .Select(i => i.UpgradeVersion)
                .SequenceEqual(scripts.Select(i => i.UpgradeVersion).OrderBy(v => v));

            Assert.IsTrue(scriptsAreOrdered);

            // Test 16.1.2 Script
            Version targetVersion = new Version(16, 1, 2);

            var targetScript = scripts.FirstOrDefault(s => s.UpgradeVersion == targetVersion);
            Assert.IsNotNull(targetVersion);
            Assert.IsFalse(string.IsNullOrEmpty(targetScript.Content));

            string expectedFirstLine = "-- Update Database Version";
            string actualFirstLine = targetScript.Content
                .Substring(0, targetScript.Content.IndexOf(Environment.NewLine));

            Assert.AreEqual(expectedFirstLine, actualFirstLine);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorInvalidTest()
        {
            GC.KeepAlive(new UpdateScript("not valid"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullTest()
        {
            GC.KeepAlive(new UpdateScript(string.Empty));
        }

        [TestMethod]
        public void ContentInvalidTest()
        {
            var target = new UpdateScript("Upgrade_1.0.0.sql");
            Assert.IsTrue(string.IsNullOrEmpty(target.Content));
        }
    }
}
