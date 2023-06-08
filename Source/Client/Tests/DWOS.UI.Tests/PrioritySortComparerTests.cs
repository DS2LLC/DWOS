using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.UI.Tests
{
    [TestClass]
    public class PrioritySortComparerTests
    {
        [TestMethod]
        public void CompareTest()
        {
            var target = new PrioritySortComparer();
            Assert.IsTrue(target.Compare("first priority", "weekend expedite") < 0);
            Assert.IsTrue(target.Compare("weekend expedite", "expedite") < 0);
            Assert.IsTrue(target.Compare("expedite", "rush") < 0);
            Assert.IsTrue(target.Compare("rush", "normal") < 0);
            Assert.IsTrue(target.Compare("rush", "normal") < 0);
            Assert.IsTrue(target.Compare("normal", "not in list") < 0);

            Assert.IsTrue(target.Compare("not in list", "first priority") > 0);
            Assert.IsTrue(target.Compare("not in list", "not in list 2") == 0);
        }
    }
}
