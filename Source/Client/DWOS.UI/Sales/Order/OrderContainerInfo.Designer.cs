namespace DWOS.UI.Sales
{
    partial class OrderContainerInfo
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
			if(disposing && (components != null))
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
            this.orderContainerWidget = new DWOS.UI.Sales.Order.OrderContainerWidget();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.orderContainerWidget);
            appearance1.Image = global::DWOS.UI.Properties.Resources.Container_16;
            this.grpData.HeaderAppearance = appearance1;
            this.grpData.Size = new System.Drawing.Size(521, 304);
            this.grpData.Text = "Order Containers";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.orderContainerWidget, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(1033, -3434);
            // 
            // orderContainerWidget
            // 
            this.orderContainerWidget.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.orderContainerWidget.Dataset = null;
            this.orderContainerWidget.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.orderContainerWidget.Location = new System.Drawing.Point(10, 26);
            this.orderContainerWidget.Name = "orderContainerWidget";
            this.orderContainerWidget.Size = new System.Drawing.Size(499, 272);
            this.orderContainerWidget.TabIndex = 1;
            // 
            // OrderContainerInfo
            // 
            this.Name = "OrderContainerInfo";
            this.Size = new System.Drawing.Size(527, 310);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            this.ResumeLayout(false);

		}

        #endregion

        private Order.OrderContainerWidget orderContainerWidget;
    }
}
