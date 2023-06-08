using System.Collections.Generic;

namespace DWOS.Data
{
    /// <summary>
    /// Schedule for the department, including each shift.
    /// </summary>
    public class DepartmentSchedule
    {
        #region Fields
        
        public string Department { get; set; }

        public List<ShiftSchedule> ShiftSchedules { get; set; }
        
        #endregion
    }


}
