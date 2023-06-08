namespace DWOS.UI.Sales
{
	partial class OrderProcessingInfo
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Work Status", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Location", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("DeleteLine");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Processing Line", Infragistics.Win.DefaultableBoolean.Default);
            this.cboWorkStatus = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.cboLocation = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.processEditor = new DWOS.UI.QA.OrderProcessEditor();
            this.lblProcessingLine = new Infragistics.Win.Misc.UltraLabel();
            this.cboProcessingLine = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboWorkStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessingLine)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.cboProcessingLine);
            this.grpData.Controls.Add(this.lblProcessingLine);
            this.grpData.Controls.Add(this.processEditor);
            this.grpData.Controls.Add(this.ultraLabel5);
            this.grpData.Controls.Add(this.cboLocation);
            this.grpData.Controls.Add(this.cboWorkStatus);
            this.grpData.Controls.Add(this.ultraLabel8);
            appearance2.Image = global::DWOS.UI.Properties.Resources.Process_16;
            this.grpData.HeaderAppearance = appearance2;
            this.grpData.MinimumSize = new System.Drawing.Size(460, 300);
            this.grpData.Size = new System.Drawing.Size(644, 470);
            this.grpData.Text = "Order Processing";
            this.grpData.Controls.SetChildIndex(this.ultraLabel8, 0);
            this.grpData.Controls.SetChildIndex(this.cboWorkStatus, 0);
            this.grpData.Controls.SetChildIndex(this.cboLocation, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel5, 0);
            this.grpData.Controls.SetChildIndex(this.processEditor, 0);
            this.grpData.Controls.SetChildIndex(this.lblProcessingLine, 0);
            this.grpData.Controls.SetChildIndex(this.cboProcessingLine, 0);
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(1656, -1103);
            // 
            // cboWorkStatus
            // 
            this.cboWorkStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboWorkStatus.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboWorkStatus.Location = new System.Drawing.Point(106, 57);
            this.cboWorkStatus.Name = "cboWorkStatus";
            this.cboWorkStatus.Size = new System.Drawing.Size(532, 22);
            this.cboWorkStatus.TabIndex = 29;
            ultraToolTipInfo3.ToolTipTextFormatted = "The current status of the order.<br/><strong>Note:</strong> This can be manually " +
    "changed if required, but generally should not be.";
            ultraToolTipInfo3.ToolTipTitle = "Work Status";
            this.tipManager.SetUltraToolTip(this.cboWorkStatus, ultraToolTipInfo3);
            this.cboWorkStatus.ValueChanged += new System.EventHandler(this.cboWorkStatus_ValueChanged);
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(3, 61);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(79, 15);
            this.ultraLabel8.TabIndex = 27;
            this.ultraLabel8.Text = "Work Status:";
            // 
            // cboLocation
            // 
            this.cboLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLocation.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboLocation.Location = new System.Drawing.Point(105, 29);
            this.cboLocation.Name = "cboLocation";
            this.cboLocation.Size = new System.Drawing.Size(533, 22);
            this.cboLocation.TabIndex = 31;
            ultraToolTipInfo2.ToolTipTextFormatted = "This box identifies which department the work order is currently in.<br/><strong>" +
    "Note:</strong> This can be manually changed if required, but generally should no" +
    "t be.";
            ultraToolTipInfo2.ToolTipTitle = "Location";
            this.tipManager.SetUltraToolTip(this.cboLocation, ultraToolTipInfo2);
            this.cboLocation.ValueChanged += new System.EventHandler(this.cboLocation_ValueChanged);
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(6, 33);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(57, 15);
            this.ultraLabel5.TabIndex = 30;
            this.ultraLabel5.Text = "Location:";
            // 
            // processEditor
            // 
            this.processEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.processEditor.DisplayCOCColumn = true;
            this.processEditor.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processEditor.IsInRework = false;
            this.processEditor.Location = new System.Drawing.Point(105, 114);
            this.processEditor.Margin = new System.Windows.Forms.Padding(4);
            this.processEditor.Name = "processEditor";
            this.processEditor.OrderRow = null;
            this.processEditor.Size = new System.Drawing.Size(533, 349);
            this.processEditor.TabIndex = 90;
            this.processEditor.ViewOnly = false;
            // 
            // lblProcessingLine
            // 
            this.lblProcessingLine.AutoSize = true;
            this.lblProcessingLine.Location = new System.Drawing.Point(3, 89);
            this.lblProcessingLine.Name = "lblProcessingLine";
            this.lblProcessingLine.Size = new System.Drawing.Size(97, 15);
            this.lblProcessingLine.TabIndex = 91;
            this.lblProcessingLine.Text = "Processing Line:";
            // 
            // cboProcessingLine
            // 
            this.cboProcessingLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            editorButton1.Appearance = appearance1;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton1.Key = "DeleteLine";
            this.cboProcessingLine.ButtonsLeft.Add(editorButton1);
            this.cboProcessingLine.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboProcessingLine.Location = new System.Drawing.Point(106, 85);
            this.cboProcessingLine.Name = "cboProcessingLine";
            this.cboProcessingLine.Size = new System.Drawing.Size(532, 22);
            this.cboProcessingLine.TabIndex = 92;
            ultraToolTipInfo1.ToolTipTextFormatted = "The current processing line of the order.";
            ultraToolTipInfo1.ToolTipTitle = "Processing Line";
            this.tipManager.SetUltraToolTip(this.cboProcessingLine, ultraToolTipInfo1);
            this.cboProcessingLine.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.cboProcessingLine_EditorButtonClick);
            // 
            // OrderProcessingInfo
            // 
            this.AutoScroll = true;
            this.Name = "OrderProcessingInfo";
            this.Size = new System.Drawing.Size(650, 476);
            this.Resize += new System.EventHandler(this.OrderProcessingInfo_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboWorkStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProcessingLine)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboWorkStatus;
		private Infragistics.Win.Misc.UltraLabel ultraLabel8;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboLocation;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private QA.OrderProcessEditor processEditor;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboProcessingLine;
        private Infragistics.Win.Misc.UltraLabel lblProcessingLine;
    }
}
