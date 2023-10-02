using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using DWOS.Shared;
using NLog;

namespace DWOS.UI.Utilities
{
    public partial class UpdateInformation : Form
    {
        #region Fields
        
        private WebClient _client                       = null;
        private List <VersionUpdateInfo> _versionInfos  = new List <VersionUpdateInfo>();
        private Version _currentVersion                 = new Version(About.ApplicationVersion);
        
        #endregion

        #region Methods
        
        public UpdateInformation()
        {
            InitializeComponent();
        }

        private void LoadInfo()
        {
            this.Cursor             = Cursors.WaitCursor;
            lblCurrentVersion.Text  = _currentVersion.ToString();
            lblLatestVersion.Text   = _currentVersion.ToString();
            btnUpload.Visible       = false; //no update by default.

            _client = new WebClient();
            _client.DownloadStringCompleted += web_DownloadStringCompleted;
            _client.DownloadStringAsync(new Uri(Updater.UPDATE_VERSION_URL.FormatWith(_currentVersion)));
        }
        
        private void ParsePayload(string versionData)
        {
            var lines = versionData.Split(Environment.NewLine);
            VersionUpdateInfo currentInfo = null;

            foreach (var line in lines)
            {
                if (!String.IsNullOrEmpty(line))
                {
                    if (line.StartsWith(";"))
                        continue;
                    if(currentInfo == null)
                    {
                        currentInfo = new VersionUpdateInfo();
                        _versionInfos.Add(currentInfo);
                    }

                    currentInfo.ParseLine(line);
                }
                else
                    currentInfo = null;
            }
        }

        private void WriteVersionInfo()
        {
            txtNotes.Value = null;

            if(_versionInfos.Count > 0)
            {
                _versionInfos       = _versionInfos.OrderByDescending(vi => vi.UpdateVersion).Where(vi => vi.UpdateVersion >= _currentVersion).ToList();
                var latestVersion   = _versionInfos.FirstOrDefault();

                if(latestVersion != null)
                {
                    lblLatestVersion.Text   = latestVersion.UpdateVersion.ToString();
                    btnUpload.Visible       = latestVersion.UpdateVersion > _currentVersion;

                    //if we are current
                    if (latestVersion.UpdateVersion == _currentVersion)
                        txtNotes.Value = "<font face=\"Verdana\" size=\"10pt\" color=\"Red\" style=\"font-weight:bold;\"> DWOS is up to date!</font><br/>--------------------<br/>";

                    foreach(var versionUpdateInfo in _versionInfos)
                    {
                        txtNotes.Value += versionUpdateInfo.Print() + Environment.NewLine;
                    }
                }
            }

            //print out must be current(?)
            if(txtNotes.Value == null || String.IsNullOrWhiteSpace(txtNotes.Value.ToString()))
            {
                txtNotes.Value = "<font face=\"Verdana\" size=\"10pt\" color=\"Black\" style=\"font-weight:bold;\"> DWOS is up to date!</font><br/>";
                lblLatestVersion.Text = _currentVersion.ToString();
            }
        }

        private void UpdateInformation_Load(object sender, EventArgs e)
        {
            LoadInfo();
        }

        #endregion

        #region Events
        
        private void ultraButton1_Click(object sender, EventArgs e)
        {
            Updater.CheckUpdates(About.ApplicationVersion, false);
        }

        private void web_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            this.Cursor = Cursors.Default;

            if (e.Cancelled || e.Error != null)
                txtNotes.Text = "Unknown";
            else
            {
                ParsePayload(e.Result);
                WriteVersionInfo();
            }
        }

        #endregion

        #region VersionUpdateInfo

