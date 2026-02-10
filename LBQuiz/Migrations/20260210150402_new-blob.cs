using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LBQuiz.Migrations
{
    /// <inheritdoc />
    public partial class newblob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuestionText",
                table: "QuestionJsonBlobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionText",
                table: "QuestionJsonBlobs");
        }
    }
}
