using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;
using DWOS.Data.Utilities;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinListView;
using NLog;

namespace DWOS.UI.Sales.Customer
{
    public partial class CustomerPartMarking : DataPanel
    {
        #region Fields

        private Point _lastMouseDown = Point.Empty;

        #endregion

        #region Properties

        public CustomersDataset Dataset
        {
            get { return base._dataset as CustomersDataset; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.PartMarking.PartMarkingIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public CustomerPartMarking() { InitializeComponent(); }

        public void LoadData(CustomersDataset dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.PartMarking.TableName;

            //bind column to control
            base.BindValue(this.cboAirframe, Dataset.PartMarking.AirframeID_dColumn.ColumnName);
            base.BindValue(this.txtDef1, Dataset.PartMarking.Def1Column.ColumnName);
            base.BindValue(this.txtDef2, Dataset.PartMarking.Def2Column.ColumnName);
            base.BindValue(this.txtDef3, Dataset.PartMarking.Def3Column.ColumnName);
            base.BindValue(this.txtDef4, Dataset.PartMarking.Def4Column.ColumnName);
            base.BindValue(this.cboSpec, Dataset.PartMarking.ProcessSpecColumn.ColumnName);

            //bind lists
            base.BindList(this.cboAirframe, Dataset.d_Airframe, Dataset.d_Airframe.AirframeIDColumn.ColumnName, Dataset.d_Airframe.AirframeIDColumn.ColumnName);
            base.BindList(this.cboManufacturer, Dataset.d_Manufacturer, Dataset.d_Manufacturer.ManufacturerIDColumn.ColumnName, Dataset.d_Manufacturer.ManufacturerIDColumn.ColumnName);

            LoadCodes();

            base._panelLoaded = true;
        }

        private void LoadCodes()
        {
            this.lvwTemplates.Items.Clear();

            var date = new UltraListViewItem("<" + Interperter.enumInterperterCommands.DATE + ">", new object[] {Interperter.ParseCommand(Interperter.enumInterperterCommands.DATE)});
            date.Tag = Interperter.enumInterperterCommands.DATE;

            var date2 = new UltraListViewItem("<" + Interperter.enumInterperterCommands.DATE2 + ">", new object[] {Interperter.ParseCommand(Interperter.enumInterperterCommands.DATE2)});
            date2.Tag = Interperter.enumInterperterCommands.DATE2;

            var time = new UltraListViewItem("<" + Interperter.enumInterperterCommands.TIME + ">", new object[] {Interperter.ParseCommand(Interperter.enumInterperterCommands.TIME)});
            time.Tag = Interperter.enumInterperterCommands.TIME;

            var custWO = new UltraListViewItem("<" + Interperter.enumInterperterCommands.CUSTOMERWO + ">", new object[] {"123456"});
            custWO.Tag = Interperter.enumInterperterCommands.CUSTOMERWO;

            var partNumber = new UltraListViewItem("<" + Interperter.enumInterperterCommands.PARTNUMBER + ">", new object[] {"00146879-2"});
            partNumber.Tag = Interperter.enumInterperterCommands.PARTNUMBER;

            var partQty = new UltraListViewItem("<" + Interperter.enumInterperterCommands.PARTQTY + ">", new object[] {"5"});
            partQty.Tag = Interperter.enumInterperterCommands.PARTQTY;

            var partRev = new UltraListViewItem("<" + Interperter.enumInterperterCommands.PARTREV + ">", new object[] {"1"});
            partRev.Tag = Interperter.enumInterperterCommands.PARTREV;

            var partAss = new UltraListViewItem("<" + Interperter.enumInterperterCommands.ASSEMBLY + ">", new object[] {"ABC-1234"});
            partAss.Tag = Interperter.enumInterperterCommands.ASSEMBLY;

            this.lvwTemplates.Items.Add(date);
            this.lvwTemplates.Items.Add(date2);
            this.lvwTemplates.Items.Add(time);
            this.lvwTemplates.Items.Add(custWO);
            this.lvwTemplates.Items.Add(partNumber);
            this.lvwTemplates.Items.Add(partQty);
            this.lvwTemplates.Items.Add(partRev);
            this.lvwTemplates.Items.Add(partAss);
        }

        public void LoadCustomCodes(int customerID)
        {
            try
            {
                var itemsToRemove = new List <UltraListViewItem>();

                //remove any other custom codes
                foreach(var item in this.lvwTemplates.Items)
                {
                    if((Interperter.enumInterperterCommands) item.Tag == Interperter.enumInterperterCommands.CUSTOMFIELD)
                        itemsToRemove.Add(item);
                }

                itemsToRemove.ForEach(lvi => this.lvwTemplates.Items.Remove(lvi));

                //add customers custom codes
                var customFields = Dataset.CustomField.Select("CustomerID = " + customerID, "Name") as CustomersDataset.CustomFieldRow[];

                foreach(var cf in customFields)
                {
                    if(!cf.IsTokenNameNull() && !String.IsNullOrWhiteSpace(cf.TokenName))
                    {
                        var lvi = new UltraListViewItem("<" + cf.TokenName + ">", new object[] {"CUSTOM"});
                        lvi.Tag = Interperter.enumInterperterCommands.CUSTOMFIELD;
                        this.lvwTemplates.Items.Add(lvi);
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error  ");
            }
        }

        public override void AddValidators(DWOS.Utilities.Validation.ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboAirframe, "Airframe is required."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboSpec, "Airframe is required."), errProvider));
        }

        public CustomersDataset.PartMarkingRow AddPartMarkingRow(CustomersDataset.CustomerRow currentCustomer)
        {
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as CustomersDataset.PartMarkingRow;
            cr.d_AirframeRow = Dataset.d_Airframe.FirstOrDefault(); 
            cr.ProcessSpec = "<DEFAULT>";

            //MUST have end edit to flush changes to PartMarking Table before adding relationship
            rowVw.EndEdit();
            //add junction table row
            Dataset.Customer_PartMarking.AddCustomer_PartMarkingRow(currentCustomer, cr);

            return cr;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            if(base.CurrentRecord is CustomersDataset.PartMarkingRow)
            {
                var row = (CustomersDataset.PartMarkingRow) base.CurrentRecord;

                this.chkDef1.Checked = !row.IsDef1Null();
                this.chkDef2.Checked = !row.IsDef2Null();
                this.chkDef3.Checked = !row.IsDef3Null();
                this.chkDef4.Checked = !row.IsDef4Null();
            }
        }

        #endregion

        #region Events

        private void cboManufacturer_ValueChanged(object sender, EventArgs e) { }

        private void chkDef1_CheckedChanged(object sender, EventArgs e)
        {
            this.txtDef1.Enabled = this.chkDef1.Checked;

            if(!this.chkDef1.Checked)
                this.txtDef1.ResetText();
        }

        private void chkDef2_CheckedChanged(object sender, EventArgs e)
        {
            this.txtDef2.Enabled = this.chkDef2.Checked;

            if(!this.chkDef2.Checked)
                this.txtDef2.ResetText();
        }

        private void chkDef3_CheckedChanged(object sender, EventArgs e)
        {
            this.txtDef3.Enabled = this.chkDef3.Checked;

            if(!this.chkDef3.Checked)
                this.txtDef3.ResetText();
        }

        private void chkDef4_CheckedChanged(object sender, EventArgs e)
        {
            this.txtDef4.Enabled = this.chkDef4.Checked;

            if(!this.chkDef4.Checked)
                this.txtDef4.ResetText();
        }

        private void cboAirframe_ValueChanged(object sender, EventArgs e)
        {
            if(this.cboAirframe.SelectedItem != null)
            {
                //update the manufacturer based on the selected airframe
                var airframe = ((DataRowView) this.cboAirframe.SelectedItem.ListObject).Row as CustomersDataset.d_AirframeRow;

                if(airframe != null && !airframe.IsManufacturerIDNull())
                {
                    ValueListItem item = this.cboManufacturer.FindItemByValue <string>(vli => vli == airframe.ManufacturerID);
                    if(item != null)
                        this.cboManufacturer.SelectedItem = item;
                }
                else
                    this.cboManufacturer.SelectedItem = null;
            }
            else
                this.cboManufacturer.SelectedItem = null;
        }

        private void lvwTemplates_MouseDown(object sender, MouseEventArgs e)
        {
            UltraListViewItem itemAtPoint = this.lvwTemplates.ItemFromPoint(e.X, e.Y, true);
            if(itemAtPoint != null)
                this._lastMouseDown = new Point(e.X, e.Y);
            else
                this._lastMouseDown = Point.Empty;
        }

        private void lvwTemplates_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left && this._lastMouseDown != Point.Empty && this.lvwTemplates.SelectedItems.Count > 0)
            {
                var cursorPos = new Point(e.X, e.Y);
                Point dragPoint = this._lastMouseDown;

                var dragRect = new Rectangle(dragPoint, SystemInformation.DragSize);
                dragRect.X -= (SystemInformation.DragSize.Width / 2);
                dragRect.Y -= (SystemInformation.DragSize.Height / 2);

                if(dragRect.Contains(cursorPos) == false)
                {
                    this.lvwTemplates.Capture = false;
                    this.lvwTemplates.DoDragDrop(this.lvwTemplates.SelectedItems, DragDropEffects.Copy);
                }
            }
        }

        private void txtDef_DragDrop(object sender, DragEventArgs e)
        {
            IDataObject dataObject = e.Data;
            if(dataObject != null)
            {
                var selectedItems = dataObject.GetData(typeof(UltraListViewSelectedItemsCollection)) as UltraListViewSelectedItemsCollection;
                if(selectedItems != null)
                {
                    foreach(var item in selectedItems)
                    {
                        if(item.Tag is Interperter.enumInterperterCommands)
                        {
                            var cmd = (Interperter.enumInterperterCommands) item.Tag;
                            ((UltraTextEditor) sender).Text += "<" + cmd + ">";
                        }
                    }
                }

                //dataObject.SetData(null);
            }
        }

        private void txtDef_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            e.Action = DragAction.Continue;
        }

        private void txtDef_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        #endregion
    }
}