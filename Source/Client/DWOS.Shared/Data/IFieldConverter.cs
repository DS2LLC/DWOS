namespace DWOS.Shared.Data
{
    /// <summary>
    /// Interface used for classes implementing custom serialization.
    /// </summary>
    public interface IFieldConverter
    {
        /// <summary>
        /// Converts a value from the data store.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        object ConvertFromField(object value);

        /// <summary>
        /// Converts a value to be stored in the data store.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        object ConvertToField(object value);
    }
}