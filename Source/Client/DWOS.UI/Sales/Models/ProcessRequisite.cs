namespace DWOS.UI.Sales.Models
{
    public class ProcessRequisite
    {
        public int ChildProcessId { get; }

        public decimal Hours { get; }

        public ProcessRequisite(int childProcessId, decimal hours)
        {
            ChildProcessId = childProcessId;
            Hours = hours;
        }
    }
}
