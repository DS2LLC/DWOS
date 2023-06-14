using System;
using System.Collections.Generic;
using Android.App;
using Android.Support.V13.App;
using DWOS.ViewModels;
using JavaString = Java.Lang.String;

namespace DWOS.Android
{
    /// <summary>
    /// <see cref="FragmentStatePagerAdapter"/> that shows process questions.
    /// </summary>
    public class ProcessQuestionFragmentAdapter: FragmentStatePagerAdapter
    {
        #region Fields

        bool _isReadOnly;
        Mode _currentMode;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of process questions for this instance.
        /// </summary>
        public IList<ProcessQuestionViewModel> Questions
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessQuestionFragmentAdapter"/> class.
        /// </summary>
        /// <param name="questions">The questions.</param>
        /// <param name="fragmentManager">The fragment manager.</param>
        /// <param name="isReadOnly">if set to <c>true</c> questions are read only.</param>
        /// <exception cref="System.ArgumentNullException">questions</exception>
        public ProcessQuestionFragmentAdapter(IList<ProcessQuestionViewModel> questions, FragmentManager fragmentManager, bool isReadOnly, Mode currentMode)
            : base(fragmentManager)
        {
            if (questions == null)
                throw new ArgumentNullException("questions");

            Questions = questions;
            _isReadOnly = isReadOnly;
            _currentMode = currentMode;
        }

        public override Fragment GetItem(int position)
        {
            var fragment = new OrderProcessQuestionFragment 
            { 
                Question = Questions[position], 
                IsReadOnly = _isReadOnly, 
                Position = position,
                CurrentMode = _currentMode
            };
            return fragment;
        }

        public override int Count
        {
            get { return Questions.Count; }
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new JavaString(string.Format("{0} {1}", Questions[position].StepOrder, Questions[position].Name.ToUpper()));
        }

        #endregion
    }
}