using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DWOS.Data.Datasets;

namespace DWOS.Data.Utilities
{
    /// <summary>
    /// Defines utility methods for interpreting various expressions.
    /// </summary>
    public static class Interperter
    {
        /// <summary>
        /// Interprets an expression using a part and order.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="part"></param>
        /// <param name="order"></param>
        /// <returns>Interpreted value</returns>
        public static string Interpert(string expression, PartsDataset.PartRow part, OrdersDataSet.OrderRow order)
        {
            var output = new StringBuilder();
            List<ParseToken> tokens = Parse(expression);

            foreach(ParseToken token in tokens)
            {
                if(token.IsCommand)
                {
                    enumInterperterCommands cmd;
                    Enum.TryParse<enumInterperterCommands>(token.Value.ToUpper(), out cmd);
                    

                    switch(cmd)
                    {
                        case enumInterperterCommands.DATE:
                        case enumInterperterCommands.DATE2:
                        case enumInterperterCommands.TIME:
                            output.Append(ParseCommand(cmd));
                            break;
                        case enumInterperterCommands.PARTNUMBER:
                            output.Append(part.Name);
                            break;
                        case enumInterperterCommands.ASSEMBLY:
                            if(!part.IsAssemblyNumberNull())
                                output.Append(part.AssemblyNumber);
                            break;
                        case enumInterperterCommands.PARTREV:
                            if(!part.IsRevisionNull())
                                output.Append(part.Revision);
                            break;
                        case enumInterperterCommands.CUSTOMERWO:
                            if(order != null && !order.IsCustomerWONull())
                                output.Append(order.CustomerWO);
                            break;
                        case enumInterperterCommands.PARTQTY:
                            if(order != null && !order.IsPartQuantityNull())
                                output.Append(order.PartQuantity).ToString();
                            break;
                        case enumInterperterCommands.CUSTOMFIELD:
                        default:
                            //Attempt to find matching custom field
                            using(var taOCF = new Data.Datasets.OrdersDataSetTableAdapters.OrderCustomFieldsTableAdapter())
                            {
                                var orderCustomField = taOCF.GetPartMarkValues(order.CustomerID, order.OrderID, token.Value.ToUpper()).FirstOrDefault();
                                if(orderCustomField != null && !orderCustomField.IsValueNull() && !String.IsNullOrWhiteSpace(orderCustomField.Value))
                                    output.Append(orderCustomField.Value).ToString();
                            }
                            break;
                    }
                }
                else
                    output.Append(token.Value);
            }

            return output.ToString();
        }

        /// <summary>
        /// Parses the specified expression and return all tokens.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        private static List<ParseToken> Parse(string expression)
        {
            var tokens = new List<ParseToken>();

            if(!expression.Contains('<'))
                tokens.Add(new ParseToken {Value = expression});
            else
            {
                string[] splitByStart = expression.Split('<');

                foreach(string token in splitByStart)
                {
                    if(!token.Contains('>')) // no command
                        tokens.Add(new ParseToken {Value = token});
                    else //contains a command
                    {
                        string[] splitByEnd = token.Split('>');

                        if(splitByEnd.Length > 0)
                        {
                            tokens.Add(new ParseToken {Value = splitByEnd[0], IsCommand = true});

                            //add back all remaining pieces, should only be one, but just in case incorrect command pattern
                            for(int i = 1; i < splitByEnd.Length; i++)
                                tokens.Add(new ParseToken {Value = splitByEnd[i], IsCommand = false});
                        }
                    }
                }
            }

            return tokens;
        }

        /// <summary>
        /// Parses the basic commands.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        public static string ParseCommand(enumInterperterCommands cmd)
        {
            switch(cmd)
            {
                case enumInterperterCommands.DATE:
                    return DateTime.Now.ToShortDateString();
                case enumInterperterCommands.DATE2:
                    return DateTime.Now.ToString("dd/MM/yyyy");
                case enumInterperterCommands.TIME:
                    return DateTime.Now.ToShortTimeString();
                case enumInterperterCommands.PARTNUMBER:
                case enumInterperterCommands.PARTREV:
                case enumInterperterCommands.ASSEMBLY:
                case enumInterperterCommands.CUSTOMERWO:
                case enumInterperterCommands.PARTQTY:
                default:
                    return null;
            }
        }

        #region Nested type: ParseToken

        private class ParseToken
        {
            public bool IsCommand { get; set; }
            public string Value { get; set; }
        }

        #endregion

        #region Nested type: enumInterperterCommands

        /// <summary>
        /// Represents a type of interpreter command
        /// </summary>
        public enum enumInterperterCommands
        {
            /// <summary>
            /// Custom field
            /// </summary>
            CUSTOMFIELD = 0,

            /// <summary>
            ///  Short date
            /// </summary>
            DATE,

            /// <summary>
            /// Short time
            /// </summary>
            TIME,

            /// <summary>
            /// Part number
            /// </summary>
            PARTNUMBER,

            /// <summary>
            /// Part revision
            /// </summary>
            PARTREV,

            /// <summary>
            /// Part assembly
            /// </summary>
            ASSEMBLY,

            /// <summary>
            /// Customer Work Order number
            /// </summary>
            CUSTOMERWO,

            /// <summary>
            /// Part quantity
            /// </summary>
            PARTQTY,

            /// <summary>
            /// Date in dd/MM/yyyy format
            /// </summary>
            DATE2
        }

        #endregion
    }
}