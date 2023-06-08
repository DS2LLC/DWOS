
/********************************************************************************
*	Notes
*		- Add Cascade Delete to OrderCustomFields.[FK_OrderCustomFields_Order] and DELETE from OrderCustomFields.[FK_OrderCustomFields_CustomField]
*		- Add Cascade Delete to OrderFees.[FK_OrderFees_Order]
********************************************************************************/


DROP FUNCTION [dbo].[fnGetNextDepartment];
GO
DROP SYNONYM [dbo].[log4net];
GO
DROP VIEW [dbo].[vUserEventLog_Deleted];
GO
DROP VIEW [dbo].[vsysdiagrams_Deleted];
GO
DROP VIEW [dbo].[vProcess_Deleted];
GO
ALTER TABLE [dbo].[old_ProcessFolders] DROP CONSTRAINT [ProcessFolders_Created_df];
GO
ALTER TABLE [dbo].[old_ProcessFolders] DROP CONSTRAINT [ProcessFolders_Modified_df];
GO
ALTER TABLE [dbo].[old_ProcessFolders] DROP CONSTRAINT [ProcessFolders_RowVersion_df];
GO
DROP TABLE [dbo].[old_ProcessFolders];
GO

EXEC	[dbo].[pAutoAuditDrop] @SchemaName = N'dbo',@TableName = N'Process'

/********************************************************************************
*	Quote
*		Rename existing quote table, then readd new one
*		Used By:
*			Get_IsUserUsed
*			Update_QuoteActiveStatus
*
********************************************************************************/

-- DROP Current Quote Table
EXEC	[dbo].[pAutoAuditDrop] @SchemaName = N'dbo',@TableName = N'Quote'

DROP FUNCTION [dbo].[Quote_RowHistory]
GO
DROP TABLE [dbo].[QuoteFees]
GO
DROP TABLE [dbo].[Quote] 
GO

-- Delete settings
DELETE FROM ApplicationSettings WHERE SettingName = 'QuoteWarranty';


USE [DWOS]
GO

/****** Create [d_Terms] table ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[d_Terms](
	[TermsID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Terms] [nvarchar](max) NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_d_Terms] PRIMARY KEY CLUSTERED 
(
	[TermsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[d_Terms] ADD  CONSTRAINT [DF_d_Terms_Active]  DEFAULT ((1)) FOR [Active]
GO


-- Default terms
INSERT INTO [d_Terms]
           ([Name]
           ,[Terms]
           ,[Active])
     VALUES
           ('Default Terms'
           ,'Additional fees may apply to each Purchase Order. Parts requiring specialized handling or processing may have additional fees. Turn time for orders are controlled by processing, specifications, and other requirements. Average turn time is <span style="font-style:italic;">5 – 7</span> days. Expedites are available upon request for an additional fee. See our website for a full list of specifications and terms.'
           ,'1')
GO

/****** Create QUOTE table ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Quote](
	[QuoteID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[Revision] [nvarchar](10) NULL,
	[UserId] [int] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[ClosedDate] [smalldatetime] NULL,
	[ExpirationDate] [smalldatetime] NOT NULL,
	[Program] [nvarchar](50) NULL,
	[RFQ] [nvarchar](50) NULL,
	[CustomerID] [int] NOT NULL,
	[ContactID] [int] NOT NULL,
	[Notes] [nvarchar](max) NULL,
	[TermsID] [int] NOT NULL,
 CONSTRAINT [PK_Quote] PRIMARY KEY CLUSTERED 
(
	[QuoteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Quote] ADD  CONSTRAINT [DF_Quote_Status]  DEFAULT (N'Open') FOR [Status]
GO

ALTER TABLE [dbo].[Quote]  WITH CHECK ADD  CONSTRAINT [FK_Quote_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customer] ([CustomerID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Quote] CHECK CONSTRAINT [FK_Quote_Customer]
GO

ALTER TABLE [dbo].[Quote]  WITH CHECK ADD  CONSTRAINT [FK_Quote_d_Contact] FOREIGN KEY([ContactID])
REFERENCES [dbo].[d_Contact] ([ContactID])
GO

ALTER TABLE [dbo].[Quote] CHECK CONSTRAINT [FK_Quote_d_Contact]
GO

ALTER TABLE [dbo].[Quote]  WITH CHECK ADD  CONSTRAINT [FK_Quote_d_Terms] FOREIGN KEY([TermsID])
REFERENCES [dbo].[d_Terms] ([TermsID])
GO

ALTER TABLE [dbo].[Quote] CHECK CONSTRAINT [FK_Quote_d_Terms]
GO

ALTER TABLE [dbo].[Quote]  WITH CHECK ADD  CONSTRAINT [FK_Quote_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserID])
GO

ALTER TABLE [dbo].[Quote] CHECK CONSTRAINT [FK_Quote_Users]
GO


USE [DWOS]
GO

/****** Create [QuotePart] Table ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QuotePart](
	[QuotePartID] [int] IDENTITY(1,1) NOT NULL,
	[QuoteID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Quantity] [int] NOT NULL,
	[PartMarking] [bit] NOT NULL,
	[EachPrice] [money] NULL,
	[LotPrice] [money] NULL,
	[Length] [float] NULL,
	[Width] [float] NULL,
	[SurfaceArea] [float] NULL,
 CONSTRAINT [PK_QuotePart] PRIMARY KEY CLUSTERED 
(
	[QuotePartID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QuotePart] ADD  CONSTRAINT [DF_QuotePart_Quantity]  DEFAULT ((0)) FOR [Quantity]
GO

ALTER TABLE [dbo].[QuotePart] ADD  CONSTRAINT [DF_QuotePart_PartMarking]  DEFAULT ((0)) FOR [PartMarking]
GO

ALTER TABLE [dbo].[QuotePart]  WITH CHECK ADD  CONSTRAINT [FK_QuotePart_Quote] FOREIGN KEY([QuoteID])
REFERENCES [dbo].[Quote] ([QuoteID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QuotePart] CHECK CONSTRAINT [FK_QuotePart_Quote]
GO


/****** Create [QuoteProcess] Table ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QuoteProcess](
	[QuoteProcessID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_QuotePart_QuoteProcess] PRIMARY KEY CLUSTERED 
(
	[QuoteProcessID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QuoteProcess] ADD  CONSTRAINT [DF_QuoteProcess_Active]  DEFAULT ((1)) FOR [Active]
GO

USE [DWOS]
GO

/****** Create [dbo].[QuotePart_Media] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QuotePart_Media](
	[QuotePartID] [int] NOT NULL,
	[MediaID] [int] NOT NULL,
 CONSTRAINT [PK_QuotePart_Media] PRIMARY KEY CLUSTERED 
(
	[QuotePartID] ASC,
	[MediaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QuotePart_Media]  WITH CHECK ADD  CONSTRAINT [FK_QuotePart_Media_Media] FOREIGN KEY([MediaID])
REFERENCES [dbo].[Media] ([MediaID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QuotePart_Media] CHECK CONSTRAINT [FK_QuotePart_Media_Media]
GO

ALTER TABLE [dbo].[QuotePart_Media]  WITH CHECK ADD  CONSTRAINT [FK_QuotePart_Media_QuotePart_Media] FOREIGN KEY([QuotePartID])
REFERENCES [dbo].[QuotePart] ([QuotePartID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QuotePart_Media] CHECK CONSTRAINT [FK_QuotePart_Media_QuotePart_Media]
GO

USE [DWOS]
GO

/****** Create [dbo].[QuotePart_QuoteProcess]  ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QuotePart_QuoteProcess](
	[QuotePartID] [int] NOT NULL,
	[QuoteProcessID] [int] NOT NULL,
	[StepOrder] [int] NOT NULL,
 CONSTRAINT [PK_QuotePart_QuoteProcess_2] PRIMARY KEY CLUSTERED 
(
	[QuotePartID] ASC,
	[QuoteProcessID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QuotePart_QuoteProcess]  WITH CHECK ADD  CONSTRAINT [FK_QuotePart_QuoteProcess_QuotePart] FOREIGN KEY([QuotePartID])
REFERENCES [dbo].[QuotePart] ([QuotePartID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QuotePart_QuoteProcess] CHECK CONSTRAINT [FK_QuotePart_QuoteProcess_QuotePart]
GO

ALTER TABLE [dbo].[QuotePart_QuoteProcess]  WITH CHECK ADD  CONSTRAINT [FK_QuotePart_QuoteProcess_QuoteProcess] FOREIGN KEY([QuoteProcessID])
REFERENCES [dbo].[QuoteProcess] ([QuoteProcessID])
GO

ALTER TABLE [dbo].[QuotePart_QuoteProcess] CHECK CONSTRAINT [FK_QuotePart_QuoteProcess_QuoteProcess]
GO




-- Reseed the col to start at higher quoteid value
dbcc checkident (Quote, reseed, 1000)

/********************************************************************************
*	[Update_QuoteActiveStatus]
*		Update to only change row if needed
*
********************************************************************************/
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Update_QuoteActiveStatus] 
AS
BEGIN

	-- Update all quotes that are expired
	Update Quote
	SET [Status] = 'Closed', ClosedDate = GETDATE()
	WHERE DATEDIFF(DAY, ExpirationDate, GETDATE()) > 1
