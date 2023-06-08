using System;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using NLog;

namespace DWOS.UI.Utilities
{
    public partial class MultiLineForm : Form
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private string _layoutName;
        private PersistWindowState _pws;

        public MultiLineForm() { InitializeComponent(); }

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            //force save location
            if(this._pws != null)
                this._pws.SaveFormLocation();
        }

        private void MultiLineForm_Shown(object sender, EventArgs e)
        {
            try
            {
                if(this.dropDown.Selected.Rows.Count == 0 && this.dropDown.Rows.Count > 0)
                    this.dropDown.Rows[0].Selected = true;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error auto selecting first row.";
                _log.Error(exc, errorMsg);
            }
        }

        private void dropDown_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if(e.Row != null && e.Row.IsDataRow)
                this.btnOK.PerformClick();
        }

        #endregion

        /// <summary>
        ///     Gets or sets the name of the layout. If set, then form will save layout after each close.
        /// </summary>
        /// <value> The name of the layout. </value>
        public string LayoutName
        {
            get { return this._layoutName; }
            set
            {
                this._layoutName = value;

                if(!string.IsNullOrEmpty(this._layoutName))
                {
                    if(this._pws == null)
                    {
                        this._pws = new PersistWindowState();
                        this._pws.ParentForm = this;
                        this._pws.FileNamePrefix = this._layoutName;
                    }
                }
            }
        }
    }
}