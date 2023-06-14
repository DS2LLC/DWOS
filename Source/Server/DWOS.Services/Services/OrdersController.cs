using System.Linq;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Services.Messages;
using System;
using System.Collections.Generic;
using System.Web.Http;
using DWOS.Shared.Utilities;

namespace DWOS.Services
{
    public class OrdersController : ApiController
    {
        #region Methods

        [HttpGet]
        [ServiceExceptionFilter("Error getting all orders.")]
        public ResponseBase GetAll()
        {
            using (new UsingTimeMe("/orders/getall"))
            {
                return new OrdersResponse { Success = true, ErrorMessage = null, Orders = CreateOrderSummaries(null, false, 0) };
            }
        }

        [HttpGet]
        [ServiceExceptionFilter("Error getting an order.")]
        public ResponseBase Get(int orderId, int imageSize=512)
        {
            return new OrderDetailResponse { Success = true, ErrorMessage = null, OrderDetail = CreateOrderDetail(orderId, imageSize) };
        }

        [HttpGet]
        [ServiceExceptionFilter("Error finding order.")]
        public ResponseBase Find(string searchValue=null, bool includeImage=false, int imageSize=512)
        {
            return new OrdersResponse { Success = true, ErrorMessage = null, Orders = CreateOrderSummaries(searchValue, includeImage, imageSize) };
        }

        [HttpPost]
        [DwosAuthenticate]
        [DwosAuthorize(Roles = "PartCheckIn")]
        [ServiceExceptionFilter("Error checking in order.")]
        public ResponseBase Checkin(CheckInRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            using (new UsingTimeMe("/orders/checkin"))
            {
                var orderCheckIn = new OrderCheckInController(request.OrderId);
                var checkInResponse = orderCheckIn.CheckIn(request.NextDepartment, request.UserId);

                var response = new ResponseBase { Success = checkInResponse.Response, ErrorMessage = null };

                if (!checkInResponse.Response)
                    response.ErrorMessage = checkInResponse.Description;

                return response;
            }
        }

        [HttpGet]
        [ServiceExceptionFilter("Error getting order schedule.")]
        public ResponseBase Schedule(string department)
        {
            using (new UsingTimeMe("/orders/schedule"))
            {
                return new OrderScheduleResponse { Success = true, ErrorMessage = null, Schedule = CreateSchedule(department) };
            }
        }

        #endregion

        #region Factories

