-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '17.2.0'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--
-- Processing Lines
--
CREATE TABLE dbo.ProcessingLine
    (
    ProcessingLineID int NOT NULL IDENTITY(1,1),
    Name nvarchar(50) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.ProcessingLine ADD CONSTRAINT
    PK_ProcessingLine PRIMARY KEY CLUSTERED 
    (
    ProcessingLineID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


GO

ALTER TABLE dbo.[Order] ADD
    CurrentLine int NULL
GO
ALTER TABLE dbo.[Order] ADD CONSTRAINT
    FK_Order_ProcessingLine FOREIGN KEY
    (
    CurrentLine
    ) REFERENCES dbo.ProcessingLine
    (
    ProcessingLineID
    ) ON UPDATE  NO ACTION 
     ON DELETE  SET NULL 
    
GO

ALTER TABLE dbo.Batch ADD
    CurrentLine int NULL
GO
ALTER TABLE dbo.Batch ADD CONSTRAINT
    FK_Batch_ProcessingLine FOREIGN KEY
    (
    CurrentLine
    ) REFERENCES dbo.ProcessingLine
    (
    ProcessingLineID
    ) ON UPDATE  NO ACTION 
     ON DELETE  SET NULL 
    
GO

-- Update Get_BatchStatus to include current line
ALTER PROCEDURE [dbo].[Get_BatchStatus] 
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT        Batch.BatchID, Batch.OpenDate, Batch.Fixture, Batch.WorkStatus, Batch.CurrentLocation, dbo.fnGetNextDeptBatch(Batch.BatchID) AS NextDept, 
                         dbo.fnGetCurrentProcessBatch(Batch.BatchID) AS CurrentProcess, SUM(BatchOrder.PartQuantity) AS PartCount, COUNT(BatchOrder.BatchID) AS OrderCount, 
                         SUM(Part.SurfaceArea * BatchOrder.PartQuantity) AS TotalSurfaceArea, SUM(ISNULL([Order].Weight, Part.Weight * BatchOrder.PartQuantity)) AS TotalWeight,
              dbo.fnGetActiveTimerBatchCount(Batch.BatchID) AS ActiveTimerCount, Batch.CurrentLine
FROM            [Order] INNER JOIN
                         BatchOrder ON [Order].OrderID = BatchOrder.OrderID INNER JOIN
                         Part ON [Order].PartID = Part.PartID RIGHT OUTER JOIN
                         Batch ON BatchOrder.BatchID = Batch.BatchID
WHERE        (Batch.Active = 1)
GROUP BY Batch.BatchID, Batch.OpenDate, Batch.Fixture, Batch.WorkStatus, Batch.CurrentLocation, dbo.fnGetNextDeptBatch(Batch.BatchID), 
                         dbo.fnGetCurrentProcessBatch(Batch.BatchID), Batch.CurrentLine

END
GO

-- ProcessingLineID column for OrderProcesses
ALTER TABLE dbo.OrderProcesses ADD
    ProcessingLineID int NULL
GO
ALTER TABLE dbo.OrderProcesses ADD CONSTRAINT
    FK_OrderProcesses_ProcessingLine FOREIGN KEY
    (
    ProcessingLineID
    ) REFERENCES dbo.ProcessingLine
    (
    ProcessingLineID
    ) ON UPDATE  NO ACTION 
     ON DELETE  NO ACTION 
    
GO