namespace DWOS.Shared
{
    internal partial class ErrorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorDialog));
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtErrorMsg = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ErrorDialog_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.ErrorDialog_Fill_Panel.ClientArea.SuspendLayout();
            this.ErrorDialog_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(347, 190);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(61, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(209, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Oops, an error has occurred...";
            // 
            // txtErrorMsg
            // 
            this.txtErrorMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtErrorMsg.Location = new System.Drawing.Point(12, 53);
            this.txtErrorMsg.Multiline = true;
            this.txtErrorMsg.Name = "txtErrorMsg";
            this.txtErrorMsg.ReadOnly = true;
            this.txtErrorMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtErrorMsg.Size = new System.Drawing.Size(410, 131);
            this.txtErrorMsg.TabIndex = 3;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // ErrorDialog_Fill_Panel
            // 
            // 
            // ErrorDialog_Fill_Panel.ClientArea
            // 
            this.ErrorDialog_Fill_Panel.ClientArea.Controls.Add(this.pictureBox1);
            this.ErrorDialog_Fill_Panel.ClientArea.Controls.Add(this.txtErrorMsg);
            this.ErrorDialog_Fill_Panel.ClientArea.Controls.Add(this.label1);
            this.ErrorDialog_Fill_Panel.ClientArea.Controls.Add(this.btnClose);
            this.ErrorDialog_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.ErrorDialog_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ErrorDialog_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.ErrorDialog_Fill_Panel.Name = "ErrorDialog_Fill_Panel";
            this.ErrorDialog_Fill_Panel.Size = new System.Drawing.Size(429, 219);
            this.ErrorDialog_Fill_Panel.TabIndex = 0;
            // 
            // ErrorDialog
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(429, 219);
            this.Controls.Add(this.ErrorDialog_Fill_Panel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ErrorDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Error Message";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ErrorDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ErrorDialog_Fill_Panel.ClientArea.ResumeLayout(false);
            this.ErrorDialog_Fill_Panel.ClientArea.PerformLayout();
            this.ErrorDialog_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtErrorMsg;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Infragistics.Win.Misc.UltraPanel ErrorDialog_Fill_Panel;
	}
}