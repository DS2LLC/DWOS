using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using NLog;

namespace DWOS.Data.Datasets
{
    public partial class OrdersDataSet
    {
        partial class CustomerShippingSummaryDataTable
        {
        }

        public partial class OrderDataTable
        {
            public override void EndInit()
            {
                base.EndInit();
                this.OrderRowChanged += OrderDataTable_OrderRowChanged;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                this.OrderRowChanged -= OrderDataTable_OrderRowChanged;
            }

            private void OrderDataTable_OrderRowChanged(object sender, OrderRowChangeEvent e)
            {
                e.Row.OnRowChanged(e);
            }
        }

        public partial class OrderRow
        {
            public event OrderRowChangeEventHandler RowChanged;

            internal void OnRowChanged(OrderRowChangeEvent e)
            {
                if (RowChanged != null)
                    RowChanged(this, e);
            }
        }

        partial class d_PriorityDataTable : IDomainTable
        {
            #region IDomainTable Members

            public string DisplayColumn
            {
                get { return this.columnPriorityID.ToString(); }
            }

            #endregion
        }

        partial class d_OrderStatusDataTable : IDomainTable
        {
            #region IDomainTable Members

            public string DisplayColumn
            {
                get { return this.columnOrderStatusID.ToString(); }
            }

            #endregion
        }

        partial class CustomerSummaryDataTable : IDomainTable
        {
            #region IDomainTable Members

            public string DisplayColumn
            {
                get { return this.columnName.ColumnName; }
            }

            #endregion
        }

        partial class UserSummaryDataTable : IDomainTable
        {
            #region IDomainTable Members

            public string DisplayColumn
            {
                get { return this.columnName.ColumnName; }
            }

            #endregion
        }
    }

    public interface IDomainTable
    {
        string DisplayColumn { get; }
    }
}


namespace DWOS.Data.Datasets.OrdersDataSetTableAdapters
{
    partial class OrderProcessesTableAdapter
    {
    }

    partial class PartProcessPriceSummaryTableAdapter
    {
        public int GetProcessesWithoutPriceCount(int partId, IEnumerable<PriceByType> priceTypes)
        {
            var priceTypesSet = new HashSet<PriceByType>(priceTypes);

            var total = 0;
            if (priceTypesSet.Contains(PriceByType.Quantity))
            {
                total += GetProcessesWithoutQuantityPriceCount(partId) ?? 0;
            }

            if (priceTypesSet.Contains(PriceByType.Weight))
            {
                total += GetProcessesWithoutWeightPriceCount(partId) ?? 0;
            }

            return total;
        }
    }

    partial class OrderTableAdapter
    {
        public enum OrderSearchField
        {
            Po,
            ExactPo,
            ExactCoc,
            PartName,
            ExactPartName,
            CustomerName,
            ExactCustomerName,
            CustomerWo,
            ExactCustomerWo,
            UserName,
            ExactUserName,
            ExactBatch,
            ExactShipmentPackage,
            SerialNumber,
            ExactSerialNumber,
            ProductClass,
            ExactProductClass,
            ExactSalesOrder,
            ExactQuantity
        }

