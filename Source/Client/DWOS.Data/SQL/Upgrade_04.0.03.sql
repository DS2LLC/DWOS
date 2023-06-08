
/********************************************************************************
*	[Fields]
*		Create Fields Table
*
********************************************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Fields](
	[FieldID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Alias] [nvarchar](50) NULL,
	[Category] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Fields] PRIMARY KEY CLUSTERED 
(
	[FieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Fields] ADD  CONSTRAINT [DF_Fields_Category]  DEFAULT (N'Order') FOR [Category]
GO

INSERT INTO [Fields] ([Name],[Alias],[Category]) VALUES ('Customer WO', NULL,	'Order');
INSERT INTO [Fields] ([Name],[Alias],[Category]) VALUES ('PO', NULL,	'Order');
INSERT INTO [Fields] ([Name],[Alias],[Category]) VALUES ('Part Rev.', NULL,	'Part');


/********************************************************************************
*	[Customer_Fields]
*		Create [Customer_Fields] Table
*
********************************************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Customer_Fields](
	[CustomerID] [int] NOT NULL,
	[FieldID] [int] NOT NULL,
	[Required] [bit] NOT NULL,
	[DefaultVaue] [nvarchar](50) NULL,
 CONSTRAINT [PK_Customer_Fields] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[FieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Customer_Fields] ADD  CONSTRAINT [DF_Customer_Fields_Required]  DEFAULT ((0)) FOR [Required]
GO

ALTER TABLE [dbo].[Customer_Fields]  WITH CHECK ADD  CONSTRAINT [FK_Customer_Fields_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customer] ([CustomerID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Customer_Fields] CHECK CONSTRAINT [FK_Customer_Fields_Customer]
GO

ALTER TABLE [dbo].[Customer_Fields]  WITH CHECK ADD  CONSTRAINT [FK_Customer_Fields_Fields] FOREIGN KEY([FieldID])
REFERENCES [dbo].[Fields] ([FieldID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Customer_Fields] CHECK CONSTRAINT [FK_Customer_Fields_Fields]
GO



/********************************************************************************
*	Customer
*		Update using Fields
*
********************************************************************************/
	-- Add PO Field by Customer
	INSERT INTO [Customer_Fields] ([CustomerID],[FieldID],[Required],[DefaultVaue])
	SELECT c.CustomerID, 2, c.[RequiresPO], c.[DefaultPO] FROM Customer c 
	WHERE (SELECT COUNT(*) FROM [Customer_Fields] cf WHERE cf.[CustomerID] = c.CustomerID AND cf.FieldID = 2) = 0

	-- Add CustomerWO Field by Customer
	INSERT INTO [Customer_Fields] ([CustomerID],[FieldID],[Required],[DefaultVaue])
	SELECT c.CustomerID, 1, c.[RequiresPO], c.[DefaultPO] FROM Customer c 
	WHERE (SELECT COUNT(*) FROM [Customer_Fields] cf WHERE cf.[CustomerID] = c.CustomerID AND cf.FieldID = 1) = 0

	-- Drop Columns no longer used
	ALTER TABLE [Customer] DROP COLUMN [DefaultPO]
	
	ALTER TABLE [Customer] DROP CONSTRAINT [DF_Customer_RequiresCustomerWO]
	ALTER TABLE [Customer] DROP COLUMN [RequiresCustomerWO]

	ALTER TABLE [Customer] DROP CONSTRAINT [DF_Customer_RequiresPO]
	ALTER TABLE [Customer] DROP COLUMN [RequiresPO]


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
	ALTER TABLE Customer ADD Active bit NOT NULL CONSTRAINT DF_Customer_Active DEFAULT 1
	GO
	ALTER TABLE Customer DROP COLUMN DNNID
	GO
	ALTER TABLE Customer SET (LOCK_ESCALATION = TABLE)
	GO
	COMMIT

/********************************************************************************
*	[SecurityRole]
*		Add new permission
*
********************************************************************************/

	IF NOT EXISTS (SELECT * FROM [SecurityRole] WHERE [SecurityRoleID] = 'OrderFeesManager')
		BEGIN
			INSERT INTO  [SecurityRole] VALUES ('OrderFeesManager', 'Ability to manage order fees.', 'Managers');
		END
	GO


