namespace DWOS.AutomatedWorkOrderTool.Model
{
    public class DocumentFolder
    {
        public int DocumentFolderId { get; set; }

        public string Name { get; set; }

        public DocumentFolder Parent { get; set; }
    }
}
