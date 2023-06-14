using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.Data;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    public class DepartmentManager : IDepartmentManager
    {
        #region Properties

        public IDataManager DataManager { get; }

        #endregion

        #region Methods

        public DepartmentManager(IDataManager dataManager)
        {
            DataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
        }

        #endregion

        #region IDepartmentManager Members

        public string PartMarkDepartment => ApplicationSettings.Current.DepartmentPartMarking;

        public IEnumerable<string> AllDepartments
        {
            get
            {
                using (var dsAwot = new AwotDataSet())
                {
                    DataManager.LoadInitialData(dsAwot);
                    return dsAwot.d_Department.Select(d => d.DepartmentID);
                }
            }
        }

        #endregion
    }
}
