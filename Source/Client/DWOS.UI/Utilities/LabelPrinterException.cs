using System;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// The exception that is thrown when the label printer fails to print.
    /// </summary>
    /// <remarks>
    /// Acts as a wrapper for Neodynamic SDK exceptions.
    /// </remarks>
    [Serializable]
    public sealed class LabelPrinterException : Exception
    {
        public LabelPrinterException()
        {
        }

        public LabelPrinterException(string message)
            : base(message)
        {
        }

        public LabelPrinterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private LabelPrinterException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
