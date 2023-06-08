using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DWOS.Shared.Utilities;
using Infragistics.Win.UltraWinDock;

namespace DWOS.UI.Utilities
{
    public partial class NotificationPanel : UserControl
    {
        #region Fields
        
        private const int MAX_RECORDS = 50;
        private List <NotificationMessage> _messages = new List <NotificationMessage>();
        private DockableControlPane _notificationPane;
        private string _notificationTextFomrmatted;

        #endregion

        #region Properties
        
        public DockableControlPane NotificationPane
        {
            get { return _notificationPane; }
            set
            {
                _notificationPane = value;
                
                if(_notificationPane != null)
                    _notificationTextFomrmatted = _notificationPane.TextTabResolved + " ({0})";
            }
        }

        #endregion

        #region Methods
        
        public NotificationPanel()
        {
            InitializeComponent();

            grdNotifications.DataSource = _messages;
        }

        public void AddNotifications(List<NotificationMessage> messages)
        {
            _messages.AddRange(messages);

            if (_messages.Count > MAX_RECORDS)
                _messages.RemoveAt(0);

            if(_notificationPane != null)
                _notificationPane.TextTab = _notificationTextFomrmatted.FormatWith(_messages.Count);

            grdNotifications.DataBind();
        }

        public void AddNotification(NotificationMessage message)
        {
            _messages.Add(message);

            if (_messages.Count > MAX_RECORDS)
                _messages.RemoveAt(0);

            if (_notificationPane != null)
                _notificationPane.TextTab = _notificationTextFomrmatted.FormatWith(_messages.Count);

            grdNotifications.DataBind();
        }

        public void Clear()
        {
            _messages.Clear();

            if (_notificationPane != null)
                _notificationPane.TextTab = _notificationTextFomrmatted.FormatWith(_messages.Count);
        }

        #endregion

        #region Events
        
        private void grdNotifications_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if(e.Row.IsDataRow)
            {
                var msg = e.Row.ListObject as NotificationMessage;
                if(msg != null)
                    e.Row.Cells["Message"].Appearance.Image = msg.Level.ToString();
            }
        }

        #endregion
    }
}
