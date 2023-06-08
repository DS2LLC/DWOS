using System;

namespace DWOS.Shared.Data
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class DataColumnAttribute : Attribute
    {
        #region Properties

        public string FieldName { get; set; }

        public Type FieldConverterType { get; set; }

        public object DefaultValue { get; set; }

        #endregion

        #region Methods

        public DataColumnAttribute()
        {
        }

        public DataColumnAttribute(string fieldName)
        {
            this.FieldName = fieldName;
        }

        #endregion
    }
}