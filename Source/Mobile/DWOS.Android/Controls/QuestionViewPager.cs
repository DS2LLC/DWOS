using System;
using Android.Content;
using Android.Views;
using Android.Support.V4.View;
using Android.Util;

namespace DWOS.Android
{
    /// <summary>
    /// <see cref="QuestionViewPager"/> is a custom implementation of a View Pager
    /// that disables swiping and resizes appropriately to a Question Fragment
    /// </summary>
    public class QuestionViewPager : ViewPager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionViewPager"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public QuestionViewPager(Context context)
            : base(context)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionViewPager"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="attributeSet">The attribute set.</param>
        public QuestionViewPager(Context context, IAttributeSet attributeSet)
            : base(context, attributeSet)
        {

        }

        /// <summary>
        /// Implement this method to intercept all touch screen motion events.
        /// </summary>
        /// <param name="ev">The motion event being dispatched down the hierarchy.</param>
        /// <returns>
        /// To be added.
        /// </returns>
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            // Never allow swiping to switch between pages
            return false;
        }

        /// <summary>
        /// Implement this method to handle touch screen motion events.
        /// </summary>
        /// <param name="e">The motion event.</param>
        /// <returns>
        /// To be added
        /// </returns>
        public override bool OnTouchEvent(MotionEvent e)
        {
            // Never allow swiping to switch between pages
            return false;
        }

        /// <summary>
        /// Measures the height of a child view based on Measure Spec Modes.
        /// </summary>
        /// <param name="measureSpec">The measure spec.</param>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        private int MeasureHeight(int measureSpec, View view)
        {
            int result = 0;

            var specMode = MeasureSpec.GetMode(measureSpec);
            int specSize = MeasureSpec.GetSize(measureSpec);

            if (specMode == MeasureSpecMode.Exactly)
                result = specSize;
            else
            {
                if (view != null)
                    result = view.MeasuredHeight;
                if (specMode == MeasureSpecMode.AtMost)
                    result = Math.Min(result, specSize);
            }

            return result;
        }
    }
}