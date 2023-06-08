using System.ComponentModel;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Base class for view models that adds validation to MVVM Light's
    /// <see cref="GalaSoft.MvvmLight.ViewModelBase"/>.
    /// </summary>
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase, IDataErrorInfo
    {
        #region Methods

        /// <summary>
        /// Returns a string containing error messages for all properties.
        /// </summary>
        /// <returns></returns>
        public virtual string ValidateAll() => string.Empty;

        /// <summary>
        /// Returns a string containing an error message for a specific property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual string Validate(string propertyName) => string.Empty;

        #endregion

        #region IDataErrorInfo Members

        public string Error => ValidateAll();

        public string this[string columnName] => Validate(columnName);

        #endregion

    }
}
