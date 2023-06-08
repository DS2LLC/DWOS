namespace DWOS.UI.Sales
{
    partial class OrderContainersPopup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderContainersPopup));
            this.inboxControlStyler = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.orderContainerWidget = new DWOS.UI.Sales.Order.OrderContainerWidget();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(541, 176);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "&OK";
            // 
            // orderContainerWidget
            // 
            this.orderContainerWidget.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.orderContainerWidget.AutoFit = true;
            this.orderContainerWidget.Dataset = null;
            this.orderContainerWidget.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.orderContainerWidget.Location = new System.Drawing.Point(5, 12);
            this.orderContainerWidget.Name = "orderContainerWidget";
            this.orderContainerWidget.Size = new System.Drawing.Size(612, 158);
            this.orderContainerWidget.TabIndex = 0;
            // 
            // OrderContainersPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 211);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.orderContainerWidget);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OrderContainersPopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Order Containers";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OrderContainersPopup_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler;
        private Order.OrderContainerWidget orderContainerWidget;
        private Infragistics.Win.Misc.UltraButton btnOK;
    }
}