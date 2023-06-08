using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DWOS.Data.Order;
using Moq;
using DWOS.Shared.Utilities;

namespace DWOS.Data.Tests
{
    [TestClass]
    public class OrderPriceTests
    {
        #region CalculatePrice Tests

        [TestMethod]
        public void CalculatePriceEachTest()
        {
            var priceUnit = OrderPrice.enumPriceUnit.Each.ToString();

            decimal basePrice;
            decimal fees;
            int quantity;
            decimal weight;
            decimal actual;

            basePrice = 0M;
            fees = 0M;
            quantity = 0;
            weight = 0M;
            actual = OrderPrice.CalculatePrice(basePrice,
                priceUnit,
                fees,
                quantity,
                weight);

            Assert.AreEqual(0M, actual);

            basePrice = 0M;
            fees = 4M;
            quantity = 0;
            weight = 0M;
            actual = OrderPrice.CalculatePrice(basePrice,
                priceUnit,
                fees,
                quantity,
                weight);

            Assert.AreEqual(4M, actual);

            basePrice = 15M;
            fees = 0M;
            quantity = 1;
            weight = 0M;
            actual = OrderPrice.CalculatePrice(basePrice,
                priceUnit,
                fees,
                quantity,
                weight);

            Assert.AreEqual(15M, actual);

            basePrice = 15M;
            fees = 0M;
            quantity = 0;
            weight = 0M;
            actual = OrderPrice.CalculatePrice(basePrice,
                priceUnit,
                fees,
                quantity,
                weight);

            Assert.AreEqual(0M, actual);

            basePrice = 1.25M;
            fees = 0M;
            quantity = 4;
            weight = 0M;
            actual = OrderPrice.CalculatePrice(basePrice,
                priceUnit,
                fees,
                quantity,
                weight);

            Assert.AreEqual(5M, actual);

            basePrice = 1.25M;
            fees = 25.009M;
            quantity = 4;
            weight = 0M;
            actual = OrderPrice.CalculatePrice(basePrice,
                priceUnit,
                fees,
                quantity,
                weight);

            Assert.AreEqual(30.009M, actual);
        }

        [TestMethod]
        public void CalculatePriceEachByWeightTest()
        {
            var priceUnit = OrderPrice.enumPriceUnit.EachByWeight.ToString();

            decimal basePrice;
            decimal fees;
            int quantity;
            decimal weight;
            decimal actual;

            basePrice = 0M;
            fees = 0M;
            quantity = 0;
            weight = 0M;
            actual = OrderPrice.CalculatePrice(basePrice,
                priceUnit,
                fees,
                quantity,
                weight);

            Assert.AreEqual(0M, actual);

            basePrice = 0M;
            fees = 4M;
            quantity = 0;
            weight = 0M;
            actual = OrderPrice.CalculatePrice(basePrice,
                priceUnit,
                fees,
                quantity,
                weight);

            Assert.AreEqual(4M, actual);

            basePrice = 15M;
            fees = 0M;
            quantity = 1;
            weight = 0M;
            actual = OrderPrice.CalculatePrice(basePrice,
                priceUnit,
                fees,
                quantity,
                weight);

            Assert.AreEqual(0M, actual);

            basePrice = 1.25M;
            fees = 0M;
            quantity = 4;
            weight = 4M;
            actual = OrderPrice.CalculatePrice(basePrice,
                priceUnit,
                fees,
                quantity,
                weight);

            Assert.AreEqual(5M, actual);

            basePrice = 1.25M;
            fees = 25.009M;
            quantity = 8;
            weight = 4M;
            actual = OrderPrice.CalculatePrice(basePrice,
                priceUnit,
                fees,
                quantity,
                weight);

            Assert.AreEqual(30.009M, actual);

            basePrice = 5M;
            fees = 0M;
            quantity = 1;
            weight = 9.05M;
            actual = OrderPrice.CalculatePrice(basePrice,
                priceUnit,
                fees,
                quantity,
                weight);

            Assert.AreEqual(45.25M, actual);
        }

