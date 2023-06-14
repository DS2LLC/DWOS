using System.ComponentModel;

namespace DWOS.ViewModels
{
    /// <summary>
    /// Base class for implementing INotifyPropertyChanged
    /// </summary>
    public class PropertyChangedBase : INotifyPropertyChanged
    {

        #region Methods

        /// <summary>
        /// Protected method for firing PropertyChanged
        /// </summary>
        /// <param name="propertyName">The name of the property that changed</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
