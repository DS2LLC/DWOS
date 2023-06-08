namespace DWOS.UI.Admin.ProcessPackagePanels
{
	partial class ProcessPackageInfo
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
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Deletes the selected process from the package.", Infragistics.Win.ToolTipImage.Default, "Delete Process", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Adds a process to the package.", Infragistics.Win.ToolTipImage.Default, "Add Process", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Name of the process package.", Infragistics.Win.ToolTipImage.Default, "Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Shift process down.", Infragistics.Win.ToolTipImage.Default, "Shift Down", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Shift process up.", Infragistics.Win.ToolTipImage.Default, "Shift Up", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.btnDeleteProcess = new Infragistics.Win.Misc.UltraButton();
            this.btnAddProcess = new Infragistics.Win.Misc.UltraButton();
            this.tvwProcesses = new Infragistics.Win.UltraWinTree.UltraTree();
            this.txtName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.btnDown = new Infragistics.Win.Misc.UltraButton();
            this.btnUp = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwProcesses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.btnDown);
            this.grpData.Controls.Add(this.btnUp);
            this.grpData.Controls.Add(this.ultraLabel8);
            this.grpData.Controls.Add(this.btnDeleteProcess);
            this.grpData.Controls.Add(this.btnAddProcess);
            this.grpData.Controls.Add(this.tvwProcesses);
            this.grpData.Controls.Add(this.txtName);
            this.grpData.Controls.Add(this.ultraLabel1);
            this.grpData.Dock = System.Windows.Forms.DockStyle.None;
            appearance5.Image = global::DWOS.UI.Properties.Resources.Package_16;
            this.grpData.HeaderAppearance = appearance5;
            this.grpData.Size = new System.Drawing.Size(360, 220);
            this.grpData.Text = "Process Package Info";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.txtName, 0);
            this.grpData.Controls.SetChildIndex(this.tvwProcesses, 0);
            this.grpData.Controls.SetChildIndex(this.btnAddProcess, 0);
            this.grpData.Controls.SetChildIndex(this.btnDeleteProcess, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel8, 0);
            this.grpData.Controls.SetChildIndex(this.btnUp, 0);
            this.grpData.Controls.SetChildIndex(this.btnDown, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(419, -1);
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(5, 58);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(65, 15);
            this.ultraLabel8.TabIndex = 61;
            this.ultraLabel8.Text = "Processes:";
            // 
            // btnDeleteProcess
            // 
            appearance3.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            this.btnDeleteProcess.Appearance = appearance3;
            this.btnDeleteProcess.AutoSize = true;
            this.btnDeleteProcess.Location = new System.Drawing.Point(61, 111);
            this.btnDeleteProcess.Name = "btnDeleteProcess";
            this.btnDeleteProcess.Size = new System.Drawing.Size(26, 26);
            this.btnDeleteProcess.TabIndex = 56;
            ultraToolTipInfo3.ToolTipText = "Deletes the selected process from the package.";
            ultraToolTipInfo3.ToolTipTitle = "Delete Process";
            this.tipManager.SetUltraToolTip(this.btnDeleteProcess, ultraToolTipInfo3);
            // 
            // btnAddProcess
            // 
            appearance4.Image = global::DWOS.UI.Properties.Resources.Add_16;
            this.btnAddProcess.Appearance = appearance4;
            this.btnAddProcess.AutoSize = true;
            this.btnAddProcess.Location = new System.Drawing.Point(61, 79);
            this.btnAddProcess.Name = "btnAddProcess";
            this.btnAddProcess.Size = new System.Drawing.Size(26, 26);
            this.btnAddProcess.TabIndex = 54;
            ultraToolTipInfo4.ToolTipText = "Adds a process to the package.";
            ultraToolTipInfo4.ToolTipTitle = "Add Process";
            this.tipManager.SetUltraToolTip(this.btnAddProcess, ultraToolTipInfo4);
            this.btnAddProcess.Click += new System.EventHandler(this.btnAddProcess_Click);
            // 
            // tvwProcesses
            // 
            this.tvwProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwProcesses.HideSelection = false;
            this.tvwProcesses.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tvwProcesses.Location = new System.Drawing.Point(93, 58);
            this.tvwProcesses.Name = "tvwProcesses";
            this.tvwProcesses.NodeConnectorColor = System.Drawing.SystemColors.ControlDark;
            _override1.Sort = Infragistics.Win.UltraWinTree.SortType.Ascending;
            this.tvwProcesses.Override = _override1;
            this.tvwProcesses.Size = new System.Drawing.Size(234, 156);
            this.tvwProcesses.TabIndex = 53;
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(93, 30);
            this.txtName.Name = "txtName";
            this.txtName.Nullable = false;
            this.txtName.Size = new System.Drawing.Size(233, 22);
            this.txtName.TabIndex = 40;
            ultraToolTipInfo5.ToolTipText = "Name of the process package.";
            ultraToolTipInfo5.ToolTipTitle = "Name";
            this.tipManager.SetUltraToolTip(this.txtName, ultraToolTipInfo5);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 34);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel1.TabIndex = 41;
            this.ultraLabel1.Text = "Name:";
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::DWOS.UI.Properties.Resources.Arrow_Down;
            this.btnDown.Appearance = appearance1;
            this.btnDown.AutoSize = true;
            this.btnDown.Location = new System.Drawing.Point(328, 111);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(26, 26);
            this.btnDown.TabIndex = 63;
            ultraToolTipInfo1.ToolTipText = "Shift process down.";
            ultraToolTipInfo1.ToolTipTitle = "Shift Down";
            this.tipManager.SetUltraToolTip(this.btnDown, ultraToolTipInfo1);
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.Image = global::DWOS.UI.Properties.Resources.Arrow_Up;
            this.btnUp.Appearance = appearance2;
            this.btnUp.AutoSize = true;
            this.btnUp.Location = new System.Drawing.Point(328, 79);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(26, 26);
            this.btnUp.TabIndex = 62;
            ultraToolTipInfo2.ToolTipText = "Shift process up.";
            ultraToolTipInfo2.ToolTipTitle = "Shift Up";
            this.tipManager.SetUltraToolTip(this.btnUp, ultraToolTipInfo2);
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // ProcessPackageInfo
            // 
            this.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.Name = "ProcessPackageInfo";
            this.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.Size = new System.Drawing.Size(366, 223);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwProcesses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraLabel ultraLabel8;
		private Infragistics.Win.Misc.UltraButton btnDeleteProcess;
		private Infragistics.Win.Misc.UltraButton btnAddProcess;
		private Infragistics.Win.UltraWinTree.UltraTree tvwProcesses;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtName;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.Misc.UltraButton btnDown;
		private Infragistics.Win.Misc.UltraButton btnUp;
	}
}
