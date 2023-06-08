using DWOS.UI.Utilities;

namespace DWOS.UI.Admin.Schedule.Manual
{
    public interface ISchedulingPersistence
    {
        ISecurityManager SecurityManager { get; }
        void SaveChanges(SchedulingTabDataContext context);
    }
}