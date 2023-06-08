using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace DWOS.Reports
{
    public abstract class CodeValueConverterAbstract: TypeConverter
    {
        #region Fields

        protected static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion
        
        #region Properties

        protected abstract IDataReader DataReader { get; }

        protected abstract StandardValuesCollection StandardValues { get; set; }

        protected abstract Dictionary<int, string> Dictionary { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///   Constructor
        /// </summary>
        public CodeValueConverterAbstract()
        {
            if(this.Dictionary == null)
            {
                //Instantiate a new array
                IDataReader dr = this.DataReader;
                var dict = new Dictionary<int, string>();

                try
                {
                    while(dr.Read())
                        dict.Add(dr.GetInt32(0), dr[1].ToString());

                    //Cast the array to a standardvaluescollection and set instance variable
                    this.StandardValues = new StandardValuesCollection(dict.Values);
                    this.Dictionary = dict;
                }
                finally
                {
                    if(dr != null)
                        dr.Dispose();

                    dr = null;
                }
            }
        }

        /// <summary>
        ///   Indicated if standard values are supported
        /// </summary>
        /// <param name="context"> </param>
        /// <returns> </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///   Indicates if manually entering values is accepts
        /// </summary>
        /// <param name="context"> </param>
        /// <returns> </returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            // returning false here means the property will
            // have a drop down and a value that can be manually
            // entered.      
            return true;
        }

        /// <summary>
        ///   Returns StandardCollection Instance Variable
        /// </summary>
        /// <param name="context"> </param>
        /// <returns> </returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return this.StandardValues;
        }

        /// <summary>
        ///   Indicates if a type descriptor can be converted
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="sourceType"> </param>
        /// <returns> true or false </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            //if its a string return true else send to base class
            return true;
        }

        /// <summary>
        ///   Converts from the display object to the persistable object
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="culture"> </param>
        /// <param name="value"> </param>
        /// <returns> </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            //If a string is passed in parse and return an object else send it to the base
            if(value is string && !String.IsNullOrEmpty(value.ToString()))
            {
                //find value based on key
                foreach(var kvp in this.Dictionary)
                {
                    if(kvp.Value == value.ToString())
                        return kvp.Key;
                }
            }

            if(value is Int32)
            {
                var number = (int)value;

                //find value based on value
                foreach(var kvp in this.Dictionary)
                {
                    if(kvp.Key == number)
                        return kvp.Value;
                }
            }

            return value;
        }

        /// <summary>
        ///   Converts from persitable object to display object
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="culture"> </param>
        /// <param name="value"> </param>
        /// <param name="destinationType"> </param>
        /// <returns> </returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if(value != null && this.Dictionary != null)
            {
                //based on key return the value (i.e. description)
                if(value is Int32 && this.Dictionary.ContainsKey(Convert.ToInt32(value)))
                    return this.Dictionary[Convert.ToInt32(value)];
                else if(value is string)
                {
                    foreach(var kvp in this.Dictionary)
                    {
                        if(kvp.Value == value.ToString())
                            return kvp.Value;
                    }

                    return value;
                }
            }

            //return null;
            //Debug.Assert(false, "Unable to convert to.", "Should be able to always handle value.");
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }
}