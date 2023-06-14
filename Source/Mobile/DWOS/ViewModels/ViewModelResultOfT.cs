namespace DWOS.ViewModels
{
    /// <summary>
    /// <see cref="ViewModelResult"/> that represents the result of a View
    /// Model Command/Action with a generic Result property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ViewModelResult<T> : ViewModelResult
    {
        #region Properties

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public T Result { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Prevents a default instance of the <see cref="ViewModelResult{T}"/> class from being created.
        /// </summary>
        private ViewModelResult()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelResult{T}"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="success">if set to <c>true</c> [success].</param>
        /// <param name="errorMessage">The error message.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="result"/> is null.
        /// </exception>
        public ViewModelResult(T result, bool success, string errorMessage)
            : base(success, errorMessage)
        {
            Result = result;
        }

        #endregion
    }
}
