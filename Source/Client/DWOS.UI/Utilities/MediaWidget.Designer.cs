namespace DWOS.UI.Utilities
{
    partial class MediaWidget
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
            OnDisposing();

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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Delete Item", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Media List", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to capture the current image displayed in the preview.", Infragistics.Win.ToolTipImage.Default, "Capture", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MediaWidget));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Item Preview", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Add a new media item.", Infragistics.Win.ToolTipImage.Default, "Add New Item", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("AddMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddMedia");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("LinkDocument");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddMedia");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("LinkDocument");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Rename");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("EditExtension");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("EditMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Rename");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("EditExtension");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Captures a document using a scanner or webcam.", Infragistics.Win.ToolTipImage.Default, "Capture Document", Infragistics.Win.DefaultableBoolean.Default);
            this.btnDelete = new Infragistics.Win.Misc.UltraButton();
            this.tvwMedia = new Infragistics.Win.UltraWinTree.UltraTree();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.scannerProperties1 = new DWOS.UI.Utilities.ScannerProperties();
            this.pnlWebCam = new System.Windows.Forms.Panel();
            this.picWebCamPreview = new System.Windows.Forms.PictureBox();
            this.btnSave = new Infragistics.Win.Misc.UltraButton();
            this.picPartImage = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.btnAddMedia = new Infragistics.Win.Misc.UltraDropDownButton();
            this.ultraToolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this.btnScanner = new Infragistics.Win.Misc.UltraDropDownButton();
            this.popScannerProps = new Infragistics.Win.Misc.UltraPopupControlContainer(this.components);
            this.MediaWidget_Fill_Panel = new System.Windows.Forms.Panel();
            this._MediaWidget_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._MediaWidget_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._MediaWidget_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._MediaWidget_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.popWebCamPreview = new Infragistics.Win.Misc.UltraPopupControlContainer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tvwMedia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlWebCam.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWebCamPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager)).BeginInit();
            this.MediaWidget_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDelete
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            this.btnDelete.Appearance = appearance1;
            this.btnDelete.AutoSize = true;
            this.btnDelete.Location = new System.Drawing.Point(3, 67);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(26, 26);
            this.btnDelete.TabIndex = 3;
            ultraToolTipInfo6.ToolTipTextFormatted = "Delete the selected item.<br/><br/>Permission: [DeleteMedia]";
            ultraToolTipInfo6.ToolTipTitle = "Delete Item";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnDelete, ultraToolTipInfo6);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // tvwMedia
            // 
            this.tvwMedia.AllowDrop = true;
            this.ultraToolbarsManager.SetContextMenuUltra(this.tvwMedia, "EditMenu");
            this.tvwMedia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwMedia.HideSelection = false;
            this.tvwMedia.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tvwMedia.Location = new System.Drawing.Point(0, 0);
            this.tvwMedia.Name = "tvwMedia";
            this.tvwMedia.NodeConnectorColor = System.Drawing.SystemColors.ControlDark;
            this.tvwMedia.Size = new System.Drawing.Size(177, 166);
            this.tvwMedia.TabIndex = 4;
            ultraToolTipInfo3.ToolTipTextFormatted = "List of media, double-click name of item to rename it.";
            ultraToolTipInfo3.ToolTipTitle = "Media List";
            this.ultraToolTipManager1.SetUltraToolTip(this.tvwMedia, ultraToolTipInfo3);
            this.tvwMedia.AfterCheck += new Infragistics.Win.UltraWinTree.AfterNodeChangedEventHandler(this.tvwMedia_AfterCheck);
            this.tvwMedia.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.tvwMedia_AfterSelect);
            this.tvwMedia.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvwMedia_DragDrop);
            this.tvwMedia.DragEnter += new System.Windows.Forms.DragEventHandler(this.tvwMedia_DragEnter);
            this.tvwMedia.DoubleClick += new System.EventHandler(this.tvwMedia_DoubleClick);
            this.tvwMedia.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvwMedia_MouseUp);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(48, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvwMedia);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.scannerProperties1);
            this.splitContainer1.Panel2.Controls.Add(this.pnlWebCam);
            this.splitContainer1.Panel2.Controls.Add(this.picPartImage);
            this.splitContainer1.Size = new System.Drawing.Size(416, 166);
            this.splitContainer1.SplitterDistance = 177;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 25;
            // 
            // scannerProperties1
            // 
            this.scannerProperties1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scannerProperties1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scannerProperties1.Location = new System.Drawing.Point(48, 128);
            this.scannerProperties1.Name = "scannerProperties1";
            this.scannerProperties1.Size = new System.Drawing.Size(268, 146);
            this.scannerProperties1.TabIndex = 3;
            this.scannerProperties1.Visible = false;
            // 
            // pnlWebCam
            // 
            this.pnlWebCam.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlWebCam.Controls.Add(this.picWebCamPreview);
            this.pnlWebCam.Controls.Add(this.btnSave);
            this.pnlWebCam.Location = new System.Drawing.Point(48, 3);
            this.pnlWebCam.Name = "pnlWebCam";
            this.pnlWebCam.Size = new System.Drawing.Size(162, 100);
            this.pnlWebCam.TabIndex = 4;
            this.pnlWebCam.Visible = false;
            // 
            // picWebCamPreview
            // 
            this.picWebCamPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picWebCamPreview.Location = new System.Drawing.Point(3, 3);
            this.picWebCamPreview.Name = "picWebCamPreview";
            this.picWebCamPreview.Size = new System.Drawing.Size(156, 68);
            this.picWebCamPreview.TabIndex = 1;
            this.picWebCamPreview.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSave.Location = new System.Drawing.Point(51, 72);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Capture";
            ultraToolTipInfo4.ToolTipText = "Click to capture the current image displayed in the preview.";
            ultraToolTipInfo4.ToolTipTitle = "Capture";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnSave, ultraToolTipInfo4);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // picPartImage
            // 
            appearance4.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance4.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.picPartImage.Appearance = appearance4;
            this.picPartImage.BorderShadowColor = System.Drawing.Color.Empty;
            this.picPartImage.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.picPartImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPartImage.DefaultImage = global::DWOS.UI.Properties.Resources.NoImage;
            this.picPartImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picPartImage.Image = ((object)(resources.GetObject("picPartImage.Image")));
            this.picPartImage.Location = new System.Drawing.Point(0, 0);
            this.picPartImage.Name = "picPartImage";
            this.picPartImage.Size = new System.Drawing.Size(234, 166);
            this.picPartImage.TabIndex = 0;
            ultraToolTipInfo5.ToolTipTextFormatted = "Click the item preview to open it locally.";
            ultraToolTipInfo5.ToolTipTitle = "Item Preview";
            this.ultraToolTipManager1.SetUltraToolTip(this.picPartImage, ultraToolTipInfo5);
            this.picPartImage.Click += new System.EventHandler(this.picPartImage_Click);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // btnAddMedia
            // 
            appearance2.Image = global::DWOS.UI.Properties.Resources.Add_16;
            this.btnAddMedia.Appearance = appearance2;
            this.btnAddMedia.AutoSize = true;
            this.btnAddMedia.Location = new System.Drawing.Point(3, 3);
            this.btnAddMedia.Name = "btnAddMedia";
            this.btnAddMedia.PopupItemKey = "AddMenu";
            this.btnAddMedia.PopupItemProvider = this.ultraToolbarsManager;
            this.btnAddMedia.Size = new System.Drawing.Size(39, 26);
            this.btnAddMedia.TabIndex = 1;
            ultraToolTipInfo1.ToolTipText = "Add a new media item.";
            ultraToolTipInfo1.ToolTipTitle = "Add New Item";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnAddMedia, ultraToolTipInfo1);
            this.btnAddMedia.Click += new System.EventHandler(this.btnAddMedia_Click);
            // 
            // ultraToolbarsManager
            // 
            this.ultraToolbarsManager.DesignerFlags = 1;
            this.ultraToolbarsManager.DockWithinContainer = this;
            this.ultraToolbarsManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedPropsInternal.Caption = "AddMenu";
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool3,
            buttonTool4});
            buttonTool1.SharedPropsInternal.Caption = "Add Media";
            buttonTool2.SharedPropsInternal.Caption = "Link Document";
            buttonTool5.SharedPropsInternal.Caption = "Rename";
            buttonTool6.SharedPropsInternal.Caption = "Edit Extension";
            popupMenuTool2.SharedPropsInternal.Caption = "EditMenu";
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool8,
            buttonTool9});
            this.ultraToolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool1,
            buttonTool2,
            buttonTool5,
            buttonTool6,
            popupMenuTool2});
            this.ultraToolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.ultraToolbarsManager_ToolClick);
            // 
            // btnScanner
            // 
            appearance3.Image = global::DWOS.UI.Properties.Resources.Scanner_16;
            this.btnScanner.Appearance = appearance3;
            this.btnScanner.AutoSize = true;
            this.btnScanner.Location = new System.Drawing.Point(3, 35);
            this.btnScanner.Name = "btnScanner";
            this.btnScanner.PopupItemKey = "scannerProperties1";
            this.btnScanner.PopupItemProvider = this.popScannerProps;
            this.btnScanner.Size = new System.Drawing.Size(39, 26);
            this.btnScanner.TabIndex = 2;
            ultraToolTipInfo2.ToolTipText = "Captures a document using a scanner or webcam.";
            ultraToolTipInfo2.ToolTipTitle = "Capture Document";
            this.ultraToolTipManager1.SetUltraToolTip(this.btnScanner, ultraToolTipInfo2);
            this.btnScanner.DroppingDown += new System.ComponentModel.CancelEventHandler(this.btnScanner_DroppingDown);
            this.btnScanner.ClosedUp += new System.EventHandler(this.btnScanner_ClosedUp);
            this.btnScanner.Click += new System.EventHandler(this.btnScanner_Click);
            // 
            // popScannerProps
            // 
            this.popScannerProps.PopupControl = this.scannerProperties1;
            // 
            // MediaWidget_Fill_Panel
            // 
            this.MediaWidget_Fill_Panel.Controls.Add(this.btnAddMedia);
            this.MediaWidget_Fill_Panel.Controls.Add(this.btnScanner);
            this.MediaWidget_Fill_Panel.Controls.Add(this.splitContainer1);
            this.MediaWidget_Fill_Panel.Controls.Add(this.btnDelete);
            this.MediaWidget_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.MediaWidget_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MediaWidget_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.MediaWidget_Fill_Panel.Name = "MediaWidget_Fill_Panel";
            this.MediaWidget_Fill_Panel.Size = new System.Drawing.Size(464, 166);
            this.MediaWidget_Fill_Panel.TabIndex = 0;
            // 
            // _MediaWidget_Toolbars_Dock_Area_Left
            // 
            this._MediaWidget_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MediaWidget_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._MediaWidget_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._MediaWidget_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._MediaWidget_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._MediaWidget_Toolbars_Dock_Area_Left.Name = "_MediaWidget_Toolbars_Dock_Area_Left";
            this._MediaWidget_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 166);
            this._MediaWidget_Toolbars_Dock_Area_Left.ToolbarsManager = this.ultraToolbarsManager;
            // 
            // _MediaWidget_Toolbars_Dock_Area_Right
            // 
            this._MediaWidget_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MediaWidget_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._MediaWidget_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._MediaWidget_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._MediaWidget_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(464, 0);
            this._MediaWidget_Toolbars_Dock_Area_Right.Name = "_MediaWidget_Toolbars_Dock_Area_Right";
            this._MediaWidget_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 166);
            this._MediaWidget_Toolbars_Dock_Area_Right.ToolbarsManager = this.ultraToolbarsManager;
            // 
            // _MediaWidget_Toolbars_Dock_Area_Top
            // 
            this._MediaWidget_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MediaWidget_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._MediaWidget_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._MediaWidget_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._MediaWidget_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._MediaWidget_Toolbars_Dock_Area_Top.Name = "_MediaWidget_Toolbars_Dock_Area_Top";
            this._MediaWidget_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(464, 0);
            this._MediaWidget_Toolbars_Dock_Area_Top.ToolbarsManager = this.ultraToolbarsManager;
            // 
            // _MediaWidget_Toolbars_Dock_Area_Bottom
            // 
            this._MediaWidget_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MediaWidget_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._MediaWidget_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._MediaWidget_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._MediaWidget_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 166);
            this._MediaWidget_Toolbars_Dock_Area_Bottom.Name = "_MediaWidget_Toolbars_Dock_Area_Bottom";
            this._MediaWidget_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(464, 0);
            this._MediaWidget_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.ultraToolbarsManager;
            // 
            // popWebCamPreview
            // 
            this.popWebCamPreview.PopupControl = this.pnlWebCam;
            this.popWebCamPreview.Closed += new System.EventHandler(this.popWebCamPreview_Closed);
            // 
            // MediaWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MediaWidget_Fill_Panel);
            this.Controls.Add(this._MediaWidget_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._MediaWidget_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._MediaWidget_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this._MediaWidget_Toolbars_Dock_Area_Top);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MediaWidget";
            this.Size = new System.Drawing.Size(464, 166);
            ((System.ComponentModel.ISupportInitialize)(this.tvwMedia)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.pnlWebCam.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picWebCamPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager)).EndInit();
            this.MediaWidget_Fill_Panel.ResumeLayout(false);
            this.MediaWidget_Fill_Panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnDelete;
        private Infragistics.Win.UltraWinTree.UltraTree tvwMedia;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picPartImage;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.Misc.UltraDropDownButton btnScanner;
        private ScannerProperties scannerProperties1;
        private Infragistics.Win.Misc.UltraPopupControlContainer popScannerProps;
        private Infragistics.Win.Misc.UltraDropDownButton btnAddMedia;
        private System.Windows.Forms.Panel MediaWidget_Fill_Panel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _MediaWidget_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager ultraToolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _MediaWidget_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _MediaWidget_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _MediaWidget_Toolbars_Dock_Area_Top;
        private System.Windows.Forms.PictureBox picWebCamPreview;
        private Infragistics.Win.Misc.UltraPopupControlContainer popWebCamPreview;
        private Infragistics.Win.Misc.UltraButton btnSave;
        private System.Windows.Forms.Panel pnlWebCam;
    }
}
