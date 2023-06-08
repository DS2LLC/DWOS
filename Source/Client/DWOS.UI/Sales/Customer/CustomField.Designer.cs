namespace DWOS.UI.Sales.Customer
{
	partial class CustomField
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
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo10 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The name of the custom field that will be shown in Order Entry.", Infragistics.Win.ToolTipImage.Default, "Display Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo9 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("If checked, then this custom field and its value will be printed on the WO Travel" +
        "er.", Infragistics.Win.ToolTipImage.Default, "Print on WO Traveler", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo8 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The name of the field\'s token. Part Mark templates and Process Manager use this v" +
        "alue. This value should not contain a \'<\' or \'>\'.", Infragistics.Win.ToolTipImage.Default, "Token Name", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo7 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("If checked, then this custom field and its value will be printed on the COC.", Infragistics.Win.ToolTipImage.Default, "Print on COC", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("A description of the custom field.", Infragistics.Win.ToolTipImage.Default, "Description", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("If checked, the this custom field and its value will be required in Order Entry.", Infragistics.Win.ToolTipImage.Default, "Required in Order Entry.", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The default value to use for this field when creating a new work order.", Infragistics.Win.ToolTipImage.Default, "Default Value", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Process Unique", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Is Visible", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomField));
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("ShowListDialog");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton2 = new Infragistics.Win.UltraWinEditors.EditorButton("Delete");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("List to show for this custom field in Order Entry.", Infragistics.Win.ToolTipImage.Default, "List", Infragistics.Win.DefaultableBoolean.Default);
            this.txtName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.chkPrintTraveler = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtTokenName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.chkPrintCOC = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtDescription = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.chkRequired = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtDefaultValue = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.chkProcessUnique = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkVisible = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cboList = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintTraveler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTokenName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintCOC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRequired)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDefaultValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkProcessUnique)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkVisible)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboList)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.ultraLabel5);
            this.grpData.Controls.Add(this.cboList);
            this.grpData.Controls.Add(this.chkVisible);
            this.grpData.Controls.Add(this.chkProcessUnique);
            this.grpData.Controls.Add(this.txtDefaultValue);
            this.grpData.Controls.Add(this.ultraLabel3);
            this.grpData.Controls.Add(this.chkRequired);
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.txtDescription);
            this.grpData.Controls.Add(this.chkPrintCOC);
            this.grpData.Controls.Add(this.ultraLabel1);
            this.grpData.Controls.Add(this.txtTokenName);
            this.grpData.Controls.Add(this.chkPrintTraveler);
            this.grpData.Controls.Add(this.ultraLabel4);
            this.grpData.Controls.Add(this.txtName);
            appearance3.Image = global::DWOS.UI.Properties.Resources.CustomField_16;
            this.grpData.HeaderAppearance = appearance3;
            this.grpData.Size = new System.Drawing.Size(339, 304);
            this.grpData.Text = "Custom Field";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.txtName, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel4, 0);
            this.grpData.Controls.SetChildIndex(this.chkPrintTraveler, 0);
            this.grpData.Controls.SetChildIndex(this.txtTokenName, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.chkPrintCOC, 0);
            this.grpData.Controls.SetChildIndex(this.txtDescription, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.grpData.Controls.SetChildIndex(this.chkRequired, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.grpData.Controls.SetChildIndex(this.txtDefaultValue, 0);
            this.grpData.Controls.SetChildIndex(this.chkProcessUnique, 0);
            this.grpData.Controls.SetChildIndex(this.chkVisible, 0);
            this.grpData.Controls.SetChildIndex(this.cboList, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel5, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(-224, -1524);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(100, 33);
            this.txtName.Name = "txtName";
            this.txtName.Nullable = false;
            this.txtName.Size = new System.Drawing.Size(215, 22);
            this.txtName.TabIndex = 0;
            ultraToolTipInfo10.ToolTipText = "The name of the custom field that will be shown in Order Entry.";
            ultraToolTipInfo10.ToolTipTitle = "Display Name";
            this.tipManager.SetUltraToolTip(this.txtName, ultraToolTipInfo10);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(6, 36);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(88, 15);
            this.ultraLabel4.TabIndex = 36;
            this.ultraLabel4.Text = "Display Name:";
            // 
            // chkPrintTraveler
            // 
            this.chkPrintTraveler.Location = new System.Drawing.Point(114, 174);
            this.chkPrintTraveler.Name = "chkPrintTraveler";
            this.chkPrintTraveler.Size = new System.Drawing.Size(201, 20);
            this.chkPrintTraveler.TabIndex = 5;
            this.chkPrintTraveler.Text = "Print on WO Traveler";
            ultraToolTipInfo9.ToolTipText = "If checked, then this custom field and its value will be printed on the WO Travel" +
    "er.";
            ultraToolTipInfo9.ToolTipTitle = "Print on WO Traveler";
            this.tipManager.SetUltraToolTip(this.chkPrintTraveler, ultraToolTipInfo9);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 92);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(81, 15);
            this.ultraLabel1.TabIndex = 42;
            this.ultraLabel1.Text = "Token Name:";
            // 
            // txtTokenName
            // 
            this.txtTokenName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtTokenName.Location = new System.Drawing.Point(100, 89);
            this.txtTokenName.Name = "txtTokenName";
            this.txtTokenName.Nullable = false;
            this.txtTokenName.Size = new System.Drawing.Size(215, 22);
            this.txtTokenName.TabIndex = 2;
            ultraToolTipInfo8.ToolTipText = "The name of the field\'s token. Part Mark templates and Process Manager use this v" +
    "alue. This value should not contain a \'<\' or \'>\'.";
            ultraToolTipInfo8.ToolTipTitle = "Token Name";
            this.tipManager.SetUltraToolTip(this.txtTokenName, ultraToolTipInfo8);
            // 
            // chkPrintCOC
            // 
            this.chkPrintCOC.Location = new System.Drawing.Point(114, 197);
            this.chkPrintCOC.Name = "chkPrintCOC";
            this.chkPrintCOC.Size = new System.Drawing.Size(201, 20);
            this.chkPrintCOC.TabIndex = 6;
            this.chkPrintCOC.Text = "Print on COC";
            ultraToolTipInfo7.ToolTipText = "If checked, then this custom field and its value will be printed on the COC.";
            ultraToolTipInfo7.ToolTipTitle = "Print on COC";
            this.tipManager.SetUltraToolTip(this.chkPrintCOC, ultraToolTipInfo7);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(6, 64);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(73, 15);
            this.ultraLabel2.TabIndex = 45;
            this.ultraLabel2.Text = "Description:";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(100, 61);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Nullable = false;
            this.txtDescription.Size = new System.Drawing.Size(215, 22);
            this.txtDescription.TabIndex = 1;
            ultraToolTipInfo6.ToolTipText = "A description of the custom field.";
            ultraToolTipInfo6.ToolTipTitle = "Description";
            this.tipManager.SetUltraToolTip(this.txtDescription, ultraToolTipInfo6);
            // 
            // chkRequired
            // 
            this.chkRequired.AutoSize = true;
            this.chkRequired.Location = new System.Drawing.Point(114, 248);
            this.chkRequired.Name = "chkRequired";
            this.chkRequired.Size = new System.Drawing.Size(157, 18);
            this.chkRequired.TabIndex = 8;
            this.chkRequired.Text = "Required in Order Entry";
            ultraToolTipInfo5.ToolTipText = "If checked, the this custom field and its value will be required in Order Entry.";
            ultraToolTipInfo5.ToolTipTitle = "Required in Order Entry.";
            this.tipManager.SetUltraToolTip(this.chkRequired, ultraToolTipInfo5);
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(6, 122);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(86, 15);
            this.ultraLabel3.TabIndex = 46;
            this.ultraLabel3.Text = "Default Value:";
            // 
            // txtDefaultValue
            // 
            this.txtDefaultValue.Location = new System.Drawing.Point(100, 118);
            this.txtDefaultValue.MaxLength = 255;
            this.txtDefaultValue.Name = "txtDefaultValue";
            this.txtDefaultValue.Size = new System.Drawing.Size(215, 22);
            this.txtDefaultValue.TabIndex = 3;
            ultraToolTipInfo4.ToolTipText = "The default value to use for this field when creating a new work order.";
            ultraToolTipInfo4.ToolTipTitle = "Default Value";
            this.tipManager.SetUltraToolTip(this.txtDefaultValue, ultraToolTipInfo4);
            // 
            // chkProcessUnique
            // 
            this.chkProcessUnique.AutoSize = true;
            this.chkProcessUnique.Location = new System.Drawing.Point(114, 223);
            this.chkProcessUnique.Name = "chkProcessUnique";
            this.chkProcessUnique.Size = new System.Drawing.Size(108, 18);
            this.chkProcessUnique.TabIndex = 7;
            this.chkProcessUnique.Text = "Process Unique";
            ultraToolTipInfo3.ToolTipTextFormatted = "If <b>checked</b>, order processing will show the value for this field.<br/>If <b" +
    ">unchecked</b> (default), order processing will not show this custom field.";
            ultraToolTipInfo3.ToolTipTitle = "Process Unique";
            this.tipManager.SetUltraToolTip(this.chkProcessUnique, ultraToolTipInfo3);
            // 
            // chkVisible
            // 
            this.chkVisible.AutoSize = true;
            this.chkVisible.Location = new System.Drawing.Point(114, 274);
            this.chkVisible.Name = "chkVisible";
            this.chkVisible.Size = new System.Drawing.Size(73, 18);
            this.chkVisible.TabIndex = 9;
            this.chkVisible.Text = "Is Visible";
            ultraToolTipInfo2.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo2.ToolTipTextFormatted");
            ultraToolTipInfo2.ToolTipTitle = "Is Visible";
            this.tipManager.SetUltraToolTip(this.chkVisible, ultraToolTipInfo2);
            // 
            // cboList
            // 
            appearance1.Image = global::DWOS.UI.Properties.Resources.Add_16;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            editorButton1.Appearance = appearance1;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton1.Key = "ShowListDialog";
            this.cboList.ButtonsLeft.Add(editorButton1);
            appearance2.Image = global::DWOS.UI.Properties.Resources.Delete_16;
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Middle;
            editorButton2.Appearance = appearance2;
            editorButton2.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            editorButton2.Key = "Delete";
            this.cboList.ButtonsRight.Add(editorButton2);
            this.cboList.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboList.Location = new System.Drawing.Point(100, 146);
            this.cboList.Name = "cboList";
            this.cboList.Size = new System.Drawing.Size(215, 22);
            this.cboList.TabIndex = 4;
            ultraToolTipInfo1.ToolTipText = "List to show for this custom field in Order Entry.";
            ultraToolTipInfo1.ToolTipTitle = "List";
            this.tipManager.SetUltraToolTip(this.cboList, ultraToolTipInfo1);
            this.cboList.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.cboList_EditorButtonClick);
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(6, 150);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(29, 15);
            this.ultraLabel5.TabIndex = 48;
            this.ultraLabel5.Text = "List:";
            // 
            // CustomField
            // 
            this.Name = "CustomField";
            this.Size = new System.Drawing.Size(345, 310);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintTraveler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTokenName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintCOC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRequired)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDefaultValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkProcessUnique)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkVisible)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboList)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtName;
		private Infragistics.Win.Misc.UltraLabel ultraLabel4;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPrintTraveler;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDescription;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPrintCOC;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTokenName;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkRequired;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDefaultValue;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkProcessUnique;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkVisible;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboList;
    }
}
