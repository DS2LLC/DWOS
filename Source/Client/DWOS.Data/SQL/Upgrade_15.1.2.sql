-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '15.1.2'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
	INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
	UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Fix the issue where ProcessStepCondition was wired to mismatch question and process due to copy and paste issue
DELETE FROM ProcessStepCondition WHERE ProcessStepConditionId IN
(
-- SELECT Wrong Process ID to Wrong Process 
SELECT        ProcessStepCondition.ProcessStepConditionId
FROM            ProcessStepCondition INNER JOIN
                         ProcessSteps ON ProcessStepCondition.ProcessStepId = ProcessSteps.ProcessStepID INNER JOIN
                         ProcessQuestion ON ProcessStepCondition.ProcessQuestionId = ProcessQuestion.ProcessQuestionID INNER JOIN
                         ProcessSteps AS ProcessSteps_1 ON ProcessQuestion.ProcessStepID = ProcessSteps_1.ProcessStepID AND 
                         ProcessSteps.ProcessID <> ProcessSteps_1.ProcessID)
GO

-- Allow users to sign COCs with their own signatures
ALTER TABLE dbo.Users ADD
    SignCOC bit NULL,
    SignatureMediaID int NULL
GO
ALTER TABLE dbo.Users ADD CONSTRAINT
    DF_Users_SignCOC DEFAULT 0 FOR SignCOC
GO
ALTER TABLE dbo.Users ADD CONSTRAINT FK_Users_SignatureMedia FOREIGN KEY (SignatureMediaID) REFERENCES dbo.Media (MediaID)
GO

-- Workaround for inability to specify 'ON DELETE SET NULL' for FK_Users_SignatureMedia
CREATE TRIGGER dbo.Media_SetSignatureMediaID_Delete
    ON dbo.Media
    AFTER DELETE
AS
UPDATE dbo.Users
    SET SignatureMediaID = NULL
    FROM dbo.Users WHERE dbo.Users.SignatureMediaID IN (SELECT deleted.MediaID FROM deleted)
GO

-- Stored procedure for deleting unused media (added Users.SignatureMediaID)
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
                                OR m.MediaID = u.SignatureMediaID))

  SELECT @rows - (SELECT count(*)
                  FROM
                    Media)
END
GO

-- COCSign permission
INSERT INTO [dbo].[SecurityRole]([SecurityRoleID], [Description], [SecurityRoleCategoryID])
    VALUES ('COCSign', 'Ability to sign COCs.', 'Quality')
GO

