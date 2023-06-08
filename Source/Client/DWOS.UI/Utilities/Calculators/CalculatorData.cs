namespace DWOS.UI.Utilities.Calculators
{
    /// <summary>
    /// Contains data for use in <see cref="CalculatorWindow"/> and
    /// its subclasses.
    /// </summary>
    public sealed class CalculatorData
    {
        #region Properties

        /// <summary>
        /// Gets or sets the rate (number of parts per hour.)
        /// </summary>
        public decimal PartsPerHour { get; set; }

        public decimal LaborCost { get; set; }

        public decimal MaterialCost { get; set; }

        public decimal OverheadCost { get; set; }

        /// <summary>
        /// Gets or sets the surface area of the current part (in square inches).
        /// </summary>
        public double SurfaceArea { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the current step of
        /// price calculation
        /// </summary>
        public CalculationStep Step { get; set; }

        /// <summary>
        /// Gets or sets an identifier for the calculation type.
        /// </summary>
        public string CalculationType { get; set; }

        /// <summary>
        /// Gets or sets existing JSON-formatted data.
        /// </summary>
        public string JsonData { get; set; }

        #endregion
    }
}
