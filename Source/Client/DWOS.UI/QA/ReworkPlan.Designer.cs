namespace DWOS.UI.QA
{
    partial class ReworkPlan
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The current user logged in.", Infragistics.Win.ToolTipImage.Default, "QA Personnel", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The current work order number.", Infragistics.Win.ToolTipImage.Default, "Work Order", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The name of the part.", Infragistics.Win.ToolTipImage.Default, "Part Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date the rework planning is taking place.", Infragistics.Win.ToolTipImage.Default, "Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Send the report directly to the printer.", Infragistics.Win.ToolTipImage.Default, "Quick Print", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The type of rework to be done on this order.", Infragistics.Win.ToolTipImage.Default, "Rework Type", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The reason for doing the rework.", Infragistics.Win.ToolTipImage.Default, "Rework Reason", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReworkPlan));
            this.grpOrderInfo = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtUser = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtOrderID = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel10 = new Infragistics.Win.Misc.UltraLabel();
            this.txtPartID = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.dteDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.chkQuickPrint = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtReworkType = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtReason = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.picReworkType = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.dsOrders = new DWOS.Data.Datasets.OrdersDataSet();
            this.taOrder = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter();
            this.taInternalRework = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.InternalReworkTableAdapter();
            this.taManager = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.TableAdapterManager();
            this.taOrderProcesses = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderProcessesTableAdapter();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.processEditor = new DWOS.UI.QA.OrderProcessEditor();
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            this.taOrderSerialNumber = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderSerialNumberTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.grpOrderInfo)).BeginInit();
            this.grpOrderInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkQuickPrint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReworkType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).BeginInit();
            this.SuspendLayout();
            // 
            // grpOrderInfo
            // 
            this.grpOrderInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOrderInfo.Controls.Add(this.txtUser);
            this.grpOrderInfo.Controls.Add(this.ultraLabel2);
            this.grpOrderInfo.Controls.Add(this.txtOrderID);
            this.grpOrderInfo.Controls.Add(this.ultraLabel10);
            this.grpOrderInfo.Controls.Add(this.txtPartID);
            this.grpOrderInfo.Controls.Add(this.ultraLabel1);
            this.grpOrderInfo.Controls.Add(this.dteDate);
            this.grpOrderInfo.Controls.Add(this.ultraLabel5);
            this.grpOrderInfo.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            appearance1.Image = global::DWOS.UI.Properties.Resources.Order_16;
            this.grpOrderInfo.HeaderAppearance = appearance1;
            this.grpOrderInfo.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grpOrderInfo.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpOrderInfo.Location = new System.Drawing.Point(11, 14);
            this.grpOrderInfo.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.grpOrderInfo.Name = "grpOrderInfo";
            this.grpOrderInfo.Size = new System.Drawing.Size(494, 117);
            this.grpOrderInfo.TabIndex = 0;
            this.grpOrderInfo.Text = "Order Information";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(134, 75);
            this.txtUser.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.txtUser.Name = "txtUser";
            this.txtUser.ReadOnly = true;
            this.txtUser.Size = new System.Drawing.Size(135, 25);
            this.txtUser.TabIndex = 2;
            this.txtUser.Text = "12345";
            ultraToolTipInfo4.ToolTipText = "The current user logged in.";
            ultraToolTipInfo4.ToolTipTitle = "QA Personnel";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtUser, ultraToolTipInfo4);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(287, 76);
            this.ultraLabel2.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(44, 18);
            this.ultraLabel2.TabIndex = 45;
            this.ultraLabel2.Text = "Date:";
            // 
            // txtOrderID
            // 
            this.txtOrderID.Location = new System.Drawing.Point(134, 38);
            this.txtOrderID.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.txtOrderID.Name = "txtOrderID";
            this.txtOrderID.ReadOnly = true;
            this.txtOrderID.Size = new System.Drawing.Size(135, 25);
            this.txtOrderID.TabIndex = 0;
            this.txtOrderID.Text = "12345";
            ultraToolTipInfo5.ToolTipText = "The current work order number.";
            ultraToolTipInfo5.ToolTipTitle = "Work Order";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtOrderID, ultraToolTipInfo5);
            // 
            // ultraLabel10
            // 
            this.ultraLabel10.AutoSize = true;
            this.ultraLabel10.Location = new System.Drawing.Point(14, 39);
            this.ultraLabel10.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.ultraLabel10.Name = "ultraLabel10";
            this.ultraLabel10.Size = new System.Drawing.Size(91, 18);
            this.ultraLabel10.TabIndex = 44;
            this.ultraLabel10.Text = "Work Order:";
            // 
            // txtPartID
            // 
            this.txtPartID.Location = new System.Drawing.Point(344, 38);
            this.txtPartID.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.txtPartID.Name = "txtPartID";
            this.txtPartID.ReadOnly = true;
            this.txtPartID.Size = new System.Drawing.Size(135, 25);
            this.txtPartID.TabIndex = 1;
            this.txtPartID.Text = "12345";
            ultraToolTipInfo6.ToolTipText = "The name of the part.";
            ultraToolTipInfo6.ToolTipTitle = "Part Name";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtPartID, ultraToolTipInfo6);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(287, 39);
            this.ultraLabel1.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(39, 18);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Part:";
            // 
            // dteDate
            // 
            this.dteDate.Location = new System.Drawing.Point(344, 75);
            this.dteDate.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.dteDate.Name = "dteDate";
            this.dteDate.ReadOnly = true;
            this.dteDate.Size = new System.Drawing.Size(135, 25);
            this.dteDate.TabIndex = 3;
            ultraToolTipInfo7.ToolTipText = "The date the rework planning is taking place.";
            ultraToolTipInfo7.ToolTipTitle = "Date";
            this.ultraToolTipManager1.SetUltraToolTip(this.dteDate, ultraToolTipInfo7);
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(14, 76);
            this.ultraLabel5.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(103, 18);
            this.ultraLabel5.TabIndex = 32;
            this.ultraLabel5.Text = "QA Personnel:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(416, 514);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 28);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(322, 514);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(87, 28);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chkQuickPrint
            // 
            this.chkQuickPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkQuickPrint.AutoSize = true;
            this.chkQuickPrint.Checked = true;
            this.chkQuickPrint.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkQuickPrint.Location = new System.Drawing.Point(185, 510);
            this.chkQuickPrint.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkQuickPrint.Name = "chkQuickPrint";
            this.chkQuickPrint.Size = new System.Drawing.Size(95, 21);
            this.chkQuickPrint.TabIndex = 10;
            this.chkQuickPrint.Text = "Quick Print";
            ultraToolTipInfo3.ToolTipText = "Send the report directly to the printer.";
            ultraToolTipInfo3.ToolTipTitle = "Quick Print";
            this.ultraToolTipManager1.SetUltraToolTip(this.chkQuickPrint, ultraToolTipInfo3);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(73, 179);
            this.ultraLabel4.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(60, 18);
            this.ultraLabel4.TabIndex = 86;
            this.ultraLabel4.Text = "Reason:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(73, 145);
            this.ultraLabel3.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(43, 18);
            this.ultraLabel3.TabIndex = 85;
            this.ultraLabel3.Text = "Type:";
            // 
            // txtReworkType
            // 
            this.txtReworkType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReworkType.Location = new System.Drawing.Point(145, 141);
            this.txtReworkType.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.txtReworkType.Name = "txtReworkType";
            this.txtReworkType.ReadOnly = true;
            this.txtReworkType.Size = new System.Drawing.Size(345, 25);
            this.txtReworkType.TabIndex = 1;
            this.txtReworkType.Text = "Full Rework";
            ultraToolTipInfo2.ToolTipText = "The type of rework to be done on this order.";
            ultraToolTipInfo2.ToolTipTitle = "Rework Type";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtReworkType, ultraToolTipInfo2);
            // 
            // txtReason
            // 
            this.txtReason.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReason.Location = new System.Drawing.Point(145, 176);
            this.txtReason.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.txtReason.Name = "txtReason";
            this.txtReason.ReadOnly = true;
            this.txtReason.Size = new System.Drawing.Size(345, 25);
            this.txtReason.TabIndex = 2;
            this.txtReason.Text = "Incorrect paint";
            ultraToolTipInfo1.ToolTipText = "The reason for doing the rework.";
            ultraToolTipInfo1.ToolTipTitle = "Rework Reason";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtReason, ultraToolTipInfo1);
            // 
            // picReworkType
            // 
            this.picReworkType.BorderShadowColor = System.Drawing.Color.Empty;
            this.picReworkType.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.picReworkType.Image = ((object)(resources.GetObject("picReworkType.Image")));
            this.picReworkType.Location = new System.Drawing.Point(11, 145);
            this.picReworkType.Name = "picReworkType";
            this.picReworkType.Size = new System.Drawing.Size(57, 50);
            this.picReworkType.TabIndex = 88;
            // 
            // dsOrders
            // 
            this.dsOrders.DataSetName = "OrdersDataSet";
            this.dsOrders.EnforceConstraints = false;
            this.dsOrders.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taOrder
            // 
            this.taOrder.ClearBeforeFill = false;
            // 
            // taInternalRework
            // 
            this.taInternalRework.ClearBeforeFill = true;
            // 
            // taManager
            // 
            this.taManager.BackupDataSetBeforeUpdate = false;
            this.taManager.BatchOperatorTableAdapter = null;
            this.taManager.BatchOperatorTimeTableAdapter = null;
            this.taManager.BatchOrderTableAdapter = null;
            this.taManager.BatchProcessesOperatorTableAdapter = null;
            this.taManager.BatchProcessesTableAdapter = null;
            this.taManager.BatchTableAdapter = null;
            this.taManager.BulkCOCOrderTableAdapter = null;
            this.taManager.BulkCOCTableAdapter = null;
            this.taManager.COCTableAdapter = null;
            this.taManager.CustomerAddressTableAdapter = null;
            this.taManager.CustomerCommunicationTableAdapter = null;
            this.taManager.d_FeeTypeTableAdapter = null;
            this.taManager.d_HoldLocationTableAdapter = null;
            this.taManager.d_HoldReasonTableAdapter = null;
            this.taManager.d_ReworkReasonTableAdapter = null;
            this.taManager.InternalReworkTableAdapter = this.taInternalRework;
            this.taManager.LaborTimeTableAdapter = null;
            this.taManager.MediaTableAdapter = null;
            this.taManager.Order_DocumentLinkTableAdapter = null;
            this.taManager.Order_MediaTableAdapter = null;
            this.taManager.OrderChangeTableAdapter = null;
            this.taManager.OrderContainersTableAdapter = null;
            this.taManager.OrderCustomFieldsTableAdapter = null;
            this.taManager.OrderFeesTableAdapter = null;
            this.taManager.OrderFeeTypeTableAdapter = null;
            this.taManager.OrderHoldTableAdapter = null;
            this.taManager.OrderNoteTableAdapter = null;
            this.taManager.OrderOperatorTableAdapter = null;
            this.taManager.OrderOperatorTimeTableAdapter = null;
            this.taManager.OrderPartMarkTableAdapter = null;
            this.taManager.OrderProcessAnswerTableAdapter = null;
            this.taManager.OrderProcessesOperatorTableAdapter = null;
            this.taManager.OrderProcessesTableAdapter = this.taOrderProcesses;
            this.taManager.OrderReviewTableAdapter = null;
            this.taManager.OrderSerialNumberTableAdapter = null;
            this.taManager.OrderShipmentTableAdapter = null;
            this.taManager.OrderTableAdapter = this.taOrder;
            this.taManager.OrderTemplateTableAdapter = null;
            this.taManager.PartInspectionAnswerTableAdapter = null;
            this.taManager.PartInspectionTableAdapter = null;
            this.taManager.PartSummaryTableAdapter = null;
            this.taManager.PricePointDetailTableAdapter = null;
            this.taManager.PricePointTableAdapter = null;
            this.taManager.PriceUnitTableAdapter = null;
            this.taManager.QuoteTableAdapter = null;
            this.taManager.SalesOrder_DocumentLinkTableAdapter = null;
            this.taManager.SalesOrder_MediaTableAdapter = null;
            this.taManager.SalesOrderTableAdapter = null;
            this.taManager.UpdateOrder = DWOS.Data.Datasets.OrdersDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            // 
            // taOrderProcesses
            // 
            this.taOrderProcesses.ClearBeforeFill = true;
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // processEditor
            // 
            this.processEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.processEditor.DisplayCOCColumn = true;
            this.processEditor.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processEditor.IsInRework = false;
            this.processEditor.Location = new System.Drawing.Point(12, 210);
            this.processEditor.Margin = new System.Windows.Forms.Padding(4);
            this.processEditor.Name = "processEditor";
            this.processEditor.OrderRow = null;
            this.processEditor.Size = new System.Drawing.Size(478, 284);
            this.processEditor.TabIndex = 89;
            this.processEditor.ViewOnly = false;
            // 
            // helpLink1
            // 
            this.helpLink1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = "rework_planning_dialog.htm";
            this.helpLink1.Location = new System.Drawing.Point(12, 527);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 90;
            // 
            // taOrderSerialNumber
            // 
            this.taOrderSerialNumber.ClearBeforeFill = false;
            // 
            // ReworkPlan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 555);
            this.Controls.Add(this.helpLink1);
            this.Controls.Add(this.processEditor);
            this.Controls.Add(this.picReworkType);
            this.Controls.Add(this.txtReason);
            this.Controls.Add(this.txtReworkType);
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.chkQuickPrint);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpOrderInfo);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReworkPlan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rework Planning";
            this.Load += new System.EventHandler(this.ReworkPlan_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grpOrderInfo)).EndInit();
            this.grpOrderInfo.ResumeLayout(false);
            this.grpOrderInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkQuickPrint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReworkType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox grpOrderInfo;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtUser;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtOrderID;
        private Infragistics.Win.Misc.UltraLabel ultraLabel10;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPartID;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteDate;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkQuickPrint;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtReworkType;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtReason;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picReworkType;
        private Data.Datasets.OrdersDataSet dsOrders;
        private Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter taOrder;
        private Data.Datasets.OrdersDataSetTableAdapters.InternalReworkTableAdapter taInternalRework;
        private Data.Datasets.OrdersDataSetTableAdapters.TableAdapterManager taManager;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private OrderProcessEditor processEditor;
        private Utilities.HelpLink helpLink1;
        private Data.Datasets.OrdersDataSetTableAdapters.OrderProcessesTableAdapter taOrderProcesses;
        private Data.Datasets.OrdersDataSetTableAdapters.OrderSerialNumberTableAdapter taOrderSerialNumber;
    }
}