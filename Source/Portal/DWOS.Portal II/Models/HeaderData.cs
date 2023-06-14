namespace DWOS.Portal.Models
{
    /// <summary>
    /// Represents data to show in the header.
    /// </summary>
    public class HeaderData
    {
        public string Logo { get; set; }

        public string CompanyName { get; set; }

        public string Tagline { get; set; }

        public double TimezoneOffsetMinutes { get; set; }

        public string Timezone { get; set; }
    }
}