using DWOS.Data.Order;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DWOS.Data.Tests
{
    [TestClass]
    public class PriceUnitPersistenceExtensionsTests
    {
        private const int CustomerId = -1;
        private static Mock<IPriceUnitPersistence> _priceUnitMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _priceUnitMock = new Mock<IPriceUnitPersistence>();
        }

        [TestMethod]
        public void DefaultPriceUnitTest()
        {
            _priceUnitMock.Setup(m => m.IsActive(OrderPrice.enumPriceUnit.Lot.ToString()))
                .Returns(true);

            Assert.AreEqual(OrderPrice.enumPriceUnit.Lot, _priceUnitMock.Object.DefaultPriceUnit());

            _priceUnitMock.Setup(m => m.IsActive(OrderPrice.enumPriceUnit.Lot.ToString()))
                .Returns(false);

            _priceUnitMock.Setup(m => m.IsActive(OrderPrice.enumPriceUnit.LotByWeight.ToString()))
                .Returns(true);

            Assert.AreEqual(OrderPrice.enumPriceUnit.LotByWeight, _priceUnitMock.Object.DefaultPriceUnit());

            _priceUnitMock.Setup(m => m.IsActive(It.IsAny<string>()))
                .Returns(false);

            Assert.AreEqual(OrderPrice.enumPriceUnit.Each, _priceUnitMock.Object.DefaultPriceUnit());
        }

        [TestMethod]
        public void DeterminePriceUnitTest()
        {
            _priceUnitMock.Setup(m => m.FindByPriceUnitId(CustomerId, It.IsAny<string>()))
                .Returns((int customerId, string s) => GetPriceData(s));

            OrderPrice.enumPriceUnit expected;

            // Normal cases - Each Minimum > Lot Maximum
            expected = OrderPrice.enumPriceUnit.Lot;
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId, 1, 0M, OrderPrice.enumPriceUnit.Lot));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId, 1, 0M, OrderPrice.enumPriceUnit.Each));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId, 30, 0M, OrderPrice.enumPriceUnit.Each));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId, 0, 0M, OrderPrice.enumPriceUnit.Each));

            expected = OrderPrice.enumPriceUnit.Each;

            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId, 31, 0M, OrderPrice.enumPriceUnit.Each));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId, 32, 0M, OrderPrice.enumPriceUnit.Lot));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId, 32, 0M, OrderPrice.enumPriceUnit.Each));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId, 33, 0M, OrderPrice.enumPriceUnit.Lot));

            expected = OrderPrice.enumPriceUnit.LotByWeight;
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,1, 0M, OrderPrice.enumPriceUnit.LotByWeight));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,1, 0M, OrderPrice.enumPriceUnit.EachByWeight));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,31, 31.9999999M, OrderPrice.enumPriceUnit.EachByWeight));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,1, -1M, OrderPrice.enumPriceUnit.LotByWeight));

            expected = OrderPrice.enumPriceUnit.EachByWeight;
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,31, 32M, OrderPrice.enumPriceUnit.EachByWeight));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,1, 32.0000000001M, OrderPrice.enumPriceUnit.LotByWeight));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,1, 32.0000000001M, OrderPrice.enumPriceUnit.EachByWeight));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,1, 33M, OrderPrice.enumPriceUnit.LotByWeight));

            // Atypical cases - Lot Minimum > Each Maximum
            _priceUnitMock.Setup(m => m.FindByPriceUnitId(CustomerId, It.IsAny<string>()))
                .Returns((int customerId, string s) => GetAtypicalPriceData(s));

            expected = OrderPrice.enumPriceUnit.Each;
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId, 1, 0M, PriceByType.Quantity));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId, 4, 0M, PriceByType.Quantity));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId, 5, 0M, PriceByType.Quantity));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId, 0, 0M, PriceByType.Quantity));

            expected = OrderPrice.enumPriceUnit.Lot;
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,32, 0M, PriceByType.Quantity));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,32, 0M, PriceByType.Quantity));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,33, 0M, PriceByType.Quantity));

            expected = OrderPrice.enumPriceUnit.EachByWeight;
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,1, 0M, PriceByType.Weight));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,1, 0M, PriceByType.Weight));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,31, 4.99M, PriceByType.Weight));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,1, -1M, PriceByType.Weight));

            expected = OrderPrice.enumPriceUnit.LotByWeight;
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,31, 5.00M, PriceByType.Weight));
            Assert.AreEqual(expected, _priceUnitMock.Object.DeterminePriceUnit(CustomerId,1, 5.0000000001M, PriceByType.Weight));
        }

        [TestMethod]
        public void GetDisplayTextTest()
        {
            _priceUnitMock.Setup(m => m.FindByPriceUnitId(CustomerId, It.IsAny<string>()))
                .Returns((int customerId, string s) => GetPriceData(s));

            Assert.AreEqual("_Each_", _priceUnitMock.Object.GetDisplayText(OrderPrice.enumPriceUnit.Each));
            Assert.AreEqual("_Lot_", _priceUnitMock.Object.GetDisplayText(OrderPrice.enumPriceUnit.Lot));
            Assert.AreEqual("_Each By Weight_", _priceUnitMock.Object.GetDisplayText(OrderPrice.enumPriceUnit.EachByWeight));
            Assert.AreEqual("_Lot By Weight_", _priceUnitMock.Object.GetDisplayText(OrderPrice.enumPriceUnit.LotByWeight));
        }

        #region Helper Method

        private PriceUnitData GetPriceData(string priceUnit)
        {
            OrderPrice.enumPriceUnit value = OrderPrice.ParsePriceUnit(priceUnit);

            switch (value)
            {
                case OrderPrice.enumPriceUnit.Each:
                    return new PriceUnitData()
                    {
                        PriceUnit = OrderPrice.enumPriceUnit.Each,
                        DisplayName = "_Each_",
                        Active = true,
                        MaxQuantity = 99999,
                        MinQuantity = 31,
                        MinWeight = null,
                        MaxWeight = null,
                    };
                case OrderPrice.enumPriceUnit.Lot:
                    return new PriceUnitData()
                    {
                        PriceUnit = OrderPrice.enumPriceUnit.Lot,
                        DisplayName = "_Lot_",
                        Active = true,
                        MaxQuantity = 1,
                        MinQuantity = 30,
                        MinWeight = null,
                        MaxWeight = null,
                    };
                case OrderPrice.enumPriceUnit.EachByWeight:
                    return new PriceUnitData()
                    {
                        PriceUnit = OrderPrice.enumPriceUnit.EachByWeight,
                        DisplayName = "_Each By Weight_",
                        Active = true,
                        MaxQuantity = 0,
                        MinQuantity = 0,
                        MinWeight = 32.00M,
                        MaxWeight = 999999.99M,
                    };

                case OrderPrice.enumPriceUnit.LotByWeight:
                    return new PriceUnitData()
                    {
                        PriceUnit = OrderPrice.enumPriceUnit.LotByWeight,
                        DisplayName = "_Lot By Weight_",
                        Active = true,
                        MaxQuantity = 0,
                        MinQuantity = 0,
                        MinWeight = 0.00M,
                        MaxWeight = 31.99M,
                    };

                default:
                    return null;
            }
        }

        private PriceUnitData GetAtypicalPriceData(string priceUnit)
        {
            OrderPrice.enumPriceUnit value = OrderPrice.ParsePriceUnit(priceUnit);

            switch (value)
            {
                case OrderPrice.enumPriceUnit.Each:
                    return new PriceUnitData()
                    {
                        PriceUnit = OrderPrice.enumPriceUnit.Each,
                        DisplayName = "_Each_",
                        Active = true,
                        MaxQuantity = 5,
                        MinQuantity = 1,
                        MinWeight = null,
                        MaxWeight = null,
                    };
                case OrderPrice.enumPriceUnit.Lot:
                    return new PriceUnitData()
                    {
                        PriceUnit = OrderPrice.enumPriceUnit.Lot,
                        DisplayName = "_Lot_",
                        Active = true,
                        MaxQuantity = 99999,
                        MinQuantity = 6,
                        MinWeight = null,
                        MaxWeight = null,
                    };
                case OrderPrice.enumPriceUnit.EachByWeight:
                    return new PriceUnitData()
                    {
                        PriceUnit = OrderPrice.enumPriceUnit.EachByWeight,
                        DisplayName = "_Each By Weight_",
                        Active = true,
                        MaxQuantity = 0,
                        MinQuantity = 0,
                        MinWeight = 0.00M,
                        MaxWeight = 5.00M,
                    };

                case OrderPrice.enumPriceUnit.LotByWeight:
                    return new PriceUnitData()
                    {
                        PriceUnit = OrderPrice.enumPriceUnit.LotByWeight,
                        DisplayName = "_Lot By Weight_",
                        Active = true,
                        MaxQuantity = 0,
                        MinQuantity = 0,
                        MinWeight = 5.00M,
                        MaxWeight = 999999.99M,
                    };

                default:
                    return null;
            }
        }

        #endregion
    }
}
