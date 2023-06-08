using System;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Datasets.ProcessesDatasetTableAdapters;

namespace DWOS.Data.Order
{
    /// <summary>
    /// Defines utility methods related to load capacity.
    /// </summary>
    public class LoadCapacity
    {
        #region Properties

        /// <summary>
        /// Gets the fixture count for this instance.
        /// </summary>
        /// <returns></returns>
        public int? FixtureCount { get; set; }

        /// <summary>
        /// Gets the weight per fixture for this instance.
        /// </summary>
        public decimal? WeightPerFixture { get; set; }

        /// <summary>
        /// Gets the source of capacity information.
        /// </summary>
        public CapacitySource Source { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadCapacity"/> class.
        /// </summary>
        /// <param name="source">The source of capacity information</param>
        public LoadCapacity(CapacitySource source)
        {
            Source = source;
        }

        /// <summary>
        /// Retrieves load capacity from an order and its order process.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderProcess"></param>
        /// <returns></returns>
        public static LoadCapacity FromOrderProcess(OrdersDataSet.OrderRow order, OrderProcessingDataSet.OrderProcessesRow orderProcess)
        {
            if (order == null || orderProcess == null)
            {
                return null;
            }

            var loadCapacityVariance = orderProcess.IsLoadCapacityVarianceNull() ?
                0M :
                orderProcess.LoadCapacityVariance;

            int? fixtureCount = null;

            if (!orderProcess.IsLoadCapacityQuantityNull() && orderProcess.LoadCapacityQuantity > 0 && !order.IsPartQuantityNull())
            {
                fixtureCount = FixturesFromQuantity(
                    order.PartQuantity,
                    orderProcess.LoadCapacityQuantity,
                    loadCapacityVariance);
            }
            else if (!orderProcess.IsLoadCapacityWeightNull() && orderProcess.LoadCapacityWeight > 0M && !order.IsWeightNull())
            {
                fixtureCount = FixturesFromWeight(
                    order.Weight,
                    orderProcess.LoadCapacityWeight,
                    loadCapacityVariance);
            }

            if (fixtureCount > 0)
            {
                decimal? weightPerFixture = null;
                if (!order.IsWeightNull() && fixtureCount > 0)
                {
                    weightPerFixture = order.Weight / Convert.ToDecimal(fixtureCount);
                }

                return new LoadCapacity(CapacitySource.OrderProcess)
                {
                    FixtureCount = fixtureCount,
                    WeightPerFixture = weightPerFixture
                };
            }

            return null;
        }

        /// <summary>
        /// Retrieves load capacity from an order and its order process.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderProcess"></param>
        /// <returns></returns>
        public static LoadCapacity FromOrderProcess(OrdersDataSet.OrderRow order, OrdersDataSet.OrderProcessesRow orderProcess)
        {
            if (order == null || orderProcess == null)
            {
                return null;
            }

            var loadCapacityVariance = orderProcess.IsLoadCapacityVarianceNull() ?
                0M :
                orderProcess.LoadCapacityVariance;

            int? fixtureCount = null;

            if (!orderProcess.IsLoadCapacityQuantityNull() && orderProcess.LoadCapacityQuantity > 0 && !order.IsPartQuantityNull())
            {
                fixtureCount = FixturesFromQuantity(
                    order.PartQuantity,
                    orderProcess.LoadCapacityQuantity,
                    loadCapacityVariance);
            }
            else if (!orderProcess.IsLoadCapacityWeightNull() && orderProcess.LoadCapacityWeight > 0M && !order.IsWeightNull())
            {
                fixtureCount = FixturesFromWeight(
                    order.Weight,
                    orderProcess.LoadCapacityWeight,
                    loadCapacityVariance);
            }

            if (fixtureCount > 0)
            {
                decimal? weightPerFixture = null;
                if (!order.IsWeightNull() && fixtureCount > 0)
                {
                    weightPerFixture = order.Weight / Convert.ToDecimal(fixtureCount);
                }

                return new LoadCapacity(CapacitySource.OrderProcess)
                {
                    FixtureCount = fixtureCount,
                    WeightPerFixture = weightPerFixture
                };
            }

            return null;
        }

        /// <summary>
        /// Retrieves load capacity information from an order and the part
        /// process for a given order process.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderProcessesId"></param>
        /// <returns></returns>
        public static LoadCapacity FromMatchingPartProcess(OrdersDataSet.OrderRow order, int orderProcessesId)
        {
            if (order == null)
            {
                return null;
            }

            var partId = order.IsPartIDNull() ? -1 : order.PartID;

            OrdersDataSet.OrderProcessesDataTable dtOrderProcesses = null;
            OrdersDataSet.PartProcessSummaryDataTable dtPartProcessDataTable = null;

            try
            {
                dtOrderProcesses = new OrdersDataSet.OrderProcessesDataTable();

                using (var taOrderProcesses = new OrderProcessesTableAdapter())
                {
                    taOrderProcesses.FillByID(dtOrderProcesses, orderProcessesId);
                }

                var orderProcess = dtOrderProcesses.FirstOrDefault();

                if (orderProcess != null && orderProcess.OrderProcessType != (int)OrderProcessType.Rework)
                {
                    using (var taPartProcess = new PartProcessSummaryTableAdapter())
                    {
                        dtPartProcessDataTable = taPartProcess.GetData(partId);
                    }

                    var match = dtPartProcessDataTable.FirstOrDefault(p => p.StepOrder == orderProcess.StepOrder && p.ProcessID == orderProcess.ProcessID) ??
                        dtPartProcessDataTable.FirstOrDefault(p => p.ProcessID == orderProcess.ProcessID);

                    return FromPartProcess(order, match);
                }

                return null;
            }
            finally 
            {
                dtOrderProcesses?.Dispose();
                dtPartProcessDataTable?.Dispose();
            }
        }

        /// <summary>
        /// Retrieves load capacity information from an order and a specific
        /// process.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        public static LoadCapacity FromProcess(OrdersDataSet.OrderRow order, int processId)
        {
            if (order == null)
            {
                return null;
            }

            using (var dtProcess = new ProcessesDataset.ProcessDataTable())
            {
                using (var taProcess = new ProcessTableAdapter())
                {
                    taProcess.FillByProcess(dtProcess, processId);
                }

                return FromProcess(order, dtProcess.FirstOrDefault());
            }
        }

        private static LoadCapacity FromPartProcess(OrdersDataSet.OrderRow order, OrdersDataSet.PartProcessSummaryRow partProcess)
        {
            if (order == null || partProcess == null)
            {
                return null;
            }

            var loadCapacityVariance = partProcess.IsLoadCapacityVarianceNull() ?
                0M :
                partProcess.LoadCapacityVariance;

            int? fixtureCount = null;

            if (!partProcess.IsLoadCapacityQuantityNull() && partProcess.LoadCapacityQuantity > 0 && !order.IsPartQuantityNull())
            {
                fixtureCount = FixturesFromQuantity(
                    order.PartQuantity,
                    partProcess.LoadCapacityQuantity,
                    loadCapacityVariance);
            }
            else if (!partProcess.IsLoadCapacityWeightNull() && partProcess.LoadCapacityWeight > 0M && !order.IsWeightNull())
            {
                fixtureCount = FixturesFromWeight(
                    order.Weight,
                    partProcess.LoadCapacityWeight,
                    loadCapacityVariance);
            }

            if (fixtureCount > 0)
            {
                decimal? weightPerFixture = null;
                if (!order.IsWeightNull() && fixtureCount > 0)
                {
                    weightPerFixture = order.Weight / Convert.ToDecimal(fixtureCount);
                }

                return new LoadCapacity(CapacitySource.PartProcess)
                {
                    FixtureCount = fixtureCount,
                    WeightPerFixture = weightPerFixture
                };
            }

            return null;
        }

        private static LoadCapacity FromProcess(OrdersDataSet.OrderRow order, ProcessesDataset.ProcessRow process)
        {
            if (order == null || process == null || process.IsLoadCapacityNull() || process.IsLoadCapacityTypeNull())
            {
                return null;
            }

            var loadCapacityVariance = process.IsLoadCapacityVarianceNull() ?
                0M :
                process.LoadCapacityVariance;

            int? fixtureCount = null;

            if (process.LoadCapacityType.ToUpper() == "QUANTITY" && !order.IsPartQuantityNull())
            {
                const int maxQuantity = 999999999;

                var roundedCapacity = Math.Round(process.LoadCapacity, MidpointRounding.AwayFromZero);
                var loadCapacity = Math.Min(Convert.ToInt32(roundedCapacity), maxQuantity);

                fixtureCount = FixturesFromQuantity(
                    order.PartQuantity,
                    loadCapacity,
                    loadCapacityVariance);
            }
            else if (process.LoadCapacityType.ToUpper() == "WEIGHT" && !order.IsWeightNull())
            {
                const decimal maxWeight = 999999.99999999M;

                var loadCapacity = Math.Min(process.LoadCapacity, maxWeight);

                fixtureCount = FixturesFromWeight(
                    order.Weight,
                    loadCapacity,
                    loadCapacityVariance);
            }

            if (fixtureCount > 0)
            {
                decimal? weightPerFixture = null;
                if (!order.IsWeightNull() && fixtureCount > 0)
                {
                    weightPerFixture = order.Weight / Convert.ToDecimal(fixtureCount);
                }

                return new LoadCapacity(CapacitySource.Process)
                {
                    FixtureCount = fixtureCount,
                    WeightPerFixture = weightPerFixture
                };
            }

            return null;
        }

        /// <summary>
        /// Calculates the number of fixtures based on quantity.
        /// </summary>
        /// <param name="orderQuantity"></param>
        /// <param name="loadCapacity"></param>
        /// <param name="variancePercentage"></param>
        /// <returns></returns>
        public static int FixturesFromQuantity(int orderQuantity, int loadCapacity, decimal variancePercentage)
        {
            if (variancePercentage < 0M)
            {
                const string errorMsg = "Variance Percentage cannot be negative.";
                throw new ArgumentOutOfRangeException(nameof(variancePercentage), errorMsg);
            }
            else if (loadCapacity <= 0)
            {
                const string errorMsg = "Load capacity cannot be 0 or negative.";
                throw new ArgumentOutOfRangeException(nameof(loadCapacity), errorMsg);
            }
            else if (orderQuantity < 0)
            {
                const string errorMsg = "Order quantity cannot be negative.";
                throw new ArgumentOutOfRangeException(nameof(orderQuantity), errorMsg);
            }
            else if (loadCapacity >= orderQuantity)
            {
                return 1;
            }

            return GetNumberOfFixtures(orderQuantity, loadCapacity, variancePercentage);
        }

        /// <summary>
        /// Calculates the number of fixtures based on weight.
        /// </summary>
        /// <param name="orderWeight"></param>
        /// <param name="loadCapacity"></param>
        /// <param name="variancePercentage"></param>
        /// <returns></returns>
        public static int FixturesFromWeight(decimal orderWeight, decimal loadCapacity, decimal variancePercentage)
        {
            if (variancePercentage < 0M)
            {
                const string errorMsg = "Variance Percentage cannot be negative.";
                throw new ArgumentOutOfRangeException(nameof(variancePercentage), errorMsg);
            }
            else if (loadCapacity <= 0)
            {
                const string errorMsg = "Load capacity cannot be 0 or negative.";
                throw new ArgumentOutOfRangeException(nameof(loadCapacity), errorMsg);
            }
            else if (orderWeight < 0)
            {
                const string errorMsg = "Order weight cannot be negative.";
                throw new ArgumentOutOfRangeException(nameof(orderWeight), errorMsg);
            }
            else if (loadCapacity >= orderWeight)
            {
                return 1;
            }

            return GetNumberOfFixtures(orderWeight, loadCapacity, variancePercentage);
        }

        private static int GetNumberOfFixtures(decimal totalCapacity, decimal loadCapacity, decimal variancePercentage)
        {
            decimal maxLoadCapacity = loadCapacity * (1M + variancePercentage);

            var fixtureCount = totalCapacity / maxLoadCapacity;
            return (int)Math.Ceiling(fixtureCount);
        }

        #endregion

        #region CapacitySource

        /// <summary>
        /// Represents a source of capacity information.
        /// </summary>
        public enum CapacitySource
        {
            /// <summary>
            /// Capacity defined on order process
            /// </summary>
            OrderProcess,

            /// <summary>
            /// Capacity defined on part process
            /// </summary>
            PartProcess,

            /// <summary>
            /// Default capacity defined on process.
            /// </summary>
            Process
        }
        #endregion
    }
}