        [TestMethod]
        public void CalculatePriceLotTest()
        {
            var priceUnitLot = OrderPrice.enumPriceUnit.Lot.ToString();
            var priceUnitLotByWeight = OrderPrice.enumPriceUnit.LotByWeight.ToString();
            decimal basePrice;
            decimal fees;
            int quantity;
            decimal weight;
            decimal actualLot;
            decimal actualLotByWeight;

            basePrice = 0M;
            fees = 0M;
            quantity = 0;
            weight = 0M;

            actualLot = OrderPrice.CalculatePrice(basePrice,
                priceUnitLot,
                fees,
                quantity,
                weight);

            actualLotByWeight = OrderPrice.CalculatePrice(basePrice,
                priceUnitLotByWeight,
                fees,
                quantity,
                weight);

            Assert.AreEqual(0M, actualLot);
            Assert.AreEqual(actualLot, actualLotByWeight);

            basePrice = 0M;
            fees = 4M;
            quantity = 0;
            weight = 0M;

            actualLot = OrderPrice.CalculatePrice(basePrice,
                priceUnitLot,
                fees,
                quantity,
                weight);

            actualLotByWeight = OrderPrice.CalculatePrice(basePrice,
                priceUnitLotByWeight,
                fees,
                quantity,
                weight);

            Assert.AreEqual(4M, actualLot);
            Assert.AreEqual(actualLot, actualLotByWeight);

            basePrice = 50M;
            fees = 0M;
            quantity = 5;
            weight = 30M;

            actualLot = OrderPrice.CalculatePrice(basePrice,
                priceUnitLot,
                fees,
                quantity,
                weight);

            actualLotByWeight = OrderPrice.CalculatePrice(basePrice,
                priceUnitLotByWeight,
                fees,
                quantity,
                weight);

            Assert.AreEqual(50M, actualLot);
            Assert.AreEqual(actualLot, actualLotByWeight);

            basePrice = 5.125M;
            fees = 0.875M;
            quantity = 5;
            weight = 30M;

            actualLot = OrderPrice.CalculatePrice(basePrice,
                priceUnitLot,
                fees,
                quantity,
                weight);

            actualLotByWeight = OrderPrice.CalculatePrice(basePrice,
                priceUnitLotByWeight,
                fees,
                quantity,
                weight);

            Assert.AreEqual(6M, actualLot);
            Assert.AreEqual(actualLot, actualLotByWeight);
        }

        #endregion

        #region CalculateFees Tests

        [TestMethod]
        public void CalculateFeesEachTest()
        {
            var priceUnit = OrderPrice.enumPriceUnit.Each.ToString();

            string feeType;
            decimal charge;
            decimal basePrice;
            int partQuantity;
            decimal weight;
            decimal actual;

            feeType = "unrecognized";
            charge = 5.25M;
            basePrice = 1M;
            partQuantity = 1;
            weight = 1M;
            actual = OrderPrice.CalculateFees(feeType,
                charge,
                basePrice,
                partQuantity,
                priceUnit,
                weight);

            Assert.AreEqual(5.25M, actual);

            feeType = OrderPrice.enumFeeType.Fixed.ToString();
            charge = 5.25M;
            basePrice = 1M;
            partQuantity = 1;
            weight = 1M;
            actual = OrderPrice.CalculateFees(feeType,
                charge,
                basePrice,
                partQuantity,
                priceUnit,
                weight);

            Assert.AreEqual(5.25M, actual);

            feeType = OrderPrice.enumFeeType.Percentage.ToString();
            charge = 5.25M;
            basePrice = 2M;
            partQuantity = 2;
            weight = 10M;
            actual = OrderPrice.CalculateFees(feeType,
                charge,
                basePrice,
                partQuantity,
                priceUnit,
                weight);

            Assert.AreEqual((partQuantity * basePrice) * 0.0525M, actual);
        }

