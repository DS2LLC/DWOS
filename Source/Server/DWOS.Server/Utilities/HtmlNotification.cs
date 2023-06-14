using DWOS.Data;
using System.Linq;

namespace DWOS.Server.Utilities
{
    internal abstract class HtmlNotification
    {
        #region Fields

        public static string TAG_LOGO = "%LOGO%";
        
        public static string TAG_COMPANY_NAME = "%COMPANY%";
        public static string TAG_COMPANY_ADDRESS1 = "%COMPANY_ADDRESS%";
        public static string TAG_COMPANY_CITY = "%COMPANY_CITY%";
        public static string TAG_COMPANY_STATE = "%COMPANY_STATE%";
        public static string TAG_COMPANY_ZIP = "%COMPANY_ZIP%";
        public static string TAG_COMPANY_PHONE = "%COMPANY_PHONE%";

        public static string TAG_COMPANY_FAX = "%COMPANY_FAX%";
        public static string TAG_COMPANY_EMAIL = "%COMPANY_EMAIL%";
        public static string TAG_PORTAL_URL = "%PORTAL_URL%";
        public static string TAG_COMPANY_URL = "%COMPANY_URL%";


        #endregion

        #region Properties

        public string HTMLOutput { get; set; }

        #endregion

        #region Methods

        protected abstract void CreateNotification();

        public void ReplaceTokens(params Token[] tokens)
        {
            if (HTMLOutput == null)
                CreateNotification();

            if (HTMLOutput != null)
            {
                tokens.ToList().ForEach(t => this.HTMLOutput = this.HTMLOutput.Replace(t.TokenName, t.Value));
            }
        }

        public void ReplaceSystemTokens()
        {
            ReplaceTokens(new Token(TAG_COMPANY_NAME, ApplicationSettings.Current.CompanyName)
                , new Token(TAG_COMPANY_ADDRESS1, ApplicationSettings.Current.CompanyAddress1)
                , new Token(TAG_COMPANY_CITY, ApplicationSettings.Current.CompanyCity)
                , new Token(TAG_COMPANY_STATE, ApplicationSettings.Current.CompanyState)
                , new Token(TAG_COMPANY_ZIP, ApplicationSettings.Current.CompanyZip)
                , new Token(TAG_COMPANY_PHONE, ApplicationSettings.Current.CompanyPhone)

                , new Token(TAG_COMPANY_FAX, ApplicationSettings.Current.CompanyFax)
                , new Token(TAG_COMPANY_EMAIL, ApplicationSettings.Current.EmailFromAddress)
                , new Token(TAG_PORTAL_URL, ApplicationSettings.Current.PortalUrl)
                , new Token(TAG_COMPANY_URL, ApplicationSettings.Current.CompanyUrl));


        }

        #endregion

        #region Token
        
        public class Token
        {
            public string TokenName { get; set; }
            public string Value { get; set; }
           
            public Token(string tokenName, string value)
            {
                this.TokenName = tokenName;
                this.Value = value;
            }
        }

        #endregion
    }
}
