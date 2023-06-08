using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWOS.UI.Utilities.Scale
{
    /// <summary>
    /// Defines functionality for classes implementing scale integration.
    /// </summary>
    /// <remarks>
    /// This interface inherits from <see cref="IDisposable"/> because
    /// resources may need to be freed after use.
    /// </remarks>
    public interface IScale : IDisposable
    {
        /// <summary>
        /// Occurs when receiving data from the scale.
        /// </summary>
        /// <remarks>
        /// This event typically occurs after sending a 'Print' command
        /// to the scale.
        /// </remarks>
        event EventHandler<ScaleDataReceivedEventArgs> ScaleDataReceived;

        /// <summary>
        /// Gets a value indicating if this scale is open.
        /// </summary>
        /// <value>
        /// <c>true</c> if the scale is open; otherwise, <c>false</c>.
        /// </value>
        bool IsOpen { get; }

        /// <summary>
        /// Gets the name of the scale's port.
        /// </summary>
        string PortName { get; }

        /// <summary>
        /// Opens a new connection to the scale.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the current connection to the scale.
        /// </summary>
        void Close();

        /// <summary>
        /// Sends a 'Zero' command to the scale.
        /// </summary>
        void Zero();

        /// <summary>
        /// Sends a 'Tare' command to the scale.
        /// </summary>
        void Tare();

        /// <summary>
        /// Sends a 'Print' command to the scale.
        /// </summary>
        void Print();
    }
}
