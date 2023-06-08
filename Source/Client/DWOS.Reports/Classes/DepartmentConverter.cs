using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ScheduleDatasetTableAdapters;
using System;
using System.Collections.Generic;
using System.Data;

namespace DWOS.Reports
{
    public class DepartmentConverter: CodeConverterAbstract
    {
        #region Fields

        private static StandardValuesCollection _standardColl;
        private static List<string> _dictionary;

        #endregion

        #region Properties

        protected override IDataReader DataReader
        {
            get { return this.LoadDepartments(); }
        }

        protected override StandardValuesCollection StandardValues
        {
            get { return _standardColl; }
            set { _standardColl = value; }
        }

        protected override List<string> Values
        {
            get { return _dictionary; }
            set { _dictionary = value; }
        }

        #endregion

        #region Methods

        private IDataReader LoadDepartments()
        {
            ScheduleDataset.d_DepartmentDataTable departments = null;

            try
            {
                using(var ta = new d_DepartmentTableAdapter())
                {
                    departments = new ScheduleDataset.d_DepartmentDataTable();
                    ta.Fill(departments);

                    return departments.CreateDataReader();
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error loading departments.");
                return null;
            }
        }

        #endregion
    }
}
