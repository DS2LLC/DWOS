namespace DWOS.Data.Datasets
{
    /// <summary>
    /// Defines utility methods for <see cref="OrderType"/>.
    /// </summary>
    public static class OrderTypeUtilities
    {
        /// <summary>
        /// Converts an <see cref="OrderType"/> instance to a display string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDisplayString(this OrderType value)
        {
            switch(value)
            {
                case OrderType.ReworkExt:
                    return "Rework External";
                case OrderType.ReworkInt:
                    return "Rework Internal";
                case OrderType.ReworkHold:
                    return "Rework Hold";
                case OrderType.Lost:
                case OrderType.Quarantine:
                case OrderType.Normal:
                default:
                    return value.ToString();
            }
        }
    }
}
