/********************************************************************************
*	QuotePart_QuoteProcess
*		- Change primary key to allow multiple of same kind
********************************************************************************/

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO

--
-- Drop primary key "PK_QuotePart_QuoteProcess_2" on table "dbo.QuotePart_QuoteProcess"
--
ALTER TABLE dbo.QuotePart_QuoteProcess
  DROP CONSTRAINT PK_QuotePart_QuoteProcess_2
GO
IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO

--
-- Create column "QuotePartProcessID" on table "dbo.QuotePart_QuoteProcess"
--
ALTER TABLE dbo.QuotePart_QuoteProcess
  ADD QuotePartProcessID int IDENTITY
GO
IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO

--
-- Create primary key "PK_QuotePart_QuoteProcess" on table "dbo.QuotePart_QuoteProcess"
--
ALTER TABLE dbo.QuotePart_QuoteProcess
  ADD CONSTRAINT PK_QuotePart_QuoteProcess3 PRIMARY KEY (QuotePartProcessID)
GO
IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO

IF @@TRANCOUNT>0 COMMIT TRANSACTION
GO

SET NOEXEC OFF
GO


/********************************************************************************
*	Delete_UnusedMedia
*		- Update delete unused media to now include quotes
********************************************************************************/

ALTER PROCEDURE dbo.Delete_UnusedMedia
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
    MediaID IN (SELECT m.MediaID
                FROM
                  Media m
                WHERE
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
                                [QuotePart_Media] o
                              WHERE
                                m.MediaID = o.MediaID)
                  AND
                  NOT EXISTS (SELECT *
                              FROM
                                [Users] u
                              WHERE
                                m.MediaID = u.MediaID))

  SELECT @rows - (SELECT count(*)
                  FROM
                    Media)

END


/********************************************************************************
*	Order
*		- Update orders with a completed date set when they are in the status of Open
*		- This was due to rework copying the completed date into a new order
********************************************************************************/
UPDATE [Order] 
  SET CompletedDate = NULL
WHERE
  [Status] = 'Open'


/********************************************************************************
*	SecurityRole
*		Ensure roles are present.
*
********************************************************************************/
IF NOT EXISTS (SELECT * FROM [SecurityRole] WHERE [SecurityRoleID] = 'OrderProcess.Edit')
	BEGIN
		INSERT INTO  [SecurityRole] VALUES ('OrderProcess.Edit', 'Ability to edit order processes in order entry.', 'Sales');
	END
	GO
