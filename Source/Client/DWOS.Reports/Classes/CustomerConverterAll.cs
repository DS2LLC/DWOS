using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using System;
using System.Collections.Generic;
using System.Data;

namespace DWOS.Reports
{
    /// <summary>
    ///   Same as Customer Converter but add a "ALL" customers item
    /// </summary>
    public class CustomerConverterAll : CodeValueConverterAbstract
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
                CustomersDataset.CustomerRow allCustomers = customers.NewCustomerRow();
                allCustomers.ItemArray = customers[0].ItemArray;
                allCustomers.CustomerID = -1;
                allCustomers.Name = "<ALL>";
                customers.Rows.InsertAt(allCustomers, 0);
                var dv = new DataView(customers);
                return dv.ToTable(true, customers.CustomerIDColumn.ColumnName, customers.NameColumn.ColumnName).CreateDataReader();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error loading customers.");
                return null;
            }
            finally
            {
                if (taCustomers != null)
                    taCustomers.Dispose();

                taCustomers = null;
            }
        }

        #endregion
    }
}
