namespace DWOS.UI.Admin.SettingsPanels
{
    partial class SettingsCOCData
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Template for default COC data.", Infragistics.Win.ToolTipImage.Default, "COC Data Template", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("List of tokens that are included in the template.", Infragistics.Win.ToolTipImage.Default, "Tokens", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtTemplate = new Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor();
            this.txtTokens = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTokens)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.txtTemplate);
            this.ultraGroupBox1.Controls.Add(this.txtTokens);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(559, 404);
            this.ultraGroupBox1.TabIndex = 0;
            this.ultraGroupBox1.Text = "Final Inspection Data";
            // 
            // txtTemplate
            // 
            this.txtTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.FontData.BoldAsString = "False";
            appearance1.FontData.ItalicAsString = "False";
            appearance1.FontData.Name = "Verdana";
            appearance1.FontData.SizeInPoints = 8.25F;
            appearance1.FontData.StrikeoutAsString = "False";
            appearance1.FontData.UnderlineAsString = "False";
            this.txtTemplate.Appearance = appearance1;
            this.txtTemplate.ContextMenuItems = ((Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems)(((((((((((((((((Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Cut | Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.Copy) 
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
            this.txtTemplate.Location = new System.Drawing.Point(6, 48);
            this.txtTemplate.Name = "txtTemplate";
            this.txtTemplate.Size = new System.Drawing.Size(547, 322);
            this.txtTemplate.TabIndex = 0;
            this.txtTemplate.TextSectionBreakMode = Infragistics.Win.FormattedLinkLabel.TextSectionBreakMode.Word;
            this.txtTemplate.TextSmoothingMode = Infragistics.Win.FormattedLinkLabel.TextSmoothingMode.SystemSettings;
            ultraToolTipInfo1.ToolTipText = "Template for default COC data.";
            ultraToolTipInfo1.ToolTipTitle = "COC Data Template";
            this.ultraToolTipManager.SetUltraToolTip(this.txtTemplate, ultraToolTipInfo1);
            this.txtTemplate.Value = "";
            // 
            // txtTokens
            // 
            this.txtTokens.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTokens.Location = new System.Drawing.Point(62, 376);
            this.txtTokens.Name = "txtTokens";
            this.txtTokens.NullText = "Tokens";
            appearance2.FontData.ItalicAsString = "True";
            appearance2.ForeColor = System.Drawing.Color.Silver;
            this.txtTokens.NullTextAppearance = appearance2;
            this.txtTokens.ReadOnly = true;
            this.txtTokens.Size = new System.Drawing.Size(491, 22);
            this.txtTokens.TabIndex = 1;
            ultraToolTipInfo2.ToolTipText = "List of tokens that are included in the template.";
            ultraToolTipInfo2.ToolTipTitle = "Tokens";
            this.ultraToolTipManager.SetUltraToolTip(this.txtTokens, ultraToolTipInfo2);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(6, 380);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(50, 15);
            this.ultraLabel2.TabIndex = 11;
            this.ultraLabel2.Text = "Tokens:";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 27);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(122, 15);
            this.ultraLabel1.TabIndex = 10;
            this.ultraLabel1.Text = "COC Data Template:";
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // SettingsCOCData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(460, 400);
            this.Name = "SettingsCOCData";
            this.Size = new System.Drawing.Size(559, 404);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTokens)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTokens;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor txtTemplate;
    }
}
