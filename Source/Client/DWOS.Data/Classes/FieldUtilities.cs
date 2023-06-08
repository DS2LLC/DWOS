using System.Linq;

namespace DWOS.Data
{
    /// <summary>
    /// Provides various utility methods for getting system field information.
    /// </summary>
    public static class FieldUtilities
    {
        /// <summary>
        /// Retrieves a value that indicates if a field is in-use.
        /// </summary>
        /// <param name="category">The field's category.</param>
        /// <param name="fieldName">The field's name.</param>
        /// <returns>
        /// <c>true</c> if the field is in-use; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFieldEnabled(string category, string fieldName)
        {
            using (var ta = new Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
            {
                var productClassField = ta.GetByCategory(category).FirstOrDefault(f => f.Name == fieldName);
                return productClassField == null || productClassField.IsRequired || productClassField.IsVisible;
            }
        }

        /// <summary>
        /// Retrieves a <see cref="SystemFieldInfo"/> instance that represents a field.
        /// </summary>
        /// <param name="category">The field's category.</param>
        /// <param name="fieldName">The field's name.</param>
        /// <returns>
        /// An <see cref="SystemFieldInfo"/> instance; returns default
        /// information if the field was not found.
        /// </returns>
        public static SystemFieldInfo GetField(string category, string fieldName)
        {
            using (var ta = new Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
            {
                var fieldRow = ta.GetByCategory(category).FirstOrDefault(f => f.Name == fieldName);

                if (fieldRow == null)
                {
                    return new SystemFieldInfo
                    {
                        IsRequired = false,
                        IsVisible = true
                    };
                }

                return new SystemFieldInfo
                {
                    IsRequired = fieldRow.IsRequired,
                    IsVisible = fieldRow.IsVisible
                };
            }
        }
    }
}
