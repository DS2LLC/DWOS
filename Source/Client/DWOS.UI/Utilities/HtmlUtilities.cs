using AngleSharp.Parser.Html;

namespace DWOS.UI.Utilities
{
    public static class HtmlUtilities
    {
        #region Methods

        public static string CreateHtmlDocument(string body) =>
            $"<!DOCTYPE HTML><html><body>{body}</body></html>";

        public static string ExtractHtmlBody(string htmlDocument)
        {
            var parser = new HtmlParser();
            var document = parser.Parse(htmlDocument);
            return document.Body.InnerHtml;
        }

        #endregion
    }
}
