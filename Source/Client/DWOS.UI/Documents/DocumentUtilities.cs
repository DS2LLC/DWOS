using DWOS.Data.Datasets;
using DWOS.Data.Datasets.DocumentsDataSetTableAdapters;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DWOS.UI.Documents
{
    /// <summary>
    /// Utilities related to Documents.
    /// </summary>
    static class DocumentUtilities
    {
        #region Fields

        /// <summary>
        /// Document links for new receiving orders should have this value
        /// for LinkToKey.
        /// </summary>
        /// <remarks>
        /// The value is a arbitrary but negative value.
        /// </remarks>
        public const int RECEIVING_ID = -1000; // arbitrary value; must be negative

        /// <summary>
        /// Document links for new receiving parts should have this value
        /// for LinkToKey.
        /// </summary>
        /// <remarks>
        /// The value is a arbitrary but negative value.
        /// </remarks>
        public const int RECEIVING_PART_ID = -1001;

        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves a document's file name.
        /// </summary>
        /// <param name="documentInfoID"></param>
        /// <returns>File name of the document.</returns>
        public static string GetFileName(int documentInfoID)
        {
            using (var taDoc = new DocumentInfoTableAdapter())
            {
                var documentInfo = taDoc.GetByID(documentInfoID).FirstOrDefault();
                return documentInfo.Name;
            }
        }

        /// <summary>
        /// Downloads a document by its document info ID.
        /// </summary>
        /// <param name="documentInfoID"></param>
        /// <returns>Local path to the downloaded file (or null if not found).</returns>
        public static string DownloadDocument(int documentInfoID)
        {
            DocumentsDataSet.DocumentInfoRow documentInfo = null;
            int currentRevisionID = -1;
            using (var taDoc = new DocumentInfoTableAdapter())
            {
                documentInfo = taDoc.GetByID(documentInfoID).FirstOrDefault();
                object revisionIDObj = taDoc.GetCurrentRevisionID(documentInfoID);

                if (revisionIDObj != null)
                {
                    currentRevisionID = Convert.ToInt32(revisionIDObj);
                }
            }

            string fileName = null;
            if (documentInfo != null && currentRevisionID > 0)
            {
                fileName = DownloadRevision(currentRevisionID, documentInfo.MediaType);
            }

            return fileName;
        }

        /// <summary>
        /// Downloads a document by its doc revision ID and media type.
        /// </summary>
        /// <param name="docRevisionID"></param>
        /// <param name="mediaType"></param>
        /// <returns>local file path to the downloaded file (or null if not found.)</returns>
        private static string DownloadRevision(int docRevisionID, string mediaType)
        {
            _log.Info("Downloading file '{0}' of type '{1}'.", docRevisionID, mediaType);

            string filePath = null;
            using (var taDocumentRevision = new DocumentRevisionTableAdapter())
            {
                var docRevision = taDocumentRevision.GetByRevisionID(docRevisionID).FirstOrDefault();

                if (docRevision != null)
                {
                    //FileName_DOCID_DOCREVNUMBER.extension
                    var uniqueFileName = Path.GetFileNameWithoutExtension(docRevision.FileName) + "_" + docRevision.DocumentInfoID + "_" + docRevisionID + "." + mediaType;
                    filePath = Path.Combine(FileSystem.UserDocumentPath(), About.ApplicationName, "Links", uniqueFileName);

                    //if not already downloaded then download
                    if (!File.Exists(filePath))
                    {
                        var bytes = taDocumentRevision.GetMediaStream(docRevision.DocumentRevisionID);

                        if (bytes != null) //Allow 0 length files as you could have an empty text file
                        {
                            if (bytes.Length > 0 && docRevision.IsCompressed)
                            {
                                NLog.LogManager.GetCurrentClassLogger().Info("UnCompressing file '{0}'", docRevision.FileName);
                                bytes = ZlibStream.UncompressBuffer(bytes);
                            }

                            var directory = Path.GetDirectoryName(filePath);

                            if (!Directory.Exists(directory))
                            {
                                Directory.CreateDirectory(directory);
                            }

                            using (var fileStream = new BinaryWriter(File.OpenWrite(filePath)))
                                fileStream.Write(bytes);
                        }
                    }
                }
            }

            return filePath;
        }

        #endregion
    }
}
