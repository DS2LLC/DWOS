using System;

namespace DWOS.Android
{
    /// <summary>
    /// Callback interface for <see cref="DatePickerFragment"/>.
    /// </summary>
    public interface IDatePickerFragmentCallback
    {
        /// <summary>
        /// Called after the user accepts the time picker dialog.
        /// </summary>
        /// <param name="hourOfDay"></param>
        /// <param name="minute"></param>
        /// <param name="tag"></param>
        void OnTimeCallback(int hourOfDay, int minute, string tag);

        /// <summary>
        /// Called after the user accepts the date picker dialog.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="monthOfYear"></param>
        /// <param name="dayOfMonth"></param>
        /// <param name="tag"></param>
        void OnDateCallback(int year, int monthOfYear, int dayOfMonth, string tag);

        /// <summary>
        /// Called after the user accepts the date/time picker dialog.
        /// </summary>
        void OnDateTimeCallback(DateTime dateTime, string tag);
    }

}