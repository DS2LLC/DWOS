using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;
using DWOS.UI.Reports;
using DWOS.Utilities.Validation;
using Infragistics.UltraGauge.Resources;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using NLog;
using DWOS.Data;


using Admin = DWOS.UI.Admin;

namespace DWOS.UI.ShippingRec
{
    public partial class Receiving : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static Color PART_ISSUE_COLOR = Color.FromArgb(60, 255, 0, 0);
        private static Color PART_OK_COLOR = Color.FromArgb(60, 0, 255, 0);

        private PartsDataset.d_AirframeDataTable _airframes;
        private PartsDataset.ReceivingRow _currentRow;
        private List<int> _customerPartsLoaded = new List<int>();
        private int _lastReceivingId;
        private List<int> _loadedParts = new List<int>();
        private DWOS.Utilities.Validation.ValidatorManager _manager = new DWOS.Utilities.Validation.ValidatorManager();

        /// <summary>
        /// When true, ignores a specific event that can be triggered multiple
        /// times while saving data.
        /// </summary>
        private bool _ignoreCboPartSelectionChangedEvent;

        #endregion

        #region Methods

        public Receiving()
        {
            InitializeComponent();

            if (DesignMode)
                return;

            var watcher = new SecurityFormWatcher(this, this.btnCancel);
        }

