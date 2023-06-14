using System;
using System.Collections.Generic;

namespace DWOS.ViewModels
{
    /// <summary>
    /// Represents a result of requesting a specific step to be set as
    /// current for an order process.
    /// </summary>
    public class SetNextStepResult : ViewModelResult
    {
        #region Properties

        /// <summary>
        /// Gets the questions and answers for this instance.
        /// </summary>
        public IList<ProcessQuestionViewModel> StepQuestionsAndAnswers { get; private set; }

        #endregion

        #region Methods

        private SetNextStepResult()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetNextStepResult"/>.
        /// </summary>
        /// <param name="questionsAndAnswers"></param>
        /// <param name="success"></param>
        /// <param name="errorMessage"></param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="questionsAndAnswers"/> is null.
        /// </exception>
        public SetNextStepResult(IList<ProcessQuestionViewModel> questionsAndAnswers, bool success, string errorMessage)
            : base(success, errorMessage)
        {
            if (questionsAndAnswers == null)
                throw new ArgumentNullException("questionsAndAnswers");

            StepQuestionsAndAnswers = questionsAndAnswers;
        }

        #endregion
    }


}
