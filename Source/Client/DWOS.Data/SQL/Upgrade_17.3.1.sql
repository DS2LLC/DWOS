-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '17.3.1'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Show/Hide Questions in Process Answer Report
ALTER TABLE dbo.ProcessQuestion ADD
    IncludeInProcessGroup bit NOT NULL CONSTRAINT DF_ProcessQuestion_IncludeInProcessGroup DEFAULT 0
GO

--
-- Labels for Product Classes
--
CREATE TABLE dbo.ProductClassLabels
    (
    ProductClassLabelID int NOT NULL IDENTITY (1, 1),
    ProductClass nvarchar(255) NULL,
    LabelType int NOT NULL,
    Data nvarchar(MAX) NULL,
    MediaID int NULL,
    Version int NOT NULL CONSTRAINT [DF_ProductClassLabels_Version]  DEFAULT ((0))
    )  ON [PRIMARY]
     TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.ProductClassLabels ADD CONSTRAINT
    PK_ProductClassLabels PRIMARY KEY CLUSTERED 
    (
    ProductClassLabelID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.ProductClassLabels ADD CONSTRAINT
    FK_ProductClassLabels_LabelType FOREIGN KEY
    (
    LabelType
    ) REFERENCES dbo.LabelType
    (
    LabelTypeID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
    
GO
ALTER TABLE dbo.ProductClassLabels ADD CONSTRAINT
    FK_ProductClassLabels_Media FOREIGN KEY
    (
    MediaID
    ) REFERENCES dbo.Media
    (
    MediaID
    ) ON UPDATE  NO ACTION 
     ON DELETE  CASCADE 
    
GO

-- Stored procedure for deleting unused media (added ProductClassLabels.MediaID)
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
                                m.MediaID = pcl.MediaID))

  SELECT @rows - (SELECT count(*)
                  FROM
                    Media)
END
GO

--
-- Optionally restrict Processing Line to a single Department
--
ALTER TABLE dbo.ProcessingLine ADD
    DepartmentId nvarchar(50) NULL
GO
ALTER TABLE dbo.ProcessingLine ADD CONSTRAINT
    FK_ProcessingLine_d_Department FOREIGN KEY
    (
    DepartmentId
    ) REFERENCES dbo.d_Department
    (
    DepartmentID
    ) ON UPDATE CASCADE
    ON DELETE SET NULL
GO

--
-- Revisions for part inspections
--
ALTER TABLE dbo.PartInspectionType ADD
    Active bit NOT NULL CONSTRAINT DF_PartInspectionType_Active DEFAULT 1,
    Revision nvarchar(50) NULL,
    ParentID int NULL
GO

ALTER TABLE dbo.PartInspectionType ADD CONSTRAINT
    FK_PartInspectionType_PartInspectionType FOREIGN KEY
    (
    ParentID
    ) REFERENCES dbo.PartInspectionType
    (
    PartInspectionTypeID
    ) ON UPDATE NO ACTION
    ON DELETE NO ACTION
GO

