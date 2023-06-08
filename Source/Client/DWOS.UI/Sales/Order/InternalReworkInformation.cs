using System;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ListsDataSetTableAdapters;

namespace DWOS.UI.Sales
{
    public partial class InternalReworkInformation : DataPanel
    {
        #region Fields

        public event Action <int> GoToOrder;

        #endregion

        #region Properties

        public OrdersDataSet Dataset
        {
            get { return base._dataset as OrdersDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.InternalRework.InternalReworkIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public InternalReworkInformation() { InitializeComponent(); }

        public void LoadData(OrdersDataSet dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.InternalRework.TableName;

            base.BindValue(this.dtDateCertified, Dataset.InternalRework.DateCreatedColumn.ColumnName);
            base.BindValue(this.txtOriginalOrder, Dataset.InternalRework.OriginalOrderIDColumn.ColumnName);
            base.BindValue(this.txtReworkOrder, Dataset.InternalRework.ReworkOrderIDColumn.ColumnName);
            base.BindValue(this.txtReworkType, Dataset.InternalRework.ReworkTypeColumn.ColumnName);
            base.BindValue(this.txtHoldLocation, Dataset.InternalRework.HoldLocationIDColumn.ColumnName);
            base.BindValue(this.cboReason, Dataset.InternalRework.ReworkReasonIDColumn.ColumnName);
            base.BindValue(this.chkActive, Dataset.InternalRework.ActiveColumn.ColumnName);
            base.BindValue(this.txtNotes, Dataset.InternalRework.NotesColumn.ColumnName);

            base.BindValue(this.cboUserCreated, Dataset.InternalRework.CreatedByColumn.ColumnName);
            base.BindList(this.cboUserCreated, Dataset.UserSummary, Dataset.UserSummary.UserIDColumn.ColumnName, Dataset.UserSummary.NameColumn.ColumnName);

            using(var ta = new d_ReworkReasonTableAdapter())
            {
                var table = ta.GetData();
                this.cboReason.DisplayMember = table.NameColumn.ColumnName;
                this.cboReason.ValueMember = table.ReworkReasonIDColumn.ColumnName;
                this.cboReason.DataSource = table.DefaultView;
            }

            base._panelLoaded = true;
        }

        #endregion

        #region Events

        private void btnGoToOriginal_Click(object sender, EventArgs e)
        {
            if(GoToOrder != null)
            {
                var internalRework = CurrentRecord as OrdersDataSet.InternalReworkRow;
                if(internalRework != null)
                    GoToOrder(internalRework.OriginalOrderID);
            }
        }

        private void btnGoToRework_Click(object sender, EventArgs e)
        {
            if(GoToOrder != null)
            {
                var internalRework = CurrentRecord as OrdersDataSet.InternalReworkRow;
                if(internalRework != null && !internalRework.IsReworkOrderIDNull())
                    GoToOrder(internalRework.ReworkOrderID);
            }
        }

        private void pnlActive_MouseHover(object sender, EventArgs e)
        {
            try
            {
                // Show tooltip for disabled control
                if (chkActive.Enabled)
                {
                    return;
                }

                tipManager.ShowToolTip(chkActive);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error showing active tooltip.");
            }
        }

        private void pnlActive_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                // Hide tooltip for disabled control
                if (chkActive.Enabled || !tipManager.IsToolTipVisible(chkActive))
                {
                    return;
                }

                tipManager.HideToolTip();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error hiding active tooltip.");
            }
        }

        #endregion
    }
}