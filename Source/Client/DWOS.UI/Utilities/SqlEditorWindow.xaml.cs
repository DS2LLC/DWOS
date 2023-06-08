using DWOS.Data.Datasets;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using DWOS.Reports.Reports;
using System.Data;
using DWOS.Reports;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Interaction logic for SqlEditorWindow.xaml
    /// </summary>
    public partial class SqlEditorWindow : Window
    {
        #region Fields

        private DataTable _table;

        #endregion

        #region Methods

        public SqlEditorWindow()
        {
            InitializeComponent();

            // Load built-in scripts
            cboScript.ItemsSource = new List<ScriptItem>
            {
                new ScriptItem() { Name = "Deleted Orders", Sql = "SELECT * FROM [vOrder_Deleted]" },
                new ScriptItem() { Name = "Orders", Sql = "SELECT TOP 1000 *  FROM [Order]" }
            };
        }

        private void SelectScript()
        {
            try
            {
                if (cboScript.SelectedItem is ScriptItem selectedScript)
                {
                    txtSql.Text = selectedScript.Sql;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error catching select script.");
            }
        }

        private void SaveResults()
        {
            if (_table == null)
            {
                return;
            }

            // Create report columns
            var reportColumns = new List<ExcelReport.Column>();

            foreach (DataColumn column in _table.Columns)
            {
                reportColumns.Add(new ExcelReport.Column(column.ColumnName));
            }

            // Create report rows
            var reportRows = new List<ExcelReport.Row>();

            foreach (DataRow record in _table.Rows)
            {
                reportRows.Add(new ExcelReport.Row { Cells = record.ItemArray });
            }

            // Create report
            var report = new ExcelReport("Custom Report", new List<ExcelReport.ReportTable>
                {
                    new ExcelReport.ReportTable
                    {
                        Columns = reportColumns,
                        Name = "Custom Report",
                        Rows = reportRows,
                        IncludeCompanyHeader = false,
                        FormattingOptions = new ExcelReport.TableFormattingOptions
                        {
                            BorderAroundCells = true
                        }
                    }
                });

            report.DisplayReport();
        }

        #endregion

        #region Events

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtStatus.Text = "Executing Script...";
                _table = UserLogging.ExecuteSQL(txtSql.Text, out int recordsAffected);

                grdSqlOutput.DataSource = _table.DefaultView;

                txtStatus.Text = recordsAffected >= 0
                    ? $"{recordsAffected} Records Affected"
                    : $"Rows: {_table.Rows.Count}";
            }
            catch (Exception exc)
            {
                MessageBoxUtilities.ShowMessageBoxWarn(exc.Message, "SQL Error");

                LogLevel logLevel = exc is SqlException
                    ? LogLevel.Info
                    : LogLevel.Error;

                LogManager.GetCurrentClassLogger().Log(logLevel, exc, "Error executing SQL Script.");
                txtStatus.Text = "Script Error";
            }
        }

        private void cboScript_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SelectScript();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error selecting built-in script.");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveResults();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error saving SQL query results.");
            }
        }

        #endregion

        /// <summary>
        /// Represents a built-in SQL script.
        /// </summary>
        public class ScriptItem
        {
            /// <summary>
            /// Gets or sets the SQL code for this instance.
            /// </summary>
            public string Sql { get; set; }

            /// <summary>
            /// Gets the name for this instance.
            /// </summary>
            public string Name { get; set; }

            public override string ToString() => Name;
        }
    }
}
