namespace DWOS.Server.Admin.SettingsPanels
{
    partial class ChangeTableSeed
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
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCurrentSeed = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.numNewSeed = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtTable = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentSeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNewSeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTable)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(198, 175);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(294, 175);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(10, 44);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(70, 15);
            this.ultraLabel3.TabIndex = 9;
            this.ultraLabel3.Text = "Current ID:";
            // 
            // txtCurrentSeed
            // 
            this.txtCurrentSeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCurrentSeed.Location = new System.Drawing.Point(184, 40);
            this.txtCurrentSeed.Name = "txtCurrentSeed";
            this.txtCurrentSeed.ReadOnly = true;
            this.txtCurrentSeed.Size = new System.Drawing.Size(201, 22);
            this.txtCurrentSeed.TabIndex = 12;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(10, 73);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(51, 15);
            this.ultraLabel1.TabIndex = 13;
            this.ultraLabel1.Text = "New ID:";
            // 
            // numNewSeed
            // 
            this.numNewSeed.Location = new System.Drawing.Point(184, 69);
            this.numNewSeed.Name = "numNewSeed";
            this.numNewSeed.Size = new System.Drawing.Size(201, 22);
            this.numNewSeed.TabIndex = 14;
            // 
            // txtTable
            // 
            this.txtTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTable.Location = new System.Drawing.Point(184, 12);
            this.txtTable.Name = "txtTable";
            this.txtTable.ReadOnly = true;
            this.txtTable.Size = new System.Drawing.Size(201, 22);
            this.txtTable.TabIndex = 17;
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(10, 16);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(40, 15);
            this.ultraLabel4.TabIndex = 16;
            this.ultraLabel4.Text = "Table:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel2.Location = new System.Drawing.Point(10, 104);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(377, 65);
            this.ultraLabel2.TabIndex = 18;
            this.ultraLabel2.Text = "WARNING: Changing the table id cannot be undone. All new items will start with th" +
    "e new id, which must be greater than the current id.";
            // 
            // ChangeTableSeed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 210);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.txtTable);
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.numNewSeed);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.txtCurrentSeed);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ChangeTableSeed";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Change Table ID";
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentSeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNewSeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCurrentSeed;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numNewSeed;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTable;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
    }
}