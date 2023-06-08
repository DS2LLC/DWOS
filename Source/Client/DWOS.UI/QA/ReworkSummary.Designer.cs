namespace DWOS.UI.QA
{
    partial class ReworkSummary
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReworkSummary));
            Infragistics.Win.UltraWinListView.UltraListViewItem ultraListViewItem1 = new Infragistics.Win.UltraWinListView.UltraListViewItem("1", new Infragistics.Win.UltraWinListView.UltraListViewSubItem[] {
            null,
            null}, null);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Status");
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn2 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Quantity");
            this.picReworkType = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.txtReason = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtReworkType = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.lvwProcesses = new Infragistics.Win.UltraWinListView.UltraListView();
            this.dsOrders = new DWOS.Data.Datasets.OrdersDataSet();
            this.taOrder = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter();
            this.taInternalRework = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.InternalReworkTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.txtReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReworkType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvwProcesses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).BeginInit();
            this.SuspendLayout();
            // 
            // picReworkType
            // 
            this.picReworkType.BorderShadowColor = System.Drawing.Color.Empty;
            this.picReworkType.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.picReworkType.Image = ((object)(resources.GetObject("picReworkType.Image")));
            this.picReworkType.Location = new System.Drawing.Point(20, 18);
            this.picReworkType.Name = "picReworkType";
            this.picReworkType.Size = new System.Drawing.Size(57, 50);
            this.picReworkType.TabIndex = 93;
            this.picReworkType.UseAppStyling = false;
            // 
            // txtReason
            // 
            this.txtReason.Location = new System.Drawing.Point(154, 49);
            this.txtReason.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.txtReason.Name = "txtReason";
            this.txtReason.ReadOnly = true;
            this.txtReason.Size = new System.Drawing.Size(281, 25);
            this.txtReason.TabIndex = 92;
            this.txtReason.Text = "Incorrect paint";
            // 
            // txtReworkType
            // 
            this.txtReworkType.Location = new System.Drawing.Point(154, 14);
            this.txtReworkType.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.txtReworkType.Name = "txtReworkType";
            this.txtReworkType.ReadOnly = true;
            this.txtReworkType.Size = new System.Drawing.Size(281, 25);
            this.txtReworkType.TabIndex = 89;
            this.txtReworkType.Text = "Full Rework";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(82, 52);
            this.ultraLabel4.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(60, 18);
            this.ultraLabel4.TabIndex = 91;
            this.ultraLabel4.Text = "Reason:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(82, 18);
            this.ultraLabel3.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(43, 18);
            this.ultraLabel3.TabIndex = 90;
            this.ultraLabel3.Text = "Type:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(350, 223);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(87, 28);
            this.btnOK.TabIndex = 94;
            this.btnOK.Text = "OK";
            // 
            // lvwProcesses
            // 
            this.lvwProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwProcesses.Items.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewItem[] {
            ultraListViewItem1});
            this.lvwProcesses.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            this.lvwProcesses.ItemSettings.HideSelection = false;
            this.lvwProcesses.ItemSettings.HotTracking = true;
            this.lvwProcesses.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.Single;
            this.lvwProcesses.Location = new System.Drawing.Point(12, 82);
            this.lvwProcesses.MainColumn.DataType = typeof(int);
            appearance1.Image = global::DWOS.UI.Properties.Resources.Order_16;
            this.lvwProcesses.MainColumn.ItemAppearance = appearance1;
            this.lvwProcesses.MainColumn.Key = "Order";
            this.lvwProcesses.MainColumn.Text = "Order";
            this.lvwProcesses.MainColumn.VisiblePositionInDetailsView = 0;
            this.lvwProcesses.MainColumn.Width = 40;
            this.lvwProcesses.Name = "lvwProcesses";
            this.lvwProcesses.Size = new System.Drawing.Size(423, 130);
            ultraListViewSubItemColumn1.Key = "Status";
            ultraListViewSubItemColumn2.Key = "Quantity";
            ultraListViewSubItemColumn2.Width = 40;
            this.lvwProcesses.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1,
            ultraListViewSubItemColumn2});
            this.lvwProcesses.TabIndex = 95;
            this.lvwProcesses.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvwProcesses.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns;
            this.lvwProcesses.ViewSettingsDetails.ColumnAutoSizeMode = ((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header | Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems)));
            this.lvwProcesses.ViewSettingsDetails.ColumnHeaderStyle = Infragistics.Win.HeaderStyle.WindowsVista;
            this.lvwProcesses.ViewSettingsDetails.FullRowSelect = true;
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
            // ReworkSummary
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 264);
            this.Controls.Add(this.lvwProcesses);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.picReworkType);
            this.Controls.Add(this.txtReason);
            this.Controls.Add(this.txtReworkType);
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.ultraLabel3);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ReworkSummary";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rework Summary";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ReworkSummary_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReworkType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvwProcesses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraPictureBox picReworkType;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtReason;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtReworkType;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.UltraWinListView.UltraListView lvwProcesses;
        private Data.Datasets.OrdersDataSet dsOrders;
        private Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter taOrder;
        private Data.Datasets.OrdersDataSetTableAdapters.InternalReworkTableAdapter taInternalRework;
    }
}