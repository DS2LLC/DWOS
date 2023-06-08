using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Shared.Utilities;

namespace DWOS.UI.Documents
{
    internal static class DocumentManagerUtilities
    {
        #region Fields

        private static string _rootWorkingPath;

        public static readonly string[] INVALID_EXTENSIONS = Properties.Settings.Default
            .DocumentTypesToExclude
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        public const int MAX_UPLOAD_BYTES = 4194304; // 4MB

        #endregion

        #region Properties

        public static string RootFolderPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_rootWorkingPath))
                {
                    _rootWorkingPath = UserSettings.Default.DocumentsWorkingDirectory;

                    if (string.IsNullOrWhiteSpace(_rootWorkingPath))
                    {
                        //if no folder then use default one
                        _rootWorkingPath = FileSystem.GetFolder(FileSystem.enumFolderType.Documents, true);
                    }
                }

                return _rootWorkingPath;
            }
            set { _rootWorkingPath = value; }
        }

        #endregion

        #region Methods

        public static string BuildPath(DocumentsDataSet documents, DocumentsDataSet.DocumentFolderRow folder)
        {
            var fullPath = new List<string>();

            DocumentsDataSet.DocumentFolderRow currentFolder = folder;
            fullPath.Add(currentFolder.Name);

            while (!currentFolder.IsParentIDNull())
            {
                currentFolder = documents.DocumentFolder.FindByDocumentFolderID(currentFolder.ParentID);
                fullPath.Add(currentFolder.Name);
            }

            fullPath.Reverse();
            return fullPath.Join('\\');
        }

        public static bool ShouldCompressFileType(string extension)
        {
            if (extension == null)
                return false;

            extension = extension.ToLower();

            switch (extension)
            {
                case "jpg":
                case "pdf":
                case "gif":
                case "mp3":
                case "mpeg":
                    return false;
                default:
                    return true;
            }
        }

        public static DocumentsDataSet.DocumentLockRow GetCurrentDocumentLock(DocumentsDataSet.DocumentInfoRow documentInfo)
        {
            return documentInfo.GetDocumentLockRows()
                .Where(dlr => dlr.IsDateUnlockedUTCNull())
                .OrderByDescending(dlr => dlr.DateLockedUTC).FirstOrDefault();
        }

        public static DocumentsDataSet.DocumentRevisionRow GetCurrentDocumentRevision(DocumentsDataSet.DocumentInfoRow documentInfo)
        {
            return documentInfo.GetDocumentRevisionRows()
                .FirstOrDefault(dr => dr.RevisionNumber == documentInfo.CurrentRevision);
        }

        #endregion
    }
}