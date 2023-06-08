-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '15.1.0'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
	INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
	UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Add new settings
INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('UsePriceUnitQuantities', 'true')
GO


-- Add New label Types
INSERT INTO [dbo].[LabelType] ([LabelTypeID],[Name],[Version]) VALUES (4,'Container',1)
GO

INSERT INTO [dbo].[LabelType] ([LabelTypeID],[Name],[Version]) VALUES (5,'ReWork',1)
GO

INSERT INTO [dbo].[LabelType] ([LabelTypeID],[Name],[Version]) VALUES (6,'COC Container',1)
GO

-- Add new security groups
INSERT INTO [dbo].[SecurityRoleCategory] ([SecurityRoleCategoryID]) VALUES ('Blanket PO')
GO


INSERT INTO [dbo].[SecurityRole]([SecurityRoleID],[Description],[SecurityRoleCategoryID])
     VALUES ('AddBlanketPOOrder','Ability to add a new order from an existing Blanket PO.',	'Blanket PO')
GO

INSERT INTO [dbo].[SecurityRole]([SecurityRoleID],[Description],[SecurityRoleCategoryID])
     VALUES ('BlanketPOManager','Ability to manage Blanket POs.', 'Blanket PO')
GO

INSERT INTO [dbo].[SecurityRole]([SecurityRoleID],[Description],[SecurityRoleCategoryID])
     VALUES ('COCPrintContainerLabels','Ability to manage Blanket POs.', 'Quality')
GO

INSERT INTO [dbo].[SecurityRole]([SecurityRoleID],[Description],[SecurityRoleCategoryID])
     VALUES ('AddContainers','Ability to add containers in Order Entry.', 'Sales')
GO


-- Customer Print COC Default
ALTER TABLE dbo.Customer ADD PrintCOC bit NULL
GO

ALTER TABLE dbo.Customer ADD CONSTRAINT	DF_Customer_PrintCOC DEFAULT 1 FOR PrintCOC
GO

 UPDATE dbo.Customer SET [PrintCOC] = 1
 GO


-- OrderTemplate

CREATE TABLE [dbo].[OrderTemplate](
	[OrderTemplateID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[OrderDate] [smalldatetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
	[PartID] [int] NULL,
	[InitialQuantity] [int] NOT NULL,
	[PurchaseOrder] [nvarchar](50) NULL,
	[BasePrice] [smallmoney] NULL,
	[PriceUnit] [nvarchar](50) NULL,
	[ShippingMethod] [int] NULL,
	[QuotePartID] [int] NULL,
	[CreatedBy] [int] NULL,
	[Priority] [nvarchar](50) NULL,
 CONSTRAINT [PK_OrderTemplate] PRIMARY KEY CLUSTERED 
(
	[OrderTemplateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[OrderTemplate]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemplate_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customer] ([CustomerID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[OrderTemplate] CHECK CONSTRAINT [FK_OrderTemplate_Customer]
GO

ALTER TABLE [dbo].[OrderTemplate]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemplate_CustomerShipping] FOREIGN KEY([ShippingMethod])
REFERENCES [dbo].[CustomerShipping] ([CustomerShippingID])
GO

ALTER TABLE [dbo].[OrderTemplate] CHECK CONSTRAINT [FK_OrderTemplate_CustomerShipping]
GO

ALTER TABLE [dbo].[OrderTemplate]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemplate_d_PriceUnit] FOREIGN KEY([PriceUnit])
REFERENCES [dbo].[d_PriceUnit] ([PriceUnitID])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[OrderTemplate] CHECK CONSTRAINT [FK_OrderTemplate_d_PriceUnit]
GO

ALTER TABLE [dbo].[OrderTemplate]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemplate_d_Priority] FOREIGN KEY([Priority])
REFERENCES [dbo].[d_Priority] ([PriorityID])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[OrderTemplate] CHECK CONSTRAINT [FK_OrderTemplate_d_Priority]
GO

ALTER TABLE [dbo].[OrderTemplate]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemplate_Part] FOREIGN KEY([PartID])
REFERENCES [dbo].[Part] ([PartID])
GO

ALTER TABLE [dbo].[OrderTemplate] CHECK CONSTRAINT [FK_OrderTemplate_Part]
GO

ALTER TABLE [dbo].[OrderTemplate]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemplate_QuotePart] FOREIGN KEY([QuotePartID])
REFERENCES [dbo].[QuotePart] ([QuotePartID])
GO

ALTER TABLE [dbo].[OrderTemplate] CHECK CONSTRAINT [FK_OrderTemplate_QuotePart]
GO

ALTER TABLE [dbo].[OrderTemplate]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemplate_Users] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Users] ([UserID])
GO

