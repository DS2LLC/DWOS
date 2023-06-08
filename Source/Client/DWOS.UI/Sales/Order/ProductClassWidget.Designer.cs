namespace DWOS.UI.Sales.Order
{
    partial class ProductClassWidget
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The product class of the order.", Infragistics.Win.ToolTipImage.Default, "Product Class", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("Delete");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The product class of the order.", Infragistics.Win.ToolTipImage.Default, "Product Class", Infragistics.Win.DefaultableBoolean.Default);
            this.txtProductClass = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboProductClass = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.txtProductClass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProductClass)).BeginInit();
            this.SuspendLayout();
            // 
            // txtProductClass
            // 
            this.txtProductClass.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProductClass.Location = new System.Drawing.Point(0, 0);
            this.txtProductClass.MaxLength = 255;
            this.txtProductClass.Name = "txtProductClass";
            this.txtProductClass.Size = new System.Drawing.Size(115, 21);
            this.txtProductClass.TabIndex = 0;
            ultraToolTipInfo2.ToolTipText = "The product class of the order.";
            ultraToolTipInfo2.ToolTipTitle = "Product Class";
            this.ultraToolTipManager.SetUltraToolTip(this.txtProductClass, ultraToolTipInfo2);
            // 
            // cboProductClass
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            editorButton1.Appearance = appearance1;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton1.Key = "Delete";
            this.cboProductClass.ButtonsRight.Add(editorButton1);
            this.cboProductClass.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboProductClass.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboProductClass.Location = new System.Drawing.Point(0, 0);
            this.cboProductClass.Name = "cboProductClass";
            this.cboProductClass.Size = new System.Drawing.Size(115, 21);
            this.cboProductClass.TabIndex = 1;
            ultraToolTipInfo1.ToolTipText = "The product class of the order.";
            ultraToolTipInfo1.ToolTipTitle = "Product Class";
            this.ultraToolTipManager.SetUltraToolTip(this.cboProductClass, ultraToolTipInfo1);
            this.cboProductClass.Visible = false;
            this.cboProductClass.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.cboProductClass_EditorButtonClick);
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // ProductClassWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.cboProductClass);
            this.Controls.Add(this.txtProductClass);
            this.Name = "ProductClassWidget";
            this.Size = new System.Drawing.Size(115, 21);
            ((System.ComponentModel.ISupportInitialize)(this.txtProductClass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProductClass)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtProductClass;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboProductClass;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
    }
}
