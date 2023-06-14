using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace DWOS.AutomatedWorkOrderTool.Dialogs
{
    /// <summary>
    /// Interaction logic for AddOspFormatCodeMapDialog.xaml
    /// </summary>
    public partial class AddOspFormatCodeMapDialog
    {
        #region Methods

        public AddOspFormatCodeMapDialog()
        {
            InitializeComponent();
        }

        public AddOspFormatCodeMapDialog(MetroDialogSettings settings)
            : base(settings)
        {
            InitializeComponent();
        }

        public AddOspFormatCodeMapDialog(MetroWindow parentWindow)
            :base(parentWindow)
        {
            InitializeComponent();
        }

        public AddOspFormatCodeMapDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            :base(parentWindow, settings)
        {
            InitializeComponent();
        }

        public void Load(OspFormatEditorViewModel currentFormat, List<OspFormatSectionViewModel> currentSections)
        {
            var vm = InnerControl.DataContext as AddOspFormatCodeMapViewModel;
            vm?.Load(currentFormat, currentSections);
        }

        internal Task<DialogResult> WaitForResult()
        {
            var tcs = new TaskCompletionSource<DialogResult>();
            Action cleanup = null;

            // Setup event handlers
            void OkHandler(object sender, RoutedEventArgs args)
            {
                cleanup?.Invoke();
                tcs.TrySetResult(GetDialogResult());
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

        private DialogResult GetDialogResult()
        {
            var vm = InnerControl.DataContext as AddOspFormatCodeMapViewModel;

            if (vm?.SelectedSection == null)
            {
                return null;
            }

            if (vm.SelectedSection.Role == RoleType.Process)
            {
                return new DialogResult
                {
                    CodeMapType = RoleType.Process,
                    Process = new OspProcessViewModel
                    {
                        OspFormatSectionId = vm.SelectedSection.OspFormatSectionId ?? -1,
                        Code = vm.OspCode,
                        ProcessId = vm.SelectedProcess?.ProcessId ?? -1,
                        ProcessAliasId = vm.SelectedProcessAlias?.ProcessAliasId ?? -1
                    }
                };
            }

            if (vm.SelectedSection.Role == RoleType.PartMark)
            {
                return new DialogResult
                {
                    CodeMapType = RoleType.PartMark,
                    PartMark = new OspPartMarkViewModel
                    {
                        OspFormatSectionId = vm.SelectedSection.OspFormatSectionId ?? -1,
                        Code = vm.OspCode,
                        Def1 = vm.Def1,
                        Def2 = vm.Def2,
                        Def3 = vm.Def3,
                        Def4 = vm.Def4,
                        ProcessSpec = vm.ProcessSpec
                    }
                };
            }

            return null;
        }

        #endregion

        #region DialogResult

        public class DialogResult
        {
            public RoleType CodeMapType { get; set; }

            public OspProcessViewModel Process { get; set; }

            public OspPartMarkViewModel PartMark { get; set; }
        }

        #endregion
    }
}
