namespace DWOS.UI.Admin.RevisePartProcess
{
    partial class RevisePackagesPanel
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
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessPackageId");
            this.btnRevise = new Infragistics.Win.Misc.UltraButton();
            this.progressBar = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.grdPackages = new Infragistics.Win.UltraWinGrid.UltraGrid();
            ((System.ComponentModel.ISupportInitialize)(this.grdPackages)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRevise
            // 
            this.btnRevise.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRevise.Location = new System.Drawing.Point(396, 207);
            this.btnRevise.Name = "btnRevise";
            this.btnRevise.Size = new System.Drawing.Size(122, 23);
            this.btnRevise.TabIndex = 5;
            this.btnRevise.Text = "Update Packages";
            this.btnRevise.Click += new System.EventHandler(this.btnRevise_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(0, 207);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(390, 23);
            this.progressBar.TabIndex = 4;
            this.progressBar.Text = "[Formatted]";
            // 
            // grdPackages
            // 
            this.grdPackages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdPackages.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 480;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn2.Width = 99;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2});
            this.grdPackages.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdPackages.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.grdPackages.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdPackages.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.grdPackages.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.grdPackages.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdPackages.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdPackages.Location = new System.Drawing.Point(0, 0);
            this.grdPackages.Name = "grdPackages";
            this.grdPackages.Size = new System.Drawing.Size(518, 201);
            this.grdPackages.TabIndex = 3;
            this.grdPackages.Text = "ultraGrid1";
            this.grdPackages.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdPackages_InitializeLayout);
            this.grdPackages.AfterSortChange += new Infragistics.Win.UltraWinGrid.BandEventHandler(this.grdPackages_AfterSortChange);
            this.grdPackages.AfterColPosChanged += new Infragistics.Win.UltraWinGrid.AfterColPosChangedEventHandler(this.grdPackages_AfterColPosChanged);
            // 
            // RevisePackagesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnRevise);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.grdPackages);
            this.Name = "RevisePackagesPanel";
            this.Size = new System.Drawing.Size(518, 230);
            ((System.ComponentModel.ISupportInitialize)(this.grdPackages)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnRevise;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar progressBar;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdPackages;
    }
}
