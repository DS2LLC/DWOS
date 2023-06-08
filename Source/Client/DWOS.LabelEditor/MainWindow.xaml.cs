using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using Neodynamic.SDK.Printing;
using Neodynamic.Windows.ThermalLabelEditor;

namespace DWOS.LabelEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// XML tag for the Thermal Label.
        /// </summary>
        public const string XML_THERMAL_LABEL = "ThermalLabel";

        /// <summary>
        /// XML attribute for print orientation.
        /// </summary>
        public const string XML_PRINT_ORIENTATION = "PrintOrientation";

        /// <summary>
        /// Gets the list of tokens to show.
        /// </summary>
        public List<Token> Tokens { get; private set; }

        /// <summary>
        /// Gets or sets the current label orientation.
        /// </summary>
        public PrintOrientation LabelOrientation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.Tokens = new List <Token>();
        }

        /// <summary>
        /// Loads an initial set of tokens for the label.
        /// </summary>
        /// <param name="tokens"></param>
        public void LoadTokens(List <Token> tokens)
        {
            this.Tokens = tokens;
            lstTokens.ItemsSource = tokens;
        }

        /// <summary>
        /// Loads label contents.
        /// </summary>
        /// <remarks>
        /// If there is no label content, this initializes a new label.
        /// </remarks>
        /// <param name="labelContent"></param>
        public void LoadLabel(string labelContent)
        {
            if(!String.IsNullOrWhiteSpace(labelContent))
            {
                this.LabelOrientation = GetLabelOrientation(labelContent);
                var tl = ThermalLabel.CreateFromXmlTemplate(labelContent.TrimStart('?'));
                thermalLabelEditor1.LoadThermalLabel(tl);
            }
            else
                NewLabel();
        }

        /// <summary>
        /// Returns an XML representation of the current label.
        /// </summary>
        /// <returns></returns>
        public string SaveLabel()
        {
            //is there any label on the editor's surface...
            if (thermalLabelEditor1.LabelDocument != null)
            {
                using(var stream = new MemoryStream())
                {
                    thermalLabelEditor1.Save(stream);
                    var labelXML = Encoding.ASCII.GetString(stream.ToArray());
                    labelXML = labelXML.TrimStart('?');
                    labelXML = SetLabelOrientation(labelXML, this.LabelOrientation);
                    return labelXML;
                }
            }

            return null;
        }
        
        /// <summary>
        /// Generates a preview image for the current label.
        /// </summary>
        /// <returns></returns>
        public byte[] GetPreviewImage()
        {
            try
            {
                var tLabel = thermalLabelEditor1.CreateThermalLabel();

                if (tLabel == null)
                    return null;

                //create a PrintJob object
                using (var pj = new PrintJob())
                {
                    pj.ThermalLabel = tLabel; // set the ThermalLabel object
                    var ms = new MemoryStream();
                    pj.ExportToImage(ms, new ImageSettings(ImageFormat.Png) { PngIncludeDpiMetadata = true }, 300);
                    return ms.ToArray();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting preview image.");
                return null;
            }
        }

        /// <summary>
        /// Retrieves the orientation for the given label XML.
        /// </summary>
        /// <param name="labelXml"></param>
        /// <returns>
        /// The label's orientation if found; otherwise,
        /// <see cref="PrintOrientation.Portrait"/>
        /// </returns>
        public static PrintOrientation GetLabelOrientation(string labelXml)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(new StringReader(labelXml));
                var labelData = (XmlElement)xmlDoc.SelectSingleNode("//" + XML_THERMAL_LABEL);
                
                if (labelData != null)
                {
                    var value = labelData.GetAttribute(XML_PRINT_ORIENTATION);

                    if (!String.IsNullOrWhiteSpace(value))
                        return (PrintOrientation)Enum.Parse(typeof(PrintOrientation), value);
                }

                return PrintOrientation.Portrait;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting label orientation.");
                return PrintOrientation.Portrait;
            }
        }

        /// <summary>
        /// Sets the label orientation for the given label XML.
        /// </summary>
        /// <param name="labelXml"></param>
        /// <param name="orientation"></param>
        /// <returns>
        /// If successful, XML for the label with its label orientation set;
        /// otherwise, returns the input label XML
        /// </returns>
        public static string SetLabelOrientation(string labelXml, PrintOrientation orientation)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(new StringReader(labelXml));
               
                var labelData = (XmlElement)xmlDoc.SelectSingleNode("//" + XML_THERMAL_LABEL);
                if (labelData != null)
                    labelData.SetAttribute(XML_PRINT_ORIENTATION, orientation.ToString());

                return xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error setting label orientation.");
                return labelXml;
            }
        }

        private void NewLabel()
        {
            try
            {
                var labelSetup = new LabelDoc { LabelUnit = UnitType.Inch, LabelWidth = 4, LabelHeight = 3 };
                var tLabel = new ThermalLabel { Width = labelSetup.LabelWidth, Height = labelSetup.LabelHeight, UnitType = labelSetup.LabelUnit };
                thermalLabelEditor1.LoadThermalLabel(tLabel);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on new label.");
            }
        }

        private void AddToken(Token token)
        {
            try
            {
                if (thermalLabelEditor1.LabelDocument != null)
                {
                    if (token.TokenType == TokenType.Text)
                    {
                        //Set the ActiveTool to Text
                        thermalLabelEditor1.ActiveTool = EditorTool.Text;

                        //Create and set the ActiveToolItem i.e. a TextItem
                        var txtItem = new TextItem { Text = token.SampleValue, Name = token.ID, Sizing = TextSizing.Stretch };
                        txtItem.Font.Name = "Tahoma";
                        txtItem.Font.Size = 10;
                        txtItem.TextPadding = new FrameThickness(0.03);
                        txtItem.ConvertToUnit(thermalLabelEditor1.LabelDocument.UnitType);

                        thermalLabelEditor1.ActiveToolItem = txtItem;
                    }
                    else if (token.TokenType == TokenType.Barcode)
                    {
                        //Set the ActiveTool to Barcode
                        thermalLabelEditor1.ActiveTool = EditorTool.Barcode;

                        var bcItem = new BarcodeItem
                        {
                            Name = token.ID,
                            Symbology = BarcodeSymbology.Code128,
                            AddChecksum = false,
                            Code = token.SampleValue,
                            Sizing = BarcodeSizing.FitProportional
                        };

                        bcItem.Font.Name = Font.NativePrinterFontA;
                        bcItem.Font.Size = 5;
                        bcItem.BarcodeAlignment = BarcodeAlignment.MiddleCenter;

                        bcItem.ConvertToUnit(thermalLabelEditor1.LabelDocument.UnitType);

                        thermalLabelEditor1.ActiveToolItem = bcItem;

                    } else if (token.TokenType == TokenType.Image)
                    {
                        //Set the ActiveTool to Barcode
                        thermalLabelEditor1.ActiveTool = EditorTool.Image;
                        var imItem = new ImageItem
                        {
                            Name = token.ID,

                           // IsGrayscaleOrBlackWhite = false,
                            //Height = 100,
                           // Width =100,
                            //SourceBinary = Encoding.ASCII.GetBytes(token.SampleValue),
                             SourceBase64 = token.SampleValue,

                        };
                        imItem.ConvertToUnit(thermalLabelEditor1.LabelDocument.UnitType);
                        thermalLabelEditor1.ActiveToolItem = imItem;

                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error add token.");
            }
        }

        private void AlignLeft()
        {
            try
            {
                if (thermalLabelEditor1.CurrentSelection is MultipleSelectionItem)
                {
                    var selections = thermalLabelEditor1.CurrentSelection as MultipleSelectionItem;
                    var minX = selections.Items.OfType<TextItem>().Min(i => i.X);

                    foreach (var item in selections.Items.OfType<TextItem>())
                        item.X = minX;

                    thermalLabelEditor1.UpdateSelectionItemsProperties();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on align left.");
            }
        }

        private void AlignRight()
        {
            try
            {
                if (thermalLabelEditor1.CurrentSelection is MultipleSelectionItem)
                {
                    var selections = thermalLabelEditor1.CurrentSelection as MultipleSelectionItem;
                    var maxX = selections.Items.OfType<TextItem>().Max(i => i.X + i.Width);

                    foreach (var item in selections.Items.OfType<TextItem>())
                        item.X = maxX - item.Width;

                    thermalLabelEditor1.UpdateSelectionItemsProperties();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on align right.");
            }
        }

        private void AlignTop()
        {
            try
            {
                if (thermalLabelEditor1.CurrentSelection is MultipleSelectionItem)
                {
                    var selections = thermalLabelEditor1.CurrentSelection as MultipleSelectionItem;
                    var minY = selections.Items.OfType<TextItem>().Min(i => i.Y);

                    foreach (var item in selections.Items.OfType<TextItem>())
                        item.Y = minY;

                    thermalLabelEditor1.UpdateSelectionItemsProperties();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on align top.");
            }
        }

        private void AlignBottom()
        {
            try
            {
                if (thermalLabelEditor1.CurrentSelection is MultipleSelectionItem)
                {
                    var selections = thermalLabelEditor1.CurrentSelection as MultipleSelectionItem;
                    var minY = selections.Items.OfType<TextItem>().Min(i => i.Y + i.Height);

                    foreach (var item in selections.Items.OfType<TextItem>())
                        item.Y = minY - item.Height;

                    thermalLabelEditor1.UpdateSelectionItemsProperties();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on align bottom.");
            }
        }

        private void AlignCenter()
        {
            try
            {
                if (thermalLabelEditor1.CurrentSelection is MultipleSelectionItem)
                {
                    var selections = thermalLabelEditor1.CurrentSelection as MultipleSelectionItem;
                    var avgX = selections.Items.OfType<TextItem>().Average(i => i.X + (i.Width / 2));

                    foreach (var item in selections.Items.OfType<TextItem>())
                        item.X = item.X + (avgX - (item.X + (item.Width / 2)));

                    thermalLabelEditor1.UpdateSelectionItemsProperties();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on align center.");
            }
        }

        private void AlignMiddle()
        {
            try
            {
                if (thermalLabelEditor1.CurrentSelection is MultipleSelectionItem)
                {
                    var selections = thermalLabelEditor1.CurrentSelection as MultipleSelectionItem;
                    var avgY = selections.Items.OfType<TextItem>().Average(i => i.Y + (i.Height / 2));

                    foreach (var item in selections.Items.OfType<TextItem>())
                        item.Y = item.Y + (avgY - (item.Y + (item.Height / 2)));

                    thermalLabelEditor1.UpdateSelectionItemsProperties();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on align middle.");
            }
        }

        private void tbbPointer_Click(object sender, RoutedEventArgs e)
        {
            //Set the ActiveTool to Pointer
            thermalLabelEditor1.ActiveTool = EditorTool.Pointer;
        }

        private void tbbRect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //is there any label on the editor's surface...
                if (thermalLabelEditor1.LabelDocument != null)
                {
                    //Set the ActiveTool to Rectangle
                    thermalLabelEditor1.ActiveTool = EditorTool.Rectangle;

                    //Create and set the ActiveToolItem i.e. a RectangleShapeItem
                    RectangleShapeItem rectItem = new RectangleShapeItem();
                    rectItem.ConvertToUnit(thermalLabelEditor1.LabelDocument.UnitType);

                    thermalLabelEditor1.ActiveToolItem = rectItem;
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on rect click.");
            }
        }

        private void tbbEllipse_Click(object sender, RoutedEventArgs e)
        {
            //is there any label on the editor's surface...
            if (thermalLabelEditor1.LabelDocument != null)
            {
                //Set the ActiveTool to Ellipse
                thermalLabelEditor1.ActiveTool = EditorTool.Ellipse;

                //Create and set the ActiveToolItem i.e. a EllipseShapeItem
                EllipseShapeItem ellItem = new EllipseShapeItem();
                ellItem.ConvertToUnit(thermalLabelEditor1.LabelDocument.UnitType);

                thermalLabelEditor1.ActiveToolItem = ellItem;
            }
        }

        private void tbbLine_Click(object sender, RoutedEventArgs e)
        {
            //is there any label on the editor's surface...
            if (thermalLabelEditor1.LabelDocument != null)
            {
                //Set the ActiveTool to Line
                thermalLabelEditor1.ActiveTool = EditorTool.Line;

                //Create and set the ActiveToolItem i.e. a LineShapeItem
                LineShapeItem lineItem = new LineShapeItem();
                lineItem.ConvertToUnit(thermalLabelEditor1.LabelDocument.UnitType);

                thermalLabelEditor1.ActiveToolItem = lineItem;
            }
        }

        private void tbbText_Click(object sender, RoutedEventArgs e)
        {
            //is there any label on the editor's surface...
            if (thermalLabelEditor1.LabelDocument != null)
            {
                //Set the ActiveTool to Text
                thermalLabelEditor1.ActiveTool = EditorTool.Text;

                //Create and set the ActiveToolItem i.e. a TextItem
                var txtItem = new TextItem();
                txtItem.Font.Name = Font.NativePrinterFontA;
                txtItem.Font.Size = 10;
                txtItem.Text = "Type here";
                
                txtItem.ConvertToUnit(thermalLabelEditor1.LabelDocument.UnitType);

                thermalLabelEditor1.ActiveToolItem = txtItem;
            }

        }

        private void tbbImage_Click(object sender, RoutedEventArgs e)
        {
            //is there any label on the editor's surface...
            if (thermalLabelEditor1.LabelDocument != null)
            {
                //Set the ActiveTool to Image
                thermalLabelEditor1.ActiveTool = EditorTool.Image;

                //Create and set the ActiveToolItem i.e. an ImageItem
                ImageItem imgItem = new ImageItem();
                //HERE YOU COULD OPEN A FILE DIALOG TO THE USE FOR SELECTING SOME IMAGE FILE                
                //OR PICK ONE FROM A 'GALLERY', ETC.
                //imgItem.SourceFile = @"C:\Pictures\glass.gif";
                                
                thermalLabelEditor1.ActiveToolItem = imgItem;
            }
        }

        private void tbbBarcode_Click(object sender, RoutedEventArgs e)
        {
            //is there any label on the editor's surface...
            if (thermalLabelEditor1.LabelDocument != null)
            {
                //Set the ActiveTool to Barcode
                thermalLabelEditor1.ActiveTool = EditorTool.Barcode;

                //Create and set the ActiveToolItem i.e. a BarcodeItem
                //HERE YOU COULD CHANGE THE DEFAULT BARCODE TO BE DISPLAYED
                //OR OPEN A BARCODE DIALOG FOR SETTINGS, ETC.
                BarcodeItem bcItem = new BarcodeItem
                {
                    Symbology = BarcodeSymbology.Code128,
                    AddChecksum = false,
                    Sizing = BarcodeSizing.FitProportional
                };
                bcItem.Code = BarcodeItemUtils.GenerateSampleCode(bcItem.Symbology);
                bcItem.Font.Name = Font.NativePrinterFontA;
                bcItem.Font.Size = 5;
                bcItem.BarcodeAlignment = BarcodeAlignment.MiddleCenter;

                bcItem.ConvertToUnit(thermalLabelEditor1.LabelDocument.UnitType);

                thermalLabelEditor1.ActiveToolItem = bcItem;
            }
        }
        
        private void tbbZoom100_Click(object sender, RoutedEventArgs e)
        {
            //Set up zoom to 100%
            this.sldZoom.Value = 100;
        }

        private void thermalLabelEditor1_SelectionChanged(object sender, EventArgs e)
        {
            //The items selection has changed...

            //For simplicity, we will not show any dialog if there's a multiple item selection
            tbbProp.IsEnabled = false;
            if (thermalLabelEditor1.CurrentSelection != null)
            {
                tbbProp.IsEnabled = !(thermalLabelEditor1.CurrentSelection is MultipleSelectionItem);
            }            
        }

        private void thermalLabelEditor1_SelectionAreaChanged(object sender, EventArgs e)
        {

            //Show in the 'status bar' the dimensions of the selected area
            //we're going to format it including the unit

            Rect selArea = thermalLabelEditor1.CurrentSelectionArea;

            if (selArea.Width > 0 && selArea.Height > 0)
            {
                string unitDescription;
                if (thermalLabelEditor1.LabelDocument.UnitType == UnitType.Inch)
                    unitDescription = "in";
                else if (thermalLabelEditor1.LabelDocument.UnitType == UnitType.DotsPerInch)
                    unitDescription = "dpi";
                else if (thermalLabelEditor1.LabelDocument.UnitType == UnitType.Pica)
                    unitDescription = "pc";
                else if (thermalLabelEditor1.LabelDocument.UnitType == UnitType.Point)
                    unitDescription = "pt";
                else
                    unitDescription = thermalLabelEditor1.LabelDocument.UnitType.ToString().ToLower();


                object[] data = new object[]{unitDescription,
                                         selArea.X,
                                         selArea.Y,
                                         selArea.Width,
                                         selArea.Height};

                string decimals = "0".PadRight(thermalLabelEditor1.LabelDocument.NumOfFractionalDigits, '0');

                txtSelectionInfo.Text = string.Format("X: {1:0." + decimals + "}{0}   Y: {2:0." + decimals + "}{0}   Width: {3:0." + decimals + "}{0}   Height: {4:0." + decimals + "}{0}", data);
            }
            else
            {
                txtSelectionInfo.Text = "";
            }
        }
        
        private void menuLabelSetup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //is there any label on the editor's surface...
                if (thermalLabelEditor1.LabelDocument != null)
                {
                    //Open dialog for label document setup
                    var labelSetup = new LabelDoc();
                    labelSetup.LabelUnit = thermalLabelEditor1.LabelDocument.UnitType;
                    labelSetup.LabelWidth = thermalLabelEditor1.LabelDocument.Width;
                    labelSetup.LabelHeight = thermalLabelEditor1.LabelDocument.Height;
                    labelSetup.LabelOrientation = this.LabelOrientation;
                    labelSetup.Owner = this;

                    if (labelSetup.ShowDialog() == true)
                    {
                        //Incoke UpdateLabelDocument method for updating the label document inside the editor
                        thermalLabelEditor1.UpdateLabelDocument(labelSetup.LabelUnit, labelSetup.LabelWidth, labelSetup.LabelHeight, thermalLabelEditor1.LabelDocument.NumOfFractionalDigits);
                        this.LabelOrientation = labelSetup.LabelOrientation;
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error during label setup.");
            }
        }

        private void editorContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            try
            {
                //Disable/Enable context menu items based on the selected items on the editor's surface
                Item selItem = thermalLabelEditor1.CurrentSelection;

                //Font option is available for TextItem and BarcodeItem only
                if (selItem is TextItem ||
                    selItem is BarcodeItem)
                {
                    cmFont.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    cmFont.Visibility = System.Windows.Visibility.Collapsed;
                }
                Sep1.Visibility = cmFont.Visibility;

                //update "format" name based on the type of the selected Item
                cmFormat.Visibility = System.Windows.Visibility.Visible;
                if (selItem is TextItem)
                    cmFormat.Header = "Format Text...";
                else if (selItem is BarcodeItem)
                    cmFormat.Header = "Format Barcode...";
                else if (selItem is RectangleShapeItem)
                    cmFormat.Header = "Format Rectangle...";
                else if (selItem is EllipseShapeItem)
                    cmFormat.Header = "Format Ellipse...";
                else if (selItem is LineShapeItem)
                    cmFormat.Header = "Format Line...";
                else if (selItem is ImageItem)
                    cmFormat.Header = "Format Picture...";
                else
                    cmFormat.Visibility = System.Windows.Visibility.Collapsed;

                Sep3.Visibility = cmFormat.Visibility;

            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on label context menu.");
            }
        }
        
        private void cmFont_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //get current selection
                Item selItem = thermalLabelEditor1.CurrentSelection;
                //show Font dialog only for TextItem or BarcodeItem objects
                if (selItem is TextItem ||
                    selItem is BarcodeItem)
                {
                    //Get current Font of selected item
                    Font itemFont;
                    if (selItem is TextItem)
                        itemFont = ((TextItem)selItem).Font;
                    else
                        itemFont = ((BarcodeItem)selItem).Font;

                    //create and open a FontDialog
                    FontDialog fontDialog = new FontDialog();
                    fontDialog.Font = itemFont;
                    fontDialog.Owner = this;

                    if (fontDialog.ShowDialog() == true)
                    {
                        //get new Font settings
                        itemFont = fontDialog.Font;

                        //update font settings on item
                        if (selItem is TextItem)
                        {
                            ((TextItem)selItem).Font.UpdateFrom(itemFont);
                        }
                        else
                        {
                            ((BarcodeItem)selItem).Font.UpdateFrom(itemFont);
                        }

                        //update editor's surface
                        thermalLabelEditor1.UpdateSelectionItemsProperties();
                    }

                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on label font.");
            }
            

        }

        private void cmFormat_Click(object sender, RoutedEventArgs e)
        {
            //get current selection
            Item selItem = thermalLabelEditor1.CurrentSelection;

            if (selItem is ImageItem)
            { 
                //create and open ImageItemDialog
                ImageItemDialog imgItemDialog = new ImageItemDialog();
                imgItemDialog.Owner = this;
                //set current ImageItem to dialog
                ImageItem curImgItem = selItem as ImageItem;
                imgItemDialog.ImageItem = curImgItem;
                if (imgItemDialog.ShowDialog() == true)
                { 
                    //update ImageItem based on dialog result
                    curImgItem.UpdateFrom(imgItemDialog.ImageItem);

                    //update editor's surface
                    thermalLabelEditor1.UpdateSelectionItemsProperties();
                }
            }
            else if (selItem is ShapeItem)
            {
                //create and open ShapeItemDialog
                ShapeItemDialog shapeItemDialog = new ShapeItemDialog();
                shapeItemDialog.Owner = this;                
                //set current ShapeItem to dialog
                ShapeItem curShapeItem = selItem as ShapeItem;
                shapeItemDialog.ShapeItem = curShapeItem;
                //customize dialog title
                shapeItemDialog.Title = curShapeItem.ToString().Replace("Neodynamic.SDK.Printing.", "").Replace("ShapeItem", "");

                if (shapeItemDialog.ShowDialog() == true)
                {
                    //update ShapeItem based on dialog result
                    curShapeItem.UpdateFrom(shapeItemDialog.ShapeItem);

                    //update editor's surface
                    thermalLabelEditor1.UpdateSelectionItemsProperties();
                }
            }
            else if (selItem is TextItem)
            {
                //create and open TextItemDialog
                TextItemDialog textItemDialog = new TextItemDialog();
                textItemDialog.Owner = this;
                //set current TextItem to dialog
                TextItem curTextItem = selItem as TextItem;
                textItemDialog.TextItem = curTextItem;

                if (textItemDialog.ShowDialog() == true)
                {
                    //update TextItem based on dialog result
                    curTextItem.UpdateFrom(textItemDialog.TextItem);

                    //update editor's surface
                    thermalLabelEditor1.UpdateSelectionItemsProperties();
                }
            }
            else if (selItem is BarcodeItem)
            {
                //create and open BarcodeItemDialog
                BarcodeItemDialog bcItemDialog = new BarcodeItemDialog();
                bcItemDialog.Owner = this;
                //set current BarcodeItem to dialog
                BarcodeItem curBarcodeItem = selItem as BarcodeItem;
                bcItemDialog.Tokens = this.Tokens;
                bcItemDialog.BarcodeItem = curBarcodeItem;

                if (bcItemDialog.ShowDialog() == true)
                {
                    //update BarcodeItem based on dialog result
                    curBarcodeItem.UpdateFrom(bcItemDialog.BarcodeItem);

                    //update editor's surface
                    thermalLabelEditor1.UpdateSelectionItemsProperties();
                }
            }
        }

        private void menuPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Create the ThermalLabel obj from the editor
                ThermalLabel tLabel = thermalLabelEditor1.CreateThermalLabel();

                if (tLabel != null)
                {
                    //Display Print Job dialog...           
                    var frmPrintJob = new PrintJobDialog { Owner = this };
                    frmPrintJob.PrintOrientation = this.LabelOrientation;

                    if (frmPrintJob.ShowDialog() ?? false)
                    {
                        if (IsPrinterValid(frmPrintJob.PrinterSettings))
                        {
                            //create a PrintJob object
                            using (var pj = new WindowsPrintJob(frmPrintJob.PrinterSettings))
                            {
                                pj.Copies = frmPrintJob.Copies; // set copies
                                pj.PrintOrientation = frmPrintJob.PrintOrientation; //set orientation
                                pj.ThermalLabel = tLabel; // set the ThermalLabel object
                                pj.Print(); // print the ThermalLabel object                        
                            }
                        }
                        else
                        {
                            MessageBox.Show("Could not access the selected printer.\nPlease try printing again.");
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                if (exc is System.Net.Sockets.SocketException)
                {
                    MessageBox.Show("Could not connect to the remote printer.");
                    NLog.LogManager.GetCurrentClassLogger().Debug(exc, "Network communication error while printing label.");
                }
                else if (exc.Message.StartsWith("Printer not found on parallel port name:"))
                {
                    // Thrown by Neodynamic SDK
                    MessageBox.Show("Could not find the selected parallel port printer.");
                    NLog.LogManager.GetCurrentClassLogger().Debug(exc, "'Printer not found' error while printing label.");
                }
                else
                {
                    NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on label print.");
                }
            }
        }

        private bool IsPrinterValid(PrinterSettings printerSettings)
        {
            bool printerValid = true;

            if (printerSettings.Communication.CommunicationType == CommunicationType.USB)
            {
                //The Neodynamic SDK can "print" to a blank USB printer without throwing an exception.
                printerValid = !string.IsNullOrEmpty(printerSettings.PrinterName);
            }
            else if (printerSettings.Communication.CommunicationType == CommunicationType.Serial)
            {
                // Empty serial port name can throw a ArgumentNullException when printing.
                printerValid = !string.IsNullOrEmpty(printerSettings.Communication.SerialPortName);
            }

            return printerValid;
        }

        private void menuPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Create the ThermalLabel obj from the editor
                ThermalLabel tLabel = thermalLabelEditor1.CreateThermalLabel();

                if (tLabel != null)
                {
                    //open save dialog...
                    var dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.DefaultExt = ".pdf";
                    dlg.Filter = "Adobe PDF (.pdf)|*.pdf";
                    if (dlg.ShowDialog() == true)
                    {
                        try
                        {
                            //export ThermalLabel to PDF
                            using (PrintJob pj = new PrintJob())
                            {
                                pj.ThermalLabel = tLabel;
                                pj.ExportToPdf(dlg.FileName, 96);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on label PDF");
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on label ok.");
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.Source is FrameworkElement)
                {
                    var element = e.Source as FrameworkElement;
                    if (element.DataContext is Token)
                    {
                        AddToken(element.DataContext as Token);
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on adding token.");
            }
        }

        private void tbbAlignLeft_Click(object sender, RoutedEventArgs e)
        {
            AlignLeft();
        }

        private void tbbAligRight_Click(object sender, RoutedEventArgs e) { AlignRight(); }

        private void tbbAligTop_Click(object sender, RoutedEventArgs e) { AlignTop(); }

        private void tbbAligBottom_Click(object sender, RoutedEventArgs e) { AlignBottom(); }

        private void tbbAligCenter_Click(object sender, RoutedEventArgs e) { AlignCenter(); }

        private void tbbAligMiddle_Click(object sender, RoutedEventArgs e) { AlignMiddle(); }
    }

}
