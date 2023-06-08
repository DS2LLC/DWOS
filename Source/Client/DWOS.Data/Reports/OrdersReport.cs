namespace DWOS.Data.Reports
{
    public partial class OrdersReport
    {
        partial class d_OrderChangeReasonDataTable
        {
        }

        partial class OrderDataTable
        {
        }
    }
}

namespace DWOS.Data.Reports.OrdersReportTableAdapters
{
    partial class PartSummaryTableAdapter
    {
    }
    
    
    public partial class OrderTableAdapter {

		public int Fill(OrdersReport.OrderDataTable dataTable, string sqlWhere)
		{
            string sql = @"SELECT        [Order].OrderID, [Order].OrderDate, Customer.Name AS CustomerName, [Order].RequiredDate, [Order].Status, [Order].CompletedDate, [Order].Priority, 
                         [Order].PurchaseOrder, [Order].CreatedBy, [Order].Invoice, [Order].ContractReviewed, [Order].PartID, [Order].PartQuantity, [Order].WorkStatus, [Order].CurrentLocation, 
                         [Order].BasePrice, [Order].PriceUnit, [Order].ShippingMethod, Part.Name AS PartName, [Order].EstShipDate, [Order].CustomerWO, [Order].OrderType, 
                         [Order].Hold
                        FROM            [Order] INNER JOIN
                         Customer ON [Order].CustomerID = Customer.CustomerID INNER JOIN
                         Part ON [Order].PartID = Part.PartID";

            //sqlWhere = DWOS.Data.Datasets.Utilities.SqlBless(sqlWhere);
			this.Adapter.SelectCommand = new System.Data.SqlClient.SqlCommand(sql + " WHERE " + sqlWhere + " ORDER BY CustomerName ASC, RequiredDate ASC", this.Connection);

			if((this.ClearBeforeFill == true))
				dataTable.Clear();
			
			int returnValue = this.Adapter.Fill(dataTable);
			return returnValue;
		}
    }
}
