

/********************************************************************************
*	[SecurityRole]
*		Add new permission
*
*		Customers.PartMark	Ability to edit part mark templates in customer manager.	Managers
*		Customers.Shipping	Ability to edit shipping templates in customer manager.	Managers
********************************************************************************/

	IF NOT EXISTS (SELECT * FROM [SecurityRole] WHERE [SecurityRoleID] = 'Customers.Shipping')
		BEGIN
			INSERT INTO  [SecurityRole] VALUES ('Customers.Shipping', 'Ability to edit shipping templates in customer manager.', 'Managers');
		END
	GO
	
	IF NOT EXISTS (SELECT * FROM [SecurityRole] WHERE [SecurityRoleID] = 'Customers.PartMark')
		BEGIN
			INSERT INTO  [SecurityRole] VALUES ('Customers.PartMark', 'Ability to edit part mark templates in customer manager.', 'Managers');
		END
	GO


/********************************************************************************
*	[Customer]
*		Update FK_CustomField_Customer relation to cascade delete
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
ALTER TABLE dbo.CustomField
	DROP CONSTRAINT FK_CustomField_Customer
GO
ALTER TABLE dbo.Customer SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.CustomField ADD CONSTRAINT
	FK_CustomField_Customer FOREIGN KEY
	(
	CustomerID
	) REFERENCES dbo.Customer
	(
	CustomerID
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.CustomField SET (LOCK_ESCALATION = TABLE)
GO
COMMIT


/********************************************************************************
*	Drop unused Functions
*
********************************************************************************/

DROP FUNCTION ApplicationSettings_RowHistory
DROP FUNCTION Audits_RowHistory
DROP FUNCTION COC_RowHistory
DROP FUNCTION CustomerCommunication_RowHistory
DROP FUNCTION d_Contact_RowHistory
DROP FUNCTION d_CustomerStatus_RowHistory
DROP FUNCTION d_InputType_RowHistory
DROP FUNCTION d_InspectionType_RowHistory
DROP FUNCTION d_OrderStatus_RowHistory
DROP FUNCTION d_PriceUnit_RowHistory
DROP FUNCTION d_Priority_RowHistory
DROP FUNCTION d_Roles_RowHistory
DROP FUNCTION d_ShippingCarrier_RowHistory
DROP FUNCTION d_WorkStatus_RowHistory
DROP FUNCTION FormSecurity_RowHistory
DROP FUNCTION Lists_RowHistory
DROP FUNCTION ListValues_RowHistory
DROP FUNCTION OrderFeeType_RowHistory
DROP FUNCTION Part_Media_RowHistory
DROP FUNCTION Process_RowHistory
DROP FUNCTION ProcessFolders_RowHistory
DROP FUNCTION QuoteFees_RowHistory
DROP FUNCTION sysdiagrams_RowHistory
DROP FUNCTION UserRoles_RowHistory