END


/********************************************************************************
*	[Get_IsUserUsed]
*		Updated to use new Quote.UserID column
*
********************************************************************************/
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Get_IsUserUsed]
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
    IF (SELECT COUNT(*) FROM [Quote] WHERE [Quote].UserID = @userID) > 0
		RETURN 1
	
	SET @isUsed = 0
	RETURN 0
END

/********************************************************************************
*	[Order] 
*		Drop QuoteID from Order Table
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
ALTER TABLE dbo.[Order]
	DROP COLUMN QuoteID
GO
ALTER TABLE dbo.[Order] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

-- Update the Trigger since removed the QuoteID column
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[Order_Audit_Update] ON [dbo].[Order]
 AFTER Update
 NOT FOR REPLICATION AS
 SET NoCount On 
 -- generated by AutoAudit on Jul 22 2009 10:55PM
 -- created by Paul Nielsen 
 -- www.SQLServerBible.com 

DECLARE @AuditTime DATETIME
SET @AuditTime = GetDate()

 Begin Try 
 IF UPDATE([OrderID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[OrderID]', Convert(VARCHAR(50), Deleted.[OrderID]),  Convert(VARCHAR(50), Inserted.[OrderID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[OrderID],'') <> isnull(Deleted.[OrderID],'')

 IF UPDATE([CustomerID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[CustomerID]', Convert(VARCHAR(50), Deleted.[CustomerID]),  Convert(VARCHAR(50), Inserted.[CustomerID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[CustomerID],'') <> isnull(Deleted.[CustomerID],'')

 IF UPDATE([OrderDate])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[OrderDate]', Convert(VARCHAR(50), Deleted.[OrderDate]),  Convert(VARCHAR(50), Inserted.[OrderDate]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[OrderDate],'') <> isnull(Deleted.[OrderDate],'')

 IF UPDATE([RequiredDate])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[RequiredDate]', Convert(VARCHAR(50), Deleted.[RequiredDate]),  Convert(VARCHAR(50), Inserted.[RequiredDate]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[RequiredDate],'') <> isnull(Deleted.[RequiredDate],'')

 IF UPDATE([Status])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Status]', Convert(VARCHAR(50), Deleted.[Status]),  Convert(VARCHAR(50), Inserted.[Status]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[Status],'') <> isnull(Deleted.[Status],'')

 IF UPDATE([CompletedDate])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[CompletedDate]', Convert(VARCHAR(50), Deleted.[CompletedDate]),  Convert(VARCHAR(50), Inserted.[CompletedDate]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[CompletedDate],'') <> isnull(Deleted.[CompletedDate],'')

 IF UPDATE([Priority])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Priority]', Convert(VARCHAR(50), Deleted.[Priority]),  Convert(VARCHAR(50), Inserted.[Priority]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[Priority],'') <> isnull(Deleted.[Priority],'')

 IF UPDATE([PurchaseOrder])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[PurchaseOrder]', Convert(VARCHAR(50), Deleted.[PurchaseOrder]),  Convert(VARCHAR(50), Inserted.[PurchaseOrder]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[PurchaseOrder],'') <> isnull(Deleted.[PurchaseOrder],'')

 IF UPDATE([CreatedBy])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[CreatedBy]', Convert(VARCHAR(50), Deleted.[CreatedBy]),  Convert(VARCHAR(50), Inserted.[CreatedBy]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[CreatedBy],'') <> isnull(Deleted.[CreatedBy],'')

 IF UPDATE([Invoice])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Invoice]', Convert(VARCHAR(50), Deleted.[Invoice]),  Convert(VARCHAR(50), Inserted.[Invoice]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[Invoice],'') <> isnull(Deleted.[Invoice],'')

 IF UPDATE([ContractReviewed])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[ContractReviewed]', Convert(VARCHAR(50), Deleted.[ContractReviewed]),  Convert(VARCHAR(50), Inserted.[ContractReviewed]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[ContractReviewed],'') <> isnull(Deleted.[ContractReviewed],'')

 IF UPDATE([PartID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[PartID]', Convert(VARCHAR(50), Deleted.[PartID]),  Convert(VARCHAR(50), Inserted.[PartID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[PartID],'') <> isnull(Deleted.[PartID],'')

 IF UPDATE([PartQuantity])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[PartQuantity]', Convert(VARCHAR(50), Deleted.[PartQuantity]),  Convert(VARCHAR(50), Inserted.[PartQuantity]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[PartQuantity],'') <> isnull(Deleted.[PartQuantity],'')

 IF UPDATE([MediaID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[MediaID]', Convert(VARCHAR(50), Deleted.[MediaID]),  Convert(VARCHAR(50), Inserted.[MediaID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[MediaID],'') <> isnull(Deleted.[MediaID],'')

 IF UPDATE([WorkStatus])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[WorkStatus]', Convert(VARCHAR(50), Deleted.[WorkStatus]),  Convert(VARCHAR(50), Inserted.[WorkStatus]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[WorkStatus],'') <> isnull(Deleted.[WorkStatus],'')

 IF UPDATE([CurrentLocation])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[CurrentLocation]', Convert(VARCHAR(50), Deleted.[CurrentLocation]),  Convert(VARCHAR(50), Inserted.[CurrentLocation]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[CurrentLocation],'') <> isnull(Deleted.[CurrentLocation],'')

 IF UPDATE([BasePrice])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[BasePrice]', Convert(VARCHAR(50), Deleted.[BasePrice]),  Convert(VARCHAR(50), Inserted.[BasePrice]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[BasePrice],'') <> isnull(Deleted.[BasePrice],'')

 IF UPDATE([PriceUnit])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[PriceUnit]', Convert(VARCHAR(50), Deleted.[PriceUnit]),  Convert(VARCHAR(50), Inserted.[PriceUnit]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[PriceUnit],'') <> isnull(Deleted.[PriceUnit],'')

 --IF UPDATE([QuoteID])
 --  INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
 --  SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
 --       NULL,     -- Row Description (e.g. Order Number)
 --       NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
 --       '[QuoteID]', Convert(VARCHAR(50), Deleted.[QuoteID]),  Convert(VARCHAR(50), Inserted.[QuoteID]),
 --DELETED.Rowversion + 1
 --         FROM Inserted
 --            JOIN Deleted
 --              ON Inserted.[OrderID] = Deleted.[OrderID]
 --              AND isnull(Inserted.[QuoteID],'') <> isnull(Deleted.[QuoteID],'')

 IF UPDATE([ShippingMethod])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[ShippingMethod]', Convert(VARCHAR(50), Deleted.[ShippingMethod]),  Convert(VARCHAR(50), Inserted.[ShippingMethod]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[ShippingMethod],'') <> isnull(Deleted.[ShippingMethod],'')

 IF UPDATE([EstShipDate])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[EstShipDate]', Convert(VARCHAR(50), Deleted.[EstShipDate]),  Convert(VARCHAR(50), Inserted.[EstShipDate]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[EstShipDate],'') <> isnull(Deleted.[EstShipDate],'')

 End Try 
 Begin Catch 
   Raiserror('error in [dbo].[Order_audit_update] trigger', 16, 1 ) with log
 End Catch


/********************************************************************************
*	[Part] 
*		Add QuoteID to Part Table
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
	QuotePartID int NULL
GO
ALTER TABLE dbo.Part ADD CONSTRAINT
	FK_Part_QuotePart FOREIGN KEY
	(
	QuotePartID
	) REFERENCES dbo.QuotePart
	(
	QuotePartID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Part SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

/********************************************************************************
*	[SecurityRole] 
*		Add new permissions for quotes
*
********************************************************************************/

