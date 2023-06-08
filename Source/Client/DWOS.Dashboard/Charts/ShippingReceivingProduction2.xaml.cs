using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using NLog;
using DWOS.Data;

namespace DWOS.Dashboard.Charts
{
    /// <summary>
    /// Interaction logic for ShippingReceivingProduction2.xaml
    /// </summary>
    /// <remarks>
    /// Changing the name of this class may break persistence of dashboard layouts.
    /// </remarks>
    public partial class ShippingReceivingProduction2 : UserControl, IDashboardWidget2
    {
        #region Fields

        private string _displayName;
        private BackgroundWorker _worker = null;
        private DispatcherTimer _timer;
        public event PropertyChangedEventHandler PropertyChanged;
        
        #endregion

        #region Properties

        private ShippingReceivingProductionSettings Settings { get; set; }

        #endregion

        #region Methods

        public ShippingReceivingProduction2()
        {
            InitializeComponent();

            this.Settings = new ShippingReceivingProductionSettings() { Date = DateTime.Now, CountByType = ShippingReceivingProductionSettings.CountBy.Parts, UseCurrentDateAlways = true };
            this.DisplayName = "Shipping & Receiving Production";

            settingsPanel.Visibility = System.Windows.Visibility.Collapsed;
            settingsPanel.DataContext = this.Settings;
            cboType.ItemsSource = Enum.GetValues(typeof(ShippingReceivingProductionSettings.CountBy)).Cast<ShippingReceivingProductionSettings.CountBy>();
        }

        public void Start()
        {
            try
            {
                this.DisplayName = this.Settings.CountByType == ShippingReceivingProductionSettings.CountBy.Orders ? "Shipping & Receiving Order Production" : "Shipping & Receiving Part Production";

                if(_timer == null)
                {
                    _timer = new DispatcherTimer(TimeSpan.FromSeconds(30), DispatcherPriority.Normal, (s, e) => Start(), System.Windows.Application.Current.Dispatcher) { IsEnabled = false };
                    Settings.PropertyChanged += Settings_PropertyChanged;
                }

                _timer.IsEnabled = false;


                if (_worker == null)
                {
                    _worker = new BackgroundWorker() { WorkerSupportsCancellation = true };
                    _worker.DoWork += (s, e) =>
                    {
                        e.Result = this.GetData();
                    };
                    
                    _worker.RunWorkerCompleted += (s, e) =>
                    {
                        if (e.Cancelled || e.Error != null)
                            return;

                        if (e.Result is Tuple<DataSummary, DataSummary>)
                        {
                            var results = e.Result as Tuple<DataSummary, DataSummary>;
                            var shipped = results.Item1;
                            var received = results.Item2;

                            txtDate.Text = this.Settings.Date.ToShortDateString();

                            if (Settings.CountByType == ShippingReceivingProductionSettings.CountBy.Parts)
                            {
                                var max = Convert.ToInt32(Math.Max(shipped.PartCount, received.PartCount) * 1.2) + 1;

                                guageShipping.Value = shipped.PartCount;
                                guageShipping.MaximumValue = max;
                                txtShipping.Text = "Parts Shipped: " + shipped.PartCount.ToString("N0");

                                guageRec.Value = received.PartCount;
                                guageRec.MaximumValue = max;
                                txtReceiving.Text = "Parts Received: " + received.PartCount.ToString("N0");
                            }
                            else
                            {
                                var max = Convert.ToInt32(Math.Max(shipped.OrderCount, received.OrderCount) * 1.2) + 1;

                                guageShipping.Value = shipped.OrderCount;
                                guageShipping.MaximumValue = max;
                                txtShipping.Text = "Orders Shipped: " + shipped.OrderCount.ToString("N0");

                                guageRec.Value = received.OrderCount;
                                guageRec.MaximumValue = max;
                                txtReceiving.Text = "Orders Received: " + received.OrderCount.ToString("N0");
                            }
                        }

                        _timer.IsEnabled = true;
                    };
                }

                if (_worker.IsBusy)
                    return;

                _worker.RunWorkerAsync();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading shipping/receiving production counts.");
            }
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.IsEnabled = false;
                _timer = null;
            }

            if (_worker != null)
            {
                if (_worker.IsBusy)
                    _worker.CancelAsync();

                _worker.Dispose();
                _worker = null;
            }
        }

        public void OnDepartmentsChanged()
        {
        }

