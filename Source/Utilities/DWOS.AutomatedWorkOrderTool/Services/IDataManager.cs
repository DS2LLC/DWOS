using System.Collections.Generic;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.Data.Datasets;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    /// <summary>
    /// Defines functionality for managing database data.
    /// </summary>
    public interface IDataManager
    {
        /// <summary>
        /// Loads data that gets used for the main screen.
        /// </summary>
        /// <param name="dataSet"></param>
        void LoadInitialData(AwotDataSet dataSet);

        /// <summary>
        /// Loads data that gets used for the OSP Format screens.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="customerId"></param>
        /// <param name="ospCodeId"></param>
        void LoadInitialData(AwotDataSet dataSet, int customerId, int ospCodeId);

        /// <summary>
        /// Loads data that gets used during master list import.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="customerId"></param>
        void LoadOspFormatData(AwotDataSet dataSet, int customerId);

        /// <summary>
        /// Loads manufacturer data without clearing data from the table.
        /// </summary>
        /// <param name="dtManufacturer"></param>
        void LoadManufacturers(AwotDataSet.d_ManufacturerDataTable dtManufacturer);

        /// <summary>
        /// Loads processes and their aliases for a department.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="department"></param>
        void LoadProcesses(AwotDataSet dataSet, string department);

        /// <summary>
        /// Loads default prices for a customer without clearing data from the table.
        /// </summary>
        /// <param name="dtCustomerDefaultPrice"></param>
        /// <param name="customerId"></param>
        void LoadCustomerDefaultPrices(AwotDataSet.CustomerDefaultPriceDataTable dtCustomerDefaultPrice, int customerId);

        /// <summary>
        /// Saves all OSP Format changes made in the dataset.
        /// </summary>
        /// <param name="dataSet"></param>
        void SaveOspData(AwotDataSet dataSet);

        void LoadPartData(PartsDataset dsParts, int customerId);

        void SavePartData(PartsDataset dataSet);

        int? GetLoadCapacityQuantity(int processId);

        decimal? GetLoadCapacityWeight(int processId);

        void LoadCustomFields(AwotDataSet.CustomFieldDataTable dtCustomField, int customerId);

        void LoadOrderData(OrdersDataSet dataSet, int customerId);

        void SaveOrderData(OrdersDataSet dataSet);

        IEnumerable<ProcessRequisite> GetProcessRequisites(int processId);

        PartMark FromPart(int partId);

        int GetOrderCount(int partId);

        OrdersDataSet.OrderRow GetMatchingWorkOrder(string customerWorkOrder, int customerId);

        AwotDataSet.OSPFormatRow NewOspFormat(int matchingCustomerId, string mfgName);
    }
}
