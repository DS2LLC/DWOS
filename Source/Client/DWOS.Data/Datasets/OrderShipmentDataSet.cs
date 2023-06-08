using System.Linq;

namespace DWOS.Data.Datasets
{
    partial class OrderShipmentDataSet
    {
        partial class ShipmentPackageDataTable
        {
            public ShipmentPackageRow[] SelectPackages(int customerId, int shipmentPackageTypeId)
            {
                var filterExpression = $"CustomerID = {customerId} AND ShipmentPackageTypeID = {shipmentPackageTypeId}";
                return Select(filterExpression, "PackageNumber DESC")
                    .Cast<ShipmentPackageRow>()
                    .ToArray();
            }
        }
    }
}
