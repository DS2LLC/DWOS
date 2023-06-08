using System;

namespace DWOS.UI.Admin.Schedule.Manual
{
    public class DepartmentAddedEventArgs : EventArgs
    {
        public DepartmentDataContext Context { get; }

        public DepartmentAddedEventArgs(DepartmentDataContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Context = context;
        }
    }
}
