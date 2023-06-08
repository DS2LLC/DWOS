using System.ComponentModel;

namespace DWOS.UI.Utilities.Calculators
{
    /// <summary>
    /// Represents data used by <see cref="RateWindow"/>.
    /// </summary>
    public sealed class RateContext : INotifyPropertyChanged
    {
        #region Fields

        private decimal _feetPerMinute;
        private decimal _rackSpacing;
        private int _barsPerRack;
        private int _partsPerBar;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the feet per minute rate.
        /// </summary>
        public decimal FeetPerMinute
        {
            get
            {
                return _feetPerMinute;
            }

            set
            {
                if (_feetPerMinute != value)
                {
                    _feetPerMinute = value;
                    OnPropertyChanged(nameof(FeetPerMinute));
                    OnPropertyChanged(nameof(RacksPerHour));
                    OnPropertyChanged(nameof(PartsPerHour));
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount of space between racks.
        /// </summary>
        public decimal RackSpacing
        {
            get
            {
                return _rackSpacing;
            }

            set
            {
                if (_rackSpacing != value)
                {
                    _rackSpacing = value;
                    OnPropertyChanged(nameof(RackSpacing));
                    OnPropertyChanged(nameof(RacksPerHour));
                    OnPropertyChanged(nameof(PartsPerHour));
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of bars in a rack.
        /// </summary>
        public int BarsPerRack
        {
            get
            {
                return _barsPerRack;
            }

            set
            {
                if (_barsPerRack != value)
                {
                    _barsPerRack = value;
                    OnPropertyChanged(nameof(BarsPerRack));
                    OnPropertyChanged(nameof(PartsPerRack));
                    OnPropertyChanged(nameof(PartsPerHour));
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of parts per bar.
        /// </summary>
        public int PartsPerBar
        {
            get
            {
                return _partsPerBar;
            }

            set
            {
                if (_partsPerBar != value)
                {
                    _partsPerBar = value;
                    OnPropertyChanged(nameof(PartsPerBar));
                    OnPropertyChanged(nameof(PartsPerRack));
                    OnPropertyChanged(nameof(PartsPerHour));
                }
            }
        }

        /// <summary>
        /// Gets the calculated rate of racks per hour.
        /// </summary>
        public decimal RacksPerHour
        {
            get
            {
                if (_rackSpacing == 0)
                {
                    return 0;
                }
                else
                {
                    const int MINUTES_PER_HOUR = 60;
                    decimal feetPerHour = MINUTES_PER_HOUR * _feetPerMinute;
                    return feetPerHour / _rackSpacing;
                }
            }
        }

        /// <summary>
        /// Gets the calculated number of parts per rack.
        /// </summary>
        public int PartsPerRack
        {
            get
            {
                return _barsPerRack * _partsPerBar;
            }
        }

        /// <summary>
        /// Gets the calculated rate of production.
        /// </summary>
        public decimal PartsPerHour
        {
            get
            {
                return RacksPerHour * PartsPerRack;
            }
        }

        #endregion

        #region Methods

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
    }
}
