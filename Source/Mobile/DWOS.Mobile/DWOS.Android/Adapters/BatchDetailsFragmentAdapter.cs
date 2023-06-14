using Android.App;
using Android.Support.V13.App;
using JavaString = Java.Lang.String;

namespace DWOS.Android
{
    /// <summary>
    /// <see cref="FragmentPagerAdapter"/> that shows batch details.
    /// </summary>
    public class BatchDetailsFragmentAdapter : FragmentPagerAdapter
    {
        #region Fields

        const int _numberOfDetailTabs = 2;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchDetailsFragmentAdapter"/> class.
        /// </summary>
        /// <param name="fragmentManager">The fragment manager.</param>
        public BatchDetailsFragmentAdapter(FragmentManager fragmentManager)
            : base(fragmentManager)
        {

        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return new BatchDetailsFragment();
                case 1:
                    return new BatchProcessDetailsFragment();
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public override int Count
        {
            get { return _numberOfDetailTabs; }
        }

        /// <summary>
        /// Gets the page title formatted.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            switch (position)
            {
                case 0:
                    return new JavaString("BATCH");
                case 1:
                    return new JavaString("PROCESS");
                default:
                    return null;
            }
        }

        #endregion
    }
}