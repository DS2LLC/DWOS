namespace DWOS.UI.Utilities
{
    partial class CustomerTicket
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
            if (disposing && (components != null))
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomerTicket));
            this.teSubject = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.teBody = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnSubmit = new Infragistics.Win.Misc.UltraButton();
            this.teFrom = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraTextEditor1 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.teSubject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teBody)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor1)).BeginInit();
            this.SuspendLayout();
            // 
            // teSubject
            // 
            this.teSubject.Location = new System.Drawing.Point(105, 80);
            this.teSubject.Name = "teSubject";
            this.teSubject.NullText = "Subject";
            appearance1.AlphaLevel = ((short)(50));
            this.teSubject.NullTextAppearance = appearance1;
            this.teSubject.Size = new System.Drawing.Size(464, 22);
            this.teSubject.TabIndex = 1;
            // 
            // teBody
            // 
            this.teBody.Location = new System.Drawing.Point(105, 107);
            this.teBody.Multiline = true;
            this.teBody.Name = "teBody";
            this.teBody.NullText = "Message";
            appearance2.AlphaLevel = ((short)(50));
            this.teBody.NullTextAppearance = appearance2;
            this.teBody.Size = new System.Drawing.Size(464, 246);
            this.teBody.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(479, 359);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(385, 359);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(87, 23);
            this.btnSubmit.TabIndex = 3;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // teFrom
            // 
            this.teFrom.Location = new System.Drawing.Point(105, 53);
            this.teFrom.Name = "teFrom";
            this.teFrom.NullText = "Email";
            appearance3.AlphaLevel = ((short)(50));
            this.teFrom.NullTextAppearance = appearance3;
            this.teFrom.Size = new System.Drawing.Size(462, 22);
            this.teFrom.TabIndex = 0;
            // 
            // ultraTextEditor1
            // 
            this.ultraTextEditor1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance4.BackColor = System.Drawing.SystemColors.Control;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
            appearance4.BorderAlpha = Infragistics.Win.Alpha.Transparent;
            this.ultraTextEditor1.Appearance = appearance4;
            this.ultraTextEditor1.BackColor = System.Drawing.Color.Transparent;
            this.ultraTextEditor1.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.ultraTextEditor1.Enabled = false;
            this.ultraTextEditor1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraTextEditor1.Location = new System.Drawing.Point(83, 13);
            this.ultraTextEditor1.Multiline = true;
            this.ultraTextEditor1.Name = "ultraTextEditor1";
            this.ultraTextEditor1.ReadOnly = true;
            this.ultraTextEditor1.Size = new System.Drawing.Size(485, 34);
            this.ultraTextEditor1.TabIndex = 6;
            this.ultraTextEditor1.Text = "Support Requests are similar to ideas but the submissions remain between you and " +
    "our team.";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(15, 57);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(39, 15);
            this.ultraLabel1.TabIndex = 8;
            this.ultraLabel1.Text = "From:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(15, 84);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(52, 15);
            this.ultraLabel2.TabIndex = 9;
            this.ultraLabel2.Text = "Subject:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(15, 110);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(58, 15);
            this.ultraLabel3.TabIndex = 10;
            this.ultraLabel3.Text = "Message:";
            // 
            // ultraPictureBox1
            // 
            this.ultraPictureBox1.AutoSize = true;
            this.ultraPictureBox1.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox1.Image = ((object)(resources.GetObject("ultraPictureBox1.Image")));
            this.ultraPictureBox1.Location = new System.Drawing.Point(15, 3);
            this.ultraPictureBox1.Name = "ultraPictureBox1";
            this.ultraPictureBox1.Size = new System.Drawing.Size(48, 48);
            this.ultraPictureBox1.TabIndex = 0;
            this.ultraPictureBox1.UseAppStyling = false;
            // 
            // CustomerTicket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(582, 390);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.ultraPictureBox1);
            this.Controls.Add(this.ultraTextEditor1);
            this.Controls.Add(this.teFrom);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.teBody);
            this.Controls.Add(this.teSubject);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomerTicket";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Submit Customer Ticket";
            this.Load += new System.EventHandler(this.CustomerTicket_Load);
            ((System.ComponentModel.ISupportInitialize)(this.teSubject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teBody)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor teSubject;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor teBody;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnSubmit;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor teFrom;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor ultraTextEditor1;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
    }
}