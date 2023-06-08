using System;
using DWOS.Shared.Data;
using DWOS.Shared.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;

namespace DWOS.Shared.Tests.Settings
{
    [TestClass]
    public class RegistrySettingsBaseTests
    {
        private const bool DEFAULT_BOOL = true;
        private const double DEFAULT_DOUBLE = 25.005d;
        private const float DEFAULT_FLOAT = 10.001f;
        private const int DEFAULT_INT = 11;
        private const string DEFAULT_STRING = "Default";
        public const string DEFAULT_CONVERTER_VALUE = "90";
        private const string DEFAULT_FIELD_NAME_TEST = "A";

        [TestMethod]
        public void RegistryTest()
        {
            // Test - defaults
            var defaultSettings = new TestSettings();
            defaultSettings.Reset();
            defaultSettings.LoadSettings();

            // Default DateTime is not currently supported
            Assert.AreEqual(DEFAULT_BOOL, defaultSettings.Bool);
            AssertEquivalence(DEFAULT_DOUBLE, defaultSettings.Double);
            AssertEquivalence(DEFAULT_FLOAT, defaultSettings.Float);
            Assert.AreEqual(DEFAULT_INT, defaultSettings.Int);
            Assert.AreEqual(DEFAULT_STRING, defaultSettings.String);
            Assert.IsNotNull(defaultSettings.ConverterTest);
            Assert.AreEqual(DEFAULT_CONVERTER_VALUE, defaultSettings.ConverterTest.Value.ToString());
            Assert.IsNotNull(defaultSettings.InnerSettings);
            Assert.AreEqual(DEFAULT_FIELD_NAME_TEST, defaultSettings.FieldNameTest);

            // Test - settings persistence
            var expectedSettings = new TestSettings
            {
                DateTime = DateTime.MaxValue.Date, // Does not currently persist milliseconds
                Bool = true,
                Double = 5.0D, // Cannot currently persist maximum value
                Float = 6.5f, // Can persist maximum float but comparison fails
                Int = int.MaxValue,
                String = "Test",
                ConverterTest = new TestObject {Value = 4},
                FieldNameTest = "B"
            };

            expectedSettings.InnerSettings = expectedSettings.NewNestedSettings();
            expectedSettings.InnerSettings.NestedString = "Nested";

            expectedSettings.Save();

            var actualSettings = new TestSettings();
            actualSettings.LoadSettings();
            Assert.AreEqual(expectedSettings.DateTime, actualSettings.DateTime);
            Assert.AreEqual(expectedSettings.Bool, actualSettings.Bool);
            AssertEquivalence(expectedSettings.Double, actualSettings.Double);
            AssertEquivalence(expectedSettings.Float, actualSettings.Float);
            Assert.AreEqual(expectedSettings.Int, actualSettings.Int);
            Assert.AreEqual(expectedSettings.String, actualSettings.String);
            Assert.IsNotNull(actualSettings.ConverterTest);
            Assert.AreEqual(expectedSettings.ConverterTest.Value, actualSettings.ConverterTest.Value);
            Assert.IsNotNull(expectedSettings.InnerSettings);
            Assert.AreEqual(expectedSettings.InnerSettings.NestedString, actualSettings.InnerSettings.NestedString);
            Assert.AreEqual(expectedSettings.FieldNameTest, actualSettings.FieldNameTest);

            // Test reload for nested settings
            expectedSettings.LoadSettings();
            Assert.IsNotNull(expectedSettings.InnerSettings);
            Assert.AreEqual(expectedSettings.InnerSettings.NestedString, actualSettings.InnerSettings.NestedString);
        }

        private static void AssertEquivalence(float expected, float actual, float threshold = 0.0000001f)
        {
            var difference = Math.Abs(expected - actual);

            var msg =
                $"Expected is not within threshold of actual.\nExpected: {expected}\nActual: {actual}\nDifference: {difference}\nThreshold: {threshold}";
            Assert.IsTrue(difference < threshold, msg);
        }

        private static void AssertEquivalence(double expected, double actual, double threshold = 0.0000001d)
        {
            var difference = Math.Abs(expected - actual);

            var msg =
                $"Expected is not within threshold of actual.\nExpected: {expected}\nActual: {actual}\nDifference: {difference}\nThreshold: {threshold}";
            Assert.IsTrue(difference < threshold, msg);
        }

        private class TestSettings : RegistrySettingsBase
        {
            protected override string RegistryKeyName => "SOFTWARE\\DS2\\DWOSTest";

            protected override RegistryKey RegistryHive => Registry.CurrentUser;

            [DataColumn]
            public DateTime DateTime { get; set; }

            [DataColumn(DefaultValue = DEFAULT_FLOAT)]
            public float Float { get; set; }

            [DataColumn(DefaultValue = DEFAULT_INT)]
            public int Int { get; set; }

            [DataColumn(DefaultValue = DEFAULT_BOOL)]
            public bool Bool { get; set; }

            [DataColumn(DefaultValue = DEFAULT_DOUBLE)]
            public double Double { get; set; }

            [DataColumn(DefaultValue = DEFAULT_STRING)]
            public string String { get; set; }

            [DataColumn(DefaultValue = DEFAULT_CONVERTER_VALUE, FieldConverterType = typeof(TestObjectConverter))]
            public TestObject ConverterTest { get; set; }

            [DataColumn]
            public NestedSettings InnerSettings { get; set; }

            [DataColumn(DefaultValue = DEFAULT_FIELD_NAME_TEST, FieldName = "FieldName_Test")]
            public string FieldNameTest { get; set; }

            public void LoadSettings()
            {
                Load();
            }

            public void Reset()
            {
                RegistryHive.DeleteSubKeyTree(RegistryKeyName, false);
            }


            public void Save()
            {
                SaveSettings();
            }

            public NestedSettings NewNestedSettings()
            {
                var nestedKey = RegistryKeyName + @"\" + nameof(InnerSettings);
                return new NestedSettings(RegistryHive, nestedKey);
            }
        }

        private class TestObject
        {
            public int Value { get; set; }
        }

        private class TestObjectConverter : IFieldConverter
        {
            public object ConvertFromField(object value)
            {
                if (value == null)
                {
                    return null;
                }

                // Assumption - value is string
                return new TestObject
                {
                    Value = int.Parse(value.ToString())
                };
            }

            public object ConvertToField(object value)
            {
                var valueAsObj = value as TestObject;

                return valueAsObj?.Value.ToString();
            }
        }

        private class NestedSettings : NestedRegistrySettingsBase
        {
            [DataColumn]
            public string NestedString { get; set; }

            public NestedSettings(RegistryKey hive, string baseKey) : base(hive, baseKey)
            {
            }
        }
    }
}