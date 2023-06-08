namespace DWOS.UI
{
	partial class DataEditorBase
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
			OnDispose();
			
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.Orders_Fill_Panel = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvwTOC = new Infragistics.Win.UltraWinTree.UltraTree();
            this.panel1 = new System.Windows.Forms.Panel();
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            this.btnApply = new Infragistics.Win.Misc.UltraButton();
            this._Orders_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.toolbarManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._Orders_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._Orders_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._Orders_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.errProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.messageBoxManager = new Infragistics.Win.UltraMessageBox.UltraMessageBoxManager(this.components);
            this.persistWindowState1 = new DWOS.UI.Utilities.PersistWindowState(this.components);
            this.Orders_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(551, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(469, 6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // Orders_Fill_Panel
            // 
            this.Orders_Fill_Panel.Controls.Add(this.splitContainer1);
            this.Orders_Fill_Panel.Controls.Add(this.panel1);
            this.Orders_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.Orders_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Orders_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.Orders_Fill_Panel.Name = "Orders_Fill_Panel";
            this.Orders_Fill_Panel.Size = new System.Drawing.Size(723, 548);
            this.inboxControlStyler1.SetStyleSettings(this.Orders_Fill_Panel, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Orders_Fill_Panel.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvwTOC);
            this.inboxControlStyler1.SetStyleSettings(this.splitContainer1.Panel1, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.AutoScrollMinSize = new System.Drawing.Size(480, 480);
            this.splitContainer1.Panel2.Margin = new System.Windows.Forms.Padding(3);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.inboxControlStyler1.SetStyleSettings(this.splitContainer1.Panel2, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.splitContainer1.Size = new System.Drawing.Size(723, 507);
            this.splitContainer1.SplitterDistance = 171;
            this.inboxControlStyler1.SetStyleSettings(this.splitContainer1, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.splitContainer1.TabIndex = 31;
            // 
            // tvwTOC
            // 
            this.tvwTOC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwTOC.FullRowSelect = true;
            this.tvwTOC.HideSelection = false;
            this.tvwTOC.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tvwTOC.Location = new System.Drawing.Point(0, 0);
            this.tvwTOC.Name = "tvwTOC";
            _override1.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            _override1.HotTracking = Infragistics.Win.DefaultableBoolean.True;
            _override1.LabelEdit = Infragistics.Win.DefaultableBoolean.False;
            _override1.SelectionType = Infragistics.Win.UltraWinTree.SelectType.Single;
            _override1.Sort = Infragistics.Win.UltraWinTree.SortType.Ascending;
            this.tvwTOC.Override = _override1;
            this.tvwTOC.Size = new System.Drawing.Size(171, 507);
            this.tvwTOC.TabIndex = 0;
            this.tvwTOC.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.tvwTOC_AfterSelect);
            this.tvwTOC.BeforeSelect += new Infragistics.Win.UltraWinTree.BeforeNodeSelectEventHandler(this.tvwTOC_BeforeSelect);
            this.tvwTOC.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvwTOC_KeyUp);
            this.tvwTOC.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvwTOC_MouseUp);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.helpLink1);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Controls.Add(this.btnApply);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 507);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(723, 41);
            this.inboxControlStyler1.SetStyleSettings(this.panel1, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.panel1.TabIndex = 32;
            // 
            // helpLink1
            // 
            this.helpLink1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = null;
            this.helpLink1.Location = new System.Drawing.Point(13, 13);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 3;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(635, 6);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(76, 23);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "&Apply";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // _Orders_Toolbars_Dock_Area_Left
            // 
            this._Orders_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._Orders_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._Orders_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._Orders_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Orders_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._Orders_Toolbars_Dock_Area_Left.Name = "_Orders_Toolbars_Dock_Area_Left";
            this._Orders_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 548);
            this._Orders_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarManager;
            // 
            // toolbarManager
            // 
            this.toolbarManager.AlwaysShowMenusExpanded = Infragistics.Win.DefaultableBoolean.True;
            this.toolbarManager.DesignerFlags = 1;
            this.toolbarManager.DockWithinContainer = this;
            this.toolbarManager.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
            appearance1.ImageBackground = global::DWOS.UI.Properties.Resources.SettingsBackground;
            appearance1.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.toolbarManager.Ribbon.ApplicationMenu2010.ContentArea.Settings.Appearance = appearance1;
            this.toolbarManager.Ribbon.FileMenuStyle = Infragistics.Win.UltraWinToolbars.FileMenuStyle.ApplicationMenu2010;
            this.toolbarManager.ShowFullMenusDelay = 500;
            this.toolbarManager.ShowShortcutsInToolTips = true;
            this.toolbarManager.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2010;
            // 
            // _Orders_Toolbars_Dock_Area_Right
            // 
            this._Orders_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._Orders_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._Orders_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._Orders_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Orders_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(723, 0);
            this._Orders_Toolbars_Dock_Area_Right.Name = "_Orders_Toolbars_Dock_Area_Right";
            this._Orders_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 548);
            this._Orders_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarManager;
            // 
            // _Orders_Toolbars_Dock_Area_Top
            // 
            this._Orders_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._Orders_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._Orders_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._Orders_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Orders_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._Orders_Toolbars_Dock_Area_Top.Name = "_Orders_Toolbars_Dock_Area_Top";
            this._Orders_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(723, 0);
            this._Orders_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarManager;
            // 
            // _Orders_Toolbars_Dock_Area_Bottom
            // 
            this._Orders_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._Orders_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._Orders_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._Orders_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Orders_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 548);
            this._Orders_Toolbars_Dock_Area_Bottom.Name = "_Orders_Toolbars_Dock_Area_Bottom";
            this._Orders_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(723, 0);
            this._Orders_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarManager;
            // 
            // errProvider
            // 
            this.errProvider.ContainerControl = this;
            // 
            // messageBoxManager
            // 
            this.messageBoxManager.ContainingControl = this;
            this.messageBoxManager.EnableSounds = Infragistics.Win.DefaultableBoolean.True;
            this.messageBoxManager.MinimumWidth = 300;
            // 
            // persistWindowState1
            // 
            this.persistWindowState1.FileNamePrefix = null;
            this.persistWindowState1.ParentForm = this;
            this.persistWindowState1.Splitter = this.splitContainer1;
            // 
            // DataEditorBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 548);
            this.Controls.Add(this.Orders_Fill_Panel);
            this.Controls.Add(this._Orders_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._Orders_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._Orders_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this._Orders_Toolbars_Dock_Area_Top);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.Name = "DataEditorBase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Editor Base Class";
            this.Load += new System.EventHandler(this.DataEditorBase_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DataEditorBase_KeyUp);
            this.Orders_Fill_Panel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private System.Windows.Forms.Panel Orders_Fill_Panel;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _Orders_Toolbars_Dock_Area_Left;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _Orders_Toolbars_Dock_Area_Right;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _Orders_Toolbars_Dock_Area_Top;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _Orders_Toolbars_Dock_Area_Bottom;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
		private System.Windows.Forms.Panel panel1;
		protected Infragistics.Win.UltraWinToolbars.UltraToolbarsManager toolbarManager;
		protected Infragistics.Win.UltraWinTree.UltraTree tvwTOC;
		protected System.Windows.Forms.ErrorProvider errProvider;
		protected System.Windows.Forms.SplitContainer splitContainer1;
		private Infragistics.Win.Misc.UltraButton btnApply;
		private DWOS.UI.Utilities.PersistWindowState persistWindowState1;
		public Infragistics.Win.UltraMessageBox.UltraMessageBoxManager messageBoxManager;
        protected Utilities.HelpLink helpLink1;
	}
}