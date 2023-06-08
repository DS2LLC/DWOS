using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using System;
using System.Collections.Generic;
using System.Data;

namespace DWOS.Reports
{
    public class CustomerConverter: CodeValueConverterAbstract
    {
        #region Fields

        private static StandardValuesCollection _standardColl;
        private static Dictionary<int, string> _dictionary;

        #endregion

        #region Properties

        protected override IDataReader DataReader
        {
            get { return this.LoadCustomers(); }
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

        private IDataReader LoadCustomers()
        {
            CustomerTableAdapter taCustomers = null;
            CustomersDataset.CustomerDataTable customers = null;

            try
            {
                customers = new CustomersDataset.CustomerDataTable();
                taCustomers = new CustomerTableAdapter();
                taCustomers.Fill(customers);

                var dv = new DataView(customers);
                return dv.ToTable(true, customers.CustomerIDColumn.ColumnName, customers.NameColumn.ColumnName).CreateDataReader();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error loading customers.");
                return null;
            }
            finally
            {
                if(taCustomers != null)
                    taCustomers.Dispose();

                taCustomers = null;
            }
        }

        #endregion
    }
}
