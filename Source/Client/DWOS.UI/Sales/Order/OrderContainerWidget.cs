using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using Infragistics.Win.UltraWinGrid;
using NLog;
using DWOS.UI.Reports;
using DWOS.UI.Utilities;
using System.ComponentModel;

namespace DWOS.UI.Sales.Order
{
    public partial class OrderContainerWidget : UserControl
    {
        #region Fields

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();
        public event EventHandler ContainerCountChanged;
        private readonly GridSettingsPersistence<UltraGridBandSettings> _containerBandSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("OrderContainers_Container", new UltraGridBandSettings());

        private readonly GridSettingsPersistence<UltraGridBandSettings> _itemBandSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("OrderContainers_Item", new UltraGridBandSettings());

        #endregion

        #region Properties

        public OrdersDataSet Dataset
        {
            get;
            set;
        }

        public OrdersDataSet.OrderRow CurrentOrder { get; private set; }

        [Category("Behavior")]
        [Description("Specifies auto fit behavior for the container table.")]
        public bool AutoFit
        {
            get => grdContainers.DisplayLayout.AutoFitStyle != AutoFitStyle.None;
            set => grdContainers.DisplayLayout.AutoFitStyle = value
                ? AutoFitStyle.ResizeAllColumns
                : AutoFitStyle.None;
        }

        #endregion

        #region Methods

        public OrderContainerWidget()
        {
            InitializeComponent();
        }

        public void LoadData(OrdersDataSet dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.OrderContainers.TableName;
            bsData.Filter = "1 = 0";

            grdContainers.DataSource = bsData;
        }

        public void LoadOrder(OrdersDataSet.OrderRow orderRow)
        {
            CurrentOrder = orderRow;

            if (orderRow == null)
            {
                bsData.Filter = "1 = 0";
                return;
            }

            if(bsData.IsBindingSuspended)
                bsData.ResumeBinding(); //ensure nobody suspended binding

            //NOTE: Changed to use the filter method instead of position method, can't mix using the two
            bsData.Filter = Dataset.OrderContainers.OrderIDColumn.ColumnName + " = " + orderRow.OrderID;

            //force controls to read values
            bsData.ResetCurrentItem();
        }

        public void EndEditing()
        {
            _log.Debug("Ending edits for " + Name);
            bsData.EndEdit();

            //Added to fix the datarowview not being taken out of edit mode even when the EndEdit on the binding source was called
            //  Error arose from settings a required field with a FK to -1 and not moving to another bsData record, would still pass the -1 to Database
            if(bsData.Current is DataRowView && ((DataRowView) bsData.Current).IsEdit)
                ((DataRowView) bsData.Current).EndEdit();
        }

        public void CancelEdits()
        {
            _log.Debug("Canceling edits for " + Name);
            bsData.CancelEdit();
        }

        private void AddNewContainer(int orderID, int partsPerContainer)
        {
            var cr = Dataset.OrderContainers.NewOrderContainersRow();
            cr.OrderID = orderID;
            cr.PartQuantity = partsPerContainer;
            cr.IsActive = true;
            cr.ShipmentPackageTypeRow = Dataset.ShipmentPackageType.OrderBy(type => type.ShipmentPackageTypeID).FirstOrDefault();
            cr.EndEdit();

            Dataset.OrderContainers.AddOrderContainersRow(cr);
        }

        #endregion

        #region Events

        private void grdContainers_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            btnRemoveProcess.Enabled = grdContainers.Selected.Rows.Count > 0;
            btnPrint.Enabled = grdContainers.Selected.Rows.Count > 0;
            btnAddItem.Enabled = grdContainers.Selected.Rows.Count > 0;
        }

        private void grdContainers_Error(object sender, ErrorEventArgs e)
        {
            try
            {
                // If the error was caused by user setting an invalid value,
                // revert back to the original value to avoid an infinite
                // error loop.
                var cell = e.DataErrorInfo?.Cell;

                if (cell == null)
                {
                    return;
                }

                cell.Value = e.DataErrorInfo.Cell.OriginalValue;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error canceling update for labor grid error.");
            }

        }

        private void grdContainers_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                grdContainers.AfterColPosChanged -= grdContainers_AfterColPosChanged;
                grdContainers.AfterSortChange -= grdContainers_AfterSortChange;

                var layout = grdContainers.DisplayLayout;
                if (!layout.ValueLists.Exists("ShipmentPackageType"))
                {
                    var shipmentPackageTypeList = layout.ValueLists.Add("ShipmentPackageType");
                    foreach (var typeRow in Dataset.ShipmentPackageType.OrderBy(type => Name))
                    {
                        shipmentPackageTypeList.ValueListItems.Add(typeRow.ShipmentPackageTypeID, typeRow.Name);
                    }
                }

