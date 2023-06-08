-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '15.3.1'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--
-- Price Points
--
CREATE TABLE dbo.PricePoint
    (
    PricePointID int NOT NULL IDENTITY (1, 1),
    Name nvarchar(50) NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.PricePoint ADD CONSTRAINT
    PK_PricePoint PRIMARY KEY CLUSTERED 
    (
    PricePointID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.PricePoint ADD CONSTRAINT
    AK_PricePoint_Name UNIQUE NONCLUSTERED 
    (
    Name
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

CREATE TABLE dbo.PricePointDetail
    (
    PricePointDetailID int NOT NULL IDENTITY (1, 1),
    PricePointID int NOT NULL,
    PriceUnit nvarchar(50) NOT NULL,
    MinValue nvarchar(12) NOT NULL,
    MaxValue nvarchar(12) NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.PricePointDetail ADD CONSTRAINT
    PK_PricePointDetail PRIMARY KEY CLUSTERED 
    (
    PricePointDetailID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.PricePointDetail ADD CONSTRAINT
    FK_PricePointDetail_PricePoint FOREIGN KEY
    (
    PricePointID
    ) REFERENCES dbo.PricePoint
    (
    PricePointID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
    
GO
ALTER TABLE dbo.PricePointDetail ADD CONSTRAINT
    FK_PricePointDetail_d_PriceUnit FOREIGN KEY
    (
    PriceUnit
    ) REFERENCES dbo.d_PriceUnit
    (
    PriceUnitID
    ) ON UPDATE  NO ACTION 
     ON DELETE  NO ACTION 
    
GO

--
-- Import d_PriceUnit data into PricePoint\PricePointDetail
--
DECLARE @PricePointID TABLE (ID int);
DECLARE @defaultPricePointID int;

-- Default price point has NULL for a name
INSERT INTO PricePoint (Name)
OUTPUT inserted.PricePointID INTO @PricePointID
VALUES(NULL);

SET @defaultPricePointID =
(
    SELECT TOP 1 ID FROM @PricePointID
);

INSERT INTO PricePointDetail(PricePointID, PriceUnit, MinValue, MaxValue)
SELECT @defaultPricePointID,
    PriceUnitID,
    CASE WHEN PriceUnitID LIKE '%ByWeight' THEN CONVERT(nvarchar(12),MinWeight) ELSE CONVERT(nvarchar(12), MinQuantity) END,
    CASE WHEN PriceUnitID LIKE '%ByWeight' THEN CONVERT(nvarchar(12),MaxWeight) ELSE CONVERT(nvarchar(12), MaxQuantity) END
FROM d_PriceUnit;

GO

-- Copy imported values to use as default volume price points
DECLARE @PricePointID TABLE (ID int);
DECLARE @defaultVolumePricePointID int;

INSERT INTO PricePoint (Name)
OUTPUT inserted.PricePointID INTO @PricePointID
VALUES('VOLUME_DEFAULT');

SET @defaultVolumePricePointID =
(
    SELECT TOP 1 ID FROM @PricePointID
);

INSERT INTO PricePointDetail(PricePointID, PriceUnit, MinValue, MaxValue)
SELECT @defaultVolumePricePointID,
    PriceUnitID,
    CASE WHEN PriceUnitID LIKE '%ByWeight' THEN CONVERT(nvarchar(12),MinWeight) ELSE CONVERT(nvarchar(12), MinQuantity) END,
    CASE WHEN PriceUnitID LIKE '%ByWeight' THEN CONVERT(nvarchar(12),MaxWeight) ELSE CONVERT(nvarchar(12), MaxQuantity) END
FROM d_PriceUnit;

-- Replace max values from d_PriceUnit with NULL
UPDATE PricePointDetail
SET MaxValue = NULL
WHERE MaxValue IN ('999999', '999999.99')

GO

--
-- Remove min/max columns from d_PriceUnit
--
ALTER TABLE d_PriceUnit
DROP CONSTRAINT DF_d_PriceUnit_MinQuantity;

ALTER TABLE d_PriceUnit
DROP COLUMN MinQuantity;

GO

ALTER TABLE d_PriceUnit
DROP CONSTRAINT DF_d_PriceUnit_MaxQuantity;

ALTER TABLE d_PriceUnit
DROP COLUMN MaxQuantity;

GO

ALTER TABLE d_PriceUnit
DROP COLUMN MinWeight;

ALTER TABLE d_PriceUnit
DROP COLUMN MaxWeight;

GO

---
--- OrderProcess prices
---
ALTER TABLE OrderProcesses
ADD Amount smallmoney NULL;

GO
