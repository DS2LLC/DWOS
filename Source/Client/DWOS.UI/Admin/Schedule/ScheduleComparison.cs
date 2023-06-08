using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DWOS.Data;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace DWOS.UI.Admin.Schedule
{
    public partial class ScheduleComparison : Form
    {
        public ScheduleComparison()
        {
            InitializeComponent();
        }

        public void LoadData(Data.Datasets.ScheduleDataset.OrderScheduleDataTable orderSchedule)
        {
            grdSchedule.DataSource = orderSchedule.DataSet;
        }

        private void grdSchedule_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if(e.Row.IsDataRow)
            {
                var orderRow = ((DataRowView) e.Row.ListObject).Row as Data.Datasets.ScheduleDataset.OrderScheduleRow;

                if(!orderRow.IsEstShipDateNull() && !orderRow.IsNull(WorkScheduler.LASTESTSHIPDATE_COLUMN))
                {
                    var previousDate = Convert.ToDateTime(orderRow[WorkScheduler.LASTESTSHIPDATE_COLUMN]);
                    var newDate = orderRow.EstShipDate;
                    var diff = newDate.Date.Subtract(previousDate.Date).TotalMinutes * -1; //Flip the sign
                    e.Row.Cells["Difference"].Value = diff; 
                    
                    if(diff < 0)
                        e.Row.Cells["Difference"].Appearance.ForeColor = Color.Red;
                    else if (diff > 0)
                        e.Row.Cells["Difference"].Appearance.ForeColor = Color.Green;

                }
            }
        }

        private class TimeSpanDataFilter : Infragistics.Win.IEditorDataFilter
        {
            object Infragistics.Win.IEditorDataFilter.Convert(Infragistics.Win.EditorDataFilterConvertArgs args)
            {
                switch (args.Direction)
                {
                    case ConversionDirection.EditorToOwner:
                        args.Handled = false;
                        return null;
                    case ConversionDirection.OwnerToEditor:
                        int minutes = 0;
                        if(args.Value != null && int.TryParse(args.Value.ToString(), out minutes))
                        {
                            args.Handled = true;
                            var time = TimeSpan.FromMinutes(minutes);
                            if(minutes == 0)
                                return " - ";
                            else
                                return "{0} {1} Days".FormatWith((minutes > 0 ? "+" : ""), time.Days, time.Hours);
                        }
                        return args.Value;
                }

                return args.Value;
            }
        }

        private void grdSchedule_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["Difference"].Editor.DataFilter = new TimeSpanDataFilter();
            e.Layout.Bands[0].Columns["Difference"].SortComparer = new TimeSpanSortComparer();
        }

        private class TimeSpanSortComparer : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                if (x is UltraGridCell && y is UltraGridCell)
                {
                    var xCell = (UltraGridCell)x;
                    var yCell = (UltraGridCell)y;

                    if (xCell.Value is int && yCell.Value is int)
                    {
                        return Convert.ToInt32(xCell.Value).CompareTo(Convert.ToInt32(yCell.Value));
                    }
                }

                return 0;
            }

            #endregion
        }
    }
}
