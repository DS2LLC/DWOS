using System;
using System.Drawing;
using System.Linq;

namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Generates default user images.
    /// </summary>
    public static class UserImageGenerator
    {
        #region Fields

        private const string FONT_FAMILY = "Arial";
        private const int FONT_SIZE = 24;
        private const int IMAGE_WIDTH = 80;
        private const int IMAGE_HEIGHT = 80;

        #endregion

        #region Methods

        /// <summary>
        /// Generates an image based off of the given full name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>New bitmap instance.</returns>
        public static Bitmap GetImage(string name)
        {
            Bitmap returnBitmap = null;
            Font rectangleFont = null;
            Graphics graphics = null;

            try
            {
                rectangleFont = new Font(FONT_FAMILY, FONT_SIZE);
                returnBitmap = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT);
                graphics = Graphics.FromImage(returnBitmap);

                graphics.Clear(GetFillColor(name));

                var format = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                graphics.DrawString(
                    GetInitials(name),
                    rectangleFont,
                    Brushes.White,
                    new RectangleF(0, 0, IMAGE_WIDTH, IMAGE_HEIGHT),
                    format);
            }
            finally
            {
                rectangleFont?.Dispose();
                graphics?.Dispose();
            }

            return returnBitmap;

        }

        private static Color GetFillColor(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Color.Black;
            }

            var code = int.Parse(name.GetHashCode().ToString().Last().ToString());

            Color fillColor;
            switch (code)
            {
                case 1:
                case 2:
                case 3:
                    fillColor = Color.RoyalBlue;
                    break;
                case 4:
                case 5:
                case 6:
                    fillColor = Color.MediumAquamarine;
                    break;
                case 7:
                case 8:
                case 9:
                    fillColor = Color.Firebrick;
                    break;
                case 0:
                default:
                    fillColor = Color.DarkSlateGray;
                    break;
            }

            return fillColor;
        }


        private static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "?";
            }
            else
            {
                return name.ToInitials(StringInitialOption.FirstTwoInitials);
            }
        }

        #endregion
    }
}
