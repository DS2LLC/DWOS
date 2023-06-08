namespace DWOS.UI.Admin.SettingsPanels
{
	partial class SettingsAuthenticationInfo
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.numMinPin = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboAuthType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinPin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAuthType)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(9, 35);
            this.ultraLabel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(123, 15);
            this.ultraLabel2.TabIndex = 34;
            this.ultraLabel2.Text = "Authentication Type:";
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox1.Controls.Add(this.numMinPin);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Controls.Add(this.cboAuthType);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(323, 300);
            this.ultraGroupBox1.TabIndex = 36;
            this.ultraGroupBox1.Text = "System Authentication";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.ForeColor = System.Drawing.Color.Red;
            this.ultraLabel3.Appearance = appearance1;
            this.ultraLabel3.Location = new System.Drawing.Point(11, 95);
            this.ultraLabel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(276, 40);
            this.ultraLabel3.TabIndex = 38;
            this.ultraLabel3.Text = "* - Setting can only be set in the server administrator tool.";
            // 
            // numMinPin
            // 
            this.numMinPin.Location = new System.Drawing.Point(150, 59);
            this.numMinPin.MaxValue = 9;
            this.numMinPin.MinValue = 1;
            this.numMinPin.Name = "numMinPin";
            this.numMinPin.ReadOnly = true;
            this.numMinPin.Size = new System.Drawing.Size(100, 22);
            this.numMinPin.TabIndex = 37;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(9, 63);
            this.ultraLabel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(125, 15);
            this.ultraLabel1.TabIndex = 36;
            this.ultraLabel1.Text = "Pin Minimum Length:";
            // 
            // cboAuthType
            // 
            this.cboAuthType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem1.DataValue = "0";
            valueListItem1.DisplayText = "Pin";
            valueListItem2.DataValue = "1";
            valueListItem2.DisplayText = "Smartcard";
            valueListItem3.DataValue = "2";
            valueListItem3.DisplayText = "PinAndSmartcard";
            valueListItem4.DataValue = "3";
            valueListItem4.DisplayText = "PinOrSmartcard";
            this.cboAuthType.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3,
            valueListItem4});
            this.cboAuthType.Location = new System.Drawing.Point(150, 31);
            this.cboAuthType.Name = "cboAuthType";
            this.cboAuthType.ReadOnly = true;
            this.cboAuthType.Size = new System.Drawing.Size(137, 22);
            this.cboAuthType.TabIndex = 35;
            // 
            // SettingsAuthenticationInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "SettingsAuthenticationInfo";
            this.Size = new System.Drawing.Size(323, 300);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinPin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAuthType)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		public Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cboAuthType;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numMinPin;
        public Infragistics.Win.Misc.UltraLabel ultraLabel1;
        public Infragistics.Win.Misc.UltraLabel ultraLabel3;

	}
}
