namespace DWOS.UI.Admin
{
	partial class WorkCalendar
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
			this.calInfo = new Infragistics.Win.UltraWinSchedule.UltraCalendarInfo(this.components);
			this.ultraMonthViewSingle1 = new Infragistics.Win.UltraWinSchedule.UltraMonthViewSingle();
			this.calLook = new Infragistics.Win.UltraWinSchedule.UltraCalendarLook(this.components);
			((System.ComponentModel.ISupportInitialize)(this.ultraMonthViewSingle1)).BeginInit();
			this.SuspendLayout();
			// 
			// calInfo
			// 
			this.calInfo.DataBindingsForAppointments.BindingContextControl = this;
			this.calInfo.DataBindingsForOwners.BindingContextControl = this;
			// 
			// ultraMonthViewSingle1
			// 
			this.ultraMonthViewSingle1.CalendarInfo = this.calInfo;
			this.ultraMonthViewSingle1.CalendarLook = this.calLook;
			this.ultraMonthViewSingle1.Location = new System.Drawing.Point(12, 12);
			this.ultraMonthViewSingle1.Name = "ultraMonthViewSingle1";
			this.ultraMonthViewSingle1.Size = new System.Drawing.Size(464, 413);
			this.ultraMonthViewSingle1.TabIndex = 0;
			// 
			// calLook
			// 
			this.calLook.ViewStyle = Infragistics.Win.UltraWinSchedule.ViewStyle.Office2007;
			// 
			// WorkCalendar
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(482, 499);
			this.Controls.Add(this.ultraMonthViewSingle1);
			this.Name = "WorkCalendar";
			this.Text = "WorkCalendar";
			((System.ComponentModel.ISupportInitialize)(this.ultraMonthViewSingle1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinSchedule.UltraCalendarInfo calInfo;
		private Infragistics.Win.UltraWinSchedule.UltraMonthViewSingle ultraMonthViewSingle1;
		private Infragistics.Win.UltraWinSchedule.UltraCalendarLook calLook;
	}
}