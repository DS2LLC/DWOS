namespace DWOS.UI.QA.QIManagerPanels
{
	partial class QIInspectionInfo
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
			if(disposing && (components != null))
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The name of the inspection.", Infragistics.Win.ToolTipImage.Default, "Inspection Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The reference of the test.", Infragistics.Win.ToolTipImage.Default, "Test Reference", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The defined requirements of the test.", Infragistics.Win.ToolTipImage.Default, "Test Requirements", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Determines if the inspection is active or not.", Infragistics.Win.ToolTipImage.Default, "Active", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The revision of the inspection.", Infragistics.Win.ToolTipImage.Default, "Revision", Infragistics.Win.DefaultableBoolean.Default);
            this.txtName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtTestRef = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtTestReq = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel10 = new Infragistics.Win.Misc.UltraLabel();
            this.docLinkManagerProcess = new DWOS.UI.Documents.DocumentLinkManager();
            this.chkActive = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.cboRevision = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTestRef)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTestReq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRevision)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Bottom = 5;
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 15;
            this.grpData.Controls.Add(this.cboRevision);
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.chkActive);
            this.grpData.Controls.Add(this.ultraLabel10);
            this.grpData.Controls.Add(this.docLinkManagerProcess);
            this.grpData.Controls.Add(this.ultraLabel1);
            this.grpData.Controls.Add(this.txtTestReq);
            this.grpData.Controls.Add(this.txtName);
            this.grpData.Controls.Add(this.ultraLabel3);
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.txtTestRef);
            appearance1.Image = global::DWOS.UI.Properties.Resources.Inspection_16;
            this.grpData.HeaderAppearance = appearance1;
            this.grpData.Size = new System.Drawing.Size(462, 349);
            this.grpData.Text = "Inspection Information";
            this.grpData.WrapText = false;
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.txtTestRef, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.grpData.Controls.SetChildIndex(this.txtName, 0);
            this.grpData.Controls.SetChildIndex(this.txtTestReq, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.docLinkManagerProcess, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel10, 0);
            this.grpData.Controls.SetChildIndex(this.chkActive, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            this.grpData.Controls.SetChildIndex(this.cboRevision, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(515, -1265);
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(128, 28);
            this.txtName.MaxLength = 50;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(261, 22);
            this.txtName.TabIndex = 1;
            ultraToolTipInfo4.ToolTipText = "The name of the inspection.";
            ultraToolTipInfo4.ToolTipTitle = "Inspection Name";
            this.tipManager.SetUltraToolTip(this.txtName, ultraToolTipInfo4);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(6, 88);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(94, 15);
            this.ultraLabel2.TabIndex = 18;
            this.ultraLabel2.Text = "Test Reference:";
            // 
            // txtTestRef
            // 
            this.txtTestRef.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTestRef.Location = new System.Drawing.Point(128, 84);
            this.txtTestRef.MaxLength = 50;
            this.txtTestRef.Name = "txtTestRef";
            this.txtTestRef.Size = new System.Drawing.Size(323, 22);
            this.txtTestRef.TabIndex = 4;
            ultraToolTipInfo5.ToolTipText = "The reference of the test.";
            ultraToolTipInfo5.ToolTipTitle = "Test Reference";
            this.tipManager.SetUltraToolTip(this.txtTestRef, ultraToolTipInfo5);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(6, 32);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel3.TabIndex = 15;
            this.ultraLabel3.Text = "Name:";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 116);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(116, 15);
            this.ultraLabel1.TabIndex = 20;
            this.ultraLabel1.Text = "Test Requirements:";
            // 
            // txtTestReq
            // 
            this.txtTestReq.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTestReq.Location = new System.Drawing.Point(128, 112);
            this.txtTestReq.MaxLength = 255;
            this.txtTestReq.Name = "txtTestReq";
            this.txtTestReq.Size = new System.Drawing.Size(323, 22);
            this.txtTestReq.TabIndex = 5;
            ultraToolTipInfo3.ToolTipText = "The defined requirements of the test.";
            ultraToolTipInfo3.ToolTipTitle = "Test Requirements";
            this.tipManager.SetUltraToolTip(this.txtTestReq, ultraToolTipInfo3);
            // 
            // ultraLabel10
            // 
            this.ultraLabel10.AutoSize = true;
            this.ultraLabel10.Location = new System.Drawing.Point(6, 140);
            this.ultraLabel10.Name = "ultraLabel10";
            this.ultraLabel10.Size = new System.Drawing.Size(101, 15);
            this.ultraLabel10.TabIndex = 48;
            this.ultraLabel10.Text = "Document Links:";
            // 
            // docLinkManagerProcess
            // 
            this.docLinkManagerProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.docLinkManagerProcess.BackColor = System.Drawing.Color.Transparent;
            this.docLinkManagerProcess.CurrentRow = null;
            this.docLinkManagerProcess.DocumentLinkTable = null;
            this.docLinkManagerProcess.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.docLinkManagerProcess.LinkType = DWOS.UI.Documents.LinkType.Process;
            this.docLinkManagerProcess.Location = new System.Drawing.Point(128, 140);
            this.docLinkManagerProcess.Name = "docLinkManagerProcess";
            this.docLinkManagerProcess.ParentTable = null;
            this.docLinkManagerProcess.Size = new System.Drawing.Size(323, 198);
            this.docLinkManagerProcess.TabIndex = 6;
            this.docLinkManagerProcess.TableKeyColumn = null;
            // 
            // chkActive
            // 
            this.chkActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkActive.AutoSize = true;
            this.chkActive.Location = new System.Drawing.Point(395, 28);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(56, 18);
            this.chkActive.TabIndex = 2;
            this.chkActive.Text = "Active";
            ultraToolTipInfo2.ToolTipText = "Determines if the inspection is active or not.";
            ultraToolTipInfo2.ToolTipTitle = "Active";
            this.tipManager.SetUltraToolTip(this.chkActive, ultraToolTipInfo2);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(6, 60);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(57, 15);
            this.ultraLabel4.TabIndex = 49;
            this.ultraLabel4.Text = "Revision:";
            // 
            // cboRevision
            // 
            this.cboRevision.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboRevision.Location = new System.Drawing.Point(128, 56);
            this.cboRevision.MaxLength = 50;
            this.cboRevision.Name = "cboRevision";
            this.cboRevision.Size = new System.Drawing.Size(323, 22);
            this.cboRevision.TabIndex = 3;
            this.cboRevision.Text = "<None>";
            ultraToolTipInfo1.ToolTipText = "The revision of the inspection.";
            ultraToolTipInfo1.ToolTipTitle = "Revision";
            this.tipManager.SetUltraToolTip(this.cboRevision, ultraToolTipInfo1);
            // 
            // QIInspectionInfo
            // 
            this.Name = "QIInspectionInfo";
            this.Size = new System.Drawing.Size(468, 355);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTestRef)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTestReq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRevision)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtName;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTestRef;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTestReq;
        private Infragistics.Win.Misc.UltraLabel ultraLabel10;
        private Documents.DocumentLinkManager docLinkManagerProcess;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkActive;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboRevision;
    }
}