        public int FillBySearch(OrdersDataSet.OrderDataTable dataTable, OrderSearchField field, string value)
        {
            var limit = field == OrderSearchField.UserName ? "TOP 2000" : string.Empty;

            var sqlSelect = "SELECT " +
                            limit +
                            " " +
                            string.Join(",", dataTable.Columns.Cast<DataColumn>().Select(i => i.ColumnName)) +
                            " FROM [Order]";

            var sqlWhere = "WHERE ";
            var param = string.Empty;

            var addParam = true;
            switch (field)
            {
                case OrderSearchField.ExactSalesOrder:
                    sqlWhere += "(SalesOrderID = @param)";
                    param = value;
                    break;
                case OrderSearchField.Po:
                    sqlWhere += "(PurchaseOrder LIKE @param) ";
                    param = "%" + Utilities.PrepareForLike(value) + "%";
                    break;
                case OrderSearchField.ExactPo:
                    sqlWhere += "(PurchaseOrder = @param) ";
                    param = value;
                    break;
                case OrderSearchField.ExactCoc:
                    sqlWhere += "(OrderID IN (SELECT OrderID FROM COC WHERE COCID = @param))";
                    param = value;
                    break;
                case OrderSearchField.PartName:
                    sqlWhere += "(PartID IN (SELECT PartID FROM Part WHERE Name LIKE @param))";
                    param = "%" + Utilities.PrepareForLike(value) + "%";
                    break;
                case OrderSearchField.ExactPartName:
                    sqlWhere += "(PartID IN (SELECT PartID FROM Part WHERE Name = @param))";
                    param = value;
                    break;
                case OrderSearchField.CustomerName:
                    sqlWhere += "(CustomerID IN (SELECT CustomerID FROM Customer WHERE Name LIKE @param))";
                    param = "%" + Utilities.PrepareForLike(value) + "%";
                    break;
                case OrderSearchField.ExactCustomerName:
                    sqlWhere += "(CustomerID IN (SELECT CustomerID FROM Customer WHERE Name = @param))";
                    param = value;
                    break;
                case OrderSearchField.CustomerWo:
                    sqlWhere += "(CustomerWO LIKE @param)";
                    param = "%" + Utilities.PrepareForLike(value) + "%";
                    break;
                case OrderSearchField.ExactCustomerWo:
                    sqlWhere += "(CustomerWO = @param)";
                    param = value;
                    break;
                case OrderSearchField.UserName:
                    sqlWhere +=
                        "(OrderID IN (SELECT OrderID FROM [Order] INNER JOIN Users ON [Order].CreatedBy = Users.UserID WHERE Users.Name LIKE @param))";
                    param = "%" + Utilities.PrepareForLike(value) + "%";
                    break;
                case OrderSearchField.ExactUserName:
                    sqlWhere +=
                        "(OrderID IN (SELECT OrderID FROM [Order] INNER JOIN Users ON [Order].CreatedBy = Users.UserID WHERE Users.Name = @param))";
                    param = value;
                    break;
                case OrderSearchField.ExactQuantity:
                    sqlWhere += "(PartQuantity = @param)";
                    param = value;
                    break;
                case OrderSearchField.ExactBatch:
                    sqlWhere += "(OrderID IN (SELECT OrderID FROM BatchOrder WHERE BatchID = @param))";
                    param = value;
                    break;
                case OrderSearchField.ExactShipmentPackage:
                    sqlWhere += "(OrderID IN (SELECT OrderID FROM OrderShipment WHERE ShipmentPackageID = @param))";
                    param = value;
                    break;
                case OrderSearchField.SerialNumber:
                    sqlWhere += "(OrderID IN (SELECT OrderID FROM OrderSerialNumber WHERE Number LIKE @param))";
                    param = "%" + Utilities.PrepareForLike(value) + "%";
                    break;
                case OrderSearchField.ExactSerialNumber:
                    sqlWhere += "(OrderID IN (SELECT OrderID FROM OrderSerialNumber WHERE Number = @param))";
                    param = value;
                    break;
                case OrderSearchField.ProductClass:
                    sqlWhere += "(OrderID IN (SELECT OrderID FROM OrderProductClass WHERE ProductClass LIKE @param))";
                    param = "%" + Utilities.PrepareForLike(value) + "%";
                    break;
                case OrderSearchField.ExactProductClass:
                    sqlWhere += "(OrderID IN (SELECT OrderID FROM OrderProductClass WHERE ProductClass = @param))";
                    param = value;
                    break;
                default:
                    addParam = false;
                    sqlWhere = string.Empty;
                    break;
            }

            var sqlOrder = field == OrderSearchField.UserName
                ? "ORDER BY [Order].OrderDate DESC;"
                : "ORDER BY [Order].OrderID;";

            var sqlQuery = $"{sqlSelect} {sqlWhere} {sqlOrder}";

            Adapter.SelectCommand = new SqlCommand(sqlQuery, Connection);

            if (addParam)
            {
                Adapter.SelectCommand.Parameters.AddWithValue("@param", param);
            }

            if (ClearBeforeFill)
            {
                dataTable.Clear();
            }

            return Adapter.Fill(dataTable);
        }

