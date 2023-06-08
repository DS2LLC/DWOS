namespace DWOS.UI.Sales
{
	partial class OrderQuickFind
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Require Exact Match", Infragistics.Win.DefaultableBoolean.Default);
            this.cboFilterField = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.btnFind = new Infragistics.Win.Misc.UltraButton();
            this.txtSearchItem = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.pnlExactMatch = new System.Windows.Forms.Panel();
            this.chkExactMatch = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.cboFilterField)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            this.pnlExactMatch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkExactMatch)).BeginInit();
            this.SuspendLayout();
            // 
            // cboFilterField
            // 
            this.cboFilterField.DropDownListWidth = -1;
            this.cboFilterField.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboFilterField.Location = new System.Drawing.Point(1, 3);
            this.cboFilterField.Name = "cboFilterField";
            this.cboFilterField.Size = new System.Drawing.Size(103, 22);
            this.cboFilterField.TabIndex = 0;
            this.cboFilterField.ValueChanged += new System.EventHandler(this.cboFilterField_ValueChanged);
            // 
            // btnFind
            // 
            this.btnFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.Image = global::DWOS.UI.Properties.Resources.Filter_16;
            this.btnFind.Appearance = appearance2;
            this.btnFind.AutoSize = true;
            this.btnFind.Location = new System.Drawing.Point(256, 1);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(26, 26);
            this.btnFind.TabIndex = 2;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtSearchItem
            // 
            this.txtSearchItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchItem.Location = new System.Drawing.Point(108, 3);
            this.txtSearchItem.Name = "txtSearchItem";
            this.txtSearchItem.Size = new System.Drawing.Size(143, 22);
            this.txtSearchItem.TabIndex = 1;
            this.txtSearchItem.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearchItem_KeyPress);
            // 
            // pnlExactMatch
            // 
            this.pnlExactMatch.Controls.Add(this.chkExactMatch);
            this.pnlExactMatch.Location = new System.Drawing.Point(3, 31);
            this.pnlExactMatch.Name = "pnlExactMatch";
            this.pnlExactMatch.Size = new System.Drawing.Size(138, 18);
            this.inboxControlStyler1.SetStyleSettings(this.pnlExactMatch, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.pnlExactMatch.TabIndex = 4;
            this.pnlExactMatch.MouseLeave += new System.EventHandler(this.pnlExactMatch_MouseLeave);
            this.pnlExactMatch.MouseHover += new System.EventHandler(this.pnlExactMatch_MouseHover);
            // 
            // chkExactMatch
            // 
            this.chkExactMatch.AutoSize = true;
            this.chkExactMatch.Location = new System.Drawing.Point(0, 0);
            this.chkExactMatch.Name = "chkExactMatch";
            this.chkExactMatch.Size = new System.Drawing.Size(138, 18);
            this.chkExactMatch.TabIndex = 3;
            this.chkExactMatch.Text = "Require Exact Match";
            ultraToolTipInfo2.ToolTipTextFormatted = "If <b>checked</b>, the filter only includes complete matches.<br/>If <b>unchecked" +
    "</b> (default), the filter will include partial matches.<br/><br/>This option is" +
    " not available for some fields.";
            ultraToolTipInfo2.ToolTipTitle = "Require Exact Match";
            this.ultraToolTipManager.SetUltraToolTip(this.chkExactMatch, ultraToolTipInfo2);
            this.chkExactMatch.CheckedChanged += new System.EventHandler(this.chkExactMatch_CheckedChanged);
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // OrderQuickFind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboFilterField);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.txtSearchItem);
            this.Controls.Add(this.pnlExactMatch);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "OrderQuickFind";
            this.Size = new System.Drawing.Size(285, 52);
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Load += new System.EventHandler(this.OrderQuickFind_Load);
            this.VisibleChanged += new System.EventHandler(this.OrderQuickFind_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.cboFilterField)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            this.pnlExactMatch.ResumeLayout(false);
            this.pnlExactMatch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkExactMatch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboFilterField;
		private Infragistics.Win.Misc.UltraButton btnFind;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSearchItem;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkExactMatch;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
        private System.Windows.Forms.Panel pnlExactMatch;
    }
}
