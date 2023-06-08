using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace DWOS.Reports
{
    public abstract class CodeConverterAbstract: TypeConverter
    {
        #region Fields

        protected static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        protected abstract IDataReader DataReader { get; }

        protected abstract StandardValuesCollection StandardValues { get; set; }

        protected abstract List<string> Values { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///   Constructor
        /// </summary>
        public CodeConverterAbstract()
        {
            if(this.Values == null)
            {
                //Instantiate a new array
                IDataReader dr = this.DataReader;
                var coll = new List<string>();

                try
                {
                    while(dr.Read())
                        coll.Add(dr[0].ToString());

                    //Cast the array to a standardvaluescollection and set instance variable
                    this.StandardValues = new StandardValuesCollection(coll);
                    this.Values = coll;
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
                if(this.Values.Contains(value.ToString()))
                    return value.ToString();
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
            if(value != null && this.Values != null)
            {
                if(this.Values.Contains(value.ToString()))
                    return value.ToString();
            }

            //return null;
            //Debug.Assert(false, "Unable to convert to.", "Should be able to always handle value.");
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }
}