        private void LoadData()
        {
            try
            {
                this.numWeight.MaskInput = string.Format("nnnn.{0} lbs",
                               string.Concat(Enumerable.Repeat("n", ApplicationSettings.Current.WeightDecimalPlaces)));

                this.dsParts.EnforceConstraints = false;

                this.dsParts.Customer.BeginLoadData();
                this.taCustomer.FillByActive(this.dsParts.Customer);
                this.dsParts.Customer.EndLoadData();

                using (new UsingDataTableLoad(dsParts.d_Priority))
                {
                    taPriority.Fill(dsParts.d_Priority);
                }

                this.partMedia.Setup(new MediaWidget.SetupArgs()
                {
                    MediaJunctionTable = this.dsParts.Part_Media,
                    MediaTable = this.dsParts.Media,
                    MediaJunctionTableParentRowColumn = this.dsParts.Part_Media.PartIDColumn,
                    MediaJunctionTableDefaultColumn = this.dsParts.Part_Media.DefaultMediaColumn,
                    AllowEditing = true,
                    MediaLinkType = Documents.LinkType.Part,
                    DocumentLinkTable = this.dsParts.Part_DocumentLink,
                    ScannerSettingsType = Data.ScannerSettingsType.Part
                });

                this.orderMedia.Setup(new MediaWidget.SetupArgs()
                {
                    MediaJunctionTable = this.dsParts.Receiving_Media,
                    MediaTable = this.dsParts.Media,
                    MediaJunctionTableParentRowColumn = this.dsParts.Receiving_Media.ReceivingIDColumn,
                    AllowEditing = true,
                    MediaLinkType = Documents.LinkType.Receiving,
                    DocumentLinkTable = this.dsParts.Receiving_DocumentLink,
                    ScannerSettingsType = Data.ScannerSettingsType.Order
                });

                this.cboCustomer.SelectedIndex = 0;

                this.dteReqDate.Value = null; // DateTime.Today.AddDays(ApplicationSettings.Current.DefaultProcessLeadTime);

                //this._manager.Add(new ImageDisplayValidator(new NumericControlValidator(this.numPartQty) { MinimumValue = 0 }, this.errProvider));

                var fields = new ApplicationSettingsDataSet.FieldsDataTable();

                using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                {
                    ta.FillByCategory(fields, "Part");
                }

                var partWeightField = fields.FirstOrDefault(f => f.Name == "Weight");

                if (partWeightField != null)
                {
                    if (!partWeightField.IsVisible)
                    {
                        pnlWeight.Visible = false;
                        pnlWeight.Height = 0;
                    }
                    else if (partWeightField.IsRequired)
                    {
                        this._manager.Add(new ImageDisplayValidator(new WeightValidator(this.numWeight)
                        {
                            AllowNull = () =>
                            {
                                var rowView = this.cboPart.SelectedItem?.ListObject as DataRowView;
                                return rowView?.Row != null;
                            }
                        }));
                    }
                }

                cboPriority.Text = Settings.Default.OrderPriorityDefault;
                cboPriority.ReadOnly = !SecurityManager.Current.IsInRole("Receiving.Priority");


                //this.dteReqDate.Value = null;
                this.partShapeWidget1.LoadData();
                this.partShapeWidget1.LoadDataTables(this.dsParts.PartArea, this.dsParts.PartAreaDimension);
                this.partShapeWidget1.AddValidators(this._manager, this.errProvider);
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        private PartsDataset.ReceivingRow SaveData()
        {
            try
            {
                _log.Info("Saving receiving part and image.");

                if (!this._manager.ValidateControls())
                    return null;

                //validate customer selected
                if (this.cboCustomer.SelectedItem == null)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("No valid customer selected.", "Invalid Operation");
                    return null;
                }
                this.bsParts.EndEdit();

                var selectedCustomer = ((DataRowView)this.cboCustomer.SelectedItem.ListObject).Row as PartsDataset.CustomerRow;

                var partQuantity = Convert.ToInt32(this.numPartQty.Value); //cache before update selected part may reset it
                var selectedPart = UpdateSelectedPart(selectedCustomer);

                //if we now have a part
                if (selectedPart != null)
                {
                    //add the new received info
                    if (this._currentRow == null)
                        this._currentRow = this.dsParts.Receiving.NewReceivingRow();

                    this._currentRow.PartID = selectedPart.PartID;
                    this._currentRow.PartQuantity = partQuantity;
                    this._currentRow.CustomerID = selectedCustomer.CustomerID;
                    this._currentRow.PurchaseOrder = this.txtPO.Text;
                    this._currentRow.CustomerWO = this.txtCustomerWO.Text;
                    this._currentRow.Priority = this.cboPriority.Text;
                    if(this.neContainers.Value != null)
                        this._currentRow.Containers = (int)this.neContainers.Value;
                    if(this.dteReqDate.Value != null)
                        this._currentRow.ReqDate = (DateTime)this.dteReqDate.Value;

                    if (this._currentRow.RowState == DataRowState.Detached)
                        this.dsParts.Receiving.AddReceivingRow(this._currentRow);

                    //update any values that did not get saved
                    var nullParents = this.dsParts.Receiving_Media.Where(rm => rm.RowState != DataRowState.Deleted && rm.IsNull(this.dsParts.Receiving_Media.ReceivingIDColumn.ColumnName));
                    nullParents.ForEach(np => np[this.dsParts.Receiving_Media.ReceivingIDColumn.ColumnName] = this._currentRow.ReceivingID);

                    var incompleteLinks = this.dsParts.Receiving_DocumentLink
                        .Where(link => link.IsValidState() && link.LinkToKey == DWOS.UI.Documents.DocumentUtilities.RECEIVING_ID);

                    foreach (var link in incompleteLinks)
                    {
                        link.LinkToKey = this._currentRow.ReceivingID;
                    }

                    //update database
                    this.taReceiving.Update(this.dsParts.Receiving);
                    this.taMedia.Update2(this.dsParts);
                    this.taReceiving_Media.Update(this.dsParts.Receiving_Media);
                    this.taReceivingDocumentLink.Update(this.dsParts.Receiving_DocumentLink);

                    var tempRow = this._currentRow;

                    UpdateTotalPartCount();
                    UpdateTotalContainerCount();
                    ResetOrder();

                    return tempRow;
                }
                MessageBoxUtilities.ShowMessageBoxWarn("No valid part was selected.", "Invalid Operation");
                return null;
            }
            catch (Exception exc)
            {
                _log.Warn("DataSet Errors: " + this.dsParts.GetDataErrors());
                ErrorMessageBox.ShowDialog("Error saving the data.", exc);
                return null;
            }
        }

        private PartsDataset.PartRow UpdateSelectedPart(PartsDataset.CustomerRow selectedCustomer)
        {
            PartsDataset.PartRow selectedPart = null;

            try
            {
                this._ignoreCboPartSelectionChangedEvent = true;

                //if part does not exist
                if (this.cboPart.SelectedItem == null)
                {
                    var partName = this.cboPart.Text != null ? this.cboPart.Text.Trim() : null;

                    if (string.IsNullOrEmpty(partName))
                        return null;

                    _log.Info("Adding new part " + partName + " to customer " + selectedCustomer.Name);

                    //add new part then select it in cbo	
                    selectedPart = this.dsParts.Part.NewPartRow();
                    selectedPart.Name = partName;
                    selectedPart.CustomerRow = selectedCustomer;
                    selectedPart.Active = true;
                    selectedPart.PartMarking = false;
                    selectedPart.LastModified = DateTime.Now;
                    selectedPart.Notes = this.txtNotes.Text;
                    selectedPart.CreatedInReceiving = true;

                    if (this.numWeight.Value != null && this.numWeight.Value != DBNull.Value)
                    {
                        selectedPart.Weight = Convert.ToDecimal(this.numWeight.Value);
                    }

                    // Part must be attached before calling partShapeWidget1.SaveRow()
                    this.dsParts.Part.AddPartRow(selectedPart);

                    this.partShapeWidget1.CurrentPart = selectedPart;
                    this.partShapeWidget1.SaveRow();

                    AutoSelectAirframe(selectedPart);

                    _log.Info("Added part row with part id: " + selectedPart.PartID);

                    //update any media junctions not saved
                    var nullParents = this.dsParts.Part_Media.Where(rm => rm.RowState != DataRowState.Deleted && rm.IsNull(this.dsParts.Part_Media.PartIDColumn.ColumnName));
                    nullParents.ForEach(np => np[this.dsParts.Part_Media.PartIDColumn.ColumnName] = selectedPart.PartID);

                    var incompleteLinks = this.dsParts.Part_DocumentLink
                        .Where(link => link.IsValidState() && link.LinkToKey == DWOS.UI.Documents.DocumentUtilities.RECEIVING_PART_ID);

                    foreach (var link in incompleteLinks)
                    {
                        link.LinkToKey = selectedPart.PartID;
                    }

                    this.bsParts.ResetBindings(false);
                    this.bsParts.Position = this.bsParts.Find(this.dsParts.Part.PartIDColumn.ColumnName, selectedPart.PartID);

                    this.taPart.Update(selectedPart);

                    //update any media changes
                    this.taMedia.Update2(this.dsParts);
                    this.taPart_Media.Update(this.dsParts);
                    this.taPartDocumentLink.Update(this.dsParts);

                    // update any area changes
                    this.taPartArea.Update(this.dsParts);
                    this.taPartAreaDimension.Update(this.dsParts);

                    this._ignoreCboPartSelectionChangedEvent = false;
                    cboPart_SelectionChanged(null, null);
                }
                else //else select existing part
                {
                    selectedPart = ((DataRowView)this.cboPart.SelectedItem.ListObject).Row as PartsDataset.PartRow;

                    if (selectedPart != null)
                    {
                        _log.Info("Updating part row with part id: " + selectedPart.PartID);

                        this.partShapeWidget1.SaveRow();
                        selectedPart.Notes = this.txtNotes.Text;

                        if (this.numWeight.Value != null && this.numWeight.Value != DBNull.Value)
                        {
                            selectedPart.Weight = Convert.ToDecimal(this.numWeight.Value);
                        }

                        //ensure part is active
                        if (!selectedPart.Active)
                            selectedPart.Active = true;

                        //update part
                        this.taPart.Update(selectedPart);

                        //update any media changes
                        this.taMedia.Update2(this.dsParts);
                        this.taPart_Media.Update(this.dsParts);
                        this.taPartDocumentLink.Update(this.dsParts);

                        // update any area changes
                        this.taPartArea.Update(this.dsParts);
                        this.taPartAreaDimension.Update(this.dsParts);
                    }
                }

                this._ignoreCboPartSelectionChangedEvent = false;

                return selectedPart;
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error updating selected part.", exc);
                return selectedPart;
            }
        }

        private void ResetOrder()
        {
            if (this._currentRow != null)
            {
                if (this._currentRow.RowState == DataRowState.Added || this._currentRow.RowState == DataRowState.Modified)
                    this._currentRow.Delete();
                else if (this._currentRow.RowState == DataRowState.Detached || this._currentRow.RowState == DataRowState.Deleted)
                    this._currentRow = null;
            }

            this._currentRow = null;

            this.txtPO.ResetText();
            this.cboPart.Focus();

            this.txtCustomerWO.ResetText();

            this.orderMedia.ClearMedia();

            //this.dteReqDate.Value = null;

            this.neContainers.Value = 0;

            UpdateDefaultFieldsPerCustomer();

            ResetPart();
        }

        private void ResetPart()
        {
            //clear part
            this.cboPart.ResetText();
            this.cboPart.SelectedItem = null;

            //clear part qty
            this.numPartQty.ResetValue();

            this.numWeight.Value = null;

            this.partShapeWidget1.LoadRow(null);
            this.partMedia.ClearMedia();
            this.txtNotes.ResetText();
        }

        private void UpdateTotalPartCount()
        {
            try
            {
                object partReceivedObject = this.taReceivingProduction.GetCurrentPartRecCount();
                int partReceivedCount = 0;

                if (partReceivedObject != null && partReceivedObject != DBNull.Value)
                    int.TryParse(partReceivedObject.ToString(), out partReceivedCount);

                var g = this.guagePartCount.Gauges[0] as DigitalGauge;
                g.Text = partReceivedCount.ToString();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error setting total part count gauge.");
            }
        }

        private void UpdateTotalContainerCount()
        {
            try
            {
                object ContainersReceivedObject = this.taReceivingProduction.GetCurrentContainersRecCount();
                int ContainersReceivedCount = 0;

                if (ContainersReceivedObject != null && ContainersReceivedObject != DBNull.Value)
                    int.TryParse(ContainersReceivedObject.ToString(), out ContainersReceivedCount);

                var g = this.guageContainers.Gauges[0] as DigitalGauge;
                g.Text = ContainersReceivedCount.ToString();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error setting total container count gauge.");
            }
        }


        private void UpdateDefaultFieldsPerCustomer()
        {
            try
            {
                int customerID = 0;

                if (this.cboCustomer.SelectedItem != null)
                    customerID = (((DataRowView)this.cboCustomer.SelectedItem.ListObject).Row as PartsDataset.CustomerRow).CustomerID;

                //if customer has a Default PO then set it for this customer...
                if (customerID > 0)
                {
                    var po = GetDefaultFieldValue(customerID, "PO");
                    if (!String.IsNullOrWhiteSpace(po))
                        this.txtPO.Text = po;

                    var customerWO = GetDefaultFieldValue(customerID, "Customer WO");
                    if (!String.IsNullOrWhiteSpace(customerWO))
                        this.txtCustomerWO.Text = customerWO;
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error setting blanked PO.";
                _log.Error(exc, errorMsg);
            }
        }

        private static string GetDefaultFieldValue(int customerID, string field)
        {
            using (var ta = new Customer_FieldsTableAdapter())
                return ta.GetDefaultValue(field, customerID);
        }

        private void FormatPartItems()
        {
            foreach (var item in this.cboPart.Items)
            {
                if (item.ListObject is DataRowView && !Convert.ToBoolean(((DataRowView)item.ListObject).Row["Active"]))
                {
                    item.Appearance.ForeColor = Color.Red;
                    item.Appearance.FontData.Italic = DefaultableBoolean.True;
                }
            }
        }

        private void LoadSelectedPartsImage(PartsDataset.PartRow partRow)
        {
            if (partRow != null)
            {
                //load part media data
                if (partRow.PartID > 0 && !this._loadedParts.Contains(partRow.PartID))
                {
                    this.taMedia.FillByPartIDWithoutMedia(this.dsParts.Media, partRow.PartID);
                    this.taPart_Media.FillByPartID(this.dsParts.Part_Media, partRow.PartID);
                    this.taPartDocumentLink.FillByPartID(this.dsParts.Part_DocumentLink, partRow.PartID);
                }

                //load images
                this.partMedia.LoadMedia(partRow.GetPart_MediaRows().ToList<DataRow>(),
                    partRow.GetPart_DocumentLinkRows().ToList<DataRow>(),
                    partRow.PartID);
            }
        }

        /// <summary>
        /// Clears unsaved media without a PartID.
        /// </summary>
        /// <remarks>
        /// This method fixes a bug where saving the Receiving Order would
        /// throw an exception due to a 'null PartID' database constraint.
        /// </remarks>
        private void CleanMediaForNewPart()
        {
            var mediaForUnsavedPart = this.dsParts
                .Part_Media
                .Where(rm => rm.IsValidState() && rm.IsNull(this.dsParts.Part_Media.PartIDColumn.ColumnName))
                .ToList();

            var linksForUnsavedPart = this.dsParts
                .Part_DocumentLink
                .Where(link => link.IsValidState() && link.LinkToKey == Documents.DocumentUtilities.RECEIVING_PART_ID)
                .ToList();

            foreach (var unsavedMedia in mediaForUnsavedPart)
            {
                unsavedMedia.Delete();
            }

            foreach (var unsavedLink in linksForUnsavedPart)
            {
                unsavedLink.Delete();
            }
        }

        private void LoadSelectedPartsArea(PartsDataset.PartRow partRow)
        {
            bool loadArea = partRow != null &&
                partRow.PartID > 0 &&
                !this._loadedParts.Contains(partRow.PartID);

            if (loadArea)
            {
                this.taPartArea.FillByPartID(this.dsParts.PartArea, partRow.PartID);
                this.taPartAreaDimension.FillByPartID(this.dsParts.PartAreaDimension, partRow.PartID);
            }
        }

        /// <summary>
        ///     Automatically select the airframe based on the part number
        /// </summary>
        /// <param name="partRow">The part row.</param>
        private void AutoSelectAirframe(PartsDataset.PartRow partRow)
        {
            try
            {
                if (this._airframes == null)
                {
                    using (var taAir = new Data.Datasets.PartsDatasetTableAdapters.d_AirframeTableAdapter())
                    {
                        this._airframes = new PartsDataset.d_AirframeDataTable();
                        taAir.Fill(this._airframes);
                    }
                }

                string partName = partRow.Name.ToUpper();

                foreach (var airframe in this._airframes.Where(af => !af.IsPartPrefixNull() && !af.IsManufacturerIDNull()))
                {
                    if (partName.StartsWith(airframe.PartPrefix.ToUpper()))
                    {
                        partRow.ManufacturerID = airframe.ManufacturerID;
                        partRow.Airframe = airframe.AirframeID;
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error auto selecting the airframe based on the name of the part.");
            }
        }

        private void OnDispose()
        {
            this._customerPartsLoaded = null;

            if (this._manager != null)
                this._manager.Dispose();
            this._manager = null;

            if (this._airframes != null)
                this._airframes.Dispose();

            this._airframes = null;
            this._currentRow = null;
        }

        private void ImportLast()
        {
            try
            {
                if (this._lastReceivingId > 0)
                {
                    var receiveLast = this.dsParts.Receiving.FindByReceivingID(this._lastReceivingId);

                    if (receiveLast != null)
                    {
                        this.txtPO.Text = receiveLast.IsPurchaseOrderNull() ? null : receiveLast.PurchaseOrder;
                        this.txtCustomerWO.Text = receiveLast.IsCustomerWONull() ? null : receiveLast.CustomerWO;
                        cboPriority.Text = receiveLast.IsPriorityNull() ? null : receiveLast.Priority;

                        if (this._currentRow == null)
                            this._currentRow = this.dsParts.Receiving.NewReceivingRow();
                        if (!receiveLast.IsReqDateNull())
                            this._currentRow.ReqDate = receiveLast.ReqDate; 
                        this._currentRow.PartID = receiveLast.PartID;
                        this._currentRow.CustomerID = receiveLast.CustomerID;
                        this._currentRow.PartQuantity = receiveLast.PartQuantity;

                        if (this._currentRow.RowState == DataRowState.Detached)
                            this.dsParts.Receiving.AddReceivingRow(this._currentRow);

                        foreach (var receivingMediaRow in receiveLast.GetReceiving_MediaRows())
                            this.dsParts.Receiving_Media.AddReceiving_MediaRow(this._currentRow, receivingMediaRow.MediaRow);

                        foreach (var receivingLink in receiveLast.GetReceiving_DocumentLinkRows())
                        {
                            this.dsParts.Receiving_DocumentLink.AddReceiving_DocumentLinkRow(
                                receivingLink.DocumentInfoID,
                                Documents.LinkType.Receiving.ToString(),
                                this._currentRow);
                        }

                        this.orderMedia.LoadMedia(this._currentRow.GetReceiving_MediaRows().ToList<DataRow>(),
                            this._currentRow.GetReceiving_DocumentLinkRows().ToList<DataRow>(),
                            this._currentRow.ReceivingID);
                    }

                    this._lastReceivingId = 0; //reset to prevent double import
                }

                this.btnImport.Enabled = this._lastReceivingId > 0;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error importing last id.");
            }
        }

        #endregion

        #region Events

        private void Receiving_Load(object sender, EventArgs e)
        {
            try
            {
                using (new UsingWaitCursor())
                {
                    LoadData();

                    this.btnSave.Enabled = this.grpPart.Enabled = SecurityManager.Current.IsInRole("Receiving");
                    this.numPrintQty.Value = Settings.Default.ReceivingPrintQuantity;

                    //Logic for displaying add parts button - Matt F.
                    this.addPartsBtn.Enabled = ApplicationSettings.Current.ReceivingCanAddParts;
                    this.addPartsBtn.Visible = ApplicationSettings.Current.ReceivingCanAddParts;

                    //Logic for displaying add parts button - Dustin
                    UpdateTotalPartCount();
                    UpdateTotalContainerCount();

                    this.cboPart.ValueList.FormatFilteredItems = DefaultableBoolean.True;
                }
            }
            catch (Exception exc)
            {
                this.btnSave.Enabled = this.grpPart.Enabled = false;
                ErrorMessageBox.ShowDialog("Error displaying Receiving dialog.", exc);
            }
        }

        private void cboCustomer_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboCustomer.SelectedItem != null)
                {
                    var customerID = Convert.ToInt32(this.cboCustomer.SelectedItem.DataValue);

                    //if customer data not loaded then load
                    if (!this._customerPartsLoaded.Contains(customerID))
                    {
                        using (new UsingWaitCursor(this))
                            this.taPart.FillByCustomerAll(this.dsParts.Part, customerID);

                        this._customerPartsLoaded.Add(customerID);
                    }

                    this.bsParts.Filter = this.dsParts.Part.CustomerIDColumn.ColumnName + " = " + this.cboCustomer.SelectedItem.DataValue;
                }
                else
                    this.bsParts.Filter = null;

                ResetOrder();

                FormatPartItems();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating customer.");
            }
        }

        private void cboPart_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (!this._ignoreCboPartSelectionChangedEvent)
                {
                    //reset part selections
                    this.partMedia.ClearMedia();
                    this.partShapeWidget1.LoadRow(null);
                    //this.numPartQty.ResetValue();
                    this.numWeight.Value = null;
                    this.txtNotes.ResetText();

                    if (this.cboPart.SelectedItem != null)
                    {
                        var pr = ((DataRowView)this.cboPart.SelectedItem.ListObject).Row as PartsDataset.PartRow;

                        // LoadSelectedPartsArea loads area info used by
                        // the part shape widget when it's loading a row.
                        LoadSelectedPartsArea(pr);
                        this.partShapeWidget1.LoadRow(pr);

                        LoadSelectedPartsImage(pr);
                        CleanMediaForNewPart();

                        if (pr != null && pr.PartID > 0 && !this._loadedParts.Contains(pr.PartID))
                        {
                            this._loadedParts.Add(pr.PartID);
                        }

                        this.numPartQty.Focus();
                        this.txtNotes.Text = pr.IsNotesNull() ? null : pr.Notes;
                        this.numWeight.Value = pr.IsWeightNull() ? null : (object)pr.Weight;
                    }

                    //update border
                    cboPart_TextChanged(null, null);
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error changing part selection.", exc);
            }
        }