IF NOT EXISTS (SELECT * FROM [SecurityRole] WHERE [SecurityRoleID] = 'QuoteProcessManager')
		BEGIN
			INSERT INTO  [SecurityRole] VALUES ('QuoteProcessManager', 'Ability to edit quote processes.', 'Managers');
		END
	GO

IF NOT EXISTS (SELECT * FROM [SecurityRole] WHERE [SecurityRoleID] = 'TermsManager')
		BEGIN
			INSERT INTO  [SecurityRole] VALUES ('TermsManager', 'Ability to edit terms and conditions.', 'Managers');
		END
	GO


/********************************************************************************
*	[d_PriceUnit]
*		Add new columns Min and Max Qty
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
ALTER TABLE dbo.d_PriceUnit ADD
	MinQuantity int NOT NULL CONSTRAINT DF_d_PriceUnit_MinQuantity DEFAULT 0,
	MaxQuantity int NOT NULL CONSTRAINT DF_d_PriceUnit_MaxQuantity DEFAULT 0
GO
ALTER TABLE dbo.d_PriceUnit SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

-- Update default values
UPDATE d_PriceUnit SET MinQuantity = 1, MaxQuantity = 30 WHERE PriceUnitID = 'Lot'
UPDATE d_PriceUnit SET MinQuantity = 31, MaxQuantity = 999999 WHERE PriceUnitID = 'Each'

IF NOT EXISTS (SELECT * FROM [SecurityRole] WHERE [SecurityRoleID] = 'PriceUnitManager')
		BEGIN
			INSERT INTO  [SecurityRole] VALUES ('PriceUnitManager', 'Ability to edit price units.', 'Managers');
		END
	GO

/********************************************************************************
*	[Users]
*		Add new column email address
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
ALTER TABLE dbo.Users ADD
	EmailAddress nvarchar(255) NULL
GO
ALTER TABLE dbo.Users SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

/********************************************************************************
*	[ApplicationSettings]
*		Updated settings
*
********************************************************************************/
IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE SettingName = 'CustomerPortalEmail')
	INSERT INTO ApplicationSettings VALUES ('CustomerPortalEmail', '<html>
<head><title>Customer Portal Authorization</title></head>
<body style="font-family: verdana; font-size: 12px; padding: 3px; margin: 3px; width: 800px;">
    <div style="text-align: center; max-height: 200px;">
        <img alt="Dynamic Paint Solutions" src="[%LOGO%]" width="400" />
    </div>
    <br />
    <div style="text-align: justify;"><strong>Congratulations</strong>! You have been authorized to use the <a href="https://dwos.dynamicpaintsolutions.com">Dynamic Paint Solutions Customer Portal</a>. The customer portal will allow us to communicate better with you and provide an overall better quality of service. Just another example of how Dynamic Paint Solutions is striving to provide you with the best service possible. Login to the  portal now to check orders status, print a COC, run reports, or check the real-time processing being performed on any order.</div>
    <br />
    <br />
    You can access our secure portal with the following information:   
    <br />
    <br />
    <a href="https://dwos.dynamicpaintsolutions.com">https://dwos.dynamicpaintsolutions.com</a>
    <br />
    User Name: [%USERNAME%]   
    <br />
    Password: [%PASSWORD%]
    <br />
    <br />
    If you have any questions or issues then please contact your DPS Sales Associate at <a href="mailto:sales@dynamicpaintsolutions.com">sales@dynamicpaintsolutions.com</a>.   
    <br />
    <br />
    Thank You for being our customer,   
    <br />
    <br />
    Dynamic Paint Solutions</body>
</html>')
ELSE
	UPDATE ApplicationSettings SET Value = '<html>
<head><title>Customer Portal Authorization</title></head>
<body style="font-family: verdana; font-size: 12px; padding: 3px; margin: 3px; width: 800px;">
    <div style="text-align: center; max-height: 200px;">
        <img alt="Dynamic Paint Solutions" src="[%LOGO%]" width="400" />
    </div>
    <br />
    <div style="text-align: justify;"><strong>Congratulations</strong>! You have been authorized to use the <a href="https://dwos.dynamicpaintsolutions.com">Dynamic Paint Solutions Customer Portal</a>. The customer portal will allow us to communicate better with you and provide an overall better quality of service. Just another example of how Dynamic Paint Solutions is striving to provide you with the best service possible. Login to the  portal now to check orders status, print a COC, run reports, or check the real-time processing being performed on any order.</div>
    <br />
    <br />
    You can access our secure portal with the following information:   
    <br />
    <br />
    <a href="https://dwos.dynamicpaintsolutions.com">https://dwos.dynamicpaintsolutions.com</a>
    <br />
    User Name: [%USERNAME%]   
    <br />
    Password: [%PASSWORD%]
    <br />
    <br />
    If you have any questions or issues then please contact your DPS Sales Associate at <a href="mailto:sales@dynamicpaintsolutions.com">sales@dynamicpaintsolutions.com</a>.   
    <br />
    <br />
    Thank You for being our customer,   
    <br />
    <br />
    Dynamic Paint Solutions</body>
</html>'
	WHERE SettingName = 'CustomerPortalEmail'
GO


/********************************************************************************
*	[ApplicationSettings]
*		Delete settings for Crypto licensing, not using that any more.
*
********************************************************************************/
DELETE FROM ApplicationSettings WHERE SettingName = 'CryptoValidationKey';
DELETE FROM ApplicationSettings WHERE SettingName = 'CryptoLicenseKey';
DELETE FROM ApplicationSettings WHERE SettingName = 'CryptoLicenseServerUrl';





/********************************************************************************
*	[Templates]
*		Create new templates table to hold template data.
*
********************************************************************************/

/****** Object:  Table [dbo].[Templates]    Script Date: 8/5/2012 12:35:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Templates](
	[TemplateID] [nvarchar](50) NOT NULL,
	[Template] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[Tokens] [nvarchar](max) NULL,
 CONSTRAINT [PK_Templates] PRIMARY KEY CLUSTERED 
(
	[TemplateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

-- Add quote reminder template

INSERT INTO [dbo].[Templates]
           ([TemplateID]
           ,[Template]
           ,[Description]
           ,[Tokens])
     VALUES
           ('QuoteReminder'
           ,'<html><head><title>Open Quotes [%COUNT%]</title></head><body style="font-family: verdana;font-size: 12px;padding: 3px;margin: 3px;width: 800px;"><br /><div style="text-align: justify;"><strong>Open Quotes</strong><br /><br />You currently have %COUNT% open quotes. Please ensure you follow up on all quotes with your customers and close out old quotes as needed.</div><br /><table id="QUOTE_TABLE" style="font-family: verdana;font-size: 12px;padding: 3px;margin: 3px;width: 100%;"><tr style="text-align: center;background-color: #990000;color: #FFFFFF;font-weight: bold"><td>Quote</td><td>Customer</td><td>Contact</td><td>Created</td><td>Expires</td></tr><tr id="ROW_TEMPLATE"><td>%QUOTEID%</td><td>%COMPANY%</td><td>%CONTACT%</td><td>%CREATED%</td><td>%EXPIRE%</td></tr></table></body></html>'
           ,'Email reminder about open quotes.'
           ,'%COUNT%, %QUOTEID%, %COMPANY%, %CONTACT%, %CREATED%, %EXPIRE%');
GO

INSERT INTO [dbo].[Templates]
           ([TemplateID]
           ,[Template]
           ,[Description]
           ,[Tokens])
     VALUES
			('CustomerPortal'
			,'<html><head><title>Customer Portal Authorization</title></head><body style="font-family: verdana;font-size: 12px;padding: 3px;margin: 3px;width: 800px;"><div style="text-align: center;max-height: 200px;"><img alt="Dynamic Paint Solutions" src="%LOGO%" width="400" /></div><br /><div style="text-align: justify;"><strong>Congratulations</strong>! You have been authorized to use the <a href="https://dwos.dynamicpaintsolutions.com">Dynamic Paint Solutions Customer Portal</a>. The customer portal will allow us to communicate better with you and provide an overall better quality of service. Just another example of how Dynamic Paint Solutions is striving to provide you with the best service possible. Login to the portal now to check orders status, print a COC, run reports, or check the real-time processing being performed on any order.</div><br /><br />You can access our secure portal with the following information:       <br /><br /><a href="https://dwos.dynamicpaintsolutions.com">https://dwos.dynamicpaintsolutions.com</a><br />User Name: %USERNAME%    <br />Password: %PASSWORD%    <br /><br />If you have any questions or issues then please contact your DPS Sales Associate at <a href="mailto:sales@dynamicpaintsolutions.com">sales@dynamicpaintsolutions.com</a>.       <br /><br />Thank You for being our customer, <br /><br />Dynamic Paint Solutions</body></html>'
			,'Email template to authorized users of the customer portal.'
			,'%USERNAME%, %PASSWORD%, %LOGO%');
GO

INSERT INTO [dbo].[Templates]
           ([TemplateID]
           ,[Template]
           ,[Description]
           ,[Tokens])
     VALUES
			('ShipmentNotification'	
			,'<html><head><title>Dynamic Paint Solutions - Order Shipment Notification</title></head><body><table style="width: 900px"><tr style="font-family: Arial;color: maroon;font-size: 22px;font-weight: bold;"><td><a href="http://www.dynamicpaintsolutions.com" style="color: maroon">Dynamic Paint Solutions</a></td><td rowspan="2"><img alt="Nadcap" src="Nadcap.jpg" width="200"></td><td style="text-align: right">Order Shipment Notification</td></tr><tr><td style="font-family: Verdana;font-size: 12px;font-weight: normal;padding: 3px;margin: auto;"> 415 Airport Road<br>Eastman, GA 31023<br>478.374.5402 Phone<br>478.374.5424 Fax</td><td colspan="2"> </td></tr></table><br><table style="width: 900px;"><tr style="font-family: Verdana;color: maroon;font-size: 14px;font-weight: bold;"><td style="width: 50%">Customer:</td><td>Shipping Information:</td></tr><tr style="font-family: Verdana;font-size: 12px;font-weight: normal;padding: 3px;margin: auto;"><td id="customerName">Sample Customer</td><td>Carrier:</td><td id="shippingCarrier">UPS</td></tr><tr style="font-family: Verdana;font-size: 12px;font-weight: normal;padding: 3px;margin: auto;"><td id="address1">123 Main St</td><td>Date Shipped:</td><td id="dateShipped">1/1/2012</td></tr><tr style="font-family: Verdana;font-size: 12px;font-weight: normal;padding: 3px;margin: auto;"><td id="cityStateZip">Eastman, GA 31023</td><td>Tracking Number:</td><td id="trackingNumber"><a href="http://ups.com">123456789</a></td></tr></table><br><table style="width: 900px;" id="shipmentTable"><thead><tr><td colspan="5" style="font-family: Verdana;color: maroon;font-size: 14px;font-weight: bold;height: 20px">Work Orders:</td></tr><tr style="font-family: Verdana;font-size: 12px;font-weight: normal;color: white;background-color: maroon;text-align: center;"><td>Work Order</td><td>Customer Work Order</td><td>Purchase Order</td><td>Part Number</td><td>Part Quantity</td></tr></thead><tr style="font-family: Verdana;font-size: 12px;font-weight: normal;padding: 3px;margin: auto;text-align: center;visibility: collapse;" id="shipmentTemplate"><td>12345</td><td>CW-12345</td><td>PO-12345</td><td>17P123523-234</td><td>25</td></tr></table><br><div style="font-family: Verdana;font-size: 12px;font-weight: normal;padding: 3px;margin: 3px;display: inline-block;width: 900px;background-position: center center;background-image: url("CompanyLogo.png");background-repeat: no-repeat;"><p style="text-align: justify">Do not reply to this email address. This is an automated notification generated by Dynamic Paint Solutions. Please direct all communications to your <a href="mailto:sales@dynamicpaintsolutions.com">DPS sales representative</a>. ITAR/EAR technical data is restricted by the Arms Export Control Act (Title 22, U.S.C., Sec 2751) or the Export Administration Act (Title 50, U.S.C., App. 2401), as amended. Violations of these export laws are subject to severe      criminal penalties. The preceding information may be confidential or privileged. It only should be used or disseminated for the purpose of conducting business with Dynamic Paint Solutions. If you are not an intended recipient, please delete the information from your system. Thank you for your cooperation. Delivery truck deliveries will be delivered on your next scheduled delivery date.</p><p>Please visit the <span style="font-weight: bold;color: #FF0000;font-style: italic">NEW</span><a href="https://dwos.dynamicpaintsolutions.com">Dynamic Paint      Solutions Customer Portal</a> to view order status, print COC, and run reports. If you have not registered for the portal, then please contact your <a href="mailto:sales@dynamicpaintsolutions.com">DPS sales representative</a>.</p></div></body></html>'
			,'Email template to notify customer that an order has been shipped.'
			,'')


DELETE FROM ApplicationSettings WHERE SettingName = 'CustomerPortalEmail';
DELETE FROM ApplicationSettings WHERE SettingName = 'ShippingNotification';


/********************************************************************************
*	[ApplicationSettings]
*		Add setting for email authentication.
*
********************************************************************************/
INSERT INTO ApplicationSettings VALUES ('EmailAuthentication', 'amNTLM');

	

/********************************************************************************
*	[LicenseActivationHistory]
*		Add table to track user license activations.
*
********************************************************************************/

/****** Object:  Table [dbo].[LicenseActivationHistory]    Script Date: 8/22/2012 3:45:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LicenseActivationHistory](
	[HistoryID] [int] IDENTITY(1,1) NOT NULL,
	[Action] [nvarchar](50) NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[ComputerName] [nvarchar](100) NOT NULL,
	[UID] [nvarchar](100) NOT NULL,
	[UserID] [int] NOT NULL,
 CONSTRAINT [PK_LicenseActivationHistory] PRIMARY KEY CLUSTERED 
(
	[HistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



/********************************************************************************
*	[SecurityGroupPermissionsReport]
*		Add new permission for report.
*
********************************************************************************/
IF NOT EXISTS (SELECT * FROM [SecurityRole] WHERE [SecurityRoleID] = 'SecurityGroupPermissionsReport')
		BEGIN
			INSERT INTO  [SecurityRole] VALUES ('SecurityGroupPermissionsReport', 'Ability to run report to show all permissions for each security group.', 'Reports');
		END
	GO
		
IF NOT EXISTS (SELECT * FROM [SecurityRole] WHERE [SecurityRoleID] = 'SecurityAuditReport')
		BEGIN
			INSERT INTO  [SecurityRole] VALUES ('SecurityAuditReport', 'Ability to run report to show all permissions for each user.', 'Reports');
		END
	GO



/********************************************************************************
*	[Order_Media]
*		Add new table then migrate records from order table to new table
*
********************************************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Create the Order_Media table
CREATE TABLE [dbo].[Order_Media](
	[OrderID] [int] NOT NULL,
	[MediaID] [int] NOT NULL,
 CONSTRAINT [PK_Order_Media] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC,
	[MediaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Order_Media]  WITH CHECK ADD  CONSTRAINT [FK_Order_Media_Media] FOREIGN KEY([MediaID])
REFERENCES [dbo].[Media] ([MediaID])
GO

ALTER TABLE [dbo].[Order_Media] CHECK CONSTRAINT [FK_Order_Media_Media]
GO

ALTER TABLE [dbo].[Order_Media]  WITH CHECK ADD  CONSTRAINT [FK_Order_Media_Order] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Order] ([OrderID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Order_Media] CHECK CONSTRAINT [FK_Order_Media_Order]
GO

-- Move records from Order to Order_Media
INSERT INTO [Order_Media] (OrderID, MediaID)
SELECT o.OrderID, o.MediaID FROM [Order] o WHERE o.MediaID IS NOT NULL

-- Drop order.mediaId column
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
ALTER TABLE dbo.[Order]
	DROP CONSTRAINT FK_Order_Media
GO
ALTER TABLE dbo.Media SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.[Order]
	DROP COLUMN MediaID
GO
ALTER TABLE dbo.[Order] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

-- Update stored procedure that uses the order.mediaId field
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
	NOT EXISTS (SELECT * FROM [Order_Media] o WHERE m.MediaID = o.MediaID)
	AND
	NOT EXISTS (SELECT * FROM [Users] u WHERE m.MediaID = u.MediaID))

select @rows - (select COUNT(*) FROM Media)
	
END

-- Update [Order_Audit_Delete] trigger to remove mediaid
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[Order_Audit_Delete] ON [dbo].[Order]
 AFTER Delete
 NOT FOR REPLICATION AS
 SET NoCount On 

DECLARE @AuditTime DATETIME
SET @AuditTime = GetDate()

 Begin Try 
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[OrderID]', Convert(VARCHAR(50), Deleted.[OrderID]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[OrderID] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[CustomerID]', Convert(VARCHAR(50), Deleted.[CustomerID]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[CustomerID] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[OrderDate]', Convert(VARCHAR(50), Deleted.[OrderDate]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[OrderDate] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[RequiredDate]', Convert(VARCHAR(50), Deleted.[RequiredDate]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[RequiredDate] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Status]', Convert(VARCHAR(50), Deleted.[Status]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Status] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[CompletedDate]', Convert(VARCHAR(50), Deleted.[CompletedDate]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[CompletedDate] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Priority]', Convert(VARCHAR(50), Deleted.[Priority]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Priority] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[PurchaseOrder]', Convert(VARCHAR(50), Deleted.[PurchaseOrder]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[PurchaseOrder] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[CreatedBy]', Convert(VARCHAR(50), Deleted.[CreatedBy]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[CreatedBy] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Invoice]', Convert(VARCHAR(50), Deleted.[Invoice]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Invoice] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[ContractReviewed]', Convert(VARCHAR(50), Deleted.[ContractReviewed]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[ContractReviewed] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[PartID]', Convert(VARCHAR(50), Deleted.[PartID]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[PartID] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[PartQuantity]', Convert(VARCHAR(50), Deleted.[PartQuantity]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[PartQuantity] is not null

   --INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   --SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
   --     NULL,     -- Row Description (e.g. Order Number)
   --     NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
   --     '[MediaID]', Convert(VARCHAR(50), Deleted.[MediaID]),  deleted.Rowversion           FROM deleted
   --       WHERE deleted.[MediaID] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[WorkStatus]', Convert(VARCHAR(50), Deleted.[WorkStatus]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[WorkStatus] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[CurrentLocation]', Convert(VARCHAR(50), Deleted.[CurrentLocation]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[CurrentLocation] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[BasePrice]', Convert(VARCHAR(50), Deleted.[BasePrice]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[BasePrice] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[PriceUnit]', Convert(VARCHAR(50), Deleted.[PriceUnit]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[PriceUnit] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[ShippingMethod]', Convert(VARCHAR(50), Deleted.[ShippingMethod]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[ShippingMethod] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[EstShipDate]', Convert(VARCHAR(50), Deleted.[EstShipDate]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[EstShipDate] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Created]', Convert(VARCHAR(50), Deleted.[Created]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Created] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Modified]', Convert(VARCHAR(50), Deleted.[Modified]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Modified] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Order', 'd', deleted.[OrderID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[RowVersion]', Convert(VARCHAR(50), Deleted.[RowVersion]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[RowVersion] is not null

 End Try 
 Begin Catch 
   Raiserror('error in [dbo].[Order_audit_delete trigger', 16, 1 ) with log
 End Catch

 -- Update [Order_Audit_Update] trigger to remove mediaid
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[Order_Audit_Update] ON [dbo].[Order]
 AFTER Update
 NOT FOR REPLICATION AS
 SET NoCount On 

DECLARE @AuditTime DATETIME
SET @AuditTime = GetDate()

 Begin Try 
 IF UPDATE([OrderID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[OrderID]', Convert(VARCHAR(50), Deleted.[OrderID]),  Convert(VARCHAR(50), Inserted.[OrderID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[OrderID],'') <> isnull(Deleted.[OrderID],'')

 IF UPDATE([CustomerID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[CustomerID]', Convert(VARCHAR(50), Deleted.[CustomerID]),  Convert(VARCHAR(50), Inserted.[CustomerID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[CustomerID],'') <> isnull(Deleted.[CustomerID],'')

 IF UPDATE([OrderDate])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[OrderDate]', Convert(VARCHAR(50), Deleted.[OrderDate]),  Convert(VARCHAR(50), Inserted.[OrderDate]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[OrderDate],'') <> isnull(Deleted.[OrderDate],'')

 IF UPDATE([RequiredDate])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[RequiredDate]', Convert(VARCHAR(50), Deleted.[RequiredDate]),  Convert(VARCHAR(50), Inserted.[RequiredDate]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[RequiredDate],'') <> isnull(Deleted.[RequiredDate],'')

 IF UPDATE([Status])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Status]', Convert(VARCHAR(50), Deleted.[Status]),  Convert(VARCHAR(50), Inserted.[Status]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[Status],'') <> isnull(Deleted.[Status],'')

 IF UPDATE([CompletedDate])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[CompletedDate]', Convert(VARCHAR(50), Deleted.[CompletedDate]),  Convert(VARCHAR(50), Inserted.[CompletedDate]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[CompletedDate],'') <> isnull(Deleted.[CompletedDate],'')

 IF UPDATE([Priority])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Priority]', Convert(VARCHAR(50), Deleted.[Priority]),  Convert(VARCHAR(50), Inserted.[Priority]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[Priority],'') <> isnull(Deleted.[Priority],'')

 IF UPDATE([PurchaseOrder])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[PurchaseOrder]', Convert(VARCHAR(50), Deleted.[PurchaseOrder]),  Convert(VARCHAR(50), Inserted.[PurchaseOrder]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[PurchaseOrder],'') <> isnull(Deleted.[PurchaseOrder],'')

 IF UPDATE([CreatedBy])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[CreatedBy]', Convert(VARCHAR(50), Deleted.[CreatedBy]),  Convert(VARCHAR(50), Inserted.[CreatedBy]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[CreatedBy],'') <> isnull(Deleted.[CreatedBy],'')

 IF UPDATE([Invoice])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Invoice]', Convert(VARCHAR(50), Deleted.[Invoice]),  Convert(VARCHAR(50), Inserted.[Invoice]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[Invoice],'') <> isnull(Deleted.[Invoice],'')

 IF UPDATE([ContractReviewed])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[ContractReviewed]', Convert(VARCHAR(50), Deleted.[ContractReviewed]),  Convert(VARCHAR(50), Inserted.[ContractReviewed]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[ContractReviewed],'') <> isnull(Deleted.[ContractReviewed],'')

 IF UPDATE([PartID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[PartID]', Convert(VARCHAR(50), Deleted.[PartID]),  Convert(VARCHAR(50), Inserted.[PartID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[PartID],'') <> isnull(Deleted.[PartID],'')

 IF UPDATE([PartQuantity])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[PartQuantity]', Convert(VARCHAR(50), Deleted.[PartQuantity]),  Convert(VARCHAR(50), Inserted.[PartQuantity]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[PartQuantity],'') <> isnull(Deleted.[PartQuantity],'')

 --IF UPDATE([MediaID])
 --  INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
 --  SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
 --       NULL,     -- Row Description (e.g. Order Number)
 --       NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
 --       '[MediaID]', Convert(VARCHAR(50), Deleted.[MediaID]),  Convert(VARCHAR(50), Inserted.[MediaID]),
 --DELETED.Rowversion + 1
 --         FROM Inserted
 --            JOIN Deleted
 --              ON Inserted.[OrderID] = Deleted.[OrderID]
 --              AND isnull(Inserted.[MediaID],'') <> isnull(Deleted.[MediaID],'')

 IF UPDATE([WorkStatus])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[WorkStatus]', Convert(VARCHAR(50), Deleted.[WorkStatus]),  Convert(VARCHAR(50), Inserted.[WorkStatus]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[WorkStatus],'') <> isnull(Deleted.[WorkStatus],'')

 IF UPDATE([CurrentLocation])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[CurrentLocation]', Convert(VARCHAR(50), Deleted.[CurrentLocation]),  Convert(VARCHAR(50), Inserted.[CurrentLocation]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[CurrentLocation],'') <> isnull(Deleted.[CurrentLocation],'')

 IF UPDATE([BasePrice])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[BasePrice]', Convert(VARCHAR(50), Deleted.[BasePrice]),  Convert(VARCHAR(50), Inserted.[BasePrice]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[BasePrice],'') <> isnull(Deleted.[BasePrice],'')

 IF UPDATE([PriceUnit])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[PriceUnit]', Convert(VARCHAR(50), Deleted.[PriceUnit]),  Convert(VARCHAR(50), Inserted.[PriceUnit]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[PriceUnit],'') <> isnull(Deleted.[PriceUnit],'')

 --IF UPDATE([QuoteID])
 --  INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
 --  SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
 --       NULL,     -- Row Description (e.g. Order Number)
 --       NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
 --       '[QuoteID]', Convert(VARCHAR(50), Deleted.[QuoteID]),  Convert(VARCHAR(50), Inserted.[QuoteID]),
 --DELETED.Rowversion + 1
 --         FROM Inserted
 --            JOIN Deleted
 --              ON Inserted.[OrderID] = Deleted.[OrderID]
 --              AND isnull(Inserted.[QuoteID],'') <> isnull(Deleted.[QuoteID],'')

 IF UPDATE([ShippingMethod])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[ShippingMethod]', Convert(VARCHAR(50), Deleted.[ShippingMethod]),  Convert(VARCHAR(50), Inserted.[ShippingMethod]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[ShippingMethod],'') <> isnull(Deleted.[ShippingMethod],'')

 IF UPDATE([EstShipDate])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[EstShipDate]', Convert(VARCHAR(50), Deleted.[EstShipDate]),  Convert(VARCHAR(50), Inserted.[EstShipDate]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[EstShipDate],'') <> isnull(Deleted.[EstShipDate],'')

 End Try 
 Begin Catch 
   Raiserror('error in [dbo].[Order_audit_update] trigger', 16, 1 ) with log
 End Catch

 /********************************************************************************
*	[fnGetNextDept]
*		Add new function used by Get_OrderStatus
*
********************************************************************************/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnGetNextDept] 
(
	@orderID int
)
RETURNS nvarchar(50)
AS
BEGIN
	-- FIND THE NEXT DEPARTMENT FOR THE ORDER
	DECLARE @nextDept NVarChar(50)
	

	-- Find Next Dept that is started but not completed
	SET @nextDept = 
	(
		SELECT  TOP(1) Department FROM [OrderProcesses] 
		WHERE StartDate IS NULL AND EndDate IS NULL AND OrderID = @orderID
		ORDER BY StepOrder
	)

	-- IF NOT IN ORDER ANSWERS THEN GET FROM PART PROCESSES ITSELF
	IF (@nextDept IS NULL)
		BEGIN
			SET @nextDept = 
			(
				SELECT  TOP(1) Department FROM [OrderProcesses] 
				WHERE StartDate IS NULL AND OrderID = @orderID
				ORDER BY StepOrder
			)
		END
	 
	 IF (@nextDept IS NULL)
		SET @nextDept = 'None'
	Return @nextDept

