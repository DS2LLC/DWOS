using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets;

namespace DWOS.Data
{
    /// <summary>
    /// Defines utility methods related to report fields and the
    /// <see cref="ReportToken"/> class.
    /// </summary>
    public static class ReportFieldMapper
    {
        #region Methods

        /// <summary>
        /// Gets the name of the report.
        /// </summary>
        /// <param name="reportType">Type of the report.</param>
        /// <returns></returns>
        public static string GetReportName(enumReportType reportType)
        {
            string reportName = String.Empty;

            switch (reportType)
            {
                case enumReportType.PackingSlip:
                    reportName = "Packing Slip";
                    break;
                default:
                    reportName = "Unknown";
                    break;
            }

            return reportName;
        }

        /// <summary>
        /// Gets a display name for the report token
        /// </summary>
        /// <param name="reportToken">Type of report token</param>
        /// <returns>Display name string</returns>
        public static string GetDisplayName(enumReportTokens reportToken)
        {
            switch (reportToken)
            {
                case enumReportTokens.OrderID:
                    return "Work Order";
                case enumReportTokens.CustomerWO:
                    return "Customer WO";
                case enumReportTokens.PurchaseOrder:
                    return "Purchase Order";
                case enumReportTokens.PartID:
                    return "Part";
                case enumReportTokens.PartQuantity:
                    return "Quantity";
                case enumReportTokens.Weight:
                    return "Weight (lbs.)";
                case enumReportTokens.SalesOrder:
                    return "Sales Order";
                case enumReportTokens.OrderPrice:
                    return "Order Price";
                case enumReportTokens.ContainerCount:
                    return "Container Count";
                case enumReportTokens.GrossWeight:
                    return "Gross Weight (lbs.)";
                case enumReportTokens.PartDescription:
                    return "Description";
                case enumReportTokens.ProcessAlias:
                    return "Processes";
                case enumReportTokens.SerialNumber:
                    return "Serial Number";
                case enumReportTokens.OrderWeight:
                    return "Order Weight (lbs.)";
                case enumReportTokens.ContainerDescription:
                    return "Containers";
                default:
                    return reportToken.ToString();
            }
        }

        /// <summary>
        /// Gets a field name from the given display name; opposite of
        /// <see cref="GetDisplayName(enumReportTokens)"/>.
        /// </summary>
        /// <remarks>
        /// Report editor dialogs show a list of fields by their default
        /// display names. This method takes that display name and returns
        /// the field's actual name.
        /// </remarks>
        /// <param name="reportTokenDisplayName">The report token's display name</param>
        /// <returns>Field name string</returns>
        public static string GetReportTokenFieldName(string reportTokenDisplayName)
        {
            string fieldName = string.Empty;
            switch (reportTokenDisplayName)
            {
                case "Work Order":
                    fieldName =  enumReportTokens.OrderID.ToString();
                    break;
                case "Part":
                    fieldName = enumReportTokens.PartID.ToString();
                    break;
                case "Weight (lbs.)":
                    fieldName = enumReportTokens.Weight.ToString();
                    break;
                case "Quantity":
                    fieldName = enumReportTokens.PartQuantity.ToString();
                    break;
                case "Gross Weight (lbs.)":
                    fieldName = enumReportTokens.GrossWeight.ToString();
                    break;
                case "Order Weight (lbs.)":
                    fieldName = enumReportTokens.OrderWeight.ToString();
                    break;
                case "Description":
                    fieldName = enumReportTokens.PartDescription.ToString();
                    break;
                case "Processes":
                    fieldName = enumReportTokens.ProcessAlias.ToString();
                    break;
                case "Containers":
                    fieldName = enumReportTokens.ContainerDescription.ToString();
                    break;
                default:
                    if (!string.IsNullOrEmpty(reportTokenDisplayName))
                    {
                        fieldName = reportTokenDisplayName.Replace(" ", string.Empty);
                    }

                    break;
            }

            return fieldName;
        }

        /// <summary>
        /// Gets the default report width for the given report field.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static int DefaultWidth(string fieldName)
        {
            const int defaultWidth = 100;

            enumReportTokens reportToken;
            if (Enum.TryParse(fieldName, out reportToken))
            {
                switch (reportToken)
                {
                    case enumReportTokens.ProcessAlias:
                        return defaultWidth + 25;
                    default:
                        return defaultWidth;
                }
            }
            else
            {
                return defaultWidth;
            }
        }

