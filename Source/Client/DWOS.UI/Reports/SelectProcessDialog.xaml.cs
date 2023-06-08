using DWOS.UI.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DWOS.UI.Reports
{
    /// <summary>
    /// Interaction logic for SelectProcessDialog.xaml
    /// </summary>
    public partial class SelectProcessDialog
    {
        #region Fields

        public static readonly DependencyProperty ProcessesProperty = DependencyProperty.Register(
            nameof(Processes), typeof(IEnumerable<ProcessAnswerContext.Process>), typeof(SelectProcessDialog));

        public static readonly DependencyProperty SelectedProcessProperty = DependencyProperty.Register(
            nameof(SelectedProcess), typeof(ProcessAnswerContext.Process), typeof(SelectProcessDialog));

        #endregion

        #region Properties

        public IEnumerable<ProcessAnswerContext.Process> Processes
        {
            get => GetValue(ProcessesProperty) as IEnumerable<ProcessAnswerContext.Process>;
            set => SetValue(ProcessesProperty, value);
        }

        public ProcessAnswerContext.Process SelectedProcess
        {
            get => GetValue(SelectedProcessProperty) as ProcessAnswerContext.Process;
            set => SetValue(SelectedProcessProperty, value);
        }

        #endregion

        #region Methods

        public SelectProcessDialog()
        {
            InitializeComponent();
            Icon = Properties.Resources.Report32.ToWpfImage();
        }

        #endregion

        #region Events

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling select button click event.");
            }
        }

        #endregion
    }
}