END

GO

 /********************************************************************************
*	[[fnGetCurrentProcess]]
*		Add new function used by Get_OrderStatus
*
********************************************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnGetCurrentProcess] 
(
	@orderID int
)
RETURNS nvarchar(50)
AS
BEGIN
	-- FIND THE NEXT DEPARTMENT FOR THE ORDER
	DECLARE @currentProcessID int
	DECLARE @currentProcessName nvarchar(50)

	-- Find Next Dept that is started but not completed
	SET @currentProcessID = 
	(
		SELECT  TOP(1) ProcessID FROM [OrderProcesses] 
		WHERE StartDate IS NOT NULL AND EndDate IS NULL AND OrderID = @orderID
		ORDER BY StepOrder
	)

	
	IF (@currentProcessID IS NOT NULL)
		BEGIN
			SET @currentProcessName = 
			(
				SELECT Name FROM Process WHERE ProcessID = @currentProcessID
			)
		END
	 
	 
	Return @currentProcessName

END

GO

/********************************************************************************
*	[Get_OrderStatus]
*		Add new function used by Get_OrderStatus
*
********************************************************************************/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Get_OrderStatus] 
AS
BEGIN
	SET NOCOUNT ON;

	SELECT     [Order].OrderID AS WO, [Order].PurchaseOrder AS PO, Customer.Name AS Customer, Part.Name AS Part, [Order].EstShipDate, [Order].Priority, 
						  [Order].WorkStatus, [Order].CurrentLocation, dbo.fnGetNextDept([Order].OrderID) AS NextDept, dbo.fnGetCurrentProcess([Order].OrderID) AS CurrentProcess, [Order].IsRework
	FROM         [Order] LEFT OUTER JOIN
						  Customer ON [Order].CustomerID = Customer.CustomerID LEFT OUTER JOIN
						  Part ON [Order].PartID = Part.PartID
	WHERE     ([Order].Status = N'Open')
					 
