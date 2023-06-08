using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.Data.Tests.Extensions
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void IsValidEmailTest()
        {
            // Valid cases
            Assert.IsTrue("test123456@example.com".IsValidEmail());
            Assert.IsTrue("Test123456@example.com".IsValidEmail());
            Assert.IsTrue("test.123456@example.com".IsValidEmail());
            Assert.IsTrue("test'@example.com".IsValidEmail());

            // Valid cases from RFC 3696
            // http://tools.ietf.org/html/rfc3696 
            Assert.IsTrue(@"Abcdef@example.com".IsValidEmail());
            Assert.IsTrue(@"Fred\ Bloggs@example.com".IsValidEmail());
            Assert.IsTrue(@"Fred Bloggs@example.com".IsValidEmail());
            Assert.IsTrue(@"Joe.\\Blow@example.com".IsValidEmail());
            Assert.IsTrue(@"user+mailbox@example.com".IsValidEmail());
            Assert.IsTrue(@"customer / department = shipping@example.com".IsValidEmail());
            Assert.IsTrue(@"!def!xyz % abc@example.com".IsValidEmail());
            Assert.IsTrue(@"_somename@example.com".IsValidEmail());

            // Invalid cases
            Assert.IsFalse(string.Empty.IsValidEmail());
            Assert.IsFalse("test".IsValidEmail());
            Assert.IsFalse("@example.com".IsValidEmail());
            Assert.IsFalse("@".IsValidEmail());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsValidEmailNullTest()
        {
            string test = null;
            test.IsValidEmail();
        }

        [TestMethod]
        public void IncrementTest()
        {
            // Numeric cases
            Assert.AreEqual("1", "0".Increment());
            Assert.AreEqual("9", "8".Increment());
            Assert.AreEqual("10", "9".Increment());
            Assert.AreEqual("11", "10".Increment());
            Assert.AreEqual("12", "11".Increment());
            Assert.AreEqual("20", "19".Increment());
            Assert.AreEqual("100", "99".Increment());
            Assert.AreEqual("A1", "A0".Increment());
            Assert.AreEqual("A10", "A9".Increment());
            Assert.AreEqual("1", "0 ".Increment());

            // Numeric case w/ padding
            Assert.AreEqual("01", "00".Increment());
            Assert.AreEqual("09", "08".Increment());
            Assert.AreEqual("10", "09".Increment());
            Assert.AreEqual("010", "009".Increment());
            Assert.AreEqual("011", "010".Increment());
            Assert.AreEqual("012", "011".Increment());
            Assert.AreEqual("020", "019".Increment());
            Assert.AreEqual("100", "099".Increment());
            Assert.AreEqual("A01", "A00".Increment());
            Assert.AreEqual("A10", "A09".Increment());
            Assert.AreEqual("01", "00 ".Increment());

            // Text cases
            Assert.AreEqual("b", "a".Increment());
            Assert.AreEqual("B", "A".Increment());
            Assert.AreEqual("Z", "Y".Increment());
            Assert.AreEqual("AA", "Z".Increment());
            Assert.AreEqual("ZZ", "ZY".Increment());
            Assert.AreEqual("AAA", "ZZ".Increment());
            Assert.AreEqual("A-B", "A-A".Increment());
            Assert.AreEqual("A-AA", "A-Z".Increment());
            Assert.AreEqual("b", "a ".Increment());

            // Text cases w/ padding
            Assert.AreEqual("0b", "0a".Increment());
            Assert.AreEqual("0B", "0A".Increment());
            Assert.AreEqual("0Z", "0Y".Increment());
            Assert.AreEqual("AA", "0Z".Increment());
            Assert.AreEqual("0AA", "00Z".Increment());
            Assert.AreEqual("0ZZ", "0ZY".Increment());
            Assert.AreEqual("AAA", "0ZZ".Increment());
            Assert.AreEqual("A-0B", "A-0A".Increment());
            Assert.AreEqual("A-AA", "A-0Z".Increment());
            Assert.AreEqual("0b", "0a ".Increment());

            // Potential for overflow
            Assert.AreEqual(((long)int.MaxValue + 1).ToString(), int.MaxValue.ToString().Increment());
            Assert.AreEqual(long.MaxValue.ToString(), long.MaxValue.ToString().Increment());

            const string greaterThanMaximumLong = "9223372036854775808";
            Assert.AreEqual(greaterThanMaximumLong, greaterThanMaximumLong.Increment());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IncrementNullTest()
        {
            string test = null;
            test.Increment();
        }
    }
}
