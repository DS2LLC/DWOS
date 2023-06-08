

/********************************************************************************
*	Part.LastModified(DateTime)
*		Add column to hold last modified, move from the existing Modified Column
*
********************************************************************************/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Part ADD
	LastModified smalldatetime NULL
GO
ALTER TABLE dbo.Part ADD CONSTRAINT
	DF_Part_LastModified DEFAULT getdate() FOR LastModified
GO
ALTER TABLE dbo.Part SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

-- Move all of the existing
UPDATE Part SET LastModified = Modified
GO


/********************************************************************************
*	[Get_IsUserUsed]
*		Create to determine if the user is used in any other table.
*
********************************************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Get_IsUserUsed]
	@userID int,
	@isUsed bit output
AS
BEGIN
	SET NOCOUNT ON;

	SET @isUsed = 1
	
	-- If is in COC
    IF (SELECT COUNT(*) FROM [COC] WHERE COC.QAUser = @userID) > 0
		RETURN 1
		
	-- If is in Order
    IF (SELECT COUNT(*) FROM [Order] WHERE [Order].CreatedBy = @userID) > 0
		RETURN 1

	-- If is in [CustomerCommunication]
    IF (SELECT COUNT(*) FROM [CustomerCommunication] WHERE [CustomerCommunication].UserID = @userID) > 0
		RETURN 1

	-- If is in [OrderReview]
    IF (SELECT COUNT(*) FROM [OrderReview] WHERE [OrderReview].ReviewedBy = @userID) > 0
		RETURN 1

	-- If is in [[OrderShipment]]
    IF (SELECT COUNT(*) FROM [OrderShipment] WHERE [OrderShipment].ShippingUserID = @userID) > 0
		RETURN 1
	
	-- If is in [PartInspection]
    IF (SELECT COUNT(*) FROM [PartInspection] WHERE [PartInspection].QAUserID = @userID) > 0
		RETURN 1

	-- If is in [Quote]
    IF (SELECT COUNT(*) FROM [Quote] WHERE [Quote].CreatedByID = @userID) > 0
		RETURN 1
	
	SET @isUsed = 0
	RETURN 0
END

GO


/********************************************************************************
*	Users.LoginPin
*		Add to allow user to login via this pin.
*
********************************************************************************/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Users ADD
	LoginPin nvarchar(50) NULL
GO
ALTER TABLE dbo.Users SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

/********************************************************************************
*	ApplicationSettings
*		Insert new application setting values.
*
********************************************************************************/
INSERT INTO ApplicationSettings VALUES ('LoginType', 'Smartcard')

-- License Settings
INSERT INTO ApplicationSettings VALUES ('CryptoValidationKey', 'AMAAMADS9Of+flACbUb7TPpEkLFPAJsw7Vobk3yVPZhvLznvBeIf5mS4Pas7ZItN7NAo25kDAAEAAQ==')
INSERT INTO ApplicationSettings VALUES ('CryptoLicenseKey', 'thKEIFWAQKQNPs0BVQClnKBVzQEeAD4AQ3VzdG9tZXI9aHR0cDovL2xvY2FsaG9zdC98RGF0ZSBQdXJjaGFzZWQ9NS8yOS8yMDEyIDY6NDQ6MzcgUE0BAQoAAAClvxUxIkHYisr0pf5mXCZJzSiNcUjTMp8FmA3JEOMiyUBKaEgQ5DXuCZQeEKrOmYo=')
INSERT INTO ApplicationSettings VALUES ('CryptoLicenseServerUrl', 'http://localhost/LicenseService/Service.asmx')

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE SettingName = 'SurfaceAreaRequired')
	INSERT INTO ApplicationSettings VALUES ('SurfaceAreaRequired', 'true')
ELSE
	PRINT 'ApplicationSettings - SurfaceAreaRequired exists'
GO



/********************************************************************************
*	Users
*		Add new columns to user profile. Title, Department, Media
*
********************************************************************************/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Media SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Users ADD
	Title nvarchar(50) NULL,
	Department nvarchar(50) NULL,
	MediaID int NULL
GO
ALTER TABLE dbo.Users ADD CONSTRAINT
	FK_Users_Media FOREIGN KEY
	(
	MediaID
	) REFERENCES dbo.Media
	(
	MediaID
	) ON UPDATE  NO ACTION 
	 ON DELETE  SET NULL 
	
GO
ALTER TABLE dbo.Users SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

/********************************************************************************
*	Delete_UnusedMedia
*		Update Stored Procedure to account for media used by Users table.
*
********************************************************************************/

