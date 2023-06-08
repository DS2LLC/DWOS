using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Neodynamic.SDK.Printing;
using System.Printing;

namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for PrintJobDialog.xaml
    /// </summary>
    public partial class PrintJobDialog : Window
    {

        #region Fields

        PrinterSettings _printerSettings = new PrinterSettings();
        int _copies = 1;
        PrintOrientation _printOrientation = PrintOrientation.Portrait;
        private bool _isClosing;
        private object _isClosingLock = new object();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the selected printer settings.
        /// </summary>
        /// <remarks>
        /// The value is synced when the dialog closes.
        /// </remarks>
        public PrinterSettings PrinterSettings
        {
            get { return _printerSettings; }
        }

        /// <summary>
        /// Gets the selected number of copies.
        /// </summary>
        /// <remarks>
        /// The value is synced when the dialog closes.
        /// </remarks>
        public int Copies
        {
            get { return _copies; }
        }

        /// <summary>
        /// Gets or sets the selected print orientation.
        /// </summary>
        /// <remarks>
        /// The retrieved value may not be current. It is synced when the
        /// dialog closes.
        /// </remarks>
        public PrintOrientation PrintOrientation
        {
            get { return _printOrientation; }
            set
            {
                _printOrientation = value;
                cboPrintOrientation.SelectedItem = value.ToString();
            }
        }

        #endregion

        #region Methods

        public PrintJobDialog()
        {
            InitializeComponent();

            this.Init();
        }

        private void Init()
        {
            this.cboProgLang.SelectedIndex = 0;
            this.cboPrintOrientation.SelectedIndex = 0;

            //Load installed printers...
            LocalPrintServer printServer = new LocalPrintServer();
            PrintQueueCollection printQueuesOnLocalServer = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local });
            List<string> installedPrinters = new List<string>();
            foreach (PrintQueue printer in printQueuesOnLocalServer)
            {
                installedPrinters.Add(printer.Name);
            }

            this.cboPrinters.DataContext = installedPrinters;

            //Load Serial Comm settings...
            this.cboSerialPorts.DataContext = System.IO.Ports.SerialPort.GetPortNames();
            this.cboParity.DataContext = Enum.GetNames(typeof(SerialPortParity));
            this.cboStopBits.DataContext = Enum.GetNames(typeof(SerialPortStopBits));
            this.cboFlowControl.DataContext = Enum.GetNames(typeof(SerialPortHandshake));
            this.cboPrintOrientation.DataContext = Enum.GetNames(typeof(PrintOrientation));
        }

        #endregion

        #region Events

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // This handler takes a while, so users can click on the
            // 'OK' button more than once.
            lock (_isClosingLock)
            {
                if (_isClosing)
                {
                    return;
                }

                _isClosing = true;
            }

            try
            {
                //Update printer comm object...
                if (this.tabPrintComm.SelectedIndex == 0)
                {
                    //USB
                    _printerSettings.Communication.CommunicationType = CommunicationType.USB;
                    _printerSettings.PrinterName = cboPrinters.SelectedItem?.ToString();
                }
                else if (this.tabPrintComm.SelectedIndex == 1)
                {
                    //Parallel
                    _printerSettings.Communication.CommunicationType = CommunicationType.Parallel;
                    _printerSettings.Communication.ParallelPortName = this.txtParallelPort.Text;
                }
                else if (this.tabPrintComm.SelectedIndex == 2)
                {
                    //Serial
                    _printerSettings.Communication.CommunicationType = CommunicationType.Serial;
                    _printerSettings.Communication.SerialPortName = cboSerialPorts.SelectedItem?.ToString();

                    int baud;
                    if (int.TryParse(this.txtBaudRate.Text, out baud))
                    {
                        _printerSettings.Communication.SerialPortBaudRate = baud;
                    }

                    int dataBits;
                    if (int.TryParse(this.txtDataBits.Text, out dataBits))
                    {
                        _printerSettings.Communication.SerialPortDataBits = dataBits;
                    }

                    SerialPortHandshake handshake;
                    if (Enum.TryParse(cboFlowControl.SelectedItem?.ToString(), out handshake))
                    {
                        _printerSettings.Communication.SerialPortFlowControl = handshake;
                    }

                    SerialPortParity parity;
                    if (Enum.TryParse(cboParity.SelectedItem?.ToString(), out parity))
                    {
                        _printerSettings.Communication.SerialPortParity = parity;
                    }

                    SerialPortStopBits stopBits;
                    if (Enum.TryParse(cboStopBits.SelectedItem?.ToString(), out stopBits) &&
                        stopBits != SerialPortStopBits.None)
                    {
                        _printerSettings.Communication.SerialPortStopBits = stopBits;
                    }
                }
                else if (this.tabPrintComm.SelectedIndex == 3)
                {
                    //Network
                    _printerSettings.Communication.CommunicationType = CommunicationType.Network;
                    System.Net.IPAddress ipAddress = System.Net.IPAddress.None;

                    try
                    {
                        ipAddress = System.Net.IPAddress.Parse(this.txtIPAddress.Text);
                    }
                    catch (FormatException)
                    {
                        // Do nothing - not a valid IP
                    }
                    catch (ArgumentException)
                    {
                        // Do nothing - blank
                    }

                    if (ipAddress != System.Net.IPAddress.None) //use IP
                        _printerSettings.Communication.NetworkIPAddress = ipAddress;
                    else //try Host Name
                        _printerSettings.PrinterName = this.txtIPAddress?.Text;

                    int port;
                    if (int.TryParse(this.txtIPPort.Text, out port))
                    {
                        _printerSettings.Communication.NetworkPort = port;
                    }
                }

                double dpi;
                if (double.TryParse(this.txtDpi.Text, out dpi))
                {
                    _printerSettings.Dpi = dpi;
                }

                ProgrammingLanguage programmingLanguage;
                if (Enum.TryParse(((ComboBoxItem)cboProgLang.SelectedItem).Content.ToString(), out programmingLanguage))
                {
                    _printerSettings.ProgrammingLanguage = programmingLanguage;
                }


                int copies;
                if (int.TryParse(this.txtCopies.Text, out copies))
                {
                    _copies = copies;
                }
                else
                {
                    _copies = 1;
                }

                PrintOrientation printOrientation;
                if (Enum.TryParse(cboPrintOrientation.SelectedItem.ToString(), out printOrientation))
                {
                    _printOrientation = printOrientation;
                }
                else
                {
                    _printOrientation = PrintOrientation.Portrait;
                }

                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, "print error");
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
            }

        }

        #endregion
    }
}
