using Ionic.Zlib;
using System;
using System.IO;
using System.Text;

namespace DWOS.Data
{
    /// <summary>
    /// Defines compression-related utility methods.
    /// </summary>
    public static class ZipUtilities
    {
        #region Methods

        /// <summary>
        /// Compresses bytes using gzip.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] CompressBytes(this byte[] input)
        {
            return GZipStream.CompressBuffer(input);
        }

        /// <summary>
        /// Decompresses gzip'd bytes.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] DecompressBytes(this byte[] input)
        {
            return GZipStream.UncompressBuffer(input);
        }

        /// <summary>
        /// Compresses a string using gzip and base64.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>
        /// A string that has been converted to bytes (using UTF-8), compressed, and 
        /// converted to base64.
        /// </returns>
        public static string CompressString(this string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                gZipStream.Write(buffer, 0, buffer.Length);

            memoryStream.Position = 0;
            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);

            return Convert.ToBase64String(gZipBuffer);
        }

        /// <summary>
        /// Decompresses a string previously compressed by
        /// <see cref="CompressString(string)"/>.
        /// </summary>
        /// <param name="compressedText"></param>
        /// <returns></returns>
        public static string DecompressString(this string compressedText)
        {
            try
            {
                var gZipBuffer = Convert.FromBase64String(compressedText);

                using (var memoryStream = new MemoryStream())
                {
                    int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                    memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                    var buffer = new byte[dataLength];

                    memoryStream.Position = 0;
                    using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        gZipStream.Read(buffer, 0, buffer.Length);
                    }

                    return Encoding.UTF8.GetString(buffer);
                }
            }
            catch
            {
                //NOTE: Have had original implementation from above since beginning. 
                //  Converted to GZipStream built-in in methods on 2-11-14
                //  Realized data stored using original implementation is not interchangable with built-in version
                //  Very few got the built-in version, while other version has been in wide use, so switching back to original version
                return GZipStream.UncompressString(Convert.FromBase64String(compressedText));
            }
        }

        #endregion
    }
}
