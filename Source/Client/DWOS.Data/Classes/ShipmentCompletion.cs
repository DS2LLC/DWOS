using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderShipmentDataSetTableAdapters;
using DWOS.Data.Order;
using DWOS.Shared.Utilities;
using NLog;

namespace DWOS.Data
{
    /// <summary>
    /// Utility class used to close shipments.
    /// </summary>
    public class ShipmentCompletion : IDisposable
    {
        #region Fields

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();
        private readonly SqlConnection _conn;

        #endregion

        #region Properties

        public OrderShipmentDataSet Dataset { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentCompletion"/> class.
        /// </summary>
        public ShipmentCompletion(OrderShipmentDataSet dataset)
        {
            Dataset = dataset ?? throw new ArgumentNullException(nameof(dataset));
            _conn = DbConnectionFactory.NewConnection();
            _conn.Open();
        }

        /// <summary>
        /// Syncs data from <paramref name="shipRow"/> to <paramref name="orderShipments"/>.
        /// </summary>
        /// <param name="shipRow"></param>
        /// <param name="orderShipments"></param>
        /// <param name="shippingUserId"></param>
        public void CloseShipment(OrderShipmentDataSet.ShipmentPackageRow shipRow, OrderShipmentDataSet.OrderShipmentRow[] orderShipments, ISecurityUserInfo shippingUser)
        {
            //update all values that can be edited by user for each order shipment
            foreach(var osRow in orderShipments)
            {
                osRow.PackingSlipID = null; //No longer used
                osRow.ShippingUserID = shippingUser.UserID;

                //Change based on PBI: 12784. User is able to change ship date now (including to future dates)
                //Fyi: shipRow.CloseDate is initally used as placeholder for user to change ship date
                //osRow.DateShipped gets assigned the shipRow.CloseDate (which contains the ship date specified by user) that is now completing shipment process
                //shipRow.CloseDate will then get updated in ShippingManager.ClosePackage() with current date of completion
                osRow.DateShipped = shipRow.CloseDate;

                osRow.ShippingCarrierID = shipRow.IsShippingCarrierIDNull() ? null : shipRow.ShippingCarrierID;

                if (shipRow.IsCustomerAddressIDNull())
                {
                    osRow.SetCustomerAddressIDNull();
                }
                else
                {
                    osRow.CustomerAddressID = shipRow.CustomerAddressID;
                }

                osRow.TrackingNumber = shipRow.IsTrackingNumberNull() ? null : shipRow.TrackingNumber;
                osRow.CarrierCustomerNumber = shipRow.IsCarrierCustomerNumberNull() ? null : shipRow.CarrierCustomerNumber;
            }

            //update database for shipment package and order shipments
            shipRow.Active = false;
            shipRow.CloseDate = DateTime.Now;

            // Save changes to shipment
            using (var transaction = _conn.BeginTransaction())
            {
                try
                {
                    using (var taShipmentPackage = new ShipmentPackageTableAdapter { Connection = _conn, Transaction = transaction })
                    {
                        var updates = taShipmentPackage.Update(Dataset.ShipmentPackage);
                        _log.Info($"Updated { updates } shipment package(s).");
                    }

                    using (var taOrderShipment = new OrderShipmentTableAdapter { Connection = _conn, Transaction = transaction })
                    {
                        var updates = taOrderShipment.Update(Dataset.OrderShipment);
                        _log.Info($"Updated { updates } order shipment(s).");
                    }

                    // Update order history for all orders in the shipment.
                    using (var taOrderHistory = new Datasets.OrderHistoryDataSetTableAdapters.OrderHistoryTableAdapter { Connection = _conn, Transaction = transaction })
                    {
                        foreach (var shipment in orderShipments)
                        {
                            taOrderHistory.UpdateOrderHistory(shipment.OrderID,
                                "Shipping",
                                "Order " + shipment.OrderID + " was shipped in package " + shipment.ShipmentPackageID + ".",
                                shippingUser.UserName);
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    _log.Warn($"Failure in {nameof(CloseShipment)}.");

                    try
                    {
                        transaction.Rollback();
                        Dataset.RejectChanges(); // Try to undo unsaved changes
                    }
                    catch (Exception cancelException)
                    {
                        _log.Error(cancelException, "Error canceling transaction.");
                    }

                    throw;
                }
            }
        }

        /// <summary>
        /// Closes all of a shipment's orders.
        /// </summary>
        /// <param name="orderShipments"></param>
        /// <param name="dsOrderShipment"></param>
        public void CloseOrders(OrderShipmentDataSet.OrderShipmentRow[] orderShipments)
        {
            var completionDate = DateTime.Now;

            SqlTransaction transaction = null;
            OrderTableAdapter taOrders = null;
            SalesOrderTableAdapter taSalesOrders = null;

            try
            {
                transaction = _conn.BeginTransaction();
                taOrders = new OrderTableAdapter { Connection = _conn, Transaction = transaction, ClearBeforeFill = false };
                taSalesOrders = new SalesOrderTableAdapter { Connection = _conn, Transaction = transaction, ClearBeforeFill = false };
                var orderTimerHelper = new OrderTimerHelper(_conn, transaction);

                var updates = 0;

                var salesOrderIds = new HashSet<int>();

                foreach (var os in orderShipments)
                {
                    _log.Info("Closing order: " + os.OrderID);

                    taOrders.CompleteOrder(ApplicationSettings.Current.WorkStatusCompleted,
                        completionDate,
                        ApplicationSettings.Current.DepartmentShipping,
                        os.OrderID);

                    orderTimerHelper.StopAllOrderTimers(os.OrderID);

                    updates++;

                    //Verify and close sales order
                    taOrders.FillByOrderID(Dataset.Order, os.OrderID);
                    var orderRow = Dataset.Order.FindByOrderID(os.OrderID);

                    if (orderRow != null && !orderRow.IsSalesOrderIDNull())
                    {
                        salesOrderIds.Add(orderRow.SalesOrderID);
                    }
                }

                foreach (var salesOrderId in salesOrderIds)
                {
                    //Check if there are any open orders that are assigned to this sales order
                    int count = taOrders.GetOpenOrdersCountBySalesOrderID(salesOrderId)
                        .GetValueOrDefault();

                    if (count == 0)
                    {
                        _log.Info($"Closing sales order: {salesOrderId}.");
                        taSalesOrders.CompleteSalesOrder(completionDate, salesOrderId);
                    }
                }

                _log.Info("Closed " + updates + " orders.");
                transaction.Commit();
            }
            catch (Exception)
            {
                _log.Warn($"Failure in {nameof(CloseOrders)}.");

                try
                {
                    transaction.Rollback();
                    Dataset.RejectChanges();
                }
                catch (Exception cancelException)
                {
                    _log.Error(cancelException, "Error canceling transaction.");
                }

                throw;
            }
            finally
            {
                taOrders?.Dispose();
                taSalesOrders?.Dispose();
                transaction?.Dispose();
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _conn.Close();
        }

        #endregion
    }
}
