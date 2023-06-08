using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.QuoteDataSetTableAdapters;
using DWOS.Data.Order;
using DWOS.UI.Utilities;
using DWOS.Utilities.Validation;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DWOS.UI.Sales
{
    public partial class QuotePartInformation: DataPanel
    {
        #region Fields

        private IPriceUnitPersistence _priceUnitPersistence;

        #endregion

        #region Properties

        public QuoteDataSet Dataset
        {
            get { return base._dataset as QuoteDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return this.Dataset.QuotePart.QuotePartIDColumn.ColumnName; }
        }

        private IQuoteProcessWidget ActiveProcessWidget
        {
            get
            {
                IQuoteProcessWidget widget;
                if (ApplicationSettings.Current.PartPricingType == PricingType.Process)
                {
                    widget = processPriceWidget;
                }
                else
                {
                    widget = processWidget;
                }

                return widget;
            }
        }

        private IPriceWidget ActivePriceWidget
        {
            get
            {
                IPriceWidget widget;
                if (ApplicationSettings.Current.PartPricingType == PricingType.Process)
                {
                    widget = simplePriceWidget;
                }
                else
                {
                    widget = calcPriceWidget;
                }

                return widget;
            }
        }

        private PriceByType DefaultPriceByType
        {
            get
            {
                if (_priceUnitPersistence == null)
                {
                    return PriceByType.Quantity;
                }

                var isQuantityActive = _priceUnitPersistence.IsActive(OrderPrice.enumPriceUnit.Each)
                    || _priceUnitPersistence.IsActive(OrderPrice.enumPriceUnit.Lot);

                return isQuantityActive
                    ? PriceByType.Quantity
                    : PriceByType.Weight;
            }
        }

        private SystemFieldInfo QuantityField => FieldUtilities.GetField("Quote", "Quantity");

        #endregion

        #region Methods

        public QuotePartInformation()
        {
            this.InitializeComponent();
        }

        public void LoadData(QuoteDataSet dataset, IPriceUnitPersistence priceUnitPersistence)
        {
            this.Dataset = dataset;
            _priceUnitPersistence = priceUnitPersistence;

            processPriceWidget.Visible = ApplicationSettings.Current.PartPricingType == PricingType.Process;
            processWidget.Visible = ApplicationSettings.Current.PartPricingType == PricingType.Part;

            if (ApplicationSettings.Current.PartPricingType == PricingType.Part)
            {
                grpData.Controls.Remove(simplePriceWidget);

                const int simpleHeightOffset = 75; // determined through trial & error
                pnlProcess.Height += simpleHeightOffset;
            }
            else
            {
                grpData.Controls.Remove(calcPriceWidget);

                const int calcHeightOffset = 195; // determined through trial & error
                pnlProcess.Height += calcHeightOffset;
            }

            ActiveProcessWidget.LoadData(dataset, Editable);
            bsData.DataSource = this.Dataset;
            bsData.DataMember = this.Dataset.QuotePart.TableName;

            //bind column to control
            base.BindValue(this.txtName, this.Dataset.QuotePart.NameColumn.ColumnName);
            base.BindValue(this.numPartQty, this.Dataset.QuotePart.QuantityColumn.ColumnName);
            base.BindValue(this.chkPartMarking, this.Dataset.QuotePart.PartMarkingColumn.ColumnName);
            BindValue(numTotalWeight, Dataset.QuotePart.TotalWeightColumn.ColumnName);
            base.BindValue(this.numWeight, this.Dataset.QuotePart.WeightColumn.ColumnName);
            base.BindValue(this.txtNotes, this.Dataset.QuotePart.NotesColumn.ColumnName);

            this.numWeight.MaskInput = string.Format("nnnn.{0} lbs",
                string.Concat(Enumerable.Repeat("n", ApplicationSettings.Current.WeightDecimalPlaces)));

            numTotalWeight.MaskInput = string.Format("nnnnnn.{0} lbs",
                string.Concat(Enumerable.Repeat("n", ApplicationSettings.Current.WeightDecimalPlaces)));

            ActivePriceWidget.Setup(
                bsData,
                Dataset.QuotePartPrice,
                Dataset.QuotePartPriceCalculation,
                _priceUnitPersistence.PriceByTypes());

            this.mediaWidget.Setup(new MediaWidget.SetupArgs()
            {
                MediaJunctionTable = this.Dataset.QuotePart_Media,
                MediaTable = this.Dataset.Media,
                MediaJunctionTableParentRowColumn = this.Dataset.QuotePart_Media.QuotePartIDColumn,
                AllowEditing = Editable,
                MediaLinkType = Documents.LinkType.QuotePart,
                DocumentLinkTable = this.Dataset.QuotePart_DocumentLink,
                ScannerSettingsType = ScannerSettingsType.Part
            });

            this.partShapeWidget.LoadData();
            this.partShapeWidget.LoadDataTables(dataset.QuotePartArea, dataset.QuotePartAreaDimension);

            // Show sync/desync icon
            picQuantitySync.Visible = ApplicationSettings.Current.SyncQuantityAndWeightForOrders;
            picQuantityDesync.Visible = !ApplicationSettings.Current.SyncQuantityAndWeightForOrders;

            base._panelLoaded = true;
        }

        public override void AddValidators(DWOS.Utilities.Validation.ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtName, "Part name required.") { MinLength = 3 }, errProvider));

            var quantityField = FieldUtilities.GetField("Quote", "Quantity");

            if (quantityField == null || quantityField.IsRequired)
            {
                manager.Add(new ImageDisplayValidator
                    (new NumericControlValidator(this.numPartQty)
                    {
                        MinimumValue = 1,
                        ValidationRequired = () => this.IsNewRow.GetValueOrDefault()
                    }, errProvider));
            }
            else if (!quantityField.IsVisible)
            {
                int totalPanelHeight = pnlTotals.Height;
                pnlTotals.Height = 0; // Fixes resize issue & allows panels below to resize correctly
                pnlTotals.Visible = false;
                pnlProcess.Height += totalPanelHeight;

                pnlTotalPrice.Visible = false;
            }

            this.partShapeWidget.AddValidators(manager, errProvider);
        }

        public QuoteDataSet.QuotePartRow AddRow(int quoteID)
        {
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as QuoteDataSet.QuotePartRow;
            cr.Name = "";
            cr.PartMarking = false;

            if (QuantityField.IsVisible)
            {
                cr.Quantity = 0;
            }

            cr.QuoteID = quoteID;
            cr.PriceBy = DefaultPriceByType.ToString();

            return cr;
        }

        public QuoteDataSet.QuotePartRow AddRow(int quoteID, int partId)
        {
            var parts = new PartsDataset.PartDataTable();
            var partMedia = new PartsDataset.Part_MediaDataTable();

            using(var ta = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
                ta.FillByPartID(parts, partId);

            using (var ta = new Data.Datasets.PartsDatasetTableAdapters.Part_MediaTableAdapter())
                ta.FillByPartID(partMedia, partId);

            var partArea = new PartsDataset.PartAreaDataTable();
            using (var ta = new Data.Datasets.PartsDatasetTableAdapters.PartAreaTableAdapter())
            {
                ta.FillByPartID(partArea, partId);
            }

            var partAreaDimension = new PartsDataset.PartAreaDimensionDataTable();
            using (var ta = new Data.Datasets.PartsDatasetTableAdapters.PartAreaDimensionTableAdapter())
            {
                ta.FillByPartID(partAreaDimension, partId);
            }

            var partProcess = new PartsDataset.PartProcessDataTable();
            using (var ta = new Data.Datasets.PartsDatasetTableAdapters.PartProcessTableAdapter())
            {
                ta.FillByPart(partProcess, partId);
            }

            var partProcessPrice = new PartsDataset.PartProcessVolumePriceDataTable();
            using (var ta = new Data.Datasets.PartsDatasetTableAdapters.PartProcessVolumePriceTableAdapter())
            {
                ta.FillByPartID(partProcessPrice, partId);
            }

            var partDocumentLink = new PartsDataset.Part_DocumentLinkDataTable();
            using (var ta = new Data.Datasets.PartsDatasetTableAdapters.Part_DocumentLinkTableAdapter())
            {
                ta.FillByPartID(partDocumentLink, partId);
            }

            var part = parts.FirstOrDefault();

            if(part == null)
                return null;

            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as QuoteDataSet.QuotePartRow;
            cr.Name = part.Name;
            cr.PartMarking = part.PartMarking;

            if (QuantityField.IsVisible)
            {
                cr.Quantity = 0;
            }

            cr.QuoteID = quoteID;
            cr.EachPrice = part.IsEachPriceNull() ? 0 : part.EachPrice;
            cr.LotPrice = part.IsLotPriceNull() ? 0 : part.LotPrice;
            cr.Weight = part.IsWeightNull() ? 0 : part.Weight;
           
            if(!part.IsShapeTypeNull())
                cr.ShapeType = part.ShapeType;
            
            cr.Length = part.Length;
            cr.Width = part.Width;
            cr.Height = part.Height;
            cr.SurfaceArea = part.SurfaceArea;

            if (!part.IsNotesNull())
                cr.Notes = part.Notes;

            cr.PriceBy = DefaultPriceByType.ToString();

            cr.EndEdit();

            //copy over all media row id's
            using (var taMedia = new MediaTableAdapter() { ClearBeforeFill = false })
            {
                foreach (var pm in partMedia)
                {
                    var media = this.Dataset.QuotePart_Media.NewQuotePart_MediaRow();
                    media.MediaID = pm.MediaID;
                    media.QuotePartRow = cr;
                    this.Dataset.QuotePart_Media.AddQuotePart_MediaRow(media);

                    //load the media itself
                    taMedia.FillByMediaId(this.Dataset.Media, pm.MediaID);
                }
            }

            // Copy document links
            foreach (var partLink in partDocumentLink)
            {
                this.Dataset.QuotePart_DocumentLink.AddQuotePart_DocumentLinkRow(
                    partLink.DocumentInfoID,
                    Documents.LinkType.QuotePart.ToString(),
                    cr);
            }


            // Copy dimension data
            if (partArea.Count > 0)
            {
                var partAreaRow = partArea.OrderByDescending(i => i.PartAreaID).FirstOrDefault();

                var quoteAreaRow = this.Dataset.QuotePartArea.AddQuotePartAreaRow(cr,
                    partAreaRow.ExclusionSurfaceArea,
                    partAreaRow.GrossSurfaceArea,
                    partAreaRow.ShapeType);

                foreach (var partDimensionRow in partAreaDimension.Where(row => row.PartAreaID == partAreaRow.PartAreaID))
                {
                    this.Dataset.QuotePartAreaDimension.AddQuotePartAreaDimensionRow(quoteAreaRow,
                        partDimensionRow.DimensionName,
                        partDimensionRow.Dimension);
                }
            }

            // Copy process data
            var partProcessToQuotePartProcessMap = new Dictionary<int, int>();

            foreach (var partProcessRow in partProcess)
            {
                var quotePartProcessRow = this.Dataset.QuotePart_Process.NewQuotePart_ProcessRow();
                quotePartProcessRow.ProcessID = partProcessRow.ProcessID;
                quotePartProcessRow.QuotePartRow = cr;
                quotePartProcessRow.StepOrder = partProcessRow.StepOrder;
                quotePartProcessRow.ProcessAliasID = partProcessRow.ProcessAliasID;

                this.Dataset.QuotePart_Process.AddQuotePart_ProcessRow(quotePartProcessRow);
                quotePartProcessRow.EndEdit();

                partProcessToQuotePartProcessMap[partProcessRow.PartProcessID] = quotePartProcessRow.QuotePartProcessID;
            }

            // Copy process price data
            foreach (var priceRow in partProcessPrice)
            {
                if (!partProcessToQuotePartProcessMap.ContainsKey(priceRow.PartProcessID))
                {
                    _log.Warn("Could not map PartProcessID to QuotePartProcessID. PartProcessID = {0}",
                        priceRow.PartProcessID);

                    continue;
                }

                var quotePartProcessPriceRow = this.Dataset.QuotePartProcessPrice.NewQuotePartProcessPriceRow();
                quotePartProcessPriceRow.QuotePartProcessID = partProcessToQuotePartProcessMap[priceRow.PartProcessID];
                quotePartProcessPriceRow.Amount = priceRow.Amount;
                quotePartProcessPriceRow.PriceUnit = priceRow.PriceUnit;

                if (priceRow.IsMinValueNull())
                {
                    quotePartProcessPriceRow.SetMinValueNull();
                }
                else
                {
                    quotePartProcessPriceRow.MinValue = priceRow.MinValue;
                }

                if (priceRow.IsMaxValueNull())
                {
                    quotePartProcessPriceRow.SetMaxValueNull();
                }
                else
                {
                    quotePartProcessPriceRow.MaxValue = priceRow.MaxValue;
                }

                this.Dataset.QuotePartProcessPrice.AddQuotePartProcessPriceRow(quotePartProcessPriceRow);
                quotePartProcessPriceRow.EndEdit();
            }

            return cr;
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            base.BeforeMoveToNewRecord(id);
            RemoveChangeHandlers();
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            this.mediaWidget.ClearMedia();

            var quotePart = CurrentRecord as QuoteDataSet.QuotePartRow;

            if (quotePart != null)
            {
                // Price
                this.ActivePriceWidget.LoadRow(quotePart);

                // Media
                this.mediaWidget.LoadMedia(quotePart.GetQuotePart_MediaRows().ToList<DataRow>(),
                    quotePart.GetQuotePart_DocumentLinkRows().ToList<DataRow>(),
                    quotePart.QuotePartID);

                // Processes
                ActiveProcessWidget.LoadRow(quotePart);
            }

            ActiveProcessWidget.UpdateButtonEnabledStates();

            UpdateTotalFields();

            this.partShapeWidget.LoadRow(quotePart);
            this.partShapeWidget.IsRecordLoading = false;

            AddChangeHandlers();

            // Adjust minimum height depending on current pricing mode
            var defaultMinimum = FirstMinimumSize ?? MinimumSize;
            if (ActivePriceWidget == simplePriceWidget)
            {
                MinimumSize = new System.Drawing.Size(defaultMinimum.Width,
                    defaultMinimum.Height - calcPriceWidget.Height);
            }
            else if (ActivePriceWidget == calcPriceWidget)
            {
                MinimumSize = new System.Drawing.Size(defaultMinimum.Width,
                    defaultMinimum.Height - simplePriceWidget.Height);
            }

            // Set quantity field nullable/non-nullable depending on settings and row
            numPartQty.Nullable = !(quotePart.RowState == DataRowState.Added
                && QuantityField.IsRequired);
        }

        public override void EndEditing()
        {
            this.ActivePriceWidget.SaveRow();
            this.partShapeWidget.SaveRow();
            this.ActiveProcessWidget.SaveRow();
            base.EndEditing();
        }

        private void UpdateTotalFields()
        {
            var currentQuotePart = this.CurrentRecord as QuoteDataSet.QuotePartRow;
            var priceByType = ActivePriceWidget.PriceBy;

            if (currentQuotePart == null)
            {
                txtFeesTotal.Value = "N/A";
                txtTotal.Text = "N/A";
            }
            else if (currentQuotePart.IsQuantityNull())
            {
                var feeCount = currentQuotePart.GetQuotePartFeesRows().Length;
                txtFeesTotal.Value = feeCount == 1
                    ? "1 Fee"
                    : $"{feeCount} Fees";

                txtTotal.Text = "N/A";
            }
            else
            {
                var lotPrice = currentQuotePart.IsLotPriceNull() ? 0 : currentQuotePart.LotPrice;
                var eachPrice = currentQuotePart.IsEachPriceNull() ? 0 : currentQuotePart.EachPrice;
                int partQuantity = currentQuotePart.Quantity;

                var useLotPricing = false;

                if (ActiveProcessWidget == processPriceWidget)
                {
                    var price = processPriceWidget.GetPrice(
                        currentQuotePart.QuoteRow.CustomerID,
                        priceByType,
                        partQuantity,
                        currentQuotePart.IsTotalWeightNull() ? 0M : currentQuotePart.TotalWeight);

                    var processWidgetPriceUnit = price?.PriceUnit ?? OrderPrice.enumPriceUnit.Lot;
                    useLotPricing = OrderPrice.GetPricingStrategy(processWidgetPriceUnit) == PricingStrategy.Lot;
                }
                else if (ApplicationSettings.Current.UsePriceUnitQuantities)
                {
                    var priceUnitPersistence = new Data.Order.PriceUnitPersistence();
                    var preferredPriceUnit = priceUnitPersistence.DeterminePriceUnit(currentQuotePart.QuoteRow.CustomerID,
                        partQuantity,
                        currentQuotePart.IsTotalWeightNull() ? 0M : currentQuotePart.TotalWeight,
                        priceByType);

                    useLotPricing = OrderPrice.GetPricingStrategy(preferredPriceUnit) == PricingStrategy.Lot;
                }
                else
                {
                    useLotPricing = lotPrice >= (eachPrice * partQuantity);
                }

                string priceUnit;
                var basePrice = 0M;

                if (useLotPricing)
                {
                    priceUnit = OrderPrice.GetPriceUnit(priceByType, PricingStrategy.Lot).ToString();
                    basePrice = lotPrice;
                }
                else
                {
                    priceUnit = OrderPrice.GetPriceUnit(priceByType, PricingStrategy.Each).ToString();
                    basePrice = eachPrice;
                }

                var weight = numTotalWeight.Value == DBNull.Value
                    ? 0M
                    : Convert.ToDecimal(numTotalWeight.Value);

                var feeTotal = 0M;
                foreach (var fee in currentQuotePart.GetQuotePartFeesRows())
                {
                    feeTotal += OrderPrice.CalculateFees(
                        fee.FeeCalculationType,
                        fee.Charge, basePrice,
                        partQuantity,
                        priceUnit,
                        weight);
                }

                txtFeesTotal.Text = feeTotal.ToString(OrderPrice.CurrencyFormatString);

                var totalPrice = OrderPrice.CalculatePrice(
                    basePrice,
                    priceUnit,
                    feeTotal,
                    partQuantity,
                    weight);

                txtTotal.Text = totalPrice.ToString(OrderPrice.CurrencyFormatString);
            }
        }

        private void AutoUpdateBasePrices()
        {
            try
            {
                RemoveChangeHandlers();

                if (ActivePriceWidget != simplePriceWidget || ActiveProcessWidget != processPriceWidget)
                {
                    return;
                }

                var currentPart = CurrentRecord as QuoteDataSet.QuotePartRow;

                var pricePointData = processPriceWidget.GetPrice(
                    currentPart.QuoteRow.CustomerID,
                    (PriceByType)Enum.Parse(typeof(PriceByType), currentPart.PriceBy),
                    currentPart.IsQuantityNull() ? 0 : currentPart.Quantity,
                    currentPart.IsTotalWeightNull() ? 0M : currentPart.TotalWeight);

                if (pricePointData != null && pricePointData.Amount != 0M)
                {
                    simplePriceWidget.UpdatePrice(OrderPrice.GetPricingStrategy(pricePointData.PriceUnit), pricePointData.Amount);
                }
            }
            finally
            {
                AddChangeHandlers();
            }
        }

        private void AddChangeHandlers()
        {
            numPartQty.Validated += numPartQty_Validated;
            numWeight.Validated += numWeight_Validated;
            numTotalWeight.Validated += numTotalWeight_Validated;
            simplePriceWidget.PriceChanged += PriceWidget_PriceChanged;
            calcPriceWidget.PriceChanged += PriceWidget_PriceChanged;
            simplePriceWidget.PriceByTypeChanged += PriceWidget_PriceByTypeChanged;
            calcPriceWidget.PriceByTypeChanged += PriceWidget_PriceByTypeChanged;
        }

        private void RemoveChangeHandlers()
        {
            numPartQty.Validated -= numPartQty_Validated;
            numWeight.Validated -= numWeight_Validated;
            numTotalWeight.Validated -= numTotalWeight_Validated;
            simplePriceWidget.PriceChanged -= PriceWidget_PriceChanged;
            calcPriceWidget.PriceChanged -= PriceWidget_PriceChanged;
            simplePriceWidget.PriceByTypeChanged -= PriceWidget_PriceByTypeChanged;
            calcPriceWidget.PriceByTypeChanged -= PriceWidget_PriceByTypeChanged;
        }

        private void AutoUpdateQuantity()
        {
            try
            {
                RemoveChangeHandlers();
                var currentPartWeight = numWeight.Value as decimal?;
                var currentTotalWeight = numTotalWeight.Value as decimal?;

                var skipUpdate = !_panelLoaded
                    || _recordLoading
                    || !ApplicationSettings.Current.SyncQuantityAndWeightForOrders
                    || !currentPartWeight.HasValue
                    || currentPartWeight.Value == 0
                    || !currentTotalWeight.HasValue;

                if (skipUpdate)
                {
                    return;
                }

                // Update quantity
                var newQuantity = currentTotalWeight.Value / currentPartWeight.Value;
                var currentQuantity = Convert.ToInt32(numPartQty.Value);

                if (Convert.ToInt32(numPartQty.MinValue) > newQuantity)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(
                        "Quantity is below its minimum value.",
                        "Quotes");
                }
                else if (Convert.ToInt32(numPartQty.MaxValue) < newQuantity)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(
                        "Quantity has exceeded its maximum value.",
                        "Quotes");
                }
                else if (newQuantity != currentQuantity)
                {
                    numPartQty.Value = newQuantity;
                    numPartQty.DataBindings[0].WriteValue();
                    _log.Info($"Automatically updated quantity to {newQuantity}");
                }
            }
            finally
            {
                AddChangeHandlers();
            }
        }

        private void AutoUpdateTotalWeight()
        {
            try
            {
                RemoveChangeHandlers();
                var currentPartWeight = numWeight.Value as decimal?;
                var currentQuantity = numPartQty.Value as int?;

                if (!currentQuantity.HasValue)
                {
                    numTotalWeight.Value = null;
                    return;
                }

                var skipUpdate = !_panelLoaded
                    || _recordLoading
                    || !ApplicationSettings.Current.SyncQuantityAndWeightForOrders
                    || !currentPartWeight.HasValue
                    || currentPartWeight.Value == 0;

                if (skipUpdate)
                {
                    return;
                }

                // Update quantity
                var newTotalWeight = currentQuantity.Value * currentPartWeight.Value;
                var currentTotalWeight = numTotalWeight.Value == DBNull.Value
                    ? 0
                    : Convert.ToDecimal(numTotalWeight.Value);

                if (Convert.ToDecimal(numTotalWeight.MinValue) > newTotalWeight)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(
                        "Total weight is below its minimum value.",
                        "Quotes");
                }
                else if (Convert.ToDecimal(numTotalWeight.MaxValue) < newTotalWeight)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(
                        "Total weight has exceeded its maximum value.",
                        "Quotes");
                }
                else if (newTotalWeight != currentTotalWeight)
                {
                    numTotalWeight.Value = newTotalWeight;
                    numTotalWeight.DataBindings[0].WriteValue();
                    _log.Info($"Automatically updated total weight to {newTotalWeight}");
                }
            }
            finally
            {
                AddChangeHandlers();
            }
        }

        #endregion

        #region Events

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            this.txtName.DataBindings[0].WriteValue(); //write to datasource so node can see it and update his node text
            base.OnUpdateCurrentNodeUI();
        }

        private void numFeesTotal_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                var current = this.CurrentRecord as QuoteDataSet.QuotePartRow;

                if (current == null)
                {
                    return;
                }

                using (var of = new QuotePartFees())
                {
                    of.QuotePartID = current.QuotePartID;
                    of.QuotesDataset = Dataset;
                    of.ShowDialog(this);
                    UpdateTotalFields();
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating fees.");
            }
        }
        private void QuotePartInformation_EditableStatusChanged(object sender, EventArgs e)
        {
            ActiveProcessWidget.Editable = Editable;
        }

        private void processPriceWidget_PricePointChanged(object sender, PricePointChangedEventArgs e)
        {
            try
            {
                if (ActivePriceWidget != simplePriceWidget || ActiveProcessWidget != processPriceWidget || e.NewAmount == 0)
                {
                    return;
                }

                var currentPart = CurrentRecord as QuoteDataSet.QuotePartRow;
                int currentQty = currentPart.IsQuantityNull() ? 0 : currentPart.Quantity;

                var isFirstPricePointWithPriceUnit = processPriceWidget.PricePoints
                    .Where(p => p.PriceUnit == e.PricePoint.PriceUnit)
                    .OrderBy(p => p.MinQuantity ?? 0M)
                    .FirstOrDefault()
                    .Equals(e.PricePoint);

                bool qtyWithinPricePoint = currentQty >= (e.PricePoint.MinQuantity ?? 0M) &&
                    currentQty <= (e.PricePoint.MaxQuantity ?? decimal.MaxValue);

                if (isFirstPricePointWithPriceUnit || qtyWithinPricePoint)
                {
                    simplePriceWidget.UpdatePrice(OrderPrice.GetPricingStrategy(e.PricePoint.PriceUnit), e.NewAmount);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating each/lot prices from price point.");
            }
        }

        private void processPriceWidget_PriceBucketsChanged(object sender, EventArgs e)
        {
            try
            {
                AutoUpdateBasePrices();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on updating VDC price categories.");
            }
        }

        private void PriceWidget_PriceChanged(object sender, PriceChangedEventArgs e)
        {
            try
            {
                UpdateTotalFields();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating after price change.");
            }
        }

        private void PriceWidget_PriceByTypeChanged(object sender, EventArgs e)
        {
            try
            {
                AutoUpdateBasePrices();
                UpdateTotalFields();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating after price by change");
            }
        }

        private void simplePriceWidget_PriceSyncClicked(object sender, PriceChangedEventArgs e)
        {
            try
            {
                if (ActivePriceWidget != simplePriceWidget || ActiveProcessWidget != processPriceWidget)
                {
                    return;
                }

                processPriceWidget.UpdatePrices(e.PricingStrategy, e.NewAmount);
                _log.Info("Updated per-process each prices.");
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating after price sync click.");
            }
        }

        private void numPartQty_Validated(object sender, EventArgs e)
        {
            try
            {
                AutoUpdateBasePrices();
                AutoUpdateTotalWeight();
                UpdateTotalFields();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating after quantity change.");
            }
        }

        private void partShapeWidget_SurfaceAreaChanged(object sender, EventArgs e)
        {
            if (ActivePriceWidget != calcPriceWidget)
            {
                return;
            }

            calcPriceWidget.SurfaceArea = partShapeWidget.SurfaceArea;
        }

        private void numWeight_Validated(object sender, EventArgs e)
        {
            try
            {
                AutoUpdateTotalWeight();
                AutoUpdateBasePrices();
                UpdateTotalFields();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating after weight change.");
            }
        }


        private void numTotalWeight_Validated(object sender, EventArgs e)
        {
            try
            {
                AutoUpdateQuantity();
                AutoUpdateBasePrices();
                UpdateTotalFields();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating after total weight change.");
            }
        }

        #endregion

    }
}