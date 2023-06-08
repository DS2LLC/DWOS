namespace DWOS.UI.Admin.SettingsPanels
{
    partial class SettingsShipping
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
            DWOS.UI.Utilities.HtmlEditor.DefaultRenderSettings defaultRenderSettings1 = new DWOS.UI.Utilities.HtmlEditor.DefaultRenderSettings();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsShipping));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The logo to use at the top of a statement of repairs.", Infragistics.Win.ToolTipImage.Default, "Statement of Repairs Logo", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("List of tokens that are included in the template.", Infragistics.Win.ToolTipImage.Default, "Tokens", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.htmlRepair = new DWOS.UI.Utilities.HtmlEditor();
            this.btnRepairLogo = new Infragistics.Win.Misc.UltraButton();
            this.picRepairLogo = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtRepairTokens = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRepairTokens)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.htmlRepair);
            this.ultraGroupBox1.Controls.Add(this.btnRepairLogo);
            this.ultraGroupBox1.Controls.Add(this.picRepairLogo);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox1.Controls.Add(this.txtRepairTokens);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(423, 500);
            this.ultraGroupBox1.TabIndex = 0;
            this.ultraGroupBox1.Text = "Shipping";
            // 
            // htmlRepair
            // 
            this.htmlRepair.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.htmlRepair.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.htmlRepair.Location = new System.Drawing.Point(7, 57);
            this.htmlRepair.Name = "htmlRepair";
            this.htmlRepair.RenderSettings = defaultRenderSettings1;
            this.htmlRepair.Size = new System.Drawing.Size(409, 251);
            this.htmlRepair.TabIndex = 7;
            this.htmlRepair.Tokens = ((System.Collections.Generic.List<DWOS.UI.Utilities.HtmlEditor.Token>)(resources.GetObject("htmlRepair.Tokens")));
            // 
            // btnRepairLogo
            // 
            this.btnRepairLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRepairLogo.Location = new System.Drawing.Point(326, 471);
            this.btnRepairLogo.Name = "btnRepairLogo";
            this.btnRepairLogo.Size = new System.Drawing.Size(90, 23);
            this.btnRepairLogo.TabIndex = 6;
            this.btnRepairLogo.Text = "Browse...";
            this.btnRepairLogo.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // picRepairLogo
            // 
            this.picRepairLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picRepairLogo.BorderShadowColor = System.Drawing.Color.Empty;
            this.picRepairLogo.BorderStyle = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.picRepairLogo.Location = new System.Drawing.Point(7, 376);
            this.picRepairLogo.Name = "picRepairLogo";
            this.picRepairLogo.Size = new System.Drawing.Size(313, 118);
            this.picRepairLogo.TabIndex = 5;
            ultraToolTipInfo1.ToolTipText = "The logo to use at the top of a statement of repairs.";
            ultraToolTipInfo1.ToolTipTitle = "Statement of Repairs Logo";
            this.ultraToolTipManager.SetUltraToolTip(this.picRepairLogo, ultraToolTipInfo1);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(7, 355);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(161, 15);
            this.ultraLabel3.TabIndex = 4;
            this.ultraLabel3.Text = "Statement of Repairs Logo:";
            // 
            // txtRepairTokens
            // 
            this.txtRepairTokens.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRepairTokens.Location = new System.Drawing.Point(59, 314);
            this.txtRepairTokens.Name = "txtRepairTokens";
            this.txtRepairTokens.NullText = "Tokens";
            appearance1.FontData.ItalicAsString = "True";
            appearance1.ForeColor = System.Drawing.Color.Silver;
            this.txtRepairTokens.NullTextAppearance = appearance1;
            this.txtRepairTokens.ReadOnly = true;
            this.txtRepairTokens.Size = new System.Drawing.Size(357, 22);
            this.txtRepairTokens.TabIndex = 3;
            ultraToolTipInfo2.ToolTipText = "List of tokens that are included in the template.";
            ultraToolTipInfo2.ToolTipTitle = "Tokens";
            this.ultraToolTipManager.SetUltraToolTip(this.txtRepairTokens, ultraToolTipInfo2);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(7, 318);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(44, 15);
            this.ultraLabel2.TabIndex = 2;
            this.ultraLabel2.Text = "Token:";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(7, 36);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(130, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Statement of Repairs:";
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // SettingsShipping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(423, 500);
            this.Name = "SettingsShipping";
            this.Size = new System.Drawing.Size(423, 500);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRepairTokens)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtRepairTokens;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraButton btnRepairLogo;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picRepairLogo;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
        private Utilities.HtmlEditor htmlRepair;
    }
}