/********************************************************************************
*	[OrderFeeType]
*		Update OrderFeeType - OrderFees to CASCADE DELETE
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
	ALTER TABLE dbo.OrderFees
		DROP CONSTRAINT FK_OrderFees_OrderFeeType
	GO
	ALTER TABLE dbo.OrderFeeType SET (LOCK_ESCALATION = TABLE)
	GO
	COMMIT
	BEGIN TRANSACTION
	GO
	ALTER TABLE dbo.OrderFees ADD CONSTRAINT
		FK_OrderFees_OrderFeeType FOREIGN KEY
		(
		OrderFeeTypeID
		) REFERENCES dbo.OrderFeeType
		(
		OrderFeeTypeID
		) ON UPDATE  CASCADE 
		 ON DELETE  CASCADE 
	
	GO
	ALTER TABLE dbo.OrderFees SET (LOCK_ESCALATION = TABLE)
	GO
	COMMIT



/********************************************************************************
*	[d_Department]
*		Add system name
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
	ALTER TABLE d_Department ADD SystemName nvarchar(50) NULL
	ALTER TABLE d_Department ADD Active bit NOT NULL CONSTRAINT DF_d_Department_Active DEFAULT 1

	GO
	ALTER TABLE d_Department SET (LOCK_ESCALATION = TABLE)
	GO
	COMMIT

/********************************************************************************
*	[SecurityRole]
*		Add new permission
*
********************************************************************************/

	IF NOT EXISTS (SELECT * FROM [SecurityRole] WHERE [SecurityRoleID] = 'DepartmentManager')
		BEGIN
			INSERT INTO  [SecurityRole] VALUES ('DepartmentManager', 'Ability to manage departments.', 'Managers');
		END
	GO


/********************************************************************************
*	Get_ShippingCarrierUsageCount
*		Add SP to get usage count of shipping carrier
*
********************************************************************************/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE Get_ShippingCarrierUsageCount 
	@carrierID nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
	(SELECT COUNT(*) 'Count' FROM CustomerShipping WHERE CUstomerShipping.CarrierID = @carrierID)
	+
	(SELECT COUNT(*) 'Count' FROM ShipmentPackage WHERE ShipmentPackage.ShippingCarrierID = @carrierID) 

END
GO

-- DROP then re-add the auto-audit
EXEC	[dbo].[pAutoAuditDrop] @SchemaName = N'dbo', @TableName = N'Customer'
EXEC	[dbo].[pAutoAudit] @SchemaName = N'dbo', @TableName = N'Customer'
	
EXEC	[dbo].[pAutoAuditDrop] @SchemaName = N'dbo', @TableName = N'COC'
EXEC	[dbo].[pAutoAuditDrop] @SchemaName = N'dbo', @TableName = N'CustomerCommunication'
EXEC	[dbo].[pAutoAuditDrop] @SchemaName = N'dbo', @TableName = N'd_CustomerStatus'
EXEC	[dbo].[pAutoAuditDrop] @SchemaName = N'dbo', @TableName = N'd_PriceUnit'
EXEC	[dbo].[pAutoAuditDrop] @SchemaName = N'dbo', @TableName = N'Lists'
EXEC	[dbo].[pAutoAuditDrop] @SchemaName = N'dbo', @TableName = N'ListValues'
EXEC	[dbo].[pAutoAuditDrop] @SchemaName = N'dbo', @TableName = N'OrderFeeType'
EXEC	[dbo].[pAutoAuditDrop] @SchemaName = N'dbo', @TableName = N'Part_Media'
EXEC	[dbo].[pAutoAuditDrop] @SchemaName = N'dbo', @TableName = N'QuoteFees'


IF NOT EXISTS (SELECT * FROM [SecurityRole] WHERE [SecurityRoleID] = 'ShippingCarrierManager')
	BEGIN
		INSERT INTO  [SecurityRole] VALUES ('ShippingCarrierManager', 'Ability to manage shipping carriers.', 'Managers');
	END
