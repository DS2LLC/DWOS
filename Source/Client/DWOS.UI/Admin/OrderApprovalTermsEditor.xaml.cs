using DWOS.UI.Utilities;
using NLog;
using System;
using System.Windows;

namespace DWOS.UI.Admin
{
    /// <summary>
    /// Interaction logic for OrderApprovalTermsEditor.xaml
    /// </summary>
    public partial class OrderApprovalTermsEditor
    {
        #region Fields

        private readonly GridSettingsPersistence<XamDataGridSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<XamDataGridSettings>("OrderApprovalTerms", new XamDataGridSettings());

        #endregion

        #region Methods

        public OrderApprovalTermsEditor()
        {
            InitializeComponent();
            Icon = Properties.Resources.Paper32.ToWpfImage();
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
                    .Error(exc, "Error handling Accepted event.");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.LoadData();

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(ApprovalTermsGrid);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling Loaded event.");

            }
        }

        #endregion

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new XamDataGridSettings();
                settings.RetrieveSettingsFrom(ApprovalTermsGrid);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling Unloaded event.");
            }
        }
    }
}
