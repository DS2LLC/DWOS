using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using ByteSizeLib;
using DWOS.DataArchiver.Messages;
using DWOS.DataArchiver.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using NLog;

namespace DWOS.DataArchiver.ViewModel
{
    public class ArchiveViewModel : ViewModelBase
    {
        #region Fields

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private double _progress;
        private OperationStatus _status;
        private readonly RelayCommand _nextCommand;

        #endregion

        #region Properties

        public GlobalOptionsProvider Options { get; }

        public double Progress
        {
            get => _progress;
            set => Set(nameof(Progress), ref _progress, value);
        }

        public ICommand Archive { get; }

        public ICommand Next => _nextCommand;

        #endregion

        #region Methods

        public ArchiveViewModel(IMessenger messenger, GlobalOptionsProvider options)
            : base(messenger)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Archive = new RelayCommand(DoArchive, () => _status == OperationStatus.Ready);
            _nextCommand = new RelayCommand(DoNext, () => _status == OperationStatus.Finished);
        }

        private async void DoArchive()
        {
            _status = OperationStatus.InProgress;
            Progress = 0;

            // Archive order media
            using (var conn = new SqlConnection(Options.ConnectionString))
            {
                const int accountingForOverhead = 1;

                conn.Open();

                var deletedMediaIds = new HashSet<int>();
                var ordersToAddHistoryFor = new HashSet<int>();

                // Get sales order media to delete
                var salesOrderMedia = new List<Media>();

                foreach (var salesOrder in Options.SalesOrdersToArchive)
                {
                    salesOrderMedia.AddRange(await GetSalesOrderMedia(conn, salesOrder.SalesOrderId));
                }

                // Get work order media to delete
                var workOrderMedia = new List<Media>();

                foreach (var order in Options.OrdersToArchive)
                {
                    workOrderMedia.AddRange(await GetOrderMedia(conn, order.OrderId));
                }

                var progressStep = 100d / (salesOrderMedia.Count + workOrderMedia.Count + accountingForOverhead);

                // Delete sales order media
                foreach (var media in salesOrderMedia)
                {
                    if (deletedMediaIds.Contains(media.MediaId))
                    {
                        // Already deleted - skip
                        Progress += progressStep;
                        continue;
                    }

                    var salesOrderIds = await GetAssociatedSalesOrders(conn, media.MediaId);
                    var orderIds = await GetAssociatedOrders(conn, media.MediaId);

                    var anyFailedToSave = false;
                    foreach (var salesOrderId in salesOrderIds)
                    {
                        if (!await SaveMediaForSalesOrder(conn, salesOrderId, media))
                        {
                            anyFailedToSave = true;
                            break;
                        }
                    }

                    if (anyFailedToSave)
                    {
                        // Failed to save media - skip
                        continue;
                    }

                    foreach (var orderId in orderIds)
                    {
                        ordersToAddHistoryFor.Add(orderId);
                    }

                    if (await DeleteMedia(conn, media))
                    {
                        deletedMediaIds.Add(media.MediaId);
                        Options.BytesSaved += media.Size;
                    }
                    else
                    {
                        _logger.Error($"Failed to remove media {media.MediaId}");
                    }

                    Progress += progressStep;
                }

                // Delete work order media
                foreach (var media in workOrderMedia)
                {
                    if (deletedMediaIds.Contains(media.MediaId))
                    {
                        // Already deleted - skip
                        Progress += progressStep;
                        continue;
                    }

                    var orderIds = await GetAssociatedOrders(conn, media.MediaId);

                    var anyFailedToSave = false;
                    foreach (var orderId in orderIds)
                    {
                        if (!await SaveMediaForWorkOrder(conn, orderId, media))
                        {
                            anyFailedToSave = true;
                            break;
                        }
                    }

                    if (anyFailedToSave)
                    {
                        // Failed to save media - skip
                        continue;
                    }

                    foreach (var orderId in orderIds)
                    {
                        ordersToAddHistoryFor.Add(orderId);
                    }

                    if (await DeleteMedia(conn, media))
                    {
                        deletedMediaIds.Add(media.MediaId);
                        Options.BytesSaved += media.Size;
                    }
                    else
                    {
                        _logger.Error($"Failed to remove media {media.MediaId}");
                    }

                    Progress += progressStep;
                }

                // Add order history
                foreach (var orderId in ordersToAddHistoryFor)
                {
                    await AddOrderHistory(conn, orderId);
                }
            }

            // Finish archiving
            Progress = 100;
            _status = OperationStatus.Finished;
            _nextCommand.RaiseCanExecuteChanged();
        }

