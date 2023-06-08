namespace DWOS.UI.Admin.OrderReset
{
	partial class TaskSelection
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.chkReset = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.chkChangeDepartment = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.chkCloseOrders = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkOpenOrders = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.chkReset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkChangeDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCloseOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkOpenOrders)).BeginInit();
            this.SuspendLayout();
            // 
            // chkReset
            // 
            this.chkReset.Location = new System.Drawing.Point(3, 47);
            this.chkReset.Name = "chkReset";
            this.chkReset.Size = new System.Drawing.Size(237, 20);
            this.chkReset.TabIndex = 1;
            this.chkReset.Text = "Reset Order Process Steps";
            this.chkReset.AfterCheckStateChanged += new Infragistics.Win.CheckEditor.AfterCheckStateChangedHandler(this.chkCloseOrders_AfterCheckStateChanged);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Location = new System.Drawing.Point(36, 71);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(227, 15);
            this.ultraLabel2.TabIndex = 70;
            this.ultraLabel2.Text = "Reset orders back to their initial state.";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(36, 116);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(454, 15);
            this.ultraLabel1.TabIndex = 72;
            this.ultraLabel1.Text = "Move the selected orders to another department and update their work status.";
            // 
            // chkChangeDepartment
            // 
            this.chkChangeDepartment.Location = new System.Drawing.Point(3, 92);
            this.chkChangeDepartment.Name = "chkChangeDepartment";
            this.chkChangeDepartment.Size = new System.Drawing.Size(237, 20);
            this.chkChangeDepartment.TabIndex = 2;
            this.chkChangeDepartment.Text = "Move Order";
            this.chkChangeDepartment.AfterCheckStateChanged += new Infragistics.Win.CheckEditor.AfterCheckStateChangedHandler(this.chkCloseOrders_AfterCheckStateChanged);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(36, 27);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(471, 15);
            this.ultraLabel3.TabIndex = 74;
            this.ultraLabel3.Text = "Move orders to the \'Closed\' state manually if they did not get closed out properl" +
    "y.";
            // 
            // chkCloseOrders
            // 
            this.chkCloseOrders.Location = new System.Drawing.Point(3, 3);
            this.chkCloseOrders.Name = "chkCloseOrders";
            this.chkCloseOrders.Size = new System.Drawing.Size(237, 20);
            this.chkCloseOrders.TabIndex = 0;
            this.chkCloseOrders.Text = "Close Order";
            this.chkCloseOrders.AfterCheckStateChanged += new Infragistics.Win.CheckEditor.AfterCheckStateChangedHandler(this.chkCloseOrders_AfterCheckStateChanged);
            // 
            // chkOpenOrders
            // 
            this.chkOpenOrders.Location = new System.Drawing.Point(3, 137);
            this.chkOpenOrders.Name = "chkOpenOrders";
            this.chkOpenOrders.Size = new System.Drawing.Size(120, 20);
            this.chkOpenOrders.TabIndex = 3;
            this.chkOpenOrders.Text = "Open Order";
            this.chkOpenOrders.AfterCheckStateChanged += new Infragistics.Win.CheckEditor.AfterCheckStateChangedHandler(this.chkCloseOrders_AfterCheckStateChanged);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.ultraLabel4.Location = new System.Drawing.Point(36, 164);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(300, 15);
            this.ultraLabel4.TabIndex = 76;
            this.ultraLabel4.Text = "Move orders to the \'Open\' state if they were closed.";
            // 
            // TaskSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.chkOpenOrders);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.chkCloseOrders);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.chkChangeDepartment);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.chkReset);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "TaskSelection";
            this.Size = new System.Drawing.Size(579, 286);
            ((System.ComponentModel.ISupportInitialize)(this.chkReset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkChangeDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCloseOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkOpenOrders)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkReset;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkChangeDepartment;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkCloseOrders;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkOpenOrders;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;

	}
}
