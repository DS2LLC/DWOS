using Android.Content;
using Android.Util;
using Android.Widget;
using System;

namespace DWOS.Android.Controls
{
    /// <summary>
    /// Spinner implementation with DWOS-specific functionality.
    /// </summary>
    public class DWOSSpinner : Spinner
    {
        #region Fields

        /// <summary>
        /// Occurs when the user selects the same item more than once in
        /// a row.
        /// </summary>
        public event EventHandler<ItemSelectedEventArgs> SameItemSelected;

        #endregion

        #region Methods

        /// <summary>
        /// Construct a new spinner with the given context's theme.
        /// </summary>
        /// <param name="context"></param>
        public DWOSSpinner(Context context)
            : base(context)
        {

        }

        /// <summary>
        /// Construct a new spinner with the given context's theme and the
        /// supplied attribute set.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attrs"></param>
        public DWOSSpinner(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        /// <summary>
        /// Construct a new spinner with the given context's theme, the
        /// supplied attribute set, and default style attribute.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attrs"></param>
        /// <param name="defStyleAttr"></param>
        public DWOSSpinner(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {

        }

        /// <summary>
        /// Sets the currently selected item.
        /// </summary>
        /// <remarks>
        /// If the same item is selected twice, fires <see cref="SameItemSelected"/>.
        /// </remarks>
        /// <param name="position"></param>
        public override void SetSelection(int position)
        {
            var positionUnchanged = SelectedItemPosition == position;
            base.SetSelection(position);

            if (positionUnchanged)
            {
                var args = new ItemSelectedEventArgs(this, SelectedView, position, SelectedItemId);
                SameItemSelected?.Invoke(this, args);
            }
        }

        /// <summary>
        /// Jump directly to a specific item in the adapter data.
        /// </summary>
        /// <remarks>
        /// If the same item is selected twice, fires <see cref="SameItemSelected"/>.
        /// </remarks>
        /// <param name="position"></param>
        /// <param name="animate"></param>
        public override void SetSelection(int position, bool animate)
        {
            var positionUnchanged = SelectedItemPosition == position;
            base.SetSelection(position, animate);

            if (positionUnchanged)
            {
                var args = new ItemSelectedEventArgs(this, SelectedView, position, SelectedItemId);
                SameItemSelected?.Invoke(this, args);
            }
        }

        #endregion
    }
}