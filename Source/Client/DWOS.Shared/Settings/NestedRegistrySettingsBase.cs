using Microsoft.Win32;
using System;

namespace DWOS.Shared.Settings
{
    /// <summary>
    /// Base class for settings that are stored within
    /// <see cref="RegistrySettingsBase"/>.
    /// </summary>
    /// <remarks>
    /// Inheriting classes need to implement a constructor that takes
    /// <see cref="RegistryKey"/> and <see cref="string"/> instances as
    /// parameters in that exact order.
    /// <see cref="InstanceOf(Type, RegistryKey, string)"/> creates instances
    /// using that constructor.
    ///
    /// This saves values in a subkey of the parent
    /// <see cref="RegistrySettingsBase"/> instance.
    /// </remarks>
    public abstract class NestedRegistrySettingsBase : RegistrySettingsBase
    {
        #region Fields

        #endregion

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="NestedRegistrySettingsBase"/> class.
        /// </summary>
        /// <param name="hive"></param>
        /// <param name="baseKey"></param>
        protected NestedRegistrySettingsBase(RegistryKey hive, string baseKey)
        {
            if (hive == null)
            {
                throw new ArgumentNullException(nameof(hive));
            }

            if (string.IsNullOrEmpty(baseKey))
            {
                throw new ArgumentNullException(nameof(baseKey));
            }

            RegistryKeyName = baseKey;
            RegistryHive = hive;
        }

        protected override string RegistryKeyName { get; }

        protected override RegistryKey RegistryHive { get; }

        /// <summary>
        /// Initializes a new instance of the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="hive">The registry hive to use.</param>
        /// <param name="baseKey">The registry base key to use.</param>
        /// <returns></returns>
        public static NestedRegistrySettingsBase InstanceOf(Type type, RegistryKey hive, string baseKey)
        {
            if (type == null || !typeof(NestedRegistrySettingsBase).IsAssignableFrom(type))
            {
                throw new ArgumentException(@"type is not a subclass of NestedRegistrySettingsBase", nameof(type));
            }

            if (hive == null)
            {
                throw new ArgumentNullException(nameof(hive));
            }

            if (string.IsNullOrEmpty(baseKey))
            {
                throw new ArgumentNullException(nameof(baseKey));
            }

            return Activator.CreateInstance(type, hive, baseKey) as NestedRegistrySettingsBase;
        }
    }
}
