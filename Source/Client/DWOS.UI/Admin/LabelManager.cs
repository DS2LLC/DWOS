using DWOS.Data.Datasets;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;

namespace DWOS.UI.Admin
{
    public partial class LabelManager: DataEditorBase
    {
        #region Fields

        private readonly Lazy<bool> _useProductClassLazy = new Lazy<bool>(
            () => FieldUtilities.IsFieldEnabled("Order", "Product Class"));

        #endregion

        #region Methods

        public LabelManager()
        {
            this.InitializeComponent();
        }

        private void LoadData()
        {
            try
            {
                using (new UsingDataSetLoad(dsLabels))
                {
                    taLabelType.FillNoData(dsLabels.LabelType);
                    taLabelCustomerSummary.Fill(dsLabels.LabelCustomerSummary);
                    taLabelMedia.FillByLabelTypeNoMedia(dsLabels.LabelMedia);
                    taLabelMedia.FillByLabelsNoMedia(dsLabels.LabelMedia);
                    taLabelMedia.FillByProductClassLabelsNoMedia(dsLabels.LabelMedia);
                    taLabels.FillNoData(dsLabels.Labels);
                    taProductClassLabels.Fill(dsLabels.ProductClassLabels);

                    taProductClass.Fill(dsLabels.ProductClass);
                }

                pnlLabelType.LoadData(dsLabels);
                base.AddDataPanel(pnlLabelType);

                pnlLabelInfo.LoadData(dsLabels);
                base.AddDataPanel(pnlLabelInfo);

                pnlLabelProductClass.LoadData(dsLabels);
                AddDataPanel(pnlLabelProductClass);

                dsLabels.Labels.LabelsRowChanged += Labels_LabelsRowChanged;
                dsLabels.ProductClassLabels.ProductClassLabelsRowChanged += ProductClassLabels_ProductClassLabelsRowChanged;
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading label data.", exc);
            }
        }

        private void LoadTOC()
        {
            using(new UsingTreeLoad(tvwTOC))
            {
                tvwTOC.Nodes.Clear();

                UltraTreeNode rootNode = new LabelsRootNode();
                tvwTOC.Nodes.Add(rootNode);
                rootNode.Expanded = true;
                rootNode.Nodes.Override.SortComparer = new LabelCategoryComparer();

                var workOrderLabelsNode = new LabelCategoryNode(LabelFactory.LabelCategory.WO);
                workOrderLabelsNode.Nodes.Override.SortComparer = new LabelTypeComparer();
                rootNode.Nodes.Add(workOrderLabelsNode);

                var containerLabelsNode = new LabelCategoryNode(LabelFactory.LabelCategory.Container);
                containerLabelsNode.Nodes.Override.SortComparer = new LabelTypeComparer();
                rootNode.Nodes.Add(containerLabelsNode);

                var shippingLabelsNode = new LabelCategoryNode(LabelFactory.LabelCategory.Shipping);
                shippingLabelsNode.Nodes.Override.SortComparer = new LabelTypeComparer();
                rootNode.Nodes.Add(shippingLabelsNode);
                
                //add to each label to correct parent
                foreach (var labelTypeRow in dsLabels.LabelType)
                {
                    var labelTypeNode = new LabelTypeNode(labelTypeRow);
                    var nodeCategory = LabelFactory.GetCategory(labelTypeNode.LabelType);

                    if (nodeCategory == LabelFactory.LabelCategory.WO)
                    {
                        workOrderLabelsNode.Nodes.Add(labelTypeNode);
                    }
                    else if (nodeCategory == LabelFactory.LabelCategory.Container)
                    {
                        containerLabelsNode.Nodes.Add(labelTypeNode);
                    }
                    else if (nodeCategory == LabelFactory.LabelCategory.Shipping)
                    {
                        shippingLabelsNode.Nodes.Add(labelTypeNode);
                    }

                    foreach (var label in labelTypeRow.GetLabelsRows())
                        labelTypeNode.Nodes.Add(new LabelNode(label));

                    if (_useProductClassLazy.Value)
                    {
                        foreach (var label in labelTypeRow.GetProductClassLabelsRows())
                        {
                            labelTypeNode.Nodes.Add(new ProductClassLabelNode(label));
                        }
                    }
                }
            }
        }

        protected override void ReloadTOC()
        {
            this.LoadTOC();
        }

        protected override bool SaveData()
        {
            try
            {
                base.EndAllEdits();

                this.taManager.UpdateAll(this.dsLabels);

                return true;
            }
            catch(Exception exc)
            {
                _log.Warn("DataSet Errors: " + dsLabels.GetDataErrors());
                ErrorMessageBox.ShowDialog("Error saving data.", exc);
                return false;
            }
        }