        private Tuple<DataSummary, DataSummary> GetData()
        {
            var shipped = new DataSummary();
            var received = new DataSummary();

            var fromDate = new DateTime(this.Settings.Date.Year, this.Settings.Date.Month, this.Settings.Date.Day, 0, 0, 0);
            var toDate = new DateTime(this.Settings.Date.Year, this.Settings.Date.Month, this.Settings.Date.Day, 0, 0, 0).AddDays(1);

            //Load Shipping Data
            using (var ta = new DWOS.Data.Reports.ProcessPartsReportTableAdapters.ShippingProductionTableAdapter() { ClearBeforeFill = true })
            {
                var shippedOrders = new Data.Reports.ProcessPartsReport.ShippingProductionDataTable();
                ta.Fill(shippedOrders, fromDate, toDate);

                //if (this.Settings.IncludeOpenPackages)
                //    ta.FillByOpenInShipmentPackage(shippedOrders);

                foreach (var order in shippedOrders)
                {
                    if (!order.IsPartQuantityNull())
                    {
                        shipped.PartCount += order.PartQuantity;
                        shipped.OrderCount += 1;

                        if (!order.IsBasePriceNull() && !order.IsPriceUnitNull())
                        {
                            decimal fees = order.IsFeesNull() ? 0 : order.Fees;
                            decimal weight = order.IsWeightNull() ? 0M : order.Weight;
                            shipped.Price = OrderPrice.CalculatePrice(order.BasePrice, order.PriceUnit, fees, order.PartQuantity, weight);
                        }
                    }
                }
            }

            //Load Receiving Data
            using (var ta = new DWOS.Data.Reports.ProcessPartsReportTableAdapters.OrderCreationTableAdapter())
            {
                var ordersCreated = ta.GetData(fromDate, toDate);

                foreach (var order in ordersCreated)
                {
                    if (!order.IsPartQuantityNull())
                    {
                        received.PartCount += order.PartQuantity;
                        received.OrderCount += 1;

                        if (!order.IsBasePriceNull() && !order.IsPriceUnitNull())
                        {
                            decimal fees = order.IsFeesNull() ? 0 : order.Fees;
                            decimal weight = order.IsWeightNull() ? 0M : order.Weight;
                            received.Price = OrderPrice.CalculatePrice(order.BasePrice, order.PriceUnit, fees, order.PartQuantity, weight);
                        }
                    }
                }
            }

            return new Tuple <DataSummary, DataSummary>(shipped, received);
        }

        #endregion

        #region Events

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (settingsPanel.Visibility == System.Windows.Visibility.Collapsed)
            {
                settingsPanel.DataContext = this.Settings;
                settingsPanel.Visibility = System.Windows.Visibility.Visible;
                guageRec.Visibility = System.Windows.Visibility.Collapsed;
                guageShipping.Visibility = System.Windows.Visibility.Collapsed;
                txtReceiving.Visibility = System.Windows.Visibility.Collapsed;
                txtShipping.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                settingsPanel.Visibility = System.Windows.Visibility.Collapsed;
                guageRec.Visibility = System.Windows.Visibility.Visible;
                guageShipping.Visibility = System.Windows.Visibility.Visible;
                txtReceiving.Visibility = System.Windows.Visibility.Visible;
                txtShipping.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Start();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            guageRec.Margin = new Thickness(0, 0, -(this.ActualWidth * .8) / 2, 0);
            guageShipping.Margin = new Thickness(-(this.ActualWidth * .8) / 2, 0, 0, 0);
        }

        #endregion

        #region IDashboardWidget

        public string DisplayName
        {
            get { return this._displayName; }
            set
            {
                this._displayName = value;

                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("DisplayName"));
            }
        }

        public System.Windows.Controls.Control Control
        {
            get { return this; }
        }

        WidgetSettings IDashboardWidget2.Settings
        {
            get { return this.Settings; }
            set { this.Settings = value as ShippingReceivingProductionSettings; }
        }

        #endregion

        #region ShippingReceivingProductionSettings

        [DataContract]
        public class ShippingReceivingProductionSettings : WidgetSettings
        {
            private DateTime _date;
            private CountBy _countByType;
            private bool _useCurrentDateAlways;

            public enum CountBy { Orders, Parts }

            [DataMember]
            public CountBy CountByType
            {
                get { return this._countByType; }
                set
                {
                    this._countByType = value;
                    OnPropertyChanged("CountByType");
                }
            }

            [DataMember]
            public DateTime Date
            {
                get { return UseCurrentDateAlways ? DateTime.Now : _date; }
                set { _date = value; OnPropertyChanged("Date"); }
            }

            [DataMember]
            public bool UseCurrentDateAlways
            {
                get { return this._useCurrentDateAlways; }
                set { this._useCurrentDateAlways = value; OnPropertyChanged("UseCurrentDateAlways"); }
            }
        }

        #endregion

        #region DataSummary
        
        private class DataSummary
        {
            public int PartCount { get; set; }
            public int OrderCount { get; set; }
            public decimal Price { get; set; }
        }

        #endregion
    }
}
