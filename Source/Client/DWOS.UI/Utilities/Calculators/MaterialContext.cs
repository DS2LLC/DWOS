using System;
using System.ComponentModel;
using System.Windows.Input;

namespace DWOS.UI.Utilities.Calculators
{
    public sealed class MaterialContext : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Fields

        /// <summary>
        /// Raised when the window should close successfully.
        /// </summary>
        public event EventHandler Accept;

        private decimal _squareFootMaterialCost;
        private double _partSquareFoot;
        private double _persistedPartSquareFoot;

        #endregion

        #region Properties

        public ICommand AcceptCommand
        {
            get;
            private set;
        }

        public decimal SquareFootMaterialCost
        {
            get
            {
                return _squareFootMaterialCost;
            }
            set
            {
                if (_squareFootMaterialCost != value)
                {
                    _squareFootMaterialCost = value;
                    OnPropertyChanged(nameof(SquareFootMaterialCost));
                    OnPropertyChanged(nameof(MaterialCostPerPart));
                }
            }
        }

        public double PartSquareFoot
        {
            get
            {
                return _partSquareFoot;
            }
            set
            {
                if (_partSquareFoot != value)
                {
                    _partSquareFoot = value;
                    OnPropertyChanged(nameof(PartSquareFoot));
                    OnPropertyChanged(nameof(MaterialCostPerPart));
                }
            }
        }

        public double PersistedPartSquareFoot
        {
            get
            {
                return _persistedPartSquareFoot;
            }
            set
            {
                if (_persistedPartSquareFoot != value)
                {
                    _persistedPartSquareFoot = value;
                    OnPropertyChanged(nameof(PersistedPartSquareFoot));
                }
            }
        }

        public decimal MaterialCostPerPart
        {
            get
            {
                return _squareFootMaterialCost * Convert.ToDecimal(_partSquareFoot);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialContext"/> class.
        /// </summary>
        public MaterialContext()
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

        public string this[string columnName]
        {
            get
            {
                const decimal maxCost = 999999.9999M;
                const decimal minCost = 0;

                const double maxArea = 9999999D;
                const double minArea = 0D;
                string returnValue = string.Empty;

                if (columnName == nameof(SquareFootMaterialCost))
                {
                    if (_squareFootMaterialCost > maxCost)
                    {
                        returnValue = "Square Foot Material Cost exceeds maximum value.";
                    }
                    else if (_squareFootMaterialCost < minCost)
                    {
                        returnValue = "Square Foot Overhead Cost cannot be negative.";
                    }
                }
                else if (columnName == nameof(PartSquareFoot))
                {
                    if (_partSquareFoot > maxArea)
                    {
                        returnValue = "Part Area exceeds maximum value.";
                    }
                    else if (_partSquareFoot < minArea)
                    {
                        returnValue = "Part Area cannot be negative.";
                    }
                }
                else if (columnName == nameof(MaterialCostPerPart))
                {
                    var costPerPart = MaterialCostPerPart;
                    if (costPerPart > maxCost)
                    {
                        return "Cost Per Part exceeds maximum value.";
                    }
                    else if (costPerPart < minCost)
                    {
                        return "Cost Per Part cannot be negative.";
                    }
                }

                return returnValue;
            }
        }

        public string Error
        {
            get
            {
                return this[nameof(SquareFootMaterialCost)] +
                    this[nameof(PartSquareFoot)] +
                    this[nameof(MaterialCostPerPart)];
            }
        }

        #endregion

        #region AcceptDialogCommand

        private sealed class AcceptDialogCommand : ICommand
        {
            #region Properties

            public MaterialContext Instance
            {
                get;
                private set;
            }
            #endregion

            #region Methods

            public AcceptDialogCommand(MaterialContext instance)
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
                handler?.Invoke(Instance, new EventArgs());
            }

            #endregion
        }

        #endregion
    }
}
