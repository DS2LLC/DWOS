using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V13.App;
using DWOS.ViewModels;
using JavaString = Java.Lang.String;

namespace DWOS.Android
{
    public class QuestionFragmentAdapter: FragmentStatePagerAdapter
    {
        bool _isReadOnly;

        #region Properties
        public IList<ProcessQuestionViewModel> Questions
        {
            get;
            private set;
        } 
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionFragmentAdapter"/> class.
        /// </summary>
        /// <param name="questions">The questions.</param>
        /// <param name="fragmentManager">The fragment manager.</param>
        /// <param name="isReadOnly">if set to <c>true</c> questions are read only.</param>
        /// <exception cref="System.ArgumentNullException">questions</exception>
        public QuestionFragmentAdapter(IList<ProcessQuestionViewModel> questions, FragmentManager fragmentManager, bool isReadOnly)
            : base(fragmentManager)
        {
            if (questions == null)
                throw new ArgumentNullException("questions");

            Questions = questions;
            _isReadOnly = isReadOnly;
        }
        public override Fragment GetItem(int position)
        {
            var fragment = new OrderProcessQuestionFragment { Question = Questions[position], IsReadOnly = _isReadOnly };
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
    }
}