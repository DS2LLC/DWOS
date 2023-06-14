namespace DWOS.Server.Admin.Wizards.InstallWizard
{
    partial class ConnectToDatabase
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
            this.txtServerName = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.btnConfigure = new Infragistics.Win.Misc.UltraButton();
            this.btnTestConnection = new Infragistics.Win.Misc.UltraButton();
            this.txtStatus = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel19 = new Infragistics.Win.Misc.UltraLabel();
            this.txtDatabaseName = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.SuspendLayout();
            // 
            // txtServerName
            // 
            this.txtServerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerName.Location = new System.Drawing.Point(91, 12);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(310, 15);
            this.txtServerName.TabIndex = 28;
            this.txtServerName.Text = "------------";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(12, 12);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(47, 15);
            this.ultraLabel3.TabIndex = 27;
            this.ultraLabel3.Text = "Server:";
            // 
            // btnConfigure
            // 
            this.btnConfigure.Location = new System.Drawing.Point(172, 75);
            this.btnConfigure.Name = "btnConfigure";
            this.btnConfigure.Size = new System.Drawing.Size(90, 23);
            this.btnConfigure.TabIndex = 29;
            this.btnConfigure.Text = "Configure...";
            this.btnConfigure.Click += new System.EventHandler(this.btnConfigure_Click);
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(76, 75);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(90, 23);
            this.btnTestConnection.TabIndex = 30;
            this.btnTestConnection.Text = "Validate";
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(91, 54);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(150, 15);
            this.txtStatus.TabIndex = 32;
            this.txtStatus.Text = "------------";
            // 
            // ultraLabel19
            // 
            this.ultraLabel19.AutoSize = true;
            this.ultraLabel19.Location = new System.Drawing.Point(12, 54);
            this.ultraLabel19.Name = "ultraLabel19";
            this.ultraLabel19.Size = new System.Drawing.Size(46, 15);
            this.ultraLabel19.TabIndex = 31;
            this.ultraLabel19.Text = "Status:";
            // 
            // txtDatabaseName
            // 
            this.txtDatabaseName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDatabaseName.Location = new System.Drawing.Point(91, 33);
            this.txtDatabaseName.Name = "txtDatabaseName";
            this.txtDatabaseName.Size = new System.Drawing.Size(310, 15);
            this.txtDatabaseName.TabIndex = 34;
            this.txtDatabaseName.Text = "------------";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(12, 33);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(63, 15);
            this.ultraLabel2.TabIndex = 33;
            this.ultraLabel2.Text = "Database:";
            // 
            // ConnectToDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtDatabaseName);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.ultraLabel19);
            this.Controls.Add(this.btnTestConnection);
            this.Controls.Add(this.btnConfigure);
            this.Controls.Add(this.txtServerName);
            this.Controls.Add(this.ultraLabel3);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ConnectToDatabase";
            this.Size = new System.Drawing.Size(404, 177);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel txtServerName;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraButton btnConfigure;
        private Infragistics.Win.Misc.UltraButton btnTestConnection;
        private Infragistics.Win.Misc.UltraLabel txtStatus;
        private Infragistics.Win.Misc.UltraLabel ultraLabel19;
        private Infragistics.Win.Misc.UltraLabel txtDatabaseName;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
    }
}
