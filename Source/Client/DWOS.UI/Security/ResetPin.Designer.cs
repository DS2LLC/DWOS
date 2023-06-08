namespace DWOS.UI.Utilities
{
	partial class ResetPin
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResetPin));
			Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The new pin to be changed to.", Infragistics.Win.ToolTipImage.Default, "New Pin", Infragistics.Win.DefaultableBoolean.Default);
			Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Retype new pin to verify it is correct.", Infragistics.Win.ToolTipImage.Default, "Retype New Pin", Infragistics.Win.DefaultableBoolean.Default);
			Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Current pin required to change pin.", Infragistics.Win.ToolTipImage.Default, "Original Pin", Infragistics.Win.DefaultableBoolean.Default);
			this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
			this.txtNew = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
			this.btnCancel = new Infragistics.Win.Misc.UltraButton();
			this.btnOK = new Infragistics.Win.Misc.UltraButton();
			this.FormLabel = new Infragistics.Win.Misc.UltraLabel();
			this.txtNew2 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
			this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
			this.txtOriginal = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
			this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
			this.lblUserName = new Infragistics.Win.Misc.UltraLabel();
			this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
			this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
			((System.ComponentModel.ISupportInitialize)(this.txtNew)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtNew2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtOriginal)).BeginInit();
			this.SuspendLayout();
			// 
			// ultraPictureBox1
			// 
			this.ultraPictureBox1.BorderShadowColor = System.Drawing.Color.Empty;
			this.ultraPictureBox1.Image = ((object)(resources.GetObject("ultraPictureBox1.Image")));
			this.ultraPictureBox1.Location = new System.Drawing.Point(11, 37);
			this.ultraPictureBox1.Name = "ultraPictureBox1";
			this.ultraPictureBox1.Size = new System.Drawing.Size(55, 55);
			this.ultraPictureBox1.TabIndex = 40;
			this.ultraPictureBox1.UseAppStyling = false;
			// 
			// txtNew
			// 
			this.txtNew.Location = new System.Drawing.Point(164, 61);
			this.txtNew.MaxLength = 50;
			this.txtNew.Name = "txtNew";
			this.txtNew.PasswordChar = '*';
			this.txtNew.Size = new System.Drawing.Size(117, 22);
			this.txtNew.TabIndex = 1;
			ultraToolTipInfo3.ToolTipText = "The new pin to be changed to.";
			ultraToolTipInfo3.ToolTipTitle = "New Pin";
			this.tipManager.SetUltraToolTip(this.txtNew, ultraToolTipInfo3);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(190, 152);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(91, 23);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(94, 152);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(89, 23);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// FormLabel
			// 
			this.FormLabel.AutoSize = true;
			this.FormLabel.Location = new System.Drawing.Point(83, 65);
			this.FormLabel.Name = "FormLabel";
			this.FormLabel.Size = new System.Drawing.Size(55, 15);
			this.FormLabel.TabIndex = 36;
			this.FormLabel.Text = "New Pin:";
			// 
			// txtNew2
			// 
			this.txtNew2.Location = new System.Drawing.Point(164, 89);
			this.txtNew2.MaxLength = 50;
			this.txtNew2.Name = "txtNew2";
			this.txtNew2.PasswordChar = '*';
			this.txtNew2.Size = new System.Drawing.Size(117, 22);
			this.txtNew2.TabIndex = 2;
			ultraToolTipInfo2.ToolTipText = "Retype new pin to verify it is correct.";
			ultraToolTipInfo2.ToolTipTitle = "Retype New Pin";
			this.tipManager.SetUltraToolTip(this.txtNew2, ultraToolTipInfo2);
			// 
			// ultraLabel1
			// 
			this.ultraLabel1.AutoSize = true;
			this.ultraLabel1.Location = new System.Drawing.Point(83, 93);
			this.ultraLabel1.Name = "ultraLabel1";
			this.ultraLabel1.Size = new System.Drawing.Size(70, 15);
			this.ultraLabel1.TabIndex = 41;
			this.ultraLabel1.Text = "Retype Pin:";
			// 
			// txtOriginal
			// 
			this.txtOriginal.Location = new System.Drawing.Point(164, 33);
			this.txtOriginal.MaxLength = 50;
			this.txtOriginal.Name = "txtOriginal";
			this.txtOriginal.PasswordChar = '*';
			this.txtOriginal.Size = new System.Drawing.Size(117, 22);
			this.txtOriginal.TabIndex = 0;
			ultraToolTipInfo1.ToolTipText = "Current pin required to change pin.";
			ultraToolTipInfo1.ToolTipTitle = "Original Pin";
			this.tipManager.SetUltraToolTip(this.txtOriginal, ultraToolTipInfo1);
			// 
			// ultraLabel2
			// 
			this.ultraLabel2.AutoSize = true;
			this.ultraLabel2.Location = new System.Drawing.Point(83, 37);
			this.ultraLabel2.Name = "ultraLabel2";
			this.ultraLabel2.Size = new System.Drawing.Size(75, 15);
			this.ultraLabel2.TabIndex = 43;
			this.ultraLabel2.Text = "Original Pin:";
			// 
			// lblUserName
			// 
			this.lblUserName.Location = new System.Drawing.Point(83, 12);
			this.lblUserName.Name = "lblUserName";
			this.lblUserName.Size = new System.Drawing.Size(198, 15);
			this.lblUserName.TabIndex = 45;
			this.lblUserName.Text = "User Name";
			// 
			// ultraLabel3
			// 
			this.ultraLabel3.Location = new System.Drawing.Point(26, 114);
			this.ultraLabel3.Name = "ultraLabel3";
			this.ultraLabel3.Size = new System.Drawing.Size(255, 34);
			this.ultraLabel3.TabIndex = 46;
			this.ultraLabel3.Text = "* New pin must contain only numbers and be at least 6 digits.";
			// 
			// tipManager
			// 
			this.tipManager.ContainingControl = this;
			// 
			// ResetPin
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(293, 187);
			this.Controls.Add(this.ultraLabel3);
			this.Controls.Add(this.lblUserName);
			this.Controls.Add(this.txtOriginal);
			this.Controls.Add(this.ultraLabel2);
			this.Controls.Add(this.txtNew2);
			this.Controls.Add(this.ultraLabel1);
			this.Controls.Add(this.ultraPictureBox1);
			this.Controls.Add(this.txtNew);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.FormLabel);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ResetPin";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Reset Pin";
			this.Load += new System.EventHandler(this.ResetPin_Load);
			((System.ComponentModel.ISupportInitialize)(this.txtNew)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtNew2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtOriginal)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNew;
		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		public Infragistics.Win.Misc.UltraLabel FormLabel;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNew2;
		public Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtOriginal;
		public Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.Misc.UltraLabel lblUserName;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
		protected Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
	}
}