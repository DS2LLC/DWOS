using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DWOS.UI.Utilities.Calculators
{
    public sealed class OverheadContext : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Fields

        /// <summary>
        /// Raised when the window should close successfully.
        /// </summary>
        public event EventHandler Accept;

        private decimal _hourlyOverhead;
        private decimal _partsPerHour;

        #endregion

        #region Properties


        public decimal HourlyOverheadCost
        {
            get
            {
                return _hourlyOverhead;
            }

            set
            {
                if (_hourlyOverhead != value)
                {
                    _hourlyOverhead = value;
                    OnPropertyChanged(nameof(HourlyOverheadCost));
                    OnPropertyChanged(nameof(HourlyOverheadCostPerPart));
                }
            }
        }

        /// <summary>
        /// Gets or sets a rate.
        /// </summary>
        /// <remarks>
        /// The rate should have been calculated in a previous step.
        /// </remarks>
        public decimal PartsPerHour
        {
            get
            {
                return _partsPerHour;
            }
            set
            {
                if (_partsPerHour != value)
                {
                    _partsPerHour = value;
                    OnPropertyChanged(nameof(PartsPerHour));
                    OnPropertyChanged(nameof(HourlyOverheadCostPerPart));
                }
            }
        }

        public decimal HourlyOverheadCostPerPart
        {
            get
            {
                if (_partsPerHour == 0)
                {
                    return 0M;
                }

                return _hourlyOverhead / Convert.ToDecimal(_partsPerHour);
            }
        }

        public ICommand AcceptCommand
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="OverheadContext"/> class.
        /// </summary>
        public OverheadContext()
        {
            AcceptCommand = new AcceptDialogCommand(this);
        }

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                return this[nameof(HourlyOverheadCostPerPart)] +
                    this[nameof(HourlyOverheadCost)] +
                    this[nameof(PartsPerHour)];
            }
        }

        public string this[string columnName]
        {
            get
            {
                const decimal maxCost = 999999.9999M;
                const decimal minCost = 0;
                string returnValue = string.Empty;
                if (columnName == nameof(HourlyOverheadCost))
                {
                    if (_hourlyOverhead > maxCost)
                    {
                        return "Hourly Overhead Cost exceeds maximum value.";
                    }
                    else if (_hourlyOverhead < minCost)
                    {
                        return "Hourly Overhead Cost cannot be negative.";
                    }
                }
                else if (columnName == nameof(HourlyOverheadCostPerPart))
                {
                    var costPerPart = HourlyOverheadCostPerPart;
                    if (costPerPart > maxCost)
                    {
                        return "Cost Per Part exceeds maximum value.";
                    }
                    else if (costPerPart < minCost)
                    {
                        return "Cost Per Part cannot be negative.";
                    }
                    else if (_partsPerHour == 0)
                    {
                        return "Parts Per Hour is 0 - cannot calculate Cost Per Part.";
                    }
                }
                else if (columnName == nameof(PartsPerHour) && _partsPerHour == 0)
                {
                    return "Parts Per Hour cannot be 0.";
                }

                return returnValue;
            }
        }

        #endregion

        #region AcceptDialogCommand

        private sealed class AcceptDialogCommand : ICommand
        {
            #region Properties

            public OverheadContext Instance
            {
                get;
                private set;
            }
            #endregion

            #region Methods

            public AcceptDialogCommand(OverheadContext instance)
            {
                Instance = instance;
                Instance.PropertyChanged += Instance_PropertyChanged;
            }

            #endregion

            #region Events

            private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                var handler = CanExecuteChanged;

                if (handler != null)
                {
                    handler(this, new EventArgs());
                }
            }

            #endregion

            #region ICommand Members

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return string.IsNullOrEmpty(Instance?.Error);
            }

            public void Execute(object parameter)
            {
                if (!string.IsNullOrEmpty(Instance?.Error))
                {
                    return;
                }

                var handler = Instance.Accept;

                if (handler != null)
                {
                    handler(Instance, new EventArgs());
                }
            }

            #endregion
        }

        #endregion
    }
}
