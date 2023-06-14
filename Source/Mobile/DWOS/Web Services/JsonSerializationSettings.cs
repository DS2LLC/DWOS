using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DWOS.Services
{
    /// <summary>
    /// Provides global Json Serialization Settings for everyone
    /// </summary>
    public static class JsonSerializationSettings
    {
        private static JsonSerializerSettings _settings;

        /// <summary>
        /// <see cref="JsonSerializerSettings"/> instance to use throughout
        /// the app.
        /// </summary>
        public static JsonSerializerSettings Settings
        {
            get
            {
                return _settings ?? (_settings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.None,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }
        }
    }
}
