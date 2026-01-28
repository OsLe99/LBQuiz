using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LBQuiz.Migrations
{
    /// <inheritdoc />
    public partial class FixedDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Question",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);

            migrationBuilder.AddColumn<string>(
                name: "AllAnswers",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectAnswers",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CorrectValue",
                table: "Question",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxValue",
                table: "Question",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinValue",
                table: "Question",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllAnswers",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "CorrectAnswers",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "CorrectValue",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "MaxValue",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "MinValue",
                table: "Question");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Question",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(21)",
                oldMaxLength: 21);
        }
    }
}
