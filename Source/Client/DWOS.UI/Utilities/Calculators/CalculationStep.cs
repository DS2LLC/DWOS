namespace DWOS.UI.Utilities.Calculators
{
    /// <summary>
    /// Represents a price calculation step.
    /// </summary>
    public enum CalculationStep
    {
        /// <summary>
        /// Per-Hour Rate (typically parts per hour)
        /// </summary>
        Rate,

        /// <summary>
        /// Labor cost for a single unit
        /// </summary>
        Labor,

        /// <summary>
        /// Markup for a single unit.
        /// </summary>
        Markup,

        /// <summary>
        /// Overhead cost for a single unit.
        /// </summary>
        Overhead,

        /// <summary>
        /// Material cost for a single unit.
        /// </summary>
        Material
    }
}
