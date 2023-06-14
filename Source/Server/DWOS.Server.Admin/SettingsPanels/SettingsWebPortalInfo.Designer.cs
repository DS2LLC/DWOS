namespace DWOS.Server.Admin.SettingsPanels
{
    partial class SettingsWebPortalInfo
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
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.teTagline = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboSkinStyle = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.lblSkinStyle = new Infragistics.Win.Misc.UltraLabel();
            this.chkShowLateOrderNotification = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teTagline)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSkinStyle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShowLateOrderNotification)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Controls.Add(this.chkShowLateOrderNotification);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Controls.Add(this.teTagline);
            this.ultraGroupBox1.Controls.Add(this.cboSkinStyle);
            this.ultraGroupBox1.Controls.Add(this.lblSkinStyle);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(321, 322);
            this.ultraGroupBox1.TabIndex = 0;
            this.ultraGroupBox1.Text = "Web Portal Settings";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Location = new System.Drawing.Point(-1, 136);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(68, 58);
            this.ultraLabel2.TabIndex = 4;
            this.ultraLabel2.Text = "Tag Line: (HTML)";
            // 
            // teTagline
            // 
            this.teTagline.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.teTagline.Location = new System.Drawing.Point(79, 133);
            this.teTagline.Multiline = true;
            this.teTagline.Name = "teTagline";
            this.teTagline.Size = new System.Drawing.Size(232, 172);
            this.teTagline.TabIndex = 3;
            // 
            // cboSkinStyle
            // 
            this.cboSkinStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSkinStyle.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem1.DataValue = "0";
            valueListItem1.DisplayText = "RubberBlack";
            valueListItem2.DataValue = "1";
            valueListItem2.DisplayText = "Trendy";
            this.cboSkinStyle.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.cboSkinStyle.Location = new System.Drawing.Point(79, 104);
            this.cboSkinStyle.Name = "cboSkinStyle";
            this.cboSkinStyle.Size = new System.Drawing.Size(232, 22);
            this.cboSkinStyle.TabIndex = 2;
            // 
            // lblSkinStyle
            // 
            this.lblSkinStyle.AutoSize = true;
            this.lblSkinStyle.Location = new System.Drawing.Point(0, 108);
            this.lblSkinStyle.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lblSkinStyle.Name = "lblSkinStyle";
            this.lblSkinStyle.Size = new System.Drawing.Size(67, 15);
            this.lblSkinStyle.TabIndex = 0;
            this.lblSkinStyle.Text = "Skin Style:";
            // 
            // chkShowLateOrderNotification
            // 
            this.chkShowLateOrderNotification.AutoSize = true;
            this.chkShowLateOrderNotification.Location = new System.Drawing.Point(6, 48);
            this.chkShowLateOrderNotification.Name = "chkShowLateOrderNotification";
            this.chkShowLateOrderNotification.Size = new System.Drawing.Size(227, 18);
            this.chkShowLateOrderNotification.TabIndex = 1;
            this.chkShowLateOrderNotification.Text = "Show Late Order Notification Option";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel1.Location = new System.Drawing.Point(6, 27);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(77, 15);
            this.ultraLabel1.TabIndex = 5;
            this.ultraLabel1.Text = "v2 Options:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel3.Location = new System.Drawing.Point(6, 87);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(77, 15);
            this.ultraLabel3.TabIndex = 6;
            this.ultraLabel3.Text = "v1 Options:";
            // 
            // SettingsWebPortalInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "SettingsWebPortalInfo";
            this.Size = new System.Drawing.Size(321, 322);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teTagline)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSkinStyle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShowLateOrderNotification)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        public Infragistics.Win.Misc.UltraLabel lblSkinStyle;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboSkinStyle;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor teTagline;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkShowLateOrderNotification;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
    }
}
