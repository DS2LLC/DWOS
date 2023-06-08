using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin.Schedule
{
    public partial class HolidayManager : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        public HolidayManager()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            _log.Debug("Loading holiday manager.");

            taWorkHoliday.FillFromDate(dsSchedule.WorkHoliday, DateTime.Now.Subtract(TimeSpan.FromDays(400)).ToShortDateString());

            foreach(var holiday in dsSchedule.WorkHoliday)
                calInfo.Holidays.Add(holiday.Holiday, holiday.Name).Tag = holiday;

            calInfo.AfterHolidayAdded += calInfo_AfterHolidayAdded;
            calInfo.BeforeHolidayRemoved += calInfo_BeforeHolidayRemoved;
        }

        private void SaveData()
        {
            _log.Debug("Saving holiday manager.");

            //sync all the holidays
            foreach (var holiday in calInfo.Holidays)
            {
                var holidayRow = holiday.Tag as ScheduleDataset.WorkHolidayRow;

                if (holidayRow != null)
                {
                    holidayRow.Holiday = holiday.Day.Date;
                    holidayRow.Name = holiday.Name;
                }
            }

            taWorkHoliday.Update(dsSchedule.WorkHoliday);
        }

        #endregion

        #region Events
        
        private void calInfo_BeforeHolidayRemoved(object sender, Infragistics.Win.UltraWinSchedule.CancelableHolidayEventArgs e)
        {
            if(e.Holiday.Tag is ScheduleDataset.WorkHolidayRow)
            {
                ((ScheduleDataset.WorkHolidayRow) e.Holiday.Tag).Delete();
            }
        }
        
        private void calInfo_AfterHolidayAdded(object sender, Infragistics.Win.UltraWinSchedule.HolidayEventArgs e)
        {
            e.Holiday.Tag = dsSchedule.WorkHoliday.AddWorkHolidayRow(e.Holiday.Day.Date, e.Holiday.Name, false);
        }
        
        private void HolidayManager_Load(object sender, EventArgs e) { LoadData(); }

        private void btnOK_Click(object sender, EventArgs e) { SaveData(); }

        private void monthView_DoubleClick(object sender, EventArgs e)
        {
            var holidayAppt = monthView.GetHolidayFromPoint(monthView.PointToClient(MousePosition));

            //if found appt then edit it
            if(holidayAppt != null)
            {
                using (var form = new TextBoxForm())
                {
                    form.Text = "Holiday Name";
                    form.FormLabel.Text = "Name:";
                    form.FormTextBox.Text = holidayAppt.Name;
                    form.FormTextBox.MaxLength = dsSchedule.WorkHoliday.NameColumn.MaxLength;

                    if (form.ShowDialog(this) == DialogResult.OK && !String.IsNullOrWhiteSpace(form.FormTextBox.Text))
                        holidayAppt.Name = form.FormTextBox.Text;
                }
            }
            else //else add new holiday if double clicked a day
            {
                var day = monthView.GetDayFromPoint(monthView.PointToClient(MousePosition));

                if(day != null)
                {
                    calInfo.Holidays.Add(day.Date, "Holiday");
                }
            }
        }

        private void HolidayManager_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
            {
                if(calInfo.SelectedHolidays.Count > 0)
                {
                    calInfo.Holidays.Remove(calInfo.SelectedHolidays[0]);
                }
            }
        }

        #endregion
    }
}
