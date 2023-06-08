using System;
using System.Collections.Generic;
using DWOS.Data.Datasets;

namespace DWOS.UI
{
    /// <summary>
    /// Interface for <see cref="OrderSummary2"/> - represents an orders tab.
    /// </summary>
    public interface IOrderSummary : IDwosTab
    {
        /// <summary>
        /// Gets the WO for the currently selected order.
        /// </summary>
        int SelectedWO { get; }

        /// <summary>
        /// Gets the work status for the currently selected order.
        /// </summary>
        string SelectedWorkStatus { get; }

        /// <summary>
        /// Gets a value indicating if the currently selected order is on hold.
        /// </summary>
        bool? SelectedHoldStatus { get; }

        /// <summary>
        /// Gets the order type for the currently selected order.
        /// </summary>
        OrderType? SelectedOrderType { get; }

        /// <summary>
        /// Gets the number of active timers for the currently selected order.
        /// </summary>
        int SelectedActiveTimerCount { get; }

        /// <summary>
        /// Gets a value indicating if the currently selected order is in a batch.
        /// </summary>
        bool SelectedInBatch {get; }

        /// <summary>
        /// Gets the location for the currently selected order.
        /// </summary>
        string SelectedLocation { get; }

        /// <summary>
        /// Gets the line for the currently selected order.
        /// </summary>
        int? SelectedLine { get; }

        /// <summary>
        /// Occurs when the user selects a row.
        /// </summary>
        event EventHandler AfterSelectedRowChanged;

        /// <summary>
        /// Selects an order by its order ID.
        /// </summary>
        /// <param name="orderId"></param>
        void SelectWO(int orderId);

        /// <summary>
        /// Gets a list of orders that are currently displayed in the tab.
        /// </summary>
        /// <returns></returns>
        List <int> GetFilteredOrders();

        /// <summary>
        /// Applys a default sort to the grid.
        /// </summary>
        void ApplyDefaultSort();

        void RefreshSettings();
    }
}