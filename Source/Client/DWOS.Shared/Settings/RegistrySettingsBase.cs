using DWOS.Shared.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Win32;
using NLog;

namespace DWOS.Shared.Settings
{
    /// <summary>
    /// Base Class for storing settings in a registry.
    /// </summary>
    public abstract class RegistrySettingsBase
    {
        #region Fields

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the registry key name for this instance.
        /// </summary>
        protected abstract string RegistryKeyName { get; }

        /// <summary>
        /// Gets the hive <see cref="RegistryKey"/> for this instance.
        /// </summary>
        protected abstract RegistryKey RegistryHive
        {
            get;
        }

        /// <summary>
        /// Gets the field mappings for this instance.
        /// </summary>
        protected List<FieldMapping> FieldMappings { get; }

        #endregion

        #region Methods

        protected RegistrySettingsBase()
        {
            FieldMappings = CreateFieldMapping(GetType());
        }

        /// <summary>
        /// Loads settings from the registry.
        /// </summary>
        protected void Load()
        {
            _logger.Info("Loading registry settings from '{0}'".FormatWith(RegistryKeyName));

            using (var baseKey = RegistryHive.OpenSubKey(RegistryKeyName, true) ?? RegistryHive.CreateSubKey(RegistryKeyName))
            {
                if (baseKey != null)
                {
                    //for all field mappings in this class
                    foreach (var fieldMap in FieldMappings)
                    {
                        LoadField(fieldMap, baseKey);
                    }
                }
            }

            AfterLoad();
        }

        private void LoadField(FieldMapping fm, RegistryKey baseKey)
        {
            var outputType = fm.Property.PropertyType;

            object value;

            if (typeof(NestedRegistrySettingsBase).IsAssignableFrom(outputType))
            {
                var settings = fm.Property.GetValue(this, null) as NestedRegistrySettingsBase ??
                               NestedRegistrySettingsBase.InstanceOf(outputType, RegistryHive,
                                   RegistryKeyName + @"\" + fm.DataMember.FieldName);

                settings.Load();
                value = settings;
            }
            else
            {
                value = baseKey.GetValue(fm.DataMember.FieldName);

                if (fm.Converter != null)
                {
                    value = fm.Converter.ConvertFromField(value ?? fm.DataMember.DefaultValue);
                }
                else if (value != null)
                {
                    if (outputType == typeof(DateTime))
                        value = Convert.ToDateTime(value);
                    else if (outputType == typeof(float))
                        value = Convert.ToSingle(value);
                    else if (outputType == typeof(int))
                        value = Convert.ToInt32(value);
                    else if (outputType == typeof(bool))
                        value = Convert.ToBoolean(value);
                    else if (outputType == typeof(double))
                        value = Convert.ToDouble(value);
                    else
                        value = Convert.ToString(value);
                }

                //set to default value if no other value
                if (value == null)
                    value = fm.DataMember.DefaultValue;
            }

            fm.Property.SetValue(this, value, null);
        }

        /// <summary>
        /// Saves settings to the registry.
        /// </summary>
        protected void SaveSettings()
        {
            _logger.Info("Saving registry settings to '{0}'.".FormatWith(RegistryKeyName));

            using (var baseKey = RegistryHive.OpenSubKey(RegistryKeyName, true) ?? RegistryHive.CreateSubKey(RegistryKeyName))
            {
                if (baseKey != null)
                {
                    foreach (var fm in FieldMappings)
                    {
                        SaveField(fm, baseKey);
                    }
                }
            }
        }

        private void SaveField(FieldMapping fm, RegistryKey baseKey)
        {
            var outputType = fm.Property.PropertyType;
            var currentValue = fm.Property.GetValue(this, null);

            if (typeof(NestedRegistrySettingsBase).IsAssignableFrom(outputType))
            {
                var nestedSettings = currentValue as NestedRegistrySettingsBase;
                nestedSettings?.SaveSettings();
            }
            else
            {
                if (fm.Converter != null)
                    currentValue = fm.Converter.ConvertToField(currentValue);

                if (outputType == typeof(int))
                    baseKey.SetValue(fm.DataMember.FieldName, currentValue ?? 0, RegistryValueKind.DWord);
                else
                    baseKey.SetValue(fm.DataMember.FieldName, currentValue ?? "", RegistryValueKind.String);
            }
        }

        /// <summary>
        /// Called when the <see cref="Load"/> method successfully finishes
        /// loading data.
        /// </summary>
        protected virtual void AfterLoad()
        {

        }

        /// <summary>
        /// Creates field mappings for the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static List<FieldMapping> CreateFieldMapping(Type type)
        {
            var fieldMappings = new List<FieldMapping>();

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var pi in properties)
            {
                var attributes = pi.GetCustomAttributes(typeof(DataColumnAttribute), false);

                if (attributes.Length == 1)
                {
                    var dma = attributes[0] as DataColumnAttribute;

                    if (dma != null)
                    {
                        //set to property name if field name not explicitly set
                        if (string.IsNullOrEmpty(dma.FieldName))
                            dma.FieldName = pi.Name;

                        fieldMappings.Add(new FieldMapping(pi, dma));
                    }
                }
            }

            return fieldMappings;
        }

        #endregion
    }
}