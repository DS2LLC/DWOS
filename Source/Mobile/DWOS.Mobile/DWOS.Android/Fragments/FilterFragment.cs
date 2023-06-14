using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using DWOS.Android.Utilities;
using DWOS.ViewModels;

namespace DWOS.Android
{
    /// <summary>
    /// Shows filter options.
    /// </summary>
    public class FilterFragment : DialogFragment
    {
        #region Fields

        public const string FILTER_FRAGMENT_TAG = "FilterFragment";
        const string BUNDLEID_DEPARTMENT = "department";
        const string BUNDLEID_LINE = "line";
        const string BUNDLEID_STATUS = "status";
        const string BUNDLEID_TYPE = "type";

        Spinner _typeSpinner;
        Spinner _departmentSpinner;
        Spinner _lineSpinner;
        Spinner _statusSpinner;
        Button _resetTypeButton;
        Button _resetDepartmentButton;
        Button _resetLineButton;
        Button _resetStatusButton;
        string _intialDepartment;
        string _intialStatus;
        string _initialLine;
        FilterType _initialType;

        #endregion

        #region Methods

        public FilterFragment()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterFragment"/> class.
        /// </summary>
        /// <param name="department">The department.</param>
        /// <param name="processingLine">The processing line.</param>
        /// <param name="status">The status.</param>
        /// <param name="type">The filter type.</param>
        public FilterFragment(string department, string processingLine, string status, FilterType type)
            : base()
        {
            _intialDepartment = department;
            _initialLine = processingLine;
            _intialStatus = status;
            _initialType = type;
        }

        /// <summary>
        /// Override to build your own custom Dialog container.
        /// </summary>
        /// <param name="savedInstanceState">The last saved instance state of the Fragment,
        /// or null if this is a freshly created Fragment.</param>
        /// <returns>
        /// To be added.
        /// </returns>
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var builder = new AlertDialog.Builder(Activity);
            builder.SetTitle("Filter");
            builder.SetPositiveButton("OK", (sender, args) => OKClicked());
            builder.SetNegativeButton("Cancel", (sender, args) => CancelClicked());
            var factory = LayoutInflater.From(Activity);
            var view = factory.Inflate(Resource.Layout.FilterFragmentLayout, null);
            _typeSpinner = view.FindViewById<Spinner>(Resource.Id.spinnerFilter);
            _departmentSpinner = view.FindViewById<Spinner>(Resource.Id.spinnerDepartment);
            _lineSpinner = view.FindViewById<Spinner>(Resource.Id.spinnerProcessingLine);
            _statusSpinner = view.FindViewById<Spinner>(Resource.Id.spinnerStatus);
            _resetTypeButton = view.FindViewById<Button>(Resource.Id.buttonResetFilter);
            _resetDepartmentButton = view.FindViewById<Button>(Resource.Id.buttonResetDepartment);
            _resetLineButton = view.FindViewById<Button>(Resource.Id.buttonResetLine);
            _resetStatusButton = view.FindViewById<Button>(Resource.Id.buttonResetStatus);
            _resetTypeButton.Click += (sender, args) => _typeSpinner.SetSelection(0, true);
            _resetStatusButton.Click += (sender, args) => _statusSpinner.SetSelection(0, animate: true);
            _resetDepartmentButton.Click += (sender, args) => _departmentSpinner.SetSelection(0, animate: true);
            _resetLineButton.Click += (sender, args) => _lineSpinner.SetSelection(0, true);

            if (savedInstanceState != null)
            {
                _intialDepartment = savedInstanceState.GetString(BUNDLEID_DEPARTMENT);
                _initialLine = savedInstanceState.GetString(BUNDLEID_LINE);
                _intialStatus = savedInstanceState.GetString(BUNDLEID_STATUS);
                _initialType = (FilterType)Enum.Parse(typeof(FilterType), savedInstanceState.GetString(BUNDLEID_TYPE));
            }

            LoadSpinners();

            var filterTypeVisibility = ApplicationSettings.Settings.UsingManualScheduling ? ViewStates.Visible : ViewStates.Gone;

            using (var filterHeaderRow = view.FindViewById(Resource.Id.filterHeaderRow))
            {
                filterHeaderRow.Visibility = filterTypeVisibility;
            }

            using (var filterContentRow = view.FindViewById(Resource.Id.filterContentRow))
            {
                filterContentRow.Visibility = filterTypeVisibility;
            }

            var lineVisibility = ApplicationSettings.Settings.UsingMultipleLines ? ViewStates.Visible : ViewStates.Gone;

            using (var lineHeaderRow = view.FindViewById(Resource.Id.lineHeaderRow))
            {
                lineHeaderRow.Visibility = lineVisibility;
            }

            using (var lineContentRow = view.FindViewById(Resource.Id.lineContentRow))
            {
                lineContentRow.Visibility = lineVisibility;
            }

            builder.SetView(view);

            return builder.Create();
        }

        public override void OnDestroyView()
        {
            _typeSpinner.Dispose();
            _departmentSpinner.Dispose();
            _lineSpinner.Dispose();
            _statusSpinner.Dispose();
            _resetTypeButton.Dispose();
            _resetDepartmentButton.Dispose();
            _resetLineButton.Dispose();
            _resetStatusButton.Dispose();

            base.OnDestroyView();
        }

