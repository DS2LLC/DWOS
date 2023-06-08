using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using NLog;

namespace DWOS.UI.Utilities
{
    /// <summary>
    ///     PersistWindowState allows a form to automatically save and restore its state. Settings are saved in the
    ///     registry under a default location or one set by the developer. Parent Form is required.
    /// </summary>
    [DefaultProperty("ParentForm")]
    public class PersistWindowState : Component
    {
        #region Fields

        private const string FILE_NAME = "_FromLocation.xml";
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        
        private int _normalHeight;
        private int _normalLeft;
        private int _normalTop;
        private int _normalWidth;
        private Form _parentForm;

        private FormWindowState _windowState;

        private Container components;

        #endregion

        #region Properties

        [Browsable(true)]
        [Description("Set the form whose properties are to be persisted.")]
        [Category("General")]
        public Form ParentForm
        {
            set
            {
                this._parentForm = value;

                if(this._parentForm != null && !DesignMode)
                {
                    // subscribe to parent form's events
                    this._parentForm.Closing += OnClosing;
                    this._parentForm.Resize += OnResize;
                    this._parentForm.Move += OnMove;
                    this._parentForm.Load += OnLoad;

                    // get initial width and height in case form is never resized
                    this._normalWidth = this._parentForm.Width;
                    this._normalHeight = this._parentForm.Height;
                }
            }
            get { return this._parentForm; }
        }

        [Browsable(true), Description("Get/Set the split container to persist panel sizes."), Category("General")]
        public SplitContainer Splitter { get; set; }

        [Browsable(true), Description("Allow the state to be saved if minimized."), DefaultValue(false), Category("General")]
        public bool AllowSaveMinimized { get; set; }

        /// <summary>
        ///     Gets or sets the file name prefix used to save and load file. If not set then useses the ParentForm name.
        /// </summary>
        /// <value> The file name prefix. </value>
        public string FileNamePrefix { get; set; }

        #endregion

        #region Windows

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(this._parentForm != null)
                {
                    this._parentForm.Closing -= OnClosing;
                    this._parentForm.Resize -= OnResize;
                    this._parentForm.Move -= OnMove;
                    this._parentForm.Load -= OnLoad;
                    this._parentForm = null;
                }

                if(this.components != null)
                    this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() { components = new System.ComponentModel.Container(); }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///     Constructor
        /// </summary>
        public PersistWindowState()
        {
            InitializeComponent();
        }

        public PersistWindowState(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        [DebuggerHidden]
        private FormLocationInfo LoadFormLocation()
        {
            try
            {
                string path = FileSystem.UserAppDataPath();
                string fileName = Path.Combine(path, (FileNamePrefix ?? ParentForm.Name) + FILE_NAME);

                if(File.Exists(fileName))
                {
                    object o = new FormLocationInfo();
                    FileSystem.DeserializeObjectXml(fileName, ref o);
                    return (FormLocationInfo) o;
                }

                return null;
            }
            catch(Exception exc)
            {
                _log.Debug(exc, "Error getting the existing form location.");
                return null;
            }
        }

        [DebuggerHidden]
        public void SaveFormLocation()
        {
            try
            {
                if(this.DesignMode)
                    return;

                var path     = FileSystem.UserAppDataPath();
                var fileName = Path.Combine(path, (FileNamePrefix ?? ParentForm.Name) + FILE_NAME);

                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var fli              = new FormLocationInfo();
                fli.Height           = this._normalHeight;
                fli.Width            = this._normalWidth;
                fli.Left             = this._normalLeft;
                fli.Top              = this._normalTop;
                fli.SplitterDistance = this.Splitter == null ? -1 : this.Splitter.SplitterDistance; 
                fli.WindowState      = (int) this._windowState;

                FileSystem.SerializeObjectXml(fileName, fli);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error saving the forms location");
            }
        }

        #endregion

        #region Events

        /// <summary>
        ///     OnResize
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void OnResize(object sender, EventArgs e)
        {
            try
            {
                // save width and height
                if(this._parentForm != null && this._parentForm.WindowState == FormWindowState.Normal)
                {
                    this._normalWidth = this._parentForm.Width;
                    this._normalHeight = this._parentForm.Height;
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error OnResize");
            }
        }

        /// <summary>
        ///     OnMove
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void OnMove(object sender, EventArgs e)
        {
            try
            {
                // save position
                if(this._parentForm != null && this._parentForm.WindowState == FormWindowState.Normal)
                {
                    this._normalLeft = this._parentForm.Left;
                    this._normalTop = this._parentForm.Top;
                }

                // save state
                this._windowState = this._parentForm.WindowState;
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error OnMove");
            }
        }

        /// <summary>
        ///     OnClosing
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void OnClosing(object sender, CancelEventArgs e)
        {
            try
            {
                if(!DesignMode)
                {
                    // check if we are allowed to save the state as minimized (not normally)
                    if(!this.AllowSaveMinimized)
                    {
                        if(this._windowState == FormWindowState.Minimized)
                            this._windowState = FormWindowState.Normal;
                    }

                    if(this._parentForm != null)
                        SaveFormLocation();
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error on form closing.");
            }
        }

        /// <summary>
        ///     OnLoad
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        //[DebuggerHidden]
        private void OnLoad(object sender, EventArgs e)
        {
            try
            {
                // attempt to read state from registry
                if (DesignMode)
                {
                    return;
                }

                var fli = LoadFormLocation();

                if (fli != null)
                {
                    var windowState = (FormWindowState) fli.WindowState;

                    // Position may be negative; reset to 0 when checking
                    // screen bounds.
                    var topLeftPosition = new Point(Math.Max(fli.Left, 0), Math.Max(fli.Top, 0));

                    // Only set location if it exists within the current monitor configuration.
                    if (Screen.AllScreens.Any(s => s.Bounds.Contains(topLeftPosition)))
                    {
                        _parentForm.Location = new Point(fli.Left, fli.Top);
                        _parentForm.Size = new Size(fli.Width, fli.Height);
                    }

                    _parentForm.WindowState = windowState;

                    if(this.Splitter != null)
                    {
                        if(fli.SplitterDistance > 0)
                            this.Splitter.SplitterDistance = fli.SplitterDistance;
                    }
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error on form loading.");
            }
        }

        #endregion
    }

    [Serializable]
    public class FormLocationInfo
    {
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Left { get; set; }
        public int SplitterDistance { get; set; }
        public int WindowState { get; set; }
    }
}