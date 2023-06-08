namespace DWOS.UI.Admin.SettingsPanels
{
    partial class SettingsReportNotification
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
			DisposeCodeBehind();
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("AddTemplate");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The currently selected template.", Infragistics.Win.ToolTipImage.Default, "Template", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("List of tokens that are included in the template.", Infragistics.Win.ToolTipImage.Default, "Tokens", Infragistics.Win.DefaultableBoolean.Default);
            DWOS.UI.Utilities.HtmlEditor.DefaultRenderSettings defaultRenderSettings1 = new DWOS.UI.Utilities.HtmlEditor.DefaultRenderSettings();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboTemplate = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtTokens = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.htmlEditor = new DWOS.UI.Utilities.HtmlEditor();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTokens)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.cboTemplate);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Controls.Add(this.txtTokens);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox1.Controls.Add(this.htmlEditor);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(464, 355);
            this.ultraGroupBox1.TabIndex = 38;
            this.ultraGroupBox1.Text = "Report Notification Information";
            // 
            // cboTemplate
            // 
            this.cboTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::DWOS.UI.Properties.Resources.Add_16;
            editorButton1.Appearance = appearance1;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton1.Key = "AddTemplate";
            this.cboTemplate.ButtonsRight.Add(editorButton1);
            this.cboTemplate.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboTemplate.Location = new System.Drawing.Point(76, 35);
            this.cboTemplate.Name = "cboTemplate";
            this.cboTemplate.Size = new System.Drawing.Size(382, 22);
            this.cboTemplate.TabIndex = 0;
            ultraToolTipInfo1.ToolTipText = "The currently selected template.";
            ultraToolTipInfo1.ToolTipTitle = "Template";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboTemplate, ultraToolTipInfo1);
            this.cboTemplate.ValueChanged += new System.EventHandler(this.cboTemplate_ValueChanged);
            this.cboTemplate.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.cboTemplate_EditorButtonClick);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(9, 39);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(62, 15);
            this.ultraLabel1.TabIndex = 182;
            this.ultraLabel1.Text = "Template:";
            // 
            // txtTokens
            // 
            this.txtTokens.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTokens.Location = new System.Drawing.Point(91, 327);
            this.txtTokens.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtTokens.Name = "txtTokens";
            this.txtTokens.NullText = "Tokens";
            appearance2.FontData.ItalicAsString = "True";
            appearance2.ForeColor = System.Drawing.Color.Silver;
            this.txtTokens.NullTextAppearance = appearance2;
            this.txtTokens.ReadOnly = true;
            this.txtTokens.Size = new System.Drawing.Size(366, 22);
            this.txtTokens.TabIndex = 3;
            ultraToolTipInfo2.ToolTipText = "List of tokens that are included in the template.";
            ultraToolTipInfo2.ToolTipTitle = "Tokens";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtTokens, ultraToolTipInfo2);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(8, 331);
            this.ultraLabel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(50, 15);
            this.ultraLabel3.TabIndex = 181;
            this.ultraLabel3.Text = "Tokens:";
            // 
            // htmlEditor
            // 
            this.htmlEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.htmlEditor.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.htmlEditor.Location = new System.Drawing.Point(9, 63);
            this.htmlEditor.Name = "htmlEditor";
            this.htmlEditor.RenderSettings = defaultRenderSettings1;
            this.htmlEditor.Size = new System.Drawing.Size(449, 258);
            this.htmlEditor.TabIndex = 2;
            this.htmlEditor.Tokens = null;
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // SettingsReportNotification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(460, 355);
            this.Name = "SettingsReportNotification";
            this.Size = new System.Drawing.Size(464, 355);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTokens)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Utilities.HtmlEditor htmlEditor;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTokens;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        public Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboTemplate;
    }
}
