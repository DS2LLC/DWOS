using System;

namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Implements utility methods related to <see cref="Version"/>.
    /// </summary>
    public static class VersionUtilities
    {
        /// <summary>
        /// Returns the smaller of two version numbers.
        /// </summary>
        /// <param name="val1">The first version number to compare.</param>
        /// <param name="val2">The second version number to compare.</param>
        /// <returns>
        /// Parameter <paramref name="val1"/> or <paramref name="val2"/>,
        /// whichever is smaller.
        /// </returns>
        public static Version Min(Version val1, Version val2)
        {
            if (val1 == null || val2 == null)
            {
                return null;
            }

            return val1 > val2 ? val2 : val1;
        }
    }
}
