namespace DWOS.UI.Admin.LabelPanels
{
    partial class LabelTypePanel
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
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.picPreview = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboLabelType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLabelType)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.cboLabelType);
            this.grpData.Controls.Add(this.ultraLabel1);
            this.grpData.Controls.Add(this.picPreview);
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.ultraLabel6);
            this.grpData.Size = new System.Drawing.Size(467, 301);
            this.grpData.Text = "Default Label";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel6, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            this.grpData.Controls.SetChildIndex(this.picPreview, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.cboLabelType, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(544, -1548);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(11, 55);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(71, 15);
            this.ultraLabel4.TabIndex = 46;
            this.ultraLabel4.Text = "Label Type:";
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(11, 82);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(88, 15);
            this.ultraLabel6.TabIndex = 41;
            this.ultraLabel6.Text = "Label Preview:";
            // 
            // picPreview
            // 
            this.picPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picPreview.BorderShadowColor = System.Drawing.Color.Empty;
            this.picPreview.BorderStyle = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.picPreview.Location = new System.Drawing.Point(11, 100);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(445, 195);
            this.picPreview.TabIndex = 49;
            // 
            // ultraLabel1
            // 
            appearance1.FontData.BoldAsString = "True";
            appearance1.ForeColor = System.Drawing.Color.Firebrick;
            this.ultraLabel1.Appearance = appearance1;
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(11, 25);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(404, 15);
            this.ultraLabel1.TabIndex = 51;
            this.ultraLabel1.Text = "This is the default label used if a customer label is not defined.";
            this.ultraLabel1.UseAppStyling = false;
            // 
            // cboLabelType
            // 
            this.cboLabelType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboLabelType.Location = new System.Drawing.Point(88, 51);
            this.cboLabelType.MaxLength = 50;
            this.cboLabelType.Name = "cboLabelType";
            this.cboLabelType.ReadOnly = true;
            this.cboLabelType.Size = new System.Drawing.Size(237, 22);
            this.cboLabelType.TabIndex = 52;
            // 
            // LabelTypePanel
            // 
            this.Name = "LabelTypePanel";
            this.Size = new System.Drawing.Size(473, 307);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLabelType)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picPreview;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboLabelType;
	}
}
