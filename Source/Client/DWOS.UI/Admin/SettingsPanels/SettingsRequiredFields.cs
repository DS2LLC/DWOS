using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Windows.Forms;
using DWOS.Data;
using Infragistics.Win.UltraWinGrid;
using System.Collections.Generic;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsRequiredFields : UserControl, ISettingsPanel
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("Settings_Fields", Default);

        /// <summary>
        /// Gets a set of fields that cannot be marked as required.
        /// </summary>
        private readonly ISet<string> _nonRequiredFields = new HashSet<string>
        {
            // Adjusted Est. Ship Date is typically blank but has a value if
            // the customer-facing est. shipping date is different than the
            // internal one.
            "Adjusted Est. Ship Date",

            // Requiring a tracking number for all packages prior to shipment
            // would very likely cause serious issues in shipping.
            "Tracking Number"
        };

        #endregion

        #region Properties

        private static UltraGridBandSettings Default =>
            new UltraGridBandSettings
            {
                ColumnSort = new Dictionary<string, UltraGridBandSettings.ColumnSortSettings>
                {
                    { "Category", new UltraGridBandSettings.ColumnSortSettings { IsDescending = false, SortIndex = 0, IsGroupByColumn = true } }
                }
            };

        #endregion

        #region Methods
        public bool CanDock
        {
            get { return true; }
        }
        public SettingsRequiredFields()
        {
            InitializeComponent();
        }

        #endregion Methods

        #region ISettingsPanel Members

        public bool Editable
        {
            get { return SecurityManager.Current.IsInRole("ApplicationSettings.Edit"); }
        }

        public string PanelKey
        {
            get
            {
                return "Fields";
            }
        }

        public void LoadData()
        {
            try
            {
                Enabled = this.Editable;
                chkSyncWeight.Checked = ApplicationSettings.Current.SyncQuantityAndWeightForOrders;
                LoadComboboxes();

                using (var ta = new FieldsTableAdapter())
                {
                    ta.FillByIsSystem(dsApplicationSettings.Fields, true);
                }

                this.grdFields.Rows.ExpandAll(true);

                // Serial number editor type
                var selectedSerialItem = this.cboSerialEditor.FindItemByValue<SerialNumberType>(v => v == ApplicationSettings.Current.SerialNumberEditorType);
                if (selectedSerialItem != null)
                {
                    cboSerialEditor.SelectedItem = selectedSerialItem;
                }

                // Product class editor type
                var selectedProductClassItem = cboProductClassEditor.FindItemByValue<ProductClassType>(v => v == ApplicationSettings.Current.ProductClassEditorType);
                if (selectedProductClassItem != null)
                {
                    cboProductClassEditor.SelectedItem = selectedProductClassItem;
                }
            }
            catch (Exception exc)
            {
                const string errorMsg = "Error loading settings.";
                _log.Error(exc, errorMsg);
            }
        }

        private void LoadComboboxes()
        {
            cboSerialEditor.Items.Clear();

            var defaultItem = cboSerialEditor.Items.Add(SerialNumberType.Basic, SerialNumberType.Basic.ToString());
            cboSerialEditor.Items.Add(SerialNumberType.Advanced, SerialNumberType.Advanced.ToString());

            cboSerialEditor.SelectedItem = defaultItem;

            cboProductClassEditor.Items.Clear();
            var defaultProductClassItem = cboProductClassEditor.Items.Add(ProductClassType.Textbox, ProductClassType.Textbox.ToString());
            cboProductClassEditor.Items.Add(ProductClassType.Combobox, ProductClassType.Combobox.ToString());
            cboProductClassEditor.SelectedItem = defaultProductClassItem;
        }

        public void SaveData()
        {
            grdFields.UpdateData();

            using (var ta = new FieldsTableAdapter())
            {
                ta.Update(dsApplicationSettings.Fields);
            }

            ApplicationSettings.Current.SyncQuantityAndWeightForOrders = chkSyncWeight.Checked;

            ApplicationSettings.Current.SerialNumberEditorType = cboSerialEditor.SelectedItem == null ?
                SerialNumberType.Basic :
                cboSerialEditor.SelectedItem.DataValue as SerialNumberType? ?? SerialNumberType.Basic;

            ApplicationSettings.Current.ProductClassEditorType = cboProductClassEditor.SelectedItem == null ?
                ProductClassType.Textbox :
                cboProductClassEditor.SelectedItem.DataValue as ProductClassType? ?? ProductClassType.Textbox;
        }

        #endregion ISettingsPanel Members

        #region Events

        private bool _isInCellUpdate = false;

        private void grdFields_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if(_isInCellUpdate)
                return;

            _isInCellUpdate = true;

            if(e.Cell.IsDataCell)
            {
                if(e.Cell.Column.Key == "IsVisible")
                {
                    //if not visible
                    if(e.Cell.Value == null || !Convert.ToBoolean(e.Cell.Value))
                    {
                        //then uncheck required
                        e.Cell.Row.Cells["IsRequired"].Value = false;
                    }
                }

                if (e.Cell.Column.Key == "IsRequired")
                {
                    //if required
                    if (e.Cell.Value != null && Convert.ToBoolean(e.Cell.Value))
                    {
                        //then check visible
                        e.Cell.Row.Cells["IsVisible"].Value = true;
                    }
                }
            }

            _isInCellUpdate = false;
        }

        private void grdFields_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                grdFields.AfterColPosChanged -= grdFields_AfterColPosChanged;
                grdFields.AfterSortChange -= grdFields_AfterSortChange;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(grdFields.DisplayLayout.Bands[0]);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error initializing grid layout.");
            }
            finally
            {
                grdFields.AfterColPosChanged += grdFields_AfterColPosChanged;
                grdFields.AfterSortChange += grdFields_AfterSortChange;
            }
        }

        private void grdFields_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                if (_nonRequiredFields.Contains(e.Row.Cells["Name"].Value?.ToString()))
                {
                    // Disable IsRequired option.
                    e.Row.Cells["IsRequired"].Activation = Activation.Disabled;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error initializing row.");
            }
        }

        private void grdFields_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdFields.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling column position change.");
            }
        }

        private void grdFields_AfterSortChange(object sender, BandEventArgs e)
        {
            try
            {
                // Save settings - this event gets called after enabling/disabling group by.
                // AfterColPosChanged gets fired too, but the grid's settings have not been updated
                // to include the change of sort.
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdFields.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling sort change.");
            }
        }

        #endregion
    }
}
