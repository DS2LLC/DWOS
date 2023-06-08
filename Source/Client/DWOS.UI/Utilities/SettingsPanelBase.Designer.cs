namespace DWOS.UI.Utilities
{
    partial class SettingsPanelBase
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
            this.components = new System.ComponentModel.Container();
            this.toolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.SuspendLayout();
            // 
            // toolTipManager
            // 
            this.toolTipManager.ContainingControl = this;
            // 
            // SettingsPanelBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SettingsPanelBase";
            this.Size = new System.Drawing.Size(737, 286);
            this.ResumeLayout(false);

        }

        #endregion

        protected Infragistics.Win.UltraWinToolTip.UltraToolTipManager toolTipManager;

    }
}
