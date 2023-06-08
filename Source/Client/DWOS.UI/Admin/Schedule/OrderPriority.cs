using System;
using System.Linq;
using System.Windows; 
using System.Windows.Forms;
using DWOS.Data.Datasets.ScheduleDatasetTableAdapters;
using DWOS.Shared;
using Infragistics.Win.UltraWinGrid;

namespace DWOS.UI.Admin.Schedule
{
    public partial class OrderPriority: Form
    {
        #region Fields
        private UltraGridRow _dragRow;

        private bool _rowIsDragging;
        #endregion

        public OrderPriority()
        {
            this.InitializeComponent();
        }

        public int SelecteOrderID { get; set; }

        private void LoadData()
        {
            using (var taPriority = new d_PriorityTableAdapter())
            {
                taPriority.Fill(dsSchedule.d_Priority);
            }

            using(var ta = new OrderPriorityTableAdapter())
            {
                ta.Fill(this.dsSchedule.OrderPriority);
            }

            using(var ta = new CustomerSummaryTableAdapter())
            {
                ta.Fill(this.dsSchedule.CustomerSummary);
            }
        }

        private bool SaveData()
        {
            try
            {
                using(var ta = new OrderPriorityTableAdapter())
                {
                    ta.Update(this.dsSchedule.OrderPriority);
                }

                return true;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving data.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
                return false;
            }
        }

        private void OrderPriority_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadData();

                
                if (this.SelecteOrderID > 0)
                {
                    UltraGridRow selectedRow = this.grdOrderPriority.Rows.FirstOrDefault(r => r.IsDataRow && Convert.ToInt32(r.Cells["OrderID"].Value) == this.SelecteOrderID);

                    if(selectedRow != null)
                    {
                        this.grdOrderPriority.Selected.Rows.Add(selectedRow);
                        this.grdOrderPriority.ActiveRow = selectedRow;
                        this.grdOrderPriority.ActiveRowScrollRegion.ScrollRowIntoView(selectedRow);
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error loading order priority.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(this.SaveData())
                Close();
            else
                DialogResult = DialogResult.Cancel;
        }

        private void grdOrderPriority_SelectionDrag(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {

                if (!_rowIsDragging)
                {
                    _dragRow = ((UltraGrid)sender).Selected.Rows[0];
                    _rowIsDragging = true;
                }


            }
            catch (Exception)
            {

                throw;
            }
        }


        private void grdOrderPriority_MouseUp(object sender, MouseEventArgs e)
        {
            if (_rowIsDragging)
            {
                _rowIsDragging = false;
                var uieOver = this.grdOrderPriority.DisplayLayout.UIElement.ElementFromPoint(new System.Drawing.Point(e.X, e.Y));
                if (uieOver != null)
                {
                    UltraGridRow rowOver = uieOver.GetContext(typeof(UltraGridRow), true) as UltraGridRow;
                    grdOrderPriority.Rows.Move(_dragRow, rowOver.Index);
                }
                
                
            }
        }

        private void grdOrderPriority_MouseMove(object sender, MouseEventArgs e)
        {
            if (_rowIsDragging)
            {
                var uieOver = this.grdOrderPriority.DisplayLayout.UIElement.ElementFromPoint(new System.Drawing.Point(e.X, e.Y));
                UltraGridRow rowOver = uieOver.GetContext(typeof(UltraGridRow), true) as UltraGridRow;
                //rowOver.Appearance.BackColor = new 

            }
        }
    }
}