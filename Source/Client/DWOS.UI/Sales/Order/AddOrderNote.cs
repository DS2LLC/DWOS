using System;
using System.Windows.Forms;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Utilities.Validation;
using NLog;

namespace DWOS.UI.Sales.Order
{
    public partial class AddOrderNote : Form
    {
        private ValidatorManager _manager;

        public AddOrderNote() { InitializeComponent(); }

        public void LoadData(int orderId)
        {
            try
            {
                this._manager = new ValidatorManager();

                using(var ta = new UserSummaryTableAdapter())
                    ta.Fill(this.dsOrders.UserSummary);

                this.dsOrders.EnforceConstraints = false;
                this.orderNoteInfo1.LoadData(this.dsOrders);
                this.orderNoteInfo1.AddValidators(this._manager, this.errProvider);

                var note = this.orderNoteInfo1.AddNote(orderId);
                this.orderNoteInfo1.MoveToRecord(note.OrderNoteID);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading add note dialog.");
            }
        }

        private void SaveData()
        {
            try
            {
                this.orderNoteInfo1.EndEditing();

                using(var ta = new OrderNoteTableAdapter())
                    ta.Update(this.dsOrders.OrderNote);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error saving note.");
            }
        }

        private void DisposeMe()
        {
            try
            {
                if(this._manager != null)
                    this._manager.Dispose();

                this._manager = null;
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error disposing.");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(this._manager.ValidateControls())
            {
                SaveData();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}