        [TestMethod]
        public void CalculateFeesEachByWeightTest()
        {
            var priceUnit = OrderPrice.enumPriceUnit.EachByWeight.ToString();

            string feeType;
            decimal charge;
            decimal basePrice;
            int partQuantity;
            decimal weight;
            decimal actual;

            feeType = "unrecognized";
            charge = 5.25M;
            basePrice = 1M;
            partQuantity = 1;
            weight = 1M;
            actual = OrderPrice.CalculateFees(feeType,
                charge,
                basePrice,
                partQuantity,
                priceUnit,
                weight);

            Assert.AreEqual(5.25M, actual);

            feeType = OrderPrice.enumFeeType.Fixed.ToString();
            charge = 5.25M;
            basePrice = 1M;
            partQuantity = 1;
            weight = 10M;
            actual = OrderPrice.CalculateFees(feeType,
                charge,
                basePrice,
                partQuantity,
                priceUnit,
                weight);

            Assert.AreEqual(5.25M, actual);

            feeType = OrderPrice.enumFeeType.Percentage.ToString();
            charge = 5.25M;
            basePrice = 2M;
            partQuantity = 2;
            weight = 10M;
            actual = OrderPrice.CalculateFees(feeType,
                charge,
                basePrice,
                partQuantity,
                priceUnit,
                weight);

            Assert.AreEqual((weight * basePrice) * 0.0525M, actual);
        }

        [TestMethod]
        public void CalculateFeesLotTest()
        {
            var priceUnitLot = OrderPrice.enumPriceUnit.Lot.ToString();
            var priceUnitLotByWeight = OrderPrice.enumPriceUnit.LotByWeight.ToString();

            string feeType;
            decimal charge;
            decimal basePrice;
            int partQuantity;
            decimal weight;
            decimal actualLot;
            decimal actualLotByWeight;

            feeType = "unrecognized";
            charge = 5.25M;
            basePrice = 1M;
            partQuantity = 1;
            weight = 1M;

            actualLot = OrderPrice.CalculateFees(feeType,
                charge,
                basePrice,
                partQuantity,
                priceUnitLot,
                weight);

            actualLotByWeight = OrderPrice.CalculateFees(feeType,
                charge,
                basePrice,
                partQuantity,
                priceUnitLotByWeight,
                weight);

            Assert.AreEqual(5.25M, actualLot);
            Assert.AreEqual(actualLot, actualLotByWeight);

            feeType = OrderPrice.enumFeeType.Fixed.ToString();
            charge = 5.25M;
            basePrice = 1M;
            partQuantity = 1;
            weight = 10M;

            actualLot = OrderPrice.CalculateFees(feeType,
                charge,
                basePrice,
                partQuantity,
                priceUnitLot,
                weight);

            actualLotByWeight = OrderPrice.CalculateFees(feeType,
                charge,
                basePrice,
                partQuantity,
                priceUnitLotByWeight,
                weight);

            Assert.AreEqual(5.25M, actualLot);
            Assert.AreEqual(actualLot, actualLotByWeight);

            feeType = OrderPrice.enumFeeType.Percentage.ToString();
            charge = 5.25M;
            basePrice = 2M;
            partQuantity = 2;
            weight = 10M;
            actualLot = OrderPrice.CalculateFees(feeType,
                charge,
                basePrice,
                partQuantity,
                priceUnitLot,
                weight);

            actualLotByWeight = OrderPrice.CalculateFees(feeType,
                charge,
                basePrice,
                partQuantity,
                priceUnitLotByWeight,
                weight);

            Assert.AreEqual(basePrice * 0.0525M, actualLot);
            Assert.AreEqual(actualLot, actualLotByWeight);
        }

        #endregion

        #region CalculateEachPrice Tests

