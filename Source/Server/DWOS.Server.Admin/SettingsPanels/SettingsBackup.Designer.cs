namespace DWOS.Server.Admin.SettingsPanels
{
    partial class SettingsBackup
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
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.numTransactionLogIntervalHours = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.chkBackupTransactionLog = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkNotify = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkCleanup = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.pnlCleanup = new Infragistics.Win.Misc.UltraPanel();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.numTransactionLogDays = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.numFilesToKeep = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.btnSaveTo = new Infragistics.Win.Misc.UltraButton();
            this.txtBackupDir = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.dteBackupTime = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.chkSunday = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkWednesday = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkTuesday = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkMonday = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkSaturday = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkThursday = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkFriday = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkEnabled = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTransactionLogIntervalHours)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkBackupTransactionLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotify)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCleanup)).BeginInit();
            this.pnlCleanup.ClientArea.SuspendLayout();
            this.pnlCleanup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTransactionLogDays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFilesToKeep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackupDir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBackupTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkSunday)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkWednesday)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkTuesday)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMonday)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSaturday)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkThursday)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFriday)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEnabled)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.ultraLabel8);
            this.ultraGroupBox1.Controls.Add(this.numTransactionLogIntervalHours);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel7);
            this.ultraGroupBox1.Controls.Add(this.chkBackupTransactionLog);
            this.ultraGroupBox1.Controls.Add(this.chkNotify);
            this.ultraGroupBox1.Controls.Add(this.chkCleanup);
            this.ultraGroupBox1.Controls.Add(this.pnlCleanup);
            this.ultraGroupBox1.Controls.Add(this.btnSaveTo);
            this.ultraGroupBox1.Controls.Add(this.txtBackupDir);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Controls.Add(this.dteBackupTime);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Controls.Add(this.ultraGroupBox2);
            this.ultraGroupBox1.Controls.Add(this.chkEnabled);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dashed;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(342, 427);
            this.ultraGroupBox1.TabIndex = 0;
            this.ultraGroupBox1.Text = "Backup Settings";
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(242, 282);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(51, 15);
            this.ultraLabel8.TabIndex = 15;
            this.ultraLabel8.Text = "hour(s).";
            // 
            // numTransactionLogIntervalHours
            // 
            this.numTransactionLogIntervalHours.Location = new System.Drawing.Point(182, 278);
            this.numTransactionLogIntervalHours.MaskInput = "nn";
            this.numTransactionLogIntervalHours.MaxValue = 23;
            this.numTransactionLogIntervalHours.MinValue = 1;
            this.numTransactionLogIntervalHours.Name = "numTransactionLogIntervalHours";
            this.numTransactionLogIntervalHours.Size = new System.Drawing.Size(54, 22);
            this.numTransactionLogIntervalHours.TabIndex = 7;
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(6, 282);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(170, 15);
            this.ultraLabel7.TabIndex = 10;
            this.ultraLabel7.Text = "Backup transaction log every";
            // 
            // chkBackupTransactionLog
            // 
            this.chkBackupTransactionLog.AutoSize = true;
            this.chkBackupTransactionLog.Location = new System.Drawing.Point(6, 254);
            this.chkBackupTransactionLog.Name = "chkBackupTransactionLog";
            this.chkBackupTransactionLog.Size = new System.Drawing.Size(151, 18);
            this.chkBackupTransactionLog.TabIndex = 6;
            this.chkBackupTransactionLog.Text = "Backup transaction log";
            this.chkBackupTransactionLog.CheckedValueChanged += new System.EventHandler(this.chkBackupTransactionLog_CheckedValueChanged);
            // 
            // chkNotify
            // 
            this.chkNotify.AutoSize = true;
            this.chkNotify.Location = new System.Drawing.Point(6, 398);
            this.chkNotify.Name = "chkNotify";
            this.chkNotify.Size = new System.Drawing.Size(294, 18);
            this.chkNotify.TabIndex = 10;
            this.chkNotify.Text = "Notify administrator after successful full backup";
            // 
            // chkCleanup
            // 
            this.chkCleanup.AutoSize = true;
            this.chkCleanup.Location = new System.Drawing.Point(6, 305);
            this.chkCleanup.Name = "chkCleanup";
            this.chkCleanup.Size = new System.Drawing.Size(174, 18);
            this.chkCleanup.TabIndex = 8;
            this.chkCleanup.Text = "Cleanup Files After Backup";
            this.chkCleanup.CheckedChanged += new System.EventHandler(this.chkCleanup_CheckedChanged);
            // 
            // pnlCleanup
            // 
            // 
            // pnlCleanup.ClientArea
            // 
            this.pnlCleanup.ClientArea.Controls.Add(this.ultraLabel6);
            this.pnlCleanup.ClientArea.Controls.Add(this.numTransactionLogDays);
            this.pnlCleanup.ClientArea.Controls.Add(this.ultraLabel5);
            this.pnlCleanup.ClientArea.Controls.Add(this.ultraLabel4);
            this.pnlCleanup.ClientArea.Controls.Add(this.ultraLabel3);
            this.pnlCleanup.ClientArea.Controls.Add(this.numFilesToKeep);
            this.pnlCleanup.Location = new System.Drawing.Point(6, 329);
            this.pnlCleanup.Name = "pnlCleanup";
            this.pnlCleanup.Size = new System.Drawing.Size(329, 63);
            this.pnlCleanup.TabIndex = 9;
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(254, 31);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(35, 15);
            this.ultraLabel6.TabIndex = 13;
            this.ultraLabel6.Text = "days.";
            // 
            // numTransactionLogDays
            // 
            this.numTransactionLogDays.Location = new System.Drawing.Point(194, 27);
            this.numTransactionLogDays.MaskInput = "nn";
            this.numTransactionLogDays.MaxValue = 99;
            this.numTransactionLogDays.MinValue = 1;
            this.numTransactionLogDays.Name = "numTransactionLogDays";
            this.numTransactionLogDays.Size = new System.Drawing.Size(54, 22);
            this.numTransactionLogDays.TabIndex = 12;
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(0, 31);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(191, 15);
            this.ultraLabel5.TabIndex = 11;
            this.ultraLabel5.Text = "Keep transaction log backups for";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(99, 7);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(97, 15);
            this.ultraLabel4.TabIndex = 10;
            this.ultraLabel4.Text = "full backup files.";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(0, 7);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(33, 15);
            this.ultraLabel3.TabIndex = 9;
            this.ultraLabel3.Text = "Keep";
            // 
            // numFilesToKeep
            // 
            this.numFilesToKeep.Location = new System.Drawing.Point(39, 3);
            this.numFilesToKeep.MaskInput = "nn";
            this.numFilesToKeep.MaxValue = 99;
            this.numFilesToKeep.MinValue = 1;
            this.numFilesToKeep.Name = "numFilesToKeep";
            this.numFilesToKeep.Size = new System.Drawing.Size(54, 22);
            this.numFilesToKeep.TabIndex = 8;
            // 
            // btnSaveTo
            // 
            this.btnSaveTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveTo.Location = new System.Drawing.Point(261, 214);
            this.btnSaveTo.Name = "btnSaveTo";
            this.btnSaveTo.Size = new System.Drawing.Size(75, 23);
            this.btnSaveTo.TabIndex = 5;
            this.btnSaveTo.Text = "Browse...";
            this.btnSaveTo.Click += new System.EventHandler(this.btnSaveTo_Click);
            // 
            // txtBackupDir
            // 
            this.txtBackupDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBackupDir.Location = new System.Drawing.Point(80, 214);
            this.txtBackupDir.Name = "txtBackupDir";
            this.txtBackupDir.ReadOnly = true;
            this.txtBackupDir.Size = new System.Drawing.Size(175, 22);
            this.txtBackupDir.TabIndex = 4;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(7, 218);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(55, 15);
            this.ultraLabel2.TabIndex = 4;
            this.ultraLabel2.Text = "Save To:";
            // 
            // dteBackupTime
            // 
            this.dteBackupTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dteBackupTime.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.dteBackupTime.Location = new System.Drawing.Point(80, 186);
            this.dteBackupTime.MaskInput = "{time}";
            this.dteBackupTime.Name = "dteBackupTime";
            this.dteBackupTime.Size = new System.Drawing.Size(256, 22);
            this.dteBackupTime.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.dteBackupTime.TabIndex = 3;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(7, 190);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(67, 15);
            this.ultraLabel1.TabIndex = 2;
            this.ultraLabel1.Text = "Backup At:";
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox2.Controls.Add(this.chkSunday);
            this.ultraGroupBox2.Controls.Add(this.chkWednesday);
            this.ultraGroupBox2.Controls.Add(this.chkTuesday);
            this.ultraGroupBox2.Controls.Add(this.chkMonday);
            this.ultraGroupBox2.Controls.Add(this.chkSaturday);
            this.ultraGroupBox2.Controls.Add(this.chkThursday);
            this.ultraGroupBox2.Controls.Add(this.chkFriday);
            this.ultraGroupBox2.Location = new System.Drawing.Point(7, 55);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(329, 125);
            this.ultraGroupBox2.TabIndex = 1;
            this.ultraGroupBox2.Text = "Backup On";
            // 
            // chkSunday
            // 
            this.chkSunday.AutoSize = true;
            this.chkSunday.Location = new System.Drawing.Point(6, 96);
            this.chkSunday.Name = "chkSunday";
            this.chkSunday.Size = new System.Drawing.Size(64, 18);
            this.chkSunday.TabIndex = 6;
            this.chkSunday.Text = "Sunday";
            // 
            // chkWednesday
            // 
            this.chkWednesday.AutoSize = true;
            this.chkWednesday.Location = new System.Drawing.Point(6, 44);
            this.chkWednesday.Name = "chkWednesday";
            this.chkWednesday.Size = new System.Drawing.Size(87, 18);
            this.chkWednesday.TabIndex = 2;
            this.chkWednesday.Text = "Wednesday";
            // 
            // chkTuesday
            // 
            this.chkTuesday.AutoSize = true;
            this.chkTuesday.Location = new System.Drawing.Point(113, 19);
            this.chkTuesday.Name = "chkTuesday";
            this.chkTuesday.Size = new System.Drawing.Size(69, 18);
            this.chkTuesday.TabIndex = 1;
            this.chkTuesday.Text = "Tuesday";
            // 
            // chkMonday
            // 
            this.chkMonday.AutoSize = true;
            this.chkMonday.Location = new System.Drawing.Point(6, 20);
            this.chkMonday.Name = "chkMonday";
            this.chkMonday.Size = new System.Drawing.Size(65, 18);
            this.chkMonday.TabIndex = 0;
            this.chkMonday.Text = "Monday";
            // 
            // chkSaturday
            // 
            this.chkSaturday.AutoSize = true;
            this.chkSaturday.Location = new System.Drawing.Point(113, 71);
            this.chkSaturday.Name = "chkSaturday";
            this.chkSaturday.Size = new System.Drawing.Size(73, 18);
            this.chkSaturday.TabIndex = 5;
            this.chkSaturday.Text = "Saturday";
            // 
            // chkThursday
            // 
            this.chkThursday.AutoSize = true;
            this.chkThursday.Location = new System.Drawing.Point(113, 45);
            this.chkThursday.Name = "chkThursday";
            this.chkThursday.Size = new System.Drawing.Size(74, 18);
            this.chkThursday.TabIndex = 3;
            this.chkThursday.Text = "Thursday";
            // 
            // chkFriday
            // 
            this.chkFriday.AutoSize = true;
            this.chkFriday.Location = new System.Drawing.Point(6, 70);
            this.chkFriday.Name = "chkFriday";
            this.chkFriday.Size = new System.Drawing.Size(56, 18);
            this.chkFriday.TabIndex = 4;
            this.chkFriday.Text = "Friday";
            // 
            // chkEnabled
            // 
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.Location = new System.Drawing.Point(7, 28);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(105, 18);
            this.chkEnabled.TabIndex = 0;
            this.chkEnabled.Text = "Enable Backup";
            this.chkEnabled.CheckedChanged += new System.EventHandler(this.chkEnabled_CheckedChanged);
            // 
            // SettingsBackup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SettingsBackup";
            this.Size = new System.Drawing.Size(342, 427);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTransactionLogIntervalHours)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkBackupTransactionLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotify)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCleanup)).EndInit();
            this.pnlCleanup.ClientArea.ResumeLayout(false);
            this.pnlCleanup.ClientArea.PerformLayout();
            this.pnlCleanup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numTransactionLogDays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFilesToKeep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackupDir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBackupTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkSunday)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkWednesday)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkTuesday)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMonday)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSaturday)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkThursday)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFriday)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEnabled)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkEnabled;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkMonday;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkSunday;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkWednesday;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkSaturday;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkTuesday;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkFriday;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkThursday;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteBackupTime;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBackupDir;
        private Infragistics.Win.Misc.UltraButton btnSaveTo;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkCleanup;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numFilesToKeep;
        private Infragistics.Win.Misc.UltraPanel pnlCleanup;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkNotify;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numTransactionLogDays;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkBackupTransactionLog;
        private Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numTransactionLogIntervalHours;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
    }
}
