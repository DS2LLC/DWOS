using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace DWOS.Data.Tests
{
    [TestClass]
    public class DataTableExtensionsTests
    {
        [TestMethod]
        public void FormattedSelectTests()
        {
            using (var dt = TestTable())
            {
                DataRow[] rows;

                // Normal cases
                rows = dt.FormattedSelect("Quantity = {0} AND Name = '{1}'", 2, "Test2");
                Assert.IsNotNull(rows);
                Assert.IsTrue(rows.Length == 1);
                Assert.AreEqual(2, rows[0]["Quantity"]);
                Assert.AreEqual("Test2", rows[0]["Name"]);

                rows = dt.FormattedSelect("Quantity > {0}", 1);
                Assert.IsNotNull(rows);
                Assert.IsTrue(rows.Length == 2);

                // Single quote case
                rows = dt.FormattedSelect("Name = '{0}'", "'Example'");
                Assert.IsNotNull(rows);
                Assert.IsTrue(rows.Length == 1);
                Assert.AreEqual(3, rows[0]["Quantity"]);
                Assert.AreEqual("'Example'", rows[0]["Name"]);

                // No params case
                rows = dt.FormattedSelect("Quantity = 1");
                Assert.IsNotNull(rows);
                Assert.IsTrue(rows.Length == 1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormattedSelectNullTest()
        {
            DataTable dt = null;
            dt.FormattedSelect("");
        }

        private static DataTable TestTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Quantity", typeof(int));
            dt.Columns.Add("Name", typeof(string));

            dt.Rows.Add(1, "Test1");
            dt.Rows.Add(2, "Test2");
            dt.Rows.Add(3, "'Example'");

            return dt;
        }
    }
}
