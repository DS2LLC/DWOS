namespace DWOS.UI.Sales
{
    partial class OrderReviewStatus
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The order currently being reviewed.", Infragistics.Win.ToolTipImage.Default, "Work Order", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The user that reviewed the order.", Infragistics.Win.ToolTipImage.Default, "Reviewed By", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add note pertaining to the review.", Infragistics.Win.ToolTipImage.Default, "Notes", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Mark the review as \'Pass\' or \'Fail\'.", Infragistics.Win.ToolTipImage.Default, "Review Status", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderReviewStatus));
            this.bsOrderReview = new System.Windows.Forms.BindingSource(this.components);
            this.grpReview = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtWorkOrder = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboReviewUser = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel10 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel17 = new Infragistics.Win.Misc.UltraLabel();
            this.txtNotes = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboStatus = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.bsOrderReview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpReview)).BeginInit();
            this.grpReview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorkOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReviewUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // grpReview
            // 
            this.grpReview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpReview.Controls.Add(this.ultraLabel2);
            this.grpReview.Controls.Add(this.txtWorkOrder);
            this.grpReview.Controls.Add(this.cboReviewUser);
            this.grpReview.Controls.Add(this.ultraLabel8);
            this.grpReview.Controls.Add(this.ultraLabel10);
            this.grpReview.Controls.Add(this.ultraLabel17);
            this.grpReview.Controls.Add(this.txtNotes);
            this.grpReview.Controls.Add(this.cboStatus);
            appearance1.Image = global::DWOS.UI.Properties.Resources.OrderReview_16;
            this.grpReview.HeaderAppearance = appearance1;
            this.grpReview.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.grpReview.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpReview.Location = new System.Drawing.Point(5, 3);
            this.grpReview.Name = "grpReview";
            this.grpReview.Size = new System.Drawing.Size(447, 323);
            this.grpReview.TabIndex = 2;
            this.grpReview.Text = "Review";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(9, 32);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(75, 15);
            this.ultraLabel2.TabIndex = 168;
            this.ultraLabel2.Text = "Work Order:";
            // 
            // txtWorkOrder
            // 
            this.txtWorkOrder.Location = new System.Drawing.Point(102, 28);
            this.txtWorkOrder.Name = "txtWorkOrder";
            this.txtWorkOrder.ReadOnly = true;
            this.txtWorkOrder.Size = new System.Drawing.Size(109, 22);
            this.txtWorkOrder.TabIndex = 0;
            ultraToolTipInfo1.ToolTipText = "The order currently being reviewed.";
            ultraToolTipInfo1.ToolTipTitle = "Work Order";
            this.ultraToolTipManager.SetUltraToolTip(this.txtWorkOrder, ultraToolTipInfo1);
            // 
            // cboReviewUser
            // 
            this.cboReviewUser.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            this.cboReviewUser.DropDownListWidth = -1;
            this.cboReviewUser.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboReviewUser.Location = new System.Drawing.Point(102, 56);
            this.cboReviewUser.Name = "cboReviewUser";
            this.cboReviewUser.Nullable = false;
            this.cboReviewUser.ReadOnly = true;
            this.cboReviewUser.Size = new System.Drawing.Size(317, 22);
            this.cboReviewUser.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboReviewUser.TabIndex = 1;
            ultraToolTipInfo2.ToolTipText = "The user that reviewed the order.";
            ultraToolTipInfo2.ToolTipTitle = "Reviewed By";
            this.ultraToolTipManager.SetUltraToolTip(this.cboReviewUser, ultraToolTipInfo2);
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(9, 60);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(82, 15);
            this.ultraLabel8.TabIndex = 117;
            this.ultraLabel8.Text = "Reviewed By:";
            // 
            // ultraLabel10
            // 
            this.ultraLabel10.AutoSize = true;
            this.ultraLabel10.Location = new System.Drawing.Point(9, 86);
            this.ultraLabel10.Name = "ultraLabel10";
            this.ultraLabel10.Size = new System.Drawing.Size(90, 15);
            this.ultraLabel10.TabIndex = 119;
            this.ultraLabel10.Text = "Review Status:";
            // 
            // ultraLabel17
            // 
            this.ultraLabel17.AutoSize = true;
            this.ultraLabel17.Location = new System.Drawing.Point(9, 114);
            this.ultraLabel17.Name = "ultraLabel17";
            this.ultraLabel17.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel17.TabIndex = 164;
            this.ultraLabel17.Text = "Notes:";
            // 
            // txtNotes
            // 
            this.txtNotes.AcceptsReturn = true;
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtNotes.AutoSize = false;
            this.txtNotes.Location = new System.Drawing.Point(102, 110);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Nullable = false;
            this.txtNotes.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(317, 207);
            this.txtNotes.TabIndex = 3;
            ultraToolTipInfo3.ToolTipText = "Add note pertaining to the review.";
            ultraToolTipInfo3.ToolTipTitle = "Notes";
            this.ultraToolTipManager.SetUltraToolTip(this.txtNotes, ultraToolTipInfo3);
            // 
            // cboStatus
            // 
            this.cboStatus.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            this.cboStatus.AutoSize = false;
            this.cboStatus.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem2.DataValue = false;
            valueListItem2.DisplayText = "Fail";
            valueListItem1.DataValue = true;
            valueListItem1.DisplayText = "Pass";
            this.cboStatus.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem2,
            valueListItem1});
            this.cboStatus.Location = new System.Drawing.Point(102, 82);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(121, 22);
            this.cboStatus.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboStatus.TabIndex = 2;
            this.cboStatus.Text = "Pass";
            ultraToolTipInfo4.ToolTipText = "Mark the review as \'Pass\' or \'Fail\'.";
            ultraToolTipInfo4.ToolTipTitle = "Review Status";
            this.ultraToolTipManager.SetUltraToolTip(this.cboStatus, ultraToolTipInfo4);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(364, 332);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "&Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(282, 332);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "&OK";
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // OrderReviewStatus
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(454, 367);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpReview);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OrderReviewStatus";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Order Review";
            ((System.ComponentModel.ISupportInitialize)(this.bsOrderReview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpReview)).EndInit();
            this.grpReview.ResumeLayout(false);
            this.grpReview.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorkOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReviewUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStatus)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource bsOrderReview;
        private Infragistics.Win.Misc.UltraGroupBox grpReview;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboReviewUser;
        private Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private Infragistics.Win.Misc.UltraLabel ultraLabel10;
        private Infragistics.Win.Misc.UltraLabel ultraLabel17;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNotes;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboStatus;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtWorkOrder;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
    }
}