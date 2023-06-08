--SQL Update to 22.1.1 from previous version

DECLARE @currentVersion nvarchar(50) = '22.1.1'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO



--Flag new parts from receiving (Added in 21.1.0.2 but SQL upgrade Script has not been running
IF NOT EXISTS (SELECT 1 FROM sys.columns 
          WHERE Name = N'CreatedInReceiving'
          AND Object_ID = Object_ID(N'Part'))
BEGIN
	ALTER TABLE dbo.Part ADD
		CreatedInReceiving bit NULL

	ALTER TABLE dbo.Part SET (LOCK_ESCALATION = TABLE)
	
END



-- BUG 43886 Hide Process on quote does not handle nulls
-- Update QuotePart_Process.ShowOnQuote to not allow nulls and default to 1
-- Also update existing records so ShowOnQuote is set to 1 

--Add value of 1 to the showonquotes for existing records
BEGIN TRANSACTION
UPDATE [dbo].[QuotePart_Process] 
	SET [ShowOnQuote] = 1 
WHERE [ShowOnQuote] IS NULL
GO
COMMIT


DECLARE @ConstraintName nvarchar(200)
--Find Constraint name
SELECT
@ConstraintName = dc.Name 
FROM sys.tables st
INNER JOIN sys.default_constraints dc ON st.object_id = dc.parent_object_id
INNER JOIN sys.columns co ON dc.parent_object_id = co.object_id AND co.column_id = dc.parent_column_id
WHERE dc.Name like '%QuotePart%ShowO%'
IF @ConstraintName IS NOT NULL
EXEC('ALTER TABLE QuotePart_Process DROP CONSTRAINT ' + @ConstraintName)
GO

--add constrain to set the default value for ShowOnQuote to 1 and not null
ALTER TABLE [dbo].[QuotePart_Process] 
ALTER COLUMN ShowOnQuote BIT NOT NULL;
Print N'Altered ShowOnQuote in QuotePart_Process table to not null';
GO

--Add Constraint for new default Value
ALTER TABLE dbo.QuotePart_Process ADD CONSTRAINT DF_QuotePart_Process_ShowOnQuote DEFAULT (1) FOR ShowOnQuote

ALTER TABLE dbo.QuotePart_Process SET (LOCK_ESCALATION = TABLE)
Print N'Altered ShowOnQuote in QuotePart_Process table to default to 1';
GO

----------------------------------------------------------------------------
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
IF NOT EXISTS (SELECT 1 FROM sys.columns 
          WHERE Name = N'Containers'
          AND Object_ID = Object_ID(N'Receiving'))
BEGIN
ALTER TABLE dbo.Receiving ADD
	Containers int NOT NULL CONSTRAINT DF_Receiving_Containers DEFAULT (0)

ALTER TABLE dbo.Receiving SET (LOCK_ESCALATION = TABLE)
	Print N'Containers Column add to Receiving.'
END

----------------------------------------------------------------------------

--PBI 43884 Add Req. Date at Receiving
--Add ReqDate column to Receiving table  
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
IF NOT EXISTS (SELECT 1 FROM sys.columns 
          WHERE Name = N'ReqDate'
          AND Object_ID = Object_ID(N'Receiving'))
BEGIN
ALTER TABLE dbo.Receiving ADD
	ReqDate datetime NULL

ALTER TABLE dbo.Receiving SET (LOCK_ESCALATION = TABLE)
	Print N'ReqDate Column add to Receiving.'
END

----------------------------------------------------------------------------

-- Add new securtiry role into SecurityRole to suppport racking parts
IF NOT EXISTS (SELECT * FROM [dbo].[SecurityRole] WHERE SecurityRoleID = 'RackParts')
BEGIN
INSERT INTO [dbo].[SecurityRole] (SecurityRoleID, Description, SecurityRoleCategoryID)
VALUES ('RackParts','Ability to use the Rack Parts tool','Order Processing');
	Print N'RackParts added to SecurityRoleID'
END

-- Add Input Type for SampleSet to d_InputType in support of new Sampling Control on process management 
IF NOT EXISTS (SELECT * FROM [dbo].[d_InputType] WHERE InputType = 'SampleSet')
BEGIN
INSERT INTO [d_InputType] (InputType, AllowInInspection)
VALUES ('SampleSet',1);
	Print N'Existing SampleSet Values set to 1'
END

----------------------------------------------------------------------------

-- Add new Label Type LabelType table for Receiving Containers Node in Label Editor
IF NOT EXISTS (SELECT * FROM [LabelType] WHERE LabelTypeID = 17)
BEGIN
	INSERT INTO [LabelType] (LabelTypeID, Name, Version)
	VALUES (17,'Receiving Container',1)
	Print N'Receiving Container added as LabelType'
END
IF NOT EXISTS (SELECT * FROM [LabelType] WHERE LabelTypeID = 18)
BEGIN
	INSERT INTO [LabelType] (LabelTypeID, Name, Version)
	VALUES (18,'Racking',1)
	Print N'Racking added as LabelType'
END

----------------------------------------------------------------------------

-- Insert Setting into the settings table for ShowCODOnTraveler
IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE SettingName = 'ShowCODOnTraveler')
BEGIN
INSERT INTO [ApplicationSettings] (SettingName, VALUE) 
VALUES ('ShowCODOnTraveler','False')
	Print N'ShowCODOnTraveler added as ApplicationSettings with False value'