END
GO




ALTER PROCEDURE [dbo].[Get_OrderProcessSummaryInfo] 
	@orderID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;

	SELECT     dbo.OrderProcesses.StepOrder, dbo.OrderProcesses.Department, dbo.OrderProcesses.StartDate, dbo.OrderProcesses.EndDate, 
                      dbo.Process.Name AS ProcessName, dbo.ProcessAlias.Name AS AliasName, dbo.CustomerProcessAlias.Name AS CustomerAliasName
	FROM         dbo.CustomerProcessAlias RIGHT OUTER JOIN
						  dbo.Process INNER JOIN
						  dbo.OrderProcesses INNER JOIN
						  dbo.[Order] ON dbo.OrderProcesses.OrderID = dbo.[Order].OrderID ON dbo.Process.ProcessID = dbo.OrderProcesses.ProcessID INNER JOIN
						  dbo.ProcessAlias ON dbo.OrderProcesses.ProcessAliasID = dbo.ProcessAlias.ProcessAliasID ON 
						  dbo.CustomerProcessAlias.CustomerID = dbo.[Order].CustomerID AND dbo.CustomerProcessAlias.ProcessAliasID = dbo.ProcessAlias.ProcessAliasID
						  WHERE OrderProcesses.OrderID = @orderID
	UNION
	SELECT 99, 'Shipping', [DateShipped], [DateShipped], [TrackingNumber], NULL, NULL 
	  FROM [dbo].[OrderShipment]  
		WHERE [OrderShipment].OrderID = @orderID
	ORDER BY OrderProcesses.StepOrder   
