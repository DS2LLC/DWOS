using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.Data.Datasets;
using NLog;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    internal class DataManager : IDataManager
    {
        #region Fields

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        private static PartMark RetrieveFromPart(int partId)
        {
            PartsDataset.Part_PartMarkingRow markFromPart;
            using (var taPartMark = new Data.Datasets.PartsDatasetTableAdapters.Part_PartMarkingTableAdapter())
            {
                markFromPart = taPartMark.GetDataByPartID(partId).FirstOrDefault();
            }

            if (markFromPart == null)
            {
                return null;
            }

            var newPartMark = new PartMark();

            if (!markFromPart.IsProcessSpecNull())
            {
                newPartMark.ProcessSpec = markFromPart.ProcessSpec;
            }

            if (!markFromPart.IsDef1Null())
            {
                newPartMark.Line1 = markFromPart.Def1;
            }

            if (!markFromPart.IsDef2Null())
            {
                newPartMark.Line2 = markFromPart.Def2;
            }

            if (!markFromPart.IsDef3Null())
            {
                newPartMark.Line3 = markFromPart.Def3;
            }

            if (!markFromPart.IsDef4Null())
            {
                newPartMark.Line4 = markFromPart.Def4;
            }

            return newPartMark;
        }

        private static PartMark RetrieveFromAirframe(int partId)
        {
            OrderProcessingDataSet.PartMarkingRow partMarkingRow;

            using(var taPartMarking = new Data.Datasets.OrderProcessingDataSetTableAdapters.PartMarkingTableAdapter())
            {
                partMarkingRow = taPartMarking.GetDataByPart(partId).FirstOrDefault();
            }

            if(partMarkingRow == null)
            {
                return null;
            }

            var newPartMark = new PartMark {ProcessSpec = partMarkingRow.ProcessSpec};

            if (!partMarkingRow.IsDef1Null())
            {
                newPartMark.Line1 = partMarkingRow.Def1;
            }

            if (!partMarkingRow.IsDef2Null())
            {
                newPartMark.Line2 = partMarkingRow.Def2;
            }

            if (!partMarkingRow.IsDef3Null())
            {
                newPartMark.Line3 = partMarkingRow.Def3;
            }

            if (!partMarkingRow.IsDef4Null())
            {
                newPartMark.Line4 = partMarkingRow.Def4;
            }

            return newPartMark;
        }

        #endregion

        #region IDataManager Members

        public void LoadInitialData(AwotDataSet dataSet)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }

            dataSet.Clear();

            using (var taCustomer = new Model.AwotDataSetTableAdapters.CustomerTableAdapter())
            {
                taCustomer.Fill(dataSet.Customer);
            }

            using (var taDepartment = new Model.AwotDataSetTableAdapters.d_DepartmentTableAdapter())
            {
                taDepartment.Fill(dataSet.d_Department);
            }

            using (var taManufacturer = new Model.AwotDataSetTableAdapters.d_ManufacturerTableAdapter())
            {
                taManufacturer.Fill(dataSet.d_Manufacturer);
            }

            using (var taOspFormat = new Model.AwotDataSetTableAdapters.OSPFormatTableAdapter())
            {
                taOspFormat.Fill(dataSet.OSPFormat);
            }
        }

        public void LoadInitialData(AwotDataSet dataSet, int customerId, int ospCodeId)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }

            dataSet.Clear();

            using (var taCustomer = new Model.AwotDataSetTableAdapters.CustomerTableAdapter())
            {
                taCustomer.FillByCustomer(dataSet.Customer, customerId);
            }

            using (var taDepartment = new Model.AwotDataSetTableAdapters.d_DepartmentTableAdapter())
            {
                taDepartment.Fill(dataSet.d_Department);
            }

            using (var taManufacturer = new Model.AwotDataSetTableAdapters.d_ManufacturerTableAdapter())
            {
                taManufacturer.Fill(dataSet.d_Manufacturer);
            }

            using (var taOspFormat = new Model.AwotDataSetTableAdapters.OSPFormatTableAdapter())
            {
                taOspFormat.FillByOspFormat(dataSet.OSPFormat, ospCodeId);
            }

            using (var taOspFormatSection = new Model.AwotDataSetTableAdapters.OSPFormatSectionTableAdapter())
            {
                taOspFormatSection.FillByOspFormat(dataSet.OSPFormatSection, ospCodeId);
            }

            using (var taSectionProcess = new Model.AwotDataSetTableAdapters.OSPFormatSectionProcessTableAdapter())
            {
                taSectionProcess.FillByOspFormat(dataSet.OSPFormatSectionProcess, ospCodeId);
            }

            using (var taSectionPartMark = new Model.AwotDataSetTableAdapters.OSPFormatSectionPartMarkTableAdapter())
            {
                taSectionPartMark.FillByOspFormat(dataSet.OSPFormatSectionPartMark, ospCodeId);
            }

            using (var taProcess = new Model.AwotDataSetTableAdapters.ProcessTableAdapter())
            {
                taProcess.Fill(dataSet.Process);
            }

            using (var taProcessAlias = new Model.AwotDataSetTableAdapters.ProcessAliasTableAdapter())
            {
                taProcessAlias.Fill(dataSet.ProcessAlias);
            }
        }

        public void LoadOspFormatData(AwotDataSet dataSet, int customerId)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }

            dataSet.Clear();

            using (var taCustomer = new Model.AwotDataSetTableAdapters.CustomerTableAdapter())
            {
                taCustomer.FillByCustomer(dataSet.Customer, customerId);
            }

            using (var taDepartment = new Model.AwotDataSetTableAdapters.d_DepartmentTableAdapter())
            {
                taDepartment.Fill(dataSet.d_Department);
            }

            using (var taManufacturer = new Model.AwotDataSetTableAdapters.d_ManufacturerTableAdapter())
            {
                taManufacturer.Fill(dataSet.d_Manufacturer);
            }

            using (var taOspFormat = new Model.AwotDataSetTableAdapters.OSPFormatTableAdapter())
            {
                taOspFormat.FillByCustomer(dataSet.OSPFormat, customerId);
            }

            foreach (var ospCodeId in dataSet.OSPFormat.Select(f => f.OSPFormatID))
            {
                using (var taOspFormatSection = new Model.AwotDataSetTableAdapters.OSPFormatSectionTableAdapter { ClearBeforeFill = false })
                {
                    taOspFormatSection.FillByOspFormat(dataSet.OSPFormatSection, ospCodeId);
                }

                using (var taSectionProcess = new Model.AwotDataSetTableAdapters.OSPFormatSectionProcessTableAdapter { ClearBeforeFill = false })
                {
                    taSectionProcess.FillByOspFormat(dataSet.OSPFormatSectionProcess, ospCodeId);
                }

                using (var taSectionPartMark = new Model.AwotDataSetTableAdapters.OSPFormatSectionPartMarkTableAdapter { ClearBeforeFill = false })
                {
                    taSectionPartMark.FillByOspFormat(dataSet.OSPFormatSectionPartMark, ospCodeId);
                }
            }

            using (var taProcess = new Model.AwotDataSetTableAdapters.ProcessTableAdapter())
            {
                taProcess.Fill(dataSet.Process);
            }

            using (var taProcessAlias = new Model.AwotDataSetTableAdapters.ProcessAliasTableAdapter())
            {
                taProcessAlias.Fill(dataSet.ProcessAlias);
            }
        }

        public void LoadManufacturers(AwotDataSet.d_ManufacturerDataTable dtManufacturer)
        {
            if (dtManufacturer == null)
            {
                throw new ArgumentNullException(nameof(dtManufacturer));
            }

            using (var taManufactuer = new Model.AwotDataSetTableAdapters.d_ManufacturerTableAdapter { ClearBeforeFill = false })
            {
                taManufactuer.Fill(dtManufacturer);
            }
        }

        public void LoadProcesses(AwotDataSet dataSet, string department)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }

            dataSet.Clear();

            using (var taProcess = new Model.AwotDataSetTableAdapters.ProcessTableAdapter())
            {
                taProcess.FillApprovedByDepartment(dataSet.Process, department ?? string.Empty);
            }

            using (var taProcessAlias = new Model.AwotDataSetTableAdapters.ProcessAliasTableAdapter { ClearBeforeFill = false })
            {
                foreach (var processId in dataSet.Process.Select(p => p.ProcessID))
                {
                    taProcessAlias.FillByProcess(dataSet.ProcessAlias, processId);
                }
            }
        }

        public void LoadCustomerDefaultPrices(AwotDataSet.CustomerDefaultPriceDataTable dtCustomerDefaultPrice, int customerId)
        {
            if (dtCustomerDefaultPrice == null)
            {
                throw new ArgumentNullException(nameof(dtCustomerDefaultPrice));
            }

            using (var taCustomerDefaultPrice = new Model.AwotDataSetTableAdapters.CustomerDefaultPriceTableAdapter())
            {
                taCustomerDefaultPrice.FillByCustomer(dtCustomerDefaultPrice, customerId);
            }
        }

        public void SaveOspData(AwotDataSet dataSet)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }

            try
            {
                using (var taManager = new Model.AwotDataSetTableAdapters.TableAdapterManager())
                {
                    taManager.OSPFormatTableAdapter = new Model.AwotDataSetTableAdapters.OSPFormatTableAdapter();
                    taManager.OSPFormatSectionTableAdapter = new Model.AwotDataSetTableAdapters.OSPFormatSectionTableAdapter();
                    taManager.OSPFormatSectionProcessTableAdapter = new Model.AwotDataSetTableAdapters.OSPFormatSectionProcessTableAdapter();
                    taManager.OSPFormatSectionPartMarkTableAdapter = new Model.AwotDataSetTableAdapters.OSPFormatSectionPartMarkTableAdapter();
                    taManager.CustomerDefaultPriceTableAdapter = new Model.AwotDataSetTableAdapters.CustomerDefaultPriceTableAdapter();

                    taManager.UpdateAll(dataSet);
                }
            }
            catch (Exception)
            {
                // Log data errors
                try
                {
                    _logger.Error($"DataSet errors: {dataSet.GetDataErrors()}");
                }
                catch (Exception loggingError)
                {
                    _logger.Error(loggingError, "Error while logging dataset errors.");
                }

                throw;
            }
        }

        public void LoadPartData(PartsDataset dsParts, int customerId)
        {
            if (dsParts == null)
            {
                throw new ArgumentNullException(nameof(dsParts));
            }

            dsParts.Clear();

            // Load parts & processes for the customer
            using (var taPart = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
            {
                taPart.FillByCustomer(dsParts.Part, customerId, true);
            }

            Data.Datasets.PartsDatasetTableAdapters.PartProcessTableAdapter taProcess = null;
            Data.Datasets.PartsDatasetTableAdapters.PartProcessVolumePriceTableAdapter taProcessVolume = null;
            Data.Datasets.PartsDatasetTableAdapters.Part_PartMarkingTableAdapter taPartMarking = null;
            Data.Datasets.PartsDatasetTableAdapters.Part_DocumentLinkTableAdapter taDocumentLink = null;

            try
            {
                taProcess =
                    new Data.Datasets.PartsDatasetTableAdapters.PartProcessTableAdapter
                    {
                        ClearBeforeFill = false
                    };

                taProcessVolume =
                    new Data.Datasets.PartsDatasetTableAdapters.PartProcessVolumePriceTableAdapter
                    {
                        ClearBeforeFill = false
                    };

                taPartMarking =
                    new Data.Datasets.PartsDatasetTableAdapters.Part_PartMarkingTableAdapter
                    {
                        ClearBeforeFill = false
                    };

                taDocumentLink =
                    new Data.Datasets.PartsDatasetTableAdapters.Part_DocumentLinkTableAdapter
                    {
                        ClearBeforeFill = false
                    };

                foreach (var partRow in dsParts.Part)
                {
                    taProcess.FillByPart(dsParts.PartProcess, partRow.PartID);
                    taProcessVolume.FillByPartID(dsParts.PartProcessVolumePrice, partRow.PartID);
                    taPartMarking.FillByPartID(dsParts.Part_PartMarking, partRow.PartID);
                    taDocumentLink.FillByPartID(dsParts.Part_DocumentLink, partRow.PartID);
                }
            }
            finally
            {
                taProcess?.Dispose();
                taProcessVolume?.Dispose();
                taPartMarking?.Dispose();
            }

            // Load airframes - should be added when creating parts w/ new airframe
            using (var taAirframe = new Data.Datasets.PartsDatasetTableAdapters.d_AirframeTableAdapter())
            {
                taAirframe.Fill(dsParts.d_Airframe);
            }

        }

        public void SavePartData(PartsDataset dataSet)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }

            try
            {
                var newParts = dataSet.Part
                    .Where(part => part.RowState == DataRowState.Added)
                    .ToList();

                var existingParts = dataSet.Part
                    .Where(part => part.RowState == DataRowState.Modified)
                    .ToList();

                // Save data
                using (var taManager = new Data.Datasets.PartsDatasetTableAdapters.TableAdapterManager())
                {
                    taManager.PartTableAdapter = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter();
                    taManager.PartProcessTableAdapter = new Data.Datasets.PartsDatasetTableAdapters.PartProcessTableAdapter();
                    taManager.Part_PartMarkingTableAdapter = new Data.Datasets.PartsDatasetTableAdapters.Part_PartMarkingTableAdapter();
                    taManager.PartProcessVolumePriceTableAdapter = new Data.Datasets.PartsDatasetTableAdapters.PartProcessVolumePriceTableAdapter();
                    taManager.d_AirframeTableAdapter = new Data.Datasets.PartsDatasetTableAdapters.d_AirframeTableAdapter();
                    taManager.Part_DocumentLinkTableAdapter = new Data.Datasets.PartsDatasetTableAdapters.Part_DocumentLinkTableAdapter();

                    taManager.UpdateAll(dataSet);
                }

                // Update part history
                using (var taHistory = new Data.Datasets.OrderHistoryDataSetTableAdapters.PartHistoryTableAdapter())
                {
                    foreach (var newPart in newParts)
                    {
                        taHistory.UpdatePartHistory(newPart.PartID,
                            "AWOT",
                            "Imported new part from master list.",
                            "Server");
                    }

                    foreach (var existingPart in existingParts)
                    {
                        taHistory.UpdatePartHistory(existingPart.PartID,
                            "AWOT",
                            "Updated part from master list.",
                            "Server");
                    }
                }
            }
            catch (Exception)
            {
                // Log data errors
                try
                {
                    _logger.Error($"DataSet errors: {dataSet.GetDataErrors()}");
                }
                catch (Exception loggingError)
                {
                    _logger.Error(loggingError, "Error while logging dataset errors.");
                }

                throw;
            }
        }

        public int? GetLoadCapacityQuantity(int processId)
        {
            const int maxQuantity = 999999999;

            decimal? loadCapacityDecimal;

            using (var taProcess = new Model.AwotDataSetTableAdapters.ProcessTableAdapter())
            {
                loadCapacityDecimal = taProcess.GetLoadCapacityQuantity(processId);
            }

            if (loadCapacityDecimal.HasValue)
            {
                var roundedCapacity = Math.Round(loadCapacityDecimal.Value, MidpointRounding.AwayFromZero);
                var loadCapacity = Math.Min(Convert.ToInt32(roundedCapacity), maxQuantity);
                return loadCapacity;
            }

            return null;
        }

        public decimal? GetLoadCapacityWeight(int processId)
        {
            const decimal maxWeight = 999999.99999999M;

            decimal? loadCapacityDecimal;

            using (var taProcess = new Model.AwotDataSetTableAdapters.ProcessTableAdapter())
            {
                loadCapacityDecimal = taProcess.GetLoadCapacityWeight(processId);
            }

            if (loadCapacityDecimal.HasValue)
            {
                return Math.Min(loadCapacityDecimal.Value, maxWeight);
            }

            return null;
        }

        public void LoadCustomFields(AwotDataSet.CustomFieldDataTable dtCustomField, int customerId)
        {
            if (dtCustomField == null)
            {
                throw new ArgumentNullException(nameof(dtCustomField));
            }

            using (var taCustomField = new Model.AwotDataSetTableAdapters.CustomFieldTableAdapter())
            {
                taCustomField.FillByCustomer(dtCustomField, customerId);
            }
        }

        public void LoadOrderData(OrdersDataSet dataSet, int customerId)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }

            // General data
            using (var taDepartments = new Data.Datasets.OrdersDataSetTableAdapters.d_DepartmentTableAdapter())
            {
                taDepartments.Fill(dataSet.d_Department);
            }

            using (var taOrderStatus = new Data.Datasets.OrdersDataSetTableAdapters.d_OrderStatusTableAdapter())
            {
                taOrderStatus.Fill(dataSet.d_OrderStatus);
            }

            using (var taPriceUnit = new Data.Datasets.OrdersDataSetTableAdapters.PriceUnitTableAdapter())
            {
                taPriceUnit.Fill(dataSet.PriceUnit);
            }

            using (var taPriority = new Data.Datasets.OrdersDataSetTableAdapters.d_PriorityTableAdapter())
            {
                taPriority.Fill(dataSet.d_Priority);
            }

            using (var taWorkStatus = new Data.Datasets.OrdersDataSetTableAdapters.d_WorkStatusTableAdapter())
            {
                taWorkStatus.Fill(dataSet.d_WorkStatus);
            }

            using (var taUser = new Data.Datasets.OrdersDataSetTableAdapters.UserSummaryTableAdapter())
            {
                taUser.Fill(dataSet.UserSummary);
            }

            // Customer-Specific data
            using (var taCustomer = new Data.Datasets.OrdersDataSetTableAdapters.CustomerSummaryTableAdapter())
            {
                taCustomer.FillByCustomerID(dataSet.CustomerSummary, customerId);
            }

            using (var taCustomerAddress = new Data.Datasets.OrdersDataSetTableAdapters.CustomerAddressTableAdapter())
            {
                taCustomerAddress.FillByCustomer(dataSet.CustomerAddress, customerId);
            }

            using (var taCustomerShipping = new Data.Datasets.OrdersDataSetTableAdapters.CustomerShippingSummaryTableAdapter())
            {
                taCustomerShipping.FillByCustomer(dataSet.CustomerShippingSummary, customerId);
            }

            using (var taParts = new Data.Datasets.OrdersDataSetTableAdapters.PartSummaryTableAdapter())
            {
                taParts.FillByCustomer(dataSet.PartSummary, customerId);
            }

            using (var taPartProcess = new Data.Datasets.OrdersDataSetTableAdapters.PartProcessSummaryTableAdapter { ClearBeforeFill = false })
            {
                using (var taPartProcessPrice = new Data.Datasets.OrdersDataSetTableAdapters.PartProcessPriceSummaryTableAdapter { ClearBeforeFill = false })
                {
                    foreach (var part in dataSet.PartSummary)
                    {
                        taPartProcess.FillByPart(dataSet.PartProcessSummary, part.PartID);
                        taPartProcessPrice.FillByPart(dataSet.PartProcessPriceSummary, part.PartID);
                    }
                }
            }
        }

        public void SaveOrderData(OrdersDataSet dataSet)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }

            try
            {
                using (var taManager = new Data.Datasets.OrdersDataSetTableAdapters.TableAdapterManager())
                {
                    taManager.OrderTableAdapter = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter();
                    taManager.OrderCustomFieldsTableAdapter = new Data.Datasets.OrdersDataSetTableAdapters.OrderCustomFieldsTableAdapter();
                    taManager.OrderProductClassTableAdapter = new Data.Datasets.OrdersDataSetTableAdapters.OrderProductClassTableAdapter();
                    taManager.OrderProcessesTableAdapter = new Data.Datasets.OrdersDataSetTableAdapters.OrderProcessesTableAdapter();
                    taManager.OrderProcessAnswerTableAdapter = new Data.Datasets.OrdersDataSetTableAdapters.OrderProcessAnswerTableAdapter();
                    taManager.OrderPartMarkTableAdapter = new Data.Datasets.OrdersDataSetTableAdapters.OrderPartMarkTableAdapter();
                    taManager.Order_DocumentLinkTableAdapter = new Data.Datasets.OrdersDataSetTableAdapters.Order_DocumentLinkTableAdapter();

                    taManager.UpdateAll(dataSet);
                }
            }
            catch (Exception)
            {
                // Log data errors
                try
                {
                    _logger.Error($"DataSet errors: {dataSet.GetDataErrors()}");
                }
                catch (Exception loggingError)
                {
                    _logger.Error(loggingError, "Error while logging dataset errors.");
                }

                throw;
            }
        }

        public IEnumerable<ProcessRequisite> GetProcessRequisites(int processId)
        {
            using (var dtProcessReq = new ProcessesDataset.ProcessRequisiteDataTable())
            {
                using (var taProcessRequisite = new Data.Datasets.ProcessesDatasetTableAdapters.ProcessRequisiteTableAdapter())
                {
                    taProcessRequisite.FillByParent(dtProcessReq, processId);
                }

                return dtProcessReq.Select(req => new ProcessRequisite
                {
                    ParentProcessId = req.ParentProcessID,
                    ChildProcessId = req.ChildProcessID,
                    Hours = req.Hours
                }).ToList();
            }
        }

        public PartMark FromPart(int partId)
        {
            return RetrieveFromPart(partId) ?? RetrieveFromAirframe(partId);
        }

        public int GetOrderCount(int partId)
        {
            using (var taOrder = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter())
            {
                return taOrder.GetCountByPart(partId) ?? 0;
            }
        }

        public OrdersDataSet.OrderRow GetMatchingWorkOrder(string customerWorkOrder, int customerId)
        {
            if (string.IsNullOrEmpty(customerWorkOrder))
            {
                return null;
            }

            using (var taOrder = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter())
            {
                return taOrder.GetByCustomerWorkOrder(customerId, customerWorkOrder).FirstOrDefault();
            }
        }

        public AwotDataSet.OSPFormatRow NewOspFormat(int matchingCustomerId, string mfgName)
        {
            using (var dtOspFormat = new AwotDataSet.OSPFormatDataTable())
            {
                var newRow = dtOspFormat.NewOSPFormatRow();
                newRow.CustomerID = matchingCustomerId;
                newRow.ManufacturerID = mfgName;
                newRow.Code = "00";
                dtOspFormat.AddOSPFormatRow(newRow);

                using (var taOspFormat = new Model.AwotDataSetTableAdapters.OSPFormatTableAdapter())
                {
                    taOspFormat.Update(dtOspFormat);
                }

                return newRow;
            }
        }

        #endregion
    }
}
