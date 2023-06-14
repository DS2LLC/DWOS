using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.Tests
{
    [TestClass]
    public class ApplicationSettingsTests
    {
        [TestMethod]
        public void ApplicationSettingsInfoTest()
        {
            var actualSettings = ApplicationSettings.Settings;
            Assert.IsNotNull(actualSettings);
        }
    }
}
