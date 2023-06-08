-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '16.1.1'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Department Account Code
ALTER TABLE dbo.d_Department ADD
    AccountingCode nvarchar(50) NULL
GO

-- Fix potential overflow issue
ALTER FUNCTION [dbo].[fnGetCurrentProcessDate] 
(
    @orderID int
)
RETURNS Date
AS
BEGIN
    -- FIND THE NEXT DEPARTMENT FOR THE ORDER
    DECLARE @dueDate Date
    
    -- Find Next process that is not completed
    SET @dueDate = 
    (
        SELECT  TOP(1) EstEndDate FROM [OrderProcesses] 
        WHERE EndDate IS NULL AND OrderID = @orderID
        ORDER BY StepOrder
    )
    
    -- If no more processes left then return last processed date
    IF (@dueDate IS NULL)
        BEGIN
            SET @dueDate = 
                (
                    SELECT  TOP(1) EstEndDate FROM [OrderProcesses] 
                    WHERE EndDate IS NOT NULL AND OrderID = @orderID
                    ORDER BY StepOrder DESC
                )

            -- if date was set then subtract 100 years to let client know this is the last processed end date
            IF (@dueDate IS NOT NULL AND DATEPART(year, @dueDate) > 100)
                BEGIN
                    SET @dueDate = DATEADD(year, -100, @dueDate) 
                END
        END
            
    Return @dueDate

END

