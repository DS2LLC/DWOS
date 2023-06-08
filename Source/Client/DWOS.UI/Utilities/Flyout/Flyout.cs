using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NLog;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// <see cref="Flyout"/> is a Windows Form styled to look like a flyout panel similiar to the Win 8 Toast navigation
    /// </summary>
    public partial class Flyout : Form
    {
        //Constants
        const int AW_SLIDE = 0X40000;
        const int AW_HOR_POSITIVE = 0X1;
        const int AW_HOR_NEGATIVE = 0X2;
        const int AW_HIDE = 0x10000;
        const int _animationTime = 250;

        /// <summary>
        /// Animates the window.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="time">The time.</param>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        [DllImport("user32")]
        static extern bool AnimateWindow(IntPtr hwnd, int time, int flags);
        
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title 
        {
            get { return labelTitle.Text; }
            set { labelTitle.Text = value; }
        }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message 
        {
            get { return labelMessage.Text; }
            set { labelMessage.Text = value; }
        }

        public bool IsAlert
        {
            set
            {
                this.BackColor = value ? Color.Red : SystemColors.Highlight;
                labelClose.BackColor = value ? Color.Red : SystemColors.Highlight;
            }
        }

        public int TimeMillseconds { get; set; } = 5000;

        public Form MainForm { get; set; }

        public Flyout()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                if (MainForm == null)
                {
                    LogManager.GetCurrentClassLogger().Error($"{nameof(MainForm)} is null - using default form.");
                    return;
                }

                //Grab the location of the window and slide flyout in from there
                var mainForm = MainForm ?? DWOSApp.MainForm;

                Location = GetLocation(mainForm, Width);
                Opacity = GetOpacity(mainForm);
                AnimateWindow(Handle, _animationTime, AW_SLIDE | AW_HOR_NEGATIVE);

                var timer = new Timer();
                timer.Tick += new EventHandler((sender, args) =>
                {
                    timer.Stop();
                    CloseWindow();
                });
                timer.Interval = TimeMillseconds;
                timer.Start();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on load of the flyout.");
            }
        }

        private static double GetOpacity(Form mainForm)
        {
            return mainForm is Main ? 1.0 : .8;
        }

        private static Point GetLocation(Form mainForm, int flyoutWidth)
        {
            Point location;

            if (mainForm is Main)
            {
                var widthOfMain = mainForm.Width;
                var locationMainX = mainForm.Location.X;
                var locationMainY = mainForm.Location.Y;
                location = new Point(locationMainX + widthOfMain - flyoutWidth, locationMainY + 54/*fudge*/);
            }
            else
            {
                var widthOfMain = mainForm.Width;
                var locationMainX = mainForm.Location.X;
                var locationMainY = mainForm.Location.Y;
                location = new Point(locationMainX + widthOfMain - flyoutWidth, locationMainY);
            }

            return location;
        }

        private void CloseWindow()
        {
            if (Visible)
                AnimateWindow(Handle, _animationTime, AW_SLIDE | AW_HOR_POSITIVE | AW_HIDE);
            Close();
        }

        private void labelClose_Click(object sender, EventArgs e)
        {
            CloseWindow();
        }

        private void labelClose_MouseEnter(object sender, EventArgs e)
        {
            labelClose.ForeColor = SystemColors.Highlight;
            labelClose.BackColor = Color.White;
        }

        private void labelClose_MouseLeave(object sender, EventArgs e)
        {
            labelClose.BackColor = SystemColors.Highlight;
            labelClose.ForeColor = Color.White;
        }

        private void labelClose_MouseDown(object sender, MouseEventArgs e)
        {
            labelClose.BackColor = SystemColors.HotTrack;
        }
    }
}
