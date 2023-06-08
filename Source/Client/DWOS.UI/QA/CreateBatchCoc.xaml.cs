using DWOS.Data;
using DWOS.Reports;
using DWOS.UI.Reports;
using DWOS.UI.Utilities;
using Infragistics.Documents.RichText;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace DWOS.UI.QA
{
    /// <summary>
    /// Interaction logic for CreateBatchCoc.xaml
    /// </summary>
    public partial class CreateBatchCoc : ICreateBatchCocView
    {
        #region Fields

        private readonly PersistWpfWindowState _persistState;
        private readonly GridSettingsPersistence<CreateBatchCocSettings> _settingsPersistence;

        #endregion

        #region Methods

        public CreateBatchCoc()
        {
            _persistState = new PersistWpfWindowState(this);
            _settingsPersistence = new GridSettingsPersistence<CreateBatchCocSettings>(
                "CreateBatchCoc",
                new CreateBatchCocSettings());

            InitializeComponent();
            Icon = Properties.Resources.Certificate_32.ToWpfImage();
        }

        public void LoadNew(int batchId, int customerId, List<int> orderIds)
        {
            ViewModel.LoadNew(batchId, customerId, orderIds);
        }

        #endregion

        #region Events

        private void XamRibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.LoadView(this);
                ViewModel.Completed += ViewModel_Completed;
                SecurityManager.Current.UserUpdated += Current_UserUpdated;

                // Load print/view settings
                var appSettings = ApplicationSettings.Current;
                ViewModel.ViewCoc = appSettings.DefaultCOCPrintSetting == ReportPrintSetting.Pdf;
                ViewModel.PrintCoc = appSettings.DefaultCOCPrintSetting == ReportPrintSetting.Printer;
                ViewModel.PrintCopies = Properties.Settings.Default.PrintQuantity;

                // Load grid/display settings
                var controlSettings = _settingsPersistence.LoadSettings();

                if (controlSettings != null)
                {
                    controlSettings.Order?.ApplyTo(OrdersDataGrid);

                    if (controlSettings.OrderAreaHeight != default(double))
                    {
                        OrdersRow.Height = new GridLength(controlSettings.OrderAreaHeight, GridUnitType.Star);
                    }

                    if (controlSettings.CertificateAreaHeight != default(double))
                    {
                        CertificateRow.Height = new GridLength(controlSettings.CertificateAreaHeight, GridUnitType.Star);
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error loading batch COC window.");
            }
        }

        private void XamRibbonWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.UnloadView();
                ViewModel.Completed -= ViewModel_Completed;
                SecurityManager.Current.UserUpdated -= Current_UserUpdated;

                // Save grid/display settings
                var orderGridSettings = new XamDataGridSettings();
                orderGridSettings.RetrieveSettingsFrom(OrdersDataGrid);

                _settingsPersistence.SaveSettings(new CreateBatchCocSettings
                {
                    Order = orderGridSettings,
                    OrderAreaHeight = OrdersRow.Height.Value,
                    CertificateAreaHeight = CertificateRow.Height.Value
                });
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error unloading batch COC window.");
            }
        }

        private void Current_UserUpdated(object sender, UserUpdatedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling user updated event.");
            }
        }

        private void ViewModel_Completed(object sender, EventArgs e)
        {
            try
            {
                if (ViewModel.PrintCoc || ViewModel.ViewCoc)
                {
                    var report = new BatchCocReport(ViewModel.ToModel());

                    if (ViewModel.PrintCoc)
                    {
                        report.PrintReport(ViewModel.PrintCopies);
                    }

                    if (ViewModel.ViewCoc)
                    {
                        report.DisplayReport();
                    }
                }

                DialogResult = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error on completed event");
            }
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Sync content
                ViewModel.SyncWithView();

                // Preview COC
                using (var report = new BatchCocReport(ViewModel.ToModel()))
                {
                    report.DisplayReport();
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error previewing batch COC.");
            }
        }

        #endregion

        #region ICreateBatchCoc Members

        public string InfoHtml
        {
            get
            {
                using (var memStream = new MemoryStream())
                {
                    CertTextEditor.Document.SaveToHtml(memStream);
                    memStream.Flush();
                    memStream.Position = 0;

                    using (var reader = new StreamReader(memStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            set
            {
                using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(value ?? string.Empty)))
                {
                    CertTextEditor.Document.LoadFromHtml(memStream);
                }
            }
        }

        #endregion

        #region CreateBatchCocSettings

        public class CreateBatchCocSettings
        {
            public XamDataGridSettings Order { get; set; }

            public double OrderAreaHeight { get; set; }

            public double CertificateAreaHeight { get; set; }
        }

        #endregion
    }
}
