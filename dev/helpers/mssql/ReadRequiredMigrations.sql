CREATE OR ALTER PROCEDURE ReadRequiredMigrations
    @migrationsJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #InputMigrations (Filename NVARCHAR(MAX))

    -- Insert JSON data into the temporary table
    INSERT INTO #InputMigrations (Filename)
    SELECT [value] FROM OPENJSON(@migrationsJson);

    -- Select migrations that do not appear in the [dbo].[migrations] table
    SELECT IM.[Filename]
    FROM [#InputMigrations] IM
    LEFT JOIN [dbo].[migrations] M ON IM.[Filename] = M.[Filename]
    WHERE M.[Filename] IS NULL
    ORDER BY [Filename] ASC

    DROP TABLE #InputMigrations
END
