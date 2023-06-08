using System;
using System.Windows.Forms;
using DWOS.Shared;
using DWOS.Data;
using NLog;
using System.Collections;
using DWOS.UI.Utilities;

namespace DWOS.UI.Admin
{
    public partial class PriceUnitManager: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("PriceUnitManager", new UltraGridBandSettings());

        #endregion

        #region Methods

        public PriceUnitManager()
        {
            this.InitializeComponent();
        }

        private void LoadData()
        {
            this.dsOrders.EnforceConstraints = false;
            this.dsOrders.PriceUnit.BeginLoadData();

            this.taPriceUnit.Fill(this.dsOrders.PriceUnit);

            this.dsOrders.PriceUnit.EndLoadData();
        }

        private bool SaveData()
        {
            try
            {
                this.grdAirframe.UpdateData();
                this.taPriceUnit.Update(this.dsOrders.PriceUnit);

                return true;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving data.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);

                return false;
            }
        }

        #endregion

        #region Events

        /// <summary>
        ///   Handles the Load event of the AirframeManager control.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="System.EventArgs" /> instance containing the event data. </param>
        private void AirframeManager_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadData();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error loading airframe manager.";
                _log.Error(exc, errorMsg);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if(this.SaveData())
                    Close();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving data.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void grdAirframe_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                grdAirframe.AfterColPosChanged -= grdAirframe_AfterColPosChanged;
                grdAirframe.AfterSortChange -= grdAirframe_AfterSortChange;

                var band = grdAirframe.DisplayLayout.Bands[0];

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(band);

                // Sort by PriceUnitID by default
                band.Columns["PriceUnitID"].SortComparer = new DisplayNameSortComparer();
                band.SortedColumns.Add("PriceUnitID", false);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error initializing grid.");
            }
            finally
            {
                grdAirframe.AfterColPosChanged += grdAirframe_AfterColPosChanged;
                grdAirframe.AfterSortChange += grdAirframe_AfterSortChange;
            }
        }

        private void grdAirframe_AfterColPosChanged(object sender, Infragistics.Win.UltraWinGrid.AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdAirframe.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling column position change.");
            }
        }

        private void grdAirframe_AfterSortChange(object sender, Infragistics.Win.UltraWinGrid.BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdAirframe.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling column sort change.");
            }
        }

        /// <summary>
        /// Exit edit mode after changing Active's value.
        /// </summary>
        /// <remarks>
        /// This handler is required so that paired price units are enabled/disabled as a pair.
        /// Otherwise, the AfterCellUpdate event does not fire until focus changes.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdAirframe_CellChange(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (e.Cell != null && e.Cell.Column.Key == "Active")
            {
                grdAirframe.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode);
            }
        }

        /// <summary>
        /// Enables or disables the other half of a matching price unit pair.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdAirframe_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (e.Cell != null && e.Cell.Row.IsDataRow && e.Cell.Column.Key == "Active")
            {
                var priceUnit = OrderPrice.ParsePriceUnit(e.Cell.Row.Cells["PriceUnitID"].Text);
                bool enabled = (e.Cell.Value as bool?).GetValueOrDefault();

                OrderPrice.enumPriceUnit pairedPriceUnit;

                switch (priceUnit)
                {
                    case OrderPrice.enumPriceUnit.EachByWeight:
                        pairedPriceUnit = OrderPrice.enumPriceUnit.LotByWeight;
                        break;
                    case OrderPrice.enumPriceUnit.LotByWeight:
                        pairedPriceUnit = OrderPrice.enumPriceUnit.EachByWeight;
                        break;
                    case OrderPrice.enumPriceUnit.Each:
                        pairedPriceUnit = OrderPrice.enumPriceUnit.Lot;
                        break;
                    case OrderPrice.enumPriceUnit.Lot:
                        pairedPriceUnit = OrderPrice.enumPriceUnit.Each;
                        break;
                    default:
                        pairedPriceUnit = priceUnit;
                        break;
                }

                foreach (var row in grdAirframe.Rows)
                {
                    if (row != null && row.IsDataRow)
                    {
                        if (row.Cells["PriceUnitID"].Text == pairedPriceUnit.ToString())
                        {
                            row.Cells["Active"].SetValue(e.Cell.Value, true);
                        }
                        else if (!enabled && row.Cells["PriceUnitID"].Text != priceUnit.ToString())
                        {
                            // Ensures that the other pair of price units is enabled.
                            row.Cells["Active"].SetValue(true, true);
                        }

                        row.Update();
                    }
                }
            }
        }



        #endregion

        #region DisplayNameSortComparer

        private class DisplayNameSortComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var xCell = x as Infragistics.Win.UltraWinGrid.UltraGridCell;
                var yCell = y as Infragistics.Win.UltraWinGrid.UltraGridCell;

                if (xCell == null && yCell != null)
                {
                    return 1;
                }
                else if (xCell != null && yCell == null)
                {
                    return -1;
                }
                else
                {
                    int xValue = GetValue(Convert.ToString(xCell.Value));
                    int yValue = GetValue(Convert.ToString(yCell.Value));

                    return xValue - yValue;
                }
            }

            private static int GetValue(string priceUnitID)
            {
                var priceUnit = OrderPrice.ParsePriceUnit(priceUnitID);

                switch (priceUnit)
                {
                    case OrderPrice.enumPriceUnit.Each:
                        return 1;
                    case OrderPrice.enumPriceUnit.Lot:
                        return 2;
                    case OrderPrice.enumPriceUnit.EachByWeight:
                        return 3;
                    case OrderPrice.enumPriceUnit.LotByWeight:
                        return 4;
                    default:
                        return 0;
                }
            }
        }

        #endregion
    }
}