        [TestMethod]
        public void CalculateEachPriceEachTest()
        {
            var priceUnit = OrderPrice.enumPriceUnit.Each.ToString();

            decimal basePrice;
            int quantity;
            decimal weight;
            decimal actual;

            basePrice = 0;
            quantity = 0;
            weight = 0;

            actual = OrderPrice.CalculateEachPrice(basePrice,
                priceUnit,
                quantity,
                weight);

            Assert.AreEqual(0M, actual);

            basePrice = 10M;
            quantity = 2;
            weight = 20M;

            actual = OrderPrice.CalculateEachPrice(basePrice,
                priceUnit,
                quantity,
                weight);

            Assert.AreEqual(10M, actual);
        }

        [TestMethod]
        public void CalculateEachPriceEachByWeightTest()
        {
            var priceUnit = OrderPrice.enumPriceUnit.EachByWeight.ToString();

            decimal basePrice;
            int quantity;
            decimal weight;
            decimal actual;

            basePrice = 0;
            quantity = 0;
            weight = 0;

            actual = OrderPrice.CalculateEachPrice(basePrice,
                priceUnit,
                quantity,
                weight);

            Assert.AreEqual(0M, actual);

            basePrice = 10M;
            quantity = 2;
            weight = 20M;

            actual = OrderPrice.CalculateEachPrice(basePrice,
                priceUnit,
                quantity,
                weight);

            Assert.AreEqual(10M, actual);
        }

        [TestMethod]
        public void CalculateEachPriceLotTest()
        {
            var priceUnit = OrderPrice.enumPriceUnit.Lot.ToString();

            decimal basePrice;
            int quantity;
            decimal weight;
            decimal actual;

            basePrice = 0;
            quantity = 0;
            weight = 0;

            actual = OrderPrice.CalculateEachPrice(basePrice,
                priceUnit,
                quantity,
                weight);

            Assert.AreEqual(0M, actual);

            basePrice = 10M;
            quantity = 2;
            weight = 20M;

            actual = OrderPrice.CalculateEachPrice(basePrice,
                priceUnit,
                quantity,
                weight);

            Assert.AreEqual(5M, actual);

            basePrice = 10M;
            quantity = 0;
            weight = 20M;

            actual = OrderPrice.CalculateEachPrice(basePrice,
                priceUnit,
                quantity,
                weight);

            Assert.AreEqual(0M, actual);
        }

        [TestMethod]
        public void CalculateEachPriceLotByWeightTest()
        {
            var priceUnit = OrderPrice.enumPriceUnit.LotByWeight.ToString();

            decimal basePrice;
            int quantity;
            decimal weight;
            decimal actual;

            basePrice = 0;
            quantity = 0;
            weight = 0;

            actual = OrderPrice.CalculateEachPrice(basePrice,
                priceUnit,
                quantity,
                weight);

            Assert.AreEqual(0M, actual);

            basePrice = 10M;
            quantity = 2;
            weight = 20M;

            actual = OrderPrice.CalculateEachPrice(basePrice,
                priceUnit,
                quantity,
                weight);

            Assert.AreEqual(0.5M, actual);

            basePrice = 10M;
            quantity = 20;
            weight = 0M;

            actual = OrderPrice.CalculateEachPrice(basePrice,
                priceUnit,
                quantity,
                weight);

            Assert.AreEqual(0M, actual);
        }

        #endregion

        [TestMethod]
        public void ParsePriceUnitTest()
        {
            var expectedToStringMapping = new Dictionary<OrderPrice.enumPriceUnit, string>()
            {
                [OrderPrice.enumPriceUnit.Each] = "Each",
                [OrderPrice.enumPriceUnit.EachByWeight] = "Each By Weight",
                [OrderPrice.enumPriceUnit.Lot] = "Lot",
                [OrderPrice.enumPriceUnit.LotByWeight] = "LotByWeight",
            };

            foreach (var pair in expectedToStringMapping)
            {
                var expected = pair.Key;
                var input = pair.Value;

                Assert.AreEqual(expected, OrderPrice.ParsePriceUnit(input));
                Assert.AreEqual(expected, OrderPrice.ParsePriceUnit(input.ToUpper()));
                Assert.AreEqual(expected, OrderPrice.ParsePriceUnit(input.ToLower()));
                Assert.AreEqual(expected, OrderPrice.ParsePriceUnit(input.Replace(" ", string.Empty).ToLower()));
            }

            Assert.AreEqual(OrderPrice.enumPriceUnit.Each, OrderPrice.ParsePriceUnit("unexpected"));
        }

