using System;
using System.Collections.Generic;
using Android.App;
using Android.Support.V13.App;
using JavaString = Java.Lang.String;
using DWOS.ViewModels;

namespace DWOS.Android
{
    /// <summary>
    /// <see cref="FragmentStatePagerAdapter"/> that shows inspection questions.
    /// </summary>
    public class InspectionQuestionFragmentAdapter : FragmentStatePagerAdapter
    {
        #region Properties

        /// <summary>
        /// Gets the questions for this instance.
        /// </summary>
        public IList<InspectionQuestionViewModel> Questions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the current mode for this instance.
        /// </summary>
        public Mode CurrentMode { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionQuestionFragmentAdapter"/> class.
        /// </summary>
        /// <param name="questions">The questions.</param>
        /// <param name="fragmentManager">The fragment manager.</param>
        /// <exception cref="System.ArgumentNullException">questions</exception>
        public InspectionQuestionFragmentAdapter(IList<InspectionQuestionViewModel> questions,  Mode currentMode, FragmentManager fragmentManager)
            : base(fragmentManager)
        {
            if (questions == null)
                throw new ArgumentNullException("questions");

            Questions = questions;
            CurrentMode = currentMode;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override Fragment GetItem(int position)
        {
            var fragment = new InspectionQuestionFragment { Question = Questions[position], CurrentMode = CurrentMode };
            return fragment;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public override int Count
        {
            get { return Questions.Count; }
        }

        /// <summary>
        /// Gets the page title formatted.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new JavaString(string.Format("{0} {1}", Questions[position].StepOrder, Questions[position].Name.ToUpper()));
        }

        #endregion
    }
}