END
GO



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


	SELECT avgQty ProcessedPartQty, dept Department, avgDay 'Day' FROM cteAvgParts
END
GO


ALTER TABLE [dbo].[Site]
    ADD [IsActive] BIT CONSTRAINT [DF_Site_IsActive] DEFAULT ((1)) NOT NULL;
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_FieldsName]
    ON [dbo].[Fields]([Name] ASC);
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
CREATE PROCEDURE Delete_UnusedPartmarking 
AS
BEGIN
	SET NOCOUNT ON;

	declare @rows int

	set @rows = (select COUNT(*) FROM [PartMarking])
    
	DELETE FROM [PartMarking] 
	WHERE PartMarkingID IN 
	(SELECT m.PartMarkingID FROM PartMarking m
		WHERE 
		NOT	EXISTS (SELECT * FROM [Customer_PartMarking] pm WHERE m.PartMarkingID = pm.PartMarkingID))

	select @rows - (select COUNT(*) FROM [PartMarking])
END
GO



/********************************************************************************
*	[MANAGE_DefragIndexes]
*		Add new function used by Get_OrderStatus
*
********************************************************************************/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
-- =============================================
-- This stored procedure checks all indexes in the current
-- database and performs either offline or online defragmentation
-- according to the specified thresholds.
-- The stored procedure also updates statistics for indexes in which the last update
-- time is older than the specified threshold.
-- Parameters:
--	@onlineDefragThreshold specifies minimum percentage of fragmentation 
--	to perform online defragmentation (default 10%).
--	@offlineDefragThreshold specifies minimum percentage of fragmentation 
--	to perform offline defragmentation (default 30%).
--	@updateStatsThreshold specifies the number of days since the last statistics update
--	which should trigger updating statistics (default 7 days).
-- =============================================
CREATE PROCEDURE [dbo].[MANAGE_DefragIndexes] 
(
	@databaseName sysname = null,
	@onlineDefragThreshold float = 10.0,
	@offlineDefragThreshold float = 30.0,
	@updateStatsThreshold int = 7
)
	
