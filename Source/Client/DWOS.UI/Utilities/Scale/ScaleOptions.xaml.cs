using DWOS.Data;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DWOS.UI.Utilities.Scale
{
    /// <summary>
    /// Interaction logic for ScaleOptions.xaml
    /// </summary>
    public partial class ScaleOptions : Window
    {
        #region Fields

        private const string SCALE_TYPE_NONE = "None";

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the ScaleOptions class.
        /// </summary>
        public ScaleOptions()
        {
            InitializeComponent();
            Icon = Properties.Resources.Settings_32.ToWpfImage();
            var model = new ScaleOptionsViewModel();
            model.Exit += model_Exit;
            model.PropertyChanged += model_PropertyChanged;

            this.DataContext = model;

            this.cboPort.IsEnabled = model.SelectedScaleType != SCALE_TYPE_NONE;
        }

        #endregion

        #region Events

        void model_Exit(object sender, EventArgs e)
        {
            this.DialogResult = true;
        }

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedScaleType")
            {
                var model = this.DataContext as ScaleOptionsViewModel;
                this.cboPort.IsEnabled = !(model == null || model.SelectedScaleType == SCALE_TYPE_NONE);
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as ScaleOptionsViewModel;

            if (model != null)
            {
                model.Exit -= model_Exit;
                model.PropertyChanged -= model_PropertyChanged;
            }
        }

        #endregion

        #region ScaleOptionsViewModel

        /// <summary>
        /// Used as a DataContext for ScaleOptions.
        /// </summary>
        private sealed class ScaleOptionsViewModel : INotifyPropertyChanged, IDataErrorInfo
        {
            #region Fields

            private readonly List<string> _availablePorts;
            private readonly Dictionary<string, ScaleType> _displayStringTypeMapping;
            private readonly Dictionary<ScaleType, string> _typeDisplayStringMapping;
            private string _selectedPort;
            private ScaleType _selectedScaleType;
            private Logger _log;

            /// <summary>
            /// Signals that the ViewModel wants to close the dialog.
            /// </summary>
            public event EventHandler Exit;

            #endregion

            #region Properties

            /// <summary>
            /// Gets a collection containing the names of every available serial port.
            /// </summary>
            public IEnumerable<string> Ports
            {
                get
                {
                    return this._availablePorts;
                }
            }

            /// <summary>
            /// Gets or sets the currently selected port.
            /// </summary>
            public string SelectedPort
            {
                get
                {
                    return this._selectedPort;
                }
                set
                {
                    if (this._selectedPort != value)
                    {
                        this._selectedPort = value;
                        this.OnPropertyChanged("SelectedPort");
                    }
                }
            }

            /// <summary>
            /// Gets a list of scale types.
            /// </summary>
            public IEnumerable<string> ScaleTypes
            {
                get
                {
                    return this._displayStringTypeMapping.Keys;
                }
            }

            /// <summary>
            /// Gets or sets the currently selected scale type.
            /// </summary>
            public string SelectedScaleType
            {
                get
                {
                    return this._typeDisplayStringMapping[this._selectedScaleType];
                }
                set
                {
                    var currentValue = this._typeDisplayStringMapping[this._selectedScaleType];

                    if (currentValue != value && this._displayStringTypeMapping.ContainsKey(value))
                    {
                        this._selectedScaleType = this._displayStringTypeMapping[value];
                        OnPropertyChanged("SelectedScaleType");
                        OnPropertyChanged("SelectedPort"); // trigger validation
                    }
                }
            }

            /// <summary>
            /// Command for the 'Save' button.
            /// </summary>
            public ICommand SaveCommand
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the ScaleOptionsViewModel class.
            /// </summary>
            public ScaleOptionsViewModel()
            {
                this._log = NLog.LogManager.GetCurrentClassLogger();

                this._displayStringTypeMapping = new Dictionary<string, ScaleType>()
                {
                    { "None", ScaleType.None },
                    { "Sterling Scale (770A)", ScaleType.Sterling },
                };

                this._typeDisplayStringMapping = this._displayStringTypeMapping.ToDictionary(m => m.Value, m => m.Key);

                this._availablePorts = new List<string>();
                try
                {
                    foreach (string portName in SerialPort.GetPortNames().OrderBy(name => name))
                    {
                        if (portName.ToUpper().StartsWith("COM"))
                        {
                            this._availablePorts.Add(portName);
                        }
                    }
                }
                catch (Win32Exception ex)
                {
                    this._log.Debug(ex, "Could not retrieve information about serial ports.");
                }

                var selectedPort = UserSettings.Default.ScalePortName;
                if (string.IsNullOrEmpty(selectedPort))
                {
                    this._selectedPort = this._availablePorts.FirstOrDefault();
                }
                else if (this._availablePorts.Contains(selectedPort))
                {
                    this._selectedPort = selectedPort;
                }
                else
                {
                    this._log.Debug("User settings specify unavailable serial port named \"{0}.\"", selectedPort);
                    this._selectedPort = this._availablePorts.FirstOrDefault();
                }

                this._selectedScaleType = UserSettings.Default.ScaleType;
                this.SaveCommand = new SaveCommand(this);
            }

            /// <summary>
            /// Saves data and fires the Exit event.
            /// </summary>
            public void Save()
            {
                UserSettings.Default.ScaleType = this._selectedScaleType;
                UserSettings.Default.ScalePortName = this._selectedPort;
                UserSettings.Default.Save();

                var handler = this.Exit;

                if (handler != null)
                {
                    handler(this, new EventArgs());
                }
            }

            private void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;

                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            #region IDataErrorInfo Members

            public string Error
            {
                get
                {
                    return this[nameof(SelectedPort)];
                }
            }

            public string this[string columnName]
            {
                get
                {
                    string errorMsg = string.Empty;
                    var selectedPortInvalid = columnName == nameof(SelectedPort) &&
                        _selectedScaleType != ScaleType.None &&
                        string.IsNullOrEmpty(_selectedPort);

                    if (selectedPortInvalid)
                    {
                        errorMsg = "Using a scale requires a serial port.";
                    }

                    return errorMsg;
                }
            }

            #endregion
        }

        #endregion

        #region SaveCommand

        /// <summary>
        /// 'Save command' implementation of ICommand.
        /// </summary>
        private sealed class SaveCommand : ICommand
        {
            #region Properties

            /// <summary>
            /// Gets the view model instance.
            /// </summary>
            public ScaleOptionsViewModel Instance
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the SaveCommand class.
            /// </summary>
            /// <param name="instance">The instance of the view model.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// Instance is null.
            /// </exception>
            public SaveCommand(ScaleOptionsViewModel instance)
            {
                if (instance == null)
                {
                    throw new ArgumentNullException("instance");
                }

                this.Instance = instance;
            }

            #endregion

            #region ICommand Members

            public bool CanExecute(object parameter)
            {
                return string.IsNullOrEmpty(Instance.Error);
            }

            public event EventHandler CanExecuteChanged
            {
                add
                {
                    CommandManager.RequerySuggested += value;
                }
                remove
                {
                    CommandManager.RequerySuggested -= value;
                }
            }

            public void Execute(object parameter)
            {
                Instance.Save();
            }

            #endregion
        }

        #endregion
    }
}
