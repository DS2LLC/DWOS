using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.DocumentsDataSetTableAdapters;
using DWOS.Shared.Utilities;
using Ionic.Zlib;

// TODO - Ensure that all file & folder names are valid
namespace DWOS.AutomatedWorkOrderTool.Services
{
    public class DocumentManager : IDocumentManager
    {
        #region Methods

        private static void Save(DocumentsDataSet dsDocuments)
        {
            using (var taManager = new TableAdapterManager())
            {
                taManager.DocumentFolderTableAdapter = new DocumentFolderTableAdapter();
                taManager.DocumentInfoTableAdapter = new DocumentInfoTableAdapter();
                taManager.DocumentFolder_DocumentInfoTableAdapter = new DocumentFolder_DocumentInfoTableAdapter();
                taManager.DocumentRevisionTableAdapter = new DocumentRevisionTableAdapter();

                taManager.UpdateAll(dsDocuments);
            }
        }

        private static string FormatMediaType(string mediaType) => mediaType.TrimStart('.');

        #endregion

        #region IDocumentManager Test

        public DocumentFile CreateFile(DocumentFolder folder, string name, string mediaType)
        {
            if (folder == null)
            {
                throw new ArgumentNullException(nameof(folder));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            using (var dsDocuments = new DocumentsDataSet())
            {
                // Load document folders - needed for relation
                using (var taDocumentFolder = new DocumentFolderTableAdapter())
                {
                    taDocumentFolder.Fill(dsDocuments.DocumentFolder);
                }

                var docInfoRow = dsDocuments.DocumentInfo.NewDocumentInfoRow();
                docInfoRow.Name = name.TrimToMaxLength(255);
                docInfoRow.MediaType = FormatMediaType(mediaType);
                docInfoRow.CurrentRevision = 0;
                docInfoRow.DocumentLocked = false;
                docInfoRow.IsDeleted = false;
                dsDocuments.DocumentInfo.AddDocumentInfoRow(docInfoRow);

                var folderDocLinkRow = dsDocuments.DocumentFolder_DocumentInfo.NewDocumentFolder_DocumentInfoRow();
                folderDocLinkRow.DocumentFolderID = folder.DocumentFolderId;
                folderDocLinkRow.DocumentInfoRow = docInfoRow;
                dsDocuments.DocumentFolder_DocumentInfo.AddDocumentFolder_DocumentInfoRow(folderDocLinkRow);

                Save(dsDocuments);

                return new DocumentFile
                {
                    DocumentInfoId = docInfoRow.DocumentInfoID,
                    Folder = folder,
                    Name = docInfoRow.Name,
                    MediaType = docInfoRow.MediaType
                };
            }
        }

        public DocumentFolder CreateFolder(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            using (var dsDocuments = new DocumentsDataSet())
            {
                var docRow = dsDocuments.DocumentFolder.NewDocumentFolderRow();
                docRow.Name = name;
                docRow.IsDeleted = false;
                dsDocuments.DocumentFolder.AddDocumentFolderRow(docRow);

                Save(dsDocuments);

                return new DocumentFolder
                {
                    Name = docRow.Name,
                    DocumentFolderId = docRow.DocumentFolderID
                };
            }
        }

        public DocumentFolder CreateFolder(DocumentFolder parentFolder, string name)
        {
            if (parentFolder == null)
            {
                throw new ArgumentNullException(nameof(parentFolder));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            using (var dsDocuments = new DocumentsDataSet())
            {
                var docRow = dsDocuments.DocumentFolder.NewDocumentFolderRow();
                docRow.Name = name;
                docRow.IsDeleted = false;
                docRow.ParentID = parentFolder.DocumentFolderId;
                dsDocuments.DocumentFolder.AddDocumentFolderRow(docRow);

                Save(dsDocuments);

                return new DocumentFolder
                {
                    Name = docRow.Name,
                    DocumentFolderId = docRow.DocumentFolderID,
                    Parent = parentFolder
                };
            }
        }

        public DocumentFile GetFile(DocumentFolder folder, string name, string mediaType)
        {
            if (folder == null)
            {
                throw new ArgumentNullException(nameof(folder));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentNullException(nameof(mediaType));
            }

            using (var dtDocumentInfo = new DocumentsDataSet.DocumentInfoDataTable())
            {
                using (var taInfo = new DocumentInfoTableAdapter())
                {
                    taInfo.FillByFolder(dtDocumentInfo, folder.DocumentFolderId);
                    var match = dtDocumentInfo.FirstOrDefault(doc => !doc.IsDeleted && doc.Name == name && doc.MediaType == FormatMediaType(mediaType));

                    if (match == null)
                    {
                        return null;
                    }

                    return new DocumentFile
                    {
                        DocumentInfoId = match.DocumentInfoID,
                        Folder = folder,
                        MediaType = match.MediaType,
                        Name = match.Name
                    };
                }
            }
        }

        public DocumentFolder GetFolder(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            using (var dtFolder = new DocumentsDataSet.DocumentFolderDataTable())
            {
                using (var taFolder = new DocumentFolderTableAdapter())
                {
                    taFolder.Fill(dtFolder);
                }

                var match = dtFolder.FirstOrDefault(f => !f.IsDeleted && f.IsParentIDNull() && f.Name == name);

                if (match == null)
                {
                    return null;
                }

                return new DocumentFolder
                {
                    DocumentFolderId = match.DocumentFolderID,
                    Name = match.Name
                };
            }
        }

        public DocumentFolder GetFolder(DocumentFolder parentFolder, string name)
        {
            if (parentFolder == null)
            {
                return GetFolder(name);
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            using (var dtFolder = new DocumentsDataSet.DocumentFolderDataTable())
            {
                using (var taFolder = new DocumentFolderTableAdapter())
                {
                    taFolder.Fill(dtFolder);
                }

                var match = dtFolder
                    .FirstOrDefault(f => !f.IsDeleted && !f.IsParentIDNull() && f.ParentID == parentFolder.DocumentFolderId && f.Name == name);

                if (match == null)
                {
                    return null;
                }

                return new DocumentFolder
                {
                    DocumentFolderId = match.DocumentFolderID,
                    Name = match.Name,
                    Parent = parentFolder
                };
            }
        }

        public bool MatchesAnyRevision(DocumentFile file, string sourceFile)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.IsNullOrEmpty(sourceFile))
            {
                throw new ArgumentNullException(nameof(sourceFile));
            }

            var contentData = MediaUtilities.CreateMediaStream(sourceFile, -1);
            using (var dtRevision = new DocumentsDataSet.DocumentRevisionDataTable())
            {
                using (var taDocumentRevision = new DocumentRevisionTableAdapter())
                {
                    taDocumentRevision.FillByDocument(dtRevision, file.DocumentInfoId);

                    foreach (var revisionRow in dtRevision)
                    {
                        var revisionData = taDocumentRevision.GetMediaStream(revisionRow.DocumentRevisionID);

                        if (revisionData.Length > 0 && revisionRow.IsCompressed)
                        {
                            revisionData = ZlibStream.UncompressBuffer(revisionData);
                        }

                        if (ByteUtilities.AreEqual(contentData, revisionData))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void Revise(DocumentFile file, string sourceFile, DwosUser currentUser)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.IsNullOrEmpty(sourceFile))
            {
                throw new ArgumentNullException(nameof(sourceFile));
            }

            if (currentUser == null)
            {
                throw new ArgumentNullException(nameof(currentUser));
            }

            using (var dsDocument = new DocumentsDataSet())
            {
                using (var taDocumentInfo = new DocumentInfoTableAdapter())
                {
                    taDocumentInfo.FillByID(dsDocument.DocumentInfo, file.DocumentInfoId);
                }

                // Check document existence & lock
                var documentRow = dsDocument.DocumentInfo.FirstOrDefault();
                if (documentRow == null)
                {
                    throw new ArgumentException(@"Document not found", nameof(file));
                }

                if (documentRow.DocumentLocked)
                {
                    throw new ArgumentException(@"Document locked", nameof(file));
                }

                // Get latest revision, even if it's different from what the document said
                using (var taRevision = new DocumentRevisionTableAdapter())
                {
                    taRevision.FillByDocument(dsDocument.DocumentRevision, documentRow.DocumentInfoID);
                }

                var nextRevision = 1;

                if (dsDocument.DocumentRevision.Count > 0)
                {
                    nextRevision = 1 + dsDocument.DocumentRevision.Max(rev => rev.RevisionNumber);
                }

                documentRow.CurrentRevision = nextRevision;

                // Revise
                var md5Hash = FileSystem.GetMD5HashFromFile(sourceFile);
                var fileContent = MediaUtilities.CreateMediaStream(sourceFile, -1);
                var newRevisionRow = dsDocument.DocumentRevision.AddDocumentRevisionRow(documentRow.Name, documentRow, null, currentUser.Id,
                    nextRevision, md5Hash, DateTime.UtcNow, false, null);

                // Save changes
                Save(dsDocument);
                using (var taRevision = new DocumentRevisionTableAdapter())
                {
                    taRevision.UpdateDocumentData(fileContent, false, newRevisionRow.DocumentRevisionID);
                }
            }
        }

        public IEnumerable<DocumentFile> GetFiles(DocumentFolder folder)
        {
            if (folder == null)
            {
                return Enumerable.Empty<DocumentFile>();
            }

            using (var dtDocumentInfo = new DocumentsDataSet.DocumentInfoDataTable())
            {
                using (var taInfo = new DocumentInfoTableAdapter())
                {
                    taInfo.FillByFolder(dtDocumentInfo, folder.DocumentFolderId);
                }

                return dtDocumentInfo
                    .Select(row => new DocumentFile
                    {
                        DocumentInfoId = row.DocumentInfoID,
                        Folder = folder,
                        Name = row.Name,
                        MediaType = row.MediaType
                    }).ToList();
            }
        }

        #endregion
    }
}
