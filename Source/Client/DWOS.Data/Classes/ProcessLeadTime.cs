namespace DWOS.Data
{
    public class ProcessLeadTime
    {
        #region Properties

        public decimal Hours { get; set; }

        public LeadTimeType Type { get; set; }

        #endregion

        #region Methods

        public ProcessLeadTime(decimal hours, LeadTimeType type)
        {
            Hours = hours;
            Type = type;
        }

        public decimal CalculateHours(int quantity)
        {
            switch (Type)
            {
                case LeadTimeType.Piece:
                    return Hours * quantity;
                default:
                    return Hours;
            }
        }

        #endregion
    }
}
