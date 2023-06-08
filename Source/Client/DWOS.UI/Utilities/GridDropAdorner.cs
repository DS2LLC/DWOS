using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DWOS.UI.Utilities
{
    internal class GridDropAdorner : Adorner
    {
        public GridDropAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var adornedElementRect = new Rect(AdornedElement.DesiredSize);
            drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.Black), 2),
                adornedElementRect.BottomLeft,
                adornedElementRect.BottomRight);
        }
    }
}
