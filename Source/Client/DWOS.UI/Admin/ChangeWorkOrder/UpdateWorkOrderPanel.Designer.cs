namespace DWOS.UI.Admin.ChangeWorkOrder
{
    partial class UpdateWorkOrderPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.numWorkOrder = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboPart = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtStatus = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.cboCustomer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.numWorkOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).BeginInit();
            this.SuspendLayout();
            // 
            // numWorkOrder
            // 
            this.numWorkOrder.Enabled = false;
            this.numWorkOrder.Location = new System.Drawing.Point(72, 3);
            this.numWorkOrder.MinValue = 0;
            this.numWorkOrder.Name = "numWorkOrder";
            this.numWorkOrder.Nullable = true;
            this.numWorkOrder.ReadOnly = true;
            this.numWorkOrder.Size = new System.Drawing.Size(170, 21);
            this.numWorkOrder.TabIndex = 0;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(0, 7);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(66, 14);
            this.ultraLabel1.TabIndex = 4;
            this.ultraLabel1.Text = "Work Order:";
            // 
            // cboPart
            // 
            this.cboPart.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            this.cboPart.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.StartsWith;
            this.cboPart.Location = new System.Drawing.Point(72, 57);
            this.cboPart.Name = "cboPart";
            this.cboPart.NullText = "- Choose a part -";
            appearance1.ForeColor = System.Drawing.Color.Gray;
            this.cboPart.NullTextAppearance = appearance1;
            this.cboPart.Size = new System.Drawing.Size(170, 21);
            this.cboPart.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboPart.TabIndex = 2;
            this.cboPart.InitializeDataItem += new Infragistics.Win.InitializeDataItemHandler(this.cboPart_InitializeDataItem);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(0, 61);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(28, 14);
            this.ultraLabel2.TabIndex = 6;
            this.ultraLabel2.Text = "Part:";
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(0, 111);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(237, 233);
            this.txtStatus.TabIndex = 3;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(0, 91);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(39, 14);
            this.ultraLabel3.TabIndex = 8;
            this.ultraLabel3.Text = "Status:";
            // 
            // cboCustomer
            // 
            this.cboCustomer.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            this.cboCustomer.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.StartsWith;
            this.cboCustomer.Location = new System.Drawing.Point(72, 30);
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.Nullable = false;
            this.cboCustomer.Size = new System.Drawing.Size(170, 21);
            this.cboCustomer.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboCustomer.TabIndex = 1;
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(3, 35);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(56, 14);
            this.ultraLabel4.TabIndex = 10;
            this.ultraLabel4.Text = "Customer:";
            // 
            // UpdateWorkOrderPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.cboCustomer);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.cboPart);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.numWorkOrder);
            this.Name = "UpdateWorkOrderPanel";
            this.Size = new System.Drawing.Size(256, 347);
            ((System.ComponentModel.ISupportInitialize)(this.numWorkOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numWorkOrder;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboPart;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtStatus;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomer;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
    }
}
