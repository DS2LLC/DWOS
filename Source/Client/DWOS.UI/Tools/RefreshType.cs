using System;

namespace DWOS.UI.Tools
{
    [Flags]
    internal enum RefreshType
    {
        Order = 1,

        Department = 2,

        Settings = 4,

        Line = 8,

        WorkingDays = 16
    }
}
