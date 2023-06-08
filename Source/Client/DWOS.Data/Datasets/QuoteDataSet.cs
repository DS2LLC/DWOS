
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Data.Datasets.QuoteDataSetTableAdapters
{
    public partial class QuoteSearchTableAdapter 
    {
        public enum QuoteSearchField { QuoteID, Program, RFQ, CustomerName, ContactName, PartName, All }

        public int FillBySearch(QuoteDataSet.QuoteSearchDataTable dataTable, QuoteSearchField field, string value, bool activeOnly, string customerName = null)
        {
            bool filterCustomer = !string.IsNullOrWhiteSpace(customerName);

            string sqlSelect = @"SELECT Quote.QuoteID,
                                        Quote.CreatedDate,
                                        Quote.Status,
                                        Quote.Program,
                                        Quote.RFQ,
                                        Customer.Name AS CustomerName,
                                        d_Contact.Name AS ContactName,
                                        Quote.ClosedDate
                                 FROM Quote ";

            string sqlJoin = @" LEFT OUTER JOIN Customer ON Quote.CustomerID = Customer.CustomerID 
                                LEFT OUTER JOIN d_Contact ON Quote.ContactID = d_Contact.ContactID ";

            string valueSearch; // search part of WHERE clause

            switch(field)
            {
                case QuoteSearchField.QuoteID:
                case QuoteSearchField.Program:
                case QuoteSearchField.RFQ:
                    valueSearch = string.Format("{0} LIKE @searchText", field);
                    break;
                case QuoteSearchField.CustomerName:
                    valueSearch = "Customer.Name LIKE @searchText";
                    break;
                case QuoteSearchField.ContactName:
                    valueSearch = "d_Contact.Name LIKE @searchText";
                    break;
                case QuoteSearchField.PartName:
                    valueSearch = "QuoteID IN (SELECT QuoteID FROM QuotePart WHERE Name LIKE @searchText)";
                    break;
                case QuoteSearchField.All:
                default:
                    valueSearch = filterCustomer ? 
                                        
                                        @"d_Contact.Name LIKE @searchText OR
                                        QuoteID LIKE @searchText OR
                                        Program LIKE @searchText OR
                                        RFQ LIKE @searchText OR
                                        QuoteID IN (SELECT QuoteID FROM QuotePart WHERE Name LIKE @searchText)" :
                        
                                        @"d_Contact.Name LIKE @searchText OR
                                        Customer.Name LIKE @searchText OR
                                        QuoteID LIKE @searchText OR
                                        Program LIKE @searchText OR
                                        RFQ LIKE @searchText OR
                                        QuoteID IN (SELECT QuoteID FROM QuotePart WHERE Name LIKE @searchText)";
                    break;
            }
        
            string sqlWhere;
            if (activeOnly)
            {
                sqlWhere = string.Format(" WHERE Status = 'Open' AND ({0}) ", valueSearch);
            }
            else
            {
                sqlWhere = string.Format(" WHERE {0} ", valueSearch);
            }

            if (filterCustomer)
                sqlWhere += "AND Customer.Name LIKE @filterCustomer";


            this.Adapter.SelectCommand = new System.Data.SqlClient.SqlCommand(sqlSelect + sqlJoin + sqlWhere, this.Connection);
            this.Adapter.SelectCommand.Parameters.AddWithValue("@searchText", "%" + value + "%");

            if (filterCustomer)
                this.Adapter.SelectCommand.Parameters.AddWithValue("@filterCustomer", "%" + customerName + "%");

            if ((this.ClearBeforeFill))
                dataTable.Clear();

            return this.Adapter.Fill(dataTable);
        }

        public int FillByQuoteIds(QuoteDataSet.QuoteSearchDataTable dataTable, List<int> quoteIds)
        {
            string sqlSelect = @"SELECT Quote.QuoteID,
                                        Quote.CreatedDate,
                                        Quote.Status,
                                        Quote.Program,
                                        Quote.RFQ,
                                        Customer.Name AS CustomerName,
                                        d_Contact.Name AS ContactName,
                                        Quote.ClosedDate
                                 FROM Quote ";

            string sqlJoin = @" LEFT OUTER JOIN Customer ON Quote.CustomerID = Customer.CustomerID 
                                LEFT OUTER JOIN d_Contact ON Quote.ContactID = d_Contact.ContactID ";

            string sqlWhere = @"WHERE QuoteID IN ( ";
            List<string> quoteIdsParam = quoteIds.Select(q => $"'{q}', ").ToList();
            quoteIdsParam.ForEach(q =>
            {
                sqlWhere += q;
                if (q == quoteIdsParam.Last())
                    sqlWhere = sqlWhere.Remove(sqlWhere.Length - 2); //remove last comma and space

            });
            sqlWhere += " )";

            this.Adapter.SelectCommand = new System.Data.SqlClient.SqlCommand(sqlSelect + sqlJoin + sqlWhere, this.Connection);
            if ((this.ClearBeforeFill))
                dataTable.Clear();

            return this.Adapter.Fill(dataTable);

        }
    }
}
namespace DWOS.Data.Datasets
{


    public partial class QuoteDataSet
    {
    }
}
namespace DWOS.Data.Datasets {
    
    
    public partial class QuoteDataSet {
    }
}
