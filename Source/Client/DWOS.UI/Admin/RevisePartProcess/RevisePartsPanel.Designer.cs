namespace DWOS.UI.Admin.RevisePartProcess
{
    partial class RevisePartsPanel
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
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CurrentRevision");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("NextRevision");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PartId", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomerName", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DisplayText", 2);
            this.grdParts = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.chkRevise = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.btnRevise = new Infragistics.Win.Misc.UltraButton();
            this.progressBar = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.grdParts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRevise)).BeginInit();
            this.SuspendLayout();
            // 
            // grdParts
            // 
            this.grdParts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdParts.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.Caption = "Part Name";
            ultraGridColumn1.Header.VisiblePosition = 1;
            ultraGridColumn1.Width = 162;
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn2.Width = 153;
            ultraGridColumn3.Header.Caption = "Current Revision";
            ultraGridColumn3.Header.VisiblePosition = 3;
            ultraGridColumn3.Width = 160;
            ultraGridColumn4.Header.Caption = "Next Revison";
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridColumn4.Width = 154;
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn6.Header.Caption = "Customer Name";
            ultraGridColumn6.Header.VisiblePosition = 0;
            ultraGridColumn6.Width = 99;
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn7.Hidden = true;
            ultraGridColumn7.Width = 101;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7});
            this.grdParts.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdParts.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.grdParts.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdParts.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.grdParts.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.grdParts.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdParts.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdParts.Location = new System.Drawing.Point(0, 24);
            this.grdParts.Name = "grdParts";
            this.grdParts.Size = new System.Drawing.Size(766, 218);
            this.grdParts.TabIndex = 1;
            this.grdParts.Text = "ultraGrid1";
            this.grdParts.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdParts_InitializeLayout);
            // 
            // chkRevise
            // 
            this.chkRevise.AutoSize = true;
            this.chkRevise.Location = new System.Drawing.Point(0, 0);
            this.chkRevise.Name = "chkRevise";
            this.chkRevise.Size = new System.Drawing.Size(116, 18);
            this.chkRevise.TabIndex = 2;
            this.chkRevise.Text = "Create Revisions";
            this.chkRevise.CheckedChanged += new System.EventHandler(this.chkRevise_CheckedChanged);
            // 
            // btnRevise
            // 
            this.btnRevise.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRevise.Location = new System.Drawing.Point(644, 248);
            this.btnRevise.Name = "btnRevise";
            this.btnRevise.Size = new System.Drawing.Size(122, 23);
            this.btnRevise.TabIndex = 4;
            this.btnRevise.Text = "Update Parts";
            this.btnRevise.Click += new System.EventHandler(this.btnRevise_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(0, 248);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(638, 23);
            this.progressBar.TabIndex = 3;
            this.progressBar.Text = "[Formatted]";
            // 
            // RevisePartsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnRevise);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.chkRevise);
            this.Controls.Add(this.grdParts);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "RevisePartsPanel";
            this.Size = new System.Drawing.Size(766, 271);
            ((System.ComponentModel.ISupportInitialize)(this.grdParts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRevise)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid grdParts;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkRevise;
        private Infragistics.Win.Misc.UltraButton btnRevise;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar progressBar;
    }
}
