using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace DWOS.LabelEditor
{
    /// <summary>
    /// Represents a label token.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets or sets a non-unique identifier.
        /// </summary>
        /// <remarks>
        /// This is used to identify what content should go in the token
        /// when a label is created.
        /// </remarks>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets this instance's display name.
        /// </summary>
        /// <remarks>
        /// This is user-facing.
        /// </remarks>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets an example value.
        /// </summary>
        /// <remarks>
        /// Used when previewing a label a label from the label editor.
        /// </remarks>
        public string SampleValue { get; set; }

        /// <summary>
        /// Gets or sets the token type.
        /// </summary>
        public TokenType TokenType { get; set; }

        /// <summary>
        /// Gets or sets the tooltip text to show.
        /// </summary>
        public string ToolTip { get; set; }

        public override string ToString()
        {
            return DisplayName + "[" + this.TokenType.ToString() + "]";
        }

        /// <summary>
        /// Replaces the contents containing tokens in the form of %TOKEN% with the sample values.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="tokens">The tokens.</param>
        /// <returns></returns>
        public static string ReplaceTokens(string content, List<Token> tokens)
        {
            //Search and replace tokens in the form of: %TOKEN%
            //var rex = new Regex(@"\${([^}]+)}");  //original ${SOMETHING}
            //var rex = Regex.Split(txtMultifieldContent.Text, @"\%(.*?)\%").ToList();
            var rex = new Regex(@"\%(.*?)\%");

            return (rex.Replace(content, delegate (Match m)
            {
                string key = m.Groups[1].Value.Replace("%", String.Empty);
                var token = tokens.FirstOrDefault(t => t.ID == key);
                string rep = token == null ? m.Value : token.SampleValue;
                return (rep);
            }));
        }
    }


}
