using System;
using System.Runtime.Serialization;

namespace DWOS.QBExport
{
    /// <summary>
    /// The exception that is thrown when DWOS cannot connect to QuickBooks.
    /// </summary>
    [Serializable]
    public class ExportConnectionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportConnectionException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public ExportConnectionException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportConnectionException"/> class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ExportConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
