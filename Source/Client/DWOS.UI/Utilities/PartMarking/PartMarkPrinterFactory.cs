using DWOS.Data;
using System;

namespace DWOS.UI.Utilities.PartMarking
{
    /// <summary>
    /// Provides a factory method to create new <see cref="IPartMarkDevice"/>
    /// instances.
    /// </summary>
    public static class PartMarkPrinterFactory
    {
        #region Methods

        /// <summary>
        /// Returns a new <see cref="IPartMarkDevice"/> instance.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IPartMarkDevice NewPrinter(PartMarkSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var deviceType = (PartMarkingDeviceType)Enum.Parse(typeof(PartMarkingDeviceType), settings.DeviceType);

            switch (deviceType)
            {
                case PartMarkingDeviceType.VideoJet1000Line:
                    return new VideoJet1000PartMarkDevice(settings);
                case PartMarkingDeviceType.VideoJetExcel:
                    return new VideoJetPartMarkDevice(settings);
                default:
                    return null;
            }
        }

        #endregion
    }
}
