namespace DWOS.UI.Sales.Order
{
    partial class NewBlanketPOOrderDialog
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
            DisposeMe();

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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The customer of the blanket PO.", Infragistics.Win.ToolTipImage.Default, "Customer", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The customer work order, if available.", Infragistics.Win.ToolTipImage.Default, "Customer Work Order", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The number of parts in the PO order.", Infragistics.Win.ToolTipImage.Default, "Part Quantity", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The blanket PO.", Infragistics.Win.ToolTipImage.Default, "Blanket PO", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The part of the blanket PO.", Infragistics.Win.ToolTipImage.Default, "Part", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab(true);
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewBlanketPOOrderDialog));
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.cboCustomer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCustomerWO = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.numPartQty = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel18 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.cboBlanketPO = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboPart = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.customFieldsWidget = new DWOS.UI.Utilities.CustomFieldsWidget();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.taOrderTemplate = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderTemplateTableAdapter();
            this.taCustomerSummary = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.CustomerSummaryTableAdapter();
            this.taPartSummary = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.PartSummaryTableAdapter();
            this.tabFields = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.errProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.dsOrders = new DWOS.Data.Datasets.OrdersDataSet();
            this.taOrder = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerWO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBlanketPO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPart)).BeginInit();
            this.ultraTabPageControl2.SuspendLayout();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabFields)).BeginInit();
            this.tabFields.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.cboCustomer);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel3);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel1);
            this.ultraTabPageControl1.Controls.Add(this.txtCustomerWO);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel2);
            this.ultraTabPageControl1.Controls.Add(this.numPartQty);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel18);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel6);
            this.ultraTabPageControl1.Controls.Add(this.cboBlanketPO);
            this.ultraTabPageControl1.Controls.Add(this.cboPart);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(419, 199);
            // 
            // cboCustomer
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Customer;
            this.cboCustomer.Appearance = appearance1;
            this.cboCustomer.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            this.cboCustomer.AutoSize = false;
            this.cboCustomer.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.StartsWith;
            this.cboCustomer.DropDownListWidth = -1;
            this.cboCustomer.Location = new System.Drawing.Point(124, 15);
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.Nullable = false;
            this.cboCustomer.NullText = "Select Customer";
            this.cboCustomer.Size = new System.Drawing.Size(292, 22);
            this.cboCustomer.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboCustomer.TabIndex = 0;
            ultraToolTipInfo1.ToolTipText = "The customer of the blanket PO.";
            ultraToolTipInfo1.ToolTipTitle = "Customer";
            this.tipManager.SetUltraToolTip(this.cboCustomer, ultraToolTipInfo1);
            this.cboCustomer.SelectionChanged += new System.EventHandler(this.cboCustomer_SelectionChanged);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(13, 19);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(64, 15);
            this.ultraLabel3.TabIndex = 48;
            this.ultraLabel3.Text = "Customer:";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(13, 103);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(85, 15);
            this.ultraLabel1.TabIndex = 83;
            this.ultraLabel1.Text = "Part Quantity:";
            // 
            // txtCustomerWO
            // 
            this.txtCustomerWO.Enabled = false;
            this.txtCustomerWO.Location = new System.Drawing.Point(124, 127);
            this.txtCustomerWO.Name = "txtCustomerWO";
            this.txtCustomerWO.Size = new System.Drawing.Size(292, 22);
            this.txtCustomerWO.TabIndex = 4;
            ultraToolTipInfo2.ToolTipText = "The customer work order, if available.";
            ultraToolTipInfo2.ToolTipTitle = "Customer Work Order";
            this.tipManager.SetUltraToolTip(this.txtCustomerWO, ultraToolTipInfo2);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(13, 131);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(88, 15);
            this.ultraLabel2.TabIndex = 89;
            this.ultraLabel2.Text = "Customer WO:";
            // 
            // numPartQty
            // 
            this.numPartQty.Enabled = false;
            this.numPartQty.Location = new System.Drawing.Point(124, 99);
            this.numPartQty.MinValue = 0;
            this.numPartQty.Name = "numPartQty";
            this.numPartQty.Size = new System.Drawing.Size(292, 22);
            this.numPartQty.TabIndex = 3;
            ultraToolTipInfo3.ToolTipText = "The number of parts in the PO order.";
            ultraToolTipInfo3.ToolTipTitle = "Part Quantity";
            this.tipManager.SetUltraToolTip(this.numPartQty, ultraToolTipInfo3);
            // 
            // ultraLabel18
            // 
            this.ultraLabel18.AutoSize = true;
            this.ultraLabel18.Location = new System.Drawing.Point(13, 47);
            this.ultraLabel18.Name = "ultraLabel18";
            this.ultraLabel18.Size = new System.Drawing.Size(32, 15);
            this.ultraLabel18.TabIndex = 75;
            this.ultraLabel18.Text = "Part:";
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(13, 75);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(72, 15);
            this.ultraLabel6.TabIndex = 87;
            this.ultraLabel6.Text = "Blanket PO:";
            // 
            // cboBlanketPO
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.BlanketPO_16;
            this.cboBlanketPO.Appearance = appearance2;
            this.cboBlanketPO.Enabled = false;
            this.cboBlanketPO.Location = new System.Drawing.Point(124, 71);
            this.cboBlanketPO.Name = "cboBlanketPO";
            this.cboBlanketPO.Size = new System.Drawing.Size(292, 22);
            this.cboBlanketPO.TabIndex = 2;
            ultraToolTipInfo4.ToolTipText = "The blanket PO.";
            ultraToolTipInfo4.ToolTipTitle = "Blanket PO";
            this.tipManager.SetUltraToolTip(this.cboBlanketPO, ultraToolTipInfo4);
            this.cboBlanketPO.SelectionChanged += new System.EventHandler(this.cboBlanketPO_SelectionChanged);
            // 
            // cboPart
            // 
            appearance3.Image = global::DWOS.UI.Properties.Resources.Part_16;
            this.cboPart.Appearance = appearance3;
            this.cboPart.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            this.cboPart.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.StartsWith;
            this.cboPart.DropDownListWidth = -1;
            this.cboPart.Enabled = false;
            this.cboPart.Location = new System.Drawing.Point(124, 43);
            this.cboPart.Name = "cboPart";
            this.cboPart.Nullable = false;
            this.cboPart.Size = new System.Drawing.Size(292, 22);
            this.cboPart.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboPart.TabIndex = 1;
            ultraToolTipInfo5.ToolTipText = "The part of the blanket PO.";
            ultraToolTipInfo5.ToolTipTitle = "Part";
            this.tipManager.SetUltraToolTip(this.cboPart, ultraToolTipInfo5);
            this.cboPart.SelectionChanged += new System.EventHandler(this.cboPart_SelectionChanged);
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.ultraPanel1);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(419, 199);
            // 
            // ultraPanel1
            // 
            this.ultraPanel1.AutoScroll = true;
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.customFieldsWidget);
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel1.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(419, 199);
            this.ultraPanel1.TabIndex = 91;
            // 
            // customFieldsWidget
            // 
            this.customFieldsWidget.AutoSize = true;
            this.customFieldsWidget.Location = new System.Drawing.Point(13, 15);
            this.customFieldsWidget.Margin = new System.Windows.Forms.Padding(0);
            this.customFieldsWidget.Name = "customFieldsWidget";
            this.customFieldsWidget.Size = new System.Drawing.Size(340, 29);
            this.customFieldsWidget.TabIndex = 90;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(357, 243);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(275, 243);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // taOrderTemplate
            // 
            this.taOrderTemplate.ClearBeforeFill = false;
            // 
            // taCustomerSummary
            // 
            this.taCustomerSummary.ClearBeforeFill = false;
            // 
            // taPartSummary
            // 
            this.taPartSummary.ClearBeforeFill = false;
            // 
            // tabFields
            // 
            this.tabFields.Controls.Add(this.ultraTabSharedControlsPage1);
            this.tabFields.Controls.Add(this.ultraTabPageControl1);
            this.tabFields.Controls.Add(this.ultraTabPageControl2);
            this.tabFields.Location = new System.Drawing.Point(12, 12);
            this.tabFields.Name = "tabFields";
            this.tabFields.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.tabFields.Size = new System.Drawing.Size(423, 225);
            this.tabFields.TabIndex = 2;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "General";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "Custom Fields";
            ultraTab2.Visible = false;
            this.tabFields.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(419, 199);
            // 
            // errProvider
            // 
            this.errProvider.ContainerControl = this;
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
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
            // NewBlanketPOOrderDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 278);
            this.Controls.Add(this.tabFields);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NewBlanketPOOrderDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add New PO Order";
            this.Load += new System.EventHandler(this.FindBlanketPO_Load);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerWO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBlanketPO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPart)).EndInit();
            this.ultraTabPageControl2.ResumeLayout(false);
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ClientArea.PerformLayout();
            this.ultraPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabFields)).EndInit();
            this.tabFields.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsOrders)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomer;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboPart;
        private Infragistics.Win.Misc.UltraLabel ultraLabel18;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numPartQty;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCustomerWO;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboBlanketPO;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Data.Datasets.OrdersDataSetTableAdapters.OrderTemplateTableAdapter taOrderTemplate;
        private Data.Datasets.OrdersDataSetTableAdapters.CustomerSummaryTableAdapter taCustomerSummary;
        private Data.Datasets.OrdersDataSetTableAdapters.PartSummaryTableAdapter taPartSummary;
        private Utilities.CustomFieldsWidget customFieldsWidget;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl tabFields;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        protected System.Windows.Forms.ErrorProvider errProvider;
        protected Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        private Data.Datasets.OrdersDataSet dsOrders;
        private Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter taOrder;
    }
}