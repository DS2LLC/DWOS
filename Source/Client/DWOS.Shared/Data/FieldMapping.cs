using System.Reflection;

namespace DWOS.Shared.Data
{
    /// <summary>
    /// Represents a field mapping for custom serialization.
    /// </summary>
    public class FieldMapping
    {
        #region Properties

        /// <summary>
        /// Gets or sets the property for this instance.
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataColumnAttribute"/> instance for this instance.
        /// </summary>
        public DataColumnAttribute DataMember { get; set; }

        /// <summary>
        /// Gets the converter for this instance.
        /// </summary>
        public IFieldConverter Converter { get; }

        #endregion

        #region Methods

        public FieldMapping(PropertyInfo property, DataColumnAttribute dataMember)
        {
            Property = property;
            DataMember = dataMember;

            if (DataMember != null && DataMember.FieldConverterType != null)
                Converter = DataMember.FieldConverterType.Assembly.CreateInstance(DataMember.FieldConverterType.FullName) as IFieldConverter;
        }

        #endregion
    }
}
