using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DWOS.Data.Datasets
{
}
namespace DWOS.Data.Datasets
{
}

namespace DWOS.Data.Datasets.PartsDatasetTableAdapters
{
    partial class ReceivingSummaryTableAdapter
    {
    }

    partial class PartTableAdapter
    {
    }

    partial class PartSearchTableAdapter
    {
        public enum PartSearchField { PartName, CustomerName, Material, Airframe, AssemblyNumber, All }

        public int FillBySearch(PartsDataset.PartSearchDataTable dataTable, PartSearchField field, string value, bool activeOnly)
        {
            string sqlSelect = @"SELECT Part.PartID, Part.Name, Customer.Name AS CustomerName, Part.Material, Part.ManufacturerID, Part.Airframe, Part.Active, Part.AssemblyNumber
                                FROM Part 
                                INNER JOIN Customer ON Part.CustomerID = Customer.CustomerID ";

            var sqlWhere = " WHERE";
            value = Utilities.SqlBless(value);

            switch (field)
            {
                case PartSearchField.All:
                    sqlWhere += string.Format(" (Part.Name LIKE '%{0}%' OR Customer.Name LIKE '%{0}%' OR Material LIKE '%{0}%' OR Airframe LIKE '%{0}%' OR AssemblyNumber LIKE '%{0}%') ", value);
                    break;
                case PartSearchField.PartName:
                     sqlWhere += string.Format(" Part.Name LIKE '%{0}%'", value);
                    break;
                case PartSearchField.Material:
                case PartSearchField.Airframe:
                case PartSearchField.AssemblyNumber:
                    sqlWhere += string.Format(" {0} LIKE '%{1}%'", field, value);
                    break;
                case PartSearchField.CustomerName:
                    sqlWhere += string.Format(" Customer.Name LIKE '%{0}%'", value);
                    break;
                default:
                    break;
            }

            if (activeOnly)
                sqlWhere += " AND Part.Active = 1";

            this.Adapter.SelectCommand = new System.Data.SqlClient.SqlCommand(sqlSelect + sqlWhere, this.Connection);

            if ((this.ClearBeforeFill == true))
                dataTable.Clear();

            return this.Adapter.Fill(dataTable);
        }
    }

	public partial class MediaTableAdapter
	{
        // These methods are named 'Update2' because the table adapter has
        // methods named 'Update' with internal access

		public virtual int Update2(PartsDataset dataSet)
		{
			return this.Adapter.Update(dataSet, "Media");
		}

        public virtual int Update2(IEnumerable<PartsDataset.MediaRow> media)
        {
            return Adapter.Update(media.OfType<DataRow>().ToArray());
        }
	}
}
namespace DWOS.Data.Datasets
{


    public partial class PartsDataset
    {
        partial class ProcessRow
        {
            /// <summary>
            /// Gets the formatted name for the process.
            /// </summary>
            public string ProcessName
            {
                get
                {
                    return IsRevisionNull()
                        ? Name
                        : $"{Name} {Revision}";
                }
            }
        }
    }
}


namespace DWOS.Data.Datasets.PartsDatasetTableAdapters
{


    public partial class ReceivingSummaryTableAdapter
    {
    }
}
