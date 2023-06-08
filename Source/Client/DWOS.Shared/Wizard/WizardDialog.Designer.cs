namespace DWOS.Shared.Wizard
{
    partial class WizardDialog
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
            OnDisposeMe();
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardDialog));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnNext = new Infragistics.Win.Misc.UltraButton();
            this.pnlFooter = new Infragistics.Win.Misc.UltraPanel();
            this.btnBack = new Infragistics.Win.Misc.UltraButton();
            this.pnlBody = new Infragistics.Win.Misc.UltraPanel();
            this.pnlHeader = new Infragistics.Win.Misc.UltraPanel();
            this.lblDescription = new Infragistics.Win.Misc.UltraLabel();
            this.lblTitle = new Infragistics.Win.Misc.UltraLabel();
            this.pnlFooter.ClientArea.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.pnlBody.SuspendLayout();
            this.pnlHeader.ClientArea.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(430, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(349, 3);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Next >";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // pnlFooter
            // 
            // 
            // pnlFooter.ClientArea
            // 
            this.pnlFooter.ClientArea.Controls.Add(this.btnBack);
            this.pnlFooter.ClientArea.Controls.Add(this.btnCancel);
            this.pnlFooter.ClientArea.Controls.Add(this.btnNext);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 265);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(517, 38);
            this.pnlFooter.TabIndex = 2;
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.Location = new System.Drawing.Point(268, 3);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "< Back";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // pnlBody
            // 
            this.pnlBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBody.Location = new System.Drawing.Point(4, 63);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Size = new System.Drawing.Size(508, 196);
            this.pnlBody.TabIndex = 3;
            // 
            // pnlHeader
            // 
            appearance1.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Appearance = appearance1;
            this.pnlHeader.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            // 
            // pnlHeader.ClientArea
            // 
            this.pnlHeader.ClientArea.Controls.Add(this.lblDescription);
            this.pnlHeader.ClientArea.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(517, 57);
            this.pnlHeader.TabIndex = 4;
            this.pnlHeader.UseAppStyling = false;
            // 
            // lblDescription
            // 
            this.lblDescription.Location = new System.Drawing.Point(33, 35);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(471, 19);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. ";
            // 
            // lblTitle
            // 
            appearance2.FontData.BoldAsString = "True";
            appearance2.FontData.SizeInPoints = 10F;
            this.lblTitle.Appearance = appearance2;
            this.lblTitle.Location = new System.Drawing.Point(12, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(501, 17);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Lorem ipsum";
            // 
            // WizardDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 303);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlFooter);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WizardDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Wizard Dialog";
            this.pnlFooter.ClientArea.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            this.pnlBody.ResumeLayout(false);
            this.pnlHeader.ClientArea.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnNext;
        private Infragistics.Win.Misc.UltraPanel pnlFooter;
        private Infragistics.Win.Misc.UltraPanel pnlBody;
        private Infragistics.Win.Misc.UltraPanel pnlHeader;
        private Infragistics.Win.Misc.UltraLabel lblDescription;
        private Infragistics.Win.Misc.UltraLabel lblTitle;
        private Infragistics.Win.Misc.UltraButton btnBack;
    }
}