        private class VersionUpdateInfo
        {
            public Version UpdateVersion { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public List<string> Features { get; set; }

            public List<string> Enhancement { get; set; }

            public List<string> BugFix { get; set; }

            public VersionUpdateInfo()
            {
                this.Features       = new List <string>();
                this.Enhancement    = new List <string>();
                this.BugFix         = new List <string>();
            }

            public void ParseLine(string line)
            {
                if (line.StartsWith("[")) //[DWOS_13.2.0.4]
                {
                    
                }
                else if (line.Contains("="))
                {
                    var keyValue = line.Split("=");

                    if(keyValue.Length == 2)
                    {
                        var key = keyValue[0].ToLower().Trim();
                        var value = keyValue[1].Trim();

                        switch(key)
                        {
                            case "productversion":
                                UpdateVersion = new Version(value);
                                break;
                            case "name":
                                Name = value;
                                break;
                            case "description":
                                Description = value;
                                break;
                            default:
                                if(key.StartsWith("feature"))
                                    Features.Add(value);
                                else if(key.StartsWith("bugfix"))
                                    BugFix.Add(value);
                                else if (key.StartsWith("enhancement"))
                                    Enhancement.Add(value);
                                break;
                        }
                    }
                }
            }

            public string Print()
            {
                var output = new StringBuilder();
                output.Append("<font face=\"Verdana\" size=\"10pt\" color=\"Black\" style=\"font-weight:bold;\">");
                output.Append("Version: " + this.UpdateVersion.ToString());
                output.Append("</font><br/>");

                output.Append("<font face=\"Verdana\" size=\"9pt\" color=\"Black\">");
               
                if(this.Features.Count > 0 || this.Enhancement.Count > 0 || this.BugFix.Count > 0)
                {
                    if(this.Features.Count > 0)
                    {
                        output.Append("<br/>");
                        output.Append("&nbsp;Features<br/>");
                        this.Features.ForEach(f => output.Append("&nbsp;&nbsp;- " + ValidateText(f) + "<br/>"));
                    }

                    if(this.Enhancement.Count > 0)
                    {
                        output.Append("<br/>");
                        output.Append("&nbsp;Enhancements<br/>");
                        this.Enhancement.ForEach(f => output.Append("&nbsp;&nbsp;- " + ValidateText(f) + "<br/>"));
                    }

                    if(this.BugFix.Count > 0)
                    {
                        output.Append("<br/>");
                        output.Append("&nbsp;Bug Fixes<br/>");
                        this.BugFix.ForEach(f => output.Append("&nbsp;&nbsp;- " + ValidateText(f) + "<br/>"));
                    }
                }
                else
                    output.Append("NA<br/>");

                output.Append("</font><br/>");

                return output.ToString();
            }

            private string ValidateText(string text)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    text = text.Replace(">", "-");
                    text = text.Replace("<", "-");
                    text = text.Replace("&", "&amp;");
                }

                return text;
            }
        }

        #endregion
    }

    public static class Updater
    {
        public const string UPDATE_VERSION_URL = "";

        public static string UpdaterPath
        {
            get
            {
                var updaterModulePath = Path.Combine(Application.StartupPath, "updater.exe");

                //if not in current folder then look up one folder
                if (!File.Exists(updaterModulePath))
                    updaterModulePath = Path.Combine(Directory.GetParent(Application.StartupPath).FullName, "updater.exe");

                return updaterModulePath;
            }
        }

        /// <summary>
        /// Checks the updates using the advanced installer update checker.
        /// </summary>
        /// <param name="requiredVersion">The required version, only really uses the Major.Minor.Build in the url whne it checks.</param>
        /// <param name="isSilent">if set to <c>true</c> [is silent].</param>
        public static void CheckUpdates(string requiredVersion, bool isSilent)
        {
            try
            {
               var updaterPath = UpdaterPath;

               if (File.Exists(updaterPath))
                {
                    try
                    {
                        if (isSilent)
                            Process.Start(updaterPath, "/silent -nofreqcheck -url " + UPDATE_VERSION_URL.FormatWith(requiredVersion));
                        else
                            Process.Start(updaterPath, "/checknow -url " + UPDATE_VERSION_URL.FormatWith(requiredVersion));
                    }
                    catch (Exception exc)
                    {
                        LogManager.GetCurrentClassLogger().Debug(exc, "Error starting process for: " + updaterPath);
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error checking for updates.");
            }
        }
    }
}
