using Infragistics.Controls.Charts;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DWOS.Dashboard.Production
{
    /// <summary>
    /// Manages a <see cref="InfoStripCollection"/> instance.
    /// </summary>
    public class ChartInfoStripsBehavior : DependencyObject
    {
        public ChartInfoStripsBehavior()
        {
            InfoStrips = new InfoStripCollection();
        }
        private XamDataChart _owner;

        #region Properties
        internal const string InfoStripsPropertyName = "InfoStrips";
        public static readonly DependencyProperty InfoStripsProperty =
            DependencyProperty.Register(InfoStripsPropertyName,
            typeof(InfoStripCollection), typeof(ChartInfoStripsBehavior),
            new PropertyMetadata(null, (o, e) => (o as ChartInfoStripsBehavior).OnStripsChanged(
                e.OldValue as InfoStripCollection,
                e.NewValue as InfoStripCollection)));

        public InfoStripCollection InfoStrips
        {
            get { return (InfoStripCollection)GetValue(InfoStripsProperty); }
            set { SetValue(InfoStripsProperty, value); }
        }

        public DataTemplate InfoStripTemplate { get; set; }
        #endregion

        #region Event Handlers
        private void OnStripsChanged(InfoStripCollection oldValue, InfoStripCollection newValue)
        {
            if (oldValue != null)
            {
                oldValue.CollectionChanged -= OnStripsCollectionChanged;
            }

            if (newValue != null)
            {
                newValue.CollectionChanged += OnStripsCollectionChanged;
            }
            RefreshStrips();
        }
        private void OnStripsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshStrips();
        }
        public void OnAttach(XamDataChart chart)
        {
            if (_owner != null)
            {
                OnDetach(_owner);
            }
            _owner = chart;

            _owner.WindowRectChanged += Owner_WindowRectChanged;
            _owner.SizeChanged += Owner_SizeChanged;
            _owner.Axes.CollectionChanged += Axes_CollectionChanged;
            _owner.Axes.CollectionResetting += Axes_CollectionResetting;

            RefreshStrips();
        }
        public void OnDetach(XamDataChart chart)
        {
            if (_owner != chart)
            {
                return;
            }

            _owner.WindowRectChanged -= Owner_WindowRectChanged;
            _owner.SizeChanged -= Owner_SizeChanged;
            _owner.Axes.CollectionChanged -= Axes_CollectionChanged;
            _owner.Axes.CollectionResetting -= Axes_CollectionResetting;

            Axis xAxis;
            Axis yAxis;
            GetAxes(out xAxis, out yAxis);

            if (xAxis == null || yAxis == null)
            {
                return;
            }

            ClearExisting(xAxis);

            _owner = null;
        }
        private void Owner_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshStrips();
        }
        private void Owner_WindowRectChanged(object sender, Infragistics.RectChangedEventArgs e)
        {
            RefreshStrips();
        }
        private void Axis_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshStrips();
        }
        private void Axes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Axis axis in e.OldItems)
                {
                    axis.SizeChanged -= Axis_SizeChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (Axis axis in e.NewItems)
                {
                    axis.SizeChanged += Axis_SizeChanged;
                }
            }
            RefreshStrips();
        }
        private void Axes_CollectionResetting(object sender, EventArgs e)
        {
            foreach (Axis axis in _owner.Axes)
            {
                axis.SizeChanged -= Axis_SizeChanged;
            }
            RefreshStrips();
        }

        #endregion

        #region Methods
        private void GetAxes(out Axis xAxis, out Axis yAxis)
        {
            if (_owner == null)
            {
                xAxis = null;
                yAxis = null;
                return;
            }

            xAxis =
               (from axis in _owner.Axes
                where axis is NumericXAxis ||
                axis is CategoryXAxis ||
                axis is CategoryDateTimeXAxis
                select axis).FirstOrDefault();

            yAxis =
                (from axis in _owner.Axes
                 where axis is NumericYAxis
                 select axis).FirstOrDefault();
        }
        private void ClearExisting(Axis xAxis)
        {
            if (xAxis.RootCanvas == null)
            {
                return;
            }
            var existing =
               from child in xAxis.RootCanvas.Children.OfType<UIElement>()
               where child is ContentControl &&
               (child as ContentControl).Content != null &&
               (child as ContentControl).Content is InfoStrip
               select child;
            existing.ToList().ForEach(ele => xAxis.RootCanvas.Children.Remove(ele));
        }
        private void RefreshStrips()
        {
            Axis xAxis;
            Axis yAxis;
            GetAxes(out xAxis, out yAxis);

            if (xAxis == null || yAxis == null || xAxis.RootCanvas == null)
            {
                return;
            }

            ClearExisting(xAxis);

            Rect viewport = GetViewportRect(xAxis);
            Rect window = _owner.WindowRect;

            foreach (InfoStrip strip in this.InfoStrips)
            {
                InfoStrip toAdd = strip.Clone();

                bool isInverted = xAxis.IsInverted;
                ScalerParams param = new ScalerParams(window, viewport, isInverted);

                if (!toAdd.UseDates)
                {
                    if (double.IsNaN(toAdd.StartX))
                    {
                        toAdd.StartX = viewport.Left;
                    }
                    else
                    {
                        toAdd.StartX = xAxis.GetScaledValue(toAdd.StartX, param);
                        //NOTE: prior to 12.1 release: 
                        //toAdd.StartX = xAxis.GetScaledValue(toAdd.StartX, window, viewport);
                    }
                    if (double.IsNaN(toAdd.EndX))
                    {
                        toAdd.EndX = viewport.Right;
                    }
                    else
                    {
                        toAdd.EndX = xAxis.GetScaledValue(toAdd.EndX, param);
                        //NOTE: prior to 12.1 release: 
                        //toAdd.EndX = xAxis.GetScaledValue(toAdd.EndX, window, viewport);
                    }
                }
                else
                {
                    toAdd.StartX = xAxis.GetScaledValue(toAdd.StartDate.Ticks, param);
                    toAdd.EndX = xAxis.GetScaledValue(toAdd.EndDate.Ticks, param);
                    //NOTE: prior to 12.1 release: 
                    //toAdd.StartX = xAxis.GetScaledValue(toAdd.StartDate.Ticks, window, viewport);
                    //toAdd.EndX = xAxis.GetScaledValue(toAdd.EndDate.Ticks, window, viewport);
                }
                // check if y-axis is inverted
                isInverted = yAxis.IsInverted;
                param = new ScalerParams(window, viewport, isInverted);

                if (double.IsNaN(toAdd.StartY))
                {
                    toAdd.StartY = viewport.Top;
                }
                else
                {
                    toAdd.StartY = yAxis.GetScaledValue(toAdd.StartY, param);
                    //NOTE: prior to 12.1 release: 
                    //toAdd.StartY = yAxis.GetScaledValue(toAdd.StartY, window, viewport);
                }
                if (double.IsNaN(toAdd.EndY))
                {
                    toAdd.EndY = viewport.Bottom;
                }
                else
                {
                    toAdd.EndY = yAxis.GetScaledValue(toAdd.EndY, param);
                    //NOTE: prior to 12.1 release: 
                    //toAdd.EndY = yAxis.GetScaledValue(toAdd.EndY, window, viewport);
                }
                if (toAdd.StripTemplate == null)
                {
                    toAdd.StripTemplate = this.InfoStripTemplate;
                }

                if (toAdd.StripTemplate != null &&
                    !double.IsNaN(toAdd.StartY) &&
                    !double.IsInfinity(toAdd.StartY) &&
                    !double.IsNaN(toAdd.StartX) &&
                    !double.IsInfinity(toAdd.StartX))
                {
                    ContentControl stripControl = new ContentControl();
                    stripControl.ContentTemplate = toAdd.StripTemplate;
                    stripControl.Content = toAdd;
                    Canvas.SetLeft(stripControl, toAdd.StartX);
                    Canvas.SetTop(stripControl, toAdd.StartY);

                    xAxis.RootCanvas.Children.Add(stripControl);
                }
            }
        }
        private Rect GetViewportRect(Axis axis)
        {
            double top = 0;
            double bottom = axis.ActualHeight;
            double left = 0;
            double right = axis.ActualWidth;

            double width = right - left;
            double height = bottom - top;

            if (width > 0.0 && height > 0.0)
            {
                return new Rect(left, top, width, height);
            }
            return Rect.Empty;
        }
        #endregion
    }
}