        private void LoadSpinners()
        {
            // Filter
            var typeItems = new List<ValueItem<FilterType>>
            {
                new ValueItem<FilterType>(FilterType.Normal, "Normal"),
                new ValueItem<FilterType>(FilterType.SchedulePriority, "Schedule Priority"),
            };

            var typeAdapter = new ArrayAdapter<ValueItem<FilterType>>(Activity, global::Android.Resource.Layout.SimpleSpinnerItem, typeItems);
            typeAdapter.SetDropDownViewResource(global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _typeSpinner.Adapter = typeAdapter;
            _typeSpinner.SetSelection(typeItems.IndexOf(typeItems.First(t => t.Value == _initialType)));

            // Department
            var departmentsAdapter = new ArrayAdapter<string>(Activity, global::Android.Resource.Layout.SimpleSpinnerItem,
                ApplicationSettings.Settings.Departments);
            departmentsAdapter.SetDropDownViewResource(global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
            departmentsAdapter.Insert("All", 0);
            _departmentSpinner.Adapter = departmentsAdapter;
            var selectedIndex = 0;
            if (!string.IsNullOrEmpty(_intialDepartment))
                selectedIndex = ApplicationSettings.Settings.Departments.IndexOf(_intialDepartment) + 1;
            _departmentSpinner.SetSelection(selectedIndex, animate: false);

            // Line
            var linesAdapter = new ArrayAdapter<string>(Activity, global::Android.Resource.Layout.SimpleSpinnerItem,
                ApplicationSettings.Settings.ProcessingLines);
            linesAdapter.SetDropDownViewResource(global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
            linesAdapter.Insert("All", 0);
            _lineSpinner.Adapter = linesAdapter;

            var selectedLineIndex = 0;
            if (!string.IsNullOrEmpty(_initialLine))
                selectedLineIndex = ApplicationSettings.Settings.ProcessingLines.IndexOf(_initialLine) + 1;
            _lineSpinner.SetSelection(selectedLineIndex, false);

            // Status
            var statusList = new List<string>();
            if (!string.IsNullOrEmpty(ApplicationSettings.Settings.WorkStatusInProcess))
                statusList.Add(ApplicationSettings.Settings.WorkStatusInProcess);
            if (!string.IsNullOrEmpty(ApplicationSettings.Settings.WorkStatusChangingDepartment))
                statusList.Add(ApplicationSettings.Settings.WorkStatusChangingDepartment);
            if (!string.IsNullOrEmpty(ApplicationSettings.Settings.WorkStatusPendingInspection))
                statusList.Add(ApplicationSettings.Settings.WorkStatusPendingInspection);
            var statusAdapter = new ArrayAdapter<string>(Activity, global::Android.Resource.Layout.SimpleSpinnerItem, statusList);
            statusAdapter.SetDropDownViewResource(global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
            statusAdapter.Insert("All", 0);
            _statusSpinner.Adapter = statusAdapter;
            selectedIndex = 0;
            if (!string.IsNullOrEmpty(_intialStatus))
                selectedIndex = statusList.IndexOf(_intialStatus) + 1;
            _statusSpinner.SetSelection(selectedIndex, animate: false);
        }

        private void OKClicked()
        {
            var callback = Activity as IFilterFragmentCallback;
            if (callback != null)
            {
                var department = GetDepartmentSpinnerValue();
                var line = GetLineSpinnerValue();
                var status = GetStatusSpinnerValue();
                var type = GetTypeSpinnerValue();
                callback.OnFilterDimissed(this, true, department, line, status, type);
            }
        }

        private FilterType GetTypeSpinnerValue()
        {
            var adapter = _typeSpinner.Adapter as ArrayAdapter<ValueItem<FilterType>>;

            if (adapter == null)
            {
                return FilterType.Normal;
            }

            return adapter.GetItem(_typeSpinner.SelectedItemPosition).Value;
        }

        private string GetDepartmentSpinnerValue()
        {
            var adapter = (ArrayAdapter<string>)_departmentSpinner.Adapter;
            var value = adapter.GetItem(_departmentSpinner.SelectedItemPosition);
            var department = value != "All" ? value : string.Empty;
            return department;
        }

        private string GetLineSpinnerValue()
        {
            var adapter = (ArrayAdapter<string>)_lineSpinner.Adapter;
            var value = adapter.GetItem(_lineSpinner.SelectedItemPosition);
            var lineValue = value != "All" ? value : string.Empty;
            return lineValue;
        }

        private string GetStatusSpinnerValue()
        {
            var adapter = (ArrayAdapter<string>)_statusSpinner.Adapter;
            var value = adapter.GetItem(_statusSpinner.SelectedItemPosition);
            var status = value != "All" ? value : string.Empty;
            return status;
        }

        private void CancelClicked()
        {
            var callback = Activity as IFilterFragmentCallback;
            if (callback != null)
            {
                callback.OnFilterDimissed(this, false, string.Empty, string.Empty, string.Empty, FilterType.Normal);
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString(BUNDLEID_TYPE, GetTypeSpinnerValue().ToString());
            outState.PutString(BUNDLEID_DEPARTMENT, GetDepartmentSpinnerValue());
            outState.PutString(BUNDLEID_LINE, GetLineSpinnerValue());
            outState.PutString(BUNDLEID_STATUS, GetStatusSpinnerValue());
        }

        #endregion
    }
}