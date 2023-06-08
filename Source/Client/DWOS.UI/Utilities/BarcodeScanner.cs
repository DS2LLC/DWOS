using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using DWOS.Data;
using Gma.System.MouseKeyHook;
using NLog;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Scans barcodes without a separate prompt.
    /// </summary>
    public class BarcodeScanner : IDisposable
    {
        #region Fields

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ConcurrentStack<char> _barcodeInput = new ConcurrentStack<char>();
        private bool _readingInput;
        private bool _catchNextReturn;
        private IKeyboardMouseEvents _keyboardListener;
        private System.Windows.Forms.Timer _keyPressTimer;

        /// <summary>
        /// Occurs when this instance begins to read a barcode.
        /// </summary>
        public event EventHandler BarcodingStarted;

        /// <summary>
        /// Occurs when this instance completely reads a barcode.
        /// </summary>
        public event EventHandler<BarcodeScannerEventArgs> BarcodingFinished;

        /// <summary>
        /// Occurs if/when this instance is unable to completely read a barcode.
        /// </summary>
        public event EventHandler BarcodingCancelled;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the prefixes used to identify that the barcode has started being read.
        /// </summary>
        /// <value>The prefixes.</value>
        public ISet<char> Prefixes { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeScanner"/> class.
        /// </summary>
        /// <param name="prefix">
        /// Array of single-character prefixes that indicate the start of a barcode
        /// </param>
        public BarcodeScanner(params char[] prefix)
        {
            if (prefix != null && prefix.Length > 0)
                Prefixes = new HashSet<char>(prefix);

            Reset();
        }

        /// <summary>
        /// Start listening for barcodes.
        /// </summary>
        public void Start()
        {
            if (_keyboardListener == null)
            {
                _keyboardListener = Hook.GlobalEvents();
                _keyboardListener.KeyPress += HookManager_KeyPress;
                _keyboardListener.KeyUp += HookManager_KeyUp;
            }

            if (_keyPressTimer == null)
            {
                _keyPressTimer = new System.Windows.Forms.Timer { Enabled = false, Interval = ApplicationSettings.Current.BarcodeIntervalMilliseconds };
                _keyPressTimer.Tick += KeyPressTimerTick;
            }
        }

        /// <summary>
        /// Stop listening for barcodes.
        /// </summary>
        public void Stop()
        {
            if (_keyboardListener != null)
            {
                _keyboardListener.KeyPress -= HookManager_KeyPress;
                _keyboardListener.KeyUp -= HookManager_KeyUp;
                _keyboardListener.Dispose();
            }
            _keyboardListener = null;

            _keyPressTimer?.Stop();
        }

        /// <summary>
        /// Call when client settings change.
        /// </summary>
        public void OnSettingsChange()
        {
            // Sync timer interval with settings.
            if (_keyPressTimer != null)
            {
                _keyPressTimer.Interval = ApplicationSettings.Current.BarcodeIntervalMilliseconds;
            }
        }

        private void OnBarcodingFinished(string barcode, char? postfix)
        {
            //Must Sleep to let last character prefix get passed through
            ThreadPool.QueueUserWorkItem(cb =>
            {
                Thread.Sleep(250);

                //get back on UI thread
                DWOSApp.MainForm.BeginInvoke(new Action<string, char?>((b, p) =>
                {
                    BarcodingFinished?.Invoke(this, new BarcodeScannerEventArgs { Output = b, Postfix = p });
                }), barcode, postfix);
            });
        }

        private void Reset()
        {
            _readingInput = false;
            _catchNextReturn = false;

            var input = _barcodeInput.ToArray();
            Array.Reverse(input);

            _log.Info($"Cancelled barcode scan: {new string(input)}");
            _barcodeInput.Clear();

            BarcodingCancelled?.Invoke(this, EventArgs.Empty);

            _keyPressTimer?.Stop();
        }

        #endregion

        #region Events

        private void KeyPressTimerTick(object sender, EventArgs e)
        {
            try
            {
                //if timer ticked before received end character then reset barcode reading as barcode didn't finish like we though it would
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
                _keyPressTimer.Stop(); //pause the timer

                if (Prefixes.Contains(e.KeyChar)) //if found Magic prefix that says this is valid command
                {
                    //START
                    if (_barcodeInput.IsEmpty)
                    {
                        _readingInput = true;
                        BarcodingStarted?.Invoke(this, EventArgs.Empty);
                        _barcodeInput.Clear();
                        _keyPressTimer.Start(); //restart timer
                    }
                    //FINISH
                    else
                    {
                        _readingInput = false;
                        var input = _barcodeInput.ToArray(); // local copy of buffer's content
                        Array.Reverse(input); // buffer is Last-In, First-Out
                        OnBarcodingFinished(new string(input), e.KeyChar);
                        _barcodeInput.Clear();
                        _catchNextReturn = true;
                    }

                    e.Handled = true;
                }
                else if (_readingInput)
                {
                    e.Handled = true;
                    _barcodeInput.Push(e.KeyChar);
                    _keyPressTimer.Start();
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
            //  suppress the carriage return automatically sent by most barcode readers when they are done
            if (_catchNextReturn)
            {
                if (e.KeyCode == Keys.Return)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    _catchNextReturn = false;
                }
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            try
            {
                if (_keyPressTimer != null)
                {
                    _keyPressTimer.Dispose();
                    _keyPressTimer = null;
                }

                if (_keyboardListener != null)
                {
                    _keyboardListener.KeyPress -= HookManager_KeyPress;
                    _keyboardListener.KeyDown -= HookManager_KeyUp;

                    _keyboardListener.Dispose();
                    _keyboardListener = null;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error disposing Barcode Scanner.");
            }
        }

        #endregion

        #region BarcodeScannerEventArgs

        /// <summary>
        /// Contains data for barcode events.
        /// </summary>
        public class BarcodeScannerEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the part of the barcode between the prefix and postfix.
            /// </summary>
            public string Output { get; set; }

            /// <summary>
            /// Gets or sets the postfix (if available) of the barcode.
            /// </summary>
            public char? Postfix { get; set; }
        }

        #endregion
    }
}