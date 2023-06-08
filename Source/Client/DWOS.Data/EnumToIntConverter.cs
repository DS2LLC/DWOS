using DWOS.Shared.Data;
using System;

namespace DWOS.Data
{
    /// <summary>
    /// <see cref="IFieldConverter"/> implementation that converts enumerated
    /// values to integers and back again.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumToIntConverter<T> : IFieldConverter where T : struct
    {
        #region Methods

        static EnumToIntConverter()
        {
            // Ensure that T is an enumerated type.
            if (typeof(T).BaseType != typeof(Enum))
            {
                NLog.LogManager.GetCurrentClassLogger().Error("EnumToIntConverter is being used with a non-enumeration type.");
            }
        }

        #endregion

        #region Implementation of IFieldConverter

        public object ConvertFromField(object value)
        {
            int valueAsInt;
            if (value == null || !int.TryParse(value.ToString(), out valueAsInt))
            {
                valueAsInt = 0;
            }

            return Enum.ToObject(typeof(T), valueAsInt);
        }

        public object ConvertToField(object value)
        {
            return Convert.ToInt32(value).ToString();
        }

        #endregion
    }
}
