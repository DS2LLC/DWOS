namespace DWOS.UI.Admin.LabelPanels
{
    partial class LabelInfoPanel
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
            this.ultraLabel16 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.cboCustomer = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.cboLabelType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.picPreview = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLabelType)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.picPreview);
            this.grpData.Controls.Add(this.cboLabelType);
            this.grpData.Controls.Add(this.ultraLabel16);
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.cboCustomer);
            this.grpData.Controls.Add(this.ultraLabel6);
            this.grpData.Size = new System.Drawing.Size(467, 301);
            this.grpData.Text = "Label";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel6, 0);
            this.grpData.Controls.SetChildIndex(this.cboCustomer, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel16, 0);
            this.grpData.Controls.SetChildIndex(this.cboLabelType, 0);
            this.grpData.Controls.SetChildIndex(this.picPreview, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(564, -1806);
            // 
            // ultraLabel16
            // 
            this.ultraLabel16.AutoSize = true;
            this.ultraLabel16.Location = new System.Drawing.Point(6, 32);
            this.ultraLabel16.Name = "ultraLabel16";
            this.ultraLabel16.Size = new System.Drawing.Size(64, 15);
            this.ultraLabel16.TabIndex = 47;
            this.ultraLabel16.Text = "Customer:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(6, 60);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(71, 15);
            this.ultraLabel4.TabIndex = 46;
            this.ultraLabel4.Text = "Label Type:";
            // 
            // cboCustomer
            // 
            this.cboCustomer.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCustomer.Location = new System.Drawing.Point(99, 28);
            this.cboCustomer.MaxLength = 50;
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.NullText = "- Company Default -";
            this.cboCustomer.Size = new System.Drawing.Size(221, 22);
            this.cboCustomer.TabIndex = 34;
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(6, 88);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(88, 15);
            this.ultraLabel6.TabIndex = 41;
            this.ultraLabel6.Text = "Label Preview:";
            // 
            // cboLabelType
            // 
            this.cboLabelType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboLabelType.Location = new System.Drawing.Point(99, 56);
            this.cboLabelType.MaxLength = 50;
            this.cboLabelType.Name = "cboLabelType";
            this.cboLabelType.ReadOnly = true;
            this.cboLabelType.Size = new System.Drawing.Size(221, 22);
            this.cboLabelType.TabIndex = 48;
            // 
            // picPreview
            // 
            this.picPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picPreview.BorderShadowColor = System.Drawing.Color.Empty;
            this.picPreview.BorderStyle = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.picPreview.Location = new System.Drawing.Point(11, 109);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(445, 186);
            this.picPreview.TabIndex = 49;
            // 
            // LabelInfoPanel
            // 
            this.Name = "LabelInfoPanel";
            this.Size = new System.Drawing.Size(473, 307);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLabelType)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel16;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboCustomer;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picPreview;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboLabelType;
	}
}
