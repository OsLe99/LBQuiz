using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LBQuiz.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add columns only if they don't already exist, populate existing rows, then make columns NOT NULL.
            // This avoids "column specified more than once" when the DB or another migration already created the column.

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE Name = N'Description' AND Object_ID = OBJECT_ID(N'dbo.Quiz')
)
BEGIN
    ALTER TABLE [dbo].[Quiz] ADD [Description] nvarchar(500) NULL;
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE Name = N'Name' AND Object_ID = OBJECT_ID(N'dbo.Quiz')
)
BEGIN
    ALTER TABLE [dbo].[Quiz] ADD [Name] nvarchar(100) NULL;
END
");

            // Ensure no NULL values exist before making NOT NULL
            migrationBuilder.Sql("UPDATE [dbo].[Quiz] SET [Description] = '' WHERE [Description] IS NULL;");
            migrationBuilder.Sql("UPDATE [dbo].[Quiz] SET [Name] = 'Untitled Quiz' WHERE [Name] IS NULL;");

            // Alter to NOT NULL if currently nullable (safe even if already NOT NULL)
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE Name = N'Description' AND Object_ID = OBJECT_ID(N'dbo.Quiz')
      AND is_nullable = 1
)
BEGIN
    ALTER TABLE [dbo].[Quiz] ALTER COLUMN [Description] nvarchar(500) NOT NULL;
END
");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE Name = N'Name' AND Object_ID = OBJECT_ID(N'dbo.Quiz')
      AND is_nullable = 1
)
BEGIN
    ALTER TABLE [dbo].[Quiz] ALTER COLUMN [Name] nvarchar(100) NOT NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop columns only if they exist
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE Name = N'Description' AND Object_ID = OBJECT_ID(N'dbo.Quiz')
)
BEGIN
    ALTER TABLE [dbo].[Quiz] DROP COLUMN [Description];
END
");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE Name = N'Name' AND Object_ID = OBJECT_ID(N'dbo.Quiz')
)
BEGIN
    ALTER TABLE [dbo].[Quiz] DROP COLUMN [Name];
END
");
        }
    }
}
