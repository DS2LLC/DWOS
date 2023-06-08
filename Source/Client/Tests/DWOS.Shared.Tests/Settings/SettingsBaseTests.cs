using DWOS.Shared.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Globalization;

namespace DWOS.Shared.Tests.Settings
{
    [TestClass]
    public sealed class SettingsBaseTests
    {
        private const string SETTING_STRING = "String";
        private const string SETTING_DATETIME = "DateTime";
        private const string SETTING_FLOAT = "Float";
        private const string SETTING_INT = "Int";
        private const string SETTING_BOOL = "Bool";
        private const string SETTING_DOUBLE = "Double";
        private const string SETTING_CONVERTER = "Converter";

        private Mock<ISettingsPersistence> _mockPersistence;
        private TestSettingsBase _instance;

        [TestInitialize]
        public void Initialize()
        {
            _mockPersistence = new Mock<ISettingsPersistence>();
            _instance = new TestSettingsBase(_mockPersistence.Object);
        }

        [TestMethod]
        public void GetSettingTest()
        {
            Assert.AreEqual(default(string), _instance.TestString);

            string expectedValue = "Testing";
            _instance.TestString = expectedValue;

            Assert.AreEqual(expectedValue, _instance.TestString);
        }

        [TestMethod]
        public void GetSettingPersistenceTest()
        {
            const string expectedValue = "Testing";

            _mockPersistence.Setup((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_STRING)))
                .Callback<SettingValue>(
                (pSetting) =>
                {
                    pSetting.Value = expectedValue;
                });

            Assert.AreEqual(expectedValue, _instance.TestString);

            // Check cached value
            Assert.AreEqual(expectedValue, _instance.TestString);

