namespace DWOS.UI.Admin
{
	partial class ListEditor
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("Main");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Add");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListEditor));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.grpProcessInformation = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnDelete = new Infragistics.Win.Misc.UltraButton();
            this.btnAdd = new Infragistics.Win.Misc.UltraButton();
            this.lvwValues = new Infragistics.Win.UltraWinListView.UltraListView();
            this.txtListValue = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtListName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.bsLists = new System.Windows.Forms.BindingSource(this.components);
            this.dsProcesses = new DWOS.Data.Datasets.ProcessesDataset();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.tvwTOC = new Infragistics.Win.UltraWinTree.UltraTree();
            this.toolbarManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this.ListEditor_Fill_Panel = new System.Windows.Forms.Panel();
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            this.pnlInfo = new System.Windows.Forms.Panel();
            this._ListEditor_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ListEditor_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ListEditor_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ListEditor_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.errProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.taLists = new DWOS.Data.Datasets.ProcessesDatasetTableAdapters.ListsTableAdapter();
            this.taListValues = new DWOS.Data.Datasets.ProcessesDatasetTableAdapters.ListValuesTableAdapter();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.taManager = new DWOS.Data.Datasets.ProcessesDatasetTableAdapters.TableAdapterManager();
            this.taPartInspectionLists = new DWOS.Data.Datasets.PartInspectionDataSetTableAdapters.ListsTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.grpProcessInformation)).BeginInit();
            this.grpProcessInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvwValues)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtListValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtListName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsLists)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsProcesses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).BeginInit();
            this.ListEditor_Fill_Panel.SuspendLayout();
            this.pnlInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(358, 288);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(276, 288);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grpProcessInformation
            // 
            this.grpProcessInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpProcessInformation.Controls.Add(this.btnDelete);
            this.grpProcessInformation.Controls.Add(this.btnAdd);
            this.grpProcessInformation.Controls.Add(this.lvwValues);
            this.grpProcessInformation.Controls.Add(this.txtListName);
            this.grpProcessInformation.Controls.Add(this.ultraLabel3);
            this.grpProcessInformation.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grpProcessInformation.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpProcessInformation.Location = new System.Drawing.Point(5, -1);
            this.grpProcessInformation.Name = "grpProcessInformation";
            this.grpProcessInformation.Size = new System.Drawing.Size(259, 274);
            this.grpProcessInformation.TabIndex = 0;
            this.grpProcessInformation.Text = "List Information";
            // 
            // btnDelete
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            this.btnDelete.Appearance = appearance1;
            this.btnDelete.Location = new System.Drawing.Point(29, 102);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(26, 26);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.Add_16;
            this.btnAdd.Appearance = appearance2;
            this.btnAdd.Location = new System.Drawing.Point(29, 70);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(26, 26);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lvwValues
            // 
            this.lvwValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwValues.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.True;
            this.lvwValues.ItemSettings.HideSelection = false;
            this.lvwValues.ItemSettings.HotTracking = true;
            this.lvwValues.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.Single;
            this.lvwValues.Location = new System.Drawing.Point(57, 53);
            this.lvwValues.MainColumn.AutoSizeMode = Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header;
            this.lvwValues.MainColumn.DataType = typeof(string);
            this.lvwValues.MainColumn.EditorComponent = this.txtListValue;
            appearance3.TextHAlignAsString = "Center";
            this.lvwValues.MainColumn.HeaderAppearance = appearance3;
            this.lvwValues.MainColumn.Text = "Values";
            this.lvwValues.Name = "lvwValues";
            this.lvwValues.Size = new System.Drawing.Size(184, 215);
            this.lvwValues.TabIndex = 1;
            this.lvwValues.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvwValues.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns;
            this.lvwValues.ViewSettingsDetails.ColumnHeaderStyle = Infragistics.Win.HeaderStyle.WindowsVista;
            this.lvwValues.ViewSettingsDetails.ImageSize = new System.Drawing.Size(0, 0);
            this.lvwValues.ItemExitedEditMode += new Infragistics.Win.UltraWinListView.ItemExitedEditModeEventHandler(this.lvwValues_ItemExitedEditMode);
            // 
            // txtListValue
            // 
            this.txtListValue.Location = new System.Drawing.Point(47, 289);
            this.txtListValue.MaxLength = 50;
            this.txtListValue.Name = "txtListValue";
            this.txtListValue.Size = new System.Drawing.Size(100, 22);
            this.txtListValue.TabIndex = 8;
            this.txtListValue.Visible = false;
            // 
            // txtListName
            // 
            this.txtListName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtListName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bsLists, "Name", true));
            this.txtListName.Location = new System.Drawing.Point(57, 27);
            this.txtListName.MaxLength = 50;
            this.txtListName.Name = "txtListName";
            this.txtListName.Size = new System.Drawing.Size(184, 22);
            this.txtListName.TabIndex = 0;
            // 
            // bsLists
            // 
            this.bsLists.DataMember = "Lists";
            this.bsLists.DataSource = this.dsProcesses;
            // 
            // dsProcesses
            // 
            this.dsProcesses.DataSetName = "ProcessesDataset";
            this.dsProcesses.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.Location = new System.Drawing.Point(6, 31);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(42, 15);
            this.ultraLabel3.TabIndex = 4;
            this.ultraLabel3.Text = "Name:";
            // 
            // tvwTOC
            // 
            this.tvwTOC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tvwTOC.HideSelection = false;
            this.tvwTOC.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tvwTOC.Location = new System.Drawing.Point(8, 8);
            this.tvwTOC.Name = "tvwTOC";
            this.tvwTOC.NodeConnectorColor = System.Drawing.SystemColors.ControlDark;
            _override1.HotTracking = Infragistics.Win.DefaultableBoolean.True;
            _override1.NodeDoubleClickAction = Infragistics.Win.UltraWinTree.NodeDoubleClickAction.ToggleExpansion;
            _override1.Sort = Infragistics.Win.UltraWinTree.SortType.Ascending;
            this.tvwTOC.Override = _override1;
            this.tvwTOC.Size = new System.Drawing.Size(165, 274);
            this.tvwTOC.TabIndex = 0;
            this.tvwTOC.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.tvwTOC_AfterSelect);
            this.tvwTOC.BeforeSelect += new Infragistics.Win.UltraWinTree.BeforeNodeSelectEventHandler(this.tvwTOC_BeforeSelect);
            // 
            // toolbarManager
            // 
            this.toolbarManager.DesignerFlags = 1;
            this.toolbarManager.DockWithinContainer = this;
            this.toolbarManager.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
            this.toolbarManager.ShowFullMenusDelay = 500;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2});
            ultraToolbar1.Text = "Main";
            this.toolbarManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            appearance4.Image = global::DWOS.UI.Properties.Resources.Add_16;
            buttonTool3.SharedPropsInternal.AppearancesSmall.Appearance = appearance4;
            buttonTool3.SharedPropsInternal.Caption = "Add";
            buttonTool3.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool3.SharedPropsInternal.Enabled = false;
            appearance5.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            buttonTool4.SharedPropsInternal.AppearancesSmall.Appearance = appearance5;
            buttonTool4.SharedPropsInternal.Caption = "Delete";
            buttonTool4.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool4.SharedPropsInternal.Enabled = false;
            this.toolbarManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool3,
            buttonTool4});
            // 
            // ListEditor_Fill_Panel
            // 
            this.ListEditor_Fill_Panel.Controls.Add(this.txtListValue);
            this.ListEditor_Fill_Panel.Controls.Add(this.helpLink1);
            this.ListEditor_Fill_Panel.Controls.Add(this.pnlInfo);
            this.ListEditor_Fill_Panel.Controls.Add(this.tvwTOC);
            this.ListEditor_Fill_Panel.Controls.Add(this.btnCancel);
            this.ListEditor_Fill_Panel.Controls.Add(this.btnOK);
            this.ListEditor_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.ListEditor_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListEditor_Fill_Panel.Location = new System.Drawing.Point(0, 25);
            this.ListEditor_Fill_Panel.Name = "ListEditor_Fill_Panel";
            this.ListEditor_Fill_Panel.Size = new System.Drawing.Size(448, 323);
            this.inboxControlStyler1.SetStyleSettings(this.ListEditor_Fill_Panel, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.ListEditor_Fill_Panel.TabIndex = 0;
            // 
            // helpLink1
            // 
            this.helpLink1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = "list_manager_dialog.htm";
            this.helpLink1.Location = new System.Drawing.Point(8, 295);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 7;
            // 
            // pnlInfo
            // 
            this.pnlInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlInfo.Controls.Add(this.grpProcessInformation);
            this.pnlInfo.Location = new System.Drawing.Point(176, 6);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Size = new System.Drawing.Size(267, 277);
            this.inboxControlStyler1.SetStyleSettings(this.pnlInfo, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.pnlInfo.TabIndex = 6;
            // 
            // _ListEditor_Toolbars_Dock_Area_Left
            // 
            this._ListEditor_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ListEditor_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._ListEditor_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._ListEditor_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ListEditor_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 25);
            this._ListEditor_Toolbars_Dock_Area_Left.Name = "_ListEditor_Toolbars_Dock_Area_Left";
            this._ListEditor_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 323);
            this._ListEditor_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarManager;
            // 
            // _ListEditor_Toolbars_Dock_Area_Right
            // 
            this._ListEditor_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ListEditor_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._ListEditor_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._ListEditor_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ListEditor_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(448, 25);
            this._ListEditor_Toolbars_Dock_Area_Right.Name = "_ListEditor_Toolbars_Dock_Area_Right";
            this._ListEditor_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 323);
            this._ListEditor_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarManager;
            // 
            // _ListEditor_Toolbars_Dock_Area_Top
            // 
            this._ListEditor_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ListEditor_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._ListEditor_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._ListEditor_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ListEditor_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._ListEditor_Toolbars_Dock_Area_Top.Name = "_ListEditor_Toolbars_Dock_Area_Top";
            this._ListEditor_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(448, 25);
            this._ListEditor_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarManager;
            // 
            // _ListEditor_Toolbars_Dock_Area_Bottom
            // 
            this._ListEditor_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ListEditor_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._ListEditor_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._ListEditor_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ListEditor_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 348);
            this._ListEditor_Toolbars_Dock_Area_Bottom.Name = "_ListEditor_Toolbars_Dock_Area_Bottom";
            this._ListEditor_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(448, 0);
            this._ListEditor_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarManager;
            // 
            // errProvider
            // 
            this.errProvider.ContainerControl = this;
            // 
            // taLists
            // 
            this.taLists.ClearBeforeFill = true;
            // 
            // taListValues
            // 
            this.taListValues.ClearBeforeFill = true;
            // 
            // taManager
            // 
            this.taManager.BackupDataSetBeforeUpdate = false;
            this.taManager.CustomerProcessAliasTableAdapter = null;
            this.taManager.d_DepartmentTableAdapter = null;
            this.taManager.d_ProcessCategoryTableAdapter = null;
            this.taManager.ListsTableAdapter = this.taLists;
            this.taManager.ListValuesTableAdapter = this.taListValues;
            this.taManager.NumericUnitsTableAdapter = null;
            this.taManager.ProcessAliasDocumentLinkTableAdapter = null;
            this.taManager.ProcessAliasSearchTableAdapter = null;
            this.taManager.ProcessAliasTableAdapter = null;
            this.taManager.ProcessDocumentLinkTableAdapter = null;
            this.taManager.ProcessInspectionsTableAdapter = null;
            this.taManager.ProcessQuestionTableAdapter = null;
            this.taManager.ProcessRequisiteTableAdapter = null;
            this.taManager.ProcessSearchTableAdapter = null;
            this.taManager.ProcessStepConditionTableAdapter = null;
            this.taManager.ProcessStepDocumentLinkTableAdapter = null;
            this.taManager.ProcessStepsTableAdapter = null;
            this.taManager.ProcessTableAdapter = null;
            this.taManager.UpdateOrder = DWOS.Data.Datasets.ProcessesDatasetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            // 
            // taPartInspectionLists
            // 
            this.taPartInspectionLists.ClearBeforeFill = true;
            // 
            // ListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 348);
            this.Controls.Add(this.ListEditor_Fill_Panel);
            this.Controls.Add(this._ListEditor_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._ListEditor_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._ListEditor_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this._ListEditor_Toolbars_Dock_Area_Top);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "ListEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "List Manager";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ListEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grpProcessInformation)).EndInit();
            this.grpProcessInformation.ResumeLayout(false);
            this.grpProcessInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvwValues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtListValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtListName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsLists)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsProcesses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvwTOC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).EndInit();
            this.ListEditor_Fill_Panel.ResumeLayout(false);
            this.ListEditor_Fill_Panel.PerformLayout();
            this.pnlInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnCancel;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.Misc.UltraGroupBox grpProcessInformation;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtListName;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
		private Infragistics.Win.UltraWinTree.UltraTree tvwTOC;
		private Infragistics.Win.UltraWinListView.UltraListView lvwValues;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager toolbarManager;
		private System.Windows.Forms.Panel ListEditor_Fill_Panel;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ListEditor_Toolbars_Dock_Area_Left;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ListEditor_Toolbars_Dock_Area_Right;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ListEditor_Toolbars_Dock_Area_Top;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ListEditor_Toolbars_Dock_Area_Bottom;
		private System.Windows.Forms.ErrorProvider errProvider;
		private System.Windows.Forms.BindingSource bsLists;
		private DWOS.Data.Datasets.ProcessesDatasetTableAdapters.ListsTableAdapter taLists;
		private DWOS.Data.Datasets.ProcessesDatasetTableAdapters.ListValuesTableAdapter taListValues;
		private DWOS.Data.Datasets.ProcessesDataset dsProcesses;
		private System.Windows.Forms.Panel pnlInfo;
		private Infragistics.Win.Misc.UltraButton btnDelete;
		private Infragistics.Win.Misc.UltraButton btnAdd;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
        private Utilities.HelpLink helpLink1;
        private Data.Datasets.ProcessesDatasetTableAdapters.TableAdapterManager taManager;
        private Data.Datasets.PartInspectionDataSetTableAdapters.ListsTableAdapter taPartInspectionLists;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtListValue;
    }
}