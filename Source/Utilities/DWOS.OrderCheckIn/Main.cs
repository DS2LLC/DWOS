using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Shared;
using Gma.System.MouseKeyHook;
using NLog;

namespace DWOS.Service
{
	public partial class Main: Form
	{
		#region Fields

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
		
		private List<string> _checkInHistory = new List<string>();
		
        private const int MAX_ORDERS_SHOWN = 5;
		private const string DISPLAY_MESSAGE = "DWOS Order CheckIn:";
        private const char BARCODE_ORDER_CHECKIN_PREFFIX = '~';          //Used by Order Check In within the 'DWOS Order Check In' system tray application
        private const char BARCODE_BATCH_CHECKIN_PREFFIX = '+';          //Used by Batch Check In
        private const int NOTIFICATION_MAX_LENGTH = 63;

        private BarcodeScanner _scanner = null;

		#endregion

		#region Properties

		public string CurrentDepartment
		{
			get
			{
				return Properties.Settings.Default.CurrentDepartment;
			}
		}

		#endregion

		#region Methods

		public Main()
		{
			InitializeComponent();
			
            _scanner = new BarcodeScanner(this,
                BARCODE_ORDER_CHECKIN_PREFFIX,
                BARCODE_BATCH_CHECKIN_PREFFIX);

            _scanner.BarcodingFinished += _scanner_BarcodingFinished;
		    _scanner.Start();

			notifyIcon.BalloonTipTitle = "DWOS CheckIn (" + Properties.Settings.Default.CurrentDepartment + ")";
			notifyIcon.Text            = "Initialized and ready...";
			notifyIcon.ShowBalloonTip(5000, notifyIcon.BalloonTipTitle, notifyIcon.Text, ToolTipIcon.Info);
		}

        private void CheckInOrder(int orderID)
        {
            try
            {
                _log.Debug("In order check in " + orderID);

                if (orderID < 1)
                    return;

                var checkIn = new OrderCheckInController(orderID);
                var results = checkIn.CheckIn(Properties.Settings.Default.CurrentDepartment, -1);
                
                notifyIcon.BalloonTipTitle = "DWOS CheckIn (" + Properties.Settings.Default.CurrentDepartment + ")";

                //Notify user of status
                if (results.Response)
                {
                    _log.Info("Order {0} checked in to '{1}'.", orderID, Properties.Settings.Default.CurrentDepartment);
                    //update tool tip
                    notifyIcon.Text = this.CreateNotificationString(orderID, "Order");

                    //show a balloon
                    notifyIcon.ShowBalloonTip(5000, notifyIcon.BalloonTipTitle, "Order " + orderID.ToString() + " has been checked in to " + this.CurrentDepartment, ToolTipIcon.Info);
                    DWOS.Shared.Utilities.Sound.Beep();
                }
                else
                {
                    _log.Info("Order {0} not checked in '{1}'.", orderID, results.Description);
                    notifyIcon.ShowBalloonTip(5000, notifyIcon.BalloonTipTitle, results.Description, ToolTipIcon.Warning);
                    DWOS.Shared.Utilities.Sound.BeepError();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error checking in order.");
            }
		}

        private string CreateNotificationString(int id, string type)
        {
            var sb = new StringBuilder();

            //add order to beginning of list
            _checkInHistory.Insert(0, type + ": " + id);

            //remove excess orders
            while(_checkInHistory.Count > MAX_ORDERS_SHOWN)
                _checkInHistory.RemoveAt(_checkInHistory.Count - 1);

            //create new display text
            sb.AppendLine(DISPLAY_MESSAGE);

            for(int i = 0; i < _checkInHistory.Count; i++)
            {
                var historyEntry = _checkInHistory[i];

                var neededLength = i == 0 ?
                    sb.Length + historyEntry.Length :
                    1 + sb.Length + historyEntry.Length;

                if (neededLength > NOTIFICATION_MAX_LENGTH)
                {
                    break;
                }

                if (i > 0)
                {
                    sb.Append(Environment.NewLine);
                }

                sb.Append(historyEntry);
            }

            return sb.ToString();
        }

        private void CheckInBatch(int batchId)
        {
            try
            {
                _log.Debug("In batch check in " + batchId);

                if (batchId < 1)
                    return;

                var checkIn = new BatchCheckInController(batchId);
                var results = checkIn.CheckIn(Properties.Settings.Default.CurrentDepartment, -1);

                notifyIcon.BalloonTipTitle = "DWOS CheckIn (" + Properties.Settings.Default.CurrentDepartment + ")";

                //Notify user of status
                if (results.Response)
                {
                    _log.Info("Batch {0} checked in to '{1}'.", batchId, Properties.Settings.Default.CurrentDepartment);
                    
                    //update tool tip
                    notifyIcon.Text = this.CreateNotificationString(batchId, "Batch");

                    //show a balloon
                    notifyIcon.ShowBalloonTip(5000, notifyIcon.BalloonTipTitle, "Batch " + batchId.ToString() + " has been checked in to " + this.CurrentDepartment, ToolTipIcon.Info);
                    DWOS.Shared.Utilities.Sound.Beep();
                }
                else
                {
                    _log.Info("Batch {0} not checked in '{1}'.", batchId, results.Description);
                    notifyIcon.ShowBalloonTip(5000, notifyIcon.BalloonTipTitle, results.Description, ToolTipIcon.Warning);
                    DWOS.Shared.Utilities.Sound.BeepError();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error checking in batch.");
            }
        }

		private void LoadData()
		{
			try
			{
				using(var taDept = new DWOS.Data.Datasets.OrderStatusDataSetTableAdapters.d_DepartmentTableAdapter())
				{
					using(var dtDept = new Data.Datasets.OrderStatusDataSet.d_DepartmentDataTable())
					{
                        taDept.Fill(dtDept);
						int index = 0;

						foreach(var item in dtDept)
						{
							if(item.DepartmentID == this.CurrentDepartment)
								index = cboDept.Items.Add(item.DepartmentID);
							else
								cboDept.Items.Add(item.DepartmentID);
						}

						cboDept.SelectedIndex = index;
					}
				}
			}
			catch(Exception exc)
			{
				NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading list of departments.");
			}
		}

		#endregion

		#region Events
		
		private void Main_Load(object sender, EventArgs e)
		{
			LoadData();
			this.Hide();
		}
        
		private void Main_Shown(object sender, EventArgs e)
		{
			this.Hide();
		}

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.Visible)
                    this.Show();
            }
            catch (Exception exc)
            {
                string errorMsg = "Error showing form.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void changeServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new ServerConnection())
                {
                    if(frm.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Application must be restarted for settings to take affect.", About.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error changing server.");
            }
        }

