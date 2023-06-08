namespace DWOS.UI.ShippingRec
{
	partial class ShippingEndOfDay
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShippingEndOfDay));
			this.btnRun = new Infragistics.Win.Misc.UltraButton();
			this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
			this.ultraProgressBar1 = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
			this.ultraProgressBar2 = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
			this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
			this.SuspendLayout();
			// 
			// btnRun
			// 
			this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRun.Location = new System.Drawing.Point(390, 75);
			this.btnRun.Name = "btnRun";
			this.btnRun.Size = new System.Drawing.Size(85, 23);
			this.btnRun.TabIndex = 2;
			this.btnRun.Text = "Run";
			// 
			// ultraLabel1
			// 
			this.ultraLabel1.AutoSize = true;
			this.ultraLabel1.Location = new System.Drawing.Point(41, 16);
			this.ultraLabel1.Name = "ultraLabel1";
			this.ultraLabel1.Size = new System.Drawing.Size(110, 15);
			this.ultraLabel1.TabIndex = 3;
			this.ultraLabel1.Text = "Email Notifications";
			// 
			// ultraProgressBar1
			// 
			this.ultraProgressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ultraProgressBar1.Location = new System.Drawing.Point(180, 12);
			this.ultraProgressBar1.Name = "ultraProgressBar1";
			this.ultraProgressBar1.Size = new System.Drawing.Size(295, 23);
			this.ultraProgressBar1.TabIndex = 4;
			this.ultraProgressBar1.Text = "[Formatted]";
			// 
			// ultraProgressBar2
			// 
			this.ultraProgressBar2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ultraProgressBar2.Location = new System.Drawing.Point(180, 41);
			this.ultraProgressBar2.Name = "ultraProgressBar2";
			this.ultraProgressBar2.Size = new System.Drawing.Size(295, 23);
			this.ultraProgressBar2.TabIndex = 6;
			this.ultraProgressBar2.Text = "[Formatted]";
			// 
			// ultraLabel2
			// 
			this.ultraLabel2.AutoSize = true;
			this.ultraLabel2.Location = new System.Drawing.Point(40, 45);
			this.ultraLabel2.Name = "ultraLabel2";
			this.ultraLabel2.Size = new System.Drawing.Size(127, 15);
			this.ultraLabel2.TabIndex = 5;
			this.ultraLabel2.Text = "Customer Summaries";
			// 
			// ShippingEndOfDay
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(487, 110);
			this.Controls.Add(this.ultraProgressBar2);
			this.Controls.Add(this.ultraLabel2);
			this.Controls.Add(this.ultraProgressBar1);
			this.Controls.Add(this.ultraLabel1);
			this.Controls.Add(this.btnRun);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ShippingEndOfDay";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Shipping End Of Day";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnRun;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinProgressBar.UltraProgressBar ultraProgressBar1;
		private Infragistics.Win.UltraWinProgressBar.UltraProgressBar ultraProgressBar2;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
	}
}