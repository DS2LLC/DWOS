namespace DWOS.UI
{
	partial class QuickViewPart
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
			if(disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuickViewPart));
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.pnlPartInfo = new DWOS.UI.Admin.PartManagerPanels.PartInfo();
            this.dsParts = new DWOS.Data.Datasets.PartsDataset();
            this.taMedia = new DWOS.Data.Datasets.PartsDatasetTableAdapters.MediaTableAdapter();
            this.taPart_Media = new DWOS.Data.Datasets.PartsDatasetTableAdapters.Part_MediaTableAdapter();
            this.taPart = new DWOS.Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter();
            this.taPriceUnit = new DWOS.Data.Datasets.PartsDatasetTableAdapters.PriceUnitTableAdapter();
            this.taProcessAnswer = new DWOS.Data.Datasets.PartsDatasetTableAdapters.PartProcessAnswerTableAdapter();
            this.taPartProcess = new DWOS.Data.Datasets.PartsDatasetTableAdapters.PartProcessTableAdapter();
            this.taManufacturer = new DWOS.Data.Datasets.PartsDatasetTableAdapters.d_ManufacturerTableAdapter();
            this.taCustomer = new DWOS.Data.Datasets.PartsDatasetTableAdapters.CustomerTableAdapter();
            this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
            this.btnPrint = new Infragistics.Win.Misc.UltraButton();
            this.taAirframe = new DWOS.Data.Datasets.PartsDatasetTableAdapters.d_AirframeTableAdapter();
            this.taProcessAlias = new DWOS.Data.Datasets.PartsDatasetTableAdapters.ProcessAliasTableAdapter();
            this.taPartDocumentLink = new DWOS.Data.Datasets.PartsDatasetTableAdapters.Part_DocumentLinkTableAdapter();
            this.taMaterial = new DWOS.Data.Datasets.PartsDatasetTableAdapters.d_MaterialTableAdapter();
            this.taPartProcessVolumePrice = new DWOS.Data.Datasets.PartsDatasetTableAdapters.PartProcessVolumePriceTableAdapter();
            this.persistWindowState = new DWOS.UI.Utilities.PersistWindowState(this.components);
            this.taLists = new DWOS.Data.Datasets.PartsDatasetTableAdapters.ListsTableAdapter();
            this.taListValues = new DWOS.Data.Datasets.PartsDatasetTableAdapters.ListValuesTableAdapter();
            this.taPartCustomFields = new DWOS.Data.Datasets.PartsDatasetTableAdapters.PartCustomFieldsTableAdapter();
            this.taPartLevelCustomField = new DWOS.Data.Datasets.PartsDatasetTableAdapters.PartLevelCustomFieldTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dsParts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(310, 726);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 14;
            this.btnOK.Text = "&OK";
            // 
            // pnlPartInfo
            // 
            this.pnlPartInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPartInfo.AutoScroll = true;
            this.pnlPartInfo.AutoScrollMinSize = new System.Drawing.Size(445, 730);
            this.pnlPartInfo.CurrentPartUsageCount = 0;
            this.pnlPartInfo.Dataset = null;
            this.pnlPartInfo.Editable = true;
            this.pnlPartInfo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlPartInfo.IsActivePanel = false;
            this.pnlPartInfo.Location = new System.Drawing.Point(6, 5);
            this.pnlPartInfo.Name = "pnlPartInfo";
            this.pnlPartInfo.Padding = new System.Windows.Forms.Padding(3);
            this.pnlPartInfo.Size = new System.Drawing.Size(471, 718);
            this.pnlPartInfo.TabIndex = 16;
            this.pnlPartInfo.ViewOnly = true;
            // 
            // dsParts
            // 
            this.dsParts.DataSetName = "PartsDataset";
            this.dsParts.EnforceConstraints = false;
            this.dsParts.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // taMedia
            // 
            this.taMedia.ClearBeforeFill = true;
            // 
            // taPart_Media
            // 
            this.taPart_Media.ClearBeforeFill = true;
            // 
            // taPart
            // 
            this.taPart.ClearBeforeFill = true;
            // 
            // taPriceUnit
            // 
            this.taPriceUnit.ClearBeforeFill = true;
            // 
            // taProcessAnswer
            // 
            this.taProcessAnswer.ClearBeforeFill = true;
            // 
            // taPartProcess
            // 
            this.taPartProcess.ClearBeforeFill = true;
            // 
            // taManufacturer
            // 
            this.taManufacturer.ClearBeforeFill = true;
            // 
            // taCustomer
            // 
            this.taCustomer.ClearBeforeFill = true;
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnPrint.Location = new System.Drawing.Point(392, 726);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(76, 23);
            this.btnPrint.TabIndex = 17;
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // taAirframe
            // 
            this.taAirframe.ClearBeforeFill = true;
            // 
            // taProcessAlias
            // 
            this.taProcessAlias.ClearBeforeFill = true;
            // 
            // taPartDocumentLink
            // 
            this.taPartDocumentLink.ClearBeforeFill = true;
            // 
            // taMaterial
            // 
            this.taMaterial.ClearBeforeFill = true;
            // 
            // taPartProcessVolumePrice
            // 
            this.taPartProcessVolumePrice.ClearBeforeFill = true;
            // 
            // persistWindowState
            // 
            this.persistWindowState.FileNamePrefix = null;
            this.persistWindowState.ParentForm = this;
            this.persistWindowState.Splitter = null;
            // 
            // taLists
            // 
            this.taLists.ClearBeforeFill = false;
            // 
            // taListValues
            // 
            this.taListValues.ClearBeforeFill = false;
            // 
            // taPartCustomFields
            // 
            this.taPartCustomFields.ClearBeforeFill = false;
            // 
            // taPartLevelCustomField
            // 
            this.taPartLevelCustomField.ClearBeforeFill = false;
            // 
            // QuickViewPart
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(480, 761);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.pnlPartInfo);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QuickViewPart";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
            this.Text = "Part Summary";
            this.Load += new System.EventHandler(this.QuickViewPart_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dsParts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnOK;
		private DWOS.UI.Admin.PartManagerPanels.PartInfo pnlPartInfo;
		private DWOS.Data.Datasets.PartsDataset dsParts;
		private DWOS.Data.Datasets.PartsDatasetTableAdapters.MediaTableAdapter taMedia;
		private DWOS.Data.Datasets.PartsDatasetTableAdapters.Part_MediaTableAdapter taPart_Media;
		private DWOS.Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter taPart;
		private DWOS.Data.Datasets.PartsDatasetTableAdapters.PriceUnitTableAdapter taPriceUnit;
		private DWOS.Data.Datasets.PartsDatasetTableAdapters.PartProcessAnswerTableAdapter taProcessAnswer;
		private DWOS.Data.Datasets.PartsDatasetTableAdapters.PartProcessTableAdapter taPartProcess;
		private DWOS.Data.Datasets.PartsDatasetTableAdapters.d_ManufacturerTableAdapter taManufacturer;
		private DWOS.Data.Datasets.PartsDatasetTableAdapters.CustomerTableAdapter taCustomer;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
		private Infragistics.Win.Misc.UltraButton btnPrint;
		private DWOS.Data.Datasets.PartsDatasetTableAdapters.d_AirframeTableAdapter taAirframe;
		private DWOS.Data.Datasets.PartsDatasetTableAdapters.ProcessAliasTableAdapter taProcessAlias;
        private Data.Datasets.PartsDatasetTableAdapters.Part_DocumentLinkTableAdapter taPartDocumentLink;
        private Data.Datasets.PartsDatasetTableAdapters.d_MaterialTableAdapter taMaterial;
        private Data.Datasets.PartsDatasetTableAdapters.PartProcessVolumePriceTableAdapter taPartProcessVolumePrice;
        private Utilities.PersistWindowState persistWindowState;
        private Data.Datasets.PartsDatasetTableAdapters.ListsTableAdapter taLists;
        private Data.Datasets.PartsDatasetTableAdapters.ListValuesTableAdapter taListValues;
        private Data.Datasets.PartsDatasetTableAdapters.PartCustomFieldsTableAdapter taPartCustomFields;
        private Data.Datasets.PartsDatasetTableAdapters.PartLevelCustomFieldTableAdapter taPartLevelCustomField;
    }
}