END

----------------------------------------------------------------------------

--Create Table for ProcessAnswerSample to support the ability answer by multiple sample values
IF OBJECT_ID (N'dbo.ProcessAnswerSample',N'U') IS NULL
BEGIN
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[ProcessAnswerSample](
	[ProcessAnswerSampleID] [int] IDENTITY(1,1) NOT NULL,
	[OrderProcessAnswerID] [int] NOT NULL,
	[SampleValue] [decimal](10, 5) NOT NULL,
 CONSTRAINT [PK_ProcessAnswerSample] PRIMARY KEY CLUSTERED 
(
	[ProcessAnswerSampleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
	Print N'Created ProcessAnswerSample Table'
END	

IF NOT EXISTS (SELECT dc.Name FROM sys.tables st
INNER JOIN sys.default_constraints dc ON st.object_id = dc.parent_object_id
INNER JOIN sys.columns co ON dc.parent_object_id = co.object_id AND co.column_id = dc.parent_column_id
WHERE dc.Name = 'DF_ProcessAnswerSample_SampleValue')
BEGIN
ALTER TABLE [dbo].[ProcessAnswerSample] ADD  CONSTRAINT [DF_ProcessAnswerSample_SampleValue]  DEFAULT ((0)) FOR [SampleValue]
	Print N'Altered ProcessAnswerSample Table so SampleValue default is 0'
END	


IF NOT EXISTS (SELECT name FROM sys.foreign_keys AS f
INNER JOIN sys.foreign_key_columns AS fc ON f.OBJECT_ID = fc.constraint_object_id
WHERE name = 'FK_OrderProcessAnswer_Samples2')
BEGIN
	ALTER TABLE [dbo].[ProcessAnswerSample]  WITH CHECK ADD CONSTRAINT [FK_OrderProcessAnswer_Samples2] FOREIGN KEY([OrderProcessAnswerID])
REFERENCES [dbo].[OrderProcessAnswer] ([OrderProcesserAnswerID])
ON DELETE CASCADE

ALTER TABLE [dbo].[ProcessAnswerSample] CHECK CONSTRAINT [FK_OrderProcessAnswer_Samples2]
	Print N'Altered FK constraint on ProcessAnswerSample Table for OrderProcessAnswerID'
END	

----------------------------------------------------------------------------

--Create Table for InspectionAnswerSample to support the ability answer by multiple sample values
IF OBJECT_ID (N'dbo.InspectionAnswerSample',N'U') IS NULL
BEGIN
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[InspectionAnswerSample](
	[InspectionAnswerSampleID] [int] IDENTITY(1,1) NOT NULL,
	[PartInspectionAnswer] [int] NOT NULL,
	[SampleValue] [decimal](10, 5) NOT NULL,
 CONSTRAINT [PK_InspectionAnswerSample] PRIMARY KEY CLUSTERED 
(
	[InspectionAnswerSampleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
	Print N'Created Table InspectionAnswerSample'
END

GO

IF NOT EXISTS (SELECT dc.Name FROM sys.tables st
INNER JOIN sys.default_constraints dc ON st.object_id = dc.parent_object_id
INNER JOIN sys.columns co ON dc.parent_object_id = co.object_id AND co.column_id = dc.parent_column_id
WHERE dc.Name = 'DF_InspectionAnswerSample_SampleValue')
BEGIN
ALTER TABLE [dbo].[InspectionAnswerSample] ADD  CONSTRAINT [DF_InspectionAnswerSample_SampleValue]  DEFAULT ((0)) FOR [SampleValue]
	Print N'Set default to 0 for SampleValue in InspectionAnswerSample table'
END

IF NOT EXISTS (SELECT name FROM sys.foreign_keys AS f
INNER JOIN sys.foreign_key_columns AS fc ON f.OBJECT_ID = fc.constraint_object_id
WHERE name = 'FK_PartInspectionAnswer_Samples')
BEGIN
ALTER TABLE [dbo].[InspectionAnswerSample]  WITH CHECK ADD  CONSTRAINT [FK_PartInspectionAnswer_Samples] FOREIGN KEY([PartInspectionAnswer])
REFERENCES [dbo].[PartInspectionAnswer] ([PartInspectionAnswer])
ON DELETE CASCADE

ALTER TABLE [dbo].[InspectionAnswerSample] CHECK CONSTRAINT [FK_PartInspectionAnswer_Samples]
		Print N'Altered FK constraint on InspectionAnswerSample Table for PartInspectionAnswer'
END

----------------------------------------------------------------------------

--PBI 43883 Add bool to contact for approval notification for show s/n in subject for approvals
-- Add Column to Customer to store property of ShowSNinApprovalSubjectLine
IF NOT EXISTS (SELECT 1 FROM sys.columns 
          WHERE Name = N'ShowSNinApprovalSubjectLine'
          AND Object_ID = Object_ID(N'Customer'))
BEGIN
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION

ALTER TABLE dbo.Customer ADD
	ShowSNinApprovalSubjectLine [bit] NOT NULL CONSTRAINT DF_ShowSNinApprovalSubjectLine DEFAULT ((0))
ALTER TABLE dbo.Customer SET (LOCK_ESCALATION = TABLE)
	Print N'Set default to 0 for ShowSNinApprovalSubjectLine in Customer table'
END

----------------------------------------------------------------------------

--Add Setting for ApplyDefaultFees
IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE SettingName = 'ApplyDefaultFees')
BEGIN
INSERT INTO [ApplicationSettings] (SettingName, VALUE) 
VALUES ('ApplyDefaultFees','True')
	Print N'Added ApplicationSettings for ApplyDefaultFees set to true'
END

----------------------------------------------------------------------------

-- Update CreatedInReceiving Column in Parts to not be NULL and default to 0
BEGIN TRANSACTION
UPDATE [dbo].[Part] 
	SET [CreatedInReceiving] = 0 
WHERE [CreatedInReceiving] IS NULL
Print N'Set the value of CreatedInReceiving to 0 where null'
GO
COMMIT

BEGIN TRANSACTION
ALTER TABLE [dbo].[Part] ALTER COLUMN CreatedInReceiving BIT NOT NULL
	Print N'Set CreatedInReceiving on Part table to not null'
GO
COMMIT


DECLARE @ConstraintName nvarchar(200)
--Find Constraint name
SELECT
@ConstraintName = dc.Name 
FROM sys.tables st
INNER JOIN sys.default_constraints dc ON st.object_id = dc.parent_object_id
INNER JOIN sys.columns co ON dc.parent_object_id = co.object_id AND co.column_id = dc.parent_column_id
WHERE dc.Name like '%Part%CreatedIn%'
IF @ConstraintName IS NOT NULL
EXEC('ALTER TABLE Part DROP CONSTRAINT ' + @ConstraintName)

--Add Constraint for new default Value
ALTER TABLE Part ADD CONSTRAINT DF_Part_CreatedInReiving DEFAULT (0) FOR CreatedInReceiving
Print N'Set CreatedInReceiving on Part table to default to 0'
----------------------------------------------------------------------------


-- Add stored procedures 

-- Add FillByPartInspectionAnswer for InspectionAnswerSample
IF EXISTS (SELECT QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) AS Name
FROM   sys.all_objects
WHERE  Name = 'FillByPartInspectionAnswer')
BEGIN
	DROP PROCEDURE [dbo].[FillByPartInspectionAnswer]
END
go
CREATE PROCEDURE [dbo].FillByPartInspectionAnswer(@PartInspectionAnswer int)
AS
	SET NOCOUNT ON;
	SELECT [InspectionAnswerSample].SampleValue, [InspectionAnswerSample].PartInspectionAnswer
	FROM [dbo].[InspectionAnswerSample]
	WHERE [InspectionAnswerSample].PartInspectionAnswer = @PartInspectionAnswer
	
go

IF EXISTS (SELECT QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) AS Name
FROM   sys.all_objects
WHERE  Name = 'FillByPartInspectionAnswer')
	Print N'Created Procedure for FillByPartInspectionAnswer by @PartInspectionAnswer'
----------------------------------------------------------------------------

-- Add FindByOrderID for Orders
IF EXISTS (SELECT QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) AS Name
FROM   sys.all_objects
WHERE  Name = 'FindByOrderID')
BEGIN
	DROP PROCEDURE [dbo].[FindByOrderID]
END
GO
CREATE PROCEDURE [dbo].[FindByOrderID]
(
	@OrderID int
)
AS
	SET NOCOUNT ON;
SELECT     [Order].OrderID, [Order].OrderDate, [Order].RequiredDate, Customer.Name AS CustomerName, Part.Name AS PartName, [Order].Status, [Order].PurchaseOrder, [Order].CreatedBy, [Order].PartQuantity, [Order].WorkStatus, [Order].CurrentLocation, [Order].EstShipDate, [Order].CustomerWO, 
                  [Order].ReceivingID, [Order].Created, [Order].SalesOrderID, [Order].Weight AS OrderWeight, [Order].CurrentLine, Part.Material AS PartMtrl, Part.MaskingRequired, Part.Notes AS PartNotes, Customer.Notes AS CustomerNotes, Part.SurfaceArea AS PartSA, Part.Length AS PartLength, 
                  Part.Width AS PartWidth, Part.Height AS PartHeight, Part.Weight AS PartWeight, Part.Description AS PartDesc
FROM        [Order] LEFT OUTER JOIN
                  Part ON [Order].PartID = Part.PartID LEFT OUTER JOIN
                  Customer ON [Order].CustomerID = Customer.CustomerID
WHERE OrderID = @OrderID
GO
IF EXISTS (SELECT QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) AS Name
FROM   sys.all_objects
WHERE  Name = 'FindByOrderID')
	Print N'Created Procedure for FindByOrderID by @OrderID'
----------------------------------------------------------------------------

-- Add GetByPartInspectionAnswer for InspectionAnswerSample
IF EXISTS (SELECT QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) AS Name
FROM   sys.all_objects
WHERE  Name = 'GetByPartInspectionAnswer')
BEGIN
	DROP PROCEDURE [dbo].[GetByPartInspectionAnswer]
