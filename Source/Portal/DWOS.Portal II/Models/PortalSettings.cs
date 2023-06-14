namespace DWOS.Portal.Models
{
    /// <summary>
    /// Represents application settings.
    /// </summary>
    public class PortalSettings
    {
        public bool ShowSerialNumbers { get; set; }

        public bool ShowRequiredDate { get; set; }

        public bool ShowManufacturer { get; set; }

        public bool ShowTrackingNumber { get; set; }

        public bool ShowOrderApprovals { get; set; }

        public bool ShowLateOrderNotificationOption { get; set; }

        public int PriceDecimalPlaces { get; set; }
    }
}