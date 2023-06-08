using System.Windows;

using Neodynamic.SDK.Printing;

namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for ShapeItemDialog.xaml
    /// </summary>
    public partial class ShapeItemDialog : Window
    {
        #region Fields

        private ShapeItem _shapeItem = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the selected shape settings.
        /// </summary>
        public ShapeItem ShapeItem
        {
            get
            {
                if (_shapeItem is RectangleShapeItem)
                {
                    RectangleShapeItem rectItem = _shapeItem.Clone() as RectangleShapeItem;
                    //set properties based on dialog inputs
                    rectItem.Comments = generalUC1.ItemComments;
                    rectItem.CornerRadius = new RectangleCornerRadius(strokeFillUC1.ItemCornerRadius);
                    rectItem.FillColor = strokeFillUC1.ItemFillColor;
                    rectItem.Height = sizeUC1.ItemHeight;
                    rectItem.Width = sizeUC1.ItemWidth;
                    rectItem.Name = generalUC1.ItemName;
                    rectItem.PrintAsGraphic = generalUC1.PrintAsGraphic;
                    rectItem.StrokeColor = strokeFillUC1.ItemStrokeColor;
                    rectItem.StrokeThickness = strokeFillUC1.ItemStrokeThickness;
                    rectItem.X = positionUC1.ItemX;
                    rectItem.Y = positionUC1.ItemY;
                    rectItem.RotationAngle = sizeUC1.ItemRotationAngle;
                    return rectItem;
                }
                else if (_shapeItem is EllipseShapeItem ||
                         _shapeItem is CircleShapeItem)
                {
                    EllipseShapeItem ellItem = _shapeItem.Clone() as EllipseShapeItem;
                    //set properties based on dialog inputs
                    ellItem.Comments = generalUC1.ItemComments;
                    ellItem.FillColor = strokeFillUC1.ItemFillColor;
                    ellItem.Height = sizeUC1.ItemHeight;
                    ellItem.Width = sizeUC1.ItemWidth;
                    ellItem.Name = generalUC1.ItemName;
                    ellItem.PrintAsGraphic = generalUC1.PrintAsGraphic;
                    ellItem.StrokeColor = strokeFillUC1.ItemStrokeColor;
                    ellItem.StrokeThickness = strokeFillUC1.ItemStrokeThickness;
                    ellItem.X = positionUC1.ItemX;
                    ellItem.Y = positionUC1.ItemY;
                    ellItem.RotationAngle = sizeUC1.ItemRotationAngle;
                    return ellItem;
                }
                else if (_shapeItem is LineShapeItem)
                {
                    LineShapeItem lineItem = _shapeItem.Clone() as LineShapeItem;
                    //set properties based on dialog inputs
                    lineItem.Comments = generalUC1.ItemComments;
                    lineItem.Height = sizeUC1.ItemHeight;
                    lineItem.Width = sizeUC1.ItemWidth;
                    lineItem.Name = generalUC1.ItemName;
                    lineItem.PrintAsGraphic = generalUC1.PrintAsGraphic;
                    lineItem.StrokeColor = strokeFillUC1.ItemStrokeColor;
                    lineItem.StrokeThickness = strokeFillUC1.ItemStrokeThickness;
                    lineItem.X = positionUC1.ItemX;
                    lineItem.Y = positionUC1.ItemY;
                    return lineItem;
                }
                else
                    return null;
            }
            set
            {
                _shapeItem = value.Clone() as ShapeItem;

                //set fill & stroke
                if (_shapeItem is LineShapeItem)
                {
                    strokeFillUC1.CornerRadiusOptionVisibility = System.Windows.Visibility.Hidden;
                    strokeFillUC1.FillOptionVisibility = System.Windows.Visibility.Hidden;
                }
                else //rect or ellipse
                {
                    strokeFillUC1.ItemFillColor = ((ClosedShapeItem)_shapeItem).FillColor;
                    if (_shapeItem is EllipseShapeItem ||
                        _shapeItem is CircleShapeItem)
                    {
                        strokeFillUC1.CornerRadiusOptionVisibility = System.Windows.Visibility.Hidden;
                    }
                    else
                    {
                        //for simplicity we are going to use uniform corner radius (but you can change it if needed!)
                        strokeFillUC1.ItemCornerRadius = ((RectangleShapeItem)_shapeItem).CornerRadius.TopLeft;
                    }
                }
                strokeFillUC1.ItemStrokeColor = _shapeItem.StrokeColor;
                strokeFillUC1.ItemStrokeThickness = _shapeItem.StrokeThickness;

                //set position & size tab
                positionUC1.ItemX = _shapeItem.X;
                positionUC1.ItemY = _shapeItem.Y;
                sizeUC1.ItemWidth = _shapeItem.Width;
                sizeUC1.ItemHeight = _shapeItem.Height;
                if (_shapeItem is RectangleShapeItem)
                    sizeUC1.ItemRotationAngle = ((RectangleShapeItem)_shapeItem).RotationAngle;
                else if (_shapeItem is EllipseShapeItem || _shapeItem is CircleShapeItem)
                    sizeUC1.ItemRotationAngle = ((EllipseShapeItem)_shapeItem).RotationAngle;
                else
                    sizeUC1.RotationAngleVisibility = System.Windows.Visibility.Hidden;

                //set general tab
                generalUC1.ItemName = _shapeItem.Name;
                generalUC1.ItemComments = _shapeItem.Comments;
            }
        }

        # endregion

        #region Methods

        public ShapeItemDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion
    }
}
