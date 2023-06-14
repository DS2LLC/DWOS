using DWOS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.ViewModels
{
    /// <summary>
    /// View Model for check in functionality.
    /// </summary>
    public class CheckInViewModel : ViewModelBase
    {
        #region Fields

        private int? _orderId;
        private int? _batchId;
        private string _nextDepartment; 

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets or sets the ID for the current order.
        /// </summary>
        /// <value>
        /// The order identifier if present; otherwise, <c>null</c>
        /// </value>
        public int? OrderId
        {
            get { return _orderId; }
            set
            {
                _orderId = value;
                _batchId = null;
                Validate();
                OnPropertyChanged("OrderId");
            }
        }

        /// <summary>
        /// Gets or sets the ID for the current batch.
        /// </summary>
        /// <value>
        /// The batch identifier if present; otherwise, <c>null</c>
        /// </value>
        public int? BatchId
        {
            get { return _batchId; }
            set
            {
                _batchId = value;
                _orderId = null;
                Validate();
                OnPropertyChanged("BatchId");
            }
        }

        /// <summary>
        /// Gets or sets the next department.
        /// </summary>
        /// <value>
        /// The next department.
        /// </value>
        public string NextDepartment
        {
            get { return _nextDepartment; }
            set
            {
                _nextDepartment = value;
                Validate();
                OnPropertyChanged("NextDepartment");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Protected method for validating the ViewModel
        /// - Fires PropertyChanged for IsValid and Errors
        /// </summary>
        protected override void Validate()
        {
            if (OrderId.HasValue)
            {
                ValidateProperty(() => OrderId.Value < 0, "Invalid Order Id.");
                var orderViewModel = ServiceContainer.Resolve<OrderViewModel>();
                if (orderViewModel.Orders != null)
                {
                    ValidateProperty(() =>
                    {
                        return !orderViewModel.Orders.Where(order => order.OrderId == OrderId.Value).Any();
                    },
                    "Order Id not found");
                    ValidateProperty(() => string.IsNullOrEmpty(NextDepartment), "Invalid department.");
                }
                else
                    ValidateProperty(() => string.IsNullOrEmpty(NextDepartment), "Invalid department.");
            }
            else if (BatchId.HasValue)
            {
                ValidateProperty(() => BatchId.Value < 0, "Invalid Batch Id.");
                var batchViewModel = ServiceContainer.Resolve<BatchViewModel>();
                if (batchViewModel.Batches != null)
                {
                    ValidateProperty(() =>
                    {
                        return !batchViewModel.Batches.Where(batch => batch.BatchId == BatchId.Value).Any();
                    },
                    "Batch Id not found");
                    ValidateProperty(() => string.IsNullOrEmpty(NextDepartment), "Invalid department.");
                }
                else
                    ValidateProperty(() => string.IsNullOrEmpty(NextDepartment), "Invalid department.");
            }

            ValidateProperty(() => !BatchId.HasValue && !OrderId.HasValue, "No Id");

            base.Validate();
        }

        /// <summary>
        /// Gets a collection of IDs for orders that can be checked in.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetAvailableCheckInOrderIds()
        {
            var orderViewModel = ServiceContainer.Resolve<OrderViewModel>();
            if (orderViewModel.Orders != null)
            {
                var ordersAvailableForCheckin = orderViewModel.Orders
                    .Where(order => order.WorkStatus == ApplicationSettings.Settings.WorkStatusChangingDepartment)
                    .Select(order => order.OrderId);
                return ordersAvailableForCheckin;
            }

            return Enumerable.Empty<int>().ToList();
        }

        /// <summary>
        /// Gets a collection of IDs for batch that can be checked in.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetAvailableCheckInBatchIds()
        {
            var batchViewModel = ServiceContainer.Resolve<BatchViewModel>();
            if (batchViewModel.Batches != null)
            {
                var batchesAvailableForCheckin = batchViewModel.Batches
                    .Where(batch => batch.WorkStatus == ApplicationSettings.Settings.WorkStatusChangingDepartment)
                    .Select(batch => batch.BatchId);
                return batchesAvailableForCheckin;
            }

            return Enumerable.Empty<int>().ToList();
        }

        /// <summary>
        /// Sets <see cref="OrderId"/> from a scanned barcode value.
        /// </summary>
        /// <param name="scannedValue">The scanned value.</param>
        public void SetOrderIdFromScan(string scannedValue)
        {
            OrderId = null;
            NextDepartment = string.Empty;
            int orderId = -1;
            if(Int32.TryParse(scannedValue.Replace("~", ""), out orderId))
                OrderId = orderId;
        }

        /// <summary>
        /// Sets <see cref="BatchId"/> from a scanned barcode value.
        /// </summary>
        /// <param name="scannedValue">The scanned value.</param>
        public void SetBatchIdFromScan(string scannedValue)
        {
            BatchId = null;
            NextDepartment = string.Empty;
            int batchId = -1;
            if (Int32.TryParse(scannedValue.Replace("`", ""), out batchId))
                BatchId = batchId;
        }

        #endregion
    }
}
