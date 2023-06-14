using System.Linq;
using DWOS.Data;
using DWOS.Services.Messages;
using System;
using System.Web.Http;

namespace DWOS.Services
{
    public class MediaController : ApiController
    {
        #region Methods

        [HttpGet]
        [ServiceExceptionFilter("Error getting media.")]
        public ResponseBase Get(int mediaId)
        {
            return new MediaDetailResponse { Success = true, ErrorMessage = null, Media = CreateMediaDetail(mediaId) };
        }

        #endregion

        #region Factories

        private static MediaInfo CreateMediaDetail(int mediaId)
        {
            var media = new MediaInfo();
           
            try
            {
                using(var taPartMedia     = new DWOS.Data.Datasets.PartsDatasetTableAdapters.MediaTableAdapter())
                using (var mediaTable = new DWOS.Data.Datasets.PartsDataset.MediaDataTable())
                {
                    taPartMedia.FillByMediaIdWithoutMedia(mediaTable, mediaId);
                    var mediaRow = mediaTable.FirstOrDefault();
                    
                    if (mediaRow != null)
                    {
                        media.MediaId = mediaRow.MediaID;
                        media.FileExtension = mediaRow.IsFileExtensionNull() ? null : mediaRow.FileExtension;
                        media.Name = mediaRow.Name;
                        media.FileName = mediaRow.FileName;

                        using(var taOrderMedia = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter())
                        {
                            media.Media = taOrderMedia.GetMediaStream(media.MediaId);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting media info.");
            }

            return media;
        }

        #endregion
    }
}