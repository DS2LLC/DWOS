-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '16.1.0'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--
-- Customer Addresses
--
CREATE TABLE dbo.CustomerAddress
    (
    CustomerAddressID int NOT NULL IDENTITY (1, 1),
    CustomerID int NOT NULL,
    Name nvarchar(255) NOT NULL,
    Address1 nvarchar(MAX) NULL,
    Address2 nvarchar(MAX) NULL,
    City nvarchar(50) NULL,
    State nvarchar(50) NULL,
    Zip nvarchar(50) NULL,
    IsDefault bit NOT NULL
    )  ON [PRIMARY]
     TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.CustomerAddress ADD CONSTRAINT
    PK_CustomerAddress PRIMARY KEY CLUSTERED 
    (
    CustomerAddressID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.CustomerAddress ADD CONSTRAINT
    FK_CustomerAddress_Customer FOREIGN KEY
    (
    CustomerID
    ) REFERENCES dbo.Customer
    (
    CustomerID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
    
GO

-- Import Customer address info
INSERT INTO CustomerAddress (CustomerID, Name, Address1, Address2, City, State, Zip, IsDefault)
SELECT CustomerID, Name, Address1, Address2, City, State, Zip, 1
FROM Customer;

-- Use customer address in OrderEntry
ALTER TABLE dbo.[Order] ADD
    CustomerAddressID int NULL
GO
ALTER TABLE dbo.[Order] ADD CONSTRAINT
    FK_Order_CustomerAddress FOREIGN KEY
    (
    CustomerAddressID
    ) REFERENCES dbo.CustomerAddress
    (
    CustomerAddressID
    ) ON UPDATE  NO ACTION 
     ON DELETE  NO ACTION
    
GO

-- Set CustomerAddressID to default
-- This should work because there is only one CustomerAddress for each Customer.
UPDATE [Order]
SET CustomerAddressID = CustomerAddress.CustomerAddressID
FROM [Order]
INNER JOIN CustomerAddress ON CustomerAddress.CustomerID = [Order].CustomerID;

GO

-- Use customer address for Blanket POs
ALTER TABLE dbo.[OrderTemplate] ADD
    CustomerAddressID int NULL
GO
ALTER TABLE dbo.OrderTemplate ADD CONSTRAINT
    FK_OrderTemplate_CustomerAddress FOREIGN KEY
    (
    CustomerAddressID
    ) REFERENCES dbo.CustomerAddress
    (
    CustomerAddressID
    ) ON UPDATE  NO ACTION 
     ON DELETE  NO ACTION
    
GO

-- Set CustomerAddressID to default
UPDATE OrderTemplate
SET CustomerAddressID = CustomerAddress.CustomerAddressID
FROM OrderTemplate
INNER JOIN CustomerAddress ON CustomerAddress.CustomerID = OrderTemplate.CustomerID;

GO

-- Use customer address for order shipments
ALTER TABLE dbo.OrderShipment ADD
    CustomerAddressID int NULL
GO
ALTER TABLE dbo.OrderShipment ADD CONSTRAINT
    FK_OrderShipment_CustomerAddress FOREIGN KEY
    (
    CustomerAddressID
    ) REFERENCES dbo.CustomerAddress
    (
    CustomerAddressID
    ) ON UPDATE  NO ACTION 
     ON DELETE  NO ACTION 

GO

-- Set CustomerAddressID to default
UPDATE OrderShipment
SET CustomerAddressID = CustomerAddress.CustomerAddressID
FROM OrderShipment
INNER JOIN [Order] ON OrderShipment.OrderID = [Order].OrderID
INNER JOIN CustomerAddress ON CustomerAddress.CustomerID = [Order].CustomerID;

GO

-- Use customer address for shipment packages
ALTER TABLE dbo.ShipmentPackage ADD
    CustomerAddressID int NULL
GO
ALTER TABLE dbo.ShipmentPackage ADD CONSTRAINT
    FK_ShipmentPackage_CustomerAddress FOREIGN KEY
    (
    CustomerAddressID
    ) REFERENCES dbo.CustomerAddress
    (
    CustomerAddressID
    ) ON UPDATE  NO ACTION 
     ON DELETE  NO ACTION 
    
GO

-- Set CustomerAddressID to default
UPDATE ShipmentPackage
SET CustomerAddressID = CustomerAddress.CustomerAddressID
FROM ShipmentPackage
INNER JOIN CustomerAddress ON CustomerAddress.CustomerID = ShipmentPackage.CustomerID;

GO

--
-- Removing unused tables.
--
DROP TABLE d_MarkupType;

GO

DROP TABLE PartProcessPrice;

GO

--
-- Increase column precision.
--

-- Weight columns need to support up to 8 decimal places.
ALTER TABLE Part
ALTER COLUMN Weight decimal(12,8) NULL;

GO

ALTER TABLE QuotePart
ALTER COLUMN Weight decimal(12,8) NULL;

GO

ALTER TABLE [Order]
ALTER COLUMN Weight decimal(14,8) NULL;

GO

ALTER TABLE OrderTemplate
ALTER COLUMN Weight decimal(14,8) NULL;

GO

ALTER TABLE OrderContainers
ALTER COLUMN Weight decimal(15,8);

GO

-- Price columns need to be support up to 5 decimal places.
ALTER TABLE Part
ALTER COLUMN EachPrice decimal(11,5) NULL;

GO

ALTER TABLE Part
ALTER COLUMN LotPrice decimal(11,5) NULL;

GO

ALTER TABLE [Order]
ALTER COLUMN BasePrice decimal(11,5) NULL;

GO

ALTER TABLE OrderProcesses
ALTER COLUMN Amount decimal(11,5) NULL;

GO

ALTER TABLE OrderTemplate
ALTER COLUMN BasePrice decimal(11,5) NULL;

GO

ALTER TABLE OrderFees
ALTER COLUMN Charge decimal(13,5) NOT NULL;

GO

ALTER TABLE OrderFeeType
ALTER COLUMN Price decimal(13,5) NOT NULL;

GO

ALTER TABLE Process
ALTER COLUMN Price decimal(8,5) NULL;

GO

ALTER TABLE QuotePart
ALTER COLUMN EachPrice decimal(11,5) NULL;

GO

ALTER TABLE QuotePart
ALTER COLUMN LotPrice decimal(11,5) NULL;

GO

ALTER TABLE QuotePartFees
ALTER COLUMN Charge decimal(11,5) NOT NULL;

GO

ALTER TABLE QuotePartPrice
ALTER COLUMN LaborCost decimal(11,5) NOT NULL;

GO

ALTER TABLE QuotePartPrice
ALTER COLUMN MaterialCost decimal(11,5) NOT NULL;

GO

ALTER TABLE QuotePartPrice
ALTER COLUMN OverheadCost decimal(11,5) NOT NULL;

GO

ALTER TABLE QuotePartPrice
ALTER COLUMN MarkupTotal decimal(11,5) NOT NULL;

GO

ALTER TABLE QuotePartProcessPrice
ALTER COLUMN Amount decimal(11,5) NOT NULL;

GO

ALTER TABLE PartProcessVolumePrice
ALTER COLUMN Amount decimal(11,5) NOT NULL;

GO

ALTER FUNCTION [dbo].[fnGetOrderFeePrice]
(
    @orderID int
)
RETURNS decimal(14,5)
AS
BEGIN

    DECLARE @result decimal(11,5)
    DECLARE @percentValue decimal(13,5)
    DECLARE @fixedValue decimal(13,5)

    DECLARE @priceUnit nvarchar(50) = (SELECT [Order].PriceUnit FROM [Order] WHERE [Order].OrderID = @orderID)

    IF (@priceUnit = 'Each')
        BEGIN
            SET @result = (SELECT BasePrice * [Order].PartQuantity FROM [Order] WHERE [Order].OrderID = @orderID)
        END
    ELSE
        BEGIN
            SET @result = (SELECT BasePrice FROM [Order] WHERE [Order].OrderID = @orderID)
        END

    -- Get all fixed cost
    SET @fixedValue = (SELECT   SUM(OrderFees.Charge)
    FROM            OrderFees INNER JOIN
                            OrderFeeType ON OrderFees.OrderFeeTypeID = OrderFeeType.OrderFeeTypeID
    WHERE OrderFeeType.FeeType = 'Fixed' AND OrderID = @orderID)


    -- Get all percent cost
    SET @percentValue = (SELECT   SUM((OrderFees.Charge / 100) * @result)
    FROM            OrderFees INNER JOIN
                             OrderFeeType ON OrderFees.OrderFeeTypeID = OrderFeeType.OrderFeeTypeID
    WHERE OrderFeeType.FeeType = 'Percentage' AND OrderID = @orderID AND OrderFees.Charge > 0)

    IF(@percentValue IS NULL)
        SET @percentValue = 0
    IF(@fixedValue IS NULL)
        SET @fixedValue = 0

    -- Return the result of the function
    RETURN  (@fixedValue + @percentValue)

END

GO

ALTER FUNCTION [dbo].[fnGetOrderTotalPrice] 
(
    @orderID int
)
RETURNS decimal(14,5)
AS
BEGIN
    
    DECLARE @result decimal(11,5)
    DECLARE @percentValue decimal(13,5)
    DECLARE @fixedValue decimal(13,5)

    DECLARE @priceUnit nvarchar(50) = (SELECT [Order].PriceUnit FROM [Order] WHERE [Order].OrderID = @orderID)

    IF (@priceUnit = 'Each')
        BEGIN
            SET @result = (SELECT BasePrice * [Order].PartQuantity FROM [Order] WHERE [Order].OrderID = @orderID)
        END
    ELSE IF (@priceUnit = 'Lot' OR @priceUnit = 'LotByWeight')
        BEGIN
            SET @result = (SELECT BasePrice FROM [Order] WHERE [Order].OrderID = @orderID)
        END
    ELSE IF (@priceUnit = 'EachByWeight')
        BEGIN
            SET @result = (SELECT [Order].BasePrice * IsNull([Order].Weight, 0) FROM [Order] WHERE [Order].OrderID = @orderID)
        END
    ELSE
        BEGIN
            SET @result = 0
        END

    -- Get all fixed cost
    SET @fixedValue = (SELECT   SUM(OrderFees.Charge)
    FROM            OrderFees INNER JOIN
                            OrderFeeType ON OrderFees.OrderFeeTypeID = OrderFeeType.OrderFeeTypeID
    WHERE OrderFeeType.FeeType = 'Fixed' AND OrderID = @orderID)
    

    -- Get all percent cost
    SET @percentValue = (SELECT   SUM((OrderFees.Charge / 100) * @result)
    FROM            OrderFees INNER JOIN
                             OrderFeeType ON OrderFees.OrderFeeTypeID = OrderFeeType.OrderFeeTypeID
    WHERE OrderFeeType.FeeType = 'Percentage' AND OrderID = @orderID AND OrderFees.Charge > 0)

    IF(@percentValue IS NULL)
        SET @percentValue = 0
    IF(@fixedValue IS NULL)
        SET @fixedValue = 0
    
    RETURN  (@result + @fixedValue + @percentValue)

END

GO

--
-- Process Price
--

ALTER TABLE Process
ADD MinPrice decimal(8,5) NULL;

GO

UPDATE Process
SET MinPrice = Price;

GO

--
-- Add label types
--
INSERT INTO LabelType (LabelTypeID, Name, Data, MediaID, Version)
VALUES
(
    7,
    'Rework Container',
    '<?xml version="1.0" encoding="utf-8"?><ThermalLabel Version="5.0" Width="6.5" Height="4" GapLength="0" MarkLength="0" OffsetLength="0" UnitType="Inch" LabelsPerRow="1" LabelsHorizontalGapLength="0" IsContinuous="False" PrintOrientation="Portrait"><Items><BarcodeItem Name="WORKORDER" X="0.409" Y="1.8368" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.3436" Height="0.7396" Symbology="Code39" Code="_x0031_23456" AddChecksum="False" AztecCodeModuleSize="0" BarcodeAlignment="MiddleCenter" BarRatio="6" DataMatrixModuleSize="0" Font="NativePrinterFontA,10,Point,,,False,90" QRCodeModuleSize="0" Sizing="Fill" TextFont="NativePrinterFontA,10,Point,,,False,90" /><BarcodeItem Name="" X="2.2152" Y="4.4375" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.0104" Height="0.0104" Symbology="Code39" Code="_x0031_23456789" AddChecksum="False" BarcodeAlignment="MiddleCenter" Font="NativePrinterFontA,5,Point,,,False,90" TextFont="NativePrinterFontA,10,Point,,,False,90" /><TextItem Name="PARTQUANTITY" X="5.3438" Y="1.3381" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="1.125" Height="0.3959" Text="_x0039_999" Font="Tahoma,18,True,False,False,False,Point,,,False,90" TextAlignment="Center" TextPadding="0.03" /><TextItem Name="" X="4.6666" Y="1.3797" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.6562" Height="0.3187" Text="QTY:" Font="Tahoma,18,True,False,False,False,Point,,,False,90" TextAlignment="Center" /><TextItem Name="CUSTOMERNAME" X="0.0381" Y="0.0209" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="3.8958" Height="0.5729" Text="My_x0020_Customer" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="ORDERPRIORITY" X="4.2985" Y="0.0104" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.1146" Height="0.5417" Text="Normal" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><LineShapeItem Name="" X="0.0225" Y="0.5938" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="6.4479" Height="0.01" /><TextItem Name="CUSTOMERWO" X="1.2465" Y="0.6337" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="1.75" Height="0.2396" Text="_x0031_0001" Font="Tahoma,12,Point,,,False,90" TextPadding="0.03" /><TextItem Name="" X="0.0382" Y="0.6649" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="1.1667" Height="0.2084" Text="Customer_x0020_WO:" Font="Tahoma,12,Point,,,False,90" TextAlignment="Right" /><TextItem Name="REQUIREDDATE" X="1.257" Y="1.2587" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="1.6771" Height="0.2604" Text="_x0037__x002F_20_x002F_2014" Font="Tahoma,12,Point,,,False,90" TextPadding="0.03" /><TextItem Name="" X="0.434" Y="1.2795" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.7708" Height="0.2187" Text="Required:" Font="Tahoma,12,Point,,,False,90" TextAlignment="Right" /><LineShapeItem Name="" X="0.1683" Y="2.6667" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="6.2083" Height="0.0104" Orientation="DiagonalUp" /><TextItem Name="PROCESSDEPT2" X="0.4097" Y="3.1316" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="1.4141" Height="0.201" Text="NDT" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><LineShapeItem Name="" X="3.3399" Y="2.705" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.0018" Height="1.2594" Orientation="DiagonalDown" /><TextItem Name="PROCESSNAME2" X="0.272" Y="3.3272" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.8877" Height="0.201" Text="FPI" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSDEPT3" X="0.4097" Y="3.5189" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="1.4332" Height="0.201" Text="Chemical" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSNAME3" X="0.2848" Y="3.7159" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.9004" Height="0.2073" Text="Chemical_x0020_Conversion" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><LineShapeItem Name="" X="4.1648" Y="0.0277" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.0104" Height="0.5417" Orientation="DiagonalDown" /><BarcodeItem Name="CHECKINCOMMAND" X="3.8728" Y="1.8334" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.5834" Height="0.7291" Symbology="Code39" Code="_x007E_123456_x007E_" AddChecksum="False" AztecCodeModuleSize="0" BarcodeAlignment="MiddleCenter" BarRatio="5" DataMatrixModuleSize="0" Font="NativePrinterFontA,10,Point,,,False,90" QRCodeModuleSize="0" Sizing="Fill" TextFont="NativePrinterFontA,10,Point,,,False,90" /><TextItem Name="" X="0.0916" Y="1.8438" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.6667" Height="0.2604" Text="Order" BackColor="Black" Font="Tahoma,10,False,False,True,False,Point,,,False,90" ForeColor="White" RotationAngle="270" TextAlignment="Center" TextPadding="0.03" /><TextItem Name="" X="3.602" Y="1.8229" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.7084" Height="0.2604" Text="Check_x0020_In" BackColor="Black" Font="Tahoma,10,False,False,True,False,Point,,,False,90" ForeColor="White" RotationAngle="270" TextAlignment="Center" TextPadding="0.03" /><LineShapeItem Name="" X="0.0642" Y="1.7605" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="6.4479" Height="0.01" /><TextItem Name="PURCHASEORDER" X="1.2583" Y="0.9375" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="1.7292" Height="0.2292" Text="P31665" Font="Tahoma,12,Point,,,False,90" TextPadding="0.03" /><TextItem Name="" X="0.4028" Y="0.9774" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.7708" Height="0.2187" Text="PO:" Font="Tahoma,12,Point,,,False,90" TextAlignment="Right" /><TextItem Name="PROCESSDEPT4" X="3.7082" Y="2.692" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.0376" Height="0.2412" Text="Masking" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSNAME4" X="3.6133" Y="2.9159" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.861" Height="0.21" Text="Mask_x0020_Part" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSDEPT5" X="3.7082" Y="3.127" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.0314" Height="0.2101" Text="Paint" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSNAME5" X="3.623" Y="3.3196" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.8235" Height="0.2163" Text="PA_x0020_Red" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSDEPT6" X="3.7082" Y="3.5206" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.0688" Height="0.1975" Text="Paint" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSNAME6" X="3.6292" Y="3.7139" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.8298" Height="0.2112" Text="PA_x0020_Blue" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PARTNAME" X="3.7298" Y="0.6277" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.6918" Height="0.6746" Text="_x0031_7P-12312311" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="PROCESSDEPT1" X="0.4097" Y="2.7146" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.8037" Height="0.1962" Text="Chemical" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="PROCESSNAME1" X="0.288" Y="2.9161" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="2.9762" Height="0.2095" Text="Sulfuric_x0020_Acid_x0020_Anodize" Font="Tahoma,10,Point,,,False,90" TextPadding="0.03" /><TextItem Name="StepOrder1" X="0.1066" Y="2.6979" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.25" Height="0.2188" Text="_x0031_-" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="StepOrder2" X="0.1171" Y="3.1146" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.2396" Height="0.2188" Text="_x0032_-" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="StepOrder3" X="0.1171" Y="3.5417" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.2396" Height="0.2083" Text="_x0033_-" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="StepOrder4" X="3.4296" Y="2.7083" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.2188" Height="0.2396" Text="_x0034_-" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="StepOrder5" X="3.4191" Y="3.125" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.2188" Height="0.2083" Text="_x0035_-" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="StepOrder6" X="3.4296" Y="3.5104" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="0.2188" Height="0.1979" Text="_x0036_-" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /><TextItem Name="REWORKPENDING" X="1.1281" Y="2.7292" DataField="" DataFieldFormatString="" CacheItemId="" Comments="" Tag="" Width="4.4167" Height="0.5625" Text="_x002A__x002A__x0020_Pending_x0020_Rework_x0020_Planning_x0020__x002A__x002A_" Font="Tahoma,10,Point,,,False,90" Sizing="Stretch" TextPadding="0.03" /></Items></ThermalLabel>',
    NULL,
    0
),
(
    8,
    'COC',
    '<?xml version="1.0" encoding="utf-8"?><ThermalLabel Version="5.0" Width="4" Height="3" GapLength="0" MarkLength="0" OffsetLength="0" UnitType="Inch" LabelsPerRow="1" LabelsHorizontalGapLength="0" IsContinuous="False" PrintOrientation="Portrait" />',
    NULL,
    0
),
(
    9,
    'Hold',
    '<?xml version="1.0" encoding="utf-8"?><ThermalLabel Version="5.0" Width="4" Height="3" GapLength="0" MarkLength="0" OffsetLength="0" UnitType="Inch" LabelsPerRow="1" LabelsHorizontalGapLength="0" IsContinuous="False" PrintOrientation="Portrait" />',
    NULL,
    0
),
(
    10,
    'Hold Container',
    '<?xml version="1.0" encoding="utf-8"?><ThermalLabel Version="5.0" Width="4" Height="3" GapLength="0" MarkLength="0" OffsetLength="0" UnitType="Inch" LabelsPerRow="1" LabelsHorizontalGapLength="0" IsContinuous="False" PrintOrientation="Portrait" />',
    NULL,
    0
),
(
    11,
    'Outside Processing',
    '<?xml version="1.0" encoding="utf-8"?><ThermalLabel Version="5.0" Width="4" Height="3" GapLength="0" MarkLength="0" OffsetLength="0" UnitType="Inch" LabelsPerRow="1" LabelsHorizontalGapLength="0" IsContinuous="False" PrintOrientation="Portrait" />',
    NULL,
    0
),
(
    12,
    'Outside Processing Container',
    '<?xml version="1.0" encoding="utf-8"?><ThermalLabel Version="5.0" Width="4" Height="3" GapLength="0" MarkLength="0" OffsetLength="0" UnitType="Inch" LabelsPerRow="1" LabelsHorizontalGapLength="0" IsContinuous="False" PrintOrientation="Portrait" />',
    NULL,
    0
),
(
    13,
    'Outside Processing Rework',
    '<?xml version="1.0" encoding="utf-8"?><ThermalLabel Version="5.0" Width="4" Height="3" GapLength="0" MarkLength="0" OffsetLength="0" UnitType="Inch" LabelsPerRow="1" LabelsHorizontalGapLength="0" IsContinuous="False" PrintOrientation="Portrait" />',
    NULL,
    0
),
(
    14,
    'Outside Processing Rework Container',
    '<?xml version="1.0" encoding="utf-8"?><ThermalLabel Version="5.0" Width="4" Height="3" GapLength="0" MarkLength="0" OffsetLength="0" UnitType="Inch" LabelsPerRow="1" LabelsHorizontalGapLength="0" IsContinuous="False" PrintOrientation="Portrait" />',
    NULL,
    0
),
(
    15,
    'External Rework',
    '<?xml version="1.0" encoding="utf-8"?><ThermalLabel Version="5.0" Width="4" Height="3" GapLength="0" MarkLength="0" OffsetLength="0" UnitType="Inch" LabelsPerRow="1" LabelsHorizontalGapLength="0" IsContinuous="False" PrintOrientation="Portrait" />',
    NULL,
    0
),
(
    16,
    'External Rework Container',
    '<?xml version="1.0" encoding="utf-8"?><ThermalLabel Version="5.0" Width="4" Height="3" GapLength="0" MarkLength="0" OffsetLength="0" UnitType="Inch" LabelsPerRow="1" LabelsHorizontalGapLength="0" IsContinuous="False" PrintOrientation="Portrait" />',
    NULL,
    0
);

GO

--
-- Create Outside Processing department
--
IF NOT EXISTS (SELECT * FROM d_Department WHERE DepartmentID = 'Outside Processing')
    INSERT INTO d_Department (DepartmentID, SystemName, Active)
    VALUES
    (
        'Outside Processing', 'Outside Processing', 1
    )
ELSE
    UPDATE d_Department
    SET SystemName = 'Outside Processing'
    WHERE DepartmentID = 'Outside Processing';

GO