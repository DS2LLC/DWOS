using System.Collections.Generic;

namespace DWOS.Services.Messages
{
    #region Part

    /// <summary>
    /// Server response containing part information.
    /// </summary>
    public class PartDetailResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the part for this instance.
        /// </summary>
        public PartDetailInfo PartDetail { get; set; }
    }

    /// <summary>
    /// Represents a part.
    /// </summary>
    public class PartDetailInfo
    {
        /// <summary>
        /// Gets or sets the part ID for this instance.
        /// </summary>
        public int PartId { get; set; }

        /// <summary>
        /// Gets or sets the name for this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the part revision for this instance.
        /// </summary>
        public string Rev { get; set; }

        /// <summary>
        /// Gets or sets the customer name for this instance.
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer for this instance.
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the model for this instance.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the material for this instance.
        /// </summary>
        public string Material { get; set; }

        /// <summary>
        /// Gets or sets the dimensions for this instance.
        /// </summary>
        /// <value>
        /// A display string of the part's dimensions.
        /// </value>
        public string Dimensions { get; set; }

        /// <summary>
        /// Gets or sets notes for this instance.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets documents for this instance.
        /// </summary>
        public List<DocumentInfo> Documents { get; set; }

        /// <summary>
        /// Gets or sets media for this instance.
        /// </summary>
        public List<MediaSummary> Media { get; set; }

        /// <summary>
        /// Gets or sets custom fields for this instance.
        /// </summary>
        public List<PartCustomField> CustomFields { get; set; }
    }

    #endregion

    #region PartCustomField

    /// <summary>
    /// Represents a custom field for a part.
    /// </summary>
    public class PartCustomField
    {
        /// <summary>
        /// Gets or sets the custom field name for this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value for this instance.
        /// </summary>
        public string Value { get; set; }
    }

    #endregion
}
