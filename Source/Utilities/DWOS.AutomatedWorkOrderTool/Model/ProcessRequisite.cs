namespace DWOS.AutomatedWorkOrderTool.Model
{
    public class ProcessRequisite
    {
        public int ParentProcessId { get; set; }

        public int ChildProcessId { get; set; }

        public decimal Hours { get; set; }
    }
}
