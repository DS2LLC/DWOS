namespace DWOS.UI.Sales
{
    partial class OrderPartMarking
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderPartMarking));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The marking definition for the 4th row.", Infragistics.Win.ToolTipImage.Default, "Marking 4", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The marking definition for the 3rd row.", Infragistics.Win.ToolTipImage.Default, "Marking 3", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The marking definition for the 2nd row.", Infragistics.Win.ToolTipImage.Default, "Marking 2", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The marking definition for the 1st row.", Infragistics.Win.ToolTipImage.Default, "Marking 1", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Enter the specification that this part marking adheres to. This specification wil" +
        "l be added to the CoC.", Infragistics.Win.ToolTipImage.Default, "Process Specification", Infragistics.Win.DefaultableBoolean.Default);
            this.grpLines = new Infragistics.Win.Misc.UltraGroupBox();
            this.picLine4 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.picLine3 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.picLine2 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.picLine1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.txtDef4 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtDef3 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtDef2 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtDef1 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtDate = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lblDate = new Infragistics.Win.Misc.UltraLabel();
            this.txtSpecification = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpLines)).BeginInit();
            this.grpLines.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDef4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDef3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDef2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDef1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSpecification)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.ultraLabel2);
            this.grpData.Controls.Add(this.txtSpecification);
            this.grpData.Controls.Add(this.lblDate);
            this.grpData.Controls.Add(this.txtDate);
            this.grpData.Controls.Add(this.grpLines);
            appearance1.Image = global::DWOS.UI.Properties.Resources.Process_16;
            this.grpData.HeaderAppearance = appearance1;
            this.grpData.Size = new System.Drawing.Size(351, 304);
            this.grpData.Text = "Part Marking";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.grpLines, 0);
            this.grpData.Controls.SetChildIndex(this.txtDate, 0);
            this.grpData.Controls.SetChildIndex(this.lblDate, 0);
            this.grpData.Controls.SetChildIndex(this.txtSpecification, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel2, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(275, -386);
            // 
            // grpLines
            // 
            this.grpLines.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLines.Controls.Add(this.picLine4);
            this.grpLines.Controls.Add(this.picLine3);
            this.grpLines.Controls.Add(this.picLine2);
            this.grpLines.Controls.Add(this.picLine1);
            this.grpLines.Controls.Add(this.txtDef4);
            this.grpLines.Controls.Add(this.txtDef3);
            this.grpLines.Controls.Add(this.txtDef2);
            this.grpLines.Controls.Add(this.txtDef1);
            this.grpLines.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpLines.Location = new System.Drawing.Point(3, 84);
            this.grpLines.Name = "grpLines";
            this.grpLines.Size = new System.Drawing.Size(339, 146);
            this.grpLines.TabIndex = 44;
            this.grpLines.Text = "Marking Lines";
            // 
            // picLine4
            // 
            this.picLine4.BorderShadowColor = System.Drawing.Color.Empty;
            this.picLine4.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.picLine4.Image = ((object)(resources.GetObject("picLine4.Image")));
            this.picLine4.Location = new System.Drawing.Point(8, 111);
            this.picLine4.Name = "picLine4";
            this.picLine4.Size = new System.Drawing.Size(24, 24);
            this.picLine4.TabIndex = 36;
            this.picLine4.UseAppStyling = false;
            // 
            // picLine3
            // 
            this.picLine3.BorderShadowColor = System.Drawing.Color.Empty;
            this.picLine3.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.picLine3.Image = ((object)(resources.GetObject("picLine3.Image")));
            this.picLine3.Location = new System.Drawing.Point(8, 83);
            this.picLine3.Name = "picLine3";
            this.picLine3.Size = new System.Drawing.Size(24, 24);
            this.picLine3.TabIndex = 35;
            this.picLine3.UseAppStyling = false;
            // 
            // picLine2
            // 
            this.picLine2.BorderShadowColor = System.Drawing.Color.Empty;
            this.picLine2.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.picLine2.Image = ((object)(resources.GetObject("picLine2.Image")));
            this.picLine2.Location = new System.Drawing.Point(8, 55);
            this.picLine2.Name = "picLine2";
            this.picLine2.Size = new System.Drawing.Size(24, 24);
            this.picLine2.TabIndex = 34;
            this.picLine2.UseAppStyling = false;
            // 
            // picLine1
            // 
            this.picLine1.BorderShadowColor = System.Drawing.Color.Empty;
            this.picLine1.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.picLine1.Image = ((object)(resources.GetObject("picLine1.Image")));
            this.picLine1.Location = new System.Drawing.Point(8, 28);
            this.picLine1.Name = "picLine1";
            this.picLine1.Size = new System.Drawing.Size(24, 24);
            this.picLine1.TabIndex = 33;
            this.picLine1.UseAppStyling = false;
            // 
            // txtDef4
            // 
            this.txtDef4.AlwaysInEditMode = true;
            this.txtDef4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDef4.Location = new System.Drawing.Point(38, 112);
            this.txtDef4.Name = "txtDef4";
            this.txtDef4.NullText = "Line 4";
            this.txtDef4.Size = new System.Drawing.Size(295, 22);
            this.txtDef4.TabIndex = 7;
            ultraToolTipInfo2.ToolTipText = "The marking definition for the 4th row.";
            ultraToolTipInfo2.ToolTipTitle = "Marking 4";
            this.tipManager.SetUltraToolTip(this.txtDef4, ultraToolTipInfo2);
            // 
            // txtDef3
            // 
            this.txtDef3.AlwaysInEditMode = true;
            this.txtDef3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDef3.Location = new System.Drawing.Point(38, 84);
            this.txtDef3.Name = "txtDef3";
            this.txtDef3.NullText = "Line 3";
            this.txtDef3.Size = new System.Drawing.Size(295, 22);
            this.txtDef3.TabIndex = 5;
            ultraToolTipInfo3.ToolTipText = "The marking definition for the 3rd row.";
            ultraToolTipInfo3.ToolTipTitle = "Marking 3";
            this.tipManager.SetUltraToolTip(this.txtDef3, ultraToolTipInfo3);
            // 
            // txtDef2
            // 
            this.txtDef2.AlwaysInEditMode = true;
            this.txtDef2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDef2.Location = new System.Drawing.Point(38, 56);
            this.txtDef2.Name = "txtDef2";
            this.txtDef2.NullText = "Line 2";
            this.txtDef2.Size = new System.Drawing.Size(295, 22);
            this.txtDef2.TabIndex = 3;
            ultraToolTipInfo4.ToolTipText = "The marking definition for the 2nd row.";
            ultraToolTipInfo4.ToolTipTitle = "Marking 2";
            this.tipManager.SetUltraToolTip(this.txtDef2, ultraToolTipInfo4);
            // 
            // txtDef1
            // 
            this.txtDef1.AllowDrop = true;
            this.txtDef1.AlwaysInEditMode = true;
            this.txtDef1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDef1.Location = new System.Drawing.Point(38, 29);
            this.txtDef1.Name = "txtDef1";
            this.txtDef1.NullText = "Line 1";
            this.txtDef1.Size = new System.Drawing.Size(295, 22);
            this.txtDef1.TabIndex = 1;
            ultraToolTipInfo5.ToolTipText = "The marking definition for the 1st row.";
            ultraToolTipInfo5.ToolTipTitle = "Marking 1";
            this.tipManager.SetUltraToolTip(this.txtDef1, ultraToolTipInfo5);
            // 
            // txtDate
            // 
            this.txtDate.AlwaysInEditMode = true;
            this.txtDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDate.Enabled = false;
            this.txtDate.Location = new System.Drawing.Point(113, 28);
            this.txtDate.Name = "txtDate";
            this.txtDate.NullText = "None";
            this.txtDate.ReadOnly = true;
            this.txtDate.Size = new System.Drawing.Size(223, 22);
            this.txtDate.TabIndex = 45;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(6, 32);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(101, 15);
            this.lblDate.TabIndex = 46;
            this.lblDate.Text = "Date Completed:";
            // 
            // txtSpecification
            // 
            this.txtSpecification.AlwaysInEditMode = true;
            this.txtSpecification.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSpecification.Location = new System.Drawing.Point(113, 56);
            this.txtSpecification.Name = "txtSpecification";
            this.txtSpecification.NullText = "None";
            this.txtSpecification.Size = new System.Drawing.Size(223, 22);
            this.txtSpecification.TabIndex = 47;
            ultraToolTipInfo1.ToolTipText = "Enter the specification that this part marking adheres to. This specification wil" +
    "l be added to the CoC.";
            ultraToolTipInfo1.ToolTipTitle = "Process Specification";
            this.tipManager.SetUltraToolTip(this.txtSpecification, ultraToolTipInfo1);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(6, 60);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(81, 15);
            this.ultraLabel2.TabIndex = 49;
            this.ultraLabel2.Text = "Specification:";
            // 
            // OrderPartMarking
            // 
            this.Name = "OrderPartMarking";
            this.Size = new System.Drawing.Size(357, 310);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpLines)).EndInit();
            this.grpLines.ResumeLayout(false);
            this.grpLines.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDef4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDef3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDef2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDef1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSpecification)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDate;
        private Infragistics.Win.Misc.UltraGroupBox grpLines;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDef4;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDef3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDef2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDef1;
        private Infragistics.Win.Misc.UltraLabel lblDate;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSpecification;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picLine4;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picLine3;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picLine2;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picLine1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;

    }
}
