namespace DWOS.UI.Utilities
{
    partial class UpdateInformation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateInformation));
            this.txtNotes = new Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor();
            this.FormLabel = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.lblCurrentVersion = new Infragistics.Win.Misc.UltraLabel();
            this.lblLatestVersion = new Infragistics.Win.Misc.UltraLabel();
            this.btnClose = new Infragistics.Win.Misc.UltraButton();
            this.btnUpload = new Infragistics.Win.Misc.UltraButton();
            this.SuspendLayout();
            // 
            // txtNotes
            // 
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.FontData.BoldAsString = "False";
            appearance1.FontData.ItalicAsString = "False";
            appearance1.FontData.Name = "Verdana";
            appearance1.FontData.SizeInPoints = 8.25F;
            appearance1.FontData.StrikeoutAsString = "False";
            appearance1.FontData.UnderlineAsString = "False";
            this.txtNotes.Appearance = appearance1;
            this.txtNotes.ContextMenuItems = ((Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems)(((((((((((((((((Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Cut | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Copy) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Paste) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Delete) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Undo) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Redo) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.SelectAll) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Font) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Image) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Link) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.LineAlignment) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Paragraph) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Bold) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Italics) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Underline) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.SpellingSuggestions) 
            | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Reserved)));
            this.txtNotes.Location = new System.Drawing.Point(12, 60);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ReadOnly = true;
            this.txtNotes.Size = new System.Drawing.Size(589, 334);
            this.txtNotes.TabIndex = 18;
            this.txtNotes.TextSectionBreakMode = Infragistics.Win.FormattedLinkLabel.TextSectionBreakMode.Word;
            this.txtNotes.TextSmoothingMode = Infragistics.Win.FormattedLinkLabel.TextSmoothingMode.SystemSettings;
            this.txtNotes.Value = "";
            this.txtNotes.WrapText = false;
            // 
            // FormLabel
            // 
            this.FormLabel.AutoSize = true;
            this.FormLabel.Location = new System.Drawing.Point(12, 36);
            this.FormLabel.Name = "FormLabel";
            this.FormLabel.Size = new System.Drawing.Size(117, 18);
            this.FormLabel.TabIndex = 19;
            this.FormLabel.Text = "Current Version:";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(12, 12);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(107, 18);
            this.ultraLabel1.TabIndex = 20;
            this.ultraLabel1.Text = "Latest Version:";
            // 
            // lblCurrentVersion
            // 
            this.lblCurrentVersion.AutoSize = true;
            this.lblCurrentVersion.Location = new System.Drawing.Point(135, 36);
            this.lblCurrentVersion.Name = "lblCurrentVersion";
            this.lblCurrentVersion.Size = new System.Drawing.Size(79, 18);
            this.lblCurrentVersion.TabIndex = 21;
            this.lblCurrentVersion.Text = "13.2.1.999";
            // 
            // lblLatestVersion
            // 
            this.lblLatestVersion.AutoSize = true;
            this.lblLatestVersion.Location = new System.Drawing.Point(135, 12);
            this.lblLatestVersion.Name = "lblLatestVersion";
            this.lblLatestVersion.Size = new System.Drawing.Size(62, 18);
            this.lblLatestVersion.TabIndex = 22;
            this.lblLatestVersion.Text = "13.1.0.0";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(525, 400);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(76, 23);
            this.btnClose.TabIndex = 23;
            this.btnClose.Text = "Close";
            // 
            // btnUpload
            // 
            this.btnUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpload.Location = new System.Drawing.Point(443, 400);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(76, 23);
            this.btnUpload.TabIndex = 24;
            this.btnUpload.Text = "Update";
            this.btnUpload.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // UpdateInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 435);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblLatestVersion);
            this.Controls.Add(this.lblCurrentVersion);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.FormLabel);
            this.Controls.Add(this.txtNotes);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateInformation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Updates";
            this.Load += new System.EventHandler(this.UpdateInformation_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor txtNotes;
        public Infragistics.Win.Misc.UltraLabel FormLabel;
        public Infragistics.Win.Misc.UltraLabel ultraLabel1;
        public Infragistics.Win.Misc.UltraLabel lblCurrentVersion;
        public Infragistics.Win.Misc.UltraLabel lblLatestVersion;
        private Infragistics.Win.Misc.UltraButton btnClose;
        private Infragistics.Win.Misc.UltraButton btnUpload;

    }
}