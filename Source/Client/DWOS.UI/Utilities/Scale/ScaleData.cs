using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.UI.Utilities.Scale
{
    /// <summary>
    /// Represents wieght & quantity data from a scale.
    /// </summary>
    /// <remarks>
    /// Nullable fields are optional and may not be present in scale data.
    /// </remarks>
    public sealed class ScaleData
    {
        /// <summary>
        /// Gets or sets the gross weight from the scale.
        /// </summary>
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// Gets or sets the tare weight from the calse.
        /// </summary>
        public decimal? TareWeight { get; set; }

        /// <summary>
        /// Gets or sets the net weight from the scale.
        /// </summary>
        public decimal? NetWeight { get; set; }

        /// <summary>
        /// Gets or sets the number of pieces on the scale.
        /// </summary>
        public int? Pieces { get; set; }

        public override string ToString()
        {
            return $"GrossWeight: {GrossWeight}, TareWeight: {TareWeight}, NetWeight: {NetWeight}, Pieces: {Pieces}";
        }
    }
}
