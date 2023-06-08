namespace DWOS.UI.Admin.SettingsPanels
{
    partial class SettingsHoldNotification
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            DWOS.UI.Utilities.HtmlEditor.DefaultRenderSettings defaultRenderSettings1 = new DWOS.UI.Utilities.HtmlEditor.DefaultRenderSettings();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsHoldNotification));
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtTokens = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.htmlEditor = new DWOS.UI.Utilities.HtmlEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTokens)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.txtTokens);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Controls.Add(this.htmlEditor);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(668, 403);
            this.ultraGroupBox1.TabIndex = 39;
            this.ultraGroupBox1.Text = "Hold Notification";
            // 
            // txtTokens
            // 
            this.txtTokens.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTokens.Location = new System.Drawing.Point(69, 375);
            this.txtTokens.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtTokens.Name = "txtTokens";
            this.txtTokens.NullText = "Tokens";
            appearance1.FontData.ItalicAsString = "True";
            appearance1.ForeColor = System.Drawing.Color.Silver;
            this.txtTokens.NullTextAppearance = appearance1;
            this.txtTokens.ReadOnly = true;
            this.txtTokens.Size = new System.Drawing.Size(592, 21);
            this.txtTokens.TabIndex = 176;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(11, 379);
            this.ultraLabel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(44, 14);
            this.ultraLabel2.TabIndex = 177;
            this.ultraLabel2.Text = "Tokens:";
            // 
            // htmlEditor
            // 
            this.htmlEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.htmlEditor.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.htmlEditor.Location = new System.Drawing.Point(9, 49);
            this.htmlEditor.MinimumSize = new System.Drawing.Size(640, 320);
            this.htmlEditor.Name = "htmlEditor";
            this.htmlEditor.RenderSettings = defaultRenderSettings1;
            this.htmlEditor.Size = new System.Drawing.Size(653, 320);
            this.htmlEditor.TabIndex = 175;
            this.htmlEditor.Tokens = ((System.Collections.Generic.List<DWOS.UI.Utilities.HtmlEditor.Token>)(resources.GetObject("htmlEditor.Tokens")));
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(9, 28);
            this.ultraLabel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(86, 14);
            this.ultraLabel1.TabIndex = 48;
            this.ultraLabel1.Text = "Email Template:";
            // 
            // SettingsHoldNotification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox1);
            this.Name = "SettingsHoldNotification";
            this.Size = new System.Drawing.Size(668, 403);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTokens)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTokens;
        public Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Utilities.HtmlEditor htmlEditor;
        public Infragistics.Win.Misc.UltraLabel ultraLabel1;
    }
}
