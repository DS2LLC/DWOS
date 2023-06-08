
USE [DWOS]
GO

-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '13.1.0'


IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
	INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
	UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO
--


/****** Added new column to  d_contact table ******/
BEGIN TRANSACTION
GO
ALTER TABLE dbo.d_Contact ADD
	PortalAuthorizationSent smalldatetime NULL
GO
ALTER TABLE dbo.d_Contact SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

-- Update existing contacts so they will not get the notification again
UPDATE d_Contact
SET PortalAuthorizationSent = GETDATE()
WHERE PortalAuthorized = 1


/****** Add new [SecurityRoleCategory] ******/
INSERT INTO [SecurityRoleCategory] ([SecurityRoleCategoryID]) VALUES ('Document Manager') 
GO
COMMIT

/****** Add new [SecurityRole] ******/
INSERT INTO [SecurityRole] ([SecurityRoleID],[Description],[SecurityRoleCategoryID]) VALUES
           ('Documents.AddFile','Ability to add files to a folder.','Document Manager')
INSERT INTO [SecurityRole] ([SecurityRoleID],[Description],[SecurityRoleCategoryID]) VALUES
           ('Documents.DeleteFile','Ability to delete files in a folder.','Document Manager')
INSERT INTO [SecurityRole] ([SecurityRoleID],[Description],[SecurityRoleCategoryID]) VALUES
           ('Documents.AddFolder','','Document Manager')	
INSERT INTO [SecurityRole] ([SecurityRoleID],[Description],[SecurityRoleCategoryID]) VALUES
           ('Documents.RenameFolder','','Document Manager')	
INSERT INTO [SecurityRole] ([SecurityRoleID],[Description],[SecurityRoleCategoryID]) VALUES
           ('Documents.DeleteFolder','','Document Manager')	
INSERT INTO [SecurityRole] ([SecurityRoleID],[Description],[SecurityRoleCategoryID]) VALUES
           ('Documents.GetFiles','','Document Manager')	
INSERT INTO [SecurityRole] ([SecurityRoleID],[Description],[SecurityRoleCategoryID]) VALUES
           ('Documents.FolderProperties','','Document Manager')	
INSERT INTO [SecurityRole] ([SecurityRoleID],[Description],[SecurityRoleCategoryID]) VALUES
           ('Documents.FileProperties','','Document Manager')	
INSERT INTO [SecurityRole] ([SecurityRoleID],[Description],[SecurityRoleCategoryID]) VALUES
           ('Documents.CheckOutFile','','Document Manager')	
INSERT INTO [SecurityRole] ([SecurityRoleID],[Description],[SecurityRoleCategoryID]) VALUES
           ('Documents.UndoCheckOutFile','','Document Manager')	
INSERT INTO [SecurityRole] ([SecurityRoleID],[Description],[SecurityRoleCategoryID]) VALUES
           ('Documents.ViewFile','','Document Manager')	
INSERT INTO [SecurityRole] ([SecurityRoleID],[Description],[SecurityRoleCategoryID]) VALUES
           ('Documents','','Document Manager')	
INSERT INTO [SecurityRole] ([SecurityRoleID],[Description],[SecurityRoleCategoryID]) VALUES
           ('Documents.Destroy','','Document Manager')	
INSERT INTO [SecurityRole] ([SecurityRoleID],[Description],[SecurityRoleCategoryID]) VALUES
           ('Documents.Restore','','Document Manager')	
GO
COMMIT

/****** Add new [[SecurityGroup]] ******/
INSERT INTO [dbo].[SecurityGroup]([Name],[SystemDefined]) VALUES('Document Administrator', 1)


/************************************************************************************************
*************************************************************************************************
	
	Add Document Tables 

*************************************************************************************************
************************************************************************************************/

