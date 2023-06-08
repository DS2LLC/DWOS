using DWOS.Data.Order;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DWOS.Data.Tests.Order
{
    [TestClass]
    public sealed class LoadCapacityTests
    {
        [TestMethod]
        public void NumberOfFixturesTest()
        {
            int loadCapacity;
            int orderQuantity;
            decimal variancePercentage;
            int expected;
            int actual;

            // Cases - w/o variance
            loadCapacity = 5;
            orderQuantity = 10;
            variancePercentage = 0M;
            expected = 2;
            actual = LoadCapacity.FixturesFromQuantity(orderQuantity, loadCapacity, variancePercentage);
            Assert.AreEqual(expected, actual);

            loadCapacity = 5;
            orderQuantity = 11;
            variancePercentage = 0M;
            expected = 3;
            actual = LoadCapacity.FixturesFromQuantity(orderQuantity, loadCapacity, variancePercentage);
            Assert.AreEqual(expected, actual);

            loadCapacity = 10;
            orderQuantity = 9;
            variancePercentage = 0M;
            expected = 1;
            actual = LoadCapacity.FixturesFromQuantity(orderQuantity, loadCapacity, variancePercentage);
            Assert.AreEqual(expected, actual);

            loadCapacity = 1;
            orderQuantity = 9;
            variancePercentage = 0M;
            expected = 9;
            actual = LoadCapacity.FixturesFromQuantity(orderQuantity, loadCapacity, variancePercentage);
            Assert.AreEqual(expected, actual);

            // Cases - w/ variance
            loadCapacity = 4;
            orderQuantity = 10;
            variancePercentage = 0.2M;
            expected = 3;
            actual = LoadCapacity.FixturesFromQuantity(orderQuantity, loadCapacity, variancePercentage);
            Assert.AreEqual(expected, actual);

            loadCapacity = 4;
            orderQuantity = 10;
            variancePercentage = 0.25M;
            expected = 2;
            actual = LoadCapacity.FixturesFromQuantity(orderQuantity, loadCapacity, variancePercentage);
            Assert.AreEqual(expected, actual);

            loadCapacity = 5;
            orderQuantity = 10;
            variancePercentage = 1M;
            expected = 1;
            actual = LoadCapacity.FixturesFromQuantity(orderQuantity, loadCapacity, variancePercentage);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NumberOfFixturesInvalidVarianceTest()
        {
            LoadCapacity.FixturesFromQuantity(1, 1, -0.1M);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NumberOfFixturesInvalidLoadCapacityTest()
        {
            LoadCapacity.FixturesFromQuantity(1, 0, 0M);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NumberOfFixturesInvalidQuantityTest()
        {
            LoadCapacity.FixturesFromQuantity(-1, 1, 0M);
        }

        [TestMethod]
        public void NumberOfFixturesDecimalTest()
        {
            decimal loadCapacity;
            decimal orderWeight;
            decimal variancePercentage;
            int expected;
            int actual;

            loadCapacity = 5.25M;
            orderWeight = 10.5M;
            variancePercentage = 0M;
            expected = 2;
            actual = LoadCapacity.FixturesFromWeight(orderWeight, loadCapacity, variancePercentage);
            Assert.AreEqual(expected, actual);
        }
    }
}