END
GO
CREATE PROCEDURE [dbo].[GetByPartInspectionAnswer]
(
	@PartInspectionAnswer int
)
AS
	SET NOCOUNT ON;
SELECT        PartInspectionAnswer, SampleValue
FROM            InspectionAnswerSample
WHERE PartInspectionAnswer  = @PartInspectionAnswer
GO
IF EXISTS (SELECT QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) AS Name
FROM   sys.all_objects
WHERE  Name = 'GetByPartInspectionAnswer')
	Print N'Created Procedure for GetByPartInspectionAnswer by @PartInspectionAnswer'
----------------------------------------------------------------------------

-- Add GetOrderProcessByDept for OrderProcesses
IF EXISTS (SELECT QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) AS Name
FROM   sys.all_objects
WHERE  Name = 'GetOrderProcessByDept')
BEGIN
	DROP PROCEDURE [dbo].[GetOrderProcessByDept]
END
GO
CREATE PROCEDURE [dbo].[GetOrderProcessByDept]
(
	@OrderID int,
	@Dept nvarchar(50)
)
AS
	SET NOCOUNT ON;
SELECT        [Order].OrderID, OrderProcesses.StepOrder, OrderProcesses.Department, Process.ProcessCode, ProcessAlias.Name AS AliasName, Process.Name AS ProcessName
FROM            OrderProcesses INNER JOIN
                         [Order] ON OrderProcesses.OrderID = [Order].OrderID INNER JOIN
                         Process ON OrderProcesses.ProcessID = Process.ProcessID INNER JOIN
                         ProcessAlias ON OrderProcesses.ProcessAliasID = ProcessAlias.ProcessAliasID
WHERE        ([Order].Status = 'Closed') AND ([Order].Invoice IS NULL OR
                         [Order].Invoice = '') AND ([Order].OrderID = @OrderID) AND (OrderProcesses.Department = @Dept)
GO
IF EXISTS (SELECT QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) AS Name
FROM   sys.all_objects
WHERE  Name = 'GetOrderProcessByDept')
	Print N'Created Procedure for GetOrderProcessByDept by @OrderID and @Dept'	
----------------------------------------------------------------------------




IF NOT EXISTS (SELECT 1 FROM sys.columns 
          WHERE Name = N'SampleSize'
          AND Object_ID = Object_ID(N'PartInspectionQuestion'))
BEGIN
ALTER TABLE dbo.PartInspectionQuestion ADD
	SampleSize nvarchar(255) NULL

ALTER TABLE dbo.PartInspectionQuestion SET (LOCK_ESCALATION = TABLE)
	Print N'Added SampleSize to PartInspectionQuestion Table '
END
