using System;
using System.Windows.Data;

namespace DWOS.UI.Utilities.Convertors
{
    /// <summary>
    /// Safely converts between <see cref="string"/> and different numerical value types.
    /// </summary>
    public sealed class DefaultNumericConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object returnValue;
            if (value == null)
            {
                returnValue = string.Empty;
            }
            else
            {
                returnValue = value.ToString();
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object returnValue = null;

            if (value != null)
            {
                if (targetType == typeof(double))
                {
                    double doubleValue;

                    if (double.TryParse(value.ToString(), out doubleValue))
                    {
                        returnValue = doubleValue;
                    }
                    else
                    {
                        returnValue = default(double);
                    }
                }
                else if (targetType == typeof(float))
                {
                    float floatValue;

                    if (float.TryParse(value.ToString(), out floatValue))
                    {
                        returnValue = floatValue;
                    }
                    else
                    {
                        returnValue = default(float);
                    }
                }
                else if (targetType == typeof(byte))
                {
                    byte byteValue;

                    if (byte.TryParse(value.ToString(), out byteValue))
                    {
                        returnValue = byteValue;
                    }
                    else
                    {
                        returnValue = default(byte);
                    }
                }
                else if (targetType == typeof(char))
                {
                    char charValue;

                    if (char.TryParse(value.ToString(), out charValue))
                    {
                        returnValue = charValue;
                    }
                    else
                    {
                        returnValue = default(char);
                    }
                }
                else if (targetType == typeof(short))
                {
                    short shortValue;

                    if (short.TryParse(value.ToString(), out shortValue))
                    {
                        returnValue = shortValue;
                    }
                    else
                    {
                        returnValue = default(short);
                    }
                }
                else if (targetType == typeof(int))
                {
                    int intValue;

                    if (int.TryParse(value.ToString(), out intValue))
                    {
                        returnValue = intValue;
                    }
                    else
                    {
                        returnValue = default(int);
                    }
                }
                else if (targetType == typeof(long))
                {
                    long longValue;

                    if (long.TryParse(value.ToString(), out longValue))
                    {
                        returnValue = longValue;
                    }
                    else
                    {
                        returnValue = default(long);
                    }
                }
                else if (targetType == typeof(decimal))
                {
                    decimal decimalValue;

                    if (decimal.TryParse(value.ToString(), out decimalValue))
                    {
                        returnValue = decimalValue;
                    }
                    else
                    {
                        returnValue = default(decimal);
                    }
                }
                else if (targetType == typeof(sbyte))
                {
                    sbyte sbyteValue;

                    if (sbyte.TryParse(value.ToString(), out sbyteValue))
                    {
                        returnValue = sbyteValue;
                    }
                    else
                    {
                        returnValue = default(sbyte);
                    }
                }
                else if (targetType == typeof(ushort))
                {
                    ushort ushortValue;

                    if (ushort.TryParse(value.ToString(), out ushortValue))
                    {
                        returnValue = ushortValue;
                    }
                    else
                    {
                        returnValue = default(ushort);
                    }
                }
                else if (targetType == typeof(uint))
                {
                    uint uintValue;

                    if (uint.TryParse(value.ToString(), out uintValue))
                    {
                        returnValue = uintValue;
                    }
                    else
                    {
                        returnValue = default(uint);
                    }
                }
                else if (targetType == typeof(ulong))
                {
                    ulong ulongValue;

                    if (ulong.TryParse(value.ToString(), out ulongValue))
                    {
                        returnValue = ulongValue;
                    }
                    else
                    {
                        returnValue = default(ulong);
                    }
                }
            }

            return returnValue;
        }

        #endregion
    }
}
