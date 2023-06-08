using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using System.Windows.Interop;
using DWOS.Data;
using DWOS.Shared;

namespace DWOS.UI.Utilities
{
    public partial class PartShapeWidget : UserControl
    {
        #region Fields

        private const string DEFAULT_SHAPE = PartAreaUtilities.SHAPE_BOX;

        private const string MODE_BASIC = "Basic";
        private const string MODE_ADVANCED = "Advanced";
        private const string DEFAULT_MODE = MODE_BASIC;

        /// <summary>
        /// Occurs when the surface area changes.
        /// </summary>
        public event EventHandler SurfaceAreaChanged;

        private PartsDataset.PartAreaDataTable _dtPartAreaTable;
        private PartsDataset.PartAreaDimensionDataTable _dtPartAreaDimensionTable;
        private QuoteDataSet.QuotePartAreaDataTable _dtQuotePartAreaTable;
        private QuoteDataSet.QuotePartAreaDimensionDataTable _dtQuotePartAreaDimensionTable;

        private NumericEditorUnitOfMeasure _lengthUOM;
        private NumericEditorUnitOfMeasure _widthUOM;
        private NumericEditorUnitOfMeasure _heightUOM;
        private NumericEditorUnitOfMeasure _surfaceAreaUOM;
        private bool _isInitialDataLoaded;
        private bool _isInAdvancedMode;
        private PartShapeData _partShapeData;

        #endregion

        #region Properties

        public bool IsRecordLoading { get; set; }
        private bool IsNewPart { get; set; }
        public DataRow CurrentPart { get; set; }

        public string LengthColumnName { get; set; }
        public string WidthColumnName { get; set; }
        public string HeightColumnName { get; set; }
        public string ShapeTypeColumnName { get; set; }
        public string SurfaceAreaColumnName { get; set; }

        /// <summary>
        /// Gets the current surface area (in square inches) from
        /// this control.
        /// </summary>
        public double SurfaceArea
        {
            get
            {
                if (_surfaceAreaUOM == null)
                {
                    return 0D;
                }

                return _surfaceAreaUOM.GetValue(UnitOfMeasure.Inch);
            }
        }

        #endregion

        #region Methods

        public PartShapeWidget()
        {
            InitializeComponent();

            this.LengthColumnName = "Length";
            this.WidthColumnName = "Width";
            this.HeightColumnName = "Height";
            this.ShapeTypeColumnName = "ShapeType";
            this.SurfaceAreaColumnName = "SurfaceArea";
        }

        public void LoadData()
        {
            cboPartShape.DisplayMember = nameof(ComboBoxItem.DisplayText);
            cboPartShape.ValueMember = nameof(ComboBoxItem.Value);

            cboPartShape.DataSource = new List<ComboBoxItem>
            {
                new ComboBoxItem(PartAreaUtilities.SHAPE_BOX, PartAreaUtilities.SHAPE_BOX),
                new ComboBoxItem(PartAreaUtilities.SHAPE_CYLINDER, PartAreaUtilities.SHAPE_CYLINDER),
                new ComboBoxItem(PartAreaUtilities.SHAPE_MANUAL, PartAreaUtilities.SHAPE_MANUAL),
                new ComboBoxItem(PartAreaUtilities.SHAPE_RECTANGLE, PartAreaUtilities.SHAPE_RECTANGLE),
                new ComboBoxItem(PartAreaUtilities.SHAPE_SPHERE, PartAreaUtilities.SHAPE_SPHERE),
                new ComboBoxItem(PartAreaUtilities.SHAPE_ROTOR, PartAreaUtilities.SHAPE_ROTOR),
                new ComboBoxItem(PartAreaUtilities.SHAPE_LENGTH, "(Length Only)")
            };

            this.cboEditorType.DataSource = new List<string>()
            {
                MODE_BASIC,
                MODE_ADVANCED
            };

            _lengthUOM      = new NumericEditorUnitOfMeasure(numLength, false);
            _lengthUOM.UoMDropDownList.SelectionChanged += UoMDropDown_SelectionChanged;
            _widthUOM       = new NumericEditorUnitOfMeasure(numWidth, false);
            _widthUOM.UoMDropDownList.SelectionChanged += UoMDropDown_SelectionChanged;
            _heightUOM      = new NumericEditorUnitOfMeasure(numHeight, false);
            _heightUOM.UoMDropDownList.SelectionChanged += UoMDropDown_SelectionChanged;
            _surfaceAreaUOM = new NumericEditorUnitOfMeasure(numSurfaceArea, true);
            _surfaceAreaUOM.UoMDropDownList.SelectionChanged += UoMDropDown_SelectionChanged;

            _surfaceAreaUOM.ValueChanged += SurfaceAreaUOM_ValueChanged;
            _isInitialDataLoaded = true;
        }

