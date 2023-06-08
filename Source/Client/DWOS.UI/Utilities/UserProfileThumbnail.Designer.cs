namespace DWOS.UI.Utilities
{
	partial class UserProfileThumbnail
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Edit Profile", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "User Picture", Infragistics.Win.DefaultableBoolean.Default);
            this.txtDepartment = new Infragistics.Win.Misc.UltraLabel();
            this.txtTitle = new Infragistics.Win.Misc.UltraLabel();
            this.txtName = new Infragistics.Win.Misc.UltraLabel();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.picEdit = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.picUserImage = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.SuspendLayout();
            // 
            // txtDepartment
            // 
            this.txtDepartment.AutoSize = true;
            this.txtDepartment.Location = new System.Drawing.Point(100, 42);
            this.txtDepartment.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.txtDepartment.Name = "txtDepartment";
            this.txtDepartment.Size = new System.Drawing.Size(72, 15);
            this.txtDepartment.TabIndex = 58;
            this.txtDepartment.Text = "Department";
            // 
            // txtTitle
            // 
            this.txtTitle.AutoSize = true;
            this.txtTitle.Location = new System.Drawing.Point(100, 23);
            this.txtTitle.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(29, 15);
            this.txtTitle.TabIndex = 57;
            this.txtTitle.Text = "Title";
            // 
            // txtName
            // 
            this.txtName.AutoSize = true;
            this.txtName.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(100, 3);
            this.txtName.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(41, 17);
            this.txtName.TabIndex = 59;
            this.txtName.Text = "Name";
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // picEdit
            // 
            this.picEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picEdit.Appearance = appearance1;
            this.picEdit.BorderShadowColor = System.Drawing.Color.Empty;
            this.picEdit.DefaultImage = global::DWOS.UI.Properties.Resources.Pencil_32;
            this.picEdit.Location = new System.Drawing.Point(176, 61);
            this.picEdit.Name = "picEdit";
            this.picEdit.ScaleImage = Infragistics.Win.ScaleImage.Always;
            this.picEdit.Size = new System.Drawing.Size(18, 18);
            this.picEdit.TabIndex = 62;
            ultraToolTipInfo1.ToolTipTextFormatted = "Click to edit your profile.";
            ultraToolTipInfo1.ToolTipTitle = "Edit Profile";
            this.tipManager.SetUltraToolTip(this.picEdit, ultraToolTipInfo1);
            this.picEdit.UseAppStyling = false;
            this.picEdit.Click += new System.EventHandler(this.ultraPictureBox1_Click);
            // 
            // picUserImage
            // 
            appearance2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picUserImage.Appearance = appearance2;
            this.picUserImage.BorderShadowColor = System.Drawing.Color.Empty;
            this.picUserImage.DefaultImage = global::DWOS.UI.Properties.Resources.nopicture_thumb;
            this.picUserImage.Dock = System.Windows.Forms.DockStyle.Left;
            this.picUserImage.Location = new System.Drawing.Point(0, 0);
            this.picUserImage.Name = "picUserImage";
            this.picUserImage.ScaleImage = Infragistics.Win.ScaleImage.Always;
            this.picUserImage.Size = new System.Drawing.Size(95, 80);
            this.picUserImage.TabIndex = 60;
            ultraToolTipInfo2.ToolTipTextFormatted = "Click to change your picture. Optimal picture size should be 95 X 80 pixels.";
            ultraToolTipInfo2.ToolTipTitle = "User Picture";
            this.tipManager.SetUltraToolTip(this.picUserImage, ultraToolTipInfo2);
            this.picUserImage.UseAppStyling = false;
            this.picUserImage.Click += new System.EventHandler(this.picUserImage_Click);
            // 
            // UserProfileThumbnail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.picEdit);
            this.Controls.Add(this.picUserImage);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtDepartment);
            this.Controls.Add(this.txtTitle);
            this.Enabled = false;
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "UserProfileThumbnail";
            this.Size = new System.Drawing.Size(195, 80);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraPictureBox picUserImage;
		private Infragistics.Win.Misc.UltraLabel txtDepartment;
		private Infragistics.Win.Misc.UltraLabel txtTitle;
        private Infragistics.Win.Misc.UltraLabel txtName;
		protected Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picEdit;
	}
}
