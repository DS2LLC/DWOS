using DWOS.Shared.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DWOS.Data
{
    /// <summary>
    /// <see cref="IFieldConverter"/> implementation that converts
    /// <see cref="TabInfoCollection"/> instances to JSON strings and
    /// back again.
    /// </summary>
    public class TabInfoToJson : IFieldConverter
    {
        #region Implementation of IFieldConverter

        /// <summary>
        /// Converts a value from the data store.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public object ConvertFromField(object value)
        {            
            return value == null ? null : JsonConvert.DeserializeObject(value.ToString(), typeof(TabInfoCollection)) as TabInfoCollection;
        }

        /// <summary>
        /// Converts a value to be stored in the data store.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public object ConvertToField(object value)
        {
            if (value == null)
                value = new TabInfoCollection() { Tabs = new List<TabInfo>() };

            var json = JsonConvert.SerializeObject(value, Formatting.None, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.None });
            return json;
        }

        #endregion
    }


}
