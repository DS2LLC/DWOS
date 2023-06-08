using System;
using System.Data;

namespace DWOS.Data
{
    /// <summary>
    /// Defines extension methods for <see cref="DataTable"/> instances.
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// Gets an array of all DataRow objects that match the filter criteria.
        /// </summary>
        /// <param name="table">Table instance to select rows from.</param>
        /// <param name="filterExpression">Filter expression to format.</param>
        /// <param name="parameters">Parameters to use when formatting filterExpression.</param>
        /// <returns></returns>
        public static DataRow[] FormattedSelect(this DataTable table, string filterExpression, params object[] parameters)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table", "table cannot be null.");
            }
            else if (parameters == null || parameters.Length == 0)
            {
                return table.Select(filterExpression);
            }

            object[] paramsToUse = new object[parameters.Length];
            for (int x = 0; x < paramsToUse.Length; x++)
            {
                string paramStr = parameters[x] as string;

                if (string.IsNullOrEmpty(paramStr))
                {
                    paramsToUse[x] = parameters[x];
                }
                else
                {
                    paramsToUse[x] = paramStr.Replace("'", "''");
                }
            }

            string newFilterExpression = string.Format(filterExpression, paramsToUse);
            return table.Select(newFilterExpression);
        }
    }
}
