using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using Newtonsoft.Json;
using System;

namespace DWOS.Android
{
    /// <summary>
    /// Shows a date/time picker in a dialog window.
    /// </summary>
    public class DatePickerFragment : DialogFragment, IDialogInterfaceOnClickListener
    {
        #region Fields

        public const string DATETIMEPICKER_FRAGMENT_TAG = "CheckInFragment";
        
        DateOrTime _presentDateOrTime;
        DateTime _dateTime;
        const string DATEPICKER_ID = "DatePickerTime";
        private const string BUNDLE_DATE_OR_TIME = "DATE_OR_TIME";
        private const string BUNDLE_TAG = "TAG";
        private const string BUNDLE_DATE_TIME = "DATE_TIME";
        string _tag;

        #endregion

        #region Methods

        /// <summary>
        /// Returns a new instance of <see cref="DatePickerFragment"/>
        /// </summary>
        /// <param name="dateOrTime">Setting for what picker to show.</param>
        /// <param name="tag">Fragment tag</param>
        /// <param name="dateTime">Default DateTime to show.</param>
        /// <returns></returns>
        public static DatePickerFragment New(DateOrTime dateOrTime, string tag, DateTime? dateTime)
        {
            var fragment = new DatePickerFragment();
            var bundle = new Bundle();

            bundle.PutString(BUNDLE_DATE_OR_TIME, JsonConvert.SerializeObject(dateOrTime));
            bundle.PutString(BUNDLE_TAG, tag);
            bundle.PutString(BUNDLE_DATE_TIME, JsonConvert.SerializeObject(dateTime));
            fragment.Arguments = bundle;

            return fragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            if (Arguments != null)
            {
                _presentDateOrTime =  JsonConvert.DeserializeObject<DateOrTime>(Arguments.GetString(BUNDLE_DATE_OR_TIME));
                _tag = Arguments.GetString(BUNDLE_TAG);

                var argumentDateTime = JsonConvert.DeserializeObject<DateTime?>(Arguments.GetString(BUNDLE_DATE_TIME));
                _dateTime = argumentDateTime ?? DateTime.Now;
            }

            if (savedInstanceState != null)
            {
                var ticks = savedInstanceState.GetLong(DATEPICKER_ID, DateTime.Now.Ticks);
                _dateTime = new DateTime(ticks);
            }

            Dialog dialog = null;
            var builder = new AlertDialog.Builder(Activity);
            var inflater = Activity.LayoutInflater;

            if (_presentDateOrTime == DateOrTime.Date)
            {
                // Try to emulate native DatePickerDialog
                // Lollipop's default picker does not have a title, but that
                // may not be the case for previous Android versions.
                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    builder.SetTitle(Resource.String.SetDate);
                }

                dialog = builder
                    .SetView(inflater.Inflate(Resource.Layout.DateDialogLayout, null))
                    .SetPositiveButton(Resource.String.Done, this)
                    .Create();
            }
            else if (_presentDateOrTime == DateOrTime.Time)
            {
                // Try to emulate native TimePickerDialog
                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    builder.SetTitle(Resource.String.SetTime);
                }

                dialog = builder
                    .SetView(inflater.Inflate(Resource.Layout.TimeDialogLayout, null))
                    .SetPositiveButton(Resource.String.Done, this)
                    .Create();
            }
            else
            {
                // Show picker for date and time
                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    builder.SetTitle(Resource.String.SetDateTime);
                }

                dialog = builder
                    .SetView(inflater.Inflate(Resource.Layout.DateTimeDialogLayout, null))
                    .SetPositiveButton(Resource.String.Done, this)
                    .Create();

            }

            return dialog;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutLong(DATEPICKER_ID, _dateTime.Ticks);
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            var parent = ParentFragment as IDatePickerFragmentCallback;
            if (_presentDateOrTime == DateOrTime.Date)
            {
                using (var picker = Dialog.FindViewById<DatePicker>(Resource.Id.dialogDatePicker))
                {
                    if (picker == null)
                    {
                        Log.Error(typeof(DatePickerFragment).Name, "Could not find date picker control.");
                    }
                    else
                    {
                        int year = picker.Year;
                        int monthOfYear = picker.Month + 1;
                        int dayOfMonth = picker.DayOfMonth;

                        parent?.OnDateCallback(year, monthOfYear, dayOfMonth, _tag);
                    }
                }

            }
            else if (_presentDateOrTime == DateOrTime.Time)
            {
                using (var picker = Dialog.FindViewById<TimePicker>(Resource.Id.dialogTimePicker))
                {
                    if (picker == null)
                    {
                        Log.Error(typeof(DatePickerFragment).Name, "Could not find time picker control.");
                    }
                    else
                    {
                        int hour = picker.CompatHour();
                        int minute = picker.CompatMinute();

                        parent?.OnTimeCallback(hour, minute, _tag);
                    }
                }
            }
            else
            {
                var hasError = false;
                var year = 0;
                var monthOfYear = 0;
                var dayOfMonth = 0;
                var hour = 0;
                var minute = 0;

                using (var picker = Dialog.FindViewById<DatePicker>(Resource.Id.dialogDatePicker))
                {
                    if (picker == null)
                    {
                        Log.Error(typeof(DatePickerFragment).Name, "Could not find date picker control.");
                        hasError = true;
                    }
                    else
                    {
                        year = picker.Year;
                        monthOfYear = picker.Month + 1;
                        dayOfMonth = picker.DayOfMonth;
                    }
                }

                using (var picker = Dialog.FindViewById<TimePicker>(Resource.Id.dialogTimePicker))
                {
                    if (picker == null)
                    {
                        Log.Error(typeof(DatePickerFragment).Name, "Could not find time picker control.");
                        hasError = true;
                    }
                    else
                    {
                        hour = picker.CompatHour();
                        minute = picker.CompatMinute();
                    }
                }

                if (!hasError)
                {
                    var dateTime = new DateTime(year, monthOfYear, dayOfMonth, hour, minute, 0);
                    parent?.OnDateTimeCallback(dateTime, _tag);
                }
            }
        }

        #endregion

        #region DateOrTime

        public enum DateOrTime { Date, Time, DateAndTime }

        #endregion
    }
}