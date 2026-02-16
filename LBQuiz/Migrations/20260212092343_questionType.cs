using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LBQuiz.Migrations
{
    /// <inheritdoc />
    public partial class questionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuestionType",
                table: "QuestionJsonBlobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionType",
                table: "QuestionJsonBlobs");
        }
    }
}
