-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '18.1.2'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Process Suggestions
CREATE TABLE dbo.ProcessSuggestion
    (
    ProcessSuggestionID int NOT NULL IDENTITY (1, 1),
    PrimaryProcessID int NOT NULL,
    SuggestedProcessID int NOT NULL,
    SuggestedProcessAliasID int NOT NULL,
    Type nchar(4) NOT NULL,
    ConditionType nvarchar(12) NULL,
    ConditionOperator nchar(1) NULL,
    ConditionValue nvarchar(255) NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.ProcessSuggestion ADD CONSTRAINT
    PK_ProcessSuggestion PRIMARY KEY CLUSTERED 
    (
    ProcessSuggestionID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.ProcessSuggestion ADD CONSTRAINT
    FK_ProcessSuggestion_Process_Primary FOREIGN KEY
    (
    PrimaryProcessID
    ) REFERENCES dbo.Process
    (
    ProcessID
    ) ON UPDATE  CASCADE
     ON DELETE  CASCADE
    
GO
ALTER TABLE dbo.ProcessSuggestion ADD CONSTRAINT
    FK_ProcessSuggestion_Process_Suggested FOREIGN KEY
    (
    SuggestedProcessID
    ) REFERENCES dbo.Process
    (
    ProcessID
    ) ON UPDATE  NO ACTION
     ON DELETE  NO ACTION
    
GO
ALTER TABLE dbo.ProcessSuggestion ADD CONSTRAINT
    FK_ProcessSuggestion_ProcessAlias FOREIGN KEY
    (
    SuggestedProcessAliasID
    ) REFERENCES dbo.ProcessAlias
    (
    ProcessAliasID
    ) ON UPDATE  NO ACTION
     ON DELETE  NO ACTION
    
GO

-- Turn on COC for individual orders
ALTER TABLE dbo.Part ADD
    RequireCocByDefault bit NOT NULL CONSTRAINT DF_Part_RequireCocByDefault DEFAULT 0
GO

ALTER TABLE dbo.[Order] ADD
    RequireCoc bit NOT NULL CONSTRAINT DF_Order_RequireCoc DEFAULT 0
GO

-- Require/Show/Hide Assembly part field
INSERT INTO Fields(Name, Alias, Category, IsRequired, IsSystem, IsVisible, IsCustomer)
VALUES
(
    'Assembly', NULL, 'Part', 0, 1, 1, 0
);

GO

--
-- SchedulePriority field for Batch
--
ALTER TABLE dbo.Batch ADD
    SchedulePriority int NOT NULL CONSTRAINT DF_Batch_SchedulePriority DEFAULT 0
GO

-- Update Get_BatchStatus to include SchedulePriority
ALTER PROCEDURE [dbo].[Get_BatchStatus] 
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT        Batch.BatchID, Batch.OpenDate, Batch.Fixture, Batch.WorkStatus, Batch.CurrentLocation, dbo.fnGetNextDeptBatch(Batch.BatchID) AS NextDept, 
                         dbo.fnGetCurrentProcessBatch(Batch.BatchID) AS CurrentProcess, SUM(BatchOrder.PartQuantity) AS PartCount, COUNT(BatchOrder.BatchID) AS OrderCount, 
                         SUM(Part.SurfaceArea * BatchOrder.PartQuantity) AS TotalSurfaceArea, SUM(ISNULL([Order].Weight, Part.Weight * BatchOrder.PartQuantity)) AS TotalWeight,
              dbo.fnGetActiveTimerBatchCount(Batch.BatchID) AS ActiveTimerCount, Batch.CurrentLine, Batch.SchedulePriority
FROM            [Order] INNER JOIN
                         BatchOrder ON [Order].OrderID = BatchOrder.OrderID INNER JOIN
                         Part ON [Order].PartID = Part.PartID RIGHT OUTER JOIN
                         Batch ON BatchOrder.BatchID = Batch.BatchID
WHERE        (Batch.Active = 1)
GROUP BY Batch.BatchID, Batch.OpenDate, Batch.Fixture, Batch.WorkStatus, Batch.CurrentLocation, dbo.fnGetNextDeptBatch(Batch.BatchID), 
                         dbo.fnGetCurrentProcessBatch(Batch.BatchID), Batch.CurrentLine, Batch.SchedulePriority

END
GO

--
-- Part History
--
CREATE TABLE dbo.PartHistory
    (
    PartHistoryID int NOT NULL IDENTITY (1, 1),
    PartID int NOT NULL,
    Category nvarchar(50) NOT NULL,
    Description nvarchar(255) NOT NULL,
    UserName nvarchar(50) NOT NULL,
    Machine nvarchar(50) NOT NULL,
    DateCreated datetime2(7) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.PartHistory ADD CONSTRAINT
    DF_PartHistory_DateCreated DEFAULT GETDATE() FOR DateCreated
GO
ALTER TABLE dbo.PartHistory ADD CONSTRAINT
    PK_PartHistory PRIMARY KEY CLUSTERED 
    (
    PartHistoryID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.PartHistory ADD CONSTRAINT
    FK_PartHistory_Part FOREIGN KEY
    (
    PartID
    ) REFERENCES dbo.Part
    (
    PartID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
    
GO

-- Add note for pre-existing parts
DECLARE @currentTime datetime2 = GETDATE();

INSERT INTO PartHistory(PartID, Category, Description, UserName, Machine, DateCreated)
SELECT PartID, 'Note', 'Part was created before part history update.', 'Server', 'N/A', @currentTime
FROM Part;

GO

--
-- Mark first shipment package type as system-defined.
-- Assumption: A user has not already deleted it.
--
ALTER TABLE ShipmentPackageType ADD
    SystemDefined bit NOT NULL CONSTRAINT DF_ShipmentPackageType_SystemDefined DEFAULT 0
GO

UPDATE ShipmentPackageType
SET SystemDefined = 1
WHERE ShipmentPackageTypeID = 1;

--
-- Add types for order containers
--
ALTER TABLE dbo.OrderContainers ADD
    ShipmentPackageTypeID int NOT NULL CONSTRAINT DF_OrderContainers_ShipmentPackageTypeID DEFAULT 1
GO

ALTER TABLE dbo.OrderContainers ADD CONSTRAINT
    FK_OrderContainers_ShipmentPackageType FOREIGN KEY
    (
    ShipmentPackageTypeID
    ) REFERENCES dbo.ShipmentPackageType
    (
    ShipmentPackageTypeID
    ) ON UPDATE NO ACTION
     ON DELETE NO ACTION

GO

--
-- Nested items for order containers - '3 crates ON 1 skid'
--
CREATE TABLE dbo.OrderContainerItem
    (
    OrderContainerItemID int NOT NULL IDENTITY (1, 1),
    OrderContainerID int NOT NULL,
    ShipmentPackageTypeID int NOT NULL
    )  ON [PRIMARY]
GO

ALTER TABLE dbo.OrderContainerItem ADD CONSTRAINT
    PK_OrderContainerItem PRIMARY KEY CLUSTERED 
    (
    OrderContainerItemID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE dbo.OrderContainerItem ADD CONSTRAINT
    FK_OrderContainerItem_OrderContainers FOREIGN KEY
    (
    OrderContainerID
    ) REFERENCES dbo.OrderContainers
    (
    OrderContainerID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
GO

ALTER TABLE dbo.OrderContainerItem ADD CONSTRAINT
    FK_OrderContainerItem_ShipmentPackageType FOREIGN KEY
    (
    ShipmentPackageTypeID
    ) REFERENCES dbo.ShipmentPackageType
    (
    ShipmentPackageTypeID
    ) ON UPDATE  NO ACTION 
     ON DELETE  NO ACTION 
GO

--
-- Require/Show/Hide Containers order field
--
INSERT INTO Fields(Name, Alias, Category, IsRequired, IsSystem, IsVisible, IsCustomer)
VALUES
(
    'Container', NULL, 'Order', 0, 1, 0, 0
);

GO

--
-- fnGetOrderNoteCount for WIP screen
--
CREATE FUNCTION [dbo].[fnGetOrderNoteCount]
(
    @orderID int
)
RETURNS int
AS
BEGIN
   RETURN (SELECT COUNT(*) FROM OrderNote WHERE OrderID = @orderID)
END

GO

--
-- Allow weight-based process pricing
--
ALTER TABLE PartProcessVolumePrice
ADD MinValue nvarchar(12) NULL,
    MaxValue nvarchar(12) NULL;

GO

UPDATE PartProcessVolumePrice
SET MinValue = CAST(MinQuantity as nvarchar(12)),
MaxValue = CAST(MaxQuantity AS nvarchar(12));

GO

ALTER TABLE PartProcessVolumePrice
DROP COLUMN MinQuantity, MaxQuantity;

GO

--
-- Bill of Lading
--
CREATE TABLE dbo.BillOfLading
    (
    BillOfLadingID int NOT NULL IDENTITY (1, 1),
    DateCreated datetime2(7) NOT NULL,
    ShipmentPackageID int NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.BillOfLading ADD CONSTRAINT
    PK_BillOfLading PRIMARY KEY CLUSTERED 
    (
    BillOfLadingID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.BillOfLading ADD CONSTRAINT
    FK_BillOfLading_ShipmentPackage FOREIGN KEY
    (
    ShipmentPackageID
    ) REFERENCES dbo.ShipmentPackage
    (
    ShipmentPackageID
    ) ON UPDATE  CASCADE
     ON DELETE  CASCADE 
    
GO

CREATE TABLE dbo.BillOfLadingOrder
    (
    BillOfLadingOrderID int NOT NULL IDENTITY (1, 1),
    BillOfLadingID int NOT NULL,
    OrderID int NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.BillOfLadingOrder ADD CONSTRAINT
    PK_BillOfLadingOrder PRIMARY KEY CLUSTERED 
    (
    BillOfLadingOrderID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.BillOfLadingOrder ADD CONSTRAINT
    FK_BillOfLadingOrder_BillOfLading FOREIGN KEY
    (
    BillOfLadingID
    ) REFERENCES dbo.BillOfLading
    (
    BillOfLadingID
    ) ON UPDATE  CASCADE
     ON DELETE  CASCADE 
    
GO
ALTER TABLE dbo.BillOfLadingOrder ADD CONSTRAINT
    FK_BillOfLadingOrder_Order FOREIGN KEY
    (
    OrderID
    ) REFERENCES dbo.[Order]
    (
    OrderID
    ) ON UPDATE  CASCADE
     ON DELETE  CASCADE 
    
GO

-- Media
CREATE TABLE dbo.BillOfLadingMedia
    (
    BillOfLadingID int NOT NULL,
    MediaID int NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.BillOfLadingMedia ADD CONSTRAINT
    PK_BillOfLadingMedia PRIMARY KEY CLUSTERED 
    (
    BillOfLadingID,
    MediaID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.BillOfLadingMedia ADD CONSTRAINT
    FK_BillOfLadingMedia_BillOfLading FOREIGN KEY
    (
    BillOfLadingID
    ) REFERENCES dbo.BillOfLading
    (
    BillOfLadingID
    ) ON UPDATE  CASCADE
     ON DELETE  CASCADE
    
GO
ALTER TABLE dbo.BillOfLadingMedia ADD CONSTRAINT
    FK_BillOfLadingMedia_Media FOREIGN KEY
    (
    MediaID
    ) REFERENCES dbo.Media
    (
    MediaID
    ) ON UPDATE  CASCADE
     ON DELETE  CASCADE
    
GO


-- Stored procedure for deleting unused media (added BillOfLadingMedia.MediaID)
ALTER PROCEDURE [dbo].[Delete_UnusedMedia]
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @rows INT

  SET @rows = (SELECT count(*)
               FROM
                 Media)

  DELETE
  FROM
    Media
  WHERE
    FileExtension != 'PartTemp' AND
    MediaID IN (SELECT m.MediaID
                FROM
                  Media m
                WHERE
                NOT EXISTS (SELECT *
                              FROM
                                Labels lbl
                              WHERE
                                m.MediaID = lbl.MediaID)
                  AND
                NOT EXISTS (SELECT *
                              FROM
                                LabelType lt
                              WHERE
                                m.MediaID = lt.MediaID)
                  AND
                 NOT EXISTS (SELECT *
                              FROM
                                Receiving_Media rm
                              WHERE
                                m.MediaID = rm.MediaID)
                  AND
                  NOT EXISTS (SELECT *
                              FROM
                                Part_Media pm
                              WHERE
                                m.MediaID = pm.MediaID)
                  AND
                  NOT EXISTS (SELECT *
                              FROM
                                [Order_Media] o
                              WHERE
                                m.MediaID = o.MediaID)
                  AND
                  NOT EXISTS (SELECT *
                              FROM
                                [SalesOrder_Media] so
                              WHERE
                                m.MediaID = so.MediaID)
                  AND
                  NOT EXISTS (SELECT *
                              FROM
                                [QuotePart_Media] o
                              WHERE
                                m.MediaID = o.MediaID)
                  AND
                  NOT EXISTS (SELECT *
                              FROM
                                [Users] u
                              WHERE
                                m.MediaID = u.MediaID
                                OR m.MediaID = u.SignatureMediaID)
                  AND
                  NOT EXISTS (SELECT *
                              FROM
                                [ProductClassLabels] pcl
                              WHERE
                                m.MediaID = pcl.MediaID)
                  AND
                  NOT EXISTS (SELECT *
                              FROM
                                [BillOfLadingMedia] bol
                              WHERE
                                m.MediaID = bol.MediaID))

  SELECT @rows - (SELECT count(*)
                  FROM
                    Media)
END
GO