AS
BEGIN
 
IF @databasename is null
BEGIN
	RETURN;
END
 
DECLARE @SQL nvarchar(max)
SET @SQL = 'USE '+ @databasename +'
 
	set nocount on
	DECLARE @objectid int
	DECLARE @indexid int
	DECLARE @frag float
	DECLARE @command varchar(8000)
	DECLARE @schemaname sysname
	DECLARE @objectname sysname
	DECLARE @indexname sysname
 
	declare @AllIndexes table (objectid int, indexid int, fragmentation float)
 
	declare @currentDdbId int
	select @currentDdbId = DB_ID()
	
	insert into @AllIndexes
	SELECT 
		object_id, index_id, avg_fragmentation_in_percent 
	FROM sys.dm_db_index_physical_stats (@currentDdbId, NULL, NULL , NULL, ''LIMITED'')
	WHERE index_id > 0
 
	DECLARE indexesToDefrag CURSOR FOR SELECT * FROM @AllIndexes
 
	OPEN indexesToDefrag;
 
	-- Loop through the partitions.
	FETCH NEXT
	   FROM indexesToDefrag
	   INTO @objectid, @indexid, @frag;
 
	WHILE @@FETCH_STATUS = 0
		BEGIN
 
		SELECT @schemaname = s.name
		FROM sys.objects AS o
		JOIN sys.schemas as s ON s.schema_id = o.schema_id
		WHERE o.object_id = @objectid
 
		SELECT @indexname = name 
		FROM sys.indexes
		WHERE  object_id = @objectid AND index_id = @indexid
 
		IF @frag > @onlineDefragThreshold
		BEGIN 
			IF @frag < @offlineDefragThreshold
				BEGIN;
					SELECT @command = ''ALTER INDEX '' + @indexname + '' ON '' + 
							@schemaname + ''.'' + object_name(@objectid) + 
							'' REORGANIZE''
					EXEC (@command)
				END
 
			IF @frag >= @offlineDefragThreshold
				BEGIN;
					SELECT @command = ''ALTER INDEX '' + 
							@indexname +'' ON '' + @schemaname + ''.'' + 
							object_name(@objectid) + '' REBUILD''
					EXEC (@command)
				END;
			PRINT ''Executed '' + @command
		END
 
		IF STATS_DATE(@objectid, @indexid) < DATEADD(dd, -@updateStatsThreshold, getdate())
		BEGIN
			SELECT @command = ''UPDATE STATISTICS '' + @schemaname + ''.'' + object_name(@objectid) + 
					'' '' + @indexname +'' WITH RESAMPLE''
			EXEC (@command)
 
			PRINT ''Executed '' + @command
		END
 
		FETCH NEXT FROM indexesToDefrag INTO @objectid, @indexid, @frag
 
	END
 
	CLOSE indexesToDefrag;
	DEALLOCATE indexesToDefrag;'
 
