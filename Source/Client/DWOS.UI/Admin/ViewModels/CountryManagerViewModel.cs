using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.Shared.Utilities;
using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DWOS.UI.Admin.ViewModels
{
    /// <summary>
    /// View model for <see cref="CountryManager"/>.
    /// </summary>
    public class CountryManagerViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Occurs when the user accepts the dialog.
        /// </summary>
        public event EventHandler Accepted;

        /// <summary>
        /// Occurs when the user adds a new country.
        /// </summary>
        public event EventHandler<EventArgsTemplate<CountryEntry>> CountryAdded;

        private readonly ApplicationSettingsDataSet.CountryDataTable _dtCountry =
            new ApplicationSettingsDataSet.CountryDataTable();

        private CountryEntry _selectedCountry;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of countries for this instance.
        /// </summary>
        public ObservableCollection<CountryEntry> Countries { get; } =
            new ObservableCollection<CountryEntry>();

        /// <summary>
        /// Gets or sets the selected country for this instance.
        /// </summary>
        public CountryEntry SelectedCountry
        {
            get => _selectedCountry;
            set => Set(nameof(SelectedCountry), ref _selectedCountry, value);
        }

        /// <summary>
        /// The 'add country' command.
        /// </summary>
        public ICommand Add { get; }

        /// <summary>
        /// The 'delete country' command.
        /// </summary>
        public ICommand Delete { get; }

        /// <summary>
        /// The 'save changes' command.
        /// </summary>
        public ICommand Save { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="CountryManagerViewModel"/> class.
        /// </summary>
        public CountryManagerViewModel()
        {
            Add = new RelayCommand(DoAdd);
            Delete = new RelayCommand(DoDeleteSelected, CanDeleteSelected);

            Save = new RelayCommand(DoSave, CanSave);

            if (IsInDesignMode)
            {
                var displayRow = _dtCountry.NewCountryRow();
                displayRow.Name = "USA";
                Countries.Add(new CountryEntry(displayRow, false));
            }
        }

        /// <summary>
        /// Loads data from the database.
        /// </summary>
        public void LoadData()
        {
            // Clear existing country entries
            Countries.Clear();

            // Load countries from database
            using (var taCountry = new CountryTableAdapter())
            {
                taCountry.Fill(_dtCountry);

                foreach (var countryRow in _dtCountry.OrderBy(c => c.Name))
                {
                    var isInUse = taCountry.GetUsageCount(countryRow.CountryID) > 0;
                    Countries.Add(new CountryEntry(countryRow, isInUse));
                }
            }
        }

        /// <summary>
        /// Performs view model cleanup.
        /// </summary>
        public void Unload()
        {
            _dtCountry.Dispose();
        }

        private void DoAdd()
        {
            try
            {
                var newRow = _dtCountry.NewCountryRow();
                newRow.Name = "New Country";
                _dtCountry.AddCountryRow(newRow);

                var newCountry = new CountryEntry(newRow, false);
                Countries.Add(newCountry);
                CountryAdded?.Invoke(this, new EventArgsTemplate<CountryEntry>(newCountry));
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error sending 'show add dialog' message.");
            }
        }

        private void DoDeleteSelected()
        {
            try
            {
                var selectedCountry = _selectedCountry;

                if (selectedCountry == null || !selectedCountry.CanDelete)
                {
                    return;
                }

                Countries.Remove(selectedCountry);
                selectedCountry.Row.Delete();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error deleting country.");
            }
        }

        private bool CanDeleteSelected() =>
            _selectedCountry != null && _selectedCountry.CanDelete;

        private void DoSave()
        {
            try
            {
                using (var taCountry = new CountryTableAdapter())
                {
                    taCountry.Update(_dtCountry);
                }

                Accepted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, $"Error saving ${nameof(CountryManager)}.");
            }
        }

        private bool CanSave() => Countries.Count > 0 &&
            Countries.All(c => !string.IsNullOrEmpty(c.Name));

        #endregion

        #region CountryEntry

        /// <summary>
        /// Represents a country entry.
        /// </summary>
        public class CountryEntry : ViewModelBase
        {
            #region Properties

            /// <summary>
            /// Gets the row for this instance.
            /// </summary>
            public ApplicationSettingsDataSet.CountryRow Row { get; }

            /// <summary>
            /// Gets or sets the name for this instance.
            /// </summary>
            public string Name
            {
                get => Row.Name;
                set
                {
                    if (!string.Equals(Row.Name, value, StringComparison.Ordinal))
                    {
                        Row.Name = value ?? string.Empty;
                        RaisePropertyChanged(nameof(Name));
                    }
                }
            }

            /// <summary>
            /// Gets a value that indicates if this instance represents
            /// a system-defined country.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance represents a system-defined
            /// country; otherwise, <c>false</c>.
            /// </value>
            public bool IsSystem
            {
                get => Row.IsSystem;
            }

            /// <summary>
            /// Gets a value that indicates if this instance's country is in-use
            /// in the system.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance represents an in-use country;
            /// otherwise, <c>false</c>.
            /// </value>
            public bool IsInUse { get; }

            /// <summary>
            /// Gets a value that indicates if this instance can be deleted.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance can be deleted;
            /// otherwise, <c>false</c>.
            /// </value>
            public bool CanDelete =>
                !IsInUse && !IsSystem;

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the <see cref="CountryEntry"/>
            /// class.
            /// </summary>
            /// <param name="row">The row.</param>
            /// <param name="isInUse">
            /// Value to use for <see cref="IsInUse"/>.
            /// </param>
            public CountryEntry(ApplicationSettingsDataSet.CountryRow row, bool isInUse)
            {
                Row = row ?? throw new ArgumentNullException(nameof(row));
                IsInUse = isInUse;
            }

            public override string ValidateAll() =>
                Validate(nameof(Name));

            public override string Validate(string propertyName)
            {
                if (propertyName == nameof(Name) && string.IsNullOrEmpty(Row.Name))
                {
                    return "Name is required.";
                }

                return string.Empty;
            }

            #endregion
        }

        #endregion
    }
}
