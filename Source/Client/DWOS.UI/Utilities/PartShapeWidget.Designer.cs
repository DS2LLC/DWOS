namespace DWOS.UI.Utilities
{
    using Infragistics.Win.UltraWinToolTip;

    partial class PartShapeWidget
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
            DisposeMe();

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
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint7 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Select the part\'s shape.", Infragistics.Win.ToolTipImage.Default, "Part Shape", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton4 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint6 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Total height of the part.", Infragistics.Win.ToolTipImage.Default, "Height", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton2 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint4 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Total surface area for the part.", Infragistics.Win.ToolTipImage.Default, "Surface Area", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton3 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint5 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Total width of the part.", Infragistics.Win.ToolTipImage.Default, "Width", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint3 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Total length of the part.", Infragistics.Win.ToolTipImage.Default, "Length", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint1 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Mode", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint2 = new Infragistics.Win.Layout.GridBagConstraint();
            this.cboPartShape = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.numHeight = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.numSurfaceArea = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.numWidth = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.numLength = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraGridBagLayoutPanel1 = new Infragistics.Win.Misc.UltraGridBagLayoutPanel();
            this.cboEditorType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.btnShowEditor = new Infragistics.Win.Misc.UltraButton();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.cboPartShape)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSurfaceArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutPanel1)).BeginInit();
            this.ultraGridBagLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboEditorType)).BeginInit();
            this.SuspendLayout();
            // 
            // cboPartShape
            // 
            this.cboPartShape.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            gridBagConstraint7.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint7.Insets.Right = 10;
            gridBagConstraint7.OriginX = 1;
            gridBagConstraint7.OriginY = 0;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.cboPartShape, gridBagConstraint7);
            this.cboPartShape.Location = new System.Drawing.Point(95, 0);
            this.cboPartShape.Name = "cboPartShape";
            this.cboPartShape.NullText = "<Default Shape>";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.cboPartShape, new System.Drawing.Size(176, 22));
            this.cboPartShape.Size = new System.Drawing.Size(104, 22);
            this.cboPartShape.TabIndex = 1;
            ultraToolTipInfo6.ToolTipText = "Select the part\'s shape.";
            ultraToolTipInfo6.ToolTipTitle = "Part Shape";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboPartShape, ultraToolTipInfo6);
            this.cboPartShape.ValueChanged += new System.EventHandler(this.cboPartShape_ValueChanged);
            // 
            // numHeight
            // 
            this.numHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance4.TextHAlignAsString = "Center";
            editorButton4.Appearance = appearance4;
            editorButton4.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2010Button;
            editorButton4.Enabled = false;
            editorButton4.Text = "H";
            editorButton4.Width = 16;
            this.numHeight.ButtonsLeft.Add(editorButton4);
            gridBagConstraint6.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint6.Insets.Top = 10;
            gridBagConstraint6.OriginX = 2;
            gridBagConstraint6.OriginY = 1;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.numHeight, gridBagConstraint6);
            this.numHeight.Location = new System.Drawing.Point(209, 32);
            this.numHeight.MaskInput = "nnnn.nnn";
            this.numHeight.MaxValue = 9999.99D;
            this.numHeight.MinValue = 0D;
            this.numHeight.Name = "numHeight";
            this.numHeight.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.numHeight, new System.Drawing.Size(170, 22));
            this.numHeight.Size = new System.Drawing.Size(100, 22);
            this.numHeight.TabIndex = 5;
            this.numHeight.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextControl;
            ultraToolTipInfo5.ToolTipText = "Total height of the part.";
            ultraToolTipInfo5.ToolTipTitle = "Height";
            this.ultraToolTipManager1.SetUltraToolTip(this.numHeight, ultraToolTipInfo5);
            this.numHeight.ValueChanged += new System.EventHandler(this.numEditor_ValueChanged);
            // 
            // numSurfaceArea
            // 
            this.numSurfaceArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.TextHAlignAsString = "Center";
            editorButton2.Appearance = appearance2;
            editorButton2.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2010Button;
            editorButton2.Enabled = false;
            editorButton2.Text = "SA";
            editorButton2.Width = 24;
            this.numSurfaceArea.ButtonsLeft.Add(editorButton2);
            gridBagConstraint4.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint4.OriginX = 2;
            gridBagConstraint4.OriginY = 0;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.numSurfaceArea, gridBagConstraint4);
            this.numSurfaceArea.Location = new System.Drawing.Point(209, 0);
            this.numSurfaceArea.MaskInput = "nnnnnnn.nnn";
            this.numSurfaceArea.MaxValue = 9999999D;
            this.numSurfaceArea.MinValue = 0D;
            this.numSurfaceArea.Name = "numSurfaceArea";
            this.numSurfaceArea.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.numSurfaceArea, new System.Drawing.Size(170, 22));
            this.numSurfaceArea.Size = new System.Drawing.Size(100, 22);
            this.numSurfaceArea.TabIndex = 2;
            this.numSurfaceArea.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextControl;
            ultraToolTipInfo3.ToolTipText = "Total surface area for the part.";
            ultraToolTipInfo3.ToolTipTitle = "Surface Area";
            this.ultraToolTipManager1.SetUltraToolTip(this.numSurfaceArea, ultraToolTipInfo3);
            // 
            // numWidth
            // 
            appearance3.TextHAlignAsString = "Center";
            editorButton3.Appearance = appearance3;
            editorButton3.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2010Button;
            editorButton3.Enabled = false;
            editorButton3.Text = "W";
            editorButton3.Width = 16;
            this.numWidth.ButtonsLeft.Add(editorButton3);
            gridBagConstraint5.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint5.Insets.Right = 10;
            gridBagConstraint5.Insets.Top = 10;
            gridBagConstraint5.OriginX = 1;
            gridBagConstraint5.OriginY = 1;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.numWidth, gridBagConstraint5);
            this.numWidth.Location = new System.Drawing.Point(95, 32);
            this.numWidth.MaskInput = "nnnn.nnn";
            this.numWidth.MaxValue = 9999.99D;
            this.numWidth.MinValue = 0D;
            this.numWidth.Name = "numWidth";
            this.numWidth.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.numWidth, new System.Drawing.Size(176, 22));
            this.numWidth.Size = new System.Drawing.Size(104, 22);
            this.numWidth.TabIndex = 4;
            this.numWidth.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextControl;
            ultraToolTipInfo4.ToolTipText = "Total width of the part.";
            ultraToolTipInfo4.ToolTipTitle = "Width";
            this.ultraToolTipManager1.SetUltraToolTip(this.numWidth, ultraToolTipInfo4);
            this.numWidth.ValueChanged += new System.EventHandler(this.numEditor_ValueChanged);
            // 
            // numLength
            // 
            appearance1.TextHAlignAsString = "Center";
            editorButton1.Appearance = appearance1;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2010Button;
            editorButton1.Enabled = false;
            editorButton1.Text = "L";
            editorButton1.Width = 16;
            this.numLength.ButtonsLeft.Add(editorButton1);
            gridBagConstraint3.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint3.Insets.Right = 10;
            gridBagConstraint3.Insets.Top = 10;
            gridBagConstraint3.OriginX = 0;
            gridBagConstraint3.OriginY = 1;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.numLength, gridBagConstraint3);
            this.numLength.Location = new System.Drawing.Point(0, 32);
            this.numLength.MaskInput = "nnnn.nnn";
            this.numLength.MaxValue = 9999.99D;
            this.numLength.MinValue = 0D;
            this.numLength.Name = "numLength";
            this.numLength.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.numLength, new System.Drawing.Size(143, 22));
            this.numLength.Size = new System.Drawing.Size(85, 22);
            this.numLength.TabIndex = 3;
            this.numLength.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextControl;
            ultraToolTipInfo2.ToolTipText = "Total length of the part.";
            ultraToolTipInfo2.ToolTipTitle = "Length";
            this.ultraToolTipManager1.SetUltraToolTip(this.numLength, ultraToolTipInfo2);
            this.numLength.ValueChanged += new System.EventHandler(this.numEditor_ValueChanged);
            // 
            // ultraGridBagLayoutPanel1
            // 
            this.ultraGridBagLayoutPanel1.Controls.Add(this.cboEditorType);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.btnShowEditor);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.numLength);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.numSurfaceArea);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.cboPartShape);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.numWidth);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.numHeight);
            this.ultraGridBagLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGridBagLayoutPanel1.ExpandToFitWidth = true;
            this.ultraGridBagLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.ultraGridBagLayoutPanel1.Name = "ultraGridBagLayoutPanel1";
            this.ultraGridBagLayoutPanel1.Size = new System.Drawing.Size(309, 87);
            this.ultraGridBagLayoutPanel1.TabIndex = 23;
            // 
            // cboEditorType
            // 
            this.cboEditorType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            gridBagConstraint1.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint1.Insets.Right = 10;
            gridBagConstraint1.OriginX = 0;
            gridBagConstraint1.OriginY = 0;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.cboEditorType, gridBagConstraint1);
            this.cboEditorType.Location = new System.Drawing.Point(0, 0);
            this.cboEditorType.Name = "cboEditorType";
            this.cboEditorType.NullText = "<Mode>";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.cboEditorType, new System.Drawing.Size(144, 22));
            this.cboEditorType.Size = new System.Drawing.Size(85, 22);
            this.cboEditorType.TabIndex = 0;
            ultraToolTipInfo1.ToolTipTextFormatted = "Select the surface area calculator to use for the part.";
            ultraToolTipInfo1.ToolTipTitle = "Mode";
            this.ultraToolTipManager1.SetUltraToolTip(this.cboEditorType, ultraToolTipInfo1);
            this.cboEditorType.ValueChanged += new System.EventHandler(this.cboEditorType_ValueChanged);
            // 
            // btnShowEditor
            // 
            this.btnShowEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            gridBagConstraint2.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint2.Insets.Top = 10;
            gridBagConstraint2.OriginX = 0;
            gridBagConstraint2.OriginY = 2;
            gridBagConstraint2.SpanX = 3;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.btnShowEditor, gridBagConstraint2);
            this.btnShowEditor.Location = new System.Drawing.Point(0, 64);
            this.btnShowEditor.Name = "btnShowEditor";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.btnShowEditor, new System.Drawing.Size(75, 23));
            this.btnShowEditor.Size = new System.Drawing.Size(309, 23);
            this.btnShowEditor.TabIndex = 6;
            this.btnShowEditor.Text = "Edit...";
            this.btnShowEditor.Visible = false;
            this.btnShowEditor.Click += new System.EventHandler(this.btnShowEditor_Click);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // PartShapeWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGridBagLayoutPanel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PartShapeWidget";
            this.Size = new System.Drawing.Size(309, 87);
            ((System.ComponentModel.ISupportInitialize)(this.cboPartShape)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSurfaceArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutPanel1)).EndInit();
            this.ultraGridBagLayoutPanel1.ResumeLayout(false);
            this.ultraGridBagLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboEditorType)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboPartShape;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numHeight;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numSurfaceArea;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numWidth;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numLength;
        private Infragistics.Win.Misc.UltraGridBagLayoutPanel ultraGridBagLayoutPanel1;
        private UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.Misc.UltraButton btnShowEditor;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboEditorType;
    }
}
