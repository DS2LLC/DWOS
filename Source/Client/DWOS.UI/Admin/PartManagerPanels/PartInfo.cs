using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.QuoteDataSetTableAdapters;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using NLog;
using System.Drawing;
using Infragistics.Win.UltraWinToolTip;

namespace DWOS.UI.Admin.PartManagerPanels
{
    public partial class PartInfo: DataPanel
    {
        #region Fields

        private const string PART_REV_FIELD_NAME = "Part Rev.";
        private const string MATERIAL_FIELD_NAME = "Material";
        private const string MANUFACTURER_FIELD_NAME = "Manufacturer";
        private const string SURFACE_AREA_FIELD_NAME = "Surface Area";
        private const string WEIGHT_FIELD_NAME = "Weight";
        private const string ASSEMBLY_FIELD_NAME = "Assembly";
        private const string FIELD_CATEGORY_PARTS = "Part";

        private bool _viewOnly;
        private DWOS.Utilities.Validation.ValidatorManager _validationManager;

        #endregion

        #region Properties

        public PartsDataset Dataset
        {
            get => _dataset as PartsDataset;
            set => _dataset = value;
        }

        protected override string BindingSourcePrimaryKey =>
            Dataset.Part.PartIDColumn.ColumnName;

        public int CurrentPartUsageCount { get; set; }

        public bool ViewOnly
        {
            get => _viewOnly;
            set
            {
                _viewOnly = value;
                processWidget.ViewOnly = value;
            }
        }

        #endregion

        #region Methods

        public PartInfo()
        {
            this.InitializeComponent();
        }

        public void LoadData(PartsDataset dataset)
        {
            this.Dataset = dataset;

            this.processWidget.LoadData(dataset);

            bsData.DataSource = this.Dataset;
            bsData.DataMember = this.Dataset.Part.TableName;

            //bind column to control
            base.BindValue(this.txtRev, this.Dataset.Part.RevisionColumn.ColumnName);
            base.BindValue(this.txtPartName, this.Dataset.Part.NameColumn.ColumnName);
            base.BindValue(this.txtDescription, this.Dataset.Part.DescriptionColumn.ColumnName);
            base.BindValue(this.cboCustomer, this.Dataset.Part.CustomerIDColumn.ColumnName);
            base.BindValue(this.cboMaterial, this.Dataset.Part.MaterialColumn.ColumnName);
            base.BindValue(this.curEachPrice, this.Dataset.Part.EachPriceColumn.ColumnName);
            base.BindValue(this.curLotPrice, this.Dataset.Part.LotPriceColumn.ColumnName);
            base.BindValue(this.txtNotes, this.Dataset.Part.NotesColumn.ColumnName);
           
            base.BindValue(this.chkActive, this.Dataset.Part.ActiveColumn.ColumnName);
            base.BindValue(this.cboManufacturer, this.Dataset.Part.ManufacturerIDColumn.ColumnName);
            base.BindValue(this.cboAirframe, this.Dataset.Part.AirframeColumn.ColumnName);
            base.BindValue(this.numWeight, this.Dataset.Part.WeightColumn.ColumnName);
            base.BindValue(this.chkRequireCoc, this.Dataset.Part.RequireCocByDefaultColumn.ColumnName);
            base.BindValue(this.chkPartMarking, this.Dataset.Part.PartMarkingColumn.ColumnName);

            curEachPrice.MaskInput = "{currency:6." + ApplicationSettings.Current.PriceDecimalPlaces + "}";
            curLotPrice.MaskInput = "{currency:6." + ApplicationSettings.Current.PriceDecimalPlaces + "}";

            numWeight.MaskInput = $"nnnn.{string.Concat(Enumerable.Repeat("n", ApplicationSettings.Current.WeightDecimalPlaces))} lbs";

            base.BindList(this.cboMaterial, this.Dataset.d_Material, this.Dataset.d_Material.MaterialIDColumn.ColumnName, this.Dataset.d_Material.MaterialIDColumn.ColumnName);

            base.BindValue(this.txtAssembly, this.Dataset.Part.AssemblyNumberColumn.ColumnName);

            //bind lists
            base.BindList(this.cboCustomer, this.Dataset.Customer, this.Dataset.Customer.CustomerIDColumn.ColumnName, this.Dataset.Customer.NameColumn.ColumnName);

            base.BindListAsDistinctValues(this.cboAirframe, this.Dataset.Part, this.Dataset.Part.AirframeColumn.ColumnName);
            base.BindList(this.cboManufacturer, this.Dataset.d_Manufacturer, this.Dataset.d_Manufacturer.ManufacturerIDColumn.ColumnName, this.Dataset.d_Manufacturer.ManufacturerIDColumn.ColumnName);
            //base.BindList(cboAirframe, this.Dataset.d_Airframe, , this.Dataset.d_Airframe.AirframeIDColumn.ColumnName);

            //bind airframe data
            this.bsAirframe.DataSource = this.Dataset;
            this.bsAirframe.DataMember = this.Dataset.d_Airframe.TableName;

            this.cboAirframe.DataSource = this.bsAirframe;
            this.cboAirframe.DisplayMember = this.Dataset.d_Airframe.AirframeIDColumn.ColumnName;
            this.cboAirframe.ValueMember = this.Dataset.d_Airframe.AirframeIDColumn.ColumnName;

            this.mediaWidget.Setup(new MediaWidget.SetupArgs()
            {
                MediaJunctionTable = this.Dataset.Part_Media,
                MediaTable = this.Dataset.Media,
                MediaJunctionTableParentRowColumn = this.Dataset.Part_Media.PartIDColumn,
                MediaJunctionTableDefaultColumn = this.Dataset.Part_Media.DefaultMediaColumn,
                AllowEditing = Editable,
                MediaLinkType = Documents.LinkType.Part,
                DocumentLinkTable = this.Dataset.Part_DocumentLink,
                ScannerSettingsType = ScannerSettingsType.Part
            });

            this.AutoScroll = false;
            partShapeWidget.LoadData();
            partShapeWidget.LoadDataTables(dataset.PartArea, dataset.PartAreaDimension);

            // Part Marking
            if (!ApplicationSettings.Current.PartMarkingEnabled)
            {
                pnlPartMarking.Visible = false;
                pnlPartMarking.Height = 0;
            }

            base._panelLoaded = true;

            if(this.ViewOnly)
            {
                this.curEachPrice.Enabled = false;
                this.curLotPrice.Enabled = false;
                this.lblEachPrice.Enabled = false;
                this.lblLotPrice.Enabled = false;
                this.curEachPrice.Enabled = false;
                this.txtRev.Enabled = false;
                this.cboCustomer.Enabled = false;
                this.cboManufacturer.Enabled = false;
                this.txtNotes.ReadOnly = true;

                this.txtPartName.Enabled = false;
                this.cboMaterial.Enabled = false;
                this.mediaWidget.AllowEditing = false;
                this.chkActive.Enabled = false;
                this.chkPartMarking.Enabled = false;
                this.cboAirframe.Enabled = false;
                this.txtAssembly.Enabled = false;
                this.partShapeWidget.Enabled = false;
                this.tableCustomFields.Enabled = false;
                
            }
            else
            {
                processWidget.RegisterCommands(Commands);
            }

            if (ApplicationSettings.Current.PartPricingType != PricingType.Process)
            {
                this.curEachPrice.ButtonsRight["Sync"].Visible = false;
                this.curLotPrice.ButtonsRight["Sync"].Visible = false;
            }
        }


