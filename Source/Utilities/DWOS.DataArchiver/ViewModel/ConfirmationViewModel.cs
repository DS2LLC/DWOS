using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DWOS.DataArchiver.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using DWOS.DataArchiver.Messages;
using GalaSoft.MvvmLight.CommandWpf;

namespace DWOS.DataArchiver.ViewModel
{
    public class ConfirmationViewModel : ViewModelBase
    {
        #region Fields

        private string _explanationText;

        #endregion

        #region Properties

        public GlobalOptionsProvider Options { get; }

        public string ExplanationText
        {
            get => _explanationText;
            set => Set(nameof(ExplanationText), ref _explanationText, value);
        }

        public ObservableCollection<Order> Orders { get; } =
            new ObservableCollection<Order>();

        public ObservableCollection<SalesOrder> SalesOrders { get; } =
            new ObservableCollection<SalesOrder>();

        public ICommand Next { get; }

        #endregion

        #region Methods

        public ConfirmationViewModel(IMessenger messenger, GlobalOptionsProvider options)
            : base(messenger)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Next = new RelayCommand(DoNext);
        }

        public async Task LoadData()
        {
            Orders.Clear();

            using (var conn = new SqlConnection(Options.ConnectionString))
            {
                conn.Open();

                // Retrieve Work Orders
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
WITH NumberedOrders
AS (
    SELECT ROW_NUMBER() OVER (
            ORDER BY OrderID ASC
            ) AS RowNumber,
        [OrderId],
        [Customer].[Name] AS CustomerName,
        [Order].OrderDate,
        [Order].CompletedDate
    FROM [Order]
    INNER JOIN [Customer] ON [Order].CustomerID = [Customer].CustomerID
    WHERE Status = 'Closed'
        AND OrderID IN (
            SELECT DISTINCT OrderID
            FROM Order_Media
            )
    )
SELECT OrderID, CustomerName, OrderDate, CompletedDate
FROM NumberedOrders
WHERE RowNumber <= @orderCount";

                    cmd.Parameters.AddWithValue("@orderCount", Options.OrderCount);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var order = new Order
                            {
                                OrderId = reader.GetInt32(0),
                                CustomerName = reader.GetString(1),
                                OrderDate = reader.IsDBNull(2) ? (DateTime?) null : reader.GetDateTime(2),
                                CompletedDate = reader.IsDBNull(3) ? (DateTime?) null : reader.GetDateTime(3)
                            };

                            Orders.Add(order);
                        }
                    }
                }

                // Retrieve Sales Orders
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
WITH NumberedOrders
AS (
    SELECT ROW_NUMBER() OVER (
            ORDER BY OrderID ASC
            ) AS RowNumber,
        [OrderId],
        SalesOrderID
    FROM [Order]
    WHERE Status = 'Closed'
        AND OrderID IN (
            SELECT DISTINCT OrderID
            FROM Order_Media
            )
    )
SELECT DISTINCT SalesOrder.SalesOrderID
FROM NumberedOrders
INNER JOIN SalesOrder ON NumberedOrders.SalesOrderID = SalesOrder.SalesOrderID
WHERE RowNumber <= @orderCount
AND SalesOrder.Status = 'Closed'";

                    cmd.Parameters.AddWithValue("@orderCount", Options.OrderCount);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var salesOrder = new SalesOrder
                            {
                                SalesOrderId = reader.GetInt32(0),
                            };

                            SalesOrders.Add(salesOrder);
                        }
                    }
                }
            }

            ExplanationText = $@"Last {Orders.Count} closed work {(Orders.Count == 1 ? "order" : "orders")} with media";

            if (SalesOrders.Count > 0)
            {
                ExplanationText += Environment.NewLine +
                    $"Includes {SalesOrders.Count} closed sales {(SalesOrders.Count == 1 ? "order" : "orders")}";
            }
        }

        private void DoNext()
        {
            Options.OrdersToArchive = Orders.ToList();
            Options.SalesOrdersToArchive = SalesOrders.ToList();
            MessengerInstance.Send(new MoveToStepMessage(Step.Archive));
        }

        #endregion
    }
}
