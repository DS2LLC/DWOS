using System;
using System.Windows.Forms;
using NLog;

namespace DWOS.Shared
{
    /// <summary>
    /// Contains a utility method to show a message box for an exception.
    /// </summary>
    public static class ErrorMessageBox
    {
        #region Fields
        
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets a value indicating whether to prevent UI interaction.
        /// </summary>
        /// <remarks>
        /// This is helpful if the client cannot display a interface (windows service)
        /// </remarks>
        /// <value>
        /// <c>true</c> if [prevent UI interaction]; otherwise, <c>false</c>.
        /// </value>
        public static bool PreventUIInteraction { get; set; }

        #endregion

        #region Methods
        
        /// <summary>
        /// Log the error then display message dialog of error to the end user.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="exc">The exc.</param>
        public static void ShowDialog(string msg, Exception exc, bool logError = true)
        {
            string fullMessage  = exc == null ? msg : msg + Environment.NewLine + Environment.NewLine + exc.Message;
            Exception fullExc   = exc == null ? new Exception(msg) : new Exception(msg, exc);

            if(logError)
                _log.Error(exc, fullMessage);
            
            if(!PreventUIInteraction)
            {
                using (var ed = new ErrorDialog(fullExc))
                    ed.ShowDialog(Form.ActiveForm);
            }
        }
        
        #endregion
    }
}