                var primaryBand = layout.Bands[0];
                primaryBand.Columns["ShipmentPackageTypeID"].ValueList = layout.ValueLists["ShipmentPackageType"];

                var itemBand = layout.Bands[1];
                itemBand.Columns["ShipmentPackageTypeID"].ValueList = layout.ValueLists["ShipmentPackageType"];

                // Load settings
                _containerBandSettingsPersistence.LoadSettings().ApplyTo(primaryBand);
                _itemBandSettingsPersistence.LoadSettings().ApplyTo(itemBand);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing main containers grid.");
            }
            finally
            {
                grdContainers.AfterColPosChanged += grdContainers_AfterColPosChanged;
                grdContainers.AfterSortChange += grdContainers_AfterSortChange;
            }
        }

        private void grdContainers_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                e.Row.ExpandAll();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing row.");
            }
        }

        private void grdContainers_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            try
            {
                var layout = grdContainers.DisplayLayout;
                var primaryBand = layout.Bands[0];
                var itemBand = layout.Bands[1];

                // Save only the band that changed
                if (e.ColumnHeaders.Any(header => header.Band == primaryBand))
                {
                    var primarySettings = new UltraGridBandSettings();
                    primarySettings.RetrieveSettingsFrom(primaryBand);
                    _containerBandSettingsPersistence.SaveSettings(primarySettings);
                }

                if (e.ColumnHeaders.Any(header => header.Band == itemBand))
                {
                    var itemSettings = new UltraGridBandSettings();
                    itemSettings.RetrieveSettingsFrom(itemBand);
                    _itemBandSettingsPersistence.SaveSettings(itemSettings);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing column position in grid.");
            }
        }

        private void grdContainers_AfterSortChange(object sender, Infragistics.Win.UltraWinGrid.BandEventArgs e)
        {
            try
            {
                var layout = grdContainers.DisplayLayout;
                var primaryBand = layout.Bands[0];
                var itemBand = layout.Bands[1];

                // Save only the band that changed
                if (e.Band == primaryBand)
                {
                    var primarySettings = new UltraGridBandSettings();
                    primarySettings.RetrieveSettingsFrom(primaryBand);
                    _containerBandSettingsPersistence.SaveSettings(primarySettings);
                }
                else if (e.Band == itemBand)
                {
                    var itemSettings = new UltraGridBandSettings();
                    itemSettings.RetrieveSettingsFrom(itemBand);
                    _itemBandSettingsPersistence.SaveSettings(itemSettings);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing column position in grid.");
            }
        }

        private void btnAddProcess_Click(object sender, EventArgs e)
        {
            try
            {
                var firstContainer = CurrentOrder;

                if(firstContainer != null && firstContainer.IsValidState())
                    AddNewContainer(firstContainer.OrderID, firstContainer.PartQuantity);

                ContainerCountChanged?.Invoke(this, EventArgs.Empty);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on add another container.");
            }
        }

        private void btnRemoveProcess_Click(object sender, EventArgs e)
        {
            try
            {
                var rowsToDelete = new List<DataRow>();

                foreach(var row in grdContainers.Selected.Rows)
                {
                    if(row.IsDataRow)
                    {
                        var container = ((DataRowView)row.ListObject).Row;

                        if(container != null)
                            rowsToDelete.Add(container);
                    }
                }

                foreach(var delete in rowsToDelete)
                    delete.Delete();

                ContainerCountChanged?.Invoke(this, EventArgs.Empty);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error removing the selected containers.");
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                foreach(var row in grdContainers.Selected.Rows)
                {
                    if(row.IsDataRow)
                    {
                        var container = ((DataRowView) row.ListObject).Row as OrdersDataSet.OrderContainersRow;

                        if(container != null)
                        {
                            var report = new ContainerLabelReport {Order = container.OrderRow, OrderContainer = container};
                            report.PrintReport();
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error print selected rows.");
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdContainers.Selected.Rows.Count == 0)
                {
                    return;
                }

                var firstSelectedRow = grdContainers.Selected.Rows[0];

                if (firstSelectedRow.IsDataRow)
                {
                    var container = (firstSelectedRow.ListObject as DataRowView)?.Row as OrdersDataSet.OrderContainersRow;

                    if (container != null)
                    {
                        Dataset.OrderContainerItem.AddOrderContainerItemRow(container, 1);
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error removing the selected containers.");
            }
        }

        #endregion
    }
}
