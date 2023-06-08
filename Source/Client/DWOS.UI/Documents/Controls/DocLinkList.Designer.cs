namespace DWOS.UI.Documents.Controls
{
    partial class DocLinkList
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.cboLinks = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            ((System.ComponentModel.ISupportInitialize)(this.cboLinks)).BeginInit();
            this.SuspendLayout();
            // 
            // cboLinks
            // 
            this.cboLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLinks.AutoSize = false;
            appearance1.Image = global::DWOS.UI.Properties.Resources.View;
            editorButton1.Appearance = appearance1;
            editorButton1.Text = "";
            this.cboLinks.ButtonsLeft.Add(editorButton1);
            this.cboLinks.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboLinks.Location = new System.Drawing.Point(0, 0);
            this.cboLinks.Name = "cboLinks";
            this.cboLinks.NullText = "<No Document Links>";
            appearance2.FontData.ItalicAsString = "True";
            appearance2.ForeColor = System.Drawing.Color.LightGray;
            this.cboLinks.NullTextAppearance = appearance2;
            this.cboLinks.Size = new System.Drawing.Size(386, 22);
            this.cboLinks.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboLinks.TabIndex = 0;
            this.cboLinks.SelectionChanged += new System.EventHandler(this.cboLinks_SelectionChanged);
            this.cboLinks.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.cboLinks_EditorButtonClick);
            // 
            // DocLinkList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboLinks);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DocLinkList";
            this.Size = new System.Drawing.Size(386, 25);
            ((System.ComponentModel.ISupportInitialize)(this.cboLinks)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboLinks;


    }
}
