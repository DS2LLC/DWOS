namespace DWOS.UI.Sales.Order
{
    partial class NewOrderForBlanketPODialog
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The number of parts in the PO order.", Infragistics.Win.ToolTipImage.Default, "Part Quantity", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The available part quantity.", Infragistics.Win.ToolTipImage.Default, "Available Part Quantity", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The blanket PO.", Infragistics.Win.ToolTipImage.Default, "Blanket PO", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab(true);
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.numPartQty = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.numAvailableQty = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtBlanketPONumber = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.customFieldsWidget = new DWOS.UI.Utilities.CustomFieldsWidget();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.tabFields = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.errProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAvailableQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBlanketPONumber)).BeginInit();
            this.ultraTabPageControl2.SuspendLayout();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabFields)).BeginInit();
            this.tabFields.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel1);
            this.ultraTabPageControl1.Controls.Add(this.numPartQty);
            this.ultraTabPageControl1.Controls.Add(this.numAvailableQty);
            this.ultraTabPageControl1.Controls.Add(this.txtBlanketPONumber);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel3);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel2);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(419, 133);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(16, 25);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(72, 15);
            this.ultraLabel1.TabIndex = 38;
            this.ultraLabel1.Text = "Blanket PO:";
            // 
            // numPartQty
            // 
            this.numPartQty.Location = new System.Drawing.Point(179, 76);
            this.numPartQty.MinValue = 1;
            this.numPartQty.Name = "numPartQty";
            this.numPartQty.Size = new System.Drawing.Size(237, 22);
            this.numPartQty.TabIndex = 2;
            ultraToolTipInfo1.ToolTipText = "The number of parts in the PO order.";
            ultraToolTipInfo1.ToolTipTitle = "Part Quantity";
            this.tipManager.SetUltraToolTip(this.numPartQty, ultraToolTipInfo1);
            this.numPartQty.Enter += new System.EventHandler(this.numPartQty_Enter);
            // 
            // numAvailableQty
            // 
            this.numAvailableQty.Location = new System.Drawing.Point(179, 48);
            this.numAvailableQty.Name = "numAvailableQty";
            this.numAvailableQty.ReadOnly = true;
            this.numAvailableQty.Size = new System.Drawing.Size(237, 22);
            this.numAvailableQty.TabIndex = 1;
            ultraToolTipInfo2.ToolTipText = "The available part quantity.";
            ultraToolTipInfo2.ToolTipTitle = "Available Part Quantity";
            this.tipManager.SetUltraToolTip(this.numAvailableQty, ultraToolTipInfo2);
            // 
            // txtBlanketPONumber
            // 
            this.txtBlanketPONumber.Location = new System.Drawing.Point(179, 21);
            this.txtBlanketPONumber.Name = "txtBlanketPONumber";
            this.txtBlanketPONumber.ReadOnly = true;
            this.txtBlanketPONumber.Size = new System.Drawing.Size(237, 22);
            this.txtBlanketPONumber.TabIndex = 0;
            ultraToolTipInfo3.ToolTipText = "The blanket PO.";
            ultraToolTipInfo3.ToolTipTitle = "Blanket PO";
            this.tipManager.SetUltraToolTip(this.txtBlanketPONumber, ultraToolTipInfo3);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(16, 80);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(85, 15);
            this.ultraLabel3.TabIndex = 40;
            this.ultraLabel3.Text = "Part Quantity:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(15, 52);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(140, 15);
            this.ultraLabel2.TabIndex = 39;
            this.ultraLabel2.Text = "Available Part Quantity:";
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.ultraPanel1);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(419, 133);
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
            this.ultraPanel1.Size = new System.Drawing.Size(419, 133);
            this.ultraPanel1.TabIndex = 91;
            // 
            // customFieldsWidget
            // 
            this.customFieldsWidget.AutoSize = true;
            this.customFieldsWidget.Location = new System.Drawing.Point(13, 15);
            this.customFieldsWidget.Margin = new System.Windows.Forms.Padding(0);
            this.customFieldsWidget.Name = "customFieldsWidget";
            this.customFieldsWidget.Size = new System.Drawing.Size(393, 29);
            this.customFieldsWidget.TabIndex = 90;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(357, 182);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(275, 182);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tabFields
            // 
            this.tabFields.Controls.Add(this.ultraTabSharedControlsPage1);
            this.tabFields.Controls.Add(this.ultraTabPageControl1);
            this.tabFields.Controls.Add(this.ultraTabPageControl2);
            this.tabFields.Location = new System.Drawing.Point(12, 12);
            this.tabFields.Name = "tabFields";
            this.tabFields.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.tabFields.Size = new System.Drawing.Size(423, 159);
            this.tabFields.TabIndex = 3;
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
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(419, 133);
            // 
            // errProvider
            // 
            this.errProvider.ContainerControl = this;
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // NewOrderForBlanketPODialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(447, 217);
            this.Controls.Add(this.tabFields);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "NewOrderForBlanketPODialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add New PO Order";
            this.Load += new System.EventHandler(this.AddBlanketPOOrder_Load);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAvailableQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBlanketPONumber)).EndInit();
            this.ultraTabPageControl2.ResumeLayout(false);
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ClientArea.PerformLayout();
            this.ultraPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabFields)).EndInit();
            this.tabFields.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numPartQty;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numAvailableQty;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBlanketPONumber;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl tabFields;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        private Utilities.CustomFieldsWidget customFieldsWidget;
        protected System.Windows.Forms.ErrorProvider errProvider;
        protected Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
    }
}