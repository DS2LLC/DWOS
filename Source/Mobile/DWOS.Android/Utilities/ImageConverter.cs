using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Content.Res;
using System.Threading.Tasks;

namespace DWOS.Android
{
    /// <summary>
    /// Provides utilities to create a <see cref="Bitmap"/> with a memory-friendly size
    /// </summary>
    public static class ImageConverter
    {
        /// <summary>
        /// Calcutes the InSample size.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="reqWidth">Width of the req.</param>
        /// <param name="reqHeight">Height of the req.</param>
        /// <returns></returns>
        public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and with of image
            var height = (float)options.OutHeight;
            var width = (float)options.OutWidth;
            var inSampleSize = 1D;
            if (height > reqHeight || width > reqWidth)
                inSampleSize = width > height ? height / reqHeight : width / reqWidth;
            
            return (int)inSampleSize;
        }

        /// <summary>
        /// Decodes the sampled bitmap from a resource.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="resId">The resource identifier.</param>
        /// <param name="reqWidth">Width of the req.</param>
        /// <param name="reqHeight">Height of the req.</param>
        /// <returns></returns>
        public static Bitmap DecodeSampledBitmapFromBytes(byte[] buffer, int reqWidth, int reqHeight)
        {
            // First decode with inJustDecodeBounds=true to check dimensions
            var options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true,
            };
            var offset = 0;
            using (var dispose = BitmapFactory.DecodeByteArray(buffer, offset, buffer.Length, options))
            {
            }

            // Calculate inSampleSize
            options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;
            
            return BitmapFactory.DecodeByteArray(buffer, offset, buffer.Length, options);
        }
    }
}