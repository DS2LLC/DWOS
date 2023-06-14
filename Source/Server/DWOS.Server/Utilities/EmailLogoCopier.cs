using DWOS.Data;
using System.Drawing;
using System.IO;
using NLog;

namespace DWOS.Server.Utilities
{
    /// <summary>
    /// Responsible for copying logo and caching the logo copy's path.
    /// </summary>
    internal sealed class EmailLogoCopier
    {
        #region Properties

        /// <summary>
        /// Gets the cached path of the logo copy.
        /// </summary>
        public string CopiedLogoPath
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Copies the company logo
        /// (<see cref="ApplicationSettings.CompanyLogoImagePath"/>) to a
        /// directory within the given root directory.
        /// </summary>
        /// <param name="rootDirectory">The directory to store the image in.</param>
        public void CopyCompanyLogo(string rootDirectory)
        {
            var companyLogoPath = ApplicationSettings.Current.CompanyLogoImagePath;

            //ensure company logo file is in the correct directory
            if (string.IsNullOrEmpty(CopiedLogoPath) && !string.IsNullOrEmpty(companyLogoPath))
            {
                PrepareDirectory(rootDirectory);
                CopiedLogoPath = CopyFile(rootDirectory, companyLogoPath);
            }
        }

        public void ResizeCompanyLogo(string rootDirectory, Size maximumSize)
        {
            var companyLogoPath = ApplicationSettings.Current.CompanyLogoImagePath;

            //ensure company logo file is in the correct directory
            if (string.IsNullOrEmpty(CopiedLogoPath) && !string.IsNullOrEmpty(companyLogoPath))
            {
                PrepareDirectory(rootDirectory);
                CopiedLogoPath = ResizeImage(rootDirectory, companyLogoPath, maximumSize);
            }
        }

        /// <summary>
        /// Resizes an image (with padding).
        /// </summary>
        /// <param name="rootDirectory"></param>
        /// <param name="originalPath"></param>
        /// <param name="imageSize">The size of the output image.</param>
        /// <returns>Path to the new image.</returns>
        private static string ResizeImage(string rootDirectory, string originalPath, Size imageSize)
        {
            if (string.IsNullOrEmpty(Path.GetFileName(originalPath)))
            {
                // No file to resize
                return string.Empty;
            }

            // Assumption: originalPath is an image file
            var fileName = string.Format("{0}_{1}x{2}{3}",
                Path.GetFileNameWithoutExtension(originalPath),
                imageSize.Width,
                imageSize.Height,
                Path.GetExtension(originalPath));

            string logoOutputPath = Path.Combine(rootDirectory, fileName);
            if (!File.Exists(logoOutputPath))
            {
                try
                {
                    using (var originalImg = Image.FromFile(originalPath))
                    {
                        var newSize = MediaUtilities.Resize(originalImg.Size, imageSize);

                        using (var newImage = new Bitmap(imageSize.Width, imageSize.Height))
                        {
                            int xOffset = (imageSize.Width - newSize.Width) / 2;
                            int yOffset = (imageSize.Height - newSize.Height) / 2;
                            using (var gfx = Graphics.FromImage(newImage))
                            {
                                gfx.DrawImage(originalImg, xOffset, yOffset, newSize.Width, newSize.Height);
                                newImage.Save(logoOutputPath);
                            }
                        }
                    }
                }
                catch (PathTooLongException)
                {
                    const string errorMsgFormat = "Encountered large file name while resizing." +
                        "\n\tSource: \"{0}\"" +
                        "\n\tDestination: \"{1}\"";

                    LogManager.GetCurrentClassLogger().Warn(errorMsgFormat, originalPath, logoOutputPath);
                    throw;
                }
            }

            return logoOutputPath;
        }

        private static string CopyFile(string rootDirectory, string originalPath)
        {
            var fileName = Path.GetFileName(originalPath);

            if (string.IsNullOrEmpty(fileName))
            {
                // No file to copy
                return string.Empty;
            }

            string logoOutputPath = Path.Combine(rootDirectory, fileName);

            if (!File.Exists(logoOutputPath))
            {
                try
                {
                    File.Copy(originalPath, logoOutputPath);
                }
                catch (PathTooLongException)
                {
                    const string errorMsgFormat = "Encountered large file name while copying." +
                        "\n\tSource: \"{0}\"" +
                        "\n\tDestination: \"{1}\"";

                    LogManager.GetCurrentClassLogger().Warn(errorMsgFormat, originalPath, logoOutputPath);
                    throw;
                }
            }

            return logoOutputPath;
        }

        private static void PrepareDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        #endregion
    }
}
