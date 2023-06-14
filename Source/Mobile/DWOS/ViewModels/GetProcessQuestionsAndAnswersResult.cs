using System;
using System.Collections.Generic;

namespace DWOS.ViewModels
{
    /// <summary>
    /// Represents a result of requesting a specific answers to process
    /// questions for an order.
    /// </summary>
    public class GetProcessQuestionsAndAnswersResult : ViewModelResult
    {
        #region Properties

        /// <summary>
        /// Gets the questions and answers for this instance.
        /// </summary>
        public IList<ProcessQuestionViewModel> QuestionsAndAnswers { get; private set; }

        #endregion

        #region Methods

        private GetProcessQuestionsAndAnswersResult()
        {

        }


        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="GetProcessQuestionsAndAnswersResult"/> class.
        /// </summary>
        /// <param name="questionsAndAnswers"></param>
        /// <param name="success"></param>
        /// <param name="errorMessage"></param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="questionsAndAnswers"/> is null.
        /// </exception>
        public GetProcessQuestionsAndAnswersResult(IList<ProcessQuestionViewModel> questionsAndAnswers, bool success, string errorMessage)
            : base(success, errorMessage)
        {
            if (questionsAndAnswers == null)
                throw new ArgumentNullException("questionsAndAnswers");

            QuestionsAndAnswers = questionsAndAnswers;
        }

        #endregion
    }

}