        private async Task<bool> SaveMediaForWorkOrder(SqlConnection conn, int orderId, Media media)
        {
            try
            {
                // Retrieve full data
                byte[] data = null;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
SELECT Media
FROM Media
WHERE MediaID = @mediaId;
";

                    cmd.Parameters.AddWithValue("@mediaId", media.MediaId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            data = reader[0] as byte[];
                        }
                    }
                }

                // Save to order's folder.
                if (data == null)
                {
                    // No data - skipping
                    _logger.Warn($"Skipping blank media {media.MediaId} for order {orderId}");
                    return true;
                }

                var orderFolder = Path.Combine(Options.Directory, $"WO {orderId}");
                if (!Directory.Exists(orderFolder))
                {
                    Directory.CreateDirectory(orderFolder);
                }

                var fileName = Path.Combine(orderFolder, $"{media.Name}_{media.MediaId}.{media.FileExtension}");
                File.WriteAllBytes(fileName, data);

                return true;
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error saving media");
                return false;
            }

        }

        private async Task<bool> SaveMediaForSalesOrder(SqlConnection conn, int salesOrderId, Media media)
        {
            try
            {
                // Retrieve full data
                byte[] data = null;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
SELECT Media
FROM Media
WHERE MediaID = @mediaId;
";

                    cmd.Parameters.AddWithValue("@mediaId", media.MediaId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            data = reader[0] as byte[];
                        }
                    }
                }

                // Save to order's folder.
                if (data == null)
                {
                    // No data - skipping
                    _logger.Warn($"Skipping blank media {media.MediaId} for sales order {salesOrderId}");
                    return true;
                }

                var salesOrderFolder = Path.Combine(Options.Directory, $"SO {salesOrderId}");
                if (!Directory.Exists(salesOrderFolder))
                {
                    Directory.CreateDirectory(salesOrderFolder);
                }

                var fileName = Path.Combine(salesOrderFolder, $"{media.Name}_{media.MediaId}.{media.FileExtension}");
                File.WriteAllBytes(fileName, data);

                return true;
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error saving media");
                return false;
            }
        }

        private async Task<bool> DeleteMedia(SqlConnection conn, Media media)
        {
            try
            {
                // Remove SalesOrder_Media relationships
                using (var cmd = conn.CreateCommand())
                {
                    // Assumption: Previous checks ensure that the media is only being used as order media.
                    cmd.CommandText = @"
DELETE FROM SalesOrder_Media
WHERE MediaID = @mediaID;
";
                    cmd.Parameters.AddWithValue("@mediaID", media.MediaId);

                    await cmd.ExecuteNonQueryAsync();
                }

                // Remove Order_Media relationships
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
DELETE FROM Order_Media
WHERE MediaID = @mediaID;
";
                    cmd.Parameters.AddWithValue("@mediaID", media.MediaId);

                    await cmd.ExecuteNonQueryAsync();
                }

                // Remove Receiving_Media relationships
                using (var cmd = conn.CreateCommand())
                {
                    // Assumption: Previous checks ensure that the media is only being used as order media.
                    cmd.CommandText = @"
DELETE FROM Receiving_Media
WHERE MediaID = @mediaID;
";
                    cmd.Parameters.AddWithValue("@mediaID", media.MediaId);

                    await cmd.ExecuteNonQueryAsync();
                }

                // Remove media
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
DELETE FROM Media
WHERE MediaID = @mediaID;
";
                    cmd.Parameters.AddWithValue("@mediaID", media.MediaId);

                    await cmd.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error removing media");
                return false;
            }
        }

        private async Task<List<int>> GetAssociatedOrders(SqlConnection conn, int mediaId)
        {
            var orderIds = new List<int>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT DISTINCT OrderID
FROM Order_Media
WHERE MediaID = @mediaId
";

                cmd.Parameters.AddWithValue("@mediaId", mediaId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        orderIds.Add(reader.GetInt32(0));
                    }
                }
            }

            return orderIds;
        }

        private async Task<List<int>> GetAssociatedSalesOrders(SqlConnection conn, int mediaId)
        {
            var salesOrderIds = new List<int>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT DISTINCT SalesOrderID
FROM SalesOrder_Media
WHERE MediaID = @mediaId
";

                cmd.Parameters.AddWithValue("@mediaId", mediaId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        salesOrderIds.Add(reader.GetInt32(0));
                    }
                }
            }

            return salesOrderIds;
        }

        private void DoNext()
        {
            MessengerInstance.Send(new MoveToStepMessage(Step.Summary));
        }

        private async Task<List<Media>> GetOrderMedia(SqlConnection conn, int orderId)
        {
            var items = new List<Media>();

            // Retrieve list
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT Media.MediaID,
    Media.Name,
    Media.FileExtension,
    DATALENGTH(Media.Media) AS SizeBytes
FROM Media
WHERE MediaID IN (
        SELECT DISTINCT MediaID
        FROM Order_Media
        WHERE OrderID = @orderId
        )";

                cmd.Parameters.AddWithValue("@orderId", orderId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var media = new Media
                        {
                            MediaId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            FileExtension = reader.GetString(2),
                            Size = ByteSize.FromBytes(reader.GetInt64(3))
                        };

                        items.Add(media);
                    }
                }
            }

            // Do not remove media that's in-use elsewhere
            foreach (var media in new List<Media>(items))
            {
                // Not currently able to remove media from OrderApproval
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT COUNT(*) FROM OrderApproval WHERE MediaID = @mediaID;";

                    cmd.Parameters.AddWithValue("@mediaId", media.MediaId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            var count = reader.GetInt32(0);
                            if (count > 0)
                            {
                                items.Remove(media);
                            }
                        }
                    }
                }
            }

            return items;
        }

        private async Task<List<Media>> GetSalesOrderMedia(SqlConnection conn, int salesOrderId)
        {
            var items = new List<Media>();

            // Retrieve list
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT Media.MediaID,
    Media.Name,
    Media.FileExtension,
    DATALENGTH(Media.Media) AS SizeBytes
FROM Media
WHERE MediaID IN (
        SELECT DISTINCT MediaID
        FROM SalesOrder_Media
        WHERE SalesOrderID = @salesOrderId
        )";

                cmd.Parameters.AddWithValue("@salesOrderId", salesOrderId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var media = new Media
                        {
                            MediaId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            FileExtension = reader.GetString(2),
                            Size = ByteSize.FromBytes(reader.GetInt64(3))
                        };

                        items.Add(media);
                    }
                }
            }

            // Do not remove media that's in-use elsewhere
            foreach (var media in new List<Media>(items))
            {
                // Not currently able to remove media from OrderApproval
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT COUNT(*) FROM OrderApproval WHERE MediaID = @mediaID;";

                    cmd.Parameters.AddWithValue("@mediaId", media.MediaId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            var count = reader.GetInt32(0);
                            if (count > 0)
                            {
                                items.Remove(media);
                            }
                        }
                    }
                }
            }

            return items;
        }

        private async Task AddOrderHistory(SqlConnection conn, int orderId)
        {
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
INSERT INTO [OrderHistory] (
    OrderID,
    Category,
    Description,
    UserName,
    Machine,
    DateCreated
    )
VALUES (
    @orderId,
    @category,
    @description,
    @userName,
    @machine,
    @currentTime
    );
";

                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    cmd.Parameters.AddWithValue("@category", "Data Archiver");
                    cmd.Parameters.AddWithValue("@description", "Archived media for this order.");
                    cmd.Parameters.AddWithValue("@userName", "N/A");
                    cmd.Parameters.AddWithValue("@machine", Environment.MachineName.TrimToMaxLength(50));
                    cmd.Parameters.AddWithValue("@currentTime", DateTime.Now);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc, $"Unable to add history for order {orderId}");
            }
        }

        #endregion

        #region OperationStatus

        private enum OperationStatus
        {
            Ready,
            InProgress,
            Finished
        }

        #endregion
    }
}
