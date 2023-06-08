using System;
using System.Windows.Forms;
using DWOS.Data.Datasets;

namespace DWOS.UI.Sales
{
    public partial class OrderRework: Form
    {
        public OrderRework()
        {
            this.InitializeComponent();
        }

        public string Notes
        {
            get { return this.txtReason.Text; }
        }

        public int ReasonCode
        {
            get { return cboReasonCode.SelectedItem == null ? 1 : Convert.ToInt32(cboReasonCode.SelectedItem.DataValue); }
        }

        public string OrderID
        {
            get { return this.txtOrderID.Text; }
            set { this.txtOrderID.Text = value; }
        }

        private void OrderRework_Load(object sender, System.EventArgs e)
        {
            using (var ta = new Data.Datasets.ListsDataSetTableAdapters.d_OrderChangeReasonTableAdapter())
            {
                var changeReasons = ta.GetDataByChangeType((int)OrderChangeType.ExtRework);
                
                cboReasonCode.DataSource = changeReasons.DefaultView;
                cboReasonCode.DisplayMember = changeReasons.NameColumn.ColumnName;
                cboReasonCode.ValueMember = changeReasons.OrderChangeReasonIDColumn.ColumnName;
            }
        }
    }
}