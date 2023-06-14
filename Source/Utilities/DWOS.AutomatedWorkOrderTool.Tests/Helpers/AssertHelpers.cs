using DWOS.AutomatedWorkOrderTool.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.AutomatedWorkOrderTool.Tests.Helpers
{
    internal static class AssertHelpers
    {
        internal static void HaveSameData(MasterListPart expected, MasterListPart actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            if (actual == null)
            {
                Assert.Fail();
            }

            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.Program, actual.Program);
            Assert.AreEqual(expected.ProductCode, actual.ProductCode);
            Assert.AreEqual(expected.Identity, actual.Identity);
            Assert.AreEqual(expected.OspCode, actual.OspCode);
            Assert.AreEqual(expected.Preferred, actual.Preferred);
            Assert.AreEqual(expected.Alt, actual.Alt);
            Assert.AreEqual(expected.MaterialDescription, actual.MaterialDescription);
            Assert.AreEqual(expected.Mask, actual.Mask);
            Assert.AreEqual(expected.PartMark, actual.PartMark);
            Assert.AreEqual(expected.IdentityCode, actual.IdentityCode);
        }

        internal static void HaveSameData(ShippingManifestOrder expected, ShippingManifestOrder actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            if (actual == null)
            {
                Assert.Fail();
            }

            Assert.AreEqual(expected.Priority, actual.Priority);
            Assert.AreEqual(expected.KacShipper, actual.KacShipper);
            Assert.AreEqual(expected.PurchaseOrder, actual.PurchaseOrder);
            Assert.AreEqual(expected.PurchaseOrderItem, actual.PurchaseOrderItem);
            Assert.AreEqual(expected.WorkOrder, actual.WorkOrder);
            Assert.AreEqual(expected.Part, actual.Part);
            Assert.AreEqual(expected.Project, actual.Project);
            Assert.AreEqual(expected.DueDate, actual.DueDate);
            Assert.AreEqual(expected.Quantity, actual.Quantity);
            Assert.AreEqual(expected.LotCost, actual.LotCost);
            Assert.AreEqual(expected.InvoiceNumber, expected.InvoiceNumber);
            Assert.AreEqual(expected.VendorPackslip, actual.VendorPackslip);
            Assert.AreEqual(expected.PurchasingInvoiceApproval, actual.PurchasingInvoiceApproval);
            Assert.AreEqual(expected.VendorNumber, actual.VendorNumber);
            Assert.AreEqual(expected.SourceCode, actual.SourceCode);
        }
    }
}
