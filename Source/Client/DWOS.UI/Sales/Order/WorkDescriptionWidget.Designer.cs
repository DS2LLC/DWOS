namespace DWOS.UI.Sales.Order
{
    partial class WorkDescriptionWidget
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

            OnDispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("Delete");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Description of work to perform on parts.", Infragistics.Win.ToolTipImage.Default, "Description of Work", Infragistics.Win.DefaultableBoolean.Default);
            this.cboWorkDescription = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.cboWorkDescription)).BeginInit();
            this.SuspendLayout();
            // 
            // cboWorkDescription
            // 
            this.cboWorkDescription.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
            this.cboWorkDescription.AutoSize = false;
            appearance1.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            editorButton1.Appearance = appearance1;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton1.Key = "Delete";
            this.cboWorkDescription.ButtonsRight.Add(editorButton1);
            this.cboWorkDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboWorkDescription.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboWorkDescription.Location = new System.Drawing.Point(0, 0);
            this.cboWorkDescription.Name = "cboWorkDescription";
            this.cboWorkDescription.Size = new System.Drawing.Size(117, 21);
            this.cboWorkDescription.TabIndex = 0;
            ultraToolTipInfo1.ToolTipText = "Description of work to perform on parts.";
            ultraToolTipInfo1.ToolTipTitle = "Description of Work";
            this.ultraToolTipManager.SetUltraToolTip(this.cboWorkDescription, ultraToolTipInfo1);
            this.cboWorkDescription.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.cboWorkDescription_EditorButtonClick);
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // WorkDescriptionWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.cboWorkDescription);
            this.Name = "WorkDescriptionWidget";
            this.Size = new System.Drawing.Size(117, 21);
            ((System.ComponentModel.ISupportInitialize)(this.cboWorkDescription)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboWorkDescription;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
    }
}