DECLARE @Params nvarchar(max)
SET @Params = N'
	@onlineDefragThreshold float,
	@offlineDefragThreshold float,
	@updateStatsThreshold int'
 
EXECUTE sp_executesql @SQL, 
		@Params,
		@onlineDefragThreshold=@onlineDefragThreshold,
		@offlineDefragThreshold=@offlineDefragThreshold,
		@updateStatsThreshold=@updateStatsThreshold;
END

/********************************************************************************
*	[MANAGE_ShrinkDatabase]
*
********************************************************************************/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[MANAGE_ShrinkDatabase]
	@DatabaseName sysname = null,
	@FreeSpace int = 0
AS
BEGIN
	SET NOCOUNT ON;

   EXEC ('DBCC SHRINKDATABASE('''+@DatabaseName+''', @FreeSpace)')
END
GO


/********************************************************************************
*	[ApplicationSettings]
*		Add PLCCOnnection setting
*
********************************************************************************/
INSERT INTO ApplicationSettings VALUES ('PLCConnection', 'Data Source=DPSData;Initial Catalog=PLCData;Persist Security Info=True;User ID=plc;Password=plc');



/********************************************************************************
*	[IX_OP_START_END]
*		Add New index to speed up search on order status
*
********************************************************************************/
/****** Object:  Index [IX_OP_START_END]    Script Date: 9/6/2012 8:39:39 PM ******/
CREATE NONCLUSTERED INDEX [IX_OP_START_END] ON [dbo].[OrderProcesses]
(
	[OrderID] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


/********************************************************************************
*	[fnGetCurrentProcess]
*		Update to get next process if no processes have started yet
*
********************************************************************************/
/****** Object:  UserDefinedFunction [dbo].[fnGetCurrentProcess]    Script Date: 9/6/2012 8:47:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[fnGetCurrentProcess] 
(
	@orderID int
)
RETURNS nvarchar(50)
AS
BEGIN
	-- FIND THE NEXT DEPARTMENT FOR THE ORDER
	DECLARE @currentProcessID int
	DECLARE @currentProcessName nvarchar(50) = 'NA'

	-- Find Next Dept that is started but not completed
	SET @currentProcessID = 
	(
		SELECT  TOP(1) ProcessID FROM [OrderProcesses] 
		WHERE StartDate IS NOT NULL AND EndDate IS NULL AND OrderID = @orderID
		ORDER BY StepOrder
	)
	
	-- get the name of the process
	IF (@currentProcessID IS NOT NULL)
		BEGIN
			SET @currentProcessName = 
			(
				SELECT Name FROM Process WHERE ProcessID = @currentProcessID
			)
		END
	 
	 
	Return @currentProcessName

END