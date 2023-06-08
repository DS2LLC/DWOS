namespace DWOS.UI.Admin.PartManagerPanels
{
    partial class PartMarkInfo
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PartMarkInfo));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The marking definition for the 3rd row.", Infragistics.Win.ToolTipImage.Default, "Marking 3", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The marking definition for the 4th row.", Infragistics.Win.ToolTipImage.Default, "Marking 4", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The marking definition for the 2nd row.", Infragistics.Win.ToolTipImage.Default, "Marking 2", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The marking definition for the 1st row.", Infragistics.Win.ToolTipImage.Default, "Marking 1", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Enter the specification that this part marking adheres to. This specification wil" +
        "l be added to the CoC.", Infragistics.Win.ToolTipImage.Default, "Specification", Infragistics.Win.DefaultableBoolean.Default);
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.grpMarkingLines = new Infragistics.Win.Misc.UltraGroupBox();
            this.picLine4 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.picLine3 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.picLine2 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.picLine1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.txtLine3 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtLine4 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtLine2 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtLine1 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtSpecification = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpMarkingLines)).BeginInit();
            this.grpMarkingLines.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLine3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLine4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLine2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLine1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSpecification)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.txtSpecification);
            this.grpData.Controls.Add(this.grpMarkingLines);
            this.grpData.Controls.Add(this.ultraLabel1);
            appearance1.Image = global::DWOS.UI.Properties.Resources.Tag_16;
            this.grpData.HeaderAppearance = appearance1;
            this.grpData.Size = new System.Drawing.Size(446, 232);
            this.grpData.Text = "Part Marking";
            this.grpData.Controls.SetChildIndex(this.picLockImage, 0);
            this.grpData.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.grpData.Controls.SetChildIndex(this.grpMarkingLines, 0);
            this.grpData.Controls.SetChildIndex(this.txtSpecification, 0);
            // 
            // picLockImage
            // 
            this.picLockImage.Location = new System.Drawing.Point(422, -654);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(11, 30);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(81, 15);
            this.ultraLabel1.TabIndex = 1;
            this.ultraLabel1.Text = "Specification:";
            // 
            // grpMarkingLines
            // 
            this.grpMarkingLines.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpMarkingLines.Controls.Add(this.picLine4);
            this.grpMarkingLines.Controls.Add(this.picLine3);
            this.grpMarkingLines.Controls.Add(this.picLine2);
            this.grpMarkingLines.Controls.Add(this.picLine1);
            this.grpMarkingLines.Controls.Add(this.txtLine3);
            this.grpMarkingLines.Controls.Add(this.txtLine4);
            this.grpMarkingLines.Controls.Add(this.txtLine2);
            this.grpMarkingLines.Controls.Add(this.txtLine1);
            this.grpMarkingLines.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.grpMarkingLines.Location = new System.Drawing.Point(11, 54);
            this.grpMarkingLines.Name = "grpMarkingLines";
            this.grpMarkingLines.Size = new System.Drawing.Size(424, 168);
            this.grpMarkingLines.TabIndex = 2;
            this.grpMarkingLines.Text = "Marking Lines";
            // 
            // picLine4
            // 
            this.picLine4.BorderShadowColor = System.Drawing.Color.Empty;
            this.picLine4.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.picLine4.Image = ((object)(resources.GetObject("picLine4.Image")));
            this.picLine4.Location = new System.Drawing.Point(6, 126);
            this.picLine4.Name = "picLine4";
            this.picLine4.Size = new System.Drawing.Size(24, 24);
            this.picLine4.TabIndex = 39;
            this.picLine4.UseAppStyling = false;
            // 
            // picLine3
            // 
            this.picLine3.BorderShadowColor = System.Drawing.Color.Empty;
            this.picLine3.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.picLine3.Image = ((object)(resources.GetObject("picLine3.Image")));
            this.picLine3.Location = new System.Drawing.Point(6, 96);
            this.picLine3.Name = "picLine3";
            this.picLine3.Size = new System.Drawing.Size(24, 24);
            this.picLine3.TabIndex = 38;
            this.picLine3.UseAppStyling = false;
            // 
            // picLine2
            // 
            this.picLine2.BorderShadowColor = System.Drawing.Color.Empty;
            this.picLine2.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.picLine2.Image = ((object)(resources.GetObject("picLine2.Image")));
            this.picLine2.Location = new System.Drawing.Point(6, 66);
            this.picLine2.Name = "picLine2";
            this.picLine2.Size = new System.Drawing.Size(24, 24);
            this.picLine2.TabIndex = 37;
            this.picLine2.UseAppStyling = false;
            // 
            // picLine1
            // 
            this.picLine1.BorderShadowColor = System.Drawing.Color.Empty;
            this.picLine1.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.picLine1.Image = ((object)(resources.GetObject("picLine1.Image")));
            this.picLine1.Location = new System.Drawing.Point(6, 36);
            this.picLine1.Name = "picLine1";
            this.picLine1.Size = new System.Drawing.Size(24, 24);
            this.picLine1.TabIndex = 34;
            this.picLine1.UseAppStyling = false;
            // 
            // txtLine3
            // 
            this.txtLine3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLine3.Location = new System.Drawing.Point(36, 96);
            this.txtLine3.Name = "txtLine3";
            this.txtLine3.Size = new System.Drawing.Size(382, 22);
            this.txtLine3.TabIndex = 2;
            ultraToolTipInfo2.ToolTipText = "The marking definition for the 3rd row.";
            ultraToolTipInfo2.ToolTipTitle = "Marking 3";
            this.tipManager.SetUltraToolTip(this.txtLine3, ultraToolTipInfo2);
            // 
            // txtLine4
            // 
            this.txtLine4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLine4.Location = new System.Drawing.Point(36, 128);
            this.txtLine4.Name = "txtLine4";
            this.txtLine4.Size = new System.Drawing.Size(382, 22);
            this.txtLine4.TabIndex = 3;
            ultraToolTipInfo3.ToolTipText = "The marking definition for the 4th row.";
            ultraToolTipInfo3.ToolTipTitle = "Marking 4";
            this.tipManager.SetUltraToolTip(this.txtLine4, ultraToolTipInfo3);
            // 
            // txtLine2
            // 
            this.txtLine2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLine2.Location = new System.Drawing.Point(36, 66);
            this.txtLine2.Name = "txtLine2";
            this.txtLine2.Size = new System.Drawing.Size(382, 22);
            this.txtLine2.TabIndex = 1;
            ultraToolTipInfo4.ToolTipText = "The marking definition for the 2nd row.";
            ultraToolTipInfo4.ToolTipTitle = "Marking 2";
            this.tipManager.SetUltraToolTip(this.txtLine2, ultraToolTipInfo4);
            // 
            // txtLine1
            // 
            this.txtLine1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLine1.Location = new System.Drawing.Point(36, 38);
            this.txtLine1.Name = "txtLine1";
            this.txtLine1.Size = new System.Drawing.Size(382, 22);
            this.txtLine1.TabIndex = 0;
            ultraToolTipInfo5.ToolTipText = "The marking definition for the 1st row.";
            ultraToolTipInfo5.ToolTipTitle = "Marking 1";
            this.tipManager.SetUltraToolTip(this.txtLine1, ultraToolTipInfo5);
            // 
            // txtSpecification
            // 
            this.txtSpecification.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSpecification.Location = new System.Drawing.Point(98, 26);
            this.txtSpecification.Name = "txtSpecification";
            this.txtSpecification.Size = new System.Drawing.Size(337, 22);
            this.txtSpecification.TabIndex = 1;
            ultraToolTipInfo1.ToolTipText = "Enter the specification that this part marking adheres to. This specification wil" +
    "l be added to the CoC.";
            ultraToolTipInfo1.ToolTipTitle = "Specification";
            this.tipManager.SetUltraToolTip(this.txtSpecification, ultraToolTipInfo1);
            // 
            // PartMarkInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "PartMarkInfo";
            this.Size = new System.Drawing.Size(452, 238);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpMarkingLines)).EndInit();
            this.grpMarkingLines.ResumeLayout(false);
            this.grpMarkingLines.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLine3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLine4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLine2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLine1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSpecification)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraGroupBox grpMarkingLines;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSpecification;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtLine3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtLine4;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtLine2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtLine1;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picLine1;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picLine4;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picLine3;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picLine2;
    }
}
