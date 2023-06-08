using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DWOS.Reports;
using DWOS.Reports.Reports;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class PartHistory : Form
    {
        #region Fields

        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("PartHistory", new UltraGridBandSettings());

        #endregion

        #region  Properties

        public int PartId { get; set; }

        public string PartName
        {
            get => txtPartName.Text;
            set => txtPartName.Text = value;
        }

        #endregion

        #region  Methods

        public PartHistory()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            taPartHistory.FillByPart(dsPartHistory.PartHistory, PartId);
        }

        private List<ExcelReport.ReportTable> CreateReportData()
        {
            var partName = txtPartName.Text;

            var table = new ExcelReport.ReportTable
            {
                Name = "Part History",
                Title = "Part History",

                FormattingOptions = new ExcelReport.TableFormattingOptions
                {
                    BorderAroundCells = true
                },

                Columns = new List<ExcelReport.Column>
                {
                    new ExcelReport.Column("Part") { Width = 30 },
                    new ExcelReport.Column("Category") { Width = 20 },
                    new ExcelReport.Column("Description") { Width = 80 },
                    new ExcelReport.Column("User Name") { Width = 20 },
                    new ExcelReport.Column("Machine") { Width = 20 },
                    new ExcelReport.Column("Date") { Width = 20 },
                },
                IncludeCompanyHeader = true,
                Rows = new List<ExcelReport.Row>()
            };

            foreach (var partHistoryRow in dsPartHistory.PartHistory)
            {
                table.Rows.Add(new ExcelReport.Row
                {
                    Cells = new List<ExcelReport.Cell>
                    {
                        new ExcelReport.Cell(partName),
                        new ExcelReport.Cell(partHistoryRow.Category),
                        new ExcelReport.Cell(partHistoryRow.Description),
                        new ExcelReport.Cell(partHistoryRow.UserName),
                        new ExcelReport.Cell(partHistoryRow.Machine),
                        new ExcelReport.Cell(partHistoryRow.DateCreated)
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                using (var report = new ExcelReport("Part History", CreateReportData()))
                {
                    report.DisplayReport();
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error printing order history report.");
            }
        }

        private void PartHistory_Load(object sender, EventArgs e)
        {
            LoadData();
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
                LogManager.GetCurrentClassLogger().Error(exc, "Error handling grid column sort change.");
            }
        }

        #endregion
    }
}