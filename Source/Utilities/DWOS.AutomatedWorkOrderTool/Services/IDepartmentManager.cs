using System.Collections.Generic;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    /// <summary>
    /// Manages strings that represent department names.
    /// </summary>
    public interface IDepartmentManager
    {
        /// <summary>
        /// Gets the name of the department responsible for part marking.
        /// </summary>
        string PartMarkDepartment { get; }

        /// <summary>
        /// Gets a sorted collection of all departments.
        /// </summary>
        IEnumerable<string> AllDepartments { get; }
    }
}
