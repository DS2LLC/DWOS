using DWOS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DWOS.Dashboard.Charts.Controls
{
    /// <summary>
    /// Interaction logic for ReworkReasonsChart.xaml
    /// </summary>
    /// <remarks>
    /// This acts as the actual chart for <see cref="ReworkReasons"/>.
    /// </remarks>
    internal partial class ReworkReasonsChart : UserControl
    {
        #region Fields

        /// <summary>
        /// Date format for rework reason counts.
        /// </summary>
        private const string REWORK_REASONS_DATE_FORMAT = "MM/dd";

        /// <summary>
        /// Text to display when the 'rework reason' database value is null.
        /// </summary>
        private const string REWORK_REASON_NOT_AVAILABLE = "N/A";

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ReworkReasonsChart"/> class.
        /// </summary>
        public ReworkReasonsChart()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows results in the chart.
        /// </summary>
        /// <param name="results">Rework entries to summarize and show.</param>
        /// <param name="settings">Chart options.</param>
        public void Show(IEnumerable<DWOS.Data.Reports.Dashboard.ReworkEntryRow> results, ReworkReasonsSettings settings)
        {
            var multipleSeries = new List<Infragistics.Controls.Charts.Series>();

            var xAxis = new Infragistics.Controls.Charts.CategoryXAxis()
            {
                Label = "{Group}"
            };

            var yAxis = new Infragistics.Controls.Charts.NumericYAxis()
            {
                Title = "Count",
                MinimumValue = 0
            };

            switch (settings.GroupBy)
            {
                case ReworkReasonsSettings.GroupByType.Days:
                    xAxis.Title = "Days";
                    foreach (var series in ChartByDay(results, settings.FromDate, settings.ToDate).GroupBy(entry => entry.SeriesName))
                    {
                        multipleSeries.Add(new Infragistics.Controls.Charts.ColumnSeries()
                        {
                            ItemsSource = series,
                            ValueMemberPath = "Count",
                            Title = series.Key,
                            XAxis = xAxis,
                            YAxis = yAxis,
                            ShowDefaultTooltip = true
                        });
                    }

                    break;
                case ReworkReasonsSettings.GroupByType.Weeks:
                    xAxis.Title = "Weeks";
                    foreach (var series in ChartByWeek(results, settings.FromDate, settings.ToDate).GroupBy(entry => entry.SeriesName))
                    {
                        multipleSeries.Add(new Infragistics.Controls.Charts.ColumnSeries()
                        {
                            ItemsSource = series,
                            ValueMemberPath = "Count",
                            Title = series.Key,
                            XAxis = xAxis,
                            YAxis = yAxis,
                            ShowDefaultTooltip = true
                        });
                    }

                    break;
                case ReworkReasonsSettings.GroupByType.Reason:
                    xAxis.Title = "Reason";
                    multipleSeries.Add(new Infragistics.Controls.Charts.ColumnSeries()
                    {
                        ItemsSource = ChartByReason(results, settings.ShowParetoChart),
                        ValueMemberPath = "Count",
                        Title = "Reworks",
                        XAxis = xAxis,
                        YAxis = yAxis,
                        ShowDefaultTooltip = true
                    });

                    break;
                default:
                    break;
            }

            reworkChart.Series.Clear();
            reworkChart.Axes.Clear();
            reworkChart.Axes.Add(xAxis);
            reworkChart.Axes.Add(yAxis);

            foreach (var series in multipleSeries)
            {
                reworkChart.Series.Add(series);
            }

            var firstSeries = multipleSeries.FirstOrDefault();
            if (firstSeries != null)
            {
                // Must be set to show the x-axis.
                xAxis.ItemsSource = firstSeries.ItemsSource;
            }

            if (settings.GroupBy == ReworkReasonsSettings.GroupByType.Reason)
            {
                reworkChartLegend.Visibility = Visibility.Collapsed;

                if (settings.ShowParetoChart)
                {
                    yAxis.MaximumValue = results.Count();

                    var yParetoAxis = new Infragistics.Controls.Charts.NumericYAxis()
                    {
                        Title = "Cumulative %",
                        Label="{}%",
                        MinimumValue = 0,
                        MaximumValue = 100,
                        MajorStrokeThickness = 0,
                        MinorStrokeThickness = 0,
                        LabelSettings = new Infragistics.Controls.Charts.AxisLabelSettings()
                        {
                            Location = Infragistics.Controls.Charts.AxisLabelsLocation.OutsideRight,
                        }
                    };

                    reworkChart.Axes.Add(yParetoAxis);

                    yAxis.TickmarkValues = new ParetoTickmarkValues(ParetoTickmarkMode.Values);
                    yParetoAxis.TickmarkValues = new ParetoTickmarkValues(ParetoTickmarkMode.Percentage);

                    // Pareto chart's cumulative total line.
                    reworkChart.Series.Add(new Infragistics.Controls.Charts.PointSeries()
                    {
                        ItemsSource = ChartForParetoLine(results),
                        ValueMemberPath = "Count",
                        XAxis = xAxis,
                        YAxis = yParetoAxis,
                        ShowDefaultTooltip = false,
                        MarkerType = Infragistics.Controls.Charts.MarkerType.None,
                        TrendLineType = Infragistics.Controls.Charts.TrendLineType.LogarithmicFit,
                        TrendLineBrush = Brushes.Green,
                        TrendLineThickness = 3d,
                        TrendLineDashArray = new DoubleCollection() { 3d, 3d }
                    });
                }
            }
            else
            {
                xAxis.LabelSettings = new Infragistics.Controls.Charts.AxisLabelSettings()
                {
                    Angle = 30,
                };

                reworkChartLegend.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Creates chart items that are grouped by reason and day.
        /// </summary>
        /// <param name="reworkDataEntries"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns>A collection of chart items.</returns>
        private static IEnumerable<ReworkChartItem> ChartByDay(IEnumerable<DWOS.Data.Reports.Dashboard.ReworkEntryRow> reworkDataEntries, DateTime fromDate, DateTime toDate)
        {
            var reworkReasons = reworkDataEntries
                .Select(i => i.IsReworkReasonNull() ? string.Empty : i.ReworkReason)
                .Distinct();

            var dates = GetAllDates(fromDate, toDate);

            var chartItems = new List<ReworkChartItem>();

            foreach (var reason in reworkReasons)
            {
                var matchingEntries = reworkDataEntries.Where(i => reason == (i.IsReworkReasonNull() ? string.Empty : i.ReworkReason));
                chartItems.AddRange(dates.Select(date => new ReworkChartItem()
                {
                    SeriesName = string.IsNullOrEmpty(reason) ? REWORK_REASON_NOT_AVAILABLE : reason,
                    Group = date.ToString(REWORK_REASONS_DATE_FORMAT),
                    Count = matchingEntries.Count(entry => entry.Date.Date == date)
                }));
            }

            return chartItems;
        }

        /// <summary>
        /// Creates chart items that are grouped by reason and week.
        /// </summary>
        /// <param name="reworkDataEntries"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns>A collection of chart items.</returns>
        private static IEnumerable<ReworkChartItem> ChartByWeek(IEnumerable<DWOS.Data.Reports.Dashboard.ReworkEntryRow> reworkDataEntries, DateTime fromDate, DateTime toDate)
        {
            var reworkReasons = reworkDataEntries
                .Select(i => i.IsReworkReasonNull() ? string.Empty : i.ReworkReason)
                .Distinct();

            var chartItemDict = new Dictionary<DateTime, List<ReworkChartItem>>();

            foreach (var monday in GetWeeks(fromDate, toDate, DayOfWeek.Monday))
            {
                var listInProgress = new List<ReworkChartItem>();

                foreach (var reason in reworkReasons)
                {
                    listInProgress.Add(new ReworkChartItem()
                    {
                        SeriesName = string.IsNullOrEmpty(reason) ? REWORK_REASON_NOT_AVAILABLE : reason,
                        Group = monday.ToString(REWORK_REASONS_DATE_FORMAT)
                    });
                }

                chartItemDict.Add(monday, listInProgress);
            }

            foreach (var entry in reworkDataEntries)
            {
                List<ReworkChartItem> itemsForWeek = null;
                var key = entry.Date.StartOfWeek(DayOfWeek.Monday);

                if (chartItemDict.TryGetValue(key, out itemsForWeek))
                {
                    var matchingChartEntry = itemsForWeek.FirstOrDefault(i => i.SeriesName == (entry.IsReworkReasonNull() ? REWORK_REASON_NOT_AVAILABLE : entry.ReworkReason));

                    if (matchingChartEntry != null)
                    {
                        matchingChartEntry.Count += 1;
                    }
                }
            }

            var returnItems = new List<ReworkChartItem>();

            foreach (var items in chartItemDict.Values)
            {
                returnItems.AddRange(items);
            }

            return returnItems;
        }

        /// <summary>
        /// Creates chart items that are grouped by reason.
        /// </summary>
        /// <remarks>
        /// If this instance is going to show a Pareto chart, items are
        /// returned in descending order by Count. Otherwise, they are
        /// in alphabetical order by rework reason.
        /// </remarks>
        /// <param name="reworkDataEntries"></param>
        /// <param name="showParetoChart"></param>
        /// <returns>A collection of chart items.</returns>
        private static IEnumerable<ReworkChartItem> ChartByReason(IEnumerable<DWOS.Data.Reports.Dashboard.ReworkEntryRow> reworkDataEntries, bool showParetoChart)
        {
            var reworkReasons = reworkDataEntries
                .Select(i => i.IsReworkReasonNull() ? string.Empty : i.ReworkReason)
                .Distinct();

            var chartItems = reworkReasons.Select(reason => new ReworkChartItem()
            {
                SeriesName = "N/A", // unused when grouping by reason
                Group = string.IsNullOrEmpty(reason) ? REWORK_REASON_NOT_AVAILABLE : reason,
                Count = reworkDataEntries.Count(entry => reason == (entry.IsReworkReasonNull() ? string.Empty : entry.ReworkReason))
            });

            if (showParetoChart)
            {
                return chartItems.OrderByDescending(i => i.Count);
            }
            else
            {
                return chartItems.OrderBy(i => i.Group);
            }
        }

        /// <summary>
        /// Creates chart items that are intended for use as a Pareto line.
        /// </summary>
        /// <remarks>
        /// Unlike other methods, this uses the the Count property of
        /// <see cref="ReworkChartItem"/> to store the cumulative percentage
        /// shown in a Pareto chart's line graph. Count should not go above
        /// 100.
        /// This method assumes that  <see cref="ChartByReason"/> returns items
        /// ordered by descending count.
        /// </remarks>
        /// <param name="reworkDataEntries"></param>
        /// <returns></returns>
        private static IEnumerable<ReworkChartItem> ChartForParetoLine(IEnumerable<DWOS.Data.Reports.Dashboard.ReworkEntryRow> reworkDataEntries)
        {
            var reworkCounts = reworkDataEntries
                .Select(i => i.IsReworkReasonNull() ? string.Empty : i.ReworkReason)
                .Distinct()
                .ToDictionary<string, string, int>(reason => reason, (reason) => reworkDataEntries.Count(entry => reason == (entry.IsReworkReasonNull() ? string.Empty : entry.ReworkReason)));

            var reworkReasons = reworkDataEntries
                .Select(i => i.IsReworkReasonNull() ? string.Empty : i.ReworkReason)
                .Distinct()
                .OrderByDescending(reason => reworkCounts[reason]);

            int total = reworkDataEntries.Count();
            int sum = 0;
            var chartItems = new List<ReworkChartItem>();
            foreach (var reason in reworkReasons)
            {
                sum += reworkCounts[reason];
                chartItems.Add(new ReworkChartItem()
                {
                    SeriesName = "N/A", // unused when grouping by reason
                    Group = string.IsNullOrEmpty(reason) ? REWORK_REASON_NOT_AVAILABLE : reason,
                    Count = Convert.ToInt32((Convert.ToDouble(sum) / Convert.ToDouble(total)) * 100) // percentage
                });
            }

            return chartItems;
        }

        /// <summary>
        /// Returns a list containing all dates in the range.
        /// </summary>
        /// <param name="fromDate">Starting date (inclusive).</param>
        /// <param name="toDate">Ending date (inclusive).</param>
        /// <returns>List of all dates in the range.</returns>
        private static List<DateTime> GetAllDates(DateTime fromDate, DateTime toDate)
        {
            var daysSpan = toDate.Subtract(fromDate).Days;

            var dates = new List<DateTime>();
            for (int x = 0; x <= daysSpan; x++)
            {
                dates.Add(fromDate.AddDays(x));
            }

            return dates;
        }

        /// <summary>
        /// Returns a list of weeks that have at least one day in the range.
        /// </summary>
        /// <param name="fromDate">Starting date (inclusive).</param>
        /// <param name="toDate">Ending date (inclusive).</param>
        /// <param name="startOfWeek">Determines starting day for the week.</param>
        /// <returns>
        /// <see cref="DateTime"/> for the start of every week in the range.
        /// </returns>
        private static List<DateTime> GetWeeks(DateTime fromDate, DateTime toDate, DayOfWeek startOfWeek)
        {
            HashSet<DateTime> dayHash = new HashSet<DateTime>();
            var daysSpan = toDate.Subtract(fromDate).Days;

            for (int x = 0; x <= daysSpan; x++)
            {
                dayHash.Add(fromDate.AddDays(x).StartOfWeek(startOfWeek));
            }

            var days = dayHash.ToList();
            days.Sort();
            return days;
        }

        #endregion

        #region ReworkChartItem

        /// <summary>
        /// Item to be shown in the chart.
        /// </summary>
        private sealed class ReworkChartItem
        {
            /// <summary>
            /// Gets or sets the series name.
            /// </summary>
            public string SeriesName { get; set; }

            /// <summary>
            /// Gets or sets the group name.
            /// </summary>
            public string Group { get; set; }

            /// <summary>
            /// Gets or sets the count.
            /// </summary>
            public int Count { get; set; }
        }

        #endregion

        #region ParetoTickmarkValues

        /// <summary>
        /// Tickmark values for use in Pareto charts.
        /// </summary>
        private sealed class ParetoTickmarkValues : Infragistics.Controls.Charts.TickmarkValues
        {
            #region Fields

            private const double RESOLUTION_LIMIT = 250.0d;
            private readonly DoubleCollection _tickmarks;

            #endregion

            #region Properties

            /// <summary>
            /// Mode of this instance.
            /// </summary>
            public ParetoTickmarkMode Mode
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="ParetoTickmarkValues"/> class.
            /// </summary>
            /// <param name="mode"></param>
            public ParetoTickmarkValues(ParetoTickmarkMode mode)
            {
                Mode = mode;
                _tickmarks = new DoubleCollection();
            }

            public override void Initialize(Infragistics.Controls.Charts.TickmarkValuesInitializationParameters initializationParameters)
            {
                base.Initialize(initializationParameters);
                _tickmarks.Clear();
                double minimumValue = initializationParameters.VisibleMinimum;
                double maximumValue = initializationParameters.VisibleMaximum;
                double incrementValue;

                switch (Mode)
                {
                    case ParetoTickmarkMode.Values:
                        if (initializationParameters.Resolution > RESOLUTION_LIMIT)
                        {
                            // Show values every 10%
                            incrementValue = (maximumValue - minimumValue) * 0.1d;
                        }
                        else
                        {
                            // Show values every 25%
                            incrementValue = (maximumValue - minimumValue) * 0.25d;
                        }

                        break;
                    case ParetoTickmarkMode.Percentage:
                        if (initializationParameters.Resolution > RESOLUTION_LIMIT)
                        {
                            // Show 0%, 10%, 20%, 30%, ..., 100%
                            incrementValue = 10.0d;
                        }
                        else
                        {
                            // Show 0%, 25%, 50%, 75%, 100%
                            incrementValue = 25.0d;
                        }

                        break;
                    default:
                        throw new InvalidOperationException("ParetoTickmarkMode has an invalid value.");
                }

                for (double tickmarkValue = minimumValue; tickmarkValue <= maximumValue; tickmarkValue += incrementValue)
                {
                    _tickmarks.Add(tickmarkValue);
                }
            }

            public override IEnumerable<double> MajorValues()
            {
                return _tickmarks;
            }

            #endregion

            public override IEnumerable<double> MinorValues()
            {
                // Do not include minor values
                yield break;
            }
        }

        #endregion

        #region ParetoTickmarkMode

        /// <summary>
        /// Mode for <see cref="ParetoTickmarkValues"/> class.
        /// </summary>
        private enum ParetoTickmarkMode
        {
            Values = 0,
            Percentage = 1,
        }

        #endregion
    }
}