        private void LoadCommands()
        {
            var addCustomerCmd = base.Commands.AddCommand("Add", new AddCustomerLabelCommand(toolbarManager.Tools["Add"], tvwTOC)) as AddCustomerLabelCommand;

            if (addCustomerCmd != null)
            {
                addCustomerCmd.AddNode += (s, e) => AddNode(AddCustomerLabel);
            }

            if (_useProductClassLazy.Value)
            {
                var addProductClassCmd = Commands.AddCommand("AddProductClass", new AddProductClassLabelCommand(toolbarManager.Tools["AddProductClass"], tvwTOC)) as AddProductClassLabelCommand;

                if (addProductClassCmd != null)
                {
                    addProductClassCmd.AddNode += (s, e) => AddNode(AddProductClassLabel);
                }
            }
            else
            {
                toolbarManager.Tools["AddProductClass"].SharedProps.Visible = false;
            }

            var edit = base.Commands.AddCommand("Edit", new EditLabelCommand(toolbarManager.Tools["Edit"], tvwTOC)) as EditLabelCommand;
            edit.EditLabel += (s, e) => EditLabel();

            var dc = base.Commands.AddCommand("Delete", new DeleteCommand(toolbarManager.Tools["Delete"], tvwTOC, this)) as DeleteCommand;

            base.Commands.AddCommand("Export", new ExportCommand(toolbarManager.Tools["Export"], tvwTOC, this));
            base.Commands.AddCommand("Import", new ImportCommand(toolbarManager.Tools["Import"], tvwTOC, this));
        }

