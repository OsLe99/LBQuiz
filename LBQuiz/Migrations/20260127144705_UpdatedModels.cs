using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LBQuiz.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllAnswers",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "CorrectAnswers",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "QuestionString",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "TimeLeft",
                table: "Question");

            migrationBuilder.AlterColumn<string>(
                name: "CorrectAnswer",
                table: "Question",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionText",
                table: "Question",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TimeLimitSeconds",
                table: "Question",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MultipleChoiceAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsCorrectAnswer = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleChoiceAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MultipleChoiceAnswer_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceAnswer_QuestionId",
                table: "MultipleChoiceAnswer",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MultipleChoiceAnswer");

            migrationBuilder.DropColumn(
                name: "QuestionText",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "TimeLimitSeconds",
                table: "Question");

            migrationBuilder.AlterColumn<string>(
                name: "CorrectAnswer",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

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

            migrationBuilder.AddColumn<string>(
                name: "QuestionString",
                table: "Question",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "TimeLeft",
                table: "Question",
                type: "float",
                nullable: true);
        }
    }
}
