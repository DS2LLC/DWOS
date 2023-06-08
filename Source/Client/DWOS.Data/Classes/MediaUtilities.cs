using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Properties;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using NLog;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DWOS.Data
{

    /// <summary>
    /// Defines media-related utility methods.
    /// </summary>
    public static class MediaUtilities
    {
        public const int TWIPS_PER_POINT = 20;

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static readonly ImageConverter _imageConverter = new ImageConverter();

        /// <summary>
        /// Prevents anything from reading thumbnail images during creation.
        /// </summary>
        private static readonly object THUMBNAIL_LOCK = new object();

        /// <summary>
        /// Creates the thumbnail based on the media id, extension, and bytes.
        /// </summary>
        /// <param name="mediaID">The media ID.</param>
        /// <param name="extension">The extension.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="imageSize">Size of the image thumbnail, default is 128.</param>
        /// <returns>Image.</returns>
        public static Image GetThumbnail(int mediaID, string extension, byte[] bytes = null, int imageSize = 128)
        {
            EncoderParameter compression = null;
            EncoderParameters codecParams = null;

            try
            {
                extension = extension.ToLower();

                if (!IsImageExtension(extension))
                    return GetGenericThumbnail(extension);

                string thumbPath = GetMediaThumbnailImagePath(mediaID, bytes, imageSize);

                lock (THUMBNAIL_LOCK)
                {
                    //if found thumbnail then return it
                    if (File.Exists(thumbPath))
                        return GetImage(thumbPath);

                    //if no bytes then get from DB
                    if (bytes == null || bytes.Length < 1)
                        bytes = GetMediaFromDatabase(mediaID);

                    //if bytes are populated (i.e. this is a new image) then create a thumbnail from it
                    if (bytes != null && bytes.Length > 0)
                    {
                        //get image then cache it locally as a thumbnail
                        using (Image fullImage = GetImage(bytes))
                        {
                            if (fullImage == null)
                            {
                                _log.Warn($"Unable to load thumbnail for image {mediaID}.");
                                return null;
                            }

                            Image thumbImage = CreateThumbnail(fullImage, imageSize);

                            ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(ic => ic.MimeType == "image/jpeg");
                            compression = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);

                            // Add the quality parameter to the list
                            codecParams = new EncoderParameters(1);
                            codecParams.Param[0] = compression;

                            thumbImage.Save(thumbPath, jpgEncoder, codecParams);
                            return thumbImage;
                        }
                    }
                }

                return Resources.UnknownImage;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error getting thumbnail image.");
                return Resources.UnknownImage;
            }
            finally
            {
                if (compression != null)
                {
                    compression.Dispose();
                }

                if (codecParams != null)
                {
                    codecParams.Dispose();
                }
            }
        }

        /// <summary>
        /// Retrieves a full-size <see cref="Image"/> instance.
        /// </summary>
        /// <param name="mediaID">The media ID.</param>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public static Image GetImage(int mediaID, string extension)
        {
            try
            {
                extension = extension.ToLower();

                if (!IsImageExtension(extension))
                    return GetGenericThumbnail(extension);

                string fullPath = GetMediaImagePath(mediaID, extension);

                //if found full image then return it
                if (File.Exists(fullPath))
                    return GetImage(fullPath);
                //if no bytes then get from DB
                byte[] bytes = GetMediaFromDatabase(mediaID);

                //if bytes are populated (i.e. this is a new image) then create a image from it
                if (bytes != null && bytes.Length > 0)
                    return GetImage(bytes);

                return Resources.NoImage;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error getting full image.");
                return Resources.NoImage;
            }
        }

        /// <summary>
        /// Retrieves a full-size <see cref="Image"/> instance.
        /// </summary>
        /// <param name="mediaID"></param>
        /// <param name="extension"></param>
        /// <param name="bytes">
        /// If non-null and non-empty, used to get a non-cached image.
        /// </param>
        /// <returns></returns>
        public static Image GetImage(int mediaID, string extension, byte[] bytes)
        {
            try
            {
                extension = extension.ToLower();

                if (!IsImageExtension(extension))
                    return GetGenericThumbnail(extension);

                string fullPath = GetMediaImagePath(mediaID, extension);

                //if found full image then return it
                if (File.Exists(fullPath))
                    return GetImage(fullPath);

                //if bytes are populated (i.e. this is a new image) then create a image from it
                if (bytes != null && bytes.Length > 0)
                    return GetImage(bytes);

                return Resources.NoImage;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error getting full image.");
                return Resources.NoImage;
            }
        }

        /// <summary>
        /// Opens media from the user's temp folder.
        /// </summary>
        /// <param name="mediaID">The media ID.</param>
        /// <param name="fileExtension">The file extension.</param>
        /// <param name="buffer">The buffer.</param>
        /// <returns>
        /// <c>true</c> if the media was opened; otherwise, <c>false</c>.
        /// </returns>
        public static bool OpenMedia(int mediaID, string fileExtension, byte[] buffer = null)
        {
            try
            {
                string file = GetMediaImagePath(mediaID, fileExtension);

                if (!File.Exists(file))
                {
                    if (buffer == null || buffer.Length < 1)
                        buffer = GetMediaFromDatabase(mediaID);

                    if (buffer != null && buffer.Length > 0)
                    {
                        using (var fileStream = new BinaryWriter(File.OpenWrite(file)))
                            fileStream.Write(buffer);
                    }
                }

                FileLauncher.New()
                    .HandleErrorsWith((exception, errorMsg) => { throw new Exception(errorMsg, exception); })
                    .Launch(file);

                return File.Exists(file);
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog(
                    "Error displaying media. Ensure you have the proper software to display the following media type: " +
                    fileExtension, exc);

                return false;
            }
        }

        /// <summary>
        /// Creates a byte array from a file.
        /// </summary>
        /// <remarks>
        /// If the file is an image, and maxDimension is greater than 0,
        /// <see cref="CreateMediaStream(string, int)"/> provides a stream of
        /// a thumbnail.
        /// </remarks>
        /// <param name="file">The file.</param>
        /// <param name="maxDimension">The max dimension - default is 640.</param>
        /// <returns></returns>
        public static byte[] CreateMediaStream(string file, int maxDimension = 640)
        {
            if (!File.Exists(file))
            {
                return null;
            }

            using (var oFS = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (oFS.Length == 0)
                {
                    return null;
                }

                string ext = Path.GetExtension(file).ToLower().Replace(".", "");

                if (IsImageExtension(ext) && maxDimension > 0)
                {
                    //reduce size of image
                    using (Image img = Image.FromStream(oFS))
                    {
                        Size newSize = GetThumbnailSize(img, maxDimension);
                        using (var bmp = new Bitmap(img, newSize.Width, newSize.Height))
                        {
                            return GetImageAsBytes(bmp);
                        }
                    }
                }

                using (var oBR = new BinaryReader(oFS))
                    return oBR.ReadBytes((int)oFS.Length);
            }
        }

        /// <summary>
        /// Displays dialog to allow a user to add new media.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="initialDirectory">The initial directory [optional].</param>
        /// <returns></returns>
        public static byte[] GetNewMedia(out string fileName, string initialDirectory = null)
        {
            fileName = null;

            using (var fd = new OpenFileDialog())
            {
                if (initialDirectory != null)
                    fd.InitialDirectory = initialDirectory;

                fd.Filter = "Any Media (*.*)|*.*";

                if (fd.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                {
                    fileName = fd.FileName;
                }
            }

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                try
                {
                    return CreateMediaStream(fileName);
                }
                catch (Exception)
                {
                    // Log name of media file - should appear in Raygun errors
                    _log.Warn("Error getting new media for file: {0}", fileName);
                    throw;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the thumbnail of an existing media item either from DB or FS.
        /// </summary>
        /// <param name="mediaID">The media ID.</param>
        /// <returns></returns>
        private static byte[] GetMediaFromDatabase(int mediaID)
        {
            using (var mediaTA = new MediaTableAdapter())
            {
                return mediaTA.GetMediaStream(mediaID);
            }
        }

        /// <summary>
        /// Retrieves a full-size <see cref="Image"/> instance.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Image GetImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            try
            {
                return (Image)_imageConverter.ConvertFrom(bytes);
            }
            catch (ArgumentException exc)
            {
                _log.Warn(exc, "Cannot load invalid image.");
                return null;
            }
        }

        /// <summary>
        /// Converts this instance to a byte array using JPEG encoding.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="compressionPercent"></param>
        /// <returns></returns>
        public static byte[] GetImageAsBytes(this Image img, long compressionPercent = 80L)
        {
            //get thumbnail bytes
            using (var ms = new MemoryStream())
            {
                ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(ic => ic.MimeType == "image/jpeg");
                var compression = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, compressionPercent);

                // Add the quality parameter to the list
                var codecParams = new EncoderParameters(1);
                codecParams.Param[0] = compression;

                img.Save(ms, jpgEncoder, codecParams);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Converts this instance to a byte array using PNG encoding.
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] GetImageAsBytesPng(this Image img)
        {
            //get thumbnail bytes
            using (var ms = new MemoryStream())
            {
                ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(ic => ic.MimeType == "image/png");
                var compression = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                // Add the quality parameter to the list
                var codecParams = new EncoderParameters(1);
                codecParams.Param[0] = compression;

                img.Save(ms, jpgEncoder, codecParams);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Compresses an image using JPEG encoding.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="compressionPercent"></param>
        /// <returns></returns>
        public static Image CompressImage(this Image img, long compressionPercent = 80L)
        {
            //get thumbnail bytes
            using (var ms = new MemoryStream())
            {
                var jpgEncoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(ic => ic.MimeType == "image/jpeg");
                var compression = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, compressionPercent);

                // Add the quality parameter to the list
                var codecParams = new EncoderParameters(1);
                codecParams.Param[0] = compression;

                img.Save(ms, jpgEncoder, codecParams);
                return Image.FromStream(ms);
            }
        }

        /// <summary>
        /// Retrieves an <see cref="Image"/> from a file.
        /// </summary>
        /// <remarks>
        /// Intended as an alternative to <see cref="Image.FromFile(string)"/>
        /// that does not throw <see cref="OutOfMemoryException"/> when
        /// dealing with an invalid image format.
        /// </remarks>
        /// <param name="filePath">Path of the file to read from.</param>
        /// <returns></returns>
        public static Image GetImage(string filePath)
        {
            return !File.Exists(filePath)
                ? null
                : GetImage(File.ReadAllBytes(filePath));
        }

        /// <summary>
        /// Prepares an image for use in a report.
        /// </summary>
        /// <remarks>
        /// This method replaces transparent with <see cref="Color.White"/>.
        /// </remarks>
        /// <param name="source">Source image</param>
        /// <returns>Prepared copy of the image.</returns>
        public static Image PrepareForReport(Image source)
        {
            if (source == null)
            {
                return null;
            }

            var newImage = new Bitmap(source.Width, source.Height);

            using (var g = Graphics.FromImage(newImage))
            {
                g.Clear(Color.White);
                g.DrawImage(source, 0, 0, source.Width, source.Height);
            }

            return newImage;
        }

        private static Image CreateThumbnail(Image img, int maxSize = 128)
        {
            Size newSize = GetThumbnailSize(img, maxSize);
            return img.GetThumbnailImage(newSize.Width, newSize.Height, () => true, IntPtr.Zero);
        }

        private static string GetMediaImagePath(int mediaID, string fileExtension)
        {
            //if temp media id
            if (mediaID < 0)
                mediaID = new Random().Next(10000000, 99999999);

            string fileName = mediaID + "." + fileExtension;

            //add tmp to file name to know that it is just temp
            if (mediaID < 0)
                fileName = "TMP_" + fileName;

            return Path.Combine(FileSystem.GetFolder(FileSystem.enumFolderType.MediaCache, true), fileName);
        }

        /// <summary>
        /// Generates the filename for an image thumbnail.
        /// </summary>
        /// <param name="mediaID">ID of the media (database)</param>
        /// <param name="bytes">data for the source image</param>
        /// <param name="size">target width/height for the thumbnail</param>
        /// <returns>filename to use for the image; may or may not exist.</returns>
        private static string GetMediaThumbnailImagePath(int mediaID, byte[] bytes, int size = 128)
        {
            string identifier;

            if (mediaID >= 0)
            {
                identifier = Convert.ToString(mediaID);
            }
            else if (bytes != null && bytes.Length > 0)
            {
                using (var hashAlgorithm = System.Security.Cryptography.MD5.Create())
                {
                    var hashData = hashAlgorithm.ComputeHash(bytes);
                    identifier = BitConverter.ToString(hashData).Replace("-", string.Empty);
                }
            }
            else
            {
                identifier = Convert.ToString(new Random().Next(10000000, 99999999));
            }

            string fileName = identifier + "_thumb_" + size + ".jpg";

            return Path.Combine(FileSystem.GetFolder(FileSystem.enumFolderType.MediaCache, true), fileName);
        }

        /// <summary>
        /// Returns a value indicating that the extension represents a known
        /// image type.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns>
        /// <c>true</c> if this extension represents a known image format;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool IsImageExtension(string extension)
        {
            if (!String.IsNullOrEmpty(extension))
            {
                extension = extension.ToLower();
                switch (extension)
                {
                    case "jpg":
                    case "gif":
                    case "bmp":
                    case "ico":
                    case "png":
                    case "tif":
                    case "tiff":
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a generic thumbnail image for the given file type.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static Image GetGenericThumbnail(string extension)
        {
            extension = extension.ToLower();

            switch (extension)
            {
                case "xls":
                case "xlsx":
                    return Resources.Excel;
                case "doc":
                case "docx":
                    return Resources.Word;
                case "pdf":
                    return Resources.PDF;
                case "ppt":
                case "pptx":
                    return Resources.PPT;
                case "jpg":
                case "bmp":
                case "png":
                case "tif":
                case "tiff":
                    return Resources.Image;
                default:
                    return Resources.UnknownFile;
            }
        }

        private static Size GetThumbnailSize(Image original, int maxDimension)
        {
            // Width and height.
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            // Compute best factor to scale entire image based on larger dimension.
            double factor;
            if (originalWidth > originalHeight)
            {
                factor = (double)maxDimension / originalWidth;
            }
            else
            {
                factor = (double)maxDimension / originalHeight;
            }

            // Return thumbnail size.
            var width = Math.Max((int)(originalWidth * factor), 1);
            var height = Math.Max((int)(originalHeight * factor), 1);
            return new Size(width, height);
        }

        /// <summary>
        /// Generates a new <see cref="Size"/> that fits within
        /// a maximum size.
        /// </summary>
        /// <param name="currentSize">The base size.</param>
        /// <param name="maximumSize">The maximum size of the generated size.</param>
        /// <returns>A new size value that fits within the minimum.</returns>
        public static Size Resize(Size currentSize, Size maximumSize)
        {
            if (currentSize.Width <= maximumSize.Width &&
                currentSize.Height <= maximumSize.Height)
            {
                return currentSize;
            }

            if (currentSize.Width > currentSize.Height)
            {
                var newSize = SizeToWidth(currentSize, maximumSize.Width);

                if (newSize.Height > maximumSize.Height)
                {
                    newSize = SizeToHeight(newSize, maximumSize.Height);
                }

                return newSize;
            }
            else if (currentSize.Width < currentSize.Height)
            {
                var newSize = SizeToHeight(currentSize, maximumSize.Height);

                if (newSize.Width > maximumSize.Width)
                {
                    newSize = SizeToWidth(newSize, maximumSize.Width);
                }
                return newSize;
            }
            else
            {
                int minDimension = Math.Min(maximumSize.Width, maximumSize.Height);
                return new Size(minDimension, minDimension);
            }
        }

        /// <summary>
        /// Generates a new size with the same aspect ratio whose height
        /// is, at max, a given value.
        /// </summary>
        /// <param name="currentSize"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        private static Size SizeToHeight(Size currentSize, int maxHeight)
        {
            int newHeight = maxHeight;
            int newWidth = (currentSize.Width * newHeight) / currentSize.Height;
            return new Size(newWidth, newHeight);
        }

        /// <summary>
        /// Generates a new size with the same aspect ratio whose width
        /// is, at max, a given value.
        /// </summary>
        /// <param name="currentSize"></param>
        /// <param name="maxWidth"></param>
        /// <returns></returns>
        private static Size SizeToWidth(Size currentSize, int maxWidth)
        {
            int newWidth = maxWidth;
            int newHeight = (currentSize.Height * newWidth) / currentSize.Width;
            return new Size(newWidth, newHeight);
        }

        public static int PixelsToTwips(int pixels, float dpi)
        {
            const int pointsPerInch = 72;

            if (dpi <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(dpi),
                    "DPI cannot be equal to or less than 0.");
            }

            double points = pixels * pointsPerInch / dpi;
            return Convert.ToInt32(points * TWIPS_PER_POINT);
        }

        /// <summary>
        /// Requests an image path from the user.
        /// </summary>
        /// <returns>
        /// Path the selected image if the user accepts the dialog; otherwise, null.
        /// </returns>
        public static string SelectImageDialog()
        {
            try
            {
                using (var fd = new OpenFileDialog())
                {
                    fd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";

                    if (fd.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                    {
                        return fd.FileName;
                    }
                }

                return null;
            }
            catch (Exception exc)
            {
                throw new Exception("Error opening media.", exc);
            }
        }
    }
}