        private static OrderDetailInfo CreateOrderDetail(int orderId, int imageSize)
        {
            var order = new OrderDetailInfo();
           
            try
            {
                var taOrders = new DWOS.Data.Reports.OrdersReportTableAdapters.OrderTableAdapter();
                var orderRow = taOrders.GetByOrder(orderId).FirstOrDefault();

                if (orderRow == null)
                    return order;

                order.OrderId = orderId;
                order.PartName = orderRow.PartName;
                order.WorkStatus = orderRow.WorkStatus;
                order.Location = orderRow.CurrentLocation;
                order.CurrentLine = orderRow.IsCurrentLineNull() ? null : GetProcessingLineName(orderRow.CurrentLine);
                order.OrderDate = orderRow.IsOrderDateNull() ? DateTime.MinValue : orderRow.OrderDate;
                order.EstShipDate = orderRow.IsEstShipDateNull() ? DateTime.MinValue : orderRow.EstShipDate;
                order.ReqDate = orderRow.IsRequiredDateNull() ? DateTime.MinValue : orderRow.RequiredDate;
                order.CustomerName = orderRow.CustomerName;
                order.CustomerWO = orderRow.IsCustomerWONull() ? null : orderRow.CustomerWO;
                order.PO = orderRow.IsPurchaseOrderNull() ? null : orderRow.PurchaseOrder;
                order.Priority = orderRow.IsPriorityNull() ? null : orderRow.Priority;
                order.SchedulePriority = orderRow.SchedulePriority;
                order.Quantity = orderRow.IsPartQuantityNull() ? 0 : orderRow.PartQuantity;
                order.ShippingMethod = orderRow.IsShippingMethodNull()
                    ? null
                    : taOrders.GetShippingCarrier(orderRow.ShippingMethod);

                order.OrderType = ((OrderType) orderRow.OrderType).ToDisplayString();
                order.Hold = orderRow.Hold;
                order.PartId = orderRow.PartID;

                order.IsInBatch = taOrders.GetInBatch(orderId) ?? false;

                // Part Marking
                using (var taPartMark = new Data.Datasets.OrdersDataSetTableAdapters.OrderPartMarkTableAdapter())
                {
                    using (var partMarksTable = taPartMark.GetByOrder(orderId))
                    {
                        var partMark = partMarksTable.FirstOrDefault();

                        if (partMark != null)
                        {
                            order.PartMarkLines = new PartMarkInfo
                            {
                                Line1 = partMark.IsLine1Null() ? null : partMark.Line1,
                                Line2 = partMark.IsLine2Null() ? null : partMark.Line2,
                                Line3 = partMark.IsLine3Null() ? null : partMark.Line3,
                                Line4 = partMark.IsLine4Null() ? null : partMark.Line4
                            };
                        }
                    }
                }

                // Thumbnail
                using(var taMedia     = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter())
                    using (var mediaTable = taMedia.GetPartsWOMediaByOrder(orderId))
                    {
                        var media = mediaTable.FirstOrDefault(m => m.FileExtension.Contains("jpg")) ?? mediaTable.FirstOrDefault();

                        if(media != null)
                        {
                            if (imageSize < 32)
                                imageSize = 32;

                            var thumb = MediaUtilities.GetThumbnail(media.MediaID, media.FileExtension, taMedia.GetMediaStream(media.MediaID), imageSize);
                            order.PartImage = thumb.GetImageAsBytes();
                        }
                    }

                // Documents
                using (var orderDocumentTable = new OrdersDataSet.Order_DocumentLinkDataTable())
                {
                    using (var taOrderDocument = new Data.Datasets.OrdersDataSetTableAdapters.Order_DocumentLinkTableAdapter())
                    {
                        taOrderDocument.FillByOrder(orderDocumentTable, orderId);
                    }

                    if (orderDocumentTable.Count > 0)
                    {
                        order.Documents = new List<DocumentInfo>();

                        using (var taDocument = new Data.Datasets.DocumentsDataSetTableAdapters.DocumentInfoTableAdapter())
                        {
                            using (var dtDocument = new DocumentsDataSet.DocumentInfoDataTable())
                            {
                                foreach (var orderDocRow in orderDocumentTable)
                                {
                                    taDocument.FillByID(dtDocument, orderDocRow.DocumentInfoID);

                                    var document = dtDocument.FindByDocumentInfoID(orderDocRow.DocumentInfoID);
                                    var currentRevisionId = (taDocument.GetCurrentRevisionID(orderDocRow.DocumentInfoID) as int?);

                                    if (currentRevisionId.HasValue)
                                    {
                                        order.Documents.Add(new DocumentInfo()
                                        {
                                            CurrentRevisionId = currentRevisionId.Value,
                                            Name = document.Name,
                                            DocumentInfoID = orderDocRow.DocumentInfoID,
                                            MediaType = document.MediaType,
                                            DocumentType = "Order"
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                // Media
                using (var dtMedia = new OrdersDataSet.MediaDataTable())
                {
                    using (var taMedia = new Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter())
                    {
                        taMedia.FillByOrder(dtMedia, orderId);
                    }

                    if (dtMedia.Count > 0)
                    {
                        order.Media = new List<MediaSummary>();

                        foreach (var mediaRow in dtMedia)
                        {
                            order.Media.Add(new MediaSummary()
                            {
                                MediaId = mediaRow.MediaID,
                                Name = mediaRow.Name,
                                FileExtension = mediaRow.IsFileExtensionNull() ? string.Empty : mediaRow.FileExtension
                            });
                        }
                    }
                }

                // Note count
                order.OrderNoteCount = taOrders.GetNoteCount(orderId) ?? 0;

                // Is in batch
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting order");
            }

            return order;
        }

        private static string GetProcessingLineName(int processingLineId)
        {
            using (var taProcessingLine = new Data.Datasets.OrderStatusDataSetTableAdapters.ProcessingLineTableAdapter())
            {
                return taProcessingLine.GetById(processingLineId).FirstOrDefault()?.Name;
            }
        }

        private static List<OrderInfo> CreateOrderSummaries(string searchValue, bool includeImages, int imageSize)
        {
            var orders = new List<OrderInfo>();
            DWOS.Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter taMedia = null;

            try
            {
                var orderTable = new Data.Datasets.OrderStatusDataSet.OrderSearchDataTable();
                using(var ta = new Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter())
                {
                    if(string.IsNullOrWhiteSpace(searchValue))
                        ta.FillBySearch(orderTable, Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter.OrderSearchField.None, null, true);
                    else
                        ta.FillBySearch(orderTable, Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter.OrderSearchField.All, searchValue, true);
                }

                //set min size
                if (imageSize < 32)
                    imageSize = 32;

                if (includeImages)
                    taMedia = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter();

                foreach (var order in orderTable)
                {
                    byte[] image = null;

                    if (includeImages)
                    {
                        var mediaTable = taMedia.GetPartsWOMediaByOrder(order.OrderID);
                        var media = mediaTable.FirstOrDefault(m => m.FileExtension.Contains("jpg")) ?? mediaTable.FirstOrDefault();

                        if (media != null)
                        {
                            var thumb = MediaUtilities.GetThumbnail(media.MediaID, media.FileExtension, taMedia.GetMediaStream(media.MediaID), imageSize);
                            image = thumb.GetImageAsBytes();
                        }
                    }

                    orders.Add(new OrderInfo() 
                    { 
                        OrderId = order.OrderID, 
                        PartName = order.PartName, 
                        WorkStatus = order.WorkStatus, 
                        Image = image, 
                        Location = order.CurrentLocation,
                        SchedulePriority = order.SchedulePriority,
                        CurrentLine = order.IsCurrentLineNull() ? null : order.CurrentLine
                    });
                }

            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error creating order summaries with search value " + searchValue);
            }
            finally
            {
                if (taMedia != null)
                    taMedia.Dispose();
            }

            return orders;
        }

        private static OrderSchedule CreateSchedule(string department)
        {
            List<int> orderIds;
            using (var dtOrderStatus = new OrderStatusDataSet.OrderStatusDataTable())
            {
                using (var taOrderStatus = new Data.Datasets.OrderStatusDataSetTableAdapters.OrderStatusTableAdapter())
                {
                    taOrderStatus.Fill(dtOrderStatus);
                }

                IEnumerable<OrderStatusDataSet.OrderStatusRow> scheduledOrders;

                if (string.IsNullOrEmpty(department))
                {
                    scheduledOrders = dtOrderStatus
                        .Where(o => o.SchedulePriority > 0 && (o.WorkStatus == ApplicationSettings.Current.WorkStatusInProcess || o.WorkStatus == ApplicationSettings.Current.WorkStatusChangingDepartment));
                }
                else
                {
                    var inProcess = dtOrderStatus
                        .Where(o => o.CurrentLocation == department && o.WorkStatus == ApplicationSettings.Current.WorkStatusInProcess);

                    var checkIn = dtOrderStatus
                        .Where(o => !o.IsNextDeptNull() && o.NextDept == department && o.WorkStatus == ApplicationSettings.Current.WorkStatusChangingDepartment);

                    scheduledOrders = inProcess.Concat(checkIn)
                        .Where(o => o.SchedulePriority > 0);
                }

                orderIds = scheduledOrders
                    .OrderBy(o => o.SchedulePriority)
                    .Select(o => o.WO)
                    .ToList();
            }

            return new OrderSchedule
            {
                OrderIds = orderIds
            };
        }

        #endregion
    }
}