using System.Security.AccessControl;
using nsoftware.Sys.Internal.Public;

namespace DWOS.UI.Admin.ReportPanels
{
    partial class ReportTypePanel
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
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo12 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add a field to this report.", Infragistics.Win.ToolTipImage.Default, "Add Report Field", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo11 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Delete the selected report field from this report.", Infragistics.Win.ToolTipImage.Default, "Delete Report Field", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo10 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Moves the selected report field\'s display order up a value.", Infragistics.Win.ToolTipImage.Default, "Move Report Field ", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Moves the selected report field\'s display order down a value.", Infragistics.Win.ToolTipImage.Default, "Move Report Field", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.grdFields = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnAddField = new Infragistics.Win.Misc.UltraButton();
            this.btnDeleteField = new Infragistics.Win.Misc.UltraButton();
            this.btnMoveUp = new Infragistics.Win.Misc.UltraButton();
            this.btnMoveDown = new Infragistics.Win.Misc.UltraButton();
            this.txtReportName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.taReportFields = new DWOS.Data.Datasets.ReportFieldsDataSetTableAdapters.ReportFieldsTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdFields)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReportName)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.txtReportName);
            this.grpData.Controls.Add(this.btnMoveDown);
            this.grpData.Controls.Add(this.btnMoveUp);
            this.grpData.Controls.Add(this.btnDeleteField);
            this.grpData.Controls.Add(this.btnAddField);
            this.grpData.Controls.Add(this.grdFields);
            this.grpData.Controls.Add(this.ultraLabel1);
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.ultraLabel6);
            this.grpData.Size = new System.Drawing.Size(611, 301);
            this.grpData.Text = "Default Report Fields";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel6, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.grdFields, 0);
            this.grpData.Controls.SetChildIndex(this.btnAddField, 0);
            this.grpData.Controls.SetChildIndex(this.btnDeleteField, 0);
            this.grpData.Controls.SetChildIndex(this.btnMoveUp, 0);
            this.grpData.Controls.SetChildIndex(this.btnMoveDown, 0);
            this.grpData.Controls.SetChildIndex(this.txtReportName, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(747, -3780);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(11, 55);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(79, 15);
            this.ultraLabel4.TabIndex = 46;
            this.ultraLabel4.Text = "Report Type:";
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(11, 82);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel6.TabIndex = 41;
            this.ultraLabel6.Text = "Fields:";
            // 
            // ultraLabel1
            // 
            appearance15.FontData.BoldAsString = "True";
            appearance15.ForeColor = System.Drawing.Color.Firebrick;
            this.ultraLabel1.Appearance = appearance15;
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(11, 25);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(386, 15);
            this.ultraLabel1.TabIndex = 51;
            this.ultraLabel1.Text = "Default fields used if customer report fields are not defined.";
            this.ultraLabel1.UseAppStyling = false;
            // 
            // grdFields
            // 
            this.grdFields.Location = new System.Drawing.Point(96, 82);
            this.grdFields.Name = "grdFields";
            this.grdFields.Size = new System.Drawing.Size(501, 207);
            this.grdFields.TabIndex = 55;
            this.grdFields.Text = "Report Fields";
            this.grdFields.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdFields_InitializeLayout);
            this.grdFields.AfterRowsDeleted += new System.EventHandler(this.grdFields_AfterRowsDeleted);
            this.grdFields.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.grdFields_BeforeRowsDeleted);
            // 
            // btnAddField
            // 
            appearance14.Image = global::DWOS.UI.Properties.Resources.Add_32;
            this.btnAddField.Appearance = appearance14;
            this.btnAddField.AutoSize = true;
            this.btnAddField.Enabled = false;
            this.btnAddField.Location = new System.Drawing.Point(64, 104);
            this.btnAddField.Name = "btnAddField";
            this.btnAddField.Size = new System.Drawing.Size(26, 26);
            this.btnAddField.TabIndex = 56;
            ultraToolTipInfo12.ToolTipText = "Add a field to this report.";
            ultraToolTipInfo12.ToolTipTitle = "Add Report Field";
            this.tipManager.SetUltraToolTip(this.btnAddField, ultraToolTipInfo12);
            this.btnAddField.Click += new System.EventHandler(this.btnAddField_Click);
            // 
            // btnDeleteField
            // 
            appearance13.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            this.btnDeleteField.Appearance = appearance13;
            this.btnDeleteField.AutoSize = true;
            this.btnDeleteField.Location = new System.Drawing.Point(64, 136);
            this.btnDeleteField.Name = "btnDeleteField";
            this.btnDeleteField.Size = new System.Drawing.Size(26, 26);
            this.btnDeleteField.TabIndex = 57;
            ultraToolTipInfo11.ToolTipText = "Delete the selected report field from this report.";
            ultraToolTipInfo11.ToolTipTitle = "Delete Report Field";
            this.tipManager.SetUltraToolTip(this.btnDeleteField, ultraToolTipInfo11);
            this.btnDeleteField.Click += new System.EventHandler(this.btnDeleteField_Click);
            // 
            // btnMoveUp
            // 
            appearance12.Image = global::DWOS.UI.Properties.Resources.Arrow_Up;
            this.btnMoveUp.Appearance = appearance12;
            this.btnMoveUp.AutoSize = true;
            this.btnMoveUp.Location = new System.Drawing.Point(64, 168);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(26, 26);
            this.btnMoveUp.TabIndex = 58;
            ultraToolTipInfo10.ToolTipText = "Moves the selected report field\'s display order up a value.";
            ultraToolTipInfo10.ToolTipTitle = "Move Report Field ";
            this.tipManager.SetUltraToolTip(this.btnMoveUp, ultraToolTipInfo10);
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveFieldUp_Click);
            // 
            // btnMoveDown
            // 
            appearance11.Image = global::DWOS.UI.Properties.Resources.Arrow_Down;
            this.btnMoveDown.Appearance = appearance11;
            this.btnMoveDown.AutoSize = true;
            this.btnMoveDown.Location = new System.Drawing.Point(64, 200);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(26, 26);
            this.btnMoveDown.TabIndex = 59;
            ultraToolTipInfo9.ToolTipText = "Moves the selected report field\'s display order down a value.";
            ultraToolTipInfo9.ToolTipTitle = "Move Report Field";
            this.tipManager.SetUltraToolTip(this.btnMoveDown, ultraToolTipInfo9);
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveFieldDown_Click);
            // 
            // txtReportName
            // 
            this.txtReportName.Enabled = false;
            this.txtReportName.Location = new System.Drawing.Point(96, 55);
            this.txtReportName.Name = "txtReportName";
            this.txtReportName.Size = new System.Drawing.Size(236, 22);
            this.txtReportName.TabIndex = 60;
            // 
            // taReportFields
            // 
            this.taReportFields.ClearBeforeFill = true;
            // 
            // ReportTypePanel
            // 
            this.Name = "ReportTypePanel";
            this.Size = new System.Drawing.Size(617, 307);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdFields)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReportName)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdFields;
        private Infragistics.Win.Misc.UltraButton btnAddField;
        private Infragistics.Win.Misc.UltraButton btnDeleteField;
        private Infragistics.Win.Misc.UltraButton btnMoveUp;
        private Infragistics.Win.Misc.UltraButton btnMoveDown;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtReportName;
        private Data.Datasets.ReportFieldsDataSetTableAdapters.ReportFieldsTableAdapter taReportFields;
    }
}
