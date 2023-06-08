namespace DWOS.UI.Sales.Order
{
    partial class OrderEntrySettings
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

            OnDisposing();
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Maximum Number of Closed Orders", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.numMaxClosedOrders = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numMaxClosedOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(0, 34);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(180, 15);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Max Number of Closed Orders:";
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // numMaxClosedOrders
            // 
            this.numMaxClosedOrders.Location = new System.Drawing.Point(186, 31);
            this.numMaxClosedOrders.MinValue = 0;
            this.numMaxClosedOrders.Name = "numMaxClosedOrders";
            this.numMaxClosedOrders.Size = new System.Drawing.Size(122, 22);
            this.numMaxClosedOrders.TabIndex = 1;
            ultraToolTipInfo1.ToolTipTextFormatted = "The maximum number of closed orders to retrieve when viewing closed orders.<br/>M" +
    "ust be at least 200.";
            ultraToolTipInfo1.ToolTipTitle = "Maximum Number of Closed Orders";
            this.ultraToolTipManager.SetUltraToolTip(this.numMaxClosedOrders, ultraToolTipInfo1);
            this.numMaxClosedOrders.ValueChanged += new System.EventHandler(this.numMaxClosedOrders_ValueChanged);
            // 
            // ultraLabel2
            // 
            appearance1.ForeColor = System.Drawing.Color.Red;
            this.ultraLabel2.Appearance = appearance1;
            this.ultraLabel2.Location = new System.Drawing.Point(4, 0);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(304, 28);
            this.ultraLabel2.TabIndex = 2;
            this.ultraLabel2.Text = "You may need to reload Order Entry before changes take effect.";
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // OrderEntrySettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.numMaxClosedOrders);
            this.Controls.Add(this.ultraLabel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "OrderEntrySettings";
            this.Size = new System.Drawing.Size(316, 67);
            ((System.ComponentModel.ISupportInitialize)(this.numMaxClosedOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numMaxClosedOrders;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