        public IEnumerable<int> GetRejoinedOrderIds(int destinationOrderId)
        {
            using (var dtOrder = GetRejoined(destinationOrderId))
            {
                return dtOrder.Select(order => order.OrderID).ToList();
            }
        }
    }

    public partial class MediaTableAdapter
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    	private static bool? _useFileStream;

        private bool? UseFileStream
		{
			get
			{
				//if using integrated security and not a SQL LocalDb then use FileStream
				if(!_useFileStream.HasValue)
					_useFileStream = this.Connection.ConnectionString.Contains(false, "Integrated Security=True") && !this.Connection.ConnectionString.Contains(false, "localdb");

				return _useFileStream.GetValueOrDefault();
			}
		}

        public byte[] GetMediaStream(int mediaID)
        {
            try
            {
                if (this.Connection.State != System.Data.ConnectionState.Open)
                    this.Connection.Open();

                //if using integrated security, this is the only way we can use FileStream
				if(UseFileStream.GetValueOrDefault())
                {
                    string path = this.GetMediaPath(mediaID);
                    _log.Info("Getting media from " + path);

                    using (var transaction = this.Connection.BeginTransaction("MediaTrans"))
                    {
                        using (var cmd = new SqlCommand { Connection = this.Connection, Transaction = transaction, CommandText = "SELECT GET_FILESTREAM_TRANSACTION_CONTEXT()" })
                        {
                            var txContext = cmd.ExecuteScalar() as byte[];
                            
                            using (var fs = new SqlFileStream(path, txContext, FileAccess.Read))
                            {
                                var buffer = new byte[fs.Length];
                                fs.Read(buffer, 0, buffer.Length);
                                fs.Close();
                                return buffer;
                            }
                        }
                    }
                }
                else //else have to use normal method to access media; Drawback is it consumes memory on server side
                {
                    return this.GetMediaBytes(mediaID);
                }
            }
            catch (Exception exc)
            {
                _log.Info(exc, "Error getting media from remote data stream.");
                return null;
            }
        }

        public void SetMediaStream(int mediaID, byte[] bytes)
        {
            try
            {
                if (this.Connection.State != System.Data.ConnectionState.Open)
                    this.Connection.Open();

                //if using integrated security, this is the only way we can use FileStream
                if (UseFileStream.GetValueOrDefault())
                {
                    string path = this.GetMediaPath(mediaID);
                    _log.Info("Setting media at " + path);

                    using (var transaction = this.Connection.BeginTransaction("MediaTransSET"))
                    {
                        using (var cmd = new SqlCommand { Connection = this.Connection, Transaction = transaction, CommandText = "SELECT GET_FILESTREAM_TRANSACTION_CONTEXT()" })
                        {
                            var txContext = cmd.ExecuteScalar() as byte[];

                            using (var fs = new SqlFileStream(path, txContext, FileAccess.Write))
                            {
                                fs.Write(bytes, 0, bytes.Length);
                                fs.Close();
                            }
                        }
                    }
                }
                else //else have to use normal method to access media; Drawback is it consumes memory on server side
                {
                    this.SetMediaBytes(bytes, mediaID);
                }
            }
            catch (Exception exc)
            {
                _log.Info(exc, "Error setting media from remote data stream.");
            }
        }

		public DWOS.Data.Datasets.OrdersDataSet.MediaRow AddMedia(string name, string fileName, string fileExt, byte[] bytes)
		{
			if(this.Connection.State != System.Data.ConnectionState.Open)
				this.Connection.Open();

			var mediaTable = new DWOS.Data.Datasets.OrdersDataSet.MediaDataTable();
			var mediaRow = mediaTable.AddMediaRow(name, fileName, fileExt, bytes);
			this.Update(mediaRow);

			return mediaRow;
		}
    }
}



