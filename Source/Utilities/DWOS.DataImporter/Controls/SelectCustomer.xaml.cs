using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DWOS.DataImporter.Controls
{
    /// <summary>
    /// Interaction logic for SelectCustomer.xaml
    /// </summary>
    public partial class SelectCustomer : Window
    {
        public enum ResultStatus { OK, NONE, ABORT }

        public DataRow SelectedRow 
        {
            get
            {
                if(cboTargetCustomer.SelectedItem is DataRowView)
                    return ((DataRowView)cboTargetCustomer.SelectedItem).Row;

                return null;
            }
        }

        public ResultStatus Status { get; set; }

        public SelectCustomer()
        {
            InitializeComponent();
        }

        public void LoadData(string sourceCustomerName, DataTable customerTable)
        {
            txtSourceCustomer.Text = sourceCustomerName;

            cboTargetCustomer.DisplayMemberPath = "Name";
            cboTargetCustomer.SelectedValuePath = "CustomerID";
            var view = customerTable.DefaultView;
            view.Sort = "Name";

            cboTargetCustomer.ItemsSource = view;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Status = ResultStatus.OK;
            this.Close();
        }

        private void btnNone_Click(object sender, RoutedEventArgs e)
        {
            Status = ResultStatus.NONE;
            this.Close();
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            Status = ResultStatus.ABORT;
            this.Close();
        }

    }
}
