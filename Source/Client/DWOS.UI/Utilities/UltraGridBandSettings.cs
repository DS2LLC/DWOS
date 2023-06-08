using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Settings base class for use with <see cref="GridSettingsPersistence{T}"/>.
    /// </summary>
    public class UltraGridBandSettings
    {
        #region Properties

        public ISet<string> HiddenColumns { get; } =
            new HashSet<string>();

        public IDictionary<string, ColumnSettings> Columns { get; } =
            new Dictionary<string, ColumnSettings>();

        public IDictionary<string, ColumnSortSettings> ColumnSort { get; set; } =
            new Dictionary<string, ColumnSortSettings>();

        #endregion

        #region Methods

        public UltraGridBandSettings()
        {
            HiddenColumns = new HashSet<string>();
            Columns = new Dictionary<string, ColumnSettings>();
        }

        public virtual void ApplyTo(UltraGridBand band)
        {
            if (band == null)
            {
                throw new ArgumentNullException(nameof(band));
            }

            foreach (var columnKey in HiddenColumns)
            {
                if (band.Columns.Exists(columnKey))
                {
                    band.Columns[columnKey].Hidden = true;
                }
            }

            foreach (var columnKeyValue in Columns)
            {
                if (band.Columns.Exists(columnKeyValue.Key))
                {
                    var column = band.Columns[columnKeyValue.Key];
                    column.Header.VisiblePosition = columnKeyValue.Value.Order;
                    column.Width = columnKeyValue.Value.Width;
                    column.Hidden = false;
                }
            }

            if (ColumnSort.Count > 0)
            {
                band.SortedColumns.Clear();
                band.ClearGroupByColumns();

                foreach (var columnKeyValue in ColumnSort.OrderBy(kv => kv.Value.SortIndex))
                {
                    if (band.Columns.Exists(columnKeyValue.Key))
                    {
                        var column = band.Columns[columnKeyValue.Key];
                        band.SortedColumns.Add(column, columnKeyValue.Value.IsDescending, columnKeyValue.Value.IsGroupByColumn);
                    }
                }
            }
        }

        public virtual void RetrieveSettingsFrom(UltraGridBand band)
        {
            if (band == null)
            {
                throw new ArgumentNullException(nameof(band));
            }

            HiddenColumns.Clear();
            Columns.Clear();

            foreach (var column in band.Columns)
            {
                if (column.ExcludeFromColumnChooser == ExcludeFromColumnChooser.True && column.Hidden)
                {
                    continue;
                }

                if (column.Hidden)
                {
                    HiddenColumns.Add(column.Key);
                }
                else
                {
                    Columns.Add(column.Key, new ColumnSettings
                    {
                        Order = column.Header.VisiblePosition,
                        Width = column.Width
                    });
                }
            }

            ColumnSort.Clear();

            var currentIndex = 0;
            foreach (var column in band.SortedColumns.OfType<UltraGridColumn>())
            {
                ColumnSort.Add(column.Key, new ColumnSortSettings()
                {
                    SortIndex = currentIndex,
                    IsDescending = column.SortIndicator == SortIndicator.Descending,
                    IsGroupByColumn = column.IsGroupByColumn
                });

                // Remove sorted column from hidden columns list.
                // Otherwise, moving the column may permanently hide it.
                HiddenColumns.Remove(column.Key);
                ++currentIndex;
            }
        }

        #endregion

        #region ColumnSettings

        public sealed class ColumnSettings
        {
            public int Order { get; set; }

            public int Width { get; set; }
        }

        #endregion


        #region ColumnSortSettings

        public class ColumnSortSettings
        {
            public bool IsDescending { get; set; }

            public int SortIndex { get; set; }

            public bool IsGroupByColumn { get; set; }
        }

        #endregion
    }
}
