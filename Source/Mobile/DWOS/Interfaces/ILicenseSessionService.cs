namespace DWOS
{
    /// <summary>
    /// Defines the interface for Services that may provide Session States
    /// with the DWOS Server.
    /// </summary>
    public interface ILicenseSessionService
    {
        /// <summary>
        /// Signals to keep the session alive.
        /// </summary>
        void KeepAlive();

        /// <summary>
        /// Signals to end the service session.
        /// </summary>
        void Stop();
    }
}
