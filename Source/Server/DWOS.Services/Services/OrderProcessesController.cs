using System.Linq;
using DWOS.Services.Messages;
using NLog;
using System;
using System.Collections.Generic;
using DWOS.Data.Order;
using DWOS.Data;
using DWOS.Data.Datasets;
using System.Web.Http;

namespace DWOS.Services
{
    public class OrderProcessesController : ApiController
    {
        #region Methods

        [HttpGet]
        [ServiceExceptionFilter("Error getting order processes.")]
        public ResponseBase GetAll(int orderId)
        {
            return new OrderProcessesResponse
            {
                Success = true,
                ErrorMessage = null,
                OrderProcesses = Create(orderId),
                OrderStatus = CreateOrderStatus(orderId)
            };
        }

        [HttpGet]
        [ServiceExceptionFilter("Error retrieving current process for order.")]
        public ResponseBase GetCurrent(int orderId)
        {
            var orderProcessRow = GetCurrentOrderProcess(orderId);

            if (orderProcessRow == null)
            {
                return new ResponseBase() { ErrorMessage = $"Could not find process for order {orderId}.", Success = false };
            }
            else
            {
                var process = CreateDetails(orderProcessRow.OrderProcessesID);
                var alias = ServiceUtilities.CreateProcessAlias(orderProcessRow);

                return new OrderCurrentProcessResponse
                {
                    Success = true,
                    ErrorMessage = null,
                    OrderProcessId = orderProcessRow.OrderProcessesID,
                    Process = process,
                    ProcessAlias = alias
                };
            }
        }

        [HttpGet]
        [ServiceExceptionFilter("Error getting order process details")]
        public ResponseBase Get(int orderProcessId)
        {
            return new OrderProcessDetailResponse
            {
                Success = true,
                ErrorMessage = null,
                OrderProcessInfo = CreateDetails(orderProcessId)
            };
        }

        #endregion

        #region Factories

        private static List<OrderProcessInfo> Create(int orderId)
        {
            var orderProcesses = new List<OrderProcessInfo>();
            OrdersDataSet.OrderProcessesDataTable dtOrder = null;
            OrderProcessingDataSet.ProcessDataTable dtProcess = null;
            OrderProcessingDataSet.ProcessAliasDataTable dtProcessAlias = null;

            try
            {
                dtOrder = new OrdersDataSet.OrderProcessesDataTable();
                dtProcess = new OrderProcessingDataSet.ProcessDataTable();
                dtProcessAlias = new OrderProcessingDataSet.ProcessAliasDataTable();

                using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderProcessesTableAdapter())
                {
                    ta.FillBy(dtOrder, orderId);
                }

                using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter())
                {
                    ta.FillByOrder(dtProcess, orderId);
                }

                using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessAliasTableAdapter())
                {
                    ta.Fill(dtProcessAlias, orderId);
                }

                foreach (var orderProcessRow in dtOrder.OrderBy(op => op.StepOrder))
                {
                    var processRow = dtProcess.FindByProcessID(orderProcessRow.ProcessID);
                    var processAliasRow = dtProcessAlias.FindByProcessAliasID(orderProcessRow.ProcessAliasID);

                    var orderProcessInfo = new OrderProcessInfo()
                    {
                        OrderId = orderId,
                        OrderProcessId = orderProcessRow.OrderProcessesID,
                        StepOrder = orderProcessRow.StepOrder,
                        Department = orderProcessRow.Department,
                        Started = orderProcessRow.IsStartDateNull() ? DateTime.MinValue : orderProcessRow.StartDate,
                        Ended = orderProcessRow.IsEndDateNull() ? DateTime.MinValue : orderProcessRow.EndDate,
                        ProcessName = processRow.Name,
                        ProcessAliasName = processAliasRow.Name,
                        ProcessId = processRow.ProcessID
                    };

                    Tuple<int?,decimal?> loadCapacityInfo = GetLoadCapacityInfo(orderProcessRow, orderId);

                    if (loadCapacityInfo != null)
                    {
                        orderProcessInfo.FixtureCount = loadCapacityInfo.Item1;
                        orderProcessInfo.WeightPerFixture = loadCapacityInfo.Item2;
                    }

                    orderProcesses.Add(orderProcessInfo);
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error ");
            }
            finally
            {
                dtOrder?.Dispose();
                dtProcess?.Dispose();
                dtProcessAlias?.Dispose();
            }

            return orderProcesses;
        }

        private static Tuple<int?, decimal?> GetLoadCapacityInfo(Data.Datasets.OrdersDataSet.OrderProcessesRow orderProcessRow, int orderId)
        {
            Data.Datasets.OrdersDataSet.OrderDataTable dtOrder = null;

            try
            {

                if (!ApplicationSettings.Current.UseLoadCapacity || orderProcessRow == null)
                {
                    return null;
                }

                dtOrder = new Data.Datasets.OrdersDataSet.OrderDataTable();
                using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter())
                {
                    ta.FillByOrderID(dtOrder, orderId);
                }

                var orderRow = dtOrder.FindByOrderID(orderId);

                var capacity = LoadCapacity.FromOrderProcess(orderRow, orderProcessRow) ??
                    LoadCapacity.FromMatchingPartProcess(orderRow, orderProcessRow.OrderProcessesID) ??
                    LoadCapacity.FromProcess(orderRow, orderProcessRow.ProcessID); 

                return new Tuple<int?, decimal?>(capacity?.FixtureCount, capacity?.WeightPerFixture);
            }
            finally
            {
                dtOrder?.Dispose();
            }
        }

        private static OrderStatusInfo CreateOrderStatus(int orderId)
        {
            var order = new OrderStatusInfo();

            try
            {
                using(var taOrders = new DWOS.Data.Reports.OrdersReportTableAdapters.OrderTableAdapter())
                {
                    var orderRow = taOrders.GetByOrder(orderId).FirstOrDefault();

                    if(orderRow == null)
                        return order;

                    order.OrderId    = orderId;
                    order.WorkStatus = orderRow.WorkStatus;
                    order.Location   = orderRow.CurrentLocation;
                    order.CurrentLine = orderRow.IsCurrentLineNull() ? (int?)null : orderRow.CurrentLine;

                    using(var taOrderProcesses = new DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                        order.NextDepartment = taOrderProcesses.GetNextDepartment(orderId);
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting order");
            }

            return order;
        }

        private static OrderProcessDetailInfo CreateDetails(int orderProcessId)
        {
            try
            {
                return ServiceUtilities.CreateInfoForOrderProcess(orderProcessId);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting order process details.");
                return null;
            }
        }

        private static OrderProcessingDataSet.OrderProcessesRow GetCurrentOrderProcess(int orderId)
        {
            var dtOrderProcess = new Data.Datasets.OrderProcessingDataSet.OrderProcessesDataTable();
            using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
            {
                ta.FillCurrentProcess(dtOrderProcess, orderId);
            }

            return dtOrderProcess.FirstOrDefault();
        }

        #endregion
    }
}