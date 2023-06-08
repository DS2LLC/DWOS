using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DWOS.UI.Utilities
{
    public class DoubleBorderDecorator : Decorator
    {
        public static readonly DependencyProperty InnerBorderThicknessProperty =
            DependencyProperty.Register("InnerBorderThickness", typeof(Thickness), typeof(DoubleBorderDecorator), 
            new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, null, CoerceThickness));

        public static readonly DependencyProperty OuterBorderThicknessProperty =
            DependencyProperty.Register("OuterBorderThickness", typeof(Thickness), typeof(DoubleBorderDecorator),
            new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, null, CoerceThickness));

        public static readonly DependencyProperty BackgroundProperty =
            Border.BackgroundProperty.AddOwner(typeof(DoubleBorderDecorator), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof (double), typeof (DoubleBorderDecorator), 
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, null, CoerceCornerRadius));

        private static object CoerceCornerRadius(DependencyObject d, object basevalue)
        {
            var cr = (double) basevalue;
            return Math.Max(0, cr);
        }

        public static readonly DependencyProperty OuterBorderBrushProperty =
            DependencyProperty.Register("OuterBorderBrush", typeof (Brush), typeof (DoubleBorderDecorator), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty InnerBorderBrushProperty =
            DependencyProperty.Register("InnerBorderBrush", typeof(Brush), typeof(DoubleBorderDecorator), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender));

        [Category("Brushes")]
        public Brush InnerBorderBrush
        {
            get { return (Brush) GetValue(InnerBorderBrushProperty); }
            set { SetValue(InnerBorderBrushProperty, value); }
        }

        [Category("Brushes")]
        public Brush OuterBorderBrush
        {
            get { return (Brush) GetValue(OuterBorderBrushProperty); }
            set { SetValue(OuterBorderBrushProperty, value); }
        }

        [Category("Appearance")]
        public double CornerRadius
        {
            get { return (double) GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        [Category("Brushes")]
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        [Category("Appearance")]
        public Thickness OuterBorderThickness
        {
            get { return (Thickness)GetValue(OuterBorderThicknessProperty); }
            set { SetValue(OuterBorderThicknessProperty, value); }
        }

        [Category("Appearance")]
        public Thickness InnerBorderThickness
        {
            get { return (Thickness)GetValue(InnerBorderThicknessProperty); }
            set { SetValue(InnerBorderThicknessProperty, value); }
        }

        private static object CoerceThickness(DependencyObject d, object basevalue)
        {
            var thickness = (Thickness) basevalue;
            var l = Math.Max(0, thickness.Left);
            var r = Math.Max(0, thickness.Right);
            var t = Math.Max(0, thickness.Top);
            var b = Math.Max(0, thickness.Bottom);

            return new Thickness(l, t, r, b);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var borderSize = GetBorderSize();
            if (Child != null)
            {
                var width = Math.Max(0, constraint.Width - borderSize.Width);
                var height = Math.Max(0, constraint.Height - borderSize.Height);

                Child.Measure(new Size(width, height));

                var measureWidth = Child.DesiredSize.Width + borderSize.Width;
                var measureHeight = Child.DesiredSize.Height + borderSize.Height;
                return new Size(measureWidth, measureHeight);
            }

            return borderSize;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {

            if (Child != null)
            {
                var borderSize = GetBorderSize();
                var rect = new Rect(arrangeSize);
                var childRect = new Rect(rect.Left + OuterBorderThickness.Left + InnerBorderThickness.Left,
                                         rect.Top + OuterBorderThickness.Top + InnerBorderThickness.Top,
                                         Math.Max(0, rect.Width - borderSize.Width),
                                         Math.Max(0, rect.Height - borderSize.Height));

                Child.Arrange(childRect);   
            }
            return arrangeSize;
        }

        private Size GetBorderSize()
        {
            var outer = new Size(OuterBorderThickness.Left + OuterBorderThickness.Right,
                                 OuterBorderThickness.Top + OuterBorderThickness.Bottom);
            var inner = new Size(InnerBorderThickness.Left + InnerBorderThickness.Right,
                                 InnerBorderThickness.Top + InnerBorderThickness.Bottom);

            return new Size(Math.Max(0, outer.Width + inner.Width), Math.Max(0, outer.Height + inner.Height));
        }

        protected override void OnRender(DrawingContext dc)
        {
            var outerRect = new Rect(RenderSize);
            dc.DrawRoundedRectangle(OuterBorderBrush, null, outerRect, CornerRadius, CornerRadius);

            var innerRect = new Rect(outerRect.Left + OuterBorderThickness.Left,
                                     outerRect.Top + OuterBorderThickness.Top,
                                     Math.Max(0,
                                              outerRect.Width - OuterBorderThickness.Left - OuterBorderThickness.Right),
                                     Math.Max(0,
                                              outerRect.Height - OuterBorderThickness.Top - OuterBorderThickness.Bottom));
            dc.DrawRoundedRectangle(InnerBorderBrush, null, innerRect, CornerRadius, CornerRadius);

            var backgroundRect = new Rect(innerRect.Left + InnerBorderThickness.Left,
                                     innerRect.Top + InnerBorderThickness.Top,
                                     Math.Max(0,
                                              innerRect.Width - InnerBorderThickness.Left - InnerBorderThickness.Right),
                                     Math.Max(0,
                                              innerRect.Height - InnerBorderThickness.Top - InnerBorderThickness.Bottom));
            dc.DrawRoundedRectangle(Background, null, backgroundRect, CornerRadius, CornerRadius);

        }
    }
}