        public void AddValidators(DWOS.Utilities.Validation.ValidatorManager manager, ErrorProvider errProvider)
        {
            var fields = new Data.Datasets.ApplicationSettingsDataSet.FieldsDataTable();
            using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                ta.Fill(fields);

            var surfaceAreaField = fields.FirstOrDefault(f => f.Name == "Surface Area");
            if (surfaceAreaField == null || surfaceAreaField.IsRequired)
            {
                manager.Add(new ImageDisplayValidator(new NumericControlValidator(this.numWidth) { MinimumValue = .0001, ValidationRequired = ValidateSurfaceAreaField }, errProvider));
                manager.Add(new ImageDisplayValidator(new NumericControlValidator(this.numLength) { MinimumValue = .0001, ValidationRequired = ValidateSurfaceAreaField }, errProvider));
                manager.Add(new ImageDisplayValidator(new NumericControlValidator(this.numHeight) { MinimumValue = .0000, ValidationRequired = ValidateSurfaceAreaField }, errProvider));
                manager.Add(new ImageDisplayValidator(new NumericControlValidator(this.numSurfaceArea) { MinimumValue = .0001, ValidationRequired = () => this.IsNewPart }, errProvider));
            }
        }

        public void LoadRow(DataRow part)
        {
            IsRecordLoading = true;

            CurrentPart   = part;
            IsNewPart     = part != null && part.RowState == DataRowState.Added;
            _partShapeData = null;
            var shapeType = DEFAULT_SHAPE;
            var mode = DEFAULT_MODE;
            
            if (_isInitialDataLoaded)
            {
                if (part != null)
                {
                    var partRow = part as DWOS.Data.Datasets.PartsDataset.PartRow;
                    var quotePartRow = part as DWOS.Data.Datasets.QuoteDataSet.QuotePartRow;

                    if (partRow != null)
                    {
                        var areaRows = partRow.GetPartAreaRows();
                        mode = areaRows == null || areaRows.Length == 0 ? MODE_BASIC : MODE_ADVANCED;
                        _partShapeData = PartShapeData.From(partRow);
                    }
                    else if (quotePartRow != null)
                    {
                        var areaRows = quotePartRow.GetQuotePartAreaRows();
                        mode = areaRows == null || areaRows.Length == 0 ? MODE_BASIC : MODE_ADVANCED;
                        _partShapeData = PartShapeData.From(quotePartRow);
                    }

                    double length = 0d;
                    if (!part.IsNull(LengthColumnName))
                        length = Convert.ToDouble(part[LengthColumnName]);


                    double width = 0d;
                    if (!part.IsNull(WidthColumnName))
                        width = Convert.ToDouble(part[WidthColumnName]);


                    double height = 0d;
                    if (!part.IsNull(HeightColumnName))
                        height = Convert.ToDouble(part[HeightColumnName]);

                    _lengthUOM.SetValue(length, UnitOfMeasure.Inch);
                    _widthUOM.SetValue(width, UnitOfMeasure.Inch);
                    _heightUOM.SetValue(height, UnitOfMeasure.Inch);

                    if (!part.IsNull(ShapeTypeColumnName)) 
                        shapeType = part[ShapeTypeColumnName].ToString();

                    double surfaceArea = 0d;
                    UnitOfMeasure surfaceAreaDistanceType = UnitOfMeasure.Inch;

                    if (mode == MODE_BASIC && shapeType != "Manual")
                    {
                        surfaceArea = this.CalculateSurfaceArea(shapeType);
                    }
                    else if (!part.IsNull(SurfaceAreaColumnName))
                    {
                        const double useFeetThreshold = 24d;
                        surfaceArea = Convert.ToDouble(part[SurfaceAreaColumnName]);

                        if (mode == MODE_ADVANCED && surfaceArea >= useFeetThreshold)
                        {
                            // Workaround for not storing unit of measure with surface area.
                            surfaceArea = DistanceUOM.Convert(surfaceArea, UnitOfMeasure.Inch, UnitOfMeasure.Feet, true);
                            surfaceAreaDistanceType = UnitOfMeasure.Feet;
                        }
                    }

                    _surfaceAreaUOM.SetValue(surfaceArea, surfaceAreaDistanceType);
                }
                else
                {
                    _lengthUOM.SetValue(0, UnitOfMeasure.Inch);
                    _widthUOM.SetValue(0, UnitOfMeasure.Inch);
                    _heightUOM.SetValue(0, UnitOfMeasure.Inch);
                    _surfaceAreaUOM.SetValue(0, UnitOfMeasure.Inch);
                }
            }

            cboPartShape.SelectedItem = cboPartShape.FindItemByValue <string>(s => s == shapeType);
            cboEditorType.SelectedItem = cboEditorType.FindItemByValue<string>(t => t == mode);

            IsRecordLoading = false;
        }

