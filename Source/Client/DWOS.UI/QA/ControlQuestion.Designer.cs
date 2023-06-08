namespace DWOS.UI.QA
{
    partial class ControlQuestion
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlQuestion));
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            this.ultraPanel5 = new Infragistics.Win.Misc.UltraPanel();
            this.lblStepNumber = new Infragistics.Win.Misc.UltraLabel();
            this.lblStepName = new Infragistics.Win.Misc.UltraLabel();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.imgListQuestion = new System.Windows.Forms.ImageList(this.components);
            this.btnNext = new Infragistics.Win.Misc.UltraButton();
            this.picQuestionStatus = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.pnlAnswerPlaceHolder = new System.Windows.Forms.Panel();
            this.txtOperator = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel22 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel21 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel20 = new Infragistics.Win.Misc.UltraLabel();
            this.lblAnswerDescription = new Infragistics.Win.Misc.UltraLabel();
            this.dteDateCompleted = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.ultraPanel5.ClientArea.SuspendLayout();
            this.ultraPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOperator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDateCompleted)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraPanel5
            // 
            appearance1.BackColor = System.Drawing.Color.Khaki;
            this.ultraPanel5.Appearance = appearance1;
            // 
            // ultraPanel5.ClientArea
            // 
            this.ultraPanel5.ClientArea.Controls.Add(this.lblStepNumber);
            this.ultraPanel5.ClientArea.Controls.Add(this.lblStepName);
            this.ultraPanel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraPanel5.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel5.Margin = new System.Windows.Forms.Padding(4);
            this.ultraPanel5.Name = "ultraPanel5";
            this.ultraPanel5.Size = new System.Drawing.Size(454, 36);
            this.ultraPanel5.TabIndex = 28;
            // 
            // lblStepNumber
            // 
            appearance2.ForeColor = System.Drawing.SystemColors.HotTrack;
            appearance2.TextHAlignAsString = "Center";
            appearance2.TextVAlignAsString = "Middle";
            this.lblStepNumber.Appearance = appearance2;
            this.lblStepNumber.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStepNumber.Location = new System.Drawing.Point(4, 4);
            this.lblStepNumber.Margin = new System.Windows.Forms.Padding(4);
            this.lblStepNumber.Name = "lblStepNumber";
            this.lblStepNumber.Size = new System.Drawing.Size(59, 28);
            this.lblStepNumber.TabIndex = 12;
            this.lblStepNumber.Text = "1.0";
            // 
            // lblStepName
            // 
            this.lblStepName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance3.ForeColor = System.Drawing.SystemColors.HotTrack;
            appearance3.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            appearance3.TextVAlignAsString = "Middle";
            this.lblStepName.Appearance = appearance3;
            this.lblStepName.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStepName.Location = new System.Drawing.Point(63, 4);
            this.lblStepName.Margin = new System.Windows.Forms.Padding(4);
            this.lblStepName.Name = "lblStepName";
            this.lblStepName.Size = new System.Drawing.Size(367, 28);
            this.lblStepName.TabIndex = 11;
            this.lblStepName.Text = "Actual Thickness";
            this.lblStepName.UseAppStyling = false;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // toolTipManager1
            // 
            this.toolTipManager1.ContainingControl = this;
            // 
            // imgListQuestion
            // 
            this.imgListQuestion.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListQuestion.ImageStream")));
            this.imgListQuestion.TransparentColor = System.Drawing.Color.Transparent;
            this.imgListQuestion.Images.SetKeyName(0, "Lock");
            this.imgListQuestion.Images.SetKeyName(1, "Question");
            this.imgListQuestion.Images.SetKeyName(2, "Check");
            this.imgListQuestion.Images.SetKeyName(3, "Error");
            // 
            // btnNext
            // 
            appearance4.BorderColor = System.Drawing.Color.Tan;
            appearance4.FontData.BoldAsString = "True";
            appearance4.FontData.SizeInPoints = 10F;
            appearance4.ForeColor = System.Drawing.Color.Blue;
            this.btnNext.Appearance = appearance4;
            this.btnNext.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Flat;
            this.btnNext.Location = new System.Drawing.Point(349, 141);
            this.btnNext.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(99, 28);
            this.btnNext.TabIndex = 36;
            this.btnNext.Text = "Next >>";
            this.btnNext.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // picQuestionStatus
            // 
            appearance5.BackColor = System.Drawing.Color.Transparent;
            this.picQuestionStatus.Appearance = appearance5;
            this.picQuestionStatus.BackColor = System.Drawing.Color.Transparent;
            this.picQuestionStatus.BorderShadowColor = System.Drawing.Color.Empty;
            this.picQuestionStatus.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.picQuestionStatus.Image = ((object)(resources.GetObject("picQuestionStatus.Image")));
            this.picQuestionStatus.Location = new System.Drawing.Point(2, 44);
            this.picQuestionStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picQuestionStatus.Name = "picQuestionStatus";
            this.picQuestionStatus.Size = new System.Drawing.Size(49, 41);
            this.picQuestionStatus.TabIndex = 41;
            this.picQuestionStatus.UseAppStyling = false;
            // 
            // pnlAnswerPlaceHolder
            // 
            this.pnlAnswerPlaceHolder.Location = new System.Drawing.Point(133, 41);
            this.pnlAnswerPlaceHolder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlAnswerPlaceHolder.Name = "pnlAnswerPlaceHolder";
            this.pnlAnswerPlaceHolder.Size = new System.Drawing.Size(171, 28);
            this.pnlAnswerPlaceHolder.TabIndex = 33;
            // 
            // txtOperator
            // 
            this.txtOperator.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOperator.Location = new System.Drawing.Point(133, 76);
            this.txtOperator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtOperator.Name = "txtOperator";
            this.txtOperator.ReadOnly = true;
            this.txtOperator.Size = new System.Drawing.Size(315, 24);
            this.txtOperator.TabIndex = 34;
            this.txtOperator.TabStop = false;
            // 
            // ultraLabel22
            // 
            this.ultraLabel22.AutoSize = true;
            this.ultraLabel22.Location = new System.Drawing.Point(54, 79);
            this.ultraLabel22.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ultraLabel22.Name = "ultraLabel22";
            this.ultraLabel22.Size = new System.Drawing.Size(71, 18);
            this.ultraLabel22.TabIndex = 39;
            this.ultraLabel22.Tag = "";
            this.ultraLabel22.Text = "Operator:";
            // 
            // ultraLabel21
            // 
            this.ultraLabel21.AutoSize = true;
            this.ultraLabel21.Location = new System.Drawing.Point(54, 46);
            this.ultraLabel21.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ultraLabel21.Name = "ultraLabel21";
            this.ultraLabel21.Size = new System.Drawing.Size(60, 18);
            this.ultraLabel21.TabIndex = 38;
            this.ultraLabel21.Tag = "Answer";
            this.ultraLabel21.Text = "Answer:";
            // 
            // ultraLabel20
            // 
            this.ultraLabel20.AutoSize = true;
            this.ultraLabel20.Location = new System.Drawing.Point(54, 111);
            this.ultraLabel20.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ultraLabel20.Name = "ultraLabel20";
            this.ultraLabel20.Size = new System.Drawing.Size(43, 18);
            this.ultraLabel20.TabIndex = 40;
            this.ultraLabel20.Tag = "";
            this.ultraLabel20.Text = "Date:";
            // 
            // lblAnswerDescription
            // 
            appearance6.TextHAlignAsString = "Right";
            appearance6.TextVAlignAsString = "Middle";
            this.lblAnswerDescription.Appearance = appearance6;
            this.lblAnswerDescription.Location = new System.Drawing.Point(310, 45);
            this.lblAnswerDescription.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblAnswerDescription.Name = "lblAnswerDescription";
            this.lblAnswerDescription.Size = new System.Drawing.Size(138, 21);
            this.lblAnswerDescription.TabIndex = 37;
            this.lblAnswerDescription.Tag = "Name";
            this.lblAnswerDescription.Text = "4 - 10 Minutes";
            // 
            // dteDateCompleted
            // 
            this.dteDateCompleted.DateTime = new System.DateTime(2009, 3, 21, 0, 0, 0, 0);
            this.dteDateCompleted.Location = new System.Drawing.Point(133, 108);
            this.dteDateCompleted.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dteDateCompleted.MaskInput = "{date} {time}";
            this.dteDateCompleted.Name = "dteDateCompleted";
            this.dteDateCompleted.Nullable = false;
            this.dteDateCompleted.Size = new System.Drawing.Size(315, 25);
            this.dteDateCompleted.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.dteDateCompleted.TabIndex = 35;
            this.dteDateCompleted.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextSection;
            this.dteDateCompleted.TabStop = false;
            this.dteDateCompleted.Value = new System.DateTime(2009, 3, 21, 0, 0, 0, 0);
            // 
            // ControlQuestion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.picQuestionStatus);
            this.Controls.Add(this.pnlAnswerPlaceHolder);
            this.Controls.Add(this.txtOperator);
            this.Controls.Add(this.ultraLabel22);
            this.Controls.Add(this.ultraLabel21);
            this.Controls.Add(this.ultraLabel20);
            this.Controls.Add(this.lblAnswerDescription);
            this.Controls.Add(this.dteDateCompleted);
            this.Controls.Add(this.ultraPanel5);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ControlQuestion";
            this.Size = new System.Drawing.Size(454, 179);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OrderProcessQuestionKeyPress);
            this.Leave += new System.EventHandler(this.OrderProcessQuestion_Leave);
            this.ultraPanel5.ClientArea.ResumeLayout(false);
            this.ultraPanel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOperator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDateCompleted)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private Infragistics.Win.Misc.UltraPanel ultraPanel5;
        private Infragistics.Win.Misc.UltraLabel lblStepNumber;
        private Infragistics.Win.Misc.UltraLabel lblStepName;
        private System.Windows.Forms.ErrorProvider errorProvider;
		private Infragistics.Win.UltraWinToolTip.UltraToolTipManager toolTipManager1;
		private System.Windows.Forms.ImageList imgListQuestion;
		private Infragistics.Win.Misc.UltraButton btnNext;
		private Infragistics.Win.UltraWinEditors.UltraPictureBox picQuestionStatus;
		private System.Windows.Forms.Panel pnlAnswerPlaceHolder;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtOperator;
		private Infragistics.Win.Misc.UltraLabel ultraLabel22;
		private Infragistics.Win.Misc.UltraLabel ultraLabel21;
		private Infragistics.Win.Misc.UltraLabel ultraLabel20;
		private Infragistics.Win.Misc.UltraLabel lblAnswerDescription;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteDateCompleted;
    }
}
