using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Windows.Forms;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// <see cref="FlyoutManager"/> services requests by the application to display flyout notifications to the user
    /// </summary>
    internal class FlyoutManager
    {
        private const int MAX_QUEUE_SIZE = 3;
        private bool _isFlyoutOpen;
        private readonly object _isFlyoutOpenLock = new object();
        private readonly ConcurrentQueue<FlyoutQueueItem> _flyoutQueue = new ConcurrentQueue<FlyoutQueueItem>();

        public Form MainForm { get; }

        public Size FlyoutSize { get; set; } = new Size(300, 83);

        internal FlyoutManager(Form mainForm)
        {
            MainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
        }

        /// <summary>
        /// Displays the flyout if available.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="isAlert">The is alert.</param>
        /// <returns>True if displayed; false if otherwise.</returns>
        internal bool DisplayFlyout(string title, string message, bool isAlert = false, int timeMillseconds = 5000)
        {
            var success = false;
            try
            {
                lock (_isFlyoutOpenLock)
                {
                    if (_isFlyoutOpen)
                    {
                        if (_flyoutQueue.Count < MAX_QUEUE_SIZE)
                        {
                            _flyoutQueue.Enqueue(new FlyoutQueueItem(title, message, isAlert));
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    _isFlyoutOpen = true;
                }

                var flyout = new Flyout()
                {
                    Title = title,
                    Message = message,
                    IsAlert = isAlert,
                    MainForm = MainForm,
                    TimeMillseconds = timeMillseconds,
                    Size = FlyoutSize
                };

                flyout.FormClosed += Flyout_FormClosed;

                flyout.Show();
                MainForm.Focus();

                success = true;
            }
            catch (Exception exception)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exception, "Error displaying flyout");
            }

            return success;
        }

        private void Flyout_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            lock (_isFlyoutOpenLock)
            {
                _isFlyoutOpen = false;
            }

            FlyoutQueueItem nextInQueue;
            if (_flyoutQueue.TryDequeue(out nextInQueue))
            {
                DisplayFlyout(nextInQueue.Title, nextInQueue.Message, nextInQueue.IsAlert);
            }
        }

        private struct FlyoutQueueItem
        {
            public readonly string Title;

            public readonly string Message;

            public readonly bool IsAlert;

            public FlyoutQueueItem(string title, string message, bool isAlert)
            {
                Title = title;
                Message = message;
                IsAlert = isAlert;
            }
        }
    }
}
