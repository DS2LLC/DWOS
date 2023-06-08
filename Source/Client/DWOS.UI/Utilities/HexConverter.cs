using System;

namespace DWOS.UI.Utilities
{
    public static class HexConverter
    {
        public static string ConvertCode(string code)
        {
            var intValue    = int.Parse(code, System.Globalization.NumberStyles.HexNumber);
            var charValue   = Convert.ToChar(intValue);
            return charValue.ToString();
        }

        public static string ConvertCodes(params string[] codes)
        {
            var value = "";
            codes.ForEach(c => value += ConvertCode(c));
            
            return value;
        }
    }
}
