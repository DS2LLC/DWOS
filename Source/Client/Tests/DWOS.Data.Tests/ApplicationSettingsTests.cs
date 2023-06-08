using DWOS.Shared.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;

namespace DWOS.Data.Tests
{
    [TestClass]
    public sealed class ApplicationSettingsTests
    {
        private ApplicationSettings _settings;
        private Mock<ISettingsPersistence> _mockPersistence;
        private Mock<IPathProvider> _mockPathProvider;

        [TestInitialize]
        public void Initialize()
        {
            _mockPersistence = new Mock<ISettingsPersistence>();
            _mockPathProvider = new Mock<IPathProvider>();
            _settings = new ApplicationSettings(_mockPersistence.Object, _mockPathProvider.Object);
        }

        [TestMethod]
        public void CompanyLogoImagePathTest()
        {
            Assert.IsTrue(string.IsNullOrEmpty(_settings.CompanyLogoImagePath));

            var testImgPath = Path.Combine("Test Files", "Test Image.png");
            _settings.CompanyLogoImagePath = testImgPath;
            Assert.AreEqual(testImgPath, _settings.CompanyLogoImagePath);
        }

        [TestMethod]
        public void CompanyLogoImagePathPersistenceTest()
        {
            var testImgPath = Path.Combine("Test Files", "Test Image.png");
            var testImg = Convert.ToBase64String(File.ReadAllBytes(testImgPath));

            _mockPersistence.Setup((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == "CompanyLogo")))
                .Callback<SettingValue>(
                (pSetting) =>
                {
                    pSetting.Value = testImg;
                });

            _mockPathProvider.Setup((p) => p.ImageDirectory)
                .Returns(Directory.GetCurrentDirectory());

            var actualPath = _settings.CompanyLogoImagePath;
            Assert.IsTrue(File.Exists(actualPath));
        }
    }
}
