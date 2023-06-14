using DWOS.Services.Messages;
using System;
using System.Collections.Generic;

namespace DWOS.ViewModels
{
    /// <summary>
    /// <see cref="ViewModelResult"/> that represents the result of a View
    /// Model Command/Action.
    /// </summary>
    public class ViewModelResult
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating if the view model successfully
        /// completed the action.
        /// </summary>
        /// <value>
        /// <c>true</c> if the action was successfully completed;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; private set; }

        /// <summary>
        /// Gets the error message of this instance.
        /// </summary>
        public string ErrorMessage { get; private set; }
        #endregion

        #region Methods

        protected ViewModelResult()
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ViewModelResult"/> class.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="errorMessage"></param>
        public ViewModelResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        #endregion
    }
}
