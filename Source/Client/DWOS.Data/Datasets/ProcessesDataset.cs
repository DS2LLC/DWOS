using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DWOS.Data.Datasets
{


    public partial class ProcessesDataset
    {
    }
}

namespace DWOS.Data.Datasets.ProcessesDatasetTableAdapters
{
    partial class PartInspectionTypeTableAdapter
    {
        public int? GetRevisedPartInspectionType(int partInspectionTypeId)
        {
            var previousRevision = partInspectionTypeId;

            while (true)
            {
                if (GetRevision(previousRevision) is int revision)
                {
                    previousRevision = revision;
                }
                else
                {
                    return partInspectionTypeId == previousRevision ? (int?) null : previousRevision;
                }
            }
        }
    }

    partial class ProcessStepDocumentLinkTableAdapter
    {
    }

    public partial class ProcessSearchTableAdapter
    {
        public enum ProcessSearchField
        {
            All,
            Name,
            Alias,
            CustomerAlias,
            CustomerName,
            Department,
            Category,
        }

        public int FillBySearch(ProcessesDataset.ProcessSearchDataTable dataTable, ProcessSearchField field, string value, bool activeOnly)
        {
            string sql = "SELECT DISTINCT Process.ProcessID," +
                "Process.Name," +
                "Process.Revision," +
                "Process.Department," +
                "Process.Category," +
                "Process.IsPaperless," +
                "Process.ModifiedDate," +
                "(CASE WHEN Process.Active = 1 AND Process.IsApproved = 1 THEN 'Approved' WHEN Process.Active = 1 AND IsNull(Process.IsApproved, 0) = 0 THEN 'Planned' ELSE 'Closed' END) AS Status\n" +
                "FROM Process\n";

            var joins = new List<string>();
            var parameters = new List<SqlParameter>();
            var searchConditions = new List<string>();

            if (activeOnly)
            {
                searchConditions.Add("Process.Active = 1");
            }

            if (!string.IsNullOrEmpty(value))
            {
                string likeValue = "%" + Convert.ToString(value) + "%";
                switch (field)
                {
                    case ProcessSearchField.Name:
                    case ProcessSearchField.Department:
                    case ProcessSearchField.Category:
                        searchConditions.Add(string.Format("Process.{0} LIKE @value", field));
                        parameters.Add(new SqlParameter("@value", likeValue));
                        break;
                    case ProcessSearchField.Alias:
                        joins.Add("INNER JOIN ProcessAlias ON ProcessAlias.ProcessID = Process.ProcessID");
                        searchConditions.Add("ProcessAlias.Name LIKE @value");
                        parameters.Add(new SqlParameter("@value", likeValue));
                        break;
                    case ProcessSearchField.CustomerAlias:
                        joins.Add("INNER JOIN ProcessAlias ON ProcessAlias.ProcessID = Process.ProcessID");
                        joins.Add("INNER JOIN CustomerProcessAlias ON CustomerProcessAlias.ProcessAliasID = ProcessAlias.ProcessAliasID");
                        searchConditions.Add("CustomerProcessAlias.Name LIKE @value");
                        parameters.Add(new SqlParameter("@value", likeValue));
                        break;
                    case ProcessSearchField.CustomerName:
                        joins.Add("INNER JOIN ProcessAlias ON ProcessAlias.ProcessID = Process.ProcessID");
                        joins.Add("INNER JOIN CustomerProcessAlias ON CustomerProcessAlias.ProcessAliasID = ProcessAlias.ProcessAliasID");
                        joins.Add("INNER JOIN Customer ON Customer.CustomerID = CustomerProcessAlias.CustomerID");
                        searchConditions.Add("Customer.Name LIKE @customerName");
                        parameters.Add(new SqlParameter("@customerName", likeValue));
                        break;
                    default:
                        break;
                }
            }

            if (joins.Count > 0)
            {
                sql += string.Join("\n", joins) + "\n";
            }

            if (searchConditions.Count > 0)
            {
                sql += "WHERE " + string.Join(" AND ", searchConditions);
            }

            sql += ";";


            this.Adapter.SelectCommand = new System.Data.SqlClient.SqlCommand(sql, this.Connection);
            this.Adapter.SelectCommand.Parameters.AddRange(parameters.ToArray());

            if (this.ClearBeforeFill)
            {
                dataTable.Clear();
            }

            //clear dependent table to prevent constraint errors
            var dsProcesses = dataTable.DataSet as ProcessesDataset;

            if (dsProcesses != null)
            {
                dsProcesses.ProcessAliasSearch.Clear();
                dsProcesses.CustomerProcessAliasSearch.Clear();
            }

            return this.Adapter.Fill(dataTable);
        }
    }
}
