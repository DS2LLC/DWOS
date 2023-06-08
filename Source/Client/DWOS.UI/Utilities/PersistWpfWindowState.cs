using DWOS.Shared.Utilities;
using NLog;
using System;
using System.IO;
using System.Windows;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Persists settings for a WPF window.
    /// </summary>
    /// <remarks>
    /// Initializing an instance of this class before the window's
    /// <c>InitializeComponent</c> call seems to make it so that the window
    /// initially appears at the correct size.
    /// </remarks>
    internal class PersistWpfWindowState
    {
        #region Fields

        private const string FILE_NAME_SUFFIX = "_Window.json";

        #endregion

        #region Properties

        public Window Window { get; }

        private string FileName => Path.Combine(FileSystem.UserAppDataPath(),
            $"{Window.GetType().Name}{FILE_NAME_SUFFIX}");

        #endregion

        #region Methods

        public PersistWpfWindowState(Window window)
        {
            Window = window ?? throw new ArgumentNullException(nameof(window));
            Window.Loaded += Window_Loaded;
            Window.Unloaded += Window_Unloaded;
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load settings
            try
            {
                string fileName = FileName;
                if (!File.Exists(fileName))
                {
                    return;
                }

                var fli = FileSystem.DeserializeJsonFile<WindowLocationInfo>(fileName);

                if (fli != null)
                {
                    // Position may be negative; reset to 0 when checking
                    // screen bounds.
                    var topLeftPosition = new Point(Math.Max(fli.Left, 0), Math.Max(fli.Top, 0));

                    // Only set location if it exists within the screen
                    var screen = new Rect(
                        SystemParameters.VirtualScreenLeft,
                        SystemParameters.VirtualScreenTop,
                        SystemParameters.VirtualScreenWidth,
                        SystemParameters.VirtualScreenHeight);

                    if (screen.Contains(topLeftPosition))
                    {
                        Window.Top = fli.Top;
                        Window.Left = fli.Left;
                        Window.Width = fli.Width;
                        Window.Height = fli.Height;
                    }

                    Window.WindowState = fli.WindowState;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling loaded event.");
            }
        }


        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            // Save settings
            try
            {
                var settings = new WindowLocationInfo
                {
                    Top = Window.Top,
                    Left = Window.Left,
                    Width = Window.Width,
                    Height = Window.Height,
                    WindowState = Window.WindowState
                };

                FileSystem.SerializeJson(FileName, settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling unloaded event.");
            }
        }

        #endregion
    }
}
