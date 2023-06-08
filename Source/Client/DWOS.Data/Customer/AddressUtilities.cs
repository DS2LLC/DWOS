namespace DWOS.Data.Customer
{
    /// <summary>
    /// Defines address-related utility methods.
    /// </summary>
    public static class AddressUtilities
    {
        #region Constants

        /// <summary>
        /// Country ID that represents an unknown country.
        /// </summary>
        public const int COUNTRY_ID_UNKNOWN = 0;

        /// <summary>
        /// Country ID that represents the United States of America.
        /// </summary>
        public const int COUNTRY_ID_USA = 1;

        /// <summary>
        /// Country ID that represents Canada.
        /// </summary>
        public const int COUNTRY_ID_CANADA = 2;

        /// <summary>
        /// Country ID that represents Mexico.
        /// </summary>
        public const int COUNTRY_ID_MEXICO = 3;

        #endregion

        #region Methods

        /// <summary>
        /// Returns a system-defined Country ID for a state/province
        /// abbreviation.
        /// </summary>
        /// <param name="abbreviation"></param>
        /// <returns></returns>
        public static int GetCountryId(string abbreviation)
        {
            if (string.IsNullOrWhiteSpace(abbreviation))
            {
                return COUNTRY_ID_UNKNOWN;
            }

            if (IsInUnitedStates(abbreviation))
            {
                return COUNTRY_ID_USA;
            }

            if (IsInCanada(abbreviation))
            {
                return COUNTRY_ID_CANADA;
            }

            return COUNTRY_ID_UNKNOWN;
        }

        /// <summary>
        /// Returns a value indicating if the abbreviation represents a state
        /// or territory in the United States.
        /// </summary>
        /// <param name="abbreviation">
        /// The state/territory/province abbreviation.
        /// </param>
        /// <returns>
        /// <c>true</c> if the abbreviation represents a state or US territory;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInUnitedStates(string abbreviation)
        {
            if (string.IsNullOrWhiteSpace(abbreviation))
            {
                return false;
            }

            switch (abbreviation.ToUpperInvariant())
            {
                case "AK":
                case "AL":
                case "AR":
                case "AS":
                case "AZ":
                case "CA":
                case "CO":
                case "CT":
                case "DC":
                case "DE":
                case "FL":
                case "FM":
                case "GA":
                case "GU":
                case "HI":
                case "IA":
                case "ID":
                case "IL":
                case "IN":
                case "KS":
                case "KY":
                case "LA":
                case "MA":
                case "MD":
                case "ME":
                case "MH":
                case "MI":
                case "MN":
                case "MO":
                case "MP":
                case "MS":
                case "MT":
                case "NC":
                case "ND":
                case "NE":
                case "NH":
                case "NJ":
                case "NM":
                case "NV":
                case "NY":
                case "OH":
                case "OK":
                case "OR":
                case "PA":
                case "PR":
                case "PW":
                case "RI":
                case "SC":
                case "SD":
                case "TN":
                case "TX":
                case "UT":
                case "VA":
                case "VI":
                case "VT":
                case "WA":
                case "WI":
                case "WV":
                case "WY":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a value indicating if the abbreviation represents a province
        /// or territory in Canada.
        /// </summary>
        /// <param name="abbreviation">
        /// The state/territory/province abbreviation.
        /// </param>
        /// <returns>
        /// <c>true</c> if the abbreviation represents a Canadian province
        /// or territory; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInCanada(string abbreviation)
        {
            if (string.IsNullOrWhiteSpace(abbreviation))
            {
                return false;
            }

            switch (abbreviation.ToUpperInvariant())
            {
                case "AB":
                case "BC":
                case "MB":
                case "NB":
                case "NL":
                case "NT":
                case "NS":
                case "NU":
                case "ON":
                case "PE":
                case "QC":
                case "SK":
                case "YT":
                    return true;

                default:
                    return false;
            }
        }

        #endregion
    }
}