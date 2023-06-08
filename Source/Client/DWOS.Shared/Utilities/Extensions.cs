using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using NLog;

namespace System
{
    public static class ArrayExtensions
    {
        /// <summary>
        ///     Determines whether [contains] [the specified array].
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="value">The value.</param>
        /// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
        /// <returns>
        ///     <c>true</c> if [contains] [the specified array]; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this string[] array, string value, bool caseSensitive)
        {
            foreach(string item in array)
            {
                if(caseSensitive && item == value)
                    return true;
                if(!caseSensitive && item.ToLower() == value.ToLower())
                    return true;
            }

            return false;
        }
    }

    public static class StringExtensions
    {
        #region Common string extensions

        /// <summary>
        ///     Formats the value with the parameters using string.Format.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static string FormatWith(this string value, params object[] parameters) { return string.Format(value, parameters); }

        /// <summary>
        ///     Trims the text to a provided maximum length.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="maxLength">Maximum length.</param>
        /// <returns></returns>
        /// <remarks>
        ///     Proposed by Rene Schulte
        /// </remarks>
        public static string TrimToMaxLength(this string value, int maxLength) { return (value == null || value.Length <= maxLength ? value : value.Substring(0, maxLength)); }

        /// <summary>
        ///     Trims the text to a provided maximum length and adds a suffix if required.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="maxLength">Maximum length.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns></returns>
        /// <remarks>
        ///     Proposed by Rene Schulte
        /// </remarks>
        public static string TrimToMaxLength(this string value, int maxLength, string suffix)
        {
            return (value == null || value.Length <= maxLength ? value : string.Concat(value.Substring(0, maxLength), suffix));
        }
        

