using System;
using System.Windows.Input;
using DWOS.Data.Datasets.SecurityDataSetTableAdapters;
using DWOS.UI.Utilities;
using Infragistics.Windows.Controls;
using Infragistics.Windows.DataPresenter;
using NLog;

namespace DWOS.UI
{
    public class CustomerFilterCommand : ICommand
    {
        private int _lastUserId;

        public CustomerFilterCommand()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter) { return true; }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            try
            {
                var recordFilter = parameter as RecordFilter;

                if (recordFilter == null || _lastUserId == SecurityManager.Current.UserID)
                {
                    return;
                }

                recordFilter.Conditions.Clear();

                if (SecurityManager.Current.IsValidUser)
                {
                    var group = new ConditionGroup();
                    group.LogicalOperator = LogicalOperator.Or;

                    using (var ta = new CustomerSummaryTableAdapter())
                    {
                        var customers = ta.GetDataByUser(SecurityManager.Current.UserID);

                        foreach (var customer in customers)
                            group.Add(new ComparisonCondition(ComparisonOperator.Equals, customer.Name));
                    }

                    recordFilter.Conditions.Add(group);
                    _lastUserId = SecurityManager.Current.UserID;
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating customer filter.");
            }
        }
    }
}