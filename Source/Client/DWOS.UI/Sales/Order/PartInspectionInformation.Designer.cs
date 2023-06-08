namespace DWOS.UI.Sales
{
    partial class PartInspectionInformation
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The date of the inspection.", Infragistics.Win.ToolTipImage.Default, "Inspection Date", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The user that inspected the parts.", Infragistics.Win.ToolTipImage.Default, "Inspected By", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The number of accepted parts.", Infragistics.Win.ToolTipImage.Default, "Accepted", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The number of rejected parts.", Infragistics.Win.ToolTipImage.Default, "Rejected", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Notes", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The type of part inspection", Infragistics.Win.ToolTipImage.Default, "Inspection Type", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.dteInspectionDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.cboUser = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.numAccepted = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.numRejected = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtNotes = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboInspectionType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteInspectionDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAccepted)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRejected)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboInspectionType)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.ultraLabel6);
            this.grpData.Controls.Add(this.cboInspectionType);
            this.grpData.Controls.Add(this.txtNotes);
            this.grpData.Controls.Add(this.numRejected);
            this.grpData.Controls.Add(this.numAccepted);
            this.grpData.Controls.Add(this.cboUser);
            this.grpData.Controls.Add(this.dteInspectionDate);
            this.grpData.Controls.Add(this.ultraLabel5);
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.ultraLabel3);
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.ultraLabel1);
            appearance1.Image = global::DWOS.UI.Properties.Resources.Inspection_16;
            this.grpData.HeaderAppearance = appearance1;
            this.grpData.Size = new System.Drawing.Size(446, 398);
            this.grpData.Text = "Inspection Notes";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel5, 0);
            this.grpData.Controls.SetChildIndex(this.dteInspectionDate, 0);
            this.grpData.Controls.SetChildIndex(this.cboUser, 0);
            this.grpData.Controls.SetChildIndex(this.numAccepted, 0);
            this.grpData.Controls.SetChildIndex(this.numRejected, 0);
            this.grpData.Controls.SetChildIndex(this.txtNotes, 0);
            this.grpData.Controls.SetChildIndex(this.cboInspectionType, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel6, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(422, -244);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(11, 32);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(36, 15);
            this.ultraLabel1.TabIndex = 1;
            this.ultraLabel1.Text = "Date:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(11, 88);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(84, 15);
            this.ultraLabel2.TabIndex = 2;
            this.ultraLabel2.Text = "Inspected By:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(11, 168);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel3.TabIndex = 3;
            this.ultraLabel3.Text = "Notes:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(11, 116);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(61, 15);
            this.ultraLabel4.TabIndex = 4;
            this.ultraLabel4.Text = "Accepted:";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(11, 144);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(59, 15);
            this.ultraLabel5.TabIndex = 5;
            this.ultraLabel5.Text = "Rejected:";
            // 
            // dteInspectionDate
            // 
            this.dteInspectionDate.Location = new System.Drawing.Point(117, 28);
            this.dteInspectionDate.Name = "dteInspectionDate";
            this.dteInspectionDate.ReadOnly = true;
            this.dteInspectionDate.Size = new System.Drawing.Size(144, 22);
            this.dteInspectionDate.TabIndex = 1;
            ultraToolTipInfo6.ToolTipText = "The date of the inspection.";
            ultraToolTipInfo6.ToolTipTitle = "Inspection Date";
            this.tipManager.SetUltraToolTip(this.dteInspectionDate, ultraToolTipInfo6);
            // 
            // cboUser
            // 
            this.cboUser.Location = new System.Drawing.Point(117, 84);
            this.cboUser.Name = "cboUser";
            this.cboUser.ReadOnly = true;
            this.cboUser.Size = new System.Drawing.Size(144, 22);
            this.cboUser.TabIndex = 3;
            ultraToolTipInfo5.ToolTipText = "The user that inspected the parts.";
            ultraToolTipInfo5.ToolTipTitle = "Inspected By";
            this.tipManager.SetUltraToolTip(this.cboUser, ultraToolTipInfo5);
            // 
            // numAccepted
            // 
            this.numAccepted.Location = new System.Drawing.Point(117, 112);
            this.numAccepted.Name = "numAccepted";
            this.numAccepted.ReadOnly = true;
            this.numAccepted.Size = new System.Drawing.Size(144, 22);
            this.numAccepted.TabIndex = 4;
            ultraToolTipInfo4.ToolTipText = "The number of accepted parts.";
            ultraToolTipInfo4.ToolTipTitle = "Accepted";
            this.tipManager.SetUltraToolTip(this.numAccepted, ultraToolTipInfo4);
            // 
            // numRejected
            // 
            this.numRejected.Location = new System.Drawing.Point(117, 140);
            this.numRejected.Name = "numRejected";
            this.numRejected.ReadOnly = true;
            this.numRejected.Size = new System.Drawing.Size(144, 22);
            this.numRejected.TabIndex = 5;
            ultraToolTipInfo3.ToolTipText = "The number of rejected parts.";
            ultraToolTipInfo3.ToolTipTitle = "Rejected";
            this.tipManager.SetUltraToolTip(this.numRejected, ultraToolTipInfo3);
            // 
            // txtNotes
            // 
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotes.Location = new System.Drawing.Point(117, 168);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ReadOnly = true;
            this.txtNotes.Size = new System.Drawing.Size(318, 224);
            this.txtNotes.TabIndex = 6;
            ultraToolTipInfo2.ToolTipTitle = "Notes";
            this.tipManager.SetUltraToolTip(this.txtNotes, ultraToolTipInfo2);
            // 
            // cboInspectionType
            // 
            this.cboInspectionType.Location = new System.Drawing.Point(117, 56);
            this.cboInspectionType.Name = "cboInspectionType";
            this.cboInspectionType.ReadOnly = true;
            this.cboInspectionType.Size = new System.Drawing.Size(144, 22);
            this.cboInspectionType.TabIndex = 2;
            ultraToolTipInfo1.ToolTipText = "The type of part inspection";
            ultraToolTipInfo1.ToolTipTitle = "Inspection Type";
            this.tipManager.SetUltraToolTip(this.cboInspectionType, ultraToolTipInfo1);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(11, 60);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(100, 15);
            this.ultraLabel6.TabIndex = 12;
            this.ultraLabel6.Text = "Inspection Type:";
            // 
            // PartInspectionInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "PartInspectionInformation";
            this.Size = new System.Drawing.Size(452, 404);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteInspectionDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAccepted)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRejected)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboInspectionType)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteInspectionDate;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numRejected;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numAccepted;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboUser;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNotes;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboInspectionType;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
    }
}