USE [DWOS]
GO
/****** Object:  StoredProcedure [dbo].[Delete_UnusedMedia]    Script Date: 6/2/2012 5:02:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Delete_UnusedMedia] 
AS
BEGIN
	SET NOCOUNT ON;

declare @rows int

set @rows = (select COUNT(*) FROM Media)

DELETE FROM Media 
WHERE MediaID IN 
(SELECT m.MediaID FROM Media m
	WHERE 
	NOT	EXISTS (SELECT * FROM Part_Media pm WHERE m.MediaID = pm.MediaID) 
	AND
	NOT EXISTS (SELECT * FROM [Order] o WHERE m.MediaID = o.MediaID)
	AND
	NOT EXISTS (SELECT * FROM [Users] u WHERE m.MediaID = u.MediaID))

select @rows - (select COUNT(*) FROM Media)
	
END

/********************************************************************************
*	OrderBatch
*		Add Fixture column to the table.
*
********************************************************************************/
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.OrderBatch ADD
	Fixture nvarchar(50) NULL
GO
ALTER TABLE dbo.OrderBatch SET (LOCK_ESCALATION = TABLE)
GO
COMMIT


/********************************************************************************
*	Site
*		Add Site table.
*
********************************************************************************/
USE [DWOS]
GO

/****** Object:  Table [dbo].[Site]    Script Date: 6/2/2012 7:52:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Site](
	[SiteID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Address1] [nvarchar](50) NOT NULL,
	[Address2] [nvarchar](50) NULL,
	[City] [nvarchar](50) NOT NULL,
	[State] [nvarchar](50) NOT NULL,
	[ZipCode] [nvarchar](50) NOT NULL,
	[IsPrimary] [bit] NOT NULL,
 CONSTRAINT [PK_Site] PRIMARY KEY CLUSTERED 
(
	[SiteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Site] ADD  CONSTRAINT [DF_Site_IsPrimary]  DEFAULT ((0)) FOR [IsPrimary]
GO


/********************************************************************************
*	ApplicationSettings
*		Add Setting to table.
*
********************************************************************************/
INSERT INTO ApplicationSettings VALUES ('MultiSite', 'false')


/********************************************************************************
*	d_WorkStatus
*		Changed 'Pending QI' to 'Pending Inspection'.
*
********************************************************************************/
UPDATE d_WorkStatus SET [WorkStatusID] = 'Pending Inspection' WHERE [WorkStatusID] = 'Pending QI'
UPDATE [Order] SET WorkStatus = 'Pending Inspection' WHERE WorkStatus = 'Pending QI'


/********************************************************************************
*	SecurityRole
*		Ensure roles are present.
*
********************************************************************************/
IF NOT EXISTS (SELECT * FROM [SecurityRole] WHERE [SecurityRoleID] = 'CustomerVolume')
	BEGIN
		INSERT INTO  [SecurityRole] VALUES ('CustomerVolume', 'Ability to run report.', 'Reports');
	END
	GO

IF NOT EXISTS (SELECT * FROM [SecurityRole] WHERE [SecurityRoleID] = 'TemperatureReport')
	BEGIN
		INSERT INTO  [SecurityRole] VALUES ('TemperatureReport', 'Ability to run report.', 'Reports');
	END
	GO


/********************************************************************************
*	[Get_AvgPartProcessingByDept]
*		Update to return the day also.
*
********************************************************************************/
USE [DWOS]
GO
/****** Object:  StoredProcedure [dbo].[Get_AvgPartProcessingByDept]    Script Date: 6/4/2012 6:04:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[Get_AvgPartProcessingByDept] 
	@numberOfDays int
AS
BEGIN
	SET NOCOUNT ON;

WITH cteAvgParts (avgDay, dept, avgQty)
	AS
	(
		SELECT   CONVERT(varchar,OrderProcesses.EndDate,111) as endday, OrderProcesses.Department, SUM([Order].PartQuantity)
		FROM         OrderProcesses INNER JOIN
							  [Order] ON OrderProcesses.OrderID = [Order].OrderID
		WHERE     (DATEDIFF(d, OrderProcesses.EndDate, GETDATE()) <= @numberOfDays) AND DATEPART(weekday, OrderProcesses.EndDate) <> 1 AND DATEPART(weekday, OrderProcesses.EndDate) <> 7
		GROUP BY CONVERT(varchar,OrderProcesses.EndDate,111) , OrderProcesses.Department
	)


	SELECT avgQty ProcessedPartQty, dept Department, avgDay 'Day' FROM cteAvgParts ORDER BY Department
END