        /// <summary>
        /// Load data tables for this instance.
        /// </summary>
        /// <remarks>
        /// PartShapeWidget is responsible for creating new PartArea and
        /// PartAreaDimension rows. Calling this is required to persist
        /// new records for those tables.
        /// </remarks>
        /// <param name="dtPartAreaTable"></param>
        /// <param name="dtPartAreaDimensionTable"></param>
        public void LoadDataTables(PartsDataset.PartAreaDataTable dtPartAreaTable, PartsDataset.PartAreaDimensionDataTable dtPartAreaDimensionTable)
        {
            _dtPartAreaTable = dtPartAreaTable;
            _dtPartAreaDimensionTable = dtPartAreaDimensionTable;
        }

        /// <summary>
        /// Load data tables for this instance.
        /// </summary>
        /// <remarks>
        /// PartShapeWidget is responsible for creating new QuotePartArea and
        /// QuotePartAreaDimension rows. Calling this is required to persist
        /// new records for those tables.
        /// </remarks>
        /// <param name="dtQuotePartAreaTable"></param>
        /// <param name="dtQuotePartAreaDimensionTable"></param>
        public void LoadDataTables(QuoteDataSet.QuotePartAreaDataTable dtQuotePartAreaTable, QuoteDataSet.QuotePartAreaDimensionDataTable dtQuotePartAreaDimensionTable)
        {
            _dtQuotePartAreaTable = dtQuotePartAreaTable;
            _dtQuotePartAreaDimensionTable = dtQuotePartAreaDimensionTable;
        }

        public void SaveRow()
        {
            if (_isInAdvancedMode && _partShapeData != null)
            {
                SaveRowAdvanced();
            }
            else
            {
                SaveRowBasic();
            }
        }

