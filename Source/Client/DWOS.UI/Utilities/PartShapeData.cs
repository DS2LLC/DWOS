using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Bindable class representing part shape.
    /// </summary>
    public sealed class PartShapeData : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Fields

        /// <summary>
        /// Maximum value for total surface area.
        /// </summary>
        /// <remarks>
        /// This is the maximum value of PartShapeWidget's surface area input.
        /// </remarks>
        public const double MAX_TOTAL_SURFACE_AREA = 9999999d;

        /// <summary>
        /// Minimum value for total surface area.
        /// </summary>
        public const double MIN_TOTAL_SURFACE_AREA = 0d;

        /// <summary>
        /// List of selectable shapes.
        /// </summary>
        public static readonly ReadOnlyCollection<string> SHAPES =
            new ReadOnlyCollection<string>(new List<string>
            {
                "Box",
                "Cone",
                "Cylinder",
                "Donut",
                "Plane",
                "Pyramid",
                "Sphere",
                "Rotor",
                "Washer",
                "SteelStamping",
                "SteelWire",
                "Screw",
                "Bolt",
                "HexNut"
            });

        private const UnitOfMeasure DEFAULT_UNIT_OF_MEASURE = UnitOfMeasure.Inch;

        /// <summary>
        /// Precision to use for rounding values.
        /// </summary>
        /// <remarks>
        /// Loss of precision occurs if totals are not rounded with the same
        /// precision used by DistanceUOM to do conversions.
        /// </remarks>
        private const int ROUNDING_PRECISION = PartShapeWidget.DistanceUOM.PRECISION;

        private string _shape;
        private double _grossArea;
        private double _excludedArea;
        private readonly BindingList<Dimension> _dimensions;
        private UnitOfMeasure _distanceType;
        private DataRow _originalData;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of Dimensions used to calculate area.
        /// </summary>
        public BindingList<Dimension> Dimensions
        {
            get
            {
                return _dimensions;
            }
        }

        /// <summary>
        /// Gets or sets the shape of the current part.
        /// </summary>
        /// <remarks>
        /// When the shape changes, this automatically populates the
        /// Dimensions list with shape-appropriate values.
        /// </remarks>
        public string ShapeType
        {
            get
            {
                return _shape;
            }
            set
            {
                if (_shape != value)
                {
                    _shape = value;
                    OnPropertyChanged("ShapeType");

                    Dimensions.Clear();
                    switch (value)
                    {
                        case "Box":
                            Dimensions.Add(new Dimension("L", _distanceType));
                            Dimensions.Add(new Dimension("W", _distanceType));
                            Dimensions.Add(new Dimension("H", _distanceType));
                            break;
                        case "Cone":
                            Dimensions.Add(new Dimension("D1", _distanceType));
                            Dimensions.Add(new Dimension("H", _distanceType));
                            break;
                        case "Cylinder":
                        case "Washer":
                            Dimensions.Add(new Dimension("D1", _distanceType));
                            Dimensions.Add(new Dimension("D2", _distanceType));
                            Dimensions.Add(new Dimension("H", _distanceType));
                            break;
                        case "Rotor":
                            Dimensions.Add(new Dimension("L", _distanceType));
                            Dimensions.Add(new Dimension("D1", _distanceType)); // major/largest
                            break;
                        case "Donut":
                            Dimensions.Add(new Dimension("D1", _distanceType)); // outer
                            Dimensions.Add(new Dimension("D2", _distanceType)); // inner
                            break;
                        case "Plane":
                            Dimensions.Add(new Dimension("L", _distanceType));
                            Dimensions.Add(new Dimension("W", _distanceType));
                            break;
                        case "Pyramid":
                            Dimensions.Add(new Dimension("L", _distanceType));
                            Dimensions.Add(new Dimension("W", _distanceType));
                            Dimensions.Add(new Dimension("H", _distanceType));
                            break;
                        case "Sphere":
                            Dimensions.Add(new Dimension("D1", _distanceType));
                            break;
                        case "SteelStamping": // Estimated
                            Dimensions.Add(new Dimension("W", UnitOfMeasure.Pound)); // Weight
                            Dimensions.Add(new Dimension("T", _distanceType)); // Thickness
                            break;
                        case "SteelWire": // Estimated
                            Dimensions.Add(new Dimension("W", UnitOfMeasure.Pound)); // Weight
                            Dimensions.Add(new Dimension("T", _distanceType)); // Thickness
                            break;
                        case "Screw": // Estimated
                        case "Bolt":
                            Dimensions.Add(new Dimension("D1", _distanceType)); // Diameter of head
                            Dimensions.Add(new Dimension("H", _distanceType)); // Height of head
                            Dimensions.Add(new Dimension("D2", _distanceType)); // Diameter of thread
                            Dimensions.Add(new Dimension("L", _distanceType)); // Length of thread
                            break;
                        case "HexNut": // Estimated
                            Dimensions.Add(new Dimension("L", _distanceType)); // Length of one side
                            Dimensions.Add(new Dimension("H", _distanceType));
                            Dimensions.Add(new Dimension("D", _distanceType)); //Diameter of hole
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the gross area in the current unit of measure.
        /// </summary>
        public double GrossSurfaceArea
        {
            get
            {
                return _grossArea;
            }
            private set
            {
                if (_grossArea != value)
                {
                    _grossArea = value;
                    OnPropertyChanged("GrossSurfaceArea");

                    // Update calculated property
                    OnPropertyChanged("TotalSurfaceArea");

                    // ExclusionSurfaceArea's validation depends on gross area
                    OnPropertyChanged("ExclusionSurfaceArea");
                }
            }
        }

        /// <summary>
        /// Gets or sets the exclusion area in the current unit of measure.
        /// </summary>
        public double ExclusionSurfaceArea
        {
            get
            {
                return _excludedArea;
            }
            set
            {
                if (_excludedArea != value)
                {
                    _excludedArea = value;
                    OnPropertyChanged("ExclusionSurfaceArea");

                    // Update calculated property
                    OnPropertyChanged("TotalSurfaceArea");
                }
            }
        }

        /// <summary>
        /// Gets the total surface area of the current part.
        /// </summary>
        public double TotalSurfaceArea
        {
            get
            {
                return Math.Round(_grossArea - _excludedArea, ROUNDING_PRECISION);
            }
        }

        /// <summary>
        /// Gets or sets distance type.
        /// </summary>
        public UnitOfMeasure DistanceType
        {
            get
            {
                return _distanceType;
            }
            set
            {
                if (_distanceType != value)
                {
                    var oldUnitOfMeasure = _distanceType;

                    _distanceType = value;
                    OnPropertyChanged("DistanceType");

                    // Update data given in units

                    var oldExcludedArea = _excludedArea;
                    _excludedArea = Math.Round(PartShapeWidget.DistanceUOM.Convert(oldExcludedArea, oldUnitOfMeasure, _distanceType, true), ROUNDING_PRECISION);
                    OnPropertyChanged("ExclusionSurfaceArea");

                    foreach (var dimension in Dimensions)
                    {
                        if (dimension.DistanceType == UnitOfMeasure.Pound)
                        {
                            // Do not adjust weight dimensions
                            continue;
                        }

                        var oldMeasurement = dimension.Measurement;

                        dimension.DistanceType = _distanceType;

                        dimension.Measurement = Math.Round(PartShapeWidget.DistanceUOM.Convert(oldMeasurement, oldUnitOfMeasure, _distanceType, false), ROUNDING_PRECISION);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the data row used to initially populate this instance.
        /// </summary>
        public DataRow OriginalData
        {
            get
            {
                return _originalData;
            }
            set
            {
                if (_originalData != value)
                {
                    _originalData = value;
                    OnPropertyChanged("OriginalData");
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PartShapeData() : this(new BindingList<Dimension>())
        {
        }

        /// <summary>
        /// Constructor taking a list of dimensions as a parameter.
        /// </summary>
        /// <param name="dimensions">list of dimensions to use</param>
        private PartShapeData(BindingList<Dimension> dimensions)
        {
            _dimensions = dimensions;
            Dimensions.ListChanged += Dimensions_ListChanged;
            _distanceType = DEFAULT_UNIT_OF_MEASURE;
        }


        /// <summary>
        /// Returns a deep copy of this instance.
        /// </summary>
        /// <returns>deep copy</returns>
        public PartShapeData Copy()
        {
            var newDimensions = new BindingList<Dimension>();

            foreach (var dimension in Dimensions)
            {
                newDimensions.Add(dimension.Copy());
            }

            var returnData = new PartShapeData(newDimensions)
            {
                _excludedArea = this._excludedArea,
                _grossArea = this._grossArea,
                _shape = this._shape,
                _distanceType = this._distanceType,
                OriginalData = this.OriginalData
            };

            return returnData;
        }

        /// <summary>
        /// Returns an instance using information from partRow.
        /// </summary>
        /// <param name="partRow">database data</param>
        /// <returns>a new instance</returns>
        public static PartShapeData From(DWOS.Data.Datasets.PartsDataset.PartRow partRow)
        {
            PartShapeData returnValue = null;
            if (partRow != null)
            {
                var areaRows = partRow.GetPartAreaRows().OrderByDescending(i => i.PartAreaID);

                if (areaRows.Count() > 0)
                {
                    var areaRowToUse = areaRows.FirstOrDefault();

                    returnValue = new PartShapeData()
                    {
                        ShapeType = areaRowToUse.ShapeType,
                        ExclusionSurfaceArea = areaRowToUse.ExclusionSurfaceArea,
                        DistanceType = UnitOfMeasure.Inch,
                        OriginalData = areaRowToUse
                    };

                    returnValue.Dimensions.Clear();

                    foreach (var persistedDimension in areaRowToUse.GetPartAreaDimensionRows())
                    {
                        var isWeightMeasurement = persistedDimension.DimensionName == "W" &&
                            (areaRowToUse.ShapeType == "SteelStamping" || areaRowToUse.ShapeType == "SteelWire" );

                        var unitOfMeasure = isWeightMeasurement ? UnitOfMeasure.Pound : UnitOfMeasure.Inch;

                        returnValue.Dimensions.Add(new Dimension(persistedDimension.DimensionName, unitOfMeasure)
                        {
                            Measurement = persistedDimension.Dimension,
                            OriginalData = persistedDimension
                        });
                    }
                }

            }

            return returnValue;
        }

        /// <summary>
        /// Returns an instance using information from quotePartRow.
        /// </summary>
        /// <param name="quotePartRow">database data</param>
        /// <returns>a new instance</returns>
        public static PartShapeData From(DWOS.Data.Datasets.QuoteDataSet.QuotePartRow quotePartRow)
        {
            PartShapeData returnValue = null;
            if (quotePartRow != null)
            {
                var areaRows = quotePartRow.GetQuotePartAreaRows().OrderByDescending(i => i.QuotePartAreaID);

                if (areaRows.Count() > 0)
                {
                    var areaRowToUse = areaRows.FirstOrDefault();

                    returnValue = new PartShapeData()
                    {
                        ShapeType = areaRowToUse.ShapeType,
                        ExclusionSurfaceArea = areaRowToUse.ExclusionSurfaceArea,
                        DistanceType = UnitOfMeasure.Inch,
                        OriginalData = areaRowToUse
                    };

                    returnValue.Dimensions.Clear();

                    foreach (var persistedDimension in areaRowToUse.GetQuotePartAreaDimensionRows())
                    {
                        var isWeightMeasurement = persistedDimension.DimensionName == "W" &&
                            (areaRowToUse.ShapeType == "SteelStamping" || areaRowToUse.ShapeType == "SteelWire" );

                        var unitOfMeasure = isWeightMeasurement ? UnitOfMeasure.Pound : UnitOfMeasure.Inch;

                        returnValue.Dimensions.Add(new Dimension(persistedDimension.DimensionName, unitOfMeasure)
                        {
                            Measurement = persistedDimension.Dimension,
                            OriginalData = persistedDimension
                        });
                    }
                }

            }

            return returnValue;
        }

        private void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// Surface area for a box.
        /// </summary>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        private static double CalculateAreaBox(IEnumerable<Dimension> dimensions)
        {
            Dimension lengthDimension = dimensions.FirstOrDefault(d => d.Name == "L");
            Dimension widthDimension = dimensions.FirstOrDefault(d => d.Name == "W");
            Dimension heightDimension = dimensions.FirstOrDefault(d => d.Name == "H");

            if (lengthDimension == null || widthDimension == null || heightDimension == null)
            {
                return 0;
            }

            var length = lengthDimension.Measurement;
            var width = widthDimension.Measurement;
            var height = heightDimension.Measurement;

            return (2* length * height) +  (2 * width * length) + (2 * width * height);
        }

        /// <summary>
        /// Surface area for a cone.
        /// </summary>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        private static double CalculateAreaCone(IEnumerable<Dimension> dimensions)
        {
            Dimension diameterDimension = dimensions.FirstOrDefault(d => d.Name == "D1");
            Dimension heightDimension = dimensions.FirstOrDefault(d => d.Name == "H");

            double returnValue;
            if (diameterDimension == null || heightDimension == null)
            {
                returnValue = 0;
            }
            else
            {
                var radius = diameterDimension.Measurement / 2;
                var height = heightDimension.Measurement;
                var slantHeight = Math.Sqrt(radius * radius + height * height);

                returnValue = Math.PI * radius * (radius + slantHeight);
            }

            return returnValue;
        }

        /// <summary>
        /// Surface area for a hollow cylinder.
        /// </summary>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        private static double CalculateAreaCylinder(IEnumerable<Dimension> dimensions)
        {
            Dimension outerDiameterDimension = dimensions.FirstOrDefault(d => d.Name == "D1");
            Dimension innerDiameterDimension = dimensions.FirstOrDefault(d => d.Name == "D2");
            Dimension heightDimension = dimensions.FirstOrDefault(d => d.Name == "H");

            double returnValue;
            if (outerDiameterDimension == null || innerDiameterDimension == null || heightDimension == null)
            {
                returnValue = 0;
            }
            else
            {
                var outerRadius = outerDiameterDimension.Measurement / 2;
                var innerRadius = innerDiameterDimension.Measurement / 2;
                var height = heightDimension.Measurement;

                returnValue = (2 * Math.PI) *
                    ((innerRadius * height) + (outerRadius * height) + (outerRadius * outerRadius) - (innerRadius * innerRadius));
            }


            return returnValue;
        }

        /// <summary>
        /// Surface area for a rotor (similar to a cylinder, but surface area
        /// is not calculated for its ends).
        /// </summary>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        private static double CalculateAreaRotor(IList<Dimension> dimensions)
        {
            var outerDiameterDimension = dimensions.FirstOrDefault(d => d.Name == "D1");
            var lengthDimension = dimensions.FirstOrDefault(d => d.Name == "L");

            double returnValue;
            if (outerDiameterDimension == null || lengthDimension == null)
            {
                returnValue = 0;
            }
            else
            {
                var outerDiameter = outerDiameterDimension.Measurement;
                var dimension = lengthDimension.Measurement;

                returnValue = Math.PI * outerDiameter * dimension;
            }


            return returnValue;
        }

        /// <summary>
        /// Surface area for a donut (ring torus).
        /// </summary>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        private static double CalculateAreaDonut(IEnumerable<Dimension> dimensions)
        {
            var outerDiameterDimension = dimensions.FirstOrDefault(d => d.Name == "D1");
            var innerDiameterDimension = dimensions.FirstOrDefault(d => d.Name == "D2");

            double returnValue;

            if (outerDiameterDimension == null || innerDiameterDimension == null)
            {
                returnValue = 0;
            }
            else
            {
                var minorRadius = (outerDiameterDimension.Measurement - innerDiameterDimension.Measurement) / 4;
                var majorRadius = (innerDiameterDimension.Measurement / 2) + minorRadius;
                returnValue = 4 * Math.PI * Math.PI * majorRadius * minorRadius;
            }

            return returnValue;
        }

        /// <summary>
        /// Surface area for a plane.
        /// </summary>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        private static double CalculateAreaPlane(IEnumerable<Dimension> dimensions)
        {
            Dimension lengthDimension = dimensions.FirstOrDefault(d => d.Name == "L");
            Dimension widthDimension = dimensions.FirstOrDefault(d => d.Name == "W");

            if (lengthDimension == null || widthDimension == null)
            {
                return 0;
            }

            var length = lengthDimension.Measurement;
            var width = widthDimension.Measurement;

            return length * width;
        }

        /// <summary>
        /// Surface area for a right rectangular pyramid.
        /// </summary>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        private static double CalculateAreaPyramid(IEnumerable<Dimension> dimensions)
        {
            Dimension lengthDimension = dimensions.FirstOrDefault(d => d.Name == "L");
            Dimension widthDimension = dimensions.FirstOrDefault(d => d.Name == "W");
            Dimension heightDimension = dimensions.FirstOrDefault(d => d.Name == "H");

            double returnValue;
            if (lengthDimension == null || widthDimension == null || heightDimension == null)
            {
                returnValue = 0;
            }
            else
            {
                var length = lengthDimension.Measurement;
                var width = widthDimension.Measurement;
                var height = heightDimension.Measurement;

                returnValue = length * width +
                    (length * Math.Sqrt(((width / 2) * (width / 2)) + (height * height))) +
                    (width * Math.Sqrt(((length / 2) * (length / 2) + (height * height))));
            }

            return returnValue;
        }

        /// <summary>
        /// Surface area for a sphere.
        /// </summary>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        private static double CalculateAreaSphere(IEnumerable<Dimension> dimensions)
        {
            Dimension diameterDimension = dimensions.FirstOrDefault(d => d.Name == "D1");

            if (diameterDimension == null)
            {
                return 0;
            }

            var radius = diameterDimension.Measurement / 2;

            return (4 * Math.PI * radius * radius);
        }

        /// <summary>
        /// Estimates surface area of a steel stamping.
        /// </summary>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        private static double EstimateAreaSteelStamping(ICollection<Dimension> dimensions)
        {
            const double conversionFactor = 7.030674616;

            var weightDimension = dimensions.FirstOrDefault(d => d.Name == "W");
            var thicknessDimension = dimensions.FirstOrDefault(d => d.Name == "T");

            if (weightDimension == null || thicknessDimension == null)
            {
                return 0;
            }

            var thickness = thicknessDimension.Measurement;
            var weight = weightDimension.Measurement;

            if (thickness > 0)
            {
                return (weight / thickness) * conversionFactor;
            }

            return 0;
        }

        /// <summary>
        /// Estimates surface area of a steel wire.
        /// </summary>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        private static double EstimateAreaSteelWire(ICollection<Dimension> dimensions)
        {
            const double conversionFactor = 14.089029053;

            var weightDimension = dimensions.FirstOrDefault(d => d.Name == "W");
            var thicknessDimension = dimensions.FirstOrDefault(d => d.Name == "T");

            if (weightDimension == null || thicknessDimension == null)
            {
                return 0;
            }

            var thickness = thicknessDimension.Measurement;
            var weight = weightDimension.Measurement;

            if (thickness > 0)
            {
                return (weight / thickness) * conversionFactor;
            }

            return 0;
        }

        private static double EstimateAreaScrew(ICollection<Dimension> dimensions)
        {
            // Estimate developed based on finishing.com message board thread: https://www.finishing.com/2600-2799/2630.shtml
            const double threadedFormMultiplier = 1.75d;

            var diameterHeadMeasurement = dimensions.FirstOrDefault(d => d.Name == "D1");
            var heightHeadMeasurement = dimensions.FirstOrDefault(d => d.Name == "H");
            var diameterThreadMeasurement = dimensions.FirstOrDefault(d => d.Name == "D2");
            var lengthThreadMeasurement = dimensions.FirstOrDefault(d => d.Name == "L");

            if (diameterHeadMeasurement == null || heightHeadMeasurement == null || diameterThreadMeasurement == null || lengthThreadMeasurement == null)
            {
                return 0;
            }

            var diameterHead = diameterHeadMeasurement.Measurement;
            var heightHead = heightHeadMeasurement.Measurement;

            var diameterThread = diameterThreadMeasurement.Measurement;
            var lengthThread = lengthThreadMeasurement.Measurement;

            var headArea = (Math.PI * diameterHead * heightHead) + (2 * Math.PI * Math.Pow(diameterHead/2, 2));
            var threadArea = Math.PI * diameterThread * lengthThread * threadedFormMultiplier;

            return headArea + threadArea;
        }

        private static double EstimateAreaHexNut(ICollection<Dimension> dimensions)
        {
            // Estimate developed based on finishing.com message board thread: https://www.finishing.com/2600-2799/2630.shtml
            const double threadedFormMultiplier = 1.75d;

            var lengthDimension = dimensions.FirstOrDefault(d => d.Name == "L");
            var heightDimension = dimensions.FirstOrDefault(d => d.Name == "H");
            var diameterDimension = dimensions.FirstOrDefault(d => d.Name == "D");

            if (lengthDimension == null || heightDimension == null || diameterDimension == null)
            {
                return 0;
            }

            var length = lengthDimension.Measurement;
            var height = heightDimension.Measurement;
            var diameter = diameterDimension.Measurement;

            var hexShape = (6 * length * height) + (3 * Math.Sqrt(3) * Math.Pow(length, 2));
            var hole = 2 * Math.PI * Math.Pow(diameter / 2, 2);
            var insideHole = Math.PI * diameter * height * threadedFormMultiplier;

            return hexShape - hole + insideHole;
        }

        #endregion

        #region Events

        void Dimensions_ListChanged(object sender, ListChangedEventArgs e)
        {
            // Ignore changes to Dimension unit of measure; they're only changed when
            // measurements change to match them
            if (Dimensions != null && (e.PropertyDescriptor == null || e.PropertyDescriptor.Name != "DistanceType"))
            {
                double newGrossArea;
                switch (_shape)
                {
                    case "Box":
                        newGrossArea = CalculateAreaBox(Dimensions);
                        break;
                    case "Cone":
                        newGrossArea = CalculateAreaCone(Dimensions);
                        break;
                    case "Cylinder":
                    case "Washer":
                        newGrossArea = CalculateAreaCylinder(Dimensions);
                        break;
                    case "Rotor":
                        newGrossArea = CalculateAreaRotor(Dimensions);
                        break;
                    case "Donut":
                        newGrossArea = CalculateAreaDonut(Dimensions);
                        break;
                    case "Plane":
                        newGrossArea = CalculateAreaPlane(Dimensions);
                        break;
                    case "Pyramid":
                        newGrossArea = CalculateAreaPyramid(Dimensions);
                        break;
                    case "Sphere":
                        newGrossArea = CalculateAreaSphere(Dimensions);
                        break;
                    case "SteelStamping":
                        newGrossArea = EstimateAreaSteelStamping(Dimensions);
                        break;
                    case "SteelWire":
                        newGrossArea = EstimateAreaSteelWire(Dimensions);
                        break;
                    case "Screw":
                    case "Bolt":
                        newGrossArea = EstimateAreaScrew(Dimensions);
                        break;
                    case "HexNut":
                        newGrossArea = EstimateAreaHexNut(Dimensions);
                        break;
                    default:
                        // Un-implemented shape
                        newGrossArea = 0;
                        break;
                }

                GrossSurfaceArea = Math.Round(newGrossArea, ROUNDING_PRECISION);
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region IDataErrorInfo Members

        public string Error
        {
            get
            {
                string exclusionSurfaceAreaError = this["ExclusionSurfaceArea"];
                string shapeTypeError = this["ShapeType"];
                string totalSurfaceAreaError = this["TotalSurfaceArea"];

                return exclusionSurfaceAreaError + shapeTypeError + totalSurfaceAreaError;
            }
        }

        public string this[string columnName]
        {
            get
            {
                string returnValue = null;

                if (columnName == "ExclusionSurfaceArea" && _excludedArea > _grossArea)
                {
                    returnValue = "Excluded area cannot be greater than gross area.";
                }
                else if (columnName == "TotalSurfaceArea" && TotalSurfaceArea > MAX_TOTAL_SURFACE_AREA)
                {
                    returnValue = "Total surface area must be under 10,000,000.";
                }
                else if (columnName == "TotalSurfaceArea" && TotalSurfaceArea < MIN_TOTAL_SURFACE_AREA)
                {
                    returnValue = "Total surface area cannot be negative.";
                }
                else if (columnName == "ShapeType" && !SHAPES.Any(s => s == _shape))
                {
                    returnValue = "Shape is not one of the known types.";
                }

                return returnValue;
            }
        }

        #endregion

        #region Dimension

        /// <summary>
        /// Bindable class representing a single dimension of a part's area.
        /// </summary>
        public sealed class Dimension : INotifyPropertyChanged
        {
            #region Fields

            private double _meaurement;
            private UnitOfMeasure _unitOfMeasure;
            private DataRow _originalData;

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets the short name.
            /// </summary>
            public string Name
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            /// <remarks>
            /// This is automatically generated using the name.
            /// </remarks>
            public string Description
            {
                get
                {
                    string returnValue;
                    switch (Name)
                    {
                        case "L":
                            returnValue = "Length";
                            break;
                        case "W":
                            returnValue = this.DistanceType == UnitOfMeasure.Pound ? "Weight" : "Width";
                            break;
                        case "H":
                            returnValue = "Height";
                            break;
                        case "D":
                            returnValue = "Diameter";
                            break;
                        case "D1":
                            returnValue = "Outer Diameter";
                            break;
                        case "D2":
                            returnValue = "Inner Diameter";
                            break;
                        case "T":
                            returnValue = "Thickness";
                            break;
                        default:
                            returnValue = string.Empty;
                            break;
                    }

                    return returnValue;
                }
            }

            /// <summary>
            /// Gets or sets the measurement's value in the current unit of measure.
            /// </summary>
            public double Measurement
            {
                get
                {
                    return _meaurement;
                }
                set
                {
                    if (_meaurement != value)
                    {
                        _meaurement = value;
                        OnPropertyChanged("Measurement");
                    }
                }
            }

            /// <summary>
            /// Gets or sets distance type.
            /// </summary>
            public UnitOfMeasure DistanceType
            {
                get
                {
                    return _unitOfMeasure;
                }
                set
                {
                    if (_unitOfMeasure != value)
                    {
                        _unitOfMeasure = value;
                        OnPropertyChanged("DistanceType");
                    }
                }
            }

            /// <summary>
            /// Gets or sets the data row used to initially populate this instance.
            /// </summary>
            public DataRow OriginalData
            {
                get
                {
                    return _originalData;
                }
                set
                {
                    if (_originalData != value)
                    {
                        _originalData = value;
                        OnPropertyChanged("OriginalData");
                    }
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="dimensionName">Name of the dimension</param>
            /// <param name="distanceType">Current unit of measure of a PartShapeData instance.</param>
            public Dimension(string dimensionName,UnitOfMeasure distanceType)
            {
                Name = dimensionName;
                _unitOfMeasure = distanceType;
            }

            /// <summary>
            /// Default, private constructor.
            /// </summary>
            private Dimension()
            {
            }

   
            /// <summary>
            /// Returns a deep copy of this instance.
            /// </summary>
            /// <returns>deep copy</returns>
            public Dimension Copy()
            {
                return new Dimension()
                {
                    Name = this.Name,
                    _meaurement = this._meaurement,
                    _unitOfMeasure = this._unitOfMeasure,
                    OriginalData = this.OriginalData
                };
            }

            private void OnPropertyChanged(string name)
            {
                var handler = PropertyChanged;

                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

    }
}
