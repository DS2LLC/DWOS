namespace DWOS.Server.Admin.Tasks
{
    partial class DatabaseConfiguration
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
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseConfiguration));
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboAuthentication = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.cboServerName = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtUserName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtPassword = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboDatabaseName = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.btnTestConnection = new Infragistics.Win.Misc.UltraButton();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.numRetryCount = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.numRetryInterval = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            ((System.ComponentModel.ISupportInitialize)(this.cboAuthentication)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboServerName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboDatabaseName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetryCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetryInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(11, 24);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(84, 15);
            this.ultraLabel1.TabIndex = 4;
            this.ultraLabel1.Text = "Server Name:";
            // 
            // cboAuthentication
            // 
            this.cboAuthentication.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem4.DataValue = "Windows";
            valueListItem4.DisplayText = "Windows Authentication";
            valueListItem3.DataValue = "SQL";
            valueListItem3.DisplayText = "SQL Server Authentication";
            this.cboAuthentication.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem4,
            valueListItem3});
            this.cboAuthentication.Location = new System.Drawing.Point(145, 48);
            this.cboAuthentication.Name = "cboAuthentication";
            this.cboAuthentication.Size = new System.Drawing.Size(283, 22);
            this.cboAuthentication.TabIndex = 6;
            this.cboAuthentication.SelectionChanged += new System.EventHandler(this.cboAuthentication_SelectionChanged);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(11, 52);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(92, 15);
            this.ultraLabel2.TabIndex = 7;
            this.ultraLabel2.Text = "Authentication:";
            // 
            // cboServerName
            // 
            this.cboServerName.Location = new System.Drawing.Point(145, 20);
            this.cboServerName.Name = "cboServerName";
            this.cboServerName.Size = new System.Drawing.Size(283, 22);
            this.cboServerName.TabIndex = 5;
            this.cboServerName.SelectionChanged += new System.EventHandler(this.cboServerName_SelectionChanged);
            this.cboServerName.BeforeDropDown += new System.ComponentModel.CancelEventHandler(this.cboServerName_BeforeDropDown);
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(145, 76);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(186, 22);
            this.txtUserName.TabIndex = 9;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(145, 104);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(186, 22);
            this.txtPassword.TabIndex = 10;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(30, 108);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(63, 15);
            this.ultraLabel3.TabIndex = 12;
            this.ultraLabel3.Text = "Password:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(30, 80);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(40, 15);
            this.ultraLabel4.TabIndex = 11;
            this.ultraLabel4.Text = "Login:";
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox1.Controls.Add(this.numRetryInterval);
            this.ultraGroupBox1.Controls.Add(this.numRetryCount);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel7);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel6);
            this.ultraGroupBox1.Controls.Add(this.cboDatabaseName);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel5);
            this.ultraGroupBox1.Controls.Add(this.btnTestConnection);
            this.ultraGroupBox1.Controls.Add(this.cboServerName);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel4);
            this.ultraGroupBox1.Controls.Add(this.txtPassword);
            this.ultraGroupBox1.Controls.Add(this.cboAuthentication);
            this.ultraGroupBox1.Controls.Add(this.txtUserName);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Location = new System.Drawing.Point(4, 12);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(443, 248);
            this.ultraGroupBox1.TabIndex = 13;
            this.ultraGroupBox1.Text = "Connection";
            // 
            // cboDatabaseName
            // 
            this.cboDatabaseName.Location = new System.Drawing.Point(145, 132);
            this.cboDatabaseName.Name = "cboDatabaseName";
            this.cboDatabaseName.Size = new System.Drawing.Size(283, 22);
            this.cboDatabaseName.TabIndex = 15;
            this.cboDatabaseName.BeforeDropDown += new System.ComponentModel.CancelEventHandler(this.cboDatabaseName_BeforeDropDown);
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(11, 136);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(100, 15);
            this.ultraLabel5.TabIndex = 14;
            this.ultraLabel5.Text = "Database Name:";
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(310, 216);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(118, 23);
            this.btnTestConnection.TabIndex = 18;
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(372, 267);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Close";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(291, 267);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 19;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(11, 164);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(78, 15);
            this.ultraLabel6.TabIndex = 16;
            this.ultraLabel6.Text = "Retry Count:";
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(11, 192);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(121, 15);
            this.ultraLabel7.TabIndex = 17;
            this.ultraLabel7.Text = "Retry Interval (sec):";
            // 
            // numRetryCount
            // 
            this.numRetryCount.Location = new System.Drawing.Point(145, 160);
            this.numRetryCount.MaxValue = 255;
            this.numRetryCount.MinValue = 0;
            this.numRetryCount.Name = "numRetryCount";
            this.numRetryCount.Size = new System.Drawing.Size(283, 22);
            this.numRetryCount.TabIndex = 16;
            // 
            // numRetryInterval
            // 
            this.numRetryInterval.Location = new System.Drawing.Point(145, 188);
            this.numRetryInterval.MaxValue = 60;
            this.numRetryInterval.MinValue = 1;
            this.numRetryInterval.Name = "numRetryInterval";
            this.numRetryInterval.Size = new System.Drawing.Size(283, 22);
            this.numRetryInterval.TabIndex = 17;
            // 
            // DatabaseConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 302);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.ultraGroupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DatabaseConfiguration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Database Connection";
            this.Load += new System.EventHandler(this.DatabaseConfiguration_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cboAuthentication)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboServerName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboDatabaseName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetryCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetryInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboAuthentication;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboServerName;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtUserName;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPassword;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraButton btnTestConnection;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboDatabaseName;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numRetryInterval;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numRetryCount;
    }
}