using NLog;
using System;
using System.Windows;
using DWOS.UI.Admin.ViewModels;
using DWOS.Shared.Utilities;
using System.Linq;
using Infragistics.Windows.DataPresenter;
using DWOS.UI.Utilities;

namespace DWOS.UI.Admin
{
    /// <summary>
    /// Interaction logic for CountryEditor.xaml
    /// </summary>
    public partial class CountryManager
    {
        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="CountryManager"/> class.
        /// </summary>
        public CountryManager()
        {
            InitializeComponent();
            Icon = Properties.Resources.Globe_128.ToWpfImage();
        }

        #endregion

        #region Events

        private void ViewModel_Accepted(object sender, EventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, $"Error accepting {nameof(CountryManager)}.");
            }
        }

        private void ViewModel_CountryAdded(object sender, EventArgsTemplate<CountryManagerViewModel.CountryEntry> e)
        {
            try
            {
                var country = e?.Item;
                if (country == null)
                {
                    return;
                }

                // Select the Name field for the new country.
                var lastRecord = ReasonGrid.Records.OfType<DataRecord>().LastOrDefault();

                if (lastRecord != null)
                {
                    ReasonGrid.ActiveCell = lastRecord.Cells[0];
                    ReasonGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error adding country.");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.LoadData();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, $"Error loading {nameof(CountryManager)}.");
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Unload();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, $"Error unloading {nameof(CountryManager)}.");
            }
        }

        #endregion
    }
}
