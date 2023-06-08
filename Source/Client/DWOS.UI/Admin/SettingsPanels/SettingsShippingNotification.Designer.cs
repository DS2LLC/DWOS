namespace DWOS.UI.Admin.SettingsPanels
{
	partial class SettingsShippingNotification
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("List of tokens that are included in the template.", Infragistics.Win.ToolTipImage.Default, "Tokens", Infragistics.Win.DefaultableBoolean.Default);
            DWOS.UI.Utilities.HtmlEditor.DefaultRenderSettings defaultRenderSettings1 = new DWOS.UI.Utilities.HtmlEditor.DefaultRenderSettings();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtTokens = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.htmlEditor = new DWOS.UI.Utilities.HtmlEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
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
            this.ultraGroupBox1.Size = new System.Drawing.Size(464, 493);
            this.ultraGroupBox1.TabIndex = 38;
            this.ultraGroupBox1.Text = "Shipping Information";
            // 
            // txtTokens
            // 
            this.txtTokens.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTokens.Location = new System.Drawing.Point(69, 465);
            this.txtTokens.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtTokens.Name = "txtTokens";
            this.txtTokens.NullText = "Tokens";
            appearance1.FontData.ItalicAsString = "True";
            appearance1.ForeColor = System.Drawing.Color.Silver;
            this.txtTokens.NullTextAppearance = appearance1;
            this.txtTokens.ReadOnly = true;
            this.txtTokens.Size = new System.Drawing.Size(388, 22);
            this.txtTokens.TabIndex = 176;
            ultraToolTipInfo1.ToolTipText = "List of tokens that are included in the template.";
            ultraToolTipInfo1.ToolTipTitle = "Tokens";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtTokens, ultraToolTipInfo1);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(11, 469);
            this.ultraLabel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(50, 15);
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
            this.htmlEditor.Name = "htmlEditor";
            this.htmlEditor.RenderSettings = defaultRenderSettings1;
            this.htmlEditor.Size = new System.Drawing.Size(449, 410);
            this.htmlEditor.TabIndex = 175;
            this.htmlEditor.Tokens = null;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(9, 28);
            this.ultraLabel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(151, 15);
            this.ultraLabel1.TabIndex = 48;
            this.ultraLabel1.Text = "Shipping Email Template:";
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // SettingsShippingNotification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(460, 355);
            this.Name = "SettingsShippingNotification";
            this.Size = new System.Drawing.Size(464, 493);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTokens)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        public Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Utilities.HtmlEditor htmlEditor;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTokens;
        public Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
	}
}
