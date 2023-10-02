using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace DWOS.Shared.Utilities
{
    public static class Gravatar
    {
        #region Fields

        public const int IMAGE_SIZE = 80;

        #endregion

        /// <summary>
        /// Creates a Gravatar URI.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="rating"></param>
        /// <param name="missingImage"></param>
        /// <returns></returns>
        public static Uri GetGravatarUri(string emailAddress, Rating rating = Rating.g, MissingImage missingImage = MissingImage.identicon)
        {
            // Reference: http://en.gravatar.com/site/implement/url
            //      http://en.gravatar.com/site/implement/images/
            var sb = new StringBuilder();

            sb.Append("http://www.gravatar.com/avatar/");
            sb.Append(Md5EncodeText(emailAddress ?? string.Empty));
            sb.Append(".jpg");

            // Size
            sb.Append("?s=");
            sb.Append(IMAGE_SIZE);

            // Rating
            if (rating != Rating.g)
            {
                sb.Append("&r=");
                sb.Append(rating);
            }

            // Default and special modes
            sb.Append("&d=");
            sb.Append(missingImage);

            return new Uri(sb.ToString());
        }

        /// <summary>
        /// Starts asynchronously retrieving a Gravatar image.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="onComplete">Method to call when retrieval is complete</param>
        /// <param name="rating"></param>
        /// <param name="missingImage"></param>
        public static void BeginGetImage(string emailAddress, Action<Bitmap> onComplete, Rating rating = Rating.g, MissingImage missingImage = MissingImage.identicon)
        {
            var url = GetGravatarUri(emailAddress, rating, missingImage);

            var wc = new WebClient();
            wc.DownloadDataCompleted += (s, e) =>
                {
                    try
                    {
                        if (e.Error == null)
                        {
                            var ms = new MemoryStream(e.Result);
                            onComplete(new Bitmap(ms));

                            ((IDisposable)e.UserState).Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        NLog.LogManager.GetCurrentClassLogger().Error(ex, "An application error has occurred.");
                    }
                    
                };
            wc.DownloadDataAsync(url, wc);
        }

        /// <summary>
        /// Retrieves the MD5 encoded hash string of the provided input.
        /// </summary>
        /// <param name="emailAddress">The input text.</param>
        /// <returns>Returns the MD5 hash string of the input string.</returns>
        private static string Md5EncodeText(string emailAddress)
        {
            var sb = new StringBuilder();

            if(emailAddress == string.Empty)
                emailAddress = RandomUtils.GetRandomString(10) + "";

            byte[] ss = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(emailAddress));
            
            foreach (byte b in ss)
            {
                sb.Append(b.ToString("X2"));
            }
            
            return sb.ToString().ToLower();
        }

        #region Rating

        public enum Rating { g, pg, r, x }

        #endregion

        #region MissingImage

        public enum MissingImage { mm, identicon, monsterid, wavatar, retro, blank }

        #endregion


    }
}
