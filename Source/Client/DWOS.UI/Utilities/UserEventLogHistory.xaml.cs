using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.SecurityDataSetTableAdapters;
using DWOS.Utilities;
using NLog;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Interaction logic for UserProfileDialog.xaml
    /// </summary>
    public partial class UserEventLogHistory : Window
    {
        #region Fields
        
        private Data.Datasets.UserLogging _dataSet = null;
        private ObservableCollection<UserEventLogInfo> EventLogs { get; set; }

        #endregion

        #region Methods

        public UserEventLogHistory()
        {
            InitializeComponent();
        }

        public void LoadData()
        {
            try
            {
                _dataSet = new Data.Datasets.UserLogging() { EnforceConstraints = false };

                using (var ta = new Data.Datasets.UserLoggingTableAdapters.UserEventLogHistoryTableAdapter())
                    ta.FillFromDate(_dataSet.UserEventLogHistory, DateTime.Now.AddDays(-90));

                EventLogs = new ObservableCollection <UserEventLogInfo>();

                foreach (var eventLog in _dataSet.UserEventLogHistory)
                {
                    EventLogs.Add(new UserEventLogInfo() {Date = eventLog.Created, Location = eventLog.Form, LogEventId = eventLog.LogEventID, Operation = eventLog.Operation, Reason = eventLog.Reason, UserName = eventLog.Name, Details = eventLog.TransactionDetails.TrimEnd('|')});
                }

                grdUserEvents.DataSource = EventLogs;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading the user event log dialog.");
            }
        }

        private void DisposeMe()
        {
            EventLogs = null;

            if(_dataSet != null)
                _dataSet.Dispose();

            _dataSet = null;
        }

        #endregion

        #region Events
        
        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { DisposeMe(); }
        
        #endregion

        public class UserEventLogInfo
        {
            public int LogEventId { get; set; }
            public string UserName { get; set; }
            public string Operation { get; set; }
            public string Location { get; set; }
            public string Reason { get; set; }
            public string Details { get; set; }
            public DateTime Date { get; set; }
        }
    }
}
