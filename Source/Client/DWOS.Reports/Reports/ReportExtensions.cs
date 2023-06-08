using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using System;
using System.Threading;
using System.Collections.Generic;
using Color = Infragistics.Documents.Reports.Graphics.Color;
using Pens = Infragistics.Documents.Reports.Graphics.Pens;

namespace DWOS.Reports
{
    public static class ReportExtensions
    {
        #region Methods

        /// <summary>
        /// Calls <see cref="IReport.DisplayReport(CancellationToken)"/> with
        /// a default cancellation token.
        /// </summary>
        /// <remarks>
        /// Use when you don't need to pass a cancellation token to the report.
        /// </remarks>
        /// <param name="report"></param>
        public static void DisplayReport(this IReport report) =>
            report.DisplayReport(CancellationToken.None);

        public static ITableCell CreateTableCell(this ITableRow row, int relativeWidth) { return row.CreateTableCell(relativeWidth, System.Drawing.Color.Transparent); }

        public static ITableCell CreateTableCell(this ITableRow row, int relativeWidth, System.Drawing.Color backgroundColor)
        {
            ITableCell cell = row.AddCell();
            cell.Width = new RelativeWidth(relativeWidth);
            cell.Borders = new Borders(Pens.Black);
            cell.Paddings.All = 5;
            cell.Background = new Background(new SolidColorBrush(new Color(backgroundColor)));

            return cell;
        }

        public static ITableCell CreateTableHeaderCell(this ITable table, int relativeWidth)
        {
            ITableCell cell = table.Header.AddCell();
            cell.Width = new RelativeWidth(relativeWidth);
            cell.Borders = new Borders(Pens.Black);
            cell.Paddings.All = 5;
            cell.Background = new Background(new SolidColorBrush(new Color(System.Drawing.Color.LightGray))); //250, 250, 250)));
            cell.Alignment.Vertical = Alignment.Middle;

            return cell;
        }

        public static ITableCell CreateTableHeaderCell(this ITable table, int relativeWidth, Borders borders)
        {
            ITableCell cell = table.Header.AddCell();
            cell.Width = new RelativeWidth(relativeWidth);
            cell.Borders = borders ?? new Borders(Pens.Black);
            cell.Paddings.All = 5;
            cell.Background = new Background(new SolidColorBrush(new Color(System.Drawing.Color.LightGray))); //250, 250, 250)));
            cell.Alignment.Vertical = Alignment.Middle;

            return cell;
        }

        public static ITableCell AddCell(this ITableRow row, int relativeWidth, string text, Style textStyle, TextAlignment alignment)
        {
            ITableCell cell = row.AddCell();
            cell.Width = new RelativeWidth(relativeWidth);
            cell.AddText(text, textStyle, alignment);

            return cell;
        }

        public static ITableCell AddCell(this ITableRow row, int relativeWidth, string text, Style textStyle, Borders borders, int cellPadding, TextAlignment alignment)
        {
            ITableCell cell = row.AddCell();
            cell.Width = new RelativeWidth(relativeWidth);
            cell.AddText(text, textStyle, alignment);
            if (borders != null)
                cell.Borders = borders;
            cell.Paddings.All = cellPadding;

            return cell;
        }

        public static ITableCell AddCell(this ITableRow row, int relativeWidth, string text, Style textStyle, TextAlignment alignment, HorizontalMargins margins)
        {
            ITableCell cell = row.AddCell();
            cell.Width = new RelativeWidth(relativeWidth);
            cell.Margins = margins;
            cell.AddText(text, textStyle, alignment);

            return cell;
        }

        public static ITableCell AddCell(this ITableRow row, float fixedWidth, string text, Style textStyle, TextAlignment alignment)
        {
            ITableCell cell = row.AddCell();
            cell.Width = new FixedWidth(fixedWidth);
            cell.AddText(text, textStyle, alignment);

            return cell;
        }

