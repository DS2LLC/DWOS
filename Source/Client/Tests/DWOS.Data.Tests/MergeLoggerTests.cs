using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;

namespace DWOS.Data.Tests
{
    [TestClass]
    public class MergeLoggerTests
    {
        [TestMethod]
        public void LogValuesTest()
        {
            var original = new DataTable();
            original.Columns.Add("ID", typeof(int));
            original.Rows.Add(5);
            original.Rows.Add(6);
            original.Rows.Add(7);

            var toMerge = new DataTable();
            toMerge.Columns.Add("ID", typeof(int));
            toMerge.Rows.Add(6);
            toMerge.Rows.Add(8);

            var mergeLogger = new MergeLogger(original, toMerge, "ID");
            var mockLogger = new Mock<ILogger>();
            mergeLogger.Logger = mockLogger.Object;

            mergeLogger.LogValues();

            mockLogger.Verify(m => m.Info("Client: 6"), Times.Once);
            mockLogger.Verify(m => m.Info("Server: 6"), Times.Once);
            mockLogger.Verify(m => m.Info("Cannot find original row"), Times.Once);
            mockLogger.Verify(m => m.Info("Server: 8"), Times.Once);

            mockLogger.Verify(m => m.Info("Client: 5"), Times.Never);
            mockLogger.Verify(m => m.Info("Server: 5"), Times.Never);
            mockLogger.Verify(m => m.Info("Client: 7"), Times.Never);
            mockLogger.Verify(m => m.Info("Server: 7"), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullClientTest()
        {
            GC.KeepAlive(new MergeLogger(null, new DataTable(), "ID"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullMergeTest()
        {
            GC.KeepAlive(new MergeLogger(new DataTable(), null, "ID"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullIdColumnTest()
        {
            GC.KeepAlive(new MergeLogger(new DataTable(), new DataTable(), null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorInvalidClientTest()
        {
            var original = new DataTable();
            var toMerge = new DataTable();
            toMerge.Columns.Add("ID", typeof(int));
            GC.KeepAlive(new MergeLogger(original, toMerge, "ID"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorInvalidMergeTest()
        {
            var original = new DataTable();
            original.Columns.Add("ID", typeof(int));
            var toMerge = new DataTable();
            GC.KeepAlive(new MergeLogger(original, toMerge, "ID"));
        }
    }
}
