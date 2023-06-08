using System.Globalization;
using DWOS.UI.Utilities.Convertors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.UI.Tests.Utilities.Convertors
{
    [TestClass]
    public class DefaultNumericConverterTests
    {
        [TestMethod]
        public void ConvertDoubleTest()
        {
            var culture = CultureInfo.CurrentCulture;
            var target = new DefaultNumericConverter();
            const double expected = 5.0D;

            var actualString = target.Convert(expected, typeof(string), null, culture) as string;
            Assert.AreEqual(expected.ToString(culture), actualString);

            var actual = target.ConvertBack(actualString, typeof(double), null, culture) as double? ?? default(double);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertFloatTest()
        {
            var culture = CultureInfo.CurrentCulture;
            var target = new DefaultNumericConverter();
            const float expected = 5.0F;

            var actualString = target.Convert(expected, typeof(string), null, culture) as string;
            Assert.AreEqual(expected.ToString(culture), actualString);

            var actual = target.ConvertBack(actualString, typeof(float), null, culture) as float? ?? default(float);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertByteTest()
        {
            var culture = CultureInfo.CurrentCulture;
            var target = new DefaultNumericConverter();
            const byte expected = 5;

            var actualString = target.Convert(expected, typeof(string), null, culture) as string;
            Assert.AreEqual(expected.ToString(culture), actualString);

            var actual = target.ConvertBack(actualString, typeof(byte), null, culture) as byte? ?? default(byte);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertCharTest()
        {
            var culture = CultureInfo.CurrentCulture;
            var target = new DefaultNumericConverter();
            const char expected = '5';

            var actualString = target.Convert(expected, typeof(string), null, culture) as string;
            Assert.AreEqual(expected.ToString(culture), actualString);

            var actual = target.ConvertBack(actualString, typeof(char), null, culture) as char? ?? default(char);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertShortTest()
        {
            var culture = CultureInfo.CurrentCulture;
            var target = new DefaultNumericConverter();
            const short expected = 5;

            var actualString = target.Convert(expected, typeof(string), null, culture) as string;
            Assert.AreEqual(expected.ToString(culture), actualString);

            var actual = target.ConvertBack(actualString, typeof(short), null, culture) as short? ?? default(short);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertUShortTest()
        {
            var culture = CultureInfo.CurrentCulture;
            var target = new DefaultNumericConverter();
            const ushort expected = 5;

            var actualString = target.Convert(expected, typeof(string), null, culture) as string;
            Assert.AreEqual(expected.ToString(culture), actualString);

            var actual = target.ConvertBack(actualString, typeof(ushort), null, culture) as ushort? ?? default(ushort);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertIntTest()
        {
            var culture = CultureInfo.CurrentCulture;
            var target = new DefaultNumericConverter();
            const int expected = 5;

            var actualString = target.Convert(expected, typeof(string), null, culture) as string;
            Assert.AreEqual(expected.ToString(culture), actualString);

            var actual = target.ConvertBack(actualString, typeof(int), null, culture) as int? ?? default(int);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertUIntTest()
        {
            var culture = CultureInfo.CurrentCulture;
            var target = new DefaultNumericConverter();
            const uint expected = 5;

            var actualString = target.Convert(expected, typeof(string), null, culture) as string;
            Assert.AreEqual(expected.ToString(culture), actualString);

            var actual = target.ConvertBack(actualString, typeof(uint), null, culture) as uint? ?? default(uint);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertLongTest()
        {
            var culture = CultureInfo.CurrentCulture;
            var target = new DefaultNumericConverter();
            const long expected = 5L;

            var actualString = target.Convert(expected, typeof(string), null, culture) as string;
            Assert.AreEqual(expected.ToString(culture), actualString);

            var actual = target.ConvertBack(actualString, typeof(long), null, culture) as long? ?? default(long);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertULongTest()
        {
            var culture = CultureInfo.CurrentCulture;
            var target = new DefaultNumericConverter();
            const ulong expected = 5L;

            var actualString = target.Convert(expected, typeof(string), null, culture) as string;
            Assert.AreEqual(expected.ToString(culture), actualString);

            var actual = target.ConvertBack(actualString, typeof(ulong), null, culture) as ulong? ?? default(ulong);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertDecimalTest()
        {
            var culture = CultureInfo.CurrentCulture;
            var target = new DefaultNumericConverter();
            const decimal expected = 0.5M;

            var actualString = target.Convert(expected, typeof(string), null, culture) as string;
            Assert.AreEqual(expected.ToString(culture), actualString);

            var actual = target.ConvertBack(actualString, typeof(decimal), null, culture) as decimal? ?? default(decimal);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertSByteTest()
        {
            var culture = CultureInfo.CurrentCulture;
            var target = new DefaultNumericConverter();
            const sbyte expected = 5;

            var actualString = target.Convert(expected, typeof(string), null, culture) as string;
            Assert.AreEqual(expected.ToString(culture), actualString);

            var actual = target.ConvertBack(actualString, typeof(sbyte), null, culture) as sbyte? ?? default(sbyte);
            Assert.AreEqual(expected, actual);
        }
    }
}
