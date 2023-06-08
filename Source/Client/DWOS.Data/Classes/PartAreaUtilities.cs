using DWOS.Data.Datasets;
using System.Linq;

namespace DWOS.Data
{
    /// <summary>
    /// Contains utility methods related to part dimensions.
    /// </summary>
    public static class PartAreaUtilities
    {
        #region Fields

        public const string SHAPE_BOX = "Box";
        public const string SHAPE_CYLINDER = "Cylinder";
        public const string SHAPE_RECTANGLE = "Rectangle";
        public const string SHAPE_SPHERE = "Sphere";
        public const string SHAPE_MANUAL = "Manual";
        public const string SHAPE_LENGTH = "Length";
        public const string SHAPE_ROTOR = "Rotor";

        private const string DIMENSION_FORMAT = "F2";
        private const string NONE = "None";

        #endregion

        #region Methods

        /// <summary>
        /// Constructs a part dimension string for the given part.
        /// </summary>
        /// <param name="part">The part to generate a string for.</param>
        /// <returns>
        /// If part is null, returns an empty string. Otherwise, returns a
        /// string representation of the part's dimensions.
        /// </returns>
        public static string PartDimensionString(OrderProcessingDataSet.PartRow part, bool includeNewline=true)
        {
            if (part == null)
            {
                return string.Empty;
            }

            var space = includeNewline ? "\n" : " ";

            var shapeType = part.IsShapeTypeNull() ?
                string.Empty :
                part.ShapeType;

            string returnValue;

            switch (shapeType)
            {
                case SHAPE_BOX:
                    returnValue = part.Length.ToString(DIMENSION_FORMAT) + "\" L X " +
                        part.Width.ToString(DIMENSION_FORMAT) + "\" W X" + space +
                        part.Height.ToString(DIMENSION_FORMAT) + "\" H";

                    break;
                case SHAPE_CYLINDER:
                case SHAPE_ROTOR:
                    returnValue = part.Width.ToString(DIMENSION_FORMAT) + "\" D X " +
                        part.Length.ToString(DIMENSION_FORMAT) + "\" L";

                    break;
                case SHAPE_RECTANGLE:
                    returnValue = part.Length.ToString(DIMENSION_FORMAT) + "\" L X " +
                        part.Width.ToString(DIMENSION_FORMAT) + "\" W";
                    break;
                case SHAPE_SPHERE:
                    returnValue = part.Width.ToString(DIMENSION_FORMAT) + "\" D";
                    break;
                case SHAPE_MANUAL:
                    returnValue = ManualPartDimensionString(part);
                    break;
                case SHAPE_LENGTH:
                    returnValue = part.Length.ToString(DIMENSION_FORMAT) + "\" L";
                    break;
                default:
                    returnValue = NONE;
                    break;
            }

            return returnValue;
        }

        private static string ManualPartDimensionString(OrderProcessingDataSet.PartRow part)
        {
            PartsDataset dsParts = null;

            try
            {
                if (part == null)
                {
                    return string.Empty;
                }

                dsParts = new PartsDataset()
                {
                    EnforceConstraints = false
                };

                using (var taPartArea = new Datasets.PartsDatasetTableAdapters.PartAreaTableAdapter())
                {
                    taPartArea.FillByPartID(dsParts.PartArea, part.PartID);
                }

                var partAreaRow = dsParts.PartArea.OrderByDescending(i => i.PartAreaID).FirstOrDefault();

                var returnValue = NONE;

                if (partAreaRow != null)
                {
                    using (var taPartAreaDimension = new Datasets.PartsDatasetTableAdapters.PartAreaDimensionTableAdapter())
                    {
                        taPartAreaDimension.FillByPartAreaID(dsParts.PartAreaDimension, partAreaRow.PartAreaID);
                    }

                    if (dsParts.PartAreaDimension.Count > 0)
                    {
                        returnValue = PartDimensionString(partAreaRow);
                    }
                }

                return returnValue;
            }
            finally
            {
                dsParts?.Dispose();
            }
        }

        private static string PartDimensionString(PartsDataset.PartAreaRow partAreaRow)
        {
            if (partAreaRow == null)
            {
                return string.Empty;
            }
            else if (partAreaRow.GetPartAreaDimensionRows().Length == 0)
            {
                return NONE;
            }

            var returnValue = string.Empty;
            var previousDimensionCount = 0;

            foreach (var dimension in partAreaRow.GetPartAreaDimensionRows())
            {
                if (previousDimensionCount > 0)
                {
                    // Newlines after every other dimension
                    if (previousDimensionCount % 2 == 0)
                    {
                        returnValue += " X\n";
                    }
                    else
                    {

                        returnValue += " X ";
                    }
                }

                var dimensionString = dimension.Dimension.ToString(DIMENSION_FORMAT) +
                    "\" " +
                    FormatDimensionName(dimension.DimensionName);

                returnValue += dimensionString;
                previousDimensionCount++;
            }

            return returnValue;
        }

        private static string FormatDimensionName(string dimensionName)
        {
            switch (dimensionName)
            {
                case "D1":
                    return "OD";
                case "D2":
                    return "ID";
                default:
                    return dimensionName;
            }
        }

        #endregion
    }
}