-- PartArea Table
CREATE TABLE dbo.PartArea
    (
    PartAreaID int NOT NULL IDENTITY(1,1),
    PartID int NOT NULL,
    ShapeType nvarchar(255) NOT NULL,
    ExclusionSurfaceArea float(53) NOT NULL,
    GrossSurfaceArea float(53) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.PartArea ADD CONSTRAINT
    PK_PartArea PRIMARY KEY CLUSTERED 
    (
    PartAreaID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- PartAreaDimension Table
CREATE TABLE dbo.PartAreaDimension
    (
    PartAreaDimensionID int NOT NULL IDENTITY(1,1),
    PartAreaID int NOT NULL,
    DimensionName nvarchar(50) NOT NULL,
    Dimension float(53) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.PartAreaDimension ADD CONSTRAINT
    PK_PartAreaDimension PRIMARY KEY CLUSTERED 
    (
    PartAreaDimensionID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- PartArea & PartAreaDimension Relationships
ALTER TABLE dbo.PartAreaDimension ADD CONSTRAINT
    FK_PartAreaDimension_PartArea FOREIGN KEY
    (
    PartAreaID
    ) REFERENCES dbo.PartArea
    (
    PartAreaID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
GO

ALTER TABLE dbo.PartArea ADD CONSTRAINT
    FK_PartArea_Part FOREIGN KEY
    (
    PartID
    ) REFERENCES dbo.Part
    (
    PartID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
GO

-- QuotePartArea Table
CREATE TABLE dbo.QuotePartArea
    (
    QuotePartAreaID int NOT NULL IDENTITY(1,1),
    QuotePartID int NOT NULL,
    ShapeType nvarchar(255) NOT NULL,
    ExclusionSurfaceArea float(53) NOT NULL,
    GrossSurfaceArea float(53) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.QuotePartArea ADD CONSTRAINT
    PK_QuotePartArea PRIMARY KEY CLUSTERED 
    (
    QuotePartAreaID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- QuotePartAreaDimension Table
CREATE TABLE dbo.QuotePartAreaDimension
    (
    QuotePartAreaDimensionID int NOT NULL IDENTITY(1,1),
    QuotePartAreaID int NOT NULL,
    DimensionName nvarchar(50) NOT NULL,
    Dimension float(53) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.QuotePartAreaDimension ADD CONSTRAINT
    PK_QuotePartAreaDimension PRIMARY KEY CLUSTERED 
    (
    QuotePartAreaDimensionID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- QuotePartArea & QuotePartAreaDimension Relationships
ALTER TABLE dbo.QuotePartAreaDimension ADD CONSTRAINT
    FK_QuotePartAreaDimension_QuotePartArea FOREIGN KEY
    (
    QuotePartAreaID
    ) REFERENCES dbo.QuotePartArea
    (
    QuotePartAreaID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
GO

ALTER TABLE dbo.QuotePartArea ADD CONSTRAINT
    FK_QuotePartArea_QuotePart FOREIGN KEY
    (
    QuotePartID
    ) REFERENCES dbo.QuotePart
    (
    QuotePartID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
GO

-- Process.IsApproved row
ALTER TABLE dbo.Process ADD
    IsApproved bit NULL
GO

-- All active processes need to be approved.
-- All non-active processes need to be unapproved.
UPDATE dbo.Process
    SET IsApproved = Active
GO

-- QuotePart_Process Table
CREATE TABLE dbo.QuotePart_Process
    (
    QuotePartProcessID int NOT NULL IDENTITY (1, 1),
    QuotePartID int NOT NULL,
    ProcessID int NOT NULL,
    ProcessAliasID int NOT NULL,
    StepOrder int NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.QuotePart_Process ADD CONSTRAINT
    PK_QuotePart_Process PRIMARY KEY CLUSTERED 
    (
    QuotePartProcessID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

-- QuotePart_Process Relationships
ALTER TABLE dbo.QuotePart_Process ADD CONSTRAINT
    FK_QuotePart_Process_QuotePart FOREIGN KEY
    (
    QuotePartID
    ) REFERENCES dbo.QuotePart
    (
    QuotePartID
    ) ON UPDATE  NO ACTION 
     ON DELETE  CASCADE 
    
GO
ALTER TABLE dbo.QuotePart_Process ADD CONSTRAINT
    FK_QuotePart_Process_Process FOREIGN KEY
    (
    ProcessID
    ) REFERENCES dbo.Process
    (
    ProcessID
    ) ON UPDATE  NO ACTION 
     ON DELETE  CASCADE 
    
GO
ALTER TABLE dbo.QuotePart_Process ADD CONSTRAINT
    FK_QuotePart_Process_ProcessAlias FOREIGN KEY
    (
        ProcessAliasID
    ) REFERENCES dbo.ProcessAlias
    (
        ProcessAliasID
    )
GO

--
-- Add 'Default' process category for merge script.
--
IF NOT EXISTS (SELECT 1 FROM d_ProcessCategory WHERE [d_ProcessCategory].ProcessCategory = 'Default')
    INSERT INTO d_ProcessCategory(ProcessCategory, LeadTime)
    VALUES
    (
        'Default',
        1.0
    )
GO

--
-- Migration from QuoteProcesses to planned Processes.
--

-- Temp table to link Processes and the QuoteProcesses that they're made from.
CREATE TABLE #TEMP_Process_QuoteProcess 
    (
    ProcessID int NOT NULL,
    QuoteProcessID int NOT NULL
    )
GO

-- Temp table to link ProcessAliases to QuoteProcesses.
-- The ProcessID column exists to avoid an INNER JOIN
-- during the final 'Insert QuotePart_Process' step.
CREATE TABLE #TEMP_ProcessAlias_QuoteProcess 
    (
    ProcessID int NOT NULL,
    ProcessAliasID int NOT NULL,
    QuoteProcessID int NOT NULL,
    )
GO

-- Doing a merge whose match always fails allows an OUTPUT
-- statement to contain a non-inserted value.
-- http://stackoverflow.com/questions/15281814/insert-output-including-column-from-other-table 

-- Create processes & populate temp table
DECLARE @salesDepartmentID nvarchar(50)
SET @salesDepartmentID = (SELECT TOP 1 DepartmentID FROM d_Department WHERE SystemName = 'Sales')

MERGE INTO Process
USING QuoteProcess
ON 1 = 0
WHEN NOT MATCHED THEN
    INSERT (Name,
            Description,
            Frozen,
            Revision,
            Active,
            Department,
            Category,
            IsPaperless,
            IsApproved)
    VALUES (QuoteProcess.Name,
            'Quote Process Import',
            0,
            '<None>',
            1,
            @salesDepartmentID,
            'Default',
            1,
            0)
OUTPUT inserted.ProcessID, QuoteProcess.QuoteProcessID
    INTO #TEMP_Process_QuoteProcess;
GO

-- Create necessary process aliases & populate temp table.
MERGE INTO ProcessAlias
USING #TEMP_Process_QuoteProcess
    INNER JOIN QuoteProcess ON #TEMP_Process_QuoteProcess.QuoteProcessID = QuoteProcess.QuoteProcessID
ON 1 = 0
WHEN NOT MATCHED THEN
    INSERT (ProcessID, Name)
    VALUES (#TEMP_Process_QuoteProcess.ProcessID,
            QuoteProcess.Name)
OUTPUT inserted.ProcessID, inserted.ProcessAliasID, QuoteProcess.QuoteProcessID 
    INTO #TEMP_ProcessAlias_QuoteProcess;
GO

-- Establish QuotePart_Process relationship for new processes.
INSERT INTO QuotePart_Process (QuotePartID, ProcessID, ProcessAliasID, StepOrder)
SELECT QuotePart_QuoteProcess.QuotePartID,
       #TEMP_ProcessAlias_QuoteProcess.ProcessID,
       #TEMP_ProcessAlias_QuoteProcess.ProcessAliasID,
       QuotePart_QuoteProcess.StepOrder
FROM #TEMP_ProcessAlias_QuoteProcess
    INNER JOIN QuotePart_QuoteProcess ON #TEMP_ProcessAlias_QuoteProcess.QuoteProcessID = QuotePart_QuoteProcess.QuoteProcessID
GO

DROP TABLE #TEMP_ProcessAlias_QuoteProcess;
GO

DROP TABLE #TEMP_Process_QuoteProcess
GO

--
-- Rename tables related to quote processes.
--
sp_rename 'QuotePart_QuoteProcess', 'old_QuotePart_QuoteProcess'
GO

sp_rename 'QuoteProcess', 'old_QuoteProcess'
GO

--
-- Remove unused 'QuoteProcessManager' security role.
--
DELETE
FROM SecurityGroup_Role
WHERE SecurityRoleID = 'QuoteProcessManager'
GO

DELETE
FROM SecurityRole
WHERE SecurityRoleID = 'QuoteProcessManager'
GO