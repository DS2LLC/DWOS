using DWOS.Data.Datasets;
using DWOS.Data.Order;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DWOS.Data.Tests.Order
{
    [TestClass]
    public class PricePointTests
    {
        [TestMethod]
        public void FromTest()
        {
            using (var dtPrice = new OrdersDataSet.PartProcessPriceSummaryDataTable())
            {
                PricePoint result;
                // Null Prices
                var source = dtPrice.NewPartProcessPriceSummaryRow();
                source.PriceUnit = OrderPrice.enumPriceUnit.Each.ToString();

                result = PricePoint.From(source);
                Assert.IsNull(result.MinQuantity);
                Assert.IsNull(result.MaxQuantity);
                Assert.AreEqual(OrderPrice.enumPriceUnit.Each, result.PriceUnit);

                source.PriceUnit = OrderPrice.enumPriceUnit.Lot.ToString();
                result = PricePoint.From(source);
                Assert.IsNull(result.MinQuantity);
                Assert.IsNull(result.MaxQuantity);
                Assert.AreEqual(OrderPrice.enumPriceUnit.Lot, result.PriceUnit);

                // Non-Null Prices
                source.MinValue = "0";
                source.MaxValue = "100";
                source.PriceUnit = OrderPrice.enumPriceUnit.Each.ToString();

                result = PricePoint.From(source);
                Assert.AreEqual(0, result.MinQuantity);
                Assert.AreEqual(100, result.MaxQuantity);
                Assert.AreEqual(OrderPrice.enumPriceUnit.Each, result.PriceUnit);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromNullTest()
        {
            PricePoint.From(null);
        }

        [TestMethod]
        public void EqualsTest()
        {
            PricePoint left;
            PricePoint right;

            // Null right
            left = PricePoint.FromQuantities(null, null, OrderPrice.enumPriceUnit.Each);
            Assert.IsFalse(left.Equals(null));

            // Null Prices - Both
            left = PricePoint.FromQuantities(null, null, OrderPrice.enumPriceUnit.Each);
            right = PricePoint.FromQuantities(null, null, OrderPrice.enumPriceUnit.Each);
            Assert.IsTrue(left.Equals(right));
            Assert.IsTrue(right.Equals(left));
            Assert.IsTrue(left == right);
            Assert.IsFalse(left != right);
            Assert.AreEqual(left.GetHashCode(), right.GetHashCode());

            // Null Prices - left
            left = PricePoint.FromQuantities(null, null, OrderPrice.enumPriceUnit.Each);
            right = PricePoint.FromQuantities(null, 5, OrderPrice.enumPriceUnit.Each);
            Assert.IsFalse(left.Equals(right));
            Assert.IsFalse(right.Equals(left));
            Assert.IsFalse(left == right);
            Assert.IsTrue(left != right);
            Assert.AreNotEqual(left.GetHashCode(), right.GetHashCode());

            // Null Prices - right
            left = PricePoint.FromQuantities(null, 5, OrderPrice.enumPriceUnit.Each);
            right = PricePoint.FromQuantities(null, null, OrderPrice.enumPriceUnit.Each);
            Assert.IsFalse(left.Equals(right));
            Assert.IsFalse(right.Equals(left));
            Assert.IsFalse(left == right);
            Assert.IsTrue(left != right);
            Assert.AreNotEqual(left.GetHashCode(), right.GetHashCode());

            // Non-Null Prices
            left = PricePoint.FromQuantities(0, null, OrderPrice.enumPriceUnit.Each);
            right = PricePoint.FromQuantities(0, null, OrderPrice.enumPriceUnit.Each);
            Assert.IsTrue(left.Equals(right));
            Assert.IsTrue(right.Equals(left));
            Assert.IsTrue(left == right);
            Assert.IsFalse(left != right);
            Assert.AreEqual(left.GetHashCode(), right.GetHashCode());

            left = PricePoint.FromQuantities(0, 10, OrderPrice.enumPriceUnit.Each);
            right = PricePoint.FromQuantities(0, 10, OrderPrice.enumPriceUnit.Each);
            Assert.IsTrue(left.Equals(right));
            Assert.IsTrue(right.Equals(left));
            Assert.IsTrue(left == right);
            Assert.IsFalse(left != right);
            Assert.AreEqual(left.GetHashCode(), right.GetHashCode());

            left = PricePoint.FromQuantities(null, 10, OrderPrice.enumPriceUnit.Each);
            right = PricePoint.FromQuantities(null, 10, OrderPrice.enumPriceUnit.Each);
            Assert.IsTrue(left.Equals(right));
            Assert.IsTrue(right.Equals(left));
            Assert.IsTrue(left == right);
            Assert.IsFalse(left != right);
            Assert.AreEqual(left.GetHashCode(), right.GetHashCode());

            // Different price unit
            left = PricePoint.FromQuantities(0, null, OrderPrice.enumPriceUnit.Each);
            right = PricePoint.FromQuantities(0, null, OrderPrice.enumPriceUnit.Lot);
            Assert.IsFalse(left.Equals(right));
            Assert.IsFalse(right.Equals(left));
            Assert.IsFalse(left == right);
            Assert.IsTrue(left != right);
            Assert.AreNotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [TestMethod]
        public void MatchesTest()
        {
            PricePoint target;
            int qty;
            OrderPrice.enumPriceUnit priceUnit;

            // Null MinQuantity
            target = PricePoint.FromQuantities(null, 5, OrderPrice.enumPriceUnit.Each);
            qty = 100;
            priceUnit = OrderPrice.enumPriceUnit.Each;
            Assert.IsTrue(target.Matches(qty, 0M, priceUnit));

            qty = 0;
            priceUnit = OrderPrice.enumPriceUnit.Each;
            Assert.IsTrue(target.Matches(qty, 0M, priceUnit));

            qty = 0;
            priceUnit = OrderPrice.enumPriceUnit.Lot;
            Assert.IsFalse(target.Matches(qty, 0M, priceUnit));

            // Null MaxQuantity
            target = PricePoint.FromQuantities(0, null, OrderPrice.enumPriceUnit.Each);
            qty = 50;
            priceUnit = OrderPrice.enumPriceUnit.Each;
            Assert.IsTrue(target.Matches(qty, 0M, priceUnit));

            target = PricePoint.FromQuantities(50, null, OrderPrice.enumPriceUnit.Each);
            qty = 49;
            priceUnit = OrderPrice.enumPriceUnit.Each;
            Assert.IsFalse(target.Matches(qty, 0M, priceUnit));

            qty = 50;
            priceUnit = OrderPrice.enumPriceUnit.Each;
            Assert.IsTrue(target.Matches(qty, 0M, priceUnit));

            qty = int.MaxValue;
            priceUnit = OrderPrice.enumPriceUnit.Each;
            Assert.IsTrue(target.Matches(qty, 0M, priceUnit));

            qty = int.MaxValue;
            priceUnit = OrderPrice.enumPriceUnit.Lot;
            Assert.IsFalse(target.Matches(qty, 0M, priceUnit));
            Assert.IsTrue(target.MatchesQuantity(qty));

            // Non-Null Values
            target = PricePoint.FromQuantities(100, 200, OrderPrice.enumPriceUnit.Each);

            qty = 0;
            priceUnit = OrderPrice.enumPriceUnit.Each;
            Assert.IsFalse(target.Matches(qty, 0M, priceUnit));

            qty = 99;
            priceUnit = OrderPrice.enumPriceUnit.Each;
            Assert.IsFalse(target.Matches(qty, 0M, priceUnit));

            qty = 100;
            priceUnit = OrderPrice.enumPriceUnit.Each;
            Assert.IsTrue(target.Matches(qty, 0M, priceUnit));

            qty = 200;
            priceUnit = OrderPrice.enumPriceUnit.Each;
            Assert.IsTrue(target.Matches(qty, 0M, priceUnit));

            qty = 201;
            priceUnit = OrderPrice.enumPriceUnit.Each;
            Assert.IsFalse(target.Matches(qty, 0M, priceUnit));

            qty = 200;
            priceUnit = OrderPrice.enumPriceUnit.Lot;
            Assert.IsFalse(target.Matches(qty, 0M, priceUnit));
            Assert.IsTrue(target.MatchesQuantity(qty));
        }
    }
}
