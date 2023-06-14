using DWOS.Shared.Utilities;
using Infragistics.Documents.Excel;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization;

namespace DWOS.DataImporter
{
    /// <summary>
    /// Provides helper methods for mapping from worksheets to the database.
    /// </summary>
    public static class FieldMapper
    {
        #region Methods

        public static string GetString(FieldMap fieldMap, WorksheetRow row)
        {
            var value = row.Cells[fieldMap.ColumnIndex].Value;

            if(value != null)
                return value.ToString();

            return null;
        }

        public static int? GetInt32(FieldMap fieldMap, WorksheetRow row)
        {
            var value = GetString(fieldMap, row);

            if (Int32.TryParse(value, out int result))
            {
                return result;
            }

            return null;
        }

        #endregion
    }

    /// <summary>
    /// Provides  functionality for mapping from worksheets to the database.
    /// </summary>
    /// <typeparam name="TDataRow"></typeparam>
    public abstract class FieldMapper<TDataRow> where TDataRow : DataRow
    {
        protected string GetString(FieldMap fieldMap, WorksheetRow row)
        {
            var value = row.Cells[fieldMap.ColumnIndex].Value;

            if(value != null)
                return value.ToString();

            return null;
        }

        protected int? GetInt(FieldMap fieldMap, WorksheetRow row)
        {
            var value = row.Cells[fieldMap.ColumnIndex].Value;

            if(value != null)
            {
                var number = 0;

                if(int.TryParse(value.ToString(), out number))
                    return number;
            }

            return null;
        }

        protected DateTime? GetDateTime(FieldMap fieldMap, WorksheetRow row)
        {
            var valueObj = row.Cells[fieldMap.ColumnIndex].Value;

            if (valueObj != null)
            {
                DateTime value;

                if (DateTime.TryParse(valueObj.ToString(), out value))
                    return value;
            }

            return null;
        }

        protected bool? GetBool(FieldMap fieldMap, WorksheetRow row)
        {
            var valueObj = row.Cells[fieldMap.ColumnIndex].Value;

            if (valueObj != null)
            {
                bool value;

                if (bool.TryParse(valueObj.ToString(), out value))
                    return value;
            }

            return null;
        }

        protected decimal? GetDecimal(FieldMap fieldMap, WorksheetRow row)
        {
            var valueObj = row.Cells[fieldMap.ColumnIndex].Value;

            if (valueObj != null)
            {
                decimal value;

                if (decimal.TryParse(valueObj.ToString(), out value))
                    return value;
            }

            return null;
        }

        protected float? GetFloat(FieldMap fieldMap, WorksheetRow row)
        {
            var valueObj = row.Cells[fieldMap.ColumnIndex].Value;

            if (valueObj != null)
            {
                float value;

                if (float.TryParse(valueObj.ToString(), out value))
                    return value;
            }

            return null;
        }

        protected double? GetDouble(FieldMap fieldMap, WorksheetRow row)
        {
            var valueObj = row.Cells[fieldMap.ColumnIndex].Value;

            if (valueObj != null)
            {
                double value;

                if (double.TryParse(valueObj.ToString(), out value))
                    return value;
            }

            return null;
        }

        public object GetValue(FieldMap fieldMap, WorksheetRow row, Type type, bool allowNull)
        {
            if(type == typeof(int))
            {
                var nullValue = GetInt(fieldMap, row);
                return nullValue.HasValue ? (object)nullValue.Value : (allowNull ? (object)DBNull.Value : 0);
            }
            if(type == typeof(DateTime))
            {
                var nullValue = GetDateTime(fieldMap, row);
                return nullValue.HasValue ? (object)nullValue.Value : (allowNull ? (object)DBNull.Value : DateTime.MinValue);
            }
            if (type == typeof(bool))
            {
                var nullValue = GetBool(fieldMap, row);
                return nullValue.HasValue ? (object)nullValue.Value : (allowNull ? (object)DBNull.Value: false);
            }
            if (type == typeof(decimal))
            {
                var nullValue = GetDecimal(fieldMap, row);
                return nullValue.HasValue ? (object)nullValue.Value : (allowNull ? (object)DBNull.Value : 0.0m);
            }
            if (type == typeof(float))
            {
                var nullValue = GetFloat(fieldMap, row);
                return nullValue.HasValue ? (object)nullValue.Value : (allowNull ? (object)DBNull.Value : 0.0f);
            }
            if (type == typeof(double))
            {
                var nullValue = GetDouble(fieldMap, row);
                return nullValue.HasValue ? (object)nullValue.Value : (allowNull ? (object)DBNull.Value : 0.0d);
            }

            return GetString(fieldMap, row);
        }

        public abstract void MapAll(IEnumerable<FieldMap> fieldMaps, WorksheetRow worksheetRow, TDataRow dbRow);
    }
}