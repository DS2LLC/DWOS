using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DWOS.Data.Tests
{
    [TestClass]
    public class SysproInvoiceSettingsTests
    {
        [TestMethod]
        public void CopyTest()
        {
            var expected = GenerateTestData();
            var actual = expected.Copy();

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.PriceUnitEach, actual.PriceUnitEach);
            Assert.AreEqual(expected.PriceUnitLot, actual.PriceUnitLot);
            Assert.AreEqual(expected.LineItem, actual.LineItem);

            Assert.IsFalse(ReferenceEquals(expected.OrderHeaderMap, actual.OrderHeaderMap));
            Assert.IsFalse(ReferenceEquals(expected.OrderHeaderMap.Fields, actual.OrderHeaderMap.Fields));
            Assert.IsFalse(ReferenceEquals(expected.OrderHeaderMap.CustomFields, actual.OrderHeaderMap.CustomFields));
            Assert.IsFalse(ReferenceEquals(expected.OrderHeaderMap.Literals, actual.OrderHeaderMap.Literals));

            Assert.IsFalse(ReferenceEquals(expected.StockLineMap, actual.StockLineMap));
            Assert.IsFalse(ReferenceEquals(expected.StockLineMap.Fields, actual.StockLineMap.Fields));
            Assert.IsFalse(ReferenceEquals(expected.StockLineMap.CustomFields, actual.StockLineMap.CustomFields));
            Assert.IsFalse(ReferenceEquals(expected.StockLineMap.Literals, actual.StockLineMap.Literals));

            Assert.IsFalse(ReferenceEquals(expected.StockLineFeeMap, actual.StockLineFeeMap));
            Assert.IsFalse(ReferenceEquals(expected.StockLineFeeMap.Fields, actual.StockLineFeeMap.Fields));
            Assert.IsFalse(ReferenceEquals(expected.StockLineFeeMap.CustomFields, actual.StockLineFeeMap.CustomFields));
            Assert.IsFalse(ReferenceEquals(expected.StockLineFeeMap.Literals, actual.StockLineFeeMap.Literals));

            Assert.IsFalse(ReferenceEquals(expected.CommentMap, actual.CommentMap));
            Assert.IsFalse(ReferenceEquals(expected.CommentMap.Fields, actual.CommentMap.Fields));
            Assert.IsFalse(ReferenceEquals(expected.CommentMap.CustomFields, actual.CommentMap.CustomFields));
            Assert.IsFalse(ReferenceEquals(expected.CommentMap.Literals, actual.CommentMap.Literals));
        }

        private SysproInvoiceSettings GenerateTestData()
        {
            return new SysproInvoiceSettings
            {
                PriceUnitEach = "each",
                PriceUnitLot = "lot",
                LineItem = SysproInvoiceSettings.LineItemType.Process,

                OrderHeaderMap = new SysproInvoiceSettings.FieldMapping
                {
                    Literals = new List<SysproInvoiceSettings.Literal>
                    {
                        new SysproInvoiceSettings.Literal {Syspro = "OH1", Value = "A" }
                    },

                    CustomFields = new List<SysproInvoiceSettings.CustomField>
                    {
                        new SysproInvoiceSettings.CustomField { Syspro = "OH2", TokenName = "B" }
                    },

                    Fields = new List<SysproInvoiceSettings.Field>
                    {
                        new SysproInvoiceSettings.Field { Syspro = "OH3", Dwos = SysproInvoiceSettings.FieldType.OrderId }
                    }
                },

                StockLineMap = new SysproInvoiceSettings.FieldMapping
                {
                    Literals = new List<SysproInvoiceSettings.Literal>
                    {
                        new SysproInvoiceSettings.Literal {Syspro = "ST1", Value = "A" }
                    },

                    CustomFields = new List<SysproInvoiceSettings.CustomField>
                    {
                        new SysproInvoiceSettings.CustomField { Syspro = "ST2", TokenName = "B" }
                    },

                    Fields = new List<SysproInvoiceSettings.Field>
                    {
                        new SysproInvoiceSettings.Field { Syspro = "ST3", Dwos = SysproInvoiceSettings.FieldType.OrderId }
                    }
                },

                StockLineFeeMap = new SysproInvoiceSettings.FieldMapping
                {
                    Literals = new List<SysproInvoiceSettings.Literal>
                    {
                        new SysproInvoiceSettings.Literal {Syspro = "MC1", Value = "A" }
                    },

                    CustomFields = new List<SysproInvoiceSettings.CustomField>
                    {
                        new SysproInvoiceSettings.CustomField { Syspro = "MC2", TokenName = "B" }
                    },

                    Fields = new List<SysproInvoiceSettings.Field>
                    {
                        new SysproInvoiceSettings.Field { Syspro = "MC3", Dwos = SysproInvoiceSettings.FieldType.OrderId }
                    }
                },

                CommentMap = new SysproInvoiceSettings.CommentMapping
                {
                    Literals = new List<SysproInvoiceSettings.CommentLiteral>
                    {
                        new SysproInvoiceSettings.CommentLiteral { Type = SysproInvoiceSettings.CommentType.Invoice, Value = "Comment", Order = 0 }
                    },

                    CustomFields = new List<SysproInvoiceSettings.CommentCustomField>
                    {
                        new SysproInvoiceSettings.CommentCustomField { Type = SysproInvoiceSettings.CommentType.Order, Format = "{0}", TokenName = "A", Order = 1 }
                    },

                    Fields = new List<SysproInvoiceSettings.CommentField>
                    {
                        new SysproInvoiceSettings.CommentField { Type = SysproInvoiceSettings.CommentType.Invoice, Format = "{0}", Dwos = SysproInvoiceSettings.FieldType.OrderPurchaseOrder, Order = 2 }
                    }
                }
            };
        }
    }
}
