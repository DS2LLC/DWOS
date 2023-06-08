using DWOS.Dashboard;
using DWOS.Dashboard.Charts;
using DWOS.Shared.Utilities;
using Infragistics.Controls.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using Newtonsoft.Json;
using System.IO;

namespace DWOS.UI.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardTab.xaml
    /// </summary>
    public partial class DashboardTab : System.Windows.Controls.UserControl, IDashboard
    {
        #region Fields

        public const string DATA_TYPE = "dashboard";
        public event EventHandler LayoutError;
        private const int MAX_ROWS = 3;

        #endregion

        #region Properties

        public string Key { get; set; }

        public System.Windows.Controls.UserControl TabControl
        {
            get { return this; }
        }

        public string TabName { get; set; }

        public string DataType
        {
            get { return DATA_TYPE; }
        }

        #endregion

        #region Methods

        public DashboardTab()
        {
            InitializeComponent();

            this.Key        = Guid.NewGuid().ToString();
            this.TabName    = "Dashboard";

            tileManager.NormalModeSettings = new NormalModeSettings();
            tileManager.NormalModeSettings.TileLayoutOrder = TileLayoutOrder.UseExplicitRowColumnOnTile;
            tileManager.NormalModeSettings.ExplicitLayoutTileSizeBehavior = ExplicitLayoutTileSizeBehavior.SynchronizeTileWidthsAcrossRows;
            tileManager.InterTileSpacingX = 3;
            tileManager.InterTileSpacingY = 3;
        }

        private void AddWidget(IDashboardWidget2 widget, double? width = null, double? height = null)
        {
            try
            {
                var tile = new XamTile()
                {
                    Content = widget.Control,
                    Header = widget.DisplayName,
                    Tag = widget,
                    CloseAction = TileCloseAction.RemoveItem,
                    AllowMaximize = false,
                    MaximizeButtonVisibility = Visibility.Collapsed
                };

                //find best position for new tile            
                var nextCol = widget.Settings.TileColumn;
                var nextRow = widget.Settings.TileRow;

                //if no defined position for tile then get next slot
                if (nextCol < 0 || nextRow < 0)
                {
                    var tiles = tileManager.Items.OfType<XamTile>().ToList();
                    int columnIndex = 0;
                    bool foundSlot = false;

                    //find first slot not filled
                    while (!foundSlot)
                    {
                        var colTiles = tiles.Where(t => Convert.ToInt32(t.GetValue(XamTileManager.ColumnProperty)) == columnIndex).ToList();

                        for(int rowIndex = 0; rowIndex < MAX_ROWS; rowIndex++)
                        {
                            if(!colTiles.Exists(t => Convert.ToInt32(t.GetValue(XamTileManager.RowProperty)) == rowIndex))
                            {
                                nextCol = columnIndex;
                                nextRow = rowIndex;
                                foundSlot = true;
                            }
                        }

                        columnIndex++;
                    }
                }

                tile.SetValue(XamTileManager.RowProperty, nextRow);
                tile.SetValue(XamTileManager.ColumnProperty, nextCol);

                tileManager.Items.Add(tile);

                //restore the tile size
                if(width.GetValueOrDefault() > 0 && height.GetValueOrDefault() > 0)
                {
                    //use constraints to set the size of the tile http://www.infragistics.com/community/forums/t/70836.aspx
                    var constraints = new TileConstraints {PreferredHeight = height.GetValueOrDefault(), PreferredWidth = width.GetValueOrDefault()};
                    tile.SetValue(XamTileManager.ConstraintsProperty, constraints);
                }

                widget.PropertyChanged += widget_PropertyChanged;
                widget.Start();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error adding widget.");
            }
        }

        public void AddWidget(string key)
        {
            IDashboardWidget2 widget = null;

            switch(key)
            {
                case "LateByDept":
                    widget = new LateByLocation2();
                    break;
                case "OrderCount":
                    widget = new OrdersByLocation2();
                    break;
                case "ShipRecProduction":
                    widget = new ShippingReceivingProduction2();
                    break;
                case "QAInspections":
                    widget = new QAByLocation2();
                    break;
                case "DeptProduction":
                    widget = new DWOS.Dashboard.Production.DeptProdHistory();
                    break;
                case "OrderPriority":
                    widget = new OrdersByPriority();
                    break;
                case "OrdersDue":
                    widget = new OrdersDue();
                    break;
                case "ReworkReasons":
                    widget = new ReworkReasons();
                    break;
                case "PriorityHistory":
                    widget = new PriorityHistory();
                    break;
                case "WIPHistory":
                    widget = new WIPHistory();
                    break;
            }

            if(widget != null)
                AddWidget(widget);
        }

        public void SaveLayout()
        {
            try
            {
                if (string.IsNullOrEmpty(this.Key))
                    this.Key = Guid.NewGuid().ToString();

                var filePath = GetLayoutPath();

                var layout = new DashboardLayout();
                layout.SaveWidgets(tileManager);

                FileSystem.SerializeJson(filePath, layout);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error saving layouts.");
            }
        }

        public void SaveLayout(string filePath)
        {
            try
            {
                var layout = new DashboardLayout();
                layout.SaveWidgets(tileManager);

                FileSystem.SerializeJson(filePath, layout);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error saving layout to file.");
            }
        }

        public void LoadLayout()
        {
            try
            {
                if (string.IsNullOrEmpty(this.Key))
                    return;

                var filePath = GetLayoutPath();

                if (System.IO.File.Exists(filePath))
                {
                    LoadLayout(File.ReadAllText(filePath));
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading dashboard layout.");
            }
        }

        public void LoadLayout(string content)
        {
            try
            {
                var layout = JsonConvert.DeserializeObject<DashboardLayout>(content, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                layout?.LoadWidgets(this);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading dashboard layout.");
                LayoutError?.Invoke(this, EventArgs.Empty);
                throw;
            }
        }

        public void Initialize(WipData data)
        {
            // Each dashboard does its own initialization
        }

        public void RefreshData(WipData data)
        {
            // Each dashboard widget does its own refresh
        }

        public void LoadDashboardFile(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    tileManager.Items.Clear();
                    LoadLayout(File.ReadAllText(filePath));
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading dashboard layout.");
            }
        }

        private string GetLayoutPath()
        {
            if (string.IsNullOrEmpty(this.Key))
                return null;

            return FileSystem.UserAppDataPath() + "\\" + this.Key + "_v1.dat";
        }

        private XamTile FindTile(IDashboardWidget2 widget)
        {
            if (widget == null)
                return null;

            foreach (var tile in tileManager.Items.OfType<XamTile>())
            {
                if (tile != null && tile.Tag == widget)
                {
                    return tile;
                }
            }

            return null;
        }

        public void OnDepartmentsChanged()
        {
            foreach (var tile in tileManager.Items.OfType<XamTile>())
            {
                var widget = tile?.Tag as IDashboardWidget2;
                if (widget != null)
                {
                    widget.OnDepartmentsChanged();
                }
            }
        }

        public void OnClose()
        {
            bool hasCaughtException = false;

            foreach (var tile in tileManager.Items.OfType<XamTile>())
            {
                try
                {
                    TileCleanup(tile);
                }
                catch (Exception exc)
                {
                    const string errorMsg = "Error closing tab.";

                    // Log an error for the first exception.
                    if (hasCaughtException)
                    {
                        NLog.LogManager.GetCurrentClassLogger().Warn(exc, errorMsg);
                    }
                    else
                    {
                        NLog.LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
                        hasCaughtException = true;
                    }
                }
            }
        }

        private void TileCleanup(XamTile tile)
        {
            var widget = tile.Tag as IDashboardWidget2;

            if (widget != null)
            {
                widget.PropertyChanged -= widget_PropertyChanged;
                widget.Stop();
            }
        }

        public DwosTabData Export()
        {
            var layout = new DashboardLayout();
            layout.SaveWidgets(tileManager);

            var layoutJson = JsonConvert.SerializeObject(layout, Formatting.Indented,
                new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto});

            return new DwosTabData
            {
                Name = TabName,
                DataType = DataType,
                Key = Key,
                Layout = layoutJson
            };
        }

        #endregion

        #region Events

        private void widget_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var widget  = sender as IDashboardWidget2;
            var tile    = FindTile(widget);

            if(tile != null && widget != null)
            {
                if(e.PropertyName == "DisplayName")
                    tile.Header = widget.DisplayName;
                if(e.PropertyName == "Row")
                    tile.SetValue(XamTileManager.RowProperty, widget.Settings.TileRow);
                if (e.PropertyName == "Column")
                    tile.SetValue(XamTileManager.ColumnProperty, widget.Settings.TileColumn);
            }
        }

        private void TileManager_TileClosed(object sender, TileClosedEventArgs e)
        {
            try
            {
                TileCleanup(e.Tile);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn(exc, "Error closing dashboard tile.");
            }
        }

        #endregion

        #region DashboardWidgetInfo
        
        [DataContract]
        [KnownType(typeof(LateByLocation2.LateByLocationSettings))]
        [KnownType(typeof(OrdersByLocation2.OrdersByLocationSettings))]
        [KnownType(typeof(QAByLocation2.QAByLocationSettings))]
        [KnownType(typeof(ShippingReceivingProduction2.ShippingReceivingProductionSettings))]
        public class DashboardWidgetInfo
        {
            #region Properties

            [DataMember]
            public string WidgetType { get; set; }
            
            [DataMember]
            public string WidgetAssembly { get; set; }
            
            [DataMember]
            public WidgetSettings Settings { get; set; }

            [DataMember]
            public double TileWidth { get; set; }

            [DataMember]
            public double TileHeight { get; set; }

            #endregion

            #region Methods

            public DashboardWidgetInfo()
            {
                
            }

            public DashboardWidgetInfo(XamTile tile)
            {
                var widget      = tile.Tag as IDashboardWidget2;
                var widgetType  = widget.GetType();

                this.WidgetType     = widgetType.FullName;
                this.WidgetAssembly = widgetType.Assembly.GetName().Name;
                this.Settings       = widget.Settings;

                //update setting from tile before saving
                this.TileWidth = tile.ActualWidth;
                this.TileHeight = tile.ActualHeight;
                this.Settings.TileRow = Convert.ToInt32(tile.GetValue(XamTileManager.RowProperty) ?? 0);
                this.Settings.TileColumn = Convert.ToInt32(tile.GetValue(XamTileManager.ColumnProperty) ?? 0);
            }

            public IDashboardWidget2 CreateWidget()
            {
                IDashboardWidget2 widget = null;

                if (!String.IsNullOrEmpty(this.WidgetType) && !String.IsNullOrEmpty(this.WidgetAssembly))
                {
                    widget = Activator.CreateInstance(this.WidgetAssembly, this.WidgetType).Unwrap() as IDashboardWidget2;

                    if(widget != null && this.Settings != null)
                        widget.Settings = this.Settings;
                }

                return widget;
            }

            #endregion
        }

        [DataContract]
        public class DashboardLayout
        {
            [DataMember]
            public List<DashboardWidgetInfo> Widgets { get; set; }

            public DashboardLayout()
            {
                this.Widgets = new List <DashboardWidgetInfo>();
            }

            public void SaveWidgets(XamTileManager tileManager)
            {
                foreach (var tile in tileManager.Items.OfType<XamTile>())
                {
                    if (tile != null && tile.Tag is IDashboardWidget2)
                    {                        
                        this.Widgets.Add(new DashboardWidgetInfo(tile));
                    }
                }
            }

            public void LoadWidgets(DashboardTab dashboard)
            {
                foreach(var dashboardWidgetInfo in this.Widgets)
                {
                    var widget = dashboardWidgetInfo.CreateWidget();

                    if(widget != null)
                    {
                        if(dashboardWidgetInfo.Settings != null)
                            widget.Settings = dashboardWidgetInfo.Settings;

                        dashboard.AddWidget(widget, dashboardWidgetInfo.TileWidth, dashboardWidgetInfo.TileHeight);
                    }
                }
            }
        }

        #endregion
    }
}