        private void EditLabel()
        {
            try
            {
                var win = new LabelEditor.MainWindow();

                var labelNode = tvwTOC.SelectedNode<LabelNode>();
                var labelTypeNode = tvwTOC.SelectedNode<LabelTypeNode>();
                var labelProductClassNode = tvwTOC.SelectedNode<ProductClassLabelNode>();
                LabelTypeNode tokenNode = null;
                
                if (labelNode != null)
                {
                    tokenNode = labelNode.Parent as LabelTypeNode;

                    if (labelNode.DataRow.IsDataNull())
                    {
                        var labelData = taLabels.GetLabelData(labelNode.DataRow.LabelID);

                        if (labelNode.DataRow.RowState == DataRowState.Unchanged)
                        {
                            labelNode.DataRow.Data = labelData;
                            labelNode.DataRow.AcceptChanges();
                        }
                        else
                            labelNode.DataRow.Data = labelData;
                    }

                    win.LoadLabel(labelNode.DataRow.IsDataNull() ? null : labelNode.DataRow.Data);
                }
                else if (labelProductClassNode != null)
                {
                    tokenNode = labelProductClassNode.Parent as LabelTypeNode;

                    if (labelProductClassNode.DataRow.IsDataNull())
                    {
                        var labelData = taProductClassLabels.GetLabelData(labelProductClassNode.DataRow.ProductClassLabelID);

                        if (labelProductClassNode.DataRow.RowState == DataRowState.Unchanged)
                        {
                            labelProductClassNode.DataRow.Data = labelData;
                            labelProductClassNode.DataRow.AcceptChanges();
                        }
                        else
                            labelProductClassNode.DataRow.Data = labelData;
                    }

                    win.LoadLabel(labelProductClassNode.DataRow.IsDataNull() ? null : labelProductClassNode.DataRow.Data);
                }
                else if (labelTypeNode != null)
                {
                    tokenNode = labelTypeNode;

                    if (labelTypeNode.DataRow.IsDataNull())
                    {
                        var labelData = taLabelType.GetLabelData(labelTypeNode.DataRow.LabelTypeID);
                        var isOriginal = labelTypeNode.DataRow.RowState == DataRowState.Unchanged && labelData != null;

                        if(labelData == null)
                        {
                            Neodynamic.SDK.Printing.ThermalLabel thermalLabel = null;
                            
                            if (labelTypeNode.DataRow.LabelTypeID == (int)LabelFactory.LabelType.ShippingPackage)
                                thermalLabel = LabelFactory.GenerateDefaultShippingPackageLabel(1000, 1, "UPS", DateTime.Now, "My Customer", "Jon Doe");
                            else if (labelTypeNode.DataRow.LabelTypeID == (int)LabelFactory.LabelType.ShippingOrder)
                                thermalLabel = LabelFactory.GenerateDefaultShippingOrderLabel(1000, "Normal", "My Customer", "CUSTWOABC", "17P-663499", 12, "Jon Doe");

                            if(thermalLabel != null)
                               labelData = thermalLabel.GetXmlTemplate();
                        }

                        labelTypeNode.DataRow.Data = labelData;

                        //if is original data just lazy loaded then accept changes since we didnt change anything
                        if (isOriginal)
                            labelTypeNode.DataRow.AcceptChanges();
                    }

                    win.LoadLabel(labelTypeNode.DataRow.IsDataNull() ? null : labelTypeNode.DataRow.Data);
                }

                //load tokens
                var labelType = (LabelFactory.LabelType)tokenNode.DataRow.LabelTypeID;
                var customerID = labelNode == null || labelNode.DataRow.IsCustomerIDNull() ? -1 : labelNode.DataRow.CustomerID;
                win.LoadTokens(LabelFactory.GetTokens(labelType, customerID));

                //show dialog
                if (win.ShowDialog().GetValueOrDefault())
                {
                    var labelText = win.SaveLabel();

                    if (labelNode != null && labelText != null)
                    {
                        labelNode.DataRow.Data = labelText;
                        labelNode.DataRow.Version = labelNode.DataRow.Version + 1;

                        var imgBytes = win.GetPreviewImage();

                        if (imgBytes != null)
                        {
                            var name = labelNode.Text;
                            if (labelNode.Text.Length > 49) 
                                name = labelNode.Text.Substring(0, 49);

                            labelNode.DataRow.LabelMediaRow = dsLabels.LabelMedia.AddLabelMediaRow(name, "Label.jpg", "jpg", imgBytes);
                        }

                        LoadNode(labelNode);
                    }
                    else if (labelProductClassNode != null && labelText != null)
                    {
                        labelProductClassNode.DataRow.Data = labelText;
                        labelProductClassNode.DataRow.Version = labelProductClassNode.DataRow.Version + 1;

                        var imgBytes = win.GetPreviewImage();

                        if (imgBytes != null)
                        {
                            var name = labelProductClassNode.Text;
                            if (labelProductClassNode.Text.Length > 49) 
                                name = labelProductClassNode.Text.Substring(0, 49);

                            labelProductClassNode.DataRow.LabelMediaRow = dsLabels.LabelMedia.AddLabelMediaRow(name, "Label.jpg", "jpg", imgBytes);
                        }

                        LoadNode(labelProductClassNode);
                    }
                    else if (labelTypeNode != null && labelText != null)
                    {
                        labelTypeNode.DataRow.Data = labelText;
                        labelTypeNode.DataRow.Version = labelTypeNode.DataRow.Version + 1;

                        var imgBytes = win.GetPreviewImage();

                        if (imgBytes != null)
                        {
                            var name = labelTypeNode.Text;
                            if (labelTypeNode.Text.Length > 49)
                                name = labelTypeNode.Text.Substring(0, 49);

                            labelTypeNode.DataRow.LabelMediaRow = dsLabels.LabelMedia.AddLabelMediaRow(name, "Label.jpg", "jpg", imgBytes);
                        }

                        LoadNode(labelTypeNode);
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading label manager editor.");
            }
        }

        private void AddNode(Action<UltraTreeNode> addFunc)
        {
            var selectedNode = tvwTOC.SelectedNode<UltraTreeNode>();

            if(IsValidControls())
            {
                _validators.Enabled = false;
                addFunc?.Invoke(selectedNode);
                _validators.Enabled = true;
            }
        }

        private void AddCustomerLabel(UltraTreeNode pn)
        {
            _log.Info("Adding a customer label node.");

            //if on label type node then cool, else find first one.
            var labelTypeNode = pn as LabelTypeNode ?? tvwTOC.FindNode(n => n is LabelTypeNode) as LabelTypeNode;

            if (labelTypeNode != null)
            {
                var labelRow = pnlLabelInfo.Add(labelTypeNode.DataRow.LabelTypeID);

                //if data for label type not loaded then load it
                if (labelTypeNode.DataRow.IsDataNull())
                {
                    var labelData = taLabelType.GetLabelData(labelTypeNode.DataRow.LabelTypeID);
                    if (labelTypeNode.DataRow.RowState == DataRowState.Unchanged)
                    {
                        labelTypeNode.DataRow.Data = labelData;
                        labelTypeNode.DataRow.AcceptChanges();
                    }
                    else
                        labelTypeNode.DataRow.Data = labelData;
                }

                //copy the label types info into this new node.
                if (!labelTypeNode.DataRow.IsDataNull())
                {
                    labelRow.Data = labelTypeNode.DataRow.Data;
                    if (!labelTypeNode.DataRow.IsMediaIDNull())
                        labelRow.MediaID = labelTypeNode.DataRow.MediaID;
                    
                    labelRow.EndEdit();
                }

                var cn = new LabelNode(labelRow);
                labelTypeNode.Nodes.Add(cn);
                cn.Select();
            }
        }

        private void AddProductClassLabel(UltraTreeNode pn)
        {
            _log.Info("Adding a product class label node.");

            //if on label type node then cool, else find first one.
            var labelTypeNode = pn as LabelTypeNode ?? tvwTOC.FindNode(n => n is LabelTypeNode) as LabelTypeNode;

            if (labelTypeNode == null)
            {
                return;
            }

            var labelRow = pnlLabelProductClass.Add(labelTypeNode.DataRow.LabelTypeID);

            //if data for label type not loaded then load it
            if (labelTypeNode.DataRow.IsDataNull())
            {
                var labelData = taLabelType.GetLabelData(labelTypeNode.DataRow.LabelTypeID);
                if (labelTypeNode.DataRow.RowState == DataRowState.Unchanged)
                {
                    labelTypeNode.DataRow.Data = labelData;
                    labelTypeNode.DataRow.AcceptChanges();
                }
                else
                    labelTypeNode.DataRow.Data = labelData;
            }

            //copy the label types info into this new node.
            if (!labelTypeNode.DataRow.IsDataNull())
            {
                labelRow.Data = labelTypeNode.DataRow.Data;
                if (!labelTypeNode.DataRow.IsMediaIDNull())
                    labelRow.MediaID = labelTypeNode.DataRow.MediaID;

                labelRow.EndEdit();
            }

            var cn = new ProductClassLabelNode(labelRow);
            labelTypeNode.Nodes.Add(cn);
            cn.Select();
        }

        protected override void LoadNode(UltraTreeNode node)
        {
            DataPanel curDataPanel  = null;
            string dataID           = null;

            if (node is LabelNode)
            {
                var prNode = (LabelNode)node;
                prNode.LoadData(this);
                curDataPanel = this.pnlLabelInfo;
                dataID = prNode.ID;
            }
            else if (node is LabelTypeNode)
            {
                var prNode = (LabelTypeNode)node;
                curDataPanel = this.pnlLabelType;
                dataID = prNode.ID;
            }
            else if (node is ProductClassLabelNode pcNode)
            {
                pcNode.LoadData(this);
                curDataPanel = pnlLabelProductClass;
                dataID = pcNode.ID;
            }

            curDataPanel?.MoveToRecord(dataID);
            DisplayPanel(curDataPanel);
        }

        protected override void SaveSelectedNode()
        {
        }
        
        #endregion

        #region Events

        private void Processes_Load(object sender, EventArgs e)
        {
            _loadingData = true; //prevent TOC from loading

            try
            {
                if(DesignMode)
                    return;

                tvwTOC.Override.SelectionType = SelectType.Extended;
                LoadCommands();
                this.LoadData();
                this.LoadTOC();
                LoadValidators();

                tvwTOC.Override.Sort = SortType.Ascending;

                _loadingData = false;

                RestoreLastSelectedNode(Properties.Settings.Default.LastSelectedProcess);

                splitContainer1.Panel2.Enabled = SecurityManager.Current.IsInRole("ProcessManager.Edit");
            }
            catch(Exception exc)
            {
                splitContainer1.Enabled = false;
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
                _log.Fatal(exc, "Error loading form.");
            }
        }

        private void Labels_LabelsRowChanged(object sender, LabelDataSet.LabelsRowChangeEvent e)
        {
            try
            {
                var changedRow = e.Row as LabelDataSet.LabelsRow;

                if (e.Action != DataRowAction.Change || changedRow == null || !changedRow.IsValidState())
                {
                    return;
                }

                var changedNode = tvwTOC.GetNodeByKey(LabelNode.CreateKey(changedRow)) as LabelNode;

                if (changedNode != null)
                {
                    changedNode.UpdateNodeUI();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing label.");
            }
        }

        private void ProductClassLabels_ProductClassLabelsRowChanged(object sender, LabelDataSet.ProductClassLabelsRowChangeEvent e)
        {
            try
            {
                var changedRow = e.Row;

                if (e.Action != DataRowAction.Change || changedRow == null || !changedRow.IsValidState())
                {
                    return;
                }

                if (tvwTOC.GetNodeByKey(ProductClassLabelNode.CreateKey(changedRow)) is ProductClassLabelNode changedNode)
                {
                    changedNode.UpdateNodeUI();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing product class label.");
            }
        }

        #endregion

        #region Nodes

        #region Nested type: LabelTypeNode

        private class LabelTypeNode : DataNode<LabelDataSet.LabelTypeRow>
        {
            #region Fields

            public const string KEY_PREFIX = "FOLDER";

            #endregion

            #region Properties

            public LabelFactory.LabelType LabelType
            {
                get
                {
                    return (LabelFactory.LabelType)DataRow.LabelTypeID;
                }
            }

            #endregion

            #region Methods

            public LabelTypeNode(LabelDataSet.LabelTypeRow row)
                : base(row, row.LabelTypeID.ToString(), KEY_PREFIX, NodeName(row))
            {
                var labelType = LabelType;

                switch (labelType)
                {
                    case LabelFactory.LabelType.WO:
                    case LabelFactory.LabelType.COC:
                    case LabelFactory.LabelType.OutsideProcessing:
                        LeftImages.Add(Properties.Resources.Order_16);
                        break;
                    case LabelFactory.LabelType.ShippingPackage:
                    case LabelFactory.LabelType.ShippingOrder:
                        LeftImages.Add(Properties.Resources.Shipping_16);
                        break;
                    case LabelFactory.LabelType.Rework:
                    case LabelFactory.LabelType.ReworkContainer:
                    case LabelFactory.LabelType.OutsideProcessingRework:
                    case LabelFactory.LabelType.OutsideProcessingReworkContainer:
                        LeftImages.Add(Properties.Resources.Repair_16);
                        break;
                    case LabelFactory.LabelType.Container:
                    case LabelFactory.LabelType.COCContainer:
                    case LabelFactory.LabelType.OutsideProcessingContainer:
                        LeftImages.Add(Properties.Resources.Container_16);
                        break;
                    case LabelFactory.LabelType.Hold:
                    case LabelFactory.LabelType.HoldContainer:
                        LeftImages.Add(Properties.Resources.Hold_16);
                        break;
                    case LabelFactory.LabelType.ExternalRework:
                    case LabelFactory.LabelType.ExternalReworkContainer:
                        LeftImages.Add(Properties.Resources.Repair_Blue_16);
                        break;
                    case LabelFactory.LabelType.ReceivingContainer:
                        LeftImages.Add(Properties.Resources.Import_16);
                        break;
                    default:
                        LeftImages.Add(Properties.Resources.Folder_16);
                        break;
                }
            }

            private static string NodeName(LabelDataSet.LabelTypeRow row)
            {
                if (row == null)
                {
                    return null;
                }

                var labelType = (LabelFactory.LabelType)row.LabelTypeID;
                string nodeName = row.Name;

                switch (labelType)
                {
                    case LabelFactory.LabelType.COC:
                    case LabelFactory.LabelType.COCContainer:
                        nodeName = "COC";
                        break;
                    case LabelFactory.LabelType.Container:
                    case LabelFactory.LabelType.WO:
                        nodeName = "Default";
                        break;
                    case LabelFactory.LabelType.Rework:
                    case LabelFactory.LabelType.ReworkContainer:
                        nodeName = "Rework";
                        break;
                    case LabelFactory.LabelType.OutsideProcessing:
                    case LabelFactory.LabelType.OutsideProcessingContainer:
                        nodeName = "Outside Processing";
                        break;
                    case LabelFactory.LabelType.OutsideProcessingRework:
                    case LabelFactory.LabelType.OutsideProcessingReworkContainer:
                        nodeName = "Outside Processing (Rework)";
                        break;
                    case LabelFactory.LabelType.Hold:
                    case LabelFactory.LabelType.HoldContainer:
                        nodeName = "Hold";
                        break;
                    case LabelFactory.LabelType.ExternalRework:
                    case LabelFactory.LabelType.ExternalReworkContainer:
                        nodeName = "External Rework";
                        break;
                    case LabelFactory.LabelType.ReceivingContainer:
                        nodeName = "Receiving";
                        break;
                    default:
                        break;
                }

                return nodeName;
            }

            public override bool CanDelete
            {
                get { return false; }
            }
            
            #endregion
        }

        #endregion

        #region Nested type: LabelNode

        private class LabelNode : DataNode<LabelDataSet.LabelsRow>
        {
            #region Fields

            public const string KEY_PREFIX = "LR";
            private static Image _imageCache = null;

            #endregion

            #region Properties

            public bool DataLoaded { get; private set; }

            #endregion

            #region Methods

            public LabelNode(LabelDataSet.LabelsRow cr)
                : base(cr, cr.LabelID.ToString(), KEY_PREFIX, "New Label")
            {
                if (_imageCache == null)
                    _imageCache = Properties.Resources.ThermalLabel_16;

                LeftImages.Add(_imageCache);

                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                try
                {
                    Text = (base.DataRow.LabelCustomerSummaryRow == null ? "-Default-" : base.DataRow.LabelCustomerSummaryRow.Name);
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error updating node UI.");
                }
            }

            public void LoadData(LabelManager manager)
            {
                if(!this.DataLoaded)
                {
                    if(this.DataRow.LabelID > 0)
                        manager.taLabelMedia.FillByLabelNoMedia(manager.dsLabels.LabelMedia, this.DataRow.LabelID);

                    this.DataLoaded = true;
                }
            }

            public static string CreateKey(LabelDataSet.LabelsRow changedRow)
            {
                if (changedRow == null)
                {
                    return string.Empty;
                }

                return CreateKey(KEY_PREFIX, changedRow.LabelID.ToString());
            }

            #endregion
        }

        #endregion

        #region Nested type: ProductClassLabelNode

        private class ProductClassLabelNode : DataNode<LabelDataSet.ProductClassLabelsRow>
        {
            #region Fields

            public const string KEY_PREFIX = "PC";
            private const string DEFAULT_NAME = "New Product Class Label";
            private static Image _imageCache;

            #endregion

            #region Properties

            public bool DataLoaded { get; private set; }

            #endregion

            #region Methods

            public ProductClassLabelNode(LabelDataSet.ProductClassLabelsRow cr)
                : base(cr, cr.ProductClassLabelID.ToString(), KEY_PREFIX, DEFAULT_NAME)
            {
                if (_imageCache == null)
                    _imageCache = Properties.Resources.ThermalLabel_16;

                LeftImages.Add(_imageCache);

                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                try
                {
                    Text = DataRow.IsProductClassNull() ? DEFAULT_NAME : $"Product Class - {DataRow.ProductClass}";
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error updating node UI.");
                }
            }

            public void LoadData(LabelManager manager)
            {
                if(!this.DataLoaded)
                {
                    if (DataRow.ProductClassLabelID > 0)
                        manager.taLabelMedia.FillByProductClassLabelNoMedia(manager.dsLabels.LabelMedia, this.DataRow.ProductClassLabelID);

                    this.DataLoaded = true;
                }
            }

            public static string CreateKey(LabelDataSet.ProductClassLabelsRow changedRow)
            {
                if (changedRow == null)
                {
                    return string.Empty;
                }

                return CreateKey(KEY_PREFIX, changedRow.ProductClassLabelID.ToString());
            }

            #endregion
        }

        #endregion

        #region Nested type: LabelsRootNode

        private class LabelsRootNode : UltraTreeNode
        {
            #region Methods

            public LabelsRootNode()
                : base("ROOT", "Labels")
            {
                LeftImages.Add(Properties.Resources.ThermalLabel_16);
            }

            #endregion
        }

        #endregion

        #region Nested type: LabelCategoryNode

        private class LabelCategoryNode : UltraTreeNode
        {
            #region Fields

            private const string KEY_PREFIX = "CATEGORY_";

            #endregion

            #region Properties

            public LabelFactory.LabelCategory Category { get; private set; }

            #endregion

            #region Methods

            public LabelCategoryNode(LabelFactory.LabelCategory category)
                : base(KEY_PREFIX + category.ToString(), CategoryName(category))
            {
                Category = category;
                base.LeftImages.Add(GetImage(category));
            }

            private static Bitmap GetImage(LabelFactory.LabelCategory category)
            {
                Bitmap returnImage = null;
                if (category == LabelFactory.LabelCategory.WO)
                {
                    returnImage = Properties.Resources.Order_16;
                }
                else if (category == LabelFactory.LabelCategory.Container)
                {
                    returnImage =  Properties.Resources.Container_16;
                }
                else if (category == LabelFactory.LabelCategory.Shipping)
                {
                    returnImage = Properties.Resources.Shipping_16;
                }

                return returnImage;
            }

            private static string CategoryName(LabelFactory.LabelCategory category)
            {
                string categoryName = string.Empty;

                if (category == LabelFactory.LabelCategory.WO)
                {
                    categoryName = "Work Order";
                }
                else if (category == LabelFactory.LabelCategory.Container)
                {
                    categoryName = "Container";
                }
                else if (category == LabelFactory.LabelCategory.Shipping)
                {
                    categoryName = "Shipping";
                }

                return categoryName;
            }

            #endregion
        }
        #endregion

        #endregion

        #region Commands

        #region Nested type: AddCustomerLabelCommand

        private class AddCustomerLabelCommand : TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddNode;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get
                {
                    return _node is LabelTypeNode;
                }
            }

            #endregion

            #region Methods

            public AddCustomerLabelCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if(this.AddNode != null)
                    this.AddNode(this, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #region Nested type: AddProductClassLabelCommand

        private class AddProductClassLabelCommand : TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddNode;

            #endregion

            #region Properties

            public override bool Enabled => _node is LabelTypeNode lblNode &&
                lblNode.LabelType != LabelFactory.LabelType.ShippingPackage;

            #endregion

            #region Methods

            public AddProductClassLabelCommand(ToolBase tool, UltraTree treeView)
                : base(tool)
            {
                TreeView = treeView;
            }

            public override void OnClick()
            {
                AddNode?.Invoke(this, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #region Nested type: AddCustomerLabelCommand

        private class EditLabelCommand : TreeNodeCommandBase
        {
            #region Fields

            public EventHandler EditLabel;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get
                {
                    return _node is LabelTypeNode || _node is LabelNode || _node is ProductClassLabelNode;
                }
            }

            #endregion

            #region Methods

            public EditLabelCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if (this.EditLabel != null)
                    this.EditLabel(this, EventArgs.Empty);
            }

            #endregion
        }
        #endregion

        #region Nested type: ExportCommand

        private class ExportCommand : CopyCommand
        {
            #region Fields

            private LabelManager _manager = null;

            #endregion

            #region Properties

            public override bool Enabled =>
                _node is LabelNode || _node is LabelTypeNode || _node is ProductClassLabelNode;

            #endregion

            #region Methods

            public ExportCommand(ToolBase tool, UltraTree toc, LabelManager manager)
                : base(tool, toc)
            {
                base.TreeView = toc;
                _manager = manager;
            }

            public override void OnClick()
            {
                try
                {
                    using (var fileDialg = new SaveFileDialog())
                    {
                        var initialDirectory = System.IO.Path.Combine(FileSystem.UserDocumentPath(), "Labels");
                        if (System.IO.Directory.Exists(initialDirectory))
                            System.IO.Directory.CreateDirectory(initialDirectory);

                        fileDialg.AddExtension = true;
                        fileDialg.Title = "Export Label";
                        fileDialg.Filter = "Label Configuration (*.label)|*.label";
                        fileDialg.OverwritePrompt = true;
                        fileDialg.InitialDirectory = initialDirectory;
                        fileDialg.FileName = _node.Text + ".label";

                        if (fileDialg.ShowDialog() == DialogResult.OK)
                        {
                            var labelNode = _node as LabelNode;
                            var labelTypeNode = _node as LabelTypeNode;
                            string labelXML = null;

                            if(labelNode != null)
                            {
                                if(labelNode.DataRow.IsDataNull())
                                    labelXML = _manager.taLabels.GetLabelData(labelNode.DataRow.LabelID);
                                else
                                    labelXML = labelNode.DataRow.Data;
                            }
                            else if (labelTypeNode != null)
                            {
                                if (labelTypeNode.DataRow.IsDataNull())
                                    labelXML = _manager.taLabelType.GetLabelData(labelTypeNode.DataRow.LabelTypeID);
                                else
                                    labelXML = labelTypeNode.DataRow.Data;
                            }
                            else if (_node is ProductClassLabelNode productClassLabelNode)
                            {
                                if (productClassLabelNode.DataRow.IsDataNull())
                                    labelXML = _manager.taProductClassLabels.GetLabelData(productClassLabelNode.DataRow.ProductClassLabelID);
                                else
                                    labelXML = productClassLabelNode.DataRow.Data;
                            }
                            
                            if(!String.IsNullOrWhiteSpace(labelXML))
                            {
                                if(System.IO.File.Exists(fileDialg.FileName))
                                    System.IO.File.Delete(fileDialg.FileName);

                                System.IO.File.WriteAllText(fileDialg.FileName, labelXML);

                                MessageBoxUtilities.ShowMessageBoxOK("Succesfully exported label.", "Export Label");
                            }
                            else
                                MessageBoxUtilities.ShowMessageBoxWarn("No label to export found.", "Export Label");
                        }
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error exporting Label.");
                }
            }

            public override void Dispose()
            {
                _manager = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region Nested type: ImportCommand

        private class ImportCommand : TreeNodeCommandBase
        {
            #region Fields

            private LabelManager _manager = null;

            #endregion

            #region Properties

            public override bool Enabled =>
                _node is LabelNode || _node is LabelTypeNode || _node is ProductClassLabelNode;

            #endregion

            #region Methods

            public ImportCommand(ToolBase tool, UltraTree toc, LabelManager manager)
                : base(tool)
            {
                base.TreeView = toc;
                _manager = manager;
            }

            public override void OnClick()
            {
                try
                {
                    using (var fileDialog = new OpenFileDialog())
                    {
                        var initialDirectory = System.IO.Path.Combine(FileSystem.UserDocumentPath(), "Labels");
                        if (System.IO.Directory.Exists(initialDirectory))
                            System.IO.Directory.CreateDirectory(initialDirectory);

                        fileDialog.Title = "Import Label";
                        fileDialog.Filter = "Label Configuration (*.label)|*.label";
                        fileDialog.InitialDirectory = initialDirectory;

                        if (fileDialog.ShowDialog() == DialogResult.OK && System.IO.File.Exists(fileDialog.FileName))
                        {
                            var labelXML = System.IO.File.ReadAllText(fileDialog.FileName);
                            var labelNode = _node as LabelNode;
                            var labelTypeNode = _node as LabelTypeNode;

                            if (labelNode != null)
                            {
                                labelNode.DataRow.Data = labelXML;
                                labelNode.DataRow.Version += labelNode.DataRow.Version;
                                labelNode.DataRow.SetMediaIDNull(); //clear media
                            }
                            else if (labelTypeNode != null)
                            {
                                labelTypeNode.DataRow.Data = labelXML;
                                labelTypeNode.DataRow.Version += labelTypeNode.DataRow.Version;
                                labelTypeNode.DataRow.SetMediaIDNull(); //clear media
                            }
                            else if (_node is ProductClassLabelNode productClassLabelNode)
                            {
                                productClassLabelNode.DataRow.Data = labelXML;
                                productClassLabelNode.DataRow.Version += productClassLabelNode.DataRow.Version;
                                productClassLabelNode.DataRow.SetMediaIDNull(); //clear media
                            }

                            //reload node
                            if(_node.Selected)
                                _manager.LoadNode(_node);
                        }
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error importing node.");
                }
            }

            public override void Dispose()
            {
                _manager = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #endregion

        #region LabelTypeComparer

        private sealed class LabelTypeComparer
            : IComparer
        {
            private static Dictionary<LabelFactory.LabelType, int> _sortOrder = new Dictionary<LabelFactory.LabelType, int>()
            {
                [LabelFactory.LabelType.ShippingOrder] = 1,
                [LabelFactory.LabelType.ShippingPackage] = 2,
                [LabelFactory.LabelType.Hold] = 3,
                [LabelFactory.LabelType.HoldContainer] = 4,
                [LabelFactory.LabelType.Rework] = 5,
                [LabelFactory.LabelType.ReworkContainer] = 6,
                [LabelFactory.LabelType.ExternalRework] = 7,
                [LabelFactory.LabelType.ExternalReworkContainer] = 8,
                [LabelFactory.LabelType.OutsideProcessing] = 9,
                [LabelFactory.LabelType.OutsideProcessingContainer] = 10,
                [LabelFactory.LabelType.OutsideProcessingRework] = 11,
                [LabelFactory.LabelType.OutsideProcessingReworkContainer] = 12,
                [LabelFactory.LabelType.COC] = 13,
                [LabelFactory.LabelType.COCContainer] = 14,
                [LabelFactory.LabelType.Container] = 15,
                [LabelFactory.LabelType.WO] = 16,
            };

            #region IComparer Members

            public int Compare(object x, object y)
            {
                var xNode = x as LabelTypeNode;
                var yNode = y as LabelTypeNode;

                if (xNode == null && yNode == null)
                {
                    return 0;
                }
                else if (xNode == null && yNode != null)
                {
                    return -1;
                }
                else if (xNode != null && yNode == null)
                {
                    return 1;
                }

                int xValue;
                int yValue;

                _sortOrder.TryGetValue(xNode.LabelType, out xValue);
                _sortOrder.TryGetValue(yNode.LabelType, out yValue);

                return xValue.CompareTo(yValue);
            }

            #endregion
        }

        #endregion

        #region LabelCategoryComparer

        private sealed class LabelCategoryComparer
            : IComparer
        {
            #region Fields

            private static Dictionary<LabelFactory.LabelCategory, int> _sortOrder = new Dictionary<LabelFactory.LabelCategory, int>()
            {
                [LabelFactory.LabelCategory.WO] = 1,
                [LabelFactory.LabelCategory.Container] = 2,
                [LabelFactory.LabelCategory.Shipping] = 3,
            };

            #endregion


            public int Compare(object x, object y)
            {
                var xNode = x as LabelCategoryNode;
                var yNode = y as LabelCategoryNode;

                if (xNode == null && yNode == null)
                {
                    return 0;
                }
                else if (xNode == null && yNode != null)
                {
                    return -1;
                }
                else if (xNode != null && yNode == null)
                {
                    return 1;
                }

                int xValue;
                int yValue;

                _sortOrder.TryGetValue(xNode.Category, out xValue);
                _sortOrder.TryGetValue(yNode.Category, out yValue);

                return xValue.CompareTo(yValue);
            }
        }
        #endregion
    }
}