        [TestMethod]
        public void RecalculatePriceLotTest()
        {
            var priceUnit = OrderPrice.enumPriceUnit.Lot;
            int originalQty = 10;
            int newQty = 5;
            decimal originalBasePrice = 1M;
            Assert.AreEqual(0.5M, OrderPrice.RecalculatePrice(priceUnit, originalQty, newQty, originalBasePrice));

            priceUnit = OrderPrice.enumPriceUnit.LotByWeight;
            originalQty = 30;
            newQty = 10;
            originalBasePrice = 1M;
            Assert.AreEqual((1M / 3M), OrderPrice.RecalculatePrice(priceUnit, originalQty, newQty, originalBasePrice));

            priceUnit = OrderPrice.enumPriceUnit.Lot;
            originalQty = 5;
            newQty = 10;
            originalBasePrice = 1M;
            Assert.AreEqual(2M, OrderPrice.RecalculatePrice(priceUnit, originalQty, newQty, originalBasePrice));

            // Max value check
            priceUnit = OrderPrice.enumPriceUnit.Lot;
            originalQty = 1;
            newQty = 2;
            originalBasePrice = 999999M;
            Assert.AreEqual(999999.99999M, OrderPrice.RecalculatePrice(priceUnit, originalQty, newQty, originalBasePrice));
        }

        [TestMethod]
        public void RecalculatePriceEachTest()
        {
            var priceUnit = OrderPrice.enumPriceUnit.Each;
            int originalQty = 10;
            int newQty = 5;
            decimal originalBasePrice = 1M;

            Assert.AreEqual(originalBasePrice, OrderPrice.RecalculatePrice(priceUnit, originalQty, newQty, originalBasePrice));
        }

        [TestMethod]
        public void GetPriceUnitsTest()
        {
            var qtyPriceUnits = OrderPrice.GetPriceUnits(PriceByType.Quantity);
            Assert.IsNotNull(qtyPriceUnits);
            Assert.IsTrue(qtyPriceUnits.Count() == 2);
            Assert.IsTrue(qtyPriceUnits.Contains(OrderPrice.enumPriceUnit.Each));
            Assert.IsTrue(qtyPriceUnits.Contains(OrderPrice.enumPriceUnit.Lot));

            var weightPriceUnits = OrderPrice.GetPriceUnits(PriceByType.Weight);
            Assert.IsNotNull(weightPriceUnits);
            Assert.IsTrue(weightPriceUnits.Count() == 2);
            Assert.IsTrue(weightPriceUnits.Contains(OrderPrice.enumPriceUnit.EachByWeight));
            Assert.IsTrue(weightPriceUnits.Contains(OrderPrice.enumPriceUnit.LotByWeight));

            var unknownPriceUnits = OrderPrice.GetPriceUnits((PriceByType)int.MaxValue);
            Assert.IsNotNull(unknownPriceUnits);
            Assert.IsFalse(unknownPriceUnits.Any());
        }