		private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if(e.ClickedItem == exitToolStripMenuItem)
				this.Close();
		}		
		
		private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			try
			{
				if(!this.Visible)
					this.Show();
			}
			catch(Exception exc)
			{
				string errorMsg = "Error showing form.";
				ErrorMessageBox.ShowDialog(errorMsg, exc);
			}
		}
		
		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				if(this.Visible)
				{
					this.Hide();
					e.Cancel = true;
				}
			}
			catch(Exception exc)
			{
				string errorMsg = "Error closing form.";
				ErrorMessageBox.ShowDialog(errorMsg, exc);
			}
		}

		private void cboDept_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if(!String.IsNullOrEmpty(cboDept.Text))
				{
					Properties.Settings.Default.CurrentDepartment = cboDept.Text;
					Properties.Settings.Default.Save();
				}
			}
			catch(Exception exc)
			{
				string errorMsg = "Error changing dept.";
				ErrorMessageBox.ShowDialog(errorMsg, exc);
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			this.Hide();
		}

        private void _scanner_BarcodingFinished(object sender, BarcodeScanner.BarcodeScannerEventArgs e)
        {
            try
            {
                int orderId;

                if (e.Output != null && int.TryParse(e.Output, out orderId))
                {
                    var barcodePostfix = e.Postfix.GetValueOrDefault();

                    if (barcodePostfix == BARCODE_ORDER_CHECKIN_PREFFIX)
                    {
                        CheckInOrder(orderId);
                    }
                    else if (barcodePostfix == BARCODE_BATCH_CHECKIN_PREFFIX)
                    {
                        CheckInBatch(orderId);
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error during bar code finishing for batch scanner.");
            }
        }
        
        #endregion
	}

    public class BarcodeScanner : IDisposable
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private string _barcodeInput;
        private object _barcodereaderLock = new object();
        private bool _catchNextCR;
        private IKeyboardMouseEvents _keyboardListener;
        private System.Windows.Forms.Timer _timer;
        private Form _mainForm;

        public event EventHandler BarcodingStarted;
        public event EventHandler<BarcodeScannerEventArgs> BarcodingFinished;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the prefixes used to identify that the barcode has started being read.
        /// </summary>
        /// <value>The prefixes.</value>
        public List<char> Prefixes { get; set; }

        /// <summary>
        ///     Gets or sets the time interval between characters to ensure this is a barcode read. Measured in milliseconds.
        /// </summary>
        /// <value>The interval between characters.</value>
        public int IntervalBetweenCharacters { get; set; }
        
        #endregion

        #region Methods

        public BarcodeScanner(Form mainForm, params char[] prefix)
        {
            _mainForm = mainForm;
            
            IntervalBetweenCharacters = 500;

            if (prefix != null && prefix.Length > 0)
                Prefixes = new List<char>(prefix);

            Reset();
        }

        public void Start()
        {
            if (this._keyboardListener == null)
            {
                this._keyboardListener = Hook.GlobalEvents();
                this._keyboardListener.KeyPress += HookManager_KeyPress;
                this._keyboardListener.KeyUp += HookManager_KeyUp;
            }

            if (this._timer == null)
            {
                this._timer = new System.Windows.Forms.Timer { Enabled = false, Interval = IntervalBetweenCharacters };
                this._timer.Tick += timer_Tick;
            }
        }

        private void OnBarcodingStarted()
        {
            if (BarcodingStarted != null)
                BarcodingStarted(this, EventArgs.Empty);
        }

        private void OnBarcodingFinished(string barcode, char? postfix)
        {
            //Must Sleep to let last character prefix get passed through
            ThreadPool.QueueUserWorkItem(cb =>
            {
                Thread.Sleep(250);

                //get back on UI thread
                _mainForm.BeginInvoke(new Action<string, char?>((b, p) =>
                {
                    if (BarcodingFinished != null)
                        BarcodingFinished(this, new BarcodeScannerEventArgs { Output = b, Postfix = p });
                }), barcode, postfix);
            });
        }

        private void Reset()
        {
            this._barcodeInput = null;
            this._catchNextCR = false;

            if (this._timer != null)
                this._timer.Stop();

            OnBarcodingFinished(null, null);
        }

        #endregion

        #region Events

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                //if timer ticked beofore recieved end character then reset barcode reading as barcode didn't finish like we though it would
                lock (this._barcodereaderLock)
                    Reset();
            }
            catch (Exception exc)
            {
                string errorMsg = "Error on timer tick";
                _log.Error(exc, errorMsg);
            }
        }

        private void HookManager_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this._timer.Stop(); //pause the timer

                lock (this._barcodereaderLock)
                {
                    if (Prefixes.Contains(e.KeyChar)) //if found Magic prefix that says this is valid command
                    {
                        //START
                        if (this._barcodeInput == null)
                        {
                            OnBarcodingStarted();
                            this._barcodeInput = String.Empty;
                            this._timer.Start(); //restart timer
                        }
                        //FINISH
                        else
                        {
                            OnBarcodingFinished(this._barcodeInput, e.KeyChar);
                            this._barcodeInput = null;
                            this._catchNextCR = true;
                        }

                        e.Handled = true;
                    }
                    else if (this._barcodeInput != null)
                    {
                        e.Handled = true;
                        this._barcodeInput += e.KeyChar;
                        this._timer.Start();
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error capturing key press events.";
                _log.Warn(exc, errorMsg);

                Reset();
            }
        }

        private void HookManager_KeyUp(object sender, KeyEventArgs e)
        {
            //use this catch to suppresse the carrage return automatically sent by most barcode readers when they are done
            if (this._catchNextCR)
            {
                if (e.KeyCode == Keys.Return)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    this._catchNextCR = false;
                }
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            try
            {
                if (this._timer != null)
                {
                    this._timer.Dispose();
                    this._timer = null;
                }

                try
                {
                    if (this._keyboardListener != null)
                    {
                        this._keyboardListener.KeyPress -= HookManager_KeyPress;
                        this._keyboardListener.KeyDown -= HookManager_KeyUp;

                        this._keyboardListener.Dispose();
                        this._keyboardListener = null;
                    }
                }
                catch
                {
                } //Keyboard listener through error in dispose sometimes
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error disposing Barcode Scanner.");
            }
        }

        #endregion

        #region BarcodeScannerEventArgs

        public class BarcodeScannerEventArgs : EventArgs
        {
            public string Output { get; set; }
            public char? Postfix { get; set; }
        }

        #endregion
    }
}