--
-- Part Marking for individual parts instead of airframe
--
CREATE TABLE dbo.Part_PartMarking
    (
    Part_PartMarkingID int NOT NULL IDENTITY (1, 1),
    PartID int NOT NULL,
    ProcessSpec nvarchar(50) NULL,
    Def1 nvarchar(MAX) NULL,
    Def2 nvarchar(MAX) NULL,
    Def3 nvarchar(MAX) NULL,
    Def4 nvarchar(MAX) NULL
    )  ON [PRIMARY]
    TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE dbo.Part_PartMarking ADD CONSTRAINT
    PK_Part_PartMarking PRIMARY KEY CLUSTERED 
    (
    Part_PartMarkingID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE dbo.Part_PartMarking ADD CONSTRAINT
    FK_Part_PartMarking_Part FOREIGN KEY
    (
    PartID
    ) REFERENCES dbo.Part
    (
    PartID
    ) ON UPDATE CASCADE
      ON DELETE CASCADE
GO

--
-- Customer-Specific Price Points
--
CREATE TABLE dbo.CustomerPricePoint
    (
    CustomerPricePointID int NOT NULL IDENTITY (1, 1),
    Type nvarchar(10) NULL,
    CustomerID int NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.CustomerPricePoint ADD CONSTRAINT
    PK_CustomerPricePoint PRIMARY KEY CLUSTERED 
    (
    CustomerPricePointID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.CustomerPricePoint ADD CONSTRAINT
    FK_CustomerPricePoint_Customer FOREIGN KEY
    (
    CustomerID
    ) REFERENCES dbo.Customer
    (
    CustomerID
    ) ON UPDATE CASCADE
      ON DELETE CASCADE
GO

CREATE TABLE dbo.CustomerPricePointDetail
    (
    CustomerPricePointDetailID int NOT NULL IDENTITY (1, 1),
    CustomerPricePointID int NOT NULL,
    PriceUnit nvarchar(50) NOT NULL,
    MinValue nvarchar(12) NOT NULL,
    MaxValue nvarchar(12) NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.CustomerPricePointDetail ADD CONSTRAINT
    PK_CustomerPricePointDetail PRIMARY KEY CLUSTERED 
    (
    CustomerPricePointDetailID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.CustomerPricePointDetail ADD CONSTRAINT
    FK_CustomerPricePointDetail_CustomerPricePoint FOREIGN KEY
    (
    CustomerPricePointID
    ) REFERENCES dbo.CustomerPricePoint
    (
    CustomerPricePointID
    ) ON UPDATE CASCADE
      ON DELETE CASCADE
GO

ALTER TABLE dbo.CustomerPricePointDetail ADD CONSTRAINT
    FK_CustomerPricePointDetail_d_PriceUnit FOREIGN KEY
    (
    PriceUnit
    ) REFERENCES dbo.d_PriceUnit
    (
    PriceUnitID
    ) ON UPDATE NO ACTION
      ON DELETE NO ACTION
GO

--
-- Automated Work Order Tool
--

-- OSP Format
CREATE TABLE dbo.OSPFormat
    (
    OSPFormatID int NOT NULL IDENTITY (1, 1),
    CustomerID int NOT NULL,
    ManufacturerID nvarchar(50) NOT NULL,
    Code nvarchar(2) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.OSPFormat ADD CONSTRAINT
    PK_OSPFormat PRIMARY KEY CLUSTERED 
    (
    OSPFormatID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.OSPFormat ADD CONSTRAINT
    FK_OSPFormat_Customer FOREIGN KEY
    (
    CustomerID
    ) REFERENCES dbo.Customer
    (
    CustomerID
    ) ON UPDATE CASCADE
     ON DELETE NO ACTION
GO
ALTER TABLE dbo.OSPFormat ADD CONSTRAINT
    FK_OSPFormat_d_Manufacturer FOREIGN KEY
    (
    ManufacturerID
    ) REFERENCES dbo.d_Manufacturer
    (
    ManufacturerID
    ) ON UPDATE CASCADE
     ON DELETE NO ACTION
GO

-- OSP Format Section
CREATE TABLE dbo.OSPFormatSection
    (
    OSPFormatSectionID int NOT NULL IDENTITY (1, 1),
    OSPFormatID int NOT NULL,
    Role nvarchar(8) NOT NULL,
    DepartmentID nvarchar(50) NOT NULL,
    SectionOrder int NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.OSPFormatSection ADD CONSTRAINT
    PK_OSPFormatSection PRIMARY KEY CLUSTERED 
    (
    OSPFormatSectionID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.OSPFormatSection ADD CONSTRAINT
    FK_OSPFormatSection_OSPFormat FOREIGN KEY
    (
    OSPFormatID
    ) REFERENCES dbo.OSPFormat
    (
    OSPFormatID
    ) ON UPDATE CASCADE
     ON DELETE CASCADE
    
GO
ALTER TABLE dbo.OSPFormatSection ADD CONSTRAINT
    FK_OSPFormatSection_d_Department FOREIGN KEY
    (
    DepartmentID
    ) REFERENCES dbo.d_Department
    (
    DepartmentID
    ) ON UPDATE CASCADE
     ON DELETE NO ACTION
GO

-- OSP Format Section - Process
CREATE TABLE dbo.OSPFormatSectionProcess
    (
    OSPFormatSectionProcessID int NOT NULL IDENTITY (1, 1),
    OSPFormatSectionID int NOT NULL,
    Code nvarchar(10) NOT NULL,
    ProcessID int NOT NULL,
    ProcessAliasID int NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.OSPFormatSectionProcess ADD CONSTRAINT
    PK_OSPFormatSectionProcess PRIMARY KEY CLUSTERED 
    (
    OSPFormatSectionProcessID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.OSPFormatSectionProcess ADD CONSTRAINT
    FK_OSPFormatSectionProcess_OSPFormatSection FOREIGN KEY
    (
    OSPFormatSectionID
    ) REFERENCES dbo.OSPFormatSection
    (
    OSPFormatSectionID
    ) ON UPDATE CASCADE
     ON DELETE CASCADE
GO
ALTER TABLE dbo.OSPFormatSectionProcess ADD CONSTRAINT
    FK_OSPFormatSectionProcess_Process FOREIGN KEY
    (
    ProcessID
    ) REFERENCES dbo.Process
    (
    ProcessID
    ) ON UPDATE  NO ACTION
     ON DELETE  NO ACTION
GO
ALTER TABLE dbo.OSPFormatSectionProcess ADD CONSTRAINT
    FK_OSPFormatSectionProcess_ProcessAlias FOREIGN KEY
    (
    ProcessAliasID
    ) REFERENCES dbo.ProcessAlias
    (
    ProcessAliasID
    ) ON UPDATE  NO ACTION
     ON DELETE  NO ACTION
GO

-- OSP Format Section - Part Mark
CREATE TABLE dbo.OSPFormatSectionPartMark
    (
    OSPFormatSectionPartMarkID int NOT NULL IDENTITY (1, 1),
    OSPFormatSectionID int NOT NULL,
    Code nvarchar(10) NOT NULL,
    ProcessSpec nvarchar(50) NULL,
    Def1 nvarchar(MAX) NULL,
    Def2 nvarchar(MAX) NULL,
    Def3 nvarchar(MAX) NULL,
    Def4 nvarchar(MAX) NULL
    )  ON [PRIMARY]
     TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.OSPFormatSectionPartMark ADD CONSTRAINT
    PK_OSPFormatSectionPartMark PRIMARY KEY CLUSTERED 
    (
    OSPFormatSectionPartMarkID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.OSPFormatSectionPartMark ADD CONSTRAINT
    FK_OSPFormatSectionPartMark_OSPFormatSection FOREIGN KEY
    (
    OSPFormatSectionID
    ) REFERENCES dbo.OSPFormatSection
    (
    OSPFormatSectionID
    ) ON UPDATE  CASCADE
     ON DELETE  CASCADE
GO

-- Permissions
INSERT INTO [dbo].[SecurityRoleCategory]([SecurityRoleCategoryID])
    VALUES ('AWOT')
GO

INSERT INTO [dbo].[SecurityRole]([SecurityRoleID],[Description],[SecurityRoleCategoryID])
    VALUES ('AWOT.MasterList','Ability to enter code breakdown and master list data into AWOT.', 'AWOT')
GO

INSERT INTO [dbo].[SecurityRole]([SecurityRoleID],[Description],[SecurityRoleCategoryID])
    VALUES ('AWOT.CreateOrders','Ability to enter shipping manifest data into AWOT.', 'AWOT')
GO

--- Default part pricing
CREATE TABLE dbo.CustomerDefaultPrice
    (
    CustomerDefaultPriceID int NOT NULL IDENTITY (1, 1),
    CustomerID int NOT NULL,
    PriceUnit nvarchar(50) NOT NULL,
    DefaultPrice decimal(11, 5) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.CustomerDefaultPrice ADD CONSTRAINT
    PK_CustomerDefaultPrice PRIMARY KEY CLUSTERED 
    (
    CustomerDefaultPriceID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.CustomerDefaultPrice ADD CONSTRAINT
    FK_CustomerDefaultPrice_Customer FOREIGN KEY
    (
    CustomerID
    ) REFERENCES dbo.Customer
    (
    CustomerID
    ) ON UPDATE  CASCADE
     ON DELETE  CASCADE
    
GO
ALTER TABLE dbo.CustomerDefaultPrice ADD CONSTRAINT
    FK_CustomerDefaultPrice_d_PriceUnit FOREIGN KEY
    (
    PriceUnit
    ) REFERENCES dbo.d_PriceUnit
    (
    PriceUnitID
    ) ON UPDATE  NO ACTION
     ON DELETE  NO ACTION
    
GO

-- 'Added from shipping manifest' indicator for orders
ALTER TABLE dbo.[Order] ADD
    FromShippingManifest bit NOT NULL CONSTRAINT DF_Order_FromShippingManifest DEFAULT 0
GO

--
-- 'Pending Rework Assessment' work status
--
INSERT INTO d_WorkStatus(WorkStatusID)
VALUES
(
    'Pending Rework Assessment'
);
