using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.UI.Utilities.Scale
{
    /// <summary>
    /// Provides data for the ScaleDataReceived event.
    /// </summary>
    public sealed class ScaleDataReceivedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets data received from the scale.
        /// </summary>
        public ScaleData Data
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the ScaleDataReceivedEventArgs class.
        /// </summary>
        /// <param name="data"></param>
        public ScaleDataReceivedEventArgs(ScaleData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            Data = data;
        }

        #endregion
    }
}
