using System.Windows;
using Infragistics.Controls.Charts;

namespace DWOS.Dashboard.Production
{
    /// <summary>
    /// Manages a <see cref="ChartInfoStripsBehavior"/> instance.
    /// </summary>
    public class ChartStripBehaviors : DependencyObject
    {
        #region Chart Info Strips Behavior

        internal const string ChartInfoStripsPropertyName = "ChartInfoStrips";
        public static readonly DependencyProperty ChartInfoStripsProperty =
            DependencyProperty.RegisterAttached(ChartInfoStripsPropertyName,
            typeof(ChartInfoStripsBehavior), typeof(ChartStripBehaviors),
            new PropertyMetadata(null, (o, e) => OnInfoStripsChanged(o as XamDataChart,
                    e.OldValue as ChartInfoStripsBehavior,
                    e.NewValue as ChartInfoStripsBehavior)));

        public static ChartInfoStripsBehavior GetChartInfoStrips(DependencyObject target)
        {
            return target.GetValue(ChartInfoStripsProperty) as ChartInfoStripsBehavior;
        }
        public static void SetChartInfoStrips(DependencyObject target, ChartInfoStripsBehavior behavior)
        {
            target.SetValue(ChartInfoStripsProperty, behavior);
        }

        private static void OnInfoStripsChanged(XamDataChart chart, ChartInfoStripsBehavior oldValue, ChartInfoStripsBehavior newValue)
        {
            if (chart == null)
            {
                return;
            }
            if (oldValue != null)
            {
                oldValue.OnDetach(chart);
            }
            if (newValue != null)
            {
                newValue.OnAttach(chart);
            }
        }
        #endregion
    }
}
