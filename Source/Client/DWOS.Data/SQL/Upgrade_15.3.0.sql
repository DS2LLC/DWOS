-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '15.3.0'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Fix spelling error in security role
-- A trigger automatically updates SecurityGroup_Role with new ID.
UPDATE SecurityRole
SET SecurityRoleID = 'Receiving'
WHERE SecurityRoleID = 'Recieving';

GO

--
-- Cleanup of CurrentLocation values for [Order] and [Batch] tables
--
DECLARE @invalidCurrentLocation bit;

IF EXISTS(SELECT * FROM [Order] WHERE CurrentLocation NOT IN (SELECT DepartmentID FROM d_Department) OR CurrentLocation IS NULL)
    SET @invalidCurrentLocation = 1;

IF EXISTS(SELECT * FROM Batch WHERE CurrentLocation NOT IN (SELECT DepartmentID FROM d_Department) OR CurrentLocation IS NULL)
    SET @invalidCurrentLocation = 1;

IF @invalidCurrentLocation = 1
    BEGIN
        -- Replace Invalid Values
        INSERT INTO d_Department(DepartmentID, SystemName, Active)
        VALUES ('-None-', 'None', 1);

        UPDATE [Order]
        SET CurrentLocation = '-None-'
        WHERE CurrentLocation NOT IN (SELECT DepartmentID FROM d_Department) OR CurrentLocation IS NULL;

        UPDATE [Batch]
        SET CurrentLocation = '-None-'
        WHERE CurrentLocation NOT IN (SELECT DepartmentID FROM d_Department) OR CurrentLocation IS NULL;
    END
GO

--
--  Associate CurrentLocation columns with d_Departments table
--
ALTER TABLE [Order]
ALTER COLUMN [CurrentLocation] nvarchar(50) NOT NULL;

GO

ALTER TABLE [Batch]
ALTER COLUMN [CurrentLocation] nvarchar(50) NOT NULL;

GO

ALTER TABLE [Order] ADD CONSTRAINT
    FK_Order_d_Department FOREIGN KEY
    (
        CurrentLocation
    ) REFERENCES d_Department
    (
        DepartmentID
    ) ON UPDATE CASCADE
    ON DELETE NO ACTION;

GO

ALTER TABLE Batch ADD CONSTRAINT
    FK_Batch_d_Department FOREIGN KEY
    (
        CurrentLocation
    ) REFERENCES d_Department
    (
        DepartmentID
    ) ON UPDATE CASCADE
    ON DELETE NO ACTION;

GO

--
-- Data for advanced calculator (Quote)
--
CREATE TABLE [dbo].[d_MarkupType](
    [MarkupTypeID] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_d_MarkupType] PRIMARY KEY CLUSTERED 
(
    [MarkupTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];

GO

INSERT INTO d_MarkupType
    VALUES ('Fixed'), ('Percentage');

GO

CREATE TABLE dbo.QuotePartPrice
    (
    QuotePartPriceID int NOT NULL IDENTITY(1, 1),
    QuotePartID int NOT NULL,
    Rate int NOT NULL,
    LaborCost money NOT NULL,
    MaterialCost money NOT NULL,
    OverheadCost money NOT NULL,
    MarkupTotal money NOT NULL
    )  ON [PRIMARY]
GO

ALTER TABLE dbo.QuotePartPrice ADD CONSTRAINT
    PK_QuotePartPrice PRIMARY KEY CLUSTERED 
    (
    QuotePartPriceID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE dbo.QuotePartPrice ADD CONSTRAINT
    FK_QuotePartPrice_QuotePart FOREIGN KEY
    (
    QuotePartID
    ) REFERENCES dbo.QuotePart
    (
    QuotePartID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
    
GO

ALTER TABLE [dbo].[QuotePartPrice] CHECK CONSTRAINT [FK_QuotePartPrice_QuotePart]
GO

--- Persist data for QuotePartPrice calculations
CREATE TABLE dbo.QuotePartPriceCalculation
    (
    QuotePartPriceCalculationID int NOT NULL IDENTITY (1, 1),
    QuotePartPriceID int NOT NULL,
    Step nvarchar(25) NOT NULL,
    CalculationType nvarchar(25) NOT NULL,
    Data nvarchar(MAX) NOT NULL
    )  ON [PRIMARY]
     TEXTIMAGE_ON [PRIMARY];
GO
ALTER TABLE dbo.QuotePartPriceCalculation ADD CONSTRAINT
    PK_QuotePartPriceCalculation PRIMARY KEY CLUSTERED 
    (
    QuotePartPriceCalculationID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

GO
ALTER TABLE dbo.QuotePartPriceCalculation ADD CONSTRAINT
    FK_QuotePartPriceCalculation_QuotePartPrice FOREIGN KEY
    (
    QuotePartPriceID
    ) REFERENCES dbo.QuotePartPrice
    (
    QuotePartPriceID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE ;
    
GO

---
--- Process Pricing
---

-- Part
CREATE TABLE dbo.PartProcessVolumePrice
    (
    PartProcessVolumePriceID int NOT NULL IDENTITY (1, 1),
    PartProcessID int NOT NULL,
    PriceUnit nvarchar(50) NOT NULL,
    Amount smallmoney NOT NULL,
    MinQuantity int NULL,
    MaxQuantity int NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.PartProcessVolumePrice ADD CONSTRAINT
    PK_PartProcessVolumePrice PRIMARY KEY CLUSTERED 
    (
    PartProcessVolumePriceID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.PartProcessVolumePrice ADD CONSTRAINT
    FK_PartProcessVolumePrice_d_PriceUnit FOREIGN KEY
    (
    PriceUnit
    ) REFERENCES dbo.d_PriceUnit
    (
    PriceUnitID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
    
GO
ALTER TABLE dbo.PartProcessVolumePrice ADD CONSTRAINT
    FK_PartProcessVolumePrice_PartProcess FOREIGN KEY
    (
    PartProcessID
    ) REFERENCES dbo.PartProcess
    (
    PartProcessID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
    
GO

-- QuotePart
CREATE TABLE dbo.QuotePartProcessPrice
    (
    QuotePartProcessPriceID int NOT NULL IDENTITY (1, 1),
    QuotePartProcessID int NOT NULL,
    PriceUnit nvarchar(50) NOT NULL,
    Amount money NOT NULL,
    MinQuantity int NULL,
    MaxQuantity int NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.QuotePartProcessPrice ADD CONSTRAINT
    PK_QuotePartProcessPrice PRIMARY KEY CLUSTERED 
    (
    QuotePartProcessPriceID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.QuotePartProcessPrice ADD CONSTRAINT
    FK_QuotePartProcessPrice_QuotePart_Process FOREIGN KEY
    (
    QuotePartProcessID
    ) REFERENCES dbo.QuotePart_Process
    (
    QuotePartProcessID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
    
GO
ALTER TABLE dbo.QuotePartProcessPrice ADD CONSTRAINT
    FK_QuotePartProcessPrice_d_PriceUnit FOREIGN KEY
    (
    PriceUnit
    ) REFERENCES dbo.d_PriceUnit
    (
    PriceUnitID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
    
GO