        /// <summary>
        ///     Determines whether the comparison value string is contained within the input value string
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
        /// <param name="findTokens">The find tokens.</param>
        /// <returns>
        ///     <c>true</c> if input value contains the specified value, otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this string value, bool caseSensitive, params string[] findTokens)
        {
            if(String.IsNullOrEmpty(value))
                return false;

            string searchIn = caseSensitive ? value : value.ToLower();

            foreach(string item in findTokens)
            {
                if(caseSensitive && searchIn.Contains(item))
                    return true;
                if(searchIn.Contains(item.ToLower()))
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Loads the string into a XML DOM object (XmlDocument)
        /// </summary>
        /// <param name="xml">The XML string.</param>
        /// <returns>The XML document object model (XmlDocument)</returns>
        public static XmlDocument ToXmlDOM(this string xml)
        {
            var document = new XmlDocument();
            document.LoadXml(xml);
            return document;
        }

        /// <summary>
        ///     Loads the string into a XML XPath DOM (XPathDocument)
        /// </summary>
        /// <param name="xml">The XML string.</param>
        /// <returns>The XML XPath document object model (XPathNavigator)</returns>
        public static XPathNavigator ToXPath(this string xml)
        {
            var document = new XPathDocument(new StringReader(xml));
            return document.CreateNavigator();
        }

        /// <summary>
        ///     Reverses / mirrors a string.
        /// </summary>
        /// <param name="value">The string to be reversed.</param>
        /// <returns>The reversed string</returns>
        public static string Reverse(this string value)
        {
            if(String.IsNullOrWhiteSpace(value) || (value.Length == 1))
                return value;

            char[] chars = value.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// <summary>
        ///     Ensures that a string starts with a given prefix.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <param name="prefix">The prefix value to check for.</param>
        /// <returns>The string value including the prefix</returns>
        /// <example>
        ///     <code>
        /// 		var extension = "txt";
        /// 		var fileName = string.Concat(file.Name, extension.EnsureStartsWith("."));
        /// 	</code>
        /// </example>
        public static string EnsureStartsWith(this string value, string prefix) { return value.StartsWith(prefix) ? value : string.Concat(prefix, value); }

        /// <summary>
        ///     Ensures that a string ends with a given suffix.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <param name="suffix">The suffix value to check for.</param>
        /// <returns>The string value including the suffix</returns>
        /// <example>
        ///     <code>
        /// 		var url = "http://www.pgk.de";
        /// 		url = url.EnsureEndsWith("/"));
        /// 	</code>
        /// </example>
        public static string EnsureEndsWith(this string value, string suffix) { return value.EndsWith(suffix) ? value : string.Concat(value, suffix); }

        public static string RemoveFromEnd(this string value, params string[] toRemove)
        {
            foreach (string item in toRemove)
            {
                if (value.EndsWith(item))
                {
                    value = value.Substring(0, value.LastIndexOf(item));
                    break; //only allow one match at most
                }
            }

            return value;
        }

        /// <summary>
        ///     Tests whether the contents of a string is a numeric value
        /// </summary>
        /// <param name="value">String to check</param>
        /// <returns>
        ///     Boolean indicating whether or not the string contents are numeric
        /// </returns>
        /// <remarks>
        ///     Contributed by Kenneth Scott
        /// </remarks>
        public static bool IsNumeric(this string value)
        {
            float output;
            return float.TryParse(value, out output);
        }

        /// <summary>
        ///     Extracts all digits from a string.
        /// </summary>
        /// <param name="value">String containing digits to extract</param>
        /// <returns>
        ///     All digits contained within the input string
        /// </returns>
        /// <remarks>
        ///     Contributed by Kenneth Scott
        /// </remarks>
        public static string ExtractDigits(this string value) { return string.Join(null, Regex.Split(value, "[^\\d]")); }

        /// <summary>
        ///     Concatenates the specified string value with the passed additional strings.
        /// </summary>
        /// <param name="value">The original value.</param>
        /// <param name="values">The additional string values to be concatenated.</param>
        /// <returns>The concatenated string.</returns>
        public static string ConcatWith(this string value, params string[] values) { return string.Concat(value, string.Concat(values)); }

        /// <summary>
        ///     Convert the provided string to a Guid value.
        /// </summary>
        /// <param name="value">The original string value.</param>
        /// <returns>The Guid</returns>
        public static Guid ToGuid(this string value) { return new Guid(value); }

        /// <summary>
        ///     Gets the string before the given string parameter.
        /// </summary>
        /// <param name="value">The default value.</param>
        /// <param name="x">The given string parameter.</param>
        /// <returns></returns>
        public static string GetBefore(this string value, string x)
        {
            int xPos = value.IndexOf(x);
            return xPos == -1 ? String.Empty : value.Substring(0, xPos);
        }

        /// <summary>
        ///     Gets the string between the given string parameters.
        /// </summary>
        /// <param name="value">The default value.</param>
        /// <param name="x">The left string parameter.</param>
        /// <param name="y">The right string parameter</param>
        /// <returns></returns>
        public static string GetBetween(this string value, string x, string y)
        {
            int xPos = value.IndexOf(x);
            int yPos = value.LastIndexOf(y);

            if(xPos == -1 || xPos == -1)
                return String.Empty;

            int startIndex = xPos + x.Length;
            return startIndex >= yPos ? String.Empty : value.Substring(startIndex, yPos - startIndex).Trim();
        }

        /// <summary>
        ///     Gets the string after the given string parameter.
        /// </summary>
        /// <param name="value">The default value.</param>
        /// <param name="x">The given string parameter.</param>
        /// <returns></returns>
        public static string GetAfter(this string value, string x)
        {
            int xPos = value.LastIndexOf(x);

            if(xPos == -1)
                return String.Empty;

            int startIndex = xPos + x.Length;
            return startIndex >= value.Length ? String.Empty : value.Substring(startIndex).Trim();
        }

        /// <summary>
        ///     A generic version of System.String.Join()
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the array to join
        /// </typeparam>
        /// <param name="separator">
        ///     The separator to appear between each element
        /// </param>
        /// <param name="value">
        ///     An array of values
        /// </param>
        /// <returns>
        ///     The join.
        /// </returns>
        /// <remarks>
        ///     Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string Join<T>(string separator, T[] value)
        {
            if(value == null || value.Length == 0)
                return string.Empty;
            if(separator == null)
                separator = string.Empty;
            Converter <T, string> converter = o => o.ToString();
            return string.Join(separator, Array.ConvertAll(value, converter));
        }

        /// <summary>
        ///     Remove any instance of the given character from the current string.
        /// </summary>
        /// <param name="value">
        ///     The input.
        /// </param>
        /// <param name="removeCharc">
        ///     The remove char.
        /// </param>
        /// <remarks>
        ///     Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string Remove(this string value, params char[] removeCharc)
        {
            string result = value;
            if(!string.IsNullOrEmpty(result) && removeCharc != null)
                Array.ForEach(removeCharc, c => result = result.Remove(c.ToString()));

            return result;
        }

        public static string Remove(this string value, char removeChar)
        {
            var result = value;
            
            if (!string.IsNullOrEmpty(result))
               result = result.Remove(new char[]{removeChar});

            return result;
        }

        /// <summary>
        ///     Remove any instance of the given string pattern from the current string.
        /// </summary>
        /// <param name="value">The input.</param>
        /// <param name="strings">The strings.</param>
        /// <returns></returns>
        /// <remarks>
        ///     Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string Remove(this string value, params string[] strings)
        {
            return strings.Aggregate(value, (current, c) => current.Replace(c, string.Empty));
            //var result = value;
            //if (!string.IsNullOrEmpty(result) && removeStrings != null)
            //  Array.ForEach(removeStrings, s => result = result.Replace(s, string.Empty));

            //return result;
        }

        /// <summary>
        ///     Checks whether the string is null, empty or consists only of white-space characters and returns a default value in case
        /// </summary>
        /// <param name="value">The string to check</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>
        ///     Either the string or the default value
        /// </returns>
        public static string IfEmptyOrWhiteSpace(this string value, string defaultValue) { return (String.IsNullOrWhiteSpace(value) ? defaultValue : value); }

        /// <summary>
        ///     Uppercase First Letter
        /// </summary>
        /// <param name="value">The string value to process</param>
        /// <returns></returns>
        public static string ToUpperFirstLetter(this string value)
        {
            if(String.IsNullOrWhiteSpace(value))
                return string.Empty;

            char[] valueChars = value.ToCharArray();
            valueChars[0] = char.ToUpper(valueChars[0]);

            return new string(valueChars);
        }

        /// <summary>
        ///     Returns the left part of the string.
        /// </summary>
        /// <param name="value">The original string.</param>
        /// <param name="characterCount">The character count to be returned.</param>
        /// <returns>The left part</returns>
        public static string Left(this string value, int characterCount) { return value.Substring(0, characterCount); }

        /// <summary>
        ///     Returns the Right part of the string.
        /// </summary>
        /// <param name="value">The original string.</param>
        /// <param name="characterCount">The character count to be returned.</param>
        /// <returns>The right part</returns>
        public static string Right(this string value, int characterCount) { return value.Substring(value.Length - characterCount); }

        /// <summary>Returns the right part of the string from index.</summary>
        /// <param name="value">The original value.</param>
        /// <param name="index">The start index for substringing.</param>
        /// <returns>The right part.</returns>
        public static string SubstringFrom(this string value, int index) { return index < 0 ? value : value.Substring(index, value.Length - index); }

        /// <summary>
        ///     Convert text's case to a title case
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToTitleCase(this string value) { return CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(value); }

        /// <summary>
        ///     Toes the plural.
        /// </summary>
        /// <param name="singular">The singular.</param>
        /// <returns></returns>
        public static string ToPlural(this string singular)
        {
            // Multiple words in the form A of B : Apply the plural to the first word only (A)
            int index = singular.LastIndexOf(" of ");
            if(index > 0)
                return (singular.Substring(0, index)) + singular.Remove(0, index).ToPlural();

            // single Word rules
            //sibilant ending rule
            if(singular.EndsWith("sh"))
                return singular + "es";
            if(singular.EndsWith("ch"))
                return singular + "es";
            if(singular.EndsWith("us"))
                return singular + "es";
            if(singular.EndsWith("ss"))
                return singular + "es";
            //-ies rule
            if(singular.EndsWith("y"))
                return singular.Remove(singular.Length - 1, 1) + "ies";
            // -oes rule
            if(singular.EndsWith("o"))
                return singular.Remove(singular.Length - 1, 1) + "oes";
            // -s suffix rule
            return singular + "s";
        }
        
        /// <summary>
        ///     Returns true if strings are equals, without consideration to case (<see cref="StringComparison.InvariantCultureIgnoreCase" />)
        /// </summary>
        public static bool EquivalentTo(this string s, string whateverCaseString) { return string.Equals(s, whateverCaseString, StringComparison.InvariantCultureIgnoreCase); }

        public static string Repeat(this string value, int number)
        {
            var sb = new StringBuilder();
            
            for (var i = 0; i < number; i++)
                sb.Append(value);

            return sb.ToString();
        }

        public static StringBuilder Append(this StringBuilder value, string value1, string value2)
        {
            return value.Append(value1)
                .Append(value2);
        }

        public static StringBuilder Append(this StringBuilder value, string value1, string value2, string value3)
        {
            return value.Append(value1)
                .Append(value2)
                .Append(value3);
        }

        public static StringBuilder Append(this StringBuilder value, params string[] values)
        {
            if(values != null)
                values.ForEach(v => value.Append(v));

            return value;
        }

        #endregion

        #region Regex based extension methods

        /// <summary>
        ///     Uses regular expressions to determine if the string matches to a given regex pattern.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <returns>
        ///     <c>true</c> if the value is matching to the specified pattern; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        ///     <code>
        /// 		var s = "12345";
        /// 		var isMatching = s.IsMatchingTo(@"^\d+$");
        /// 	</code>
        /// </example>
        public static bool IsMatchingTo(this string value, string regexPattern) { return IsMatchingTo(value, regexPattern, RegexOptions.None); }

        /// <summary>
        ///     Uses regular expressions to determine if the string matches to a given regex pattern.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <param name="options">The regular expression options.</param>
        /// <returns>
        ///     <c>true</c> if the value is matching to the specified pattern; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        ///     <code>
        /// 		var s = "12345";
        /// 		var isMatching = s.IsMatchingTo(@"^\d+$");
        /// 	</code>
        /// </example>
        public static bool IsMatchingTo(this string value, string regexPattern, RegexOptions options) { return Regex.IsMatch(value, regexPattern, options); }

        /// <summary>
        ///     Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <param name="replaceValue">The replacement value.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        ///     <code>
        /// 		var s = "12345";
        /// 		var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// 	</code>
        /// </example>
        public static string ReplaceWith(this string value, string regexPattern, string replaceValue) { return ReplaceWith(value, regexPattern, replaceValue, RegexOptions.None); }

        /// <summary>
        ///     Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <param name="replaceValue">The replacement value.</param>
        /// <param name="options">The regular expression options.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        ///     <code>
        /// 		var s = "12345";
        /// 		var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// 	</code>
        /// </example>
        public static string ReplaceWith(this string value, string regexPattern, string replaceValue, RegexOptions options) { return Regex.Replace(value, regexPattern, replaceValue, options); }

        /// <summary>
        ///     Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <param name="evaluator">The replacement method / lambda expression.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        ///     <code>
        /// 		var s = "12345";
        /// 		var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// 	</code>
        /// </example>
        public static string ReplaceWith(this string value, string regexPattern, MatchEvaluator evaluator) { return ReplaceWith(value, regexPattern, RegexOptions.None, evaluator); }

        /// <summary>
        ///     Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <param name="options">The regular expression options.</param>
        /// <param name="evaluator">The replacement method / lambda expression.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        ///     <code>
        /// 		var s = "12345";
        /// 		var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// 	</code>
        /// </example>
        public static string ReplaceWith(this string value, string regexPattern, RegexOptions options, MatchEvaluator evaluator) { return Regex.Replace(value, regexPattern, evaluator, options); }

        /// <summary>
        ///     Uses regular expressions to determine all matches of a given regex pattern.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <returns>A collection of all matches</returns>
        public static MatchCollection GetMatches(this string value, string regexPattern) { return GetMatches(value, regexPattern, RegexOptions.None); }

        /// <summary>
        ///     Uses regular expressions to determine all matches of a given regex pattern.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <param name="options">The regular expression options.</param>
        /// <returns>A collection of all matches</returns>
        public static MatchCollection GetMatches(this string value, string regexPattern, RegexOptions options) { return Regex.Matches(value, regexPattern, options); }

        /// <summary>
        ///     Uses regular expressions to determine all matches of a given regex pattern and returns them as string enumeration.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <returns>An enumeration of matching strings</returns>
        /// <example>
        ///     <code>
        /// 		var s = "12345";
        /// 		foreach(var number in s.GetMatchingValues(@"\d")) {
        /// 		Console.WriteLine(number);
        /// 		}
        /// 	</code>
        /// </example>
        public static IEnumerable <string> GetMatchingValues(this string value, string regexPattern) { return GetMatchingValues(value, regexPattern, RegexOptions.None); }

        /// <summary>
        ///     Uses regular expressions to determine all matches of a given regex pattern and returns them as string enumeration.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <param name="options">The regular expression options.</param>
        /// <returns>An enumeration of matching strings</returns>
        /// <example>
        ///     <code>
        /// 		var s = "12345";
        /// 		foreach(var number in s.GetMatchingValues(@"\d")) {
        /// 		Console.WriteLine(number);
        /// 		}
        /// 	</code>
        /// </example>
        public static IEnumerable <string> GetMatchingValues(this string value, string regexPattern, RegexOptions options) { return from Match match in GetMatches(value, regexPattern, options) where match.Success select match.Value; }

        /// <summary>
        ///     Uses regular expressions to split a string into parts.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <returns>The splitted string array</returns>
        public static string[] Split(this string value, string regexPattern) { return value.Split(regexPattern, RegexOptions.None); }

        /// <summary>
        ///     Uses regular expressions to split a string into parts.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <param name="options">The regular expression options.</param>
        /// <returns>The splitted string array</returns>
        public static string[] Split(this string value, string regexPattern, RegexOptions options) { return Regex.Split(value, regexPattern, options); }

        /// <summary>
        ///     Splits the given string into words and returns a string array.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <returns>The splitted string array</returns>
        public static string[] GetWords(this string value) { return value.Split(@"\W"); }

        /// <summary>
        ///     Gets the nth "word" of a given string, where "words" are substrings separated by a given separator
        /// </summary>
        /// <param name="value">The string from which the word should be retrieved.</param>
        /// <param name="index">Index of the word (0-based).</param>
        /// <returns>
        ///     The word at position n of the string.
        ///     Trying to retrieve a word at a position lower than 0 or at a position where no word exists results in an exception.
        /// </returns>
        /// <remarks>
        ///     Originally contributed by MMathews
        /// </remarks>
        public static string GetWordByIndex(this string value, int index)
        {
            string[] words = value.GetWords();

            if((index < 0) || (index > words.Length - 1))
                throw new IndexOutOfRangeException("The word number is out of range.");

            return words[index];
        }

        /// <summary>
        ///     Removed all special characters from the string.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <returns>The adjusted string.</returns>
        /// <remarks>
        ///     Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string RemoveAllSpecialCharacters(this string value)
        {
            var sb = new StringBuilder();
            foreach(char c in value.Where(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')))
                sb.Append(c);
            return sb.ToString();
        }

        /// <summary>
        ///     Add space on every upper character
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <returns>The adjusted string.</returns>
        /// <remarks>
        ///     Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string SpaceOnUpper(this string value) { return Regex.Replace(value, "([A-Z])(?=[a-z])|(?<=[a-z])([A-Z]|[0-9]+)", " $1$2").TrimStart(); }

        #endregion

        #region String to Enum

        /// <summary>
        ///     Parse a string to a enum item if that string exists in the enum otherwise return the default enum item.
        /// </summary>
        /// <typeparam name="TEnum">The Enum type.</typeparam>
        /// <param name="dataToMatch">The data will use to convert into give enum</param>
        /// <param name="ignorecase">Whether the enum parser will ignore the given data's case or not.</param>
        /// <returns>Converted enum.</returns>
        /// <example>
        ///     <code>
        /// 		public enum EnumTwo {  None, One,}
        /// 		object[] items = new object[] { "One".ParseStringToEnum<EnumTwo>(), "Two".ParseStringToEnum<EnumTwo>() };
        /// 	</code>
        /// </example>
        /// <remarks>
        ///     Contributed by Mohammad Rahman, http://mohammad-rahman.blogspot.com/
        /// </remarks>
        public static TEnum ParseStringToEnum<TEnum>(this string dataToMatch, bool ignorecase = default(bool)) where TEnum : struct { return dataToMatch.IsItemInEnum <TEnum>()() ? default(TEnum) : (TEnum) Enum.Parse(typeof(TEnum), dataToMatch, default(bool)); }

        /// <summary>
        ///     To check whether the data is defined in the given enum.
        /// </summary>
        /// <typeparam name="TEnum">The enum will use to check, the data defined.</typeparam>
        /// <param name="dataToCheck">To match against enum.</param>
        /// <returns>Anonoymous method for the condition.</returns>
        /// <remarks>
        ///     Contributed by Mohammad Rahman, http://mohammad-rahman.blogspot.com/
        /// </remarks>
        public static Func <bool> IsItemInEnum<TEnum>(this string dataToCheck) where TEnum : struct { return () => { return string.IsNullOrEmpty(dataToCheck) || !Enum.IsDefined(typeof(TEnum), dataToCheck); }; }

        #endregion
    }

    public static class ListExtensions
    {
        public static List<TOutput> Convert<T, TOutput>(this IList<T> list, Func<T, TOutput> convert)
        {
            var newList = new List<TOutput>();
            if (list != null)
                list.ForEach(i => newList.Add(convert(i)));
            return newList;
        }

        /// <summary>
        /// 	Inserts an item uniquely to to a list and returns a value whether the item was inserted or not.
        /// </summary>
        /// <typeparam name = "T">The generic list item type.</typeparam>
        /// <param name = "list">The list to be inserted into.</param>
        /// <param name = "index">The index to insert the item at.</param>
        /// <param name = "item">The item to be added.</param>
        /// <returns>Indicates whether the item was inserted or not</returns>
        public static bool InsertUnique<T>(this IList<T> list, int index, T item)
        {
            if (list.Contains(item) == false)
            {
                list.Insert(index, item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 	Return the index of the first matching item or -1.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "list">The list.</param>
        /// <param name = "comparison">The comparison.</param>
        /// <returns>The item index</returns>
        public static int IndexOf<T>(this IList<T> list, Func<T, bool> comparison)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (comparison(list[i]))
                    return i;
            }
            return -1;
        }

        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (var value in values)
                action(value);
        }

        public static int Remove<T>(this List<T> values, Predicate<T> where)
        {
            if(values == null || values.Count == 0)
                return 0;
            
            var toDelete = values.Where(value => where(value)).ToList();
            toDelete.ForEach(v => values.Remove(v));
            
            return toDelete.Count;
        }

        /// <summary>
        /// 	Join all the elements in the list and create a string seperated by the specified char.
        /// </summary>
        /// <param name = "list">
        /// 	The list.
        /// </param>
        /// <param name = "joinChar">
        /// 	The join char.
        /// </param>
        /// <typeparam name = "T">
        /// </typeparam>
        /// <returns>
        /// 	The resulting string of the elements in the list.
        /// </returns>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string Join<T>(this IList<T> list, char joinChar)
        {
            return list.Join(joinChar.ToString());
        }

        /// <summary>
        /// 	Join all the elements in the list and create a string seperated by the specified string.
        /// </summary>
        /// <param name = "list">
        /// 	The list.
        /// </param>
        /// <param name = "joinString">
        /// 	The join string.
        /// </param>
        /// <typeparam name = "T">
        /// </typeparam>
        /// <returns>
        /// 	The resulting string of the elements in the list.
        /// </returns>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// 	Optimised by Mario Majcica
        /// </remarks>
        public static string Join<T>(this IList<T> list, string joinString)
        {
            if (list == null || !list.Any())
                return String.Empty;

            var result = new StringBuilder();

            int listCount = list.Count;
            int listCountMinusOne = listCount - 1;

            if (listCount > 1)
            {
                for (var i = 0; i < listCount; i++)
                {
                    if (i != listCountMinusOne)
                    {
                        result.Append(list[i]);
                        result.Append(joinString);
                    }
                    else
                        result.Append(list[i]);
                }
            }
            else
                result.Append(list[0]);

            return result.ToString();
        }

        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the default eqaulity comparer for the projected type.
        /// </summary>
        /// <remarks>
        /// This operator uses deferred execution and streams the results, although
        /// a set of already-seen keys is retained. If a key is seen multiple times,
        /// only the first element with that key is returned.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="keySelector">Projection for determining "distinctness"</param>
        /// <returns>A sequence consisting of distinct elements from the source sequence,
        /// comparing them by the specified key projection.</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }

        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the specified comparer for the projected type.
        /// </summary>
        /// <remarks>
        /// This operator uses deferred execution and streams the results, although
        /// a set of already-seen keys is retained. If a key is seen multiple times,
        /// only the first element with that key is returned.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="keySelector">Projection for determining "distinctness"</param>
        /// <param name="comparer">The equality comparer to use to determine whether or not keys are equal.
        /// If null, the default equality comparer for <c>TSource</c> is used.</param>
        /// <returns>A sequence consisting of distinct elements from the source sequence,
        /// comparing them by the specified key projection.</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            return DistinctByImpl(source, keySelector, comparer);
        }

        private static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var knownKeys = new HashSet<TKey>(comparer);
            foreach (var element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }

    public static class DataRowExtensions
    {
        /// <summary>
        /// 	Gets the record value casted to the specified data type or the data types default value.
        /// </summary>
        /// <typeparam name = "T">The return data type</typeparam>
        /// <param name = "row">The data row.</param>
        /// <param name = "field">The name of the record field.</param>
        /// <returns>The record value</returns>
        public static T Get<T>(this DataRow row, string field)
        {
            return row.Get(field, default(T));
        }

        /// <summary>
        /// 	Gets the record value casted to the specified data type or the specified default value.
        /// </summary>
        /// <typeparam name = "T">The return data type</typeparam>
        /// <param name = "row">The data row.</param>
        /// <param name = "field">The name of the record field.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <returns>The record value</returns>
        public static T Get<T>(this DataRow row, string field, T defaultValue)
        {
            var value = row[field];
            if (value == DBNull.Value)
                return defaultValue;
            return value.ConvertTo(defaultValue);
        }

        /// <summary>
        /// 	Gets the record value casted as byte array.
        /// </summary>
        /// <param name = "row">The data row.</param>
        /// <param name = "field">The name of the record field.</param>
        /// <returns>The record value</returns>
        public static byte[] GetBytes(this DataRow row, string field)
        {
            return (row[field] as byte[]);
        }

        /// <summary>
        /// 	Gets the record value casted as string or null.
        /// </summary>
        /// <param name = "row">The data row.</param>
        /// <param name = "field">The name of the record field.</param>
        /// <returns>The record value</returns>
        public static string GetString(this DataRow row, string field)
        {
            return row.GetString(field, null);
        }

        /// <summary>
        /// 	Gets the record value casted as string or the specified default value.
        /// </summary>
        /// <param name = "row">The data row.</param>
        /// <param name = "field">The name of the record field.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <returns>The record value</returns>
        public static string GetString(this DataRow row, string field, string defaultValue)
        {
            var value = row[field];
            return (value is string ? (string)value : defaultValue);
        }

        /// <summary>
        /// 	Gets the record value casted as int or 0.
        /// </summary>
        /// <param name = "row">The data row.</param>
        /// <param name = "field">The name of the record field.</param>
        /// <returns>The record value</returns>
        public static int GetInt32(this DataRow row, string field)
        {
            return row.GetInt32(field, 0);
        }

        /// <summary>
        /// 	Gets the record value casted as int or the specified default value.
        /// </summary>
        /// <param name = "row">The data row.</param>
        /// <param name = "field">The name of the record field.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <returns>The record value</returns>
        public static int GetInt32(this DataRow row, string field, int defaultValue)
        {
            var value = row[field];
            return (value is int ? (int)value : defaultValue);
        }

        /// <summary>
        /// 	Gets the record value casted as bool or false.
        /// </summary>
        /// <param name = "row">The data row.</param>
        /// <param name = "field">The name of the record field.</param>
        /// <returns>The record value</returns>
        public static bool GetBoolean(this DataRow row, string field)
        {
            return row.GetBoolean(field, false);
        }

        /// <summary>
        /// 	Gets the record value casted as bool or the specified default value.
        /// </summary>
        /// <param name = "row">The data row.</param>
        /// <param name = "field">The name of the record field.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <returns>The record value</returns>
        public static bool GetBoolean(this DataRow row, string field, bool defaultValue)
        {
            var value = row[field];
            return (value is bool ? (bool)value : defaultValue);
        }

        /// <summary>
        /// Gets the data errors for all the tables in the data set.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <returns>System.String.</returns>
        public static string GetDataErrors(this DataSet dataset)
        {
            var s = new StringBuilder();

            if (dataset != null)
            {
                s.AppendLine("Dataset " + dataset.DataSetName + " Errors");

                if (dataset.HasErrors)
                {
                    foreach(DataTable item in dataset.Tables)
                    {
                        var tableErrors = GetDataErrors(item);
                        if(!String.IsNullOrWhiteSpace(tableErrors))
                            s.AppendLine(tableErrors);
                    }
                }
                else
                    s.AppendLine("\tNone");
            }

            return s.ToString();
        }

        /// <summary>
        /// Gets any data errors for the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        private static string GetDataErrors(this DataTable table)
        {
            var s = new StringBuilder();

            if (table != null)
            {
                if (table.HasErrors)
                {
                    try
                    {
                        s.AppendLine(" -- Table '" + table.TableName + "' Errors --");

                        foreach (DataRow row in table.GetErrors())
                        {
                            s.AppendLine("Row Error: " + row.RowError);
                            s.AppendLine($" Row State: {row.RowState}");

                            if (row.RowState == DataRowState.Detached || row.RowState == DataRowState.Deleted)
                            {
                                s.AppendLine($" Type: {row.GetType()}");
                            }
                            else
                            {
                                s.AppendLine(" Row Values: " + String.Join(",", row.ItemArray));
                            }
                             
                            foreach (DataColumn column in table.Columns)
                            {
                                var error = row.GetColumnError(column);
                                if (!String.IsNullOrWhiteSpace(error))
                                {
                                    object rowValue = row.HasVersion(DataRowVersion.Current) ? row[column, DataRowVersion.Current] : "No Current Version";
                                    s.AppendLine(String.Format("\tColumn: {0} \tValue: {1} \tError: {2}", column.ColumnName, (rowValue == null ? "NULL" : rowValue.ToString()), error));
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        LogManager.GetCurrentClassLogger().Error(exc, "Error getting data errors.");
                    }
                }
            }

            return s.ToString();
        }

        public static List<string> GetDistinctColumnValues(this  DataTable table, string columnName)
        {
            var values = new List <string>();

            if (table != null)
            {
                foreach(DataRow row in table.Rows)
                {
                    var value = row[columnName];
                    
                    if (value != null && value != DBNull.Value && !values.Contains(value.ToString()))
                        values.Add(value.ToString());
                }
            }

            return values;
        }
    }

    public static class ObjectExtensions
    {
        /// <summary>
        /// 	Converts an object to the specified target type or returns the default value if
        ///     those 2 types are not convertible.
        ///     <para>
        ///     If the <paramref name="value"/> can't be convert even if the types are 
        ///     convertible with each other, an exception is thrown.</para>
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "value">The value.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <returns>The target type</returns>
        public static T ConvertTo<T>(this object value, T defaultValue)
        {
            if (value != null)
            {
                var targetType = typeof(T);

                if (value.GetType() == targetType) return (T)value;

                var converter = TypeDescriptor.GetConverter(value);
                if (converter != null)
                {
                    if (converter.CanConvertTo(targetType))
                        return (T)converter.ConvertTo(value, targetType);
                }

                converter = TypeDescriptor.GetConverter(targetType);
                if (converter != null)
                {
                    if (converter.CanConvertFrom(value.GetType()))
                        return (T)converter.ConvertFrom(value);
                }
            }
            return defaultValue;
        }
    }

    /// <summary>
    /// 	Extension methods for the reflection meta data type "Type"
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 	Creates and returns an instance of the desired type
        /// </summary>
        /// <param name = "type">The type to be instanciated.</param>
        /// <param name = "constructorParameters">Optional constructor parameters</param>
        /// <returns>The instanciated object</returns>
        /// <example>
        /// 	<code>
        /// 		var type = Type.GetType(".NET full qualified class Type")
        /// 		var instance = type.CreateInstance();
        /// 	</code>
        /// </example>
        public static object CreateInstance(this Type type, params object[] constructorParameters)
        {
            return CreateInstance<object>(type, constructorParameters);
        }

        /// <summary>
        /// 	Creates and returns an instance of the desired type casted to the generic parameter type T
        /// </summary>
        /// <typeparam name = "T">The data type the instance is casted to.</typeparam>
        /// <param name = "type">The type to be instanciated.</param>
        /// <param name = "constructorParameters">Optional constructor parameters</param>
        /// <returns>The instanciated object</returns>
        /// <example>
        /// 	<code>
        /// 		var type = Type.GetType(".NET full qualified class Type")
        /// 		var instance = type.CreateInstance&lt;IDataType&gt;();
        /// 	</code>
        /// </example>
        public static T CreateInstance<T>(this Type type, params object[] constructorParameters)
        {
            var instance = Activator.CreateInstance(type, constructorParameters);
            return (T)instance;
        }
    }

    public static class VersionExtensions
    {
        public static decimal ToDecimal(this Version version)
        {
            try
            {
                if (version == null)
                    return 0;

                //15.23.55.23 => 1523.5523
                var numberString = version.Major.ToString("D2") + version.Minor.ToString("D2") + "." + version.Build.ToString("D2") + version.Revision.ToString("D2");
                return Convert.ToDecimal(numberString);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error converting version number to decimal.");
                return 0;
            }
        }

        public static bool CompareMajorMinorBuild(this Version version, Version compareTo) { return version.Major == compareTo.Major && version.Minor == compareTo.Minor && version.Build == compareTo.Build; }

        /// <summary>
        /// Creates a new <see cref="Version"/> instance using the major,
        /// minor, and build components of a source instance.
        /// </summary>
        /// <param name="version">Source version</param>
        /// <returns>New instance.</returns>
        public static Version ToMajorMinorBuild(this Version version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            int major = version.Major;
            int minor = version.Minor;
            int build = version.Build;

            return new Version(major, minor, build, 0);
        }
    }
}