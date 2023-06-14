namespace DWOS.AutomatedWorkOrderTool.Model
{
    public class DocumentFile
    {
        public DocumentFolder Folder { get; set; }

        public int DocumentInfoId { get; set; }

        public string Name { get; set; }

        public string MediaType { get; set; }
    }
}
