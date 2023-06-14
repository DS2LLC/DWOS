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
using Android.Graphics.Drawables;
using Android.Graphics;

namespace DWOS.Android
{
    /// <summary>
    /// <see cref="CircleDrawable"/> provides Circle "masking" support for anything that
    /// can take a drawable.  Taken from http://xamarin.com/prebuilt/sharp-shirt
    /// </summary>
    public class CircleDrawable : Drawable
    {
        Bitmap bmp;
        BitmapShader bmpShader;
        Paint paint;
        int circleCenterX;
        int circleCenterY;
        int radius;

        public CircleDrawable(Bitmap bmp)
        {
            this.bmp = bmp;
            this.bmpShader = new BitmapShader(bmp, Shader.TileMode.Clamp, Shader.TileMode.Clamp);
            this.paint = new Paint() { AntiAlias = true };
            this.paint.SetShader(bmpShader);
        }

        public override void Draw(Canvas canvas)
        {
            canvas.DrawCircle(circleCenterX, circleCenterY, radius, paint);
        }

        protected override void OnBoundsChange(Rect bounds)
        {
            base.OnBoundsChange(bounds);
            circleCenterX = bounds.Width() / 2;
            circleCenterY = bounds.Height() / 2;

            if (bounds.Width() >= bounds.Height())
                radius = bounds.Width() / 2;
            else
                radius = bounds.Height() / 2;
        }

        public override int IntrinsicWidth
        {
            get
            {
                return bmp.Width;
            }
        }

        public override int IntrinsicHeight
        {
            get
            {
                return bmp.Height;
            }
        }

        public override void SetAlpha(int alpha)
        {

        }

        public override int Opacity
        {
            get
            {
                return (int)Format.Opaque;
            }
        }

        public override void SetColorFilter(ColorFilter cf)
        {

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                bmp.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}