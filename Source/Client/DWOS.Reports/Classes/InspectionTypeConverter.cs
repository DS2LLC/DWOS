using DWOS.Data.Reports;
using System;
using System.Collections.Generic;
using System.Data;

namespace DWOS.Reports
{
    public class InspectionTypeConverter : CodeValueConverterAbstract
    {
        #region Fields

        private static StandardValuesCollection _standardColl;
        private static Dictionary<int, string> _dictionary;

        #endregion

        #region Properties

        protected override IDataReader DataReader
        {
            get { return this.LoadData(); }
        }

        protected override StandardValuesCollection StandardValues
        {
            get { return _standardColl; }
            set { _standardColl = value; }
        }

        protected override Dictionary<int, string> Dictionary
        {
            get { return _dictionary; }
            set { _dictionary = value; }
        }

        #endregion

        #region Methods

        private IDataReader LoadData()
        {
            try
            {
                var dsQA = new QAReport();

                using (var taInpsectionType = new DWOS.Data.Reports.QAReportTableAdapters.PartInspectionTypeTableAdapter())
                    taInpsectionType.Fill(dsQA.PartInspectionType);

                var dv = new DataView(dsQA.PartInspectionType);
                return dv.ToTable(true, dsQA.PartInspectionType.PartInspectionTypeIDColumn.ColumnName, dsQA.PartInspectionType.NameColumn.ColumnName).CreateDataReader();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error loading part inspection names.");
                return null;
            }
        }

        #endregion
    }
}
