using DWOS.Data.Datasets;
using DWOS.Data.Datasets.DocumentsDataSetTableAdapters;
using DWOS.Services.Messages;
using NLog;
using System;
using System.Linq;
using System.Web.Http;

namespace DWOS.Services
{
    public class DocumentsController : ApiController
    {
        #region Methods

        [HttpGet]
        [ServiceExceptionFilter("Error getting document.")]
        public ResponseBase Get(int revisionId)
        {
            return new DocumentResponse { Success = true, ErrorMessage = null, Document = CreateDocumentDetail(revisionId) };
        }

        #endregion

        #region Factories

        private static DocumentDetail CreateDocumentDetail(int revisionId)
        {
            var detail = new DocumentDetail();

            DocumentRevisionTableAdapter taRevision = null;
            DocumentInfoTableAdapter taDocument = null;

            DocumentsDataSet.DocumentRevisionDataTable dtRevision = null;
            DocumentsDataSet.DocumentInfoDataTable dtDocumentInfo = null;

            try
            {
                taRevision = new DocumentRevisionTableAdapter();
                taDocument = new DocumentInfoTableAdapter();

                dtRevision = taRevision.GetByRevisionID(revisionId);

                if (dtRevision.Count > 0)
                {
                    var docRevision = dtRevision.First();

                    dtDocumentInfo = taDocument.GetByID(docRevision.DocumentInfoID);

                    if (dtDocumentInfo.Count > 0)
                    {
                        detail.DocumentRevisionId = docRevision.DocumentRevisionID;
                        detail.DocumentInfoID = docRevision.DocumentInfoID;
                        detail.MediaType = dtDocumentInfo.First().MediaType;
                        detail.IsCompressed = docRevision.IsCompressed;
                        detail.DocumentData = taRevision.GetMediaStream(docRevision.DocumentRevisionID);
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting document info.");
            }
            finally
            {
                taRevision?.Dispose();
                taDocument?.Dispose();

                dtRevision?.Dispose();
                dtRevision?.Dispose();
            }

            return detail;
        }


        #endregion
    }
}
