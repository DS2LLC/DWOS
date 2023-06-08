namespace DWOS.UI.Sales.Models
{
    public class Part
    {
        #region Properties

        public int PartId { get; }

        public string Name { get; }

        public decimal? Weight { get; }

        public bool IsPartMarking { get; }

        #endregion

        #region Methods

        public Part(int partId, string name, decimal? weight, bool isPartMarking)
        {
            PartId = partId;
            Name = name;
            Weight = weight;
            IsPartMarking = isPartMarking;
        }

        #endregion
    }

}
