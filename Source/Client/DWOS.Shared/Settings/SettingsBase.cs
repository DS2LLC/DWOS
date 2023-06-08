using NLog;
using System;
using System.Collections.Concurrent;

namespace DWOS.Shared.Settings
{
    /// <summary>
    /// Base class for storing settings using a
    /// <see cref="ISettingsPersistence"/> instance.
    /// </summary>
    public abstract class SettingsBase
    {
        #region Fields

        protected static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private ConcurrentDictionary<string, SettingValue> _cache = new ConcurrentDictionary<string, SettingValue>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the connection string for this instance.
        /// </summary>
        public string ConnectionString
        {
            get { return Persistence.ConnectionString; }
        }

        /// <summary>
        /// Gets or sets the persistence object for this instance.
        /// </summary>
        protected ISettingsPersistence Persistence
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsBase"/>
        /// class.
        /// </summary>
        /// <param name="persistence"></param>
        protected SettingsBase(ISettingsPersistence persistence)
        {
            if (persistence == null)
            {
                throw new ArgumentNullException(nameof(persistence));
            }

            Persistence = persistence;
        }
        
        /// <summary>
        /// Gets a setting value from the cache or persistence.
        /// </summary>
        /// <remarks>
        /// If a value is found in cache, it is returned. Otherwise, it is
        /// retrieved, put into cache, and returned.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="converter"></param>
        /// <param name="defaultValue"></param>
        /// <param name="fallbackToDefault"></param>
        /// <returns></returns>
        protected T GetSettingValue<T>(string propertyName, Func<string, T> converter = null, Func<string> defaultValue = null, bool fallbackToDefault = false)
        {
            var settingValue =  _cache.GetOrAdd(propertyName,
                (key) =>
                {
                    var settingInfo = new SettingValue() { Name = key};

                    try
                    {
                         _log.Debug("Getting setting from database {0}".FormatWith(settingInfo.Name));
                        Persistence.LoadFromDataStore(settingInfo);

                        if (string.IsNullOrEmpty(settingInfo.Value))
                        {
                            settingInfo.Value = defaultValue?.Invoke();
                        }
                    }
                    catch (Exception exc)
                    {
                        _log.Error(exc, "Error loading setting {0} from database.".FormatWith(settingInfo.Name));

                        if (fallbackToDefault)
                        {
                            _log.Warn($"Falling back to default value for {propertyName}.");
                            settingInfo.Value = defaultValue?.Invoke();
                        }
                    }

                    if(!String.IsNullOrWhiteSpace(settingInfo.Value))
                    {
                        var outputType = typeof(T);

                        if(converter != null)
                            settingInfo.ConvertedValue = converter(settingInfo.Value);
                        else if (outputType == typeof(DateTime))
                            settingInfo.ConvertedValue = Convert.ToDateTime(settingInfo.Value);
                        else if (outputType == typeof(float))
                            settingInfo.ConvertedValue = Convert.ToSingle(settingInfo.Value);
                        else if (outputType == typeof(int) || outputType == typeof(int?))
                            settingInfo.ConvertedValue = Convert.ToInt32(settingInfo.Value);
                        else if (outputType == typeof(bool))
                            settingInfo.ConvertedValue = Convert.ToBoolean(settingInfo.Value);
                        else if (outputType == typeof(double))
                            settingInfo.ConvertedValue = Convert.ToDouble(settingInfo.Value);
                        else
                            settingInfo.ConvertedValue = Convert.ToString(settingInfo.Value);
                    }

                    return settingInfo;
                });

            //if has cached value
            if (settingValue.ConvertedValue != null)
                return (T)settingValue.ConvertedValue;
            
            //if not cached but T is of type string then return the value
            if(typeof(T) == typeof(string))
                return (T)(object)settingValue.Value;

            return default(T);
        }

        /// <summary>
        /// Sets a value for a setting.
        /// </summary>
        /// <remarks>
        /// This only changes the setting within persistence. To save this value, call
        /// <see cref="Save"/>.
        /// </remarks>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="convertedValue"></param>
        protected void SetSettingValue(string propertyName, string value, object convertedValue = null)
        {
            _cache.AddOrUpdate(propertyName,
                // Add value
                (key) =>
                {
                    return new SettingValue()
                    {
                        Name = propertyName,
                        Value = value,
                        ConvertedValue = convertedValue
                    };
                },

                // Update value
                (key, updateValue) =>
                {
                    updateValue.Value = value;
                    updateValue.ConvertedValue = convertedValue;
                    return updateValue;
                });
        }

        /// <summary>
        /// Persists all cached or changed settings.
        /// </summary>
        public void Save()
        {
            _log.Info("Saving settings to datastore.");
            Persistence.SaveToDataStore(_cache.Values);
        }

        /// <summary>
        /// Clears a setting from cache.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected int ClearSetting(string propertyName)
        {
            SettingValue removedSetting;
            if (_cache.TryRemove(propertyName, out removedSetting))
            {
                return removedSetting == null ? 0 : 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Is the property's value currently cached?
        /// </summary>
        /// <param name="propertyName">Name of the property to search cache for.</param>
        /// <returns>true if a value is cached; otherwise, false.</returns>
        protected bool IsInCache(string propertyName)
        {
            return _cache.ContainsKey(propertyName);
        }

        /// <summary>
        /// Returns the setting value. NOTE: Does not request from DB if does not exist.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>SettingValue.</returns>
        protected SettingValue FindSettingValue(string propertyName)
        {
            SettingValue cachedSetting;

            if (_cache.TryGetValue(propertyName, out cachedSetting))
            {
                return cachedSetting;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Clear every value from cache.
        /// </summary>
        protected void ClearCachedSettings()
        {
            _log.Info("Clearing cached settings.");
            _cache.Clear();
        }

        /// <summary>
        /// Validates a connection to the database.
        /// </summary>
        /// <param name="showErrorDialog"></param>
        /// <returns>
        /// True if the connection is valid; otherwise, false.
        /// </returns>
        public bool ValidateConnectionToDatabase(bool showErrorDialog = true)
        {
            try
            {
                _log.Debug("Validating connection to database.");
                Persistence.ValidateConnection();
                return true;
            }
            catch(Exception exc)
            {
                if (showErrorDialog)
                {
                    ErrorMessageBox.ShowDialog("Error connecting to the database.", exc, false);
                }

                _log.Warn(exc, "Error connecting to the database");

                return false;
            }
        }

        #endregion
    }
}