/****** Object:  Table [dbo].[DocumentFolder]    Script Date: 1/10/2013 10:20:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentFolder](
	[DocumentFolderID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[ParentID] [int] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_DocumentFolder] PRIMARY KEY CLUSTERED 
(
	[DocumentFolderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocumentFolder_DocumentInfo]    Script Date: 1/10/2013 10:20:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentFolder_DocumentInfo](
	[DocumentFolderID] [int] NOT NULL,
	[DocumentInfoID] [int] NOT NULL,
 CONSTRAINT [PK_DoucmentFolder_DocumentInfo] PRIMARY KEY CLUSTERED 
(
	[DocumentFolderID] ASC,
	[DocumentInfoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocumentFolder_SecurityGroup]    Script Date: 1/10/2013 10:20:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentFolder_SecurityGroup](
	[DocumentFolderID] [int] NOT NULL,
	[SecurityGroupID] [int] NOT NULL,
 CONSTRAINT [PK_DocumentFolder_SecurityGroup] PRIMARY KEY CLUSTERED 
(
	[DocumentFolderID] ASC,
	[SecurityGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocumentInfo]    Script Date: 1/10/2013 10:20:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentInfo](
	[DocumentInfoID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[CurrentRevision] [int] NOT NULL,
	[DocumentLocked] [bit] NOT NULL,
	[MediaType] [nvarchar](50) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_DocumentInfo] PRIMARY KEY CLUSTERED 
(
	[DocumentInfoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocumentLock]    Script Date: 1/10/2013 10:20:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentLock](
	[DocumentLockID] [int] IDENTITY(1,1) NOT NULL,
	[LockedByUser] [int] NOT NULL,
	[DateLockedUTC] [datetime] NOT NULL,
	[DateUnlockedUTC] [datetime] NULL,
	[DocumentInfoID] [int] NOT NULL,
	[ComputerName] [nvarchar](50) NULL,
	[LocalFilePath] [nvarchar](255) NULL,
 CONSTRAINT [PK_DocumentLock] PRIMARY KEY CLUSTERED 
(
	[DocumentLockID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocumentRevision]    Script Date: 1/10/2013 10:20:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DocumentRevision](
	[DocumentRevisionID] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[DocumentInfoID] [int] NOT NULL,
	[Folder] [nvarchar](255) NULL,
	[UserCreated] [int] NULL,
	[DateCreatedUTC] [datetime] NULL,
	[RevisionNumber] [int] NOT NULL,
	[FileHash] [nvarchar](255) NULL,
	[IsCompressed] [bit] NOT NULL,
	[Comments] [nvarchar](max) NULL,
	[RowGUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[DocumentData] [varbinary](max) NULL,
 CONSTRAINT [PK_DocumentRevision] PRIMARY KEY CLUSTERED 
(
	[DocumentRevisionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[RowGUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[DocumentRevision] ADD  DEFAULT (newid()) FOR [RowGUID]
GO
ALTER TABLE [dbo].[DocumentFolder] ADD  CONSTRAINT [DF_DocumentFolder_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[DocumentInfo] ADD  CONSTRAINT [DF_DocumentInfo_CurrentRevision]  DEFAULT ((0)) FOR [CurrentRevision]
GO
ALTER TABLE [dbo].[DocumentInfo] ADD  CONSTRAINT [DF_DocumentInfo_DocumentLocked]  DEFAULT ((0)) FOR [DocumentLocked]
GO
ALTER TABLE [dbo].[DocumentInfo] ADD  CONSTRAINT [DF_DocumentInfo_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[DocumentRevision] ADD  CONSTRAINT [DF_DocumentRevision_DateCreated]  DEFAULT (getdate()) FOR [DateCreatedUTC]
GO
ALTER TABLE [dbo].[DocumentRevision] ADD  CONSTRAINT [DF_DocumentRevision_RevisionNumber]  DEFAULT ((1)) FOR [RevisionNumber]
GO
ALTER TABLE [dbo].[DocumentRevision] ADD  CONSTRAINT [DF_DocumentRevision_IsCompressed]  DEFAULT ((0)) FOR [IsCompressed]
GO
ALTER TABLE [dbo].[DocumentFolder_DocumentInfo]  WITH CHECK ADD  CONSTRAINT [FK_DoucmentFolder_DocumentInfo_DocumentFolder] FOREIGN KEY([DocumentFolderID])
REFERENCES [dbo].[DocumentFolder] ([DocumentFolderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DocumentFolder_DocumentInfo] CHECK CONSTRAINT [FK_DoucmentFolder_DocumentInfo_DocumentFolder]
GO
ALTER TABLE [dbo].[DocumentFolder_DocumentInfo]  WITH CHECK ADD  CONSTRAINT [FK_DoucmentFolder_DocumentInfo_DocumentInfo] FOREIGN KEY([DocumentInfoID])
REFERENCES [dbo].[DocumentInfo] ([DocumentInfoID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DocumentFolder_DocumentInfo] CHECK CONSTRAINT [FK_DoucmentFolder_DocumentInfo_DocumentInfo]
GO
ALTER TABLE [dbo].[DocumentFolder_SecurityGroup]  WITH CHECK ADD  CONSTRAINT [FK_DocumentFolder_SecurityGroup_DocumentFolder] FOREIGN KEY([DocumentFolderID])
REFERENCES [dbo].[DocumentFolder] ([DocumentFolderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DocumentFolder_SecurityGroup] CHECK CONSTRAINT [FK_DocumentFolder_SecurityGroup_DocumentFolder]
GO
ALTER TABLE [dbo].[DocumentFolder_SecurityGroup]  WITH CHECK ADD  CONSTRAINT [FK_DocumentFolder_SecurityGroup_SecurityGroup] FOREIGN KEY([SecurityGroupID])
REFERENCES [dbo].[SecurityGroup] ([SecurityGroupID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DocumentFolder_SecurityGroup] CHECK CONSTRAINT [FK_DocumentFolder_SecurityGroup_SecurityGroup]
GO
ALTER TABLE [dbo].[DocumentLock]  WITH CHECK ADD  CONSTRAINT [FK_DocumentLock_DocumentInfo] FOREIGN KEY([DocumentInfoID])
REFERENCES [dbo].[DocumentInfo] ([DocumentInfoID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DocumentLock] CHECK CONSTRAINT [FK_DocumentLock_DocumentInfo]
GO
ALTER TABLE [dbo].[DocumentRevision]  WITH CHECK ADD  CONSTRAINT [FK_DocumentRevision_DocumentInfo] FOREIGN KEY([DocumentInfoID])
REFERENCES [dbo].[DocumentInfo] ([DocumentInfoID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DocumentRevision] CHECK CONSTRAINT [FK_DocumentRevision_DocumentInfo]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Delete_UnusedDocuments 
AS
BEGIN
	SET NOCOUNT ON;

	DELETE
	FROM
		DocumentInfo
	WHERE
		DocumentInfoID IN (SELECT d.DocumentInfoID
                FROM
                  DocumentInfo d
                WHERE
                  NOT EXISTS (SELECT *
                              FROM
                                DocumentFolder_DocumentInfo df
                              WHERE
                                d.DocumentInfoID = df.DocumentInfoID)
								AND
				  NOT EXISTS (Select *
							  FROM
							  DocumentLink dl
							  WHERE
							  d.DocumentInfoID = dl.DocumentInfoID))
END
GO

--Update Cascading for CustomerShipping

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
ALTER TABLE dbo.CustomerShipping
	DROP CONSTRAINT FK_CustomerShipping_d_ShippingCarrier
GO
ALTER TABLE dbo.d_ShippingCarrier SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.CustomerShipping ADD CONSTRAINT
	FK_CustomerShipping_d_ShippingCarrier FOREIGN KEY
	(
	CarrierID
	) REFERENCES dbo.d_ShippingCarrier
	(
	CarrierID
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.CustomerShipping SET (LOCK_ESCALATION = TABLE)
GO
COMMIT




--Update To Cascade Update for ShipmentPackage

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
ALTER TABLE dbo.ShipmentPackage
	DROP CONSTRAINT FK_ShipmentPackage_d_ShippingCarrier
GO
ALTER TABLE dbo.d_ShippingCarrier SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ShipmentPackage ADD CONSTRAINT
	FK_ShipmentPackage_d_ShippingCarrier FOREIGN KEY
	(
	ShippingCarrierID
	) REFERENCES dbo.d_ShippingCarrier
	(
	CarrierID
	) ON UPDATE  CASCADE 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ShipmentPackage SET (LOCK_ESCALATION = TABLE)
GO
COMMIT




-- Set Delete to Cascade for dbo.OrderCustomFields

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
ALTER TABLE dbo.OrderCustomFields
	DROP CONSTRAINT FK_OrderCustomFields_Order
GO
ALTER TABLE dbo.[Order] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.OrderCustomFields ADD CONSTRAINT
	FK_OrderCustomFields_Order FOREIGN KEY
	(
	OrderID
	) REFERENCES dbo.[Order]
	(
	OrderID
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.OrderCustomFields SET (LOCK_ESCALATION = TABLE)
GO
COMMIT




-- Set Delete and Update to Cascade for dbo.Quote

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
ALTER TABLE dbo.Quote
	DROP CONSTRAINT FK_Quote_d_Contact
GO
ALTER TABLE dbo.d_Contact SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Quote ADD CONSTRAINT
	FK_Quote_d_Contact FOREIGN KEY
	(
	ContactID
	) REFERENCES dbo.d_Contact
	(
	ContactID
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.Quote SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

/****** Object:  Table [dbo].[DocumentLink]    Script Date: 3/15/2013 5:02:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DocumentLink](
	[DocumentLinkID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[DocumentInfoID] [int] NOT NULL,
	[LinkToType] [nvarchar](50) NOT NULL,
	[LinkToKey] [int] NOT NULL,
 CONSTRAINT [PK_DocumentLink] PRIMARY KEY CLUSTERED 
(
	[DocumentLinkID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DocumentLink]  WITH CHECK ADD  CONSTRAINT [FK_DocumentLink_DocumentInfo] FOREIGN KEY([DocumentInfoID])
REFERENCES [dbo].[DocumentInfo] ([DocumentInfoID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[DocumentLink] CHECK CONSTRAINT [FK_DocumentLink_DocumentInfo]
GO


-- Set Update to Cascade for dbo.WorkSchedule

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
ALTER TABLE dbo.WorkSchedule
	DROP CONSTRAINT FK_WorkSchedule_d_Department
GO
ALTER TABLE dbo.d_Department SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.WorkSchedule ADD CONSTRAINT
	FK_WorkSchedule_d_Department FOREIGN KEY
	(
	DepartmentID
	) REFERENCES dbo.d_Department
	(
	DepartmentID
	) ON UPDATE  CASCADE 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.WorkSchedule SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