ALTER TABLE [dbo].[OrderTemplate] CHECK CONSTRAINT [FK_OrderTemplate_Users]
GO


-- Relate Order to OrderTemplate
ALTER TABLE dbo.[Order] ADD
	OrderTemplateID int NULL
GO

ALTER TABLE dbo.[Order] ADD CONSTRAINT
	FK_Order_OrderTemplate FOREIGN KEY
	(
	OrderTemplateID
	) REFERENCES dbo.OrderTemplate
	(
	OrderTemplateID
	) ON UPDATE  NO ACTION 
	 ON DELETE   NO ACTION 
	
GO

-- Set Seed
DBCC CHECKIDENT('OrderTemplate', RESEED, 1000)
GO

-- OrderContainers
CREATE TABLE [dbo].[OrderContainers](
	[OrderContainerID] [int] IDENTITY(100,1) NOT NULL,
	[PartQuantity] [int] NOT NULL,
	[OrderID] [int] NOT NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_OrderContainers] PRIMARY KEY CLUSTERED 
(
	[OrderContainerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[OrderContainers]  WITH CHECK ADD  CONSTRAINT [FK_OrderContainers_Order] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Order] ([OrderID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[OrderContainers] CHECK CONSTRAINT [FK_OrderContainers_Order]
GO

-- Report Table
CREATE TABLE dbo.Report
	(
	ReportID int NOT NULL IDENTITY (1, 1),
	CustomerID int NOT NULL,
	ReportType int NOT NULL,
	ReportName nvarchar(50) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Report ADD CONSTRAINT
	PK_Report PRIMARY KEY CLUSTERED 
	(
	ReportID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

-- ReportFields Table
DROP TABLE [dbo].[ReportFields]
GO

CREATE TABLE [dbo].[ReportFields](
	[ReportFieldID] [int] IDENTITY(1,1) NOT NULL,
	[ReportID] [int] NOT NULL,
	[FieldName] [nvarchar](50) NOT NULL,
	[DisplayName] [nvarchar](50) NOT NULL,
	[Width] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[IsCustomField] [bit] NOT NULL CONSTRAINT [DF_ReportFields_IsCustom]  DEFAULT ((0)),
	[ExcelFunction] [nvarchar](50) NULL,
 CONSTRAINT [PK_ReportFields] PRIMARY KEY CLUSTERED 
(
	[ReportFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ReportFields]  WITH CHECK ADD  CONSTRAINT [FK_ReportFields_Report] FOREIGN KEY([ReportID])
REFERENCES [dbo].[Report] ([ReportID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

-- Report Table Relationships
ALTER TABLE dbo.Report ADD CONSTRAINT
	FK_Report_Customer FOREIGN KEY
	(
	CustomerID
	) REFERENCES dbo.Customer
	(
	CustomerID
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 
	
GO



-----------------------------------------------------------------------
--- Adding process count function
-----------------------------------------------------------------------
CREATE FUNCTION fnGetProcessCount 
(
	@orderID int
)
RETURNS int
AS
BEGIN
	-- Return the result of the function
	RETURN (SELECT  COUNT(OrderProcessesID) AS Expr1 FROM OrderProcesses WHERE (OrderID = @orderId))

END
GO

-----------------------------------------------------------------------
--- Function that determines if the batch orders part counts equal the order's part count
-----------------------------------------------------------------------
CREATE FUNCTION [dbo].[fnGetHasRemainingBatchQty]
(
	@orderID int
)
RETURNS bit
AS
BEGIN

DECLARE @hasRemainingQty bit = 0
DECLARE @batchPartQty int = 0
DECLARE @orderPartQty int = 0

SET @orderPartQty = (SELECT SUM(BatchOrder.PartQuantity) as BatchedPartQty FROM BatchOrder INNER JOIN Batch ON BatchOrder.BatchID = Batch.BatchID WHERE BatchOrder.OrderID = @orderID AND Active = 1)
SET @batchPartQty = (SELECT PartQuantity FROM  [Order] WHERE OrderID = @orderID)
			
IF (@batchPartQty <= @orderPartQty)
			BEGIN
				SET @hasRemainingQty = 1
			END

RETURN @hasRemainingQty

END
GO

-----------------------------------------------------------------------
--- Creating the Batch Process, Order Process junction table
-----------------------------------------------------------------------
CREATE TABLE [dbo].[BatchProcess_OrderProcess](
	[BatchProcessID] [int] NOT NULL,
	[OrderProcessID] [int] NOT NULL,
 CONSTRAINT [PK_BatchProcess_OrderProcess] PRIMARY KEY CLUSTERED 
(
	[BatchProcessID] ASC,
	[OrderProcessID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[BatchProcess_OrderProcess]  WITH CHECK ADD  CONSTRAINT [FK_BatchProcess_OrderProcess_BatchProcesses] FOREIGN KEY([BatchProcessID])
REFERENCES [dbo].[BatchProcesses] ([BatchProcessID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[BatchProcess_OrderProcess] CHECK CONSTRAINT [FK_BatchProcess_OrderProcess_BatchProcesses]
GO

ALTER TABLE [dbo].[BatchProcess_OrderProcess]  WITH CHECK ADD  CONSTRAINT [FK_BatchProcess_OrderProcess_OrderProcesses] FOREIGN KEY([OrderProcessID])
REFERENCES [dbo].[OrderProcesses] ([OrderProcessesID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[BatchProcess_OrderProcess] CHECK CONSTRAINT [FK_BatchProcess_OrderProcess_OrderProcesses]
GO

-----------------------------------------------------------------------
--- Dropping the unused BatchProcessID column from OrderProcesses
-----------------------------------------------------------------------
ALTER TABLE dbo.OrderProcesses DROP CONSTRAINT FK_OrderProcesses_BatchProcesses
GO

ALTER TABLE dbo.OrderProcesses DROP COLUMN BatchProcessID
GO

-- Add new column to OrderHold
ALTER TABLE dbo.OrderHold ADD OriginalWorkStatus nvarchar(50) NULL
GO

-----------------------------------------------------------------------------
--- Adding the default Rework label
-----------------------------------------------------------------------------
DELETE FROM [dbo].[LabelType] WHERE [LabelTypeID] = 5
Go 

INSERT INTO [dbo].[LabelType]
           ([LabelTypeID]
           ,[Name]
           ,[Data]
           ,[Version])
     VALUES
           (5
           ,'Rework'
           ,'<?xml version="1.0" encoding="utf-8"?><ThermalLabel Version="5.0" Width="6.5" Height="4" GapLength="0" MarkLength="0" OffsetLength="0" UnitType="Inch" LabelsPerRow="1" LabelsHorizontalGapLength="0" IsContinuous="False" PrintOrientation="Portrait"><Items><BarcodeItem Name="WORKORDER" X="0.409" Y="1.8368" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.3436" Height="0.7396" Symbology="Code39" Code="_x0031_23456" AddChecksum="False" AztecCodeModuleSize="0" BarcodeAlignment="MiddleCenter" BarRatio="6" DataMatrixModuleSize="0" Font="NativePrinterFontA,10,Point,,,False,90" QRCodeModuleSize="0" Sizing="Fill" TextFont="NativePrinterFontA,10,Point,,,False,90" /><BarcodeItem Name="" X="2.2152" Y="4.4375" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.0104" Height="0.0104" Symbology="Code39" Code="_x0031_23456789" AddChecksum="False" BarcodeAlignment="MiddleCenter" Font="NativePrinterFontA,5,Point,,,False,90" TextFont="NativePrinterFontA,10,Point,,,False,90" /><TextItem Name="PARTQUANTITY" X="5.3438" Y="1.3381" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="1.125" Height="0.3959" Text="_x0039_999" Font="Tahoma,18,True,False,False,False,Point,,,False,90" TextAlignment="Center" TextPadding="0.03" /><TextItem Name="" X="4.6666" Y="1.3797" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.6562" Height="0.3187" Text="QTY:" Font="Tahoma,18,True,False,False,False,Point,,,False,90" TextAlignment="Center" /><TextItem Name="CUSTOMERNAME" X="0.0381" Y="0.0209" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="3.8958" Height="0.5729" Text="My_x0020_Customer" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="ORDERPRIORITY" X="4.2985" Y="0.0104" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.1146" Height="0.5417" Text="Normal" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><LineShapeItem Name="" X="0.0225" Y="0.5938" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="6.4479" Height="0.01" /><TextItem Name="CUSTOMERWO" X="1.2465" Y="0.6337" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="1.75" Height="0.2396" Text="_x0031_0001" Font="Tahoma,12,Point,,,False,90" TextPadding="0.03" /><TextItem Name="" X="0.0382" Y="0.6649" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="1.1667" Height="0.2084" Text="Customer_x0020_WO:" Font="Tahoma,12,Point,,,False,90" TextAlignment="Right" /><TextItem Name="REQUIREDDATE" X="1.257" Y="1.2587" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="1.6771" Height="0.2604" Text="_x0037__x002F_20_x002F_2014" Font="Tahoma,12,Point,,,False,90" TextPadding="0.03" /><TextItem Name="" X="0.434" Y="1.2795" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.7708" Height="0.2187" Text="Required:" Font="Tahoma,12,Point,,,False,90" TextAlignment="Right" /><LineShapeItem Name="" X="0.1683" Y="2.6667" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="6.2083" Height="0.0104" Orientation="DiagonalUp" /><TextItem Name="PROCESSDEPT2" X="0.4097" Y="3.1316" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="1.4141" Height="0.201" Text="NDT" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><LineShapeItem Name="" X="3.3399" Y="2.705" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.0018" Height="1.2594" Orientation="DiagonalDown" /><TextItem Name="PROCESSNAME2" X="0.272" Y="3.3272" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.8877" Height="0.201" Text="FPI" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSDEPT3" X="0.4097" Y="3.5189" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="1.4332" Height="0.201" Text="Chemical" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSNAME3" X="0.2848" Y="3.7159" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.9004" Height="0.2073" Text="Chemical_x0020_Conversion" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><LineShapeItem Name="" X="4.1648" Y="0.0277" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.0104" Height="0.5417" Orientation="DiagonalDown" /><BarcodeItem Name="CHECKINCOMMAND" X="3.8728" Y="1.8334" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.5834" Height="0.7291" Symbology="Code39" Code="_x007E_123456_x007E_" AddChecksum="False" AztecCodeModuleSize="0" BarcodeAlignment="MiddleCenter" BarRatio="5" DataMatrixModuleSize="0" Font="NativePrinterFontA,10,Point,,,False,90" QRCodeModuleSize="0" Sizing="Fill" TextFont="NativePrinterFontA,10,Point,,,False,90" /><TextItem Name="" X="0.0916" Y="1.8438" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.6667" Height="0.2604" Text="Order" BackColor="Black" Font="Tahoma,10,False,False,True,False,Point,,,False,90" ForeColor="White" RotationAngle="270" TextAlignment="Center" TextPadding="0.03" /><TextItem Name="" X="3.602" Y="1.8229" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.7084" Height="0.2604" Text="Check_x0020_In" BackColor="Black" Font="Tahoma,10,False,False,True,False,Point,,,False,90" ForeColor="White" RotationAngle="270" TextAlignment="Center" TextPadding="0.03" /><LineShapeItem Name="" X="0.0642" Y="1.7605" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="6.4479" Height="0.01" /><TextItem Name="PURCHASEORDER" X="1.2583" Y="0.9375" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="1.7292" Height="0.2292" Text="P31665" Font="Tahoma,12,Point,,,False,90" TextPadding="0.03" /><TextItem Name="" X="0.4028" Y="0.9774" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.7708" Height="0.2187" Text="PO:" Font="Tahoma,12,Point,,,False,90" TextAlignment="Right" /><TextItem Name="PROCESSDEPT4" X="3.7082" Y="2.692" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.0376" Height="0.2412" Text="Masking" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSNAME4" X="3.6133" Y="2.9159" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.861" Height="0.21" Text="Mask_x0020_Part" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSDEPT5" X="3.7082" Y="3.127" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.0314" Height="0.2101" Text="Paint" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSNAME5" X="3.623" Y="3.3196" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.8235" Height="0.2163" Text="PA_x0020_Red" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSDEPT6" X="3.7082" Y="3.5206" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.0688" Height="0.1975" Text="Paint" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSNAME6" X="3.6292" Y="3.7139" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.8298" Height="0.2112" Text="PA_x0020_Blue" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PARTNAME" X="3.7298" Y="0.6277" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.6918" Height="0.6746" Text="_x0031_7P-12312311" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="PROCESSDEPT1" X="0.4097" Y="2.7146" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.8037" Height="0.1962" Text="Chemical" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSNAME1" X="0.288" Y="2.9161" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.9762" Height="0.2095" Text="Sulfuric_x0020_Acid_x0020_Anodize" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="StepOrder1" X="0.1066" Y="2.6979" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.25" Height="0.2188" Text="_x0031_-" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="StepOrder2" X="0.1171" Y="3.1146" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.2396" Height="0.2188" Text="_x0032_-" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="StepOrder3" X="0.1171" Y="3.5417" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.2396" Height="0.2083" Text="_x0033_-" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="StepOrder4" X="3.4296" Y="2.7083" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.2188" Height="0.2396" Text="_x0034_-" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="StepOrder5" X="3.4191" Y="3.125" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.2188" Height="0.2083" Text="_x0035_-" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="StepOrder6" X="3.4296" Y="3.5104" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.2188" Height="0.1979" Text="_x0036_-" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="REWORKPENDING" X="1.1281" Y="2.7292" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="4.4167" Height="0.5625" Text="_x002A__x002A__x0020_Pending_x0020_Rework_x0020_Planning_x0020__x002A__x002A_" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /></Items></ThermalLabel>'
           ,0)
GO

-----------------------------------------------------------------------------
--- Set old batches to inactive
-----------------------------------------------------------------------------
UPDATE [dbo].[Batch] SET [Active] = 0 WHERE [Active] = 1
GO