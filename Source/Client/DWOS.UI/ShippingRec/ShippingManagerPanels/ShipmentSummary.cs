using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using DWOS.Utilities.Validation;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinListView;

namespace DWOS.UI.ShippingRec.ShippingManagerPanels
{
    public partial class ShipmentSummary : DataPanel
    {
        #region Fields

        private readonly List<ContainerCountItem> _containerCountItems = new List<ContainerCountItem>();
        private readonly BindingList<SummaryItem> _summaryItems = new BindingList<SummaryItem>();
        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("ShipmentSummary_Main", new UltraGridBandSettings());

        #endregion

        #region Properties

        private OrderShipmentDataSet Dataset
        {
            get { return base._dataset as OrderShipmentDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.ShipmentPackage.ShipmentPackageIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public ShipmentSummary() { InitializeComponent(); }

        public void LoadData(OrderShipmentDataSet dataset)
        {
            Dataset = dataset;

            //bind shipment packages
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.ShipmentPackage.TableName;

            // Bind container counts
            grdContainerCounts.DataSource = _containerCountItems;

            // Bind summary
            grdLog.DataSource = _summaryItems;

            base._panelLoaded = true;
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider) { }

        public void AddShipmentLog(string customerName, string description, string userName, ShippingChange category)
        {
            try
            {
                _summaryItems.Add(new SummaryItem
                {
                    Customer = customerName,
                    Description = description,
                    Action = category.ToString(),
                    User = userName,
                    Time = DateTime.Now.ToShortTimeString()
                });
            }
            catch(Exception exc)
            {
                string errorMsg = "Error adding log to display.";
                _log.Error(exc, errorMsg);
            }
        }

        public void RefreshData()
        {
            try
            {
                _containerCountItems.Clear();

                // Get count for today's items
                foreach (var packageGroup in Dataset.ShipmentPackage.Where(s => s.IsValidState()).GroupBy(s => s.ShipmentPackageTypeID))
                {
                    var packageType = Dataset.ShipmentPackageType.FindByShipmentPackageTypeID(packageGroup.Key);

                    if (packageType == null)
                    {
                        continue;
                    }

                    _containerCountItems.Add(new ContainerCountItem
                    {
                        PackageType = packageType.Name,
                        Count = packageGroup.Count()
                    });
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error refreshing shipment data.");
            }
        }

        #endregion

        #region Events


        private void grdLog_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                grdLog.AfterColPosChanged -= grdLog_AfterColPosChanged;
                grdLog.AfterSortChange -= grdLog_AfterSortChange;

                var band = grdLog.DisplayLayout.Bands[0];

                // Load grid settings
                var gridSettings = _gridSettingsPersistence.LoadSettings();
                gridSettings.ApplyTo(band);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error loading grdLog");
            }
            finally
            {
                grdLog.AfterColPosChanged += grdLog_AfterColPosChanged;
                grdLog.AfterSortChange += grdLog_AfterSortChange;
            }
        }

        private void grdLog_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            try
            {
                var band = grdLog.DisplayLayout.Bands[0];
                var gridSettings = new UltraGridBandSettings();
                gridSettings.RetrieveSettingsFrom(band);

                _gridSettingsPersistence.SaveSettings(gridSettings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after showing/hiding/moving a column.");
            }
        }

        private void grdLog_AfterSortChange(object sender, BandEventArgs e)
        {
            try
            {
                var band = grdLog.DisplayLayout.Bands[0];
                var gridSettings = new UltraGridBandSettings();
                gridSettings.RetrieveSettingsFrom(band);

                _gridSettingsPersistence.SaveSettings(gridSettings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after showing/hiding/moving a column.");
            }
        }

        #endregion

        #region SummaryItem

        private class SummaryItem
        {
            public string Customer { get; set; }

            public string Description { get; set; }

            public string Action { get; set; }

            public string User { get; set; }

            public string Time { get; set; }
        }

        #endregion

        #region ContainerCountItem

        private class ContainerCountItem
        {
            public string PackageType { get; set; }

            public int Count { get; set; }
        }

        #endregion

        #region SummaryGridSettings

        public sealed class SummaryGridSettings
        {
            public IDictionary<string, ColumnSettings> Columns { get; } =
                new Dictionary<string, ColumnSettings>();

            public sealed class ColumnSettings
            {
                public int Order { get; set; }

                public int Width { get; set; }
            }
        }

        #endregion
    }
}