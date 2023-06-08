
namespace DWOS.UI.Processing
{
    partial class SampleSetEntry
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SampleSetEntry));
            this.ultraGridBagLayoutManager1 = new Infragistics.Win.Misc.UltraGridBagLayoutManager(this.components);
            this.lblMinVal = new Infragistics.Win.Misc.UltraLabel();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.tbxAverageValue = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.dgrSampleSet = new System.Windows.Forms.DataGridView();
            this.questionSampleSetDataTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.orderProcessesDataTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.lblMaxVal = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.txtSampleSize = new Infragistics.Win.Misc.UltraLabel();
            this.txtMinMax = new Infragistics.Win.Misc.UltraLabel();
            this.orderProcessesTableAdapterBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.sampleSetEntryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbxAverageValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrSampleSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.questionSampleSetDataTableBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.orderProcessesDataTableBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.orderProcessesTableAdapterBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sampleSetEntryBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMinVal
            // 
            this.lblMinVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance1.TextHAlignAsString = "Left";
            appearance1.TextVAlignAsString = "Middle";
            this.lblMinVal.Appearance = appearance1;
            this.lblMinVal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMinVal.Location = new System.Drawing.Point(78, 303);
            this.lblMinVal.Name = "lblMinVal";
            this.lblMinVal.Size = new System.Drawing.Size(56, 21);
            this.lblMinVal.TabIndex = 1;
            this.lblMinVal.Text = "0";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(184, 357);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(103, 357);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tbxAverageValue
            // 
            this.tbxAverageValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.BackColor = System.Drawing.SystemColors.Window;
            appearance2.BackColor2 = System.Drawing.SystemColors.Window;
            appearance2.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            appearance2.TextHAlignAsString = "Right";
            this.tbxAverageValue.Appearance = appearance2;
            this.tbxAverageValue.BackColor = System.Drawing.Color.Transparent;
            this.tbxAverageValue.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.tbxAverageValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxAverageValue.Location = new System.Drawing.Point(130, 331);
            this.tbxAverageValue.Name = "tbxAverageValue";
            this.tbxAverageValue.ReadOnly = true;
            this.tbxAverageValue.Size = new System.Drawing.Size(129, 20);
            this.tbxAverageValue.TabIndex = 4;
            this.tbxAverageValue.Text = "0";
            // 
            // dgrSampleSet
            // 
            this.dgrSampleSet.AllowUserToResizeColumns = false;
            this.dgrSampleSet.AllowUserToResizeRows = false;
            this.dgrSampleSet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgrSampleSet.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgrSampleSet.BackgroundColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.dgrSampleSet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgrSampleSet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.Value});
            this.dgrSampleSet.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgrSampleSet.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.dgrSampleSet.Location = new System.Drawing.Point(12, 51);
            this.dgrSampleSet.Name = "dgrSampleSet";
            this.dgrSampleSet.RowHeadersVisible = false;
            this.dgrSampleSet.RowHeadersWidth = 22;
            this.dgrSampleSet.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgrSampleSet.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgrSampleSet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrSampleSet.Size = new System.Drawing.Size(247, 247);
            this.dgrSampleSet.TabIndex = 0;
            this.dgrSampleSet.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrSampleSet_CellEndEdit);
            this.dgrSampleSet.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgrSampleSet_CellValidating);
            this.dgrSampleSet.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgrSampleSet_CellValueNeeded);
            this.dgrSampleSet.NewRowNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgrSampleSet_NewRowNeeded);
            this.dgrSampleSet.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgrSampleSet_RowsAdded);
            this.dgrSampleSet.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgrSampleSet_UserDeletedRow);
            // 
            // questionSampleSetDataTableBindingSource
            // 
            this.questionSampleSetDataTableBindingSource.DataSource = typeof(DWOS.Data.Datasets.OrderProcessingDataSet.QuestionSampleSetDataTable);
            // 
            // orderProcessesDataTableBindingSource
            // 
            this.orderProcessesDataTableBindingSource.DataSource = typeof(DWOS.Data.Datasets.OrderProcessingDataSet.OrderProcessesDataTable);
            // 
            // ultraLabel1
            // 
            appearance3.TextHAlignAsString = "Left";
            appearance3.TextVAlignAsString = "Middle";
            this.ultraLabel1.Appearance = appearance3;
            this.ultraLabel1.Location = new System.Drawing.Point(12, 27);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(122, 20);
            this.ultraLabel1.TabIndex = 5;
            this.ultraLabel1.Text = "Average Min/Max:";
            // 
            // ultraLabel2
            // 
            appearance4.TextHAlignAsString = "Left";
            appearance4.TextVAlignAsString = "Middle";
            this.ultraLabel2.Appearance = appearance4;
            this.ultraLabel2.Location = new System.Drawing.Point(12, 3);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(122, 20);
            this.ultraLabel2.TabIndex = 7;
            this.ultraLabel2.Text = "Min Samples Size:";
            // 
            // lblMaxVal
            // 
            this.lblMaxVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance5.TextHAlignAsString = "Left";
            appearance5.TextVAlignAsString = "Middle";
            this.lblMaxVal.Appearance = appearance5;
            this.lblMaxVal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaxVal.Location = new System.Drawing.Point(205, 304);
            this.lblMaxVal.Name = "lblMaxVal";
            this.lblMaxVal.Size = new System.Drawing.Size(53, 21);
            this.lblMaxVal.TabIndex = 10;
            this.lblMaxVal.Text = "0";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance6.TextHAlignAsString = "Left";
            appearance6.TextVAlignAsString = "Middle";
            this.ultraLabel5.Appearance = appearance6;
            this.ultraLabel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel5.Location = new System.Drawing.Point(12, 330);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(112, 21);
            this.ultraLabel5.TabIndex = 11;
            this.ultraLabel5.Text = "Average Value:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance7.TextHAlignAsString = "Left";
            appearance7.TextVAlignAsString = "Middle";
            this.ultraLabel3.Appearance = appearance7;
            this.ultraLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel3.Location = new System.Drawing.Point(12, 303);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(60, 21);
            this.ultraLabel3.TabIndex = 12;
            this.ultraLabel3.Text = "Min Value:";
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance8.TextHAlignAsString = "Left";
            appearance8.TextVAlignAsString = "Middle";
            this.ultraLabel4.Appearance = appearance8;
            this.ultraLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel4.Location = new System.Drawing.Point(130, 303);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(69, 21);
            this.ultraLabel4.TabIndex = 13;
            this.ultraLabel4.Text = "Max Value:";
            // 
            // txtSampleSize
            // 
            this.txtSampleSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance9.ForeColor = System.Drawing.Color.Red;
            appearance9.TextHAlignAsString = "Left";
            appearance9.TextVAlignAsString = "Middle";
            this.txtSampleSize.Appearance = appearance9;
            this.txtSampleSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSampleSize.Location = new System.Drawing.Point(140, 3);
            this.txtSampleSize.Name = "txtSampleSize";
            this.txtSampleSize.Size = new System.Drawing.Size(118, 21);
            this.txtSampleSize.TabIndex = 14;
            this.txtSampleSize.Text = "0";
            // 
            // txtMinMax
            // 
            this.txtMinMax.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance10.TextHAlignAsString = "Left";
            appearance10.TextVAlignAsString = "Middle";
            this.txtMinMax.Appearance = appearance10;
            this.txtMinMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMinMax.Location = new System.Drawing.Point(140, 27);
            this.txtMinMax.Name = "txtMinMax";
            this.txtMinMax.Size = new System.Drawing.Size(119, 21);
            this.txtMinMax.TabIndex = 15;
            this.txtMinMax.Text = "0";
            // 
            // orderProcessesTableAdapterBindingSource
            // 
            this.orderProcessesTableAdapterBindingSource.DataSource = typeof(DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter);
            // 
            // sampleSetEntryBindingSource
            // 
            this.sampleSetEntryBindingSource.DataSource = typeof(DWOS.UI.Processing.SampleSetEntry);
            // 
            // ID
            // 
            this.ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ID.FillWeight = 40F;
            this.ID.HeaderText = "ID";
            this.ID.MinimumWidth = 10;
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ID.Width = 40;
            // 
            // Value
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "N5";
            this.Value.DefaultCellStyle = dataGridViewCellStyle1;
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            // 
            // SampleSetEntry
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(271, 392);
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.txtMinMax);
            this.Controls.Add(this.txtSampleSize);
            this.Controls.Add(this.ultraLabel5);
            this.Controls.Add(this.lblMaxVal);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.tbxAverageValue);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblMinVal);
            this.Controls.Add(this.dgrSampleSet);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(280, 280);
            this.Name = "SampleSetEntry";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sampling Data";
            this.Load += new System.EventHandler(this.SampleSetEntry_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbxAverageValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrSampleSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.questionSampleSetDataTableBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.orderProcessesDataTableBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.orderProcessesTableAdapterBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sampleSetEntryBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraGridBagLayoutManager ultraGridBagLayoutManager1;
        private System.Windows.Forms.DataGridView dgrSampleSet;
        private Infragistics.Win.Misc.UltraLabel lblMinVal;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        public Infragistics.Win.UltraWinEditors.UltraTextEditor tbxAverageValue;
        private System.Windows.Forms.BindingSource questionSampleSetDataTableBindingSource;
        private System.Windows.Forms.BindingSource orderProcessesDataTableBindingSource;
        private System.Windows.Forms.BindingSource orderProcessesTableAdapterBindingSource;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel lblMaxVal;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel txtSampleSize;
        private Infragistics.Win.Misc.UltraLabel txtMinMax;
        private System.Windows.Forms.BindingSource sampleSetEntryBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
    }
}