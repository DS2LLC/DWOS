using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace DWOS.AutomatedWorkOrderTool.Dialogs
{
    /// <summary>
    /// Interaction logic for AddOspFormatSectionDialog.xaml
    /// </summary>
    public partial class AddOspFormatSectionDialog
    {
        #region Properties

        public OspFormatViewModel CurrentFormat { get; set; }

        public List<OspFormatSectionViewModel> CurrentSections { get; set; }

        #endregion

        #region Methods

        public AddOspFormatSectionDialog()
        {
            InitializeComponent();
        }

        public AddOspFormatSectionDialog(MetroDialogSettings settings)
            : base(settings)
        {
            InitializeComponent();
        }

        public AddOspFormatSectionDialog(MetroWindow parentWindow)
            :base(parentWindow)
        {
            InitializeComponent();
        }

        public AddOspFormatSectionDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            :base(parentWindow, settings)
        {
            InitializeComponent();
        }


        internal Task<OspFormatSectionViewModel> WaitForResult()
        {
            var tcs = new TaskCompletionSource<OspFormatSectionViewModel>();
            Action cleanup = null;

            // Setup event handlers
            void OkHandler(object sender, RoutedEventArgs args)
            {
                cleanup?.Invoke();
                tcs.TrySetResult(GetFormatSection());
            }

            OkButton.Click += OkHandler;

            void CancelHandler(object sender, RoutedEventArgs args)
            {
                cleanup?.Invoke();
                tcs.TrySetResult(null);
            }

            CancelButton.Click += CancelHandler;

            void CancelKeyHandler(object sender, KeyEventArgs args)
            {
                if (args.Key == Key.Escape)
                {
                    cleanup?.Invoke();
                    tcs.TrySetResult(null);
                }
            }

            KeyDown += CancelKeyHandler;

            // Register cancellation handler
            var cancelRegistration = DialogSettings.CancellationToken.Register(() =>
            {
                cleanup?.Invoke();
                tcs.TrySetResult(null);
            });

            cleanup = () =>
            {
                // Clean up event handlers
                OkButton.Click -= OkHandler;
                CancelButton.Click -= CancelHandler;
                KeyDown -= CancelKeyHandler;

                // Dispose of the cancellation callback because it's not needed
                cancelRegistration.Dispose();
            };

            return tcs.Task;
        }


        private OspFormatSectionViewModel GetFormatSection()
        {
            var vm = InnerControl.DataContext as AddOspFormatSectionViewModel;
            return vm?.CreateFormatSection();
        }

        #endregion

        #region Events

        private void AddOspFormatSectionDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = InnerControl.DataContext as AddOspFormatSectionViewModel;
            vm?.LoadData(CurrentFormat.OspFormatId, CurrentSections);
        }

        #endregion
    }
}