        public static ITableCell AddCell(this ITableRow row, Width width, HorizontalMargins margins = null)
        {
            var cell = row.AddCell();
            cell.Width = width;

            if (margins != null)
            {
                cell.Margins = margins;
            }

            return cell;
        }

        public static IText AddText(this ITableCell cell, string text) { return cell.AddText(text, Report.DefaultStyles.NormalStyle); }

        public static IText AddText(this ITableCell cell, string text, Style textStyle, TextAlignment alignment)
        {
            IText txt = cell.AddText();
            txt.Style = textStyle;
            txt.AddContent(text == null ? string.Empty : text);
            txt.Alignment = alignment;

            return txt;
        }

        public static IText AddText(this ITableCell cell, string text, Style textStyle) { return cell.AddText(text, textStyle, TextAlignment.Center); }

        public static ITableCell[] AddCells(this ITableRow row, Style textStyle, Borders borders, int cellPadding, TextAlignment alignment, params string[] text)
        {
            int relWidth = 100 / text.Length;
            var cells = new ITableCell[text.Length];
            int index = 0;

            foreach(string item in text)
            {
                ITableCell cell = row.AddCell();
                cell.Width = new RelativeWidth(relWidth);
                if(borders != null)
                    cell.Borders = borders;
                cell.Paddings.All = cellPadding;

                IText txt = cell.AddText();
                txt.Style = textStyle;
                txt.AddContent(item ?? String.Empty);
                txt.Alignment = alignment;

                cells[index++] = cell;
            }

            return cells;
        }

        public static ITableCell[] AddCells(this ITableRow row, Style textStyle, Infragistics.Documents.Reports.Graphics.Brush backgroundBrush, Borders borders, int cellPadding, TextAlignment alignment, params string[] text)
        {
            int relWidth = 100 / text.Length;
            var cells = new ITableCell[relWidth];
            int index = 0;

            foreach(string item in text)
            {
                ITableCell cell = row.AddCell();
                cell.Background = new Background(backgroundBrush);
                cell.Width = new RelativeWidth(relWidth);
                if(borders != null)
                    cell.Borders = borders;
                cell.Paddings.All = cellPadding;

                IText txt = cell.AddText();
                txt.Style = textStyle;
                txt.AddContent(item);
                txt.Alignment = alignment;

                cells[index++] = cell;
            }

            return cells;
        }

        public static void AddRowSeperator(this ITableRow row, int cellPadding, Borders borders, bool addRule)
        {
            ITableCell cell = row.AddCell();
            cell.Width = new RelativeWidth(100);
            if(borders != null)
                cell.Borders = borders;
            cell.Paddings.All = cellPadding;

            if(addRule)
                cell.AddRule();
        }


        public static ITableCell[] AddHeaderCells(this ITableRow row, Style textStyle, Borders borders, int cellPadding, TextAlignment alignment, List<int> widths, params string[] text)
        {

            var cells = new ITableCell[text.Length];
            int index = 0;
            foreach (string item in text)
            {
                ITableCell cell = row.AddCell();
                cell.Width = new RelativeWidth(widths[index]);
                if (borders != null)
                    cell.Borders = borders;
                cell.Paddings.All = cellPadding;
                cell.Background = new Background(new Color(System.Drawing.Color.LightGray));

                IText txt = cell.AddText();
                txt.Style = textStyle;
                txt.AddContent(item);
                txt.Alignment = alignment;

                cells[index++] = cell;
            }

            return cells;
        }

        public static ITableCell[] AddHeaderCells(this ITableRow row, Style textStyle, Borders borders, int cellPadding, TextAlignment alignment, params string[] text)
        {
            int relWidth = 100 / text.Length;
            List<int> widths = new List<int>();
            for (var i=0; i<text.Length; i++)
            {
                widths.Add(relWidth);
            }

            var cells = AddHeaderCells(row, textStyle, borders, cellPadding, alignment, widths, text);

            return cells;
        }

        #endregion
    }
}