using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using Infragistics.Documents.Excel;
using DWOS.Shared.Utilities;
using System.Text.RegularExpressions;

namespace DWOS.DataImporter
{
    /// <summary>
    /// Maps parts from worksheets to the database.
    /// </summary>
    public sealed class PartFieldMapper : FieldMapper<Data.Datasets.PartsDataset.PartRow>
    {
        #region Fields

        private const string OUT_OF_RANGE_ERROR_FORMAT =
            "{0} had a value for {1} that was out of range.\n\t{1} has been set to {2}";
       
        private const decimal MIN_PRICE = -214748.3648M;
        private const decimal MAX_PRICE = 214748.3647M;
        private const string ProcessColumnPrefix = "Process_";
        private static readonly Regex ProcessColumnFormat = new Regex(@"Process_(\d+)");

        private readonly ReadOnlyCollection<string> _fieldsToIgnore = new ReadOnlyCollection<string>(new List<string>
        {
            "QuotePartID",
            "LastModified",
            "ParentID",
            "CustomerID",
            "PartID"
        });

        #endregion

        #region Properties

        public Data.Datasets.PartsDataset.PartDataTable Table
        {
            get;
            private set;
        }

        public IUserNotifier Notifier
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public PartFieldMapper(Data.Datasets.PartsDataset.PartDataTable table, IUserNotifier notifier)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table", "table cannot be null");
            }
            else if (notifier == null)
            {

                throw new ArgumentNullException(nameof(notifier));
            }

            Table = table;
            Notifier = notifier;
        }

        public override void MapAll(IEnumerable<FieldMap> fieldMaps, WorksheetRow worksheetRow, Data.Datasets.PartsDataset.PartRow dbRow)
        {
            foreach (var fieldMap in fieldMaps)
            {
                if (_fieldsToIgnore.Contains(fieldMap.ColumnName))
                {
                    continue;
                }
                else if (fieldMap.ColumnName.StartsWith(ProcessColumnPrefix))
                {
                    // Process import field - do not map yet
                    continue;
                }

                var column = Table.Columns[fieldMap.ColumnName];
                dbRow[column] = GetValue(fieldMap, worksheetRow, column.DataType, column.AllowDBNull);
            }

            // Correct & report errors
            if (!dbRow.IsEachPriceNull())
            {
                if (dbRow.EachPrice > MAX_PRICE || dbRow.EachPrice < MIN_PRICE)
                {
                    dbRow.EachPrice = 0M;
                    Notifier.ShowNotification(OUT_OF_RANGE_ERROR_FORMAT
                        .FormatWith(dbRow.Name, "EachPrice", dbRow.EachPrice.ToString()));
                }
            }

            if (!dbRow.IsLotPriceNull())
            {
                if (dbRow.LotPrice > MAX_PRICE || dbRow.LotPrice < MIN_PRICE)
                {
                    dbRow.LotPrice = 0M;
                    Notifier.ShowNotification(OUT_OF_RANGE_ERROR_FORMAT
                        .FormatWith(dbRow.Name, "LotPrice", dbRow.LotPrice.ToString()));
                }

            }

            dbRow.Name = dbRow.Name
                .Trim()
                .ToUpper();
        }

        public IEnumerable<string> GetProcessNames(IEnumerable<FieldMap> fieldMaps, WorksheetRow worksheetRow)
        {
            var idNameEntries = new List<Tuple<int, string>>();
            foreach (var fieldMap in fieldMaps.OrderBy(m => m.ColumnName))
            {
                if (!fieldMap.ColumnName.StartsWith(ProcessColumnPrefix))
                {
                    continue;
                }

                var idString = ProcessColumnFormat
                    .Match(fieldMap.ColumnName)
                    .Groups[1]
                    .Value;

                var processName = GetString(fieldMap, worksheetRow);

                if (int.TryParse(idString, out var id) && !string.IsNullOrEmpty(processName))
                {
                    idNameEntries.Add(new Tuple<int, string>(id, processName));
                }
            }

            return idNameEntries
                .OrderBy(e => e.Item1)
                .Select(e => e.Item2);
        }

        #endregion
    }
}
