using DWOS.Reports;
using DWOS.Reports.Reports;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DWOS.UI.Sales
{
    public partial class OrderHistory : Form
    {
        #region Fields

        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("OrderHistory", new UltraGridBandSettings());

        private readonly List<HistoryItem> _history =
            new List<HistoryItem>();

        #endregion

        #region Properties

        public int OrderID { get; set; }


        #endregion

        #region Methods

        public OrderHistory()
        {
            this.InitializeComponent();
            grdValues.SetDataBinding(_history, null, true);
        }

        private void LoadData()
        {
            _history.Clear();

            // Receiving
            using (var taReceiving = new Data.Datasets.PartsDatasetTableAdapters.ReceivingSummaryTableAdapter())
            {
                using (var dtReceiving = taReceiving.GetByOrder(OrderID))
                {
                    var receivingRow = dtReceiving.FirstOrDefault();

                    if (receivingRow != null)
                    {
                        _history.Add(new HistoryItem
                        {
                            OrderId = OrderID,
                            Category = "Receiving",
                            Description = "Parts entered through receiving.",
                            UserName = "N/A",
                            Machine = "N/A",
                            DateCreated = receivingRow.CheckedIn
                        });
                    }
                }
            }

            // Actual order history
            taOrderHistory.FillByOrder(dsOrderHistory.OrderHistory, OrderID);

            foreach (var historyRow in dsOrderHistory.OrderHistory)
            {
                _history.Add(new HistoryItem
                {
                    OrderId = historyRow.OrderID,
                    Category = historyRow.Category,
                    Description = historyRow.Description,
                    UserName = historyRow.UserName,
                    Machine = historyRow.Machine,
                    DateCreated = historyRow.IsDateCreatedNull()
                        ? (DateTime?)null
                        : historyRow.DateCreated
                });
            }
        }

        private List<ExcelReport.ReportTable> CreateReportData()
        {
            var table = new ExcelReport.ReportTable
            {
                Name = "Order History",
                Title = "Order History",

                FormattingOptions = new ExcelReport.TableFormattingOptions
                {
                    BorderAroundCells = true
                },

                UseExcelTable = true,

                Columns = new List<ExcelReport.Column>
                {
                    new ExcelReport.Column("WO") { Width = 20 },
                    new ExcelReport.Column("Category") { Width = 20 },
                    new ExcelReport.Column("Description") { Width = 80 },
                    new ExcelReport.Column("User Name") { Width = 20 },
                    new ExcelReport.Column("Machine") { Width = 20 },
                    new ExcelReport.Column("Date") { Width = 20 },
                },
                IncludeCompanyHeader = true,
                Rows = new List<ExcelReport.Row>()
            };

            foreach (var orderHistoryItem in _history)
            {
                table.Rows.Add(new ExcelReport.Row
                {
                    Cells = new List<ExcelReport.Cell>
                    {
                        new ExcelReport.Cell(orderHistoryItem.OrderId),
                        new ExcelReport.Cell(orderHistoryItem.Category),
                        new ExcelReport.Cell(orderHistoryItem.Description),
                        new ExcelReport.Cell(orderHistoryItem.UserName),
                        new ExcelReport.Cell(orderHistoryItem.Machine),
                        new ExcelReport.Cell(orderHistoryItem.DateCreated)
                    }
                });
            }

            return new List<ExcelReport.ReportTable>
            {
                table
            };
        }

        #endregion

        #region Events

        private void OrderHistory_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadData();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error loading order history.");
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                using (var report = new ExcelReport("Order History", CreateReportData()))
                {
                    report.DisplayReport();
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error printing order history report.");
            }
        }

        private void grdValues_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                grdValues.AfterColPosChanged -= grdValues_AfterColPosChanged;
                grdValues.AfterSortChange -= grdValues_AfterSortChange;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(grdValues.DisplayLayout.Bands[0]);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing grid layout.");
            }
            finally
            {
                grdValues.AfterColPosChanged += grdValues_AfterColPosChanged;
                grdValues.AfterSortChange += grdValues_AfterSortChange;
            }
        }

        private void grdValues_AfterColPosChanged(object sender, Infragistics.Win.UltraWinGrid.AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdValues.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error handling grid column position change.");
            }
        }

        private void grdValues_AfterSortChange(object sender, Infragistics.Win.UltraWinGrid.BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdValues.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error handling grid column position change.");
            }
        }

        #endregion

        #region HistoryItem

        public class HistoryItem
        {
            public int OrderId { get; set; }

            public string Category { get; set; }

            public string Description { get; set; }

            public string UserName { get; set; }

            public string Machine { get; set; }

            public DateTime? DateCreated { get; set; }
        }

        #endregion
    }
}