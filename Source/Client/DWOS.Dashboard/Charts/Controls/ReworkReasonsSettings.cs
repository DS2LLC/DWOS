using DWOS.Data;
using System;
using System.Runtime.Serialization;

namespace DWOS.Dashboard.Charts.Controls
{
    /// <summary>
    /// Contains settings used by <see cref="ReworkReasons"/>
    /// and <see cref="ReworkReasonsChart"/>.
    /// </summary>
    [DataContract]
    public class ReworkReasonsSettings : WidgetSettings
    {
        #region Fields

        /// <summary>
        /// Special value for DepartmentID that represents all departments.
        /// </summary>
        public const string DEPARTMENT_ALL = "All";

        private const string DEFAULT_DEPARTMENT = DEPARTMENT_ALL;
        private const int DEFAULT_DAYS = 10;
        private const GroupByType DEFAULT_GROUPING = GroupByType.Reason;
        private const bool DEFAULT_PARETO_VALUE = true;

        private int _days;
        private string _departmentID;
        private GroupByType _groupBy;
        private bool _showParetoChart;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the number of days to span.
        /// </summary>
        [DataMember]
        public int Days
        {
            get
            {
                return _days;
            }
            set
            {
                if (_days != value)
                {
                    _days = value;
                    OnPropertyChanged("Days");
                    OnPropertyChanged("FromDate");
                    OnPropertyChanged("ToDate");
                }
            }
        }

        /// <summary>
        /// Gets or set the DepartmentID.
        /// </summary>
        [DataMember]
        public string DepartmentID
        {
            get
            {
                return _departmentID;
            }
            set
            {
                if (_departmentID != value)
                {
                    _departmentID = value;
                    OnPropertyChanged("DepartmentID");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how items should be grouped
        /// in the chart.
        /// </summary>
        [DataMember]
        public GroupByType GroupBy
        {
            get
            {
                return _groupBy;
            }
            set
            {
                if (_groupBy != value)
                {
                    _groupBy = value;
                    OnPropertyChanged("GroupBy");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that toggles a special "Pareto chart" mode
        /// for the chart.
        /// </summary>
        [DataMember]
        public bool ShowParetoChart
        {
            get
            {
                return _showParetoChart;
            }
            set
            {
                if (_showParetoChart != value)
                {
                    _showParetoChart = value;
                    OnPropertyChanged("ShowParetoChart");
                }
            }
        }

        /// <summary>
        /// Gets the starting date of the date range.
        /// </summary>
        public DateTime FromDate
        {
            get
            {
                return DateTime.Today.AddDays((-1 * _days) + 1);
            }
        }

        /// <summary>
        /// Gets the ending date of the date range.
        /// </summary>
        public DateTime ToDate
        {
            get
            {
                return DateTime.Today.EndOfDay();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ReworkReasonsSettings"/> class.
        /// </summary>
        public ReworkReasonsSettings()
        {
            _days = DEFAULT_DAYS;
            _departmentID = DEFAULT_DEPARTMENT;
            _groupBy = DEFAULT_GROUPING;
            _showParetoChart = DEFAULT_PARETO_VALUE;
        }

        #endregion

        #region GroupBy

        /// <summary>
        /// Types of item grouping.
        /// </summary>
        public enum GroupByType
        {
            /// <summary>
            /// Group items only by reason.
            /// </summary>
            Reason = 0,

            /// <summary>
            /// Group items by reason and day.
            /// </summary>
            Days = 1,

            /// <summary>
            /// Group items by reason and week.
            /// </summary>
            Weeks = 2,
        }

        #endregion
    }
}
