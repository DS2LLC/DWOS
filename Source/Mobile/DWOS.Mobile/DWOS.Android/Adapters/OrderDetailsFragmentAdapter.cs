using Android.App;
using Android.Support.V13.App;
using JavaString = Java.Lang.String;

namespace DWOS.Android
{
    /// <summary>
    /// <see cref="FragmentPagerAdapter"/> implementation for Order Details.
    /// </summary>
    public class OrderDetailsFragmentAdapter : FragmentPagerAdapter
    {
        #region Fields

        const int NUMBER_OF_DETAIL_TABS = 4;
        private const int POSITION_ORDER = 0;
        private const int POSITION_PROCESS = 1;
        private const int POSITION_PART = 2;
        private const int POSITION_NOTES = 3;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDetailsFragmentAdapter"/> class.
        /// </summary>
        /// <param name="fragmentManager">The fragment manager.</param>
        public OrderDetailsFragmentAdapter(FragmentManager fragmentManager)
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
                case POSITION_ORDER:
                    return new OrderDetailsFragment();
                case POSITION_PROCESS:
                    return new ProcessDetailsFragment();
                case POSITION_PART:
                    return new PartDetailsFragment();
                case POSITION_NOTES:
                    return new OrderNotesFragment();
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
            get { return NUMBER_OF_DETAIL_TABS; }
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
                case POSITION_ORDER:
                    return new JavaString("ORDER");
                case POSITION_PROCESS:
                    return new JavaString("PROCESS");
                case POSITION_PART:
                    return new JavaString("PART");
                case POSITION_NOTES:
                    return new JavaString("NOTES");
                default:
                    return null;
            }
        }
    }
}