using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.UI.Utilities;

namespace DWOS.UI.Sales.Order
{
    public partial class BatchInformation : DataPanel
    {
        #region Fields

        private BatchOrderTableAdapter _taBatchOrder;
        private PartSummaryTableAdapter _taPartSummary;
        public event Action <OrderSearchField, string> QuickFilter;
        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("OrderEntry_BatchInformation", new UltraGridBandSettings());

        #endregion

        #region Properties

        public OrdersDataSet Dataset
        {
            get { return base._dataset as OrdersDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.Batch.BatchIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public BatchInformation()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads initial data into the control.
        /// </summary>
        /// <param name="dataset">Dataset to use.</param>
        /// <param name="taBatchOrder">TableAdapter instance for batch orders. Used to list all orders in the batch.</param>
        /// <param name="taPartSummary">TableAdapter instance for parts. Used to list all parts in the batch.</param>
        public void LoadData(OrdersDataSet dataset, BatchOrderTableAdapter taBatchOrder, PartSummaryTableAdapter taPartSummary)
        {
            this.Dataset = dataset;
            this._taBatchOrder = taBatchOrder;
            this._taPartSummary = taPartSummary;

            base.bsData.DataSource = Dataset;
            base.bsData.DataMember = Dataset.Batch.TableName;

            base.BindValue(this.txtBatch, Dataset.Batch.BatchIDColumn.ColumnName);
            base.BindValue(this.dteOpenDate, Dataset.Batch.OpenDateColumn.ColumnName);
            base.BindValue(this.dteCloseDate, Dataset.Batch.CloseDateColumn.ColumnName);
            base.BindValue(this.txtFixture, Dataset.Batch.FixtureColumn.ColumnName);
            base.BindValue(this.txtWorkStatus, Dataset.Batch.WorkStatusColumn.ColumnName);
            base.BindValue(this.txtCurrentLocation, Dataset.Batch.CurrentLocationColumn.ColumnName);

            base._panelLoaded = true;
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            base.BeforeMoveToNewRecord(id);
            this.grdBatchOrder.DataSource = null;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            var currentBatch = this.CurrentRecord as DWOS.Data.Datasets.OrdersDataSet.BatchRow;

            // Construct data to show in grdBatchOrder.
            if (currentBatch != null)
            {
                this._taBatchOrder.FillByBatchID(Dataset.BatchOrder, currentBatch.BatchID);

                var orderTable = new DataTable();
                orderTable.Columns.Add("OrderID");
                orderTable.Columns.Add("PartName");
                orderTable.Columns.Add("PartQuantity");

                foreach (var batchOrder in Dataset.BatchOrder.Where(row => row.BatchID == currentBatch.BatchID))
                {
                    var newRow = orderTable.NewRow();
                    newRow["OrderID"] = batchOrder.OrderID;
                    newRow["PartQuantity"] = batchOrder.PartQuantity;

                    newRow["PartName"] = this._taPartSummary.GetPartNameByOrder(batchOrder.OrderID) ?? "NA";
                    orderTable.Rows.Add(newRow);
                }

                this.grdBatchOrder.DataSource = orderTable;
            }
        }

        #endregion

        #region Events

        private void grdBatchOrder_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                grdBatchOrder.AfterColPosChanged -= grdBatchOrder_AfterColPosChanged;
                grdBatchOrder.AfterSortChange -= grdBatchOrder_AfterSortChange;

                var band = this.grdBatchOrder.DisplayLayout.Bands[0];
                band.Columns["OrderID"].Header.Caption = "Work Order";
                band.Columns["PartName"].Header.Caption = "Part Name";
                band.Columns["PartQuantity"].Header.Caption = "Batch Qty.";

                // Grid selects the first row by default - make it look unselected.
                this.grdBatchOrder.DisplayLayout.Override.ActiveRowAppearance.Reset();

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(band);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error initializing layout.");
            }
            finally
            {
                grdBatchOrder.AfterColPosChanged += grdBatchOrder_AfterColPosChanged;
                grdBatchOrder.AfterSortChange += grdBatchOrder_AfterSortChange;
            }
        }

        private void grdBatchOrder_AfterColPosChanged(object sender, Infragistics.Win.UltraWinGrid.AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdBatchOrder.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing column position in grid.");
            }
        }

        private void grdBatchOrder_AfterSortChange(object sender, Infragistics.Win.UltraWinGrid.BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdBatchOrder.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing column position in grid.");
            }
        }

        private void txtBatch_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            switch (e.Button.Key)
            {
                case "btnFilterBatch":
                    var handler = QuickFilter;
                    var currentBatch = base.CurrentRecord as DWOS.Data.Datasets.OrdersDataSet.BatchRow;

                    if (handler != null && currentBatch != null)
                    {
                        handler(OrderSearchField.Batch, currentBatch.BatchID.ToString());
                    }

                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
