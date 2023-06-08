using System.Collections.Generic;
using System.Linq;
using DWOS.Shared.Settings;

namespace DWOS.UI.DesignTime
{
    internal class FakeSettingsPersistence : ISettingsPersistence
    {
        #region Fields

        private readonly IDictionary<string, SettingValue> _settings =
            new Dictionary<string, SettingValue>();

        #endregion

        #region ISettingsPersistence Members

        public string ConnectionString { get; set; }

        public void LoadFromDataStore(SettingValue setting)
        {
            _settings.TryGetValue(setting.Name, out var foundSetting);

            if (foundSetting != null)
            {
                setting.Value = foundSetting.Value;
                setting.ConvertedValue = null;
            }
        }

        public void SaveToDataStore(IEnumerable<SettingValue> settings)
        {
            foreach (var setting in settings)
            {
                _settings[setting.Name] = setting;
            }
        }

        public void ValidateConnection()
        {
            // Do Nothing
        }

        #endregion
    }
}