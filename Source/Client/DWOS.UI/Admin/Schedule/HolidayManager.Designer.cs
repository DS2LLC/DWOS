namespace DWOS.UI.Admin.Schedule
{
    partial class HolidayManager
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HolidayManager));
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.monthView = new Infragistics.Win.UltraWinSchedule.UltraMonthViewSingle();
            this.calInfo = new Infragistics.Win.UltraWinSchedule.UltraCalendarInfo(this.components);
            this.calLook = new Infragistics.Win.UltraWinSchedule.UltraCalendarLook(this.components);
            this.dsSchedule = new DWOS.Data.Datasets.ScheduleDataset();
            this.taWorkHoliday = new DWOS.Data.Datasets.ScheduleDatasetTableAdapters.WorkHolidayTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.monthView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsSchedule)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(557, 462);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(475, 462);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 14;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // monthView
            // 
            this.monthView.ActivityDisplayStyle = Infragistics.Win.UltraWinSchedule.ActivityDisplayStyleEnum.Holidays;
            this.monthView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.monthView.AutoAppointmentDialog = false;
            this.monthView.CalendarInfo = this.calInfo;
            this.monthView.CalendarLook = this.calLook;
            this.monthView.Location = new System.Drawing.Point(12, 12);
            this.monthView.Name = "monthView";
            this.monthView.Size = new System.Drawing.Size(623, 444);
            this.monthView.TabIndex = 18;
            this.monthView.DoubleClick += new System.EventHandler(this.monthView_DoubleClick);
            // 
            // calInfo
            // 
            this.calInfo.DataBindingsForAppointments.BindingContextControl = this;
            this.calInfo.DataBindingsForOwners.BindingContextControl = this;
            // 
            // calLook
            // 
            this.calLook.ViewStyle = Infragistics.Win.UltraWinSchedule.ViewStyle.Office2007;
            // 
            // dsSchedule
            // 
            this.dsSchedule.DataSetName = "ScheduleDataset";
            this.dsSchedule.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taWorkHoliday
            // 
            this.taWorkHoliday.ClearBeforeFill = true;
            // 
            // HolidayManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(647, 497);
            this.Controls.Add(this.monthView);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "HolidayManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Holidays";
            this.Load += new System.EventHandler(this.HolidayManager_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HolidayManager_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.monthView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsSchedule)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Data.Datasets.ScheduleDataset dsSchedule;
        private Data.Datasets.ScheduleDatasetTableAdapters.WorkHolidayTableAdapter taWorkHoliday;
        private Infragistics.Win.UltraWinSchedule.UltraMonthViewSingle monthView;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarInfo calInfo;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarLook calLook;
    }
}