using System.Collections.Generic;

namespace DWOS.LicenseManager
{
    /// <summary>
    ///   Determines the information about the licenses current status.
    /// </summary>
    public class LicenseSummary
    {
        public int TotalActivations { get; set; }

        public int AvaliableActivations { get; set; }

        public List<UserActivation> CurrentActivations { get; set; }
    }
}