GO

/********************************************************************************
*	UserLoginHistory
*		Add table to track user login history
*
********************************************************************************/

/****** Object:  Table [dbo].[UserLoginHistory]    Script Date: 6/22/2012 4:02:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserLoginHistory](
	[HistoryID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[LoginTimeStamp] [smalldatetime] NOT NULL,
	[MachineName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_UserLoginHistory] PRIMARY KEY CLUSTERED 
(
	[HistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[UserLoginHistory] ADD  CONSTRAINT [DF_UserLoginHistory_LoginTimeStamp]  DEFAULT (getdate()) FOR [LoginTimeStamp]
GO

/********************************************************************************
*	Views
*		Drop unused views not needed from AutoAudit any more
*
********************************************************************************/

/****** Object:  View [dbo].[vFormSecurity_Deleted]    Script Date: 06/22/2012 16:21:54 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vFormSecurity_Deleted]'))
DROP VIEW [dbo].[vFormSecurity_Deleted]
GO

/****** Object:  View [dbo].[vCustomerCommunication_Deleted]    Script Date: 06/22/2012 16:22:26 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vCustomerCommunication_Deleted]'))
DROP VIEW [dbo].[vCustomerCommunication_Deleted]
GO

/****** Object:  View [dbo].[vd_CustomerStatus_Deleted]    Script Date: 06/22/2012 16:22:48 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vd_CustomerStatus_Deleted]'))
DROP VIEW [dbo].[vd_CustomerStatus_Deleted]
GO

/****** Object:  View [dbo].[vd_PriceUnit_Deleted]    Script Date: 06/22/2012 16:23:03 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vd_PriceUnit_Deleted]'))
DROP VIEW [dbo].[vd_PriceUnit_Deleted]
GO

/****** Object:  View [dbo].[vLists_Deleted]    Script Date: 06/22/2012 16:23:23 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vLists_Deleted]'))
DROP VIEW [dbo].[vLists_Deleted]
GO

/****** Object:  View [dbo].[vListValues_Deleted]    Script Date: 06/22/2012 16:23:35 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vListValues_Deleted]'))
DROP VIEW [dbo].[vListValues_Deleted]
GO

/****** Object:  View [dbo].[vOrderFeeType_Deleted]    Script Date: 06/22/2012 16:27:52 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vOrderFeeType_Deleted]'))
DROP VIEW [dbo].[vOrderFeeType_Deleted]
GO

/****** Object:  View [dbo].[vPart_Media_Deleted]    Script Date: 06/22/2012 16:28:14 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vPart_Media_Deleted]'))
DROP VIEW [dbo].[vPart_Media_Deleted]
GO

/****** Object:  View [dbo].[vQuoteFees_Deleted]    Script Date: 06/22/2012 16:28:31 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vQuoteFees_Deleted]'))
DROP VIEW [dbo].[vQuoteFees_Deleted]
GO

/****** Object:  View [dbo].[vUserRoles_Deleted]    Script Date: 06/22/2012 16:30:23 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vUserRoles_Deleted]'))
DROP VIEW [dbo].[vUserRoles_Deleted]
GO

/****** Object:  View [dbo].[vMedia_Deleted]    Script Date: 06/22/2012 16:31:14 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vMedia_Deleted]'))
DROP VIEW [dbo].[vMedia_Deleted]
GO

/****** Object:  View [dbo].[vd_Priority_Deleted]    Script Date: 06/22/2012 16:31:34 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vd_Priority_Deleted]'))
DROP VIEW [dbo].[vd_Priority_Deleted]
GO

/****** Object:  View [dbo].[vsysdiagrams_Deleted]    Script Date: 6/22/2012 4:33:28 PM ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[[vsysdiagrams_Deleted]]'))
DROP VIEW [dbo].[vsysdiagrams_Deleted]
GO

/****** Object:  View [dbo].[vUserEventLog_Deleted]    Script Date: 6/22/2012 4:33:42 PM ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[[vUserEventLog_Deleted]]'))
DROP VIEW [dbo].[vUserEventLog_Deleted]
GO
