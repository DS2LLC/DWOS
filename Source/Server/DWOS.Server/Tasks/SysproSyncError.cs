using System.Xml.Serialization;

namespace DWOS.Server.Tasks
{
    /// <summary>
    /// Represents the contents of a SYSPRO integration error file.
    /// </summary>
    [XmlRoot("Error")]
    public class SysproSyncError
    {
        #region Methods

        /// <summary>
        /// Gets or sets the transmission reference for this instance.
        /// </summary>
        public string TransmissionReference { get; set; }

        /// <summary>
        /// Gets or sets the error message for this instance.
        /// </summary>
        public string ErrorMessage { get; set; }

        #endregion
    }
}