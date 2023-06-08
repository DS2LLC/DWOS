namespace DWOS.UI.QA
{
    partial class ReworkJoin
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The original work order ID.", Infragistics.Win.ToolTipImage.Default, "Work Order", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The number of parts in the original work order.", Infragistics.Win.ToolTipImage.Default, "Quantity", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The work order ID for the rework order.", Infragistics.Win.ToolTipImage.Default, "Work Order", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The number of parts included in the rework order.", Infragistics.Win.ToolTipImage.Default, "Quantity", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Re-Print Work Order Travelers", Infragistics.Win.DefaultableBoolean.Default);
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtOriginalOrderID = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.numOriginalQty = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtReworkOrderID = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.numReworkQty = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.chkReprintWO = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.txtOriginalOrderID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOriginalQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtReworkOrderID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReworkQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkReprintWO)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(357, 142);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(275, 142);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(18, 37);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(75, 15);
            this.ultraLabel2.TabIndex = 19;
            this.ultraLabel2.Text = "Work Order:";
            // 
            // txtOriginalOrderID
            // 
            this.txtOriginalOrderID.Location = new System.Drawing.Point(113, 33);
            this.txtOriginalOrderID.Name = "txtOriginalOrderID";
            this.txtOriginalOrderID.ReadOnly = true;
            this.txtOriginalOrderID.Size = new System.Drawing.Size(85, 22);
            this.txtOriginalOrderID.TabIndex = 18;
            ultraToolTipInfo5.ToolTipText = "The original work order ID.";
            ultraToolTipInfo5.ToolTipTitle = "Work Order";
            this.ultraToolTipManager.SetUltraToolTip(this.txtOriginalOrderID, ultraToolTipInfo5);
            // 
            // numOriginalQty
            // 
            this.numOriginalQty.Location = new System.Drawing.Point(113, 61);
            this.numOriginalQty.MaskInput = "nnn,nnn";
            this.numOriginalQty.MaxValue = 999999;
            this.numOriginalQty.MinValue = 0;
            this.numOriginalQty.Name = "numOriginalQty";
            this.numOriginalQty.Size = new System.Drawing.Size(85, 22);
            this.numOriginalQty.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.numOriginalQty.TabIndex = 41;
            ultraToolTipInfo4.ToolTipText = "The number of parts in the original work order.";
            ultraToolTipInfo4.ToolTipTitle = "Quantity";
            this.ultraToolTipManager.SetUltraToolTip(this.numOriginalQty, ultraToolTipInfo4);
            this.numOriginalQty.Value = 1000;
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(18, 65);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(58, 15);
            this.ultraLabel8.TabIndex = 42;
            this.ultraLabel8.Text = "Quantity:";
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.txtOriginalOrderID);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel8);
            this.ultraGroupBox1.Controls.Add(this.numOriginalQty);
            this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(12, 12);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(210, 110);
            this.ultraGroupBox1.TabIndex = 50;
            this.ultraGroupBox1.Text = "Original Order";
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Controls.Add(this.txtReworkOrderID);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox2.Controls.Add(this.numReworkQty);
            this.ultraGroupBox2.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ultraGroupBox2.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox2.Location = new System.Drawing.Point(228, 13);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(210, 110);
            this.ultraGroupBox2.TabIndex = 51;
            this.ultraGroupBox2.Text = "Rework Order";
            // 
            // txtReworkOrderID
            // 
            this.txtReworkOrderID.Location = new System.Drawing.Point(113, 33);
            this.txtReworkOrderID.Name = "txtReworkOrderID";
            this.txtReworkOrderID.ReadOnly = true;
            this.txtReworkOrderID.Size = new System.Drawing.Size(85, 22);
            this.txtReworkOrderID.TabIndex = 18;
            ultraToolTipInfo2.ToolTipText = "The work order ID for the rework order.";
            ultraToolTipInfo2.ToolTipTitle = "Work Order";
            this.ultraToolTipManager.SetUltraToolTip(this.txtReworkOrderID, ultraToolTipInfo2);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(18, 37);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(75, 15);
            this.ultraLabel1.TabIndex = 19;
            this.ultraLabel1.Text = "Work Order:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(18, 65);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(58, 15);
            this.ultraLabel3.TabIndex = 42;
            this.ultraLabel3.Text = "Quantity:";
            // 
            // numReworkQty
            // 
            this.numReworkQty.Location = new System.Drawing.Point(113, 61);
            this.numReworkQty.MaskInput = "nnn,nnn";
            this.numReworkQty.MaxValue = 999999;
            this.numReworkQty.MinValue = 0;
            this.numReworkQty.Name = "numReworkQty";
            this.numReworkQty.Size = new System.Drawing.Size(85, 22);
            this.numReworkQty.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.numReworkQty.TabIndex = 41;
            ultraToolTipInfo3.ToolTipText = "The number of parts included in the rework order.";
            ultraToolTipInfo3.ToolTipTitle = "Quantity";
            this.ultraToolTipManager.SetUltraToolTip(this.numReworkQty, ultraToolTipInfo3);
            this.numReworkQty.Value = 1000;
            // 
            // chkReprintWO
            // 
            this.chkReprintWO.AutoSize = true;
            this.chkReprintWO.Checked = true;
            this.chkReprintWO.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReprintWO.Location = new System.Drawing.Point(12, 146);
            this.chkReprintWO.Name = "chkReprintWO";
            this.chkReprintWO.Size = new System.Drawing.Size(194, 18);
            this.chkReprintWO.TabIndex = 53;
            this.chkReprintWO.Text = "Re-Print Work Order Travelers";
            ultraToolTipInfo1.ToolTipTextFormatted = "If <strong>checked</strong>, the work order traveler reports will be reprinted fo" +
    "r the original order and rework order. ";
            ultraToolTipInfo1.ToolTipTitle = "Re-Print Work Order Travelers";
            this.ultraToolTipManager.SetUltraToolTip(this.chkReprintWO, ultraToolTipInfo1);
            // 
            // helpLink1
            // 
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = "rework_join_dialog.htm";
            this.helpLink1.Location = new System.Drawing.Point(12, 129);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 54;
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // ReworkJoin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 177);
            this.Controls.Add(this.helpLink1);
            this.Controls.Add(this.chkReprintWO);
            this.Controls.Add(this.ultraGroupBox2);
            this.Controls.Add(this.ultraGroupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ReworkJoin";
            this.Text = "Rework Join";
            this.Load += new System.EventHandler(this.ReworkJoin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtOriginalOrderID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOriginalQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtReworkOrderID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReworkQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkReprintWO)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtOriginalOrderID;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numOriginalQty;
        private Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtReworkOrderID;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numReworkQty;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkReprintWO;
        private Utilities.HelpLink helpLink1;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
    }
}