            _mockPersistence
                .Verify((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_STRING)), Times.Once);
        }

        [TestMethod]
        public void SetSettingTest()
        {
            const string expectedValue = "Set";

            _instance.TestString = expectedValue;

            Assert.AreEqual(expectedValue, _instance.TestString);

            // Changing existing value
            _instance.TestString = "Updated Value";
            Assert.AreEqual("Updated Value", _instance.TestString);

            // Settings should not be retrieved from persistence
            _mockPersistence
                .Verify((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_STRING)), Times.Never);
        }

        [TestMethod]
        public void DateTimeSettingTest()
        {
            Assert.AreEqual(default(DateTime), _instance.TestDate);

            var expectedDateTime = new DateTime(2016, 1, 1, 12, 25, 50);
            _instance.TestDate = expectedDateTime;

            Assert.AreEqual(expectedDateTime, _instance.TestDate);
        }

        [TestMethod]
        public void DateTimeSettingPersistenceTest()
        {
            DateTime expectedValue = new DateTime(2016, 1, 1, 12, 25, 50);
            _mockPersistence.Setup((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_DATETIME)))
                .Callback<SettingValue>(
                (pSetting) =>
                {
                    pSetting.Value = expectedValue.ToString(CultureInfo.InvariantCulture);
                });

            Assert.AreEqual(expectedValue, _instance.TestDate);

            // Check cached value
            Assert.AreEqual(expectedValue, _instance.TestDate);

            _mockPersistence
                .Verify((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_DATETIME)), Times.Once);
        }

        [TestMethod]
        public void FloatSettingTest()
        {
            Assert.AreEqual(default(float), _instance.TestFloat);

            var expectedValue = 5.55f;
            _instance.TestFloat = expectedValue;

            Assert.AreEqual(expectedValue, _instance.TestFloat);
        }

        [TestMethod]
        public void FloatSettingPersistenceTest()
        {
            var expectedValue = 5.55f;
            _mockPersistence.Setup((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_FLOAT)))
                .Callback<SettingValue>(
                (pSetting) =>
                {
                    pSetting.Value = expectedValue.ToString(CultureInfo.InvariantCulture);
                });

            Assert.AreEqual(expectedValue, _instance.TestFloat);

            // Check cached value
            Assert.AreEqual(expectedValue, _instance.TestFloat);

            _mockPersistence
                .Verify((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_FLOAT)), Times.Once);
        }

        [TestMethod]
        public void IntSettingTest()
        {
            Assert.AreEqual(default(int), _instance.TestInt);

            var expected = 15;
            _instance.TestInt = expected;

            Assert.AreEqual(expected, _instance.TestInt);
        }

        [TestMethod]
        public void IntSettingPersistenceTest()
        {
            var expectedValue = 15;
            _mockPersistence.Setup((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_INT)))
                .Callback<SettingValue>(
                (pSetting) =>
                {
                    pSetting.Value = expectedValue.ToString(CultureInfo.InvariantCulture);
                });

            Assert.AreEqual(expectedValue, _instance.TestInt);

            // Check cached value
            Assert.AreEqual(expectedValue, _instance.TestInt);

            _mockPersistence
                .Verify((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_INT)), Times.Once);
        }

        [TestMethod]
        public void BoolSettingTest()
        {
            Assert.AreEqual(default(bool), _instance.TestBool);

            var expectedValue = true;
            _instance.TestBool = expectedValue;

            Assert.AreEqual(expectedValue, _instance.TestBool);
        }

        [TestMethod]
        public void BoolSettingPersistenceTest()
        {
            var expectedValue = true;
            _mockPersistence.Setup((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_BOOL)))
                .Callback<SettingValue>(
                (pSetting) =>
                {
                    pSetting.Value = expectedValue.ToString(CultureInfo.InvariantCulture);
                });

            Assert.AreEqual(expectedValue, _instance.TestBool);

            // Check cached value
            Assert.AreEqual(expectedValue, _instance.TestBool);

            _mockPersistence
                .Verify((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_BOOL)), Times.Once);
        }

        [TestMethod]
        public void DoubleSettingTest()
        {
            Assert.AreEqual(default(double), _instance.TestDouble);

            var expectedValue = 15.95D;
            _instance.TestDouble = expectedValue;

            Assert.AreEqual(expectedValue, _instance.TestDouble);
        }

        [TestMethod]
        public void DoubleSettingPersistenceTest()
        {
            var expectedValue = 15.95D;
            _mockPersistence.Setup((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_DOUBLE)))
                .Callback<SettingValue>(
                (pSetting) =>
                {
                    pSetting.Value = expectedValue.ToString(CultureInfo.InvariantCulture);
                });

            Assert.AreEqual(expectedValue, _instance.TestDouble);

            // Check cached value
            Assert.AreEqual(expectedValue, _instance.TestDouble);

            _mockPersistence
                .Verify((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_DOUBLE)), Times.Once);
        }

        [TestMethod]
        public void ConverterSettingTest()
        {
            //_mockPersistence.Setup((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_CONVERTER)))
            //    .Callback<SettingValue, Func<string>>(
            //    (pSetting) =>
            //    {
            //        pSetting.Value = defaultFunc?.Invoke();
            //    });
            Assert.IsNotNull(_instance.TestConverter);
            Assert.AreEqual(TestSettingsBase.DEFAULT_TEST_CONVERTER, _instance.TestConverter.Name);

            var expected = new PlainClass()
            {
                Name = "Testing"
            };

            _instance.TestConverter = expected;

            Assert.IsNotNull(_instance.TestConverter);
            Assert.AreEqual(expected.Name, _instance.TestConverter.Name);

            _mockPersistence
                .Verify((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_CONVERTER)), Times.Once);
        }

        [TestMethod]
        public void ConverterSettingPersistenceTest()
        {
            var expectedValue = new PlainClass()
            {
                Name = "Testing"
            };

            _mockPersistence.Setup((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_CONVERTER)))
                .Callback<SettingValue>(
                (pSetting) =>
                {
                    pSetting.Value = expectedValue.Name;
                });

            Assert.AreEqual(expectedValue.Name, _instance.TestConverter.Name);

            // Check cached value
            Assert.IsNotNull(_instance.TestConverter);
            Assert.AreEqual(expectedValue.Name, _instance.TestConverter.Name);

            _mockPersistence
                .Verify((p) => p.LoadFromDataStore(It.Is<SettingValue>(v => v.Name == SETTING_CONVERTER)), Times.Once);
        }


        [TestMethod]
        public void IsInCacheTest()
        {
            Assert.IsFalse(_instance.IsInCacheAccess(SETTING_STRING));

            _instance.TestString = "Test";

            Assert.IsTrue(_instance.IsInCacheAccess(SETTING_STRING));
        }

        [TestMethod]
        public void FindSettingValueTest()
        {
            Assert.IsNull(_instance.FindSettingValueAccess(SETTING_STRING));

            _instance.TestString = "Test";

            var settingValue = _instance.FindSettingValueAccess(SETTING_STRING);
            Assert.IsNotNull(settingValue);
            Assert.AreEqual(SETTING_STRING, settingValue.Name);
            Assert.AreEqual("Test", settingValue.Value);
            Assert.IsNull(settingValue.ConvertedValue);
        }

        [TestMethod]
        public void ClearCachedSetting()
        {
            _instance.TestString = "Test";
            Assert.IsTrue(_instance.IsInCacheAccess(SETTING_STRING));

            _instance.ClearCachedSettingsAccess();

            Assert.IsFalse(_instance.IsInCacheAccess(SETTING_STRING));
        }

        [TestMethod]
        public void ClearSettingTest()
        {
            _instance.TestString = "Test";
            Assert.IsTrue(_instance.IsInCacheAccess(SETTING_STRING));

            Assert.AreEqual(1, _instance.ClearSettingAccess(SETTING_STRING));

            // Second removal should not remove any values.
            Assert.AreEqual(0, _instance.ClearSettingAccess(SETTING_STRING));

            Assert.IsFalse(_instance.IsInCacheAccess(SETTING_STRING));
        }

        #region TestSettingsBase

        private sealed class TestSettingsBase : SettingsBase
        {
            public const string DEFAULT_TEST_CONVERTER = "Default";

            public TestSettingsBase(ISettingsPersistence persistence) :
                base(persistence)
            {
            }

            public string TestString
            {
                get
                {
                    return GetSettingValue<string>(SETTING_STRING, null, null);
                }
                set
                {
                    SetSettingValue(SETTING_STRING, value);
                }
            }

            public DateTime TestDate
            {
                get
                {
                    return GetSettingValue<DateTime>(SETTING_DATETIME, null, null);
                }
                set
                {
                    SetSettingValue(SETTING_DATETIME, value.ToString(CultureInfo.InvariantCulture), value);
                }
            }

            public float TestFloat
            {
                get
                {
                    return GetSettingValue<float>(SETTING_FLOAT, null, null);
                }
                set
                {
                    SetSettingValue(SETTING_FLOAT, value.ToString(), value);
                }
            }

            public int TestInt
            {
                get
                {
                    return GetSettingValue<int>(SETTING_INT, null, null);
                }
                set
                {
                    SetSettingValue(SETTING_INT, value.ToString(), value);
                }
            }

            public bool TestBool
            {
                get
                {
                    return GetSettingValue<bool>(SETTING_BOOL, null, null);
                }
                set
                {
                    SetSettingValue(SETTING_BOOL, value.ToString(), value);
                }
            }

            public double TestDouble
            {
                get
                {
                    return GetSettingValue<double>(SETTING_DOUBLE, null, null);
                }
                set
                {
                    SetSettingValue(SETTING_DOUBLE, value.ToString(), value);
                }
            }

            public PlainClass TestConverter
            {
                get
                {
                    return GetSettingValue(SETTING_CONVERTER,
                        // Conversion
                        (input) =>
                        {
                            return new PlainClass()
                            {
                                Name = input
                            };
                        },

                        // Default
                        () =>
                        {
                            return DEFAULT_TEST_CONVERTER;
                        }
                    );
                }

                set
                {
                    SetSettingValue(SETTING_CONVERTER, value?.Name, value);
                }
            }

            public bool IsInCacheAccess(string propertyName)
            {
                return IsInCache(propertyName);
            }

            public SettingValue FindSettingValueAccess(string propertyName)
            {
                return FindSettingValue(propertyName);
            }

            public void ClearCachedSettingsAccess()
            {
                ClearCachedSettings();
            }

            public int ClearSettingAccess(string propertyName)
            {
                return ClearSetting(propertyName);
            }
        }

        #endregion

        #region Poco

        public class PlainClass
        {
            public string Name { get; set; }
        }

        #endregion
    }
}
