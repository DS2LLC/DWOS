using DWOS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.UI.Utilities.Scale
{
    /// <summary>
    /// Provides a factory method to create new <see cref="IScale"/> instances.
    /// </summary>
    public static class ScaleFactory
    {
        /// <summary>
        /// Returns a new <see cref="IScale"/> instance.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="IScale"/> from user options (or null
        /// if scale functionality has been disabled.)
        /// </returns>
        public static IScale NewScale()
        {
            IScale instance;
            switch (UserSettings.Default.ScaleType)
            {
                case ScaleType.Sterling:
                    instance = new SterlingScale(UserSettings.Default.ScalePortName);
                    break;
                case ScaleType.None:
                default:
                    instance = null;
                    break;
            }

            return instance;
        }
    }
}