        private void SaveRowAdvanced()
        {
            try
            {
                var partRow = CurrentPart as DWOS.Data.Datasets.PartsDataset.PartRow;
                var quotePartRow = CurrentPart as DWOS.Data.Datasets.QuoteDataSet.QuotePartRow;

                var exclusionSurfaceArea = DistanceUOM.Convert(_partShapeData.ExclusionSurfaceArea, _partShapeData.DistanceType, UnitOfMeasure.Inch, true);
                var grossSurfaceArea = DistanceUOM.Convert(_partShapeData.GrossSurfaceArea, _partShapeData.DistanceType, UnitOfMeasure.Inch, true);
                var totalSurfaceArea = DistanceUOM.Convert(_partShapeData.TotalSurfaceArea, _partShapeData.DistanceType, UnitOfMeasure.Inch, true);

                if (CurrentPart != null && CurrentPart.IsValidState())
                {
                    if (CurrentPart.IsNull(LengthColumnName) || Convert.ToDouble(CurrentPart[LengthColumnName]) != 0)
                    {
                        CurrentPart[LengthColumnName] = 0;
                    }

                    if (CurrentPart.IsNull(WidthColumnName) || Convert.ToDouble(CurrentPart[WidthColumnName]) != 0)
                    {
                        CurrentPart[WidthColumnName] = 0;
                    }

                    if (CurrentPart.IsNull(HeightColumnName) || Convert.ToDouble(CurrentPart[HeightColumnName]) != 0)
                    {
                        CurrentPart[HeightColumnName] = 0;
                    }

                    if (CurrentPart.IsNull(SurfaceAreaColumnName) || Convert.ToDouble(CurrentPart[SurfaceAreaColumnName]) != totalSurfaceArea)
                    {
                        CurrentPart[SurfaceAreaColumnName] = totalSurfaceArea;
                    }

                    if (CurrentPart.IsNull(ShapeTypeColumnName) || Convert.ToString(CurrentPart[ShapeTypeColumnName]) != "Manual")
                    {
                        CurrentPart[ShapeTypeColumnName] = "Manual";
                    }
                }

                if (partRow != null && partRow.IsValidState())
                {
                    var areaRow = _partShapeData.OriginalData as DWOS.Data.Datasets.PartsDataset.PartAreaRow;

                    if (areaRow != null)
                    {
                        if (areaRow.ExclusionSurfaceArea != exclusionSurfaceArea)
                        {
                            areaRow.ExclusionSurfaceArea = exclusionSurfaceArea;
                        }

                        if (areaRow.GrossSurfaceArea != grossSurfaceArea)
                        {
                            areaRow.GrossSurfaceArea = grossSurfaceArea;
                        }

                        if (areaRow.ShapeType != _partShapeData.ShapeType)
                        {
                            areaRow.ShapeType = _partShapeData.ShapeType;
                        }

                        bool hasNewDimensions = _partShapeData.Dimensions.Any(d => !(d.OriginalData is DWOS.Data.Datasets.PartsDataset.PartAreaDimensionRow));
                        if (hasNewDimensions)
                        {
                            foreach (var areaDimensionRow in areaRow.GetPartAreaDimensionRows())
                            {
                                areaDimensionRow.Delete();
                            }
                        }

                        foreach (var dimensionData in _partShapeData.Dimensions)
                        {
                            var dimension = DistanceUOM.Convert(dimensionData.Measurement, dimensionData.DistanceType, UnitOfMeasure.Inch, false);

                            DWOS.Data.Datasets.PartsDataset.PartAreaDimensionRow dimensionRow;

                            if (hasNewDimensions)
                            {
                                dimensionRow = null;
                                dimensionData.OriginalData = null;
                            }
                            else
                            {
                                dimensionRow = dimensionData.OriginalData as DWOS.Data.Datasets.PartsDataset.PartAreaDimensionRow;
                            }

                            if (dimensionRow != null)
                            {
                                if (dimensionRow.DimensionName != dimensionData.Name)
                                {
                                    dimensionRow.DimensionName = dimensionData.Name;
                                }

                                if (dimensionRow.Dimension != dimension)
                                {
                                    dimensionRow.Dimension = dimension;
                                }
                            }
                            else
                            {
                                var partAreaDimensionRow = _dtPartAreaDimensionTable.AddPartAreaDimensionRow(areaRow, dimensionData.Name, dimension);
                                dimensionData.OriginalData = partAreaDimensionRow;
                            }
                        }
                    }
                    else
                    {
                        var newAreaRow = _dtPartAreaTable.AddPartAreaRow(partRow, exclusionSurfaceArea, grossSurfaceArea, _partShapeData.ShapeType);
                        _partShapeData.OriginalData = newAreaRow;

                        foreach (var dimensionData in _partShapeData.Dimensions)
                        {
                            var dimension = DistanceUOM.Convert(dimensionData.Measurement, _partShapeData.DistanceType, UnitOfMeasure.Inch, false);
                            var partAreaDimensionRow = _dtPartAreaDimensionTable.AddPartAreaDimensionRow(newAreaRow, dimensionData.Name, dimension);
                            dimensionData.OriginalData = partAreaDimensionRow;
                        }
                    }
                }
                else if (quotePartRow != null && quotePartRow.IsValidState())
                {
                    var areaRow = _partShapeData.OriginalData as DWOS.Data.Datasets.QuoteDataSet.QuotePartAreaRow;

                    if (areaRow != null)
                    {
                        if (areaRow.ExclusionSurfaceArea != exclusionSurfaceArea)
                        {
                            areaRow.ExclusionSurfaceArea = exclusionSurfaceArea;
                        }

                        if (areaRow.GrossSurfaceArea != grossSurfaceArea)
                        {
                            areaRow.GrossSurfaceArea = grossSurfaceArea;
                        }

                        if (areaRow.ShapeType != _partShapeData.ShapeType)
                        {
                            areaRow.ShapeType = _partShapeData.ShapeType;
                        }

                        bool hasNewDimensions = _partShapeData.Dimensions.Any(d => !(d.OriginalData is DWOS.Data.Datasets.QuoteDataSet.QuotePartAreaDimensionRow));

                        if (hasNewDimensions)
                        {
                            foreach (var areaDimensionRow in areaRow.GetQuotePartAreaDimensionRows())
                            {
                                areaDimensionRow.Delete();
                            }
                        }

                        foreach (var dimensionData in _partShapeData.Dimensions)
                        {
                            var dimension = DistanceUOM.Convert(dimensionData.Measurement, dimensionData.DistanceType, UnitOfMeasure.Inch, false);

                            DWOS.Data.Datasets.QuoteDataSet.QuotePartAreaDimensionRow dimensionRow;

                            if (hasNewDimensions)
                            {
                                dimensionRow = null;
                                dimensionData.OriginalData = null;
                            }
                            else
                            {
                                dimensionRow = dimensionData.OriginalData as DWOS.Data.Datasets.QuoteDataSet.QuotePartAreaDimensionRow;
                            }

                            if (dimensionRow != null)
                            {
                                if (dimensionRow.DimensionName != dimensionData.Name)
                                {
                                    dimensionRow.DimensionName = dimensionData.Name;
                                }

                                if (dimensionRow.Dimension != dimension)
                                {
                                    dimensionRow.Dimension = dimension;
                                }
                            }
                            else
                            {
                                var quotePartAreaRow = _dtQuotePartAreaDimensionTable.AddQuotePartAreaDimensionRow(areaRow, dimensionData.Name, dimension);
                                dimensionData.OriginalData = quotePartAreaRow;
                            }
                        }
                    }
                    else
                    {
                       var newAreaRow = _dtQuotePartAreaTable.AddQuotePartAreaRow(quotePartRow, exclusionSurfaceArea, grossSurfaceArea, _partShapeData.ShapeType);
                       _partShapeData.OriginalData = newAreaRow;

                        foreach (var dimensionData in _partShapeData.Dimensions)
                        {
                            var dimension = DistanceUOM.Convert(dimensionData.Measurement, _partShapeData.DistanceType, UnitOfMeasure.Inch, false);
                            var quotePartAreaDimensionRow = _dtQuotePartAreaDimensionTable.AddQuotePartAreaDimensionRow(newAreaRow, dimensionData.Name, dimension);
                            dimensionData.OriginalData = quotePartAreaDimensionRow;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error saving part shape row.");
            }
        }

        private void SaveRowBasic()
        {
            try
            {
                if (this.CurrentPart != null && this.CurrentPart.IsValidState())
                {
                    //Save everything as inches
                    double length = _lengthUOM.GetValue(UnitOfMeasure.Inch);
                    if (CurrentPart.IsNull(LengthColumnName) || Convert.ToDouble(CurrentPart[LengthColumnName]) != length)
                    {
                        CurrentPart[LengthColumnName] = length;
                    }

                    double width = _widthUOM.GetValue(UnitOfMeasure.Inch);
                    if (CurrentPart.IsNull(WidthColumnName) || Convert.ToDouble(CurrentPart[WidthColumnName]) != width)
                    {
                        CurrentPart[WidthColumnName] = width;
                    }

                    double height = _heightUOM.GetValue(UnitOfMeasure.Inch);
                    if (CurrentPart.IsNull(HeightColumnName) || Convert.ToDouble(CurrentPart[HeightColumnName]) != height)
                    {
                        CurrentPart[HeightColumnName] = height;
                    }

                    double surfaceArea = _surfaceAreaUOM.GetValue(UnitOfMeasure.Inch);
                    if (CurrentPart.IsNull(SurfaceAreaColumnName) || Convert.ToDouble(CurrentPart[SurfaceAreaColumnName]) != surfaceArea)
                    {
                        CurrentPart[SurfaceAreaColumnName] = surfaceArea;
                    }

                    if (cboPartShape.SelectedItem != null)
                    {
                        string shapeType = cboPartShape.SelectedItem.DataValue.ToString();

                        if (CurrentPart.IsNull(ShapeTypeColumnName) || Convert.ToString(CurrentPart[ShapeTypeColumnName]) != shapeType)
                        {
                            CurrentPart[ShapeTypeColumnName] = shapeType;
                        }
                    }

                    var partRow = CurrentPart as DWOS.Data.Datasets.PartsDataset.PartRow;
                    var quotePartRow = CurrentPart as DWOS.Data.Datasets.QuoteDataSet.QuotePartRow;

                    DataRow[] areaRows = null;
                    if (partRow != null)
                    {
                        areaRows = partRow.GetPartAreaRows();
                    }
                    else if (quotePartRow != null)
                    {
                        areaRows = quotePartRow.GetQuotePartAreaRows();
                    }

                    if (areaRows != null)
                    {
                        foreach (var areaRow in areaRows)
                        {
                            areaRow.Delete();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error saving part shape row.");
            }
        }

        /// <summary>
        /// Calculates surface area from current form values.
        /// </summary>
        /// <param name="shapeOverride">Overrides the form value for shape; may not be loaded in the form yet.</param>
        /// <returns></returns>
        private double CalculateSurfaceArea(string shapeOverride = null)
        {
            try
            {
                var surfaceArea = 0.0;
                var length = this.numLength.Value == null || this.numLength.Value == DBNull.Value ? 0 : Convert.ToDouble(this.numLength.Value);
                var width = this.numWidth.Value == null || this.numWidth.Value == DBNull.Value ? 0 : Convert.ToDouble(this.numWidth.Value);
                var height = this.numHeight.Value == null || this.numHeight.Value == DBNull.Value ? 0 : Convert.ToDouble(this.numHeight.Value);

                if (_lengthUOM != null && _lengthUOM.SelectedDistanceType != UnitOfMeasure.Inch)
                    length = DistanceUOM.Convert(length, _lengthUOM.SelectedDistanceType, UnitOfMeasure.Inch, false);
                if (_widthUOM != null && _widthUOM.SelectedDistanceType != UnitOfMeasure.Inch)
                    width = DistanceUOM.Convert(width, _widthUOM.SelectedDistanceType, UnitOfMeasure.Inch, false);
                if (_heightUOM != null && _heightUOM.SelectedDistanceType != UnitOfMeasure.Inch)
                    height = DistanceUOM.Convert(height, _heightUOM.SelectedDistanceType, UnitOfMeasure.Inch, false);

                var radius = Convert.ToDouble(width / 2);

                string partShape;

                if (!string.IsNullOrEmpty(shapeOverride))
                {
                    partShape = shapeOverride;
                }
                else
                {
                    partShape = cboPartShape.Value == null ? DEFAULT_SHAPE : cboPartShape.Value.ToString();
                }

                switch (partShape)
                {
                    case PartAreaUtilities.SHAPE_CYLINDER:
                        surfaceArea = (2 * Math.PI * radius * length) + (2 * Math.PI * (radius * radius));
                        break;
                    case PartAreaUtilities.SHAPE_ROTOR:
                        surfaceArea = Math.PI * 2 * radius * length;
                        break;
                    case PartAreaUtilities.SHAPE_BOX:
                        surfaceArea = (2 * length * height) + (2 * width * length) + (2 * width * height);
                        break;
                    case PartAreaUtilities.SHAPE_RECTANGLE:
                        surfaceArea = (2 * width * length);
                        break;
                    case PartAreaUtilities.SHAPE_SPHERE:
                        surfaceArea = (4 * Math.PI) * (radius * radius);
                        break;
                    case PartAreaUtilities.SHAPE_MANUAL:
                    case PartAreaUtilities.SHAPE_LENGTH:
                        // No calculation
                        break;
                    default:
                        surfaceArea = length * width * 2;
                        break;
                }

                return surfaceArea;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error calculating surface area.");
                return 0;
            }
        }

        /// <summary>
        /// Determines if validation should occur for surface area fields
        /// (length, width, and height).
        /// </summary>
        /// <returns>true if validation should occur; otherwise, false.</returns>
        private bool ValidateSurfaceAreaField()
        {
            var partShape = cboPartShape.Value?.ToString() ?? DEFAULT_SHAPE;
            var editorType = cboEditorType.Value?.ToString() ?? DEFAULT_MODE;

            return IsNewPart &&
                partShape != "Manual" &&
                editorType != MODE_ADVANCED;
        }

        private void DisposeMe()
        {
            if(_lengthUOM != null)
                _lengthUOM.Dispose();
            if (_widthUOM != null)
                _widthUOM.Dispose();
            if (_heightUOM != null)
                _heightUOM.Dispose();
            if (_surfaceAreaUOM != null)
                _surfaceAreaUOM.Dispose();

            _lengthUOM = null;
            _widthUOM = null;
            _heightUOM = null;
            _surfaceAreaUOM = null;
        }

        #endregion

        #region Events

        private void UoMDropDown_SelectionChanged(object sender, EventArgs e)
        {
            if (!IsRecordLoading && !_isInAdvancedMode)
            {
                //Calculate surface area
                var sa =  this.CalculateSurfaceArea();

                //Convert from INCH to current display units
                if (_surfaceAreaUOM != null && _surfaceAreaUOM.SelectedDistanceType != UnitOfMeasure.Inch)
                    sa = DistanceUOM.Convert(sa, UnitOfMeasure.Inch, _surfaceAreaUOM.SelectedDistanceType, true);

                numSurfaceArea.SetValue(sa);
            }
        }

        private void numEditor_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!IsRecordLoading)
                {
                    //Calculate surface area
                    var sa = this.CalculateSurfaceArea();

                    //Convert from INCH to current display units
                    if (_surfaceAreaUOM != null && _surfaceAreaUOM.SelectedDistanceType != UnitOfMeasure.Inch)
                        sa = DistanceUOM.Convert(sa, UnitOfMeasure.Inch, _surfaceAreaUOM.SelectedDistanceType, true);

                    numSurfaceArea.SetValue(sa);
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on numeric editor value changed.");
            }
        }

        private void cboPartShape_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                // Enable all then disable what is not needed
                numWidth.Enabled = true;
                numHeight.Enabled = true;
                numLength.Enabled = true;
                numSurfaceArea.Enabled = true;

                var partShape = cboPartShape.Value?.ToString() ?? DEFAULT_SHAPE;

                switch (partShape)
                {
                    case PartAreaUtilities.SHAPE_CYLINDER:
                    case PartAreaUtilities.SHAPE_ROTOR:
                    case PartAreaUtilities.SHAPE_RECTANGLE:
                        numHeight.Value = 0;
                        numHeight.Enabled = false;
                        break;
                    case PartAreaUtilities.SHAPE_SPHERE:
                        numLength.Value = 0;
                        numHeight.Value = 0;
                        numLength.Enabled = false;
                        numHeight.Enabled = false;
                        break;
                    case PartAreaUtilities.SHAPE_LENGTH:
                        numWidth.Value = 0;
                        numHeight.Value = 0;
                        numSurfaceArea.Value = 0;

                        numWidth.Enabled = false;
                        numHeight.Enabled = false;
                        numSurfaceArea.Enabled = false;
                        break;
                }

                if (!IsRecordLoading && !_isInAdvancedMode)
                {
                    //Calculate surface area
                    var sa = this.CalculateSurfaceArea();

                    //Convert from INCH to current display units
                    if (_surfaceAreaUOM != null && _surfaceAreaUOM.SelectedDistanceType != UnitOfMeasure.Inch)
                        sa = DistanceUOM.Convert(sa, UnitOfMeasure.Inch, _surfaceAreaUOM.SelectedDistanceType, true);

                    numSurfaceArea.SetValue(sa);
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on part shape changed.");
            }
        }

        private void cboEditorType_ValueChanged(object sender, EventArgs e)
        {
            string editorType;
            if (cboEditorType.Value == null)
            {
                editorType = DEFAULT_MODE;
            }
            else
            {
                editorType = cboEditorType.Value.ToString();
            }

            bool isAdvanced = editorType == MODE_ADVANCED;

            cboPartShape.Visible = !isAdvanced;
            numLength.Visible = !isAdvanced;
            numWidth.Visible = !isAdvanced;
            numHeight.Visible = !isAdvanced;
            btnShowEditor.Visible = isAdvanced;
            numSurfaceArea.ReadOnly = isAdvanced;

            _isInAdvancedMode = isAdvanced;

            if (!IsRecordLoading)
            {
                double sa;

                if (_isInAdvancedMode && _partShapeData != null)
                {
                    sa = _partShapeData.TotalSurfaceArea;
                }
                else if (_isInAdvancedMode)
                {
                    sa = 0;
                }
                else
                {
                    sa = this.CalculateSurfaceArea();
                }

                //Convert from INCH to current display units
                if (_surfaceAreaUOM != null && _surfaceAreaUOM.SelectedDistanceType != UnitOfMeasure.Inch)
                {
                    sa = DistanceUOM.Convert(sa, UnitOfMeasure.Inch, _surfaceAreaUOM.SelectedDistanceType, true);
                }

                numSurfaceArea.SetValue(sa);
            }
        }

        private void btnShowEditor_Click(object sender, EventArgs e)
        {
            if (_isInAdvancedMode)
            {
                var window = new PartShapeDialog();
                var helper = new WindowInteropHelper(window) { Owner = DWOSApp.MainForm.Handle };

                if (_partShapeData == null)
                {
                    window.LoadData(new PartShapeData()
                    {
                        ShapeType = DEFAULT_SHAPE
                    });
                }
                else
                {
                    window.LoadData(_partShapeData);
                }

                window.Data.DistanceType = _surfaceAreaUOM.SelectedDistanceType;

                var userAcceptsChanges = window.ShowDialog().GetValueOrDefault();

                if (userAcceptsChanges && window.Data != null)
                {
                    _partShapeData = window.Data;
                    _surfaceAreaUOM.SetValue(_partShapeData.TotalSurfaceArea, _partShapeData.DistanceType);

                    // Not changing the shape to 'Manual' allows the user to swtich
                    // back and forth with the Basic editor until one is saved.
                }
            }
        }
        private void SurfaceAreaUOM_ValueChanged(object sender, EventArgs e)
        {
            var handler = SurfaceAreaChanged;
            handler?.Invoke(this, e);
        }

        #endregion

        #region NumericEditorUnitOfMeasure

        public class NumericEditorUnitOfMeasure: IDisposable
        {
            #region Fields

            public static UnitOfMeasure DefaultDistanceType = UnitOfMeasure.Inch;

            public event EventHandler ValueChanged;

            /// <summary>
            /// Count used to ensure that <see cref="Editor"/>'s ValueChanged
            /// event is only registered once.
            /// </summary>
            private int _valueChangedCount;

            private object _valueChangedCountLock = new object();
            
            #endregion

            #region Properties
           
            public bool IsSquared { get; set; }
           
            public UltraNumericEditor Editor { get; set; }
            
            public UltraComboEditor UoMDropDownList { get; set; }
            
            public UnitOfMeasure SelectedDistanceType { get; private set; }

            #endregion

            #region Methods

            public NumericEditorUnitOfMeasure(UltraNumericEditor editor, bool isSquared)
            {
                SelectedDistanceType = DefaultDistanceType;
                IsSquared            = isSquared;
                Editor               = editor;

                Editor.ButtonsRight.Clear();
              

                var dropDown = new DropDownEditorButton();
                UoMDropDownList = new UltraComboEditor() { DropDownStyle = DropDownStyle.DropDownList };

                if (IsSquared)
                {
                    UoMDropDownList.Items.Add(UnitOfMeasure.Inch, "in2");
                    UoMDropDownList.Items.Add(UnitOfMeasure.Feet, "ft2");
                }
                else
                {
                    UoMDropDownList.Items.Add(UnitOfMeasure.Inch, "in");
                    UoMDropDownList.Items.Add(UnitOfMeasure.Feet, "ft");
                }

                dropDown.Control = UoMDropDownList;

                Editor.ButtonsRight.Add(dropDown);
                UoMDropDownList.ValueChanged += UnitOfMeasure_ValueChanged;
                
                SelectDistanceType(SelectedDistanceType);

                Editor.ValueChanged += Editor_ValueChanged;
            }

            private void SelectDistanceType(UnitOfMeasure distanceType)
            {
                var selectedItem = Enumerable.Cast<ValueListItem>(this.UoMDropDownList.Items).FirstOrDefault(item => (UnitOfMeasure)item.DataValue == distanceType);

                if (selectedItem != null)
                {
                    if (selectedItem == UoMDropDownList.SelectedItem)
                        UnitOfMeasure_ValueChanged(this, EventArgs.Empty); //manually call to fire the change event
                    else
                        UoMDropDownList.SelectedItem = selectedItem;
                }
            }

            public void Dispose()
            {
                this.Editor = null;
                this.UoMDropDownList = null;
            }

            public void SetValue(double value, UnitOfMeasure distanceType)
            {
                try
                {
                    UnregisterValueChanged();
                    SelectedDistanceType = distanceType;
                    SelectDistanceType(distanceType);
                    Editor.SetValue(value);
                }
                finally
                {
                    RegisterValueChanged();
                }
            }

            public double GetValue(UnitOfMeasure distanceType)
            {
                var value = Editor.Value;
                return value != null ? DistanceUOM.Convert(Convert.ToDouble(value), this.SelectedDistanceType, distanceType, this.IsSquared) : 0;
            }

            private void OnValueChanged()
            {
                var handler = ValueChanged;
                handler?.Invoke(this, new EventArgs());
            }

            private void UnregisterValueChanged()
            {
                lock (_valueChangedCountLock)
                {
                    _valueChangedCount -= 1;

                    if (_valueChangedCount == 0)
                    {
                        Editor.ValueChanged -= Editor_ValueChanged;
                    }
                }
            }

            private void RegisterValueChanged()
            {
                lock (_valueChangedCountLock)
                {
                    _valueChangedCount += 1;

                    if (_valueChangedCount == 1)
                    {
                        Editor.ValueChanged += Editor_ValueChanged;
                    }
                }
            }

            #endregion

            #region Events

            private void UnitOfMeasure_ValueChanged(object sender, EventArgs e)
            {
                try
                {
                    UnregisterValueChanged();
                    var item = UoMDropDownList.SelectedItem;

                    if (item == null)
                    {
                        return;
                    }

                    var distanceType = (UnitOfMeasure)item.DataValue;

                    //Correct to correct unit
                    if (this.SelectedDistanceType != distanceType)
                    {
                        if (Editor.Value != null)
                        {
                            var value = Convert.ToDouble(Editor.Value);
                            Editor.Value = DistanceUOM.Convert(value, this.SelectedDistanceType, distanceType, IsSquared);
                        }

                        this.SelectedDistanceType = distanceType;
                        OnValueChanged();
                    }

                    ((DropDownEditorButton)Editor.ButtonsRight[0]).Text = UoMDropDownList.Text;
                }
                finally
                {
                    RegisterValueChanged();
                }
            }

            private void Editor_ValueChanged(object sender, EventArgs e)
            {
                OnValueChanged();
            }

            #endregion
        }

        public static class DistanceUOM
        {
            public const int PRECISION = 5;

            internal static double Convert(double value, UnitOfMeasure fromDistance, UnitOfMeasure toDistanceType, bool isSquared)
            {
                if (fromDistance == UnitOfMeasure.Pound || toDistanceType == UnitOfMeasure.Pound)
                {
                    // Cannot support conversion from/to weight
                    return value;
                }

                if (fromDistance == toDistanceType)
                {
                    return value;
                }
                else if (isSquared)
                {
                    switch (toDistanceType)
                    {
                        case UnitOfMeasure.Inch:
                            return Math.Round(value * 144F, PRECISION);
                        case UnitOfMeasure.Feet:
                            return Math.Round(value / 144F, PRECISION);
                        default:
                            return value;
                    }
                }
                else
                {
                    switch (toDistanceType)
                    {
                        case UnitOfMeasure.Inch:
                            return Math.Round(value * 12F, PRECISION);
                        case UnitOfMeasure.Feet:
                            return Math.Round(value / 12F, PRECISION);
                        default:
                            return value;
                    }
                }
            }
        }

        #endregion
    }
}
