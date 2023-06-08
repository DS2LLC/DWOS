using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Interop;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.UI.Admin;
using DWOS.UI.Sales;
using NLog;

namespace DWOS.UI.Utilities
{
    internal static class SplitUtilities
    {
        public static List<OrdersDataSet.OrderRow> DoSplit(OrdersDataSet.OrderRow orgOrder, OrdersDataSet dsOrders,
            TableAdapterManager taManager)
        {
            var orgQty = orgOrder.IsPartQuantityNull()
                ? 0
                : orgOrder.PartQuantity;

            var newOrders = new List<OrdersDataSet.OrderRow>();
            using (var so = new SplitOrder(orgQty, orgOrder.OrderID))
            {
                so.PrintTravelers = UserSettings.Default.SplitPrintTraveler;
                if (so.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                {
                    UserSettings.Default.SplitPrintTraveler = so.PrintTravelers;
                    UserSettings.Default.Save();

                    var factory = new OrderFactory();
                    factory.Load(dsOrders, taManager);
                    newOrders = factory.SplitOrder(orgOrder, so.SplitOrders.ToList(),
                        so.ReasonCode);

                    // Split containers
                    if (orgOrder.GetOrderContainersRows().Length > 0)
                    {
                        var dialog = new SplitContainers();
                        var helper = new WindowInteropHelper(dialog) {Owner = DWOSApp.MainForm.Handle};

                        var ordersInSplit = new List<OrdersDataSet.OrderRow>();
                        ordersInSplit.Add(orgOrder);
                        ordersInSplit.AddRange(newOrders);
                        dialog.Load(orgOrder.OrderID, orgQty, ordersInSplit, dsOrders.ShipmentPackageType);

                        if (dialog.ShowDialog() ?? false)
                        {
                            dialog.Sync(dsOrders.OrderContainers, dsOrders.OrderContainerItem);
                        }

                        GC.KeepAlive(helper);
                    }

                    // Split serial numbers
                    // Assumption: If an order only has one serial number, then it does not need to be removed.
                    if (orgOrder.GetOrderSerialNumberRows().Length > 1)
                    {
                        var dialog = new SplitSerialNumbers();
                        var helper = new WindowInteropHelper(dialog) {Owner = DWOSApp.MainForm.Handle};

                        var ordersInSplit = new List<OrdersDataSet.OrderRow> {orgOrder};
                        ordersInSplit.AddRange(newOrders);
                        dialog.Load(orgOrder.OrderID, ordersInSplit);

                        if (dialog.ShowDialog() ?? false)
                        {
                            dialog.Sync(dsOrders.OrderSerialNumber);
                        }

                        GC.KeepAlive(helper);
                    }

                    // Set shipment quantities
                    using (var taOrderShipment = new OrderShipmentTableAdapter())
                    {
                        foreach (var orgShipment in orgOrder.GetOrderShipmentRows())
                        {
                            var shipmentPackageId = orgShipment.IsShipmentPackageIDNull()
                                ? -1
                                : orgShipment.ShipmentPackageID;

                            var updateShipmentQty = orgShipment.PartQuantity == orgQty &&
                                                    (taOrderShipment.IsShipmentPackageActive(shipmentPackageId) ??
                                                     false);

                            if (updateShipmentQty)
                            {
                                orgShipment.PartQuantity = orgOrder.IsPartQuantityNull()
                                    ? 0
                                    : orgOrder.PartQuantity;
                            }
                        }
                    }

                    foreach (var newOrder in newOrders)
                    {
                        foreach (var shipment in newOrder.GetOrderShipmentRows())
                        {
                            shipment.PartQuantity = newOrder.IsPartQuantityNull()
                                ? 0
                                : newOrder.PartQuantity;
                        }
                    }

                    // Checking for incorrect departments for processes
                    // Please see VSTS #19784
                    var orgOrderProcesses = orgOrder.GetOrderProcessesRows();
                    var newOrderProcessArrays = newOrders.Select(o => o.GetOrderProcessesRows()).ToList();

                    if (newOrderProcessArrays.Any(p => p.Length != orgOrderProcesses.Length))
                    {
                        LogManager.GetCurrentClassLogger().Error("Split created order with incorrect number of processes.");
                    }
                    else
                    {
                        var stopChecking = false;
                        for (var i = 0; i < orgOrderProcesses.Length; i++)
                        {
                            var orgOrderProcess = orgOrderProcesses[i];
                            foreach (var newOrderProcesses in newOrderProcessArrays)
                            {
                                var newOrderProcess = newOrderProcesses[i];

                                if (orgOrderProcess.Department != newOrderProcess.Department)
                                {
                                    LogManager.GetCurrentClassLogger().Error("Split created order with incorrect department for process.");
                                    stopChecking = true;
                                    break;
                                }
                            }

                            if (stopChecking)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return newOrders;
        }
    }
}