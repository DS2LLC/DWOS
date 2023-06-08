namespace DWOS.UI.Admin.RevisePartProcess
{
    partial class ReviseProcessesPanel
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CurrentRevision");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("NextRevision");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessId", 0);
            this.grdProcesses = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.progressBar = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.btnRevise = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.grdProcesses)).BeginInit();
            this.SuspendLayout();
            // 
            // grdProcesses
            // 
            this.grdProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdProcesses.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 163;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 161;
            ultraGridColumn3.Header.Caption = "Current Revision";
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 161;
            ultraGridColumn4.Header.Caption = "Next Revison";
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Width = 161;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Hidden = true;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5});
            this.grdProcesses.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdProcesses.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.grdProcesses.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdProcesses.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.grdProcesses.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.grdProcesses.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdProcesses.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdProcesses.Location = new System.Drawing.Point(0, 0);
            this.grdProcesses.Name = "grdProcesses";
            this.grdProcesses.Size = new System.Drawing.Size(684, 370);
            this.grdProcesses.TabIndex = 0;
            this.grdProcesses.Text = "ultraGrid1";
            this.grdProcesses.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdProcesses_InitializeLayout);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(0, 376);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(556, 23);
            this.progressBar.TabIndex = 1;
            this.progressBar.Text = "[Formatted]";
            // 
            // btnRevise
            // 
            this.btnRevise.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRevise.Location = new System.Drawing.Point(562, 376);
            this.btnRevise.Name = "btnRevise";
            this.btnRevise.Size = new System.Drawing.Size(122, 23);
            this.btnRevise.TabIndex = 2;
            this.btnRevise.Text = "Revise Processes";
            this.btnRevise.Click += new System.EventHandler(this.btnRevise_Click);
            // 
            // ReviseProcessesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.btnRevise);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.grdProcesses);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ReviseProcessesPanel";
            this.Size = new System.Drawing.Size(684, 399);
            ((System.ComponentModel.ISupportInitialize)(this.grdProcesses)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid grdProcesses;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar progressBar;
        private Infragistics.Win.Misc.UltraButton btnRevise;
    }
}
