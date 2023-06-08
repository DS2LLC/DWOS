using System;
using System.IO.Ports;
using NLog;

namespace DWOS.UI.Utilities.PartMarking
{
    public abstract class VideoJetDeviceBase : IPartMarkDevice
    {
        #region Fields

        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected const string END_OF_LINE        = "0D";
        protected const string CLEAR_BUFFERS      = "1B,01,01";
        protected const string CLEAR_BUFFERS2     = "1B,01,40";
        protected const string REMOTE_OFF         = "1B,01,1C";
        protected const string REMOTE_ON          = "1B,01,1D";
        protected const string BLANK_SPACE        = "00";

        private SerialPort _serialPort;

        #endregion

        #region Properties

        public PartMarkSettings Settings { get; }

        #endregion

        #region Methods

        protected VideoJetDeviceBase(PartMarkSettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void Open()
        {
            _log.Info("Opening part marker connection.");
            
            if (_serialPort == null)
            {
                _serialPort = new SerialPort(Settings.PortName);
                _serialPort.DataReceived += serialPort_DataReceived;
                _serialPort.ErrorReceived += serialPort_ErrorReceived;
            }

            //turn remote messages on, will disable use of Part Marking User Interface
            SendMessage("SetModeRemoteMessages", REMOTE_ON);
        }

        public void Close()
        {
            _log.Info("Closing part marker connection.");

            //turn remote messages off
            SendMessage("SetModeNotRemoteMessages", REMOTE_OFF);

            if (_serialPort != null)
            {
                _serialPort.DataReceived -= serialPort_DataReceived;
                _serialPort.ErrorReceived -= serialPort_ErrorReceived;
                _serialPort.Dispose();
            }

            _serialPort = null;
        }

        public void ClearBuffer()
        {
            SendMessage("Clear Internal and External Buffers", CLEAR_BUFFERS);
            SendMessage("Clear Buffer", CLEAR_BUFFERS2);
        }

        public void SetFont(int lineCount, MultiStroke stroke, PartMarkFontSize fontSize)
        {
            var multiStrokeCode = Settings.GetMultiStrokeCode(stroke);

            if(multiStrokeCode != null)
                SendMessage(stroke.ToString(), multiStrokeCode.HexCode);
            else
                _log.Warn("No matching multi stroke code found for {0}.".FormatWith(stroke));

            var matrix = Settings.GetFontMatrix(lineCount, fontSize);

            if (matrix != null)
                SendMessage(matrix.Name, matrix.HexCode);
            else
                _log.Warn("No matching matrix code found for font size {0} and {1} number of lines.".FormatWith(fontSize, lineCount));
        }

        public void SetCustomFont(string text)
        {
            SendMessage("Custom Font", text);
        }

        public abstract void WriteText(params string[] lines);

        protected void SendMessage(string commandDisplayName, string message, bool isHex = true)
        {
            var messageToSend = message;

            if(isHex)
                messageToSend = HexConverter.ConvertCodes(message.Split(","));

            _log.Debug("Sending command: " + commandDisplayName);

            if(!_serialPort.IsOpen)
                _serialPort.Open();

            _serialPort.WriteLine(messageToSend);
            _serialPort.Close();

            SentMessages?.Invoke(commandDisplayName + ": " + message);
        }

        public Action<string> SentMessages { get; set; }
        
        public Action<string, bool> ReceivedMessages { get; set; }

        #endregion

        #region Events

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    string msg = _serialPort.ReadExisting();

                    ReceivedMessages?.Invoke(msg, false);
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error on receiving data.";
                _log.Error(exc, errorMsg);
            }
        }

        private void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    string msg = _serialPort.ReadExisting();

                    ReceivedMessages?.Invoke(msg, true);
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error on receiving data.";
                _log.Error(exc, errorMsg);
            }
        }

        #endregion
    }
}
