
namespace DWOS.UI.Processing
{
    partial class PartRacking
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PartRacking));
            this.cboOrder = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.lableWO = new Infragistics.Win.Misc.UltraLabel();
            this.picPartImage = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.btnClose = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.numPrintQty = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.numPartQty = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtOrderNotes = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraComboEditor2 = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.taOrder = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderSummaryTableAdapter();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.tePartNotes = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.gbFixture = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.gbPart = new Infragistics.Win.Misc.UltraGroupBox();
            this.lbPart = new Infragistics.Win.Misc.UltraLabel();
            this.gbOrder = new Infragistics.Win.Misc.UltraGroupBox();
            this.lbDueDate = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.lbCustomer = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.lbPO = new Infragistics.Win.Misc.UltraLabel();
            this.gbBatch = new Infragistics.Win.Misc.UltraGroupBox();
            this.lbBatch = new Infragistics.Win.Misc.UltraLabel();
            this.cbPrintPreview = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.helpLink1 = new DWOS.UI.Utilities.HelpLink();
            ((System.ComponentModel.ISupportInitialize)(this.cboOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrintQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderNotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraComboEditor2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tePartNotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbFixture)).BeginInit();
            this.gbFixture.SuspendLayout();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbPart)).BeginInit();
            this.gbPart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbOrder)).BeginInit();
            this.gbOrder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbBatch)).BeginInit();
            this.gbBatch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbPrintPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // cboOrder
            // 
            this.cboOrder.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
            this.cboOrder.DisplayMember = "OrderID";
            this.cboOrder.Location = new System.Drawing.Point(74, 48);
            this.cboOrder.Name = "cboOrder";
            this.cboOrder.Size = new System.Drawing.Size(118, 23);
            this.cboOrder.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cboOrder.TabIndex = 28;
            this.cboOrder.ValueMember = "OrderID";
            this.cboOrder.SelectionChanged += new System.EventHandler(this.cboOrder_SelectionChanged);
            // 
            // lableWO
            // 
            appearance1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lableWO.Appearance = appearance1;
            this.lableWO.AutoSize = true;
            this.lableWO.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lableWO.Location = new System.Drawing.Point(4, 51);
            this.lableWO.Name = "lableWO";
            this.lableWO.Size = new System.Drawing.Size(40, 16);
            this.lableWO.TabIndex = 29;
            this.lableWO.Text = "Order:";
            // 
            // picPartImage
            // 
            this.picPartImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.picPartImage.Appearance = appearance2;
            this.picPartImage.BorderShadowColor = System.Drawing.Color.Empty;
            this.picPartImage.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.picPartImage.DefaultImage = global::DWOS.UI.Properties.Resources.NoImage;
            this.picPartImage.Location = new System.Drawing.Point(196, 30);
            this.picPartImage.Name = "picPartImage";
            this.picPartImage.Size = new System.Drawing.Size(182, 195);
            this.picPartImage.TabIndex = 31;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(315, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(76, 23);
            this.btnClose.TabIndex = 30;
            this.btnClose.Text = "&Close";
            this.btnClose.Click += new System.EventHandler(this.btnCLose_Click);
            // 
            // ultraLabel1
            // 
            appearance3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ultraLabel1.Appearance = appearance3;
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel1.Location = new System.Drawing.Point(4, 30);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(31, 16);
            this.ultraLabel1.TabIndex = 34;
            this.ultraLabel1.Text = "Part:";
            // 
            // ultraLabel2
            // 
            appearance4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ultraLabel2.Appearance = appearance4;
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel2.Location = new System.Drawing.Point(4, 53);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(54, 16);
            this.ultraLabel2.TabIndex = 35;
            this.ultraLabel2.Text = "Quantity:";
            // 
            // ultraLabel3
            // 
            appearance5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ultraLabel3.Appearance = appearance5;
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel3.Location = new System.Drawing.Point(4, 34);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(35, 16);
            this.ultraLabel3.TabIndex = 36;
            this.ultraLabel3.Text = "Type:";
            // 
            // ultraLabel4
            // 
            appearance6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ultraLabel4.Appearance = appearance6;
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel4.Location = new System.Drawing.Point(4, 23);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(20, 16);
            this.ultraLabel4.TabIndex = 30;
            this.ultraLabel4.Text = "ID:";
            // 
            // numPrintQty
            // 
            this.numPrintQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numPrintQty.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Button;
            editorButton1.Text = "Print";
            editorButton1.Width = 70;
            this.numPrintQty.ButtonsLeft.Add(editorButton1);
            this.numPrintQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numPrintQty.Location = new System.Drawing.Point(184, 7);
            this.numPrintQty.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.numPrintQty.MaskInput = "nnn,nnn";
            this.numPrintQty.MaxValue = 10;
            this.numPrintQty.MinValue = 1;
            this.numPrintQty.Name = "numPrintQty";
            this.numPrintQty.Size = new System.Drawing.Size(125, 20);
            this.numPrintQty.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.numPrintQty.TabIndex = 2;
            this.numPrintQty.Value = 2;
            this.numPrintQty.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.numPrintQty_EditorButtonClick);
            // 
            // numPartQty
            // 
            this.numPartQty.Location = new System.Drawing.Point(74, 50);
            this.numPartQty.MaskInput = "nnn,nnn,nnn";
            this.numPartQty.MinValue = 0;
            this.numPartQty.Name = "numPartQty";
            this.numPartQty.NullText = "0";
            this.numPartQty.PromptChar = ' ';
            this.numPartQty.Size = new System.Drawing.Size(117, 23);
            this.numPartQty.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.OnMouseEnter;
            this.numPartQty.TabIndex = 89;
            // 
            // txtOrderNotes
            // 
            this.txtOrderNotes.AcceptsReturn = true;
            this.txtOrderNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOrderNotes.Location = new System.Drawing.Point(196, 48);
            this.txtOrderNotes.Multiline = true;
            this.txtOrderNotes.Name = "txtOrderNotes";
            this.txtOrderNotes.NullText = "<Notes>";
            appearance7.ForeColor = System.Drawing.Color.Silver;
            this.txtOrderNotes.NullTextAppearance = appearance7;
            this.txtOrderNotes.Size = new System.Drawing.Size(182, 61);
            this.txtOrderNotes.TabIndex = 90;
            // 
            // ultraComboEditor2
            // 
            this.ultraComboEditor2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
            this.ultraComboEditor2.DisplayMember = "OrderID";
            this.ultraComboEditor2.Location = new System.Drawing.Point(74, 30);
            this.ultraComboEditor2.Name = "ultraComboEditor2";
            this.ultraComboEditor2.Size = new System.Drawing.Size(118, 23);
            this.ultraComboEditor2.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.ultraComboEditor2.TabIndex = 90;
            this.ultraComboEditor2.ValueMember = "OrderID";
            // 
            // ultraLabel5
            // 
            appearance8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ultraLabel5.Appearance = appearance8;
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel5.Location = new System.Drawing.Point(4, 73);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(26, 16);
            this.ultraLabel5.TabIndex = 91;
            this.ultraLabel5.Text = "PO:";
            // 
            // taOrder
            // 
            this.taOrder.ClearBeforeFill = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cbPrintPreview);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.numPrintQty);
            this.panel2.Controls.Add(this.helpLink1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 377);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(400, 38);
            this.inboxControlStyler1.SetStyleSettings(this.panel2, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.panel2.TabIndex = 95;
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // tePartNotes
            // 
            this.tePartNotes.AcceptsReturn = true;
            this.tePartNotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tePartNotes.Location = new System.Drawing.Point(4, 79);
            this.tePartNotes.Multiline = true;
            this.tePartNotes.Name = "tePartNotes";
            this.tePartNotes.NullText = "<Notes>";
            appearance9.ForeColor = System.Drawing.Color.Silver;
            this.tePartNotes.NullTextAppearance = appearance9;
            this.tePartNotes.Size = new System.Drawing.Size(187, 146);
            this.tePartNotes.TabIndex = 92;
            // 
            // gbFixture
            // 
            this.gbFixture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFixture.Controls.Add(this.ultraComboEditor2);
            this.gbFixture.Controls.Add(this.ultraLabel3);
            this.gbFixture.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFixture.Location = new System.Drawing.Point(8, 298);
            this.gbFixture.Name = "gbFixture";
            this.gbFixture.Size = new System.Drawing.Size(383, 65);
            this.gbFixture.TabIndex = 93;
            this.gbFixture.Text = "Fixture";
            this.gbFixture.TextRenderingMode = Infragistics.Win.TextRenderingMode.GDI;
            this.gbFixture.Visible = false;
            // 
            // ultraPanel1
            // 
            this.ultraPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.gbPart);
            this.ultraPanel1.ClientArea.Controls.Add(this.gbOrder);
            this.ultraPanel1.ClientArea.Controls.Add(this.gbFixture);
            this.ultraPanel1.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(399, 369);
            this.ultraPanel1.TabIndex = 103;
            // 
            // gbPart
            // 
            this.gbPart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPart.Controls.Add(this.lbPart);
            this.gbPart.Controls.Add(this.tePartNotes);
            this.gbPart.Controls.Add(this.picPartImage);
            this.gbPart.Controls.Add(this.ultraLabel1);
            this.gbPart.Controls.Add(this.numPartQty);
            this.gbPart.Controls.Add(this.ultraLabel2);
            this.gbPart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbPart.Location = new System.Drawing.Point(7, 133);
            this.gbPart.Name = "gbPart";
            this.gbPart.Size = new System.Drawing.Size(383, 236);
            this.gbPart.TabIndex = 97;
            this.gbPart.Text = "Part";
            // 
            // lbPart
            // 
            this.lbPart.AutoSize = true;
            this.lbPart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPart.Location = new System.Drawing.Point(74, 30);
            this.lbPart.Name = "lbPart";
            this.lbPart.Size = new System.Drawing.Size(56, 16);
            this.lbPart.TabIndex = 93;
            this.lbPart.Text = "Unknown";
            // 
            // gbOrder
            // 
            this.gbOrder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbOrder.Controls.Add(this.lbDueDate);
            this.gbOrder.Controls.Add(this.ultraLabel7);
            this.gbOrder.Controls.Add(this.lbCustomer);
            this.gbOrder.Controls.Add(this.ultraLabel6);
            this.gbOrder.Controls.Add(this.lbPO);
            this.gbOrder.Controls.Add(this.txtOrderNotes);
            this.gbOrder.Controls.Add(this.ultraLabel5);
            this.gbOrder.Controls.Add(this.lableWO);
            this.gbOrder.Controls.Add(this.cboOrder);
            this.gbOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbOrder.Location = new System.Drawing.Point(8, 5);
            this.gbOrder.Name = "gbOrder";
            this.gbOrder.Size = new System.Drawing.Size(383, 122);
            this.gbOrder.TabIndex = 96;
            this.gbOrder.Text = "Order";
            // 
            // lbDueDate
            // 
            this.lbDueDate.AutoSize = true;
            this.lbDueDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDueDate.Location = new System.Drawing.Point(74, 93);
            this.lbDueDate.Name = "lbDueDate";
            this.lbDueDate.Size = new System.Drawing.Size(56, 16);
            this.lbDueDate.TabIndex = 95;
            this.lbDueDate.Text = "Unknown";
            // 
            // ultraLabel7
            // 
            appearance10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ultraLabel7.Appearance = appearance10;
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel7.Location = new System.Drawing.Point(4, 93);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(60, 16);
            this.ultraLabel7.TabIndex = 94;
            this.ultraLabel7.Text = "Due Date:";
            // 
            // lbCustomer
            // 
            this.lbCustomer.AutoSize = true;
            this.lbCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCustomer.Location = new System.Drawing.Point(74, 30);
            this.lbCustomer.Name = "lbCustomer";
            this.lbCustomer.Size = new System.Drawing.Size(56, 16);
            this.lbCustomer.TabIndex = 93;
            this.lbCustomer.Text = "Unknown";
            // 
            // ultraLabel6
            // 
            appearance11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ultraLabel6.Appearance = appearance11;
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel6.Location = new System.Drawing.Point(4, 30);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(61, 16);
            this.ultraLabel6.TabIndex = 92;
            this.ultraLabel6.Text = "Customer:";
            // 
            // lbPO
            // 
            this.lbPO.AutoSize = true;
            this.lbPO.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPO.Location = new System.Drawing.Point(74, 73);
            this.lbPO.Name = "lbPO";
            this.lbPO.Size = new System.Drawing.Size(56, 16);
            this.lbPO.TabIndex = 32;
            this.lbPO.Text = "Unknown";
            // 
            // gbBatch
            // 
            this.gbBatch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbBatch.Controls.Add(this.lbBatch);
            this.gbBatch.Controls.Add(this.ultraLabel4);
            this.gbBatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbBatch.Location = new System.Drawing.Point(7, 12);
            this.gbBatch.Name = "gbBatch";
            this.gbBatch.Size = new System.Drawing.Size(384, 48);
            this.gbBatch.TabIndex = 95;
            this.gbBatch.Text = "Batch";
            this.gbBatch.Visible = false;
            // 
            // lbBatch
            // 
            this.lbBatch.AutoSize = true;
            this.lbBatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBatch.Location = new System.Drawing.Point(74, 23);
            this.lbBatch.Name = "lbBatch";
            this.lbBatch.Size = new System.Drawing.Size(25, 16);
            this.lbBatch.TabIndex = 31;
            this.lbBatch.Text = "N/A";
            // 
            // cbPrintPreview
            // 
            this.cbPrintPreview.Location = new System.Drawing.Point(81, 8);
            this.cbPrintPreview.Name = "cbPrintPreview";
            this.cbPrintPreview.Size = new System.Drawing.Size(71, 20);
            this.cbPrintPreview.TabIndex = 34;
            this.cbPrintPreview.Text = "Preview";
            // 
            // helpLink1
            // 
            this.helpLink1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpLink1.BackColor = System.Drawing.Color.Transparent;
            this.helpLink1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLink1.HelpPage = "order_check_in_dialog.htm";
            this.helpLink1.Location = new System.Drawing.Point(9, 12);
            this.helpLink1.Name = "helpLink1";
            this.helpLink1.Size = new System.Drawing.Size(33, 16);
            this.helpLink1.TabIndex = 33;
            // 
            // PartRacking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(400, 415);
            this.Controls.Add(this.ultraPanel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.gbBatch);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(416, 454);
            this.Name = "PartRacking";
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Part Racking";
            this.Load += new System.EventHandler(this.PartRacking_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cboOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrintQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderNotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraComboEditor2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tePartNotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbFixture)).EndInit();
            this.gbFixture.ResumeLayout(false);
            this.gbFixture.PerformLayout();
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gbPart)).EndInit();
            this.gbPart.ResumeLayout(false);
            this.gbPart.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbOrder)).EndInit();
            this.gbOrder.ResumeLayout(false);
            this.gbOrder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbBatch)).EndInit();
            this.gbBatch.ResumeLayout(false);
            this.gbBatch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbPrintPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboOrder;
        private Infragistics.Win.Misc.UltraLabel lableWO;
        private Utilities.HelpLink helpLink1;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picPartImage;
        private Infragistics.Win.Misc.UltraButton btnClose;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numPrintQty;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numPartQty;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtOrderNotes;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor ultraComboEditor2;
        private Data.Datasets.OrdersDataSetTableAdapters.OrderSummaryTableAdapter taOrder;
        private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor tePartNotes;
        private Infragistics.Win.Misc.UltraGroupBox gbFixture;
        private System.Windows.Forms.Panel panel2;
        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        private Infragistics.Win.Misc.UltraGroupBox gbBatch;
        private Infragistics.Win.Misc.UltraGroupBox gbOrder;
        private Infragistics.Win.Misc.UltraGroupBox gbPart;
        private Infragistics.Win.Misc.UltraLabel lbPart;
        private Infragistics.Win.Misc.UltraLabel lbCustomer;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.Misc.UltraLabel lbPO;
        private Infragistics.Win.Misc.UltraLabel lbBatch;
        private Infragistics.Win.Misc.UltraLabel lbDueDate;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbPrintPreview;
    }
}