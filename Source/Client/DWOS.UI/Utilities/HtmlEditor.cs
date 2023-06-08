using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Forms;
using DWOS.Data;
using NLog;

namespace DWOS.UI.Utilities
{
    public partial class HtmlEditor : UserControl
    {
        #region Fields

        public const string TAG_LOGO = "%LOGO%";
        public const string TAG_COMPANY_NAME = "%COMPANY%";
        public const string TAG_COMPANY_ADDRESS1 = "%COMPANY_ADDRESS%";
        public const string TAG_COMPANY_CITY = "%COMPANY_CITY%";
        public const string TAG_COMPANY_STATE = "%COMPANY_STATE%";
        public const string TAG_COMPANY_ZIP = "%COMPANY_ZIP%";
        public const string TAG_COMPANY_PHONE = "%COMPANY_PHONE%";
        public const string TAG_COMPANY_FAX = "%COMPANY_FAX%";
        public const string TAG_COMPANY_EMAIL = "%COMPANY_EMAIL%";
        public const string TAG_PORTAL_URL = "%PORTAL_URL%";
        public const string TAG_COMPANY_URL = "%COMPANY_URL%";

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private IRenderSettings _renderSettings = new DefaultRenderSettings();

        #endregion

        #region Properties

        public IRenderSettings RenderSettings
        {
            get
            {
                return _renderSettings;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _renderSettings = value;
            }
        }

        public List<Token> Tokens { get; set; } =
            new List<Token>();

        #endregion

        #region Methods

        public HtmlEditor() { InitializeComponent(); }

        public void LoadHtml(string html)
        {
            try
            {
                txtTemplate.ValueChanged -= txtTemplate_ValueChanged;
                this.txtTemplate.Text = html;
                UpdateHtmlView();
            }
            finally
            {
                txtTemplate.ValueChanged += txtTemplate_ValueChanged;
            }
        }

        public string GetHtml() { return this.txtTemplate.Text; }

        private void UpdateHtmlView()
        {
            try
            {
                string html;

                if (String.IsNullOrWhiteSpace(this.txtTemplate.Text))
                {
                    html = "NONE";
                }
                else
                {
                    html = this.txtTemplate.Text
                        .Replace(TAG_LOGO, RenderSettings.CompanyLogoImagePath)
                        .Replace(TAG_COMPANY_NAME, RenderSettings.CompanyName)
                        .Replace(TAG_COMPANY_ADDRESS1, RenderSettings.CompanyAddress1)
                        .Replace(TAG_COMPANY_CITY, RenderSettings.CompanyCity)
                        .Replace(TAG_COMPANY_STATE, RenderSettings.CompanyState)
                        .Replace(TAG_COMPANY_ZIP, RenderSettings.CompanyZip)
                        .Replace(TAG_COMPANY_PHONE, RenderSettings.CompanyPhone)
                        .Replace(TAG_COMPANY_FAX, RenderSettings.CompanyFax)
                        .Replace(TAG_COMPANY_EMAIL, RenderSettings.EmailFromAddress)
                        .Replace(TAG_PORTAL_URL, RenderSettings.PortalUrl)
                        .Replace(TAG_COMPANY_URL, RenderSettings.CompanyUrl);

                    if (Tokens != null)
                    {
                        foreach (var token in Tokens)
                        {
                            html = token.Apply(html);
                        }
                    }
                }

                this.htmlPanel.Text = html;
            }
            catch(Exception exc)
            {
                _log.Info(exc, "Error rendering html.");
            }
        }

        #endregion

        #region Events

        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateHtmlView();
            this.timer.Stop();
        }

        private void txtTemplate_ValueChanged(object sender, EventArgs e)
        {
            this.timer.Start();
        }

        private void HtmlEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                UpdateHtmlView();
            }
        }

        #endregion

        #region IRenderSettings

        public interface IRenderSettings
        {
            string CompanyLogoImagePath
            {
               get;
            }

            string CompanyName
            {
               get;
            }

            string CompanyAddress1
            {
               get;
            }

            string CompanyCity
            {
               get;
            }

            string CompanyState
            {
               get;
            }

            string CompanyZip
            {
               get;
            }

            string CompanyPhone
            {
               get;
            }

            string CompanyFax
            {
               get;
            }

            string EmailFromAddress
            {
               get;
            }

            string PortalUrl
            {
               get;
            }

            string CompanyUrl
            {
               get;
            }
        }

        #endregion

        #region DefaultRenderSettings

        public class DefaultRenderSettings : IRenderSettings
        {
            #region IRenderSettings Members

            public virtual string CompanyAddress1
            {
                get
                {
                    return ApplicationSettings.Current.CompanyAddress1;
                }
            }

            public virtual string CompanyCity
            {
                get
                {
                    return ApplicationSettings.Current.CompanyCity;
                }
            }

            public virtual string CompanyFax
            {
                get
                {
                    return ApplicationSettings.Current.CompanyFax;
                }
            }

            public virtual string CompanyLogoImagePath
            {
                get
                {
                    return ApplicationSettings.Current.CompanyLogoImagePath;
                }
            }

            public virtual string CompanyName
            {
                get
                {
                    return ApplicationSettings.Current.CompanyName;
                }
            }

            public virtual string CompanyPhone
            {
                get
                {
                    return ApplicationSettings.Current.CompanyPhone;
                }
            }

            public virtual string CompanyState
            {
                get
                {
                    return ApplicationSettings.Current.CompanyState;
                }
            }

            public virtual string CompanyUrl
            {
                get
                {
                    return ApplicationSettings.Current.CompanyUrl;
                }
            }

            public virtual string CompanyZip
            {
                get
                {
                    return ApplicationSettings.Current.CompanyZip;
                }
            }

            public virtual string EmailFromAddress
            {
                get
                {
                    return ApplicationSettings.Current.EmailFromAddress;
                }
            }

            public virtual string PortalUrl
            {
                get
                {
                    return ApplicationSettings.Current.PortalUrl;
                }
            }

            #endregion
        }

        #endregion

        #region Token

        [Serializable]
        public class Token : ISerializable
        {
            public string Tag {  get; set;}

            public string Value { get; set; }

            public string Apply(string input)
            {
                return input.Replace(Tag, Value);
            }

            #region Methods

            public Token()
            {

            }

            public Token(SerializationInfo info, StreamingContext context)
            {
                Tag = info.GetString(nameof(Tag));
                Value = info.GetString(nameof(Value));
            }
            #endregion

            #region ISerializable Implementation

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(Tag), Tag);
                info.AddValue(nameof(Value), Value);
            }

            #endregion
        }

        #endregion
    }
}