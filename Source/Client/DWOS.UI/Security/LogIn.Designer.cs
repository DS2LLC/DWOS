namespace DWOS.UI.Utilities
{
	partial class LogIn
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogIn));
			this.btnCancel = new Infragistics.Win.Misc.UltraButton();
			this.btnOK = new Infragistics.Win.Misc.UltraButton();
			this.FormLabel = new Infragistics.Win.Misc.UltraLabel();
			this.txtPin = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
			this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
			((System.ComponentModel.ISupportInitialize)(this.txtPin)).BeginInit();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(170, 73);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(91, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(74, 73);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(89, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			// 
			// FormLabel
			// 
			this.FormLabel.AutoSize = true;
			this.FormLabel.Location = new System.Drawing.Point(96, 33);
			this.FormLabel.Name = "FormLabel";
			this.FormLabel.Size = new System.Drawing.Size(26, 15);
			this.FormLabel.TabIndex = 31;
			this.FormLabel.Text = "Pin:";
			// 
			// txtPin
			// 
			this.txtPin.Location = new System.Drawing.Point(128, 29);
			this.txtPin.MaxLength = 50;
			this.txtPin.Name = "txtPin";
			this.txtPin.PasswordChar = '*';
			this.txtPin.Size = new System.Drawing.Size(117, 22);
			this.txtPin.TabIndex = 0;
			// 
			// ultraPictureBox1
			// 
			this.ultraPictureBox1.BorderShadowColor = System.Drawing.Color.Empty;
			this.ultraPictureBox1.Image = ((object)(resources.GetObject("ultraPictureBox1.Image")));
			this.ultraPictureBox1.Location = new System.Drawing.Point(12, 12);
			this.ultraPictureBox1.Name = "ultraPictureBox1";
			this.ultraPictureBox1.Size = new System.Drawing.Size(55, 55);
			this.ultraPictureBox1.TabIndex = 35;
			this.ultraPictureBox1.UseAppStyling = false;
			// 
			// LogIn
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(273, 108);
			this.Controls.Add(this.ultraPictureBox1);
			this.Controls.Add(this.txtPin);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.FormLabel);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LogIn";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Log In";
			this.TopMost = true;
			this.Shown += new System.EventHandler(this.LogIn_Shown);
			((System.ComponentModel.ISupportInitialize)(this.txtPin)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		public Infragistics.Win.Misc.UltraLabel FormLabel;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPin;
		private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
	}
}