        [TestMethod]
        public void GetPriceByTypeTest()
        {
            Assert.AreEqual(PriceByType.Quantity, OrderPrice.GetPriceByType(OrderPrice.enumPriceUnit.Each));
            Assert.AreEqual(PriceByType.Quantity, OrderPrice.GetPriceByType(OrderPrice.enumPriceUnit.Lot));

            Assert.AreEqual(PriceByType.Weight, OrderPrice.GetPriceByType(OrderPrice.enumPriceUnit.EachByWeight));
            Assert.AreEqual(PriceByType.Weight, OrderPrice.GetPriceByType(OrderPrice.enumPriceUnit.LotByWeight));

            Assert.AreEqual(PriceByType.Quantity, OrderPrice.GetPriceByType((OrderPrice.enumPriceUnit)int.MaxValue));
        }

        [TestMethod]
        public void IsPriceUnitLotTest()
        {
            Assert.IsTrue(OrderPrice.IsPriceUnitLot(OrderPrice.enumPriceUnit.Lot));
            Assert.IsTrue(OrderPrice.IsPriceUnitLot(OrderPrice.enumPriceUnit.LotByWeight));

            Assert.IsFalse(OrderPrice.IsPriceUnitLot(OrderPrice.enumPriceUnit.Each));
            Assert.IsFalse(OrderPrice.IsPriceUnitLot(OrderPrice.enumPriceUnit.EachByWeight));

            Assert.IsFalse(OrderPrice.IsPriceUnitLot((OrderPrice.enumPriceUnit)int.MaxValue));
        }

        [TestMethod]
        public void GetQuantityValueTest()
        {
            Assert.AreEqual(OrderPrice.enumPriceUnit.Each, OrderPrice.GetQuantityValue(OrderPrice.enumPriceUnit.Each));
            Assert.AreEqual(OrderPrice.enumPriceUnit.Each, OrderPrice.GetQuantityValue(OrderPrice.enumPriceUnit.EachByWeight));

            Assert.AreEqual(OrderPrice.enumPriceUnit.Lot, OrderPrice.GetQuantityValue(OrderPrice.enumPriceUnit.Lot));
            Assert.AreEqual(OrderPrice.enumPriceUnit.Lot, OrderPrice.GetQuantityValue(OrderPrice.enumPriceUnit.LotByWeight));

            Assert.AreEqual(OrderPrice.enumPriceUnit.Each, OrderPrice.GetQuantityValue((OrderPrice.enumPriceUnit)int.MaxValue));
        }

        [TestMethod]
        public void GetWeightValueTest()
        {
            Assert.AreEqual(OrderPrice.enumPriceUnit.EachByWeight, OrderPrice.GetWeightValue(OrderPrice.enumPriceUnit.Each));
            Assert.AreEqual(OrderPrice.enumPriceUnit.EachByWeight, OrderPrice.GetWeightValue(OrderPrice.enumPriceUnit.EachByWeight));

            Assert.AreEqual(OrderPrice.enumPriceUnit.LotByWeight, OrderPrice.GetWeightValue(OrderPrice.enumPriceUnit.Lot));
            Assert.AreEqual(OrderPrice.enumPriceUnit.LotByWeight, OrderPrice.GetWeightValue(OrderPrice.enumPriceUnit.LotByWeight));

            Assert.AreEqual(OrderPrice.enumPriceUnit.EachByWeight, OrderPrice.GetWeightValue((OrderPrice.enumPriceUnit)int.MaxValue));
        }

        [TestMethod]
        public void GetPriceUnitTest()
        {
            Assert.AreEqual(OrderPrice.enumPriceUnit.Each,
                OrderPrice.GetPriceUnit(PriceByType.Quantity, PricingStrategy.Each));

            Assert.AreEqual(OrderPrice.enumPriceUnit.Lot,
                OrderPrice.GetPriceUnit(PriceByType.Quantity, PricingStrategy.Lot));

            Assert.AreEqual(OrderPrice.enumPriceUnit.EachByWeight,
                OrderPrice.GetPriceUnit(PriceByType.Weight, PricingStrategy.Each));

            Assert.AreEqual(OrderPrice.enumPriceUnit.LotByWeight,
                OrderPrice.GetPriceUnit(PriceByType.Weight, PricingStrategy.Lot));
        }
    }
}
