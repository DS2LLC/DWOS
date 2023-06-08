namespace DWOS.UI.Processing
{
    partial class OrderProcessQuestion
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The operator who completed the question.", Infragistics.Win.ToolTipImage.Default, "Operator", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("test", Infragistics.Win.ToolTipImage.Default, "test", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("The answer is required to complete this question.", Infragistics.Win.ToolTipImage.Default, "Required", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Question Status", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Save your answer and continue to the next question.", Infragistics.Win.ToolTipImage.Default, "Next", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderProcessQuestion));
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Instructional notes pertaining to this question.", Infragistics.Win.ToolTipImage.Default, "Instructions", Infragistics.Win.DefaultableBoolean.Default);
            this.txtOperator = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lblAnswerDescription = new Infragistics.Win.Misc.UltraLabel();
            this.dteDateCompleted = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.ultraLabel20 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel21 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel22 = new Infragistics.Win.Misc.UltraLabel();
            this.pnlHeader = new Infragistics.Win.Misc.UltraPanel();
            this.lblQuestionNumber = new Infragistics.Win.Misc.UltraLabel();
            this.lblQuestion = new Infragistics.Win.Misc.UltraLabel();
            this.pnlAnswerPlaceHolder = new System.Windows.Forms.Panel();
            this.toolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.lblRequired = new Infragistics.Win.Misc.UltraLabel();
            this.picQuestionStatus = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.btnNext = new Infragistics.Win.Misc.UltraButton();
            this.txtNotes = new Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor();
            this.imgListQuestion = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.txtOperator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDateCompleted)).BeginInit();
            this.pnlHeader.ClientArea.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtOperator
            // 
            this.txtOperator.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOperator.Location = new System.Drawing.Point(138, 76);
            this.txtOperator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtOperator.Name = "txtOperator";
            this.txtOperator.ReadOnly = true;
            this.txtOperator.Size = new System.Drawing.Size(203, 24);
            this.txtOperator.TabIndex = 1;
            this.txtOperator.TabStop = false;
            ultraToolTipInfo6.ToolTipText = "The operator who completed the question.";
            ultraToolTipInfo6.ToolTipTitle = "Operator";
            this.toolTipManager.SetUltraToolTip(this.txtOperator, ultraToolTipInfo6);
            // 
            // lblAnswerDescription
            // 
            this.lblAnswerDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance9.TextVAlignAsString = "Middle";
            this.lblAnswerDescription.Appearance = appearance9;
            this.lblAnswerDescription.Location = new System.Drawing.Point(367, 46);
            this.lblAnswerDescription.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblAnswerDescription.Name = "lblAnswerDescription";
            this.lblAnswerDescription.Size = new System.Drawing.Size(238, 21);
            this.lblAnswerDescription.TabIndex = 16;
            this.lblAnswerDescription.Tag = "Name";
            this.lblAnswerDescription.Text = "4 - 10 Minutes";
            // 
            // dteDateCompleted
            // 
            this.dteDateCompleted.DateTime = new System.DateTime(2009, 3, 21, 0, 0, 0, 0);
            this.dteDateCompleted.Location = new System.Drawing.Point(138, 108);
            this.dteDateCompleted.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dteDateCompleted.MaskInput = "{date} {time}";
            this.dteDateCompleted.Name = "dteDateCompleted";
            this.dteDateCompleted.Nullable = false;
            this.dteDateCompleted.Size = new System.Drawing.Size(203, 25);
            this.dteDateCompleted.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.dteDateCompleted.TabIndex = 2;
            this.dteDateCompleted.TabNavigation = Infragistics.Win.UltraWinMaskedEdit.MaskedEditTabNavigation.NextSection;
            this.dteDateCompleted.TabStop = false;
            this.dteDateCompleted.Value = new System.DateTime(2009, 3, 21, 0, 0, 0, 0);
            this.dteDateCompleted.ValueChanged += new System.EventHandler(this.dteDateCompleted_ValueChanged);
            // 
            // ultraLabel20
            // 
            this.ultraLabel20.AutoSize = true;
            this.ultraLabel20.Location = new System.Drawing.Point(61, 111);
            this.ultraLabel20.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ultraLabel20.Name = "ultraLabel20";
            this.ultraLabel20.Size = new System.Drawing.Size(43, 18);
            this.ultraLabel20.TabIndex = 21;
            this.ultraLabel20.Tag = "";
            this.ultraLabel20.Text = "Date:";
            // 
            // ultraLabel21
            // 
            this.ultraLabel21.AutoSize = true;
            this.ultraLabel21.Location = new System.Drawing.Point(61, 47);
            this.ultraLabel21.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ultraLabel21.Name = "ultraLabel21";
            this.ultraLabel21.Size = new System.Drawing.Size(60, 18);
            this.ultraLabel21.TabIndex = 19;
            this.ultraLabel21.Tag = "Answer";
            this.ultraLabel21.Text = "Answer:";
            // 
            // ultraLabel22
            // 
            this.ultraLabel22.AutoSize = true;
            this.ultraLabel22.Location = new System.Drawing.Point(61, 79);
            this.ultraLabel22.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ultraLabel22.Name = "ultraLabel22";
            this.ultraLabel22.Size = new System.Drawing.Size(71, 18);
            this.ultraLabel22.TabIndex = 20;
            this.ultraLabel22.Tag = "";
            this.ultraLabel22.Text = "Operator:";
            // 
            // pnlHeader
            // 
            appearance6.BackColor = System.Drawing.Color.Khaki;
            this.pnlHeader.Appearance = appearance6;
            // 
            // pnlHeader.ClientArea
            // 
            this.pnlHeader.ClientArea.Controls.Add(this.lblQuestionNumber);
            this.pnlHeader.ClientArea.Controls.Add(this.lblQuestion);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(610, 36);
            this.pnlHeader.TabIndex = 28;
            // 
            // lblQuestionNumber
            // 
            appearance7.ForeColor = System.Drawing.SystemColors.HotTrack;
            appearance7.TextHAlignAsString = "Center";
            appearance7.TextVAlignAsString = "Middle";
            this.lblQuestionNumber.Appearance = appearance7;
            this.lblQuestionNumber.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQuestionNumber.Location = new System.Drawing.Point(5, 4);
            this.lblQuestionNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblQuestionNumber.Name = "lblQuestionNumber";
            this.lblQuestionNumber.Size = new System.Drawing.Size(70, 28);
            this.lblQuestionNumber.TabIndex = 12;
            this.lblQuestionNumber.Text = "99.0";
            this.lblQuestionNumber.WrapText = false;
            // 
            // lblQuestion
            // 
            this.lblQuestion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance8.ForeColor = System.Drawing.SystemColors.HotTrack;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            appearance8.TextVAlignAsString = "Middle";
            this.lblQuestion.Appearance = appearance8;
            this.lblQuestion.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQuestion.Location = new System.Drawing.Point(74, 5);
            this.lblQuestion.Margin = new System.Windows.Forms.Padding(3, 4, 4, 4);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(531, 26);
            this.lblQuestion.TabIndex = 11;
            this.lblQuestion.Text = "Exposure Time";
            this.lblQuestion.UseAppStyling = false;
            this.lblQuestion.WrapText = false;
            // 
            // pnlAnswerPlaceHolder
            // 
            this.pnlAnswerPlaceHolder.Location = new System.Drawing.Point(140, 44);
            this.pnlAnswerPlaceHolder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlAnswerPlaceHolder.Name = "pnlAnswerPlaceHolder";
            this.pnlAnswerPlaceHolder.Size = new System.Drawing.Size(201, 28);
            this.pnlAnswerPlaceHolder.TabIndex = 0;
            ultraToolTipInfo5.ToolTipText = "test";
            ultraToolTipInfo5.ToolTipTitle = "test";
            this.toolTipManager.SetUltraToolTip(this.pnlAnswerPlaceHolder, ultraToolTipInfo5);
            // 
            // toolTipManager
            // 
            this.toolTipManager.ContainingControl = this;
            // 
            // lblRequired
            // 
            appearance3.FontData.BoldAsString = "True";
            appearance3.ForeColor = System.Drawing.Color.Red;
            this.lblRequired.Appearance = appearance3;
            this.lblRequired.AutoSize = true;
            this.lblRequired.Location = new System.Drawing.Point(347, 44);
            this.lblRequired.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblRequired.Name = "lblRequired";
            this.lblRequired.Size = new System.Drawing.Size(14, 18);
            this.lblRequired.TabIndex = 33;
            this.lblRequired.Tag = "";
            this.lblRequired.Text = "*";
            ultraToolTipInfo2.ToolTipText = "The answer is required to complete this question.";
            ultraToolTipInfo2.ToolTipTitle = "Required";
            this.toolTipManager.SetUltraToolTip(this.lblRequired, ultraToolTipInfo2);
            this.lblRequired.Visible = false;
            // 
            // picQuestionStatus
            // 
            appearance5.BackColor = System.Drawing.Color.Transparent;
            this.picQuestionStatus.Appearance = appearance5;
            this.picQuestionStatus.BackColor = System.Drawing.Color.Transparent;
            this.picQuestionStatus.BorderShadowColor = System.Drawing.Color.Empty;
            this.picQuestionStatus.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.picQuestionStatus.Image = ((object)(resources.GetObject("picQuestionStatus.Image")));
            this.picQuestionStatus.Location = new System.Drawing.Point(5, 44);
            this.picQuestionStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picQuestionStatus.Name = "picQuestionStatus";
            this.picQuestionStatus.Size = new System.Drawing.Size(49, 41);
            this.picQuestionStatus.TabIndex = 32;
            ultraToolTipInfo4.ToolTipTitle = "Question Status";
            this.toolTipManager.SetUltraToolTip(this.picQuestionStatus, ultraToolTipInfo4);
            this.picQuestionStatus.UseAppStyling = false;
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance4.BorderColor = System.Drawing.Color.Tan;
            appearance4.FontData.BoldAsString = "True";
            appearance4.FontData.SizeInPoints = 10F;
            appearance4.ForeColor = System.Drawing.Color.Blue;
            this.btnNext.Appearance = appearance4;
            this.btnNext.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Flat;
            this.btnNext.Location = new System.Drawing.Point(242, 141);
            this.btnNext.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(99, 28);
            this.btnNext.TabIndex = 3;
            this.btnNext.Text = "Next >>";
            ultraToolTipInfo3.ToolTipText = "Save your answer and continue to the next question.";
            ultraToolTipInfo3.ToolTipTitle = "Next";
            this.toolTipManager.SetUltraToolTip(this.btnNext, ultraToolTipInfo3);
            this.btnNext.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // txtNotes
            // 
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.AlphaLevel = ((short)(59));
            appearance1.FontData.BoldAsString = "False";
            appearance1.FontData.ItalicAsString = "False";
            appearance1.FontData.Name = "Verdana";
            appearance1.FontData.SizeInPoints = 9.75F;
            appearance1.FontData.StrikeoutAsString = "False";
            appearance1.FontData.UnderlineAsString = "False";
            appearance1.ForegroundAlpha = Infragistics.Win.Alpha.Opaque;
            appearance1.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance1.ImageBackground")));
            appearance1.ImageBackgroundAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
            appearance1.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(0, 0, 127, 20);
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Right;
            this.txtNotes.Appearance = appearance1;
            this.txtNotes.ContextMenuItems = Infragistics.Win.FormattedLinkLabel.FormattedTextMenuItems.None;
            appearance2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtNotes.LinkAppearance = appearance2;
            this.txtNotes.Location = new System.Drawing.Point(347, 76);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ReadOnly = true;
            this.txtNotes.Size = new System.Drawing.Size(258, 93);
            this.txtNotes.TabIndex = 34;
            this.txtNotes.TextSmoothingMode = Infragistics.Win.FormattedLinkLabel.TextSmoothingMode.AntiAlias;
            ultraToolTipInfo1.ToolTipText = "Instructional notes pertaining to this question.";
            ultraToolTipInfo1.ToolTipTitle = "Instructions";
            this.toolTipManager.SetUltraToolTip(this.txtNotes, ultraToolTipInfo1);
            this.txtNotes.UnderlineLinks = Infragistics.Win.FormattedLinkLabel.UnderlineLink.Always;
            this.txtNotes.Value = "<span style=\"color:Silver;\">&lt;None&gt;</span>";
            // 
            // imgListQuestion
            // 
            this.imgListQuestion.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListQuestion.ImageStream")));
            this.imgListQuestion.TransparentColor = System.Drawing.Color.Transparent;
            this.imgListQuestion.Images.SetKeyName(0, "Lock");
            this.imgListQuestion.Images.SetKeyName(1, "Question");
            this.imgListQuestion.Images.SetKeyName(2, "Check");
            this.imgListQuestion.Images.SetKeyName(3, "Error");
            this.imgListQuestion.Images.SetKeyName(4, "Active");
            // 
            // OrderProcessQuestion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.lblRequired);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.picQuestionStatus);
            this.Controls.Add(this.pnlAnswerPlaceHolder);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.txtOperator);
            this.Controls.Add(this.ultraLabel22);
            this.Controls.Add(this.ultraLabel21);
            this.Controls.Add(this.ultraLabel20);
            this.Controls.Add(this.lblAnswerDescription);
            this.Controls.Add(this.dteDateCompleted);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "OrderProcessQuestion";
            this.Size = new System.Drawing.Size(610, 177);
            this.Enter += new System.EventHandler(this.OrderProcessQuestion_Enter);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OrderProcessQuestionKeyPress);
            this.Leave += new System.EventHandler(this.OrderProcessQuestion_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.txtOperator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDateCompleted)).EndInit();
            this.pnlHeader.ClientArea.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtOperator;
        private Infragistics.Win.Misc.UltraLabel lblAnswerDescription;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteDateCompleted;
        private Infragistics.Win.Misc.UltraLabel ultraLabel20;
        private Infragistics.Win.Misc.UltraLabel ultraLabel21;
        private Infragistics.Win.Misc.UltraLabel ultraLabel22;
        private Infragistics.Win.Misc.UltraPanel pnlHeader;
        private Infragistics.Win.Misc.UltraLabel lblQuestionNumber;
        private Infragistics.Win.Misc.UltraLabel lblQuestion;
		private System.Windows.Forms.Panel pnlAnswerPlaceHolder;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager toolTipManager;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picQuestionStatus;
        private Infragistics.Win.Misc.UltraButton btnNext;
        private System.Windows.Forms.ImageList imgListQuestion;
        private Infragistics.Win.Misc.UltraLabel lblRequired;
		private Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor txtNotes;
    }
}
