using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LBQuiz.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuestionJsonBlobModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MultipleChoiceAnswer");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "QuestionJsonBlobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuestionMultipleId",
                table: "Question",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MultipleOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectFalse = table.Column<bool>(type: "bit", nullable: false),
                    ColorString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MultipleChoiceAnswerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MultipleOptions_Question_MultipleChoiceAnswerId",
                        column: x => x.MultipleChoiceAnswerId,
                        principalTable: "Question",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionMultipleId",
                table: "Question",
                column: "QuestionMultipleId");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleOptions_MultipleChoiceAnswerId",
                table: "MultipleOptions",
                column: "MultipleChoiceAnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Question_QuestionMultipleId",
                table: "Question",
                column: "QuestionMultipleId",
                principalTable: "Question",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_Question_QuestionMultipleId",
                table: "Question");

            migrationBuilder.DropTable(
                name: "MultipleOptions");

            migrationBuilder.DropIndex(
                name: "IX_Question_QuestionMultipleId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "QuestionJsonBlobs");

            migrationBuilder.DropColumn(
                name: "QuestionMultipleId",
                table: "Question");

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
    }
}
