using System;
using System.Windows.Forms;
using DWOS.Data.Datasets;

namespace DWOS.UI.Sales.Order
{
    public partial class OrderReviewSummaryPanel : DataPanel
    {
        #region Properties

        public OrdersDataSet Dataset
        {
            get { return base._dataset as OrdersDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.OrderReview.OrderReviewIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public OrderReviewSummaryPanel() { InitializeComponent(); }

        public void LoadData(OrdersDataSet dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.OrderReview.TableName;

            this.cboStatus.Items.Clear();
            this.cboStatus.Items.Add(false, "Fail");
            this.cboStatus.Items.Add(true, "Pass");

            base.BindValue(this.cboReviewUser, Dataset.OrderReview.ReviewedByColumn.ColumnName);
            base.BindValue(this.cboStatus, Dataset.OrderReview.StatusColumn.ColumnName);
            base.BindValue(this.txtNotes, Dataset.OrderReview.NotesColumn.ColumnName);

            base.BindList(this.cboReviewUser, Dataset.UserSummary, Dataset.UserSummary.UserIDColumn.ColumnName, Dataset.UserSummary.NameColumn.ColumnName);

            base._panelLoaded = true;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            var row = CurrentRecord as OrdersDataSet.OrderReviewRow;
            grpData.Text = row.OrderReviewTypeRow.Name;
        }

        #endregion

        private void OrderReviewSummaryPanel_Resize(object sender, EventArgs e)
        {
            if((grpData.Width > Width) || (grpData.Height > Height))
                grpData.Dock = DockStyle.None;
            else
                grpData.Dock = DockStyle.Fill;
        }
    }
}