using System;
using System.Threading;
using System.Windows.Forms;
using DWOS.Shared;

namespace DWOS.UI
{
    /// <summary>
    /// Creates the splash screen based on the image and the About class.
    /// </summary>
    public class SplashScreen : Form
    {
        #region Fields
    
        /// <summary>
        /// The splash screen thread.
        /// </summary>
        private static Thread _thread;

        /// <summary>
        /// The main form to watch.
        /// </summary>
        private static SplashScreen _splashForm;
        private static string _initialLabelStatus;

        /// <summary>
        /// Used to control code that closes <see cref="_splashForm"/>.
        /// </summary>
        private static readonly object CloseLock = new object();

        /// <summary>
        /// Used to control code that accesses <see cref="_thread"/>.
        /// </summary>
        private static readonly object ThreadLock = new object();

        private readonly System.ComponentModel.IContainer components = null;
        private Label _lblVersion;
        private Label _lblDate;
        private Label _lblCompany;
        private Label _lblTagline;
        private Label _lblStatus;
        private PictureBox _picLogo;
        private Label _lblSubtitle;
        private Label _lblCopyright;

        #endregion

        #region Form

        public SplashScreen()
        {
            InitializeComponent();
            _lblVersion.Text = $@"Version {About.ApplicationVersionMajorMinor}";

            var releaseDate = About.ApplicationReleaseDate ?? new DateTime(2017, 1, 1);

            string releaseDateText = releaseDate.ToString("MMMM d, yyyy");

            _lblDate.Text = releaseDateText;

            _lblCopyright.Text = $@"Copyright 2006-{releaseDate:yyyy} All Rights Reserved.";

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this._lblVersion = new System.Windows.Forms.Label();
            this._lblDate = new System.Windows.Forms.Label();
            this._lblCompany = new System.Windows.Forms.Label();
            this._lblCopyright = new System.Windows.Forms.Label();
            this._lblTagline = new System.Windows.Forms.Label();
            this._lblStatus = new System.Windows.Forms.Label();
            this._picLogo = new System.Windows.Forms.PictureBox();
            this._lblSubtitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // _lblVersion
            // 
            this._lblVersion.BackColor = System.Drawing.Color.Transparent;
            this._lblVersion.Font = new System.Drawing.Font("Calibri Light", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._lblVersion.Location = new System.Drawing.Point(16, 167);
            this._lblVersion.Name = "_lblVersion";
            this._lblVersion.Size = new System.Drawing.Size(522, 39);
            this._lblVersion.TabIndex = 38;
            this._lblVersion.Text = "Version Major.Minor";
            this._lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lblDate
            // 
            this._lblDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._lblDate.BackColor = System.Drawing.Color.Transparent;
            this._lblDate.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._lblDate.Location = new System.Drawing.Point(347, 9);
            this._lblDate.Name = "_lblDate";
            this._lblDate.Size = new System.Drawing.Size(191, 23);
            this._lblDate.TabIndex = 36;
            this._lblDate.Text = "Release Date";
            this._lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _lblCompany
            // 
            this._lblCompany.AutoSize = true;
            this._lblCompany.BackColor = System.Drawing.Color.Transparent;
            this._lblCompany.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblCompany.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._lblCompany.Location = new System.Drawing.Point(12, 9);
            this._lblCompany.Name = "_lblCompany";
            this._lblCompany.Size = new System.Drawing.Size(227, 23);
            this._lblCompany.TabIndex = 35;
            this._lblCompany.Text = "Dynamic Software Solutions";
            this._lblCompany.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _lblCopyright
            // 
            this._lblCopyright.AutoSize = true;
            this._lblCopyright.BackColor = System.Drawing.Color.Transparent;
            this._lblCopyright.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblCopyright.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._lblCopyright.Location = new System.Drawing.Point(152, 270);
            this._lblCopyright.Name = "_lblCopyright";
            this._lblCopyright.Size = new System.Drawing.Size(233, 15);
            this._lblCopyright.TabIndex = 34;
            this._lblCopyright.Text = "Copyright 2006-2016 All Rights Reserved.";
            this._lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _lblTagline
            // 
            this._lblTagline.AutoSize = true;
            this._lblTagline.BackColor = System.Drawing.Color.Transparent;
            this._lblTagline.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblTagline.ForeColor = System.Drawing.Color.DimGray;
            this._lblTagline.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this._lblTagline.Location = new System.Drawing.Point(86, 240);
            this._lblTagline.Name = "_lblTagline";
            this._lblTagline.Size = new System.Drawing.Size(373, 23);
            this._lblTagline.TabIndex = 39;
            this._lblTagline.Text = "The complete work order management system.";
            // 
            // _lblStatus
            // 
            this._lblStatus.BackColor = System.Drawing.Color.Transparent;
            this._lblStatus.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._lblStatus.Location = new System.Drawing.Point(138, 213);
            this._lblStatus.Name = "_lblStatus";
            this._lblStatus.Size = new System.Drawing.Size(269, 18);
            this._lblStatus.TabIndex = 41;
            this._lblStatus.Text = "Loading...";
            this._lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _picLogo
            // 
            this._picLogo.BackColor = System.Drawing.Color.Transparent;
            this._picLogo.Image = global::DWOS.UI.Properties.Resources.DWOS_logo_monotone;
            this._picLogo.Location = new System.Drawing.Point(94, 65);
            this._picLogo.Name = "_picLogo";
            this._picLogo.Size = new System.Drawing.Size(356, 75);
            this._picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._picLogo.TabIndex = 42;
            this._picLogo.TabStop = false;
            // 
            // _lblSubtitle
            // 
            this._lblSubtitle.AutoSize = true;
            this._lblSubtitle.BackColor = System.Drawing.Color.Transparent;
            this._lblSubtitle.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblSubtitle.ForeColor = System.Drawing.Color.DimGray;
            this._lblSubtitle.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this._lblSubtitle.Location = new System.Drawing.Point(156, 142);
            this._lblSubtitle.Name = "_lblSubtitle";
            this._lblSubtitle.Size = new System.Drawing.Size(232, 23);
            this._lblSubtitle.TabIndex = 43;
            this._lblSubtitle.Text = "Dynamic Work Order System";
            // 
            // SplashScreen
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(7, 16);
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(550, 294);
            this.Controls.Add(this._lblSubtitle);
            this.Controls.Add(this._picLogo);
            this.Controls.Add(this._lblStatus);
            this.Controls.Add(this._lblTagline);
            this.Controls.Add(this._lblVersion);
            this.Controls.Add(this._lblDate);
            this.Controls.Add(this._lblCompany);
            this.Controls.Add(this._lblCopyright);
            this.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreen";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmSplash_Load);
            ((System.ComponentModel.ISupportInitialize)(this._picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #endregion

        #region Methods

        public static void Start()
        {
            lock (ThreadLock)
            {
                _thread = new Thread(StartUp) { Name = "SplashScreen Thread" };
                _thread.SetApartmentState(ApartmentState.STA);
                _thread.Priority = ThreadPriority.Highest;
                _thread.Start();
            }
        }

        private static void StartUp()
        {
            try
            {
                _splashForm = new SplashScreen();

                lock (ThreadLock)
                {
                    if (_thread != null)
                    {
                        _thread.Priority = ThreadPriority.Normal;
                    }
                }
                _splashForm.ShowDialog();
            }
            catch(Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error showing splash screen.");
            }
            finally
            {
                // Close _splashForm unless something else closed it first.
                lock(CloseLock)
                {
                    if(_splashForm != null && !_splashForm.IsDisposed)
                    {
                        _splashForm.Close();
                    }

                    _splashForm = null;
                }
            }
        }

        public static void Stop()
        {
            lock (CloseLock)
            {
                try
                {
                    if (_splashForm != null && !_splashForm.IsDisposed)
                    {
                        _splashForm?.Invoke(new MethodInvoker(_splashForm.Close));
                    }
                }
                catch (Exception exc)
                {
                    NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error stopping splash screen.");
                }
                finally
                {
                    _splashForm = null;
                }
            }

            lock (ThreadLock)
            {
                _thread = null;
            }
        }

        public static void UpdateStatusText(string status)
        {
            if (_splashForm == null || _splashForm.IsDisposed)
            {
                _initialLabelStatus = status;
            }
            else if (_splashForm.InvokeRequired)
            {
                _splashForm.BeginInvoke(new Action<string>(UpdateStatusText), status);
            }
            else
            {
                _splashForm._lblStatus.Text = status;
                _splashForm._lblStatus.Refresh();
                Application.DoEvents();
            }
        }

        #endregion

        #region Events

        private void frmSplash_Load(object sender, EventArgs e)
        {
            _lblStatus.Text = _initialLabelStatus;
            Application.DoEvents();
        }

        #endregion
    }
}