        private int GetTextHeight(UltraTextEditor tBox)
        {
            var height = (TextRenderer.MeasureText(tBox.Text, tBox.Font, tBox.ClientSize,
                     TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl).Height) + 20;
            return height + Convert.ToInt16(height * .1);
        }

        public override void AddValidators(DWOS.Utilities.Validation.ValidatorManager manager, ErrorProvider errProvider)
        {
            _validationManager = manager;
            var additionalHeightForBottomPanel = 0;

            // Hide prices unless user is in a correct role.
            var currentUser = SecurityManager.Current;
            if (ViewOnly && !currentUser.IsInRole("PartsManager") && !currentUser.IsInRole("OrderEntry"))
            {
                additionalHeightForBottomPanel += pnlPrice.Height;
                pnlPrice.Height = 0;
                pnlPrice.Visible = false;
                // The process widget hides price fields when needed
            }

            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtPartName, "Part name required."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboCustomer, "Customer selection required.") { PreserveWhitespace = true }, errProvider));
            manager.Add(new ImageDisplayValidator(new PartNameControlValidator(this.txtPartName, this), errProvider));

            var fields = new Data.Datasets.ApplicationSettingsDataSet.FieldsDataTable();
            using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                ta.FillByCategory(fields, FIELD_CATEGORY_PARTS);

            // Material field
            var materialField = fields.FirstOrDefault(f => f.Name == MATERIAL_FIELD_NAME);
            if (materialField == null || materialField.IsRequired)
            {
                manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboMaterial, "Part material type required.") { ValidationRequired = () => this.IsNewRow.GetValueOrDefault(), PreserveWhitespace = true }, errProvider));
            }
            else if (!materialField.IsVisible)
            {
                lblMaterial.Visible = false;
                cboMaterial.Visible = false;
                pnlMaterial.Visible = false;
                additionalHeightForBottomPanel += pnlManufacturer.Height;
                pnlMaterial.Height = 0;
            }

            // Manufacturer field
            var manufacturerField = fields.FirstOrDefault(f => f.Name == MANUFACTURER_FIELD_NAME);
            if (manufacturerField == null || manufacturerField.IsRequired)
            {
                manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboManufacturer, "Part manufacturer is required.") { ValidationRequired = () => this.IsNewRow.GetValueOrDefault(), PreserveWhitespace = true }, errProvider));
            }
            else if (!manufacturerField.IsVisible)
            {
                lblManufacturer.Visible = false;
                cboAirframe.Visible = false;
                cboManufacturer.Visible = false;
                pnlManufacturer.Visible = false;
                additionalHeightForBottomPanel += pnlManufacturer.Height;
                pnlManufacturer.Height = 0;
            }

            // Part revision field
            var partRevField = fields.FirstOrDefault(f => f.Name == PART_REV_FIELD_NAME);
            if (partRevField == null || partRevField.IsRequired)
                manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtRev, "Part revision is required.") { ValidationRequired = () => this.IsNewRow.GetValueOrDefault() }, errProvider));
            else if(!partRevField.IsVisible)
            {
                txtPartName.Width += (txtRev.Location.X + txtRev.Width) - (txtPartName.Location.X + txtPartName.Width);
                lblRev.Visible = false;
                txtRev.Visible = false;
            }

            // Surface area fields
            var surfaceAreaField = fields.FirstOrDefault(f => f.Name == SURFACE_AREA_FIELD_NAME);
            if (surfaceAreaField == null || surfaceAreaField.IsRequired)
                partShapeWidget.AddValidators(manager, errProvider);
            else if (!surfaceAreaField.IsVisible)
            {
                lblSize.Visible = false;
                partShapeWidget.Visible = false;
                pnlSize.Visible = false;
                additionalHeightForBottomPanel += pnlSize.Height;
                pnlSize.Height = 0;
            }

            // Weight Field
            var weightField = fields.FirstOrDefault(f => f.Name == WEIGHT_FIELD_NAME);
            if (weightField == null || weightField.IsRequired)
            {
                this.numWeight.Nullable = false;
                manager.Add(new ImageDisplayValidator(new NumericControlValidator(this.numWeight) { ValidationRequired = () => this.IsNewRow.GetValueOrDefault() }, errProvider));
            }
            else if (!weightField.IsVisible)
            {
                numWeight.Visible = false;
                pnlWeight.Visible = false;
                additionalHeightForBottomPanel += pnlWeight.Height;
                pnlWeight.Height = 0;
                processWidget.ShowLoadCapacityWeight = false;
            }

            // Assembly field
            var assemblyField = fields.FirstOrDefault(f => f.Name == ASSEMBLY_FIELD_NAME);

            if (assemblyField == null || assemblyField.IsRequired)
            {
                manager.Add(new ImageDisplayValidator(new TextControlValidator(txtAssembly, "Assembly is required.") { ValidationRequired = () => this.IsNewRow.GetValueOrDefault() }, errProvider));
            }
            else if (!assemblyField.IsVisible)
            {
                txtAssembly.Visible = false;
                pnlAssembly.Visible = false;
                additionalHeightForBottomPanel += pnlAssembly.Height;
                pnlAssembly.Height = 0;
            }

            // Add process validators
            processWidget.AddValidators(manager, errProvider);

            // Require COC by Default
            if (!ApplicationSettings.Current.AllowSkippingCoc)
            {
                pnlRequireCoc.Visible = false;
                additionalHeightForBottomPanel += pnlRequireCoc.Height;
                pnlRequireCoc.Height = 0;
            }

            // Fix control heights
            pnlBottom.Height += additionalHeightForBottomPanel;

            var defaultMinSize = FirstMinimumSize ?? MinimumSize;
            MinimumSize = new System.Drawing.Size(
                defaultMinSize.Width,
                defaultMinSize.Height - additionalHeightForBottomPanel);
        }

        public PartsDataset.PartRow AddPart(int customerID)
        {
            var rowVw       = bsData.AddNew() as DataRowView;
            var cr          = rowVw.Row as PartsDataset.PartRow;
            cr.Name         = "New Part";
            cr.CustomerID   = customerID;
            cr.PartMarking  = false;
            cr.LastModified = DateTime.Now;

            return cr;
        }

        /// <summary>
        /// Syncs per-process prices with the current each and lot prices.
        /// </summary>
        public void UpdateProcessPrices()
        {
            try
            {
                if (!processWidget.HasPricePoints)
                {
                    return;
                }

                processWidget.UpdatePrices(PricingStrategy.Each, curEachPrice.Value);
                _log.Info("Updated per-process each prices.");
                processWidget.UpdatePrices(PricingStrategy.Lot, curLotPrice.Value);
                _log.Info("Updated per-process lot prices.");
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating process-level prices to match current each/lot prices");
            }
        }

        internal int GetNextProcessStepOrder(PartsDataset.PartRow partRow)
        {
            var processes = partRow.GetPartProcessRows();

            return processes == null || processes.Length == 0 ? 1 : partRow.GetPartProcessRows().Max(p => p.StepOrder) + 1;
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            numWeight.Validating -= NumWeight_Validating;

            partShapeWidget.SaveRow();
            processWidget.SaveRow();
            base.BeforeMoveToNewRecord(id);
            partShapeWidget.IsRecordLoading = true;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            numWeight.Validating += NumWeight_Validating;

            var partRow = base.CurrentRecord as PartsDataset.PartRow;

            this.processWidget.LoadRow(partRow);

            CreateCustomFields();
            BindCustomFields();

            if(partRow != null)
            {
                // If the part is linked to a quote part, show the quote ID
                if (partRow.IsQuotePartIDNull())
                {
                    this.txtQuote.Value = string.Empty;
                }
                else
                {
                    using (var ta = new QuotePartTableAdapter())
                    {
                        var quotePart = ta.GetDataById(partRow.QuotePartID).FirstOrDefault();

                        if (quotePart == null)
                        {
                            this.txtQuote.Value = string.Empty;
                        }
                        else
                        {
                            this.txtQuote.Value = quotePart.QuoteID;
                        }
                    }
                }

                //Load media
                this.mediaWidget.LoadMedia(partRow.GetPart_MediaRows().ToList<DataRow>(),
                    partRow.GetPart_DocumentLinkRows().ToList<DataRow>(),
                    partRow.PartID);
            }
            else
                this.mediaWidget.ClearMedia();


            int orgNoteHeight = txtNotes.Height;

            //int textHeight = GetTextHeight(txtNotes);
            //if (textHeight > 50)
            //    this.upNotes.Height = textHeight;
            //else
            //    this.upNotes.Height = orgNoteHeight;

            LoadPartShapeWidget(partRow);
            base.AfterMovedToNewRecord(id);
        }

        public void LoadPartShapeWidget(PartsDataset.PartRow partRow)
        {
            partShapeWidget.LoadRow(partRow);
            partShapeWidget.IsRecordLoading = false;
        }

        public override void EndEditing()
        {
            partShapeWidget.SaveRow();
            processWidget.SaveRow();
            base.EndEditing();
        }

        protected override void OnDispose()
        {
            try
            {
                _log.Info("Disposing of PartInfo");

                Properties.Settings.Default.Save();

                this.mediaWidget.ClearMedia();
                _validationManager = null;

                base.OnDispose();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on dispose of PartInfo");
            }
        }

        public PartsDataset.PartRow AddPartFromQuote(int quotePartId)
        {
            var rowVw       = bsData.AddNew() as DataRowView;
            var cr          = rowVw.Row as PartsDataset.PartRow;
            cr.Name         = "New Part";
            cr.PartMarking  = false;
            cr.LastModified = DateTime.Now;

            using(var ta = new QuotePartTableAdapter())
                cr.CustomerID = ta.GetCustomerId(quotePartId).GetValueOrDefault();

            bsData.EndEdit();

            AddQuoteToPart(cr, quotePartId);

            return cr;
        }

        private void AddQuoteToPart(PartsDataset.PartRow part, int quotePartId)
        {
            if (part == null)
                return;

            using (var ta = new QuotePartTableAdapter())
            {
                var quotePart = ta.GetDataById(quotePartId).FirstOrDefault();

                if (quotePart != null)
                {
                    if (!quotePart.IsLengthNull())
                        part.Length = quotePart.Length;
                    if (!quotePart.IsWidthNull())
                        part.Width = quotePart.Width;
                    if (!quotePart.IsHeightNull())
                        part.Height = quotePart.Height;
                    if (!quotePart.IsShapeTypeNull())
                        part.ShapeType = quotePart.ShapeType;
                    if (!quotePart.IsSurfaceAreaNull())
                        part.SurfaceArea = quotePart.SurfaceArea;
                    if (!quotePart.IsWeightNull())
                        part.Weight = quotePart.Weight;
                    if (!quotePart.IsNotesNull())
                        part.Notes = quotePart.Notes;

                    part.Name = quotePart.Name;
                    part.QuotePartID = quotePart.QuotePartID;
                    part.PartMarking = quotePart.PartMarking;

                    //Copy over price and price unit
                    if (!quotePart.IsLotPriceNull())
                        part.LotPrice = quotePart.LotPrice;
                    if (!quotePart.IsEachPriceNull())
                        part.EachPrice = quotePart.EachPrice;

                    //Copy over media
                    using (var taQuoteMedia = new QuotePart_MediaTableAdapter())
                    {
                        using (var taMedia = new DWOS.Data.Datasets.PartsDatasetTableAdapters.MediaTableAdapter() { ClearBeforeFill = false })
                        {
                            var media = new QuoteDataSet.QuotePart_MediaDataTable();
                            taQuoteMedia.FillByQuotePart(media, quotePart.QuotePartID);

                            media.ForEach(mr =>
                            {
                                taMedia.FillByMediaIdWithoutMedia(this.Dataset.Media, mr.MediaID);
                                var mediaRow = this.Dataset.Media.FindByMediaID(mr.MediaID);
                                this.Dataset.Part_Media.AddPart_MediaRow(part, mediaRow, false);
                            });
                        }
                    }

                    // Copy document links
                    using (var taQuoteDocumentLinks = new QuotePart_DocumentLinkTableAdapter())
                    {
                        var quoteLinks = new QuoteDataSet.QuotePart_DocumentLinkDataTable();
                        taQuoteDocumentLinks.FillByQuotePart(quoteLinks, quotePart.QuotePartID);

                        foreach (var quoteLink in quoteLinks)
                        {
                            this.Dataset.Part_DocumentLink.AddPart_DocumentLinkRow(
                                quoteLink.DocumentInfoID,
                                Documents.LinkType.Part.ToString(),
                                part);
                        }
                    }

                    // Copy processes
                    var quoteProcessToPartProcessMap = new Dictionary<int, int>();

                    using (var taQuotePartProcess = new QuotePart_ProcessTableAdapter())
                    {
                        using (var quotePartProcesses = new QuoteDataSet.QuotePart_ProcessDataTable())
                        {
                            taQuotePartProcess.FillByQuotePart(quotePartProcesses, quotePart.QuotePartID);

                            foreach (var quotePartProcess in quotePartProcesses)
                            {
                                var partProcess = this.Dataset.PartProcess.NewRow() as PartsDataset.PartProcessRow;
                                partProcess.ProcessID = quotePartProcess.ProcessID;
                                partProcess.PartRow = part;
                                partProcess.StepOrder = quotePartProcess.StepOrder;
                                partProcess.ProcessAliasID = quotePartProcess.ProcessAliasID;

                                this.Dataset.PartProcess.Rows.Add(partProcess);
                                partProcess.EndEdit();

                                quoteProcessToPartProcessMap[quotePartProcess.QuotePartProcessID] = partProcess.PartProcessID;
                            }
                        }
                    }

                    // Copy process price data
                    using (var taQuotePartProcessPrice = new QuotePartProcessPriceTableAdapter())
                    {
                        using (var quotePartProcessPrices = new QuoteDataSet.QuotePartProcessPriceDataTable())
                        {
                            taQuotePartProcessPrice.FillByQuotePart(quotePartProcessPrices, quotePart.QuotePartID);

                            foreach (var quoteProcessPrice in quotePartProcessPrices)
                            {
                                if (!quoteProcessToPartProcessMap.ContainsKey(quoteProcessPrice.QuotePartProcessID))
                                {
                                    _log.Warn("Could not map QuotePartProcessID to PartProcessID. QuotePartProcessID = {0}",
                                        quoteProcessPrice.QuotePartProcessID);

                                    continue;
                                }

                                var partProcessID = quoteProcessToPartProcessMap[quoteProcessPrice.QuotePartProcessID];
                                var partProcessPrice = this.Dataset.PartProcessVolumePrice.NewPartProcessVolumePriceRow();
                                partProcessPrice.PartProcessID = partProcessID;
                                partProcessPrice.Amount = quoteProcessPrice.Amount;
                                partProcessPrice.PriceUnit = quoteProcessPrice.PriceUnit;

                                if (quoteProcessPrice.IsMinValueNull())
                                {
                                    partProcessPrice.SetMinValueNull();
                                }
                                else
                                {
                                    partProcessPrice.MinValue = quoteProcessPrice.MinValue;
                                }

                                if (quoteProcessPrice.IsMaxValueNull())
                                {
                                    partProcessPrice.SetMaxValueNull();
                                }
                                else
                                {
                                    partProcessPrice.MaxValue = quoteProcessPrice.MaxValue;
                                }

                                this.Dataset.PartProcessVolumePrice.AddPartProcessVolumePriceRow(partProcessPrice);
                                partProcessPrice.EndEdit();
                            }
                        }
                    }

                    // Copy advanced area info.
                    QuoteDataSet.QuotePartAreaRow quoteArea = null;
                    PartsDataset.PartAreaRow partArea = null;
                    using (var taQuotePartArea = new QuotePartAreaTableAdapter())
                    {
                        using (var quotePartArea = new QuoteDataSet.QuotePartAreaDataTable())
                        {
                            taQuotePartArea.FillByQuotePart(quotePartArea, quotePart.QuotePartID);

                            if (quotePartArea.Count > 0)
                            {
                                quoteArea = quotePartArea.OrderByDescending(i => i.QuotePartAreaID).FirstOrDefault();
                                partArea = this.Dataset.PartArea.AddPartAreaRow(part,
                                    quoteArea.ExclusionSurfaceArea,
                                    quoteArea.GrossSurfaceArea,
                                    quoteArea.ShapeType);
                            }
                        }
                    }

                    if (quoteArea != null && partArea != null)
                    {
                        using (var taQuotePartAreaDimension = new QuotePartAreaDimensionTableAdapter())
                        {
                            using (var quotePartAreaDimension = new QuoteDataSet.QuotePartAreaDimensionDataTable())
                            {
                                taQuotePartAreaDimension.FillByQuotePartArea(quotePartAreaDimension, quoteArea.QuotePartAreaID);

                                foreach (var quoteDimensionRow in quotePartAreaDimension)
                                {
                                    this.Dataset.PartAreaDimension.AddPartAreaDimensionRow(partArea,
                                        quoteDimensionRow.DimensionName,
                                        quoteDimensionRow.Dimension);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates custom fields based on the current customer.
        /// </summary>
        private void CreateCustomFields()
        {
            try
            {
                const int customFieldMaxLength = 255;

                _log.Debug("Creating custom field controls.");
                int tableRowCount = 0;

                var currentPart = CurrentRecord as PartsDataset.PartRow;

                if (Dataset != null && currentPart != null)
                {
                    //clean up previous controls
                    var controls = new List<Control>(tableCustomFields.Controls.OfType<Control>());

                    foreach (var control in controls)
                    {
                        //remove the validator from the previous custom field
                        if (control.Tag is CustomFieldUIInfo && ((CustomFieldUIInfo)control.Tag).Validator != null)
                        {
                            _validationManager.Remove((control.Tag as CustomFieldUIInfo).Validator);
                        }

                        control.DataBindings.Clear();
                        control.Dispose();
                    }

                    tableCustomFields.Controls.Clear();

                    //find current customer
                    var customer = Dataset.Customer.FindByCustomerID(currentPart.CustomerID);

                    if (customer != null)
                    {
                        var customFields = customer.GetPartLevelCustomFieldRows()
                            .OrderBy(c => c.Name);

                        foreach(var customField in customFields)
                        {
                            if (!customField.IsVisible)
                            {
                                continue;
                            }

                            var label = new Label
                            {
                                Text = customField.Name + ":",
                                Dock = DockStyle.Fill,
                                TextAlign = ContentAlignment.TopLeft,
                                Padding = new Padding(2, 0, 0, 0),
                            };

                            tableCustomFields.Controls.Add(label, 0, tableRowCount);

                            Control control;
                            if (customField.IsListIDNull())
                            {
                                control = new UltraTextEditor
                                {
                                    Tag = new CustomFieldUIInfo { PartLevelCustomFieldID = customField.PartLevelCustomFieldID },
                                    Dock = DockStyle.Fill,
                                    MaxLength = customFieldMaxLength,
                                    Margin = new Padding(2, 0, 30, 4),
                                    AcceptsReturn = true,
                                    Multiline = true,
                                    Scrollbars = ScrollBars.Both,
                                    Height = 35,
                                    WordWrap = false
                                };

                            }
                            else
                            {
                                control = new UltraComboEditor()
                                {
                                    Tag = new CustomFieldUIInfo { PartLevelCustomFieldID = customField.PartLevelCustomFieldID },
                                    Dock = DockStyle.Fill,
                                    MaxLength = customFieldMaxLength,
                                    Margin = new Padding(2, 0, 30, 4),
                                    Padding = new Padding(0),
                                    DropDownStyle = DropDownStyle.DropDownList
                                };
                            }

                            if (!customField.IsDescriptionNull())
                            {
                                tipManager.SetUltraToolTip(
                                    control,
                                    new UltraToolTipInfo(customField.Description, ToolTipImage.Info, customField.Name, DefaultableBoolean.True));
                            }

                            tableCustomFields.Controls.Add(control, 1, tableRowCount);

                            tableRowCount++;
                        }
                    }
                }

                tableCustomFields.RowCount = tableRowCount;

                const int heightOffset = 3;
                var tableHeight = tableRowCount > 0 ? tableCustomFields.Height  + heightOffset : 0;

                pnlCustomFields.Height = tableHeight;
                pnlCustomFields.Visible = tableRowCount > 0;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error creating custom fields.");
            }
        }


        /// <summary>
        /// Bind the part's values to custom fields.
        /// </summary>
        private void BindCustomFields()
        {
            try
            {
                _log.Debug("Binding custom fields.");

                if (Dataset != null)
                {
                    //find current customer and order

                    if (CurrentRecord is PartsDataset.PartRow part)
                    {
                        foreach (Control control in tableCustomFields.Controls)
                        {
                            if (!(control.Tag is CustomFieldUIInfo fieldInfo))
                            {
                                continue;
                            }

                            var customField = Dataset.PartLevelCustomField
                                .FindByPartLevelCustomFieldID(fieldInfo.PartLevelCustomFieldID);

                            var defaultValue = part.PartID > 0 || customField.IsDefaultValueNull() ?
                                string.Empty :
                                customField.DefaultValue;

                            var orderCustomField = Dataset.PartCustomFields
                                .FindByPartIDPartLevelCustomFieldID(part.PartID, customField.PartLevelCustomFieldID);

                            if (orderCustomField == null)
                            {
                                //check to see if it was deleted earlier, then added back
                                var deletedCustomField = Dataset.PartCustomFields
                                    .Select($"PartID = {part.PartID} AND PartLevelCustomFieldID = {customField.PartLevelCustomFieldID}", string.Empty, DataViewRowState.Deleted)
                                    .FirstOrDefault() as PartsDataset.PartCustomFieldsRow;

                                if (deletedCustomField != null)
                                {
                                    _log.Info($"Reversing custom field deletion for PartID = {part.PartID}, PartLevelCustomFieldID = {customField.PartLevelCustomFieldID}");
                                    deletedCustomField.RejectChanges();
                                    orderCustomField = deletedCustomField;
                                }
                                else
                                {
                                    orderCustomField = Dataset.PartCustomFields.AddPartCustomFieldsRow(
                                        part,
                                        customField,
                                        defaultValue);
                                }
                            }
                            else if (orderCustomField.IsValueNull())
                            {
                                orderCustomField.Value = defaultValue;
                            }

                            var comboEditor = control as UltraComboEditor;
                            if (comboEditor != null && !customField.IsListIDNull())
                            {
                                // Bind list contents
                                comboEditor.Items.Clear();
                                comboEditor.BeginUpdate();
                                comboEditor.Items.Add(string.Empty);
                                var listValues = Dataset.ListValues.Select($"ListID = {customField.ListID}");

                                var addCurrentValue = !orderCustomField.IsValueNull() &&
                                    !string.IsNullOrEmpty(orderCustomField.Value) &&
                                    listValues.All(v => v["Value"].ToString() != orderCustomField.Value);

                                if (addCurrentValue)
                                {
                                    comboEditor.Items.Add(orderCustomField.Value);
                                }

                                foreach (var row in listValues)
                                {
                                    comboEditor.Items.Add(row[Dataset.ListValues.ValueColumn.ColumnName]);
                                }

                                comboEditor.EndUpdate();
                            }

                            control.DataBindings.Clear();

                            // Add text binding - also works for combo editor
                            control.DataBindings.Add(new Binding("Text", orderCustomField, "Value", true)
                            {
                                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged,
                                NullValue = string.Empty
                            });
                        }
                    }
                }

                //ensure panel is hidden if it doesn't have any controls.
                if (pnlCustomFields.Visible && tableCustomFields.Controls.Count < 1)
                {
                    pnlCustomFields.Visible = false;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error binding custom fields.");
            }
        }

        #endregion

        #region Events

        private void cboManufacturer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if(this.cboManufacturer.SelectedItem != null && this.cboManufacturer.SelectedItem.DataValue != null)
                {
                    var filterValue = this.cboManufacturer.SelectedItem.DataValue.ToString().Replace("'", "''");

                    this.bsAirframe.Filter = this.Dataset.d_Airframe.ManufacturerIDColumn.ColumnName + " = '" + filterValue + "'";

                    //ensure selected airframe is still valid
                    var part = CurrentRecord as PartsDataset.PartRow;
                    if(part != null && !part.IsAirframeNull())
                    {
                        ValueListItem selectedAirframe = this.cboAirframe.FindItemByValue<string>(p => p == part.Airframe);

                        //if not a valid airframe for this
                        if(selectedAirframe == null)
                        {
                            this.cboAirframe.SelectedIndex = -1;
                            this.cboAirframe.DataBindings[0].WriteValue();
                        }
                    }

                    //if no airframe is selected then try and autoselect if this is a new part
                    if(part != null && part.PartID < 0 && part.IsAirframeNull())
                    {
                        string partName = part.Name.ToUpper();
                        EnumerableRowCollection<PartsDataset.d_AirframeRow> airFrames = this.Dataset.d_Airframe.Where(af => !af.IsManufacturerIDNull() && af.ManufacturerID == this.cboManufacturer.SelectedItem.DataValue.ToString() && !af.IsPartPrefixNull());
                        airFrames = airFrames.OrderByDescending(af => af.PartPrefix.Length);

                        foreach(PartsDataset.d_AirframeRow af in airFrames)
                        {
                            if(partName.StartsWith(af.PartPrefix.ToUpper()))
                            {
                                part.Airframe = af.AirframeID;
                                this.cboAirframe.DataBindings[0].ReadValue();
                            }
                        }
                    }
                }
                else
                    this.bsAirframe.Filter = this.Dataset.d_Airframe.ManufacturerIDColumn.ColumnName + " IS NULL";
            }
            catch(Exception exc)
            {
                string errorMsg = "Error updating binding source for airframe.";
                _log.Error(exc, errorMsg);
            }
        }

        private void txtPartName_TextChanged(object sender, EventArgs e)
        {
            if (!(CurrentRecord is PartsDataset.PartRow part))
            {
                return;
            }

            this.txtPartName.DataBindings[0].WriteValue();
            OnUpdateCurrentNodeUI();
        }

        private void processPriceWidget_PriceChanged(object sender, PriceChangedEventArgs e)
        {
            const string MAX_WARNING = "Total price exceeded maximum value. Please correct process prices as needed.";
            const string MIN_WARNING = "Total price is below minimum value. Please correct process prices as needed.";
            const string WARNING_TITLE = "Incorrect Total Price";
            const int currencyRounding = 2;

            try
            {
                if (!processWidget.Visible || e.NewAmount == 0M || !(CurrentRecord is PartsDataset.PartRow currentPart))
                {
                    return;
                }

                if (e.PricingStrategy == PricingStrategy.Each && (currentPart.IsEachPriceNull() || currentPart.EachPrice != e.NewAmount))
                {
                    decimal newValue;
                    if (e.NewAmount > curEachPrice.MaxValue)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(MAX_WARNING, WARNING_TITLE);
                        newValue = curEachPrice.MaxValue;
                    }
                    else if (e.NewAmount < curEachPrice.MinValue)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(MIN_WARNING, WARNING_TITLE);
                        newValue = curEachPrice.MinValue;
                    }
                    else
                    {
                        newValue = e.NewAmount;
                    }

                    currentPart.EachPrice = Math.Round(newValue, currencyRounding);
                }
                else if (e.PricingStrategy == PricingStrategy.Lot && (currentPart.IsLotPriceNull() || currentPart.LotPrice != e.NewAmount))
                {
                    decimal newValue;
                    if (e.NewAmount > curLotPrice.MaxValue)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(MAX_WARNING, WARNING_TITLE);
                        newValue = curLotPrice.MaxValue;
                    }
                    else if (e.NewAmount < curLotPrice.MinValue)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(MIN_WARNING, WARNING_TITLE);
                        newValue = curLotPrice.MinValue;
                    }
                    else
                    {
                        newValue = e.NewAmount;
                    }

                    currentPart.LotPrice = Math.Round(newValue, currencyRounding);
                }

                bsData.ResetBindings(false); // forces UI to update w/ new price
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating prices");
            }
        }

        private void curEachPrice_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                processWidget.UpdatePrices(PricingStrategy.Each, curEachPrice.Value);
                _log.Info("Updated per-process each prices.");
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after validating curEachPrice.");
            }
        }

        private void curLotPrice_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                processWidget.UpdatePrices(PricingStrategy.Lot, curLotPrice.Value);
                _log.Info("Updated per-process lot prices.");
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after validating curLotPrice.");
            }
        }

        private void NumWeight_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (numWeight.Value is decimal weight && weight > 0)
                {
                    processWidget.UpdateWeights(weight);
                }
                else
                {
                    processWidget.ClearWeights();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after validating numWeight.");
            }
        }

        #endregion

        #region PartName Validator

        private class PartNameControlValidator: ControlValidatorBase
        {
            #region Fields

            private PartInfo _partInfo;

            #endregion

            #region Methods

            public PartNameControlValidator(Control control, PartInfo partInfo)
                : base(control)
            {
                this._partInfo = partInfo;
            }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                if (Control is UltraTextEditor editor && editor.Enabled && !IsOnlyActivePartName())
                {
                    e.Cancel = true;
                    FireAfterValidation(false, "Only one part '" + editor.Text + "'can be active at one time.");
                    return;
                }

                //passed
                e.Cancel = false;
                FireAfterValidation(true, String.Empty);
            }

            private bool IsOnlyActivePartName()
            {
                var part = this._partInfo.CurrentRecord as PartsDataset.PartRow;

                if(part == null)
                    return true;

                //if not Active then don't worry about it
                if(!this._partInfo.chkActive.Checked) //use check box because may not have written to datatable yet
                    return true;

                //NOTE: Use _control.Text NOT part.Name because the value in part.name may have been changed by another validator but not updated the databindings yet
                //find all other parts with same name and are active
                DataRow[] activeParts = this._partInfo.Dataset.Part.Select("PartID <> " + part.PartID + " AND Name = '" + Data.Datasets.Utilities.SqlBless(Control.Text) + "' AND Active = 'true' AND CustomerID = " + part.CustomerID);

                //if someone else is active
                return activeParts.Length <= 0;
            }

            public override void Dispose()
            {
                this._partInfo = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region CustomFieldUIInfo

        private class CustomFieldUIInfo
        {
            public int PartLevelCustomFieldID { get; set; }
            public DisplayValidator Validator { get; set; }
        }

        #endregion


        private void tbNotesReadonly_TextChanged(object sender, EventArgs e)
        {
            var test = sender;
        }

        private void pnlMain_Resize(object sender, EventArgs e)
        {
           //pnlMain.AutoScrollMinSize = new Size(pnlMain.Width, pnlMain.Height);
        }
    }
}