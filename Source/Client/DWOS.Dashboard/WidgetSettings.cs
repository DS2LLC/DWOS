using System.ComponentModel;
using System.Runtime.Serialization;

namespace DWOS.Dashboard
{
    /// <summary>
    /// Base class for per-widget settings.
    /// </summary>
    [DataContract]
    public class WidgetSettings : INotifyPropertyChanged
    {
        #region Fields

        private int _column = -1;
        private int _row = -1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the widget's column.
        /// </summary>
        [DataMember]
        public int TileColumn
        {
            get { return _column; }
            set
            {
                _column = value;
                OnPropertyChanged("TileColumn");
            }
        }

        /// <summary>
        /// Gets or sets the widget's row.
        /// </summary>
        [DataMember]
        public int TileRow
        {
            get { return _row; }
            set
            {
                _row = value;
                OnPropertyChanged("TileRow");
            }
        }

        #endregion

        #region Methods

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}