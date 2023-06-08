namespace DWOS.UI.Utilities
{
	partial class HelpLink
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
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            this.lblHelp = new Infragistics.Win.Misc.UltraLabel();
            this.SuspendLayout();
            // 
            // lblHelp
            // 
            appearance3.FontData.UnderlineAsString = "True";
            appearance3.ForeColor = System.Drawing.Color.Blue;
            this.lblHelp.Appearance = appearance3;
            this.lblHelp.AutoSize = true;
            this.lblHelp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblHelp.Location = new System.Drawing.Point(0, 0);
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(30, 15);
            this.lblHelp.TabIndex = 0;
            this.lblHelp.Text = "Help";
            this.lblHelp.UseAppStyling = false;
            this.lblHelp.Click += new System.EventHandler(this.lblHelp_Click);
            // 
            // HelpLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lblHelp);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "HelpLink";
            this.Size = new System.Drawing.Size(33, 16);
            this.Load += new System.EventHandler(this.HelpLink_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private Infragistics.Win.Misc.UltraLabel lblHelp;

    }
}
