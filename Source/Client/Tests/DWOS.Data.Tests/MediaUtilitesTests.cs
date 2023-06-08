using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DWOS.Data.Tests
{
    [TestClass]
    public sealed class MediaUtilitesTests
    {
        [TestMethod]
        public void ResizeTest()
        {
            Size currentSize;
            Size maximumSize;
            Size result;

            // No change - smaller than max
            currentSize = new Size(95, 99);
            maximumSize = new Size(100, 100);
            result = MediaUtilities.Resize(currentSize, maximumSize);

            Assert.AreEqual(95, result.Width);
            Assert.AreEqual(99, result.Height);
         
            // No change - equal to max
            currentSize = new Size(100, 100);
            maximumSize = new Size(100, 100);
            result = MediaUtilities.Resize(currentSize, maximumSize);

            Assert.AreEqual(100, result.Width);
            Assert.AreEqual(100, result.Height);

            // Width is larger
            currentSize = new Size(150, 100);
            maximumSize = new Size(100, 100);
            result = MediaUtilities.Resize(currentSize, maximumSize);

            Assert.AreEqual(100, result.Width);
            Assert.AreEqual(66, result.Height);

            currentSize = new Size(500, 400);
            maximumSize = new Size(100, 25);
            result = MediaUtilities.Resize(currentSize, maximumSize);

            Assert.AreEqual(31, result.Width);
            Assert.AreEqual(25, result.Height);

            // Height is larger
            currentSize = new Size(100, 150);
            maximumSize = new Size(100, 100);
            result = MediaUtilities.Resize(currentSize, maximumSize);

            Assert.AreEqual(66, result.Width);
            Assert.AreEqual(100, result.Height);

            // Height is larger
            currentSize = new Size(400, 500);
            maximumSize = new Size(25, 100);
            result = MediaUtilities.Resize(currentSize, maximumSize);

            Assert.AreEqual(25, result.Width);
            Assert.AreEqual(31, result.Height);

            // Both dimensions are equal
            currentSize = new Size(100, 100);
            maximumSize = new Size(95, 95);
            result = MediaUtilities.Resize(currentSize, maximumSize);

            Assert.AreEqual(95, result.Width);
            Assert.AreEqual(95, result.Height);

            currentSize = new Size(100, 100);
            maximumSize = new Size(95, 95);
            result = MediaUtilities.Resize(currentSize, maximumSize);

            Assert.AreEqual(95, result.Width);
            Assert.AreEqual(95, result.Height);

            // Height is okay, width is smaller than max
            currentSize = new Size(100, 100);
            maximumSize = new Size(95, 100);
            result = MediaUtilities.Resize(currentSize, maximumSize);

            Assert.AreEqual(95, result.Width);
            Assert.AreEqual(95, result.Height);

            // Width is okay, height is smaller than max
            currentSize = new Size(100, 100);
            maximumSize = new Size(100, 95);
            result = MediaUtilities.Resize(currentSize, maximumSize);

            Assert.AreEqual(95, result.Width);
            Assert.AreEqual(95, result.Height);
        }

        [TestMethod]
        public void PixelsToTwipsTest()
        {
            const int dpi = 60;
            Assert.AreEqual(24, MediaUtilities.PixelsToTwips(1, dpi));
            Assert.AreEqual(48, MediaUtilities.PixelsToTwips(2, dpi));

            Assert.AreEqual(-24, MediaUtilities.PixelsToTwips(-1, dpi));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PixelsToTwipsNegativeDpiTest()
        {
            MediaUtilities.PixelsToTwips(1, -1.0f);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PixelsToTwipsZeroDpiTest()
        {
            MediaUtilities.PixelsToTwips(1, 0f);
        }
    }
}
