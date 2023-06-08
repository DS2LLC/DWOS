namespace DWOS.UI.Admin.Processes
{
    partial class QuestionFieldPopup
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("Delete");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.grpPopup = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboToken = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.grpPopup)).BeginInit();
            this.grpPopup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboToken)).BeginInit();
            this.SuspendLayout();
            // 
            // grpPopup
            // 
            this.grpPopup.Controls.Add(this.cboToken);
            this.grpPopup.Controls.Add(this.btnCancel);
            this.grpPopup.Controls.Add(this.btnOK);
            this.grpPopup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPopup.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOnBorder;
            this.grpPopup.Location = new System.Drawing.Point(0, 0);
            this.grpPopup.Name = "grpPopup";
            this.grpPopup.Size = new System.Drawing.Size(225, 92);
            this.grpPopup.TabIndex = 0;
            this.grpPopup.Text = "Tokens";
            // 
            // cboToken
            // 
            this.cboToken.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            editorButton1.Appearance = appearance1;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton1.Key = "Delete";
            this.cboToken.ButtonsLeft.Add(editorButton1);
            this.cboToken.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboToken.Location = new System.Drawing.Point(6, 30);
            this.cboToken.Name = "cboToken";
            this.cboToken.Size = new System.Drawing.Size(213, 21);
            this.cboToken.TabIndex = 3;
            this.cboToken.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.cboToken_EditorButtonClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(122, 57);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(23, 57);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // QuestionFieldPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpPopup);
            this.Name = "QuestionFieldPopup";
            this.Size = new System.Drawing.Size(225, 92);
            ((System.ComponentModel.ISupportInitialize)(this.grpPopup)).EndInit();
            this.grpPopup.ResumeLayout(false);
            this.grpPopup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboToken)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox grpPopup;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboToken;
    }
}
