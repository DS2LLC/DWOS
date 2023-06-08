--
-- Update Database Version
--
DECLARE @currentVersion nvarchar(50) = '20.1.1'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--
-- FailedLoginHistory
--
CREATE TABLE dbo.FailedLoginHistory
    (
    HistoryID int NOT NULL IDENTITY (1, 1),
    TimeStamp datetime2(7) NOT NULL,
    MachineName nvarchar(50) NOT NULL
    ) ON [PRIMARY];
GO

ALTER TABLE dbo.FailedLoginHistory ADD CONSTRAINT
    DF_FailedLoginHistory_TimeStamp DEFAULT GETDATE() FOR TimeStamp;
GO

ALTER TABLE dbo.FailedLoginHistory ADD CONSTRAINT
    PK_FailedLoginHistory PRIMARY KEY CLUSTERED
    (
    HistoryID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
GO

--
-- DeliveryPerformanceReport permission.
--
INSERT INTO SecurityRole (SecurityRoleID, Description, SecurityRoleCategoryID)
VALUES
('DeliveryPerformanceReport', 'Ability to run the Delivery Performance Report', 'Reports');

GO

--
-- Show rework-related columns on WIP screen.
--

GO

-- Determines if a Work Order is currently in rework.
CREATE FUNCTION dbo.fnGetIsInRework
(
    @orderID int
)
RETURNS bit
AS
BEGIN
    DECLARE @orderReworkType int = 4;
    DECLARE @processReworkType int = 2;
    DECLARE @hasRework bit = 0;
    DECLARE @currentOrderProcessesID int;
    DECLARE @inReworkProcess bit = 0;

    IF EXISTS (SELECT 1 FROM [Order] WHERE OrderID = @orderID AND OrderType = @orderReworkType)
    RETURN 1;

    IF EXISTS(SELECT 1 FROM InternalRework WHERE (OriginalOrderID = @orderID AND ReworkType = 'Full') OR (ReworkOrderID = @OrderID AND ReworkType IN ('Split', 'SplitHold')))
    SET @hasRework = 1;

        -- Find Next Dept that is started but not completed
    SET @currentOrderProcessesID = 
    (
        SELECT  TOP(1) OrderProcessesID FROM [OrderProcesses] 
        WHERE StartDate IS NOT NULL AND EndDate IS NULL AND OrderID = @orderID
        ORDER BY StepOrder
    )

    -- If didn't find the process started then show next process not started
    IF (@currentOrderProcessesID IS NULL)
    SET @currentOrderProcessesID = 
        (
            SELECT  TOP(1) OrderProcessesID FROM [OrderProcesses] 
            WHERE StartDate IS NULL AND EndDate IS NULL AND OrderID = @orderID
            ORDER BY StepOrder
        )

    IF EXISTS(SELECT 1 FROM OrderProcesses WHERE OrderProcessesID = @currentOrderProcessesID AND OrderProcessType = 2)
        SET @inReworkProcess = 1;

    RETURN @hasRework & @inReworkProcess;
END
GO

--
-- Multiple media items per Order Approval
--

CREATE TABLE dbo.OrderApprovalMedia
    (
    OrderApprovalMediaID int NOT NULL IDENTITY (1, 1),
    OrderApprovalID int NOT NULL,
    MediaID int NOT NULL,
    IsPrimary bit NOT NULL
    ) ON [PRIMARY];

GO

ALTER TABLE dbo.OrderApprovalMedia ADD CONSTRAINT
    DF_OrderApprovalMedia_IsPrimary DEFAULT 0 FOR IsPrimary;

GO

ALTER TABLE dbo.OrderApprovalMedia ADD CONSTRAINT
    PK_OrderApprovalMedia PRIMARY KEY CLUSTERED 
    (
    OrderApprovalMediaID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

GO

ALTER TABLE dbo.OrderApprovalMedia ADD CONSTRAINT
    FK_OrderApprovalMedia_OrderApproval FOREIGN KEY
    (
    OrderApprovalID
    ) REFERENCES dbo.OrderApproval
    (
    OrderApprovalID
    ) ON UPDATE NO ACTION
     ON DELETE NO ACTION;

GO

ALTER TABLE dbo.OrderApprovalMedia ADD CONSTRAINT
    FK_OrderApprovalMedia_Media FOREIGN KEY
    (
    MediaID
    ) REFERENCES dbo.Media
    (
    MediaID
    ) ON UPDATE NO ACTION
     ON DELETE NO ACTION;
GO

-- Migrate all existing Media links to use the new table

INSERT INTO dbo.OrderApprovalMedia(OrderApprovalID, MediaID, IsPrimary)
SELECT OrderApprovalID, MediaID, 1
FROM OrderApproval
WHERE MediaID IS NOT NULL;

GO

-- Remove original MediaID column from OrderApproval table
ALTER TABLE dbo.OrderApproval
DROP CONSTRAINT FK_OrderApproval_Media;

GO

ALTER TABLE dbo.OrderApproval
DROP COLUMN MediaID;

GO

-- Stored procedure for deleting unused media (removed OrderApproval.MediaID,
-- added OrderApprovalMedia.MediaID)
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
                                m.MediaID = bol.MediaID)
                 AND
                 NOT EXISTS (SELECT *
                             FROM
                               [OrderApprovalMedia] orderApprovalMedia
                             WHERE
                               m.MediaID = orderApprovalMedia.MediaID))

  SELECT @rows - (SELECT count(*)
                  FROM
                    Media)
END

GO
