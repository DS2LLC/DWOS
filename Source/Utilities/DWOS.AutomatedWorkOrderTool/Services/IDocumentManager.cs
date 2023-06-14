using System.Collections.Generic;
using DWOS.AutomatedWorkOrderTool.Model;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    public interface IDocumentManager
    {
        DocumentFolder GetFolder(string name);

        DocumentFolder GetFolder(DocumentFolder parentFolder, string name);

        DocumentFolder CreateFolder(string name);

        DocumentFolder CreateFolder(DocumentFolder parentFolder, string name);

        DocumentFile GetFile(DocumentFolder folder, string name, string mediaType);

        DocumentFile CreateFile(DocumentFolder folder, string name, string mediaType);

        void Revise(DocumentFile file, string sourceFile, DwosUser currentUser);

        bool MatchesAnyRevision(DocumentFile file, string sourceFile);

        IEnumerable<DocumentFile> GetFiles(DocumentFolder folder);
    }
}