        /// <summary>
        /// Gets default report tokens for a report type and customer.
        /// </summary>
        /// <param name="reportType">Type of the report.</param>
        /// <param name="customerID">The customer identifier.</param>
        /// <returns></returns>
        public static List<ReportToken> GetReportTokens(enumReportType reportType, int customerID = -1)
        {
            var tokens = new List<ReportToken>();

            switch (reportType)
            {
                case enumReportType.PackingSlip:
                    tokens.Add(new ReportToken(enumReportTokens.OrderID) {DisplayOrder = 1 });
                    tokens.Add(new ReportToken(enumReportTokens.CustomerWO) {DisplayOrder = 2 });
                    tokens.Add(new ReportToken(enumReportTokens.PurchaseOrder) {DisplayOrder = 3 });
                    tokens.Add(new ReportToken(enumReportTokens.PartID) {DisplayOrder = 4});
                    tokens.Add(new ReportToken(enumReportTokens.PartQuantity) {DisplayOrder = 5});
                    tokens.Add(new ReportToken(enumReportTokens.Weight) {DisplayOrder = 6});
                    break;
                default:
                    break;
            }


            if (reportType != enumReportType.PackingSlip)
            {
                //If customer has custom order fields
                if (customerID > 0)
                {
                    var customFields = new DWOS.Data.Datasets.CustomersDataset.CustomFieldDataTable();
                    
                    using (var ta = new DWOS.Data.Datasets.CustomersDatasetTableAdapters.CustomFieldTableAdapter())
                        ta.FillByCustomer(customFields, customerID);

                    foreach (var customField in customFields)
                    {
                        tokens.Add(new ReportToken() {FieldName = customField.CustomFieldID.ToString(), DisplayName = customField.Name, IsCustom = true});
                    }
                }
            }
            return tokens;
        }

        /// <summary>
        /// Gets report tokens for a report type.
        /// </summary>
        /// <param name="reportType"></param>
        /// <returns></returns>
        public static List<ReportToken> GetFieldsByReport(ReportFieldMapper.enumReportType reportType)
        {
            try
            {
                var tokens = new List<ReportToken>();

                var dsReportFields = new ReportFieldsDataSet { EnforceConstraints = false };

                using (var taReport = new DWOS.Data.Datasets.ReportFieldsDataSetTableAdapters.ReportTableAdapter())
                {
                    taReport.FillByReportType(dsReportFields.Report, Convert.ToInt32(reportType));
                }

                var reportRow = dsReportFields.Report.GetDefaultReport(reportType);

                if (reportRow != null)
                {
                    using (var taReportFields = new Data.Datasets.ReportFieldsDataSetTableAdapters.ReportFieldsTableAdapter())
                    {
                        taReportFields.FillByReportID(dsReportFields.ReportFields, reportRow.ReportID);
                    }

                    foreach (var row in reportRow.GetReportFieldsRows())
                    {
                        tokens.Add(ReportToken.From(row));
                    }
                }

                return tokens;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting default fields for report");
                return GetReportTokens(reportType);
            }
        }

        /// <summary>
        /// Gets the fields to use in the report.
        /// </summary>
        /// <param name="reportType">Type of the report.</param>
        /// <param name="customerID">The customer identifier.</param>
        /// <returns></returns>
        public static List<ReportToken> GetFieldsByReport(ReportFieldMapper.enumReportType reportType, int customerID)
        {
            try
            {
                var tokens = new List<ReportToken>();

                var dsReportFields = new ReportFieldsDataSet { EnforceConstraints = false };

                //Try and get any report fields defined by customer for this report
                using (var taReport = new Data.Datasets.ReportFieldsDataSetTableAdapters.ReportTableAdapter())
                    taReport.FillByCustomerAndReportType(dsReportFields.Report, customerID, Convert.ToInt32(ReportFieldMapper.enumReportType.PackingSlip));

                DWOS.Data.Datasets.ReportFieldsDataSet.ReportRow reportRow = null;
                if(dsReportFields.Report.Any())
                {
                    reportRow = dsReportFields.Report.First();
                }
                else
                { 
                    //No report fields defined by customer for this report, so use defaults
                    using (var taReport = new DWOS.Data.Datasets.ReportFieldsDataSetTableAdapters.ReportTableAdapter())
                    {
                        taReport.FillByReportType(dsReportFields.Report, Convert.ToInt32(reportType));
                    }

                    reportRow = dsReportFields.Report.GetDefaultReport(reportType);
                }

                if (reportRow != null)
                {
                    using(var taReportFields = new Data.Datasets.ReportFieldsDataSetTableAdapters.ReportFieldsTableAdapter())
                        taReportFields.FillByReportID(dsReportFields.ReportFields, reportRow.ReportID);

                    foreach (var row in reportRow.GetReportFieldsRows())
                    {
                        tokens.Add(ReportToken.From(row));
                    }
                }

                return tokens;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting fields for report");
                return GetReportTokens(reportType);
            }
        }

        #endregion

        #region enumReportType

        /// <summary>
        /// Represents a user-configurable report type.
        /// </summary>
        public enum enumReportType { PackingSlip = 1 }

        #endregion

        #region enumReportTokens

        /// <summary>
        /// Represents a report token type.
        /// </summary>
        public enum enumReportTokens
        {
            OrderID,
            CustomerWO,
            PurchaseOrder,
            PartID,
            PartQuantity,
            Weight,
            SalesOrder,
            OrderPrice,
            ContainerCount,
            GrossWeight,
            PartDescription,
            ProcessAlias,
            SerialNumber,
            OrderWeight,
            ContainerDescription
        }

        #endregion
    }
}
