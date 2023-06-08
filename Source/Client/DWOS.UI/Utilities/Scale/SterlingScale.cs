using NLog;
using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DWOS.UI.Utilities.Scale
{
    /// <summary>
    /// Implementation of <see cref="IScale"/> for Sterling Scale scales
    /// designed for XC 780A.
    /// </summary>
    public sealed class SterlingScale : IScale
    {
        #region Fields

        private const int BufferSize = 1024;
        private const string SterlingNewLine = "\r\n";
        private readonly Regex _endOfInputRegex = new Regex(@"\r\n[ ]*\r\n$");
        private const string CommandPrint = "b";
        private const string CommandZero = "a";
        private const string CommandTare = "d";

        private readonly object _portAccessLock = new object();
        private StringBuilder _currentWeightResponse;
        private byte[] _buffer;
        private readonly SerialPort _port;
        private bool _expectingContent;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the SterlingScale class.
        /// </summary>
        /// <param name="portName">The name of the serial port.</param>
        /// <exception cref="ArgumentNullException">portName is null or empty.</exception>
        public SterlingScale(string portName)
        {
            if (string.IsNullOrEmpty(portName))
            {
                throw new ArgumentNullException(nameof(portName));
            }

            PortName = portName;
            _port = new SerialPort(PortName, 9600, Parity.None, 8, StopBits.One);
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposing)
            {
                return;
            }

            Close();
        }

        private void ReadCallback(IAsyncResult result)
        {
            lock (_portAccessLock)
            {
                if (_buffer == null || _currentWeightResponse == null)
                {
                    // Read canceled
                    return;
                }

                var receivedLength = -1;

                try
                {
                    receivedLength = _port.BaseStream.EndRead(result);
                }
                catch (IOException)
                {
                    // Stream closed or internal error.
                }
                catch (InvalidOperationException)
                {
                    // Assumption: base stream was closed prior to the end of read.
                }

                if (receivedLength > 0 && _expectingContent)
                {
                    var currentReadContent = Encoding.ASCII.GetString(_buffer, 0, receivedLength);
                    _currentWeightResponse.Append(currentReadContent);
                }

                if (_endOfInputRegex.IsMatch(_currentWeightResponse.ToString()))
                {
                    FinishCurrentRead();
                }

                _port.BaseStream.BeginRead(_buffer, 0, BufferSize, ReadCallback, null);
            }
        }

        private void FinishCurrentRead()
        {
            _expectingContent = false;

            var weightResponse = _currentWeightResponse.ToString();
            _logger.Info("Successfully read data from scale: " + Environment.NewLine + weightResponse);

            var responseLines = weightResponse.Split(new [] { SterlingNewLine }, StringSplitOptions.RemoveEmptyEntries);
            ScaleData data = new ScaleData();

            const string fieldRegexPattern = @"^
                (\w+(?:\s\w+)?) # name
                \s+
                (-?\d+\.?\d*)    # value
                \s?.*          # units of measure (if any)
            $";

            Regex fieldRegex = new Regex(fieldRegexPattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            foreach (var responseLine in responseLines)
            {
                var fieldMatches = fieldRegex.Matches(responseLine);
                string name = string.Empty;
                decimal value = 0M;
                if (fieldMatches.Count > 0)
                {
                    name = fieldMatches[0].Groups[1].Value.ToUpper();
                    value = decimal.Parse(fieldMatches[0].Groups[2].Value);
                }

                switch (name)
                {
                    case "GROSS WT":
                        data.GrossWeight = value;
                        break;
                    case "TARE WT":
                        data.TareWeight = value;
                        break;
                    case "NET WT":
                        data.NetWeight = value;
                        break;
                    case "QUANTITY":
                        data.Pieces = Convert.ToInt32(value);
                        break;
                }
            }

            _currentWeightResponse.Clear();

            ScaleDataReceived?.Invoke(this, new ScaleDataReceivedEventArgs(data));
            _logger.Info("Parsed scale data: {0}", data);
        }

        private void Write(string value)
        {
            _logger.Info("Sending to scale: " + value);

            var valueBytes = Encoding.ASCII.GetBytes(value);

            lock (_portAccessLock)
            {
                _port.BaseStream.BeginWrite(valueBytes, 0, valueBytes.Length, WriteCallback, null);
            }
        }

        private void WriteCallback(IAsyncResult ar)
        {
            lock (_portAccessLock)
            {
                if (!_port.IsOpen)
                {
                    return;
                }
                _port.BaseStream.EndWrite(ar);
            }
        }

        #endregion

        #region IScale Members

        public event EventHandler<ScaleDataReceivedEventArgs> ScaleDataReceived;

        public bool IsOpen
        {
            get
            {
                lock (_portAccessLock)
                {
                    return _port.IsOpen;
                }
            }
        }

        public string PortName
        {
            get;
        }

        public void Open()
        {
            lock (_portAccessLock)
            {
                if (_port.IsOpen || !SerialPort.GetPortNames().Contains(PortName))
                {
                    return;
                }

                try
                {
                    _port.Open();
                    _buffer = new byte[BufferSize];
                    _currentWeightResponse = new StringBuilder();

                    // Start reading
                    _port.BaseStream.BeginRead(_buffer, 0, BufferSize, ReadCallback, null);
                }
                catch (UnauthorizedAccessException exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Error accessing sterling scale");
                }
            }
        }

        public void Close()
        {
            lock (_portAccessLock)
            {
                if (_port.IsOpen)
                {
                    _port.DiscardInBuffer();
                    _port.DiscardOutBuffer();
                    _port.Close();
                }

                _buffer = null;
                _currentWeightResponse = new StringBuilder();
            }
        }

        public void Zero()
        {
            Write(CommandZero);
        }

        public void Tare()
        {
            Write(CommandTare);
        }

        public void Print()
        {
            _expectingContent = true;
            Write(CommandPrint);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