        //public IReport CreateReport(string reportType)
        //{
        //    try
        //    {
        //        if (reportType == "COC Label")
        //        {
        //            return new COCLabelReport()
        //            {
        //                OrderId = base.DataRow.OrderID,
        //                COCId = base.DataRow.COCID
        //            };
        //        }
        //        else
        //        {
        //            return new COCReport(base.DataRow.COCID);
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        string errorMsg = "Error creating COC report.";
        //        ErrorMessageBox.ShowDialog(errorMsg, exc);
        //        return null;
        //    }
        //}


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (new UsingWaitCursor(this))
                {
                    Settings.Default.ReceivingPrintQuantity = Convert.ToInt32(this.numPrintQty.Value);

                    PartsDataset.ReceivingSummaryRow row = null;

                    //Qery receiving Summary
                    Data.Datasets.PartsDataset.ReceivingSummaryDataTable dtReicvingSummary = new PartsDataset.ReceivingSummaryDataTable();
                    using (var taReicvingSummary = new Data.Datasets.PartsDatasetTableAdapters.ReceivingSummaryTableAdapter())
                    {
                        var newRow = SaveData();
                        taReicvingSummary.FillByReceivingID(dtReicvingSummary, newRow.ReceivingID);

                        if (dtReicvingSummary != null)
                            row = (PartsDataset.ReceivingSummaryRow)dtReicvingSummary.Rows[0];
                    }

                    if (row != null)
                    {
                        //var rep = new OrderReceivingReport(row);
                        var printCount = Convert.ToInt32(this.numPrintQty.Value);

                        this._lastReceivingId = row.ReceivingID;
                        this.btnImport.Enabled = this._lastReceivingId > 0;

                        //print labels
                        if (this.cboPrintContainers.Checked || this.cboLabelPreview.Checked)
                        {

                            var rptReceivingContainer = new ReceivingContainerLabelReport();

                            rptReceivingContainer.ReceivingRow = row;
                            if (this.cboLabelPreview.Checked)
                                rptReceivingContainer.DisplayReport(new System.Threading.CancellationToken());
                            if (this.cboPrintContainers.Checked)
                            {
                                // Labels use a different default printer than other reports.
                                var labelPrinterName = !string.IsNullOrEmpty(UserSettings.Default.ShippingLabelPrinterName)
                                ? UserSettings.Default.ShippingLabelPrinterName
                                : PrinterUtilities.SelectPrinterNameDialog(Utilities.PrinterType.Label);
                                rptReceivingContainer.PrintLabel(labelPrinterName);
                            }
                               
                        }


                        //print Receipt
                        if ((this.cboPreview.Checked) || ((StateEditorButton)this.numPrintQty.ButtonsLeft[0]).Checked && printCount > 0)
                        {
                            var rep = new OrderReceivingReport(row);
                            if (((StateEditorButton)this.numPrintQty.ButtonsLeft[0]).Checked && printCount > 0)
                                rep.PrintReport(printCount);

                            if (this.cboPreview.Checked)
                                rep.DisplayReport();
                        }



                    }
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error printing order receipt.", exc);
            }
        }

        private void cboPart_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.cboPart.Text))
                {
                    this.cboPart.Appearance.ResetBackColor();
                    return;
                }

                this.cboPart.Appearance.BackColor = this.cboPart.SelectedItem == null ?
                    PART_ISSUE_COLOR :
                    PART_OK_COLOR;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error on part test changed");
            }
        }

        private void cboPart_Leave(object sender, EventArgs e)
        {
            try
            {
                this.cboPart.CloseUp();

                //if typed a new part then the selection changed event will not fire
                if (this.cboPart.SelectedItem == null)
                {
                    this.partMedia.ClearMedia();
                    this.partShapeWidget1.LoadRow(null);
                    this.txtNotes.ResetText();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error part leave.");
            }
        }

        private void numPartQty_Enter(object sender, EventArgs e)
        {
            try
            {
                this.numPartQty.SelectAll();
            }
            catch
            {
            }
        }

        private void ultraPanel1_Resize(object sender, EventArgs e)
        {
            //if ((grpPart.Width > this.Width) || (grpPart.Height > this.Height))
            //{
            //    grpPart.Dock = DockStyle.None;
            //}
            //else
            //{
            //    grpPart.Dock = DockStyle.Fill;
            //}
        }

        private void btnImport_Click(object sender, EventArgs e) { ImportLast(); }

        private void addPart_btnClick(object sender, EventArgs e)
        {
            //store state
            var currentCustomerIndex = this.cboCustomer.SelectedIndex;
            var currentPO = this.txtPO.Text;
            var currentWO = this.txtCustomerWO.Text;
            var currentPriorityindex = this.cboPriority.SelectedIndex;

            try
            {
                using (var p = new Admin.PartManager())
                {

                    var customerID = Convert.ToInt32(this.cboCustomer.SelectedItem.DataValue);
                    p.CustomerFilterID = customerID;
                    var result = p.ShowDialog(this);
                    if (result == DialogResult.OK || p.DataChanged)
                    {
                        //if data was changed then reload
                        if (p.DataChanged)
                        {
                            LogManager.GetCurrentClassLogger().Warn("Part data was changed so reload.");
                            using (new UsingWaitCursor(this))
                            {
                                this.dsParts.Clear(); //update parts table
                                this.dsParts.EnforceConstraints = false;

                                this.dsParts.Customer.BeginLoadData();
                                this.taCustomer.FillByActive(this.dsParts.Customer);
                                this.dsParts.Customer.EndLoadData();
                                this.taPart.FillByCustomerAll(this.dsParts.Part, customerID);

                                using (new UsingDataTableLoad(dsParts.d_Priority))
                                {
                                    taPriority.Fill(dsParts.d_Priority);
                                }
                            }
                            //rebind to ensure refreshes data
                            this.cboPart.DataBind();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error adding a new part.", exc);
            }

            //refresh state
            this.cboCustomer.SelectedIndex = currentCustomerIndex;
            this.cboPriority.SelectedIndex = currentPriorityindex;
            this.txtPO.Text = currentPO;
            this.txtCustomerWO.Text = currentWO;
            this.cboPart.Focus();
        }
        #endregion

        #region WeightValidator

        private class WeightValidator : ControlValidatorBase
        {
            #region Properties

            public Func<bool> AllowNull { get; set; }

            #endregion

            #region Methods

            public WeightValidator(UltraNumericEditor control) : base(control)
            {
            }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                if (AllowNull == null || !AllowNull())
                {
                    var numControl = this.Control as UltraNumericEditor;

                    // numWeight's input mask prevents user from inputting invalid values.

                    if (numControl.Value == null)
                    {
                        e.Cancel = true;
                        FireAfterValidation(false, "Please provide weight.");
                        return;
                    }
                }
            }

            #endregion
        }


        #endregion

    }
}