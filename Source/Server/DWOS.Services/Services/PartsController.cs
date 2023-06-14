using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Services.Messages;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace DWOS.Services
{
    public class PartsController : ApiController
    {
        #region Methods

        [HttpGet]
        [ServiceExceptionFilter("Error getting part.")]
        public ResponseBase Get(int orderId)
        {
            return new PartDetailResponse { Success = true, ErrorMessage = null, PartDetail = CreatePartDetail(orderId) };
        }

        #endregion

        #region Factories

        private static PartDetailInfo CreatePartDetail(int orderId)
        {
            var part = new PartDetailInfo();
           
            try
            {
                Data.Reports.OrdersReport.PartSummaryRow partRow;

                using (var taOrders = new Data.Reports.OrdersReportTableAdapters.PartSummaryTableAdapter())
                {
                    partRow = taOrders.GetByOrderId(orderId).FirstOrDefault();
                }

                if (partRow == null)
                    return part;

                part.PartId = partRow.PartID;
                part.Name = partRow.Name;
                part.CustomerName = partRow.CustomerName;
                part.Rev = partRow.IsRevisionNull() ? null : partRow.Revision;
                part.Manufacturer = partRow.IsManufacturerIDNull() ? null : partRow.ManufacturerID;
                part.Model = partRow.IsAirframeNull() ? null : partRow.Airframe;
                part.Notes = partRow.IsNotesNull() ? null : partRow.Notes;
                part.Material = partRow.IsMaterialNull() ? null : partRow.Material;

                // Construct string representation of part dimensions.
                var dimensions = string.Empty;
                if(partRow.Length > 0)
                    dimensions += (partRow.Length + "L X ");
                if (partRow.Width > 0)
                    dimensions += (partRow.Width + "W X ");
                if (partRow.Height > 0)
                    dimensions += (partRow.Height + "H");

                dimensions = dimensions.RemoveFromEnd("X ");

                part.Dimensions = dimensions;

                // Retrieve documents.
                using (var dtPartDocument = new PartsDataset.Part_DocumentLinkDataTable())
                {
                    using (var taPartDocument = new Data.Datasets.PartsDatasetTableAdapters.Part_DocumentLinkTableAdapter())
                    {
                        taPartDocument.FillByPartID(dtPartDocument, partRow.PartID);
                    }

                    if (dtPartDocument.Count > 0)
                    {
                        part.Documents = new List<DocumentInfo>();

                        using (var dtDocument = new DocumentsDataSet.DocumentInfoDataTable())
                        {
                            using (var taDocument = new Data.Datasets.DocumentsDataSetTableAdapters.DocumentInfoTableAdapter())
                            {
                                foreach (var partDocRow in dtPartDocument)
                                {
                                    taDocument.FillByID(dtDocument, partDocRow.DocumentInfoID);
                                    var docRow = dtDocument.FindByDocumentInfoID(partDocRow.DocumentInfoID);

                                    var currentRevisionID = taDocument.GetCurrentRevisionID(partDocRow.DocumentInfoID)
                                        as int?;

                                    if (currentRevisionID.HasValue)
                                    {
                                        part.Documents.Add(new DocumentInfo()
                                        {
                                            CurrentRevisionId = currentRevisionID.Value,
                                            DocumentInfoID = docRow.DocumentInfoID,
                                            MediaType = docRow.MediaType,
                                            Name = docRow.Name,
                                            DocumentType = "Part"
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                // Retrieve media.
                using (var dtMedia = new PartsDataset.MediaDataTable())
                {
                    using (var taMedia = new Data.Datasets.PartsDatasetTableAdapters.MediaTableAdapter())
                    {
                        taMedia.FillByPartIDWithoutMedia(dtMedia, partRow.PartID);
                    }

                    if (dtMedia.Count > 0)
                    {
                        part.Media = new List<MediaSummary>();

                        foreach (var mediaRow in dtMedia)
                        {
                            part.Media.Add(new MediaSummary()
                            {
                                MediaId = mediaRow.MediaID,
                                Name = mediaRow.Name,
                                FileExtension = mediaRow.IsFileExtensionNull() ? string.Empty : mediaRow.FileExtension
                            });
                        }
                    }
                }
                // Retrieve custom fields
                part.CustomFields = new List<PartCustomField>();
                using (var dtPartLevelCustomField = new PartsDataset.PartLevelCustomFieldDataTable())
                {
                    using (var taPartLevelCustomField = new Data.Datasets.PartsDatasetTableAdapters.PartLevelCustomFieldTableAdapter())
                    {
                        taPartLevelCustomField.FillByPartID(dtPartLevelCustomField, partRow.PartID);
                    }

                    using (var dtPartCustomFields = new PartsDataset.PartCustomFieldsDataTable())
                    {
                        using (var taPartCustomFields = new Data.Datasets.PartsDatasetTableAdapters.PartCustomFieldsTableAdapter())
                        {
                            taPartCustomFields.FillByPartID(dtPartCustomFields, partRow.PartID);
                        }

                        foreach (var partCustomField in dtPartCustomFields)
                        {
                            var field = dtPartLevelCustomField.FindByPartLevelCustomFieldID(partCustomField.PartLevelCustomFieldID);

                            if (field == null || !field.IsVisible)
                            {
                                continue;
                            }

                            part.CustomFields.Add(new PartCustomField
                            {
                                Name = field.Name,
                                Value = partCustomField.IsValueNull() ? string.Empty : partCustomField.Value
                            });
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting part info.");
            }

            return part;
        }

        #endregion
    }
}